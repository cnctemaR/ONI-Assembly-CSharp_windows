using System;
using System.Collections.Generic;

public class NavGridUpdater
{
	public static void InitializeNavGrid(NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type)
	{
		NavGridUpdater.MarkValidCells(nav_table, valid_nav_types, validators, bounding_offsets);
		NavGridUpdater.CreateLinks(nav_table, max_links_per_cell, links, transitions_by_nav_type);
	}

	public static void UpdateNavGrid(NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type, HashSet<int> dirty_nav_cells)
	{
		NavGridUpdater.UpdateValidCells(dirty_nav_cells, nav_table, valid_nav_types, validators, bounding_offsets);
		NavGridUpdater.UpdateLinks(dirty_nav_cells, nav_table, max_links_per_cell, links, transitions_by_nav_type);
	}

	private static void UpdateValidCells(HashSet<int> dirty_solid_cells, NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets)
	{
		foreach (int num in dirty_solid_cells)
		{
			for (int i = 0; i < validators.Length; i++)
			{
				validators[i].UpdateCell(num, nav_table, bounding_offsets);
			}
		}
	}

	private static void CreateLinksForCell(int cell, NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type)
	{
		NavGridUpdater.CreateLinks(cell, nav_table, max_links_per_cell, links, transitions_by_nav_type);
	}

	private static void UpdateLinks(HashSet<int> dirty_nav_cells, NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type)
	{
		foreach (int num in dirty_nav_cells)
		{
			NavGridUpdater.CreateLinksForCell(num, nav_table, max_links_per_cell, links, transitions_by_nav_type);
		}
	}

	private static void CreateLinks(NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type)
	{
		WorkItemCollection<NavGridUpdater.CreateLinkWorkItem, object> workItemCollection = new WorkItemCollection<NavGridUpdater.CreateLinkWorkItem, object>();
		workItemCollection.Reset(null);
		for (int i = 0; i < Grid.HeightInCells; i++)
		{
			workItemCollection.Add(new NavGridUpdater.CreateLinkWorkItem(Grid.OffsetCell(0, new CellOffset(0, i)), nav_table, max_links_per_cell, links, transitions_by_nav_type));
		}
		GlobalJobManager.Run(workItemCollection);
	}

	private static void CreateLinks(int cell, NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type)
	{
		int num = cell * max_links_per_cell;
		int num2 = 0;
		for (int i = 0; i < 10; i++)
		{
			NavType navType = (NavType)i;
			NavGrid.Transition[] array = transitions_by_nav_type[i];
			if (array != null && nav_table.IsValid(cell, navType))
			{
				foreach (NavGrid.Transition transition in array)
				{
					int num3 = transition.IsValid(cell, nav_table);
					if (num3 != Grid.InvalidCell)
					{
						links[num] = new NavGrid.Link(num3, transition.start, transition.end, transition.id, transition.cost);
						num++;
						num2++;
					}
				}
			}
		}
		if (num2 >= max_links_per_cell)
		{
			Debug.LogError("Out of nav links. Need to increase maxLinksPerCell:" + max_links_per_cell);
		}
		links[num].link = Grid.InvalidCell;
	}

	private static void MarkValidCells(NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets)
	{
		WorkItemCollection<NavGridUpdater.MarkValidCellWorkItem, object> workItemCollection = new WorkItemCollection<NavGridUpdater.MarkValidCellWorkItem, object>();
		workItemCollection.Reset(null);
		for (int i = 0; i < Grid.HeightInCells; i++)
		{
			workItemCollection.Add(new NavGridUpdater.MarkValidCellWorkItem(Grid.OffsetCell(0, new CellOffset(0, i)), nav_table, bounding_offsets, validators));
		}
		GlobalJobManager.Run(workItemCollection);
	}

	public static void DebugDrawPath(int start_cell, int end_cell)
	{
		Grid.CellToPosCCF(start_cell, Grid.SceneLayer.Move);
		Grid.CellToPosCCF(end_cell, Grid.SceneLayer.Move);
	}

	public static void DebugDrawPath(PathFinder.Path path)
	{
		if (path.nodes != null)
		{
			for (int i = 0; i < path.nodes.Count - 1; i++)
			{
				NavGridUpdater.DebugDrawPath(path.nodes[i].cell, path.nodes[i + 1].cell);
			}
		}
	}

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	private struct CreateLinkWorkItem : IWorkItem<object>
	{
		public CreateLinkWorkItem(int start_cell, NavTable nav_table, int max_links_per_cell, NavGrid.Link[] links, NavGrid.Transition[][] transitions_by_nav_type)
		{
			this.startCell = start_cell;
			this.navTable = nav_table;
			this.maxLinksPerCell = max_links_per_cell;
			this.links = links;
			this.transitionsByNavType = transitions_by_nav_type;
		}

		public void Run(object shared_data)
		{
			for (int i = 0; i < Grid.WidthInCells; i++)
			{
				NavGridUpdater.CreateLinksForCell(this.startCell + i, this.navTable, this.maxLinksPerCell, this.links, this.transitionsByNavType);
			}
		}

		private int startCell;

		private NavTable navTable;

		private int maxLinksPerCell;

		private NavGrid.Link[] links;

		private NavGrid.Transition[][] transitionsByNavType;
	}

	private struct MarkValidCellWorkItem : IWorkItem<object>
	{
		public MarkValidCellWorkItem(int start_cell, NavTable nav_table, CellOffset[] bounding_offsets, NavTableValidator[] validators)
		{
			this.startCell = start_cell;
			this.navTable = nav_table;
			this.boundingOffsets = bounding_offsets;
			this.validators = validators;
		}

		public void Run(object shared_data)
		{
			for (int i = 0; i < Grid.WidthInCells; i++)
			{
				int num = this.startCell + i;
				NavTableValidator[] array = this.validators;
				for (int j = 0; j < array.Length; j++)
				{
					array[j].UpdateCell(num, this.navTable, this.boundingOffsets);
				}
			}
		}

		private NavTable navTable;

		private CellOffset[] boundingOffsets;

		private NavTableValidator[] validators;

		private int startCell;
	}
}
