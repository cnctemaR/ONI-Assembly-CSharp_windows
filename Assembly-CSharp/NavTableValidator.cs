using System;

public class NavTableValidator
{
	protected bool IsClear(int cell, CellOffset[] bounding_offsets, ushort[] grid_bit_fields, bool allow_forcefield_traversal)
	{
		foreach (CellOffset cellOffset in bounding_offsets)
		{
			int num = Grid.OffsetCell(cell, cellOffset);
			if (!Grid.IsValidCell(num) || NavTableValidator.IsCellSolid(grid_bit_fields, num, allow_forcefield_traversal))
			{
				return false;
			}
		}
		return true;
	}

	protected static bool IsCellSolid(ushort[] grid_bit_fields, int cell, bool allow_forcefield_traversal)
	{
		ushort num = grid_bit_fields[cell];
		bool flag = (num & 32) != 0;
		bool flag2 = (num & 4) != 0;
		bool flag3 = (num & 256) != 0;
		return (flag || flag3) && (!flag2 || !allow_forcefield_traversal);
	}

	public virtual void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
	{
	}

	public virtual void Clear()
	{
	}

	public Action<int> onDirty;
}
