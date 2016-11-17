using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using GameData;

namespace PreBattle
{
    public class CardInfo : MonoBehaviour
    {
        [SerializeField]
        private Text mName = null;

        [SerializeField]
        private Text mSkill = null;

        [SerializeField]
        private Text mTiming = null;

        [SerializeField]
        private Text mUseCount = null;

        [SerializeField]
        private Text mTarget = null;

        [SerializeField]
        private Text mScope = null;

        [SerializeField]
        private Text mComment = null;

        //プロパティ
        public int CardID { get; set; }

        public void Show()
        {
            GetComponent<Canvas>().enabled = true;

            //テキストセット
            var game = Game.GetInstance();
            var card = game.CardData[CardID];
            var skill = game.SkillData[card.SkillID];

            mName.text = card.Name;
            mSkill.text = skill.Name;
            mUseCount.text = (card.Duration==-1) ? "∞" : card.Duration.ToString();
            mComment.text = card.Description;

            //ターゲット
            switch (skill.Target)
            {
                case SkillDataFormat.SkillTarget.Player:
                    mTarget.text = "味方";
                    break;
                case SkillDataFormat.SkillTarget.Enemy:
                    mTarget.text = "敵";
                    break;
                case SkillDataFormat.SkillTarget.PlayerLeader:
                    mTarget.text = "味方隊長";
                    break;
                case SkillDataFormat.SkillTarget.EnemyLeader:
                    mTarget.text = "敵隊長";
                    break;
                case SkillDataFormat.SkillTarget.EnemyAndPlayer:
                    mTarget.text = "敵味方";
                    break;
                case SkillDataFormat.SkillTarget.Own:
                    mTarget.text = "自身";
                    break;
            }

            //効果範囲
            switch(skill.Range)
            {
                case SkillDataFormat.SkillRange.All:
                    mTarget.text = "全体";
                    break;
                case SkillDataFormat.SkillRange.Single:
                    mTarget.text = "単体";
                    break;
            }

            //タイミング
            switch(card.Timing)
            {
                case CardDataFormat.CardTiming.BattleBegin:
                    mTiming.text = "戦闘開始時";
                    break;
                case CardDataFormat.CardTiming.BattleEnd:
                    mTiming.text = "戦闘終了";
                    break;
                case CardDataFormat.CardTiming.EnemyState_Death:
                    mTiming.text = "敵ユニットの\n死亡";
                    break;
                case CardDataFormat.CardTiming.EnemyState_HP10:
                    mTiming.text = "敵ユニットの\nHPが10%以下";
                    break;
                case CardDataFormat.CardTiming.EnemyState_HP50:
                    mTiming.text = "敵ユニットの\nHPが50%以下";
                    break;
                case CardDataFormat.CardTiming.EnemyState_Poison:
                    mTiming.text = "敵ユニットが\n毒にかかった";
                    break;
                case CardDataFormat.CardTiming.EnemyState_S10:
                    mTiming.text = "敵ユニットの\n兵士数が10%以下";
                    break;
                case CardDataFormat.CardTiming.EnemyState_S50:
                    mTiming.text = "敵ユニットの\n兵士数が50%以下";
                    break;
                case CardDataFormat.CardTiming.UserState_Death:
                    mTiming.text = "自軍ユニット\nの死亡";
                    break;
                case CardDataFormat.CardTiming.UserState_HP10:
                    mTiming.text = "自軍ユニットの\nHPが10%以下";
                    break;
                case CardDataFormat.CardTiming.UserState_HP50:
                    mTiming.text = "自軍ユニットの\nHPが50%以下";
                    break;
                case CardDataFormat.CardTiming.UserState_Poison:
                    mTiming.text = "自軍ユニットの\n毒にかかった";
                    break;
                case CardDataFormat.CardTiming.UserState_S10:
                    mTiming.text = "自軍ユニットの\n兵士数が10%以下";
                    break;
                case CardDataFormat.CardTiming.UserState_S50:
                    mTiming.text = "自軍ユニットの\n兵士数が50%以下";
                    break;
                case CardDataFormat.CardTiming.Rand20:
                    mTiming.text = "20%の確率で\nランダム";
                    break;
                case CardDataFormat.CardTiming.Rand50:
                    mTiming.text = "50%の確率で\nランダム";
                    break;
                case CardDataFormat.CardTiming.Rand80:
                    mTiming.text = "80%の確率で\nランダム";
                    break;
            }
        }

        public void Close()
        {
            GetComponent<Canvas>().enabled = false;
        }
    }
}