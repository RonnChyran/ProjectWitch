using UnityEngine;
using System.Collections;

public class Opening : MonoBehaviour {

    //最初に実行するスクリプト
    [SerializeField]
    private string mScript_First = "s0000";

    //戦闘勝利後に実行するスクリプト
    [SerializeField]
    private int mNextScriptA = 1;

    //戦闘敗北後に実行するスクリプト
    [SerializeField]
    private int mNextScriptB = 1;

	// Use this for initialization
	void Start () {
        StartCoroutine(CoOpening());

	}



    //イベント制御
    private IEnumerator CoOpening()
    {
        var game = Game.GetInstance();

        //イベントデータの設定
        var ev = new GameData.EventDataFormat();
        ev.FileName = mScript_First;
        ev.NextA = mNextScriptA;
        ev.NextB = mNextScriptB;

        //スクリプト実行
        game.CallScript(ev);
        yield return null;
        while (game.IsTalk) yield return null;

        //戦闘オンだったら戦闘開始
        if (game.BattleIn.IsEvent)
            yield return StartCoroutine(CallBattle(game.BattleIn.AreaID, 0, true));


        //スクリプトを実行したら終了
        //フィールドの呼び出し
        yield return StartCoroutine(game.CallField());

        yield break;
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
        yield return StartCoroutine(game.CallPreBattle());
        yield return null;

        //戦闘終了まで待機
        while (game.IsBattle) yield return null;

        //戦闘終了処理
        yield return StartCoroutine(AfterBattle());
    }

    //戦闘後処理
    public IEnumerator AfterBattle()
    {
        var game = Game.GetInstance();

        //戦闘情報をリセット
        game.BattleIn.Reset();

        //戦闘後スクリプトの開始
        //勝敗で実行されるスクリプトの分岐
        //戦闘後スクリプトの終了
        if (game.BattleOut.IsWin)    //戦闘勝利時のスクリプト
        {
            var exescript = game.ScenarioIn.NextA;
            if (exescript >= 0)
                game.CallScript(game.FieldEventData[exescript]);
            yield return null;
        }
        else                        //戦闘敗北時の
        {
            var exescript = game.ScenarioIn.NextB;
            if (exescript >= 0)
                game.CallScript(game.FieldEventData[exescript]);
            yield return null;

        }
        while (game.IsTalk) yield return null;

        game.BattleIn.Reset();
        game.ScenarioIn.Reset();

        yield return null;
    }
}
