//==================================
//author	:shotta
//summary	:字句解析器
//==================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ProjectWitch.Talk.Compiler;

namespace ProjectWitch.Talk.Compiler{
	
	//識別された字句の格納用
	public class WordWithName{
		public int Index{ get{ return mIndex; } }
		public int Line{ get{ return mLine; } }
		public string Name{ get{ return mName; } }
		public string Word{ get{ return mWord; } }

		private int mIndex;
		private int mLine;
		private string mName;
		private string mWord;
		public WordWithName(int index, int line, string name, string word)
		{
			mIndex = index;
			mLine = line;
			mName = name;
			mWord = word;
		}
	}

	//字句解析器
	//※ここでは単語を分割するのみ、複雑な構文は取り入れないよう徹する
	public class WordAnalyzer
	{
		private struct RegexWithName{
			//字句の分類名
			public string Name;
			//字句を検索するための正規表現
			public Regex RegulerExpression;
		}
		private string mRestName;
		private List<RegexWithName> mRwNList;

		public WordAnalyzer()
		{
			mRestName = null;
			mRwNList = new List<RegexWithName> ();
		}
			
		//検索字句と分類名を登録
		//これは追加したのが早いものほど優先度が高くなる
		public void AddAnalyzeWordInfo(string name, string regulerExpression)
		{
			RegexWithName rwn = new RegexWithName ();
			rwn.Name = name;
			rwn.RegulerExpression = new Regex(regulerExpression);
			mRwNList.Add (rwn);
		}
		//検索しきれなかった字句の分類名を登録
		public void AddRestWordInfo(string name)
		{
			mRestName = name;
		}

		//字句解析をする
		public WordWithName[] Analyze(string text)
		{
			CompilerLog.Log ("字句解析:");

			if (text != null) 
			{
				bool isValid = true;
				List<string> lines = new List<string> ();
				{//行を抽出
					Regex r = new Regex ("[^\n]*\n?");
					int currIndex = 0;
					int textLength = text.Length;
					while (currIndex < textLength) {
						Match m = r.Match (text, currIndex);
						lines.Add (m.Value);
						currIndex = m.Index + m.Length;
					}
				}

				List<WordWithName> wordList = new List<WordWithName> ();

				{//行ごとに字句解析を実施、前方から順に読む
					for (int currLine = 0; currLine < lines.Count; currLine++) {
						string textLine = lines [currLine];
						int currIndex = 0;
						int lineLength = textLine.Length;
						while (currIndex < lineLength) {
							bool oneIsValid;
							WordWithName word = AnalyzeWord (textLine, currIndex, currLine, out oneIsValid);
							wordList.Add (word);
							currIndex = word.Index + word.Word.Length;
							isValid &= oneIsValid;
						}
					}
				}

				if (isValid) {
					CompilerLog.Log ("完了");
					return wordList.ToArray ();
				}
			}
			CompilerLog.Log ("失敗");
			return null;
		}

		//実際に字句解析をしてる部分
		private WordWithName AnalyzeWord(string text, int currIndex, int currLine, out bool isValid)
		{
			isValid = true;
			//字句解析部
			//	登録された正規表現を用いて、一致箇所を検索
			//	最も近いものが今回の解析結果となる
			WordWithName word = new WordWithName(text.Length, currLine, "", "");
			for (int i = 0; i < mRwNList.Count; i++)
			{
				RegexWithName rwn = mRwNList [i];
				Regex r = rwn.RegulerExpression;
				Match m = r.Match (text, currIndex);
				if (word.Index > m.Index && m.Success && m.Length!=0)
					word = new WordWithName (m.Index, currLine, rwn.Name, m.Value);
			}

			//余りの分類名
			string restName = mRestName;
			if(restName!=null){
				//もし余りがあるなら(検索開始位置と検索箇所の位置の最小にズレがあるなら)それを余りとする
				if (word.Index > currIndex) 
				{
					string str = text.Substring (currIndex, word.Index - currIndex);
					word = new WordWithName (currIndex, currLine, restName, str);
				}
			}
			//不可記号の処理
			if (new Regex ("^Invalid_").Match (word.Name).Success) {
				CompilerLog.Log (word.Line ,word.Index ,"無効な文字列を検出しました(" + word.Word + ")。" );
				isValid = false;
			}
			return word;
		}
	}

}