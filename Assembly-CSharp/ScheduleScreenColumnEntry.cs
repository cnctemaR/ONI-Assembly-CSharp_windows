using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScheduleScreenColumnEntry : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerDownHandler
{
	public void OnPointerEnter(PointerEventData event_data)
	{
		this.RunCallbacks();
	}

	private void RunCallbacks()
	{
		if (Input.GetMouseButton(0) && this.onLeftClick != null)
		{
			this.onLeftClick();
		}
	}

	public void OnPointerDown(PointerEventData event_data)
	{
		this.RunCallbacks();
	}

	public Image image;

	public global::System.Action onLeftClick;
}
