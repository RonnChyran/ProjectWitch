using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class BuyItemInfo : BaseItemInfo
    {
        [SerializeField]
        private Button mBuyButton = null;

        public override void Reset()
        {
            if (ItemID != -1)
            {
                var game = Game.GetInstance();
                var item = game.GameData.Equipment[ItemID];

                var mana = game.GameData.PlayerMana - item.BuyingPrice;
                mNextManaWindow.SetMana(mana);

                //マナが足りない場合は購入できないようにする
                if (mana > 0) mBuyButton.interactable = true;
                else mBuyButton.interactable = false;

                //文字列にアイテム名を差し込む
                mMessageA = mMessageA.Replace("[0]", item.Name);
                mMessageA = mMessageA.Replace("[1]", item.BuyingPrice.ToString());
                mMessageB = mMessageB.Replace("[0]", item.Name);

                //メッセージセット、マナが足りるか足りないかでメッセージが変わる。
                var nextMana = game.GameData.PlayerMana - item.BuyingPrice;
                if (nextMana > 0)
                    mMessageBox.SetText(mMesNameA, mMessageA);
                else
                    mMessageBox.SetText(mMesNameB, mMessageB);
            }

            base.Reset();
        }

        public void ClickBuyButton()
        {
            //プレイヤーのデータに装備データを入れる
            var game = Game.GetInstance();
            game.GameData.Territory[0].EquipmentList[ItemID].Add(-1);

            //マナを減らす
            game.GameData.PlayerMana -= game.GameData.Equipment[ItemID].BuyingPrice;

            //メッセージを表示
            mMessageBox.SetText(mMesNameC, mMessageC);
            
            //データをリセット
            mList.Reset();

            Close();
        }
    }
}