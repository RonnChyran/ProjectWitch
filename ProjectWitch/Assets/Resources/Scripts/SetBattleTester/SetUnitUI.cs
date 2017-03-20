using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MK.Utility;

namespace ProjectWitch.BattleTester
{
	public static class Expansion_SetUnitUI
	{
		public static void SetInactive(this InputNumber _IN)
		{
			_IN.ValueInt = 0;
			_IN.InputField.interactable = false;
		}

		public static void SetActive(this InputNumber _IN, int _num)
		{
			_IN.ValueInt = _num;
			_IN.InputField.interactable = true;
		}

		public static void SetInactive(this Transform _transform)
		{
			_transform.FindChild("INLv0").GetComponent<InputNumber>().SetInactive();
			_transform.FindChild("INLv100").GetComponent<InputNumber>().SetInactive();
			_transform.FindChild("TeValue").GetComponent<Text>().text = "";
		}

		public static void SetActive(this Transform _transform, int _num0, int _num100, int _num)
		{
			var INLv0 = _transform.FindChild("INLv0").GetComponent<InputNumber>();
			var INLv100 = _transform.FindChild("INLv100").GetComponent<InputNumber>();
			INLv0.MaxInt = 99999 - _num100;
			INLv100.MaxInt = 99999 - _num0;
			INLv0.SetActive(_num0);
			INLv100.SetActive(_num100);
			_transform.FindChild("TeValue").GetComponent<Text>().text = _num.ToString();
		}

		public static void SetValue(this Transform _transform, int _value)
		{
			_transform.FindChild("TeValue").GetComponent<Text>().text = _value.ToString();
		}

		public static void SetMin(this Transform _transform, int _value, int _num)
		{
			_transform.FindChild("INLv100").GetComponent<InputNumber>().MaxInt = 99999 - _value;
			_transform.SetValue(_num);
		}

		public static void SetMax(this Transform _transform, int _value, int _num)
		{
			_transform.FindChild("INLv0").GetComponent<InputNumber>().MaxInt = 99999 - _value;
			_transform.SetValue(_num);
		}

		public static void SetInactive(this InputField _if, string _str)
		{
			_if.interactable = false;
			_if.text = _str;
		}

		public static void SetActive(this InputField _if, string _str)
		{
			_if.interactable = true;
			_if.text = _str;
		}

		public static void SetInactiveSkill(this Transform _transform)
		{
			var dd = _transform.FindChild("Dropdown").GetComponent<Dropdown>();
			var button = _transform.FindChild("Button").GetComponent<Button>();
			dd.interactable = false;
			button.interactable = false;
			dd.value = 0;
		}

		public static void SetActiveSkill(this Transform _transform, SkillDataFormat _skill, UnityAction _action)
		{
			var dd = _transform.FindChild("Dropdown").GetComponent<Dropdown>();
			var button = _transform.FindChild("Button").GetComponent<Button>();
			dd.interactable = true;
			button.interactable = true;
			button.onClick.AddListener(_action);
			dd.value = _skill.ID;
		}

		public static void SetSliderValueText(this Slider _slider, int _value, int _max)
		{
			_slider.transform.FindChild("TeValue").GetComponent<Text>().text = _value.ToString() + String.Format("/{0,5}", _max);
		}

		public static void SetInactive(this Slider _slider)
		{
			_slider.interactable = false;
			_slider.SetSliderValueText(0, 0);
		}

		public static void SetActive(this Slider _slider, int _min, int _max, int _value)
		{
			_slider.interactable = true;
			_slider.SetSliderValueText(_value, _max);
			_slider.maxValue = _max;
			_slider.minValue = _min;
			_slider.value = _value;
		}
	}

	public class SetUnitUI : EventRightClick
	{
		[SerializeField]
		private SetBattleTester m_SBT = null;
		[SerializeField]
		private SelectUnitUI m_SelectUnitUI = null;
		[SerializeField]
		private SetSkillUI m_SetSkillUI = null;
		[SerializeField]
		private Button m_BuClearUnit = null;
		[SerializeField]
		private Text m_TeIsPlayer = null, m_TePosition = null, m_TeID = null;

