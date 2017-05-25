using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    public class BuyItem : MonoBehaviour {

        //選択時のメッセージ
        [SerializeField]
        private string mMesNameA = "";
        [SerializeField,Multiline]
        private string mMessageA = "";

        //選択時、マナが足りない場合のメッセージ
        [SerializeField]
        private string mMesNameB = "";
        [SerializeField,Multiline]
        private string mMessageB = "";

        //商品のID（装備のID）
        public int ItemID { get; set; }

        //商品名
        [SerializeField]
        private Text mName = null;

        //価格
        [SerializeField]
        private Text mPrice = null;

        //所持数
        [SerializeField]
        private Text mNum = null;

        //商品の情報ウィンドウ
        public ItemInfo InfoWindow { get; set; }

        //メッセージウィンドウ
        public MessageBox MesBox { get; set; }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void Reset()
        {
            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];
            var itemList = game.GameData.Territory[0].EquipmentList;

            //テキストをセット
            mName.text = item.Name;
            mPrice.text = item.BuyingPrice.ToString();

            var num = itemList[ItemID].Count;
            mNum.text = num.ToString();
        }

        public void OnClicked()
        {
            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];

            InfoWindow.ItemID = ItemID;
            InfoWindow.Reset();

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