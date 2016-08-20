//===================================================
//Author	:shotta
//Summary	:タグパターンのフォーマット及びその関連機器
//===================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Script.Compiler;
using Script.Command;

namespace Script.Analyzer
{
	//タグを作るやつ
	abstract public class Pattern_TagFormat : Pattern_CreateCommand
	{
		protected class Argument
		{
			public string Name{ get { return mName; } }
			public OutputCommand Value{ get { return mValue; } }
			private string mName;
			private OutputCommand mValue;
			public Argument(string name, OutputCommand value)
			{
				mName = name;
				mValue = value;
			}
		}

		abstract protected string TagName ();
		abstract protected RunCommand CreateCommand(Queue<Argument> arguments, out string errorMessage);

		public override Result Match(List<WordWithName> wordList, int currIndex)
		{
			int line;
			int index;
			{
				WordWithName word = wordList [currIndex];
				line = word.Line;
				index = word.Index;
			}
			bool isTagMatched = false;

			Queue<Argument> arguments = new Queue<Argument> ();
			PatternFormat patternTagBegin = new Pattern_Object (delegate(WordWithName word) {
				return word.Name == "ID_TagBegin";
			});
			//タグの名前を読む
			PatternFormat patternTagName = new Pattern_Object (delegate(WordWithName word) {
				return word.Word == TagName();
			});
			patternTagName.GetResultMethod += delegate(Result r) {
				isTagMatched = r.Matched;
			};

			//タグの引数を読む
			PatternFormat patternArgName = new Pattern_Object (delegate(WordWithName word) {
				return word.Name == "CO_KeyWord";
			});
			PatternFormat patternEqual = new Pattern_Object (delegate(WordWithName word) {
				return word.Word == "=";
			});
			PatternFormat patternValue = new CreateCommandsOfValue();

			List<PatternFormat> patternArgList = new List<PatternFormat> ();
			patternArgList.Add (patternArgName);
			patternArgList.Add (patternEqual);
			patternArgList.Add (patternValue);
			PatternFormat patternArgument = new Pattern_List(patternArgList);
			patternArgument.GetResultMethod += delegate(Result r) {
				if (!r.Matched) return;
				List<object> expressions = r.Value as List<object>;
				string name = (expressions[0] as WordWithName).Word;
				OutputCommand command = expressions[2] as OutputCommand;
				arguments.Enqueue(new Argument(name, command));
			};

			PatternFormat patternArguments = new Pattern_Loop (patternArgument, 0);
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
			object value = null;
			ErrorInfo e = null;
			if (result.Matched)
			{
				string errorMsg = null;
				currIndex = result.CurrIndex;
				value = CreateCommand (arguments, out errorMsg);
				if (errorMsg != null)
					e = new ErrorInfo (line, index, errorMsg);
			}
			else
			{
				if (isTagMatched)
					e = new ErrorInfo(line, index, "「" + TagName() + "」タグの表記が異常です。");
			}
			return new Result (value, currIndex, e);
		}
	}

	//値のパターンリスト
	public class CreateCommandsOfValue : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfValue(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfValue() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateValueCommandPattern ());
			mPatternList = patternList;
		}
	}

	//値のコマンドパターン
	public class CreateValueCommandPattern : Pattern_CreateCommand
	{
		public class ValueCommand : OutputCommand
		{
			private object mValue;
			protected ValueCommand(){}
			public ValueCommand(object value){
				mValue = value;
			}
			public override object Get(){
				return mValue;
			}
		}
		public override Result Match(List<WordWithName> wordList, int currIndex)
		{
			object value = null;
			WordWithName word = wordList[currIndex];
			if (word.Name == "CO_Number") {
				value = new ValueCommand (float.Parse(word.Word));
			}
			if (word.Name == "CO_String") {
				string str = word.Word.Substring(1, word.Word.Length - 2);
				value = new ValueCommand (str);
			}
			if (word.Name == "CO_16bitNumber") {
				string value16Str = word.Word.Substring(1);
				int value16Num = 0;
				for (int i = 0;i<value16Str.Length;i++)
				{
					string numStr = value16Str.Substring (value16Str.Length - (i+1), 1);
					int num = -1;
					if (numStr == "0") num = 0x0;
					if (numStr == "1") num = 0x1;
					if (numStr == "2") num = 0x2;
					if (numStr == "3") num = 0x3;
					if (numStr == "4") num = 0x4;
					if (numStr == "5") num = 0x5;
					if (numStr == "6") num = 0x6;
					if (numStr == "7") num = 0x7;
					if (numStr == "8") num = 0x8;
					if (numStr == "9") num = 0x9;
					if (numStr == "A" || numStr == "a") num = 0xA;
					if (numStr == "B" || numStr == "b") num = 0xB;
					if (numStr == "C" || numStr == "c") num = 0xC;
					if (numStr == "D" || numStr == "d") num = 0xD;
					if (numStr == "E" || numStr == "e") num = 0xE;
					if (numStr == "F" || numStr == "f") num = 0xF;
					value16Num |= num << i*4;
				}
				value = new ValueCommand (value16Num);
			}
			Result result = new Result (value, currIndex + 1, null);
			SendResultToDelegate (result);
			return result;
		}
	}
	//変数のコマンドパターン
	public class CreateValiableCommandPattern : Pattern_CreateCommand
	{
		public class ValiableCommand : IOCommand
		{
			private string mValiableName;
			public ValiableCommand(string valiableName){
				mValiableName = valiableName;
			}
			public override object Get(){
				return null;
			}
			public override void Set(object value){
			}
		}
		public override Result Match(List<WordWithName> wordList, int currIndex)
		{
			object value = null;
			WordWithName word = wordList[currIndex];
			if (word.Name == "CO_KeyWord") {
				value = new ValiableCommand (word.Word);
			}
			Result result = new Result (value, currIndex + 1, null);
			SendResultToDelegate (result);
			return result;
		}
	}
}

