using System;
using UnityEngine;

public static class KSelectableExtensions
{
	public static string GetProperName(this Component cmp)
	{
		string text;
		if (cmp != null && cmp.gameObject != null)
		{
			text = cmp.gameObject.GetProperName();
		}
		else
		{
			text = "";
		}
		return text;
	}

	public static string GetProperName(this GameObject go)
	{
		if (go != null)
		{
			KSelectable component = go.GetComponent<KSelectable>();
			if (component != null)
			{
				return component.GetName();
			}
		}
		return "";
	}

	public static string GetProperName(this KSelectable cmp)
	{
		string text;
		if (cmp != null)
		{
			text = cmp.GetName();
		}
		else
		{
			text = "";
		}
		return text;
	}
}
