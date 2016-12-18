using UnityEngine;
using UnityEngine.UI;
using System;

using ProjectWitch;

namespace ProjectWitch.Talk.WorkSpace
{
    public class FieldWorkSpace : MonoBehaviour
    {

        //フィールドのコマンド実行システム
        public Field.TalkCommandHelper FieldCommand { get; set; }

        void Start()
        {
            var fieldCtrl = GameObject.FindWithTag("FieldController");
            if (fieldCtrl)
                FieldCommand = fieldCtrl.GetComponent<Field.FieldController>().TalkCommandHelper;
        }


        //立ち絵の位置用のアップデータ
        private class FieldAreaHilightUpdater : UpdaterFormat
        {

            //エンドフラグ
            private bool isEnd = false;

            //対象の領地
            private int mArea = -1;

            //FieldWorkSpace
            private FieldWorkSpace mFWS = null;

            private FieldAreaHilightUpdater() { }
            public FieldAreaHilightUpdater(int area, FieldWorkSpace fws)
            {
                mArea = area;
                mFWS = fws;
            }

            public override void Setup()
            {
                mFWS.FieldCommand.HilightArea(mArea, () => { isEnd = true; });
            }

            public override void Update(float deltaTime)
            {
                if (isEnd)
                    SetActive(false);
            }

            public override void Finish()
            {
            }
        }

        public void SetCommandDelegaters(VirtualMachine vm)
        {
            //エリア強調エフェクト
            vm.AddCommandDelegater(
                "Field_HilightArea",
                new CommandDelegater(true, 1, delegate (object[] arguments) {
                    string error;

                    var area = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;

                    if(!FieldCommand)
                    {
                        error = "[field_hilight]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    arguments[1] = new FieldAreaHilightUpdater(area, this) as UpdaterFormat;
                    return null;
                }));

            //エリアウィンドウの表示非表示
            vm.AddCommandDelegater(
                "Field_OpenAreaWindow",
                new CommandDelegater(false, 1, delegate(object[] arguments)
                {
                    string error;

                    var area = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;
                    
                    if (!FieldCommand)
                    {
                        error = "[field_open_areawindow]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    FieldCommand.OpenAreaWindow(area);

                    return null;
                }));
            vm.AddCommandDelegater(
                "Field_CloseAreaWindow",
                new CommandDelegater(false, 0, delegate (object[] arguments)
                  {
                      string error = null;
                      if (!FieldCommand)
                      {
                          error = "[field_close_areawindow]が実行できません。フィールドがロードされているシーンで実行してください。";
                          return error;
                      }

                      FieldCommand.CloseAreaWindow();
                      
                      return null;
                  }));

            vm.AddCommandDelegater(
                "Field_CallBattle_AreaWindow",
                new CommandDelegater(false, 0, delegate (object[] arguments)
                {
                    string error = null;
                    if (!FieldCommand)
                    {
                        error = "[field_callbattle_areawindow]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    FieldCommand.CallBattleFromAreaWindow();

                    return null;
                }));

            vm.AddCommandDelegater(
                "Field_OpenMenu",
                new CommandDelegater(false, 0, delegate (object[] arguments)
                {
                    string error = null;
                    if (!FieldCommand)
                    {
                        error = "[field_open_menu]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    FieldCommand.OpenMenu();

                    return null;
                }));
        }
    }
}