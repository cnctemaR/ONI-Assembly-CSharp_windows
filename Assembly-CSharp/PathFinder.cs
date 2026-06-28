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

	public static void UpdatePath(NavGrid nav_grid, PathFinderAbilities abilities, PathFinder.PotentialPath potential_path, PathFinderQuery query, ref PathFinder.Path path)
	{
		PathFinder.Run(nav_grid, abilities, potential_path, query, ref path);
	}

	public static void Run(NavGrid nav_grid, PathFinderAbilities abilities, PathFinder.PotentialPath potential_path, PathFinderQuery query)
	{
		int invalidCell = PathFinder.InvalidCell;
		NavType navType = NavType.NumNavTypes;
		query.ClearResult();
		int num = 0;
		PathFinder.FindPaths(nav_grid, ref abilities, potential_path, PathFinder.PathGrid, query, ref PathFinder.QueryId, ref num, PathFinder.Temp.Potentials, ref invalidCell, ref navType);
		if (invalidCell != PathFinder.InvalidCell)
		{
			PathFinder.Cell cell = PathFinder.PathGrid.GetCell(invalidCell, navType);
			query.SetResult(invalidCell, cell.cost, navType);
		}
	}

	public static void Run(NavGrid nav_grid, PathFinderAbilities abilities, PathFinder.PotentialPath potential_path, PathFinderQuery query, ref PathFinder.Path path)
	{
		PathFinder.Run(nav_grid, abilities, potential_path, query);
		if (query.GetResultCell() != PathFinder.InvalidCell)
		{
			PathFinder.BuildResultPath(query.GetResultCell(), query.GetResultNavType(), PathFinder.PathGrid, ref path);
		}
		else
		{
			path.Clear();
		}
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

	private static void FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialPath potential_path, PathGrid path_grid, PathFinderQuery query, ref int query_id, ref int next_potential_idx, List<PathFinder.PotentialPath> potentials, ref int result_cell, ref NavType result_nav_type)
	{
		potentials.Clear();
		query_id++;
		PathFinder.Cell cell = path_grid.GetCell(potential_path);
		PathFinder.AddPotential(potential_path, Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, ref next_potential_idx, potentials, query_id, path_grid, false, ref cell);
		PathFinder.FindPaths(nav_grid, ref abilities, ref next_potential_idx, potentials, query_id, path_grid, query, ref result_cell, ref result_nav_type);
	}

	private static void FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, ref int next_potential_idx, List<PathFinder.PotentialPath> potentials, int query_id, PathGrid path_grid, PathFinderQuery query, ref int result_cell, ref NavType result_nav_type)
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
					int cost = path_grid.GetCell(potentials[i]).cost;
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

	private static bool FindPaths(NavGrid nav_grid, ref PathFinderAbilities abilities, ref int next_potential_idx, PathFinder.PotentialPath potential, List<PathFinder.PotentialPath> potentials, int query_id, PathGrid path_grid, PathFinderQuery query, ref int result_cell, ref NavType result_nav_type, ref int result_cost)
	{
		PathFinder.Cell cell = path_grid.GetCell(potential);
		int cost = cell.cost;
		NavType navType = cell.navType;
		bool flag = query.IsMatch(potential.cell, cell.parent, cost) && cost < result_cost;
		if (flag)
		{
			result_cell = potential.cell;
			result_cost = cost;
			result_nav_type = navType;
		}
		path_grid.SetCell(potential, ref cell);
		if (!flag)
		{
			PathFinder.AddPotentials(potential, cell.cost, (int)cell.underwaterCost, ref abilities, query, nav_grid.LinkTable, nav_grid.Links, ref next_potential_idx, potentials, query_id, path_grid);
		}
		return flag;
	}

	public static void AddPotential(PathFinder.PotentialPath potential_path, int parent_cell, NavType parent_nav_type, int cost, int underwater_cost, int transition_id, ref int next_potential_idx, List<PathFinder.PotentialPath> potentials, int query_id, PathGrid path_grid, bool is_better_path, ref PathFinder.Cell cell_data)
	{
		cell_data.queryId = query_id;
		cell_data.cost = cost;
		cell_data.underwaterCost = (byte)Math.Min(underwater_cost, 255);
		cell_data.parent = parent_cell;
		cell_data.navType = potential_path.navType;
		cell_data.parentNavType = parent_nav_type;
		cell_data.transitionId = transition_id;
		if (is_better_path && next_potential_idx > 0)
		{
			next_potential_idx--;
			potentials[next_potential_idx] = potential_path;
		}
		else
		{
			potentials.Add(potential_path);
		}
		path_grid.SetCell(potential_path, ref cell_data);
	}

	public static bool IsSubmerged(int cell)
	{
		int num = Grid.CellAbove(cell);
		return Grid.IsValidCell(num) && Grid.Element[num].IsLiquid;
	}

	public static void AddPotentials(PathFinder.PotentialPath parent_potential, int cost, int underwater_cost, ref PathFinderAbilities abilities, PathFinderQuery query, int[] link_table, NavGrid.Link[] links, ref int next_potential_idx, List<PathFinder.PotentialPath> potentials, int query_id, PathGrid path_grid)
	{
		int num = link_table[parent_potential.cell];
		if (num != PathFinder.InvalidHandle)
		{
			for (int num2 = links[num].link; num2 != PathFinder.InvalidHandle; num2 = links[num].link)
			{
				NavType startNavType = links[num].startNavType;
				if (startNavType == parent_potential.navType)
				{
					PathFinder.Cell cell = path_grid.GetCell(num2, links[num].endNavType);
					if (cell.cost >= 0)
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
						bool flag = query_id != cell.queryId;
						bool flag2 = num3 < cell.cost;
						if ((flag || flag2) && Grid.IsValidCell(num2))
						{
							PathFinder.PotentialPath potentialPath = parent_potential;
							potentialPath.cell = num2;
							potentialPath.navType = links[num].endNavType;
							if (abilities.CanTraverse(potentialPath, parent_potential.cell, num3, num4) && (query == null || query.CanTraverse(num2, parent_potential.cell, num3, num4)))
							{
								abilities.ApplyTraversalToPath(ref potentialPath, parent_potential.cell);
								PathFinder.AddPotential(potentialPath, parent_potential.cell, parent_potential.navType, num3, num4, links[num].transitionId, ref next_potential_idx, potentials, query_id, path_grid, !flag && flag2, ref cell);
							}
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

	public struct PotentialPath
	{
		public PotentialPath(int cell, NavType nav_type, PathFinder.PotentialPath.Flags flags)
		{
			this.cell = cell;
			this.navType = nav_type;
			this.flags = flags;
		}

		public void SetFlags(PathFinder.PotentialPath.Flags new_flags)
		{
			this.flags |= new_flags;
		}

		public void ClearFlags(PathFinder.PotentialPath.Flags new_flags)
		{
			this.flags &= ~new_flags;
		}

		public bool HasFlag(PathFinder.PotentialPath.Flags flag)
		{
			return (this.flags & flag) != PathFinder.PotentialPath.Flags.None;
		}

		public int cell;

		public NavType navType;

		private PathFinder.PotentialPath.Flags flags;

		[Flags]
		public enum Flags
		{
			None = 0,
			HasSuit = 1,
			UnlimitedSubmergedTravel = 2,
			PerformSuitChecks = 4
		}
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

			public int transitionId;
		}
	}

	private class Temp
	{
		public static List<PathFinder.PotentialPath> Potentials = new List<PathFinder.PotentialPath>();
	}
}
