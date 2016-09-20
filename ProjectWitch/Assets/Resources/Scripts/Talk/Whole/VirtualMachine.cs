//===================================
//author	:shotta
//summary	:仮想マシン
//===================================

using UnityEngine;
using System.Collections.Generic;
using System; //Exception
using System.Globalization;

using Scenario;
using Scenario.Pattern;
using Scenario.Compiler;
using Scenario.Command;
using Scenario.WorkSpace;

namespace Scenario.Command
{
	//仮想マシンコマンドの抽象クラス
	abstract public class CommandFormat
	{
		//コマンドを実行(エラーがあればfalseが返る)
		abstract public bool Run(VirtualMachine vm);
	}
	//引数セットコマンド
	public class SetArgumentCommand : CommandFormat
	{
		public object Value{
			get{ return mValue; }
		}
		private object mValue;
		public SetArgumentCommand(object value)
		{
			mValue = value;
		}
		public override bool Run(VirtualMachine vm)
		{
			vm.Push (mValue);
			return true;
		}
	}
	//命令実行コマンド
	//	VirtualMachine内にある、指定のCommandDelegaterを実行。
	public class RunOrderCommand : CommandFormat
	{
		private string mOrderCode;
		public string orderCode{
			get{ return mOrderCode; }
		}
		public RunOrderCommand(string orderCode)
		{
			mOrderCode = orderCode;
		}
		public override bool Run(VirtualMachine vm)
		{
			return vm.RunOrder(mOrderCode);
		}
	}
}

namespace Scenario.WorkSpace
{
	//!IMPORTANT!
	//拡張コマンド:操作の委譲をするやつ
	//	これに委譲する操作を格納して、VirtualMachineに登録する。
	//	登録はVirtualMachineのAddCommandDelegaterメソッドにより行う。
	public class CommandDelegater 
	{
		public delegate string RunOrderDelegate(object[] arguments);
		private RunOrderDelegate mDelegate;
		private bool mIsReturn;
		private int mCount;
		private CommandDelegater(){}

		//isReturn	:返り値
		//count		:命令実行に必要な引数の数
		//method	:委譲される命令
		//		引数		:引数の配列(返り値が存在する場合、この配列の一番最後が配列を返すための領域になる)
		//		返り値	:エラーメッセージ
		public CommandDelegater(bool isReturn, int count, RunOrderDelegate method)
		{
			mIsReturn = isReturn;
			mCount = count;
			mDelegate = method;
		}

		//コマンドを実行
		public string Run(VirtualMachine vm)
		{
			string error = null;
			//引数を作成
			object[] arguments;
			int count = mCount;
			if (mIsReturn) {
				arguments = new object[count + 1];
				arguments [count] = null;
			} else {
				arguments = new object[count];
			}
			for (int i = count; i > 0; i--)
			{
				error = vm.Pop (out arguments [i - 1]);
				if (error != null)
					return error;
			}
			//実行部
			error = mDelegate (arguments);
			//エラー処理
			if (error != null)
				return error;
			//返り値を処理
			if (mIsReturn)
				vm.Push (arguments[count]);
			
			return null;
		}
	}

	public delegate void NotifyMethod();
	//仮想マシン
	public class VirtualMachine
	{
		//マシンステート(true:稼働	false:停止)
		private bool mState;
		//現在のスクリプト上での位置
		private int mLine = 0;
		private int mIndex = 0;

		//ラベルを管理
		private Dictionary<string, int> mLabelIndexDic;
		//コマンドを管理
		private CommandFormat[] mCommandArray;
		private int mCommandIndex;
		//一時スタック
		private Stack<object> mValueStack = new Stack<object> ();
		//委譲コマンド辞典
		Dictionary<string, CommandDelegater> mCommandDelegaterDic = new Dictionary<string, CommandDelegater>();

		//オブザーバー
		private Dictionary<string, List<NotifyMethod>> mNotificationDic = new Dictionary<string, List<NotifyMethod>> ();

		//ローカルメモリ
		private const int mLocalMemCount = 256;
		private object[] mLocalMemory = new object[mLocalMemCount];

