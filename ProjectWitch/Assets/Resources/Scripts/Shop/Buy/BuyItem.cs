using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    public class BuyItem : BaseShopItem
    {
        //所持数
        [SerializeField]
        private Text mNum = null;

        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();
            var itemList = game.GameData.Territory[0].EquipmentList;

            //個数をセット
            var num = itemList[ItemID].Count;
            mNum.text = num.ToString();
        }

        public override void OnClicked()
        {
            base.OnClicked();

            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];

            //文字列にアイテム名を差し込む
            mMessageA = mMessageA.Replace("[0]", item.Name);
            mMessageA = mMessageA.Replace("[1]", item.BuyingPrice.ToString());
            mMessageB = mMessageB.Replace("[0]", item.Name);

            //メッセージセット、マナが足りるか足りないかでメッセージが変わる。
            var nextMana = game.GameData.PlayerMana - item.BuyingPrice;
            if (nextMana > 0)
                MesBox.SetText(mMesNameA, mMessageA);
            else
                MesBox.SetText(mMesNameB, mMessageB);
        }
    }
}