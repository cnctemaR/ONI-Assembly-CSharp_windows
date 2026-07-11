using System;
using UnityEngine.EventSystems;

public class KPointerImage : KImage, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	public event global::System.Action onPointerEnter;

	public event global::System.Action onPointerExit;

	public event global::System.Action onPointerDown;

	public event global::System.Action onPointerUp;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (this.onPointerEnter != null)
		{
			this.onPointerEnter();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (this.onPointerExit != null)
		{
			this.onPointerExit();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (this.onPointerDown != null)
		{
			this.onPointerDown();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (this.onPointerUp != null)
		{
			this.onPointerUp();
		}
	}

	public void ClearPointerEvents()
	{
		this.onPointerEnter = null;
		this.onPointerExit = null;
		this.onPointerDown = null;
		this.onPointerUp = null;
	}
}
