using UnityEngine;
using System.Linq; //iOSで問題が起こるかも？
using System.Collections.Generic;
using System.Collections;

using GameData;

class FieldController : MonoBehaviour
{
    //自動操作時のカメラスピード
    [SerializeField]
    private float mCameraSpeed = 1.0f;

    //領地ハイライトのエフェクト
    [SerializeField]
    private GameObject mHiLightEffect;

    [SerializeField]
    private Canvas mCanvas;
    private Canvas mCanvasInst;

    [SerializeField]
    private GameObject mLine;

    //メニューが開いているかどうか
    public bool OpeningMenu{ get; set; }

    //内部変数
    //コルーチンが動いているかどうか
    private bool mIsCoroutineExec = false;

    //ベースへの参照
    private List<GameObject> mBases = new List<GameObject>();

    void Start()
    {
        //リストの初期化
        mBases = Enumerable.Repeat<GameObject>(null, Game.GetInstance().AreaData.Count).ToList();

        //拠点設置
        AreaPointReset();

        //道の描画
        AddRoad();
        
    }

    void Update()
    {
        var game = Game.GetInstance();

        //戦闘後処理
        if(game.IsBattle)
        {
            StartCoroutine(AfterBattle());
        }

        //コルーチンが動いていたら何もしない
        if (mIsCoroutineExec) return;

        //コルーチンを起動
        if(game.CurrentTime <= 2)
        {
            //現在の時間が0~2のとき
            //プレイヤーのターン
            StartCoroutine(PlayerTurn());
        }
        else
        {
            //現在の時間が3~のとき
            //敵のターン
            var territory = game.CurrentTime - 2;   //領地ＩＤを求める
            
            //領地IDが存在する範囲なら領地イベントを実行
            if (territory < game.TerritoryData.Count)
                StartCoroutine(EnemyTurn(territory));
            else
            {
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
                    if(nextAreas.Count > 0)
                    {
                        targetArea = nextAreas[Random.Range(0,nextAreas.Count-1)];
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
                    if (Random.value < 0.5f)
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


                //戦闘情報セット
                game.BattleIn.AreaID = targetArea;
                game.BattleIn.EnemyTerritory = territory;
                game.BattleIn.IsInvasion = false;

                //攻めてきた演出
                var camera = Camera.main;
                var targetpos = game.AreaData[targetArea].Position;

                //カメラをその領地に移動
                var dir = targetpos - (Vector2)camera.transform.position;
                var moveVec = dir.normalized;
                while (dir.magnitude > 0.1f)
                {
                    camera.transform.position += new Vector3(moveVec.x, moveVec.y, 0.0f)/100.0f * mCameraSpeed;
                    dir = targetpos - (Vector2)camera.transform.position;
                    moveVec = dir.normalized;
                    yield return new WaitForSeconds(0.01f);
                }

                var inst = Instantiate(mHiLightEffect);
                inst.transform.position = targetpos;
                yield return new WaitForSeconds(1.0f);

                //敵行動
                game.CallPreBattle();

                //実行されないはず
                mIsCoroutineExec = false;

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

    //戦闘後処理
    private IEnumerator AfterBattle()
    {
        var game = Game.GetInstance();
        game.IsBattle = false;  //戦闘が終わったので戦闘フラグをオフ

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

        yield return null;
    }

    //戦闘の開始
    private IEnumerator CallBattle(int area, int territory)
    {
        var game = Game.GetInstance();

        //戦闘前スクリプトの開始
        game.ShowDialog("ExecuteScript", "戦闘前イベントの開始");
        while (game.IsDialogShowd) yield return null;

        //戦闘情報の格納
        game.BattleIn.AreaID = area;
        game.BattleIn.EnemyTerritory = territory;
        game.BattleIn.IsInvasion = false;

        //戦闘の呼び出し
        game.CallPreBattle();

        yield return null;
    }

    //イベント制御
    IEnumerator EventExecute(List<EventDataFormat> eventlist)
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
            if (eventlist[i].If_Val != -1)  //条件なしの時If_Val == -1
            {
                int src = (int)game.SystemMemory.Memory[eventlist[i].If_Val];
                var imm = eventlist[i].If_Imm;

                //演算結果用
                bool result = false;
                switch (eventlist[i].If_Ope)
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
                if (!result) continue;
            }

            //実行
            game.CallScript(eventlist[i].FileName);
            while (game.IsDialogShowd) yield return null;
            
        }
    }

    //拠点の設置
    private void AreaPointReset()
    {
        var game = Game.GetInstance();

        mCanvasInst = Instantiate(mCanvas);
        for (int i = 1; i < game.AreaData.Count; i++)
        {
            AddAreaPoint(i,game.AreaData[i].Owner);
        }
    }

    private void AddAreaPoint(int area, int owner)
    {
        var game = Game.GetInstance();

        var Base = Instantiate(game.TerritoryData[owner].FlagPrefab);
        Base.transform.SetParent(mCanvasInst.transform);
        Base.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(game.AreaData[area].Position.x, game.AreaData[area].Position.y, 1.0f);
        Base.GetComponent<FieldButton>().AreaID = area;

        mBases[area] = Base;
    }

    //拠点の変更
    private void ChangeAreaOwner(int targetArea, int newOwner)
    {
        var game = Game.GetInstance();

        //拠点を再配置
        Destroy(mBases[targetArea]);
        AddAreaPoint(targetArea,newOwner);

        game.ChangeAreaOwner(targetArea, newOwner);
    }

    //道の描画
    private void AddRoad()
    {
        var Openlist = new List<AreaDataFormat>();
        var Closelist = new List<AreaDataFormat>();
        int current=1;
        int child=0;

        var game = Game.GetInstance();

        Openlist.Add(game.AreaData[1]);

        while (Openlist.Count != 0)
        {
            //オープンリストとクローズリストにない隣接地点の検出
            for (int i = 0; i < game.AreaData[current].NextArea.Count; i++)
            {
                child = game.AreaData[current].NextArea[i];

                bool a = Openlist.Contains(game.AreaData[child]);
                bool b = Closelist.Contains(game.AreaData[child]);
                if (a == false && b == false)
                    break;
                else
                {
                    child = 0;
                }
            }


            //隣接する地点がない時
            if (child == 0)
            {
                var target = game.AreaData[current].NextArea;
                //オープンリスト内の親以外の点で隣り合っている点をすべて返す（配列）
                foreach (int a in target)
                {
                    if (a != 0)//aが０じゃないなら線を引く
                        DrawLine(new Vector3(game.AreaData[current].Position.x, game.AreaData[current].Position.y, 2.0f), 
                            new Vector3(game.AreaData[a].Position.x, game.AreaData[a].Position.y, 2.0f));
                }
                Closelist.Add(game.AreaData[current]);
                Openlist.Remove(game.AreaData[current]);

                if (Openlist.Count == 0)
                    break;

                current = Openlist[0].ID;

            }
            else
            {  //子供がいれば
                DrawLine(new Vector3(game.AreaData[current].Position.x, game.AreaData[current].Position.y, 2.0f),
                    new Vector3(game.AreaData[child].Position.x, game.AreaData[child].Position.y, 2.0f));
                bool c = Openlist.Contains(game.AreaData[current]);

                if (c == false)
                    Openlist.Add(game.AreaData[current]);

                current = child;
            }
        }
    }

    //線の描画
    private void DrawLine(Vector3 pointA, Vector3 pointB)
    {
        if (!mLine) return;

        var inst = Instantiate(mLine);
        inst.transform.SetParent(mCanvasInst.transform);
        inst.GetComponent<LineRenderer>().SetPositions(new Vector3[] { pointA, pointB });
    }
}