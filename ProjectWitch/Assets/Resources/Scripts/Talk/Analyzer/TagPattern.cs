//===================================================
//Author	:shotta
//Summary	:タグパターンのフォーマット及びその関連機器
//===================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ProjectWitch.Talk.Compiler;
using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;

namespace ProjectWitch.Talk.Pattern
{
	//タグのフォーマット
	abstract public class Pattern_TagFormat : Pattern_CreateCommand
	{
		public class ArgumentDictionary
		{
			private Dictionary<string, CommandFormat[]> mArgDic;
			public ArgumentDictionary(Dictionary<string, CommandFormat[]> arguments){
				mArgDic = new Dictionary<string, CommandFormat[]>(arguments);
			}

			//引数が含まれるか見る
			public bool ContainName (string name)
			{
				return mArgDic.ContainsKey (name);
			}

			//引数を得て、削除
			//引数nullならその引数は存在しない
			public CommandFormat[] Get (string name)
			{
				CommandFormat[] command = null;
				if (mArgDic.ContainsKey (name))
				{
					command = mArgDic [name];
					mArgDic.Remove (name);
				}
				return command;
			}

			//残り引数を数える
			public int Count{get{ return mArgDic.Count; }}
		}
		abstract protected string TagName ();
		abstract protected CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index);

		public override Result Match(WordWithName[] wordList, int currIndex)
		{
			int line;
			int index;
			{
				WordWithName word = wordList [currIndex];
				line = word.Line;
				index = word.Index;
			}
			CommandFormat[] infoCommand = CreateReferFromCommand.Create (line, index);

			int nextIndex;
			WordWithName[] argumentWordList = GetTagArgument(wordList, currIndex, out nextIndex);
			if (argumentWordList == null)
				return new Result (false, null, currIndex);

			ArgumentDictionary argumentDic = GetArguments (argumentWordList);
			if (argumentDic == null)
				return new Result (false, null, nextIndex);

			CommandFormat[] commandArray = CreateCommand(argumentDic, line, index);
			if (commandArray == null)
				return new Result (false, null, nextIndex);

			List<CommandFormat> commandArrays = new List<CommandFormat> (infoCommand);
			commandArrays.AddRange (commandArray);

			return new Result (true, commandArrays.ToArray (), nextIndex);
		}

		//該当のタグを読み取る&引数全体を読み取る
		private WordWithName[] GetTagArgument(WordWithName[] wordList, int currIndex, out int nextIndex)
		{
			//引数のパラメータを保持
			List<WordWithName> argumentWordList = new List<WordWithName> ();

			//「[」を読む
			PatternFormat patternTagBegin = new Pattern_Object (delegate(WordWithName word) {
				return word.Name == "ID_TagBegin";
			});
			//タグの名前を読む
			PatternFormat patternTagName = new Pattern_Object (delegate(WordWithName word) {
				return word.Word == TagName();
			});

			int tagDepthCount = 1;
			//タグの引数関係を切り出し
			PatternFormat patternArgument = new Pattern_Object (delegate(WordWithName word) {
				if (word.Name == "ID_TagBegin")
					tagDepthCount ++;
				if (word.Name == "ID_TagEnd")
					tagDepthCount --;
				if (tagDepthCount != 0)
					return true;
				else
					return false;
			});
			patternArgument.GetResultMethod = delegate(Result r) {
				if (r.Matched)
					argumentWordList.Add(r.Value as WordWithName);
			};
			PatternFormat patternArguments = new Pattern_Loop (patternArgument, 0);

			//「]」を読む
			PatternFormat patternTagEnd = new Pattern_Object (delegate(WordWithName word) {
				return word.Name == "ID_TagEnd";
			});

			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (patternTagBegin);
			patternList.Add (patternTagName);
			patternList.Add (patternArguments);
			patternList.Add (patternTagEnd);
			PatternFormat pattern = new Pattern_List (patternList);

			Result result = pattern.Match (wordList, currIndex);
			nextIndex = result.CurrIndex;

			if (result.Matched) {
				return argumentWordList.ToArray ();
			}

			return null;
		}
			
