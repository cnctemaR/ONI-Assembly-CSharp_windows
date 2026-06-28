using System;
using UnityEngine.EventSystems;

public class HoverCallback : KMonoBehaviour, IEventSystemHandler, IPointerEnterHandler, IPointerExitHandler
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
