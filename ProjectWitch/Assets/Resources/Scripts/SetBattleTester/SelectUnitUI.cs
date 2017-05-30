using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MK.Utility;

namespace ProjectWitch.BattleTester
{
	public class SelectUnitUI : EventRightClick
	{
		[SerializeField]
		private SetBattleTester m_SBT = null;
		[SerializeField]
		private SetUnitUI m_SetUnitUI = null;
		[SerializeField]
		private Transform m_TrContentParent = null;
		[SerializeField]
		private GameObject m_PrefabSelectUnitButton = null;
		[SerializeField]
		private InputNumber m_INID = null;
		[SerializeField]
		private Text m_TeName = null, m_TeIsPlayer = null, m_TePosition= null;
		[SerializeField]
		private Image m_ImUnitFace = null;

		public SetBattleTester.UnitData Unit { get; private set; }
		public List<Button> Buttons { get; private set; }

		void Awake() { Buttons = new List<Button>(); }

		public void SetUnit(SetBattleTester.UnitData _unit)
		{
			Unit = _unit;
			m_TeIsPlayer.text = (Unit.IsPlayer ? "プレイヤー" : "敵") + "陣営";
			m_TePosition.text = (Unit.Pos == 0 ? "前衛" : (Unit.Pos == 1 ? "中衛" : "後衛"));
			SetUnitData(Unit.ID);
			m_INID.MaxInt = m_SBT.Game.GameData.Unit.Count - 1;
			foreach (var button in Buttons)
				Destroy(button.gameObject);
			Buttons.Clear();
			var unitIDs = m_SBT.PlayerUnits.Concat(m_SBT.EnemyUnits).Select(unit => unit.ID).Where(id => id != -1).ToList();
			foreach (var unit in m_SBT.Game.GameData.Unit)
			{
				var button = Instantiate(m_PrefabSelectUnitButton, m_TrContentParent).GetComponent<Button>();
				Buttons.Add(button);
				var rect = button.GetComponent<RectTransform>();
				var text = button.transform.Find("Text").GetComponent<Text>();
				var uiEvent = button.GetComponent<UIEvent>();
				rect.localScale = new Vector3(1, 1, 1);
				text.text = "ID:" + String.Format("{0:D3}", unit.ID) + "\t\t" + unit.Name;
				button.interactable = unitIDs.All(id => id != unit.ID);
				button.onClick.AddListener(() => { ClickUnit(unit.ID); });
				uiEvent.onEnter.AddListener(e => SetUnitData(unit.ID));
				uiEvent.onExit.AddListener(e => SetUnitData(Unit.ID));
			}
		}

		public void SetUnitData(int _unitID)
		{
			m_INID.ValueInt = _unitID;
			if (_unitID == -1)
			{
				m_TeName.text = "(選択なし)";
				m_ImUnitFace.sprite = m_SBT.SpriteNoneFace;
			}
			else
			{
				var unit = m_SBT.Game.GameData.Unit[_unitID];
				m_TeName.text = unit.Name;
				m_ImUnitFace.sprite = Resources.Load<Sprite>("Textures/Face/" + unit.FaceIamgePath);
			}
		}

		public void ClickUnit(int _unitID)
		{
			Unit.ID = _unitID;
			SetUnitData(_unitID);
			m_SetUnitUI.SetUnit(Unit);
		}

		public void ChangeID()
		{
			if (m_SBT.PlayerUnits.Concat(m_SBT.EnemyUnits).Select(unit => unit.ID).Where(id => id != -1).All(id => id != m_INID.ValueInt))
				ClickUnit(m_INID.ValueInt);
		}
	}
}

