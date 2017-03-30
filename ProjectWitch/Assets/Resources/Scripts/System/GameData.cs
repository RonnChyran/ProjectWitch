using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq; //iOSで動かないかも

using UnityEngine;
using ProjectWitch.Extention;

namespace ProjectWitch
{
    //プレイヤーデータ用のメタデータ
    public class GameMetaData : ISaveMetaData
    {
        //タイムスタンプ
        public int Year { get; set; }       //yearのみ255を超えるのでint型
        public byte Month { get; set; }
        public byte Day { get; set; }
        public byte Hour { get; set; }
        public byte Minute { get; set; }

        //占領済み領地数
        public int DominatedTerritory { get; set; }

        //アリスのレベル
        public int Level { get; set; }

        //経過ターン数
        public int Turn { get; set; }

        //クラスサイズの取得
        public override int GetSize()
        {
            var size = base.GetSize();
            size += Marshal.SizeOf(Year);
            size += Marshal.SizeOf(Month);
            size += Marshal.SizeOf(Day);
            size += Marshal.SizeOf(Hour);
            size += Marshal.SizeOf(Minute);
            size += Marshal.SizeOf(DominatedTerritory);
            size += Marshal.SizeOf(Level);
            size += Marshal.SizeOf(Turn);

            return size;
        }

        //セーブファイルに書き込むバイト列を取得
        public override byte[] GetSaveBytes()
        {
            List<byte> outList = new List<byte>();

            //値のアップデート
            Update();

            //version
            outList.Add(Major);
            outList.Add(Minor);

            //time stamp
            outList.AddRange(BitConverter.GetBytes(Year));
            outList.Add(Month);
            outList.Add(Day);
            outList.Add(Hour);
            outList.Add(Minute);

            //ゲームデータ
            outList.AddRange(BitConverter.GetBytes(DominatedTerritory));
            outList.AddRange(BitConverter.GetBytes(Level));
            outList.AddRange(BitConverter.GetBytes(Turn));

            return outList.ToArray();
        }

        //バイト列からデータを復元
        public override void SetFromBytes(byte[] data)
        {
            int offset = 0;

            //version
            Major = data[offset++];
            Minor = data[offset++];

            //time stamp
            Year = BitConverter.ToInt32(data, offset); offset += 4;
            Month = data[offset++];
            Day = data[offset++];
            Hour = data[offset++];
            Minute = data[offset++];

            //ゲームデータ
            DominatedTerritory = BitConverter.ToInt32(data, offset); offset += 4;
            Level = BitConverter.ToInt32(data, offset); offset += 4;
            Turn = BitConverter.ToInt32(data, offset); offset += 4;
        }

        //値のアップ―デート
        private void Update()
        {
            var game = Game.GetInstance();

            //タイムスタンプ
            var timestamp = DateTime.Now;
            Year = timestamp.Year;
            Month = (byte)timestamp.Month;
            Day = (byte)timestamp.Day;
            Hour = (byte)timestamp.Hour;
            Minute = (byte)timestamp.Minute;

            //ゲームデータ
            DominatedTerritory = game.GameData.Territory[0].AreaList.Count;
            Level = game.GameData.Unit[0].Level;
            Turn = game.GameData.CurrentTurn;

        }
    }

    //システムデータ用のメタデータ
    public class SystemMetaData : ISaveMetaData
    {
        //セーブファイルに書き込むバイト列を取得
        public override byte[] GetSaveBytes()
        {
            List<byte> outList = new List<byte>();

            //version
            outList.Add(Major);
            outList.Add(Minor);

            return outList.ToArray();
        }

        //バイト列からデータを復元
        public override void SetFromBytes(byte[] data)
        {
            int offset = 0;

            //version
            Major = data[offset++];
            Minor = data[offset++];
        }

    }

    //プレイヤーデータ
    public class GameData : ISaveableData
    {
        //セーブファイルのバージョン
        private GameMetaData metaData = new GameMetaData();

        #region data_member

        //ユニットデータ
        public List<UnitDataFormat> Unit { get; set; }
        //スキルデータ
        public List<SkillDataFormat> Skill { get; set; }
        //土地データ
        public List<AreaDataFormat> Area { get; set; }
        //領地データ
        public List<TerritoryDataFormat> Territory { get; set; }
        //グループデータ
        public List<GroupDataFormat> Group { get; set; }
        //AIデータ
        public List<AIDataFormat> AI { get; set; }
        //装備データ
        public List<EquipmentDataFormat> Equipment { get; set; }
        //カードデータ
        public List<CardDataFormat> Card { get; set; }
        //イベントデータ
        public List<EventDataFormat> FieldEvent { get; set; }
        public List<EventDataFormat> TownEvent { get; set; }

        //所持マナ
        public int PlayerMana { get; set; }
        //現在のターン数
        public int CurrentTurn { get; set; }
        //現在の時間数 0:朝 1:昼 2:夜 3~:敵ターン
        public int CurrentTime { get; set; }
        //フィールドのBGM
        public string FieldBGM { get; set; }
        //通常バトルのBGM
        public string BattleBGM { get; set; }

        //システム変数
        public VirtualMemory Memory { get; private set; }
        #endregion

        #region method
        //データをリセットする
        public void Reset()
        {
            try
            {
                //データ系の初期化
                Unit = new List<UnitDataFormat>();
                Skill = new List<SkillDataFormat>();
                Area = new List<AreaDataFormat>();
                Territory = new List<TerritoryDataFormat>();
                Group = new List<GroupDataFormat>();
                AI = new List<AIDataFormat>();
                Equipment = new List<EquipmentDataFormat>();
                Card = new List<CardDataFormat>();
                FieldEvent = new List<EventDataFormat>();
                TownEvent = new List<EventDataFormat>();
                PlayerMana = 10000;

                CurrentTime = 0; //朝から
                CurrentTurn = 1;
                FieldBGM = "002_alice1";
                BattleBGM = "004_battle1";

                Memory = new VirtualMemory(20000);


            }
            catch (InvalidCastException)
            {
                Debug.LogError("キャストミスです");
                return;
            }
            catch (OverflowException)
            {
                Debug.LogError("データがオーバーフローしました");
                return;
            }

            //データロード

            //ユニットデータの読み出し
            Unit = DataLoader.LoadUnitData(GamePath.Data + "unit_data");

            //スキルデータの読み出し
            Skill = DataLoader.LoadSkillData(GamePath.Data + "skill_data");

            //地点データの読み出し
            Area = DataLoader.LoadAreaData(GamePath.Data + "area_data");

            //領地データの読み出し
            Territory = DataLoader.LoadTerritoryData(GamePath.Data + "territory_data");

            //グループデータの読み出し
            Group = DataLoader.LoadGroupData(GamePath.Data + "group_data");

            //AI
            AI = DataLoader.LoadAIData(GamePath.Data + "ai_data");

            //装備
            Equipment = DataLoader.LoadEquipmentData(GamePath.Data + "equipment_data");

            //カード
            Card = DataLoader.LoadCardData(GamePath.Data + "card_data");

            //イベントデータの読み出し
            FieldEvent = DataLoader.LoadEventData(GamePath.Data + "event_data_field");
            TownEvent = DataLoader.LoadEventData(GamePath.Data + "event_data_town");

        }

