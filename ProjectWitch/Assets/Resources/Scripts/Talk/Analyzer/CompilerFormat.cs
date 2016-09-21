//==================================
//author	:shotta
//summary	:コンパイラ？
//==================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//参考にすっぞ
//http://qiita.com/KDKTN/items/a151d8d003a62c7b2ca3

using Scenario.Command;

namespace Scenario.Compiler
{

	//コンパイラ用のログ
	static public class CompilerLog
	{
		public static void Log(string message)
		{
			Debug.Log(message);
		}
		public static void Log(int line, int index, string message)
		{
			string log = "" + (line + 1) + "行 " + (index + 1) + "文字目: " + message + "";
			Debug.Log(log);
		}
		public static void Assert(string message)
		{
			Debug.Assert (false, message);
		}
		public static void Assert(int line, int index, string message)
		{
			string log = "" + (line + 1) + "行 " + (index + 1) + "文字目: " + message + "";
			Debug.Assert (false, log);
		}
	}
}