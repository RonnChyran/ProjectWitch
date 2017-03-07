//=====================================
//author	:shotta
//summary	:グラフィックコマンド全般
//=====================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;
using System;

namespace ProjectWitch.Talk.Compiler{
	//これをパターンに追加すると演出全般が追加されるよ
	public class CreateCommandsOfGraphics : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfGraphics(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfGraphics() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
            patternList.Add(new CreateDrawBGCommand());
            patternList.Add(new CreateClearBGCommand());
			patternList.Add (new CreateLoadCGCommand ());
			patternList.Add (new CreateShowCGCommand ());
            patternList.Add (new CreateChangeCGCommand());
			patternList.Add (new CreateHideCGCommand ());

            patternList.Add(new CreateCreateGameObject());
            patternList.Add(new CreateDeleteGameObject());

			patternList.Add (new CreateMoveToCommand ());
			patternList.Add (new CreateMoveCommand ());
			patternList.Add (new CreateRotateCommand ());
			patternList.Add (new CreateScaleCommand ());

			patternList.Add (new CreateMaskCommand ());

			patternList.Add (new CreateMovieCommand ());

			mPatternList = patternList;
		}
	}

	//背景設定
	public class CreateDrawBGCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "drawbg";
		}
		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();
			if (arguments.ContainName ("ref")) {
				commandList.Add(arguments.Get ("ref"));
                commandList.Add(new SetArgumentCommand("0"));
			} else {
				CompilerLog.Log (line, index, "ref引数が不足しています。");
				return null;
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			commandList.Add(new RunOrderCommand("SetBackground"));
			return commandList.GetArray ();
		}
	}

    //背景削除
    public class CreateClearBGCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "clearbg";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            commandList.Add(new SetArgumentCommand("0"));
            commandList.Add(new SetArgumentCommand("1"));

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("SetBackground"));
            return commandList.GetArray();
        }
    }

    //立ち絵読み込み
    public class CreateLoadCGCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "loadcg";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("id")) {
				commandList.Add(arguments.Get ("id"));
			} else {
				CompilerLog.Log (line, index, "id引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("ref")) {
				commandList.Add(arguments.Get ("ref"));
			} else {
				CompilerLog.Log (line, index, "ref引数が不足しています。");
				return null;
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add(new RunOrderCommand("LoadCG"));
			return commandList.GetArray ();
		}
	}

	//立ち絵表示
	public class CreateShowCGCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "drawcg";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("id")) {
				commandList.Add(arguments.Get ("id"));
			} else {
				CompilerLog.Log (line, index, "id引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("pos")) {
				commandList.Add(arguments.Get ("pos"));
			} else {
				commandList.Add(new SetArgumentCommand ("center"));
			}

			if (arguments.ContainName ("mode")) {
				commandList.Add(arguments.Get ("mode"));
			} else {
				commandList.Add(new SetArgumentCommand ("fadein"));
			}

			if (arguments.ContainName ("layer")) {
				commandList.Add(arguments.Get ("layer"));
			} else {
				commandList.Add(new SetArgumentCommand ("back"));
			}

            if(arguments.ContainName("dir"))
            {
                commandList.Add(arguments.Get("dir"));
            }
            else
            {
                commandList.Add(new SetArgumentCommand("left"));
            }

            if(arguments.ContainName("posx"))
            {
                commandList.Add(arguments.Get("posx"));
            }
            else
            {
                commandList.Add(new SetArgumentCommand(""));
            }

            if(arguments.ContainName("posy"))
            {
                commandList.Add(arguments.Get("posy"));
            }
            else
            {
                commandList.Add(new SetArgumentCommand(""));
            }

            if(arguments.ContainName("state"))
            {
                commandList.Add(arguments.Get("state"));
            }
            else
            {
                commandList.Add(new SetArgumentCommand(""));
            }

			commandList.Add (new RunOrderCommand ("ShowCG"));
			commandList.Add (new RunOrderCommand ("SetUpdater"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}

	//立ち絵非表示
	public class CreateHideCGCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "clearcg";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("id")) {
				commandList.Add (arguments.Get ("id"));
			} else {
				CompilerLog.Log (line, index, "id引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("mode")) {
				commandList.Add(arguments.Get ("mode"));
			} else {
				commandList.Add(new SetArgumentCommand ("fadeout"));
			}
			commandList.Add(new RunOrderCommand("HideCG"));
			commandList.Add (new RunOrderCommand ("SetUpdater"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			return commandList.GetArray ();
		}
	}

    public class CreateChangeCGCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "changecg";
        }

        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            var commandList = new CommandList();

            if(arguments.ContainName("id"))
            {
                commandList.Add(arguments.Get("id"));
            }
            else
            {
                CompilerLog.Log(line, index, "id引数が不足しています");
                return null;
            }

            if(arguments.ContainName("state"))
            {
                commandList.Add(arguments.Get("state"));
            }
            else
            {
                CompilerLog.Log(line, index, "state引数が不足しています");
                return null;
            }

            commandList.Add(new RunOrderCommand("ChangeCG"));

            return commandList.GetArray();
        }
    }

    //ゲームオブジェクトの生成
    public class CreateCreateGameObject : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "create_gameobject"; 
        }

        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if(arguments.ContainName("id"))
            {
                commandList.Add(arguments.Get("id"));
            }
            else
            {
                CompilerLog.Log(line, index, "id引数が不足しています。");
                return null;
            }
            if(arguments.ContainName("ref"))
            {
                commandList.Add(arguments.Get("ref"));
            }
            else
            {
                CompilerLog.Log(line, index, "引数refが不足しています。");
                return null;
            }
            if(arguments.ContainName("x"))
            {
                commandList.Add(arguments.Get("x"));
            }
            else
            {
                commandList.Add(new SetArgumentCommand("0"));
            }
            if(arguments.ContainName("y"))
            {
                commandList.Add(arguments.Get("y"));
            }
            else
            {
                commandList.Add(new SetArgumentCommand("0"));
            }

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }

            commandList.Add(new RunOrderCommand("CreateGameObject"));

            return commandList.GetArray();

        }
    }

    //ゲームオブジェクトの削除
    public class CreateDeleteGameObject : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "delete_gameobject";
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
                CompilerLog.Log(line, index, "id引数が不足しています。");
                return null;
            }

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }

            commandList.Add(new RunOrderCommand("DeleteGameObject"));

            return commandList.GetArray();

        }
    }


    //立ち絵移動
    public class CreateMoveToCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "moveto";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("id")) {
				commandList.Add (arguments.Get ("id"));
			} else {
				CompilerLog.Log (line, index, "id引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("x")) {
				commandList.Add (arguments.Get ("x"));
			} else {
                commandList.Add(new SetArgumentCommand(""));
			}
			if (arguments.ContainName ("y")) {
				commandList.Add (arguments.Get ("y"));
			} else {
                commandList.Add(new SetArgumentCommand(""));
			}
			if (arguments.ContainName ("time")) {
				commandList.Add (arguments.Get ("time"));
			} else {
				CompilerLog.Log (line, index, "time引数が不足しています。");
				return null;
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add (new RunOrderCommand ("moveTo"));
			commandList.Add (new RunOrderCommand ("SetUpdater"));

			return commandList.GetArray ();
		}
	}

	//立ち絵移動
	public class CreateMoveCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "move";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("id")) {
				commandList.Add (arguments.Get ("id"));
			} else {
				CompilerLog.Log (line, index, "id引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("x")) {
				commandList.Add (arguments.Get ("x"));
			} else {
                commandList.Add(new SetArgumentCommand("0"));
			}
			if (arguments.ContainName ("y")) {
				commandList.Add (arguments.Get ("y"));
			} else {
                commandList.Add(new SetArgumentCommand("0"));
            }
			if (arguments.ContainName ("time")) {
				commandList.Add (arguments.Get ("time"));
			} else {
				CompilerLog.Log (line, index, "time引数が不足しています。");
				return null;
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add (new RunOrderCommand ("move"));
			commandList.Add (new RunOrderCommand ("SetUpdater"));

			return commandList.GetArray ();
		}
	}

	//立ち絵回転
	public class CreateRotateCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "rotate";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("id")) {
				commandList.Add (arguments.Get ("id"));
			} else {
				CompilerLog.Log (line, index, "id引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("angle")) {
				commandList.Add (arguments.Get ("angle"));
			} else {
				CompilerLog.Log (line, index, "angle引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("time")) {
				commandList.Add (arguments.Get ("time"));
			} else {
				CompilerLog.Log (line, index, "time引数が不足しています。");
				return null;
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add (new RunOrderCommand ("rotate"));
			commandList.Add (new RunOrderCommand ("SetUpdater"));

			return commandList.GetArray ();
		}
	}

	//立ち絵拡大縮小
	public class CreateScaleCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "scale";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("id")) {
				commandList.Add (arguments.Get ("id"));
			} else {
				CompilerLog.Log (line, index, "id引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("size")) {
				commandList.Add (arguments.Get ("size"));
                commandList.Add(arguments.Get("size"));
			} else {
                //sx,sy指定の場合
                if (arguments.ContainName("sx"))
                {
                    commandList.Add(arguments.Get("sx"));
                }
                else
                {
                    commandList.Add(new SetArgumentCommand("1.0"));
                }
                if (arguments.ContainName("sy"))
                {
                    commandList.Add(arguments.Get("sy"));
                }
                else
                {
                    commandList.Add(new SetArgumentCommand("1.0"));
                }
			}
			if (arguments.ContainName ("time")) {
				commandList.Add (arguments.Get ("time"));
			} else {
				CompilerLog.Log (line, index, "time引数が不足しています。");
				return null;
			}



            if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add (new RunOrderCommand ("scale"));
			commandList.Add (new RunOrderCommand ("SetUpdater"));

			return commandList.GetArray ();
		}
	}


	//マスクを作成
	public class CreateMaskCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "filter";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("name")) {
				commandList.Add (arguments.Get ("name"));
			} else {
				CompilerLog.Log (line, index, "name引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("time")) {
				commandList.Add (arguments.Get ("time"));
			} else {
				CompilerLog.Log (line, index, "time引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("color")) {
				commandList.Add (arguments.Get ("color"));
			} else {
				commandList.Add (new SetArgumentCommand("000000FF"));
			}
				
			if (arguments.ContainName ("trans")) {
				commandList.Add (arguments.Get ("trans"));
			} else {
				commandList.Add (new SetArgumentCommand(1.0f));
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			commandList.Add (new RunOrderCommand ("Filter"));
			commandList.Add (new RunOrderCommand ("SetUpdater"));

			return commandList.GetArray ();
		}
	}

	//動画再生
	public class CreateMovieCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "movie";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("ref")) {
				commandList.Add(arguments.Get ("ref"));
			} else {
				CompilerLog.Log (line, index, "ref引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("skip")) {
				commandList.Add(arguments.Get ("skip"));
			} else {
				CompilerLog.Log (line, index, "skip引数が不足しています。");
				return null;
			}

			commandList.Add (new RunOrderCommand("movie"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}

	}
}