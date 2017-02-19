using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Load
{
    public class LoadController : MonoBehaviour {

        // Use this for initialization
        void Start() {

        }


        //フィールドへ飛ぶ（決定の時の動作
        void JumpToField()
        {
            var game = Game.GetInstance();

            game.CallField();
        }

        //そのまま終了（キャンセル時の操作
        void Close()
        {
            SceneManager.UnloadSceneAsync("Load");
        }
    }
}