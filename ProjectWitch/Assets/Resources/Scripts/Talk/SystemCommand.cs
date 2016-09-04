//==============================
//author	:shotta
//summary	:システム全般
//==============================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Script.Compiler;
using Script.Command;

namespace Script.Analyzer{
	public class CreateCommandsOfSystem : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfSystem(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfSystem() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateLabelCommandPattern ());
			mPatternList = patternList;
		}
	}

	//ラベル情報コマンド
	public class CreateLabelCommandPattern : Pattern_CreateCommand
	{
		public class LabelCommand : CommandFormat
		{
			public string LabelName{get{return mLabelName;}}
			private string mLabelName;
			public LabelCommand(string labelName)
			{
				mLabelName = labelName;
			}
		}
			
		public override Result Match(List<WordWithName> wordList , int currIndex){
			WordWithName asterisk = null;
			bool existError = false;
			string labelName = "";

			//ここでパターンを作って
			PatternFormat patternLabelBegin = new Pattern_Object(delegate(WordWithName word) {
				return word.Name == "ID_LabelBegin";
			});
			patternLabelBegin.GetResultMethod = delegate(Result r) {
				if (!r.Matched) return;
				asterisk = r.Value as WordWithName;
			};
			PatternFormat patternLabelWord = new Pattern_Object(delegate(WordWithName word) {
				return new Regex("[^\n]").Match(word.Word).Success;
			});
			patternLabelWord.GetResultMethod += delegate(Result r) {
				if (!r.Matched) return;
				WordWithName word = r.Value as WordWithName;
				if (word.Name != "CO_KeyWord")
					existError = true;
				labelName += word.Word;
			};
			PatternFormat patternLabelWords = new Pattern_Loop (patternLabelWord, 1);

			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (patternLabelBegin);
			patternList.Add (patternLabelWords);
			PatternFormat pattern = new Pattern_List(patternList);

			//パターンに当てはめ、
			PatternFormat.Result result = pattern.Match(wordList, currIndex);

			//パターン通りにできているかを調べる
			if (!result.Matched) {
				return new Result (null, currIndex, null);
			}

			currIndex = result.CurrIndex;
			//エラー時
			if (existError) {
				ErrorInfo e = new ErrorInfo (asterisk.Line, asterisk.Index, "ラベル名が無効です( *" + labelName + " )");
				return new Result (null, currIndex, e);
			}

			Debug.Log ("ラベル　:" + labelName + "");
			return new Result(new LabelCommand (labelName), currIndex, null);
		}
	}

	/*
	//値のコマンドクラス
	public class ValueCommand : Command
	{
		private object mValue;
		public ValueCommand(object value)
		{
			mValue = value;
		}
		public override string CommandName()
		{
			return "ValueCommand";
		}

		public object RunCommand(){
			return mValue;
		}
	}

	//計算のコマンドクラス
	*/
	/*

	//変数のコマンドクラス
	public class ValiableCommand : Command
	{
		private object mValiableName;
		public ValiableCommand(string valiableName)
		{
			mValiableName = mValiableName;
		}

		public object RunCommand(){
			return null;
		}
	}
	*/
	/*
	//システム関係のパターンを作る静的クラス
	public class CreateSystemPatternClass
	{
		private CreateSystemPatternClass(){}
		public static Dictionary<string, BlankPattern> CreatePattern()
		{
			Dictionary<string, BlankPattern> pattern = new Dictionary<string, BlankPattern> ();

			//ラベルのパターン
			pattern.Add("LabelPattern", new BlankPattern (delegate(List<WordWithName>words, int currIndex, out string error)
				{
					error = null;
					int line  = words[currIndex].Line;
					int index = words[currIndex].Index;

					string label = "";
					int i = 0;
					//ラベルを抽出
					for(;currIndex + i<words.Count;i++)
					{
						bool isEnd = false;
						WordWithName word = words [currIndex + i];
						switch (i) {
						case 0:
							if (word.Name != "ID_LabelBegin") {
								return new PatternMatchResult (false, currIndex, null);
							}
							break;
						default:
							if (word.Name == "CO_Space" || word.Name == "ID_CommentBegin")
								isEnd = true;
							else
								label += word.Word;
							break;
						}
						if (isEnd)
							break;
					}
					//ラベル名が存在するか確認
					if (label == "") {
						error = StructureAnalyzerHelper.ErrorMessage (line, index, "ラベル名が存在しません。");
						return new PatternMatchResult(false, currIndex, null);
					}

					Debug.Log ("ラベル　: "+label+"");
					return new PatternMatchResult(true, currIndex + i, new LabelCommand(label));
				}));

			StructureAnalyzer valueAnalyzer = new StructureAnalyzer ();
			StructureAnalyzer operationAnalyzer = new StructureAnalyzer ();

			//計算のパターン
			pattern.Add("CalculationPattern", new BlankPattern (delegate(List<WordWithName>words, int currIndex, out string error)
				{
					error = null;
					int line  = words[currIndex].Line;
					int index = words[currIndex].Index;

					int i=0;
					int state = 0;
					//a op b
					object a;
					string op;
					object b;
					for(;currIndex + i<words.Count;i++)
					{
						WordWithName word = words [currIndex + i];
						switch (state)
						{
						case 0://演算子の左側を読む
							
							break;
						case 1://演算子を読む
							break;
						case 2://演算子の右側を読む
							break;
						case 3://次に来る演算子と自分の優先度を比較する
							break;
						case 4://自分の優先度が高ければ自分の計算を作る
							break;
						}
					}

					return new PatternMatchResult(true, currIndex + i, new CalcurationCommand);
				}));


			return pattern;
		}
	}
	*/
}