		//仮想マシンを生成
		public static VirtualMachine Create(CommandFormat[] commandArray)
		{
			CompilerLog.Log ("仮想マシンの作成");
			bool isValid = true;

			//ラベルの取得
			Dictionary<string, int> labelIndexDic = new Dictionary<string, int> ();
			int line = 0;
			int index = 0;
			for (int i = 0; i < commandArray.Length; i++)
			{
				CommandFormat command = commandArray [i];
				if (command is RunOrderCommand) {
					RunOrderCommand roCommand = command as RunOrderCommand;
					if (roCommand.orderCode == "referFrom")
					{
						line	= (int)(commandArray [i - 2] as SetArgumentCommand).Value;
						index	= (int)(commandArray [i - 1] as SetArgumentCommand).Value;
					}
					if (roCommand.orderCode == "label") {
						string label = (commandArray[i-1] as SetArgumentCommand).Value as string;
						if (!labelIndexDic.ContainsKey (label))
							labelIndexDic.Add (label, i);
						else
						{
							CompilerLog.Log (line, index, "ラベル名の重複があります(" + label + ")");
							isValid = false;
						}
					}
				}
			}

			if (isValid) 
			{
				VirtualMachine vm = new VirtualMachine ();
				//コマンドをセット
				vm.mCommandArray = commandArray;
				//ラベルをセット
				vm.mLabelIndexDic = labelIndexDic;
				//マシンを稼働状態にする
				vm.mState = true;

				CompilerLog.Log ("完了");
				return vm;
			}
			else
			{
				CompilerLog.Log ("失敗");
				return null;
			}
		}

		private VirtualMachine()
		{
			//コマンド位置を初期化
			mCommandIndex = 0;
			//自分が持つ分の拡張コマンドを登録
			this.SetCommandDelegater (this);
			Converter.SetCommandDelegaters (this);
		}

		//コマンドを実行
		public bool RunCommand()
		{
			int i = Mathf.Max(0, mCommandIndex);
			bool isValidIndex = i < mCommandArray.Length;

			if (isValidIndex && mState)
			{
				mCommandIndex = i + 1;
				mState = mCommandArray [i].Run (this);
			}
			return isValidIndex && mState;
		}
		//ログ出力
		private void Log(string message)
		{
			Debug.Log ("log:"+ message);
		}
		//引数をセット
		public void Push(object value)
		{
			mValueStack.Push (value);
		}
		//引数をゲット(エラーメッセージを返す)
		public string Pop(out object value)
		{
			if (mValueStack.Count != 0) {
				value = mValueStack.Pop ();
				return null;
			} else {
				value = null;
				return "実行時エラーです(Pop)。プログラムの製作者に連絡してください。";
			}
		}

		//外部命令を実行
		public bool RunOrder(string orderCode)
		{
			CommandDelegater cd = null;
			bool valid = true;
			if (mCommandDelegaterDic.TryGetValue (orderCode, out cd))
			{
				string error = cd.Run (this);
				if (error != null)
				{
					CompilerLog.Assert (mLine, mIndex, error);
					valid = false;
				}
			}
			else
				Log ("委譲コマンド:" + orderCode + "が存在しません");
			return valid;
		}
		//拡張コマンドを追加
		public void AddCommandDelegater(string name, CommandDelegater commandDelegater)
		{
			if (!mCommandDelegaterDic.ContainsKey (name)){
				mCommandDelegaterDic.Add (name, commandDelegater);
			} else {
				CompilerLog.Assert ("コンパイラエラーが発生しました(命令重複)。製作者に報告してください。");
				mState = false;
				return;
			}
		}
		//通知を追加
		public void AddNotification(string name, NotifyMethod method)
		{
			List<NotifyMethod> methodList = null;
			if (!mNotificationDic.TryGetValue (name, out methodList))
			{
				methodList = new List<NotifyMethod> ();
				mNotificationDic [name] = methodList;
			}
			methodList.Add (method);
		}

