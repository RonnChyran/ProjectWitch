using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using ProjectWitch.Talk.Pattern;
using ProjectWitch.Talk.Command;

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
            patternList.Add(new CreateFieldShowCursorCommand());
            patternList.Add(new CreateFieldHideCursorCommand());
            patternList.Add(new CreateFieldShowAccentCursorCommand());
            patternList.Add(new CreateFieldHideAccentCursorCommand());

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

    //カーソルの表示非表示
    public class CreateFieldShowCursorCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_show_cursor";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Field_ShowCursor"));
            return commandList.GetArray();
        }
    }
    public class CreateFieldHideCursorCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_hide_cursor";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Field_HideCursor"));
            return commandList.GetArray();
        }
    }

    //強調カーソルの表示非表示
    public class CreateFieldShowAccentCursorCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_show_accentcursor";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.ContainName("x"))
            {
                commandList.Add(arguments.Get("x"));
            }
            else
            {
                CompilerLog.Log(line, index, "引数xが不足しています。");
                return null;
            }

            if (arguments.ContainName("y"))
            {
                commandList.Add(arguments.Get("y"));
            }
            else
            {
                CompilerLog.Log(line, index, "引数yが不足しています。");
                return null;
            }

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Field_ShowAccentCursor"));
            return commandList.GetArray();
        }
    }
    public class CreateFieldHideAccentCursorCommand : Pattern_TagFormat
    {
        protected override string TagName()
        {
            return "field_hide_accentcursor";
        }
        protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
        {
            CommandList commandList = new CommandList();

            if (arguments.Count > 0)
            {
                CompilerLog.Log(line, index, "無効な引数があります。");
                return null;
            }
            commandList.Add(new RunOrderCommand("Field_HideAccentCursor"));
            return commandList.GetArray();
        }
    }


}