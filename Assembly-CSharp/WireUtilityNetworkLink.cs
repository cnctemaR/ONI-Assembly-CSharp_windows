using System;
using UnityEngine;

public class WireUtilityNetworkLink : KMonoBehaviour, IWattageRating
{
	public Wire.WattageRating GetMaxWattageRating()
	{
		return this.maxWattageRating;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.Subscribe(774203113, new Action<object>(this.OnBuildingBroken));
		this.Subscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
		this.Connect();
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireConnected, null);
	}

	protected override void OnCleanUp()
	{
		this.Unsubscribe(774203113, new Action<object>(this.OnBuildingBroken));
		this.Unsubscribe(-1735440190, new Action<object>(this.OnBuildingFullyRepaired));
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
			Game.Instance.electricalConduitSystem.AddLink(num, num2);
			Game.Instance.circuitManager.Connect(this);
		}
	}

	private void Disconnect()
	{
		if (!this.visualizeOnly && this.connected)
		{
			this.connected = false;
			int num;
			int num2;
			this.GetCells(out num, out num2);
			Game.Instance.electricalConduitSystem.RemoveLink(num, num2);
			Game.Instance.circuitManager.Disconnect(this);
		}
	}

	public void GetCells(out int linked_cell1, out int linked_cell2)
	{
		Building component = base.GetComponent<Building>();
		if (component != null)
		{
			Orientation orientation = component.Orientation;
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.link1, orientation);
			CellOffset rotatedCellOffset2 = Rotatable.GetRotatedCellOffset(this.link2, orientation);
			int num = Grid.PosToCell(this.transform.position);
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

	[MyCmpGet]
	private Rotatable rotatable;

	[SerializeField]
	public CellOffset link1;

	[SerializeField]
	public CellOffset link2;

	[SerializeField]
	public bool visualizeOnly;

	[SerializeField]
	public Wire.WattageRating maxWattageRating;

	private bool connected;
}
