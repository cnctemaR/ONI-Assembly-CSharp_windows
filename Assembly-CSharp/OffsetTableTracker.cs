using System;
using System.Collections.Generic;

public class OffsetTableTracker : OffsetTracker
{
	public OffsetTableTracker(CellOffset[][] table, KMonoBehaviour cmp)
	{
		this.table = table;
		this.cmp = cmp;
	}

	protected override void UpdateCell(int previous_cell, int current_cell)
	{
		if (previous_cell == current_cell)
		{
			return;
		}
		base.UpdateCell(previous_cell, current_cell);
		if (!this.solidPartitionerEntry.IsValid())
		{
			Extents extents = new Extents(current_cell, this.table);
			extents.height += 2;
			extents.y--;
			this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("OffsetTableTracker.UpdateCell", this.cmp.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnCellChanged));
			this.validNavCellChangedPartitionerEntry = GameScenePartitioner.Instance.Add("OffsetTableTracker.UpdateCell", this.cmp.gameObject, extents, GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnCellChanged));
		}
		else
		{
			GameScenePartitioner.Instance.UpdatePosition(this.solidPartitionerEntry, current_cell);
			GameScenePartitioner.Instance.UpdatePosition(this.validNavCellChangedPartitionerEntry, current_cell);
		}
		this.offsets = null;
	}

	private static bool IsValidRow(int current_cell, CellOffset[] row)
	{
		for (int i = 1; i < row.Length; i++)
		{
			int num = Grid.OffsetCell(current_cell, row[i]);
			if (!Grid.IsValidCell(num))
			{
				return false;
			}
			if (Grid.Solid[num])
			{
				return false;
			}
		}
		return true;
	}

	public static void GetOffsets(int cell, CellOffset[][] table, NavGrid nav_grid, List<CellOffset> offsets)
	{
		foreach (CellOffset[] array in table)
		{
			int num = Grid.OffsetCell(cell, array[0]);
			for (int j = 0; j < nav_grid.ValidNavTypes.Length; j++)
			{
				NavType navType = nav_grid.ValidNavTypes[j];
				if (navType != NavType.Tube && nav_grid.NavTable.IsValid(num, navType) && OffsetTableTracker.IsValidRow(cell, array))
				{
					offsets.Add(array[0]);
					break;
				}
			}
		}
	}

	protected override void UpdateOffsets(int current_cell)
	{
		base.UpdateOffsets(current_cell);
		if (!Grid.IsValidCell(current_cell))
		{
			return;
		}
		if (this.navGrid == null)
		{
			this.navGrid = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
		}
		this.newOffsets.Clear();
		OffsetTableTracker.GetOffsets(current_cell, this.table, this.navGrid, this.newOffsets);
		if (this.offsets != null && this.newOffsets.Count == this.offsets.Length)
		{
			this.newOffsets.CopyTo(this.offsets);
		}
		else
		{
			this.offsets = this.newOffsets.ToArray();
		}
	}

	private void OnCellChanged(object data)
	{
		this.offsets = null;
	}

	public override void Clear()
	{
		GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
		GameScenePartitioner.Instance.Free(ref this.validNavCellChangedPartitionerEntry);
	}

	private CellOffset[][] table;

	public HandleVector<int>.Handle solidPartitionerEntry;

	public HandleVector<int>.Handle validNavCellChangedPartitionerEntry;

	private NavGrid navGrid;

	private KMonoBehaviour cmp;

	private List<CellOffset> newOffsets = new List<CellOffset>();
}
