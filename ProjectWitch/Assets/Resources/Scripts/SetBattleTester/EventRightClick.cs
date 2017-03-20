using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EventRightClick : MonoBehaviour, IPointerClickHandler
{
	[SerializeField]
	private UnityEvent m_OnRightClick = null;
	public UnityEvent onRightClick { get { return m_OnRightClick; } }

	public void OnPointerClick(PointerEventData _eventData)
	{
		if (_eventData.button == PointerEventData.InputButton.Right)
			onRightClick.Invoke();
	}
}
