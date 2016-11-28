using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.PreBattle
{
    public class Card : MonoBehaviour
    {

        //プロパティ
        public int CardID { get; set; }
        public PreBattleController Controller { get; set; }
        public CardInfo CardInfo { get; set; }

        //子プレハブ
        [SerializeField]
        private Text mRace = null;
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mSkillName = null;
        [SerializeField]
        private GameObject mPositionPrefab = null;
        [SerializeField]
        private Text mPositionText = null;

        [Space(1)]
        [SerializeField]
        private Color mRaceColor_Damage = Color.white;  //ダメージスキル
        [SerializeField]
        private Color mRaceColor_Heal = Color.white;    //回復スキル
        [SerializeField]
        private Color mRaceColor_Support = Color.white; //補助スキル
        [SerializeField]
        private Color mRaceColor_Summon = Color.white;  //召喚スキル
        [SerializeField]
        private Color mRaceColor_Random = Color.white;  //ランダム

        //内部変数
        private bool mCoIsRunning = false;
        private Button mButton = null;

        // Use this for initialization
        void Start()
        {
            mButton = GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {
            if (!mCoIsRunning)
                StartCoroutine(_Update());
        }

        private IEnumerator _Update()
        {
            mCoIsRunning = true;

            var game = Game.GetInstance();
            var card = game.CardData[CardID];

            //もし使用カードに含まれていたら、無効にして、位置を表示
            var battleID = Controller.CardList.IndexOf(CardID);
            if (battleID != -1)
            {
                mButton.interactable = false;
                mPositionPrefab.SetActive(true);
                switch (battleID)
                {
                    case 0: mPositionText.text = "I"; break;
                    case 1: mPositionText.text = "II"; break;
                    case 2: mPositionText.text = "III"; break;
                    default: break;
                }
            }
            else
            {
                mButton.interactable = true;
                mPositionPrefab.SetActive(false);
            }


            //テキストをセット
            mName.text = card.Name;
            mSkillName.text = game.SkillData[card.SkillID].Name;
            SetRace(card);

            yield return new WaitForSeconds(0.1f);

            mCoIsRunning = false;
        }

        public void OnClicked()
        {
            var targetID = 2;
            for (int i = 0; i < Controller.CardList.Count; i++)
            {
                if (Controller.CardList[i] == -1)
                {
                    targetID = i;
                    break;
                }
            }

            Controller.CardList[targetID] = CardID;
            Controller.CardSetHistory.HistoryAdd(targetID);
            Controller.CancelTargetIsUnit = false;
        }

        public void OnClicked_Info()
        {
            CardInfo.CardID = CardID;
            CardInfo.Show();
        }

        private void SetRace(CardDataFormat card)
        {
            string text = "";
            Color color;

            //スキルを取得
            var skill = Game.GetInstance().SkillData[card.SkillID];

            //リーダーのステータスから種を判断
            switch (skill.Type)
            {
                case SkillDataFormat.SkillType.Damage:
                    text = "攻撃";
                    color = mRaceColor_Damage;
                    break;
                case SkillDataFormat.SkillType.Heal:
                    text = "回復";
                    color = mRaceColor_Heal;
                    break;
                case SkillDataFormat.SkillType.Summon:
                    text = "召喚";
                    color = mRaceColor_Summon;
                    break;
                case SkillDataFormat.SkillType.Random:
                    text = "不明";
                    color = mRaceColor_Random;
                    break;
                default:
                    text = "補助";
                    color = mRaceColor_Support;
                    break;
            }

            mRace.text = text;
            mRace.color = color;
        }
    }
}