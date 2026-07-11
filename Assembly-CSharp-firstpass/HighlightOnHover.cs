using System;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/Plugins/HighlightOnHover")]
public class HighlightOnHover : KMonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
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
