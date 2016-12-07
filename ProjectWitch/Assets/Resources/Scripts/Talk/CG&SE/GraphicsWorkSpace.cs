//=====================================
//author	:shotta
//summary	:グラフィックの作業場
//=====================================

using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Talk.WorkSpace
{
    public class GraphicsWorkSpace : MonoBehaviour
	{
        [SerializeField]
        ScenarioWorkSpace mSWS = null;

        //キャンバス
        [SerializeField]
        private RectTransform mCanvas = null;
		private Rect CanvasRect{
			get{ return mCanvas.rect; }
		}

        //背景のパス
        [SerializeField]
        private string mBackgroundPath = null;
        //背景用スプライト
        [SerializeField]
        private GameObject mBackgroundSprite = null;

		//背景の不透明度(最初の段階で1にする)
		private bool mNeedsToFadeBackGround = true;

        //立ち絵のパス
        [SerializeField]
        private string mCGPath = null;
        //立ち絵を表示させるレイヤー
        [SerializeField]
        private StandCGController mCGLayer = null;
        //立ち絵のアンカー
        [SerializeField]
        private Transform mStandCGAnchor1 = null;
        [SerializeField]
        private Transform mStandCGAnchor2 = null;
        [SerializeField]
        private Transform mStandCGAnchor3 = null;
        [SerializeField]
        private Transform mStandCGAnchor4 = null;
        [SerializeField]
        private Transform mStandCGAnchor5 = null;
        [SerializeField]
        private Transform mEventCGAnchor = null;

		//立ち絵の位置用のアップデータ
		private class CGAnimationUpdater : UpdaterFormat
		{
			public delegate void AnimationDelegate(float progress);

			private float mTime;
			private AnimationDelegate mMethod;
			private float mDuration;
			private CGAnimationUpdater(){}
			public CGAnimationUpdater(float duration, AnimationDelegate method)
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
				mMethod (Mathf.Clamp(mTime/mDuration, 0f, 1f));
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
        private Image mMask = null;

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
		//[SerializeField]
		//private string mMoviePath = "";

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
					if (mNeedsToFadeBackGround)
					{
						float time = 0.5f;
						RawImage img = mBackgroundSprite.GetComponent<RawImage>();
						Color prevColor = img.color;
						mSWS.SetUpdater(new CGAnimationUpdater(time, delegate(float progress) {
							img.color = new Color(prevColor.r, prevColor.g, prevColor.b, progress);
						}));
						mNeedsToFadeBackGround = true;
					}
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
				new CommandDelegater(true, 9, delegate(object[] arguments){
					string error;
					int id = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					string pos = Converter.ObjectToString(arguments[1], out error);
					if (error != null) return error;
                    string dir = Converter.ObjectToString(arguments[2], out error);
                    if (error != null) return error;
					string mode = Converter.ObjectToString(arguments[3], out error);
					if (error != null) return error;
					string layer = Converter.ObjectToString(arguments[4], out error);
                    if (error != null) return error;
                    string posx = Converter.ObjectToString(arguments[5], out error);
                    if (error != null) return error;
                    string posy = Converter.ObjectToString(arguments[6], out error);
                    if (error != null) return error;
                    string posz = Converter.ObjectToString(arguments[7], out error);
                    if (error != null) return error;
                    string state = Converter.ObjectToString(arguments[8], out error);
                    if (error != null) return error;


                    Rect canvasRect = CanvasRect;
					float x = 1.5f * canvasRect.width;
					Vector3 position_prev = new Vector3(0.0f, 0.0f, 0.0f);
					Vector3 position_next = new Vector3(0.0f, 0.0f, 0.0f);
					switch (pos) {
                    case "event":
                        pos = "6";
                        break;
					case "right":
						pos = "4";
						break;
					case "center":
						pos = "3";
						break;
					case "left":
						pos = "2";
						break;
					default:
						break;
					}
					switch (pos) {
                    case "6":
                        position_prev = new Vector3(x, 0.0f, 0.0f);
                        position_next = new Vector3(mEventCGAnchor.localPosition.x, 0.0f, 0.0f);
                        break;
                    case "5":
						position_prev = new Vector3(x, 0.0f, 0.0f);
						position_next = new Vector3(mStandCGAnchor5.localPosition.x, 0.0f, 0.0f);
						break;
					case "4":
						position_prev = new Vector3(x, 0.0f, 0.0f);
						position_next = new Vector3(mStandCGAnchor4.localPosition.x, 0.0f, 0.0f);
						break;
					case "3":
						position_prev = new Vector3(x, 0.0f, 0.0f);
						position_next = new Vector3(mStandCGAnchor3.localPosition.x, 0.0f, 0.0f);
						break;
					case "2":
						position_prev = new Vector3(-x, 0.0f, 0.0f);
						position_next = new Vector3(mStandCGAnchor2.localPosition.x, 0.0f, 0.0f);
						break;
					case "1":
						position_prev = new Vector3(-x, 0.0f, 0.0f);
						position_next = new Vector3(mStandCGAnchor1.localPosition.x, 0.0f, 0.0f);
						break;
					default:
						return "正しい位置を指定してください。";
					}

					bool isShowFront;
					switch (layer)
					{
					case "front":
						isShowFront = true;
						break;
					case "back":
						isShowFront = false;
						break;
					default:
						return "正しいモードを指定してください。";
					}
					position_next = mCGLayer.GetUnduplicatePosition(position_next);

					mCGLayer.ShowStandCG(id, isShowFront, out error);
					if (error != null) return error;

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
							updater = new CGAnimationUpdater(time, delegate(float progress) {
								trans.localPosition = position_prev * (1.0f - progress) + position_next * progress;
							});
						}
						break;

					case "fadein":
						{
							Transform trans = obj.transform;
							trans.localPosition = position_next;
							RawImage image = obj.GetComponent<RawImage> ();
							updater = new CGAnimationUpdater(time, delegate(float progress) {
								Color cp = image.color;
								image.color = new Color(cp.r, cp.g, cp.b, progress);
							});
						}
						break;

					default:
						return "正しいモードを指定してください。";
					}

					arguments[4] = updater;
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
							updater = new CGAnimationUpdater(time, delegate(float progress) {
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
							updater = new CGAnimationUpdater(time, delegate(float progress) {
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

					UpdaterFormat transUpdater = new CGAnimationUpdater(time, delegate(float progress) {
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

					UpdaterFormat transUpdater = new CGAnimationUpdater(time, delegate(float progress) {
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

					UpdaterFormat transUpdater = new CGAnimationUpdater(time, delegate(float progress) {
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

					UpdaterFormat transUpdater = new CGAnimationUpdater(time, delegate(float progress) {
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
		}
	}
}