using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Structure")]
public class Structure : KMonoBehaviour
{
	public bool IsEntombed()
	{
		return this.isEntombed;
	}

	public static bool IsBuildingEntombed(Building building)
	{
		if (!Grid.IsValidCell(Grid.PosToCell(building)))
		{
			return false;
		}
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
		this.partitionerEntry = GameScenePartitioner.Instance.Add("Structure.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
		this.OnSolidChanged(null);
		base.Subscribe<Structure>(-887025858, Structure.RocketLandedDelegate);
	}

	public void UpdatePosition()
	{
		GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, this.building.GetExtents());
	}

	private void RocketChanged(object data)
	{
		this.OnSolidChanged(data);
	}

	private void OnSolidChanged(object data)
	{
		bool flag = Structure.IsBuildingEntombed(this.building);
		if (flag != this.isEntombed)
		{
			this.isEntombed = flag;
			if (this.isEntombed)
			{
				base.GetComponent<KPrefabID>().AddTag(GameTags.Entombed, false);
			}
			else
			{
				base.GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
			}
			this.operational.SetFlag(Structure.notEntombedFlag, !this.isEntombed);
			base.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.Entombed, this.isEntombed, this);
			base.Trigger(-1089732772, null);
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

	[MyCmpReq]
	private Operational operational;

	public static readonly Operational.Flag notEntombedFlag = new Operational.Flag("not_entombed", Operational.Flag.Type.Functional);

	private bool isEntombed;

	private HandleVector<int>.Handle partitionerEntry;

	private static EventSystem.IntraObjectHandler<Structure> RocketLandedDelegate = new EventSystem.IntraObjectHandler<Structure>(delegate(Structure cmp, object data)
	{
		cmp.RocketChanged(data);
	});
}
