//=====================================
//author	:shotta
//summary	:グラフィックの作業場
//=====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; //Exception
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

using Scenario.Pattern;
using Scenario.Command;
using Scenario.Compiler;

namespace Scenario.WorkSpace
{
	public class GraphicsWorkSpace : MonoBehaviour
	{
		//キャンバス
		[SerializeField]
		private RectTransform mCanvas;
		private Rect CanvasRect{
			get{ return mCanvas.rect; }
		}

		//背景のパス
		[SerializeField]
		private string mBackgroundPath;
		//背景用スプライト
		[SerializeField]
		private GameObject mBackgroundSprite;

		//立ち絵のパス
		[SerializeField]
		private string mCGPath;
		//立ち絵を表示させるレイヤー
		[SerializeField]
		private StandCGController mCGLayer;

		//立ち絵の位置用のアップデータ
		private class StandCGUpdater : UpdaterFormat
		{
			public delegate void AnimationDelegate(float progress);

			private float mTime;
			private AnimationDelegate mMethod;
			private float mDuration;
			private StandCGUpdater(){}
			public StandCGUpdater(float duration, AnimationDelegate method)
			{
				mDuration = duration;
				mMethod = method;
			}

			public override void Setup ()
			{
				mTime = 0.0f;
				mMethod (0.0f);
			}

			public override void Update (float deltaTime)
			{
				mTime += deltaTime;
				mMethod (mTime/mDuration);
				if (mTime > mDuration)
					SetActive(false);
			}

			public override void Finish ()
			{
				mMethod (1.0f);
			}
		}

		//マスクのカラーをセット
		public Color MaskColor{
			get{return mMask.color;}
			set{mMask.color = value;}
		}
		//マスク
		[SerializeField]
		private Image mMask;

		//フィルタ用のアップデータ
		class MaskAnimation : PauseUpdater
		{
			//progress	:0.0~1.0
			public delegate float TransparencyDelegate(float progress);

			private MaskAnimation (){}
			public MaskAnimation(TransparencyDelegate method, Color32 color, float time, GraphicsWorkSpace gws)
			{
				mMethod = method;
				mNextColor = color;
				mDuration = time;
				mGWS = gws;
			}

			private TransparencyDelegate mMethod;
			private GraphicsWorkSpace mGWS;

			private Color32 mPrevColor;
			private Color32 mNextColor;
			private float mTime = 0.0f;
			private float mDuration = 0.0f;

			public override void Setup ()
			{
				mPrevColor = mGWS.MaskColor;
				mTime = 0.0f;
			}
			public override void Update (float deltaTime)
			{
				mTime += deltaTime;

				SetMaskColor (Mathf.Clamp(mTime/mDuration, 0.0f, 1.0f));

				if (mTime > mDuration)
					SetActive (false);
			}
			public override void Finish ()
			{
				SetMaskColor (1.0f);
			}

			//マスクのカラーを設定(ヘルパー)
			private void SetMaskColor(float progress)
			{
				float percent = Mathf.Clamp(mMethod (progress), 0.0f, 1.0f);
				float r = mPrevColor.r * (1.0f - percent) + mNextColor.r * percent;
				float g = mPrevColor.g * (1.0f - percent) + mNextColor.g * percent;
				float b = mPrevColor.b * (1.0f - percent) + mNextColor.b * percent;
				float a = mPrevColor.a * (1.0f - percent) + mNextColor.a * percent;

				mGWS.MaskColor = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
			}
		}

		//立ち絵のパス
		[SerializeField]
		private string mMoviePath;

