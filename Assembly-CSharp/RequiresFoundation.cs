using System;

[SkipSaveFileSerialization]
public class RequiresFoundation : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.building = base.GetComponent<Building>();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.building.Def.ContinuouslyCheckFoundation)
		{
			Extents validPlacementExtents = this.building.GetValidPlacementExtents();
			int num = 0;
			num |= GameScenePartitioner.Instance.solidChangedMask.mask;
			num |= GameScenePartitioner.Instance.objectLayerMasks[1].mask;
			this.partitionerEntry = GameScenePartitioner.Instance.Add("Overheatable.OnSpawn", base.gameObject, validPlacementExtents, num, new Action<object>(this.OnSolidChanged));
			this.OnSolidChanged(null);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.partitionerEntry != null)
		{
			this.partitionerEntry.Release();
		}
		base.OnCleanUp();
	}

	private void OnSolidChanged(object data)
	{
		SimCellOccupier component = base.GetComponent<SimCellOccupier>();
		if (!this.isBuildingDamaged && (component == null || component.IsReady()))
		{
			string text = null;
			if (this.building.IsValidBuildLocation(this.transform.position, out text))
			{
				this.UpdateSolidState(true);
			}
			else
			{
				this.UpdateSolidState(false);
			}
		}
	}

	private void UpdateSolidState(bool is_solid)
	{
		if (this.solid != is_solid)
		{
			this.solid = is_solid;
			Operational component = base.GetComponent<Operational>();
			if (component != null)
			{
				component.SetFlag(RequiresFoundation.solidFoundation, is_solid);
			}
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.MissingFoundation, !is_solid, this);
		}
	}

	private Building building;

	private GameScenePartitionerEntry partitionerEntry;

	private bool solid = true;

	private bool isBuildingDamaged;

	public static Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);
}
