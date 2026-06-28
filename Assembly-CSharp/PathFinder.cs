using System;
using System.Collections.Generic;

public class PathFinder
{
	public static void Initialize()
	{
		NavType[] array = new NavType[7];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (NavType)i;
		}
		PathFinder.PathGrid = new PathGrid(Grid.WidthInCells, Grid.HeightInCells, false, array);
	}

	public static void UpdatePath(NavGrid nav_grid, PathFinderAbilities abilities, int source_cell, NavType nav_type, PathFinderQuery query, ref PathFinder.Path path)
	{
		PathFinder.Run(nav_grid, abilities, source_cell, nav_type, query, ref path);
	}

	public static void Run(NavGrid nav_grid, PathFinderAbilities abilities, int cell, NavType nav_type, PathFinderQuery query)
	{
		PathFinder.SourceCell sourceCell = new PathFinder.SourceCell
		{
			cell = cell
		};
		int invalidCell = PathFinder.InvalidCell;
		NavType navType = NavType.NumNavTypes;
		query.ClearResult();
		int num = 0;
		PathFinder.FindPaths(nav_grid, ref abilities, sourceCell, nav_type, PathFinder.PathGrid, query, ref PathFinder.QueryId, ref num, PathFinder.Temp.Potentials, ref invalidCell, ref navType);
		if (invalidCell != PathFinder.InvalidCell)
		{
			PathFinder.Cell cell2 = PathFinder.PathGrid.GetCell(invalidCell, navType);
			query.SetResult(invalidCell, cell2.cost, navType);
		}
	}

	public static void Run(NavGrid nav_grid, PathFinderAbilities abilities, int cell, NavType nav_type, PathFinderQuery query, ref PathFinder.Path path)
	{
		PathFinder.Run(nav_grid, abilities, cell, nav_type, query);
		if (query.GetResultCell() != PathFinder.InvalidCell)
		{
			PathFinder.BuildResultPath(query.GetResultCell(), query.GetResultNavType(), PathFinder.PathGrid, ref path);
		}
		else
		{
			path.Clear();
		}
	}

	private static void FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.SourceCell source, NavType nav_type, PathGrid path_grid, PathFinderQuery query, ref int query_id, ref int next_potential_idx, List<PathFinder.Potential> potentials, ref int result_cell, ref NavType result_nav_type)
	{
		int cell = source.cell;
		PathFinder.FindPaths(nav_grid, ref abilities, cell, nav_type, path_grid, query, ref query_id, ref next_potential_idx, potentials, ref result_cell, ref result_nav_type);
	}

	private static void BuildResultPath(int path_cell, NavType path_nav_type, PathGrid path_grid, ref PathFinder.Path path)
	{
		if (path_cell != PathFinder.InvalidCell)
		{
			PathFinder.Cell cell = path_grid.GetCell(path_cell, path_nav_type);
			path.Clear();
			path.cost = cell.cost;
			while (path_cell != PathFinder.InvalidCell)
			{
				path.AddNode(new PathFinder.Path.Node
				{
					cell = path_cell,
					navType = cell.navType,
					transitionId = cell.transitionId
				});
				path_cell = cell.parent;
				if (path_cell != PathFinder.InvalidCell)
				{
					cell = path_grid.GetCell(path_cell, cell.parentNavType);
				}
			}
			if (path.nodes != null)
			{
				for (int i = 0; i < path.nodes.Count / 2; i++)
				{
					PathFinder.Path.Node node = path.nodes[i];
					path.nodes[i] = path.nodes[path.nodes.Count - i - 1];
					path.nodes[path.nodes.Count - i - 1] = node;
				}
			}
		}
	}

	private static void FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, int source_cell, NavType nav_type, PathGrid path_grid, PathFinderQuery query, ref int query_id, ref int next_potential_idx, List<PathFinder.Potential> potentials, ref int result_cell, ref NavType result_nav_type)
	{
		potentials.Clear();
		query_id++;
		PathFinder.Cell cell = path_grid.GetCell(source_cell, nav_type);
		PathFinder.AddPotential(source_cell, Grid.InvalidCell, nav_type, NavType.NumNavTypes, 0, 0, -1, ref next_potential_idx, potentials, query_id, path_grid, false, ref cell);
		PathFinder.FindPaths(nav_grid, ref abilities, ref next_potential_idx, potentials, query_id, path_grid, query, ref result_cell, ref result_nav_type);
	}

	private static void FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, ref int next_potential_idx, List<PathFinder.Potential> potentials, int query_id, PathGrid path_grid, PathFinderQuery query, ref int result_cell, ref NavType result_nav_type)
	{
		int maxValue = int.MaxValue;
		while (next_potential_idx < potentials.Count)
		{
			next_potential_idx++;
			if (PathFinder.FindPaths(nav_grid, ref abilities, ref next_potential_idx, potentials[next_potential_idx - 1], potentials, query_id, path_grid, query, ref result_cell, ref result_nav_type, ref maxValue))
			{
				bool flag = true;
				for (int i = next_potential_idx; i < potentials.Count; i++)
				{
					int cost = path_grid.GetCell(potentials[i].cell, potentials[i].navType).cost;
					if (cost < maxValue)
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
	}

	private static bool FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, ref int next_potential_idx, PathFinder.Potential potential, List<PathFinder.Potential> potentials, int query_id, PathGrid path_grid, PathFinderQuery query, ref int result_cell, ref NavType result_nav_type, ref int result_cost)
	{
		int cell = potential.cell;
		PathFinder.Cell cell2 = path_grid.GetCell(cell, potential.navType);
		int cost = cell2.cost;
		NavType navType = cell2.navType;
		bool flag = query.IsMatch(cell, cell2.parent, cost) && cost < result_cost;
		if (flag)
		{
			result_cell = cell;
			result_cost = cost;
			result_nav_type = navType;
		}
		path_grid.SetCell(cell, navType, ref cell2);
		if (!flag)
		{
			PathFinder.AddPotentials(cell, cell2.parent, potential.navType, cell2.cost, (int)cell2.underwaterCost, ref abilities, nav_grid.LinkTable, nav_grid.Links, ref next_potential_idx, potentials, query_id, path_grid);
		}
		return flag;
	}

	public static void AddPotential(int cell, int parent_cell, NavType nav_type, NavType parent_nav_type, int cost, int underwater_cost, int transition_id, ref int next_potential_idx, List<PathFinder.Potential> potentials, int query_id, PathGrid path_grid, bool is_better_path, ref PathFinder.Cell cell_data)
	{
		cell_data.queryId = query_id;
		cell_data.cost = cost;
		cell_data.underwaterCost = (byte)Math.Min(underwater_cost, 255);
		cell_data.parent = parent_cell;
		cell_data.navType = nav_type;
		cell_data.parentNavType = parent_nav_type;
		cell_data.transitionId = transition_id;
		PathFinder.Potential potential = default(PathFinder.Potential);
		potential.cell = cell;
		potential.navType = nav_type;
		if (is_better_path && next_potential_idx > 0)
		{
			next_potential_idx--;
			potentials[next_potential_idx] = potential;
		}
		else
		{
			potentials.Add(potential);
		}
		path_grid.SetCell(cell, nav_type, ref cell_data);
	}

	public static bool IsSubmerged(int cell)
	{
		int num = Grid.CellAbove(cell);
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid;
	}

	public static void AddPotentials(int cell, int parent_cell, NavType nav_type, int cost, int underwater_cost, ref PathFinderAbilities abilities, int[] link_table, NavGrid.Link[] links, ref int next_potential_idx, List<PathFinder.Potential> potentials, int query_id, PathGrid path_grid)
	{
		int num = link_table[cell];
		if (num != PathFinder.InvalidHandle)
		{
			for (int num2 = links[num].link; num2 != PathFinder.InvalidHandle; num2 = links[num].link)
			{
				NavType startNavType = links[num].startNavType;
				if (startNavType == nav_type)
				{
					PathFinder.Cell cell2 = path_grid.GetCell(num2, links[num].endNavType);
					if (cell2.cost >= 0)
					{
						int num3 = cost + links[num].cost;
						int num4;
						if (PathFinder.IsSubmerged(num2))
						{
							num4 = underwater_cost + 1;
						}
						else
						{
							num4 = 0;
						}
						bool flag = query_id != cell2.queryId;
						bool flag2 = num3 < cell2.cost;
						if ((flag || flag2) && Grid.IsValidCell(num2) && abilities.CanTraverse(num2, cell, num4))
						{
							PathFinder.AddPotential(num2, cell, links[num].endNavType, nav_type, num3, num4, links[num].transitionId, ref next_potential_idx, potentials, query_id, path_grid, !flag && flag2, ref cell2);
						}
					}
				}
				num++;
				if (num >= links.Length)
				{
					Debug.LogError("Out of nav links. Need to increase NavGrid.MaxLinksPerCell", null);
				}
			}
		}
	}

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public static int QueryId;

	public static PathGrid PathGrid;

	public struct Cell
	{
		public int queryId;

		public int cost;

		public int parent;

		public byte underwaterCost;

		public NavType navType;

		public NavType parentNavType;

		public int transitionId;
	}

	public struct SourceCell
	{
		public int cell;
	}

	public struct Potential
	{
		public int cell;

		public NavType navType;
	}

	public struct Path
	{
		public void AddNode(PathFinder.Path.Node node)
		{
			if (this.nodes == null)
			{
				this.nodes = new List<PathFinder.Path.Node>();
			}
			this.nodes.Add(node);
		}

		public bool IsValid()
		{
			return this.nodes != null && this.nodes.Count > 1;
		}

		public bool HasArrived()
		{
			return this.nodes != null && this.nodes.Count > 0;
		}

		public void Clear()
		{
			this.cost = 0;
			if (this.nodes != null)
			{
				this.nodes.Clear();
			}
		}

		public int cost;

		public List<PathFinder.Path.Node> nodes;

		public struct Node
		{
			public int cell;

			public NavType navType;

			public PathFinderFlags flags;

			public int transitionId;
		}
	}

	private class Temp
	{
		public static List<PathFinder.Potential> Potentials = new List<PathFinder.Potential>();
	}
}
