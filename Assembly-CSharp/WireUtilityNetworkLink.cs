using System;
using System.Collections.Generic;
using UnityEngine;

public class WireUtilityNetworkLink : UtilityNetworkLink, IWattageRating, IHaveUtilityNetworkMgr, IUtilityNetworkItem, IBridgedNetworkItem
{
	public Wire.WattageRating GetMaxWattageRating()
	{
		return this.maxWattageRating;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
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

	public void AddNetworks(ICollection<UtilityNetwork> networks)
	{
		int num;
		int num2;
		base.GetCells(out num, out num2);
		IUtilityNetworkMgr networkManager = this.GetNetworkManager();
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(num);
		if (networkForCell != null)
		{
			networks.Add(networkForCell);
		}
	}

	public bool IsConnectedToNetworks(ICollection<UtilityNetwork> networks)
	{
		int num;
		int num2;
		base.GetCells(out num, out num2);
		IUtilityNetworkMgr networkManager = this.GetNetworkManager();
		UtilityNetwork networkForCell = networkManager.GetNetworkForCell(num);
		return networks.Contains(networkForCell);
	}

	[SerializeField]
	public Wire.WattageRating maxWattageRating;
}
