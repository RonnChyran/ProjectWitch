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
			var panel = mPanel.GetComponent<Image>();
			var cardBack = mCardBack.GetComponent<Image>();
			var cardFront = mCardFront.GetComponent<Image>();
			var cardBackR = mCardBack.GetComponent<RectTransform>();
			var cardFrontR = mCardFront.GetComponent<RectTransform>();
			var cardObjR = cardObj.GetComponent<RectTransform>();
			var name = mName.GetComponent<Text>();
			var exposition = mExposition.GetComponent<Text>();
			name.text = "";
			exposition.text = "";
			cardBack.sprite = Resources.Load<Sprite>("Textures/Card/" + card.ImageBack);
			cardFront.sprite= Resources.Load<Sprite>("Textures/Card/" + card.ImageFront);
			cardFront.fillAmount = 0;
			cardBackR.position = cardObjR.position;
			cardBackR.localScale = cardObjR.localScale;
			cardBackR.sizeDelta = cardObjR.sizeDelta;
			float time = 0;
			var parPos = (cardFrontR.position - cardBackR.position) / mCardMoveTime;
			var parSize = (cardFrontR.sizeDelta - cardBackR.sizeDelta) / mCardMoveTime;
			var panelColorA = panel.color.a;
			var parColorA = panel.color.a / mCardMoveTime;
			panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
			while (time < mCardMoveTime)
			{
				cardBackR.position += parPos * Time.deltaTime;
				cardBackR.sizeDelta += parSize * Time.deltaTime;
				panel.color += new Color(0, 0, 0, parColorA * Time.deltaTime);
				time += Time.deltaTime;
				yield return null;
			}
			cardBackR.position = cardFrontR.position;
			cardBackR.sizeDelta = cardFrontR.sizeDelta;
			panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panelColorA);

			time = 0;
			while(time < mCardMoveTime)
			{
				cardFront.fillAmount += 1.0f / mCardOpenTime * Time.deltaTime;
				time += Time.deltaTime;
				yield return null;
			}
			cardFront.fillAmount = 1.0f;
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