		public void SetCommandDelegaters(VirtualMachine vm)
		{
			//背景を表示
			vm.AddCommandDelegater(
				"SetBackground",
				new CommandDelegater(false, 1, delegate(object[] arguments){
					string error;
					string name = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;
					string path = mBackgroundPath + name;
					mBackgroundSprite.GetComponent<RawImage>().texture = Resources.Load (path) as Texture2D;
					return null;
				}));
			//立ち絵を読み込み
			vm.AddCommandDelegater(
				"LoadCG",
				new CommandDelegater(false, 2, delegate(object[] arguments){
					string error;
					int id = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					string name = Converter.ObjectToString(arguments[1], out error);
					if (error != null) return error; 

					string path = mCGPath + name;
					mCGLayer.AddStandCG(id, path, out error);
					if (error != null) return error;
					GameObject obj = mCGLayer.GetStandCG(id, out error);
					obj.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
					return error;
				}));
			//立ち絵を表示
			vm.AddCommandDelegater(
				"ShowCG",
				new CommandDelegater(true, 3, delegate(object[] arguments){
					string error;
					int id = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					string pos = Converter.ObjectToString(arguments[1], out error);
					if (error != null) return error;
					string mode = Converter.ObjectToString(arguments[2], out error);
					if (error != null) return error;

					Rect canvasRect = CanvasRect;
					float x = 1.5f * canvasRect.width;
					Vector3 position_prev = new Vector3(0.0f, 0.0f, 0.0f);
					Vector3 position_next = new Vector3(0.0f, 0.0f, 0.0f);
					switch (pos) {
					case "right":
						position_prev = new Vector3(x, 0.0f, 0.0f);
						position_next = new Vector3(500.0f, 0.0f, 0.0f);
						break;
					case "center":
						position_prev = new Vector3(x, 0.0f, 0.0f);
						position_next = new Vector3(0.0f, 0.0f, 0.0f);
						break;
					case "left":
						position_prev = new Vector3(-x, 0.0f, 0.0f);
						position_next = new Vector3(-500.0f, 0.0f, 0.0f);
						break;
					default:
						return "正しい位置を指定してください。";
					}
					position_next = mCGLayer.GetUnduplicatePosition(position_next);

					mCGLayer.ShowStandCG(id, out error);

					GameObject obj = mCGLayer.GetStandCG(id, out error);
					if (error != null) return error;

					float time = 0.5f;
					UpdaterFormat updater = null;
					switch (mode)
					{
					case "slidein":
						{
							Transform trans = obj.transform;
							trans.localPosition = position_prev;
							updater = new StandCGUpdater(time, delegate(float progress) {
								trans.localPosition = position_prev * (1.0f - progress) + position_next * progress;
							});
						}
						break;

					case "fadein":
						{
							Transform trans = obj.transform;
							trans.localPosition = position_next;
							RawImage image = obj.GetComponent<RawImage> ();
							updater = new StandCGUpdater(time, delegate(float progress) {
								Color cp = image.color;
								image.color = new Color(cp.r, cp.g, cp.b, progress);
							});
						}
						break;

					default:
						return "正しいモードを指定してください。";
					}
					arguments[3] = updater;
					return error;
				}));
			//立ち絵を非表示
			vm.AddCommandDelegater(
				"HideCG",
				new CommandDelegater(true, 2, delegate(object[] arguments){
					string error = null;
					int id = Converter.ObjectToInt(arguments[0], out error);
					if (error!= null) return error;
					string mode = Converter.ObjectToString(arguments[1], out error);
					if (error != null) return error;

					GameObject obj = mCGLayer.GetStandCG(id, out error);
					if (error != null) return error;

					Transform trans = obj.transform;

					Rect canvasRect = CanvasRect;
					float x = 1.5f * canvasRect.width;
					Vector3 position_prev = trans.localPosition;
					Vector3 position_next;
					if (position_prev.x>-250)
						position_next = new Vector3(x, position_prev.y, position_prev.z);
					else
						position_next = new Vector3(-x, position_prev.y, position_prev.z);

					float time = 0.5f;
					UpdaterFormat updater = null;
					switch (mode)
					{
					case "slideout":
						{
							updater = new StandCGUpdater(time, delegate(float progress) {
								trans.localPosition = position_prev * (1.0f - progress) + position_next * progress;
								if (progress>0.99)
									mCGLayer.HideStandCG(id, out error);
							});
						}
						break;

					case "fadeout":
						{
							RawImage image = obj.GetComponent<RawImage> ();
							Color cp = image.color;
							updater = new StandCGUpdater(time, delegate(float progress) {
								image.color = new Color(cp.r, cp.g, cp.b, (1.0f - progress) * cp.a);
								if (progress>0.99)
									mCGLayer.HideStandCG(id, out error); 
							});
						}
						break;

					default:
						return "正しいモードを指定してください。";
					}
					arguments[2] = updater;
					return error;
				}));
			
			//立ち絵の
			//移動
			vm.AddCommandDelegater (
				"moveTo",
				new CommandDelegater (true, 4, delegate(object[] arguments) {
					string error;
					int id = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					float x_per = Converter.ObjectToFloat(arguments[1], out error);
					if (error != null) return error;
					float y_per = Converter.ObjectToFloat(arguments[2], out error);
					if (error != null) return error;
					float time_ms = Converter.ObjectToInt(arguments[3], out error);
					if (error != null) return error;

					Rect canvasRect = CanvasRect;
					float x = (x_per/100.0f) * canvasRect.width;
					float y = (y_per/100.0f) * canvasRect.height;
					float time = time_ms / 1000.0f;

					GameObject obj = mCGLayer.GetStandCG(id, out error);
					if (error != null) return error;

					Transform trans = obj.transform;
					Vector3 position_prev = trans.localPosition;
					Vector3 position_next = new Vector3(x, y, 0.0f) + new Vector3(canvasRect.x, canvasRect.y, 0.0f);

					UpdaterFormat transUpdater = new StandCGUpdater(time, delegate(float progress) {
						trans.localPosition = position_prev * (1.0f - progress) + position_next * progress;
					});
					arguments[4] = transUpdater;
					return null;
				}));

			//移動
			vm.AddCommandDelegater (
				"move",
				new CommandDelegater (true, 4, delegate(object[] arguments) {
					string error;
					int id = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					float x_per = Converter.ObjectToFloat(arguments[1], out error);
					if (error != null) return error;
					float y_per = Converter.ObjectToFloat(arguments[2], out error);
					if (error != null) return error;
					float time_ms = Converter.ObjectToInt(arguments[3], out error);
					if (error != null) return error;

					Rect canvasRect = CanvasRect;
					float x = (x_per/100.0f) * canvasRect.width;
					float y = (y_per/100.0f) * canvasRect.height;
					float time = time_ms / 1000.0f;

					GameObject obj = mCGLayer.GetStandCG(id, out error);
					if (error != null) return error;

					Transform trans = obj.transform;
					Vector3 position_prev = trans.localPosition;
					Vector3 position_next = trans.localPosition + new Vector3(x, y, 0.0f);

					UpdaterFormat transUpdater = new StandCGUpdater(time, delegate(float progress) {
						trans.localPosition = position_prev * (1.0f - progress) + position_next * progress;
					});
					arguments[4] = transUpdater;
					return null;
				}));

			//回転
			vm.AddCommandDelegater (
				"rotate",
				new CommandDelegater (true, 3, delegate(object[] arguments) {
					string error;
					int id = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					float angle = Converter.ObjectToFloat(arguments[1], out error);
					if (error != null) return error;
					float time_ms = Converter.ObjectToInt(arguments[2], out error);
					if (error != null) return error;
					float time = time_ms / 1000.0f;

					GameObject obj = mCGLayer.GetStandCG(id, out error);
					if (error != null) return error;

					Transform trans = obj.transform;
					Quaternion quaternion_prev = trans.localRotation;

					UpdaterFormat transUpdater = new StandCGUpdater(time, delegate(float progress) {
						trans.localRotation = quaternion_prev * Quaternion.Euler(new Vector3(0.0f, 0.0f, progress * angle));
					});
					arguments[3] = transUpdater;
					return null;
				}));
			
			//拡大
			vm.AddCommandDelegater (
				"scale",
				new CommandDelegater (true, 3, delegate(object[] arguments) {
					string error;
					int id = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					float scale = Converter.ObjectToFloat(arguments[1], out error);
					if (error != null) return error;
					float time_ms = Converter.ObjectToInt(arguments[2], out error);
					if (error != null) return error;
					float time = time_ms / 1000.0f;

					GameObject obj = mCGLayer.GetStandCG(id, out error);
					if (error != null) return error;

					Transform trans = obj.transform;
					Vector3 size_prev = trans.localScale;

					UpdaterFormat transUpdater = new StandCGUpdater(time, delegate(float progress) {
						float currScale = (1.0f - progress)  * 1.0f + progress * scale;
						trans.localScale = Vector3.Scale(size_prev, new Vector3(currScale, currScale, 1.0f));
					});
					arguments[3] = transUpdater;
					return null;
				}));
			
			//動画を再生

			//フィルターを設定
			vm.AddCommandDelegater(
				"Filter",
				new CommandDelegater(true, 4, delegate(object[] arguments){

					//変数をキャスト
					string error;
					string type 	= Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;

					float time_ms 	= (float)Converter.ObjectToInt(arguments[1], out error);
					if (error != null) return error;
					float time	= time_ms / 1000.0f;

					Color32 color 	= Converter.ObjectToColor(arguments[2], out error);
					if (error != null) return error;
					float trans 	= Converter.ObjectToFloat(arguments[3], out error);
					if (error != null) return error;
					Color32 adjustedColor = new Color32(color.r, color.g, color.b, (byte)((float)color.a * trans));

					//本題の処理
					UpdaterFormat updater = null;
					switch(type)
					{
					case "fade":
						updater = new MaskAnimation(delegate(float progress) {
							return Mathf.Pow(2.0f, progress) - 1.0f;
						}, new Color32(0,0,0,0xFF), time, this);
						break;
					case "clear":
						updater = new MaskAnimation(delegate(float progress) {
							return 2.0f - Mathf.Pow(2.0f, 1.0f - progress);
						}, new Color32(0,0,0,0), time, this);
						break;
					case "flush":
						updater = new MaskAnimation(delegate(float progress) {
							float per = Mathf.Pow(2.0f, 1.0f - Mathf.Abs(progress * 2.0f - 1.0f)) - 1.0f;
							return per;
						}, adjustedColor, time, this);
						break;
					case "color":
						updater = new MaskAnimation(delegate(float progress) {
							return Mathf.Pow(2.0f, progress) - 1.0f;
						}, adjustedColor, time, this);
						break;
					default:
						return "[Filter] 正しい処理名を指定してください。";
					}
					arguments[4] = updater;
					return null;
				}));

			//動画を呼び出し
			vm.AddCommandDelegater (
				"movie",
				new CommandDelegater (false, 2, delegate(object[] arguments) {
					string error;
					string name = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error;
					string path = mMoviePath + name;

					string skipStr = Converter.ObjectToString(arguments[1], out error);
					if (error != null) return error;

					FullScreenMovieControlMode mode = FullScreenMovieControlMode.Hidden;
					switch(skipStr){
					case "true":
						mode = FullScreenMovieControlMode.CancelOnInput;
						break;
					case "false":
						break;
					default:
						return "skipには true もしくは false を指定してください。";
					}
					Debug.Log("movie:" + path);

					Handheld.PlayFullScreenMovie(path, new Color32(0,0,0,0xFF), mode);
					return null;
				}));
		}
	}
}