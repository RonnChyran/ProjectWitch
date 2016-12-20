using UnityEngine;
using UnityEngine.UI;
using System;

using ProjectWitch;

namespace ProjectWitch.Talk.WorkSpace
{
    public class BattleWorkSpace : MonoBehaviour
    {

        //フィールドのコマンド実行システム
        public Battle.TalkCommandHelper BattleCommand { get; set; }

        void Start()
        {
            var battleCtrl = GameObject.FindWithTag("BattleController");
            if (battleCtrl)
                BattleCommand = battleCtrl.GetComponent<Battle.BattleController>().TalkCommandHelper;
        }

        public void SetCommandDelegaters(VirtualMachine vm)
        {
            //スキルボタンの表示
            vm.AddCommandDelegater(
                "Battle_ShowSkillButton",
                new CommandDelegater(false, 1, delegate (object[] arguments) {
                    string error;

                    var target = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;

                    if (!BattleCommand)
                    {
                        error = "[battle_show_skill_button]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    BattleCommand.ShowSkillButton(target, out error);

                    return error;
                }));

            //スキルボタンの非表示
            vm.AddCommandDelegater(
                "Battle_HideSkillButton",
                new CommandDelegater(false, 0, delegate (object[] arguments) {
                    string error;
                    
                    if (!BattleCommand)
                    {
                        error = "[battle_hide_skill_button]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    BattleCommand.HideSkillButton(out error);

                    return error;
                }));

            //スキルの実行
            vm.AddCommandDelegater(
                "Battle_ExecuteSkill",
                new CommandDelegater(false, 2, delegate (object[] arguments) {
                    string error;

                    var target = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;
                    var type = Converter.ObjectToInt(arguments[1], out error);
                    if (error != null) return error;

                    if (!BattleCommand)
                    {
                        error = "[battle_execute_skill]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    BattleCommand.ExecuteSkill(target, type, out error);

                    return error;
                }));

            //ポーズ
            vm.AddCommandDelegater(
                "Battle_Pause",
                new CommandDelegater(false, 1, delegate (object[] arguments)
                {
                    string error;

                    var enable = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;

                    if (!BattleCommand)
                    {
                        error = "[battle_execute_skill]が実行できません。フィールドがロードされているシーンで実行してください。";
                        return error;
                    }

                    BattleCommand.Pause(enable == 0 ? false : true);
                    return error;
                }));
        }
    }
}