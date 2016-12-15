using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;

namespace ProjectWitch.Talk.Compiler
{
    //これをパターンに追加すると演出全般が追加されるよ
    public class CreateCommandsOfBattle : Pattern_Component
    {
        //隠蔽
        private CreateCommandsOfBattle(List<PatternFormat> pattern) : base(pattern) { }

        public CreateCommandsOfBattle() : base()
        {
            List<PatternFormat> patternList = new List<PatternFormat>();

            patternList.Add(new CreateBattleShowSkillButtonCommand());
            patternList.Add(new CreateBattleHideSkillButtonCommand());
            patternList.Add(new CreateBattleExecuteSkillButtonCommand());

            mPatternList = patternList;
        }
    }

    //スキルボタンを表示
    public class CreateBattleShowSkillButtonCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "battle_show_skill_button";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if(arguments.ContainName("target"))
            {
                commandList.Add(arguments.Get("target"));
            }
            else
            {
                CompilerLog.Log(line, index, "引数targetが見つかりません。");
                return null;
            }
            

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Battle_ShowSkillButton"));
            return commandList.GetArray();
        }
    }

    //スキルボタンを非表示
    public class CreateBattleHideSkillButtonCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "battle_hide_skill_button";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Battle_HideSkillButton"));
            return commandList.GetArray();
        }
    }

    //スキルを実行
    public class CreateBattleExecuteSkillButtonCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "battle_execute_skill";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.ContainName("target"))
            {
                commandList.Add(arguments.Get("target"));
            }
            else
            {
                CompilerLog.Log(line, index, "引数targetが見つかりません。");
                return null;
            }

            if(arguments.ContainName("type"))
            {
                commandList.Add(arguments.Get("type"));
            }
            else
            {
                CompilerLog.Log(line, index, "引数typeが見つかりません。");
                return null;
            }
            
            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Battle_ExecuteSkill"));
            return commandList.GetArray();
        }
    }





}