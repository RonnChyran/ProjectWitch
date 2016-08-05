using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace PreBattle
{
    public class RecieveUnit : RecieveObject
    {
        //ユニット番号
        [SerializeField]
        private int mUnitID = -1;
        public int UnitID { get { return mUnitID; } set { mUnitID = value; } }

        private Text mHP;
        private Text mSoldier;
        private Text mName;
        
        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            mHP = transform.FindChild("HP").gameObject.GetComponent<Text>();
            mSoldier = transform.FindChild("Soldier").gameObject.GetComponent<Text>();
            mName = transform.FindChild("Name").gameObject.GetComponent<Text>();

            DataUpdate();
        }

        public override void OnDrop(PointerEventData e)
        {
            base.OnDrop(e);
            
            //数値の更新
            //DragUnitがつけられた場合と,RecieveUnitが移動してきた場合
            var preBattleObjD = e.pointerDrag.GetComponent<DragUnit>();
            var preBattleObjR = e.pointerDrag.GetComponent<RecieveUnit>();

            if (preBattleObjD)      //ドラッグされたのがDragUnitだった場合
            {
                UnitID = preBattleObjD.UnitID;
            }
            else if (preBattleObjR) //ドラッグされたのがRevieveUnitだった場合
            {
                UnitID = preBattleObjR.UnitID;
            }

            DataUpdate();
        }

        public override void OnEndDrag(PointerEventData e)
        {
            base.OnEndDrag(e);

            //数値の更新
            UnitID = -1;
            DataUpdate();
        }

        //データの更新
        public void DataUpdate()
        {
            var game = Game.GetInstance();

            if (UnitID < 0)
            {
                mName.text = "";
                mSoldier.text = "";
                mHP.text = "";
            }
            else
            {
                mName.text = game.UnitData[UnitID].Name;
                mSoldier.text = "S:" + game.UnitData[UnitID].SoldierNum.ToString();
                mHP.text = "H:" + game.UnitData[UnitID].HP.ToString();
            }

            //BattleInを更新
            game.BattleIn.PlayerUnits[Position] = UnitID;
        }
    }
}