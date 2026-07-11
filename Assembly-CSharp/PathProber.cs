using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/PathProber")]
public class PathProber : KMonoBehaviour
{
	protected override void OnCleanUp()
	{
		if (this.PathGrid != null)
		{
			this.PathGrid.OnCleanUp();
		}
		base.OnCleanUp();
	}

	public void SetGroupProber(IGroupProber group_prober)
	{
		this.PathGrid.SetGroupProber(group_prober);
	}

	public void SetValidNavTypes(NavType[] nav_types, int max_probing_radius)
	{
		if (max_probing_radius != 0)
		{
			this.PathGrid = new PathGrid(max_probing_radius * 2, max_probing_radius * 2, true, nav_types);
			return;
		}
		this.PathGrid = new PathGrid(Grid.WidthInCells, Grid.HeightInCells, false, nav_types);
	}

	public int GetCost(int cell)
	{
		return this.PathGrid.GetCost(cell);
	}

	public int GetNavigationCostIgnoreProberOffset(int cell, CellOffset[] offsets)
	{
		return this.PathGrid.GetCostIgnoreProberOffset(cell, offsets);
	}

	public PathGrid GetPathGrid()
	{
		return this.PathGrid;
	}

	public void UpdateProbe(NavGrid nav_grid, int cell, NavType nav_type, PathFinderAbilities abilities, PathFinder.PotentialPath.Flags flags)
	{
		if (this.scratchPad == null)
		{
			this.scratchPad = new PathFinder.PotentialScratchPad(nav_grid.maxLinksPerCell);
		}
		bool flag = this.updateCount == -1;
		bool flag2 = this.Potentials.Count == 0 || flag;
		this.PathGrid.BeginUpdate(cell, !flag2);
		if (flag2)
		{
			this.updateCount = 0;
			bool flag3;
			PathFinder.Cell cell2 = this.PathGrid.GetCell(cell, nav_type, out flag3);
			PathFinder.AddPotential(new PathFinder.PotentialPath(cell, nav_type, flags), Grid.InvalidCell, NavType.NumNavTypes, 0, 0, -1, this.Potentials, this.PathGrid, ref cell2);
		}
		int num = ((this.potentialCellsPerUpdate <= 0 || flag) ? int.MaxValue : this.potentialCellsPerUpdate);
		this.updateCount++;
		while (this.Potentials.Count > 0 && num > 0)
		{
			KeyValuePair<int, PathFinder.PotentialPath> keyValuePair = this.Potentials.Next();
			num--;
			bool flag3;
			PathFinder.Cell cell3 = this.PathGrid.GetCell(keyValuePair.Value, out flag3);
			if (cell3.cost == keyValuePair.Key)
			{
				PathFinder.AddPotentials(this.scratchPad, keyValuePair.Value, cell3.cost, (int)cell3.underwaterCost, ref abilities, null, nav_grid.maxLinksPerCell, nav_grid.Links, this.Potentials, this.PathGrid, cell3.parent, cell3.parentNavType);
			}
		}
		bool flag4 = this.Potentials.Count == 0;
		this.PathGrid.EndUpdate(flag4);
		if (flag4)
		{
			int num2 = this.updateCount;
		}
	}

	public const int InvalidHandle = -1;

	public const int InvalidIdx = -1;

	public const int InvalidCell = -1;

	public const int InvalidCost = -1;

	private PathGrid PathGrid;

	private PathFinder.PotentialList Potentials = new PathFinder.PotentialList();

	public int updateCount = -1;

	private const int updateCountThreshold = 25;

	private PathFinder.PotentialScratchPad scratchPad;

	public int potentialCellsPerUpdate = -1;
}
