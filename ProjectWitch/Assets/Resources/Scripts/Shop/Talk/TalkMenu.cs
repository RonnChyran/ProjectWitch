using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWitch.Shop
{
    public class TalkMenu : BaseMenu
    {
        //最初の値引き率
        [SerializeField]
        private float mDiscountRateA = 0.9f;

        //最終的な値引き率
        [SerializeField]
        private float mDiscountRateB = 0.5f;

        //選択肢ウィンドウへの参照
        [SerializeField]
        private GameObject mChoicePanel = null;

        //選択肢までのセリフ
        [SerializeField]
        private ShopMessage[] mMessages = new ShopMessage[10];

        [Space(30)]

        //選択肢に買うと答えた場合
        [SerializeField]
        private ShopMessage[] mMes_OK = new ShopMessage[10];

        //選択肢に買うと答えた場合で、お金が足りなかった場合
        [SerializeField]
        private ShopMessage[] mMes_OK_NoMoney = new ShopMessage[10];

        [Space(20)]
        
        //選択肢に買わないと答えた場合
        [SerializeField]
        private ShopMessage[] mMes_NO = new ShopMessage[10];

        //置換する文字列
        private string mTag_ItemName = "[name]";
        private string mTag_ItemPriceA = "[priceA]";
        private string mTag_ItemPriceB = "[priceB]";
        private string mTag_ItemPriceC = "[priceC]";

        //装備品のID
        private int mEquipmentID = -1;

        //トークシーンの開始
        public void Begin()
        {
            //装備をランダムで選択
            var game = Game.GetInstance();
            var list = new List<int>();
            for (int i = 0; i < game.GameData.Equipment.Count; i++)
            {
                //フラグを満たしていない商品は除外
                var flag = game.GameData.Equipment[i].ShopFlag;
                if (game.GameData.Memory[EquipmentDataFormat.ShopFlagID] < flag) continue;

                list.Add(i);
            }

            mEquipmentID = list[UnityEngine.Random.Range(0, list.Count)];

            StartCoroutine(_TalkProcess());
        }

        public void End()
        {
            mMesBox.SetText("", "");
            mTop.SetBool("IsShow", true);
        }

        //選択肢前までのメッセージ表示
        private IEnumerator _TalkProcess()
        {
            //会話の開始

            //選択肢までの会話
            foreach (var mes in mMessages)
            {
                yield return StartCoroutine(_ShowText(mes));
            }
            mChoicePanel.SetActive(true);
            yield return null;
        }

        //OKクリック後の処理
        public void Click_OK()
        {
            StartCoroutine(_ClickOK());
        }
        private IEnumerator _ClickOK()
        {
            mChoicePanel.SetActive(false);
            yield return null;

            //所持金が足りるか足りないかでメッセージ変更
            var gamedata = Game.GetInstance().GameData;
            var equipment = gamedata.Equipment[mEquipmentID];
            ShopMessage[] exeMessage = new ShopMessage[1];
            if (gamedata.PlayerMana >= (int)(equipment.BuyingPrice*mDiscountRateB))
            {
                exeMessage = mMes_OK;

                //所持金を減らす処理
                gamedata.PlayerMana -= (int)(equipment.BuyingPrice * mDiscountRateB);

                //購入処理
                gamedata.Territory[0].EquipmentList[mEquipmentID].Add(-1);
            }
            else
            {
                exeMessage = mMes_OK_NoMoney;
            }

            //メッセージを表示
            foreach (var mes in exeMessage)
            {
                yield return StartCoroutine(_ShowText(mes));
            }

            End();
            yield return null;
        }

        //NOクリック後の処理
        public void Click_NO()
        {
            StartCoroutine(_ClickNO());
        }
        private IEnumerator _ClickNO()
        {
            mChoicePanel.SetActive(false);
            yield return null;

            foreach (var mes in mMes_NO)
            {
                yield return StartCoroutine(_ShowText(mes));
            }

            End();
            yield return null;
        }
        

        //テキストの中に含まれるタグを置換
        private string ReplaceMessage(string message)
        {
            //装備データを取得
            var equipment = Game.GetInstance().GameData.Equipment[mEquipmentID];

            //文字列を置換
            var outstr = message.Replace(mTag_ItemName, equipment.Name);
            outstr = outstr.Replace(mTag_ItemPriceA, equipment.BuyingPrice.ToString());
            outstr = outstr.Replace(mTag_ItemPriceB, ((int)(equipment.BuyingPrice * mDiscountRateA)).ToString());
            outstr = outstr.Replace(mTag_ItemPriceC, ((int)(equipment.BuyingPrice * mDiscountRateB)).ToString());

            return outstr;
        }

        //テキストを終端まで読み出し
        private IEnumerator _ShowText(ShopMessage mes)
        {
            //テキストを置換してセット
            var message = ReplaceMessage(mes.Message);
            mMesBox.SetText(mes.Name, message);

            //テキストの表示終了待ち
            while (mMesBox.TextState == MessageBox.State.Active) yield return null;
            
            //キー入力待ち
            while (Input.GetButtonDown("TalkNext")) yield return null; //押し直すまでウェイト
            while (!Input.GetButtonDown("TalkNext")) yield return null;
        }
    }

    //名前とメッセージを一つにしたクラス
    [System.Serializable]
    public class ShopMessage
    {
        [SerializeField]
        private string mName = "";
        [SerializeField,Multiline]
        private string mMessage = "";

        public string Name { get { return mName; } set { mName = value; } }
        public string Message { get { return mMessage; } set { mMessage = value; } }
    }
}