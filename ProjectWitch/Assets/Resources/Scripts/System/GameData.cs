using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq; //iOSで動かないかも

using UnityEngine;

namespace GameData
{
    //各種データ構造

    #region ゲームデータ系

    //ユニットデータ
    public class UnitDataFormat
    {
        //コンストラクタ
        public UnitDataFormat()
        {
            Love = 0;
        }

        //ユニット名
        public string Name { get; set; }

        //レベル
        public int Level { get; set; }

        //レベル成長限界
        public int MaxLevel { get; set; }

        //HP
        public int HP { get; set; }
        //最大HP
        public int MaxHP { get; set; }
        public int HPRate { get; set; }     //HP成長率

        //経験値
        public int Experience { get; set; }

        //リーダー(Leader)
        public int LeaderPAtk { get; set; } //物理攻撃
        public int LeaderMAtk { get; set; } //魔法攻撃
        public int LeaderPDef { get; set; } //物理防御
        public int LeaderMDef { get; set; } //魔法防御
                                            //集団(Group)
        public int GroupPAtk { get; set; }      //物理攻撃
        public int GroupMAtk { get; set; }  //魔法攻撃
        public int GroupPDef { get; set; }      //物理防御
        public int GroupMDef { get; set; }  //魔法防御
                                            //成長率（リーダー）
        public int LeaderPAtkRate { get; set; } //物理攻撃成長率
        public int LeaderMAtkRate { get; set; } //魔法攻撃成長率
        public int LeaderPDefRate { get; set; } //物理防御成長率
        public int LeaderMDefRate { get; set; } //魔法防御成長率
                                                //成長率（集団）
        public int GroupPAtkRate { get; set; }      //物理攻撃成長率
        public int GroupMAtkRate { get; set; }  //魔法攻撃成長率
        public int GroupPDefRate { get; set; }      //物理防御成長率
        public int GroupMDefRate { get; set; }  //魔法防御成長率

        //指揮力
        public int Leadership { get; set; }
        public int LeadershipRate { get; set; } //指揮力成長率
        //機動力
        public int Agility { get; set; }
        public int AgilityRate { get; set; }    //機動力成長率
        //回復力
        public int Curative { get; set; }
        public int CurativeRate { get; set; }   //回復力成長率
        //兵士数
        public int SoldierNum { get; set; }
        //最大兵士数
        public int MaxSoldierNum { get; set; }

        //撤退するか死ぬか ture:死ぬ　false：撤退する（捕獲コマンドが無効
        public bool Deathable { get; set; }

        //好感度
        public int Love { get; set; }

        //リーダースキル
        public int LAtkSkill { get; set; }
        public int LDefSkill { get; set; }

        //部下スキル
        public int GAtkSkill { get; set; }

        //部下の大きさ（小:0 中:1 大:2 特大:3
        public int GUnitSize { get; set; }

        //装備ID
        public int Equipment { get; set; }

        //AI番号
        public int AIID { get; set; }

        //立ち絵画像名
        public string StandImagePath { get; set; }
        //顔アイコン画像名
        public string FaceIamgePath { get; set; }
        //戦闘リーダープレハブ名
        public string BattleLeaderPrefabPath { get; set; }
        //戦闘兵士プレハブ名
        public string BattleGroupPrefabPath { get; set; }

    }

    //スキルデータ
    public class SkillDataFormat
    {
        public SkillDataFormat()
        {
            Status = Enumerable.Repeat<bool>(false, 7).ToList();
            Attribute = Enumerable.Repeat<bool>(false, 3).ToList();
        }

        //名前
        public string Name { get; set; }

        //威力
        public int Power { get; set; }

        //種類
        public enum SkillType
        {
            Damage=0,       //0:ダメ―ジ
            Heal,           //1:回復
            StatusUp,       //2:ステ上昇
            StatusDown,     //3:ステ下降
            Summon,         //4:召喚
            SoulSteal,      //5:ダメージ還元
            Guard,          //6:ガード
            TurnWait,       //7:順番下げ
            NoDamage,       //8:ダメージ無効
            PutTime,        //9:時間消費
            StatusOff,      //10:ステータス取り消し
            Random          //11:ランダム
        };
        public SkillType Type { get; set; }

