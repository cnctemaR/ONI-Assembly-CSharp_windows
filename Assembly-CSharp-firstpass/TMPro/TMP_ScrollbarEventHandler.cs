using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TMPro
{
	public class TMP_ScrollbarEventHandler : MonoBehaviour, IPointerClickHandler, ISelectHandler, IDeselectHandler, IEventSystemHandler
	{
		public void OnPointerClick(PointerEventData eventData)
		{
			global::Debug.Log("Scrollbar click...", null);
		}

		public void OnSelect(BaseEventData eventData)
		{
			global::Debug.Log("Scrollbar selected", null);
			this.isSelected = true;
		}

		public void OnDeselect(BaseEventData eventData)
		{
			global::Debug.Log("Scrollbar De-Selected", null);
			this.isSelected = false;
		}

		public bool isSelected;
	}
}
