using UnityEngine;
using System.Collections;

namespace Field
{
    public class MenuButton : FieldButtonBase
    {
        //生成するメニューのプレハブ
        [SerializeField]
        private GameObject mCommonMenu = null;

        //今このメニューが開いているかどうか
        private bool isOpening = false;

        //メニューを開く
        public void OnClicked()
        {
            //メニューが反応可能かどうか
            if (!mFieldController.MenuClickable) return;

            //メニューが開いていた場合メニューを消す
            if (isOpening)
            {
                HideMenu();

                //メニューを親にセット
                mInst.transform.SetParent(transform);
            }
            //開いていなかった場合メニューを開く
            else
            {
                ShowMenu(mCommonMenu);
            }

            //メニューの状態フラグを反転
            isOpening = (isOpening) ? false : true;
        }

    }
}