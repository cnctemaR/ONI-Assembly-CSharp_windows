using System;
using UnityEngine;

public static class KSelectableExtensions
{
	public static string GetProperName(this Component cmp)
	{
		if (cmp.gameObject != null)
		{
			return cmp.gameObject.GetProperName();
		}
		return string.Empty;
	}

	public static string GetProperName(this GameObject go)
	{
		KSelectable component = go.GetComponent<KSelectable>();
		if (component != null)
		{
			return component.GetName();
		}
		return go.name;
	}

	public static string GetProperName(this KSelectable cmp)
	{
		return cmp.GetName();
	}
}