		private ArgumentDictionary GetArguments(WordWithName[] wordList)
		{
			Dictionary<string, CommandFormat[]> argumentDic = new Dictionary<string, CommandFormat[]> ();

			//	0	前回までの引数が処理完了
			//	1	引数の値読み取り中
			//	2	引数の名前読み取り中
			//	3	エラー
			int state = 0;
			//現在のインデックスの位置
			int currIndex = wordList.Length - 1;
			//引数の値の定義文
			List<WordWithName> valueWordList = new List<WordWithName> ();
			while (currIndex >= 0)
			{
				WordWithName word = wordList [currIndex];

				//開始フラグを立てる
				if (state == 0)
					state = 1;
				
				switch (state)
				{
				case 1://値を抽出
					if (word.Word == "=")
						//イコールの場合は名前を読む方に切り替え
						state = 2;
					else
						valueWordList.Insert (0, word);
					break;
				case 2://名前を読む
					string name = word.Word;
					CommandFormat[] value = null;

					//引数の値をコマンド化
					WordWithName[] valueWordArray = valueWordList.ToArray ();
					//読み取った分は捨てる
					valueWordList.RemoveRange (0, valueWordList.Count);

					//値として読み取る
					Result r = new CreateCommandsOfValue ().Match (valueWordArray, 0);
					if (r.Matched && r.CurrIndex == valueWordArray.Length) {
						value = r.Value as CommandFormat[];

						//引数の重複なしを確認して登録
						if (!argumentDic.ContainsKey (name)) {
							argumentDic.Add (name, value);
							//この引数の処理は終了
							state = 0;
							break;
						} else {
							CompilerLog.Log (word.Line, word.Index, "引数の名前が重複しています。");
						}
					} else if (currIndex == r.CurrIndex) {
						//マッチしなかった場合
						WordWithName errorWord = wordList[currIndex + 2];
						CompilerLog.Log (errorWord.Line , errorWord.Index , "値の読み取りに失敗しました。");
					}

					//エラー処理
					state = 3;
					break;
				default:
					Debug.Assert (false, "コンパイラエラーです(Tag読み取り)。開発者に報告してください。");
					break;
				}

				if (state >= 3)
					break;
				
				currIndex--;
			}

			if (state == 0) {
				return new ArgumentDictionary (argumentDic);
			} else if (state == 1 || state == 2) {
				WordWithName word = wordList [0];
				CompilerLog.Log (word.Line, word.Index, "引数の書き方が異常です。");
			}
			return null;
		}
	}

	//値のパターンリスト
	public class CreateCommandsOfValue : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfValue(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfValue() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateArrayCommandPattern ());
			patternList.Add (new CreateValueCommandPattern ());
			mPatternList = patternList;
		}
	}

	//値のコマンドパターン
	public class CreateValueCommandPattern : Pattern_CreateCommand
	{
		public override Result Match(WordWithName[] wordList, int currIndex)
		{
			bool matched = false;
			CommandFormat[] value = new CommandFormat[1];
			WordWithName word = wordList[currIndex];
			if (word.Name == "CO_Number") {
				value [0] = new SetArgumentCommand (float.Parse (word.Word));
				matched = true;
				currIndex+=1;
			}
			if (word.Name == "CO_String") {
				string str = word.Word.Substring (1, word.Word.Length - 2);
				value [0] = new SetArgumentCommand (str);
				matched = true;
				currIndex+=1;
			}
			Result result = new Result (matched, value, currIndex);
			SendResultToDelegate (result);
			return result;
		}
	}

	//配列のコマンドパターン
	public class CreateArrayCommandPattern : Pattern_CreateCommand
	{
		public override Result Match(WordWithName[] wordList, int currIndex)
		{
			bool matched = false;
			CommandList commandList = new CommandList ();
			commandList.Add (new RunOrderCommand("CreateList"));

			PatternFormat patternValue = new CreateValueCommandPattern ();
			PatternFormat patternPartation = new Pattern_Object (delegate(WordWithName w) {
				bool oneMatched = (w.Word == ",");
				if (oneMatched) matched = true;
				return oneMatched;
			});

			List<PatternFormat> patternElementList = new List<PatternFormat> ();
			patternElementList.Add (patternValue);
			patternElementList.Add (patternPartation);

			PatternFormat patternElement = new Pattern_List (patternElementList);
			patternElement.GetResultMethod = delegate(Result r) {
				if (r.Matched)
				{
					List<object> list = r.Value as List<object>;
					commandList.Add (list[0] as CommandFormat[]);
					commandList.Add (new RunOrderCommand("AddList"));
				}
			};

			PatternFormat patternLoop = new Pattern_Loop(patternElement, 1);

			List<PatternFormat> patternArrayList = new List<PatternFormat> ();
			patternArrayList.Add (patternLoop);
			patternArrayList.Add (patternValue);

			PatternFormat patternArray = new Pattern_List (patternArrayList);
			patternArray.GetResultMethod = delegate(Result r) {
				if (r.Matched)
				{
					List<object> list = r.Value as List<object>;
					commandList.Add (list [1] as CommandFormat[]);
					commandList.Add (new RunOrderCommand ("AddList"));
				}
			};

			Result result = patternArray.Match (wordList, currIndex);

			WordWithName word = wordList[currIndex];
			if (!result.Matched)
			{
				if (matched)
				{
					CompilerLog.Log (word.Line, word.Index, "配列の書き方に異常があります。");
					return new Result (false, null, result.CurrIndex);
				}
				return new Result(false, null, currIndex);
			}

			Result returnResult = new Result (true, commandList.GetArray(), result.CurrIndex);
			SendResultToDelegate (returnResult);
			return returnResult;
		}
	}
}
