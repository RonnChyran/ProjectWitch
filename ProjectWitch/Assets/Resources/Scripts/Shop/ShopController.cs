using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class ShopController : MonoBehaviour
    {
        //シーン名
        [SerializeField]
        private string mSceneName = "";

        //町シーンへの参照
        private Menu.TownMenu mTownMenu = null;

        private Animator mcAnimator = null;

        void Start()
        {
            mTownMenu = GameObject.FindWithTag("TownController").GetComponent<Menu.TownMenu>();
            mcAnimator = GetComponent<Animator>();
        }

        public void Close()
        {
            mcAnimator.SetBool("IsShow", false);
        }

        //閉じる際の最終処理（アニメーションから呼び出す
        public void Close_End()
        {
            mTownMenu.Closable = true;
            SceneManager.UnloadSceneAsync(mSceneName);
        }
    }
}