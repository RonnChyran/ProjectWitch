//=====================================
//author	:shotta
//summary	:テキストコマンド全般
//=====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; //Exception
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

using Scenario.Pattern;
using Scenario.Command;
using Scenario.WorkSpace;

namespace Scenario.Compiler{
	//これをパターンに追加するとテキスト全般が追加されるよ
	public class CreateCommandsOfText : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfText(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfText() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateNameCommand ());
			patternList.Add (new CreateNewPageCommand ());
			patternList.Add (new CreateNewLineCommand ());
			patternList.Add (new CreateTextCommand ());
			patternList.Add (new CreateClearNameCommand ());
			patternList.Add (new CreateTextSpeedCommand ());
			mPatternList = patternList;
		}
	}

	//名前表示コマンド
	public class CreateNameCommand : Pattern_CreateCommand
	{

		public override Result Match(WordWithName[] wordList , int currIndex){
			string name = "";

			//ここでパターンを作る
			PatternFormat patternSharp = new Pattern_Object(delegate(WordWithName word) {
				return word.Name == "ID_NameBegin";
			});
			PatternFormat patternNameWord = new Pattern_Object(delegate(WordWithName word) {
				return new Regex("[^\n]+").Match(word.Word).Success;
			});
			patternNameWord.GetResultMethod += delegate(Result r) {
				if (!r.Matched) return;
				WordWithName word = r.Value as WordWithName;
				name += word.Word;
			};
			PatternFormat patternNameWords = new Pattern_Loop (patternNameWord, 0);

			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (patternSharp);
			patternList.Add (patternNameWords);
			PatternFormat pattern = new Pattern_List(patternList);

			//パターンに当てはめる
			PatternFormat.Result result = pattern.Match(wordList, currIndex);

			//パターン通りにできているかを調べる
			if (!result.Matched) {
				return new Result (false, null, currIndex);
			}
			currIndex = result.CurrIndex;

			CommandList commandList = new CommandList ();
			commandList.Add (new SetArgumentCommand (name));
			commandList.Add (new RunOrderCommand("Name"));

			return new Result(true, commandList.GetArray(), currIndex);
		}
	}
	//名前クリア
	public class CreateClearNameCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "cn";
		}
		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add(new RunOrderCommand("InvisibleName"));
			return commandList.GetArray ();
		}
	}

	//テキスト表示コマンド
	public class CreateTextCommand : Pattern_CreateCommand
	{
		public override Result Match(WordWithName[] wordList , int currIndex){
			string text = "";

			//ここでパターンを作って
			PatternFormat patternText = new Pattern_Object(delegate(WordWithName word) {
				return new Regex("^CO_").Match(word.Name).Success;
			});
			patternText.GetResultMethod += delegate(Result r) {
				if (!r.Matched) return;
				WordWithName word = r.Value as WordWithName;
				text += word.Word;
			};
			PatternFormat pattern = new Pattern_Loop (patternText, 1);

			//パターンに当てはめる
			PatternFormat.Result result = pattern.Match(wordList, currIndex);

			//パターン通りにできているか
			if (!result.Matched) {
				return new Result (false, null, currIndex);
			}
			//改行は消去
			text = new Regex ("\n").Replace (text, "");
			if (text == "") {
				return new Result (false, null, currIndex);
			}

			currIndex = result.CurrIndex;

			CommandList commandList = new CommandList ();

			commandList.Add(new SetArgumentCommand (text));
			commandList.Add(new RunOrderCommand ("Text"));
			commandList.Add(new RunOrderCommand ("SetUpdater"));

			return new Result(true, commandList.GetArray(), currIndex);
		}
	}
	//改ページ
	public class CreateNewPageCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "p";
		}
		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add(new RunOrderCommand ("NewPage"));
			commandList.Add(new RunOrderCommand ("SetUpdater"));
			return commandList.GetArray ();
		}
	}
	//改行
	public class CreateNewLineCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "n";
		}
		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add(new SetArgumentCommand("\n"));
			commandList.Add(new RunOrderCommand ("Text"));
			commandList.Add(new RunOrderCommand ("SetUpdater"));
			return commandList.GetArray ();
		}
	}

	//テキスト速度のプロパティ
	public class CreateTextSpeedCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "textspeed";
		}
		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();
			if (arguments.ContainName ("value")) {
				commandList.Add(arguments.Get ("value"));
			} else {
				CompilerLog.Log (line, index, "value引数が不足しています。");
				return null;
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			commandList.Add(new RunOrderCommand("TextSpeed"));

			return commandList.GetArray ();
		}
	}
}