using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class FieldButton : MonoBehaviour {


    //地点番号
    public int AreaID { get; set; }

    //シーン名
    [SerializeField]
    private string mPlayerMenuPath = "Prefabs/Field/Menu/PlayerMenu";

    [SerializeField]
    private string mEnemyMenuAPath = "Prefabs/Field/Menu/EnemyMenuA";

    [SerializeField]
    private string mEnemyMenuBPath = "Prefabs/Field/Menu/EnemyMenuB";

    [SerializeField]
    private string mCommonMenuPath = "Prefabs/Field/Menu/CommonMenu";

    //field controller
    private FieldController mFieldController;

    void Start()
    {
        //フィールドコントローラを取得
        var obj = GameObject.Find("FieldController");
        mFieldController = obj.GetComponent<FieldController>();
    }
    

    //プレイヤーの領地メニューを開く
    public void OpenPlayerMenu()
    {
        ShowMenu(mPlayerMenuPath);
    }

    //敵の領地メニューを開く
    public void OpenEnemyMenu()
    {
        var game = Game.GetInstance();

        //地点が隣接しているかどうか判定
        var nextAreas = new List<int>();
        {
            //隣接地点の取得
            foreach (var area in game.TerritoryData[0].AreaList)
            {
                nextAreas.AddRange(game.AreaData[area].NextArea);
            }
            //重複を削除
            nextAreas = nextAreas.Distinct().ToList();
        }

        if (nextAreas.Contains(AreaID))
            //隣接していたら戦闘ありのメニューを呼ぶ
            ShowMenu(mEnemyMenuAPath);
        else
            //していなかったら戦闘なしのメニューを呼ぶ
            ShowMenu(mEnemyMenuBPath);
    }

    //メニューを開く
    public void OpenCommonMenu()
    {
        ShowMenu(mCommonMenuPath);
    }

    //メニューを表示する
    private void ShowMenu(string MenuName)
    {
        //敵ターンのときは無効
        if (Game.GetInstance().CurrentTime >= 3) return;
        //メニュー開いていたら何もしない
        if (mFieldController.OpeningMenu) return;
        //ダイアログが開いていたら何もしない
        if (Game.GetInstance().IsDialogShowd) return;

        var obj = (GameObject)Resources.Load(MenuName);
        var inst = Instantiate(obj);
        inst.GetComponent<FieldMenu>().AreaID = AreaID;

        //メニューが開いている状態にする
        mFieldController.OpeningMenu = true;
    }
}
