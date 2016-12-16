using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;
using System;

namespace ProjectWitch.Talk.Compiler
{
    //これをパターンに追加すると演出全般が追加されるよ
    public class CreateCommandsOfField : Pattern_Component
    {
        //隠蔽
        private CreateCommandsOfField(List<PatternFormat> pattern) : base(pattern) { }

        public CreateCommandsOfField() : base()
        {
            List<PatternFormat> patternList = new List<PatternFormat>();
            patternList.Add(new CreateFieldAreaHilightCommand());

            patternList.Add(new CreateFieldOpenAreaWindowCommand());
            patternList.Add(new CreateFieldCloseAreaWindowCommand());
            patternList.Add(new CreateFieldCallBattleFromAreaWindowCommand());
            patternList.Add(new CreateFieldOpenMenuCommand());

            mPatternList = patternList;
        }
    }

    //エリア強調エフェクト
    public class CreateFieldAreaHilightCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_area_hilight";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();
            if (arguments.ContainName("id"))
            {
                commandList.Add(arguments.Get("id"));
            }
            else {
                CompilerLog.Log(line, index, "引数idが不足しています。");
                return null;
            }

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Field_HilightArea"));
            commandList.Add(new RunOrderCommand("SetUpdater"));
            return commandList.GetArray();
        }
    }

    //エリアウィンドウの表示非表示
    public class CreateFieldOpenAreaWindowCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_open_areawindow";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();
            if (arguments.ContainName("id"))
            {
                commandList.Add(arguments.Get("id"));
            }
            else {
                CompilerLog.Log(line, index, "引数idが不足しています。");
                return null;
            }

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Field_OpenAreaWindow"));
            return commandList.GetArray();
        }
    }
    public class CreateFieldCloseAreaWindowCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_close_areawindow";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }

            commandList.Add(new RunOrderCommand("Field_CloseAreaWindow"));
            return commandList.GetArray();
        }
    }

    //エリアウィンドウから先頭を呼び出す
    public class CreateFieldCallBattleFromAreaWindowCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_callbattle_areawindow";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Field_CallBattle_AreaWindow"));
            return commandList.GetArray();
        }
    }

    //メニューを開く
    public class CreateFieldOpenMenuCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_open_menu";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Field_OpenMenu"));
            return commandList.GetArray();
        }
    }


}