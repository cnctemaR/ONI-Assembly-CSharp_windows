using System;
using KSerialization;
using UnityEngine;

public class Wire : KMonoBehaviour, IDisconnectable
{
	public bool IsConnected
	{
		get
		{
			int num = Grid.PosToCell(this.transform.position);
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(num) as ElectricalUtilityNetwork;
			return electricalUtilityNetwork != null;
		}
	}

	public ushort NetworkID
	{
		get
		{
			int num = Grid.PosToCell(this.transform.position);
			ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkForCell(num) as ElectricalUtilityNetwork;
			if (electricalUtilityNetwork == null)
			{
				return ushort.MaxValue;
			}
			return (ushort)electricalUtilityNetwork.id;
		}
	}

	protected override void OnSpawn()
	{
		int num = Grid.PosToCell(this.transform.position);
		bool flag = false;
		GameObject gameObject = Grid.Objects[num, 3];
		if (gameObject != null)
		{
			Switch component = gameObject.GetComponent<Switch>();
			if (component != null)
			{
				flag = component.IsSwitchedOn;
			}
		}
		if (flag)
		{
			this.Connect();
		}
		else
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireNominal, null);
		}
	}

	protected override void OnCleanUp()
	{
		this.Disconnect();
		base.OnCleanUp();
	}

	public void Connect()
	{
		int num = Grid.PosToCell(this.transform.position);
		if (this.cachedConnections != (UtilityConnections)0)
		{
			UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
			electricalConduitSystem.SetConnections(this.cachedConnections, num, true);
			electricalConduitSystem.AddToNetworks(num, null);
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireConnected, null);
	}

	public void Disconnect()
	{
		int num = Grid.PosToCell(this.transform.position);
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		if (component.Def.ReplacementLayer == ObjectLayer.NumLayers || Grid.Objects[num, (int)component.Def.ReplacementLayer] == null)
		{
			UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
			this.cachedConnections = electricalConduitSystem.GetConnections(num, true);
			electricalConduitSystem.RemoveFromNetworks(num, null);
		}
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireDisconnected, null);
	}

	public UtilityConnections GetWireConnections()
	{
		int num = Grid.PosToCell(this.transform.position);
		return Game.Instance.electricalConduitSystem.GetConnections(num, true);
	}

	public string GetWireConnectionsString()
	{
		UtilityConnections wireConnections = this.GetWireConnections();
		return Game.Instance.electricalConduitSystem.GetVisualizerString(wireConnections);
	}

	[Serialize]
	private UtilityConnections cachedConnections;
}
