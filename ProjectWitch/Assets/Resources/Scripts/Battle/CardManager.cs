using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ProjectWitch.Battle
{
    public class CardManager : MonoBehaviour
    {
        private Game mGame;
        public GameObject CardObj { get; private set; }
        public GameObject Flame { get { return CardObj.transform.FindChild("Flame").gameObject; } }
        public CardDataFormat CardData { get; private set; }
        public SkillDataFormat Skill { get; private set; }
        public int ID { get; private set; }
        public int Duration { get; private set; }
        public bool IsCanUse { get; private set; }
		public bool IsPlayer { get; private set; }

		// 初期設定
		public void Setup(int id, bool isPlayer, GameObject cardObj)
        {
            ID = id;
			IsPlayer = isPlayer;
            mGame = Game.GetInstance();
            CardObj = cardObj;
            CardObj.SetActive(id != -1);
            Flame.SetActive(false);
            if (id == -1)
            {
                CardData = null;
                return;
            }
            CardData = mGame.GameData.Card[id];
            Skill = mGame.GameData.Skill[CardData.SkillID];
            Duration = CardData.Duration;
            CardObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Card/" + CardData.ImageBack);
            IsCanUse = true;
		}

        // カード1枚消費
        public void SetUsedCard()
        {
			if (Duration != -1)
            {
                if (Duration == CardData.Duration)
                {
                    // 初めての使用の場合
                    mGame.BattleOut.UsedCards.Add(ID);
                }
                Duration--;
                if (Duration <= 0)
                {
                    IsCanUse = false;
                    // 表示処理
                    CardObj.SetActive(false);
                }
            }
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