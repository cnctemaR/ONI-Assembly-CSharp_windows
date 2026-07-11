using System;
using System.Collections.Generic;

[SkipSaveFileSerialization]
public class PathProber : KMonoBehaviour
{
	public void SetGroupProber(IGroupProber group_prober)
	{
		this.PathGrid.SetGroupProber(group_prober);
	}

	public void SetValidNavTypes(NavType[] nav_types, int max_probing_radius)
	{
		if (max_probing_radius != 0)
		{
			this.PathGrid = new PathGrid(max_probing_radius * 2, max_probing_radius * 2, true, nav_types);
		}
		else
		{
			this.PathGrid = new PathGrid(Grid.WidthInCells, Grid.HeightInCells, false, nav_types);
		}
	}

	public int GetCost(int cell)
	{
		return this.PathGrid.GetCost(cell, this.QueryId);
	}

	public int GetNavigationCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		return this.PathGrid.GetCostIgnoreProberOffset(cell, offsets, this.QueryId);
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
		}
		this.PathGrid.SetRootCell(cell);
		bool flag = false;
		PathFinder.Cell cell2 = this.PathGrid.GetCell(cell, nav_type, this.QueryId, out flag);
		PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(cell, nav_type, flags);
		PathFinder.AddPotential(potentialPath, Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, this.Potentials, this.QueryId, this.PathGrid, ref cell2);
		this.UpdateProbe(nav_grid, ref abilities, this.Potentials, this.QueryId);
	}

	private void UpdateProbe(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialList potentials, int query_id)
	{
		while (potentials.Count > 0)
		{
			KeyValuePair<int, PathFinder.PotentialPath> keyValuePair = potentials.Next();
			this.UpdateProbe(nav_grid, ref abilities, keyValuePair.Value, keyValuePair.Key, potentials, query_id);
		}
	}

	private void UpdateProbe(NavGrid nav_grid, ref PathFinderAbilities abilities, PathFinder.PotentialPath potential, int potential_cost, PathFinder.PotentialList potentials, int query_id)
	{
		bool flag;
		PathFinder.Cell cell = this.PathGrid.GetCell(potential, query_id, out flag);
		if (cell.cost == potential_cost)
		{
			PathFinder.AddPotentials(nav_grid.potentialScratchPad, potential, cell.cost, (int)cell.underwaterCost, ref abilities, null, nav_grid.maxLinksPerCell, nav_grid.Links, potentials, query_id, this.PathGrid, cell.parent, cell.parentNavType);
		}
	}

	public const int InvalidHandle = -1;

	public const int InvalidIdx = -1;

	public const int InvalidCell = -1;

	public const int InvalidCost = -1;

	public int QueryId = 1;

	private PathGrid PathGrid;

	private PathFinder.PotentialList Potentials = new PathFinder.PotentialList();
}
