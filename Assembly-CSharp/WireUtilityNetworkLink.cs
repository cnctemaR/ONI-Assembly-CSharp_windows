using System;
using UnityEngine;

public class WireUtilityNetworkLink : UtilityNetworkLink, IWattageRating, IWire
{
	public Wire.WattageRating GetMaxWattageRating()
	{
		return this.maxWattageRating;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireConnected, null);
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.electricalConduitSystem.RemoveLink(cell1, cell2);
		Game.Instance.circuitManager.Disconnect(this);
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		Game.Instance.electricalConduitSystem.AddLink(cell1, cell2);
		Game.Instance.circuitManager.Connect(this);
	}

	public IUtilityNetworkMgr GetNetworkMgr()
	{
		return Game.Instance.electricalConduitSystem;
	}

	[SerializeField]
	public Wire.WattageRating maxWattageRating;
}
