//=====================================
//author	:shotta
//summary	:演出コマンド全般
//=====================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Script.Command;

namespace Script.Analyzer{
	//これをパターンに追加すると演出全般が追加されるよ
	public class CreateCommandsOfGraphicAndSound : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfGraphicAndSound(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfGraphicAndSound() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateBackgroundCommand ());
			patternList.Add (new CreateLoadCGCommand ());
			mPatternList = patternList;
		}
	}

	//背景設定
	public class CreateBackgroundCommand : Pattern_TagFormat
	{
		public class BackgroundCommand : RunCommand
		{
			private OutputCommand mPath;
			public BackgroundCommand(OutputCommand path)
			{
				mPath = path;
			}

			public override void Setup (){}
			public override void Run()
			{
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				workSpace.StepCommandNext();
			}
			public override void Finish ()
			{
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				string name = mPath.GetString (); 
				workSpace.SetBackgroundImage (name);
			}
		}
		protected override string TagName(){
			return "background";
		}
		protected override RunCommand CreateCommand(Queue<Argument> arguments, out string errorMessage)
		{
			errorMessage = null;
			OutputCommand path = null;

			while(arguments.Count>0)
			{
				Argument arg = arguments.Dequeue ();
				if (arg.Name == "ref") {
					path = arg.Value;
					continue;
				}
				errorMessage = "無効な引数があります( " + arg.Name + " )";
				return null;
			}
			if (path == null)
				errorMessage = "引数が不足しています(path)";

			Debug.Log ("タグ　　:bakground");
			return new BackgroundCommand (path);
		}
	}

	//立ち絵読み込み
	public class CreateLoadCGCommand : Pattern_TagFormat
	{
		public class LoadCGCommand : RunCommand
		{
			private OutputCommand mPath;
			private OutputCommand mID;
			public LoadCGCommand( OutputCommand path, OutputCommand id)
			{
				mPath = path;
				mID = id;
			}

			public override void Setup (){}
			public override void Run()
			{
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				workSpace.StepCommandNext();
			}
			public override void Finish ()
			{
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				string name = mPath.GetString ();
				int id = mID.GetInt32 ();
				workSpace.LoadCG (id, name);
			}
		}
		protected override string TagName(){
			return "loadcg";
		}
		protected override RunCommand CreateCommand(Queue<Argument> arguments, out string errorMessage)
		{
			errorMessage = null;
			OutputCommand path = null;
			OutputCommand id = null;

			while(arguments.Count>0)
			{
				Argument arg = arguments.Dequeue ();
				if (arg.Name == "ref") {
					path = arg.Value;
					continue;
				}
				if (arg.Name == "id") {
					id = arg.Value;
					continue;
				}
				errorMessage = "無効な引数があります( " + arg.Name + " )";
				return null;
			}

			string errorArg = "";
			if (path == null)
				errorArg += " path ";
			if (id == null)
				errorArg += " id ";
			if (errorArg != "")
				errorMessage = "引数が不足しています(" + errorArg + ")";

			Debug.Log ("タグ　　:loadcg");
			return new LoadCGCommand (path, id);
		}
	}

}