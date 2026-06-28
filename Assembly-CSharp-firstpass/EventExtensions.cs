using System;
using UnityEngine;

public static class EventExtensions
{
	public static void Trigger(this GameObject go, int hash, object data = null)
	{
		KMonoBehaviour component = go.GetComponent<KMonoBehaviour>();
		if (component != null)
		{
			component.Trigger(hash, data);
		}
	}
}
