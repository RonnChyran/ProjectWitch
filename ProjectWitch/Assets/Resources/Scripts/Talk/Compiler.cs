//===================================
//author	:shotta
//summary	:コンパイラ
//===================================

using UnityEngine;
using System.Collections.Generic;
using System.IO; //System.IO.FileInfo, System.IO.StreamReader, System.IO.StreamWriter
using System; //Exception
using System.Text; //Encoding
using System.Text.RegularExpressions;

using Scenario;
using Scenario.Pattern;
using Scenario.Compiler;
using Scenario.Command;
using Scenario.WorkSpace;

namespace Scenario.Compiler
{
	public class ScriptCompiler
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
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_Number", "-?[0-9]+(?:\\.[0-9]+)?");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_String", "\"[^(?!\\\\)\"]+\"");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_KeyWord", "[a-zA-Z_][a-zA-Z0-9_]*");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_ArrayOperator", ",");
			//mWordAnalyzer.AddAnalyzeWordInfo ("CO_Operator", "[=\\+\\-\\*\\/<>&\\|],+\\(\\)");//式とか使う時はこれも
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_CalculationBlock", "\\(\\)");
			mWordAnalyzer.AddAnalyzeWordInfo ("CO_Space", "\\s+");
			mWordAnalyzer.AddRestWordInfo ("CO_OtherText");//残りの部分は全部テキストに

			//以下は仕様
			//Invalid_ は無効な表現を示す
			mWordAnalyzer.AddAnalyzeWordInfo ("Invalid_#", "#");
			mWordAnalyzer.AddAnalyzeWordInfo ("Invalid_*", "\\*");

			//ここで構文を作る
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateCommandsOfText ());
			patternList.Add (new CreateCommandsOfGraphics ());
			patternList.Add (new CreateCommandsOfSound ());
			patternList.Add (new CreateCommandsOfSystem ());
			patternList.Add (new CreateCommandsOfOther ());
			patternList.Add (new ErrorTagPattern ());

			PatternFormat pattern = new Pattern_Component (patternList);
			mStructureAnalyzer = new StructureAnalyzer(pattern);
		}

		//コンパイル
		public VirtualMachine CompileScript(string path)
		{
            //if (!File.Exists (path))
            //{
            //	CompilerLog.Log ("(" + path + ")ファイルが存在しません");
            //	return null;
            //}
            //FileInfo fi = new FileInfo(path);
            //StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.GetEncoding("UTF-8"));

            var tasset = Resources.Load(path) as TextAsset;            
            if (!tasset)
            {
            	CompilerLog.Log ("(" + path + ")ファイルが存在しません");
            	return null;
            }
            var reader = new StringReader(tasset.text);
			string text = reader.ReadToEnd ();

			//CRLF、CRをLFに変換
			text = new Regex ("\r\n|\r").Replace (text, "\n");
			//コメントを消去
			text = new Regex (";[^\n]*").Replace (text, "");

			//字句解析
			WordWithName[] wordList = mWordAnalyzer.Analyze (text);
			if (wordList == null) return null;
			//タグ内の余分な空白を除去
			WordWithName[] adjustedWordList = DeleteSpaceFromWordList (wordList);
			if (adjustedWordList == null) return null;
			//構文解析
			CommandFormat[] commandArray = mStructureAnalyzer.Analyze (adjustedWordList);
			if (commandArray == null) return null;
			//一部不完全なコマンドを完成させる
			CommandFormat[] adjustedCommandArray = LinkIfAndEndif (commandArray);
			if (adjustedCommandArray == null) return null;

			//仮想マシンにコマンドを登録
			VirtualMachine vm = VirtualMachine.Create(adjustedCommandArray);
			if (vm == null) return null;

			return vm;
		}

		//タグ内の余分な空白を除去(ついでにタグの[]についてのエラーも検出する)
		public WordWithName[] DeleteSpaceFromWordList(WordWithName[] wordList)
		{
			CompilerLog.Log ("無効文字除去及び対のタグを検出:");
			bool isValid = true;
			List<WordWithName> adjustedWordList = new List<WordWithName> ();
			Stack<WordWithName> tagStack = new Stack<WordWithName> ();

			for (int i = 0; i < wordList.Length; i++) {
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

				if (dependentTagEnd) {
					CompilerLog.Log (word.Line, word.Index, "タグが閉じられていません( " + word.Word + " )");
					isValid = false;
				}
			}

			while (tagStack.Count > 0) {
				WordWithName word = tagStack.Pop ();
				CompilerLog.Log (word.Line, word.Index, "タグが閉じられていません( " + word.Word + " )");
				isValid = false;
			}

			if (isValid) {
				CompilerLog.Log ("完了");
				return adjustedWordList.ToArray ();
			}

			CompilerLog.Log("失敗");
			return null;
		}

		//if文のリンク用のブロック
		private class IFBlockInfo
		{
			//これら2つの変数はif文のLinkingの処理で利用するためのもの
			public int NextIfIndex = -1;
			public int NextEndIfIndex = -1;
		}
		//if文、elif文、else文、endif文を関連付け
		public CommandFormat[] LinkIfAndEndif(CommandFormat[] commandArray)
		{
			CompilerLog.Log ("if文、elif文、else文、endif文を関連付け");

			CommandFormat[] adjustedCommandArray = new CommandFormat[commandArray.Length];
			Array.Copy (commandArray, 0, adjustedCommandArray, 0, commandArray.Length);

			Stack<IFBlockInfo> ibInfoStack = new Stack<IFBlockInfo> ();
			IFBlockInfo ibInfo_tmp = null;
			Dictionary<int, string> errorMessageDic = new Dictionary<int, string> ();

			for (int i = adjustedCommandArray.Length - 1; i >= 0; i--) {
				CommandFormat command = adjustedCommandArray [i];

				if (command is RunOrderCommand)
				{
					RunOrderCommand roCommand = command as RunOrderCommand;
					if (roCommand.orderCode == "endif")
					{
						IFBlockInfo ibInfo = new IFBlockInfo ();
						ibInfo.NextIfIndex = i + 1;
						ibInfo.NextEndIfIndex = i;
						ibInfoStack.Push (ibInfo);
						ibInfo_tmp = null;
					}
					if (roCommand.orderCode == "jump_endif")
					{
						if (ibInfo_tmp == null) {
							if (ibInfoStack.Count > 0)
								ibInfo_tmp = ibInfoStack.Pop ();
							else {
								errorMessageDic [i] = "対応するendifが見つかりません。";
								ibInfo_tmp = new IFBlockInfo ();
							}
						}
						IFBlockInfo ibInfo = ibInfo_tmp;
						ibInfo.NextIfIndex = i + 1;
						adjustedCommandArray [i - 1] = new SetArgumentCommand(ibInfo.NextEndIfIndex);
						ibInfoStack.Push (ibInfo);
						ibInfo_tmp = null;
					}
					if (roCommand.orderCode == "if")
					{
						IFBlockInfo ibInfo = null;
						if (ibInfoStack.Count > 0)
							ibInfo = ibInfoStack.Pop ();
						else {
							errorMessageDic [i] = "対応するendifが見つかりません。";
							continue;
						}
						adjustedCommandArray [i - 1] = new SetArgumentCommand(ibInfo.NextIfIndex);
						ibInfo_tmp = ibInfo;
					}
				}
			}

			if (ibInfoStack.Count > 0)
			{
				while (ibInfoStack.Count > 0)
				{
					IFBlockInfo ibInfo = ibInfoStack.Pop ();
					errorMessageDic [ibInfo.NextIfIndex] = "対応するifが見つかりません。";
				}
			}

			if (errorMessageDic.Count == 0)
			{
				CompilerLog.Log ("完了");
				return adjustedCommandArray;
			}
			else
			{
				int line = 0;
				int index = 0;
				for (int i = 0; i < commandArray.Length; i++)
				{
					string error;
					CommandFormat command = commandArray [i];
					if (command is RunOrderCommand)
					{
						RunOrderCommand roCommand = command as RunOrderCommand;
						if (roCommand.orderCode == "referFrom")
						{
							line	= (int)(commandArray [i - 2] as SetArgumentCommand).Value;
							index	= (int)(commandArray [i - 1] as SetArgumentCommand).Value;
						}
					}
					if (errorMessageDic.TryGetValue (i, out error))
						CompilerLog.Log (line, index, error);
				}
				CompilerLog.Log ("失敗");
			}

			return null;
		}
	}

	//未検出のタグをエラーとして検出
	public class ErrorTagPattern : Pattern_CreateCommand
	{
		public override Result Match (WordWithName[] wordList, int currIndex)
		{
			int line;
			int index;
			{
				WordWithName word = wordList [currIndex];
				line = word.Line;
				index = word.Index;
			}
			
			string tagName = "";
			//「[」を読む
			PatternFormat patternTagBegin = new Pattern_Object (delegate(WordWithName word) {
				return word.Name == "ID_TagBegin";
			});
			//タグの名前を読む
			PatternFormat patternTagName = new Pattern_Object (delegate(WordWithName word) {
				tagName = word.Word;
				return true;
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

			if (result.Matched) {
				CompilerLog.Log (line, index, "無効なタグ名です(" + tagName + ")");
				currIndex = result.CurrIndex;
			}

			return new Result(false, null, currIndex);
		}
	}
}