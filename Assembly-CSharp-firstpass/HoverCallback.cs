using System;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/Plugins/HoverCallback")]
public class HoverCallback : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public void OnPointerEnter(PointerEventData data)
	{
		if (this.OnHover != null)
		{
			this.OnHover(true);
		}
	}

	public void OnPointerExit(PointerEventData data)
	{
		if (this.OnHover != null)
		{
			this.OnHover(false);
		}
	}

	public Action<bool> OnHover;
}