        //効果持続時間
        public int Duration { get; set; }

        //ステ種類
        //[0]物功,[1]物防,[2]魔攻,[3]魔防,[4]機動,[5]指揮力,[6]地形補正
        public List<bool> Status { get; set; }

        //攻撃属性
        //[0]毒付与、[1]対ホムンクルス, [2]対ゾンビ
        public List<bool> Attribute { get; set; }

        //召喚用ユニット番号
        public int SummonUnit { get; set; }

        //範囲
        public enum SkillRange
        {
            All=0,      //0:全員
            Single      //1:単体
        }
        public SkillRange Range { get; set; }

        //対象
        public enum SkillTarget
        {
            Enemy=0,        //0:敵
            Player,         //1:味方
            Own,            //2:自分
            EnemyLeader,    //3:敵リーダー
            PlayerLeader,   //4:味方リーダー
            EnemyAndPlayer, //5:敵味方両方
        }
        //[0]敵集団、[1]味方集団、[2]自分、[3]敵リーダー、[4]味方リーダー
        public SkillTarget Target { get; set; }

        //視覚エフェクト
        public string EffectPath { get; set; }
    }

    //カードデータ
    public class CardDataFormat
    {
        //名前
        public string Name { get; set; }

        //タイミング
        public enum CardTiming
        {
            BattleBegin=0,      //戦闘開始
            BattleEnd,          //戦闘終了
            UserState_S10,      //カード使用側のどれかのユニットの兵士数10%以下
            UserState_S50,      //兵士数50%以下
            UserState_HP10,     //HP10%以下
            UserState_HP50,     //HP50%以下
            UserState_Poison,   //毒にかかった
            UserState_Death,    //死亡した
            EnemyState_S10,     //カード使用側では内容のユニットの兵士数10%以下
            EnemyState_S50,     //50%以下
            EnemyState_HP10,    //HP10%以下
            EnemyState_HP50,    //HP50%以下
            EnemyState_Poison,  //毒にかかった
            EnemyState_Death,   //死亡した
            Rand80,             //80%で発動
            Rand50,             //50%で発動
            Rand20,             //20%で発動
        }
        public CardTiming Timing { get; set; }

        //持続回数 (-1は無限)
        public int Duration { get; set; }

        //スキルID
        public int SkillID { get; set; }

        //画像表
        public string ImageFront { get; set; }

        //画像裏
        public string ImageBack { get; set; }

        //効果説明
        public string Description { get; set; }

    }

    //地形補正データ(倍率指定)
    public class AreaBattleFactor
    {
        public float PAtk { get; set; }         //物理攻撃力
        public float MAtk { get; set; }         //魔法攻撃力
        public float PDef { get; set; }         //物理防御力
        public float MDef { get; set; }         //魔法防御力
        public float Leadership { get; set; }   //指揮力
        public float Agility { get; set; }      //機動力
    }

    //地点データ
    public class AreaDataFormat
    {
        //コンストラクタ
        public AreaDataFormat()
        {
            Position = new Vector2();
            BattleFactor = new AreaBattleFactor();
            NextArea = new List<int>();
        }

        //地点番号
        public int ID { get; set; }

        //地点名
        public string Name { get; set; }

        //座標
        public Vector2 Position { get; set; }

        //地点所有者 (TerritoryDataFormatのリストのインデックス
        public int Owner { get; set; }

        //レベル
        public int Level { get; set; }

        //所有マナ
        public int Mana { get; set; }

        //戦闘時間
        public int Time { get; set; }

        //地形補正
        public AreaBattleFactor BattleFactor { get; set; }

        //隣接地点
        public List<int> NextArea { get; set; }

        //背景プレハブパス
        public string BackgroundName { get; set; }
    }

    //領地データ
    public class TerritoryDataFormat
    {
        public TerritoryDataFormat()
        {
            IsActive = true;
            //IsActive = false;

            IsAlive = true;
        }

        //領主名
        public string OwnerName { get; set; }

        //領主名（英語）
        public string OwnerNameEng { get; set; }

