//======================================
//author	:shotta
//summary	:その他、戦闘用などのコマンド
//======================================

using UnityEngine;
using System.Collections.Generic;

using Scenario.Command;
using Scenario.Pattern;
using Scenario.WorkSpace;

namespace Scenario.Compiler{
	public class CreateCommandsOfOther : Pattern_Component
	{
		//隠蔽
		private CreateCommandsOfOther(List<PatternFormat> pattern) : base(pattern){}

		public CreateCommandsOfOther() : base(){
			List<PatternFormat> patternList = new List<PatternFormat> ();
			patternList.Add (new CreateSetBattleUnitCommand ());
			patternList.Add (new CreateSetBattleAreaCommand ());
			patternList.Add (new CreateSetBattleNonPreCommand ());
			patternList.Add (new CreateSetAutoBattleCommand ());
			patternList.Add (new CreateCallEndingCommand ());

			patternList.Add (new CreateIfAliveCommand ());
			patternList.Add (new CreateIfDeathCommand ());
			patternList.Add (new CreateHealUnitAllCommand ());
			patternList.Add (new CreateHealUnitCommand ());
			patternList.Add (new CreateKillUnitCommand ());
			patternList.Add (new CreateEmployUnitCommand ());
			patternList.Add (new CreateChangeAreaOwnerCommand ());
			mPatternList = patternList;
		}
	}

	//戦闘データセット
	public class CreateSetBattleUnitCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "battle_unit_in";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("p0")) {
				commandList.Add(arguments.Get ("p0"));
			} else {
				CompilerLog.Log (line, index, "p0引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("p1")) {
				commandList.Add(arguments.Get ("p1"));
			} else {
				CompilerLog.Log (line, index, "p1引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("p2")) {
				commandList.Add(arguments.Get ("p2"));
			} else {
				CompilerLog.Log (line, index, "p2引数が不足しています。");
				return null;
			}

			if (arguments.ContainName ("e0")) {
				commandList.Add(arguments.Get ("e0"));
			} else {
				CompilerLog.Log (line, index, "e0引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("e1")) {
				commandList.Add(arguments.Get ("e1"));
			} else {
				CompilerLog.Log (line, index, "e1引数が不足しています。");
				return null;
			}
			if (arguments.ContainName ("e2")) {
				commandList.Add(arguments.Get ("e2"));
			} else {
				CompilerLog.Log (line, index, "e2引数が不足しています。");
				return null;
			}

			commandList.Add (new RunOrderCommand("SetBattleUnit"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//戦う領地を指定
	public class CreateSetBattleAreaCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "battle_area";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("id")) {
				commandList.Add(arguments.Get ("id"));
			} else {
				CompilerLog.Log (line, index, "id引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand("SetBattleArea"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//戦闘準備画面の非表示
	public class CreateSetBattleNonPreCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "battle_nonpre";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			commandList.Add (new RunOrderCommand("SetBattleNonPre"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//自動戦闘モード呼び出し
	public class CreateSetAutoBattleCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "battle_auto";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			commandList.Add (new RunOrderCommand("SetBattleAuto"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//エンディングを呼び出す
	public class CreateCallEndingCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "call_ending";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("index")) {
				commandList.Add(arguments.Get ("index"));
			} else {
				CompilerLog.Log (line, index, "index引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand("CallEnding"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//指定キャラの生存を確認
	public class CreateIfAliveCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "if_alive";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			commandList.Add (new SetArgumentCommand ("eqr"));
			if (arguments.ContainName ("unit")) {
				commandList.Add(arguments.Get ("unit"));
			} else {
				CompilerLog.Log (line, index, "unit引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand("IfAlive"));
			commandList.Add (new SetArgumentCommand (1));
			//次の移動インデックス用の領域
			commandList.Add (new SetArgumentCommand (0));
			commandList.Add (new RunOrderCommand ("if"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//指定キャラの生存を確認
	public class CreateIfDeathCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "if_death";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			commandList.Add (new SetArgumentCommand ("eqr"));
			if (arguments.ContainName ("unit")) {
				commandList.Add(arguments.Get ("unit"));
			} else {
				CompilerLog.Log (line, index, "unit引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand("IfDeath"));
			commandList.Add (new SetArgumentCommand (1));
			//次の移動インデックス用の領域
			commandList.Add (new SetArgumentCommand (0));
			commandList.Add (new RunOrderCommand ("if"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//指定領地のユニット回復
	public class CreateHealUnitAllCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "unit_heal_all";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("territory")) {
				commandList.Add(arguments.Get ("territory"));
			} else {
				CompilerLog.Log (line, index, "territory引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand("HealAllUnit"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//指定のユニット回復
	public class CreateHealUnitCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "unit_heal";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("unit")) {
				commandList.Add(arguments.Get ("unit"));
			} else {
				CompilerLog.Log (line, index, "unit引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand("HealUnit"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//指定のユニットを解雇
	public class CreateKillUnitCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "unit_kill";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("unit")) {
				commandList.Add(arguments.Get ("unit"));
			} else {
				CompilerLog.Log (line, index, "unit引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand("FireUnit"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//指定領地のユニット回復
	public class CreateEmployUnitCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "unit_employ";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("unit")) {
				commandList.Add(arguments.Get ("unit"));
			} else {
				CompilerLog.Log (line, index, "unit引数が不足しています。");
				return null;
			}
			commandList.Add (new RunOrderCommand("EmployUnit"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}

	//指定領地のユニット回復
	public class CreateChangeAreaOwnerCommand : Pattern_TagFormat
	{
		protected override string TagName(){
			return "change_area_owner";
		}

		protected override CommandFormat[] CreateCommand(ArgumentDictionary arguments, int line, int index)
		{
			CommandList commandList = new CommandList();

			if (arguments.ContainName ("area")) {
				commandList.Add(arguments.Get ("area"));
			} else {
				CompilerLog.Log (line, index, "area引数が不足しています。");
				return null;
            }
            if (arguments.ContainName("owner"))
            {
                commandList.Add(arguments.Get("owner"));
            }
            else
            {
                CompilerLog.Log(line, index, "owner引数が不足しています。");
                return null;
            }
            commandList.Add (new RunOrderCommand("ChangeAreaOwner"));

			if (arguments.Count > 0) {
				CompilerLog.Log(line, index, "無効な引数があります。");
				return null;
			}
			return commandList.GetArray ();
		}
	}
}