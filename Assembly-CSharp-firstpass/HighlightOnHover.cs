using System;
using UnityEngine.EventSystems;

public class HighlightOnHover : KMonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	public void OnPointerEnter(PointerEventData data)
	{
		this.image.ColorState = KImage.ColorSelector.Hover;
	}

	public void OnPointerExit(PointerEventData data)
	{
		this.image.ColorState = KImage.ColorSelector.Inactive;
	}

	public KImage image;
}
