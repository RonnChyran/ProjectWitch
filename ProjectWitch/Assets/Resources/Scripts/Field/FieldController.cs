using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq; //iOSで問題が起こるかも？
using System.Collections.Generic;
using System.Collections;
using System;

using GameData;

namespace Field
{
    public class FieldController : MonoBehaviour
    {
        //内部変数
        //コルーチンが動いているかどうか
        private bool mIsCoroutineExec = false;

        //FieldUIControllerのインスタンス
        private FieldUIController mFieldUIController;
        public FieldUIController FieldUIController
        {
            get
            {
                //FieldUIControllerを取得済みか
                if (!mFieldUIController)
                {
                    //フィールドＵＩコントローラを探す
                    mFieldUIController = GameObject.FindGameObjectWithTag("FieldUIController").GetComponent<FieldUIController>();

                    //見つからなかったら強制停止
                    Debug.Assert(mFieldUIController, "FieldUIControllerが見つかりません");
                }

                return mFieldUIController;
            }

            set { mFieldUIController = value; }
        }

        //CameraControllerのインスタンス
        private CameraController mCameraController;
        public CameraController CameraController
        {
            get
            {
                //CameraControllerを取得済みか
                if (!mCameraController)
                {
                    //フィールドＵＩコントローラを探す
                    mCameraController = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CameraController>();

                    //見つからなかったら強制停止
                    Debug.Assert(mCameraController, "CameraControllerが見つかりません");
                }

                return mCameraController;
            }
            set
            {
                mCameraController = value;
            }
        }

        //メニューが開くかどうか
        public bool MenuClickable { get; set; }
        public bool FlagClickable { get; set; }

        void Start()
        {
            MenuClickable = true;
            FlagClickable = true;
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
                    StartCoroutine(EnemyTurn(territory));
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
                    game.BattleCount = 0;

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

            //コルーチン開始
            mIsCoroutineExec = true;

            //ダイアログが閉じるまで処理を進めない
            while (game.IsDialogShowd) yield return null;

            //フィールドのイベントデータを取得
            var fieldEventData = game.FieldEventData;
            var eventlist = fieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.EnemyTurnBegin).ToList();

