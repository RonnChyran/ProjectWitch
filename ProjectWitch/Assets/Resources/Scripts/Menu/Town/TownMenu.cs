using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class TownMenu : MonoBehaviour
    {
        //コントローラへの参照
        [SerializeField]
        private MenuController mController = null;

        //トップメニューへの参照
        [SerializeField]
        private Animator mTopMenu = null;

        //component 
        private Animator mcAnim = null;

        //閉じれるかどうか
        public bool Closable { get; set; }

        // Use this for initialization
        void Start()
        {
            mcAnim = GetComponent<Animator>();
            Closable = true;
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

        public void Click_ToolShop()
        {
            Closable = false;
            SceneManager.LoadScene("ToolShop",LoadSceneMode.Additive);
        }

        public void Click_MagicShop()
        {

        }
    }
}