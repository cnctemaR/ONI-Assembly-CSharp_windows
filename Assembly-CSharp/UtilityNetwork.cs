using System;

public class UtilityNetwork
{
	public virtual void AddItem(object item)
	{
	}

	public virtual void RemoveItem(object item)
	{
	}

	public virtual void ConnectItem(object item)
	{
	}

	public virtual void DisconnectItem(object item)
	{
	}

	public virtual void Reset(UtilityNetworkGridNode[] grid)
	{
	}

	public int id;

	public ConduitType conduitType;
}
