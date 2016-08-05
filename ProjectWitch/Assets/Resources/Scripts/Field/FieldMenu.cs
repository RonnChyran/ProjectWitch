using UnityEngine;
using System.Collections;

public class FieldMenu : MonoBehaviour {

    //地点番号
    public int AreaID { get; set; }

    //field controller
    private FieldController mFieldController;

    void Start()
    {
        //フィールドコントローラを取得
        var obj = GameObject.Find("FieldController");
        mFieldController = obj.GetComponent<FieldController>();
    }


    void Update()
    {
        var game = Game.GetInstance();

        //ダイアログが出ていたら何もしない
        if (game.IsDialogShowd) return;

        if (Input.GetMouseButton(1))
        {
            Close();
        }
    }

    //地点情報を表示
    public void ShowAreaInfo()
    {
        var game = Game.GetInstance();

        //二重起動の防止
        if (game.IsDialogShowd) return;

        //地点情報の読み出し
        string areaInfo = "";
        areaInfo += game.AreaData[AreaID].Name + "\n";
        areaInfo += game.TerritoryData[game.AreaData[AreaID].Owner].OwnerName + "領\n";
        areaInfo += "地点レベル：　" + game.AreaData[AreaID].Level.ToString() + "\n";
        areaInfo += "所持マナ：　" + game.AreaData[AreaID].Mana.ToString() + "\n";

        game.ShowDialog("地点情報", areaInfo);

        //時間を進める
        game.CurrentTime++;

        Close();
    }

    //戦闘開始
    public void CallBattle()
    {
        var game = Game.GetInstance();

        //二重起動の防止
        if (game.IsDialogShowd) return;

        //コルーチンの開始
        StartCoroutine(_CallBattle());
    }

    //マナ集め
    public void ManaGathering()
    {
        var game = Game.GetInstance();

        //二重起動の防止
        if (game.IsDialogShowd) return;

        game.ShowDialog("マナ収集", "xxxxのマナを収集しました");

        //時間を進める
        game.CurrentTime++;

        //メニューを閉じる
        Close();
    }


    public void Close()
    {
        mFieldController.OpeningMenu = false;
        Destroy(this.gameObject);
    }


    //コルーチン
    private IEnumerator _CallBattle()
    {
        var game = Game.GetInstance();

        //戦闘前スクリプトの開始
        game.ShowDialog("ExecuteScript", "戦闘前イベントの開始");
        while (game.IsDialogShowd) yield return null;
        //戦闘前スクリプト終了

        //戦闘情報の格納
        game.BattleIn.AreaID = AreaID;
        if (game.AreaData[AreaID].Owner == 0)   //自領地での戦闘は防衛戦、それ以外は侵攻戦
            game.BattleIn.IsInvasion = false;
        else
            game.BattleIn.IsInvasion = true;

        //戦闘の呼び出し
        game.CallPreBattle();

        yield return null;
    }
}
