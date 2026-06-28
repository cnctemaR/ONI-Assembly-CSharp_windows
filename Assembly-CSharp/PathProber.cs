using System;
using System.Collections.Generic;

[SkipSaveFileSerialization]
public class PathProber : KMonoBehaviour
{
	public void SetValidNavTypes(NavType[] nav_types, int max_probing_radius)
	{
		if (max_probing_radius != 0)
		{
			this.PathGrid = new PathGrid(max_probing_radius, max_probing_radius, true, nav_types);
		}
		else
		{
			this.PathGrid = new PathGrid(Grid.WidthInCells, Grid.HeightInCells, false, nav_types);
		}
	}

	public int GetCost(int cell)
	{
		int num;
		if (this.PathGrid == null)
		{
			num = PathProber.InvalidCost;
		}
		else
		{
			num = this.PathGrid.GetCost(cell, this.QueryId);
		}
		return num;
	}

	public PathGrid GetPathGrid()
	{
		return this.PathGrid;
	}

	public void UpdateProbe(NavGrid nav_grid, int cell, NavType nav_type, PathFinderAbilities abilities, PathFinder.PotentialPath.Flags flags, bool new_query = true)
	{
		this.Potentials.Clear();
		if (new_query)
		{
			this.QueryId++;
			this.IslandCount = 1;
		}
		else
		{
			this.IslandCount++;
		}
		this.PathGrid.SetRootCell(cell);
		int num = 0;
		PathFinder.Cell cell2 = this.PathGrid.GetCell(cell, nav_type);
		PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(cell, nav_type, flags);
		PathFinder.AddPotential(potentialPath, Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, ref num, this.Potentials, this.QueryId, this.PathGrid, false, ref cell2);
		if (this.IslandCount > this.Islands.Length)
		{
			PathProber.Island[] array = new PathProber.Island[this.IslandCount];
			for (int i = 0; i < this.Islands.Length; i++)
			{
				array[i] = this.Islands[i];
			}
			this.Islands = array;
		}
		int num2 = this.IslandCount - 1;
		this.Islands[num2].cell = cell;
		this.UpdateProbe(nav_grid, ref abilities, this.Potentials, this.QueryId, num2);
	}

	private void UpdateProbe(NavGrid nav_grid, ref PathFinderAbilities abilities, List<PathFinder.PotentialPath> potentials, int query_id, int island)
	{
		int i = 0;
		while (i < potentials.Count)
		{
			i++;
			this.UpdateProbe(nav_grid, ref abilities, ref i, potentials[i - 1], potentials, query_id, island);
		}
	}

	private void UpdateProbe(NavGrid nav_grid, ref PathFinderAbilities abilities, ref int next_potential_idx, PathFinder.PotentialPath potential, List<PathFinder.PotentialPath> potentials, int query_id, int island)
	{
		PathFinder.Cell cell = this.PathGrid.GetCell(potential);
		PathFinder.AddPotentials(potential, cell.cost, (int)cell.underwaterCost, ref abilities, null, nav_grid.LinkTable, nav_grid.Links, ref next_potential_idx, potentials, query_id, this.PathGrid);
	}

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public static int InvalidCost = -1;

	public static int InvalidIsland = -1;

	public int QueryId = 1;

	public int IslandCount;

	private PathGrid PathGrid;

	private List<PathFinder.PotentialPath> Potentials = new List<PathFinder.PotentialPath>();

	public PathProber.Island[] Islands = new PathProber.Island[1];

	public struct Island
	{
		public int cell;
	}
}
