//=====================================
//author	:shotta
//summary	:テキストの作業場
//=====================================

using System;
using UnityEngine;

namespace ProjectWitch.Talk.WorkSpace
{
	public class TextWorkSpace : MonoBehaviour
	{
        //テキストウィンドウマネージャ
        [SerializeField]
        private TextWindowManager mTextWindowManager = null;

        //テキストウィンドウのアンカー
        [SerializeField]
        private GameObject mWindowAnchorTop = null;
        [SerializeField]
        private GameObject mWindowAnchorBottom = null;

        //テキストウィンドウスキンフォルダへの参照
        [SerializeField]
        private string mWindowSkinFolderPath = "Prefabs/Talk/Skin/";

        void Start()
		{
			ResetSpeed ();
		}

		//テキスト関連のビューの可視不可視を設定
		//名前ビューのオンオフ
		protected void SetNameVisible(bool isVisible)
		{
            if (isVisible)
                mTextWindowManager.Window.ShowName();
            else
                mTextWindowManager.Window.HideName();

		}
		//テキストウィンドウのオンオフ
		protected void SetTextVisible(bool isVisible)
		{
            if (isVisible)
                mTextWindowManager.Window.ShowWindow();
            else
                mTextWindowManager.Window.HideWindow();

		}

        //テキストウィンドウの位置変更
        protected void SetTextWindowPos(string pos)
        {
            if(pos=="top")
            {
                mTextWindowManager.Window.Position = mWindowAnchorTop.transform.localPosition;
            }
            else
            {
                mTextWindowManager.Window.Position = mWindowAnchorBottom.transform.localPosition;
            }
        }

        //テキストウィンドウのスキンを変更
        protected void ChangeWindowSkin(string name)
        {
            mTextWindowManager.ChangeSkin(mWindowSkinFolderPath + name);
        }

        //顔グラのロード
        protected void LoadFace(int id, string name)
        {
            mTextWindowManager.Window.LoadFace(id, name);
        }

        //顔グラの表示
        protected void ShowFace(int id, string name)
        {
            mTextWindowManager.Window.ShowFace(id, name);
        }

        //顔グラの非表示
        protected void HideFace()
        {
            mTextWindowManager.Window.HideFace();
        }

        //顔グラ変更
        protected void ChangeFace(string state)
        {
            mTextWindowManager.Window.ChangeStateFace(state);
        }

		//テキストを指定速度で再生するアップデータ
		public class TextWindowUpdater : UpdaterFormat
		{
			private TextWorkSpace mTWS;
			private bool mHidden;

			private float mTime = 0.0f;
			private float mDuration;

			public TextWindowUpdater(bool hidden, float duration, TextWorkSpace tws)
			{
				mDuration = duration;
				mHidden = hidden;
				mTWS = tws;
			}

			//これまでのテキストを取得
			public override void Setup ()
			{
                mTWS.SetTextVisible(mHidden);
			}
			//テキストを追加
			public override void Update (float deltaTime)
			{
				mTime += deltaTime;
				if (mTime >= mDuration)
				{
					SetActive (false);
					return;
				}
			}
			public override void Finish ()
			{
			}
		}

		//テキスト
		protected string Text
		{
			get{return mTextWindowManager.Window.Message;}
			set{mTextWindowManager.Window.Message = value;}
		}
		//テキストを指定速度で再生するアップデータ
		public class TextUpdater : UpdaterFormat
		{
			private TextWorkSpace mTWS;

			private float mTime = 0.0f;
			private string mPrevText;
			private string mAddedText;
			public TextUpdater(string text, TextWorkSpace tws)
			{
				mAddedText = text;
				mTWS = tws;
			}

			//これまでのテキストを取得
			public override void Setup ()
			{
				mPrevText = mTWS.Text;
			}
			//テキストを追加
			public override void Update (float deltaTime)
			{
				mTWS.Text = mPrevText + mAddedText;
				//変数準備
				float speed = mTWS.mSpeed;
				mTime += deltaTime;

				//時間に合わせて文字数を設定
				int chCount = (int)(speed * mTime);
				if (chCount >= mAddedText.Length || speed < 0.0)
				{
					SetActive (false);
					return;
				}

				//表示
				mTWS.Text = mPrevText + mAddedText.Substring (0, chCount);
			}
			public override void Finish ()
			{
				mTWS.Text = mPrevText + mAddedText;
			}
		}

        public class FaceUpdater : UpdaterFormat
        {
            private bool mHidden;

            private float mTime = 0.0f;
            private float mDuration;

            public FaceUpdater(bool hidden, float duration, TextWorkSpace tws)
            {
                mDuration = duration;
                mHidden = hidden;
            }
            //これまでのテキストを取得
            public override void Setup()
            {
            }
            //テキストを追加
            public override void Update(float deltaTime)
            {
                mTime += deltaTime;
                if (mTime >= mDuration)
                {
                    SetActive(false);
                    return;
                }
            }
            public override void Finish()
            {
            }
        }


