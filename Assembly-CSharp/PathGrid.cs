using System;

public class PathGrid
{
	public PathGrid(int width_in_cells, int height_in_cells, bool apply_offset, NavType[] valid_nav_types)
	{
		this.applyOffset = apply_offset;
		this.widthInCells = width_in_cells;
		this.heightInCells = height_in_cells;
		this.ValidNavTypes = valid_nav_types;
		int num = 0;
		this.NavTypeTable = new int[8];
		for (int i = 0; i < this.NavTypeTable.Length; i++)
		{
			this.NavTypeTable[i] = -1;
			for (int j = 0; j < this.ValidNavTypes.Length; j++)
			{
				if (this.ValidNavTypes[j] == (NavType)i)
				{
					this.NavTypeTable[i] = num++;
					break;
				}
			}
		}
		this.Cells = new PathFinder.Cell[width_in_cells * height_in_cells * this.ValidNavTypes.Length];
	}

	public PathFinder.Cell GetCell(PathFinder.PotentialPath potential_path)
	{
		return this.GetCell(potential_path.cell, potential_path.navType);
	}

	public PathFinder.Cell GetCell(int cell, NavType nav_type)
	{
		int num = this.OffsetCell(cell);
		PathFinder.Cell cell2;
		if (!this.IsValidOffsetCell(num))
		{
			cell2 = new PathFinder.Cell
			{
				cost = PathProber.InvalidCost
			};
		}
		else
		{
			int num2 = this.NavTypeTable[(int)nav_type];
			int num3 = num * this.ValidNavTypes.Length + num2;
			cell2 = this.Cells[num3];
		}
		return cell2;
	}

	public void SetCell(PathFinder.PotentialPath potential_path, ref PathFinder.Cell cell_data)
	{
		int num = this.OffsetCell(potential_path.cell);
		if (this.IsValidOffsetCell(num))
		{
			int num2 = this.NavTypeTable[(int)potential_path.navType];
			int num3 = num * this.ValidNavTypes.Length + num2;
			this.Cells[num3] = cell_data;
		}
	}

	public int GetCost(int cell, int query_id)
	{
		int num = PathProber.InvalidCost;
		if (Grid.IsValidCell(cell))
		{
			for (int i = 0; i < this.ValidNavTypes.Length; i++)
			{
				NavType navType = this.ValidNavTypes[i];
				PathFinder.Cell cell2 = this.GetCell(cell, navType);
				if (cell2.queryId == query_id)
				{
					if (num == PathProber.InvalidCost || cell2.cost < num)
					{
						num = cell2.cost;
					}
				}
			}
		}
		return num;
	}

	private bool IsValidOffsetCell(int offset_cell)
	{
		return !this.applyOffset || PathProber.InvalidCell != offset_cell;
	}

	private int OffsetCell(int cell)
	{
		int num3;
		if (this.applyOffset)
		{
			int num;
			int num2;
			Grid.CellToXY(cell, out num, out num2);
			if (num < this.rootX || num >= this.rootX + this.widthInCells || num2 < this.rootY || num2 >= this.rootY + this.heightInCells)
			{
				num3 = PathProber.InvalidCell;
			}
			else
			{
				int num4 = num - this.rootX;
				int num5 = num2 - this.rootY;
				num3 = num5 * this.widthInCells + num4;
			}
		}
		else
		{
			num3 = cell;
		}
		return num3;
	}

	public void SetRootCell(int root_cell)
	{
		if (this.applyOffset)
		{
			Grid.CellToXY(root_cell, out this.rootX, out this.rootY);
			this.rootX -= this.widthInCells / 2;
			this.rootY -= this.heightInCells / 2;
		}
	}

	private PathFinder.Cell[] Cells;

	private NavType[] ValidNavTypes;

	private int[] NavTypeTable;

	private int widthInCells;

	private int heightInCells;

	private bool applyOffset;

	private int rootX;

	private int rootY;
}
