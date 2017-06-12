using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    public class BaseShopItem : MonoBehaviour
{
        //商品のID（装備のID）
        public int ItemID { get; set; }

        //商品名
        [SerializeField]
        private Text mName = null;

        //価格
        [SerializeField]
        protected Text mPrice = null;

        //商品の情報ウィンドウ
        public BaseItemInfo InfoWindow { get; set; }

        //メッセージウィンドウ
        public MessageBox MesBox { get; set; }

        public virtual void Reset()
        {
            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];

            //テキストをセット
            mName.text = item.Name;
        }

        public virtual void OnClicked()
        {
            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];

            InfoWindow.ItemID = ItemID;
            InfoWindow.Reset();
        }
    }
}
