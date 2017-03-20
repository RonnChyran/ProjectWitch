using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileManager
{
	public enum FilePos_e
	{
		Common = 0,
		FromResources,
		FromStreaming,
		FromPersistent,
	}

	public class UniINIManager : INIManager
	{
		private FilePos_e mFilePos;

		public UniINIManager() : base() { mFilePos = FilePos_e.Common; }

		public UniINIManager(string _filePath, FilePos_e _pos) : base()
		{
			Open(_filePath, _pos);
		}
		public UniINIManager(string _filePath, FilePos_e _pos, Encoding _enc) : base()
		{
			Open(_filePath, _pos, _enc);
		}

		[System.Obsolete("使用不可です", true)]
		private new bool Open(string _filePath) { return false; }
		[System.Obsolete("使用不可です", true)]
		private new bool Open(string _filePath, Encoding _enc) { return false; }

		// 読み込み
		public bool Open(string _filePath, FilePos_e _pos)
		{
			return Open(_filePath, _pos, null);
		}
		public bool Open(string _filePath, FilePos_e _pos, Encoding _enc)
		{
			IsFileOpen = false;
			mFilePos = _pos;
			List<string> baseTexts = new List<string>();
			switch (_pos)
			{
				case FilePos_e.Common:
					baseTexts = new List<string>((_enc == null ? File.ReadAllLines(_filePath) : File.ReadAllLines(_filePath, _enc)));
					break;
				case FilePos_e.FromResources:
					var tAsset = Resources.Load(_filePath) as TextAsset;
					if (_enc != null)
						Debug.LogAssertion("エンコード形式は意味がありません");
					if (!tAsset)
					{
						Debug.Assert(false, "データファイルの読み込みに失敗しました : " + _filePath);
						return false;
					}
					using(var reader = new StringReader(tAsset.text))
					{
						while (reader.Peek() > -1)
							baseTexts.Add(reader.ReadLine());
					}
					break;
				case FilePos_e.FromStreaming:
					if (_enc != null)
						Debug.LogAssertion("エンコード形式は意味がありません");
					WWW www = new WWW(Path.Combine(Application.streamingAssetsPath, _filePath));
					while (!www.isDone) { }
					using (var reader = new StringReader(www.text))
					{
						while (reader.Peek() > -1)
							baseTexts.Add(reader.ReadLine());
					}
					baseTexts = new List<string>(File.ReadAllLines(_filePath));
					break;
				case FilePos_e.FromPersistent:
					var path = Path.Combine(Application.persistentDataPath, _filePath);
					baseTexts = new List<string>((_enc == null ? File.ReadAllLines(path) : File.ReadAllLines(path, _enc)));
					break;
				default:
					break;
			}
			if (baseTexts.Count == 0)
				return false;
			Analysis(baseTexts);
			FilePath = _filePath;
			IsFileOpen = true;
			return true;
		}

		// 書き込み 
		public override bool Write()
		{
			return Write(FilePath, mFilePos);
		}

		public override bool Write(Encoding _enc)
		{
			return Write(FilePath, mFilePos, _enc);
		}

		[System.Obsolete("使用不可です", true)]
		private new bool Write(string _filePath) { return false; }
		[System.Obsolete("使用不可です", true)]
		private new bool Write(string _filePath, Encoding _enc) { return false; }

		// 別名書き込み 
		public bool Write(string _filePath, FilePos_e _pos)
		{
			if (_pos == FilePos_e.FromResources || _pos == FilePos_e.FromStreaming)
			{
				Debug.Assert(false, "この場所には書き込めません");
				return false;
			}
			var path = (_pos == FilePos_e.Common ? _filePath : Path.Combine(Application.persistentDataPath, _filePath));
			using (var writer = new StreamWriter(path))
			{
				foreach (var sec in mData.Keys)
				{
					foreach (var str in mComments[sec])
						writer.WriteLine("# " + str);
					writer.WriteLine("[" + sec + "]");
					foreach (var par in mData[sec].Keys)
					{
						foreach (var str in mComments[GetComParKey(sec, par)])
							writer.WriteLine("# " + str);
						writer.WriteLine(par + "=" + mData[sec][par]);
					}
					writer.WriteLine();
				}
			}
			return true;
		}
		public bool Write(string _filePath, FilePos_e _pos, Encoding _enc)
		{
			if (_pos == FilePos_e.FromResources || _pos == FilePos_e.FromStreaming)
			{
				Debug.Assert(false, "この場所には書き込めません");
				return false;
			}
			var path = (_pos == FilePos_e.Common ? _filePath : Path.Combine(Application.persistentDataPath, _filePath));
			using (var writer = (_enc == null ? new StreamWriter(path) : new StreamWriter(path, false, _enc)))
			{
				foreach (var sec in mData.Keys)
				{
					foreach (var str in mComments[sec])
						writer.WriteLine("# " + str);
					writer.WriteLine("[" + sec + "]");
					foreach (var par in mData[sec].Keys)
					{
						foreach (var str in mComments[GetComParKey(sec, par)])
							writer.WriteLine("# " + str);
						writer.WriteLine(par + "=" + mData[sec][par]);
					}
					writer.WriteLine();
				}
			}
			return true;
		}

		// CSVファイルへ変換する、空欄がセクション名、その他がパラメータ名
		public UniCSVManager TransformToUniCSV(List<string> _keys, string _secName)
		{
			return TransformToUniCSV(_keys, _secName, null);
		}
		public UniCSVManager TransformToUniCSV(List<string> _keys, string _secName, IComparer<List<string>> _comparer)
		{
			UniCSVManager csv = new UniCSVManager();
			csv.Column = _keys.Count;
			for (int i = 0; i < _keys.Count; i++)
			{
				if (_keys[i] == "")
					csv[0, i] = _secName;
				else
					csv[0, i] = _keys[i];
			}
			foreach (var sec in mData)
			{
				List<string> rowLine = new List<string>();
				foreach (var key in _keys)
				{
					if (key == "")
						rowLine.Add(sec.Key);
					else if (sec.Value.ContainsKey(key))
						rowLine.Add(sec.Value[key]);
					else
						rowLine.Add("");
				}
				csv.AddRow(rowLine);
			}
			if (_comparer != null)
				csv.Sort(1, csv.Row - 1, _comparer);
			return csv;
		}

	}

	public class UniCSVManager : CSVManager
	{
		private FilePos_e mFilePos;

		public UniCSVManager() : base() { mFilePos = FilePos_e.Common; }

		public UniCSVManager(string _filePath, FilePos_e _pos) : base() { Open(_filePath, _pos); }
		public UniCSVManager(string _filePath, FilePos_e _pos, Encoding _enc) : base() { Open(_filePath, _pos, _enc); }

		[System.Obsolete("使用不可です", true)]
		private new bool Open(string _filePath) { return false; }
		[System.Obsolete("使用不可です", true)]
		private new bool Open(string _filePath, Encoding _enc) { return false; }

		// 読み込み
		public bool Open(string _filePath, FilePos_e _pos)
		{
			return Open(_filePath, _pos, null);
		}
		public bool Open(string _filePath, FilePos_e _pos, Encoding _enc)
		{
			IsFileOpen = false;
			mFilePos = _pos;
			List<string> baseTexts = new List<string>();
			switch (_pos)
			{
				case FilePos_e.Common:
					baseTexts = new List<string>((_enc == null ? File.ReadAllLines(_filePath) : File.ReadAllLines(_filePath, _enc)));
					break;
				case FilePos_e.FromResources:
					var tAsset = Resources.Load(_filePath) as TextAsset;
					if (_enc != null)
						Debug.LogAssertion("エンコード形式は意味がありません");
					if (!tAsset)
					{
						Debug.Assert(false, "データファイルの読み込みに失敗しました : " + _filePath);
						return false;
					}
					using (var reader = new StringReader(tAsset.text))
					{
						while (reader.Peek() > -1)
							baseTexts.Add(reader.ReadLine());
					}
					break;
				case FilePos_e.FromStreaming:
					if (_enc != null)
						Debug.LogAssertion("エンコード形式は意味がありません");
					WWW www = new WWW(Path.Combine(Application.streamingAssetsPath, _filePath));
					while (!www.isDone) { }
					using (var reader = new StringReader(www.text))
					{
						while (reader.Peek() > -1)
							baseTexts.Add(reader.ReadLine());
					}
					baseTexts = new List<string>(File.ReadAllLines(_filePath));
					break;
				case FilePos_e.FromPersistent:
					var path = Path.Combine(Application.persistentDataPath, _filePath);
					baseTexts = new List<string>((_enc == null ? File.ReadAllLines(path) : File.ReadAllLines(path, _enc)));
					break;
				default:
					break;
			}
			if (baseTexts.Count == 0)
				return false;
			Analysis(baseTexts);
			FilePath = _filePath;
			IsFileOpen = true;
			return true;
		}

		// 書き込み
		public override bool Write() { return Write(FilePath, mFilePos); }
		public override bool Write(Encoding _enc) { return Write(FilePath, mFilePos, _enc); }

		[System.Obsolete("使用不可です", true)]
		private new bool Write(string _filePath) { return false; }
		[System.Obsolete("使用不可です", true)]
		private new bool Write(string _filePath, Encoding _enc) { return false; }

		// 別名書き込み
		public bool Write(string _filePath, FilePos_e _pos) { return Write(_filePath, _pos, null); }
		public bool Write(string _filePath, FilePos_e _pos, Encoding _enc)
		{
			if (_pos == FilePos_e.FromResources || _pos == FilePos_e.FromStreaming)
			{
				Debug.Assert(false, "この場所には書き込めません");
				return false;
			}
			var path = (_pos == FilePos_e.Common ? _filePath : Path.Combine(Application.persistentDataPath, _filePath));
			using (var writer = (_enc == null ? new StreamWriter(path) : new StreamWriter(path, false, _enc)))
			{
				foreach (var strList in mBaseTexts)
				{
					string line = "";
					bool isFirstLine = true;
					foreach (var str in strList)
					{
						if (!isFirstLine)
							line += ",";
						if (str.Contains(","))
							line += "" + str + "";
						else
							line += str;
						if (isFirstLine)
							isFirstLine = false;
					}
					writer.WriteLine(line);
				}
			}
			return true;
		}
	}
}