		[SerializeField]
		private Image m_ImFace = null;
		[SerializeField]
		private InputField m_IFName = null;
		[SerializeField]
		private InputNumber m_INLevelFirst = null, m_INLevelMax = null, m_INSoldierNum = null, m_INLove = null;
		[SerializeField]
		private Toggle m_ToDeathable = null;
		[SerializeField]
		private Transform m_TrHP = null;
		[SerializeField]
		private Transform m_TrLPAtk = null, m_TrLMAtk = null, m_TrLPDef = null, m_TrLMDef = null;
		[SerializeField]
		private Transform m_TrGPAtk = null, m_TrGMAtk = null, m_TrGPDef = null, m_TrGMDef = null;
		[SerializeField]
		private Transform m_TrLeadership = null, m_TrAgility = null, m_TrCurative = null;
		[SerializeField]
		private Dropdown m_DdSize = null;
		[SerializeField]
		private InputNumber m_INEquipID = null;
		[SerializeField]
		private Text m_TeEquipName = null;
		[SerializeField]
		private InputField m_IFStand = null, m_IFFace = null, m_IFLeaderPF = null, m_IFGroupPF = null;
		[SerializeField]
		private InputField m_IFComment = null, m_IFDead = null, m_IFCaptured = null, m_IFEscape = null;
		[SerializeField]
		private Transform m_TrSkillLA = null, m_TrSkillLD = null, m_TrSkillGA = null;
		[SerializeField]
		private InputNumber m_INAIID = null, m_INAIAttackRate = null;
		[SerializeField]
		private Slider m_SlLevel = null, m_SlHP = null, m_SlSoldeirNum = null;

