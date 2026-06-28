using System;
using UnityEngine;

public class UtilityNetworkLink : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		int num;
		int num2;
		this.GetCells(out num, out num2);
		Game.Instance.electricalConduitSystem.AddLink(num, num2);
		base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.WireConnected, null);
	}

	protected override void OnCleanUp()
	{
		int num;
		int num2;
		this.GetCells(out num, out num2);
		Game.Instance.electricalConduitSystem.RemoveLink(num, num2);
		base.OnCleanUp();
	}

	private void GetCells(out int cell, out int linked_cell)
	{
		Building component = base.GetComponent<Building>();
		if (component != null)
		{
			Orientation orientation = component.Orientation;
			CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(this.link, orientation);
			cell = Grid.PosToCell(this.transform.position);
			linked_cell = Grid.OffsetCell(cell, rotatedCellOffset);
		}
		else
		{
			cell = -1;
			linked_cell = -1;
		}
	}

	[SerializeField]
	public CellOffset link;
}
