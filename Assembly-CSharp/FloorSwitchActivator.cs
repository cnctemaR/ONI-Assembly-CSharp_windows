using System;

public class FloorSwitchActivator : KMonoBehaviour
{
	public PrimaryElement PrimaryElement
	{
		get
		{
			return this.primaryElement;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Register();
		this.OnCellChange(0, Grid.PosToCell(base.gameObject));
	}

	protected override void OnCleanUp()
	{
		this.Unregister();
		base.OnCleanUp();
	}

	private void OnCellChange(int previous_cell, int new_cell)
	{
		this.partitionerEntry.UpdatePosition(new_cell);
		if (Grid.IsValidCell(previous_cell) && new_cell != previous_cell)
		{
			this.NotifyChanged(previous_cell);
		}
		this.NotifyChanged(new_cell);
		this.last_cell_occupied = new_cell;
	}

	private void NotifyChanged(int cell)
	{
		GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.floorSwitchActivatorChangedLayer, this);
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.Register();
	}

	protected override void OnCmpDisable()
	{
		this.Unregister();
		base.OnCmpDisable();
	}

	private void Register()
	{
		if (!this.registered)
		{
			int num = Grid.PosToCell(this);
			this.partitionerEntry = GameScenePartitioner.Instance.Add("FloorSwitchActivator.Register", this, num, GameScenePartitioner.Instance.floorSwitchActivatorLayer, null);
			CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnCellChange), false);
			this.registered = true;
		}
	}

	private void Unregister()
	{
		if (this.registered)
		{
			this.partitionerEntry.Release();
			CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnCellChange), false);
			if (this.last_cell_occupied > -1)
			{
				this.NotifyChanged(this.last_cell_occupied);
			}
			this.registered = false;
		}
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	private bool registered = false;

	private GameScenePartitionerEntry partitionerEntry;

	private int last_cell_occupied = -1;
}