        //データをセーブファイルに書き出す
        public void Save(int slot)
        {
            FileIO.SaveBinary(GamePath.GameSaveFilePath(slot), metaData, this);
        }

        //データをセーブファイルから読み込む
        public void Load(int slot)
        {
            var meta = new GameMetaData();

            var inst = new GameData();
            inst.Copy(this);
            FileIO.LoadBinary(GamePath.GameSaveFilePath(slot), meta, inst);
            this.Copy(inst);

            //ファイルのバージョンチェック
            metaData = meta;
        }

        //引数に与えられたオブジェクトをコピーする
        public void Copy(GameData inst)
        {
            Unit = inst.Unit;
            Skill = inst.Skill;
            Area = inst.Area;
            Territory = inst.Territory;
            Group = inst.Group;
            AI = inst.AI;
            Equipment = inst.Equipment;
            Card = inst.Card;
            FieldEvent = inst.FieldEvent;
            TownEvent = inst.TownEvent;
            PlayerMana = inst.PlayerMana;
            CurrentTime = inst.CurrentTime;
            CurrentTurn = inst.CurrentTurn;
            FieldBGM = inst.FieldBGM;
            BattleBGM = inst.BattleBGM;
            Memory = inst.Memory;
        }

        //セーブ用データをByte配列にパックして取得
        public override byte[] GetSaveBytes()
        {
            var outdata = new List<byte>();

            //セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
            outdata.AddRange(BitConverter.GetBytes(Unit.Count));
            outdata.AddRange(Unit.GetBytes());

            outdata.AddRange(BitConverter.GetBytes(Area.Count));
            outdata.AddRange(Area.GetBytes());

            outdata.AddRange(BitConverter.GetBytes(Territory.Count));
            outdata.AddRange(Territory.GetBytes());

            outdata.AddRange(BitConverter.GetBytes(Group.Count));
            outdata.AddRange(Group.GetBytes());

            outdata.AddRange(BitConverter.GetBytes(PlayerMana));
            outdata.AddRange(BitConverter.GetBytes(CurrentTurn));
            outdata.AddRange(BitConverter.GetBytes(CurrentTime));

            outdata.AddRange(BitConverter.GetBytes(
                Encoding.UTF8.GetByteCount(FieldBGM)));
            outdata.AddRange(Encoding.UTF8.GetBytes(FieldBGM));

            outdata.AddRange(BitConverter.GetBytes(
                Encoding.UTF8.GetByteCount(BattleBGM)));
            outdata.AddRange(Encoding.UTF8.GetBytes(BattleBGM));

            outdata.AddRange(Memory.GetSaveBytes());

            return outdata.ToArray();
        }

        //byte配列からデータを再現
        public override int SetFromBytes(int _offset, byte[] data)
        {
            int offset = _offset;

            //データ代入
            var unitCount = BitConverter.ToInt32(data, offset); offset += 4;
            if (Unit.Count < unitCount) Unit = new List<UnitDataFormat>(unitCount);
            for (int i = 0; i < unitCount; i++)
            {
                offset = Unit[i].SetFromBytes(offset, data);
            }

            var areaCount = BitConverter.ToInt32(data, offset); offset += 4;
            if (Area.Count < areaCount) Area = new List<AreaDataFormat>(areaCount);
            for (int i = 0; i < areaCount; i++)
            {
                offset = Area[i].SetFromBytes(offset, data);
            }

            var territoryCount = BitConverter.ToInt32(data, offset); offset += 4;
            if (Territory.Count < territoryCount) Territory = new List<TerritoryDataFormat>(territoryCount);
            for (int i = 0; i < territoryCount; i++)
            {
                offset = Territory[i].SetFromBytes(offset, data);
            }

            var groupCount = BitConverter.ToInt32(data, offset); offset += 4;
            if (Group.Count > groupCount) Group = new List<GroupDataFormat>(groupCount);
            for (int i = 0; i < groupCount; i++)
            {
                offset = Group[i].SetFromBytes(offset, data);
            }

            PlayerMana = BitConverter.ToInt32(data, offset); offset += 4;
            CurrentTurn = BitConverter.ToInt32(data, offset); offset += 4;
            CurrentTime = BitConverter.ToInt32(data, offset); offset += 4;

            var strlength = BitConverter.ToInt32(data, offset); offset += 4;
            byte[] strbytes = new byte[strlength];
            Array.Copy(data, offset, strbytes, 0, strlength); offset += strlength;
            FieldBGM = Encoding.UTF8.GetString(strbytes);

            strlength = BitConverter.ToInt32(data, offset); offset += 4;
            strbytes = new byte[strlength];
            Array.Copy(data, offset, strbytes, 0, strlength); offset += strlength;
            BattleBGM = Encoding.UTF8.GetString(strbytes);

            offset = Memory.SetFromBytes(offset, data);

            return offset;
        }
        #endregion
    }

    //システムデータ
    public class SystemData : ISaveableData
    {
        //セーブファイルのバージョン
        private SystemMetaData metaData = new SystemMetaData();

        #region data_member

        //コンフィグ
        public ConfigDataFormat Config { get; set; }

        //仮想メモリ(CGギャラリーの開放、周回フラグなどを含める)
        public VirtualMemory Memory { get; set; }
        #endregion

        #region method
        //データを初期化する
        public void Reset()
        {
            Config = new ConfigDataFormat();
            Memory = new VirtualMemory(1024);
        }

        //データをシステムファイルに書き出す
        public void Save()
        {
            FileIO.SaveBinary(GamePath.SystemSaveFilePath(), metaData, this);
        }

        //データをシステムファイルから読み込む
        public void Load()
        {
            var inst = new SystemData();
            inst.Copy(this);
            FileIO.LoadBinary(GamePath.SystemSaveFilePath(), metaData, inst);
            this.Copy(inst);
        }

        //コピーメソッド
        public void Copy(SystemData inst)
        {
            Config = inst.Config;
            Memory = inst.Memory;
        }

        //セーブ用データをByte配列にパックして取得
        public override byte[] GetSaveBytes()
        {
            var outdata = new List<byte>();

            //セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
            outdata.AddRange(Config.GetSaveBytes());
            outdata.AddRange(Memory.GetSaveBytes());

            return outdata.ToArray();
        }

        //byte配列からデータを再現
        public override int SetFromBytes(int _offset, byte[] data)
        {
            int offset = _offset;

            //データ代入
            offset = Config.SetFromBytes(offset, data);
            offset = Memory.SetFromBytes(offset, data);

            return offset;
        }
        #endregion
    }

    //各種データ構造

    #region ゲームデータ系

    //ユニットデータ
    public class UnitDataFormat : ISaveableData
    {
        //コンストラクタ
        public UnitDataFormat()
        {
            Love = 0;
            IsAlive = true;
            Experience = 10;
            SoldierCost = 2;
            HPCost = 8;
            SoldierLimitCost = 35;
        }

        #region data_member
        //ID
        [System.Xml.Serialization.XmlAttribute("id")]
        public int ID { get; set; }

