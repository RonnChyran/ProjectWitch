using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Shop.Tool
{
    public class SellList : BaseList
    {
        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();

            //子供を追加
            var equipmentList = game.GameData.Territory[0].EquipmentList;
            for (int i = 0; i < equipmentList.Count; i++)
            {
                for (int j = 0; j < equipmentList[i].Count; j++)
                {
                    var inst = Instantiate(mListContent);
                    var cp = inst.GetComponent<SellItem>();
                    cp.ItemID = i;
                    cp.ItemUniID = j;
                    cp.InfoWindow = mInfoWindow;
                    cp.MesBox = mMessageBox;
                    cp.Reset();

                    inst.transform.SetParent(mListContentParent.transform, false);
                }
            }
        }
    }
}