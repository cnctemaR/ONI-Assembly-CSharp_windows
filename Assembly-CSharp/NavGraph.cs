using System;

public class NavGraph
{
	public NavGraph(int cell_count, NavGrid nav_grid)
	{
		this.grid = new HandleVector<NavGraph.NavGraphEdge>.Handle[NavGrid.MaxLinksPerCell * cell_count];
		for (int i = 0; i < this.grid.Length; i++)
		{
			this.grid[i] = HandleVector<NavGraph.NavGraphEdge>.InvalidHandle;
		}
		this.edges = new HandleVector<NavGraph.NavGraphEdge>(cell_count);
		for (int j = 0; j < cell_count; j++)
		{
			int num = j * NavGrid.MaxLinksPerCell;
			NavGrid.Link link = nav_grid.Links[num];
			while (link.link != NavGrid.InvalidHandle)
			{
				NavGraph.NavGraphEdge navGraphEdge = default(NavGraph.NavGraphEdge);
				navGraphEdge.startNavType = link.startNavType;
				navGraphEdge.endNavType = link.endNavType;
				navGraphEdge.endCell = link.link;
				navGraphEdge.startCell = j;
				HandleVector<NavGraph.NavGraphEdge>.Handle handle = this.edges.Add(navGraphEdge);
				this.grid[num] = handle;
				num++;
				link = nav_grid.Links[num];
			}
			this.grid[num] = HandleVector<NavGraph.NavGraphEdge>.InvalidHandle;
		}
	}

	public void Cleanup()
	{
	}

	private HandleVector<NavGraph.NavGraphEdge>.Handle[] grid;

	private HandleVector<NavGraph.NavGraphEdge> edges;

	private struct NavGraphEdge
	{
		public NavType startNavType;

		public NavType endNavType;

		public int startCell;

		public int endCell;
	}
}
