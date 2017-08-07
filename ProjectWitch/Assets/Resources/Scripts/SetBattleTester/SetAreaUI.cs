using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MK.Utility;

namespace ProjectWitch.BattleTester
{
	public class SetAreaUI : EventRightClick
	{
		[SerializeField]
		private SetBattleTester m_SBT = null;
		[SerializeField]
		private Text m_TeID = null;

		[SerializeField]
		private InputField m_IFName = null;
		[SerializeField]
		private InputNumber m_INMana = null, m_INTime = null;
		[SerializeField]
		private InputNumber m_INPA = null, m_INPD = null, m_INMA = null, m_INMD = null, m_INLead = null, m_INAgi = null;

		[SerializeField]
		private Dropdown m_DdPlayer = null, m_DdEnemy = null, m_DdDayTime = null, m_DdSpeed = null;
		[SerializeField]
		private Toggle m_TgIsInvasion = null, m_TgIsAuto = null;

		[SerializeField]
		private Transform m_TrContentParent = null;
		[SerializeField]
		private GameObject m_PrefabSelectButton = null;
		public List<Button> Buttons { get; private set; }

		public int AreaID { get { return m_SBT.AreaID; } set { m_SBT.AreaID = value; } }
		public AreaDataFormat Area { get { return m_SBT.Game.GameData.Area[AreaID]; } }
		public int ID
		{
			get { return AreaID; }
			set
			{
				if (value >= 0 && m_SBT.Game.GameData.Area.Count > value)
				{
					AreaID = value;

					SetAreaData(value);
				}
			}
		}

		void Awake() { Buttons = new List<Button>(); }

		public void SetArea()
		{
			m_DdPlayer.ClearOptions();
			m_DdEnemy.ClearOptions();
			List<string> options = new List<string>();
			foreach (var territory in m_SBT.Game.GameData.Territory)
				options.Add(territory.ID.ToString() + ": " + territory.OwnerName);
			m_DdPlayer.AddOptions(options);
			m_DdEnemy.AddOptions(options);

			ID = AreaID;

			foreach (var button in Buttons)
				Destroy(button.gameObject);
			Buttons.Clear();
			foreach (var area in m_SBT.Game.GameData.Area)
			{
				if (area.ID == 0)
					continue;
				var button = Instantiate(m_PrefabSelectButton, m_TrContentParent).GetComponent<Button>();
				Buttons.Add(button);
				var rect = button.GetComponent<RectTransform>();
				var text = button.transform.Find("Text").GetComponent<Text>();
				var uiEvent = button.GetComponent<UIEvent>();
				rect.localScale = new Vector3(1, 1, 1);
				text.text = "ID:" + String.Format("{0:D3}", area.ID) + "\t\t" + area.Name;
				button.onClick.AddListener(() => { ID = area.ID; });
				uiEvent.onEnter.AddListener(e => SetAreaData(area.ID));
				uiEvent.onExit.AddListener(e => SetAreaData(AreaID));
			}
		}

		private void SetAreaData(int _value)
		{
			m_TeID.text = "ID: " + String.Format("{0:D3}", _value);
			var area = m_SBT.Game.GameData.Area[_value];
			m_IFName.text = area.Name;
			m_INMana.ValueInt = area.Mana;
			m_INTime.ValueInt = area.Time;
			m_INPA.ValueFloat = area.BattleFactor.PAtk;
			m_INPD.ValueFloat = area.BattleFactor.PDef;
			m_INMA.ValueFloat = area.BattleFactor.MAtk;
			m_INMD.ValueFloat = area.BattleFactor.MDef;
			m_INLead.ValueFloat = area.BattleFactor.Leadership;
			m_INAgi.ValueFloat = area.BattleFactor.Agility;

			m_DdPlayer.value = m_SBT.Game.BattleIn.PlayerTerritory;
			m_DdEnemy.value = m_SBT.Game.BattleIn.EnemyTerritory;
			m_DdDayTime.value = m_SBT.Game.BattleIn.TimeOfDay;
			m_DdSpeed.value = m_SBT.Game.SystemData.Config.BattleSpeed;
			m_TgIsInvasion.isOn = m_SBT.Game.BattleIn.IsInvasion;
			m_TgIsAuto.isOn = m_SBT.Game.BattleIn.IsAuto;
		}

		public void ChangeName(string _str) { Area.Name = _str; }
		public void ChangeMana(int _value) { Area.Mana = _value; }
		public void ChangeTime(int _value) { Area.Time = _value; }

		public void ChangePAtk(float _value) { Area.BattleFactor.PAtk = _value; }
		public void ChangePDef(float _value) { Area.BattleFactor.PDef = _value; }
		public void ChangeMAtk(float _value) { Area.BattleFactor.MAtk = _value; }
		public void ChangeMDef(float _value) { Area.BattleFactor.MDef = _value; }
		public void ChangeLeadership(float _value) { Area.BattleFactor.Leadership = _value; }
		public void ChangeAgility(float _value) { Area.BattleFactor.Agility = _value; }

		public void ChangePlayerTerritory(int _value) { m_SBT.Game.BattleIn.PlayerTerritory = _value; }
		public void ChangeEnemyTerritory(int _value) { m_SBT.Game.BattleIn.EnemyTerritory = _value; }
		public void ChangeTimeOfDay(int _value) { m_SBT.Game.BattleIn.TimeOfDay = _value; }
		public void ChangeBattleSpeed(int _value) { m_SBT.Game.SystemData.Config.BattleSpeed = _value; }
		public void ChangeIsInvasion(bool _value) { m_SBT.Game.BattleIn.IsInvasion = _value; }
		public void ChangeIsAuto(bool _value) { m_SBT.Game.BattleIn.IsAuto = _value; }
	}
}