		public SetBattleTester.UnitData Unit { get; private set; }
		private int m_ID;
		public int ID
		{
			get { return m_ID; }
			set
			{
				if (value == -1)
				{
					m_BuClearUnit.interactable = false;

					Unit.ID = m_ID = -1;
					m_ImFace.sprite = m_SBT.SpriteNoneFace;

					m_IFName.SetInactive("(選択なし)");
					m_INLevelFirst.SetInactive();
					m_INLevelMax.SetInactive();
					m_TrHP.SetInactive();
					m_INSoldierNum.SetInactive();
					m_INLove.SetInactive();
					m_ToDeathable.isOn = true;
					m_ToDeathable.interactable = false;

					m_TrLPAtk.SetInactive();
					m_TrLMAtk.SetInactive();
					m_TrLPDef.SetInactive();
					m_TrLMDef.SetInactive();

					m_TrGPAtk.SetInactive();
					m_TrGMAtk.SetInactive();
					m_TrGPDef.SetInactive();
					m_TrGMDef.SetInactive();
					m_TrLeadership.SetInactive();
					m_TrAgility.SetInactive();
					m_TrCurative.SetInactive();
					m_DdSize.value = 0;
					m_DdSize.interactable = false;
					m_INEquipID.SetInactive();
					m_INEquipID.ValueInt = -1;
					m_TeEquipName.text = "(装備なし)";

					m_IFStand.SetInactive("");
					m_IFFace.SetInactive("");
					m_IFLeaderPF.SetInactive("");
					m_IFGroupPF.SetInactive("");

					m_IFComment.SetInactive("");
					m_IFDead.SetInactive("");
					m_IFCaptured.SetInactive("");
					m_IFEscape.SetInactive("");

					m_TrSkillLA.SetInactiveSkill();
					m_TrSkillLD.SetInactiveSkill();
					m_TrSkillGA.SetInactiveSkill();

					m_INAIID.SetInactive();
					m_INAIAttackRate.SetInactive();

					m_SlLevel.SetInactive();
					m_SlHP.SetInactive();
					m_SlSoldeirNum.SetInactive();
				}
				else if (value >= 0 && m_SBT.Game.GameData.Unit.Count > value)
				{
					m_BuClearUnit.interactable = true;

					Unit.ID = m_ID = value;
					m_IFName.SetActive(Unit.Data.Name);
					m_INLevelFirst.IsUseMax = (Unit.Data.MaxLevel != -1);
					if (m_INLevelFirst.IsUseMax)
						m_INLevelFirst.MaxInt = Unit.Data.MaxLevel;
					m_INLevelFirst.SetActive(Unit.FirstLevel);
					m_INLevelMax.SetActive(Unit.Data.MaxLevel);
					m_TrHP.SetActive(Unit.Data.HP0, Unit.Data.HP100, Unit.Data.MaxHP);
					m_INSoldierNum.SetActive(Unit.Data.MaxSoldierNum);
					m_INLove.SetActive(Unit.Data.Love);
					m_ToDeathable.isOn = Unit.Data.Deathable;
					m_ToDeathable.interactable = true;

					m_TrLPAtk.SetActive(Unit.Data.LPAtk0, Unit.Data.LPAtk100, Unit.Data.LeaderPAtk);
					m_TrLMAtk.SetActive(Unit.Data.LMAtk0, Unit.Data.LMAtk100, Unit.Data.LeaderMAtk);
					m_TrLPDef.SetActive(Unit.Data.LPDef0, Unit.Data.LPDef100, Unit.Data.LeaderPDef);
					m_TrLMDef.SetActive(Unit.Data.LMDef0, Unit.Data.LMDef100, Unit.Data.LeaderMDef);

					m_TrGPAtk.SetActive(Unit.Data.GPAtk0, Unit.Data.GPAtk100, Unit.Data.GroupPAtk);
					m_TrGMAtk.SetActive(Unit.Data.GMAtk0, Unit.Data.GMAtk100, Unit.Data.GroupMAtk);
					m_TrGPDef.SetActive(Unit.Data.GPDef0, Unit.Data.GPDef100, Unit.Data.GroupPDef);
					m_TrGMDef.SetActive(Unit.Data.GMDef0, Unit.Data.GMDef100, Unit.Data.GroupMDef);

					m_TrLeadership.SetActive(Unit.Data.Lead0, Unit.Data.Lead100, Unit.Data.Leadership);
					m_TrAgility.SetActive(Unit.Data.Agi0, Unit.Data.Agi100, Unit.Data.Agility);
					m_TrCurative.SetActive(Unit.Data.Cur0, Unit.Data.Cur100, Unit.Data.Curative);
					m_DdSize.value = Unit.Data.GUnitSize;
					m_DdSize.interactable = true;
					m_INEquipID.MaxInt = m_SBT.Game.GameData.Equipment.Count - 1;
					m_INEquipID.SetActive(Unit.Data.Equipment);
					m_TeEquipName.text = (Unit.Data.Equipment == -1 ? "(装備なし)" :
						m_SBT.Game.GameData.Equipment[Unit.Data.Equipment].Name);

					m_IFStand.SetActive(Unit.Data.StandImagePath);
					m_IFFace.SetActive(Unit.Data.FaceIamgePath);
					m_IFLeaderPF.SetActive(Unit.Data.BattleLeaderPrefabPath);
					m_IFGroupPF.SetActive(Unit.Data.BattleGroupPrefabPath);

					m_IFComment.SetActive(Unit.Data.Comment);
					m_IFDead.SetActive(Unit.Data.OnDeadSerif);
					m_IFCaptured.SetActive(Unit.Data.OnCapturedSerif);
					m_IFEscape.SetActive(Unit.Data.OnEscapedSerif);

					m_TrSkillLA.SetActiveSkill(m_SBT.Game.GameData.Skill[Unit.Data.LAtkSkill], () => SetSkill(SkillType.LA));
					m_TrSkillLD.SetActiveSkill(m_SBT.Game.GameData.Skill[Unit.Data.LDefSkill], () => SetSkill(SkillType.LD));
					m_TrSkillGA.SetActiveSkill(m_SBT.Game.GameData.Skill[Unit.Data.GAtkSkill], () => SetSkill(SkillType.GA));

					m_INAIID.SetActive(Unit.Data.AIID);
					m_INAIAttackRate.InputField.interactable = true;
					m_INAIAttackRate.ValueFloat = m_SBT.Game.GameData.AI[Unit.Data.AIID].AttackRate;

					m_SlLevel.SetActive(Unit.FirstLevel, Unit.Data.MaxLevel == -1 ? 500 : Unit.Data.MaxLevel, Unit.Data.Level);
					m_SlHP.SetActive(1, Unit.Data.MaxHP, Unit.Data.HP);
					m_SlSoldeirNum.SetActive(0, Unit.Data.MaxSoldierNum, Unit.Data.SoldierNum);

					m_ImFace.sprite = Resources.Load<Sprite>("Textures/Face/" + Unit.Data.FaceIamgePath);
					if (m_ImFace.sprite == null)
						m_ImFace.sprite = m_SBT.SpriteNoneFace;
				}
			}
		}

