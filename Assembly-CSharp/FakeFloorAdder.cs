using System;

public class FakeFloorAdder : KMonoBehaviour
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (this.initiallyActive)
		{
			this.SetFloor(true);
		}
	}

	public void SetFloor(bool active)
	{
		if (this.isActive == active)
		{
			return;
		}
		int num = Grid.PosToCell(this);
		Rotatable component = base.GetComponent<Rotatable>();
		foreach (CellOffset cellOffset in this.floorOffsets)
		{
			CellOffset cellOffset2 = ((component == null) ? cellOffset : component.GetRotatedCellOffset(cellOffset));
			int num2 = Grid.OffsetCell(num, cellOffset2);
			if (active)
			{
				Grid.FakeFloor.Add(num2);
			}
			else
			{
				Grid.FakeFloor.Remove(num2);
			}
			Pathfinding.Instance.AddDirtyNavGridCell(num2);
		}
		this.isActive = active;
	}

	protected override void OnCleanUp()
	{
		this.SetFloor(false);
		base.OnCleanUp();
	}

	public CellOffset[] floorOffsets;

	public bool initiallyActive = true;

	private bool isActive;
}
