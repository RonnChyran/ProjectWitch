using UnityEngine;
using System.Collections;

namespace ProjectWitch
{
    public class TitleController : MonoBehaviour
    {
        //ロードウィンドウへの参照
        [SerializeField]
        private GameObject mLoadGame = null;

        //コンフィグウィンドウへの参照
        [SerializeField]
        private GameObject mConfig = null;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();
            game.HideNowLoading();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (mLoadGame.activeSelf) CloseLoadWindow();
                if (mConfig.activeSelf) CloseConfigWindow();
            }
        }

        public void OnClick_Load()
        {
            mLoadGame.SetActive(true);
        }

        public void OnClick_Config()
        {
            mConfig.SetActive(true);
        }

        //ロードウィンドウを閉じる
        public void CloseLoadWindow()
        {
            var game = Game.GetInstance();
            game.SoundManager.PlaySE(SE.Cancel);

            mLoadGame.SetActive(false);
        }

        //コンフィグウィンドウを閉じる
        public void CloseConfigWindow()
        {
            var game = Game.GetInstance();
            game.SoundManager.PlaySE(SE.Cancel);

            game.SystemData.Save();

            mConfig.SetActive(false);
        }
    }
}