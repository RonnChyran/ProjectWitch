using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace ProjectWitch.Battle
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
            cardFront.sprite = Resources.Load<Sprite>("Textures/Card/" + card.ImageFront);
            cardBack.fillAmount = 0;
            cardFrontR.position = cardObjR.position;
            cardFrontR.localScale = cardObjR.localScale;
            cardFrontR.sizeDelta = cardObjR.sizeDelta;
            float time = 0;
            var parPos = (cardBackR.position - cardFrontR.position) / mCardMoveTime;
            var parSize = (cardBackR.sizeDelta - cardFrontR.sizeDelta) / mCardMoveTime;
            var panelColorA = panel.color.a;
            var parColorA = panel.color.a / mCardMoveTime;
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, 0);
            while (time < mCardMoveTime)
            {
                cardFrontR.position += parPos * Time.deltaTime;
                cardFrontR.sizeDelta += parSize * Time.deltaTime;
                panel.color += new Color(0, 0, 0, parColorA * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            cardFrontR.position = cardBackR.position;
            cardFrontR.sizeDelta = cardBackR.sizeDelta;
            panel.color = new Color(panel.color.r, panel.color.g, panel.color.b, panelColorA);

            time = 0;
            while (time < mCardMoveTime)
            {
				cardBack.fillAmount += 1.0f / mCardOpenTime * Time.deltaTime;
                time += Time.deltaTime;
                yield return null;
            }
			cardBack.fillAmount = 1.0f;
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