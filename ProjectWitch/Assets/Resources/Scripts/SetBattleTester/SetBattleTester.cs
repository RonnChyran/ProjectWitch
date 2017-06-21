using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FileManager;
using MK.Utility;
using System.IO;

namespace ProjectWitch.BattleTester
{
	public class SetBattleTester : MonoBehaviour
	{
		public Game Game { get; set; }
		// 空顔画像
		[SerializeField]
		private Sprite m_SpriteNoneFace = null, m_SpriteNoneCard = null;
		public Sprite SpriteNoneFace { get { return m_SpriteNoneFace; } }
		public Sprite SpriteNoneCard { get { return m_SpriteNoneCard; } }
		// ユニットセットUI
		[SerializeField]
		private SetUnitUI m_SetUnitUI = null;
		// カードセットUI
		[SerializeField]
		private SetCardUI m_SetCardUI = null;
		// 地形情報セットUI
		[SerializeField]
		private SetAreaUI m_SetAreaUI = null;
		// 地形情報セットUI
		[SerializeField]
		private SetSkillUI m_SetSkillUI = null;
		// 戦闘開始ボタン
		[SerializeField]
		private Button m_BuBattleStart = null;
		[SerializeField]
		private Dropdown m_DdSkill = null;
		[SerializeField]
		private InputField m_IFSave = null;
		[SerializeField]
		private GameObject m_BackUI = null;
		// 選択キャラ情報
		[SerializeField]
		private Transform m_TrPlayer = null, m_TrEnemy = null;
		public class UnitData
		{
			public SetBattleTester SBT { get; private set; }
			public Transform Transform { get; private set; }
			public int FirstLevel { get; set; }
			public int Pos { get; private set; }
			public bool IsPlayer { get; private set; }
			public Image ImageFace { get { return Transform.Find("ImFace").GetComponent<Image>(); } }
			public Text TextName { get { return Transform.Find("TeName").GetComponent<Text>(); } }
			public Slider SliderHP { get { return Transform.Find("SlHP").GetComponent<Slider>(); } }
			public Slider SliderSoldierNum { get { return Transform.Find("SlSoldierNum").GetComponent<Slider>(); } }
			public UnitDataFormat Data { get { return SBT.Game.GameData.Unit[ID]; } }
			private int mID;
			public int ID
			{
				get { return mID; }
				set
				{
					if (value == -1)
					{
						mID = -1;
						FirstLevel = 0;
						ImageFace.sprite = SBT.m_SpriteNoneFace;
						TextName.text = "(選択なし)";
					}
					else if (value >= 0 && SBT.Game.GameData.Unit.Count > value)
					{
						mID = value;
						FirstLevel = Data.Level;
						
						ImageFace.sprite = Resources.Load<Sprite>("Textures/Face/" + Data.FaceIamgePath);
						if (ImageFace.sprite == null)
							ImageFace.sprite = SBT.m_SpriteNoneFace;
						TextName.text = Data.Name;
					}
					SetHPs();
				}
			}

			public UnitData(Transform _transform, int _pos, bool _isPlayer, SetBattleTester _setBattleTester)
			{
				SBT = _setBattleTester;
				Transform = _transform;
				Pos = _pos;
				IsPlayer = _isPlayer;
				ID = -1;
				Transform.GetComponent<UIEvent>().onUpLeft.AddListener(e => SBT.SelectCharaUnit(this));
				Transform.GetComponent<UIEvent>().onUpRight.AddListener(e => {
					if (ID != -1)
						Data.Level = FirstLevel;
					ID = -1;
				});
				SliderHP.onValueChanged.AddListener(value => ChangeHP(value));
				SliderSoldierNum.onValueChanged.AddListener(value => ChangeSoldierNum(value));
			}

			public void SetHPs()
			{
				if (ID == -1)
				{
					SliderHP.interactable = false;
					SliderSoldierNum.interactable = false;
					SliderHP.minValue = SliderHP.maxValue = 0;
					ChangeHP(0);
					SliderSoldierNum.minValue = SliderSoldierNum.maxValue = 0;
					ChangeSoldierNum(0);
				}
				else
				{
					SliderHP.interactable = true;
					SliderSoldierNum.interactable = true;
					SliderHP.maxValue = Data.MaxHP;
					SliderHP.minValue = 1;
					if (Data.HP <= 0)
						Data.HP = 1;
					SliderHP.value = Data.HP;
					SliderSoldierNum.maxValue = Data.MaxSoldierNum;
					SliderSoldierNum.minValue = 0;
					SliderSoldierNum.value = Data.SoldierNum;
				}
			}

