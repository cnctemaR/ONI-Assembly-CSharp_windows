using System;
using UnityEngine.UI;

public class KPanel : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Image image = base.GetComponent<Image>();
		if (image == null)
		{
			image = base.gameObject.AddComponent<Image>();
		}
		image.type = Image.Type.Sliced;
	}
}
