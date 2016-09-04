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

using Script.Command;

namespace Script.Analyzer{
	//これをパターンに追加するとテキスト全般が追加されるよ
	public class CreateCommandsOfText : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfText(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfText() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateNameCommandPattern ());
			patternList.Add (new CreateNewPageCommand ());
			patternList.Add (new CreateNewLineCommand ());
			patternList.Add (new CreateTextSpeedPropertyCommand ());

			patternList.Add (new CreateTextCommandPattern ());
			mPatternList = patternList;
		}
	}

	//名前表示コマンド
	public class CreateNameCommandPattern : Pattern_CreateCommand
	{
		public class NameCommand : RunCommand
		{
			private string mName;
			public NameCommand(string name)
			{
				mName = name;
			}
			public override void Setup (){}
			public override void Run()
			{
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				workSpace.StepCommandNext ();
			}
			public override void Finish ()
			{
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				if (mName != "") {
					workSpace.isVisibleNameView = true;
					workSpace.NameView.text = mName;
				} else {
					workSpace.isVisibleNameView = false;
				}
			}
		}

		public override Result Match(List<WordWithName> wordList , int currIndex){
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
				return new Result (null, currIndex, null);
			}
			currIndex = result.CurrIndex;
			Debug.Log ("名前宣言:" + name + "");
			return new Result(new NameCommand (name), currIndex, null);
		}
	}

	//テキスト表示コマンド
	public class CreateTextCommandPattern : Pattern_CreateCommand
	{
		public class TextCommand : RunCommand
		{
			private string mText;
			public TextCommand(string text)
			{
				mText = text;
			}

			//ここに自分以前に保存されていたテキストデータが入る
			private string mPrevText;
			private float mTime;
			public override void Setup ()
			{
				mPrevText = ScenarioWorkSpace.SharedInstance ().TextView.text;
				mTime = 0.0f;
			}
			public override void Run()
			{
				//変数準備
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				Color32 color = workSpace.Color;
				float speed = workSpace.Speed;
				mTime += workSpace.DeltaTime;

				//時間に合わせて文字数を設定
				int chCount = (int)(speed * mTime);
				string showText;
				if (chCount < mText.Length) {
					showText = mText.Substring (0, chCount);
					//表示
					workSpace.TextView.text = mPrevText + ColorTagBeginText(color) + showText + "</color>";
				} else {
					workSpace.StepCommandNext ();
				}
			}
			public override void Finish ()
			{
				//変数準備
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				Color32 color = workSpace.Color;

				//表示
				workSpace.TextView.text = mPrevText + ColorTagBeginText(color) + mText + "</color>";
			}

			public string ColorTagBeginText(Color32 color)
			{
				string r = color.r.ToString("X2");
				string g = color.g.ToString("X2");
				string b = color.b.ToString("X2");
				string a = color.a.ToString("X2");
				return "<color=#"+r+g+b+a+">";
			}
		}

		public override Result Match(List<WordWithName> wordList , int currIndex){
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
				return new Result (null, currIndex, null);
			}
			text = new Regex ("\n").Replace (text, "");
			if (text == "") {
				return new Result (null, currIndex, null);
			}
			currIndex = result.CurrIndex;
			Debug.Log ("テキスト:" + text + "");
			return new Result(new TextCommand (text), currIndex, null);
		}
	}

	//改ページ
	public class CreateNewPageCommand : Pattern_TagFormat
	{
		public class NewPageCommand : RunCommand
		{
			public override void Setup ()
			{
			}
			public override void Run()
			{
				
			}
			public override void Finish()
			{
				Debug.Log("改ページ");
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				workSpace.TextView.text = "";
			}
		}
		protected override string TagName(){
			return "p";
		}
		protected override RunCommand CreateCommand(Queue<Argument> arguments, out string errorMessage)
		{
			errorMessage = null;
			if (arguments.Count > 0) {
				errorMessage = "無効な引数があります。";
				return null;
			}
			Debug.Log ("タグ　　:p");
			return new NewPageCommand ();
		}
	}

	//改行
	public class CreateNewLineCommand : Pattern_TagFormat
	{
		public class NewLineCommand : RunCommand
		{
			public override void Setup (){}
			public override void Run()
			{
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				workSpace.StepCommandNext ();
			}
			public override void Finish ()
			{
				ScenarioWorkSpace workSpace = ScenarioWorkSpace.SharedInstance ();
				//表示
				workSpace.TextView.text += "\n";
			}
		}
		protected override string TagName(){
			return "n";
		}
		protected override RunCommand CreateCommand(Queue<Argument> arguments, out string errorMessage)
		{
			errorMessage = null;
			if (arguments.Count > 0) {
				errorMessage = "無効な引数があります。";
				return null;
			}
			Debug.Log ("タグ　　:n");
			return new NewLineCommand ();
		}
	}

	//テキスト速度のプロパティ
	public class CreateTextSpeedPropertyCommand : Pattern_TagFormat
	{
		public class TextSpeedPropertyCommand : RunCommand
		{
			private OutputCommand mColor;
			private OutputCommand mSpeed;
			public TextSpeedPropertyCommand(OutputCommand color, OutputCommand speed)
			{
				mColor = color;
				mSpeed = speed;
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

				if (mColor != null) {
					int color = (int)mColor.GetInt32 ();
					int r = color >> 24 & 0xFF, g = color >> 16 & 0xFF, b = color >> 8 & 0xFF, a = color >> 0 & 0xFF;
					workSpace.Color = new Color32 ((byte)r, (byte)g, (byte)b, (byte)a);
				}
				if (mSpeed != null) {
					float speed = mSpeed.GetFloat ();
					workSpace.Speed = speed;
				}
			}
		}
		protected override string TagName(){
			return "textspeed";
		}
		protected override RunCommand CreateCommand(Queue<Argument> arguments, out string errorMessage)
		{
			errorMessage = null;
			OutputCommand color = null;
			OutputCommand speed = null;

			while(arguments.Count>0)
			{
				Argument arg = arguments.Dequeue ();
				if (arg.Name == "value") {
					speed = arg.Value;
					continue;
				}
				errorMessage = "無効な引数があります( " + arg.Name + " )";
				return null;
			}

			Debug.Log ("タグ　　:text");
			return new TextSpeedPropertyCommand (color, speed);
		}
	}

}
