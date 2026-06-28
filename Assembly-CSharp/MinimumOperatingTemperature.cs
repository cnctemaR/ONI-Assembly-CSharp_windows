using System;

[SkipSaveFileSerialization]
public class MinimumOperatingTemperature : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.TestTemperature(true);
	}

	private void SimUpdate(float dt)
	{
		this.TestTemperature(false);
	}

	private void TestTemperature(bool force)
	{
		bool flag = true;
		if (this.primaryElement.Temperature < this.minimumTemperature)
		{
			flag = false;
		}
		else
		{
			for (int i = 0; i < this.building.PlacementCells.Length; i++)
			{
				int num = this.building.PlacementCells[i];
				if (Grid.Temperature[num] < this.minimumTemperature)
				{
					flag = false;
					break;
				}
			}
		}
		if (flag != this.isWarm || force)
		{
			this.isWarm = flag;
			this.operational.SetFlag(MinimumOperatingTemperature.warmEnoughFlag, this.isWarm);
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.TooCold, !this.isWarm, this);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
			this.partitionerEntry = null;
		}
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	public float minimumTemperature = 275.15f;

	public static Operational.Flag warmEnoughFlag = new Operational.Flag("warm_enough", Operational.Flag.Type.Functional);

	private bool isWarm;

	private GameScenePartitionerEntry partitionerEntry;
}
