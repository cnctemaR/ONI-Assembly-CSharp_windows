using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{Id}")]
[Serializable]
public class AssignableSlot : Resource
{
	public AssignableSlot(string id, string name, bool showInUI = true)
		: base(id, name)
	{
		this.showInUI = showInUI;
	}

	public AssignableSlotInstance Lookup(GameObject go)
	{
		Assignables component = go.GetComponent<Assignables>();
		if (component != null)
		{
			return component.GetSlot(this);
		}
		return null;
	}

	public bool showInUI = true;
}
