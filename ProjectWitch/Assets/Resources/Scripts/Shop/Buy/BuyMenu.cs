using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class BuyMenu: MonoBehaviour
    {
        //トップへの参照
        [SerializeField]
        private Animator mTop = null;

        private Animator mAnim = null;

        //情報ウィンドウ
        [SerializeField]
        private ItemInfo mInfoWindow = null;

        //メッセージウィンドウ
        [SerializeField]
        private MessageBox mMesBox = null;

        //消費後マナウィンドウ
        [SerializeField]
        private NextManaWindow mNextMana = null;

        // Use this for initialization
        void Start()
        {
            mAnim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if(mAnim.GetBool("IsShow") && Input.GetButtonDown("Cancel"))
            {
                Close();
            }
        }

        public void Close()
        {
            mMesBox.SetText("", "");

            mInfoWindow.Close();
            mNextMana.Close();

            mTop.SetBool("IsShow", true);
            mAnim.SetBool("IsShow", false);
        }
    }
}