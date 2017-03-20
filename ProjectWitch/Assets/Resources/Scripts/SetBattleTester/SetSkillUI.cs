using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MK.Utility;

namespace ProjectWitch.BattleTester
{
	public class SetSkillUI : EventRightClick
	{
		public SkillDataFormat Skill { get; private set; }

		[SerializeField]
		private SetBattleTester m_SBT = null;
		[SerializeField]
		private Text m_TeID = null;
		[SerializeField]
		private InputField m_IFName = null;
		[SerializeField]
		private InputNumber m_INPower = null;
		[SerializeField]
		private Dropdown m_DdType = null;
		[SerializeField]
		private InputNumber m_INDuration = null, m_INSummonUnitID = null;
		[SerializeField]
		private Text m_TeSummonUnitName = null;
		[SerializeField]
		private Dropdown m_DdRange = null, m_DdTarget = null;
		[SerializeField]
		private InputField m_IFEffectPath = null, m_IFDescription = null;
		[SerializeField]
		private Toggle m_TgPA = null, m_TgPD = null, m_TgMA = null, m_TgMD = null, m_TgAgi = null, m_TgLead = null, m_TgTerrain = null;
		[SerializeField]
		private Toggle m_TgPoison = null, m_TgVSHomunculus = null, m_TgVSZombie = null;

		public void SetSkill(SkillDataFormat _skill)
		{
			Skill = _skill;
			m_TeID.text = Skill.ID.ToString();
			m_IFName.text = Skill.Name;
			m_INPower.ValueInt = Skill.Power;
			m_DdType.value = (int)Skill.Type;
			m_INDuration.ValueInt = Skill.Duration;
			m_INSummonUnitID.ValueInt = Skill.SummonUnit;
			m_INSummonUnitID.MaxInt = m_SBT.Game.GameData.Unit.Count - 1;
			m_TeSummonUnitName.text = (Skill.SummonUnit == 0 ? "(なし)" : m_SBT.Game.GameData.Unit[Skill.SummonUnit].Name);
			m_DdRange.value = (int)Skill.Range;
			m_DdTarget.value = (int)Skill.Target;
			m_IFEffectPath.text = Skill.EffectPath;
			m_IFDescription.text = Skill.Description;
			m_TgPA.isOn = Skill.Status[0];
			m_TgPD.isOn = Skill.Status[1];
			m_TgMA.isOn = Skill.Status[2];
			m_TgMD.isOn = Skill.Status[3];
			m_TgAgi.isOn = Skill.Status[4];
			m_TgLead.isOn = Skill.Status[5];
			m_TgTerrain.isOn = Skill.Status[6];
			m_TgPoison.isOn = Skill.Attribute[0];
			m_TgVSHomunculus.isOn = Skill.Attribute[1];
			m_TgVSZombie.isOn = Skill.Attribute[2];
		}

		public void ChangeName(string _str) { Skill.Name = _str; }
		public void ChangePower(int _value) { Skill.Power = _value; }
		public void ChangeType(int _value) { Skill.Type = (SkillDataFormat.SkillType)_value; }
		public void ChangeDuration(int _value) { Skill.Duration = _value; }
		public void ChangeSummonUnit(int _value)
		{
			Skill.SummonUnit = _value;
			m_TeSummonUnitName.text = (_value == 0 ? "(なし)" : m_SBT.Game.GameData.Unit[_value].Name);
		}
		public void ChangeRange(int _value) { Skill.Range = (SkillDataFormat.SkillRange)_value; }
		public void ChangeTarget(int _value) { Skill.Target = (SkillDataFormat.SkillTarget)_value; }
		public void ChangeEffectPath(string _str) { Skill.EffectPath = _str; }
		public void ChangeDescription(string _str) { Skill.Description = _str; }
		public void ChangeStatusPA(bool _value) { Skill.Status[0] = _value; }
		public void ChangeStatusPD(bool _value) { Skill.Status[1] = _value; }
		public void ChangeStatusMA(bool _value) { Skill.Status[2] = _value; }
		public void ChangeStatusMD(bool _value) { Skill.Status[3] = _value; }
		public void ChangeStatusAgi(bool _value) { Skill.Status[4] = _value; }
		public void ChangeStatusLead(bool _value) { Skill.Status[5] = _value; }
		public void ChangeStatusTerrain(bool _value) { Skill.Status[6] = _value; }
		public void ChangePoison(bool _value) { Skill.Attribute[0] = _value; }
		public void ChangeVSHomunculus(bool _value) { Skill.Attribute[1] = _value; }
		public void ChangeVSZombie(bool _value) { Skill.Attribute[2] = _value; }
	}
}

