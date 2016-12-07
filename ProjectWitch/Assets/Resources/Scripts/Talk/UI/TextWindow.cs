using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Talk
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

        //プロパティ
        public string Name { get { return mNameText.text; } set { mNameText.text = value; } }
        public string Message { get { return mMessageText.text; } set { mMessageText.text = value; } }

        // Use this for initialization
        void Start()
        {
            mcAnimator = mMessage.GetComponent<Animator>();
        }

        //ウィンドウを表示
        public void ShowWindow()
        {
            mcAnimator.SetTrigger("Show");
        }

        //ウィンドウ全体を隠す
        public void HideWindow()
        {
            HideName();
            HideFace();
            HideNextIcon();
            mcAnimator.SetTrigger("Hide");
        }

        //名前ウィンドウを表示
        public void ShowName()
        {
            ShowWindow();

            mNameText.text = Name;

            if(mName)
                mName.GetComponent<Animator>().SetTrigger("Show");
        }

        //名前ウィンドウを削除
        public void HideName()
        {
            mNameText.text = "";

            if(mName)
                mName.GetComponent<Animator>().SetTrigger("Hide");
        }

        //顔グラを表示
        public void ShowFace(string name)
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
        public void HideFace()
        {
            if (mCurrentFace)
            {
                mCurrentFace.GetComponent<Animator>().SetTrigger("Destroy");
                mCurrentFace = null;
            }
        }

        //顔グラの表情を変える
        public void ChangeStateFace(string stateName)
        {
            mCurrentFace.GetComponent<Animator>().Play(stateName);
        }

        //ページ送りアイコンの表示
        public void ShowNextIcon()
        {
            ShowWindow();

            mNextIcon.SetActive(true);
        }

        //ページ送りアイコンの非表示
        public void HideNextIcon()
        {
            mNextIcon.SetActive(false);
        }

    }
}