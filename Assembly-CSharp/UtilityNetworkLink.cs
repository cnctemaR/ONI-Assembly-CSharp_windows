using System;
using UnityEngine;

public abstract class UtilityNetworkLink : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		base.Subscribe(774203113, new Action<object>(this.OnBuildingBroken));
		base.Subscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
		this.Connect();
	}

	protected override void OnCleanUp()
	{
		base.Unsubscribe(774203113, new Action<object>(this.OnBuildingBroken));
		base.Unsubscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
		this.Disconnect();
		base.OnCleanUp();
	}

	private void Connect()
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

	private void Disconnect()
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
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.link1, orientation);
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.link2, orientation);
			int num = Grid.PosToCell(base.transform.GetPosition());
			linked_cell1 = Grid.OffsetCell(num, rotatedCellOffset);
			linked_cell2 = Grid.OffsetCell(num, rotatedCellOffset2);
		}
		else
		{
			linked_cell1 = -1;
			linked_cell2 = -1;
		}
	}

	private void OnBuildingBroken(object data)
	{
		this.Disconnect();
	}

	private void OnBuildingFullyRepaired(object data)
	{
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
}