        //ユニット名
        public string Name { get; set; }

        //レベル
        public int Level { get; set; }

        //レベル成長限界
        public int MaxLevel { get; set; }

        //HP
        public int HP { get; set; }
        //最大HP
        public int MaxHP { get { return (int)(HP0 + HP100 / 100.0f * Level); } private set { } }
        public int HP0 { get; set; }
        public int HP100 { get; set; }     //HP成長率

        //経験値
        public int Experience { get; set; }

        //初期値
        public int LPAtk0 { get; set; }     //物理攻撃
        public int LMAtk0 { get; set; }     //魔法攻撃
        public int LPDef0 { get; set; }     //物理防御
        public int LMDef0 { get; set; }     //魔法防御

        public int GPAtk0 { get; set; }     //物理攻撃
        public int GMAtk0 { get; set; }     //魔法攻撃
        public int GPDef0 { get; set; }     //物理防御
        public int GMDef0 { get; set; }     //魔法防御

        public int LPAtk100 { get; set; }   //物理攻撃Lv100時
        public int LMAtk100 { get; set; }   //魔法攻撃Lv100時
        public int LPDef100 { get; set; }   //物理防御Lv100時
        public int LMDef100 { get; set; }   //魔法防御Lv100時

        public int GPAtk100 { get; set; }   //物理攻撃Lv100時
        public int GMAtk100 { get; set; }   //魔法攻撃Lv100時
        public int GPDef100 { get; set; }   //物理防御Lv100時
        public int GMDef100 { get; set; }   //魔法防御Lv100時

        //指揮力
        public int Lead0 { get; set; }    //指揮力初期値
        public int Lead100 { get; set; }    //指揮力Lv100時
                                            //機動力
        public int Agi0 { get; set; }    //機動力初期値
        public int Agi100 { get; set; }    //機動力Lv100時
                                           //回復力
        public int Cur0 { get; set; }   //回復力初期値
        public int Cur100 { get; set; }   //回復力Lv100時

        //兵士数
        public int SoldierNum { get; set; }
        public int MaxSoldierNum { get; set; }

        //撤退するか死ぬか ture:死ぬ　false：撤退する（捕獲コマンドが無効
        public bool Deathable { get; set; }

        //生きているか
        public bool IsAlive { get; set; }

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

        //HP回復コスト
        public int HPCost { get; set; }

        //兵士回復コスト
        public int SoldierCost { get; set; }

        //最大兵士数成長コスト
        public int SoldierLimitCost { get; set; }

        //立ち絵画像名
        public string StandImagePath { get; set; }
        //顔アイコン画像名
        public string FaceIamgePath { get; set; }
        //戦闘リーダープレハブ名
        public string BattleLeaderPrefabPath { get; set; }
        //戦闘兵士プレハブ名
        public string BattleGroupPrefabPath { get; set; }

        //死亡時セリフ
        public string OnDeadSerif { get; set; }
        //捕獲時セリフ
        public string OnCapturedSerif { get; set; }
        //逃走時セリフ
        public string OnEscapedSerif { get; set; }

        //アリスのコメント
        public string Comment { get; set; }
        #endregion

        #region query

        //リーダーのステータス
        [System.Xml.Serialization.XmlIgnore]
        public int LeaderPAtk { get { return (int)(LPAtk0 + LPAtk100 / 100.0f * Level); } private set { } } //物理攻撃
        [System.Xml.Serialization.XmlIgnore]
        public int LeaderMAtk { get { return (int)(LMAtk0 + LMAtk100 / 100.0f * Level); } private set { } } //魔法攻撃
        [System.Xml.Serialization.XmlIgnore]
        public int LeaderPDef { get { return (int)(LPDef0 + LPDef100 / 100.0f * Level); } private set { } } //物理防御
        [System.Xml.Serialization.XmlIgnore]
        public int LeaderMDef { get { return (int)(LMDef0 + LMDef100 / 100.0f * Level); } private set { } } //魔法防御

        //兵士のステータス
        [System.Xml.Serialization.XmlIgnore]
        public int GroupPAtk { get { return (int)(GPAtk0 + GPAtk100 / 100.0f * Level); } private set { } }  //物理攻撃
        [System.Xml.Serialization.XmlIgnore]
        public int GroupMAtk { get { return (int)(GMAtk0 + GMAtk100 / 100.0f * Level); } private set { } }  //魔法攻撃
        [System.Xml.Serialization.XmlIgnore]
        public int GroupPDef { get { return (int)(GPDef0 + GPDef100 / 100.0f * Level); } private set { } }  //物理防御
        [System.Xml.Serialization.XmlIgnore]
        public int GroupMDef { get { return (int)(GMDef0 + GMDef100 / 100.0f * Level); } private set { } }  //魔法防御
        [System.Xml.Serialization.XmlIgnore]
        public int Leadership { get { return (int)(Lead0 + Lead100 / 100.0f * Level); } private set { } }  //指揮力
        [System.Xml.Serialization.XmlIgnore]
        public int Curative { get { return (int)(Cur0 + Cur100 / 100.0f * Level); } private set { } }       //回復力
        [System.Xml.Serialization.XmlIgnore]
        public int Agility { get { return (int)(Agi0 + Agi100 / 100.0f * Level); } private set { } }        //機動力
        #endregion

        #region method 

        //コピーメソッド
        public UnitDataFormat Clone()
        {
            return (UnitDataFormat)MemberwiseClone();
        }

        //死亡状態にする
        public void Kill()
        {
            SoldierNum = 0;
            HP = 0;
            IsAlive = false;
        }

        //復活させる
        public void Rebirth()
        {
            SoldierNum = MaxSoldierNum;
            HP = MaxHP;
            IsAlive = true;
        }

        //セーブ用データをByte配列にパックして取得
        public override byte[] GetSaveBytes()
        {
            var outdata = new List<byte>();

            //セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
            outdata.AddRange(BitConverter.GetBytes(ID));
            outdata.AddRange(BitConverter.GetBytes(Level));
            outdata.AddRange(BitConverter.GetBytes(HP));
            outdata.AddRange(BitConverter.GetBytes(Experience));
            outdata.AddRange(BitConverter.GetBytes(SoldierNum));
            outdata.AddRange(BitConverter.GetBytes(Deathable));
            outdata.AddRange(BitConverter.GetBytes(IsAlive));
            outdata.AddRange(BitConverter.GetBytes(Love));
            outdata.AddRange(BitConverter.GetBytes(Equipment));

            return outdata.ToArray();
        }

        //byte配列からデータを再現
        public override int SetFromBytes(int _offset, byte[] data)
        {
            int offset = _offset;

            ID = BitConverter.ToInt32(data, offset); offset += 4;
            Level = BitConverter.ToInt32(data, offset); offset += 4;
            HP = BitConverter.ToInt32(data, offset); offset += 4;
            Experience = BitConverter.ToInt32(data, offset); offset += 4;
            SoldierNum = BitConverter.ToInt32(data, offset); offset += 4;
            Deathable = BitConverter.ToBoolean(data, offset); offset += 1;
            IsAlive = BitConverter.ToBoolean(data, offset); offset += 1;
            Love = BitConverter.ToInt32(data, offset); offset += 4;
            Equipment = BitConverter.ToInt32(data, offset); offset += 4;


            return offset;
        }
        #endregion

    }

