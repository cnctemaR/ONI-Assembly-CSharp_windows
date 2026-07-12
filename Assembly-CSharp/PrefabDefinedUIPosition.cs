using System;
using UnityEngine;

public class PrefabDefinedUIPosition
{
	public void SetOn(GameObject gameObject)
	{
		if (this.position.HasValue)
		{
			gameObject.rectTransform().anchoredPosition = this.position.Value;
			return;
		}
		this.position = gameObject.rectTransform().anchoredPosition;
	}

	public void SetOn(Component component)
	{
		if (this.position.HasValue)
		{
			component.rectTransform().anchoredPosition = this.position.Value;
			return;
		}
		this.position = component.rectTransform().anchoredPosition;
	}

	private Option<Vector2> position;
}