		//改ページコマンド
		public class NewPageUpdater : WaitUpdater{
			private TextWorkSpace mTWS;

			protected NewPageUpdater(){}
			public NewPageUpdater(TextWorkSpace tws)
			{
				mTWS = tws;
			}
			public override void Setup ()
			{
				mTWS.mTextWindowManager.Window.ShowNextIcon();
				base.Setup ();
			}
			public override void Finish()
			{
				mTWS.mTextWindowManager.Window.HideNextIcon();
				mTWS.Text = "";
				mTWS.ResetSpeed ();
				base.Finish ();
			}
		}

		//名前
		public string Name
		{
			get{ return mTextWindowManager.Window.Name; }
			set{ mTextWindowManager.Window.Name = value; }
		}

		//テキスト関連のプロパティ
		//	速さ(文字/sec)
		[SerializeField]
		private float mSpeed = 30.0f;
		protected void ResetSpeed()
		{
            var config = Game.GetInstance().SystemData.Config;
			mSpeed = ConfigDataFormat.TextSpeedValues[config.TextSpeed];
		}

		public void SetCommandDelegaters(VirtualMachine vm)
		{
            vm.AddCommandDelegater(
                "ChangeSkin",
                new CommandDelegater(false, 1, delegate (object[] arguments){
                    string error = null;

                    var name = Converter.ObjectToString(arguments[0], out error);
                    if (error != null) return error;

                    ChangeWindowSkin(name);

                    return null;
                }));
			vm.AddCommandDelegater(
				"InvisibleName",
				new CommandDelegater(false, 0, delegate(object[] arguments){
					SetNameVisible(false);
					return null;
				}));
			vm.AddCommandDelegater(
				"ShowMessage",
				new CommandDelegater(true, 1, delegate(object[] arguments){
                    string error = null;

                    var pos = Converter.ObjectToString(arguments[0], out error);
                    if (error != null) return error;

                    SetTextWindowPos(pos);

                    UpdaterFormat updater = new TextWindowUpdater(true, 0.15f, this);
					arguments[1] = updater;
					return error;
				}));
			vm.AddCommandDelegater(
				"HideMessage",
				new CommandDelegater(true, 0, delegate(object[] arguments){
					UpdaterFormat updater = new TextWindowUpdater(false, 0.15f, this);
					arguments[0] = updater;
					return null;
				}));
            vm.AddCommandDelegater(
                "LoadFace",
                new CommandDelegater(false, 2, delegate (object[] arguments)
                {
                    string error = null;

                    var id = Converter.ObjectToInt(arguments[0], out error);
                    var name = Converter.ObjectToString(arguments[1], out error);
                    if (error != null) return error;

                    LoadFace(id, name);

                    return error;
                }));
            vm.AddCommandDelegater(
                "DrawFace",
                new CommandDelegater(true, 2, delegate (object[] arguments)
                {
                    string error = null;

                    var id = Converter.ObjectToInt(arguments[0], out error);
                    var state = Converter.ObjectToString(arguments[1], out error);
                    if (error != null) return error;

                    ShowFace(id, state);

                    UpdaterFormat updater = new FaceUpdater(true, 0.05f, this);
                    arguments[2] = updater;
                    return error;
                }));
            vm.AddCommandDelegater(
                "ClearFace",
                new CommandDelegater(true, 1, delegate (object[] arguments)
                {
                    string error = null;

                    HideFace();

                    UpdaterFormat updater = new FaceUpdater(false, 0.05f, this);
                    arguments[0] = updater;
                    return error;
                }));
            vm.AddCommandDelegater(
                "ChangeFace",
                new CommandDelegater(false, 1, delegate (object[] arguments)
                {
                    string error = null;

                    var state = Converter.ObjectToString(arguments[0], out error);
                    if (error != null) return error;

                    ChangeFace(state);

                    return error;
                }));
			vm.AddCommandDelegater(
				"Name",
				new CommandDelegater(false, 1, delegate(object[] arguments){
					string error;
					string name = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error; 
					Name = name;
					SetNameVisible(true);
					return null;
				}));
			vm.AddCommandDelegater(
				"Text",
				new CommandDelegater(true, 1, delegate(object[] arguments){
					string error;
					string text = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;
					arguments[1] = new TextUpdater(text, this);
					return null;
				}));
			vm.AddCommandDelegater(
				"TextSpeed",
				new CommandDelegater(false, 1, delegate(object[] arguments){
					string error;
					float speed = Converter.ObjectToFloat(arguments[0], out error);
					if (error != null) return error; 
					if (speed > 0.0f)
						mSpeed = speed;
					else
						mSpeed = -1.0f;
					return null;
				}));
			vm.AddCommandDelegater(
				"NewPage",
				new CommandDelegater(true, 0, delegate(object[] arguments){
					arguments[0] = new NewPageUpdater(this);
					return null;
				}));
		}
	}

}