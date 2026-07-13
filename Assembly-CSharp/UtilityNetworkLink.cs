using System;
using UnityEngine;

public abstract class UtilityNetworkLink : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<UtilityNetworkLink>(774203113, UtilityNetworkLink.OnBuildingBrokenDelegate);
		base.Subscribe<UtilityNetworkLink>(-1735440190, UtilityNetworkLink.OnBuildingFullyRepairedDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Connect();
		if ((this.spawnState & UtilityNetworkLink.SpawnFlags.PendingBroken) != UtilityNetworkLink.SpawnFlags.None)
		{
			this.Disconnect();
			this.spawnState &= ~UtilityNetworkLink.SpawnFlags.PendingBroken;
		}
		if ((this.spawnState & UtilityNetworkLink.SpawnFlags.PendingRepaired) != UtilityNetworkLink.SpawnFlags.None)
		{
			this.Connect();
			this.spawnState &= ~UtilityNetworkLink.SpawnFlags.PendingRepaired;
		}
		this.spawnState &= ~UtilityNetworkLink.SpawnFlags.PendingSpawn;
		DebugUtil.DevAssert(this.spawnState == UtilityNetworkLink.SpawnFlags.None, "Failed to resolve all pending spawn actions", null);
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe<UtilityNetworkLink>(774203113, UtilityNetworkLink.OnBuildingBrokenDelegate, false);
		base.Unsubscribe<UtilityNetworkLink>(-1735440190, UtilityNetworkLink.OnBuildingFullyRepairedDelegate, false);
		this.Disconnect();
		base.OnCleanUp();
	}

	protected void Connect()
	{
		if (!this.visualizeOnly && !this.connected)
		{
			this.connected = true;
			int num;
			int num2;
			this.GetCells(out num, out num2);
			this.OnConnect(num, num2);
		}
	}

	protected virtual void OnConnect(int cell1, int cell2)
	{
	}

	protected void Disconnect()
	{
		if (!this.visualizeOnly && this.connected)
		{
			this.connected = false;
			int num;
			int num2;
			this.GetCells(out num, out num2);
			this.OnDisconnect(num, num2);
		}
	}

	protected virtual void OnDisconnect(int cell1, int cell2)
	{
	}

	public void GetCells(out int linked_cell1, out int linked_cell2)
	{
		Building component = base.GetComponent<Building>();
		if (component != null)
		{
			Orientation orientation = component.Orientation;
			int num = Grid.PosToCell(base.transform.GetPosition());
			this.GetCells(num, orientation, out linked_cell1, out linked_cell2);
			return;
		}
		linked_cell1 = -1;
		linked_cell2 = -1;
	}

	public void GetCells(int cell, Orientation orientation, out int linked_cell1, out int linked_cell2)
	{
		CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.link1, orientation);
		CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.link2, orientation);
		linked_cell1 = Grid.OffsetCell(cell, rotatedCellOffset);
		linked_cell2 = Grid.OffsetCell(cell, rotatedCellOffset2);
	}

	public bool AreCellsValid(int cell, Orientation orientation)
	{
		CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.link1, orientation);
		CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.link2, orientation);
		return Grid.IsCellOffsetValid(cell, rotatedCellOffset) && Grid.IsCellOffsetValid(cell, rotatedCellOffset2);
	}

	private void OnBuildingBroken(object data)
	{
		if ((this.spawnState & UtilityNetworkLink.SpawnFlags.PendingSpawn) != UtilityNetworkLink.SpawnFlags.None)
		{
			this.spawnState |= UtilityNetworkLink.SpawnFlags.PendingBroken;
			return;
		}
		this.Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
		if ((this.spawnState & UtilityNetworkLink.SpawnFlags.PendingSpawn) != UtilityNetworkLink.SpawnFlags.None)
		{
			this.spawnState |= UtilityNetworkLink.SpawnFlags.PendingRepaired;
			return;
		}
		this.Connect();
	}

	public int GetNetworkCell()
	{
		int num;
		int num2;
		this.GetCells(out num, out num2);
		return num;
	}

	[MyCmpGet]
	private Rotatable rotatable;

	[SerializeField]
	public CellOffset link1;

	[SerializeField]
	public CellOffset link2;

	[SerializeField]
	public bool visualizeOnly;

	private bool connected;

	private static readonly EventSystem.IntraObjectHandler<UtilityNetworkLink> OnBuildingBrokenDelegate = new EventSystem.IntraObjectHandler<UtilityNetworkLink>(delegate(UtilityNetworkLink component, object data)
	{
		component.OnBuildingBroken(data);
	});

	private static readonly EventSystem.IntraObjectHandler<UtilityNetworkLink> OnBuildingFullyRepairedDelegate = new EventSystem.IntraObjectHandler<UtilityNetworkLink>(delegate(UtilityNetworkLink component, object data)
	{
		component.OnBuildingFullyRepaired(data);
	});

	private UtilityNetworkLink.SpawnFlags spawnState = UtilityNetworkLink.SpawnFlags.PendingSpawn;

	[Flags]
	private enum SpawnFlags
	{
		None = 0,
		PendingSpawn = 1,
		PendingBroken = 2,
		PendingRepaired = 4
	}
}
