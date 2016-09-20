﻿//==========================================================
//	author	:shotta
//	summary	:Gameクラスとのアダプタ
//==========================================================

using UnityEngine;

using System.Collections.Generic;

using GameData;
using Scenario.Compiler;
using Scenario.Command;
using Scenario.WorkSpace;

namespace Scenario.WorkSpace
{
	//シナリオ操作時のワークスペース
	public class GameWorkSpace : MonoBehaviour
	{
		//battle_unit_inタグ
		//	p		:p?引数、0~2
		//	e		:e?引数、0~2
		//	error	:エラーメッセージ
		void BattleUnitIn(int[] p, int[] e, out string error)
		{
			error = null;

		}

		//battle_nonpreタグ
		//	error	:エラーメッセージ
		void BattleNonPre(out string error)
		{
			error = null;

		}

		//battle_autoタグ
		//	error	:エラーメッセージ
		void BattleAuto(out string error)
		{
			error = null;

		}

		//call_endingタグ
		//	index	:エンディングのインデックス
		//	error	:エラーメッセージ
		void CallEnding(int index, out string error)
		{
			error = null;

		}

		//if_aliveタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		bool IfAlive(int[] unitIds, out string error)
		{
			error = null;

			return false;
		}

		//if_deathタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		bool IfDeath(int[] unitIds, out string error)
		{
			error = null;

			return false;
		}

		//unit_heal_allタグ
		//	territory:指定された領地のID
		//	error	 :エラーメッセージ
		void UnitHealAll(int territory, out string error)
		{
			error = null;

		}

		//unit_healタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		void UnitHeal(int[] unitIds, out string error)
		{
			error = null;

		}

		//unit_killタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		void UnitKill(int[] unitIds, out string error)
		{
			error = null;

		}

		//unit_employタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		void UnitEmploy(int[] unitIds, out string error)
		{
			error = null;

		}

        //スクリプト終了
        public void ScriptEnd()
        {
            Debug.Log("終了");
        }

        public void SetCommandDelegaters(VirtualMachine vm)
		{
			//システム変数
			//取得
			vm.AddCommandDelegater (
				"GetSystem",
				new CommandDelegater (true, 1, delegate(object[] arguments) {
					string error;
					int index = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;

					VirtualMemory memory = Game.GetInstance().SystemMemory;
					int count = memory.Memory.Count;
					if (0 <= index && index<count){
						arguments[1] = memory.Memory[index];
						return null;
					}
					return "システム変数のインデックスは0 ~ "+ (count - 1) +"です(" + index + ")";
				}));
			//入力
			vm.AddCommandDelegater (
				"SetSystem",
				new CommandDelegater (false, 2, delegate(object[] arguments) {
					string error;
					int index = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;

					VirtualMemory memory = Game.GetInstance().SystemMemory;
					int count = memory.Memory.Count;
					object value = arguments[1];
					if (0 <= index && index<count){
						memory.Memory[index] = value;
						return null;
					}
					return "システム変数のインデックスは0 ~ "+ (count - 1) +"です(" + index + ")";
				}));

			//戦闘データのセット
			vm.AddCommandDelegater (
				"SetBattleUnit",
				new CommandDelegater (false, 6, delegate(object[] arguments) {
					string error;
					int[] p = new int[3];
					p[0] = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					p[1] = Converter.ObjectToInt(arguments[1], out error);
					if (error != null) return error;
					p[2] = Converter.ObjectToInt(arguments[2], out error);
					if (error != null) return error;

					int[] e = new int[3];
					e[0] = Converter.ObjectToInt(arguments[3], out error);
					if (error != null) return error;
					e[1] = Converter.ObjectToInt(arguments[4], out error);
					if (error != null) return error;
					e[2] = Converter.ObjectToInt(arguments[5], out error);
					if (error != null) return error;

					BattleUnitIn(p, e, out error);
					return error;
				}));

			//戦闘データのセット
			vm.AddCommandDelegater (
				"SetBattleNonPre",
				new CommandDelegater (false, 0, delegate(object[] arguments) {
					string error;
					BattleNonPre(out error);
					return error;
				}));
			
			//戦闘データのセット
			vm.AddCommandDelegater (
				"SetBattleAuto",
				new CommandDelegater (false, 0, delegate(object[] arguments) {
					string error;
					BattleAuto(out error);
					return error;
				}));

			//エンディング呼び出し
			vm.AddCommandDelegater (
				"CallEnding",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					int index = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					CallEnding (index, out error);
					return error;
				}));

			//指定キャラの生存フラグ
			vm.AddCommandDelegater (
				"IfAlive",
				new CommandDelegater (true, 1, delegate(object[] arguments) {
					string error;
					List<object> list = Converter.ObjectToList(arguments[0], out error);
					if (error != null) return error;

					int[] unitIds = new int[list.Count];
					for(int i=0;i<list.Count;i++)
					{
						unitIds[i] = Converter.ObjectToInt(list[i], out error);
						if (error != null) return error;
					}
					if (IfAlive(unitIds, out error))
						arguments[1] = 1;
					else
						arguments[1] = 0;
					return error;
				}));

			//指定キャラの死亡フラグ
			vm.AddCommandDelegater (
				"IfDeath",
				new CommandDelegater (true, 1, delegate(object[] arguments) {
					string error;
					List<object> list = Converter.ObjectToList(arguments[0], out error);
					if (error != null) return error;

					int[] unitIds = new int[list.Count];
					for(int i=0;i<list.Count;i++)
					{
						unitIds[i] = Converter.ObjectToInt(list[i], out error);
						if (error != null) return error;
					}
					if (IfDeath(unitIds, out error))
						arguments[1] = 1;
					else
						arguments[1] = 0;
					return error;
				}));
			
			//指定領地の全ユニットを回復
			vm.AddCommandDelegater (
				"HealAllUnit",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					int territoryId = Converter.ObjectToInt(arguments[0], out error);
					if (error != null) return error;
					UnitHealAll(territoryId, out error);
					return error;
				}));

			//指定ユニットの体力を回復
			vm.AddCommandDelegater (
				"HealUnit",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					List<object> list = Converter.ObjectToList(arguments[0], out error);
					if (error != null) return error;

					int[] unitIds = new int[list.Count];
					for(int i=0;i<list.Count;i++)
					{
						unitIds[i] = Converter.ObjectToInt(list[i], out error);
						if (error != null) return error;
					}
					UnitHeal(unitIds, out error);
					return error;
				}));

			//指定ユニットを解雇
			vm.AddCommandDelegater (
				"FireUnit",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					List<object> list = Converter.ObjectToList(arguments[0], out error);
					if (error != null) return error;

					int[] unitIds = new int[list.Count];
					for(int i=0;i<list.Count;i++)
					{
						unitIds[i] = Converter.ObjectToInt(list[i], out error);
						if (error != null) return error;
					}
					UnitKill(unitIds, out error);
					return error;
				}));

			//指定ユニットを解雇
			vm.AddCommandDelegater (
				"EmployUnit",
				new CommandDelegater (false, 1, delegate(object[] arguments) {
					string error;
					List<object> list = Converter.ObjectToList(arguments[0], out error);
					if (error != null) return error;

					int[] unitIds = new int[list.Count];
					for(int i=0;i<list.Count;i++)
					{
						unitIds[i] = Converter.ObjectToInt(list[i], out error);
						if (error != null) return error;
					}
					UnitEmploy(unitIds, out error);
					return error;
				}));
		}
	}
}