        //旗画像パス
        public GameObject FlagPrefab { get; set; }

        //メイン領地
        public int MainArea { get; set; }

        //交戦中かどうか
        public bool IsActive { get; set; }

        //その領地が有効かどうか（占領済みかどうか
        public bool IsAlive { get; set; }

        //最小連戦数
        public int MinBattleNum { get; set; }

        //最大連戦数
        public int MaxBattleNum { get; set; }

        //所有地点リスト
        public List<int> AreaList { get; set; }

        //ユニットリスト
        public List<int> UnitList { get; set; }

        //所持カードリスト
        public List<int> CardList { get; set; }
    }

    //AIデータ
    public class AIDataFormat
    {
        public float AttackRate { get; set; }
    }

    //装備データ
    public class EquipmentDataFormat
    {
        //名前
        public string Name { get; set; }

        //最大HP
        public int MaxHP { get; set; }

        //リーダー(Leader)
        public int LeaderPAtk { get; set; } //物理攻撃
        public int LeaderMAtk { get; set; } //魔法攻撃
        public int LeaderPDef { get; set; } //物理防御
        public int LeaderMDef { get; set; } //魔法防御
        
        //集団(Group)
        public int GroupPAtk { get; set; }  //物理攻撃
        public int GroupMAtk { get; set; }  //魔法攻撃
        public int GroupPDef { get; set; }  //物理防御
        public int GroupMDef { get; set; }  //魔法防御

        //指揮力
        public int Leadership { get; set; }
        //機動力
        public int Agility { get; set; }
        //回復力
        public int Curative { get; set; }

        //説明
        public string Description { get; set; }
    }

    //コンフィグ
    public class ConfigDataFormat
    {
        public ConfigDataFormat()
        {
            //iniで読み込むようにする
            TextSpeed = 10.0f;
        }

        //解像度
        public Vector2 Resolution { get; set; }
        //フルスクリーンか否か
        public bool IsFullScreen { get; set; }
        //グラフィックの質
        public enum GraphicQualityEnum
        {
            High = 0,
            Low = 1
        };
        public GraphicQualityEnum GraphicQuality { get; set; }

        //全体の音量
        public int MasterVolume { get; set; }
        //BGMの音量
        public int BGMVolume { get; set; }
        //SEの音量
        public int SEVolume { get; set; }

        //戦闘の速さ
        public int BattleSpeed { get; set; }

        //テキストスピード
        public float TextSpeed { get; set; }
    }

    //仮想メモリ
    public class VirtualMemory
    {
        public List<object> Memory{ get; private set; }

        public VirtualMemory()
        {
            Memory = Enumerable.Repeat<object>(-1, 70000).ToList();
        }
    }

    //イベントデータ
    public class EventDataFormat
    {
        //スクリプトファイル名
        public string FileName { get; set; }

        //タイミング
        public enum TimingType
        {
            PlayerTurnBegin=0,
            EnemyTurnBegin,
            PlayerBattle,
            EnemyBattle
        }
        public TimingType Timing { get; set; }

        //場所
        public int Area { get; set; }

        //登場人物味方
        public List<int> ActorA { get; set; }

        //登場人物敵
        public List<int> ActorB { get; set; }

        //条件式に使う変数番号
        public int If_Val { get; set; }

        //条件式に使う演算子
        public enum OperationType : int
        {
            Equal=0,
            Bigger,
            Smaller,
            BiggerEqual,
            SmallerEqual,
            NotEqual
        }
        public OperationType If_Ope { get; set; }

        //条件式に使う即値
        public int If_Imm { get; set; }

        //次のスクリプトA
        public int NextA { get; set; }

        //次のスクリプトB
        public int NextB { get; set; }
        
    }

    #endregion

    #region シーン間データ遷移系

    public class BattleDataIn
    {
        public BattleDataIn()
        {
            Reset();
        }

        public void Reset()
        {
            PlayerUnits = Enumerable.Repeat<int>(-1, 3).ToList();
            EnemyUnits = Enumerable.Repeat<int>(-1, 3).ToList();
            PlayerCards = Enumerable.Repeat<int>(-1, 3).ToList();
            EnemyCards = Enumerable.Repeat<int>(-1, 3).ToList();

            PlayerTerritory = 0;
            EnemyTerritory = 1;

            AreaID = 0;
            TimeOfDay = 0;
            IsInvasion = true;
            IsAuto = false;
        }

