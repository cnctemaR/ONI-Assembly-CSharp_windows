using System;

public class PathGrid
{
	public PathGrid(int cell_count)
	{
		this.ValidNavTypes = new NavType[7];
		this.NavTypeTable = new int[7];
		for (int i = 0; i < this.NavTypeTable.Length; i++)
		{
			this.NavTypeTable[i] = i;
			this.ValidNavTypes[i] = (NavType)i;
		}
		this.Cells = new PathFinder.Cell[cell_count * this.ValidNavTypes.Length];
		for (int j = 0; j < this.Cells.Length; j++)
		{
			this.Cells[j].cost = PathProber.InvalidCost;
		}
	}

	public void SetValidNavTypes(NavType[] valid_nav_types)
	{
		this.ValidNavTypes = valid_nav_types;
		int num = 0;
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
		this.Cells = new PathFinder.Cell[Grid.CellCount * this.ValidNavTypes.Length];
	}

	public PathFinder.Cell GetCell(int cell, NavType nav_type)
	{
		int num = this.NavTypeTable[(int)nav_type];
		int num2 = cell * this.ValidNavTypes.Length + num;
		return this.Cells[num2];
	}

	public void SetCell(int cell, NavType nav_type, ref PathFinder.Cell cell_data)
	{
		int num = this.NavTypeTable[(int)nav_type];
		int num2 = cell * this.ValidNavTypes.Length + num;
		if (this.OnCellCostChanged != null)
		{
			PathFinder.Cell cell2 = this.Cells[num2];
			if (cell2.cost != cell_data.cost)
			{
				this.OnCellCostChanged(cell, nav_type, cell2.cost, cell_data.cost);
			}
		}
		this.Cells[num2] = cell_data;
	}

	public int GetCost(int cell, int query_id)
	{
		int num = PathProber.InvalidCost;
		if (Grid.IsValidCell(cell))
		{
			foreach (NavType navType in this.ValidNavTypes)
			{
				PathFinder.Cell cell2 = this.GetCell(cell, navType);
				if (cell2.queryId == query_id && (num == PathProber.InvalidCost || cell2.cost < num))
				{
					num = cell2.cost;
				}
			}
		}
		return num;
	}

	private PathFinder.Cell[] Cells;

	private NavType[] ValidNavTypes;

	private int[] NavTypeTable;

	public Action<int, NavType, int, int> OnCellCostChanged;
}
