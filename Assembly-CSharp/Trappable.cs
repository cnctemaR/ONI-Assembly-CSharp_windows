using System;

public class Trappable : KMonoBehaviour
{
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

	private void OnCellChange(int previous_cell, int current_cell)
	{
		GameScenePartitioner.Instance.TriggerEvent(current_cell, GameScenePartitioner.Instance.trapsLayer, this);
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
		CellChangeMonitor.Instance.Add(this, new Action<int, int>(this.OnCellChange), false);
		this.registered = true;
	}

	private void Unregister()
	{
		if (!this.registered)
		{
			return;
		}
		CellChangeMonitor.Instance.Remove(this, new Action<int, int>(this.OnCellChange), false);
		this.registered = false;
	}

	private bool registered;
}
