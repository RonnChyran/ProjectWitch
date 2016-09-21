using UnityEngine;
using System.Collections;

namespace Field
{
    //メニューボタンで開いたメニューの制御クラス
    public class Menu : MenuBase
    {
        private FieldController mFieldController;

        // Use this for initialization
        void Start()
        {
            //Field UI Controllerを取得
            var gameobj = GameObject.FindWithTag("FieldController");
            mFieldController = gameobj.GetComponent<FieldController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //軍備をクリック
        public void CallArmy()
        {
            var game = Game.GetInstance();
            game.ShowDialog("CallArmy", "軍備がクリックされました。\n軍備はまだ実装していません");
        }

        //開発をクリック
        public void CallDevelop()
        {

            var game = Game.GetInstance();
            game.ShowDialog("CallDevelop", "開発がクリックされました。\n開発はまだ実装していません");
        }

        //町へ出るをクリック
        public void CallTown()
        {

            var game = Game.GetInstance();
            game.ShowDialog("CallTown", "町へ行くがクリックされました。\n町へ行くはまだ実装していません");
        }

        //情報をクリック
        public void CallInfo()
        {
            var game = Game.GetInstance();
            game.ShowDialog("CallInfo", "情報がクリックされました。\n情報はまだ実装していません");

        }

        //システムをクリック
        public void CallSystem()
        {
            var game = Game.GetInstance();
            game.ShowDialog("CallSystem", "システムがクリックされました。\nシステムはまだ実装していません");

        }

        //メニューを閉じる
        public override void Close()
        {
            mFieldController.MenuClickable = true;
            Destroy(this.gameObject);
        }
    }
}