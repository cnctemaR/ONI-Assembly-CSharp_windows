using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KSlider : Slider
{
	public event global::System.Action onReleaseHandle;

	public override void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		if (this.onReleaseHandle != null)
		{
			this.onReleaseHandle();
		}
	}

	public void ClearReleaseHandleEvent()
	{
		this.onReleaseHandle = null;
	}
}
