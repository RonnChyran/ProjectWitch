using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
	// スキルのボタン制御
	public class SkillButton : MonoBehaviour
	{
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
			var text = transform.FindChild("Text").GetComponent<Text>();
			text.text = name;
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3(mBasePos.x,
				mBasePos.y - mBaseSize.y * pos * 1.2f, mBasePos.z);
			gameObject.SetActive(true);
		}

		// ボタンを非表示にする
		public void HideButton()
		{
			gameObject.SetActive(false);
		}

		// ボタンが押された時の動作
		public void PushButton(int type)
		{
			BaseFace.BattleObj.PushSkillButton(BaseFace.Unit, type);
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
