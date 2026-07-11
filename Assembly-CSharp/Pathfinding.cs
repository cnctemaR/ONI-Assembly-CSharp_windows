using System;
using System.Collections.Generic;

public class Pathfinding : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		Pathfinding.Instance = null;
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
		Debug.LogError("Could not find nav grid: " + id, null);
		return null;
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
			foreach (NavGrid navGrid in this.NavGrids)
			{
				navGrid.UpdateGraph();
			}
		}
		else
		{
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

	public void AddNavigationFeature(int cell, Pathfinding.INavigationFeature feature)
	{
		this.NavigationFeatures[cell] = feature;
	}

	public void RemoveNavigationFeature(int cell, Pathfinding.INavigationFeature feature)
	{
		this.NavigationFeatures.Remove(cell);
	}

	public Pathfinding.INavigationFeature GetNavigationFeature(int cell)
	{
		Pathfinding.INavigationFeature navigationFeature = null;
		this.NavigationFeatures.TryGetValue(cell, out navigationFeature);
		return navigationFeature;
	}

	protected override void OnCleanUp()
	{
		this.NavGrids.Clear();
	}

	private List<NavGrid> NavGrids = new List<NavGrid>();

	private Dictionary<int, Pathfinding.INavigationFeature> NavigationFeatures = new Dictionary<int, Pathfinding.INavigationFeature>();

	private int UpdateIdx;

	private bool navGridsHaveBeenFlushedOnLoad;

	public static Pathfinding Instance;

	public interface INavigationFeature
	{
		bool IsTraversable(Navigator agent, PathFinder.PotentialPath path, int from_cell, int cost, PathFinderAbilities abilities);

		void ApplyTraversalToPath(Navigator agent, ref PathFinder.PotentialPath path, int from_cell);
	}
}
