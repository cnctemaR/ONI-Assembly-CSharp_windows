using System;
using UnityEngine;

[Serializable]
public class TintedSprite : ISerializationCallbackReceiver
{
	public void OnAfterDeserialize()
	{
	}

	public void OnBeforeSerialize()
	{
		if (this.sprite != null)
		{
			this.name = this.sprite.name;
		}
	}

	[ReadOnly]
	public string name;

	public Sprite sprite;

	public Color color;
}
