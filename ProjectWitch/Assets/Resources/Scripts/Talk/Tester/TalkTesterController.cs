using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

namespace ProjectWitch.Talk
{
    public class TalkTesterController : MonoBehaviour
    {
        [SerializeField]
        private string mScenarioFolderPath = null;

        [SerializeField]
        private Canvas mCanvas0 = null;
        [SerializeField]
        private Canvas mCanvas1 = null;

        [SerializeField]
        private Text mText = null;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();

            //トーク中はUI非表示
            if(game.IsTalk)
            {
                mCanvas0.enabled = false;
                mCanvas1.enabled = true;
            }
            //トークじゃないときはUI表示
            else
            {
                mCanvas0.enabled = true;
                mCanvas1.enabled = false;
            }
        }

        //コールバック：トークを開始
        public void StartTalk()
        {
            var game = Game.GetInstance();

            //シナリオセット
            game.ScenarioIn.FileName = mScenarioFolderPath + "/" + mText.text + ".txt";
            game.ScenarioIn.NextA = -1;
            game.ScenarioIn.NextB = -1;
            game.ScenarioIn.IsTest = true;

            //トークシーンロード
            SceneManager.LoadScene("Talk", LoadSceneMode.Additive);
        }

        //コールバック：トークを中止
        public void StopTalk()
        {
            var game = Game.GetInstance();

            //トークシーンの終了
            game.IsTalk = false;

            //シーンのアンロード
            SceneManager.UnloadSceneAsync(game.SceneName_Talk);

            //bgm停止
            game.SoundManager.Stop(SoundType.BGM);
        }
    }
}