            //占領されている場合はスルー
            if (game.AreaData[game.TerritoryData[territory].MainArea].Owner == territory)
            {

                //各領地に設定されたイベントを抜き出す
                eventlist = eventlist.Where(p => p.Area == territory).ToList();

                //ターンはじめイベント開始
                EventExecute(eventlist);
                while (game.IsDialogShowd) yield return null;

                //ターンはじめイベントで戦闘フラグが立った
                //戦闘開始

                //すでに交戦状態に入っている場合
                if (game.TerritoryData[territory].IsActive)
                {
                    //攻め込む領地
                    int targetArea = -1;

                    //隣接している領地から攻め込む領地を決定
                    {
                        //nextAreaを列挙する
                        var nextAreas = new List<int>();
                        foreach (var area in game.TerritoryData[territory].AreaList)
                        {
                            //次の地点のリストを作っていく
                            nextAreas.AddRange(game.AreaData[area].NextArea);
                        }
                        //重複要素を削除する
                        nextAreas = nextAreas.Distinct().ToList();

                        //nextAreasと被っている自領地を抜き出す
                        nextAreas = nextAreas.Intersect(game.TerritoryData[0].AreaList).ToList();

                        //被っている場所があったら、そこをランダムにターゲットとする
                        if (nextAreas.Count > 0)
                        {
                            targetArea = nextAreas[UnityEngine.Random.Range(0, nextAreas.Count - 1)];
                        }
                        //被っていなかったら何もできないので終了
                        else
                        {
                            game.CurrentTime++;
                            mIsCoroutineExec = false;
                            yield break;
                        }
                    }

                    //敵ターンを進めるかどうか
                    if (game.TerritoryData[territory].MinBattleNum > game.BattleCount)
                        game.BattleCount++;
                    else if (game.TerritoryData[territory].MaxBattleNum < game.BattleCount)
                    {
                        game.BattleCount = 0;
                        game.CurrentTime++;
                        mIsCoroutineExec = false;
                        yield break;
                    }
                    else
                    {
                        //次に進むかどうかは乱数で決定
                        if (UnityEngine.Random.value < 0.5f)
                        {
                            game.BattleCount++;
                        }
                        else
                        {
                            game.BattleCount = 0;
                            game.CurrentTime++;
                            mIsCoroutineExec = false;
                            yield break;
                        }
                    }

                    //攻めてきた演出
                    var camera = Camera.main;
                    var targetpos = game.AreaData[targetArea].Position;

                    //カメラをその領地に移動
                    yield return StartCoroutine(CameraController.MoveTo(targetpos));

                    //エフェクトを表示
                    yield return StartCoroutine(FieldUIController.ShowHiLightEffect(targetpos));

                    //戦闘開始
                    yield return StartCoroutine(CallBattle(targetArea, territory, false));

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

            //コルーチンの起動フラグを立てる
            //mIsCoroutineExec = true;

            //先頭の開始
            yield return StartCoroutine(CallBattle(area, territory, true));
            yield return null;

            //コルーチンの終了
            //mIsCoroutineExec = false;

            //時間を進める
            game.CurrentTime++;
            

        }

        //戦闘後処理
        public IEnumerator AfterBattle()
        {
            var game = Game.GetInstance();

            //フィールドUIを再ロード
            yield return SceneManager.LoadSceneAsync(game.SceneName_FieldUI, LoadSceneMode.Additive);

            //シーンの初期化を待つ
            yield return new WaitForEndOfFrame();

            //領地の占領判定
            //オート戦闘の場合、占領判定はスクリプトで行う
            //侵攻戦で勝った場合 領地を占領する
            if (game.BattleIn.IsInvasion && game.BattleOut.IsWin)
                ChangeAreaOwner(game.BattleIn.AreaID, game.BattleIn.PlayerTerritory);
            //防衛戦で負けた場合 領地を奪われる
            else if (!game.BattleIn.IsInvasion && !game.BattleOut.IsWin)
                ChangeAreaOwner(game.BattleIn.AreaID, game.BattleIn.EnemyTerritory);


            //戦闘情報をリセット
            game.BattleIn.Reset();

            //戦闘後スクリプトの開始
            //勝敗で実行されるスクリプトの分岐
            //戦闘後スクリプトの終了
            if(game.BattleOut.IsWin)    //戦闘勝利時のスクリプト
            {
                var exescript = game.ScenarioIn.NextA;
                if(exescript>=0)
                    game.CallScript(game.FieldEventData[exescript]);
                yield return null;
            }
            else                        //戦闘敗北時の
            {
                var exescript = game.ScenarioIn.NextB;
                if(exescript>=0)
                    game.CallScript(game.FieldEventData[exescript]);
                yield return null;

            }
            while (game.IsTalk) yield return null;

            game.BattleIn.Reset();
            game.ScenarioIn.Reset();

            yield return null;
        }

        //戦闘処理
        private IEnumerator CallBattle(int area, int territory, bool invation)
        {
            var game = Game.GetInstance();
            
            //戦闘前スクリプトの開始
            game.ShowDialog("ExecuteScript", "戦闘前イベントの開始");
            while (game.IsDialogShowd) yield return null;

            //戦闘情報の格納
            game.BattleIn.AreaID = area;
            game.BattleIn.EnemyTerritory = territory;
            game.BattleIn.IsInvasion = invation;

            //戦闘呼び出し
            yield return StartCoroutine(game.CallPreBattle());
            FieldUIController = null;
            CameraController = null;
            SceneManager.UnloadScene(game.SceneName_FieldUI);
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
                    //自領地にユニットが含まれているか
                    if (game.TerritoryData[0].UnitList.Contains(unit) == false)
                    {
                        //含まれていなかったらイベントイベント棄却
                        isEventEnable = false;
                        break;
                    }
                }
                if (!isEventEnable) continue;

                //敵の生存判定
                foreach (int unit in eventlist[i].ActorB)
                {
                    isEventEnable = false;
                    for (int j = 1; j < game.TerritoryData.Count; j++)
                    {
                        //任意の領地にユニットが含まれているか
                        if (game.TerritoryData[j].UnitList.Contains(unit))
                        {
                            isEventEnable = true;
                            break;
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
                            int src = int.Parse(game.SystemMemory.Memory[eventlist[i].If_Val[j]]);
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
    }
}