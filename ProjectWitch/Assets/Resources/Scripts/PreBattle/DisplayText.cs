using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.PreBattle
{
    public class DisplayText : MonoBehaviour
    {

        [Header("テキストコンポーネント")]
        [SerializeField]
        private Text mLv = null;
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private Text mSoldier = null;
        [SerializeField]
        private Text mSkill = null;

        //プロパティ
        private int mUnitID = -1;
        public int UnitID { get { return mUnitID; } set { mUnitID = value; Reset(); } }

        private void Start()
        {
            UnitID = -1;
        }

        private void Reset()
        {
            //テキストの更新
            if (UnitID == -1)
            {
                mLv.text = mName.text = mHP.text
                    = mSoldier.text = mSkill.text = "";
            }
            else
            {
                var game = Game.GetInstance();
                var unit = game.GameData.Unit[UnitID];
                var atkSkill = game.GameData.Skill[unit.LAtkSkill];
                var defSkill = game.GameData.Skill[unit.LDefSkill];

                mLv.text = "Lv." + unit.Level.ToString();
                mName.text = unit.Name;
                mHP.text = "HP " + unit.HP.ToString() +
                    "/" + unit.MaxHP.ToString();
                mSoldier.text = "Soldier " + unit.SoldierNum.ToString() +
                    "/" + unit.MaxSoldierNum.ToString();
                mSkill.text = "Skill\n　A:" + atkSkill.Name +
                    "\n　D:" + defSkill.Name;
            }
        }
    }
}