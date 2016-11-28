using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace ProjectWitch.Battle
{
	public class MouseCover : MonoBehaviour, IPointerEnterHandler
	{

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}

		// マウスオーバーした時
		public void OnPointerEnter(PointerEventData e)
		{
			gameObject.SetActive(false);
		}
	}
}