    //スキルデータ
    public class SkillDataFormat
    {
        public SkillDataFormat()
        {
            Status = Enumerable.Repeat<bool>(false, 7).ToList();
            Attribute = Enumerable.Repeat<bool>(false, 3).ToList();
        }

        //ID
        [System.Xml.Serialization.XmlAttribute("id")]
        public int ID { get; set; }

        //名前
        public string Name { get; set; }

        //威力
        public int Power { get; set; }

        //種類
        public enum SkillType
        {
            Damage = 0,       //0:ダメ―ジ
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
            All = 0,      //0:全員
            Single      //1:単体
        }
        public SkillRange Range { get; set; }

        //対象
        public enum SkillTarget
        {
            Enemy = 0,        //0:敵
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

        //説明
        public string Description { get; set; }

    }

    //カードデータ
    public class CardDataFormat
    {
        //ID
        [System.Xml.Serialization.XmlAttribute("id")]
        public int ID { get; set; }

        //名前
        public string Name { get; set; }

        //タイミング
        public enum CardTiming
        {
            BattleBegin = 0,      //戦闘開始
            BattleEnd,          //戦闘終了
            UserState_S10,      //カード使用側のどれかのユニットの兵士数10%以下
            UserState_S50,      //兵士数50%以下
            UserState_HP10,     //HP10%以下
            UserState_HP50,     //HP50%以下
            UserState_Poison,   //毒にかかった
            UserState_Death,    //死亡した
            EnemyState_S10,     //カード使用側ではないどれかのユニットの兵士数10%以下
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

        //売値
        public int BuyingPrice { get; set; }
        //買値
        public int SellingPrice { get; set; }
        //店に並ぶフラグ
        public int ShopFlag { get; set; }

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
    public class AreaDataFormat : ISaveableData
    {
        //コンストラクタ
        public AreaDataFormat()
        {
            Position = new Vector2();
            BattleFactor = new AreaBattleFactor();
            NextArea = new List<int>();
        }

        #region data_member
        //地点番号
        [System.Xml.Serialization.XmlAttribute("id")]
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

        //限界マナ
        public int MaxMana { get; set; }

        //戦闘時間
        public int Time { get; set; }

        //地形補正
        public AreaBattleFactor BattleFactor { get; set; }

        //隣接地点
        public List<int> NextArea { get; set; }

        //背景プレハブパス
        public string BackgroundName { get; set; }
        #endregion

        #region method
        public override byte[] GetSaveBytes()
        {
            var outdata = new List<byte>();

            //セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
            outdata.AddRange(BitConverter.GetBytes(ID));
            outdata.AddRange(BitConverter.GetBytes(Owner));
            outdata.AddRange(BitConverter.GetBytes(Mana));
            outdata.AddRange(BitConverter.GetBytes(NextArea.Count));
            outdata.AddRange(NextArea.GetBytes());

            return outdata.ToArray();
        }

        //byte配列からデータを再現
        public override int SetFromBytes(int _offset, byte[] data)
        {
            int offset = _offset;

            ID = BitConverter.ToInt32(data, offset); offset += 4;
            Owner = BitConverter.ToInt32(data, offset); offset += 4;
            Mana = BitConverter.ToInt32(data, offset); offset += 4;

            var nextAreaCount = BitConverter.ToInt32(data, offset); offset += 4;
            NextArea = new List<int>();
            for (int i = 0; i < nextAreaCount; i++)
            {
                NextArea.Add(BitConverter.ToInt32(data, offset)); offset += 4;
            }

            return offset;
        }
        #endregion
    }

    //領地データ
    public class TerritoryDataFormat : ISaveableData
    {
        public TerritoryDataFormat()
        {
        }

        #region data_member

        //領主ID
        [System.Xml.Serialization.XmlAttribute("id")]
        public int ID { get; set; }

        //領主名
        public string OwnerName { get; set; }

        //領主名（英語）
        public string OwnerNameEng { get; set; }

        //旗画像名
        public string FlagTexName { get; set; }

        //メイン領地
        public int MainArea { get; set; }

        //所有地点リスト
        [System.Xml.Serialization.XmlIgnore]
        public List<int> AreaList
        {
            get
            {
                var game = Game.GetInstance();
                var areadata = game.GameData.Area;

                //地域データから自分が所持している地点を探す
                var list = new List<int>();
                foreach (var area in areadata)
                {
                    if (area.Owner == ID) list.Add(area.ID);
                }
                return list;
            }
            private set { }
        }

        //所持グループリスト
        public List<int> GroupList { get; set; }

        //占領済みフラグの変数番号
        public int DeadFlagIndex { get; set; }

        //交戦フラグの変数番号
        public int ActiveFlagIndex { get; set; }

        //宣戦布告可能フラグの変数番号
        public int InvationableFlagIndex { get; set; }

        //状態
        public enum TerritoryState : int
        {
            Prepare = 0,    //宣戦布告不可
            Ready,      //宣戦布告可
            Active,     //交戦中
            Dead        //占領済み
        }
        private TerritoryState state = TerritoryState.Prepare;
        public TerritoryState State
        {
            get
            {
                var game = Game.GetInstance();

                if (state == TerritoryState.Prepare)
                {
                    if (InvationableFlagIndex == -1 || !game.GameData.Memory.IsZero(InvationableFlagIndex))
                        state = TerritoryState.Ready;
                }
                if (state == TerritoryState.Ready)
                {
                    if (ActiveFlagIndex == -1 || !game.GameData.Memory.IsZero(ActiveFlagIndex))
                        state = TerritoryState.Active;
                }
                if (state == TerritoryState.Active)
                {
                    if (DeadFlagIndex == -1 || !game.GameData.Memory.IsZero(DeadFlagIndex))
                        state = TerritoryState.Dead;
                }
                return state;
            }
            private set { }
        }

        //そのターンの行動数(アクションパネルの表示用
        public int ActionCount { get; set; }

        #endregion

        #region method

        //指定のユニットを全グループから除外
        public void RemoveUnit(int unit)
        {
            var game = Game.GetInstance();
            var groups = game.GameData.Group.GetFromIndex(GroupList);

            //すべてのグループからユニットを除外
            foreach (var group in groups)
            {
                group.UnitList.Remove(unit);
            }
        }

        //セーブするデータをbyte配列にパックして取得
        public override byte[] GetSaveBytes()
        {
            var outdata = new List<byte>();

            //セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
            outdata.AddRange(BitConverter.GetBytes(ID));
            outdata.AddRange(BitConverter.GetBytes(AreaList.Count));
            outdata.AddRange(AreaList.GetBytes());
            outdata.AddRange(BitConverter.GetBytes((int)State));
            outdata.AddRange(BitConverter.GetBytes(ActionCount));

            return outdata.ToArray();
        }

