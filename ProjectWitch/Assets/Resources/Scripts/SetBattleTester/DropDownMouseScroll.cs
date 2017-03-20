using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Dropdown))]
public class DropDownMouseScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public Dropdown mDropDown { get { return gameObject.GetComponent<Dropdown>(); } }
	public bool IsEnter { get; private set; }

	public void OnPointerEnter(PointerEventData eventData) { IsEnter = true; }

	public void OnPointerExit(PointerEventData eventData) { IsEnter = false; }

	void Awake() { IsEnter = false; }

	void Update()
	{
		if (IsEnter)
		{
			var scroll = Input.GetAxis("Mouse ScrollWheel");
			if (scroll != 0)
			{
				if (scroll > 0)
				{
					if (mDropDown.value != 0)
						mDropDown.value--;
				}
				else
				{
					if (mDropDown.value != mDropDown.options.Count - 1)
						mDropDown.value++;
				}
			}
		}
	}
}