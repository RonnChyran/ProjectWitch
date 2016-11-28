using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class MenuController : MonoBehaviour
    {
        //メニューを閉じる
        public void Close()
        {
            var game = Game.GetInstance();
            game.SoundManager.PlaySE(SE.Cancel);
            
            var fctrl = GameObject.FindWithTag("FieldController").GetComponent<Field.FieldController>();
            fctrl.MenuClickable = true;
            fctrl.FieldUIController.ShowUI();

            SceneManager.UnloadScene(game.SceneName_Menu);
        }
    }
}