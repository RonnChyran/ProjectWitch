using UnityEngine;
using System;

namespace ProjectWitch.Menu
{
    public class TalkCommandHelper : MonoBehaviour
    {
        //controller
        [SerializeField]
        private MenuController mMenuController = null;
        
        //メニューを閉じる
        public void CloseMenu()
        {
            mMenuController.Close();
        }

        //軍団を開く
        public void GoArmy()
        {
            mMenuController.TopMenu.OnClickArmy();
        }
        
        //軍団で、特定のユニットのメニューを開く
        public void ArmyOpenUnit(int index, out string error)
        {
            error = null;

            var units = mMenuController.ArmyMenu.UnitList.GetUnitList();
            try
            {
                units[index].OnClicked();
            }
            catch(ArgumentException)
            {
                error = "indexの範囲が不正です。";
                return;
            }
        }


    }
}