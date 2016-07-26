using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using GameData;

public class Game : MonoBehaviour
{
    //----------------------------------
    //データ関連
    //----------------------------------
    //プレイヤーのデータ
    //ユニットデータ
    public List<UnitDataFormat> UnitData { get; set; }
    //スキルデータ
    public List<SkillDataFormat> SkillData { get; set; }
    //所持マナ
    public int PlayerMana { get; set; }

    //環境のデータ
    //現在のターン数
    public int CurrentTrun { get; set; }
    //現在の時間数
    public int CurrentTime { get; set; }
    //土地データ
    public List<AreaDataFormat> AreaData { get; set; }
    //領地データ
    public List<TerritoryDataFormat> TerritoryData { get; set; }

    //その他データ
    //システム変数
    public VirtualMemory Memory { get; set; }
    //コンフィグ
    public ConfigDataFormat Config { get; set; }
    //AIデータ
    public List<AIDataFormat> AIData { get; set; }
    //装備データ
    public List<EquipmentDataFormat> EquipmentData { get; set; }
    //カードデータ
    public List<SkillDataFormat> CardData { get; set; }
    //イベントデータ
    public List<EventDataFormat> FieldEventData { get; set; }
    public List<EventDataFormat> TownEventData { get; set; }
    public List<EventDataFormat> ArmyEventData { get; set; }

    //---------------------
    //内部変数
    //---------------------
    public bool IsDialogShowd { get; set; }

    //Singleton
    private static Game mInst;
    public static Game GetInstance()
    {
        if (mInst == null)
        {
            GameObject gameObject = new GameObject("Game");
            mInst = gameObject.AddComponent<Game>();
        }
        return mInst;
    }
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (mInst == null)
        {
            this.Setup();
            mInst = this;
        }
        else if (mInst != this)
        {
            Destroy(this.gameObject);
        }
    }

    //初期化処理
    void Setup()
    {
        //ダイアログ非表示
        IsDialogShowd = false;

        //データ系の初期化
        CurrentTime = -1; //朝から

        //システム変数の初期化
        Memory = new VirtualMemory();
        Memory.SetValue(0, 0);
        //あとセーブデータ読み込みなど

#if DEBUG
        FirstLoad();
#endif
    }

    //ダイアログを表示
    public void ShowDialog(string caption, string message)
    {
        if (IsDialogShowd) return;

        GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/dialog");
        if (!prefab)
            Debug.Log("ダイアログのプレハブが見つかりません");

        //インスタンス化
        var inst = Instantiate(prefab);
        inst.GetComponent<DialogWindow>().Caption = caption;
        inst.GetComponent<DialogWindow>().Text = message;

        //ダイアログ表示
        IsDialogShowd = true;

    }

    //スクリプトの開始
    public void ExecuteScript(string filePath)
    {
        ShowDialog("ExecuteScript", filePath + "を実行します");
    }

    //現在の状態をセーブする
    public void Save(int slot)
    {

    }

    //スロット番号のセーブデータからデータを読み込む
    public void Load(int slot)
    {

    }

    //タイトルで初めからを選択したときの初回ロード（既存データの読み出し
    public void FirstLoad()
    {
        //ユニットデータの読み出し
        UnitData = DataLoader.LoadUnitData("Assets\\Resources\\Data\\unit_data.csv");

        //スキルデータの読み出し
        
        //地点データの読み出し
        AreaData = DataLoader.LoadAreaData("Assets\\Resources\\Data\\area_data.csv");

        //領地データの読み出し
        TerritoryData = DataLoader.LoadTerritoryData("Assets\\Resources\\Data\\territory_data.csv");

        //所持地点リスト（地点リストから算出
        foreach (TerritoryDataFormat terData in TerritoryData)
        {
            terData.AreaList = new List<int>();
            for (int j = 1; j < AreaData.Count; j++)
            {
                if (AreaData[j].Owner == terData.AreaList.Count)
                {
                    terData.AreaList.Add(AreaData[j].ID);
                }
            }
        }

        //イベントデータの読み出し
        FieldEventData = DataLoader.LoadEventData("Assets\\Resources\\Data\\event_data_field.csv");
    }

}
