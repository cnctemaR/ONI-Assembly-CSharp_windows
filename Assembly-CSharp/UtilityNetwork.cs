using System;

public class UtilityNetwork
{
	public void AddItem(int cell, FlowUtilityNetwork.IItem item)
	{
		this.AddItemInternal(cell, item);
	}

	protected virtual void AddItemInternal(int cell, FlowUtilityNetwork.IItem item)
	{
	}

	public virtual void Reset()
	{
	}

	public int id;

	public Vent.Transfer transferType;
}
