using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq; //iOSで問題が起こるかも？
using System.Collections.Generic;
using System.Collections;
using System;

using GameData;
using Extention;

namespace Field
{
    public class FieldController : MonoBehaviour
    {
        //UI部分のルートゲームオブジェクト
        [SerializeField]
        private GameObject mUIObject = null;

        //FieldUIControllerのインスタンス
        [SerializeField]
        private FieldUIController mFieldUIController = null;
        public FieldUIController FieldUIController { get { return mFieldUIController; } private set { } }

        //CameraControllerのインスタンス
        [SerializeField]
        private CameraController mCameraController = null;
        public CameraController CameraController { get { return mCameraController; } private set { } }


        //内部変数
        //コルーチンが動いているかどうか
        private bool mIsCoroutineExec = false;

        //メニューが開くかどうか
        public bool MenuClickable { get; set; }
        public bool FlagClickable { get; set; }

        void Start()
        {
            MenuClickable = true;
            FlagClickable = true;

            //BGMの再生
            PlayBGM();
        }

        void Update()
        {
            var game = Game.GetInstance();

            //コルーチンが動いていたら何もしない
            if (mIsCoroutineExec) return;

            //コルーチンを起動
            if (game.CurrentTime <= 2)
            {
                //現在の時間が0~2のとき
                //プレイヤーのターン
                FieldUIController.ActiveTerritory = 0;
                StartCoroutine(PlayerTurn());
            }
            else
            {
                //メニュー操作を無効にする
                MenuClickable = false;
                FlagClickable = false;

                //カメラ操作を無効にする
                CameraController.IsPlayable = false;

                //現在の時間が3~のとき
                //敵のターン
                var territory = game.CurrentTime - 2;   //領地ＩＤを求める

                //領地IDが存在する範囲なら領地イベントを実行
                if (territory < game.TerritoryData.Count)
                {
                    FieldUIController.ActiveTerritory = territory;
                    StartCoroutine(EnemyTurn(territory));
                }
                else
                {
                    //メニュー操作を有効にする
                    MenuClickable = true;
                    FlagClickable = true;

                    //カメラ操作を有効にする
                    CameraController.IsPlayable = true;

                    //敵ターンが終了したので次のターンへ移行
                    game.CurrentTurn++;
                    game.CurrentTime = 0;

                    //オートセーブ
                    game.AutoSave();
                }
            }
        }

        //味方ターンの動作
        private IEnumerator PlayerTurn()
        {
            var game = Game.GetInstance();
            var currentTime = game.CurrentTime;

            //コルーチンの開始
            mIsCoroutineExec = true;

            //フィールドのイベントデータを取得
            var fieldEventData = game.FieldEventData;
            var eventlist = fieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.PlayerTurnBegin).ToList();

            //ターンはじめイベントを実行
            yield return EventExecute(eventlist);

            //戦闘オンだったら戦闘開始
            if (game.BattleIn.IsEvent)
                yield return StartCoroutine(CallBattle(game.BattleIn.AreaID, 0, true));

            //BGM再開
            PlayBGM();

            //一日の始まりのみの処理
            if (currentTime == 0)
            {
                //二重起動防止
                while (game.IsDialogShowd) yield return null;

                //ターンはじめ表示
                var settle = GetSettlement();
                string str = game.CurrentTurn.ToString() + "日目\n" +
                    "本日の収支：" + settle.ToString() + "M";
                game.ShowDialog("収支報告", str);
                while (game.IsDialogShowd) yield return null;

                //収支加算
                game.PlayerMana += settle;

                //マナ回復
                game.RecoverMana();

                //兵数回復
                game.RecoverUnit();

            }

            //時間が変化するまで待機
            while (currentTime == game.CurrentTime) yield return null;
            
            //コルーチン終了
            mIsCoroutineExec = false;
            yield return null;
        }

        //敵ターンの動作
        private IEnumerator EnemyTurn(int territory)
        {
            var game = Game.GetInstance();
            var ter = game.TerritoryData[territory];

#if DEBUG
            game.DebugMessage.Push(ter.OwnerName, ter.State);
#endif

            //コルーチン開始
            mIsCoroutineExec = true;

            //ダイアログが閉じるまで処理を進めない
            while (game.IsDialogShowd) yield return null;

            //フィールドのイベントデータを取得
            var fieldEventData = game.FieldEventData;
            var eventlist = fieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.EnemyTurnBegin).ToList();

