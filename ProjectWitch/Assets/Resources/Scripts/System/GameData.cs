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
        public string FlagTexPath { get; set; }

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
        private Dictionary<string, object> mMemory;
        public void setValue(object value, string name)
        {
            mMemory.Add(name, value);
        }
        public object getValue(string name, object defaultValue)
        {
            object value;
            bool succeed = mMemory.TryGetValue(name, out value);
            if (succeed)
                return value;
            else
                return defaultValue;
        }
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

                try
                {
                    unit.Name = data[1];
                    unit.Level = Int32.Parse(data[2]);
                    unit.MaxLevel = Int32.Parse(data[3]);
                    unit.HP = Int32.Parse(data[4]);
                    unit.MaxHP = unit.HP;
                    unit.LeaderPAtk = Int32.Parse(data[5]);
                    unit.LeaderMAtk = Int32.Parse(data[6]);
                    unit.LeaderPDef = Int32.Parse(data[7]);
                    unit.LeaderMDef = Int32.Parse(data[8]);
                    unit.GroupPAtk = Int32.Parse(data[9]);
                    unit.GroupMAtk = Int32.Parse(data[10]);
                    unit.GroupPDef = Int32.Parse(data[11]);
                    unit.GroupMDef = Int32.Parse(data[12]);
                    unit.LeaderPAtkRate = Int32.Parse(data[13]);
                    unit.LeaderMAtkRate = Int32.Parse(data[14]);
                    unit.LeaderPDefRate = Int32.Parse(data[15]);
                    unit.LeaderMDefRate = Int32.Parse(data[16]);
                    unit.GroupPAtkRate = Int32.Parse(data[17]);
                    unit.GroupMAtkRate = Int32.Parse(data[18]);
                    unit.GroupPDefRate = Int32.Parse(data[19]);
                    unit.GroupMDefRate = Int32.Parse(data[20]);
                    unit.Leadership = Int32.Parse(data[21]);
                    unit.Agility = Int32.Parse(data[22]);
                    unit.Curative = Int32.Parse(data[23]);
                    unit.SoldierNum = Int32.Parse(data[24]);
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
                    unit.AIID = Int32.Parse(data[32]);
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
                    area.Name = data[1];
                    area.Position = new Vector2(Int32.Parse(data[2]), Int32.Parse(data[3]));
                    area.Owner = Int32.Parse(data[4]);
                    area.Level = Int32.Parse(data[5]);
                    area.Mana = Int32.Parse(data[6]);
                    area.NextArea = new List<int>();

                    for(int j=7; j<data.Count; j++)
                    {
                        area.NextArea.Add(Int32.Parse(data[i]));
                    }
                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("ユニットデータの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("ユニットデータの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("ユニットデータの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(area);
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