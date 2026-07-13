using System;
using System.Collections.Generic;

public class PathGrid
{
	public ulong AllocatedClassification
	{
		get
		{
			DebugUtil.Assert(this.widthInCells < 65535);
			DebugUtil.Assert(this.heightInCells < 65535);
			DebugUtil.Assert(this.ValidNavTypes.Length < 256);
			return (ulong)((((long)this.widthInCells << 16) + (long)this.heightInCells << 8) + (long)this.ValidNavTypes.Length);
		}
	}

	public ushort SerialNo
	{
		get
		{
			return this.serialNo;
		}
	}

	public PathGrid(PathGrid other)
		: this(other.widthInCells, other.heightInCells, other.applyOffset, other.ValidNavTypes)
	{
	}

	public PathGrid(int width_in_cells, int height_in_cells, bool apply_offset, NavType[] valid_nav_types)
	{
		this.applyOffset = apply_offset;
		this.widthInCells = width_in_cells;
		this.heightInCells = height_in_cells;
		this.ValidNavTypes = valid_nav_types;
		int num = 0;
		this.NavTypeTable = new int[11];
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
		DebugUtil.DevAssert(true, "Cell packs nav type into 4 bits!", null);
		this.Cells = new PathFinder.Cell[width_in_cells * height_in_cells * this.ValidNavTypes.Length];
		this.ProberCells = new PathGrid.ProberCell[width_in_cells * height_in_cells];
	}

	public void CloneNavTypes(PathGrid other)
	{
		DebugUtil.Assert(other.ValidNavTypes.Length == this.ValidNavTypes.Length);
		other.ValidNavTypes.CopyTo(this.ValidNavTypes, 0);
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
	}

	public void OnCleanUp()
	{
	}

	public void BeginUpdate(ushort new_serial_no, int root_cell, List<int> found_cells_list = null)
	{
		this.freshlyOccupiedCells = found_cells_list;
		if (this.applyOffset)
		{
			Grid.CellToXY(root_cell, out this.rootX, out this.rootY);
			this.rootX -= this.widthInCells / 2;
			this.rootY -= this.heightInCells / 2;
		}
		this.serialNo = new_serial_no;
	}

	public void EndUpdate()
	{
		this.freshlyOccupiedCells = null;
	}

	private bool IsValidSerialNo(ushort serialNo)
	{
		return serialNo == this.serialNo && serialNo > 0;
	}

	public PathFinder.Cell GetCell(PathFinder.PotentialPath potential_path, out bool is_cell_in_range)
	{
		return this.GetCell(potential_path.cell, potential_path.navType, out is_cell_in_range);
	}

	public PathFinder.Cell GetCell(int cell, NavType nav_type, out bool is_cell_in_range)
	{
		int num = this.OffsetCell(cell);
		is_cell_in_range = -1 != num;
		if (!is_cell_in_range)
		{
			return PathGrid.InvalidCell;
		}
		if ((int)nav_type >= this.NavTypeTable.Length)
		{
			return PathGrid.InvalidCell;
		}
		if (num * this.ValidNavTypes.Length + this.NavTypeTable[(int)nav_type] >= this.Cells.Length)
		{
			return PathGrid.InvalidCell;
		}
		PathFinder.Cell cell2 = this.Cells[num * this.ValidNavTypes.Length + this.NavTypeTable[(int)nav_type]];
		if (!this.IsValidSerialNo(cell2.queryId))
		{
			return PathGrid.InvalidCell;
		}
		return cell2;
	}

	private PathGrid.ProberCell GetProberCell(int cell)
	{
		int num = this.OffsetCell(cell);
		if (num == -1)
		{
			return PathGrid.InvalidProberCell;
		}
		return this.ProberCells[num];
	}

	public void SetCell(PathFinder.PotentialPath potential_path, ref PathFinder.Cell cell_data)
	{
		int num = this.OffsetCell(potential_path.cell);
		if (-1 == num)
		{
			return;
		}
		cell_data.queryId = this.serialNo;
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
				proberCell.navType = potential_path.navType;
				this.ProberCells[num] = proberCell;
				List<int> list = this.freshlyOccupiedCells;
				if (list == null)
				{
					return;
				}
				list.Add(potential_path.cell);
			}
		}
	}

	public int GetCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		int num = -1;
		foreach (CellOffset cellOffset in offsets)
		{
			int num2 = Grid.OffsetCell(cell, cellOffset);
			if (Grid.IsValidCell(num2))
			{
				PathGrid.ProberCell proberCell = this.ProberCells[num2];
				if (this.IsValidSerialNo(proberCell.queryId) && (num == -1 || proberCell.cost < num))
				{
					num = proberCell.cost;
				}
			}
		}
		return num;
	}

	public int GetCost(int cell)
	{
		int num = this.OffsetCell(cell);
		if (-1 == num)
		{
			return -1;
		}
		PathGrid.ProberCell proberCell = this.ProberCells[num];
		if (!this.IsValidSerialNo(proberCell.queryId))
		{
			return -1;
		}
		return proberCell.cost;
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
		return (num2 - this.rootY) * this.widthInCells + num3;
	}

	public bool BuildPath(int source_cell, int target_cell, NavType current_nav_type, ref PathFinder.Path path)
	{
		if (path.nodes != null)
		{
			path.nodes.Clear();
		}
		path.cost = -1;
		if (target_cell == PathFinder.InvalidCell || this.GetCost(target_cell) == -1)
		{
			return false;
		}
		bool flag = false;
		PathGrid.ProberCell proberCell = this.GetProberCell(target_cell);
		PathFinder.Cell cell = this.GetCell(target_cell, proberCell.navType, out flag);
		path.Clear();
		path.cost = cell.cost;
		while (target_cell != PathFinder.InvalidCell)
		{
			path.AddNode(new PathFinder.Path.Node
			{
				cell = target_cell,
				navType = cell.navType,
				transitionId = cell.transitionId
			});
			if (target_cell == source_cell && cell.navType == current_nav_type)
			{
				path.nodes.Reverse();
				return true;
			}
			if (target_cell != PathFinder.InvalidCell)
			{
				target_cell = cell.parent;
				cell = this.GetCell(target_cell, cell.parentNavType, out flag);
			}
		}
		path.Clear();
		return false;
	}

	private PathFinder.Cell[] Cells;

	private PathGrid.ProberCell[] ProberCells;

	private List<int> freshlyOccupiedCells;

	private NavType[] ValidNavTypes;

	private int[] NavTypeTable;

	private int widthInCells;

	private int heightInCells;

	private bool applyOffset;

	private int rootX;

	private int rootY;

	private ushort serialNo;

	public static readonly PathFinder.Cell InvalidCell = new PathFinder.Cell
	{
		cost = -1,
		parent = -1
	};

	private static readonly PathGrid.ProberCell InvalidProberCell = new PathGrid.ProberCell
	{
		cost = -1,
		queryId = 0,
		navType = NavType.Floor
	};

	private struct ProberCell
	{
		public int cost;

		public ushort queryId;

		public NavType navType;
	}
}