			public void ChangeHP(float _value)
			{
				if (ID == -1)
					SliderHP.transform.Find("TeValue").GetComponent<Text>().text = "0/0";
				else
				{
					Data.HP = (int)_value;
					SliderHP.transform.Find("TeValue").GetComponent<Text>().text =
						Data.HP.ToString() + String.Format("/{0,5}", Data.MaxHP);
				}
			}

			public void ChangeSoldierNum(float _value)
			{
				if (ID == -1)
					SliderSoldierNum.transform.Find("TeValue").GetComponent<Text>().text = "0/0";
				else
				{
					Data.SoldierNum = (int)_value;
					SliderSoldierNum.transform.Find("TeValue").GetComponent<Text>().text =
						Data.SoldierNum.ToString() + String.Format("/{0,5}", Data.MaxSoldierNum);
				}
			}
		}
		public List<UnitData> PlayerUnits { get; private set; }
		public List<UnitData> EnemyUnits { get; private set; }
		public List<UnitData> Units { get; private set; }
		public class CardData
		{
			public SetBattleTester SBT { get; private set; }
			public Transform Transform { get; private set; }
			public int Num { get; private set; }
			public bool IsPlayer { get; private set; }
			public Image ImageCard { get { return Transform.Find("ImCard").GetComponent<Image>(); } }
			public Text TextName { get { return Transform.Find("TeName").GetComponent<Text>(); } }
			public CardDataFormat Data { get { return SBT.Game.GameData.Card[ID]; } }
			private int mID;
			public int ID
			{
				get { return mID; }
				set
				{
					if (value == -1)
					{
						mID = -1;
						ImageCard.sprite = SBT.m_SpriteNoneCard;
						TextName.text = "(選択なし)";
					}
					else if (value >= 0 && SBT.Game.GameData.Card.Count > value)
					{
						mID = value;
						ImageCard.sprite = Resources.Load<Sprite>("Textures/Card/" + Data.ImageBack);
						TextName.text = Data.Name;
					}
				}
			}

			public CardData(Transform _transform, int _num, bool _isPlayer, SetBattleTester _setBattleTester)
			{
				SBT = _setBattleTester;
				Transform = _transform;
				Num = _num;
				IsPlayer = _isPlayer;
				ID = -1;
				Transform.GetComponent<UIEvent>().onUpLeft.AddListener(e => SBT.SelectCharaCard(this));
				Transform.GetComponent<UIEvent>().onUpRight.AddListener(e => { ID = -1; });
			}

		}
		public List<CardData> PlayerCards { get; private set; }
		public List<CardData> EnemyCards { get; private set; }
		public List<CardData> Cards { get; private set; }
		public int AreaID { get; set; }

		// Use this for initialization
		void Start()
		{
			Game = Game.GetInstance();
			PlayerUnits = new List<UnitData>();
			EnemyUnits = new List<UnitData>();
			Units = new List<UnitData>();
			PlayerCards = new List<CardData>();
			EnemyCards = new List<CardData>();
			Cards = new List<CardData>();
			for (int i = 0; i < 3; i++)
			{
				PlayerUnits.Add(new UnitData(m_TrPlayer.Find("Units").Find("Unit" + i.ToString()), i, true, this));
				EnemyUnits.Add(new UnitData(m_TrEnemy.Find("Units").Find("Unit" + i.ToString()), i, false, this));
				PlayerCards.Add(new CardData(m_TrPlayer.Find("Cards").Find("Card" + i.ToString()), i, true, this));
				EnemyCards.Add(new CardData(m_TrEnemy.Find("Cards").Find("Card" + i.ToString()), i, false, this));
			}
			AreaID = 1;
			SetSkillList();
			for (int i = 0; i < 3; i++)
			{
				Units.Add(PlayerUnits[2 - i]);
				Cards.Add(PlayerCards[i]);
			}
			for (int i = 0; i < 3; i++)
			{
				Units.Add(EnemyUnits[i]);
				Cards.Add(EnemyCards[2 - i]);
			}
		}

