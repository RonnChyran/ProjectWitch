using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using GameData;

namespace Battle
{
	public class CardStartUI : MonoBehaviour
	{
		[SerializeField]
		private GameObject mPanel = null, mCardBack = null, mCardFront = null, mName = null, mExposition = null;
		[SerializeField]
		private float mCardMoveTime = 0.125f, mCardOpenTime = 0.125f, mWaitTime = 0.75f;

		public BattleObj BattleObj { get; private set; }

		private IEnumerator CoCardStart(CardDataFormat card, GameObject cardObj)
		{
			int count = 50;
			float parTime = mCardMoveTime / count;
			var panel = mPanel.GetComponent<Image>();
			var cardBack = mCardBack.GetComponent<Image>();
			var cardFront = mCardFront.GetComponent<Image>();
			var cardBackR = mCardBack.GetComponent<RectTransform>();
			var cardFrontR = mCardFront.GetComponent<RectTransform>();
			var cardObjR = cardObj.GetComponent<RectTransform>();
			var name = mName.GetComponent<Text>();
			var exposition = mExposition.GetComponent<Text>();
			var parPanelA = panel.color.a / count;
			panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
			name.text = "";
			exposition.text = "";
			cardBack.sprite = Resources.Load<Sprite>("Textures/Card/" + card.ImageBack);
			cardFront.sprite= Resources.Load<Sprite>("Textures/Card/" + card.ImageFront);
			cardFront.fillAmount = 0;
			cardBackR.position = cardObjR.position;
			cardBackR.localScale = cardObjR.localScale;
			cardBackR.sizeDelta = cardObjR.sizeDelta;
			var parPos = (cardFrontR.position - cardBackR.position) / count;
			var parSize = (cardFrontR.sizeDelta - cardBackR.sizeDelta) / count;
			for (int i = 0; i < count; i++)
			{
				cardBackR.position += parPos;
				cardBackR.sizeDelta += parSize;
				panel.color += new Color(0, 0, 0, parPanelA);
				yield return BattleObj.WaitSeconds(parTime);
			}
			float parFillAmount = 1.0f / count;
			parTime = mCardOpenTime / count;
			for (int i = 0; i < count; i++)
			{
				cardFront.fillAmount += parFillAmount;
				yield return BattleObj.WaitSeconds(parTime);
			}
			name.text = card.Name;
			exposition.text = card.Description;
			yield return BattleObj.WaitInputOrSeconds(mWaitTime);
		}

		public IEnumerator CardStart(CardDataFormat card, GameObject cardObj)
		{
			if (!BattleObj)
				BattleObj = GameObject.Find("/BattleObject").GetComponent<BattleObj>();
			yield return StartCoroutine(CoCardStart(card, cardObj));
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

