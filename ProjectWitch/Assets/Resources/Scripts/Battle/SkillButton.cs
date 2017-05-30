using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProjectWitch.Battle
{
	// スキルのボタン制御
	public class SkillButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private int mType = 0;

		private Vector3 mBasePos;
		private Vector2 mBaseSize;

		public FaceObj BaseFace { get; private set; }

		// セットアップ
		public void Setup(FaceObj face)
		{
			BaseFace = face;
			mBasePos = gameObject.GetComponent<RectTransform>().localPosition;
			mBaseSize = gameObject.GetComponent<RectTransform>().sizeDelta;
		}

		// ボタンの名称等を設定する
		public void SetButton(string name, int pos)
		{
			var text = transform.Find("Text").GetComponent<Text>();
			text.text = name;
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3(mBasePos.x,
				mBasePos.y + mBaseSize.y * pos * 1f, mBasePos.z);
			gameObject.SetActive(true);
		}

		// ボタンを非表示にする
		public void HideButton()
		{
			gameObject.SetActive(false);
		}

		// ボタンが押された時の動作
		public void PushButton()
		{
			BaseFace.BattleObj.PushSkillButton(BaseFace.Unit, mType);
		}

		// マウスオーバーした時
		public void OnPointerEnter(PointerEventData e)
		{
			// 音を鳴らす
			BaseFace.Music.PlayOverSkillButton();
			BaseFace.BattleObj.MouseOverSkillButton(mType);
		}

		// マウスが離されたとき
		public void OnPointerExit(PointerEventData e)
		{
			BaseFace.BattleObj.HideDescription();
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
