using System;
using System.Collections.Generic;
using UnityEngine;

public class NavGridUpdater
{
	public static void InitializeNavGrid(NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets, NavGrid.Link[] links, NavGrid.Transition[] transitions, ushort[] gridBitFields)
	{
		NavGridUpdater.MarkValidCells(gridBitFields, nav_table, valid_nav_types, validators, bounding_offsets);
		NavGridUpdater.CreateLinks(nav_table, valid_nav_types, gridBitFields, links, transitions);
	}

	public static void UpdateNavGrid(NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets, NavGrid.Link[] links, NavGrid.Transition[] transitions, ushort[] gridBitFields, ICollection<int> dirty_nav_cells)
	{
		NavGridUpdater.UpdateValidCells(dirty_nav_cells, gridBitFields, nav_table, valid_nav_types, validators, bounding_offsets);
		NavGridUpdater.UpdateLinks(dirty_nav_cells, nav_table, valid_nav_types, gridBitFields, links, transitions);
	}

	private static void UpdateValidCells(IEnumerable<int> dirty_solid_cells, ushort[] gridBitFields, NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets)
	{
		foreach (int num in dirty_solid_cells)
		{
			foreach (NavTableValidator navTableValidator in validators)
			{
				navTableValidator.UpdateCell(num, nav_table, bounding_offsets);
			}
		}
	}

	private static void CreateLinksForCell(int cell, NavTable nav_table, NavType[] valid_nav_types, ushort[] gridBitFields, NavGrid.Link[] links, NavGrid.Transition[] link_offsets)
	{
		NavGridUpdater.CreateLinks(cell, nav_table, gridBitFields, links, link_offsets);
	}

	private static void UpdateLinks(IEnumerable<int> dirty_nav_cells, NavTable nav_table, NavType[] valid_nav_types, ushort[] gridBitFields, NavGrid.Link[] links, NavGrid.Transition[] link_offsets)
	{
		foreach (int num in dirty_nav_cells)
		{
			NavGridUpdater.CreateLinksForCell(num, nav_table, valid_nav_types, gridBitFields, links, link_offsets);
		}
	}

	private static void CreateLinks(NavTable nav_table, NavType[] valid_nav_types, ushort[] gridBitFields, NavGrid.Link[] links, NavGrid.Transition[] link_offsets)
	{
		int num = 4;
		int cell_count_per_job = Grid.CellCount / num;
		JobBatch jobBatch = new JobBatch();
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			int first_cell = num2;
			jobBatch.Add(delegate
			{
				for (int j = first_cell; j < first_cell + cell_count_per_job; j++)
				{
					NavGridUpdater.CreateLinksForCell(j, nav_table, valid_nav_types, gridBitFields, links, link_offsets);
				}
			});
			num2 += cell_count_per_job;
		}
		jobBatch.Run();
	}

	private static void CreateLinks(int cell, NavTable nav_table, ushort[] gridBitFields, NavGrid.Link[] links, NavGrid.Transition[] transitions)
	{
		int num = cell * NavGrid.MaxLinksPerCell;
		int num2 = 0;
		foreach (NavGrid.Transition transition in transitions)
		{
			int num3 = transition.IsValid(cell, nav_table, gridBitFields, true);
			if (num3 != Grid.InvalidCell)
			{
				links[num] = new NavGrid.Link(num3, transition.start, transition.end, transition.id, transition.cost);
				num++;
				num2++;
			}
		}
		if (num2 >= NavGrid.MaxLinksPerCell)
		{
			global::Debug.LogError("Out of nav links. Need to increase NavGrid.MaxLinksPerCell", null);
		}
		links[num].link = Grid.InvalidCell;
	}

	private static void MarkValidCells(ushort[] gridBitFields, NavTable nav_table, NavType[] valid_nav_types, NavTableValidator[] validators, CellOffset[] bounding_offsets)
	{
		int num = 4;
		int cell_count_per_job = Grid.CellCount / num;
		JobBatch jobBatch = new JobBatch();
		int num2 = 0;
		for (int i = 0; i < num; i++)
		{
			int first_cell = num2;
			jobBatch.Add(delegate
			{
				for (int j = first_cell; j < first_cell + cell_count_per_job; j++)
				{
					foreach (NavTableValidator navTableValidator in validators)
					{
						navTableValidator.UpdateCell(j, nav_table, bounding_offsets);
					}
				}
			});
			num2 += cell_count_per_job;
		}
		jobBatch.Run();
	}

	public static void DebugDrawPath(int start_cell, int end_cell)
	{
		Vector3 vector = Grid.CellToPosCCF(start_cell, Grid.SceneLayer.Move);
		Vector3 vector2 = Grid.CellToPosCCF(end_cell, Grid.SceneLayer.Move);
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
}
