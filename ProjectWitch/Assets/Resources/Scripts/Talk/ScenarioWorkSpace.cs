//==========================================================
//	author	:shotta
//	summary	:コマンドを走らせて、操作を受け取るまでする部分
//==========================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Script.Command;
using Script.Analyzer;
using Script.Compiler;

//シナリオ操作時のワークスペース
public class ScenarioWorkSpace : MonoBehaviour 
{
	//スクリプトのパス(値の渡し方は決めてない)
	[SerializeField]
	private string mPath;

	//ここから現在使ってるワークスペースを呼び出す
	private static ScenarioWorkSpace mWorkSpace = null;
	public static ScenarioWorkSpace SharedInstance()
	{
		return mWorkSpace;
	}

	//コマンドを読み込む
	void Start (){
		//現在使ってるワークスペースは自分だよ！！！
		mWorkSpace = this;

		CreateCommand ( GameData.GamePath.Senario + mPath);
		SetCommand (0);
	}
	// Update is called once per frame
	void Update ()
	{
		//現在使ってるワークスペースは自分だよ！！！
		mWorkSpace = this;

		//以下コマンドを実行する部分
		mDeltaTime = Time.deltaTime;
		RunCommand ();

		//次のページへ移動&読み飛ばし
		if (Input.GetKeyDown (KeyCode.Return))
			SkipCommand ();
	}

	//コマンド系
	private List<RunCommand> mCommandList;
	private int mCommandIndex = -1; 
	//コマンド読み出し
	public void CreateCommand(string path)
	{
		if (path == "")
		{
			Debug.Log("読み出し失敗: " + path);
			return;
		}
		ScriptCompiler compiler = new ScriptCompiler();
		ErrorList errorList;
		mCommandList = compiler.CompileScript (path, out errorList);

		if (errorList.Count == 0)
			Debug.Log("読み出し完了");
		else
			Debug.Log ("" + errorList.ErrorMessage + "");

	}

	//コマンドを次のインデックスに更新
	public void StepCommandNext()
	{
		SetCommand(mCommandIndex+1);
	}
	//コマンドを指定位置に更新
	public void SetCommand(int index)
	{
		//今のコマンドを終了
		FinishCommand ();
		//コマンドインデックス更新
		mCommandIndex = index;
		//コマンド初期化
		SetupCommand ();

		Debug.Log ("現在のコマンドのIndex: " + index + "");
	}
	//コマンドを入手
	private RunCommand GetCommand(int i)
	{
		RunCommand command = null;
		if (mCommandList != null)
		{
			if (0<=i&&i<mCommandList.Count)
				command = mCommandList [i];
		}
		return command;
	}

	//コマンドを初期化
	public void SetupCommand()
	{
		RunCommand command = GetCommand (mCommandIndex);
		if (command != null)
			command.Setup ();
	}
	//コマンドを実行
	public void RunCommand()
	{
		RunCommand command = GetCommand (mCommandIndex);
		if (command != null)
			command.Run ();
	}
	//コマンドを終了
	public void FinishCommand()
	{
		RunCommand command = GetCommand (mCommandIndex);
		if (command != null)
			command.Finish ();
	}
	//改ページまでコマンドを飛ばす
	public void SkipCommand()
	{
		if (GetCommand (mCommandIndex) is CreateNewPageCommand.NewPageCommand) 
		{
			//次のページへ
			StepCommandNext ();
		}
		else 
		{
			//一気に表示
			for (int i = mCommandIndex + 1; i <= mCommandList.Count; i++)
			{
				RunCommand command = GetCommand (i);
				SetCommand (i);
				if (command is CreateNewPageCommand.NewPageCommand)
					break;
			}
		}
	}

	//以下コマンドから参照するオブジェクト群

	//テキスト表示ビュー
	public Text TextView{
		get { return mTextView; }
	}
	[SerializeField]
	private Text mTextView;

	//名前表示ビュー
	public Text NameView{
		get{ return mNameView; }
	}
	[SerializeField]
	private Text mNameView;

	//名前ビューのオンオフ
	public bool isVisibleNameView{
		get{ return mNameWrapper.activeSelf; }
		set{ mNameWrapper.SetActive (value); }
	}
	[SerializeField]
	private GameObject mNameWrapper;

	//背景
	public void SetBackgroundImage(string name)
	{
		string path = "Textures/Background2D/" + name;
		mBackground.GetComponent<RawImage>().texture = Resources.Load (path) as Texture2D;
	}
	[SerializeField]
	private GameObject mBackground;

	//立ち絵の位置
	public enum CGPosition{
		Right,
		Center,
		Left
	}
	//立ち絵のロード
	public void LoadCG(int id, string name)
	{
		GameObject obj = mCGArray [id];
		if (obj != null)
			Destroy (obj);

		string path = "Textures/Character_Stand/" + name;
		obj = Instantiate (mCGPrefab) as GameObject;
		obj.GetComponent<RawImage>().texture = Resources.Load (path) as Texture2D;
		obj.transform.parent = mCGLayer.transform;

		mCGArray [id] = obj;
	}
	//立ち絵の表示
	public void DrawCG(int id, CGPosition pos)
	{
		GameObject obj = mCGArray [id];
		obj.SetActive (true);
		switch (pos)
		{
		case CGPosition.Right:
			break;
		case CGPosition.Center:
			break;
		case CGPosition.Left:
			break;
		}
	}
	//立ち絵の削除
	public void ClearCG(int id)
	{
		GameObject obj = mCGArray [id];
		obj.SetActive (false);
	}
	//	立ち絵管理用の配列
	private GameObject[] mCGArray = new GameObject[16];
	[SerializeField]
	private GameObject mCGLayer;
	[SerializeField]
	private GameObject mCGPrefab;

	//以下コマンドから操作・参照するプロパティ群

	//	速さ(文字/sec)
	public float Speed
	{
		get{ return mSpeed; }
		set{ mSpeed = value; }
	}
	private float mSpeed = 15.0f;

	//	色(RGBA)
	public Color32 Color
	{
		get{ return mColor; }
		set{ mColor = value; }
	}
	private Color32 mColor = new Color32(0,0,0,0xFF);

	//	微小時間
	public float DeltaTime
	{
		get{ return mDeltaTime;}
	}
	private static float mDeltaTime = 0.0f;
}