using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace PreBattleOld
{
    public class RecieveCard : RecieveObject
    {

        //カード番号
        [SerializeField]
        private int mCardID = -1;
        public int CardID { get { return mCardID; } set { mCardID = value; } }

        //use this for initialization
        protected override void Start()
        {
            base.Start();

            DataUpdate();
        }

        //ドロップ時の処理
        public override void OnDrop(PointerEventData e)
        {
            base.OnDrop(e);

            //数値の更新
            //DragCardがつけられた場合と,RecieveCardが移動してきた場合
            var preBattleObjD = e.pointerDrag.GetComponent<DragCard>();
            var preBattleObjR = e.pointerDrag.GetComponent<RecieveCard>();

            if (preBattleObjD)      //ドラッグされたのがDragUnitだった場合
            {
                CardID = preBattleObjD.CardID;
            }
            else if (preBattleObjR) //ドラッグされたのがRevieveUnitだった場合
            {
                CardID = preBattleObjR.CardID;
            }

            DataUpdate();
        }

        public override void OnEndDrag(PointerEventData e)
        {
            base.OnEndDrag(e);

            //数値の更新
            CardID = -1;
            DataUpdate();
        }

        public void DataUpdate()
        {
            var game = Game.GetInstance();

            //battleinを更新
            game.BattleIn.PlayerCards[Position] = CardID;
        }
    }
}