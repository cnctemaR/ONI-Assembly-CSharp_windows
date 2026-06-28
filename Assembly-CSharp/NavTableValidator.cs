using System;

public class NavTableValidator
{
	protected bool IsClear(int cell, CellOffset[] bounding_offsets, ushort[] grid_bit_fields)
	{
		foreach (CellOffset cellOffset in bounding_offsets)
		{
			int num = Grid.OffsetCell(cell, cellOffset);
			if (!Grid.IsValidCell(num) || NavTableValidator.IsCellSolid(grid_bit_fields, num))
			{
				return false;
			}
		}
		return true;
	}

	protected static bool IsCellSolid(ushort[] grid_bit_fields, int cell)
	{
		ushort num = grid_bit_fields[cell];
		return (num & 32) != 0 && (num & 4) == 0;
	}

	public virtual void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
	{
	}

	public virtual void Clear()
	{
	}

	public Action<int> onDirty;
}
