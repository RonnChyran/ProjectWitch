using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;

namespace ProjectWitch.Talk.Compiler
{
    //これをパターンに追加すると演出全般が追加されるよ
    public class CreateCommandsOfPreBattle : Pattern_Component
    {
        //隠蔽
        private CreateCommandsOfPreBattle(List<PatternFormat> pattern) : base(pattern) { }

        public CreateCommandsOfPreBattle() : base()
        {
            List<PatternFormat> patternList = new List<PatternFormat>();

            patternList.Add(new CreatePreBattleSetUnitCommand());
            patternList.Add(new CreatePreBattleRemoveUnitCommand());
            patternList.Add(new CreatePreBattleSetCardCommand());
            patternList.Add(new CreatePreBattleRemoveCardCommand());
            patternList.Add(new CreatePreBattleCallBattleCommand());

            mPatternList = patternList;
        }

        //ユニットのセット
        public class CreatePreBattleSetUnitCommand : Pattern_TagFormat
        {
            protected override string TagName()
            {
                return "prebattle_setunit";
            }
            protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
            {
                CommandList commandList = new CommandList();
                if (arguments.ContainName("index"))
                {
                    commandList.Add(arguments.Get("index"));
                }
                else
                {
                    CompilerLog.Log(line, index, "引数indexが不足しています。");
                    return null;
                }

                if (arguments.Count > 0)
                {
                    CompilerLog.Log(line, index, "無効な引数があります。");
                    return null;
                }
                commandList.Add(new RunOrderCommand("PreBattle_SetUnit"));
                return commandList.GetArray();
            }
        }

        //ユニットの除外
        public class CreatePreBattleRemoveUnitCommand : Pattern_TagFormat
        {
            protected override string TagName()
            {
                return "prebattle_removeunit";
            }
            protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
            {
                CommandList commandList = new CommandList();
                if (arguments.ContainName("index"))
                {
                    commandList.Add(arguments.Get("index"));
                }
                else
                {
                    CompilerLog.Log(line, index, "引数indexが不足しています。");
                    return null;
                }

                if (arguments.Count > 0)
                {
                    CompilerLog.Log(line, index, "無効な引数があります。");
                    return null;
                }
                commandList.Add(new RunOrderCommand("PreBattle_RemoveUnit"));
                return commandList.GetArray();
            }
        }

        //カードの追加
        public class CreatePreBattleSetCardCommand : Pattern_TagFormat
        {
            protected override string TagName()
            {
                return "prebattle_setcard";
            }
            protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
            {
                CommandList commandList = new CommandList();
                if (arguments.ContainName("index"))
                {
                    commandList.Add(arguments.Get("index"));
                }
                else
                {
                    CompilerLog.Log(line, index, "引数indexが不足しています。");
                    return null;
                }

                if (arguments.Count > 0)
                {
                    CompilerLog.Log(line, index, "無効な引数があります。");
                    return null;
                }
                commandList.Add(new RunOrderCommand("PreBattle_SetCard"));
                return commandList.GetArray();
            }
        }

        //カードの除外
        public class CreatePreBattleRemoveCardCommand : Pattern_TagFormat
        {
            protected override string TagName()
            {
                return "prebattle_removecard";
            }
            protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
            {
                CommandList commandList = new CommandList();
                if (arguments.ContainName("index"))
                {
                    commandList.Add(arguments.Get("index"));
                }
                else
                {
                    CompilerLog.Log(line, index, "引数indexが不足しています。");
                    return null;
                }

                if (arguments.Count > 0)
                {
                    CompilerLog.Log(line, index, "無効な引数があります。");
                    return null;
                }
                commandList.Add(new RunOrderCommand("PreBattle_RemoveCard"));
                return commandList.GetArray();
            }
        }

        //バトルの呼び出し
        //ユニットの除外
        public class CreatePreBattleCallBattleCommand : Pattern_TagFormat
        {
            protected override string TagName()
            {
                return "prebattle_callbattle";
            }
            protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
            {
                CommandList commandList = new CommandList();

                if (arguments.Count > 0)
                {
                    CompilerLog.Log(line, index, "無効な引数があります。");
                    return null;
                }
                commandList.Add(new RunOrderCommand("PreBattle_CallBattle"));
                return commandList.GetArray();
            }
        }

    }
}