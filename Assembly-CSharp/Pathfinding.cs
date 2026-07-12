using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Pathfinding")]
public class Pathfinding : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		Pathfinding.Instance = null;
		OffsetTableTracker.OnPathfindingInvalidated();
	}

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
		global::Debug.LogError("Could not find nav grid: " + id);
		return null;
	}

	public List<NavGrid> GetNavGrids()
	{
		return this.NavGrids;
	}

	public void ResetNavGrids()
	{
		foreach (NavGrid navGrid in this.NavGrids)
		{
			navGrid.InitializeGraph();
		}
	}

	public void FlushNavGridsOnLoad()
	{
		if (this.navGridsHaveBeenFlushedOnLoad)
		{
			return;
		}
		this.navGridsHaveBeenFlushedOnLoad = true;
		this.UpdateNavGrids(true);
	}

	public void UpdateNavGrids(bool update_all = false)
	{
		update_all = true;
		if (update_all)
		{
			using (List<NavGrid>.Enumerator enumerator = this.NavGrids.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					NavGrid navGrid = enumerator.Current;
					navGrid.UpdateGraph();
				}
				return;
			}
		}
		foreach (NavGrid navGrid2 in this.NavGrids)
		{
			if (navGrid2.updateEveryFrame)
			{
				navGrid2.UpdateGraph();
			}
		}
		this.NavGrids[this.UpdateIdx].UpdateGraph();
		this.UpdateIdx = (this.UpdateIdx + 1) % this.NavGrids.Count;
	}

	public void RenderEveryTick()
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
		OffsetTableTracker.OnPathfindingInvalidated();
	}

	private List<NavGrid> NavGrids = new List<NavGrid>();

	private int UpdateIdx;

	private bool navGridsHaveBeenFlushedOnLoad;

	public static Pathfinding Instance;
}
