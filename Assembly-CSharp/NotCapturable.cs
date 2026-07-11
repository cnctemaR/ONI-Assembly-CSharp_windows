using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/NotCapturable")]
public class NotCapturable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (base.GetComponent<Capturable>() != null)
		{
			DebugUtil.LogErrorArgs(this, new object[] { "Entity has both Capturable and NotCapturable!" });
		}
		Components.NotCapturables.Add(this);
	}

	protected override void OnCleanUp()
	{
		Components.NotCapturables.Remove(this);
		base.OnCleanUp();
	}
}
