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


        //内部変数

        //現在のフェイスオブジェクト
        private GameObject mCurrentFace = null;

        //フェイスオブジェクトの配列
        private GameObject[] mFaces = new GameObject[16];

        //プロパティ
        public string Name { get { return mNameText.text; } set { mNameText.text = value; } }
        public string Message { get { return mMessageText.text; } set { mMessageText.text = value; } }
        public Vector3 Position { get { return transform.localPosition; } set { transform.localPosition = value;} }

        // Use this for initialization
        void Start()
        {
        }

        //ウィンドウを表示
        public void ShowWindow()
        {
            mMessage.GetComponent<Animator>().SetTrigger("Show");
        }

        //ウィンドウ全体を隠す
        public void HideWindow()
        {
            HideName();
            HideFace();
            HideNextIcon();
            mMessage.GetComponent<Animator>().SetTrigger("Hide");
        }

        //名前ウィンドウを表示
        public void ShowName()
        {
            ShowWindow();

            mNameText.text = Name;

            if (mName)
            {
                var anim = mName.GetComponent<Animator>();
                var info = anim.GetCurrentAnimatorStateInfo(0);
                //現在表示されていなかったら表示する
                if(info.fullPathHash == Animator.StringToHash("Base Layer.hide"))
                    mName.GetComponent<Animator>().SetTrigger("Show");
            }
        }

        //名前ウィンドウを削除
        public void HideName()
        {
            mNameText.text = "";

            if (mName)
            {
                var anim = mName.GetComponent<Animator>();
                var info = anim.GetCurrentAnimatorStateInfo(0);
                //現在表示されていたら隠す
                if(info.fullPathHash == Animator.StringToHash("Base Layer.show"))
                    mName.GetComponent<Animator>().SetTrigger("Hide");
            }
        }

        public void LoadFace(int id, string name)
        {
            //IDの妥当性チェック
            if (!FaceIdCheck(id)) return;

            //顔グラが有効なスキンかどうか判断
            if (mFaceParent == null) return;

            //顔グラのロード
            var path = mFaceFolderPath + name;
            var resource = Resources.Load<GameObject>(path);
            var inst = Instantiate(resource);
            inst.transform.SetParent(mFaceParent.transform);
            inst.SetActive(false);

            //スロットが空いていなかったら古いものを開放する
            if (mFaces[id])
                Destroy(mFaces[id]);

            mFaces[id] = inst;

        }

        //顔グラを表示
        public void ShowFace(int id, string state)
        {
            //IDの妥当性チェック
            if (!FaceIdCheck(id)) return;

            //ロードされていなかったら表示しない
            if (!mFaces[id]) return;

            //ウィンドウを表示
            ShowWindow();

            //現在表示している顔グラを非表示にする
            if (mCurrentFace)
                mCurrentFace.GetComponent<Animator>().SetTrigger("Hide");

            //表示
            mFaces[id].GetComponent<Animator>().SetTrigger("Show");

            //現在の顔グラの参照をセット
            mCurrentFace = mFaces[id];
        }

        //顔グラを非表示
        public void HideFace()
        {
            if (mCurrentFace)
            {
                mCurrentFace.GetComponent<Animator>().SetTrigger("Hide");
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

        //faceidの妥当性をチェック
        private bool FaceIdCheck(int id)
        {
            if (id < 0)
            {
                Debug.LogError("FaceIDが不正です");
                return false;
            }
            if (id >= mFaces.Length)
            {
                Debug.LogError("FaceIDが範囲をオーバーしています");
                return false;
            }

            return true;
        }
    }
}