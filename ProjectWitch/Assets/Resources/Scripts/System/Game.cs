using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using GameData;

public class Game : MonoBehaviour
{
    #region 定数

    //シーン名
    private const string cSceneName_Battle      = "Battle";
    private const string cSceneName_PreBattle   = "PreBattle";
    private const string cSceneName_Field       = "Field";
    private const string cSceneName_Save        = "Save";
    private const string cSceneName_Load        = "Load";

    //読み取り専用プロパティ
    public string SceneName_Battle      { get { return cSceneName_Battle; } private set { } }
    public string SceneName_PreBattle   { get { return cSceneName_PreBattle; } private set { } }
    public string SceneName_Field       { get { return cSceneName_Field; } private set { } }
    public string SceneName_Save        { get { return cSceneName_Save; } private set { } }
    public string SceneName_Load        { get { return cSceneName_Load; } private set { } }


    #endregion

    #region ゲームデータ関連

    //プレイヤーのデータ
    //ユニットデータ
    public List<UnitDataFormat> UnitData { get; set; }
    //スキルデータ
    public List<SkillDataFormat> SkillData { get; set; }
    //所持マナ
    public int PlayerMana { get; set; }

    //環境のデータ
    //現在のターン数
    public int CurrentTurn { get; set; }
    //現在の時間数 0:朝 1:昼 2:夜 3~:敵ターン
    public int CurrentTime { get; set; }
    //土地データ
    public List<AreaDataFormat> AreaData { get; set; }
    //領地データ
    public List<TerritoryDataFormat> TerritoryData { get; set; }

    //その他データ
    //システム変数
    public VirtualMemory SystemMemory { get; set; }
    //コンフィグ
    public ConfigDataFormat Config { get; set; }
    //AIデータ
    public List<AIDataFormat> AIData { get; set; }
    //装備データ
    public List<EquipmentDataFormat> EquipmentData { get; set; }
    //カードデータ
    public List<CardDataFormat> CardData { get; set; }
    //イベントデータ
    public List<EventDataFormat> FieldEventData { get; set; }
    public List<EventDataFormat> TownEventData { get; set; }
    public List<EventDataFormat> ArmyEventData { get; set; }

    #endregion

    #region シーン間データ関連

    public BattleDataIn BattleIn { get; set; }
    public BattleDataOut BattleOut { get; set; }

    #endregion

    #region 制御変数

    public bool IsDialogShowd { get; set; }
    public bool IsBattle { get; set; }
    public bool IsScript { get; set; }

    #endregion

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
        UnitData = new List<UnitDataFormat>();
        SkillData = new List<SkillDataFormat>();
        PlayerMana = 10000;
        CurrentTime = 0; //朝から
        CurrentTurn = 1;
        AreaData = new List<AreaDataFormat>();
        TerritoryData = new List<TerritoryDataFormat>();
        SystemMemory = new VirtualMemory();
        SystemMemory.Memory[0] = 0;
        Config = new ConfigDataFormat();
        AIData = new List<AIDataFormat>();
        EquipmentData = new List<EquipmentDataFormat>();
        CardData = new List<CardDataFormat>();
        FieldEventData = new List<EventDataFormat>();
        TownEventData = new List<EventDataFormat>();
        ArmyEventData = new List<EventDataFormat>();

        //シーン間データの初期化
        BattleIn = new BattleDataIn();
        BattleOut = new BattleDataOut();

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

    //フィールドの開始
    public void CallField()
    {
        SceneManager.LoadScene(cSceneName_Field);
    }

    //戦闘の開始
    public void CallPreBattle()
    {
        CallBattle();
    }

    public void CallBattle()
    {
        SceneManager.LoadScene(cSceneName_Battle);
    }

    //スクリプトの開始
    public void CallScript(string filePath)
    {
        ShowDialog("ExecuteScript", filePath + "を実行します");
    }

    //セーブ画面を呼び出す
    public void CallSave()
    {
        SceneManager.LoadScene(cSceneName_Save);
    }

    //ロード画面を呼び出す
    public void CallLoad()
    {
        SceneManager.LoadScene(cSceneName_Load);
    }

    //オートセーブする
    public void AutoSave()
    {
        ShowDialog("オートセーブ", CurrentTurn.ToString() + "ターン目\nオートセーブしました。");
        Save(0); //0スロットはオートセーブ用スロット
    }

    //現在の状態をセーブする
    private void Save(int slot)
    {
        ShowDialog("save", "セーブ機能は実装されていません");
    }

    //スロット番号のセーブデータからデータを読み込む
    private void Load(int slot)
    {
        ShowDialog("load", "ロード機能は実装されていません");
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
