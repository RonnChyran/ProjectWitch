using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MK.Utility
{
	public class UIEvent : MonoBehaviour,
	IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler,
	IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[Serializable]
		public class EventPointerData : UnityEvent<PointerEventData> { }
		public PointerEventData PointerEventData { get; private set; }

		void Awake() { IsOnOver = false; }

		void Update()
		{
			if (IsOnOver)
			{
				onOver.Invoke(PointerEventData);
				if (IsPressLeft) onPressLeft.Invoke(PointerEventData);
				if (IsPressRight) onPressRight.Invoke(PointerEventData);
				if (IsPressMiddle) onPressMiddle.Invoke(PointerEventData);
			}
		}

		#region マウスクリックされた時
		[SerializeField]
		private EventPointerData m_OnClickLeft = new EventPointerData();
		public EventPointerData onClickLeft { get { return m_OnClickLeft; } }
		[SerializeField]
		private EventPointerData m_OnClickRight = new EventPointerData();
		public EventPointerData onClickRight { get { return m_OnClickRight; } }
		[SerializeField]
		private EventPointerData m_OnClickMiddle = new EventPointerData();
		public EventPointerData onClickMiddle { get { return m_OnClickMiddle; } }
		public void OnPointerClick(PointerEventData _eventData)
		{
			switch (_eventData.button)
			{
				case PointerEventData.InputButton.Left:
					onClickLeft.Invoke(_eventData);
					break;
				case PointerEventData.InputButton.Right:
					onClickRight.Invoke(_eventData);
					break;
				case PointerEventData.InputButton.Middle:
					onClickMiddle.Invoke(_eventData);
					break;
				default:
					break;
			}
		}
		#endregion

		#region 範囲に入った
		[SerializeField]
		private EventPointerData m_OnEnter = new EventPointerData();
		public EventPointerData onEnter { get { return m_OnEnter; } }
		public void OnPointerEnter(PointerEventData _eventData) {
			IsOnOver = true;
			PointerEventData = _eventData;
			onEnter.Invoke(_eventData);
		}
		#endregion

		#region 範囲から出た
		[SerializeField]
		private EventPointerData m_OnExit = new EventPointerData();
		public EventPointerData onExit { get { return m_OnExit; } }
		public void OnPointerExit(PointerEventData _eventData) { IsOnOver = false; onExit.Invoke(_eventData); }
		#endregion

		#region マウスオーバー中
		public bool IsOnOver { get; private set; }
		[SerializeField]
		private EventPointerData m_OnOver = new EventPointerData();
		public EventPointerData onOver { get { return m_OnOver; } }
		#endregion

		#region マウスクリック中
		public bool IsPressLeft { get { return Input.GetMouseButton(0); } }
		[SerializeField]
		private EventPointerData m_OnPressLeft = new EventPointerData();
		public EventPointerData onPressLeft { get { return m_OnPressLeft; } }
		public bool IsPressRight { get { return Input.GetMouseButton(1); } }
		[SerializeField]
		private EventPointerData m_OnPressRight = new EventPointerData();
		public EventPointerData onPressRight { get { return m_OnPressRight; } }
		public bool IsPressMiddle { get { return Input.GetMouseButton(2); } }
		[SerializeField]
		private EventPointerData m_OnPressMiddle = new EventPointerData();
		public EventPointerData onPressMiddle { get { return m_OnPressMiddle; } }
		#endregion

		#region マウスボタンを押した
		[SerializeField]
		private EventPointerData m_OnDownLeft = new EventPointerData();
		public EventPointerData onDownLeft { get { return m_OnDownLeft; } }
		[SerializeField]
		private EventPointerData m_OnDownRight = new EventPointerData();
		public EventPointerData onDownRight { get { return m_OnDownRight; } }
		[SerializeField]
		private EventPointerData m_OnDownMiddle = new EventPointerData();
		public EventPointerData onDownMiddle { get { return m_OnDownMiddle; } }
		public void OnPointerDown(PointerEventData _eventData)
		{
			switch (_eventData.button)
			{
				case PointerEventData.InputButton.Left:
					onDownLeft.Invoke(_eventData);
					break;
				case PointerEventData.InputButton.Right:
					onDownRight.Invoke(_eventData);
					break;
				case PointerEventData.InputButton.Middle:
					onDownMiddle.Invoke(_eventData);
					break;
				default:
					break;
			}
		}
		#endregion

		#region マウスボタンを離した
		[SerializeField]
		private EventPointerData m_OnUpLeft = new EventPointerData();
		public EventPointerData onUpLeft { get { return m_OnUpLeft; } }
		[SerializeField]
		private EventPointerData m_OnUpRight = new EventPointerData();
		public EventPointerData onUpRight { get { return m_OnUpRight; } }
		[SerializeField]
		private EventPointerData m_OnUpMiddle = new EventPointerData();
		public EventPointerData onUpMiddle { get { return m_OnUpMiddle; } }
		public void OnPointerUp(PointerEventData _eventData)
		{
			switch (_eventData.button)
			{
				case PointerEventData.InputButton.Left:
					onUpLeft.Invoke(_eventData);
					break;
				case PointerEventData.InputButton.Right:
					onUpRight.Invoke(_eventData);
					break;
				case PointerEventData.InputButton.Middle:
					onUpMiddle.Invoke(_eventData);
					break;
				default:
					break;
			}
		}
		#endregion

		#region ドラッグが開始された時
		[SerializeField]
		private EventPointerData m_OnBeginDrag = new EventPointerData();
		public EventPointerData onBeginDrag { get { return m_OnBeginDrag; } }
		public void OnBeginDrag(PointerEventData _eventData) { onBeginDrag.Invoke(_eventData); }
		#endregion

		#region ドラッグ中
		[SerializeField]
		private EventPointerData m_OnDrag = new EventPointerData();
		public EventPointerData onDrag { get { return m_OnDrag; } }
		public void OnDrag(PointerEventData _eventData) { onDrag.Invoke(_eventData); }
		#endregion

		#region ドラッグを終了した時
		[SerializeField]
		private EventPointerData m_OnEndDrag = new EventPointerData();
		public EventPointerData onEndDrag { get { return m_OnEndDrag; } }
		public void OnEndDrag(PointerEventData _eventData) { onEndDrag.Invoke(_eventData); }
		#endregion
	}
}
