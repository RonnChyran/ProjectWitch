//===================================
//author	:shotta
//summary	:コンパイラ
//===================================

using System.Collections.Generic;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System; //Exception
using System.Text; //Encoding
using System.Text.RegularExpressions;

using Script.Command;
using Script.Analyzer;

namespace Script.Compiler
{
	public class ScriptCompiler : CompilerFormat
	{
		private WordAnalyzer mWordAnalyzer;
		private StructureAnalyzer mStructureAnalyzer;

		public ScriptCompiler()
		{
			//字句解析器を作る
			//文頭、文末表現は行の頭と末を表す仕様になってる
			mWordAnalyzer = new WordAnalyzer();

			//!!CAUTION!!
			//インデックスの若いものから優先的にマッチングされる

			//以下は自分で定義したもの
			//ID_ は分類記号を示す
			mWordAnalyzer.AddAnalyzeWordInfo ("ID_TagBegin", "(?!\\\\)\\[");
			mWordAnalyzer.AddAnalyzeWordInfo ("ID_TagEnd", "(?!\\\\)\\]");
			mWordAnalyzer.AddAnalyzeWordInfo ("ID_NameBegin", "^#");
			mWordAnalyzer.AddAnalyzeWordInfo ("ID_LabelBegin", "^\\*");

			//CO_ はコンテンツを示す
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_16bitNumber", "#[0-9A-F]+");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_Number", "[0-9]+(?:\\.[0-9]+)?");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_Operator", "[=\\+\\-\\*\\/<>&\\|]+");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_CalculationBlock", "\\(\\)");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_KeyWord", "[a-zA-Z_][a-zA-Z0-9_]*");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_String", "\"[^(?!\\\\)\"]+\"");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_Space", "\\s+");
			mWordAnalyzer.AddRestWordInfo ("CO_OtherText");//残りの部分は全部テキストに

			//以下は仕様
			//Invalid_ は無効な表現を示す
			mWordAnalyzer.AddAnalyzeWordInfo ("Invalid_#", "#");
			mWordAnalyzer.AddAnalyzeWordInfo ("Invalid_*", "\\*");

			//ここで構文を作る
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateCommandsOfText ());
			patternList.Add (new CreateCommandsOfGraphicAndSound ());
			patternList.Add (new CreateCommandsOfSystem ());
			//patternList.Add (new CreateTagCommandPatternFormat ());

			PatternFormat pattern = new Pattern_Component (patternList);
			mStructureAnalyzer = new StructureAnalyzer(pattern);
		}

		//コンパイル
		public List<RunCommand> CompileScript(string path, out ErrorList errorList)
		{
			FileInfo fi = new FileInfo(path);
			StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.GetEncoding("UTF-8"));

			string text = sr.ReadToEnd ();
			//コメントを消去
			text = new Regex (";[^\n]*").Replace (text, "");

			//字句解析
			List<WordWithName> wordList = mWordAnalyzer.Analyze (text, out errorList);
			if (errorList.Count != 0)
				return null;
			//タグ内の余分な空白を除去
			List<WordWithName> adjustedWordList = DeleteSpaceFromWordList(wordList, out errorList);
			if (errorList.Count != 0)
				return null;
			//構文解析
			List<CommandFormat> commandList = mStructureAnalyzer.Analyze(adjustedWordList, out errorList);
			if (errorList.Count != 0)
				return null;

			List<RunCommand> runCommandList = ConvertCommandFormatToRunCommand (commandList, out errorList);
			if (errorList.Count != 0)
				return null;

			return runCommandList;
		}

		//タグ内の余分な空白を除去(ついでにタグの[]についてのエラーも検出する)
		public List<WordWithName> DeleteSpaceFromWordList(List<WordWithName> wordList, out ErrorList errorList)
		{
			errorList = new ErrorList ();
			List<WordWithName> adjustedWordList = new List<WordWithName> ();
			Stack<WordWithName> tagStack = new Stack<WordWithName> ();

			for (int i = 0; i < wordList.Count; i++) {
				WordWithName word = wordList [i];
				bool dependentTagEnd = false;

				if (word.Name == "ID_TagBegin")
					tagStack.Push (word);
				if (word.Name == "ID_TagEnd") {
					if (tagStack.Count > 0)
						tagStack.Pop ();
					else
						dependentTagEnd = true;
				}

				if (!(word.Name == "CO_Space" && tagStack.Count > 0))
					adjustedWordList.Add (word);

				if (dependentTagEnd)
					errorList.Add(word.Line, word.Index, "タグが閉じられていません( "+word.Word+" )");
			}

			while (tagStack.Count > 0) 
			{
				WordWithName word = tagStack.Pop ();
				errorList.Add(word.Line, word.Index, "タグが閉じられていません( "+word.Word+" )");
			}

			return adjustedWordList;
		}

		public List<RunCommand> ConvertCommandFormatToRunCommand(List<CommandFormat> commandList, out ErrorList errorList)
		{
			errorList = new ErrorList ();
			List<RunCommand> commandList_new = new List<RunCommand> ();

			for (int i=0; i<commandList.Count; i++) 
			{
				CommandFormat command = commandList [i];
				bool isCorrectCommand = command is RunCommand;
				if (isCorrectCommand) {
					commandList_new.Add (command as RunCommand);
				} else {
					errorList.Add (-1, -1, "内部エラーが発生しました。開発者に報告してください。(" + command.GetType() + ")");
				}
			}
			if (errorList.Count != 0)
				commandList_new = null;
			
			return commandList_new;
		}
	}
}