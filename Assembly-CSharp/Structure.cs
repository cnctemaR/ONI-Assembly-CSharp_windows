using System;

public class Structure : KMonoBehaviour
{
	public bool IsEntombed()
	{
		return this.isEntombed;
	}

	public static bool IsBuildingEntombed(Building building)
	{
		for (int i = 0; i < building.PlacementCells.Length; i++)
		{
			int num = building.PlacementCells[i];
			if (Grid.Element[num].IsSolid && !Grid.Foundation[num])
			{
				return true;
			}
		}
		return false;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Extents extents = this.building.GetExtents();
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Structure.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedMask.mask, new Action<object>(this.OnSolidChanged));
		this.OnSolidChanged(null);
	}

	private void OnSolidChanged(object data)
	{
		bool flag = Structure.IsBuildingEntombed(this.building);
		if (flag != this.isEntombed)
		{
			this.isEntombed = flag;
			this.operational.SetFlag(Structure.notEntombedFlag, !this.isEntombed);
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Entombed, this.isEntombed, this);
			GameHashes gameHashes = ((!this.isEntombed) ? GameHashes.EntombedExited : GameHashes.EntombedEntered);
			this.Trigger((int)gameHashes, null);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
	}

	[MyCmpAdd]
	private Notifier notifier;

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpReq]
	private Operational operational;

	public static Operational.Flag notEntombedFlag = new Operational.Flag("not_entombed", Operational.Flag.Type.Functional);

	private bool isEntombed;

	private GameScenePartitionerEntry partitionerEntry;
}
