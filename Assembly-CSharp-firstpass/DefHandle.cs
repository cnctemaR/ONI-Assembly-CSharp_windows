using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DefHandle
{
	public bool IsValid()
	{
		return this.defIdx > 0;
	}

	public DefType Get<DefType>() where DefType : class, new()
	{
		if (this.defIdx == 0)
		{
			DefHandle.defs.Add(new DefType());
			this.defIdx = DefHandle.defs.Count;
		}
		return DefHandle.defs[this.defIdx - 1] as DefType;
	}

	public void Set<DefType>(DefType value) where DefType : class, new()
	{
		DefHandle.defs.Add(value);
		this.defIdx = DefHandle.defs.Count;
	}

	[SerializeField]
	private int defIdx;

	private static List<object> defs = new List<object>();
}
