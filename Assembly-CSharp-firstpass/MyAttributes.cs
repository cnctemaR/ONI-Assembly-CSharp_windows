using System;
using System.Collections.Generic;

public static class MyAttributes
{
	public static void Register(IAttributeManager mgr)
	{
		MyAttributes.s_attributeMgrs.Add(mgr);
	}

	public static void OnAwake(object obj, KMonoBehaviour cmp)
	{
		foreach (IAttributeManager attributeManager in MyAttributes.s_attributeMgrs)
		{
			attributeManager.OnAwake(obj, cmp);
		}
	}

	public static void OnAwake(KMonoBehaviour c)
	{
		MyAttributes.OnAwake(c, c);
	}

	public static void OnStart(object obj, KMonoBehaviour cmp)
	{
		foreach (IAttributeManager attributeManager in MyAttributes.s_attributeMgrs)
		{
			attributeManager.OnStart(obj, cmp);
		}
	}

	public static void OnStart(KMonoBehaviour c)
	{
		MyAttributes.OnStart(c, c);
	}

	private static List<IAttributeManager> s_attributeMgrs = new List<IAttributeManager>();
}