        //ユニットデータ
        public List<int> PlayerUnits { get; set; }
        public List<int> EnemyUnits { get; set; }

        //カードデータ
        public List<int> PlayerCards { get; set; }
        public List<int> EnemyCards { get; set; }

        //領地情報
        public int PlayerTerritory { get; set; }
        public int EnemyTerritory { get; set; }

        //地点情報
        public int AreaID { get; set; }

        //時間帯
        public int TimeOfDay { get; set; }

        //侵攻戦か防衛戦か
        public bool IsInvasion { get; set; }

        //自動戦闘か否か
        public bool IsAuto { get; set; }
    }

    public class BattleDataOut
    {
        public bool IsWin { get; set; }
    }

    public class ScenarioDataIn
    {
        public string FileName { get; set; }
    }

    #endregion

    public class DataLoader
    {
        public static List<UnitDataFormat> LoadUnitData(string filePath)
        {
            var outData = new List<UnitDataFormat>();

            //ファイルからテキストデータを抽出
            var rowData = CSVReader(filePath);

            //ユニットデータに格納（0番目はキャプションなので読み飛ばす
            for(int i=1; i<rowData.Count; i++)
            {
                if (rowData[i].Count != 41) continue;

                //データの順番
                //[0]ID         [1]名前       [2]レベル      [3]レベル成長限界          [4]HP
                //[5]HP成長率
                //[6]LATK       [7]LMAT       [8]LDEF        [9]LMDE        [10]GATK    [11]GMAT
                //[12]GDEF      [13]GMDE      [14]成LATK     [15]成LMAT     [16]成LDEF  [17]成LMDE
                //[18]成GATK    [19]成GMAT    [20]成GDEF     [21]成GMDE     [22]指揮力　[23]機動力
                //[24]成指揮    [25]成機動
                //[26]回復力　  [27]回復力成長率
                //[28]兵士数    [29]死ぬか撤退か　[30]好感度 
                //[31]リーダー攻撃スキル
                //[32]リーダー防御スキル
                //[33]兵士攻撃スキル   [34]兵士サイズ
                //[35]装備             [36]AI番号
                //[37]立ち絵画像名     [38]顔アイコン画像    
                //[39]戦闘リーダープレハブ名
                //[40]戦闘兵士プレハブ名
                var unit = new UnitDataFormat();
                var data = rowData[i];

                if (data[0] == "") continue;

                try
                {
                    unit.Name = data[1];
                    unit.Level = int.Parse(data[2]);
                    unit.MaxLevel = int.Parse(data[3]);
                    unit.HP = int.Parse(data[4]);
                    unit.MaxHP = unit.HP;
                    unit.HPRate = int.Parse(data[5]);
                    unit.LeaderPAtk = int.Parse(data[6]);
                    unit.LeaderMAtk = int.Parse(data[7]);
                    unit.LeaderPDef = int.Parse(data[8]);
                    unit.LeaderMDef = int.Parse(data[9]);
                    unit.GroupPAtk = int.Parse(data[10]);
                    unit.GroupMAtk = int.Parse(data[11]);
                    unit.GroupPDef = int.Parse(data[12]);
                    unit.GroupMDef = int.Parse(data[13]);
                    unit.LeaderPAtkRate = int.Parse(data[14]);
                    unit.LeaderMAtkRate = int.Parse(data[15]);
                    unit.LeaderPDefRate = int.Parse(data[16]);
                    unit.LeaderMDefRate = int.Parse(data[17]);
                    unit.GroupPAtkRate = int.Parse(data[18]);
                    unit.GroupMAtkRate = int.Parse(data[19]);
                    unit.GroupPDefRate = int.Parse(data[20]);
                    unit.GroupMDefRate = int.Parse(data[21]);
                    unit.Leadership = int.Parse(data[22]);
                    unit.Agility = int.Parse(data[23]);
                    unit.LeadershipRate = int.Parse(data[24]);
                    unit.AgilityRate = int.Parse(data[25]);
                    unit.Curative = int.Parse(data[26]);
                    unit.CurativeRate = int.Parse(data[27]);
                    unit.SoldierNum = int.Parse(data[28]);
                    unit.MaxSoldierNum = unit.SoldierNum;
                    unit.Deathable = (data[29] == "0") ? false : true;
                    unit.Love = int.Parse(data[30]);
                    unit.LAtkSkill = int.Parse(data[31]);
                    unit.LDefSkill = int.Parse(data[32]);
                    unit.GAtkSkill = int.Parse(data[33]);
                    unit.GUnitSize = int.Parse(data[34]);
                    unit.Equipment = int.Parse(data[35]);
                    unit.AIID = int.Parse(data[36]);

                    unit.StandImagePath = data[37];
                    unit.FaceIamgePath = data[38];
                    unit.BattleLeaderPrefabPath = data[39];
                    unit.BattleGroupPrefabPath = data[40];
                }
                catch(ArgumentNullException e)
                {
                    Debug.Log("ユニットデータの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch(FormatException e)
                {
                    Debug.Log("ユニットデータの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch(OverflowException e)
                {
                    Debug.Log("ユニットデータの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(unit);
            }


            return outData;
        }

        public static List<AreaDataFormat> LoadAreaData(string filePath)
        {
            var outData = new List<AreaDataFormat>();
            outData.Add(new AreaDataFormat()); //初めから一つ入れておく(デフォルト地点

            //ファイルからテキストデータを抽出
            var rowData = CSVReader(filePath);

            //地点データに格納（0番目はキャプションなので読み飛ばす
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count < 15) continue;

                //データの順番
                //[0]ID         [1]地点名     [2]x     [3]y    [4]所有者
                //[5]レベル       [6]所有マナ  [7]戦闘時間     
                //[8-13]地形補正    [14]背景プレハブ名 [15-]隣接地点 
                var area = new AreaDataFormat();
                var data = rowData[i];

                try
                {
                    area.ID = int.Parse(data[0]);
                    area.Name = data[1];
                    area.Position = new Vector2(float.Parse(data[2]), float.Parse(data[3]));
                    area.Owner = int.Parse(data[4]);
                    area.Level = int.Parse(data[5]);
                    area.Mana = int.Parse(data[6]);
                    area.Time = int.Parse(data[7]);

                    //地形補正
                    area.BattleFactor.PAtk = int.Parse(data[8]);
                    area.BattleFactor.MAtk = int.Parse(data[9]);
                    area.BattleFactor.PDef = int.Parse(data[10]);
                    area.BattleFactor.MDef = int.Parse(data[11]);
                    area.BattleFactor.Leadership = int.Parse(data[12]);
                    area.BattleFactor.Agility = int.Parse(data[13]);

                    //背景プレハブ名
                    area.BackgroundName = data[14];

                    //隣接地点
                    for (int j=15; j<data.Count; j++)
                    {
                        if (data[j] == "") continue;
                        area.NextArea.Add(int.Parse(data[j]));
                    }
                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("エリアデータの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("エリアデータの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("エリアデータの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(area);
            }



            return outData;
        }

        public static List<EventDataFormat> LoadEventData(string filePath)
        {
            var outData = new List<EventDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for(int i=1; i<rowData.Count; i++)
            {
                if (rowData[i].Count != 10) continue;

                var data = rowData[i];
                var eventData = new EventDataFormat();

                if (data[0] == "" || data[1] == "") continue;

                try
                {
                    //ファイル名
                    eventData.FileName = data[0];

                    //タイミング
                    eventData.Timing = 
                        (EventDataFormat.TimingType)Enum.ToObject(typeof(EventDataFormat.TimingType),
                        int.Parse(data[1]));

                    //地点ＩＤ
                    eventData.Area = int.Parse(data[2]);

                    //味方登場人物
                    eventData.ActorA = new List<int>();
                    var parts = data[3].Split(' ');
                    foreach( string part in parts )
                    {
                        if (part == "") continue;
                        eventData.ActorA.Add(int.Parse(part));
                    }

                    //敵登場人物
                    eventData.ActorB = new List<int>();
                    parts = data[4].Split(' ');
                    foreach( string part in parts)
                    {
                        if (part == "") continue;
                        eventData.ActorB.Add(int.Parse(part));
                    }

                    //条件読み出し
                    if (data[5] != "")
                    {
                        eventData.If_Val = int.Parse(data[5]);
                        eventData.If_Ope =
                            (EventDataFormat.OperationType)Enum.ToObject(typeof(EventDataFormat.OperationType),
                            int.Parse(data[6]));
                        eventData.If_Imm = int.Parse(data[7]);
                    }
                    else
                    {
                        eventData.If_Val = -1;
                    }

                    //次のスクリプト
                    if (data[8] != "") eventData.NextA = int.Parse(data[8]);
                    else eventData.NextA = -1;
                    if (data[9] != "") eventData.NextB = int.Parse(data[9]);
                    else eventData.NextB = -1;
                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("イベントデータの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("イベントデータの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("イベントデータの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(eventData);
            }


            return outData;
        }

        public static List<TerritoryDataFormat>LoadTerritoryData(string filePath)
        {
            var outData = new List<TerritoryDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 8) continue;

                var data = rowData[i];
                var terData = new TerritoryDataFormat();

                if (data[0] == "") continue;

                try
                {
                    //領主名
                    terData.OwnerName = data[0];

                    //領主名英語
                    terData.OwnerNameEng = data[1];

                    //プレハブのロード
                    terData.FlagPrefab = (GameObject)Resources.Load("Prefabs/Field/Flag/" + data[2]);

                    //メイン領地の読み出し
                    terData.MainArea = int.Parse(data[3]);

                    //連戦数の読み出し
                    terData.MinBattleNum = int.Parse(data[4]);
                    terData.MaxBattleNum = int.Parse(data[5]);

                    //所持ユニットリスト
                    terData.UnitList = new List<int>();
                    var parts = data[6].Split(' ');
                    foreach (string part in parts)
                    {
                        if (part == "") continue;
                        terData.UnitList.Add(int.Parse(part));
                    }

                    //所持カードリスト
                    terData.CardList = new List<int>();
                    parts = data[7].Split(' ');
                    foreach (string part in parts)
                    {
                        if (part == "") continue;
                        terData.CardList.Add(int.Parse(part));
                    }
                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("領地データの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("領地データの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("領地データの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(terData);
            }

            return outData;
        }

        public static List<EquipmentDataFormat>LoadEquipmentData(string filePath)
        {
            var outData = new List<EquipmentDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for(int i=1; i<rowData.Count; i++)
            {
                if (rowData[i].Count != 15) continue;


                //データの順番
                //[0]ID     [1]名前       [2]HP
                //[3]PAtk   [4]MAtk       [5]PDef
                //[6]MDef   [7]GPAtk      [8]GMAtk
                //[9]GPDef  [10]GMDef     [11]Lead
                //[12]Agi   [13]回復力    [14]説明
                var data = rowData[i];
                var equipData = new EquipmentDataFormat();

                //無名アイテムがあったら読み飛ばす
                if (data[1] == "") continue;

                try
                {
                    equipData.Name = data[1];
                    equipData.MaxHP = int.Parse(data[2]);
                    equipData.LeaderPAtk = int.Parse(data[3]);
                    equipData.LeaderMAtk = int.Parse(data[4]);
                    equipData.LeaderPDef = int.Parse(data[5]);
                    equipData.LeaderMDef = int.Parse(data[6]);
                    equipData.GroupPAtk = int.Parse(data[7]);
                    equipData.GroupMAtk = int.Parse(data[8]);
                    equipData.GroupPDef = int.Parse(data[9]);
                    equipData.GroupMDef = int.Parse(data[10]);
                    equipData.Leadership = int.Parse(data[11]);
                    equipData.Agility = int.Parse(data[12]);
                    equipData.Curative = int.Parse(data[13]);
                    equipData.Description = data[14];
                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("装備データの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("装備データの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("装備データの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(equipData);
            }

            return outData;
        }

        public static List<AIDataFormat>LoadAIData(string filePath)
        {
            var outData = new List<AIDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 2) continue;


                //データの順番
                //[0]ID     [1]攻撃率
                var data = rowData[i];
                var AIData = new AIDataFormat();

                //無名アイテムがあったら読み飛ばす
                if (data[1] == "") continue;

                try
                {
                    AIData.AttackRate = float.Parse(data[1]);
                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("AIデータの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("AIデータの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("AIデータの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(AIData);
            }

            return outData;
        }

        public static List<SkillDataFormat>LoadSkillData(string filePath)
        {
            var outData = new List<SkillDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 19) continue;


                //データの順番
                //[0]ID [1]名前   [2]威力   [3]スキルタイプ
                //[4]効果時間
                //[5]~[11]ステータスフラグ  
                //[5]物功 [6]物防   [7]魔功   [8]魔防
                //[9]機動 [10]指揮  [11]地形
                //[12]~[14]攻撃属性
                //[12]毒 [13]対ホムンクルス    [14]対ゾンビ
                //[15]召喚するユニットID
                //[16]効果範囲 
                //[17]効果対象
                //[18]エフェクト名
                var data = rowData[i];
                var skill = new SkillDataFormat();

                //無名アイテムがあったら読み飛ばす
                if (data[1] == "") continue;

                try
                {
                    skill.Name = data[1];
                    skill.Power = int.Parse(data[2]);
                    skill.Type = (SkillDataFormat.SkillType)Enum.ToObject(
                        typeof(SkillDataFormat.SkillType), int.Parse(data[3]));
                    skill.Duration = int.Parse(data[4]);

                    //ステータス
                    for (int j = 0, index = 5; j < 7; j++, index++)
                        skill.Status[j] = (data[index] == "0") ? false : true;

                    //特殊ステータス
                    for (int j = 0, index = 12; j < 3; j++, index++)
                        skill.Attribute[j] = (data[index] == "0") ? false : true;

                    skill.SummonUnit = int.Parse(data[15]);
                    skill.Range = (SkillDataFormat.SkillRange)Enum.ToObject(
                        typeof(SkillDataFormat.SkillRange), int.Parse(data[16]));
                    skill.Target = (SkillDataFormat.SkillTarget)Enum.ToObject(
                        typeof(SkillDataFormat.SkillTarget), int.Parse(data[17]));
                    skill.EffectPath = data[18];
                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("スキルデータの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("スキルデータの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("スキルデータの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(skill);
            }
            
            return outData;
        }

        public static List<CardDataFormat>LoadCardData(string filePath)
        {
            var outData = new List<CardDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 7) continue;


                //データの順番
                var data = rowData[i];
                var card = new CardDataFormat();

                //無名アイテムがあったら読み飛ばす
                if (data[0] == "") continue;

                try
                {
                    card.Name = data[0];
                    card.Timing = (CardDataFormat.CardTiming)Enum.ToObject(
                        typeof(CardDataFormat.CardTiming), int.Parse(data[1]));
                    card.Duration = int.Parse(data[2]);
                    card.SkillID = int.Parse(data[3]);
                    card.ImageFront = data[4];
                    card.ImageBack = data[5];
                    card.Description = data[6];

                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("カードデータの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("カードデータの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("カードデータの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(card);
            }

            return outData;
        }

        private static List<List<string>> CSVReader(string filePath)
        {
            var outData = new List<List<string>>();

            try
            {
                using (var stream = new StreamReader(filePath, Encoding.GetEncoding("Shift_JIS")))
                {
                    while (!stream.EndOfStream)
                    {
                        var line = stream.ReadLine();
                        var factors = line.Split(',');

                        var list = new List<string>();
                        foreach (var value in factors)
                        {
                            list.Add(value);
                        }
                        outData.Add(list);
                    }
                }
            }
            catch(System.Exception e)
            {
                Debug.Log("データファイルの読み込みに失敗しました : " + filePath);
                Debug.Log(e.Message);
            }

            return outData;
        }
    }

    public class GamePath
    {
        public const string Data = "Assets\\Resources\\Data\\";
        public const string Senario = "Assets\\Resources\\Scenarios\\";
    }
}