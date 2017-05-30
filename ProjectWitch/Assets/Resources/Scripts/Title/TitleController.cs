using UnityEngine;
using System.Collections;

namespace ProjectWitch
{
    public class TitleController : MonoBehaviour
    {
        [SerializeField]
        private GameObject mLoadGame = null;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();
            game.HideNowLoading();
        }

        // Update is called once per frame
        void Update()
        {
            if(mLoadGame.activeSelf && Input.GetButtonDown("Cancel"))
            {
                CloseLoadWindow();
            }
        }

        public void OnClick_Load()
        {
            mLoadGame.SetActive(true);
        }

        //ロードウィンドウを閉じる
        public void CloseLoadWindow()
        {
            var game = Game.GetInstance();
            game.SoundManager.PlaySE(SE.Cancel);

            mLoadGame.SetActive(false);
        }
    }
}