            //占領されている場合はスルー
            if (ter.State != TerritoryDataFormat.TerritoryState.Dead)
            {

                //各領地に設定されたイベントを抜き出す
                eventlist = eventlist.Where(p => p.Area == territory).ToList();

                //ターンはじめイベント開始
                yield return StartCoroutine(EventExecute(eventlist));

                //ターンはじめイベントで戦闘フラグが立った
                //戦闘開始

                //すでに交戦状態に入っている場合
                if (ter.State == TerritoryDataFormat.TerritoryState.Active)
                {
                    foreach (var group in ter.GroupList)
                    {
                        var groupData = game.GroupData[group];
                        if (groupData.State != GroupDataFormat.GroupState.Active)
                            continue;

                        //攻め込む領地
                        int targetArea = GetDominationTarget(territory, group);
                        if (targetArea == -1) continue;

                        //攻めてきた演出
                        yield return StartCoroutine(DominationEffect(targetArea));

                        //敵ユニットのセット
                        SetEnemy(group, true);

                        //戦闘前スクリプトの開始
                        eventlist = fieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.EnemyBattle).ToList();
                        eventlist = eventlist.Where(p => p.Area == targetArea).ToList();
                        yield return StartCoroutine(EventExecute(eventlist));
                        while (game.IsTalk) yield return null;

                        //戦闘開始
                        yield return StartCoroutine(CallBattle(targetArea, territory, false));

                        //BGM再開
                        PlayBGM();
                    }
                    game.CurrentTime++;

                    //コルーチンの終了
                    mIsCoroutineExec = false;
                    yield break;

                }
                //入っていない場合
                else
                {
                    //そのまま終了
                    game.CurrentTime++;
                    mIsCoroutineExec = false;
                    yield break;
                }
            }
            else
            {
                game.CurrentTime++;

                //コルーチン終了
                mIsCoroutineExec = false;
            }

            yield return null;
        }

        //侵攻戦の開始
        public void DominationBattle(int area, int territory)
        {
            //戦闘開始
            StartCoroutine(_DominationBattle(area, territory));
        }
        private IEnumerator _DominationBattle(int area, int territory)
        {
            var game = Game.GetInstance();

            //戦闘前スクリプトの開始
            var eventlist = game.FieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.PlayerBattle).ToList();
            eventlist = eventlist.Where(p => p.Area == area).ToList();
            yield return StartCoroutine(EventExecute(eventlist));
            while (game.IsTalk) yield return null;

            if (!game.BattleIn.IsEvent)
            {
                //防衛戦を担当する敵グループを取得
                var group = GetDefenseGroup(area, territory);

                //グループからユニットをセット
                SetEnemy(group, false);
            }

            //先頭の開始
            yield return StartCoroutine(CallBattle(area, territory, true));
            yield return null;

            //時間を進める
            game.CurrentTime++;
            

        }

        //戦闘後処理
        public IEnumerator AfterBattle()
        {
            var game = Game.GetInstance();

            //フィールドUIを再ロード
            ShowUI();
            yield return null;
            game.HideNowLoading();
            yield return null;

            //領地の占領判定
            //オート戦闘の場合、占領判定はスクリプトで行う
            //侵攻戦で勝った場合 領地を占領する
            if (game.BattleIn.IsInvasion && game.BattleOut.IsWin)
                ChangeAreaOwner(game.BattleIn.AreaID, game.BattleIn.PlayerTerritory);
            //防衛戦で負けた場合 領地を奪われる
            else if (!game.BattleIn.IsInvasion && !game.BattleOut.IsWin)
                ChangeAreaOwner(game.BattleIn.AreaID, game.BattleIn.EnemyTerritory);

            //ユニットの死亡処理
            foreach(var unit in game.BattleOut.DeadUnits)
            {
                game.UnitData[unit].IsAlive = false;

                //すべての領地からユニットを除外
                foreach(var territory in game.TerritoryData)
                {
                    territory.RemoveUnit(unit);
                }

                //すべてのグループからユニットを除外
                foreach(var group in game.GroupData)
                {
                    group.UnitList.Remove(unit);
                }
            }

            //ユニットの捕獲処理
            var dist = game.BattleOut.CapturedUnits.Distinct().ToList();
            foreach(var unit in dist)
            {
                //敵領地のすべてのグループからユニットを除外
                var territory = game.TerritoryData[game.BattleIn.EnemyTerritory];
                territory.RemoveUnit(unit);

                //味方領地に追加
                var groupID = game.TerritoryData[game.BattleIn.PlayerTerritory].GroupList[0];
                game.GroupData[groupID].UnitList.Add(unit);
            }

            //ユニットの逃走処理
            foreach(var unit in game.BattleOut.EscapedUnits)
            {
                //回復処理
                var udata = game.UnitData[unit];
                udata.HP = (int)(udata.MaxHP * 0.3f);
                udata.SoldierNum = (int)(udata.MaxSoldierNum * 0.3f);
            }

            //グループの消滅処理
            foreach(var group in game.GroupData)
            {
                if (group.UnitList.Count == 0)
                    group.Kill();
            }


            //戦闘情報をリセット
            game.BattleIn.Reset();

            //戦闘後スクリプトの開始
            if(game.BattleOut.IsWin)    //戦闘勝利時のスクリプト
            {
                var exescript = game.ScenarioIn.NextA;
                if(exescript>=0)
                    game.CallScript(game.FieldEventData[exescript]);
                yield return null;
            }
            else                        //戦闘敗北時のスクリプト
            {
                var exescript = game.ScenarioIn.NextB;
                if(exescript>=0)
                    game.CallScript(game.FieldEventData[exescript]);
                yield return null;

            }
            while (game.IsTalk) yield return null;

            //UIの更新
            FieldUIController.AreaPointReset();

            game.BattleIn.Reset();
            game.ScenarioIn.Reset();

            yield return null;
        }

        //戦闘処理
        private IEnumerator CallBattle(int area, int territory, bool invation)
        {
            var game = Game.GetInstance();

            //戦闘情報の格納
            game.BattleIn.AreaID = area;
            game.BattleIn.EnemyTerritory = territory;
            game.BattleIn.IsInvasion = invation;

            //戦闘呼び出し
            HideUI();
            yield return StartCoroutine(game.CallPreBattle());
            
            yield return null;

            //戦闘終了まで待機
            while (game.IsBattle) yield return null;

            //戦闘終了処理
            yield return StartCoroutine(AfterBattle());
        }
        
        //イベント制御
        private IEnumerator EventExecute(List<EventDataFormat> eventlist)
        {
            var game = Game.GetInstance();

            for (int i = 0; i < eventlist.Count; i++)
            {
                //イベント実行フラグ
                bool isEventEnable = true;

                //味方の生存判定
                foreach (int unit in eventlist[i].ActorA)
                {
                    //ユニットが生きているか
                    if (!game.UnitData[unit].IsAlive)
                    {
                        isEventEnable = false;
                        break;
                    }

                    //自領地にユニットが含まれているか
                    var playerUnitList = game.GroupData[game.TerritoryData[0].GroupList[0]].UnitList;
                    if (playerUnitList.Contains(unit) == false)
                    {
                        //含まれていなかったらイベント棄却
                        isEventEnable = false;
                        break;
                    }
                }
                if (!isEventEnable) continue;

                //敵の生存判定
                foreach (int unit in eventlist[i].ActorB)
                {
                    isEventEnable = false;

                    //ユニットが生きているか
                    if (!game.UnitData[unit].IsAlive) break;

                    //任意の領地にユニットが含まれているか
                    for (int j = 1; j < game.TerritoryData.Count; j++)
                    {
                        //領地の所持するグループに所属するユニットから総当たりする
                        var groupList = game.TerritoryData[j].GroupList;
                        foreach (var groupID in groupList)
                        {
                            var group = game.GroupData[groupID];
                            if (group.UnitList.Contains(unit))
                            {
                                isEventEnable = true;
                                break;
                            }
                        }
                    }

                    //どの領地にも敵が見つからなかったらイベント棄却
                    if (!isEventEnable) break;
                }
                if (!isEventEnable) continue;

                //条件判定
                try
                {
                    bool result = false;
                    for (int j = 0; j < eventlist[i].If_Val.Count; j++)
                    {
                        if (eventlist[i].If_Val[j] != -1)  //条件なしの時If_Val == -1
                        {
                            int src = int.Parse(game.SystemMemory[eventlist[i].If_Val[j]]);
                            var imm = eventlist[i].If_Imm[j];

                            //演算結果用
                            switch (eventlist[i].If_Ope[j])
                            {
                                case EventDataFormat.OperationType.Equal:
                                    result = (src == imm);
                                    break;
                                case EventDataFormat.OperationType.Bigger:
                                    result = (src > imm);
                                    break;
                                case EventDataFormat.OperationType.BiggerEqual:
                                    result = (src >= imm);
                                    break;
                                case EventDataFormat.OperationType.Smaller:
                                    result = (src < imm);
                                    break;
                                case EventDataFormat.OperationType.SmallerEqual:
                                    result = (src <= imm);
                                    break;
                                case EventDataFormat.OperationType.NotEqual:
                                    result = (src != imm);
                                    break;
                                default:
                                    result = false;
                                    break;
                            }
                            if (!result) break;
                        }
                    }
                    if (!result) continue;
                }
                catch(ArgumentException e)
                {
                    Debug.Log(e.Message);
                }

                //実行
                game.CallScript(eventlist[i]);
                yield return null;
                while (game.IsTalk) yield return null;

                //スクリプトを一つ実行したら終了
                yield break;

            }
        }

        //地点の領主を変更する
        private void ChangeAreaOwner(int targetArea, int newOwner)
        {
            //データの変更
            var game = Game.GetInstance();
            game.ChangeAreaOwner(targetArea, newOwner);

            //表示の変更
            FieldUIController.ChangeAreaOwner(targetArea, newOwner);
        }

        //収入の計算
        private int GetSettlement()
        {
            var game = Game.GetInstance();
            //計算方法：現在の所持領地の所持マナの平均の10%

            //現在の所持地点を取得
            var areas = game.TerritoryData[0].AreaList;

            //領地がなかったら抜ける
            if (areas.Count == 0) return -1;

            //平均の算出
            var sum = 0;
            foreach (var area in areas) sum += game.AreaData[area].Mana;
            var ave = sum / areas.Count;

            //平均の10%を返す
            return ave / 10;

        }

        //侵攻する領地を侵攻ルートから決定する
        private int GetDominationTarget(int territory, int group)
        {
            var game = Game.GetInstance();
            var route = game.GroupData[group].DominationRoute;

            try
            {
                //終点から検索して、一番最初の自領地を見つける
                //その次の領地がプレイヤーの領地ならその領地をターゲットにする
                for (int i = route.Count - 2; i >= 0; i--)
                {
                    var area = game.AreaData[route[i]];

                    //終点から数えた一番最初の自領地を見つける
                    if (area.Owner == territory)
                    {
                        var nextArea = game.AreaData[route[i + 1]];
                        //次の領地がプレイヤーの領地かどうか
                        if (nextArea.Owner == 0)
                            return route[i + 1];
                    }
                }
            }
            catch (ArgumentException)
            {
                Debug.LogError("グループの侵攻ルートが不正です。データを確認してください : groupID:"+
                    group.ToString());
            }
            return -1;
        }

        //攻めてくる演出
        private IEnumerator DominationEffect(int targetArea)
        {
            var game = Game.GetInstance();
            var targetpos = game.AreaData[targetArea].Position;

            //カメラをその領地に移動
            yield return StartCoroutine(CameraController.MoveTo(targetpos));

            //エフェクトを表示
            yield return StartCoroutine(FieldUIController.ShowHiLightEffect(targetpos));

        }

        //敵情報のセット
        private void SetEnemy(int groupID, bool IsDomination)
        {
            var game = Game.GetInstance();
            var group = game.GroupData[groupID];

            //グループのユニットリストから3体取得
            var units = group.GetBattleUnits();

            //BattleInにセット
            game.BattleIn.EnemyUnits = units;

            //バトルタイプのセット
            game.BattleIn.EnemyBattleType = 
                (IsDomination) ? group.DominationType : group.DefenseType;

        }

        //優先度を基準に防御する敵ユニットを求める
        private int GetDefenseGroup(int area, int territory)
        {
            var game = Game.GetInstance();

            var groupIDs = game.TerritoryData[territory].GroupList;
            var groups = game.GroupData.GetFromIndex(groupIDs);

#if DEBUG
            foreach(var g in groups)
            {
                game.DebugMessage.Push(g.Name, g.State);
            }
#endif

            //その地域を防衛ラインに指定するグループをフィルタにかける
            groups = groups.Where(g => g.DominationRoute.Contains(area) == true).ToList();

            //防衛ラインに持つグループがいなかったら自営団を出す
            if (groups.Count == 0) return GroupDataFormat.GetDefaultID();

            //生きているグループをフィルタにかける
            groups = groups.Where(g => g.State == GroupDataFormat.GroupState.Active).ToList();

            //生きているグループがいなかったら自営団を出す
            if (groups.Count == 0) return GroupDataFormat.GetDefaultID();

            //グループを優先度でソートする
            groups = groups.OrderBy(p => p.DefensePriority).ToList();
            
            //指定領地を死守領地としているグループがあれば最優先
            foreach(var group in groups)
            {
                if (group.DominationRoute[0] == area)
                    return group.ID;
            }

            //死守領地でなければ優先度の先頭グループが返る
            return groups[0].ID;
        }

        //BGMの再生
        private void PlayBGM()
        {
            var game = Game.GetInstance();

            //すでに再生済みなら無視する
            if (game.SoundManager.GetCueName(SoundType.BGM).Equals(game.FieldBGM))
                return;

            //再生
            game.SoundManager.Play(game.FieldBGM, SoundType.BGM);
        }

        //BGMの停止
        private void StopBGM()
        {
            var game = Game.GetInstance();
            game.SoundManager.Stop(SoundType.BGM);
        }

        //UI再表示
        private void ShowUI()
        {
            mUIObject.SetActive(true);
        }

        //UI非表示
        private void HideUI()
        {
            mUIObject.SetActive(false);
        }

    }
}