		public void SetSkillList()
		{
			m_DdSkill.ClearOptions();
			List<string> options = new List<string>();
			foreach (var skill in Game.GameData.Skill)
				options.Add(String.Format("{0:D3}", skill.ID) + ":" + skill.Name);
			m_DdSkill.AddOptions(options);
		}

		public void SelectCharaUnit(UnitData _unit)
		{
			m_SetUnitUI.gameObject.SetActive(true);
			m_SetUnitUI.SetUnit(_unit);
			if (_unit.ID == -1)
				m_SetUnitUI.ChangeUnit();
		}

		public void SelectCharaCard(CardData _card)
		{
			m_SetCardUI.gameObject .SetActive(true);
			m_SetCardUI.SetCard(_card);
		}

		public void SetSkill()
		{
			m_SetSkillUI.gameObject.SetActive(true);
			m_SetSkillUI.SetSkill(Game.GameData.Skill[m_DdSkill.value]);
		}

		public void SetArea()
		{
			m_SetAreaUI.gameObject.SetActive(true);
			m_SetAreaUI.SetArea();
		}

		public void Save()
		{
			var dp_Data = Application.dataPath + @"/Data";
			var dp_Backup = Application.dataPath + @"/Backup";
			// フォルダがなければ作成する
			if (Directory.Exists(dp_Data))
			{
				var now = DateTime.Now;
				var str = String.Format(@"{0:D4}{1:D2}{2:D2}_{3:D2}{4:D2}", now.Year, now.Month, now.Day, now.Hour, now.Minute);
				CopyDirectory(dp_Data, Path.Combine(dp_Backup, str));
			}
			else
				Directory.CreateDirectory(dp_Data);

			// ユニットデータ
			foreach (var unit in Units)
				if (unit.ID != -1)
					unit.Data.Level = unit.FirstLevel;
			var fp = Path.Combine(dp_Data, "unit_data.csv");
			UniCSVManager uCsv;
			if (File.Exists(fp))
				uCsv = new UniCSVManager(fp, FilePos_e.Common);
			else
				uCsv = new UniCSVManager(@"Data/unit_data", FilePos_e.FromResources);
			for (int i = 0; i < uCsv.Row; i++)
				if (uCsv[i, 0] == "")
					uCsv[i, 0] = String.Format("c{0:D3}", i);
			var uIni = new UniINIManager();
			uIni.TransformFromCSV(uCsv, 0);
			foreach (var unit in Game.GameData.Unit)
			{
				var idStr = unit.ID.ToString();
				uIni[idStr, "名前"] = unit.Name;
				uIni[idStr, "初期レベル"] = unit.Level.ToString();
				uIni[idStr, "レベル限界"] = unit.MaxLevel.ToString();
				uIni[idStr, "初期HP"] = unit.HP0.ToString();
				uIni[idStr, "HP100"] = unit.HP100.ToString();
				uIni[idStr, "初期L ATK"] = unit.LPAtk0.ToString();
				uIni[idStr, "初期L MAT"] = unit.LMAtk0.ToString();
				uIni[idStr, "初期L DEF"] = unit.LPDef0.ToString();
				uIni[idStr, "初期L MDE"] = unit.LMDef0.ToString();
				uIni[idStr, "初期G ATK"] = unit.GPAtk0.ToString();
				uIni[idStr, "初期G MAT"] = unit.GMAtk0.ToString();
				uIni[idStr, "初期G DEF"] = unit.GPDef0.ToString();
				uIni[idStr, "初期G MDE"] = unit.GMDef0.ToString();
				uIni[idStr, "L ATK100"] = unit.LPAtk100.ToString();
				uIni[idStr, "L MAT100"] = unit.LMAtk100.ToString();
				uIni[idStr, "L DEF100"] = unit.LPDef100.ToString();
				uIni[idStr, "L MDE100"] = unit.LMDef100.ToString();
				uIni[idStr, "G ATK100"] = unit.GPAtk100.ToString();
				uIni[idStr, "G MAT100"] = unit.GMAtk100.ToString();
				uIni[idStr, "G DEF100"] = unit.GPDef100.ToString();
				uIni[idStr, "G MDE100"] = unit.GMDef100.ToString();
				uIni[idStr, "初期指揮"] = unit.Lead0.ToString();
				uIni[idStr, "初期機動"] = unit.Agi0.ToString();
				uIni[idStr, "指揮100"] = unit.Lead100.ToString();
				uIni[idStr, "機動100"] = unit.Agi100.ToString();
				uIni[idStr, "初期回復力"] = unit.Cur0.ToString();
				uIni[idStr, "回復100"] = unit.Cur100.ToString();
				uIni[idStr, "兵士数"] = unit.MaxSoldierNum.ToString();
				uIni[idStr, "死亡可"] = (unit.Deathable ? "1" : "0");
                uIni[idStr, "捕獲可"] = (unit.Catchable ? "1" : "0");
				uIni[idStr, "好感度"] = unit.Love.ToString();
				uIni[idStr, "ス L ATK"] = unit.LAtkSkill.ToString();
				uIni[idStr, "ス L DEF"] = unit.LDefSkill.ToString();
				uIni[idStr, "ス G ATK"] = unit.GAtkSkill.ToString();
				uIni[idStr, "サイズ　部下"] = unit.GUnitSize.ToString();
				uIni[idStr, "装備"] = unit.Equipment.ToString();
				uIni[idStr, "AI番号"] = unit.AIID.ToString();
				uIni[idStr, "立ち絵画像名"] = unit.StandImagePath;
				uIni[idStr, "顔アイコン画像名"] = unit.FaceIamgePath;
				uIni[idStr, "戦闘リーダープレハブ"] = unit.BattleLeaderPrefabPath;
				uIni[idStr, "戦闘兵士プレハブ"] = unit.BattleGroupPrefabPath;
				uIni[idStr, "キャラ説明"] = unit.Comment;
				uIni[idStr, "死亡時セリフ"] = unit.OnDeadSerif;
				uIni[idStr, "捕獲時セリフ"] = unit.OnCapturedSerif;
				uIni[idStr, "逃走時セリフ"] = unit.OnEscapedSerif;
			}
			uCsv = uIni.TransformToUniCSV(uCsv[0].Select(key => key == "ID" ? "" : key).ToList(), "ID");
			for (int i = 0; i < uCsv.Row; i++)
				if (uCsv[i, 0].Contains("c"))
					uCsv[i, 0] = "";
			uCsv.Write(fp, FilePos_e.Common);

			// スキルデータ
			fp = Path.Combine(dp_Data, "skill_data.csv");
			if (File.Exists(fp))
				uCsv = new UniCSVManager(fp, FilePos_e.Common);
			else
				uCsv = new UniCSVManager(@"Data/skill_data", FilePos_e.FromResources);
			for (int i = 0; i < uCsv.Row; i++)
				if (uCsv[i, 0] == "")
					uCsv[i, 0] = String.Format("c{0:D3}", i);
			uIni.TransformFromCSV(uCsv, 0);
			var statusName = new List<string>{ "物功", "物防", "魔攻", "魔防", "機動", "指揮", "地形" };
			var attributeName = new List<string>{ "毒", "対ホ", "対ゾ" };
			foreach (var skill in Game.GameData.Skill)
			{
				var idStr = skill.ID.ToString();
				uIni[idStr, "名前"] = skill.Name;
				uIni[idStr, "威力"] = skill.Power.ToString();
				uIni[idStr, "スキルタイプ"] = ((int)skill.Type).ToString();
				uIni[idStr, "効果時間"] = skill.Duration.ToString();
				for (int i = 0; i < statusName.Count; i++)
					uIni[idStr, statusName[i]] = (skill.Status[i] ? "1" : "0");
				for (int i = 0; i < attributeName.Count; i++)
					uIni[idStr, attributeName[i]] = (skill.Attribute[i] ? "1" : "0");
				uIni[idStr, "ユニットID"] = skill.SummonUnit.ToString();
				uIni[idStr, "効果範囲"] = ((int)skill.Range).ToString();
				uIni[idStr, "効果対象"] = ((int)skill.Target).ToString();
				uIni[idStr, "エフェクト名"] = skill.EffectPath;
				uIni[idStr, "説明"] = skill.Description;
			}
			uCsv = uIni.TransformToUniCSV(uCsv[0].Select(key => key == "ID" ? "" : key).ToList(), "ID");
			for (int i = 0; i < uCsv.Row; i++)
				if (uCsv[i, 0].Contains("c"))
					uCsv[i, 0] = "";
			uCsv.Write(fp, FilePos_e.Common);

			// カードデータ
			fp = Path.Combine(dp_Data, "card_data.csv");
			if (File.Exists(fp))
				uCsv = new UniCSVManager(fp, FilePos_e.Common);
			else
				uCsv = new UniCSVManager(@"Data/card_data", FilePos_e.FromResources);
			for (int i = 0; i < uCsv.Row; i++)
				if (uCsv[i, 0] == "")
					uCsv[i, 0] = String.Format("c{0:D3}", i);
			uIni.TransformFromCSV(uCsv, 0);
			foreach (var card in Game.GameData.Card)
			{
				var idStr = card.ID.ToString();
				uIni[idStr, "名前"] = card.Name;
				uIni[idStr, "タイミング"] = ((int)card.Timing).ToString();
				uIni[idStr, "使用回数"] = card.Duration.ToString();
				uIni[idStr, "スキルID"] = card.SkillID.ToString();
				uIni[idStr, "画像名表"] = card.ImageFront;
				uIni[idStr, "画像名裏"] = card.ImageBack;
				uIni[idStr, "効果説明"] = card.Description;
			}
			uCsv = uIni.TransformToUniCSV(uCsv[0].Select(key => key == "ID" ? "" : key).ToList(), "ID");
			for (int i = 0; i < uCsv.Row; i++)
				if (uCsv[i, 0].Contains("c"))
					uCsv[i, 0] = "";
			uCsv.Write(fp, FilePos_e.Common);

			// AIデータ
			fp = Path.Combine(dp_Data, "ai_data.csv");
			if (File.Exists(fp))
				uCsv = new UniCSVManager(fp, FilePos_e.Common);
			else
				uCsv = new UniCSVManager(@"Data/ai_data", FilePos_e.FromResources);
			uIni.TransformFromCSV(uCsv, 0);
			for (int i = 0; i < Game.GameData.AI.Count; i++)
				uIni[i.ToString(), "Rate"] = Game.GameData.AI[i].AttackRate.ToString();
			uCsv = uIni.TransformToUniCSV(uCsv[0].Select(key => key == "ID" ? "" : key).ToList(), "ID");
			uCsv.Write(fp, FilePos_e.Common);

			// エリアデータ
			fp = Path.Combine(dp_Data, "area_data.csv");
			if (File.Exists(fp))
				uCsv = new UniCSVManager(fp, FilePos_e.Common);
			else
				uCsv = new UniCSVManager(@"Data/area_data", FilePos_e.FromResources);
			foreach (var area in Game.GameData.Area)
			{
				for (int i = 1; i < uCsv.Row; i++)
				{
					if (uCsv[i, 0] == area.ID.ToString())
					{
						uCsv[i, 1] = area.Name;
						uCsv[i, 5] = area.Mana.ToString();
						uCsv[i, 7] = area.Time.ToString();
						uCsv[i, 8] = area.BattleFactor.PAtk.ToString();
						uCsv[i, 9] = area.BattleFactor.MAtk.ToString();
						uCsv[i, 10] = area.BattleFactor.PDef.ToString();
						uCsv[i, 11] = area.BattleFactor.MDef.ToString();
						uCsv[i, 12] = area.BattleFactor.Leadership.ToString();
						uCsv[i, 13] = area.BattleFactor.Agility.ToString();
						break;
					}
				}
			}
			uCsv.Write(fp, FilePos_e.Common);

			// オプションデータ
			uIni = new UniINIManager();
			fp = Path.Combine(dp_Data, "options.ini");
			string op = "Options";
			uIni[op, "コメント"] = m_IFSave.text;
			m_IFSave.text = "";
			uIni[op, "自軍"] = Game.BattleIn.PlayerTerritory.ToString();
			uIni[op, "敵軍"] = Game.BattleIn.EnemyTerritory.ToString();
			uIni[op, "時間帯"] = Game.BattleIn.TimeOfDay.ToString();
			uIni[op, "戦闘速度"] = Game.SystemData.Config.BattleSpeed.ToString();
			uIni[op, "侵攻戦かどうか"] = Game.BattleIn.IsInvasion.ToString();
			uIni[op, "自動戦闘かどうか"] = Game.BattleIn.IsAuto.ToString();
			uIni.Write(fp, FilePos_e.Common);
		}

