using System;
using KSerialization;
using UnityEngine;

public class UprootedMonitor : KMonoBehaviour
{
	public bool IsUprooted
	{
		get
		{
			return this.uprooted;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(-216549700, delegate(object d)
		{
			if (!this.uprooted)
			{
				this.uprooted = true;
				base.Trigger(-216549700, null);
			}
		});
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
		return CreatureHelpers.isSolidGround(this.ground);
	}

	public void OnGroundChanged(object callbackData)
	{
		if (!this.CheckTileGrowable())
		{
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
}
