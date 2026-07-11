using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Floodable")]
public class Floodable : KMonoBehaviour
{
	public bool IsFlooded
	{
		get
		{
			return this.isFlooded;
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
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Floodable.OnSpawn", base.gameObject, this.building.GetExtents(), GameScenePartitioner.Instance.liquidChangedLayer, new Action<object>(this.OnElementChanged));
		this.OnElementChanged(null);
	}

	private void OnElementChanged(object data)
	{
		bool flag = false;
		for (int i = 0; i < this.building.PlacementCells.Length; i++)
		{
			if (Grid.IsSubstantialLiquid(this.building.PlacementCells[i], 0.35f))
			{
				flag = true;
				break;
			}
		}
		if (flag != this.isFlooded)
		{
			this.isFlooded = flag;
			this.operational.SetFlag(Floodable.notFloodedFlag, !this.isFlooded);
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Flooded, this.isFlooded, this);
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

	public static Operational.Flag notFloodedFlag = new Operational.Flag("not_flooded", Operational.Flag.Type.Functional);

	private bool isFlooded;

	private HandleVector<int>.Handle partitionerEntry;
}
