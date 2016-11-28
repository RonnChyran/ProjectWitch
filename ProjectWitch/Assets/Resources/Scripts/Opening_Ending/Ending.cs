using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch
{
    public class Ending : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();
            game.HideNowLoading();

            StartCoroutine(CoEnding());

        }


        //イベント制御
        private IEnumerator CoEnding()
        {
            var game = Game.GetInstance();

            //実行するエンディングIDの取得
            var id = game.EndingID;
            //var id = 16;

            //イベントデータの設定
            var ev = new EventDataFormat();
            switch (id)
            {
                case 14:
                    ev.FileName = "s9994";
                    ev.NextA = -1;
                    ev.NextB = -1;
                    break;
                case 15:
                    ev.FileName = "s9995";
                    ev.NextA = -1;
                    ev.NextB = -1;
                    break;
                case 16:
                    ev.FileName = "s9996";
                    ev.NextA = -1;
                    ev.NextB = -1;
                    break;
                default:
                    ev.FileName = "s9994";
                    ev.NextA = -1;
                    ev.NextB = -1;
                    break;
            }

            //スクリプト実行
            game.CallScript(ev);
            yield return null;
            while (game.IsTalk) yield return null;

            //ムービーシーンの再生
            switch (id)
            {
                case 16:
                    SceneManager.LoadScene("EndMovie0");
                    break;
                default:
                    break;
            }


            //スクリプトを実行したら終了
            //タイトルの呼び出し
            StartCoroutine(game.CallTitle());

            yield break;
        }

    }
}