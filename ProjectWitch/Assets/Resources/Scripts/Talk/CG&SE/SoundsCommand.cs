//=====================================
//author	:shotta
//summary	:音コマンド全般
//=====================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Scenario.Pattern;
using Scenario.Command;
using Scenario.WorkSpace;

namespace Scenario.Compiler{
	//これをパターンに追加すると演出全般が追加されるよ
	public class CreateCommandsOfSound : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfSound(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfSound() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreatePlayBGMCommand ());
			patternList.Add (new CreateStopBGMCommand ());
			patternList.Add (new CreatePlaySECommand ());
			patternList.Add (new CreatePlayVoiceCommand ());

			mPatternList = patternList;
		}
	}

	//BGM再生
	public class CreatePlayBGMCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "bgm";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("ref")) {
				commandList.Add(arguments.Get ("ref"));
			} else {
				CompilerLog.Log (line, index, "ref引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("volume")) {
				commandList.Add(arguments.Get ("volume"));
			} else {
				commandList.Add(new SetArgumentCommand (100));
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add (new RunOrderCommand("PlayBGM"));

			return commandList.GetArray ();
		}
	}

	//BGM停止
	public class CreateStopBGMCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "stopbgm";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}

			commandList.Add (new RunOrderCommand ("StopBGM"));
			return commandList.GetArray ();
		}
	}

	//SE再生
	public class CreatePlaySECommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "se";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("ref")) {
				commandList.Add(arguments.Get ("ref"));
			} else {
				CompilerLog.Log (line, index, "ref引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("volume")) {
				commandList.Add (arguments.Get ("volume"));
			} else {
				commandList.Add(new SetArgumentCommand (100));
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			commandList.Add (new RunOrderCommand ("PlaySE"));

			return commandList.GetArray ();
		}
	}

	//SE再生
	public class CreatePlayVoiceCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "voice";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList ();

			if (arguments.ContainName ("ref")) {
				commandList.Add(arguments.Get ("ref"));
			} else {
				CompilerLog.Log (line, index, "ref引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("volume")) {
				commandList.Add (arguments.Get ("volume"));
			} else {
				commandList.Add(new SetArgumentCommand (100));
			}

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			commandList.Add (new RunOrderCommand ("PlayVoice"));

			return commandList.GetArray ();
		}
	}
}