		void Update()
		{
			var kLeft = Input.GetKeyUp(KeyCode.LeftArrow);
			var kRight = Input.GetKeyUp(KeyCode.RightArrow);
			if(kLeft || kRight)
			{
				Unit.ID = ID;
				Unit.SetHPs();
				int id;
				for (id = 0; id < m_SBT.Units.Count; id++)
					if (Unit == m_SBT.Units[id])
						break;
				if (kLeft && !kRight)
					SetUnit(m_SBT.Units[(id + 5) % 6]);
				else if (!kLeft && kRight)
					SetUnit(m_SBT.Units[(id + 1) % 6]);

			}
		}

		public void SetUnit(SetBattleTester.UnitData _unit)
		{
			Unit = _unit;
			SetSkillDropDown();
			ID = Unit.ID;
			m_TeIsPlayer.text = (Unit.IsPlayer ? "プレイヤー" : "敵") + "陣営";
			m_TePosition.text = (Unit.Pos == 0 ? "前衛" : (Unit.Pos == 1 ? "中衛" : "後衛"));
			m_TeID.text = "ID: " + ID.ToString();
			m_INEquipID.MaxInt = m_SBT.Game.GameData.Equipment.Count - 1;
			m_INAIID.MaxInt = m_SBT.Game.GameData.AI.Count - 1;
		}

		public void ClearUnit()
		{
			Unit.ID = ID = -1;
		}

		public void Back()
		{
			Unit.ID = ID;
			Unit.SetHPs();
			gameObject.SetActive(false);
		}

		public void SetSkillDropDown()
		{
			var ddLA = m_TrSkillLA.FindChild("Dropdown").GetComponent<Dropdown>();
			var ddLD = m_TrSkillLD.FindChild("Dropdown").GetComponent<Dropdown>();
			var ddGA = m_TrSkillGA.FindChild("Dropdown").GetComponent<Dropdown>();
			ddLA.ClearOptions();
			ddLD.ClearOptions();
			ddGA.ClearOptions();
			List<string> options = new List<string>();
			foreach (var skill in m_SBT.Game.GameData.Skill)
				options.Add(skill.ID.ToString() + ": " + skill.Name);
			ddLA.AddOptions(options);
			ddLD.AddOptions(options);
			ddGA.AddOptions(options);
		}

		public enum SkillType
		{
			LA,
			LD,
			GA,
		}
		public void SetSkill(SkillType _type)
		{
			m_SetSkillUI.gameObject.SetActive(true);
			var gameData = m_SBT.Game.GameData.Skill;
			var unit = m_SBT.Game.GameData.Unit[Unit.ID];
			switch (_type)
			{
				case SkillType.LA:
					m_SetSkillUI.SetSkill(gameData[unit.LAtkSkill]);
					break;
				case SkillType.LD:
					m_SetSkillUI.SetSkill(gameData[unit.LDefSkill]);
					break;
				case SkillType.GA:
					m_SetSkillUI.SetSkill(gameData[unit.GAtkSkill]);
					break;
				default:
					break;
			}
		}

		public void ChangeUnit()
		{
			m_SelectUnitUI.gameObject.SetActive(true);
			m_SelectUnitUI.SetUnit(Unit);
		}

