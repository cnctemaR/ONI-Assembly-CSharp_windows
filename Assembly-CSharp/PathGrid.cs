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
		this.NavTypeTable = new int[9];
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
		this.ProberCells = new PathGrid.ProberCell[width_in_cells * height_in_cells];
	}

	public void SetGroupProber(IGroupProber group_prober)
	{
		this.groupProber = group_prober;
	}

	public PathFinder.Cell GetCell(PathFinder.PotentialPath potential_path, int query_id)
	{
		return this.GetCell(potential_path.cell, potential_path.navType, query_id);
	}

	public bool IsCellInRange(int cell)
	{
		int num = this.OffsetCell(cell);
		return this.IsValidOffsetCell(num);
	}

	public PathFinder.Cell GetCell(int cell, NavType nav_type, int query_id)
	{
		int num = this.OffsetCell(cell);
		if (!this.IsValidOffsetCell(num))
		{
			return new PathFinder.Cell
			{
				cost = -1
			};
		}
		int num2 = this.NavTypeTable[(int)nav_type];
		int num3 = num * this.ValidNavTypes.Length + num2;
		PathFinder.Cell cell2 = this.Cells[num3];
		if (cell2.queryId == query_id)
		{
			return cell2;
		}
		return new PathFinder.Cell
		{
			cost = -1
		};
	}

	public void SetCell(PathFinder.PotentialPath potential_path, ref PathFinder.Cell cell_data)
	{
		int num = this.OffsetCell(potential_path.cell);
		if (!this.IsValidOffsetCell(num))
		{
			return;
		}
		int num2 = this.NavTypeTable[(int)potential_path.navType];
		int num3 = num * this.ValidNavTypes.Length + num2;
		this.Cells[num3] = cell_data;
		if (potential_path.navType != NavType.Tube)
		{
			PathGrid.ProberCell proberCell = this.ProberCells[num];
			if (cell_data.queryId != proberCell.queryId || cell_data.cost < proberCell.cost)
			{
				proberCell.queryId = cell_data.queryId;
				proberCell.cost = cell_data.cost;
				this.ProberCells[num] = proberCell;
				if (this.groupProber != null)
				{
					this.groupProber.SetProberCell(potential_path.cell);
				}
			}
		}
	}

	public int GetCostIgnoreProberOffset(int cell, CellOffset[] offsets, int query_id)
	{
		int num = -1;
		foreach (CellOffset cellOffset in offsets)
		{
			int num2 = Grid.OffsetCell(cell, cellOffset);
			if (Grid.IsValidCell(num2))
			{
				PathGrid.ProberCell proberCell = this.ProberCells[num2];
				if (proberCell.queryId == query_id && (num == -1 || proberCell.cost < num))
				{
					num = proberCell.cost;
				}
			}
		}
		return num;
	}

	public int GetCost(int cell, int query_id)
	{
		int num = this.OffsetCell(cell);
		if (!this.IsValidOffsetCell(num))
		{
			return -1;
		}
		int num2 = -1;
		PathGrid.ProberCell proberCell = this.ProberCells[num];
		if (proberCell.queryId == query_id)
		{
			num2 = proberCell.cost;
		}
		return num2;
	}

	private bool IsValidOffsetCell(int offset_cell)
	{
		return !this.applyOffset || -1 != offset_cell;
	}

	private int OffsetCell(int cell)
	{
		if (!this.applyOffset)
		{
			return cell;
		}
		int num;
		int num2;
		Grid.CellToXY(cell, out num, out num2);
		if (num < this.rootX || num >= this.rootX + this.widthInCells || num2 < this.rootY || num2 >= this.rootY + this.heightInCells)
		{
			return -1;
		}
		int num3 = num - this.rootX;
		int num4 = num2 - this.rootY;
		return num4 * this.widthInCells + num3;
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

	private PathGrid.ProberCell[] ProberCells;

	private NavType[] ValidNavTypes;

	private int[] NavTypeTable;

	private int widthInCells;

	private int heightInCells;

	private bool applyOffset;

	private int rootX;

	private int rootY;

	private IGroupProber groupProber;

	private struct ProberCell
	{
		public int cost;

		public int queryId;
	}
}
