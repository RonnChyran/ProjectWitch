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
            var anim = mMessage.GetComponent<Animator>();
            //現在表示されていなかったら表示する
            mMessage.GetComponent<Animator>().SetBool("IsShow",true);
        }

        //ウィンドウ全体を隠す
        public void HideWindow()
        {
            HideName();
            HideFace();
            HideNextIcon();

            var anim = mMessage.GetComponent<Animator>();
            //現在表示されていなかったら表示する
            mMessage.GetComponent<Animator>().SetBool("IsShow",false);
        }

        //名前ウィンドウを表示
        public void ShowName()
        {
            ShowWindow();

            mNameText.text = Name;

            if (mName)
            {
                var anim = mName.GetComponent<Animator>();
                //現在表示されていなかったら表示する
                anim.SetBool("IsShow",true);
            }
        }

        //名前ウィンドウを削除
        public void HideName()
        {
            mNameText.text = "";

            if (mName)
            {
                var anim = mName.GetComponent<Animator>();
                //現在表示されていたら隠す
                anim.SetBool("IsShow",false);
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
            inst.transform.SetParent(mFaceParent.transform, false);

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

            //現在表示している顔グラを非表示にする(違う場合)
            //一緒の場合は表情だけ変更し終了
            if (mFaces[id] != mCurrentFace)
            {
                HideFace();
            }

            //表示
            var anim = mFaces[id].GetComponent<Animator>();
            mFaces[id].GetComponent<Animator>().SetBool("IsShow",true);

            //現在の顔グラの参照をセット
            mCurrentFace = mFaces[id];


            //表情のセット
            ChangeStateFace(state);

        }

        //顔グラを非表示
        public void HideFace()
        {
            if (mCurrentFace)
            {
                var anim = mCurrentFace.GetComponent<Animator>();
                //現在表示されていたら隠す
                anim.SetBool("IsShow",false);
            }
        }

        //顔グラの表情を変える
        public void ChangeStateFace(string stateName)
        {
            if(mCurrentFace && stateName != "")
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