        //byte配列からデータを再現
        public override int SetFromBytes(int _offset, byte[] data)
        {
            int offset = _offset;

            ID = BitConverter.ToInt32(data, offset); offset += 4;

            var areaListCount = BitConverter.ToInt32(data, offset); offset += 4;
            AreaList = new List<int>();
            for (int i = 0; i < areaListCount; i++)
            {
                AreaList.Add(BitConverter.ToInt32(data, offset)); offset += 4;
            }

            State = EnumConverter.ToEnum<TerritoryState>(
                        BitConverter.ToInt32(data, offset)); offset += 4;
            ActionCount = BitConverter.ToInt32(data, offset); offset += 4;

            return offset;
        }

        #endregion
    }

    //グループデータ
    public class GroupDataFormat : ISaveableData
    {
        #region data_member

        //ID
        [System.Xml.Serialization.XmlAttribute("id")]
        public int ID { get; set; }

        //グループ名
        public string Name { get; set; }

        //戦闘タイプ列挙
        public enum BattleType : int
        {
            ToDestroyedAll = 1,         //全ユニットが死亡するまで戦う
            ToDestroyedOne = 2,         //あるユニットの兵士が全滅するか、リーダーが死亡するまで戦う
            ToDestroyedOneLeader = 3,   //あるユニットのリーダーが死亡するまで戦う
            HarfTimePass = 4,           //総戦闘時間の半分が経過するまで戦う
            HarfTimePassForceQuit = 5,  //全体ターンの半分が経過したら戦闘を中断する
            DontDomination = 6,         //侵攻しない（進行パターン
        }

        //侵攻タイプ
        public BattleType DominationType { get; set; }

        //防衛タイプ
        public BattleType DefenseType { get; set; }

        //防衛優先度
        public int DefensePriority { get; set; }

        //リストの選択方法列挙
        public enum ChoiseMethod : int
        {
            AscendingOrder = 1,     //順番に生きているユニットを選択
            Random3 = 2,            //ランダムに生きている３体を選択
            RandomAll = 3,          //数もメンバーもランダムに決める
        }

        //ユニットの選択方法
        public ChoiseMethod UnitChoiseMethod { get; set; }

        //カードの選択方法
        public ChoiseMethod CardChoiseMethod { get; set; }

        //侵攻開始フラグ
        public int BeginDominationFlagIndex { get; set; }

        //侵攻ルート
        public List<int> DominationRoute { get; set; }

        //ユニットリスト
        public List<int> UnitList { get; set; }

        //カードリスト
        public List<int> CardList { get; set; }

        //状態
        public enum GroupState : int
        {
            Ready,  //始動前
            Active, //活動中
            Dead    //死亡
        }
        private GroupState state = GroupState.Ready;
        [System.Xml.Serialization.XmlIgnore]
        public GroupState State
        {
            get
            {
                var game = Game.GetInstance();

                if (state == GroupState.Ready)
                    if (BeginDominationFlagIndex == -1)
                        state = GroupState.Active;
                    else if (!game.GameData.Memory.IsZero(BeginDominationFlagIndex))
                        state = GroupState.Active;

                return state;
            }
            private set { }
        }

        #endregion

        #region method

        //戦闘に出すユニットを取得
        public List<int> GetBattleUnits()
        {
            //ユニットリストからメソッドに応じて３体抽出
            var units = new List<int>();
            switch (UnitChoiseMethod)
            {
                case ChoiseMethod.AscendingOrder:
                    units = UnitList.GetOrderN(3);
                    break;
                case ChoiseMethod.Random3:
                    units = UnitList.RandomN(3);
                    break;
                case ChoiseMethod.RandomAll:
                    units = UnitList.RandomN(UnityEngine.Random.Range(1, 3));
                    break;
                default:
                    break;
            }

            //3隊に満たない部分を-1で補充
            while (units.Count < 3)
                units.Add(-1);

            return units;
        }

        //戦闘に出すカードを取得
        public List<int> GetBattleCards()
        {
            //カードリストからメソッドに応じて３体抽出
            var cards = new List<int>();
            switch (CardChoiseMethod)
            {
                case ChoiseMethod.AscendingOrder:
                    cards = CardList.GetOrderN(3);
                    break;
                case ChoiseMethod.Random3:
                    cards = CardList.RandomN(3);
                    break;
                case ChoiseMethod.RandomAll:
                    cards = CardList.RandomN(UnityEngine.Random.Range(1, 3));
                    break;
                default:
                    break;
            }

            //３つに満たない部分を-1で補充
            while (cards.Count < 3)
                cards.Add(-1);

            return cards;
        }

        //死亡状態に移行する
        public void Kill()
        {
            state = GroupState.Dead;
        }

        //復活させる
        public void Rebirth()
        {
            state = GroupState.Active;
        }

        //自営団のIDを取得
        public static int GetDefaultID()
        {
            return 49;
        }

        //セーブするデータをbyte配列にパックして取得
        public override byte[] GetSaveBytes()
        {
            var outdata = new List<byte>();

            //セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
            outdata.AddRange(BitConverter.GetBytes(ID));
            outdata.AddRange(BitConverter.GetBytes(UnitList.Count));
            outdata.AddRange(UnitList.GetBytes());
            outdata.AddRange(BitConverter.GetBytes((int)State));

            return outdata.ToArray();
        }

        //byte配列からデータを再現
        public override int SetFromBytes(int _offset, byte[] data)
        {
            int offset = _offset;

            ID = BitConverter.ToInt32(data, offset); offset += 4;

            var unitListCount = BitConverter.ToInt32(data, offset); offset += 4;
            UnitList = new List<int>();
            for (int i = 0; i < unitListCount; i++)
            {
                UnitList.Add(BitConverter.ToInt32(data, offset)); offset += 4;
            }

            State = EnumConverter.ToEnum<GroupState>(
                        BitConverter.ToInt32(data, offset)); offset += 4;

            return offset;
        }


        #endregion
    }

    //AIデータ
    public class AIDataFormat
    {
        public float AttackRate { get; set; }
    }

    //装備データ
    public class EquipmentDataFormat
    {
        //ID
        public int ID { get; set; }

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

        //買値
        public int BuyingPrice { get; set; }
        //売値
        public int SellingPrice { get; set; }
        //商店に並ぶかどうかフラグ
        public int ShopFlag { get; set; }

        //説明
        public string Description { get; set; }
    }

    //コンフィグ
    public class ConfigDataFormat : ISaveableData
    {
        public ConfigDataFormat()
        {
            //iniで読み込むようにする
            TextSpeed = 50.0f;

            BGMVolume = 0.5f;
            SEVolume = 0.5f;
            VoiceVolume = 1.0f;
            MasterVolume = 0.3f;
        }

        #region data_member
        //解像度
        public Vector2 Resolution { get; set; }
        //フルスクリーンか否か
        public bool IsFullScreen { get; set; }
        //グラフィックの質
        public enum GraphicQualityEnum : int
        {
            High = 0,
            Low = 1
        };
        public GraphicQualityEnum GraphicQuality { get; set; }

        //全体の音量
        public float MasterVolume { get; set; }
        //BGMの音量
        public float BGMVolume { get; set; }
        //SEの音量
        public float SEVolume { get; set; }
        //Voiceの音量
        public float VoiceVolume { get; set; }

