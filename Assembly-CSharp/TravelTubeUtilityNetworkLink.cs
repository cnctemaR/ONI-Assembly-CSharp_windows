using System;

public class TravelTubeUtilityNetworkLink : UtilityNetworkLink, IWire
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		Game.Instance.travelTubeSystem.AddLink(cell1, cell2);
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.travelTubeSystem.RemoveLink(cell1, cell2);
	}

	public IUtilityNetworkMgr GetNetworkMgr()
	{
		return Game.Instance.travelTubeSystem;
	}
}
