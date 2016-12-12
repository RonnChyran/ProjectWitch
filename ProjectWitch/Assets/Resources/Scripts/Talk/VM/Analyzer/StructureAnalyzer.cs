//=============================
//author	:shotta
//summary	:構文解析器
//=============================

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;
using ProjectWitch.Talk.Compiler;


namespace ProjectWitch.Talk.Compiler
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
		public CommandFormat[] Analyze(WordWithName[] wordList)
		{
			if (wordList == null)
				return null;
			
			CompilerLog.Log ("構文解析:");
			bool isValid = true;
			List<CommandFormat> commandList = new List<CommandFormat> ();

			int currIndex = 0;
			int count = wordList.Length;
			while (currIndex < count) {
				//パターンマッチ部分
				PatternFormat.Result result = mPattern.Match (wordList, currIndex);
				if (currIndex > result.CurrIndex) {
					CompilerLog.Assert ("コンパイラエラーです、製作者に報告してください。" +
					"(" + wordList [currIndex].Line + "行目," + wordList [currIndex].Index + "文字目)");
					break;
				}

				CommandFormat[] infoCommand = CreateReferFromCommand.Create (wordList[currIndex].Line, wordList[currIndex].Index);
				commandList.AddRange (infoCommand);
				if (result.Matched) {//成功
					CommandFormat[] oneCommandArray = result.Value as CommandFormat[];
					commandList.AddRange (oneCommandArray);
				} else {//失敗
					WordWithName word = wordList [currIndex];
					bool isSpaceChar = new Regex ("\\s+").Match (word.Word).Success;
					bool isInvolveError = currIndex != result.CurrIndex;

					//パターンにはまらなかった文字にエラーをつける
					if (!isInvolveError && !isSpaceChar)
						CompilerLog.Log (word.Line, word.Index, "無効な言葉です(" + word.Word + ")");

					//エラーがあったらコンパイル失敗にする
					if (isInvolveError || !isSpaceChar)
						isValid = false;
				}

				//現在位置を動かす
				if (currIndex != result.CurrIndex)
					currIndex = result.CurrIndex;
				else
					currIndex++;
			}

			commandList.InsertRange (0, CreateNotificationCommand.Create ("scriptBegin"));
			commandList.AddRange (CreateNotificationCommand.Create ("scriptEnd"));

			if (isValid)
			{
				CompilerLog.Log ("完了");
				CommandFormat[] commandArray = commandList.ToArray();
				return commandArray;
			}

			CompilerLog.Log ("失敗");
			return null;
		}

	}

	public class CommandList
	{
		private List<CommandFormat> mCommandList = new List<CommandFormat> ();
		//コマンドを追加
		public void Add(CommandFormat command)
		{
			mCommandList.Add (command);
		}
		//複数コマンドを追加
		public void Add(IEnumerable<CommandFormat> commands)
		{
			mCommandList.AddRange (commands);
		}

		//指定インデックスのコマンドを取得
		public CommandFormat Get(int index)
		{
			return mCommandList [index];
		}
		//配列内のコマンドの数
		public int Count
		{
			get { return mCommandList.Count; }
		}

		//コマンドの配列を取得
		public CommandFormat[] GetArray()
		{
			return mCommandList.ToArray ();
		}
	}

	/*
	//データ整列用の基本クラス
	abstract public class DataElement
	{
		abstract public DataElement[] GetArray ();
	}

	//データの配列
	public class DataContentsList : DataElement
	{
		private List<DataElement> mDataList = new List<DataElement> ();
		//データを追加
		public void Add (DataElement data)
		{
			mDataList.Add (data);
		}
		//データを挿入
		public void InsertAt (int index, DataElement data)
		{
			mDataList.Insert (index, data);
		}
		//データを置換
		public void ReplaceAt (int index, DataElement data)
		{
			mDataList.RemoveAt (index);
			mDataList.Insert (index, data);
		}
		//データを削除
		public void RemoveAt (int index)
		{
			mDataList.RemoveAt (index);
		}

		//データを取得
		public DataElement GetAt (int index)
		{
			return mDataList [index];
		}
		public int Count{
			get { return mDataList.Count; } 
		}

		//配列を取得
		public override DataElement[] GetArray ()
		{
			List<DataElement> dataArray = new List<DataElement> ();
			for (int i = 0; i < mDataList.Count; i++)
			{
				dataArray.AddRange (mDataList [i].GetArray ());
			}
			return dataArray.ToArray ();
		}
	}

	//データの具体的なコンテンツ
	public class DataContents : DataElement
	{
		private object mContents;

		private DataContents(){}
		public DataContents(object contents)
		{
			mContents = contents;
		}

		public override DataElement[] GetArray ()
		{
			DataElement[] dataArray = new DataElement[1];
			dataArray [0] = this;
			return dataArray;
		}
	}
	*/
}

