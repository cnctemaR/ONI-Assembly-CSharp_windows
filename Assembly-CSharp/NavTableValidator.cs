using System;

public class NavTableValidator
{
	protected bool IsClear(int cell, CellOffset[] bounding_offsets, bool is_dupe)
	{
		foreach (CellOffset cellOffset in bounding_offsets)
		{
			int num = Grid.OffsetCell(cell, cellOffset);
			if (!Grid.IsValidCell(num) || !NavTableValidator.IsCellPassable(num, is_dupe))
			{
				return false;
			}
			int num2 = Grid.CellAbove(num);
			if (Grid.IsValidCell(num2) && Grid.Element[num2].IsUnstable)
			{
				return false;
			}
		}
		return true;
	}

	protected static bool IsCellPassable(int cell, bool is_dupe)
	{
		Grid.BuildFlags buildFlags = Grid.BuildMasks[cell] & (Grid.BuildFlags.Solid | Grid.BuildFlags.DupePassable | Grid.BuildFlags.DupeImpassable | Grid.BuildFlags.CritterImpassable);
		if (buildFlags == (Grid.BuildFlags)0)
		{
			return true;
		}
		if (is_dupe)
		{
			return (buildFlags & Grid.BuildFlags.DupeImpassable) == (Grid.BuildFlags)0 && ((buildFlags & Grid.BuildFlags.Solid) == (Grid.BuildFlags)0 || (buildFlags & Grid.BuildFlags.DupePassable) > (Grid.BuildFlags)0);
		}
		return (buildFlags & (Grid.BuildFlags.Solid | Grid.BuildFlags.CritterImpassable)) == (Grid.BuildFlags)0;
	}

	public virtual void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
	{
	}

	public virtual void Clear()
	{
	}

	public Action<int> onDirty;
}
