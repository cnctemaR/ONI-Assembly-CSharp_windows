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
		if (this.PathGrid == null)
		{
			return PathProber.InvalidCost;
		}
		return this.PathGrid.GetCost(cell, this.QueryId);
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
		PathFinder.Cell cell2 = this.PathGrid.GetCell(cell, nav_type);
		PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(cell, nav_type, flags);
		PathFinder.AddPotential(potentialPath, Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, this.Potentials, this.QueryId, this.PathGrid, ref cell2);
		if (this.IslandCount > this.Islands.Length)
		{
			PathProber.Island[] array = new PathProber.Island[this.IslandCount];
			for (int i = 0; i < this.Islands.Length; i++)
			{
				array[i] = this.Islands[i];
			}
			this.Islands = array;
		}
		int num = this.IslandCount - 1;
		this.Islands[num].cell = cell;
		this.UpdateProbe(nav_grid, ref abilities, this.Potentials, this.QueryId, num);
	}

	private void UpdateProbe(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialList potentials, int query_id, int island)
	{
		while (potentials.Count > 0)
		{
			KeyValuePair<int, PathFinder.PotentialPath> keyValuePair = potentials.Next();
			this.UpdateProbe(nav_grid, ref abilities, keyValuePair.Value, keyValuePair.Key, potentials, query_id, island);
		}
	}

	private void UpdateProbe(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialPath potential, int potential_cost, PathFinder.PotentialList potentials, int query_id, int island)
	{
		PathFinder.Cell cell = this.PathGrid.GetCell(potential);
		if (cell.cost == potential_cost)
		{
			PathFinder.AddPotentials(potential, cell.cost, (int)cell.underwaterCost, ref abilities, null, nav_grid.Links, potentials, query_id, this.PathGrid, cell.parent, cell.parentNavType);
		}
	}

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public static int InvalidCost = -1;

	public static int InvalidIsland = -1;

	public int QueryId = 1;

	public int IslandCount;

	private PathGrid PathGrid;

	private PathFinder.PotentialList Potentials = new PathFinder.PotentialList();

	public PathProber.Island[] Islands = new PathProber.Island[1];

	public struct Island
	{
		public int cell;
	}
}
