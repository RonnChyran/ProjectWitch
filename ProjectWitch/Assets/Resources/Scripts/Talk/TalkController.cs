using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Talk
{
    public class TalkController : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            //トークシーンの開始
            game.IsTalk = true;

        }

        public IEnumerator EndScript()
        {
            var game = Game.GetInstance();

            yield return new WaitForSeconds(1.5f);

            //トークシーンの終了
            game.IsTalk = false;

            //シーンのアンロード
            SceneManager.UnloadSceneAsync(game.SceneName_Talk);

            yield return null;
        }
    }
}