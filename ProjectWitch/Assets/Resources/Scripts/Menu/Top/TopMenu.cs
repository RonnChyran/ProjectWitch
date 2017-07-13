using UnityEngine;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class TopMenu : MonoBehaviour
    {

        [SerializeField]
        private MenuController mMenuController = null;

        [Header("各メニューのキャンバス")]
        [SerializeField]
        private Animator mArmy = null;
        [SerializeField]
        private Animator mTown = null;
        [SerializeField]
        private Animator mInfo = null;
        [SerializeField]
        private Animator mSystem = null;

        //内部変数
        private Animator mcAnim = null;

        // Use this for initialization
        void Start()
        {
            mcAnim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (mcAnim.GetBool("IsShow") && mMenuController.InputEnable)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    mMenuController.Close();
                }
            }
        }

        //軍拡をクリック
        public void OnClickArmy()
        {
            mArmy.SetBool("IsShow", true);
            mcAnim.SetBool("IsShow", false);
        }

        //町へ行くをクリック
        public void OnClickTown()
        {
            mTown.SetBool("IsShow", true);
            mcAnim.SetBool("IsShow", false);
        }

        //情報をクリック
        public void OnClickInfo()
        {
            mInfo.SetBool("IsShow", true);
            mcAnim.SetBool("IsShow", false);
        }

        //システムをクリック
        public void OnClickSystem()
        {
            mSystem.SetBool("IsShow", true);
            mcAnim.SetBool("IsShow", false);
        }

        public void Show()
        {
            mcAnim.SetBool("IsShow", true);
        }
    }
}