using System;
using KSerialization;

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
		this.Subscribe(-216549700, delegate(object d)
		{
			if (!this.uprooted)
			{
				this.uprooted = true;
				this.Trigger(-216549700, null);
			}
		});
		this.position = Grid.PosToCell(base.gameObject);
		this.ground = Grid.CellBelow(this.position);
		if (Grid.IsValidCell(this.position) && Grid.IsValidCell(this.ground))
		{
			this.partitionerEntry = GameScenePartitioner.Instance.Add("UprootedMonitor.OnSpawn", base.gameObject, this.ground, GameScenePartitioner.Instance.solidChangedMask.mask, new Action<object>(this.OnGroundChanged));
		}
		this.OnGroundChanged(null);
	}

	protected override void OnCleanUp()
	{
		this.partitionerEntry.Release();
		base.OnCleanUp();
	}

	public bool CheckTileGrowable()
	{
		return !this.canBeUprooted || (!this.uprooted && this.IsCellSafe(this.position));
	}

	public bool IsCellSafe(int cell)
	{
		return CreatureHelpers.isSolidGround(Grid.CellBelow(cell));
	}

	public void OnGroundChanged(object callbackData)
	{
		if (!this.CheckTileGrowable())
		{
			this.uprooted = true;
			this.Trigger(-216549700, null);
		}
	}

	private int position;

	private int ground;

	[Serialize]
	public bool canBeUprooted = true;

	[Serialize]
	private bool uprooted;

	private GameScenePartitionerEntry partitionerEntry;
}
