using System;

public class LogicUtilityNetworkLink : UtilityNetworkLink, IHaveUtilityNetworkMgr
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireConnected, null);
	}

	protected override void OnConnect(int cell1, int cell2)
	{
		Game.Instance.logicCircuitSystem.AddLink(cell1, cell2);
	}

	protected override void OnDisconnect(int cell1, int cell2)
	{
		Game.Instance.logicCircuitSystem.RemoveLink(cell1, cell2);
	}

	public IUtilityNetworkMgr GetNetworkManager()
	{
		return Game.Instance.logicCircuitSystem;
	}
}
