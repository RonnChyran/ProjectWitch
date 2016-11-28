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
        private Canvas mArmy = null;
        [SerializeField]
        private Canvas mTown = null;
        [SerializeField]
        private Canvas mInfo = null;
        [SerializeField]
        private Canvas mSystem = null;

        //内部変数
        private Canvas mcCanvas = null;

        // Use this for initialization
        void Start()
        {
            mcCanvas = GetComponent<Canvas>();
        }

        // Update is called once per frame
        void Update()
        {
            if (mcCanvas.enabled)
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
            mArmy.enabled = true;
            mcCanvas.enabled = false;
        }

        //町へ行くをクリック
        public void OnClickTown()
        {
            mTown.enabled = true;
            mcCanvas.enabled = false;
        }

        //情報をクリック
        public void OnClickInfo()
        {
            mInfo.enabled = true;
            mcCanvas.enabled = false;
        }

        //システムをクリック
        public void OnClickSystem()
        {
            mSystem.enabled = true;
            mcCanvas.enabled = false;
        }

    }
}