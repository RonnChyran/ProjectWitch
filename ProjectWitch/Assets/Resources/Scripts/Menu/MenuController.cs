using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class MenuController : MonoBehaviour
    {
        //各メニューへの参照
        [SerializeField]
        private TopMenu mTopMenu = null;
        public TopMenu TopMenu { get { return mTopMenu; } private set { } }

        [SerializeField]
        private ArmyMenu mArmyMenu = null;
        public ArmyMenu ArmyMenu { get { return mArmyMenu; } private set { } }

        [SerializeField]
        private TalkCommandHelper mTalkCommandHelper = null;
        public TalkCommandHelper TalkCommandHelper { get { return mTalkCommandHelper; } private set { } }
       

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