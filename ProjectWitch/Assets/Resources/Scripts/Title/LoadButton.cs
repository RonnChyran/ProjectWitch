using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch
{
    public class LoadButton : MonoBehaviour
    {

        public void OnClick()
        {
            var game = Game.GetInstance();

            //ロード画面を呼び出す
            game.CallLoad();
        }
    }
}