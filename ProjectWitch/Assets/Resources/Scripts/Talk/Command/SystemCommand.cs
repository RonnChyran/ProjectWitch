//==============================
//author	:shotta
//summary	:システム全般
//==============================

using UnityEngine;
using System.Collections.Generic;

using ProjectWitch.Talk.Command;
using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.WorkSpace;

namespace ProjectWitch.Talk.Compiler{
	public class CreateCommandsOfSystem : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfSystem(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfSystem() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateLabelCommand ());
			patternList.Add (new CreateJumpLabelCommand ());
			patternList.Add (new CreateLocalGetCommand ());
			patternList.Add (new CreateLocalSetCommand ());
			patternList.Add (new CreateSystemGetCommand ());
			patternList.Add (new CreateSystemSetCommand ());
			patternList.Add (new CreatePrintCommand ());
			patternList.Add (new CreateLogCommand ());
			patternList.Add (new CreateCalcCommand ());
			patternList.Add (new CreateIfCommand ());
			patternList.Add (new CreateElifCommand ());
			patternList.Add (new CreateElseCommand ());
			patternList.Add (new CreateEndifCommand ());
			mPatternList = patternList;
		}
	}

	//スクリプトの参照元の情報コマンド
	public class CreateReferFromCommand
	{
		public static CommandFormat[] Create(int line, int index)
		{
			CommandFormat[] commandArray = new CommandFormat[3];
			commandArray [0] = new SetArgumentCommand (line);
			commandArray [1] = new SetArgumentCommand (index);
			commandArray [2] = new RunOrderCommand ("referFrom");
			return commandArray;
		}
	}

	//通知コマンド
	public class CreateNotificationCommand
	{
		public static CommandFormat[] Create(string name)
		{
			CommandFormat[] commandArray = new CommandFormat[2];
			commandArray [0] = new SetArgumentCommand (name);
			commandArray [1] = new RunOrderCommand ("notification");
			return commandArray;
		}
	}

	//ラベル情報コマンド
	public class CreateLabelCommand : Pattern_CreateCommand
	{		
		public override Result Match(WordWithName[] wordList , int currIndex){
			string labelName = null;

			//ここでパターンを作って
			PatternFormat patternLabelBegin = new Pattern_Object (delegate(WordWithName word) {
				return word.Name == "ID_LabelBegin";
			});
			PatternFormat patternLabelWord = new Pattern_Object (delegate(WordWithName word) {
				return true;
			});
			patternLabelWord.GetResultMethod = delegate(Result r) {
				WordWithName word = r.Value as WordWithName;
				if (word.Name == "CO_KeyWord")
					labelName = word.Word;
			};

			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (patternLabelBegin);
			patternList.Add (patternLabelWord);
			PatternFormat pattern = new Pattern_List (patternList);

			//パターンに当てはめ、
			PatternFormat.Result result = pattern.Match (wordList, currIndex);

			//パターン通りにできているかを調べる
			bool matched = false;
			CommandFormat[] value = null;
			int nextIndex = currIndex;

			if (labelName != null) {
				matched = true;
				value = new CommandFormat[2];
				value[0] = new SetArgumentCommand (labelName);
				value[1] = new RunOrderCommand ("label");
			}
			if (result.Matched)
			{
				nextIndex = result.CurrIndex;
			}

			return new Result(matched, value, nextIndex);
		}
	}

	//ジャンプ
	public class CreateJumpLabelCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "jump";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("label")) {
				commandList.Add(arguments.Get ("label"));
			} else {
				CompilerLog.Log (line, index, "ref引数が不足しています。");
				return null;
			}

			commandList.Add (new RunOrderCommand("jumpLabel"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//ローカル変数の取得
	public class CreateLocalGetCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "local_memget";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("to")) {
				commandList.Add(arguments.Get ("to"));
			} else {
				CompilerLog.Log (line, index, "to引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("index")) {
				commandList.Add(arguments.Get ("index"));
			} else {
				CompilerLog.Log (line, index, "index引数が不足しています。");
				return null;
			}

			commandList.Add (new RunOrderCommand ("GetLocal"));
			commandList.Add (new RunOrderCommand ("SetLocal"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//ローカル変数に代入
	public class CreateLocalSetCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "local_memset";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("index")) {
				commandList.Add(arguments.Get ("index"));
			} else {
				CompilerLog.Log (line, index, "index引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("value")) {
				commandList.Add(arguments.Get ("value"));
			} else {
				CompilerLog.Log (line, index, "value引数が不足しています。");
				return null;
			}

			commandList.Add (new RunOrderCommand("SetLocal"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//システム変数の取得
	public class CreateSystemGetCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "sys_memget";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("local")) {
				commandList.Add(arguments.Get ("local"));
			} else {
				CompilerLog.Log (line, index, "to引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("index")) {
				commandList.Add(arguments.Get ("index"));
			} else {
				CompilerLog.Log (line, index, "index引数が不足しています。");
				return null;
			}

			commandList.Add (new RunOrderCommand ("GetSystem"));
			commandList.Add (new RunOrderCommand ("SetLocal"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//システム変数に代入
	public class CreateSystemSetCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "sys_memset";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("index")) {
				commandList.Add(arguments.Get ("index"));
			} else {
				CompilerLog.Log (line, index, "index引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("value")) {
				commandList.Add (arguments.Get ("value"));
			} else if (arguments.ContainName ("local")) {
				commandList.Add (arguments.Get ("local"));
				commandList.Add (new RunOrderCommand ("GetLocal"));
			} else {
				CompilerLog.Log (line, index, "value引数もしくはlocal引数が不足しています。");
				return null;
			}

			commandList.Add (new RunOrderCommand("SetSystem"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//プログラム側からテキストを表示
	public class CreatePrintCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "local_memprint";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("index")) {
				commandList.Add(arguments.Get ("index"));
			} else {
				CompilerLog.Log (line, index, "index引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand ("GetLocal"));

			commandList.Add (new RunOrderCommand("Text"));
			commandList.Add (new RunOrderCommand ("SetUpdater"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}

	//プログラム側からログを出力
	public class CreateLogCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "log";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("index")) {
				commandList.Add (arguments.Get ("index"));
				commandList.Add (new RunOrderCommand ("GetLocal"));
			} else if (arguments.ContainName ("value")) {
				commandList.Add (arguments.Get ("value"));
			} else {
				CompilerLog.Log (line, index, "index引数もしくはvalue引数が不足しています。");
				return null;
			}

			commandList.Add (new RunOrderCommand("log"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}

	//計算
	public class CreateCalcCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "calc";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			//SetLocalに渡される{
			if (arguments.ContainName ("to")) {
				commandList.Add (arguments.Get ("to"));
			} else {
				CompilerLog.Log (line, index, "to引数が不足しています。");
				return null;
			}

			//Calcに渡される{
			if (arguments.ContainName ("opr")) {
				commandList.Add (arguments.Get ("opr"));
			} else {
				CompilerLog.Log (line, index, "opr引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("index0")) {
				commandList.Add (arguments.Get ("index0"));
				commandList.Add (new RunOrderCommand ("GetLocal"));
			} else if (arguments.ContainName ("value0")) {
				commandList.Add (arguments.Get ("value0"));
			} else {
				CompilerLog.Log (line, index, "index0引数とvalue0引数のいずれかが不足しています。");
				return null;
			}

			if (arguments.ContainName ("index1")) {
				commandList.Add (arguments.Get ("index1"));
				commandList.Add (new RunOrderCommand ("GetLocal"));
			} else if (arguments.ContainName ("value1")) {
				commandList.Add (arguments.Get ("value1"));
			} else {
				CompilerLog.Log (line, index, "index1引数とvalue1引数のいずれかが不足しています。");
				return null;
			}
			//}
			commandList.Add (new RunOrderCommand("calc"));
			//}
			commandList.Add (new RunOrderCommand ("SetLocal"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}

	//if文
	public class CreateIfCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "if";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			//Ifに渡される{
			if (arguments.ContainName ("opr")) {
				commandList.Add (arguments.Get ("opr"));
			} else {
				CompilerLog.Log (line, index, "opr引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("index0")) {
				commandList.Add (arguments.Get ("index0"));
				commandList.Add (new RunOrderCommand ("GetLocal"));
			} else if (arguments.ContainName ("value0")) {
				commandList.Add (arguments.Get ("value0"));
			} else {
				CompilerLog.Log (line, index, "index0引数とvalue0引数のいずれかが不足しています。");
				return null;
			}

			if (arguments.ContainName ("index1")) {
				commandList.Add (arguments.Get ("index1"));
				commandList.Add (new RunOrderCommand ("GetLocal"));
			} else if (arguments.ContainName ("value1")) {
				commandList.Add (arguments.Get ("value1"));
			} else {
				CompilerLog.Log (line, index, "index1引数とvalue1引数のいずれかが不足しています。");
				return null;
			}

			//次に来るelseもしくはendifの、インデックス+1が入るための領域(後の処理で定義)
			commandList.Add (new SetArgumentCommand (0));
			//}
			commandList.Add (new RunOrderCommand("if"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}
	//elif文
	public class CreateElifCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "elif";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			//前のif文の末端
			commandList.Add (new RunOrderCommand ("jump_endif"));
			//Ifに渡される{
			if (arguments.ContainName ("opr")) {
				commandList.Add (arguments.Get ("opr"));
			} else {
				CompilerLog.Log (line, index, "opr引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("index0")) {
				commandList.Add (arguments.Get ("index0"));
				commandList.Add (new RunOrderCommand ("GetLocal"));
			} else if (arguments.ContainName ("value0")) {
				commandList.Add (arguments.Get ("value0"));
			} else {
				CompilerLog.Log (line, index, "index0引数とvalue0引数のいずれかが不足しています。");
				return null;
			}

			if (arguments.ContainName ("index1")) {
				commandList.Add (arguments.Get ("index1"));
				commandList.Add (new RunOrderCommand ("GetLocal"));
			} else if (arguments.ContainName ("value1")) {
				commandList.Add (arguments.Get ("value1"));
			} else {
				CompilerLog.Log (line, index, "index1引数とvalue1引数のいずれかが不足しています。");
				return null;
			}

			//次に来るelseもしくはendifの、インデックス+1が入るための領域(後の処理で定義)
			commandList.Add (new SetArgumentCommand (0));
			//}
			commandList.Add (new RunOrderCommand("if"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}
	//else文
	public class CreateElseCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "else";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			//次に来るelseもしくはendifの、インデックス+1が入るための領域(後の処理で定義)
			commandList.Add (new SetArgumentCommand(0));
			//前のif文の末端
			commandList.Add (new RunOrderCommand ("jump_endif"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}
	//endif文
	public class CreateEndifCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "endif";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			//前のif文の末端
			commandList.Add (new RunOrderCommand ("endif"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}

}