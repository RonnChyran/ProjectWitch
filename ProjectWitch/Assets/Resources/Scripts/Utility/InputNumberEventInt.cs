//author	: masanori.k
//version	: 1.0.0
//summary	: InputNumberのイベント拡張、Int型版

using System;
using UnityEngine;
using UnityEngine.Events;

namespace MK.Utility
{
	[RequireComponent(typeof(InputNumber))]
	public class InputNumberEventInt : MonoBehaviour
	{
		[Serializable]
		public class EventInt : UnityEvent<int> { }

		// 数値が変更されたとき
		[SerializeField]
		private EventInt m_OnValueChanged = new EventInt();
		public EventInt onValueChanged { get { return m_OnValueChanged; } }
		private UnityAction<string> m_UnityActionValueChanged;

		// 変更が終了したとき
		[SerializeField]
		private EventInt m_OnEventEndEdit = new EventInt();
		public EventInt onEventEndEdit { get { return m_OnEventEndEdit; } }
		private UnityAction<string> m_UnityActionEndEdit;

		// InputNumber参照
		public InputNumber InputNumber { get { return gameObject.GetComponent<InputNumber>(); } }

		void Awake()
		{
			m_UnityActionValueChanged = (str => { if (InputNumber.Type == InputNumber.NumberType.Integer) m_OnValueChanged.Invoke(InputNumber.ValueInt); });
			m_UnityActionEndEdit = (str => { if (InputNumber.Type == InputNumber.NumberType.Integer) m_OnEventEndEdit.Invoke(InputNumber.ValueInt); });
			AddListenerValueChanged();
			AddListenerEndEdit();
		}

		// 数値が変更されたときのリスナーを登録
		public void AddListenerValueChanged()
		{
			InputNumber.InputField.onValueChanged.RemoveListener(m_UnityActionValueChanged);
			InputNumber.InputField.onValueChanged.AddListener(m_UnityActionValueChanged);
		}

		// 数値の変更が終了したときのリスナーを登録
		public void AddListenerEndEdit()
		{
			InputNumber.InputField.onEndEdit.RemoveListener(m_UnityActionEndEdit);
			InputNumber.InputField.onEndEdit.AddListener(m_UnityActionEndEdit);
		}
	}
}
