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
    private const string cSceneName_FieldUI     = "FieldUI";
    private const string cSceneName_Save        = "Save";
    private const string cSceneName_Load        = "Load";
    private const string cSceneName_Talk        = "Talk";

    //読み取り専用プロパティ
    public string SceneName_Battle      { get { return cSceneName_Battle; } private set { } }
    public string SceneName_PreBattle   { get { return cSceneName_PreBattle; } private set { } }
    public string SceneName_Field       { get { return cSceneName_Field; } private set { } }
    public string SceneName_FieldUI     { get { return cSceneName_FieldUI; }  private set { } }
    public string SceneName_Save        { get { return cSceneName_Save; } private set { } }
    public string SceneName_Load        { get { return cSceneName_Load; } private set { } }
    public string SceneName_Talk        { get { return cSceneName_Talk; } private set { } }

    //毎ターンのマナ回復率
    private float mManaRecoveryRate = 0.1f; //10%
    //毎ターンのHP回復率
    private float mHPRecoveryRate = 0.2f; //20%

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

    public ScenarioDataIn ScenarioIn { get; set; }

    #endregion

    #region 制御変数

    //ダイアログが開いているか
    public bool IsDialogShowd { get; set; }

    //戦闘中かどうか
    public bool IsBattle { get; set; }

    //スクリプト実行中かどうか
    public bool IsTalk { get; set; }

    //連戦数
    public int BattleCount { get; set; }

    //編成画面から戦闘に入るかどうか
    public bool UsePreBattle { get; set; }

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
        //制御変数初期化
        IsDialogShowd = false;
        IsBattle = false;
        IsTalk = false;
        UsePreBattle = true;
        BattleCount = 0;

        //データ系の初期化
        UnitData = new List<UnitDataFormat>();
        SkillData = new List<SkillDataFormat>();
        PlayerMana = 10000;
        CurrentTime = 0; //朝から
        CurrentTurn = 1;
        AreaData = new List<AreaDataFormat>();
        TerritoryData = new List<TerritoryDataFormat>();
        SystemMemory = new VirtualMemory();
        SystemMemory.Memory[0] = "0";
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
        ScenarioIn = new ScenarioDataIn();

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
        SceneManager.LoadScene(cSceneName_FieldUI, LoadSceneMode.Additive);
    }
    public Scene CallFieldUI()
    {
        SceneManager.LoadScene(cSceneName_FieldUI, LoadSceneMode.Additive);
        return SceneManager.GetSceneByName(cSceneName_FieldUI);
    }

    //戦闘の開始
    public IEnumerator CallPreBattle()
    {
        if (UsePreBattle)
        {
            yield return SceneManager.LoadSceneAsync(cSceneName_PreBattle,LoadSceneMode.Additive);
        }
        else
        {
            //戦闘準備画面を出さず直接戦闘
            UsePreBattle = true;
            yield return StartCoroutine(CallBattle());
        }
    }

    public IEnumerator CallBattle()
    {
        //戦闘情報の格納
        var time = (CurrentTime <= 2) ? CurrentTime : 2;
        BattleIn.TimeOfDay = time;
        
        SceneManager.UnloadScene(cSceneName_PreBattle);
        yield return SceneManager.LoadSceneAsync(cSceneName_Battle,LoadSceneMode.Additive);
        yield return null;
    }

    //スクリプトの開始
    public void CallScript(EventDataFormat e)
    {
        ScenarioIn.FileName = e.FileName;

        ScenarioIn.NextA = e.NextA;
        ScenarioIn.NextB = e.NextB;
        SceneManager.LoadScene(cSceneName_Talk, LoadSceneMode.Additive);
    }

    //セーブ画面を呼び出す
    public void CallSave()
    {
        SceneManager.LoadScene(cSceneName_Save);
    }

    //ロード画面を呼び出す
    public void CallLoad()
    {
        ShowDialog("じっそうしてないよ！", "");
        //SceneManager.LoadScene(cSceneName_Load);
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
        UnitData = DataLoader.LoadUnitData(GamePath.Data + "unit_data");

        //スキルデータの読み出し
        SkillData = DataLoader.LoadSkillData(GamePath.Data + "skill_data");

        //地点データの読み出し
        AreaData = DataLoader.LoadAreaData(GamePath.Data + "area_data");

        //領地データの読み出し
        TerritoryData = DataLoader.LoadTerritoryData(GamePath.Data + "territory_data");

        //所持地点リスト（地点リストから算出
        for (int i = 0; i < TerritoryData.Count; i++)
        {
            var terData = TerritoryData[i];
            terData.AreaList = new List<int>();
            for (int j = 1; j < AreaData.Count; j++)
            {
                if (AreaData[j].Owner == i)
                {
                    terData.AreaList.Add(AreaData[j].ID);
                }
            }
        }

        //AI
        AIData = DataLoader.LoadAIData(GamePath.Data + "ai_data");

        //装備
        EquipmentData = DataLoader.LoadEquipmentData(GamePath.Data + "equipment_data");

        //カード
        CardData = DataLoader.LoadCardData(GamePath.Data + "card_data");

        //イベントデータの読み出し
        ArmyEventData =     DataLoader.LoadEventData(GamePath.Data + "event_data_army");
        FieldEventData =    DataLoader.LoadEventData(GamePath.Data + "event_data_field");
        TownEventData =     DataLoader.LoadEventData(GamePath.Data + "event_data_town");

    }


    //各コマンド

    //地点の領主を変更
    //targetArea:変更する地点ＩＤ
    //newOwner:新しい領主ID
    public void ChangeAreaOwner(int targetArea, int newOwner)
    {
        //領地データのエリア番号を移し替える
        var oldOwner = AreaData[targetArea].Owner;
        TerritoryData[oldOwner].AreaList.Remove(targetArea);
        TerritoryData[newOwner].AreaList.Add(targetArea);

        //地点の領主番号を更新
        AreaData[targetArea].Owner = newOwner;

    }

    //エリアのマナを回復量だけ回復
    public void RecoverMana()
    {
        foreach(var area in AreaData)
        {
            //最大マナの10%回復
            area.Mana += (int)((float)area.MaxMana * mManaRecoveryRate);
            if (area.Mana > area.MaxMana) area.Mana = area.MaxMana;
        }
    }

    //ユニットを回復量だけ回復
    public void RecoverUnit()
    {
        foreach(var unit in UnitData)
        {
            //死んでいたらスルー
            if (!unit.IsAlive) continue;

            //ユニットの回復量分兵士数を回復
            //HPは固定比率回復

            //HP回復
            unit.HP += (int)((float)unit.MaxHP * mHPRecoveryRate);
            if (unit.HP > unit.MaxHP) unit.HP = unit.MaxHP;

            //兵数回復
            unit.SoldierNum += unit.Curative;
            if (unit.SoldierNum > unit.MaxSoldierNum) unit.SoldierNum = unit.MaxSoldierNum;

        }
    }
}
