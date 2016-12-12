using UnityEngine;
using UnityEngine.UI;
using System;

using ProjectWitch;

namespace ProjectWitch.Talk.WorkSpace
{
    public class PreBattleWorkSpace : MonoBehaviour
    {

        //フィールドのコマンド実行システム
        public PreBattle.TalkCommandHelper PreBattleCommand { get; set; }

        void Start()
        {
            var Ctrl = GameObject.FindWithTag("PreBattleController");
            if (Ctrl)
                PreBattleCommand = Ctrl.GetComponent<PreBattle.PreBattleController>().TalkCommandHelper;
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
            //ユニットのセット
            vm.AddCommandDelegater(
                "PreBattle_SetUnit",
                new CommandDelegater(false, 1, delegate (object[] arguments) {
                    string error;

                    var index = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;

                    if (!PreBattleCommand)
                    {
                        error = "[prebattle_setunit]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    PreBattleCommand.SetUnit(index);

                    return null;
                }));
            //ユニットの除外
            vm.AddCommandDelegater(
                "PreBattle_RemoveUnit",
                new CommandDelegater(false, 1, delegate (object[] arguments) {
                    string error;

                    var index = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;

                    if (!PreBattleCommand)
                    {
                        error = "[prebattle_removeunit]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    PreBattleCommand.RemoveUnit(index);

                    return null;
                }));    
            
            //カードの追加
            vm.AddCommandDelegater(
                "PreBattle_SetCard",
                new CommandDelegater(false, 1, delegate (object[] arguments) {
                    string error;

                    var index = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;

                    if (!PreBattleCommand)
                    {
                        error = "[prebattle_setcard]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    PreBattleCommand.SetCard(index);

                    return null;
                }));
            //カードの除外
            vm.AddCommandDelegater(
                "PreBattle_RemoveCard",
                new CommandDelegater(false, 1, delegate (object[] arguments) {
                    string error;

                    var index = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;

                    if (!PreBattleCommand)
                    {
                        error = "[prebattle_removecard]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    PreBattleCommand.RemoveCard(index);

                    return null;
                }));

            //戦闘の呼び出し
            vm.AddCommandDelegater(
                "PreBattle_CallBattle",
                new CommandDelegater(false, 0, delegate (object[] arguments)
                {
                    string error = null;
                    if(!PreBattleCommand)
                    {
                        error = "[prebattle_callbattle]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    PreBattleCommand.CallBattle();

                    return null;
                }));
        }
    }
}