using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.PreBattle
{
    public class Unit : MonoBehaviour
    {
        //子プレハブ
        [SerializeField]
        private Text mRace = null;
        [SerializeField]
        private Text mLv = null;
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mSoldier = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private Text mGAtk = null;
        [SerializeField]
        private Text mLAtk = null;
        [SerializeField]
        private GameObject mPositionPrefab = null;
        [SerializeField]
        private Text mPositionText = null;

        [Space(1)]
        [SerializeField]
        private Color mRaceColor_Magic = Color.white;
        [SerializeField]
        private Color mRaceColor_Physics = Color.white;
        [SerializeField]
        private Color mRaceColor_Balance = Color.white;

        [Space(1)]
        [SerializeField]
        private Color mSoldierColor_Max = Color.white;
        [SerializeField]
        private Color mSoldierColor_Normal = Color.white;
        [SerializeField]
        private Color mSoldierColor_Dest = Color.white;
        [SerializeField]
        private Color mSoldierColor_Anni = Color.white;

        [Space(1)]
        [SerializeField]
        private Color mHP_Max = Color.white;
        [SerializeField]
        private Color mHP_Normal = Color.white;
        [SerializeField]
        private Color mHP_Dest = Color.white;


        //プロパティ
        public int UnitID { get; set; }
        public PreBattleController Controller { get; set; }

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
            var unit = game.GameData.Unit[UnitID];

            //もし出撃ユニットに含まれていたら、無効にして、位置を表示
            var battleID = Controller.UnitList.IndexOf(UnitID);
            if (battleID != -1)
            {
                mButton.interactable = false;
                mPositionPrefab.SetActive(true);
                switch (battleID)
                {
                    case 0: mPositionText.text = "前衛"; break;
                    case 1: mPositionText.text = "中衛"; break;
                    case 2: mPositionText.text = "後衛"; break;
                    default: break;
                }
            }
            else
            {
                mButton.interactable = true;
                mPositionPrefab.SetActive(false);
            }


            //テキストをセット
            mName.text = unit.Name;
            mLv.text = unit.Level.ToString();
            SetRace(unit);
            SetSoldierNum(unit);
            SetHP(unit);
            mGAtk.text = (unit.GroupPAtk + unit.GroupMAtk).ToString();
            mLAtk.text = (unit.LeaderPAtk + unit.LeaderMAtk).ToString();

            yield return new WaitForSeconds(0.1f);

            mCoIsRunning = false;
        }

        public void OnClicked()
        {
            var targetID = 2;
            for (int i = 0; i < Controller.UnitList.Count; i++)
            {
                if (Controller.UnitList[i] == -1)
                {
                    targetID = i;
                    break;
                }
            }

            Controller.UnitList[targetID] = UnitID;
            Controller.UnitSetHistory.HistoryAdd(targetID);
            Controller.CancelTargetIsUnit = true;
        }

        private void SetRace(UnitDataFormat unit)
        {
            string text = "";
            Color color;

            //リーダーのステータスから種を判断
            if (unit.LeaderPAtk != 0 &&
                unit.LeaderMAtk != 0)
            {
                text = "万";
                color = mRaceColor_Balance;
            }
            else if (unit.LeaderPAtk == 0)
            {
                text = "魔";
                color = mRaceColor_Magic;
            }
            else
            {
                text = "物";
                color = mRaceColor_Physics;
            }

            mRace.text = text;
            mRace.color = color;
        }

        private void SetSoldierNum(UnitDataFormat unit)
        {
            mSoldier.text = unit.SoldierNum.ToString();

            var num = unit.SoldierNum;
            var max = unit.MaxSoldierNum;
            if (num == max)
            {
                mSoldier.color = mSoldierColor_Max;
            }
            else if (num == 0)
            {
                mSoldier.color = mSoldierColor_Anni;
            }
            else if ((float)num / max < 0.25f)
            {
                mSoldier.color = mSoldierColor_Dest;
            }
            else
            {
                mSoldier.color = mSoldierColor_Normal;
            }
        }

        private void SetHP(UnitDataFormat unit)
        {
            mHP.text = unit.HP.ToString();

            var num = unit.HP;
            var max = unit.MaxHP;
            if (num == max)
            {
                mHP.color = mHP_Max;
            }
            else if ((float)num / max < 0.25f)
            {
                mHP.color = mHP_Dest;
            }
            else
            {
                mHP.color = mHP_Normal;
            }

        }
    }

}