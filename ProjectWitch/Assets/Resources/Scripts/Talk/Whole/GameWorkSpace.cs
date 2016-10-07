//==========================================================
//	author	:shotta
//	summary	:Gameクラスとのアダプタ
//==========================================================

using UnityEngine;

using System.Collections.Generic;
using System;

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

            var game = Game.GetInstance();
            for(int i=0; i<2; i++)
            {
                if (p[i] == -1) continue;

                game.BattleIn.PlayerUnits[i] = p[i];
            }

            for(int i=0; i<2; i++)
            {
                if (e[i] == -1) continue;

                game.BattleIn.EnemyUnits[i] = e[i];
            }

            game.BattleIn.IsEvent = true;
		}

        //battle_areaタグ
        //	id		:戦闘領地を指定
        //	error	:エラーメッセージ
        void BattleArea(int id, out string error)
        {
            error = null;

        }

        //battle_nonpreタグ
        //	error	:エラーメッセージ
        void BattleNonPre(out string error)
		{
			error = null;
            var game = Game.GetInstance();

            //戦闘準備画面を用いない
            game.UsePreBattle = false;

        }

		//battle_autoタグ
		//	error	:エラーメッセージ
		void BattleAuto(out string error)
		{
			error = null;
            var game = Game.GetInstance();

            //次の戦闘をオート戦闘に
            game.BattleIn.IsAuto = true;

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
		bool IfAlive(int[] unitIDs, out string error)
		{
			error = null;
            var game = Game.GetInstance();
            
            foreach(var unitID in unitIDs)
            {
                try
                {
                    if (!game.UnitData[unitID].IsAlive)
                        return false;
                }
                catch(ArgumentException e)
                {
                    error = "unitIDが存在しない範囲の値です";
                    error += e.Message;
                }
            }

			return true;
		}

		//if_deathタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		bool IfDeath(int[] unitIDs, out string error)
		{
			error = null;
            var game = Game.GetInstance();

            foreach(var unitID in unitIDs)
            {
                try
                {
                    if (game.UnitData[unitID].IsAlive)
                        return false;
                }
                catch(ArgumentException e)
                {
                    error = "unitIDが存在しない範囲の値です";
                    error += e.Message;
                }
            }
			return true;
		}

		//unit_heal_allタグ
		//	territory:指定された領地のID
		//	error	 :エラーメッセージ
		void UnitHealAll(int territory, out string error)
		{
			error = null;
            var game = Game.GetInstance();

            //指定領地のユニットIDをすべて受け取る
            var unitIDs = game.TerritoryData[territory].UnitList;

            foreach(var unitID in unitIDs)
            {
                try
                {
                    game.UnitData[unitID].HP = game.UnitData[unitID].MaxHP;
                    game.UnitData[unitID].SoldierNum = game.UnitData[unitID].MaxSoldierNum;
                }
                catch(ArgumentException e)
                {
                    error = "unitIDが存在しない範囲の値です:";
                    error += e.Message;
                }
            }


		}

		//unit_healタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		void UnitHeal(int[] unitIDs, out string error)
		{
			error = null;
            var game = Game.GetInstance();

            foreach(var unitID in unitIDs)
            {
                try
                {
                    game.UnitData[unitID].HP = game.UnitData[unitID].MaxHP;
                    game.UnitData[unitID].SoldierNum = game.UnitData[unitID].MaxSoldierNum;
                }
                catch(ArgumentException e)
                {
                    error = "unitIDが存在しない範囲です";
                    error += e.Message;
                }
            }

		}

		//unit_killタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		void UnitKill(int[] unitIDs, out string error)
		{
			error = null;
            var game = Game.GetInstance();

            foreach (var unitID in unitIDs)
            {
               game.TerritoryData[0].UnitList.Remove(unitID);
            }

        }

		//unit_employタグ
		//	unitIds	:指定されたユニットのIDの配列
		//	error	:エラーメッセージ
		void UnitEmploy(int[] unitIDs, out string error)
		{
			error = null;
            var game = Game.GetInstance();
            var unitList = game.TerritoryData[0].UnitList;

            foreach (var unitID in unitIDs)
            {
                //すでに雇っていた場合は無効にする
                if (unitList.Contains(unitID))
                    continue;

                //ユニットを自領地に含める
                unitList.Add(unitID);
            }

        }

        [SerializeField]
        private GameObject mTalkController;

        //インスペクターから登録してくだせ
        [SerializeField]
        private ScenarioWorkSpace mSWS;
        
        //スタート時に仮想マシンへ割り込みする処理
        private class ScriptBeginAnimator : PauseUpdater
        {
            //継続時間
            float mTime = 2.0f;
            //現在の時間
            private float mCurrentTime = 0.0f;
            //アニメーションコンポーネント
            private Animator mcTWindow;
            private Animator mcNWindow;

            public ScriptBeginAnimator(float time, Animator cTWindow, Animator cNWindow)
            {
                mTime = time;
                mcTWindow = cTWindow;
                mcNWindow = cNWindow;
                mcTWindow.SetBool("IsBegin", true);
                mcNWindow.SetBool("IsBegin", true);
            }

            //初期化処理
            public override void Setup()
            {
            }
            //更新処理
            public override void Update(float deltaTime)
            {
                mCurrentTime += deltaTime;
                if (mCurrentTime > mTime)
                {
                    SetActive(false);//アップデータを抜ける処理
                }
            }
            //終了処理
            public override void Finish()
            {

            }
        }

        [SerializeField]
        private float mBeginTime = 2.0f;

        //スクリプト開始
        public void ScriptBegin()
        {
            var cTWindow = mTWindow.GetComponent<Animator>();
            var cNWindow = mNWindow.GetComponent<Animator>();

            //割り込みを登録
            mSWS.SetUpdater(new ScriptBeginAnimator(mBeginTime,cTWindow,cNWindow));
            Debug.Log("開始");
        }

        [SerializeField]
        private GameObject mTWindow;
        [SerializeField]
        private GameObject mTWindowBack;
        [SerializeField]
        private GameObject mNWindow;

        //スクリプト終了
        public void ScriptEnd()
        {
            var cTWindow = mTWindow.GetComponent<Animator>();
            var cTWindowBack = mTWindowBack.GetComponent<Animator>();
            var cNWindow = mNWindow.GetComponent<Animator>();

            cTWindow.SetBool("IsEnd", true);
            cTWindowBack.SetBool("IsEnd", true);
            cNWindow.SetBool("IsEnd", true);

           // StartCoroutine(mTalkController.GetComponent<TalkController>().EndScript());
        }

        //change_owner_areaタグ
        //	id		:指定された領地のID
        //	error	:エラーメッセージ
        void ChangeAreaOwner(int id, out string error)
        {
            error = null;

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
						memory.Memory[index] = value.ToString();
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
            vm.AddCommandDelegater(
                "SetBattleArea",
                new CommandDelegater(false, 1, delegate (object[] arguments) {
                    string error;
                    int id = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;
                    BattleArea(id, out error);
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

            //戦闘データのセット
            vm.AddCommandDelegater(
                "ChangeAreaOwner",
                new CommandDelegater(false, 1, delegate (object[] arguments) {
                    string error;
                    int id = Converter.ObjectToInt(arguments[0], out error);
                    if (error != null) return error;
                    ChangeAreaOwner(id, out error);
                    return error;
                }));
        }
	}
}