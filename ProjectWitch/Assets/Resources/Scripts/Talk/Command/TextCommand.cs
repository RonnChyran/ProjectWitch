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

using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;
using ProjectWitch.Talk.WorkSpace;

namespace ProjectWitch.Talk.Compiler{
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
            patternList.Add(new CreateChangeSkinCommand());
			patternList.Add (new CreateShowTextWindowCommand ());
			patternList.Add (new CreateHideTextWindowCommand ());
			patternList.Add (new CreateTextSpeedCommand ());

            patternList.Add(new CreateLoadFaceCommand());
            patternList.Add(new CreateDrawFaceCommand());
            patternList.Add(new CreateClearFaceCommand());
            patternList.Add(new CreateChangeFaceCommand());

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

    //テキストスキンの変更
    public class CreateChangeSkinCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "setskin";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if(arguments.ContainName("ref"))
            {
                commandList.Add(arguments.Get("ref"));
            }
            else
            {
                CompilerLog.Log(line, index, "引数refが見つかりません。");
                return null;
            }

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }

            commandList.Add(new RunOrderCommand("ChangeSkin"));

            return commandList.GetArray();
        }


    }


	//テキストウィンドウを表示
	public class CreateShowTextWindowCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "show_message";
		}
		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

            if(arguments.ContainName("pos"))
            {
                commandList.Add(arguments.Get("pos"));
            }
            else
            {
                commandList.Add(new SetArgumentCommand("bottom"));
            }

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add(new RunOrderCommand("ShowMessage"));
			commandList.Add(new RunOrderCommand ("SetUpdater"));
			return commandList.GetArray ();
		}
	}
	//テキストウィンドウを非表示
	public class CreateHideTextWindowCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "hide_message";
		}
		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add(new RunOrderCommand("HideMessage"));
			commandList.Add(new RunOrderCommand ("SetUpdater"));
			return commandList.GetArray ();
		}
	}
    
    //顔グラロード
    public class CreateLoadFaceCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "loadfg";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line ,int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.ContainName("id"))
            {
                commandList.Add(arguments.Get("id"));
            }
            else
            {
                CompilerLog.Log("引数idが足りません。");
                return null;
            }

            if (arguments.ContainName("ref"))
            {
                commandList.Add(arguments.Get("ref"));
            }
            else
            {
                CompilerLog.Log("引数refが足りません");
                return null;
            }

            
            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります");
                return null;
            }

            commandList.Add(new RunOrderCommand("LoadFace"));

            return commandList.GetArray();
        }
    }
    //顔グラ表示
    public class CreateDrawFaceCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "drawfg";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.ContainName("id"))
            {
                commandList.Add(arguments.Get("id"));
            }
            else
            {
                CompilerLog.Log("引数idが足りません。");
                return null;
            }

            if (arguments.ContainName("state"))
            {
                commandList.Add(arguments.Get("state"));
            }
            else
            {
                commandList.Add(new SetArgumentCommand(""));
            }


            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります");
                return null;
            }

            commandList.Add(new RunOrderCommand("DrawFace"));

            return commandList.GetArray();
        }
    }
    //顔グラ非表示
    public class CreateClearFaceCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "clearfg";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります");
                return null;
            }

            commandList.Add(new RunOrderCommand("ClearFace"));

            return commandList.GetArray();
        }
    }
    //顔ぐら表情変更
    public class CreateChangeFaceCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "changefg";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if(arguments.ContainName("state"))
            {
                commandList.Add(arguments.Get("state"));
            }
            else
            {
                CompilerLog.Log("引数stateが必要です。");
                return null;
            }

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります");
                return null;
            }

            commandList.Add(new RunOrderCommand("ChangeFace"));

            return commandList.GetArray();
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
            text = new Regex("\r").Replace(text, "");
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