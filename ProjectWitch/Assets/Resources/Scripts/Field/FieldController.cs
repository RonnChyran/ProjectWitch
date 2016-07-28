using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Linq; //iOSで問題が起こるかも？
using System.Collections.Generic;
using System.Collections;

using GameData;

class FieldController : MonoBehaviour
{
    [SerializeField]
    private Canvas mCanvas;
    private Canvas mCanvasInst;

    [SerializeField]
    private GameObject mLine;

    //メニューが開いているかどうか
    public bool OpeningMenu{ get; set; }

    void Start()
    {
        //拠点設置
        AddAreaPoint();

        //道の描画
        AddRoad();
        
    }

    void Update()
    {
        if(Game.GetInstance().CurrentTime < 0)
            StartCoroutine("_Update");
    }

    //メインの動作部分（コルーチン
    private IEnumerator _Update()
    {
        var game = Game.GetInstance();

        //フィールドのイベントデータを取得
        var fieldEventData = game.FieldEventData;
        var eventlist = fieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.PlayerTurnBegin).ToList();

        for (game.CurrentTime = 3; game.CurrentTime > 0; game.CurrentTime--)
        {
            //ターンはじめイベントを実行
            yield return EventExecute(eventlist);

            //操作可能
            game.ShowDialog("fase:" + (4-game.CurrentTime).ToString(), "プレイヤー操作開始");
            while (game.IsDialogShowd) yield return null;
            while (!Input.GetKeyDown(KeyCode.Space)) yield return null;

            //ユーザー行動
            //敵陣地選択
            //戦闘前イベント
            //戦闘
            //分岐：戦闘敗北、戦闘勝利
            //戦闘勝利　勝利後イベント
            //戦闘敗北　敗北後イベント
            //ターンはじめイベント・・・・・・繰り返し
        }
        //ターン終了

        //敵ターン開始
        //敵ターンはじめイベント
        eventlist = fieldEventData.Where(p => p.Timing == EventDataFormat.TimingType.EnemyTurnBegin).ToList();
        for (int i = 1; i < game.TerritoryData.Count; i++)
        {
            //領地がない場合はスルー
            if (game.TerritoryData[i].AreaList.Count == 0) continue;

            game.ShowDialog("敵ターン", game.TerritoryData[i].OwnerName + "領" + '\n' + "ターンはじめイベント開始");
            while (game.IsDialogShowd) yield return null;



            game.ShowDialog("敵ターン", "ターンはじめイベント終了");
            while (game.IsDialogShowd) yield return null;

            //敵行動
            game.ShowDialog("敵ターン", "敵行動");
            while (game.IsDialogShowd) yield return null;
        }
        //戦闘
        //戦闘前イベント
        //戦闘
        //分岐：戦闘敗北、戦闘勝利
        //次の領地……繰り返し

        game.CurrentTime = -1;

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

            //生存判定
            foreach (int unit in eventlist[i].ActorA)
            {
                //味方の生存判定
                if (game.TerritoryData[0].UnitList.Contains(unit) == false)
                {
                    isEventEnable = false;
                    break;
                }
            }

            foreach (int unit in eventlist[i].ActorB)
            {
                //敵の生存判定
                isEventEnable = false;
                for (int j = 1; j < game.TerritoryData.Count; j++)
                {
                    if (game.TerritoryData[j].UnitList.Contains(unit))
                    {
                        isEventEnable = true;
                        break;
                    }
                }

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
            game.ExecuteScript(eventlist[i].FileName);
            while (game.IsDialogShowd) yield return null;

            //次のスクリプト実行
        }
    }

    //拠点の設置
    void AddAreaPoint()
    {
        var game = Game.GetInstance();

        mCanvasInst = Instantiate(mCanvas);
        for (int i = 1; i < game.AreaData.Count; i++)
        {
            var Base = Instantiate(game.TerritoryData[game.AreaData[i].Owner].FlagPrefab);
            Base.transform.SetParent(mCanvasInst.transform);
            Base.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(game.AreaData[i].Position.x, game.AreaData[i].Position.y, 1.0f);
        }
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