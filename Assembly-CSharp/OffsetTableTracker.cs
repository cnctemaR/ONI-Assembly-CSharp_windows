using System;

public class OffsetTableTracker : OffsetTracker
{
	private static NavGrid navGrid
	{
		get
		{
			if (OffsetTableTracker.navGridImpl == null)
			{
				OffsetTableTracker.navGridImpl = Pathfinding.Instance.GetNavGrid("MinionNavGrid");
			}
			Debug.Assert(OffsetTableTracker.navGridImpl == Pathfinding.Instance.GetNavGrid("MinionNavGrid"), "Cached NavGrid reference is invalid");
			return OffsetTableTracker.navGridImpl;
		}
	}

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

	private unsafe void UpdateOffsets(int cell, CellOffset[][] table)
	{
		Debug.Assert(table.Length <= 192, string.Format("validRowIndices[{0}] isn't big enough < {1}", 192, table.Length));
		int* ptr = stackalloc int[(UIntPtr)768];
		int num = 0;
		if (Grid.IsValidCell(cell))
		{
			for (int i = 0; i < table.Length; i++)
			{
				CellOffset[] array = table[i];
				int num2 = Grid.OffsetCell(cell, array[0]);
				for (int j = 0; j < OffsetTableTracker.navGrid.ValidNavTypes.Length; j++)
				{
					NavType navType = OffsetTableTracker.navGrid.ValidNavTypes[j];
					if (navType != NavType.Tube && OffsetTableTracker.navGrid.NavTable.IsValid(num2, navType) && OffsetTableTracker.IsValidRow(cell, array))
					{
						ptr[num] = i;
						num++;
						break;
					}
				}
			}
		}
		if (this.offsets == null || this.offsets.Length != num)
		{
			this.offsets = new CellOffset[num];
		}
		for (int num3 = 0; num3 != num; num3++)
		{
			this.offsets[num3] = table[ptr[num3]][0];
		}
	}

	protected override void UpdateOffsets(int current_cell)
	{
		base.UpdateOffsets(current_cell);
		this.UpdateOffsets(current_cell, this.table);
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

	public static void OnPathfindingInvalidated()
	{
		OffsetTableTracker.navGridImpl = null;
	}

	private readonly CellOffset[][] table;

	public HandleVector<int>.Handle solidPartitionerEntry;

	public HandleVector<int>.Handle validNavCellChangedPartitionerEntry;

	private static NavGrid navGridImpl;

	private KMonoBehaviour cmp;
}
