using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Battle
{
    public class BattleController : MonoBehaviour
    {
        [SerializeField]
        private TalkCommandHelper mTalkCommandHelper = null;
        public TalkCommandHelper TalkCommandHelper { get { return mTalkCommandHelper; } private set { } }

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();
//            game.HideNowLoading();
            game.IsBattle = true;

            //スクリプト実行のサンプル
            //EventDataFormat e = new EventDataFormat();
            //e.FileName = "s9804";
            //game.CallScript(e);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void BattleEnd()
        {
            var game = Game.GetInstance();

            game.IsBattle = false;
            SceneManager.UnloadScene(game.SceneName_Battle);
        }
    }
}