		public void ChangeName(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.Name = _str;
		}
		public void ChangeLevelFirst(int _value)
		{
			if (ID == -1)
				return;
			m_SlLevel.minValue = Unit.FirstLevel = _value;
			m_SlHP.maxValue = Unit.Data.MaxHP;
			m_SlLevel.SetSliderValueText(Unit.Data.Level, Unit.Data.MaxLevel);
			m_SlHP.SetSliderValueText(Unit.Data.HP, Unit.Data.MaxHP);
		}
		public void ChangeLevelMax(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.MaxLevel = m_INLevelFirst.MaxInt = _value;
			m_SlLevel.maxValue = (_value == -1 ? 500 : _value);
			if (m_SlLevel.maxValue < m_SlLevel.minValue)
				m_SlLevel.minValue = m_SlLevel.maxValue;
			m_SlHP.maxValue = Unit.Data.MaxHP;
			m_SlLevel.SetSliderValueText(Unit.Data.Level, Unit.Data.MaxLevel);
			m_SlHP.SetSliderValueText(Unit.Data.HP, Unit.Data.MaxHP);
		}
		public void ChangeHPLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.HP0 = _value;
			m_TrHP.FindChild("INLv100").GetComponent<InputNumber>().MaxInt = 999999 - _value;
			m_TrHP.FindChild("TeValue").GetComponent<Text>().text = Unit.Data.MaxHP.ToString();
			m_SlHP.maxValue = Unit.Data.MaxHP;
			m_SlHP.SetSliderValueText(Unit.Data.HP, Unit.Data.MaxHP);
		}
		public void ChangeHPLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.HP100 = _value;
			m_TrHP.FindChild("INLv0").GetComponent<InputNumber>().MaxInt = 999999 - _value;
			m_TrHP.FindChild("TeValue").GetComponent<Text>().text = Unit.Data.MaxHP.ToString();
			m_SlHP.maxValue = Unit.Data.MaxHP;
			m_SlHP.SetSliderValueText(Unit.Data.HP, Unit.Data.MaxHP);
		}
		public void ChangeSoldierNum(int _value)
		{
			if (ID == -1)
				return;
			m_SlSoldeirNum.maxValue = Unit.Data.MaxSoldierNum = _value;
			m_SlSoldeirNum.SetSliderValueText(Unit.Data.SoldierNum, Unit.Data.MaxSoldierNum);
		}
		public void ChangeLove(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.Love = _value;
		}
		public void ChangeDeathable(bool _value)
		{
			if (ID == -1)
				return;
			Unit.Data.Deathable = _value;
		}

		public void ChangeLPAtkLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LPAtk0 = _value;
			m_TrLPAtk.SetMin(_value, Unit.Data.LeaderPAtk);
		}
		public void ChangeLPAtkLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LPAtk100 = _value;
			m_TrLPAtk.SetMax(_value, Unit.Data.LeaderPAtk);
		}
		public void ChangeLMAtkLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LMAtk0 = _value;
			m_TrLMAtk.SetMin(_value, Unit.Data.LeaderMAtk);
		}
		public void ChangeLMAtkLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LMAtk100 = _value;
			m_TrLMAtk.SetMax(_value, Unit.Data.LeaderMAtk);
		}
		public void ChangeLPDefLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LPDef0 = _value;
			m_TrLPDef.SetMin(_value, Unit.Data.LeaderPDef);
		}
		public void ChangeLPDefLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LPDef100 = _value;
			m_TrLPDef.SetMax(_value, Unit.Data.LeaderPDef);
		}
		public void ChangeLMDefLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LMDef0 = _value;
			m_TrLMDef.SetMin(_value, Unit.Data.LeaderMDef);
		}
		public void ChangeLMDefLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LMDef100 = _value;
			m_TrLMDef.SetMax(_value, Unit.Data.LeaderMDef);
		}

		public void ChangeGPAtkLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GPAtk0 = _value;
			m_TrGPAtk.SetMin(_value, Unit.Data.GroupPAtk);
		}
		public void ChangeGPAtkLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GPAtk100 = _value;
			m_TrGPAtk.SetMax(_value, Unit.Data.GroupPAtk);
		}
		public void ChangeGMAtkLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GMAtk0 = _value;
			m_TrGMAtk.SetMin(_value, Unit.Data.GroupMAtk);
		}
		public void ChangeGMAtkLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GMAtk100 = _value;
			m_TrGMAtk.SetMax(_value, Unit.Data.GroupMAtk);
		}
		public void ChangeGPDefLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GPDef0 = _value;
			m_TrGPDef.SetMin(_value, Unit.Data.GroupPDef);
		}
		public void ChangeGPDefLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GPDef100 = _value;
			m_TrGPDef.SetMax(_value, Unit.Data.GroupPDef);
		}
		public void ChangeGMDefLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GMDef0 = _value;
			m_TrGMDef.SetMin(_value, Unit.Data.GroupMDef);
		}
		public void ChangeGMDefLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GMDef100 = _value;
			m_TrGMDef.SetMax(_value, Unit.Data.GroupMDef);
		}

		public void ChangeLeadershipLv0(int _value) {
			if (ID == -1)
				return;
			Unit.Data.Lead0 = _value; m_TrLeadership.SetMin(_value, Unit.Data.Leadership); }
		public void ChangeLeadershipLv100(int _value) {
			if (ID == -1)
				return;
			Unit.Data.Lead100 = _value; m_TrLeadership.SetMax(_value, Unit.Data.Leadership); }
		public void ChangeAgilityLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.Agi0 = _value; m_TrAgility.SetMin(_value, Unit.Data.Agility); }
		public void ChangeAgilityLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.Agi100 = _value; m_TrAgility.SetMax(_value, Unit.Data.Agility); }
		public void ChangeCurativeLv0(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.Cur0 = _value; m_TrCurative.SetMin(_value, Unit.Data.Curative); }
		public void ChangeCurativeLv100(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.Cur100 = _value; m_TrCurative.SetMax(_value, Unit.Data.Curative); }
		public void ChangeSize(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GUnitSize = _value; }
		public void ChangeEquipment(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.Equipment = _value;
			m_TeEquipName.text = (Unit.Data.Equipment == -1 ? "(装備なし)" : m_SBT.Game.GameData.Equipment[Unit.Data.Equipment].Name);
		}

		public void ChangeStandImagePath(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.StandImagePath = _str;
		}
		public void ChangeFaceIamgePath(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.FaceIamgePath = _str;
			m_ImFace.sprite = Resources.Load<Sprite>("Textures/Face/" + Unit.Data.FaceIamgePath);
			if (m_ImFace.sprite == null)
				m_ImFace.sprite = m_SBT.SpriteNoneFace;
		}
		public void ChangeBattleLeaderPrefabPath(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.BattleLeaderPrefabPath = _str;
		}
		public void ChangeBattleGroupPrefabPath(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.BattleGroupPrefabPath = _str;
		}

		public void ChangeComment(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.Comment = _str;
		}
		public void ChangeOnDeadSerif(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.OnDeadSerif = _str;
		}
		public void ChangeOnCapturedSerif(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.OnCapturedSerif = _str;
		}
		public void ChangeOnEscapedSerif(string _str)
		{
			if (ID == -1)
				return;
			Unit.Data.OnEscapedSerif = _str;
		}

		public void ChangeSkillLA(int _value) {
			if (ID == -1)
				return;
			Unit.Data.LAtkSkill = _value;
		}
		public void ChangeSkillLD(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.LDefSkill = _value;
		}
		public void ChangeSkillGA(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.GAtkSkill = _value;
		}

		public void ChangeAI(int _value)
		{
			if (ID == -1)
				return;
			Unit.Data.AIID = _value;
			m_INAIAttackRate.ValueFloat = m_SBT.Game.GameData.AI[_value].AttackRate;
		}

		public void ChangeAIAttackRate(float _value)
		{
			if (ID == -1)
				return;
			m_SBT.Game.GameData.AI[Unit.Data.AIID].AttackRate = _value;
		}

		public void ChangeLevel(float _value)
		{
			if (ID == -1)
				return;
			Unit.Data.Level = (int)_value;
			m_SlLevel.SetSliderValueText((int)_value, Unit.Data.MaxLevel);
			m_SlHP.maxValue = Unit.Data.MaxHP;
			m_TrHP.SetValue(Unit.Data.MaxHP);
			m_TrLPAtk.SetValue(Unit.Data.LeaderPAtk);
			m_TrLPDef.SetValue(Unit.Data.LeaderPDef);
			m_TrLMAtk.SetValue(Unit.Data.LeaderMAtk);
			m_TrLMDef.SetValue(Unit.Data.LeaderMDef);
			m_TrGPAtk.SetValue(Unit.Data.GroupPAtk);
			m_TrGPDef.SetValue(Unit.Data.GroupPDef);
			m_TrGMAtk.SetValue(Unit.Data.GroupMAtk);
			m_TrGMDef.SetValue(Unit.Data.GroupMDef);
			m_TrLeadership.SetValue(Unit.Data.Leadership);
			m_TrAgility.SetValue(Unit.Data.Agility);
			m_TrCurative.SetValue(Unit.Data.Curative);
		}

		public void ChangeHP(float _value)
		{
			if (ID == -1)
				return;
			Unit.Data.HP = (int)_value;
			m_SlHP.SetSliderValueText((int)_value, Unit.Data.MaxHP);
		}

		public void ChangeSoldierNum(float _value)
		{
			if (ID == -1)
				return;
			Unit.Data.SoldierNum = (int)_value;
			m_SlSoldeirNum.SetSliderValueText((int)_value, Unit.Data.MaxSoldierNum);
		}

		public void Recovery()
		{
			if (ID == -1)
				return;
			m_SlHP.value = Unit.Data.MaxHP;
			m_SlSoldeirNum.value = Unit.Data.MaxSoldierNum;
		}
	}
}