        //戦闘の速さ
        public int BattleSpeed { get; set; }

        //テキストスピード
        public float TextSpeed { get; set; }
        #endregion

        #region method
        //セーブするデータをbyte配列にパックして取得
        public override byte[] GetSaveBytes()
        {
            var outdata = new List<byte>();

            //セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
            outdata.AddRange(BitConverter.GetBytes(Resolution.x));
            outdata.AddRange(BitConverter.GetBytes(Resolution.y));
            outdata.AddRange(BitConverter.GetBytes(IsFullScreen));
            outdata.AddRange(BitConverter.GetBytes((int)GraphicQuality));
            outdata.AddRange(BitConverter.GetBytes(MasterVolume));
            outdata.AddRange(BitConverter.GetBytes(BGMVolume));
            outdata.AddRange(BitConverter.GetBytes(SEVolume));
            outdata.AddRange(BitConverter.GetBytes(BattleSpeed));
            outdata.AddRange(BitConverter.GetBytes(TextSpeed));

            return outdata.ToArray();
        }

        //byte配列からデータを再現
        public override int SetFromBytes(int _offset, byte[] data)
        {
            int offset = _offset;

            var resolution = new Vector2();
            resolution.x = BitConverter.ToSingle(data, offset); offset += 4;
            resolution.y = BitConverter.ToSingle(data, offset); offset += 4;
            Resolution = resolution;

            IsFullScreen = BitConverter.ToBoolean(data, offset); offset += 1;
            GraphicQuality = EnumConverter.ToEnum<GraphicQualityEnum>(
                                BitConverter.ToInt32(data, offset)); offset += 4;
            MasterVolume = BitConverter.ToSingle(data, offset); offset += 4;
            BGMVolume = BitConverter.ToSingle(data, offset); offset += 4;
            SEVolume = BitConverter.ToSingle(data, offset); offset += 4;
            BattleSpeed = BitConverter.ToInt32(data, offset); offset += 4;
            TextSpeed = BitConverter.ToSingle(data, offset); offset += 4;

            return offset;
        }


        #endregion
    }

    //仮想メモリ
    public class VirtualMemory : ISaveableData
    {
        //シリアライズ出力用のアクセスメンバ
        public List<int> Data { get; set; }

        public int this[int index]
        {
            get { return Data[index]; }
            set { Data[index] = value; }
        }

        public VirtualMemory(int num)
        {
            Data = Enumerable.Repeat<int>(0, num).ToList();
        }
        public VirtualMemory()
        {
            Data = Enumerable.Repeat<int>(0, 1).ToList();
        }

        #region query

        //指定したインデックスの値が、整数の0かどうか
        public bool IsZero(int index)
        {
            if (index < 0 || index >= Count) return false;

            var num = Data[index];
            return (num == 0);
        }

        //配列サイズの取得
        [System.Xml.Serialization.XmlIgnore]
        public int Count
        {
            get
            {
                return Data.Count;
            }
            private set { }
        }

        #endregion

        #region method
        //セーブするデータをbyte配列にパックして取得
        public override byte[] GetSaveBytes()
        {
            var outdata = new List<byte>();

            //セーブするデータ（ゲーム内で変更の可能性のあるデータ）を追加
            outdata.AddRange(BitConverter.GetBytes(Data.Count));
            outdata.AddRange(Data.GetBytes());

            return outdata.ToArray();
        }

        //byte配列からデータを再現
        public override int SetFromBytes(int _offset, byte[] data)
        {
            int offset = _offset;

            var dataCount = BitConverter.ToInt32(data, offset); offset += 4;
            if (dataCount > Data.Count) Data = new List<int>(dataCount);
            for (int i = 0; i < dataCount; i++)
            {
                Data[i] = BitConverter.ToInt32(data, offset); offset += 4;
            }

            return offset;
        }


        #endregion
    }

    //イベントデータ
    public class EventDataFormat
    {
        public EventDataFormat()
        {
            If_Var = new List<int>();
            If_Ope = new List<OperationType>();
            If_Imm = new List<int>();

            NextA = -1;
            NextB = -1;
        }

        //スクリプトファイル名
        public string FileName { get; set; }

        //タイミング
        public enum TimingType
        {
            PlayerTurnBegin = 0,
            EnemyTurnBegin,
            PlayerBattle,
            EnemyBattle,
            AfterBattle
        }
        public TimingType Timing { get; set; }

        //場所
        public int Area { get; set; }

        //生きている必要があるユニット
        public List<int> IfAlive { get; set; }

        //条件式に使う変数番号
        public List<int> If_Var { get; set; }

        //条件式に使う演算子
        public enum OperationType : int
        {
            Equal = 0,
            Bigger,
            Smaller,
            BiggerEqual,
            SmallerEqual,
            NotEqual
        }
        public List<OperationType> If_Ope { get; set; }

        //条件式に使う即値
        public List<int> If_Imm { get; set; }

        //次のスクリプトA
        public int NextA { get; set; }

        //次のスクリプトB
        public int NextB { get; set; }

    }

    //イベントデータ用演算子の拡張メソッド
    public static class OperationTypeExt
    {
        static string[] opeStr =
{
                "=",
                ">",
                "<",
                ">=",
                "<=",
                "!="
            };
        public static EventDataFormat.OperationType Parse(this EventDataFormat.OperationType ope, string str)
        {
            for (int i = 0; i < opeStr.Length; i++)
            {
                if (str.Equals(opeStr[i]))
                {
                    return (EventDataFormat.OperationType)Enum.ToObject(typeof(EventDataFormat.OperationType),
                        i);
                }
            }
            return EventDataFormat.OperationType.Equal;
        }
    }

    #endregion

    #region シーン間データ遷移系

    //バトルシーンへの入力
    public class BattleDataIn
    {
        public BattleDataIn()
        {
            Init();
            BGM = "004_battle1";
        }

        public void Reset()
        {
            Init();
            BGM = Game.GetInstance().GameData.BattleBGM;
        }

        void Init()
        {

            PlayerUnits = Enumerable.Repeat<int>(-1, 3).ToList();
            EnemyUnits = Enumerable.Repeat<int>(-1, 3).ToList();
            PlayerCards = Enumerable.Repeat<int>(-1, 3).ToList();
            EnemyCards = Enumerable.Repeat<int>(-1, 3).ToList();

            //テストデータセット
            PlayerTerritory = 0;
            EnemyTerritory = 9;

            AreaID = 3;
            TimeOfDay = 1;
            IsInvasion = true;
            IsAuto = false;
            IsEvent = false;
            IsEnable = true;

            PlayerUnits[0] = 296;
            PlayerUnits[1] = 297;
            PlayerUnits[2] = 300;
            EnemyUnits[0] = 298;
            EnemyUnits[1] = 301;
            EnemyUnits[2] = 302;

            IsTutorial = false;

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

        //イベント戦闘か否か
        public bool IsEvent { get; set; }
        
        //バトルを行うかどうか
        public bool IsEnable { get; set; }

        //バトルBGM
        public string BGM { get; set; }

        //敵の戦闘タイプ
        public GroupDataFormat.BattleType EnemyBattleType { get; set; }

        //チュートリアルフラグ
        public bool IsTutorial { get; set; }
    }

