using UnityEngine.UI;
using UnityEngine;

namespace ProjectWitch.Shop
{
    public class SellItem : BaseShopItem
    {
        //プレイヤーが所持するアイテムの識別用ID(複数のアイテムIDを所持するための識別用)
        public int ItemUniID { get; set; }

        //所持数
        [SerializeField]
        private Text mIsEquipment = null;

        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];
            var itemList = game.GameData.Territory[0].EquipmentList;
  
            //誰かが装備していないかチェック
            var isEquipment = itemList[ItemID][ItemUniID] == -1 ? false : true;
            if (isEquipment) mIsEquipment.text = "E";
            else mIsEquipment.text = "";
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