		protected void SetCommandDelegater(VirtualMachine vm)
		{
			//スクリプトの参照元の情報
			vm.AddCommandDelegater (
				"referFrom",
				new CommandDelegater (false, 2, delegate(object[] arguments) {
					string error;
					int line = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					int index = Converter.ObjectToInt(arguments[1], out error);
					if (error != null) return error;
					mLine = line;
					mIndex = index;

					return null;
				}));
			//ラベル
			vm.AddCommandDelegater (
				"label",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					string labelName = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;

					Log ("ラベル:" + labelName + " を通過しました。");
					return null;
				}));
			//ラベルに飛ぶ(ラベル名)
			vm.AddCommandDelegater (
				"jumpLabel",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					string labelName = Converter.ObjectToString (arguments[0], out error);
					if (error != null) return error;

					int labelIndex = 0;
					if (mLabelIndexDic.TryGetValue (labelName, out labelIndex)) {
						mCommandIndex = labelIndex;
					} else {
						return "ラベル(" + labelName + ")が存在しません。";
					}
					return null;
				}));
			
			//ログ出力
			vm.AddCommandDelegater (
				"log",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					string message = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;
					Log(message);
					return null;
				}));

			//ローカル変数
			//取得
			vm.AddCommandDelegater (
				"GetLocal",
				new CommandDelegater (true, 1, delegate(object[] arguments) {
					string error;
					int index = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error; 
					if (0 <= index && index<mLocalMemory.Length){
						arguments[1] = mLocalMemory[index];
						return null;
					}
					return "ローカル変数のインデックスは0 ~ "+ (mLocalMemory.Length - 1) +"です(" + index + ")";
				}));
			//入力
			vm.AddCommandDelegater (
				"SetLocal",
				new CommandDelegater (false, 2, delegate(object[] arguments) {
					string error;
					int index = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;

					object value = arguments[1];
					if (0 <= index && index<mLocalMemory.Length){
						mLocalMemory[index] = value;
						return null;
					}
					return "ローカル変数のインデックスは0 ~ "+ (mLocalMemory.Length - 1) +"です(" + index + ")";
				}));
			
			//計算
			vm.AddCommandDelegater (
				"calc",
				new CommandDelegater(true, 3, delegate(object[] arguments){
					string error;
					string type = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;
					float value0 = Converter.ObjectToFloat(arguments[1], out error);
					if (error != null) return error;
					float value1 = Converter.ObjectToFloat(arguments[2], out error);
					if (error != null) return error;
					float value2 = 0.0f;

					switch(type)
					{
					case "add":
						value2 = value0 + value1;
						break;
					case "sub":
						value2 = value0 - value1;
						break;
					case "mul":
						value2 = value0 * value1;
						break;
					case "div":
						value2 = value0 / value1;
						break;
					case "mod":
						value2 = value0 - (Mathf.Floor(value0 / value1) * value1);
						break;
					default:
						return "正しいoprを指定してください(" + type + ")";
					}
					arguments[3] = value2;
					return null;
				}));

			//条件分岐のif
			vm.AddCommandDelegater (
				"if",
				new CommandDelegater (false, 4, delegate(object[] arguments) {
					string error;
					string type = Converter.ObjectToString (arguments [0], out error);
					if (error != null) return error;
					float value0 = Converter.ObjectToFloat (arguments [1], out error);
					if (error != null) return error;
					float value1 = Converter.ObjectToFloat (arguments [2], out error);
					if (error != null) return error;

					//(次のelseまたはendif)の次の位置
					int commandIndex = Converter.ObjectToInt(arguments [3], out error);
					if (error != null) return error;

					bool flag;

					switch (type) {
					case "eqr":
						flag = value0 == value1;
						break;
					case "neqr":
						flag = value0 != value1;
						break;
					case "grt":
						flag = value0 > value1;
						break;
					case "geq":
						flag = value0 >= value1;
						break;
					case "les":
						flag = value0 < value1;
						break;
					case "leq":
						flag = value0 <= value1;
						break;
					default:
						return "正しいoprを指定してください(" + type + ")";
					}

					//不適格なら次の条件分岐に飛ぶ
					if (!flag)
						mCommandIndex = commandIndex;

					return null;
				}));

			//条件分岐終了時にendifまで飛ぶやつ
			vm.AddCommandDelegater (
				"jump_endif",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					//(次のendif)の次の位置
					string error;
					mCommandIndex = Converter.ObjectToInt(arguments [0], out error);
					if (error != null) return error;

					return null;
				}));
			
			//条件分岐のendif
			vm.AddCommandDelegater (
				"endif",
				new CommandDelegater (false, 0, delegate(object[] arguments) {
					return null;
				}));

			//通知
			vm.AddCommandDelegater (
				"notification",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					string name = Converter.ObjectToString(arguments [0], out error);
					if (error != null) return error;

					List<NotifyMethod> methodList = null;
					if (mNotificationDic.TryGetValue(name, out methodList))
					{
						foreach(NotifyMethod method in methodList)
							method();
					}
					return null;
				}));
		}
	}

	//ヘルパークラス
	public static class Converter
	{
		//objectから型変換
		//	valueは変換前
		//	返り値は変換後
		//	errorはエラーメッセージ
		//Intに変換
		public static int ObjectToInt(object value, out string error)
		{
			error = null;
			int num;
			if (int.TryParse("" + value + "", out num)) {
				return num;
			} else {
				error = "int型にパースできません。(" + value + ")";
				return 0;
			}
		}
		//Floatに変換
		public static float ObjectToFloat(object value, out string error)
		{
			error = null;
			float num;
			if (float.TryParse("" + value + "", out num)) {
				return num;
			} else {
				error = "float型にパースできません。(" + value + ")";
				return 0.0f;
			}
		}
		//Stringに変換
		public static string ObjectToString(object value, out string error)
		{
			error = null;
			return "" + value + "";
		}
		//Colorに変換
		public static Color32 ObjectToColor(object value, out string error)
		{
			error = null;
			string str = Converter.ObjectToString(value, out error);
			if (error != null)
				return new Color32();

			Converter.CreateCodeTable ();
			byte[] element = new byte[4];
			element [0] = 0x00;
			element [1] = 0x00;
			element [2] = 0x00;
			element [3] = 0x00;

			if (str.Length == 6)
				str = str + "FF";

			if (str.Length == 8)
			{
				bool validStr = true;
				for (int i = 0; i < str.Length; i++) {
					int num;
					if (!gCodeTable.TryGetValue (str.Substring (i, 1), out num)) {
						validStr = false;
						break;
					}
					element [i / 2] |= (byte)(num << (i % 2) * 4);
				}

				if (validStr){
					return new Color32 (element [0], element [1], element [2], element [3]);
				}
			}

			error = "カラーにパースできません。(" + str + ")";
			return new Color32 ();
		}
		//Listに変換
		public static List<object> ObjectToList(object value, out string error)
		{
			error = null;
			List<object> list = null;
			if (value is List<object>)
				list = value as List<object>;
			else
			{
				list = new List<object> ();
				list.Add (value);
			}
			return list;
		}

		//変換表を作成
		private static Dictionary<string, int> gCodeTable;
		private static void CreateCodeTable()
		{
			if (gCodeTable != null) return;

			//テーブルの初期化処理
			gCodeTable = new Dictionary<string, int> ();
			gCodeTable ["0"] = 0x0;
			gCodeTable ["1"] = 0x1;
			gCodeTable ["2"] = 0x2;
			gCodeTable ["3"] = 0x3;
			gCodeTable ["4"] = 0x4;
			gCodeTable ["5"] = 0x5;
			gCodeTable ["6"] = 0x6;
			gCodeTable ["7"] = 0x7;
			gCodeTable ["8"] = 0x8;
			gCodeTable ["9"] = 0x9;
			gCodeTable ["A"] = 0xA;
			gCodeTable ["a"] = 0xA;
			gCodeTable ["B"] = 0xB;
			gCodeTable ["b"] = 0xB;
			gCodeTable ["C"] = 0xC;
			gCodeTable ["c"] = 0xC;
			gCodeTable ["D"] = 0xD;
			gCodeTable ["d"] = 0xD;
			gCodeTable ["E"] = 0xE;
			gCodeTable ["e"] = 0xE;
			gCodeTable ["F"] = 0xF;
			gCodeTable ["f"] = 0xF;
		}

		public static void SetCommandDelegaters(VirtualMachine vm)
		{
			//リストを作って、値を追加するところまで
			vm.AddCommandDelegater (
				"CreateList",
				new CommandDelegater (true, 0, delegate(object[] arguments) {
					arguments[0] = new List<object> ();
					return null;
				}));
			vm.AddCommandDelegater (
				"AddList",
				new CommandDelegater (true, 2, delegate(object[] arguments) {
					List<object> list = arguments[0] as List<object>;
					list.Add(arguments[1]);
					arguments[2] = list;
					return null;
				}));
		}
	}
}