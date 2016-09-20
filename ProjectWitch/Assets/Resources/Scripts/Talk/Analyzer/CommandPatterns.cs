//=========================================================
//Author	:shotta
//Summary	:プログラムをパターン認識するためのパターン生成用クラス
//=========================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Scenario.Compiler;

namespace Scenario.Pattern
{
	//パターン用の抽象クラス(コンポーネント)
	abstract public class PatternFormat
	{
		//パターン適応結果
		public class Result
		{
			//マッチしているかどうか
			public bool Matched {get{ return mMatched; } }
			//値
			public object Value {get{ return mValue; } }
			//次のインデックス
			public int CurrIndex { get{ return mCurrIndex; } }

			private bool mMatched;
			private object mValue;
			private int  mCurrIndex;

			public Result(bool matched, object value, int currIndex)
			{
				mMatched	= matched;
				mValue		= value;
				mCurrIndex 	= currIndex;
			}
		}

		//要素を取得するデリゲート
		public delegate void GetResultDelegate(Result result);
		public GetResultDelegate GetResultMethod{ get; set; }
		protected void SendResultToDelegate(Result result)
		{
			if (GetResultMethod != null)
				GetResultMethod (result);
		}

		//パターンマッチングするメソッド
		//wordList	: 解析された字句
		//currIndex	: 今から解析するインデックス
		abstract public Result Match(WordWithName[] wordList, int currIndex);
	}

	//パターン(リーフ)
	//具体的なマッチングを行う
	public class Pattern_Object : PatternFormat
	{
		//隠蔽
		protected Pattern_Object(){}

		public delegate bool JudgeMatchingDelegate(WordWithName word);
		protected JudgeMatchingDelegate mJudgeMatchingMethod;
		//judgeMatchingMethod(word)	wordがパターンに当てはまっていればtrue
		//											 そうでなければfalseを返すデリゲート
		public Pattern_Object(JudgeMatchingDelegate judgeMatchingMethod)
		{
			mJudgeMatchingMethod = judgeMatchingMethod;
		}

		//パターンマッチングをするメソッド
		public override Result Match(WordWithName[] wordList, int currIndex){
			WordWithName word = wordList [currIndex];

			bool isMatched = mJudgeMatchingMethod (word);
			object value = word;
			if (isMatched) currIndex++;

			Result r = new Result (isMatched, value, currIndex);
			SendResultToDelegate (r);
			return r;
		}
	}

	//パターン(リーフ:コマンド生成)
	//コマンド生成用の抽象クラス
	abstract public class Pattern_CreateCommand : PatternFormat
	{
		abstract public override Result Match(WordWithName[] wordList, int currIndex);
	}

	//パターン(リーフ:ループ)
	//1種類のパターンが続いている間繰り返す
	public class Pattern_Loop : PatternFormat
	{
		//隠蔽
		protected Pattern_Loop(){}

		protected PatternFormat mPattern;
		protected int mMinCount;
		//pattern	検出するパターン
		//minCount	最小検出回数、これ以下だとエラーとする
		public Pattern_Loop(PatternFormat pattern, int minCount)
		{
			mPattern = pattern;
			mMinCount = minCount;
		}

		//パターンマッチングをするメソッド
		public override Result Match(WordWithName[] wordList, int currIndex)
		{
			List<object> matchedContentsList = new List<object> ();

			while(true){
				if (currIndex >= wordList.Length) break;

				//パターンマッチングをする
				Result result = mPattern.Match (wordList, currIndex);
				if (result.Matched) {
					matchedContentsList.Add (result.Value);
					currIndex = result.CurrIndex;
				} else {
					break;
				}
			}

			bool isMatched = matchedContentsList.Count >= mMinCount;
			Result r = new Result (isMatched, matchedContentsList, currIndex);
			SendResultToDelegate (r);
			return r;
		}
	}

	//パターンマッチング(コンポーネント)
	//複数のパターンとマッチングさせて適合したものを返す
	public class Pattern_Component : PatternFormat
	{
		//隠蔽
		protected Pattern_Component(){}

		protected List<PatternFormat> mPatternList;
		//patternList	ここで読むパターンのリスト
		public Pattern_Component(List<PatternFormat> patternList)
		{
			mPatternList = new List<PatternFormat> (patternList);
		}

		//マッチングをするメソッド
		public override Result Match(WordWithName[] wordList, int currIndex)
		{
			bool isMatched = false;
			object content = null;
			foreach (PatternFormat pattern in mPatternList)
			{
				//パターンマッチングをする
				Result result = pattern.Match (wordList, currIndex);
				if (result.Matched)
				{//マッチ時
					isMatched 	= true;

					content 	= result.Value;
					currIndex 	= result.CurrIndex;
					break;
				}
				else if (currIndex != result.CurrIndex)
				{//エラー時
					isMatched 	= false;
					content 	= result.Value;
					currIndex 	= result.CurrIndex;
					break;
				}
			}
			Result r = new Result (isMatched, content, currIndex);
			SendResultToDelegate (r);
			return r;
		}
	}

	//パターンマッチング(リスト)
	//複数のパターンを順番にマッチングして一連のパターンとして返す
	public class Pattern_List : PatternFormat
	{
		//隠蔽
		protected Pattern_List(){}

		protected List<PatternFormat> mPatternList;
		//patternList	ここで読むパターンのリスト
		public Pattern_List(List<PatternFormat> patternList)
		{
			mPatternList = new List<PatternFormat> (patternList);
		}

		//マッチングをするメソッド
		public override Result Match(WordWithName[] wordList, int currIndex)
		{
			List<object> contentsList = new List<object>();
			bool isMatched = true;
			for (int i = 0; i < mPatternList.Count; i++)
			{
				if (currIndex >= wordList.Length)
				{
					isMatched = false;
					break;
				}

				PatternFormat pattern = mPatternList [i];
				//パターンマッチングをする
				Result result = pattern.Match (wordList, currIndex);
				contentsList.Add (result.Value);
				currIndex = result.CurrIndex;
				if (!result.Matched) {
					isMatched = false;
					break;
				}
			}

			Result r = new Result (isMatched, contentsList, currIndex);
			SendResultToDelegate (r);
			return r;
		}
	}

}
