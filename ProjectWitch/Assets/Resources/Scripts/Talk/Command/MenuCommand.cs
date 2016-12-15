using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;

namespace ProjectWitch.Talk.Compiler
{
    //これをパターンに追加すると演出全般が追加されるよ
    public class CreateCommandsOfMenu : Pattern_Component
    {
        //隠蔽
        private CreateCommandsOfMenu(List<PatternFormat> pattern) : base(pattern) { }

        public CreateCommandsOfMenu() : base()
        {
            List<PatternFormat> patternList = new List<PatternFormat>();
            patternList.Add(new CreateMenuCloseCommand());
            patternList.Add(new CreateMenuGoArmyCommand());
            patternList.Add(new CreateMenuArmyOpenUnitCommand());
            
            mPatternList = patternList;
        }
    }

    //メニューの非表示
    public class CreateMenuCloseCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "menu_close";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Menu_Close"));
            return commandList.GetArray();
        }
    }

    //軍団を開く
    public class CreateMenuGoArmyCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "menu_go_army";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Menu_GoArmy"));
            return commandList.GetArray();
        }
    }

    //軍団で、特定のユニットのメニューを開く
    public class CreateMenuArmyOpenUnitCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "menu_army_open_unit";
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
                CompilerLog.Log("引数indexが足りません。");
                return null;
            }

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }

            commandList.Add(new RunOrderCommand("Menu_ArmyOpenUnit"));
            return commandList.GetArray();
        }
    }

}