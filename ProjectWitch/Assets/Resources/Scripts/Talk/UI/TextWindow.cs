using UnityEngine;
using UnityEngine.UI;

namespace Talk
{
    public class TextWindow : MonoBehaviour
    {
        [Header("パス")]
        [SerializeField]
        private string mFaceFolderPath = "Prefabs/Talk/Face/";

        //メッセージウィンドウ
        [Header ("メッセージウィンドウ")]
        [SerializeField]
        private GameObject mMessage = null;
        [SerializeField]
        private Text mMessageText = null;
        
        //名前ウィンドウ
        [Header("名前ウィンドウ")]
        [SerializeField]
        private GameObject mName = null;
        [SerializeField]
        private Text mNameText = null;

        //ページ送りアイコン
        [Header("ページ送りアイコン")]
        [SerializeField]
        private GameObject mNextIcon = null;

        //顔グラ
        [Header("顔グラの親オブジェクト")]
        [SerializeField]
        private GameObject mFaceParent = null;

        //内部コンポーネント
        private Animator mcAnimator = null;

        //内部変数

        //現在のフェイスオブジェクト
        private GameObject mCurrentFace = null;

        // Use this for initialization
        void Start()
        {
            mcAnimator = GetComponent<Animator>();
        }

        //ウィンドウを表示
        void ShowWindow()
        {
            mcAnimator.SetTrigger("Show");
        }

        //ウィンドウ全体を隠す
        void HideWindow()
        {
            HideName();
            HideFace();
            HideNextIcon();
            mcAnimator.SetTrigger("Hide");
        }

        //メッセージテキストをセットする
        void SetMessage(string str)
        {
            mMessageText.text = str;
        }

        //名前ウィンドウを表示
        void ShowName(string str)
        {
            ShowWindow();

            mNameText.text = str;

            if(mName)
                mName.GetComponent<Animator>().SetTrigger("Show");
        }

        //名前ウィンドウを削除
        void HideName()
        {
            mNameText.text = "";

            if(mName)
                mName.GetComponent<Animator>().SetTrigger("Hide");
        }

        //顔グラを表示
        void ShowFace(string name)
        {
            ShowWindow();

            //現在表示している顔グラを非表示にする
            if (mCurrentFace)
                mCurrentFace.GetComponent<Animator>().SetTrigger("Destroy");

            //顔グラが有効なスキンかどうか判断
            if (mFaceParent == null) return;

            //顔グラのロード
            var path = mFaceFolderPath + name;
            var resource = Resources.Load<GameObject>(path);
            var inst = Instantiate(resource);

            //親のセット
            inst.transform.SetParent(mFaceParent.transform);

            //現在の顔グラの参照をセット
            mCurrentFace = inst;
        }

        //顔グラを非表示
        void HideFace()
        {
            if (mCurrentFace)
            {
                mCurrentFace.GetComponent<Animator>().SetTrigger("Destroy");
                mCurrentFace = null;
            }
        }

        //顔グラの表情を変える
        void ChangeStateFace(string stateName)
        {
            mCurrentFace.GetComponent<Animator>().Play(stateName);
        }

        //ページ送りアイコンの表示
        void ShowNextIcon()
        {
            ShowWindow();

            mNextIcon.SetActive(true);
        }

        //ページ送りアイコンの非表示
        void HideNextIcon()
        {
            mNextIcon.SetActive(false);
        }

    }
}