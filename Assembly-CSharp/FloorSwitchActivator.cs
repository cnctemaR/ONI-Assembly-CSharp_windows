using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FloorSwitchActivator")]
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
		this.OnCellChange();
	}

	protected override void OnCleanUp()
	{
		this.Unregister();
		base.OnCleanUp();
	}

	private void OnCellChange()
	{
		int num = Grid.PosToCell(this);
		GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, num);
		if (Grid.IsValidCell(this.last_cell_occupied) && num != this.last_cell_occupied)
		{
			this.NotifyChanged(this.last_cell_occupied);
		}
		this.NotifyChanged(num);
		this.last_cell_occupied = num;
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
		if (this.registered)
		{
			return;
		}
		int num = Grid.PosToCell(this);
		this.partitionerEntry = GameScenePartitioner.Instance.Add("FloorSwitchActivator.Register", this, num, GameScenePartitioner.Instance.floorSwitchActivatorLayer, null);
		Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange), "FloorSwitchActivator.Register");
		this.registered = true;
	}

	private void Unregister()
	{
		if (!this.registered)
		{
			return;
		}
		GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
		Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(base.transform, new global::System.Action(this.OnCellChange));
		if (this.last_cell_occupied > -1)
		{
			this.NotifyChanged(this.last_cell_occupied);
		}
		this.registered = false;
	}

	[MyCmpReq]
	private PrimaryElement primaryElement;

	private bool registered;

	private HandleVector<int>.Handle partitionerEntry;

	private int last_cell_occupied = -1;
}
