using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FileManager
{
	public class INIManager
	{
		// 読み込んだデータ
		protected Dictionary<string, Dictionary<string, string>> mData;
		// コメント
		protected Dictionary<string, List<string>> mComments;
		// ファイルパス名
		public string FilePath { get; protected set; }
		// ファイル読み出しが成功したかどうか
		public bool IsFileOpen { get; protected set; }
		// セクション一覧
		public List<string> SecKeys { get { return new List<string>(mData.Keys); } }

		// コンストラクタ
		public INIManager()
		{
			IsFileOpen = false;
			mData = new Dictionary<string, Dictionary<string, string>>();
			mComments = new Dictionary<string, List<string>>();
		}

		// ファイルオープンコンストラクタ
		public INIManager(string _filePath) : this()
		{
			Open(_filePath);
		}
		public INIManager(string _filePath, Encoding _enc) : this()
		{
			Open(_filePath, _enc);
		}

		// コピーコンストラクタ
		public INIManager(INIManager _ini)
		{
			FilePath = _ini.FilePath;
			IsFileOpen = _ini.IsFileOpen;
			mData = new Dictionary<string, Dictionary<string, string>>();
			foreach (var dic in _ini.mData)
				mData.Add(dic.Key, new Dictionary<string, string>(dic.Value));
			mComments = new Dictionary<string, List<string>>();
			foreach (var dic in _ini.mComments)
				mComments.Add(dic.Key, new List<string>(dic.Value));
		}

		// csv変換コンストラクタ
		public INIManager(CSVManager _csv, int _secColumn) : this()
		{
			TransformFromCSV(_csv, _secColumn);
		}

		// データクリア
		public void Clear()
		{
			mData.Clear();
			mComments.Clear();
		}

		// パラメータコメントKey取得
		protected string GetComParKey(string _sec, string _par)
		{
			return (_sec + "." + _par);
		}

		// 読み込み
		public bool Open(string _filePath)
		{
			return Open(_filePath, null);
		}
		public bool Open(string _filePath, Encoding _enc)
		{
			IsFileOpen = false;
			List<string> baseTexts = new List<string>(_enc == null ? File.ReadAllLines(_filePath) : File.ReadAllLines(_filePath, _enc));
			if (baseTexts.Count == 0)
				return false;
			Analysis(baseTexts);
			FilePath = _filePath;
			IsFileOpen = true;
			return true;
		}

		// 文章分析
		protected void Analysis(List<string> _texts)
		{
			Clear();
			string sec = null;
			string par = null;
			List<string> comments = new List<string>();
			foreach (var str in _texts)
			{
				if (string.IsNullOrEmpty(str))
					continue;
				// コメント文の場合
				if (str[0] == '#' || str[0] == ';')
				{
					string buff = str.Remove(0, 1);
					while (buff[0] == ' ')
						buff = buff.Remove(0, 1);
					comments.Add(buff);
					continue;
				}
				// セクション文の場合
				if (str[0] == '[' && str[str.Length - 1] == ']')
				{
					sec = str.Replace("[", "").Replace("]", "");
					mData.Add(sec, new Dictionary<string, string>());
					mComments.Add(sec, new List<string>(comments));
					comments = new List<string>();
					continue;
				}
				// パラメータ文の場合
				if (!string.IsNullOrEmpty(sec) && str.Contains("="))
				{
					int num = str.IndexOf("=");
					par = str.Remove(num);
					mData[sec].Add(par, str.Substring(num + 1));
					mComments.Add(GetComParKey(sec, par), new List<string>(comments));
					comments = new List<string>();
					continue;
				}
			}
		}

		// CSVファイルから変換する
		public void TransformFromCSV(CSVManager _csv, int _secColumn)
		{
			Clear();
			int column = _csv.Column;
			if (_secColumn >= column)
				_secColumn = 0;
			for (int i = 1; i < _csv.Row; i++)
			{
				if (_csv[i, _secColumn] == "")
					continue;
				Dictionary<string, string> dic = new Dictionary<string, string>();
				mComments.Add(_csv[i, _secColumn], new List<string>());
				for (int j = 0; j < column; j++)
				{
					if (_csv[0, j] == "")
						continue;
					dic.Add(_csv[0, j], _csv[i, j]);
					mComments.Add(GetComParKey(_csv[i, _secColumn], _csv[0, j]), new List<string>());
				}
				mData.Add(_csv[i, _secColumn], dic);
			}
		}

		// CSVファイルへ変換する、空欄がセクション名、その他がパラメータ名
		public CSVManager TransformToCSV(List<string> _keys, string _secName)
		{
			return TransformToCSV(_keys, _secName, null);
		}
		public CSVManager TransformToCSV(List<string> _keys, string _secName, IComparer<List<string>> _comparer)
		{
			CSVManager csv = new CSVManager();
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

		// 書き込み
		public virtual bool Write()
		{
			return Write(FilePath);
		}
		public virtual bool Write(Encoding _enc)
		{
			return Write(FilePath, _enc);
		}

		// 別名書き込み
		public bool Write(string _filePath)
		{
			return Write(_filePath, null);
		}
		public bool Write(string _filePath, Encoding _enc)
		{
			// ファイルにテキストを書き出し
			using (var writer = (_enc == null ? new StreamWriter(_filePath) : new StreamWriter(_filePath, false, _enc)))
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

		// セクション削除
		public bool Remove(string _sec)
		{
			if (string.IsNullOrEmpty(_sec))
				throw new ArgumentNullException();
			mComments.Remove(_sec);
			return mData.Remove(_sec);
		}

		// パラメータ削除
		public bool Remove(string _sec, string _par)
		{
			if (string.IsNullOrEmpty(_sec))
				throw new ArgumentNullException();
			if (mData.ContainsKey(_sec))
			{
				mComments.Remove(GetComParKey(_sec, _par));
				return mData[_sec].Remove(_par);
			}
			else
				return false;
		}

		// セクションにコメント文を付加
		public void AddComment(string _comment, string _sec)
		{
			// Keyがnullの場合の例外
			if (string.IsNullOrEmpty(_sec))
				throw new ArgumentNullException();
			if (mComments.ContainsKey(_sec))
				mComments[_sec].Add(_comment);
			else
				// データが存在しない場合の例外
				throw new KeyNotFoundException();
		}

		// パラメータにコメント文を付加
		public void AddComment(string _comment, string _sec, string _par)
		{
			// Keyがnullの場合の例外
			if (string.IsNullOrEmpty(_sec) || string.IsNullOrEmpty(_par))
				throw new ArgumentNullException();
			AddComment(_comment, GetComParKey(_sec, _par));
		}

		// セクションのコメント文を取得
		public List<string> GetComment(string _sec)
		{
			// Keyがnullの場合の例外
			if (string.IsNullOrEmpty(_sec))
				throw new ArgumentNullException();
			if (mComments.ContainsKey(_sec))
				return new List<string>(mComments[_sec]);
			// データが存在しない場合の例外
			throw new KeyNotFoundException();
		}

		// パラメータのコメント文を取得
		public List<string> GetComment(string _sec, string _par)
		{
			// Keyがnullの場合の例外
			if (string.IsNullOrEmpty(_sec) || string.IsNullOrEmpty(_par))
				throw new ArgumentNullException();
			return GetComment(GetComParKey(_sec, _par));
		}

		// セクションのコメント文をクリア
		public bool ClearComment(string _sec)
		{
			// Keyがnullの場合の例外
			if (string.IsNullOrEmpty(_sec))
				throw new ArgumentNullException();
			if (mComments.ContainsKey(_sec))
			{
				mComments[_sec].Clear();
				return true;
			}
			return false;
		}

		// パラメータのコメント文をクリア
		public bool ClearComment(string _sec, string _par)
		{
			// Keyがnullの場合の例外
			if (string.IsNullOrEmpty(_sec) || string.IsNullOrEmpty(_par))
				throw new ArgumentNullException();
			return ClearComment(GetComParKey(_sec, _par));
		}

		// 全てのコメント文をクリア
		public void ClearAllComment()
		{
			var keys = new List<string>(mComments.Keys);
			foreach (var key in keys)
				mComments[key].Clear();
		}

		// 全てのセクション名に変更を適応
		public void AdaptSectionName(Func<string, string> _func)
		{
			Dictionary<string, Dictionary<string, string>> dataBuf = new Dictionary<string, Dictionary<string, string>>(mData);
			mData.Clear();
			foreach (var sec in dataBuf.Keys)
			{
				var secName = _func(sec);
				if (!string.IsNullOrEmpty(secName))
				{
					var secBuf = new Dictionary<string, string>(dataBuf[sec]);
					mData.Add(secName, dataBuf[sec]);
					if (secName != sec)
					{
						mComments.Add(secName, new List<string>(mComments[sec]));
						mComments.Remove(sec);
						foreach (var par in secBuf.Keys)
						{
							mComments.Add(GetComParKey(secName, par), mComments[GetComParKey(sec, par)]);
							mComments.Remove(GetComParKey(sec, par));
						}
					}
				}
			}
		}

		// 全てのパラメータ名に変更を適応
		public void AdaptParameterName(Func<string, string> _func)
		{
			foreach (var sec in mData.Keys)
			{
				Dictionary<string, string> secBuf = new Dictionary<string, string>(mData[sec]);
				mData[sec].Clear();
				foreach (var par in secBuf.Keys)
				{
					var parName = _func(par);
					if (!string.IsNullOrEmpty(parName))
					{
						mData[sec].Add(parName, secBuf[par]);
						if (parName != par)
						{
							mComments.Add(GetComParKey(sec, parName), new List<string>(mComments[GetComParKey(sec, par)]));
							mComments.Remove(GetComParKey(sec, par));
						}
					}
				}
			}
		}

		// 全てのデータに変更を適応
		public void AdaptTexts(Func<string, string> _func)
		{
			foreach (var sec in mData.Keys)
			{
				var pars = new List<string>(mData[sec].Keys);
				foreach (var par in pars)
					mData[sec][par] = _func(mData[sec][par]);
			}
		}

		// そのセクションのパラメータ一覧
		public List<string> GetParameter(string _sec)
		{
			// Keyがnullの場合の例外
			if (string.IsNullOrEmpty(_sec))
				throw new ArgumentNullException();
			return new List<string>(mData[_sec].Keys);
		}

		// 変換取得
		public bool TryParse<T>(string _sec, string _par, out T _result)
		{
			_result = (default(T));
			// 空文字だった場合
			if (string.IsNullOrEmpty(this[_sec, _par]))
				return true;
			else
			{
				try
				{
					var converter = TypeDescriptor.GetConverter(typeof(T));
					if (converter != null)
					{
						_result = (T)converter.ConvertFromString(this[_sec, _par]);
						return true;
					}
					return false;
				}
				catch
				{
					return false;
				}
			}
		}

		// 変換取得
		public T Parse<T>(string _sec, string _par)
		{
			// 空文字だった場合
			if (string.IsNullOrEmpty(this[_sec, _par]))
				return (default(T));
			else
				return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(this[_sec, _par]);
		}

		public string this[string _sec, string _par]
		{
			get
			{
				// Keyがnullの場合の例外
				if (string.IsNullOrEmpty(_sec) || string.IsNullOrEmpty(_par))
					throw new ArgumentNullException();
				if (mData.ContainsKey(_sec) && mData[_sec].ContainsKey(_par))
					return mData[_sec][_par];
				// データが存在しない場合の例外
				throw new KeyNotFoundException();
			}
			set
			{
				// Keyがnullの場合の例外
				if (string.IsNullOrEmpty(_sec) || string.IsNullOrEmpty(_par))
					throw new ArgumentNullException();
				if (mData.ContainsKey(_sec))
				{
					if (!mData.ContainsKey(_par))
						mComments[GetComParKey(_sec, _par)] = new List<string>();
					mData[_sec][_par] = value;

				}
				else
				{
					mData[_sec] = new Dictionary<string, string>();
					mData[_sec][_par] = value;
					mComments[_sec] = new List<string>();
					mComments[GetComParKey(_sec, _par)] = new List<string>();
				}
			}
		}
		public Dictionary<string, string> this[string _sec]
		{
			get
			{
				// Keyがnullの場合の例外
				if (string.IsNullOrEmpty(_sec))
					throw new ArgumentNullException();
				if (mData.ContainsKey(_sec))
					return mData[_sec];
				// データが存在しない場合の例外
				throw new KeyNotFoundException();
			}
			set
			{
				mData[_sec] = value;
				mComments[_sec] = new List<string>();
				foreach (var dic in mData[_sec])
					mComments[GetComParKey(_sec, dic.Key)] = new List<string>();
			}
		}

		public static bool operator true(INIManager _ini) { return _ini.IsFileOpen; }
		public static bool operator false(INIManager _ini) { return !_ini.IsFileOpen; }
		public static bool operator !(INIManager _ini) { return !_ini.IsFileOpen; }
	}

	public class CSVManager
	{
		// もとのテキストデータ
		protected List<List<string>> mBaseTexts;
		// ファイルパス名
		public string FilePath { get; protected set; }
		// ファイル読み出しが成功したかどうか
		public bool IsFileOpen { get; protected set; }
		// 行
		public int Row
		{
			get
			{
				return (mBaseTexts == null ? 0 : mBaseTexts.Count);
			}
			set
			{
				if (mBaseTexts.Count > value)
					mBaseTexts.RemoveRange(mBaseTexts.Count, mBaseTexts.Count - value);
				else
				{
					int column = Column;
					for (int i = mBaseTexts.Count; i < value; i++)
					{
						List<string> rowLine = new List<string>();
						for (int j = 0; j < column; j++)
							rowLine.Add("");
						mBaseTexts.Add(rowLine);
					}
				}
			}
		}
		// 列
		public int Column
		{
			get
			{
				return (mBaseTexts == null || mBaseTexts.Count == 0 ? 0 : mBaseTexts[0].Count);
			}
			set
			{
				if (mBaseTexts == null || mBaseTexts.Count == 0)
				{
					if (mBaseTexts == null)
						mBaseTexts = new List<List<string>>();
					List<string> columunLine = new List<string>();
					for (int i = 0; i < value; i++)
						columunLine.Add("");
					mBaseTexts.Add(columunLine);
				}
				else if (mBaseTexts[0].Count > value)
				{
					for (int i = 0; i < Row; i++)
						mBaseTexts.RemoveRange(mBaseTexts[0].Count, mBaseTexts[0].Count - value);
				}
				else
				{
					for (int i = 0; i < Row; i++)
					{
						for (int j = mBaseTexts[i].Count; j < value; j++)
							mBaseTexts[i].Add("");
					}
				}
			}
		}

		// コンストラクタ
		public CSVManager()
		{
			IsFileOpen = false;
			mBaseTexts = new List<List<string>>();
		}

		// ファイルオープンコンストラクタ
		public CSVManager(string _filePath) : this()
		{
			Open(_filePath);
		}
		public CSVManager(string _filePath, Encoding _enc) : this()
		{
			Open(_filePath, _enc);
		}

		// コピーコンストラクタ
		public CSVManager(CSVManager _csv)
		{
			FilePath = _csv.FilePath;
			IsFileOpen = _csv.IsFileOpen;
			mBaseTexts = new List<List<string>>();
			foreach (var listStr in _csv.mBaseTexts)
				mBaseTexts.Add(new List<string>(listStr));
		}

		// データクリア
		public void Clear()
		{
			mBaseTexts.Clear();
		}

		public bool Open(string _filePath)
		{
			return Open(_filePath, null);
		}
		public bool Open(string _filePath, Encoding _enc)
		{
			List<string> baseTexts = new List<string>(_enc == null ? File.ReadAllLines(_filePath) : File.ReadAllLines(_filePath, _enc));
			if (baseTexts.Count == 0)
				return false;
			Analysis(baseTexts);
			FilePath = _filePath;
			IsFileOpen = true;
			return true;
		}

		// 文章分析
		protected void Analysis(List<string> _texts)
		{
			Clear();
			foreach (var line in _texts)
			{
				var factors = line.Split(',');
				var list = new List<string>();
				string str = "";
				foreach (var value in factors)
				{
					if (string.IsNullOrEmpty(str))
					{
						if (value.StartsWith("\""))
							str = value + ",";
						else
							list.Add(value);
					}
					else
					{
						str += value;
						if (value.EndsWith("\""))
						{
							list.Add(str);
							str = "";
						}
						else
							str += ",";
					}
				}
				mBaseTexts.Add(list);
			}
		}

		// 書き込み
		public virtual bool Write()
		{
			return Write(FilePath, null);
		}
		public virtual bool Write(Encoding _enc)
		{
			return Write(FilePath, _enc);
		}

		// 別名書き込み
		public bool Write(string _filePath)
		{
			return Write(_filePath, null);
		}
		public bool Write(string _filePath, Encoding _enc)
		{
			// ファイルにテキストを書き出し
			using (var writer = (_enc == null ? new StreamWriter(_filePath) : new StreamWriter(_filePath, false, _enc)))
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

		// 行追加
		public void AddRow()
		{
			AddRow(new List<string>());
		}

		public void AddRow(List<string> _rowLine)
		{
			List<string> rowLine = new List<string>();
			int column = Column;
			for (int i = 0; i < column; i++)
			{
				if (i < _rowLine.Count)
					rowLine.Add(_rowLine[i]);
				else
					rowLine.Add("");
			}
			mBaseTexts.Add(rowLine);
		}

		// 列追加
		public void AddColumn()
		{
			AddColumn(new List<string>());
		}

		public void AddColumn(List<string> _columnLine)
		{
			int column = Column;
			for (int i = 0; i < Row; i++)
			{
				for (int j = mBaseTexts[i].Count; j < column; j++)
					mBaseTexts[i].Add("");
				if (i < _columnLine.Count)
					mBaseTexts[i].Add(_columnLine[i]);
				else
					mBaseTexts[i].Add("");
			}
		}

		// 行挿入
		public void InsertRow(int _row, List<string> _rowLine)
		{
			List<string> rowLine = new List<string>();
			int column = Column;
			for (int i = 0; i < column; i++)
			{
				if (i < _rowLine.Count)
					rowLine.Add(_rowLine[i]);
				else
					rowLine.Add("");
			}
			mBaseTexts.Insert(_row, rowLine);
		}

		// 列挿入
		public void InsertColumn(int _colmun, List<string> _columnLine)
		{
			int column = Column;
			for (int i = 0; i < Row; i++)
			{
				for (int j = mBaseTexts[i].Count; j < column; j++)
					mBaseTexts[i].Add("");
				if (i < _columnLine.Count)
					mBaseTexts[i].Insert(_colmun, _columnLine[i]);
				else
					mBaseTexts[i].Insert(_colmun, "");
			}
		}

		// 行削除
		public bool RemoveRow(int _row)
		{
			if (_row >= Row)
				return false;
			mBaseTexts.RemoveAt(_row);
			return true;
		}

		// 列削除
		public bool RemoveColumn(int _column)
		{
			if (_column >= Column)
				return false;
			for (int i = 0; i < Row; i++)
				mBaseTexts[i].RemoveAt(_column);
			return true;
		}

		// 変換取得
		public bool TryParse<T>(int _row, int _column, out T _result)
		{
			_result = (default(T));
			// 空文字だった場合
			if (string.IsNullOrEmpty(this[_row, _column]))
				return true;
			else
			{
				try
				{
					var converter = TypeDescriptor.GetConverter(typeof(T));
					if (converter != null)
					{
						_result = (T)converter.ConvertFromString(this[_row, _column]);
						return true;
					}
					return false;
				}
				catch
				{
					return false;
				}
			}
		}

		// 変換取得
		public T Parse<T>(int _row, int _column)
		{
			// 空文字だった場合
			if (string.IsNullOrEmpty(this[_row, _column]))
				return (default(T));
			else
				return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(this[_row, _column]);
		}

		// ソート
		public void Sort(IComparer<List<string>> _comparer)
		{
			mBaseTexts.Sort(_comparer);
		}

		public void Sort(int _index, int _count, IComparer<List<string>> _comparer)
		{
			mBaseTexts.Sort(_index, _count, _comparer);
		}

		public string this[int _row, int _column] { get { return mBaseTexts[_row][_column]; } set { mBaseTexts[_row][_column] = value; } }
		public IList<string> this[int _row] { get { return mBaseTexts[_row].AsReadOnly(); } }
		public List<string> GetColumnLine(int _column)
		{
			var list = new List<string>();
			foreach (var line in mBaseTexts)
				list.Add(line[_column]);
			return list;
		}

		public static bool operator true(CSVManager _ini) { return _ini.IsFileOpen; }
		public static bool operator false(CSVManager _ini) { return !_ini.IsFileOpen; }
		public static bool operator !(CSVManager _ini) { return !_ini.IsFileOpen; }
	}
}
