using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace GameData
{
    //各種データ構造
    //ユニットデータ
    public class UnitDataFormat
    {
        //ユニット名
        public string Name { get; set; }

        //レベル
        public int Level { get; set; }

        //レベル成長限界
        public int MaxLevel { get; set; }

        //立ち絵画像名
        public string StandImagePath { get; set; }
        //顔アイコン画像名
        public string FaceIamgePath { get; set; }
        //戦闘リーダー画像名
        public List<string> BattleLeaderImagePath { get; set; }
        //戦闘兵士画像名
        public List<string> BattleGroupImagePath { get; set; }

        //AI番号
        public int AIID { get; set; }


        //HP
        public int HP { get; set; }
        //最大HP
        public int MaxHP { get; set; }

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
        //機動力
        public int Agility { get; set; }
        //回復力
        public int Curative { get; set; }
        //兵士数
        public int SoldierNum { get; set; }
        //最大兵士数
        public int MaxSoldierNum { get; set; }

        //装備ID
        public int Equipment { get; set; }
    }

    //スキルデータ
    public class SkillDataFormat
    {
        //威力
        public int Power { get; set; }

        //種類
        public enum SkillType
        {
            Damage,         //ダメ―ジ
            Heal,               //回復
            StatusUp,       //ステ上昇
            StatusDown  //ステ下降
        };
        public SkillType Type { get; set; }

        //ステ種類
        //[0]物功,[1]物防,[2]魔攻,[3]魔防,[4]機動,[5]指揮力,[6]地形補正
        public List<bool> Status { get; set; }

        //特殊（拡張可能性あり）
        //[0]毒付与、[1]ガード、[2]ダメージ還元、[3]順番下げ、[4]ダメ無効、[5]対ホムンクルス
        public List<bool> OtherState { get; set; }

        //範囲
        public enum SkillRange
        {
            All,
            Single
        }
        public SkillRange Range { get; set; }

        //対象
        //[0]敵集団、[1]味方集団、[2]自分、[3]敵リーダー、[4]味方リーダー
        public List<bool> Target { get; set; }

        //効果時間(ターン数）
        public int Duration { get; set; }

        //視覚エフェクト
        public string EffectPath { get; set; }
    }

    //地形補正データ(倍率指定)
    public class AreaBattleFactor
    {
        public float PAtk { get; set; }         //物理攻撃力
        public float MAtk { get; set; }         //魔法攻撃力
        public float PDef { get; set; }         //物理防御力
        public float MDef { get; set; }         //魔法防御力
        public float Readership { get; set; }   //指揮力
        public float Agility { get; set; }      //機動力
    }

    //地点データ
    public class AreaDataFormat
    {
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

        //地形補正
        public AreaBattleFactor BattleFactor { get; set; }

        //隣接地点
        public List<int> NextArea { get; set; }

        //背景プレハブパス
        public string BackgroundPath { get; set; }
    }
    //領地データ
    public class TerritoryDataFormat
    {
        //領主名
        public string OwnerName { get; set; }

        //旗画像パス
        public GameObject FlagPrefab { get; set; }

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
        //最大HP
        public int MaxHP { get; set; }

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

        //指揮力
        public int Leadership { get; set; }
        //機動力
        public int Agility { get; set; }
        //回復力
        public int Curative { get; set; }
    }

    //コンフィグ
    public class ConfigDataFormat
    {
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
    }

    //仮想メモリ
    public class VirtualMemory
    {
        private Dictionary<int, object> mMemory = new Dictionary<int, object>();

        public void SetValue(int id, object value)
        {
            mMemory.Add(id, value);
        }
        public object GetValue(int id, object defaultValue)
        {
            object value;
            bool succeed = mMemory.TryGetValue(id, out value);
            if (succeed)
                return value;
            else
                return defaultValue;
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
                if (rowData[i].Count != 33) continue;

                //データの順番
                //[0]ID         [1]名前       [2]レベル      [3]レベル成長限界          [4]HP
                //[5]LATK       [6]LMAT       [7]LDEF        [8]LMDE        [9]GATK     [10]GMAT
                //[11]GDEF      [12]GMDE      [13]成LATK     [14]成LMAT     [15]成LDEF  [16]成LMDE
                //[17]成GATK    [18]成GMAT    [19]成GDEF     [20]成GMDE     [21]指揮力　[22]機動力
                //[23]回復力　  [24]兵士数　  [25]立ち絵画像名    [26]顔アイコン画像    [27-29]戦闘リーダー画像
                //[30-31]戦闘兵士画像         [32]AI番号
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
                    unit.LeaderPAtk = int.Parse(data[5]);
                    unit.LeaderMAtk = int.Parse(data[6]);
                    unit.LeaderPDef = int.Parse(data[7]);
                    unit.LeaderMDef = int.Parse(data[8]);
                    unit.GroupPAtk = int.Parse(data[9]);
                    unit.GroupMAtk = int.Parse(data[10]);
                    unit.GroupPDef = int.Parse(data[11]);
                    unit.GroupMDef = int.Parse(data[12]);
                    unit.LeaderPAtkRate = int.Parse(data[13]);
                    unit.LeaderMAtkRate = int.Parse(data[14]);
                    unit.LeaderPDefRate = int.Parse(data[15]);
                    unit.LeaderMDefRate = int.Parse(data[16]);
                    unit.GroupPAtkRate = int.Parse(data[17]);
                    unit.GroupMAtkRate = int.Parse(data[18]);
                    unit.GroupPDefRate = int.Parse(data[19]);
                    unit.GroupMDefRate = int.Parse(data[20]);
                    unit.Leadership = int.Parse(data[21]);
                    unit.Agility = int.Parse(data[22]);
                    unit.Curative = int.Parse(data[23]);
                    unit.SoldierNum = int.Parse(data[24]);
                    unit.MaxSoldierNum = unit.SoldierNum;
                    unit.StandImagePath = data[25];
                    unit.FaceIamgePath = data[26];
                    unit.BattleLeaderImagePath = new List<string>();
                    unit.BattleGroupImagePath = new List<string>();
                    unit.BattleLeaderImagePath.Add(data[27]);
                    unit.BattleLeaderImagePath.Add(data[28]);
                    unit.BattleLeaderImagePath.Add(data[29]);
                    unit.BattleGroupImagePath.Add(data[30]);
                    unit.BattleGroupImagePath.Add(data[31]);
                    unit.AIID = int.Parse(data[32]);
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
            outData.Add(new AreaDataFormat()); //初めから一つ入れておく

            //ファイルからテキストデータを抽出
            var rowData = CSVReader(filePath);

            //地点データに格納（0番目はキャプションなので読み飛ばす
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count < 8) continue;

                //データの順番
                //[0]ID         [1]地点名     [2]x     [3]y    [4]所有者
                //[5]レベル       [6]所有マナ       [7-]隣接地点 
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
                    area.NextArea = new List<int>();

                    for(int j=7; j<data.Count; j++)
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

                if (data[0] == "") continue;

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
                    if(data[8] != "")eventData.NextA = int.Parse(data[8]);
                    if(data[9] != "")eventData.NextB = int.Parse(data[9]);
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
                if (rowData[i].Count != 4) continue;

                var data = rowData[i];
                var terData = new TerritoryDataFormat();

                if (data[0] == "") continue;

                try
                {
                    //領主名
                    terData.OwnerName = data[0];

                    //プレハブのロード
                    terData.FlagPrefab = (GameObject)Resources.Load("Prefabs/Field/Flag/" + data[1]);

                    //所持ユニットリスト
                    terData.UnitList = new List<int>();
                    var parts = data[2].Split(' ');
                    foreach (string part in parts)
                    {
                        if (part == "") continue;
                        terData.UnitList.Add(int.Parse(part));
                    }

                    //所持ユニットリスト
                    terData.CardList = new List<int>();
                    parts = data[3].Split(' ');
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

}