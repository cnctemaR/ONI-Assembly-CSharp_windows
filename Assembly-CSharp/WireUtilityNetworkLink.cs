using System;
using UnityEngine;

public class WireUtilityNetworkLink : UtilityNetworkLink, IWattageRating, IHaveUtilityNetworkMgr, IUtilityNetworkItem
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

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.electricalConduitSystem;
	}

	public ushort NetworkID
	{
		get
		{
			int num;
			int num2;
			base.GetCells(out num, out num2);
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(num) as ElectricalUtilityNetwork;
			return (electricalUtilityNetwork == null) ? ushort.MaxValue : ((ushort)electricalUtilityNetwork.id);
		}
	}

	[SerializeField]
	public Wire.WattageRating maxWattageRating;
}