		public void Load()
		{
			var dp_Data = Application.dataPath + @"/Data";
			// フォルダがなければ戻る
			if (!Directory.Exists(dp_Data))
				return;

			// ユニットデータ
			var fp = Path.Combine(dp_Data, "unit_data.csv");
			var uIni = new UniINIManager();
			if (File.Exists(fp))
			{
				UniCSVManager uCsv = new UniCSVManager(fp, FilePos_e.Common);
				for (int i = 0; i < uCsv.Row; i++)
					if (uCsv[i, 0] == "")
						uCsv[i, 0] = String.Format("c{0:D3}", i);
				uIni.TransformFromCSV(uCsv, 0);
				foreach (var unit in Game.GameData.Unit)
				{
					var idStr = unit.ID.ToString();
					unit.Name = uIni[idStr, "名前"];
					unit.Level = uIni.Parse<int>(idStr, "初期レベル");
					unit.MaxLevel = uIni.Parse<int>(idStr, "レベル限界");
					unit.HP0 = uIni.Parse<int>(idStr, "初期HP");
					unit.HP100 = uIni.Parse<int>(idStr, "HP100");
					unit.LPAtk0 = uIni.Parse<int>(idStr, "初期L ATK");
					unit.LMAtk0 = uIni.Parse<int>(idStr, "初期L MAT");
					unit.LPDef0 = uIni.Parse<int>(idStr, "初期L DEF");
					unit.LMDef0 = uIni.Parse<int>(idStr, "初期L MDE");
					unit.GPAtk0 = uIni.Parse<int>(idStr, "初期G ATK");
					unit.GMAtk0 = uIni.Parse<int>(idStr, "初期G MAT");
					unit.GPDef0 = uIni.Parse<int>(idStr, "初期G DEF");
					unit.GMDef0 = uIni.Parse<int>(idStr, "初期G MDE");
					unit.LPAtk100 = uIni.Parse<int>(idStr, "L ATK100");
					unit.LMAtk100 = uIni.Parse<int>(idStr, "L MAT100");
					unit.LPDef100 = uIni.Parse<int>(idStr, "L DEF100");
					unit.LMDef100 = uIni.Parse<int>(idStr, "L MDE100");
					unit.GPAtk100 = uIni.Parse<int>(idStr, "G ATK100");
					unit.GMAtk100 = uIni.Parse<int>(idStr, "G MAT100");
					unit.GPDef100 = uIni.Parse<int>(idStr, "G DEF100");
					unit.GMDef100 = uIni.Parse<int>(idStr, "G MDE100");
					unit.Lead0 = uIni.Parse<int>(idStr, "初期指揮");
					unit.Agi0 = uIni.Parse<int>(idStr, "初期機動");
					unit.Lead100 = uIni.Parse<int>(idStr, "指揮100");
					unit.Agi100 = uIni.Parse<int>(idStr, "機動100");
					unit.Cur0 = uIni.Parse<int>(idStr, "初期回復力");
					unit.Cur100 = uIni.Parse<int>(idStr, "回復100");
					unit.MaxSoldierNum = uIni.Parse<int>(idStr, "兵士数");
					unit.Deathable = (uIni[idStr, "死亡可"] == "1");
                    unit.Catchable = (uIni[idStr, "捕獲可"] == "1");
					unit.Love = uIni.Parse<int>(idStr, "好感度");
					unit.LAtkSkill = uIni.Parse<int>(idStr, "ス L ATK");
					unit.LDefSkill = uIni.Parse<int>(idStr, "ス L DEF");
					unit.GAtkSkill = uIni.Parse<int>(idStr, "ス G ATK");
					unit.GUnitSize = uIni.Parse<int>(idStr, "サイズ　部下");
					unit.Equipment = uIni.Parse<int>(idStr, "装備");
					unit.AIID = uIni.Parse<int>(idStr, "AI番号");
					unit.StandImagePath = uIni[idStr, "立ち絵画像名"];
					unit.FaceIamgePath = uIni[idStr, "顔アイコン画像名"];
					unit.BattleLeaderPrefabPath = uIni[idStr, "戦闘リーダープレハブ"];
					unit.BattleGroupPrefabPath = uIni[idStr, "戦闘兵士プレハブ"];
					unit.Comment = uIni[idStr, "キャラ説明"];
					unit.OnDeadSerif = uIni[idStr, "死亡時セリフ"];
					unit.OnCapturedSerif = uIni[idStr, "捕獲時セリフ"];
					unit.OnEscapedSerif = uIni[idStr, "逃走時セリフ"];
				}
			}

			// スキルデータ
			fp = Path.Combine(dp_Data, "skill_data.csv");
			if (File.Exists(fp))
			{
				var uCsv = new UniCSVManager(fp, FilePos_e.Common);
				for (int i = 0; i < uCsv.Row; i++)
					if (uCsv[i, 0] == "")
						uCsv[i, 0] = String.Format("c{0:D3}", i);
				uIni.TransformFromCSV(uCsv, 0);
				var statusName = new List<string> { "物功", "物防", "魔攻", "魔防", "機動", "指揮", "地形" };
				var attributeName = new List<string> { "毒", "対ホ", "対ゾ" };
				foreach (var skill in Game.GameData.Skill)
				{
					var idStr = skill.ID.ToString();
					skill.Name = uIni[idStr, "名前"];
					skill.Power = uIni.Parse<int>(idStr, "威力");
					skill.Type = (SkillDataFormat.SkillType)uIni.Parse<int>(idStr, "スキルタイプ");
					skill.Duration = uIni.Parse<int>(idStr, "効果時間");
					for (int i = 0; i < statusName.Count; i++)
						skill.Status[i] = (uIni[idStr, statusName[i]] == "1");
					for (int i = 0; i < attributeName.Count; i++)
						skill.Attribute[i] = (uIni[idStr, attributeName[i]] == "1");
					skill.SummonUnit = uIni.Parse<int>(idStr, "ユニットID");
					skill.Range = (SkillDataFormat.SkillRange)uIni.Parse<int>(idStr, "効果範囲");
					skill.Target = (SkillDataFormat.SkillTarget)uIni.Parse<int>(idStr, "効果対象");
					skill.EffectPath = uIni[idStr, "エフェクト名"];
					skill.Description = uIni[idStr, "説明"];
				}
			}

			// カードデータ
			fp = Path.Combine(dp_Data, "card_data.csv");
			if (File.Exists(fp))
			{
				var uCsv = new UniCSVManager(fp, FilePos_e.Common);
				for (int i = 0; i < uCsv.Row; i++)
					if (uCsv[i, 0] == "")
						uCsv[i, 0] = String.Format("c{0:D3}", i);
				uIni.TransformFromCSV(uCsv, 0);
				foreach (var card in Game.GameData.Card)
				{
					var idStr = card.ID.ToString();
					card.Name = uIni[idStr, "名前"];
					card.Timing = (CardDataFormat.CardTiming)uIni.Parse<int>(idStr, "タイミング");
					card.Duration = uIni.Parse<int>(idStr, "使用回数");
					card.SkillID = uIni.Parse<int>(idStr, "スキルID");
					card.ImageFront = uIni[idStr, "画像名表"];
					card.ImageBack = uIni[idStr, "画像名裏"];
					card.Description = uIni[idStr, "効果説明"];
				}
			}

			// AIデータ
			fp = Path.Combine(dp_Data, "ai_data.csv");
			if (File.Exists(fp))
			{
				var uCsv = new UniCSVManager(fp, FilePos_e.Common);
				uIni.TransformFromCSV(uCsv, 0);
				for (int i = 0; i < Game.GameData.AI.Count; i++)
					Game.GameData.AI[i].AttackRate = uIni.Parse<float>(i.ToString(), "Rate");
			}

			// エリアデータ
			fp = Path.Combine(dp_Data, "area_data.csv");
			if (File.Exists(fp))
			{
				var uCsv = new UniCSVManager(fp, FilePos_e.Common);
				foreach (var area in Game.GameData.Area)
				{
					for (int i = 1; i < uCsv.Row; i++)
					{
						if (uCsv[i, 0] == area.ID.ToString())
						{
							area.Name = uCsv[i, 1];
							area.Mana = uCsv.Parse<int>(i, 5);
							area.Time = uCsv.Parse<int>(i, 7);
							area.BattleFactor.PAtk = uCsv.Parse<float>(i, 8);
							area.BattleFactor.MAtk = uCsv.Parse<float>(i, 9);
							area.BattleFactor.PDef = uCsv.Parse<float>(i, 10);
							area.BattleFactor.MDef = uCsv.Parse<float>(i, 11);
							area.BattleFactor.Leadership = uCsv.Parse<float>(i, 12);
							area.BattleFactor.Agility = uCsv.Parse<float>(i, 13);
							break;
						}
					}
				}
			}

			// オプションデータ
			fp = Path.Combine(dp_Data, "options.ini");
			if (File.Exists(fp))
			{
				uIni = new UniINIManager(fp, FilePos_e.Common);
				string op = "Options";
				Game.BattleIn.PlayerTerritory = uIni.Parse<int>(op, "自軍");
				Game.BattleIn.EnemyTerritory = uIni.Parse<int>(op, "敵軍");
				Game.BattleIn.TimeOfDay = uIni.Parse<int>(op, "時間帯");
				Game.SystemData.Config.BattleSpeed = uIni.Parse<int>(op, "戦闘速度");
				Game.BattleIn.IsInvasion = uIni.Parse<bool>(op, "侵攻戦かどうか");
				Game.BattleIn.IsAuto = uIni.Parse<bool>(op, "自動戦闘かどうか");
			}
		}

