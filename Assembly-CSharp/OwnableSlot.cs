using System;
using UnityEngine;

public class OwnableSlot : AssignableSlot
{
	public OwnableSlot(string id, string name)
		: base(id, name, true)
	{
	}

	public OwnableSlotInstance Get(Component cmp)
	{
		return this.Get(cmp.gameObject);
	}

	public OwnableSlotInstance Get(GameObject go)
	{
		return (OwnableSlotInstance)go.GetComponent<Ownables>().GetSlot(this);
	}
}
