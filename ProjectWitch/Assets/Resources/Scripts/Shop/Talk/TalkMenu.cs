using UnityEngine;
using System.Collections;

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

        [Space(20)]
        
        //選択肢に買わないと答えた場合
        [SerializeField]
        private ShopMessage[] mMes_NO = new ShopMessage[10];

        //トークシーンの開始
        public void Begin()
        {
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
                mMesBox.SetText(mes.Name, mes.Message);

                while (mMesBox.TextState == MessageBox.State.Active) yield return null;
                while (Input.GetButtonDown("TalkNext")) yield return null; //押し直すまでウェイト
                while (!Input.GetButtonDown("TalkNext")) yield return null;
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

            foreach(var mes in mMes_OK)
            {
                mMesBox.SetText(mes.Name, mes.Message);
                
                while (mMesBox.TextState == MessageBox.State.Active) yield return null;
                while (Input.GetButtonDown("TalkNext")) yield return null; //押し直すまでウェイト
                while (!Input.GetButtonDown("TalkNext")) yield return null;
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
                mMesBox.SetText(mes.Name, mes.Message);

                while (mMesBox.TextState == MessageBox.State.Active) yield return null;
                while (Input.GetButtonDown("TalkNext")) yield return null; //押し直すまでウェイト
                while (!Input.GetButtonDown("TalkNext")) yield return null;
            }

            End();
            yield return null;
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