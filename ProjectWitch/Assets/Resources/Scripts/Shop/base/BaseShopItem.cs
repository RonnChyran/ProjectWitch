using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    public class BaseShopItem : MonoBehaviour
{
        //選択時のメッセージ
        [SerializeField]
        protected string mMesNameA = "";
        [SerializeField, Multiline]
        protected string mMessageA = "";

        //選択時、マナが足りない場合のメッセージ
        [SerializeField]
        protected string mMesNameB = "";
        [SerializeField, Multiline]
        protected string mMessageB = "";

        //商品のID（装備のID）
        public int ItemID { get; set; }

        //商品名
        [SerializeField]
        private Text mName = null;

        //価格
        [SerializeField]
        private Text mPrice = null;

        //商品の情報ウィンドウ
        public ItemInfo InfoWindow { get; set; }

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
