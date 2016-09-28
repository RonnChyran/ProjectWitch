//=====================================
//author	:shotta
//summary	:テキストの作業場
//=====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; //Exception
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

using Scenario.Pattern;
using Scenario.Command;

namespace Scenario.WorkSpace
{
	public class TextWorkSpace : MonoBehaviour
	{
		void Start()
		{
			ResetSpeed ();
		}

		//テキスト関連のビューの可視不可視を設定
		//名前ビューのオンオフ
		protected void SetNameVisible(bool isVisible)
		{
			mNameVisible = isVisible;
			RefreshVisibility();
		}
		//テキストウィンドウのオンオフ
		protected void SetTextVisible(bool isVisible)
		{
			mTextVisible = isVisible;
			RefreshVisibility ();

		}

		private bool mNameVisible = true;
		private bool mTextVisible = true;
		private void RefreshVisibility()
		{
			mNameView.SetActive	(mNameVisible && mTextVisible);
			mTextView.SetActive	(mTextVisible);
			mTextBackground.SetActive (mTextVisible);
		}

		//テキスト、名前の背景
		[SerializeField]
		private GameObject mTextView;
		[SerializeField]
		private GameObject mNameView;
		[SerializeField]
		private GameObject mTextBackground;

		//テキストを指定速度で再生するアップデータ
		public class TextWindowUpdater : UpdaterFormat
		{
			private TextWorkSpace mTWS;
			private bool mHidden;

			private RawImage mNameImage;
			private Text mName;
			private RawImage mTextImage;
			private Text mText;

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
				mNameImage 	= mTWS.mNameView.GetComponent<RawImage> ();
				mName 		= mTWS.mName;
				mTextImage 	= mTWS.mTextView.GetComponent<RawImage> ();
				mText	 	= mTWS.mText;
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

				float progress = Mathf.Clamp (mTime, 0, mDuration)/mDuration; 
				SetOpacity(GetOpacity (progress));
			}
			public override void Finish ()
			{
				SetOpacity(GetOpacity (1.0f));
			}
			private float GetOpacity (float progress)
			{
				if (mHidden)
					return progress;
				else
					return 1.0f - progress;
			}
			private void SetOpacity (float opacity)
			{
				Color buf;
				buf = mName.color;
				buf.a = opacity;
				mName.color = buf;

				buf = mNameImage.color;
				buf.a = opacity;
				mNameImage.color = buf;

				buf = mText.color;
				buf.a = opacity;
				mText.color = buf;

				buf = mTextImage.color;
				buf.a = opacity;
				mTextImage.color = buf;
			}
		}

		//テキストのNextアイコン
		[SerializeField]
		private GameObject mNextIcon;

		//テキスト
		protected string Text
		{
			get{return mText.text;}
			set{mText.text = value;}
		}
		[SerializeField]
		private Text mText;
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
				mTWS.mNextIcon.SetActive(true);
				base.Setup ();
			}
			public override void Finish()
			{
				mTWS.mNextIcon.SetActive(false);
				mTWS.Text = "";
				mTWS.ResetSpeed ();
				base.Finish ();
			}
		}

		//名前
		public string Name
		{
			get{ return mName.text; }
			set{ mName.text = value; }
		}
		[SerializeField]
		private Text mName;

		//テキスト関連のプロパティ
		//	速さ(文字/sec)
		[SerializeField]
		private float mSpeed = 30.0f;
		protected void ResetSpeed()
		{
			mSpeed = Game.GetInstance ().Config.TextSpeed;
		}

		public void SetCommandDelegaters(VirtualMachine vm)
		{
			vm.AddCommandDelegater(
				"InvisibleName",
				new CommandDelegater(false, 0, delegate(object[] arguments){
					SetNameVisible(false);
					return null;
				}));
			vm.AddCommandDelegater(
				"InvisibleTextWindow",
				new CommandDelegater(true, 0, delegate(object[] arguments){
					UpdaterFormat updater = new TextWindowUpdater(true, 0.5f, this);
					arguments[0] = updater;
					return null;
				}));
			vm.AddCommandDelegater(
				"VisibleTextWindow",
				new CommandDelegater(true, 0, delegate(object[] arguments){
					UpdaterFormat updater = new TextWindowUpdater(false, 0.5f, this);
					arguments[0] = updater;
					return null;
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