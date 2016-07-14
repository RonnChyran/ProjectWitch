using System;
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

        //表示名(戦闘中など)
        public string DisplayName { get; set; }

        //レベル
        public int Level { get; set; }

        //レベル成長限界
        public int MaxLevel { get; set; }

        //立ち絵画像名
        public string StandImagePath { get; set; }
        //顔アイコン画像名
        public string FaceIamgePath { get; set; }

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

}