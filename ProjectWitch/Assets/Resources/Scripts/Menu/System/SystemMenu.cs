using UnityEngine;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class SystemMenu : MonoBehaviour
    {
        //コントローラへの参照
        [SerializeField]
        private MenuController mController = null;

        //トップメニューへの参照
        [SerializeField]
        private Animator mTopMenu = null;

        //セーブパネルへの参照
        [SerializeField]
        private GameObject mSavePanel = null;

        //ロードパネルへの参照
        [SerializeField]
        private GameObject mLoadPanel = null;

        //コンフィグパネルへの参照
        [SerializeField]
        private GameObject mConfigPanel = null;

        //component 
        private Animator mcAnim = null;

        //閉じれるかどうか
        public bool Closable { get; set; }

        // Use this for initialization
        void Start()
        {
            mcAnim = GetComponent<Animator>();
            Closable = true;

            mSavePanel.SetActive(false);
            mLoadPanel.SetActive(false);
            mConfigPanel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (mcAnim.GetBool("IsShow") && Closable && mController.InputEnable)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    Close();
                }
            }
        }

        public void Close()
        {
            StartCoroutine(_Close());
        }

        private IEnumerator _Close()
        {
            //キャンセル音再生
            Game.GetInstance().SoundManager.PlaySE(SE.Cancel);

            mcAnim.SetBool("IsShow", false);
            yield return new WaitForSeconds(0.2f);
            mTopMenu.SetBool("IsShow", true);
            yield return new WaitForSeconds(0.2f);

        }

        //セーブメニューを開く
        public void OpenSave()
        {
            mSavePanel.SetActive(true);
            mLoadPanel.SetActive(false);
            mConfigPanel.SetActive(false);
        }

        //ロードメニューを開く
        public void OpenLoad()
        {
            mSavePanel.SetActive(false);
            mLoadPanel.SetActive(true);
            mConfigPanel.SetActive(false);
        }

        //コンフィグメニューを開く
        public void OpenConfig()
        {
            mSavePanel.SetActive(false);
            mLoadPanel.SetActive(false);
            mConfigPanel.SetActive(true);
        }

    }
}