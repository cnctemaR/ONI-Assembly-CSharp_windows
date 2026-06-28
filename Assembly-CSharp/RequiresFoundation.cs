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
			this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Overheatable.OnSpawn", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
			this.buildingPartitionerEntry = GameScenePartitioner.Instance.Add("Overheatable.OnSpawn", base.gameObject, validPlacementExtents, GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnSolidChanged));
			this.OnSolidChanged(null);
		}
	}

	protected override void OnCleanUp()
	{
		if (this.solidPartitionerEntry != null)
		{
			this.solidPartitionerEntry.Release();
			this.solidPartitionerEntry = null;
		}
		if (this.buildingPartitionerEntry != null)
		{
			this.buildingPartitionerEntry.Release();
			this.buildingPartitionerEntry = null;
		}
		base.OnCleanUp();
	}

	private void OnSolidChanged(object data)
	{
		SimCellOccupier component = base.GetComponent<SimCellOccupier>();
		if (!this.isBuildingDamaged && (component == null || component.IsReady()))
		{
			string text = null;
			Rotatable component2 = base.GetComponent<Rotatable>();
			Orientation orientation = ((!(component2 != null)) ? Orientation.Neutral : component2.GetOrientation());
			if (this.building.IsValidBuildLocation(base.transform.position, out text, orientation))
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

	private GameScenePartitionerEntry solidPartitionerEntry;

	private GameScenePartitionerEntry buildingPartitionerEntry;

	private bool solid = true;

	private bool isBuildingDamaged = false;

	public static Operational.Flag solidFoundation = new Operational.Flag("solid_foundation", Operational.Flag.Type.Functional);
}
