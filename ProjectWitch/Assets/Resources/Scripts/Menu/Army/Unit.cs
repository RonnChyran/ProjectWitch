using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class Unit : MonoBehaviour
    {
        //子プレハブ
        [SerializeField]
        private Text mRace = null;
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mSoldier = null;
        [SerializeField]
        private Text mHP = null;

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
        public StatusWindow StatusWindow { get; set; }

        //内部変数
        private bool mCoIsRunning = false;

        // Use this for initialization
        void Start()
        {

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
            var unit = game.UnitData[UnitID];

            //テキストをセット
            mName.text = unit.Name;
            SetRace(unit);
            SetSoldierNum(unit);
            SetHP(unit);


            yield return new WaitForSeconds(0.1f);

            mCoIsRunning = false;
        }

        public void OnClicked()
        {
            StatusWindow.UnitID = UnitID;
            StatusWindow.Reset();
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
            if(num == max)
            {
                mSoldier.color = mSoldierColor_Max;
            }
            else if(num == 0)
            {
                mSoldier.color = mSoldierColor_Anni;
            }
            else if((float)num/max < 0.25f)
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
            else if ((float)num/max < 0.25f)
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