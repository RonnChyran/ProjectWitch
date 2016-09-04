//=============================
//author	:shotta
//summary	:構文解析器
//=============================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Script.Command;
using Script.Compiler;

namespace Script.Analyzer
{
	//構文解析器
	public class StructureAnalyzer
	{
		private PatternFormat mPattern;
		public StructureAnalyzer (PatternFormat pattern)
		{
			mPattern = pattern;
		}

		//構文解析をする
		public List<CommandFormat> Analyze(List<WordWithName> wordList, out ErrorList error)
		{
			ErrorList errorList = new ErrorList ();
			List<CommandFormat> commandList = new List<CommandFormat> ();

			int currIndex = 0;
			int count = wordList.Count;
			while(currIndex<count)
			{
				//パターンマッチ部分
				PatternFormat.Result result = mPattern.Match(wordList, currIndex);

				if (result.Value != null)
				{//成功
					commandList.Add (result.Value as CommandFormat);
				}
				else
				{//失敗
					ErrorInfo oneErrorInfo = result.Error;
					WordWithName word = wordList [currIndex];
					if (result.Error == null){
						if (!(new Regex ("\\s+").Match (word.Word).Success)) {
							oneErrorInfo = new ErrorInfo(word.Line, word.Index, "無効な言葉です(" + word.Word + ")");
						}
					}
					if (oneErrorInfo != null)
						errorList.Add (oneErrorInfo);
				}
				//現在位置を動かす
				if (currIndex != result.CurrIndex)
					currIndex = result.CurrIndex;
				else
					currIndex++;//基本でないはずだけど安全防止策
			}

			//エラーメッセージを作成
			if (errorList.Count != 0)
				commandList = null;
			error = errorList;

			return commandList;
		}
	}
}

