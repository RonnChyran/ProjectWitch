using UnityEngine;
using System.Collections;

public class Ending : MonoBehaviour {
 
    // Use this for initialization
    void Start()
    {
        StartCoroutine(CoEnding());

    }


    //イベント制御
    private IEnumerator CoEnding()
    {
        var game = Game.GetInstance();

        //実行するエンディングIDの取得
        var id = game.EndingID;

        //イベントデータの設定
        var ev = new GameData.EventDataFormat();
        ev.FileName = "s9991";
        ev.NextA = -1;
        ev.NextB = -1;

        //スクリプト実行
        game.CallScript(ev);
        yield return null;
        while (game.IsTalk) yield return null;

        //スクリプトを実行したら終了
        //タイトルの呼び出し
        StartCoroutine(game.CallTitle());

        yield break;
    }

}
