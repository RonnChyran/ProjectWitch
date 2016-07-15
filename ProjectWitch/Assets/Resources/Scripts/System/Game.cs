using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using GameData;

public class Game : MonoBehaviour
{

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
        //システム変数の初期化
        Memory = new VirtualMemory();
        //あとセーブデータ読み込みなど
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


    }

}
