using System;

public class BuildingSubmergable : Submergable
{
	private static StatusItem GetSubmergableStatusItem()
	{
		return Db.Get().BuildingStatusItems.NotSubmerged;
	}

	protected override void OnSpawn()
	{
		this.operational.SetFlag(BuildingSubmergable.notSubmergedFlag, this.isSubmerged);
		this.GetStatusItem = new Func<StatusItem>(BuildingSubmergable.GetSubmergableStatusItem);
		base.OnSpawn();
	}

	protected override void OnSubmergedStateChanged()
	{
		this.operational.SetFlag(BuildingSubmergable.notSubmergedFlag, this.isSubmerged);
		base.OnSubmergedStateChanged();
	}

	public static Operational.Flag notSubmergedFlag = new Operational.Flag("submerged", Operational.Flag.Type.Functional);

	[MyCmpReq]
	private Operational operational;
}
