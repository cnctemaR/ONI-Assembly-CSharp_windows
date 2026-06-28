using System;
using System.Diagnostics;
using UnityEngine.EventSystems;

public class KPointerImage : KImage, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onPointerEnter;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onPointerExit;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event global::System.Action onPointerDown;

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
