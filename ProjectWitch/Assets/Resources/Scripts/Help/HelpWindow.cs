using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.Help
{
    public class HelpWindow : MonoBehaviour
    {

        //親となるメインウィンドウ
        [SerializeField]
        private GameObject mMainWindow = null;

        [Header("画面のプレハブ")]
        [SerializeField]
        private GameObject mTop = null;
        [SerializeField]
        private GameObject[] mField = null;
        [SerializeField]
        private GameObject[] mMenu = null;
        [SerializeField]
        private GameObject[] mBattle = null;

        [Header("ボタン")]
        [SerializeField]
        private Button mNextButton = null;
        [SerializeField]
        private Button mPrevButton = null;

        [Header("テキスト")]
        [SerializeField]
        private Text mPageNum = null;
        
        //内部変数
        public enum Chapter
        {
            Top,Field,Menu,Battle
        }
        private Chapter mChapter = Chapter.Top; //項目
        private int mPage = 0;                  //ページ数
        private int mPageMax = 0;

        //ページのインスタンス
        private GameObject mInst = null;

        //コンポーネント
        private Canvas mCanvas = null;

        void Start()
        {
            mCanvas = GetComponent<Canvas>();
        }

        //表示の更新
        private void Reset()
        {
            //古いインスタンスを削除
            if (mInst) Destroy(mInst);

            //最大ページ数
            switch (mChapter)
            {
                case Chapter.Top:
                    mInst = Instantiate(mTop);
                    var ctrl = mInst.GetComponent<TopMenu>();
                    ctrl.HelpWindow = this;
                    mPageMax = 0;
                    break;
                case Chapter.Field:
                    mInst = Instantiate(mField[mPage]);
                    mPageMax = mField.Length;
                    break;
                case Chapter.Menu:
                    mInst = Instantiate(mMenu[mPage]);
                    mPageMax = mMenu.Length;
                    break;
                case Chapter.Battle:
                    mInst = Instantiate(mBattle[mPage]);
                    mPageMax = mBattle.Length;
                    break;
                default:
                    break;
            }
            
            //親をウィンドウにセット
            mInst.transform.SetParent(mMainWindow.transform, false);

            //ページ表示を更新,ボタンの有効無効
            if(mPageMax == 0)
            {
                mNextButton.interactable = false;
                mPrevButton.interactable = false;
                mPageNum.text = "0/0";
            }
            else
            {
                mNextButton.interactable = true;
                mPrevButton.interactable = true;
                mPageNum.text = (mPage + 1).ToString() + "/" + mPageMax.ToString();
            }
        }

        public void Show()
        {
            mCanvas.enabled = true;

            mChapter = Chapter.Top;
            mPage = 0;
            Reset();
        }

        public void OnClicked_Top()
        {
            mChapter = Chapter.Top;
            mPage = 0;
            Reset();
        }

        public void OnClicked_Close()
        {
            mCanvas.enabled = false;
        }

        public void OnClicked_Field()
        {
            mChapter = Chapter.Field;
            mPage = 0;
            Reset();
        }

        public void OnClicked_Menu()
        {

            mChapter = Chapter.Menu;
            mPage = 0;
            Reset();
        }

        //戦闘の説明ボタン
        public void OnClicked_Battle()
        {
            mChapter = Chapter.Battle;
            mPage = 0;
            Reset();
        }

        //次のページへボタン
        public void OnClicked_Next()
        {
            mPage++;
            if (mPage >= mPageMax) mPage = 0;
            Reset();
        }

        //前のページへボタン
        public void OnClicked_Prev()
        {
            mPage--;
            if (mPage < 0) mPage = mPageMax - 1;
            Reset();
        }

    }
}