    //バトルシーンからの出力
    public class BattleDataOut
    {
        public BattleDataOut()
        {
            DeadUnits = new List<int>();
            CapturedUnits = new List<int>();
            EscapedUnits = new List<int>();
            UsedCards = new List<int>();
        }

        //戦闘勝利フラグ
        public bool IsWin { get; set; }
        //死亡したユニット
        public List<int> DeadUnits { get; set; }
        //捕獲したユニット
        public List<int> CapturedUnits { get; set; }
        //逃走したユニット
        public List<int> EscapedUnits { get; set; }
        //使用したカード
        public List<int> UsedCards { get; set; }

        //初期状態に戻す
        public void Reset()
        {
            DeadUnits.Clear();
            CapturedUnits.Clear();
            EscapedUnits.Clear();
            UsedCards.Clear();
        }
    }

    //会話シーンへの入力
    public class ScenarioDataIn
    {
        public void Reset()
        {
            FileName = "";
            NextA = -1;
            NextB = -1;
            IsTest = false;
        }

        public string FileName { get; set; }
        public int NextA { get; set; }
        public int NextB { get; set; }
        public bool IsTest { get; set; } //テストモードフラグ
    }

    //メニューシーンへの入力
    public class MenuDataIn
    {
        public void Reset()
        {
            TutorialMode = false;
        }

        public bool TutorialMode { get; set; }
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
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 45) continue;

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
                //[40]戦闘兵士プレハブ名 [41]キャラ説明
                //[42]死亡時セリフ
                //[43]捕獲時セリフ
                //[44]逃走時セリフ
                var unit = new UnitDataFormat();
                var data = rowData[i];

                if (data[0] == "") continue;

                try
                {
                    unit.ID = int.Parse(data[0]);
                    unit.Name = data[1];
                    unit.Level = int.Parse(data[2]);
                    unit.MaxLevel = int.Parse(data[3]);
                    unit.HP0 = int.Parse(data[4]);
                    unit.HP100 = int.Parse(data[5]);
                    unit.HP = unit.MaxHP;
                    unit.LPAtk0 = int.Parse(data[6]);
                    unit.LMAtk0 = int.Parse(data[7]);
                    unit.LPDef0 = int.Parse(data[8]);
                    unit.LMDef0 = int.Parse(data[9]);
                    unit.GPAtk0 = int.Parse(data[10]);
                    unit.GMAtk0 = int.Parse(data[11]);
                    unit.GPDef0 = int.Parse(data[12]);
                    unit.GMDef0 = int.Parse(data[13]);
                    unit.LPAtk100 = int.Parse(data[14]);
                    unit.LMAtk100 = int.Parse(data[15]);
                    unit.LPDef100 = int.Parse(data[16]);
                    unit.LMDef100 = int.Parse(data[17]);
                    unit.GPAtk100 = int.Parse(data[18]);
                    unit.GMAtk100 = int.Parse(data[19]);
                    unit.GPDef100 = int.Parse(data[20]);
                    unit.GMDef100 = int.Parse(data[21]);
                    unit.Lead0 = int.Parse(data[22]);
                    unit.Agi0 = int.Parse(data[23]);
                    unit.Lead100 = int.Parse(data[24]);
                    unit.Agi100 = int.Parse(data[25]);
                    unit.Cur0 = int.Parse(data[26]);
                    unit.Cur100 = int.Parse(data[27]);
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
                    unit.Comment = data[41];

                    unit.OnDeadSerif = data[42];
                    unit.OnCapturedSerif = data[43];
                    unit.OnEscapedSerif = data[44];

                    unit.IsAlive = true;

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
                    area.MaxMana = int.Parse(data[6]);
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
                    for (int j = 15; j < data.Count; j++)
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

            //データの形式
            //[0]id [1]filename [2]タイミング
            //[3]場所 [4]味方登場人物 [5]敵登場人物
            //[6]条件1:変数 [7]条件1:式 [8]条件1:即値
            //[9]条件2:変数 [10]条件2:式 [11]条件2:即値
            //[13]次のスクリプト1 [14]次のスクリプト2

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count < 3) continue;

                var data = rowData[i];
                var eventData = new EventDataFormat();

                if (data[0] == "" || data[1] == "") continue;