		public void CopyDirectory(string _sourceDirName, string _destDirName)
		{
			//コピー先のディレクトリがないときは作る
			if (!Directory.Exists(_destDirName))
			{
				Directory.CreateDirectory(_destDirName);
				//属性もコピー
				File.SetAttributes(_destDirName, File.GetAttributes(_sourceDirName));
			}

			//コピー先のディレクトリ名の末尾に"\"をつける
			if (_destDirName[_destDirName.Length - 1] !=
					Path.DirectorySeparatorChar)
				_destDirName = _destDirName + Path.DirectorySeparatorChar;

			//コピー元のディレクトリにあるファイルをコピー
			string[] files = System.IO.Directory.GetFiles(_sourceDirName);
			foreach (string file in files)
				File.Copy(file, _destDirName + Path.GetFileName(file), true);

			//コピー元のディレクトリにあるディレクトリについて、再帰的に呼び出す
			string[] dirs = Directory.GetDirectories(_sourceDirName);
			foreach (string dir in dirs)
				CopyDirectory(dir, _destDirName + Path.GetFileName(dir));
		}

		// Update is called once per frame
		void Update()
		{
			m_BuBattleStart.interactable = PlayerUnits.Any(unit => unit.ID != -1) && EnemyUnits.Any(unit => unit.ID != -1);
		}

