using UnityEngine;
using UnityEngine.UI;
using System;

using ProjectWitch;

namespace ProjectWitch.Talk.WorkSpace
{
    public class MenuWorkSpace : MonoBehaviour
    {

        //フィールドのコマンド実行システム
        public Menu.TalkCommandHelper MenuCommand { get; set; }

        void Start()
        {
            var menuCtrl = GameObject.FindWithTag("MenuController");
            if (menuCtrl)
                MenuCommand = menuCtrl.GetComponent<Menu.MenuController>().TalkCommandHelper;
        }


        public void SetCommandDelegaters(VirtualMachine vm)
        {
            //メニューを閉じる
            vm.AddCommandDelegater(
                "Menu_Close",
                new CommandDelegater(false, 0, delegate (object[] arguments) {
                    string error=null;

                    if (!MenuCommand)
                    {
                        error = "[menu_close]が実行できません。メニューがロードされているシーンで実行してください。";
                        return error;
                    }

                    MenuCommand.CloseMenu();

                    return null;
                }));

            //軍団を開く
            vm.AddCommandDelegater(
                "Menu_GoArmy",
                new CommandDelegater(false, 0, delegate (object[] arguments) {
                    string error = null;

                    if (!MenuCommand)
                    {
                        error = "[menu_go_army]が実行できません。メニューがロードされているシーンで実行してください。";
                        return error;
                    }

                    MenuCommand.GoArmy();

                    return null;
                }));

            //軍団で特定のユニットのステータスを表示
            vm.AddCommandDelegater(
                "Menu_ArmyOpenUnit",
                new CommandDelegater(false, 1, delegate (object[] arguments) {
                    string error = null;

                    var index = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;

                    if (!MenuCommand)
                    {
                        error = "[menu_army_open_unit]が実行できません。メニューがロードされているシーンで実行してください。";
                        return error;
                    }

                    MenuCommand.ArmyOpenUnit(index, out error);

                    return error;
                }));

        }
    }
}