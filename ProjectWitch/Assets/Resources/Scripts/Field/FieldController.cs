using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq; //iOSで問題が起こるかも？
using System.Collections.Generic;
using System.Collections;
using System;


using ProjectWitch.Extention;

namespace ProjectWitch.Field
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

        //Talkシーン用のヘルパーコマンド
        [SerializeField]
        private TalkCommandHelper mTalkCommandHelper = null;
        public TalkCommandHelper TalkCommandHelper { get{ return mTalkCommandHelper; } private set { } }

        //収入計算用の定数
        [SerializeField]
        private int mIncomesFactor = 200;

        //内部変数
        //コルーチンが動いているかどうか
        private bool mIsCoroutineExec = false;

        //メニューが開くかどうか
        public bool MenuClickable { get; set; }
        public bool FlagClickable { get; set; }

        protected virtual void Start()
        {
            var game = Game.GetInstance();
            game.HideNowLoading();

            //スクリプト初期化
            game.ScenarioIn.Reset();

            MenuClickable = true;
            FlagClickable = true;

            //BGMの再生
            PlayBGM();
        }

        protected virtual void Update()
        {

            if (!mIsCoroutineExec)
                StartCoroutine(TurnProcess());
        }

        //ターンの流れ動作
        private IEnumerator TurnProcess()
        {
            mIsCoroutineExec = true;

            //メニュー操作を無効にする
            MenuClickable = false;
            FlagClickable = false;


            var game = Game.GetInstance();
            var currentTime = game.GameData.CurrentTime;

            //行動数の更新
            game.GameData.Territory[0].ActionCount = 3;
            UpdateActionCount();

            //フィールドのイベントデータを取得
            var fieldEventData = game.GameData.FieldEvent;


            //プレイヤーターンの開始
            #region Player Turn

            //行動領地をプレイヤーにセット
            FieldUIController.ActiveTerritory = 0;
            var eventlist = fieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.PlayerTurnBegin).ToList();

            //一日の始まりのみの処理
            if (currentTime == 0)
            {
                //二重起動防止
                while (game.IsDialogShowd) yield return null;

                //ターンはじめ表示
                var settle = GetSettlement();
                string str = game.GameData.CurrentTurn.ToString() + "日目\n" +
                    "本日の収支：" + settle.ToString() + "M";
                game.ShowDialog("収支報告", str);
                while (game.IsDialogShowd) yield return null;

                //収支加算
                game.GameData.PlayerMana += settle;

                //マナ回復
                game.RecoverMana();

                //兵数回復
                game.RecoverUnit();

            }

            //プレイヤー３ターン
            while(game.GameData.CurrentTime < 3)
            {
                //ターン数更新
                game.GameData.Territory[0].ActionCount = 3 - game.GameData.CurrentTime;

                //カメラ操作を無効にする
                CameraController.IsPlayable = false;

                //ターンはじめイベントを実行
                yield return EventExecute(eventlist);

                //戦闘オンだったら戦闘開始
                if (game.BattleIn.IsEvent)
                    yield return StartCoroutine(CallBattle(game.BattleIn.AreaID, 0, true));

                //BGM再開
                PlayBGM();
               
                //カメラ操作を有効にする
                CameraController.IsPlayable = true;

                //メニュー操作を有効にする
                MenuClickable = true;
                FlagClickable = true;

                //時間が変化するまで待機
                while (currentTime == game.GameData.CurrentTime) yield return null;

                //カメラ操作を無効にする
                CameraController.IsPlayable = false;

                //メニュー操作を無効にする
                MenuClickable = false;
                FlagClickable = false;

                currentTime = game.GameData.CurrentTime;

                //領地の行動数を更新
                UpdateActionCount();
            }
            game.GameData.Territory[0].ActionCount = 0;

            //プレイヤーターン終了
            yield return null;

            #endregion

            #region Enemy Turn

            //カメラ操作を無効にする
            CameraController.IsPlayable = false;
            
            for(int i=1; i<game.GameData.Territory.Count; i++)
            {
                var ter = game.GameData.Territory[i];

                //有効な領地を更新
                FieldUIController.ActiveTerritory = i;

                //ダイアログが閉じるまで処理を進めない
                while (game.IsDialogShowd) yield return null;

                //フィールドのイベントデータを取得
                eventlist = fieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.EnemyTurnBegin).ToList();

                //占領されている場合はスルー
                if (ter.State != TerritoryDataFormat.TerritoryState.Dead)
                {

                    //各領地に設定されたイベントを抜き出す
                    eventlist = eventlist.Where(p => p.Area == i).ToList();

                    //ターンはじめイベント開始
                    yield return StartCoroutine(EventExecute(eventlist));

                    //ターンはじめイベントで戦闘フラグが立った
                    //戦闘開始

                    //すでに交戦状態に入っている場合
                    if (ter.State == TerritoryDataFormat.TerritoryState.Active)
                    {
                        foreach (var group in ter.GroupList)
                        {
                            var groupData = game.GameData.Group[group];
                            if (groupData.State != GroupDataFormat.GroupState.Active)
                                continue;

                            //攻め込む領地
                            int targetArea = GetDominationTarget(i, group);
                            if (targetArea == -1)
                            {
                                ter.ActionCount--;
                                continue;
                            }

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
                            yield return StartCoroutine(CallBattle(targetArea, i, false));

                            //BGM再開
                            PlayBGM();

                            //行動数を更新
                            ter.ActionCount--;

                            //領地が占領されたら抜ける
                            if (ter.State == TerritoryDataFormat.TerritoryState.Dead)
                                break;
                        }
                    }
                }
            }
            #endregion

            mCameraController.IsPlayable = true;
            game.GameData.CurrentTime = 0;
            game.GameData.CurrentTurn++;
            game.GameData.Territory[0].ResetIsBattleFlag();

            mIsCoroutineExec = false;

            yield return null;
        }

        //侵攻戦の開始
        public void DominationBattle(int area, int territory)
        {
            //カメラ操作を無効にする
            CameraController.IsPlayable = false;

            //メニュー操作を無効にする
            MenuClickable = false;
            FlagClickable = false;

            //戦闘開始
            StartCoroutine(_DominationBattle(area, territory));
        }
        private IEnumerator _DominationBattle(int area, int territory)
        {
            var game = Game.GetInstance();

            //戦闘前スクリプトの開始
            var eventlist = game.GameData.FieldEvent.Where(p => p.Timing == EventDataFormat.TimingType.PlayerBattle).ToList();
            eventlist = eventlist.Where(p => p.Area == area).ToList();
            yield return StartCoroutine(EventExecute(eventlist));
            while (game.IsTalk) yield return null;


            if (game.BattleIn.IsEnable)
            {
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
            }
            else
            {
                //時間を進める
                if (game.GameData.CurrentTime <= 2)
                    game.GameData.CurrentTime++;
                yield return null;
            }

        }

        //戦闘後処理
        public IEnumerator AfterBattle()
        {
            var game = Game.GetInstance();

            //領地の占領判定
            //オート戦闘の場合、占領判定はスクリプトで行う
            //侵攻戦で勝った場合 領地を占領する
            if (!game.BattleIn.IsAuto)
            {
                if (game.BattleIn.IsInvasion && game.BattleOut.IsWin)
                    ChangeAreaOwner(game.BattleIn.AreaID, game.BattleIn.PlayerTerritory);
                //防衛戦で負けた場合 領地を奪われる
                else if (!game.BattleIn.IsInvasion && !game.BattleOut.IsWin)
                    ChangeAreaOwner(game.BattleIn.AreaID, game.BattleIn.EnemyTerritory);
            }

            //戦闘に出したユニットの戦闘済み判定
            //戦闘済みフラグは味方全ユニットと、捕獲したユニットに立てる
            foreach(var unit in game.BattleIn.PlayerUnits)
            {
                if(unit >= 0)
                    game.GameData.Unit[unit].IsBattled = true;
            }
            foreach(var unit in game.BattleOut.CapturedUnits)
            {
                game.GameData.Unit[unit].IsBattled = true;
            }

            //ユニットの死亡処理
            foreach(var unit in game.BattleOut.DeadUnits)
            {
                game.GameData.Unit[unit].IsAlive = false;

                //すべての領地からユニットを除外
                foreach(var territory in game.GameData.Territory)
                {
                    territory.RemoveUnit(unit);
                }

                //すべてのグループからユニットを除外
                foreach(var group in game.GameData.Group)
                {
                    group.UnitList.Remove(unit);
                }
            }

            //ユニットの捕獲処理
            var dist = game.BattleOut.CapturedUnits.Distinct().ToList();
            foreach(var unit in dist)
            {
                //敵領地のすべてのグループからユニットを除外
                var territory = game.GameData.Territory[game.BattleIn.EnemyTerritory];
                territory.RemoveUnit(unit);

                //味方領地に追加
                territory = game.GameData.Territory[game.BattleIn.PlayerTerritory];
                territory.AddUnit(unit);
            }

            //ユニットの逃走処理
            foreach(var unit in game.BattleOut.EscapedUnits)
            {
                //回復処理
                var udata = game.GameData.Unit[unit];
                udata.HP = (int)(udata.MaxHP * 0.3f);
                udata.SoldierNum = (int)(udata.MaxSoldierNum * 0.3f);
            }

            //経験値処理
            var ex = 0; //取得経験値
            if (game.BattleIn.IsInvasion)
            {                                       //侵攻戦で
                if (game.BattleOut.IsWin) ex = 3;   //勝った場合:3
                else ex = 2;                        //負けた場合:2
            }
            else
            {                                       //防衛戦で
                if (game.BattleOut.IsWin) ex = 2;   //勝った場合:2
                else ex = 1;                        //負けた場合:1
            }
            foreach(var unitID in game.BattleIn.PlayerUnits)
            {
                if (unitID < 0) continue;
                var unit = game.GameData.Unit[unitID];
                unit.Experience += ex;
            }

            //グループの消滅処理
            foreach(var group in game.GameData.Group)
            {
                //ユニットの残数が0なら消滅
                if (group.UnitList.Count == 0)
                    group.Kill();


                //死守領地がない場合は続行
                if (group.DominationRoute.Count == 0) continue;

                //稼働状態で、死守領地が落ちたら消滅
                if(group.State == GroupDataFormat.GroupState.Active &&
                    game.GameData.Area[group.DominationRoute[0]].Owner == game.BattleIn.PlayerTerritory)
                {
                    group.Kill();
                }
            }

            //カードの処理(自軍のときのみ有効)
            if (game.BattleIn.PlayerTerritory == 0)
            {
                foreach (var cardID in game.BattleOut.UsedCards)
                {
                    var group = game.GameData.Group[game.GameData.Territory[0].GroupList[0]];
                    group.CardList.Remove(cardID);
                }
            }

            //自営軍の復活
            var g = game.GameData.Group[GroupDataFormat.GetDefaultID()];
            g.Rebirth();    //グループの復活
            foreach (var unit in g.UnitList)
            {
                //ユニットの復活
                game.GameData.Unit[unit].Rebirth();
            }

            //アリスが死んでいたらゲームオーバー
            if (game.GameData.Unit[0].IsAlive == false)
            {
                StartCoroutine(game.CallEnding(14));
            }

            //アリスの館が落ちたらゲームオーバー
            if (game.GameData.Area[1].Owner != 0)
            {
                StartCoroutine(game.CallEnding(15));
            }

            //戦闘情報をリセット
            game.BattleIn.Reset();

            //フィールドUIを再ロード
            ShowUI();
            yield return null;
            game.HideNowLoading();
            yield return null;

            //戦闘後スクリプトの開始
            if (game.BattleOut.IsWin)    //戦闘勝利時のスクリプト
            {
                var exescript = game.ScenarioIn.NextA;
                if(exescript>=0)
                    game.CallScript(game.GameData.FieldEvent[exescript]);
                yield return null;
            }
            else                        //戦闘敗北時のスクリプト
            {
                var exescript = game.ScenarioIn.NextB;
                if(exescript>=0)
                    game.CallScript(game.GameData.FieldEvent[exescript]);
                yield return null;

            }
            while (game.IsTalk) yield return null;
            
            //UIの更新
            FieldUIController.AreaPointReset();

            game.BattleIn.Reset();
            game.BattleOut.Reset();
            game.ScenarioIn.Reset();

            //時間を進める
            if (game.GameData.CurrentTime <= 2)
                game.GameData.CurrentTime++;

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
                foreach (int unit in eventlist[i].IfAlive)
                {
                    //ユニットが生きているか
                    if (!game.GameData.Unit[unit].IsAlive)
                    {
                        isEventEnable = false;
                        break;
                    }
                }
                if (!isEventEnable) continue;

                //条件判定
                try
                {
                    bool result = false;
                    for (int j = 0; j < eventlist[i].If_Var.Count; j++)
                    {
                        if (eventlist[i].If_Var[j] != -1)  //条件なしの時If_Val == -1
                        {
                            int src = game.GameData.Memory[eventlist[i].If_Var[j]];
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

                //UIの更新
                mFieldUIController.AreaPointReset();

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
            var areas = game.GameData.Territory[0].AreaList;

            //領地数*定数を収入とする 
            return areas.Count * mIncomesFactor;
            

        }

        //侵攻する領地を侵攻ルートから決定する
        private int GetDominationTarget(int territory, int group)
        {
            var game = Game.GetInstance();
            var route = game.GameData.Group[group].DominationRoute;

            try
            {
                //終点から検索して、一番最初の自領地を見つける
                //その次の領地がプレイヤーの領地ならその領地をターゲットにする
                for (int i = route.Count - 2; i >= 0; i--)
                {
                    var area = game.GameData.Area[route[i]];

                    //終点から数えた一番最初の自領地を見つける
                    if (area.Owner == territory)
                    {
                        var nextArea = game.GameData.Area[route[i + 1]];
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
            var targetpos = game.GameData.Area[targetArea].Position;

            //カメラをその領地に移動
            yield return StartCoroutine(CameraController.MoveTo(targetpos));

            //エフェクトを表示
            yield return StartCoroutine(FieldUIController.ShowHiLightEffect(targetpos));

        }

        //敵情報のセット
        private void SetEnemy(int groupID, bool IsDomination)
        {
            var game = Game.GetInstance();
            var group = game.GameData.Group[groupID];

            //グループのユニットリストから3体取得
            var units = group.GetBattleUnits();

            //グループのカードリストから3つ取得
            var cards = group.GetBattleCards();

            //BattleInにセット
            game.BattleIn.EnemyUnits = units;
            game.BattleIn.EnemyCards = cards;

            //バトルタイプのセット
            game.BattleIn.EnemyBattleType = 
                (IsDomination) ? group.DominationType : group.DefenseType;

        }

        //優先度を基準に防御する敵ユニットを求める
        private int GetDefenseGroup(int area, int territory)
        {
            var game = Game.GetInstance();

            var groupIDs = game.GameData.Territory[territory].GroupList;
            var groups = game.GameData.Group.GetFromIndex(groupIDs);

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
            if (game.SoundManager.GetCueName(SoundType.BGM).Equals(game.GameData.FieldBGM))
                return;

            //再生
            game.SoundManager.Play(game.GameData.FieldBGM, SoundType.BGM);
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
        
        //行動数の更新(敵のみ)
        private void UpdateActionCount()
        {
            var game = Game.GetInstance();

            for (int i = 1; i < game.GameData.Territory.Count; i++)
            {
                var ter = game.GameData.Territory[i];
                ter.ActionCount = 0;
                if (ter.State != TerritoryDataFormat.TerritoryState.Active) continue;
                foreach (var g in ter.GroupList)
                {
                    var gData = game.GameData.Group[g];
                    if (gData.State == GroupDataFormat.GroupState.Active) ter.ActionCount++;
                }
            }
        }
    }
}