		public IEnumerator CallBattle()
		{
			Game.ShowNowLoading();
			yield return null;
			m_BackUI.SetActive(true);
			yield return SceneManager.LoadSceneAsync(Game.SceneName_Battle, LoadSceneMode.Additive);
			gameObject.SetActive(false);
		}

		public void GoToBattle()
		{
			//データのセット
			var pUnit = PlayerUnits.Where(unit => unit.ID != -1).ToList();
			var eUnit = EnemyUnits.Where(unit => unit.ID != -1).ToList();
			var pCard = PlayerCards.Where(card => card.ID != -1).ToList();
			var eCard = EnemyCards.Where(card => card.ID != -1).ToList();
			for (int i = 0; i < 3; i++)
			{
				Game.BattleIn.PlayerUnits[i] = (pUnit.Count > i ? pUnit[i].ID : -1);
				Game.BattleIn.EnemyUnits[i] = (eUnit.Count > i ? eUnit[i].ID : -1);
				Game.BattleIn.PlayerCards[i] = (pCard.Count > i ? pCard[i].ID : -1);
				Game.BattleIn.EnemyCards[i] = (eCard.Count > i ? eCard[i].ID : -1);
			}

			Game.BattleIn.AreaID = AreaID;

			StartCoroutine(CallBattle());
		}

		public void Recovery()
		{
			foreach (var unit in Game.GameData.Unit)
			{
				unit.HP = unit.MaxHP;
				unit.SoldierNum = unit.MaxSoldierNum;
			}
			foreach (var unit in Units)
				unit.SetHPs();
		}

		public void Reset()
		{
			foreach (var unit in Units)
				unit.ID = -1;
			foreach (var card in Cards)
				card.ID = -1;
		}
	}
}
