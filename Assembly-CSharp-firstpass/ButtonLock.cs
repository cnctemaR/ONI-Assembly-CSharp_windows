using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonLock : MonoBehaviour, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		this.target.SendMessage("ToggleLock", SendMessageOptions.DontRequireReceiver);
	}

	public void OnDrag(PointerEventData eventData)
	{
		this.target.SendMessage("OnDrag", SendMessageOptions.DontRequireReceiver);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		this.target.SendMessage("Lock", true, SendMessageOptions.DontRequireReceiver);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		this.target.SendMessage("Lock", false, SendMessageOptions.DontRequireReceiver);
	}

	public GameObject target;
}
