using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FieldButton : MonoBehaviour {


    //地点番号
    public int AreaID { get; set; }

    //シーン名
    [SerializeField]
    private string mPlayerMenuPath = "Prefabs/Field/Menu/PlayerMenu";

    [SerializeField]
    private string mEnemyMenuAPath = "Prefabs/Field/Menu/EnemyMenuA";

    [SerializeField]
    private string mEnemuMenuBPath = "Prefabs/Field/Menu/EnemyMenuB";

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
        //地点が隣接しているかどうか判定

        //隣接していたら戦闘ありのメニューを呼ぶ
        ShowMenu(mEnemyMenuAPath);

        //していなかったら戦闘なしのメニューを呼ぶ
        //ShowMenu(mEnemyMenuBPath);
    }

    //メニューを開く
    public void OpenCommonMenu()
    {
        ShowMenu(mCommonMenuPath);
    }

    //メニューを表示する
    private void ShowMenu(string MenuName)
    {
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
