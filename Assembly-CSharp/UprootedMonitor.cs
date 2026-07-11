using System;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UprootedMonitor")]
public class UprootedMonitor : KMonoBehaviour
{
	public bool IsUprooted
	{
		get
		{
			return this.uprooted || base.GetComponent<KPrefabID>().HasTag(GameTags.Uprooted);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe<UprootedMonitor>(-216549700, UprootedMonitor.OnUprootedDelegate);
		this.position = Grid.PosToCell(base.gameObject);
		this.ground = Grid.OffsetCell(this.position, this.monitorCell);
		if (Grid.IsValidCell(this.position) && Grid.IsValidCell(this.ground))
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("UprootedMonitor.OnSpawn", base.gameObject, this.ground, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnGroundChanged));
		}
		this.OnGroundChanged(null);
	}

	protected override void OnCleanUp()
	{
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		base.OnCleanUp();
	}

	public bool CheckTileGrowable()
	{
		return !this.canBeUprooted || (!this.uprooted && this.IsCellSafe(this.position));
	}

	public bool IsCellSafe(int cell)
	{
		if (!Grid.IsCellOffsetValid(cell, this.monitorCell))
		{
			return false;
		}
		int num = Grid.OffsetCell(cell, this.monitorCell);
		return Grid.Solid[num];
	}

	public void OnGroundChanged(object callbackData)
	{
		if (!this.CheckTileGrowable())
		{
			base.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
			this.uprooted = true;
			base.Trigger(-216549700, null);
		}
	}

	public static bool IsObjectUprooted(GameObject plant)
	{
		UprootedMonitor component = plant.GetComponent<UprootedMonitor>();
		return !(component == null) && component.IsUprooted;
	}

	private int position;

	private int ground;

	[Serialize]
	public bool canBeUprooted = true;

	[Serialize]
	private bool uprooted;

	public CellOffset monitorCell = new CellOffset(0, -1);

	private HandleVector<int>.Handle partitionerEntry;

	private static readonly EventSystem.IntraObjectHandler<UprootedMonitor> OnUprootedDelegate = new EventSystem.IntraObjectHandler<UprootedMonitor>(delegate(UprootedMonitor component, object data)
	{
		if (!component.uprooted)
		{
			component.GetComponent<KPrefabID>().AddTag(GameTags.Uprooted, false);
			component.uprooted = true;
			component.Trigger(-216549700, null);
		}
	});
}
