using System;

public class NavTableValidator
{
	protected bool IsClear(int cell, CellOffset[] bounding_offsets, bool allow_forcefield_traversal)
	{
		foreach (CellOffset cellOffset in bounding_offsets)
		{
			int num = Grid.OffsetCell(cell, cellOffset);
			if (!Grid.IsValidCell(num) || NavTableValidator.IsCellSolid(num, allow_forcefield_traversal))
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

	protected static bool IsCellSolid(int cell, bool allow_forcefield_traversal)
	{
		Grid.BuildFlags buildFlags = Grid.BuildMasks[cell] & (Grid.BuildFlags.ForceField | Grid.BuildFlags.Solid | Grid.BuildFlags.Impassable);
		return buildFlags != ~(Grid.BuildFlags.FakeFloor | Grid.BuildFlags.ForceField | Grid.BuildFlags.Foundation | Grid.BuildFlags.Solid | Grid.BuildFlags.PreviousSolid | Grid.BuildFlags.Impassable | Grid.BuildFlags.LiquidPumpFloor | Grid.BuildFlags.Door) && (byte)(buildFlags & (Grid.BuildFlags.Solid | Grid.BuildFlags.Impassable)) != 0 && ((byte)(buildFlags & Grid.BuildFlags.ForceField) == 0 || !allow_forcefield_traversal);
	}

	public virtual void UpdateCell(int cell, NavTable nav_table, CellOffset[] bounding_offsets)
	{
	}

	public virtual void Clear()
	{
	}

	public Action<int> onDirty;
}
