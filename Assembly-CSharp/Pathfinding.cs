using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Pathfinding.Instance = this;
	}

	public void AddNavGrid(NavGrid nav_grid)
	{
		this.NavGrids.Add(nav_grid);
	}

	public NavGrid GetNavGrid(string id)
	{
		foreach (NavGrid navGrid in this.NavGrids)
		{
			if (navGrid.id == id)
			{
				return navGrid;
			}
		}
		Debug.LogError("Could not find nav grid: " + id);
		return null;
	}

	public void ResetNavGrids()
	{
		foreach (NavGrid navGrid in this.NavGrids)
		{
			navGrid.InitializeGraph();
		}
	}

	public void UpdateNavGrids()
	{
		this.NavGrids[this.UpdateIdx].UpdateGraph();
		this.UpdateIdx = (this.UpdateIdx + 1) % this.NavGrids.Count;
	}

	public void DebugUpdate()
	{
		foreach (NavGrid navGrid in this.NavGrids)
		{
			navGrid.DebugUpdate();
		}
	}

	public void AddDirtyNavGridCell(int cell)
	{
		foreach (NavGrid navGrid in this.NavGrids)
		{
			navGrid.AddDirtyCell(cell);
		}
	}

	public void RefreshNavCell(int cell)
	{
		HashSet<int> hashSet = new HashSet<int>();
		hashSet.Add(cell);
		foreach (NavGrid navGrid in this.NavGrids)
		{
			navGrid.UpdateGraph(hashSet);
		}
	}

	protected override void OnCleanUp()
	{
		this.NavGrids.Clear();
	}

	private List<NavGrid> NavGrids = new List<NavGrid>();

	private int UpdateIdx;

	public static Pathfinding Instance;
}
