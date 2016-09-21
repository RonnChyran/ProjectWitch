using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Field
{
    public class FlagButton : FieldButtonBase
    {

        //地点番号
        public int AreaID { get; set; }

        //各メニューのプレハブ
        [SerializeField]
        private GameObject mPlayerMenu;

        [SerializeField]
        private GameObject mEnemyMenuA;

        [SerializeField]
        private GameObject mEnemyMenuB;


        //プレイヤーの領地メニューを開く
        public void OpenPlayerMenu()
        {
            //フラグメニューが開けるかどうか
            if (!mFieldController.FlagClickable) return;

            //メニューを開く
            ShowMenu(mPlayerMenu);

            //フラグメニューを開けないようにする
            mFieldController.FlagClickable = false;
        }

        //敵の領地メニューを開く
        //領地が味方領地に隣接していたら地点制圧コマンドが追加される
        public void OpenEnemyMenu()
        {
            //フラグメニューが開けるかどうか
            if (!mFieldController.FlagClickable) return;

            var game = Game.GetInstance();

            //地点が隣接しているかどうか判定
            var nextAreas = new List<int>();
            {
                //隣接地点の取得
                foreach (var area in game.TerritoryData[0].AreaList)
                {
                    nextAreas.AddRange(game.AreaData[area].NextArea);
                }
                //重複を削除
                nextAreas = nextAreas.Distinct().ToList();
            }

            if (nextAreas.Contains(AreaID))
                //隣接していたら戦闘ありのメニューを呼ぶ
                ShowMenu(mEnemyMenuA);
            else
                //していなかったら戦闘なしのメニューを呼ぶ
                ShowMenu(mEnemyMenuB);

            //フラグメニューを開けないようにする
            mFieldController.FlagClickable = false;
        }

        protected override void ShowMenu(GameObject menu)
        {
            base.ShowMenu(menu);

            mInst.GetComponent<FlagMenu>().AreaID = AreaID;
        }
    }
}
