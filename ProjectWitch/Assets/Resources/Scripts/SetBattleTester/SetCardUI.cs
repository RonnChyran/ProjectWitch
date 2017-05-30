using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MK.Utility;

namespace ProjectWitch.BattleTester
{
	public class SetCardUI : EventRightClick
	{
		[SerializeField]
		private SetBattleTester m_SBT = null;
		[SerializeField]
		private SetSkillUI m_SetSkillUI = null;
		[SerializeField]
		private Button m_BuClearCard = null;
		[SerializeField]
		private Text m_TeIsPlayer = null, m_TeNum = null, m_TeID = null;

		[SerializeField]
		private Image m_ImFront = null, m_ImBack = null;
		[SerializeField]
		private InputField m_IFName = null;
		[SerializeField]
		private Dropdown m_DdTiming = null;
		[SerializeField]
		private InputNumber m_INDuration = null;
		[SerializeField]
		private Dropdown m_DdSkill = null;
		[SerializeField]
		private Button m_BuSkill = null;
		[SerializeField]
		private InputField m_IFDescription = null, m_IFFrontImageName = null, m_IFBackImageName = null;

		[SerializeField]
		private Transform m_TrContentParent = null;
		[SerializeField]
		private GameObject m_PrefabSelectCardButton = null;
		public List<Button> Buttons { get; private set; }

		public SetBattleTester.CardData Card { get; private set; }
		public CardDataFormat Data { get { return m_SBT.Game.GameData.Card[ID]; } }
		private int m_ID;
		public int ID { get { return m_ID; } set { Card.ID = m_ID = value; ChangeDisplay(m_ID); } }
		public bool IsDisplayChange { get; private set; }

		void Awake() { Buttons = new List<Button>(); IsDisplayChange = false; }

		private void SetSkillDropDown()
		{
			m_DdSkill.ClearOptions();
			List<string> options = new List<string>();
			foreach (var skill in m_SBT.Game.GameData.Skill)
				options.Add(skill.ID.ToString() + ": " + skill.Name);
			m_DdSkill.AddOptions(options);
		}

		public void SetCard(SetBattleTester.CardData _card)
		{
			Card = _card;
			SetSkillDropDown();
			ID = Card.ID;
			m_TeIsPlayer.text = (Card.IsPlayer ? "プレイヤー" : "敵") + "陣営";
			m_TeNum.text = (Card.Num + 1).ToString() + "番目";

			foreach (var button in Buttons)
				Destroy(button.gameObject);
			Buttons.Clear();
			foreach (var card in m_SBT.Game.GameData.Card)
			{
				var button = Instantiate(m_PrefabSelectCardButton, m_TrContentParent).GetComponent<Button>();
				Buttons.Add(button);
				var rect = button.GetComponent<RectTransform>();
				var text = button.transform.Find("Text").GetComponent<Text>();
				var uiEvent = button.GetComponent<UIEvent>();
				rect.localScale = new Vector3(1, 1, 1);
				text.text = "ID:" + String.Format("{0:D3}", card.ID) + "\t\t" + card.Name;
				button.onClick.AddListener(() => { ID = card.ID; });
				uiEvent.onEnter.AddListener(e => ChangeDisplay(card.ID));
				uiEvent.onExit.AddListener(e => ChangeDisplay(Card.ID));
			}
		}

		private void ChangeDisplay(int _ID)
		{
			IsDisplayChange = true;
			if (_ID == -1)
			{
				m_BuClearCard.interactable = false;

				m_ImFront.sprite = m_SBT.SpriteNoneCard;
				m_ImBack.sprite = m_SBT.SpriteNoneCard;

				m_TeID.text = "ID: -1";
				m_IFName.SetInactive("(指定なし)");
				m_DdTiming.interactable = false;
				m_DdTiming.value = 0;
				m_INDuration.SetInactive();
				m_DdSkill.interactable = false;
				m_DdSkill.value = 0;
				m_BuSkill.interactable = false;
				m_IFDescription.SetInactive("");
				m_IFFrontImageName.SetInactive("");
				m_IFBackImageName.SetInactive("");
			}
			else if (_ID >= 0 && m_SBT.Game.GameData.Card.Count > _ID)
			{
				m_BuClearCard.interactable = true;

				var data = m_SBT.Game.GameData.Card[_ID];
				m_TeID.text = "ID: " + String.Format("{0:D3}", m_ID);
				m_IFName.SetActive(data.Name);
				m_DdTiming.interactable = true;
				m_DdTiming.value = (int)data.Timing;
				m_INDuration.SetActive(data.Duration);
				m_DdSkill.interactable = true;
				m_DdSkill.value = data.SkillID;
				m_BuSkill.interactable = true;
				m_IFDescription.SetActive(data.Description);
				m_IFFrontImageName.SetActive(data.ImageFront);
				m_IFBackImageName.SetActive(data.ImageBack);

				m_ImFront.sprite = Resources.Load<Sprite>("Textures/Card/" + data.ImageFront);
				if (m_ImFront.sprite == null)
					m_ImFront.sprite = m_SBT.SpriteNoneCard;
				m_ImBack.sprite = Resources.Load<Sprite>("Textures/Card/" + data.ImageBack);
				if (m_ImBack.sprite == null)
					m_ImBack.sprite = m_SBT.SpriteNoneCard;
			}
			IsDisplayChange = false;
		}

		public void ClearCard() { Card.ID = ID = -1; }

		public void SetSkill()
		{
			m_SetSkillUI.gameObject.SetActive(true);
			m_SetSkillUI.SetSkill(m_SBT.Game.GameData.Skill[m_SBT.Game.GameData.Card[Card.ID].SkillID]);
		}

		public void ChangeName(string _str) { Data.Name = _str; }
		public void ChangeTiming(int _value)
		{
			if (ID == -1 || IsDisplayChange)
				return;
			Data.Timing = (CardDataFormat.CardTiming)_value;
		}
		public void ChangeDuration(int _value) { Data.Duration = _value; }
		public void ChangeSkillID(int _value)
		{
			if (ID == -1 || IsDisplayChange)
				return;
			Data.SkillID = _value;
		}
		public void ChangeDescription(string _str) { Data.Description = _str; }
		public void ChangeImageFront(string _str)
		{
			Data.ImageFront = _str;
			m_ImFront.sprite = Resources.Load<Sprite>("Textures/Card/" + _str);
			if (m_ImFront.sprite == null)
				m_ImFront.sprite = m_SBT.SpriteNoneCard;
		}
		public void ChangeImageBack(string _str)
		{
			Data.ImageBack = _str;
			m_ImBack.sprite = Resources.Load<Sprite>("Textures/Card/" + _str);
			if (m_ImBack.sprite == null)
				m_ImBack.sprite = m_SBT.SpriteNoneCard;
		}
	}
}