                try
                {
                    //ファイル名
                    eventData.FileName = data[1];

                    //タイミング
                    eventData.Timing = EnumConverter.ToEnum<EventDataFormat.TimingType>(int.Parse(data[2]));

                    //地点ＩＤ
                    if (data[3] != "")
                        eventData.Area = int.Parse(data[3]);
                    else
                        eventData.Area = -1;

                    //IfAlive
                    eventData.IfAlive = new List<int>();
                    var parts = data[4].Split(' ');
                    foreach (string part in parts)
                    {
                        if (part == "") continue;
                        eventData.IfAlive.Add(int.Parse(part));
                    }

                    //次のスクリプト
                    if (data[5] != "") eventData.NextA = int.Parse(data[5]);
                    else eventData.NextA = -1;
                    if (data[6] != "") eventData.NextB = int.Parse(data[6]);
                    else eventData.NextB = -1;


                    for (int index = 7, j = 0; j < 3 * 3; j += 3)
                    {
                        //条件読み出し1
                        if (data[index + j] != "")
                        {
                            var dummy = EventDataFormat.OperationType.Equal;
                            eventData.If_Var.Add(int.Parse(data[index + j]));
                            eventData.If_Ope.Add(dummy.Parse(data[index + j + 1]));
                            eventData.If_Imm.Add(int.Parse(data[index + j + 2]));
                        }
                        else
                        {
                            eventData.If_Var.Add(-1);
                            eventData.If_Ope.Add(EventDataFormat.OperationType.Equal);
                            eventData.If_Imm.Add(0);
                        }
                    }

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

        public static List<TerritoryDataFormat> LoadTerritoryData(string filePath)
        {
            var outData = new List<TerritoryDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //[0]ID [1]領主名　[2]領主名(英語)
            //[3]旗画像パス [4]メイン領地 [5]グループリスト 
            //[6]占領フラグ [7]交戦フラグ
            //[8]宣戦布告可能フラグ

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 9) continue;

                var data = rowData[i];
                var terData = new TerritoryDataFormat();

                if (data[0] == "") continue;

                try
                {
                    //ID
                    terData.ID = int.Parse(data[0]);

                    //領主名
                    terData.OwnerName = data[1];

                    //領主名英語
                    terData.OwnerNameEng = data[2];

                    //プレハブのロード
                    terData.FlagTexName = data[3];

                    //メイン領地の読み出し
                    terData.MainArea = int.Parse(data[4]);

                    //グループリスト
                    terData.GroupList = new List<int>();
                    var parts = data[5].Split(' ');
                    foreach (string part in parts)
                    {
                        if (part == "") continue;
                        terData.GroupList.Add(int.Parse(part));
                    }

                    //占領フラグ
                    if (data[6] == "")
                        terData.DeadFlagIndex = -1;
                    else
                        terData.DeadFlagIndex = int.Parse(data[6]);

                    //交戦フラグ
                    if (data[7] == "")
                        terData.ActiveFlagIndex = -1;
                    else
                        terData.ActiveFlagIndex = int.Parse(data[7]);

                    //宣戦布告可能フラグ
                    if (data[8] == "")
                        terData.InvationableFlagIndex = -1;
                    else
                        terData.InvationableFlagIndex = int.Parse(data[8]);
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

        public static List<GroupDataFormat> LoadGroupData(string filePath)
        {
            var outData = new List<GroupDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //[0]ID [1]名前
            //[2]侵攻タイプ [3]防衛タイプ 
            //[4]防衛優先度 [5]ユニットの選択方法
            //[6]カードの選択方法 [7]侵攻開始フラグ [8]侵攻ルート
            //[9]ユニットリスト [10]カードリスト

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 11) continue;

                var data = rowData[i];
                var groupData = new GroupDataFormat();

                if (data[0] == "") continue;

                try
                {
                    //ID
                    groupData.ID = int.Parse(data[0]);

                    //領主名
                    groupData.Name = data[1];

                    //侵攻タイプ
                    groupData.DominationType =
                        EnumConverter.ToEnum<GroupDataFormat.BattleType>(int.Parse(data[2]));

                    //防衛タイプ
                    groupData.DefenseType =
                        EnumConverter.ToEnum<GroupDataFormat.BattleType>(int.Parse(data[3]));

                    //防衛優先度
                    groupData.DefensePriority = int.Parse(data[4]);

                    //ユニットの選択方法
                    groupData.UnitChoiseMethod =
                        EnumConverter.ToEnum<GroupDataFormat.ChoiseMethod>(int.Parse(data[5]));

                    //カードの選択方法
                    groupData.CardChoiseMethod =
                        EnumConverter.ToEnum<GroupDataFormat.ChoiseMethod>(int.Parse(data[6]));

                    //侵攻開始フラグ
                    if (data[7] == "")
                        groupData.BeginDominationFlagIndex = -1;
                    else
                        groupData.BeginDominationFlagIndex = int.Parse(data[7]);

                    //侵攻ルート
                    var list = new List<int>();
                    var areas = data[8].Split(' ');
                    foreach (var area in areas)
                    {
                        if (area == "") continue;
                        list.Add(int.Parse(area));
                    }
                    groupData.DominationRoute = list;

                    //ユニットリスト
                    list = new List<int>();
                    var units = data[9].Split(' ');
                    foreach (var unit in units)
                    {
                        if (unit == "") continue;
                        list.Add(int.Parse(unit));
                    }
                    groupData.UnitList = list;

                    //カードリスト
                    list = new List<int>();
                    var cards = data[10].Split(' ');
                    foreach (var card in cards)
                    {
                        if (card == "") continue;
                        list.Add(int.Parse(card));
                    }
                    groupData.CardList = list;
                }
                catch (ArgumentNullException e)
                {
                    Debug.Log("グループデータの読み取りに失敗：データが空です");
                    Debug.Log(e.Message);
                }
                catch (FormatException e)
                {
                    Debug.Log("グループデータの読み取りに失敗：データの形式が違います");
                    Debug.Log(e.Message);
                }
                catch (OverflowException e)
                {
                    Debug.Log("グループデータの読み取りに失敗：データがオーバーフローしました");
                    Debug.Log(e.Message);
                }

                outData.Add(groupData);
            }

            return outData;
        }

        public static List<EquipmentDataFormat> LoadEquipmentData(string filePath)
        {
            var outData = new List<EquipmentDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 18) continue;


                //データの順番
                //[0]ID     [1]名前       [2]HP
                //[3]PAtk   [4]MAtk       [5]PDef
                //[6]MDef   [7]GPAtk      [8]GMAtk
                //[9]GPDef  [10]GMDef     [11]Lead
                //[12]Agi   [13]回復力    [14]売値
                //[15]買値  [16]店に出るかフラグ [17]説明
                var data = rowData[i];
                var equipData = new EquipmentDataFormat();

                //無名アイテムがあったら読み飛ばす
                if (data[1] == "") continue;

                try
                {
                    equipData.ID = int.Parse(data[0]);
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
                    equipData.BuyingPrice = int.Parse(data[14]);
                    equipData.SellingPrice = int.Parse(data[15]);
                    if (data[16] == "")
                        equipData.ShopFlag = -1;
                    else
                        equipData.ShopFlag = int.Parse(data[16]);
                    equipData.Description = data[17];
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

        public static List<AIDataFormat> LoadAIData(string filePath)
        {
            var outData = new List<AIDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 3) continue;


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

        public static List<SkillDataFormat> LoadSkillData(string filePath)
        {
            var outData = new List<SkillDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 20) continue;


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
                //[19]説明
                var data = rowData[i];
                var skill = new SkillDataFormat();

                //無名アイテムがあったら読み飛ばす
                if (data[1] == "") continue;

                try
                {
                    skill.ID = int.Parse(data[0]);
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
                    skill.Description = data[19];
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

        public static List<CardDataFormat> LoadCardData(string filePath)
        {
            var outData = new List<CardDataFormat>();

            //生データの読み出し
            var rowData = CSVReader(filePath);

            //データの代入
            for (int i = 1; i < rowData.Count; i++)
            {
                if (rowData[i].Count != 11) continue;


                //データの順番
                var data = rowData[i];
                var card = new CardDataFormat();

                //無名アイテムがあったら読み飛ばす
                if (data[1] == "") continue;

                try
                {
                    card.ID = int.Parse(data[0]);
                    card.Name = data[1];
                    card.Timing = EnumConverter.ToEnum<CardDataFormat.CardTiming>(int.Parse(data[2]));
                    card.Duration = int.Parse(data[3]);
                    card.SkillID = int.Parse(data[4]);
                    card.ImageFront = data[5];
                    card.ImageBack = data[6];
                    card.BuyingPrice = int.Parse(data[7]);
                    card.SellingPrice = int.Parse(data[8]);
                    if (data[9] == "")
                        card.ShopFlag = -1;
                    else
                        card.ShopFlag = int.Parse(data[9]);
                    card.Description = data[7];

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

            var tAsset = Resources.Load(filePath) as TextAsset;
            if (tAsset == null)
            {
                Debug.Assert(false, "データファイルの読み込みに失敗しました : " + filePath);
            }

            var text = tAsset.text;
            var reader = new StringReader(text);

            while (reader.Peek() > -1)
            {
                var line = reader.ReadLine();
                var factors = line.Split(',');

                var list = new List<string>();
                foreach (var value in factors)
                {
                    list.Add(value);
                }
                outData.Add(list);
            }

            return outData;
        }

    }

    public class GamePath
    {
        public static readonly string Data = "Data/";
        public static readonly string SaveFolderPath = Application.dataPath + "/SaveData/";

        public static string GameSaveFilePath(int index) { return SaveFolderPath + "save" + index.ToString() + ".dat"; }
        public static string SystemSaveFilePath() { return SaveFolderPath + "sys_save"; }
    }
}