//=========================================================
//Author	:shotta
//Summary	:プログラムをパターン認識するためのパターン生成用クラス
//=========================================================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Script.Compiler;
using Script.Command;

namespace Script.Command
{
	//コマンドの抽象クラス
	abstract public class CommandFormat
	{
	}
	//自立型のコマンド
	abstract public class RunCommand : CommandFormat
	{
		//コマンドを初期化
		abstract public void Setup ();
		//コマンドを実行(引数、返り値なし)
		abstract public void Run();
		//コマンドを強制的に終わらせる
		abstract public void Finish ();
	}
	//値を返すタイプのコマンド
	abstract public class OutputCommand : CommandFormat
	{
		//コマンドを実行(引数なし、返り値あり)
		abstract public object Get();
		//Stringを取得
		public string GetString(){
			object value = this.Get ();
			if (value is string) {
				return value as string;
			}else{
				Debug.Assert(true, "「" + value + "」はStringに変換できません");
				return null;
			}
		}
		//Int32を取得
		public Int32 GetInt32(){
			object value = this.Get ();
			if (value is IConvertible) {
				return ((IConvertible)value).ToInt32 (null);
			}else{
				Debug.Assert(true, "「" + value + "」はInt32に変換できません");
				return 0;
			}
		}
		//Floatを取得
		public float GetFloat(){
			object value = this.Get ();
			if (value is IConvertible) {
				return (float)((IConvertible)value).ToDouble (null);
			}else{
				Debug.Assert(true, "「" + value + "」はFloatに変換できません");
				return 0;
			}
		}
	}
	//値を入出力するタイプのコマンド
	abstract public class IOCommand : OutputCommand
	{
		//コマンドを実行(引数あり、返り値なし)
		abstract public void Set(object value);
	}
}

namespace Script.Analyzer
{
	//パターン用の抽象クラス(コンポーネント)
	abstract public class PatternFormat
	{
		//パターン適応結果
		public class Result
		{
			//マッチしているかどうか
			public bool Matched {get{ return mValue != null; } }
			//値
			public object Value {get{ return mValue; } }
			//次のインデックス
			public int CurrIndex { get{ return mCurrIndex; } }
			//エラー
			public ErrorInfo Error { get{ return mError; } }

			private object mValue;
			private int  mCurrIndex;
			private ErrorInfo mError;
			//どっちでも使える
			public Result(object value, int currIndex, ErrorInfo error)
			{
				SetArgs(value, currIndex, error);
			}
			private void SetArgs(object value, int currIndex, ErrorInfo error)
			{
				if (error != null)
					value = null;
				mValue		= value;
				mCurrIndex 	= currIndex;
				mError 		= error;
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
		//finished	: 解析できるものがない時 true
		abstract public Result Match(List<WordWithName> wordList, int currIndex);
	}
	//パターン(リーフ)
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
		public override Result Match(List<WordWithName> wordList, int currIndex){
			object value = null;
			WordWithName word = wordList [currIndex];
			if (mJudgeMatchingMethod (word)) {
				value = word;
			}
			Result r = new Result (value, currIndex + 1, null);
			SendResultToDelegate (r);
			return r;
		}
	}

	//パターン(リーフ:コマンド生成)
	abstract public class Pattern_CreateCommand : PatternFormat
	{
		abstract public override Result Match(List<WordWithName> wordList, int currIndex);
	}

	//パターン(リーフ:ループ)
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
		public override Result Match(List<WordWithName> wordList, int currIndex)
		{
			ErrorInfo errorInfo = null;
			List<object> matchedContentsList = new List<object> ();

			while(true){
				if (currIndex >= wordList.Count) break;

				//パターンマッチングをする
				Result result = mPattern.Match (wordList, currIndex);
				if (result.Matched) {
					matchedContentsList.Add (result.Value);
					currIndex = result.CurrIndex;
				} else {
					errorInfo = result.Error;
					break;
				}
			}

			if (matchedContentsList.Count < mMinCount) 
			{
				matchedContentsList = null;
			}
			Result r = new Result (matchedContentsList, currIndex, errorInfo);
			SendResultToDelegate (r);
			return r;
		}
	}
	//パターンマッチング(コンポーネント)
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
		public override Result Match(List<WordWithName> wordList, int currIndex)
		{
			ErrorInfo errorInfo = null;
			object matchedContent = null;
			foreach (PatternFormat pattern in mPatternList)
			{
				//パターンマッチングをする
				Result result = pattern.Match (wordList, currIndex);
				if (result.Matched) {
					matchedContent = result.Value;
					currIndex = result.CurrIndex;
					break;
				} else if (result.Error != null){
					errorInfo = result.Error;
					currIndex = result.CurrIndex;
					break;
				}
			}
			Result r = new Result (matchedContent, currIndex, errorInfo);
			SendResultToDelegate (r);
			return r;
		}
	}
	//パターンマッチング(リスト)
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
		public override Result Match(List<WordWithName> wordList, int currIndex)
		{
			ErrorInfo errorInfo = null;
			List<object> matchedContentsList = new List<object>();
			for (int i = 0; i < mPatternList.Count; i++)
			{
				if (currIndex >= wordList.Count)
					break;

				PatternFormat pattern = mPatternList [i];
				//パターンマッチングをする
				Result result = pattern.Match (wordList, currIndex);
				if (result.Matched) {
					matchedContentsList.Add (result.Value);
					currIndex = result.CurrIndex;
				} else {
					errorInfo = result.Error;
					break;
				}
			}
			if (matchedContentsList.Count != mPatternList.Count)
				matchedContentsList = null;
			Result r = new Result (matchedContentsList, currIndex, errorInfo);
			SendResultToDelegate (r);
			return r;
		}
	}

}
