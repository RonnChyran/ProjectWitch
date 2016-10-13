using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Battle
{
	// 行動順の表示オブジェクト単体を管理するクラス
	public class OrderDiplayObj : MonoBehaviour
	{
		// 基本の箱
		private RectTransform mBaseRect;

		public BattleObj BattleObj { get; private set; }
		// 管理するUnit
		public BattleUnit BattleUnit { get; private set; }
		// アニメーション中かどうか
		public bool IsAnimation { get; private set; }
		// 現在の場所
		public int Pos { get; private set; }
		// 自身の箱
		public RectTransform Rect { get { return GetComponent<RectTransform>(); } }

		// セットアップ
		public void Setup(BattleUnit battleUnit, RectTransform baseRect)
		{
			BattleUnit = battleUnit;
			BattleObj = GameObject.Find("/BattleObject").GetComponent<BattleObj>();
			mBaseRect = baseRect;

			var child = transform.FindChild("Text").GetComponent<Text>();
			child.text = BattleUnit.UnitData.Name;

			Rect.localPosition = mBaseRect.localPosition + new Vector3(mBaseRect.sizeDelta.x * OrderController.DisplayOutCount, 0, 0);
			Rect.localScale = mBaseRect.localScale;
		}

		// スライドさせるコルーチン
		private IEnumerator CoSlideToPos(int pos)
		{
			IsAnimation = true;
			var targetPosX = mBaseRect.localPosition.x + mBaseRect.sizeDelta.x * pos;
			float speedPerSec = 1200f * BattleObj.BattleSpeedMagni;
			while (pos < Pos ? Rect.localPosition.x > targetPosX : Rect.localPosition.x < targetPosX)
			{
				Rect.localPosition -= new Vector3(speedPerSec * Time.deltaTime * (pos < Pos ? 1 : -1), 0, 0);
				yield return null;
			}
			Rect.localPosition = mBaseRect.localPosition + new Vector3(mBaseRect.sizeDelta.x * pos, 0, 0);
			Pos = pos;
			IsAnimation = false;
		}

		// スライドしながら入らせる
		public void SlideIn(int pos)
		{
			Rect.localPosition = mBaseRect.localPosition + new Vector3(mBaseRect.sizeDelta.x * OrderController.DisplayOutCount, 0, 0);
			Pos = OrderController.DisplayOutCount;
			StartCoroutine("CoSlideToPos", pos);
		}

		// 該当位置までスライドさせる
		public void SlideToPos(int pos)
		{
			if (pos == Pos)
				return;
			StartCoroutine("CoSlideToPos", pos);
		}

		// 画面上に出すコルーチン
		private IEnumerator CoSlideUpOut()
		{
			IsAnimation = true;
			var targetPosY = mBaseRect.localPosition.y + mBaseRect.sizeDelta.y;
			float speedPerSec = 100f * BattleObj.BattleSpeedMagni;
			while (Rect.localPosition.y < targetPosY)
			{
				Rect.localPosition += new Vector3(0, speedPerSec * Time.deltaTime, 0);
				yield return null;
			}
			IsAnimation = false;
		}

		// 画面上に出す
		public void SlideUpOut()
		{
			StartCoroutine("CoSlideUpOut");
		}

		// 画面上から入らせるコルーチン
		private IEnumerator CoSlideUpIn(int pos)
		{
			IsAnimation = true;
			Rect.localPosition = new Vector3(mBaseRect.localPosition.x + mBaseRect.sizeDelta.x * pos,
				mBaseRect.localPosition.y + mBaseRect.sizeDelta.y, mBaseRect.localPosition.z);
			float speedPerSec = 100f * BattleObj.BattleSpeedMagni;
			while (Rect.localPosition.y > mBaseRect.localPosition.y)
			{
				Rect.localPosition -= new Vector3(0, speedPerSec * Time.deltaTime, 0);
				yield return null;
			}
			Rect.localPosition = new Vector3(mBaseRect.localPosition.x + mBaseRect.sizeDelta.x * pos,
				mBaseRect.localPosition.y, mBaseRect.localPosition.z);
			Pos = pos;
			IsAnimation = false;
		}

		// 画面上から入らせる
		public void SlideUpIn(int pos)
		{
			StartCoroutine("CoSlideUpIn", pos);
		}

		// 画面左に出すコルーチン
		private IEnumerator CoSlideLeftOut()
		{
			IsAnimation = true;
			var canvas = transform.parent.parent.GetComponent<CanvasScaler>();
			var targetPosX = -(canvas.referenceResolution.x + mBaseRect.sizeDelta.x) / 2;
			float speedPerSec = 1200f * BattleObj.BattleSpeedMagni;
			while (Rect.localPosition.x > targetPosX)
			{
				Rect.localPosition -= new Vector3(speedPerSec * Time.deltaTime, 0, 0);
				yield return null;
			}
			Rect.localPosition = new Vector3(targetPosX, mBaseRect.localPosition.y, mBaseRect.localPosition.z);
			IsAnimation = false;
		}

		// 画面左に出す
		public void SlideLeftOut()
		{
			StartCoroutine("CoSlideLeftOut");
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}

}
