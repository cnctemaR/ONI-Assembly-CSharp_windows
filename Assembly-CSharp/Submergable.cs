using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Submergable")]
public class Submergable : KMonoBehaviour
{
	public bool IsSubmerged
	{
		get
		{
			return this.isSubmerged;
		}
	}

	public BuildingDef Def
	{
		get
		{
			return this.building.Def;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Submergable.OnSpawn", base.gameObject, this.building.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnElementChanged));
		this.OnElementChanged(null);
		this.operational.SetFlag(Submergable.notSubmergedFlag, this.isSubmerged);
		base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NotSubmerged, !this.isSubmerged, this);
	}

	private void OnElementChanged(object data)
	{
		bool flag = true;
		for (int i = 0; i < this.building.PlacementCells.Length; i++)
		{
			if (!Grid.IsLiquid(this.building.PlacementCells[i]))
			{
				flag = false;
				break;
			}
		}
		if (flag != this.isSubmerged)
		{
			this.isSubmerged = flag;
			this.operational.SetFlag(Submergable.notSubmergedFlag, this.isSubmerged);
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NotSubmerged, !this.isSubmerged, this);
		}
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
	}

	[MyCmpReq]
	private Building building;

	[MyCmpReq]
	private PrimaryElement primaryElement;

	[MyCmpGet]
	private SimCellOccupier simCellOccupier;

	[MyCmpReq]
	private Operational operational;

	public static Operational.Flag notSubmergedFlag = new Operational.Flag("submerged", Operational.Flag.Type.Functional);

	private bool isSubmerged;

	private HandleVector<int>.Handle partitionerEntry;
}
