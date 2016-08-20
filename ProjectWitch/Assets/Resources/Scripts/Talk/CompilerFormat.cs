//==================================
//author	:shotta
//summary	:コンパイラ？
//==================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//参考にすっぞ
//http://qiita.com/KDKTN/items/a151d8d003a62c7b2ca3

using Script.Analyzer;
using Script.Command;

namespace Script.Compiler
{
	public class CompilerFormat
	{
	}
	//エラーのコンテンツ
	public class ErrorInfo
	{
		public int Line{ get{ return mLine; } }
		public int Index{ get{ return mIndex; } }
		public string Message{ get{ return mMessage; } }

		private int mLine = 0;
		private int mIndex = 0;
		private string mMessage = "";
		private ErrorInfo(){}
		public ErrorInfo(int line, int index, string message)
		{
			mLine = line;
			mIndex = index;
			mMessage = message;
		}
		public new string ToString ()
		{
			int line 		= mLine;
			int index 		= mIndex;
			string message 	= mMessage;
			return "" + (line + 1) + "行 " + (index + 1) + "文字目: " + message + "";
		}
	}
	//エラー処理
	public class ErrorList{
		private List<ErrorInfo> mErrorList;
		public ErrorList()
		{
			mErrorList = new List<ErrorInfo>();
		}
		//エラーリストを出力
		public string ErrorMessage{
			get{
				//できたエラーリストをまとめて、返す
				string errorMessage = "";
				foreach (ErrorInfo oneError in mErrorList) 
				{
					errorMessage += "" + oneError.ToString() + "\n";
				}
				return errorMessage;
			}
		}
		//エラー件数を取得
		public int Count{get{return mErrorList.Count;}}
		//エラー時のメッセージを作成
		public void Add(int line, int index, string message)
		{
			ErrorInfo errorInfo = new ErrorInfo(line, index, message);
			mErrorList.Add (errorInfo);
		}
		//エラー時のメッセージを作成
		public void Add(ErrorInfo errorInfo)
		{
			if (errorInfo != null)
				mErrorList.Add (errorInfo);
		}
	}
}

