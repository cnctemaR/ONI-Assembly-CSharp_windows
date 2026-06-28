using System;
using System.Collections.Generic;
using HUSL;
using UnityEngine;

public class NavGrid
{
	public NavGrid(string id, NavGrid.Transition[] transitions, Dictionary<NavType, string> idle_anims, CellOffset[] bounding_offsets, NavTableValidator[] validators)
	{
		this.id = id;
		this.Validators = validators;
		this.idleAnims = idle_anims;
		this.transitions = transitions;
		this.boundingOffsets = bounding_offsets;
		List<NavType> list = new List<NavType>();
		for (int i = 0; i < transitions.Length; i++)
		{
			transitions[i].id = i;
			if (!list.Contains(transitions[i].start))
			{
				list.Add(transitions[i].start);
			}
			if (!list.Contains(transitions[i].end))
			{
				list.Add(transitions[i].end);
			}
		}
		this.ValidNavTypes = list.ToArray();
		this.LinkTable = new int[Grid.CellCount];
		this.Links = new NavGrid.Link[NavGrid.MaxLinksPerCell * Grid.CellCount];
		this.NavTable = new NavTable(Grid.CellCount);
		this.transitions = transitions;
		foreach (NavTableValidator navTableValidator in validators)
		{
			NavTableValidator navTableValidator2 = navTableValidator;
			navTableValidator2.onDirty = (Action<int>)Delegate.Combine(navTableValidator2.onDirty, new Action<int>(this.AddDirtyCell));
		}
		this.InitializeGraph();
	}

	public int[] LinkTable { get; set; }

	public NavTable NavTable { get; private set; }

	public NavGrid.Transition[] transitions { get; set; }

	private static NavType MirrorNavType(NavType nav_type)
	{
		if (nav_type == NavType.LeftWall)
		{
			return NavType.RightWall;
		}
		if (nav_type == NavType.RightWall)
		{
			return NavType.LeftWall;
		}
		return nav_type;
	}

	public string GetIdleAnim(NavType nav_type)
	{
		return this.idleAnims[nav_type];
	}

	public void InitializeGraph()
	{
		NavGridUpdater.InitializeNavGrid(this.NavTable, this.ValidNavTypes, this.Validators, this.boundingOffsets, this.LinkTable, this.Links, this.transitions, Grid.BitFields);
	}

	public void UpdateGraph()
	{
		foreach (int num in this.DirtyCells)
		{
			for (int i = -3; i <= 3; i++)
			{
				for (int j = -2; j <= 2; j++)
				{
					int num2 = Grid.OffsetCell(num, j, i);
					if (Grid.IsValidCell(num2))
					{
						this.ExpandedDirtyCells.Add(num2);
					}
				}
			}
		}
		this.UpdateGraph(this.ExpandedDirtyCells);
		this.DirtyCells.Clear();
		this.ExpandedDirtyCells.Clear();
	}

	public void UpdateGraph(HashSet<int> dirty_nav_cells)
	{
		NavGridUpdater.UpdateNavGrid(this.NavTable, this.ValidNavTypes, this.Validators, this.boundingOffsets, this.LinkTable, this.Links, this.transitions, Grid.BitFields, dirty_nav_cells);
	}

	public void RunQuery(int start_cell, Func<int, bool> is_valid_cell, Func<int, bool> is_target_cell)
	{
		NavGrid.Potentials.Clear();
		NavGrid.Potentials.Add(start_cell);
		NavGrid.Link[] links = this.Links;
		int[] linkTable = this.LinkTable;
		for (int i = 0; i < NavGrid.Potentials.Count; i++)
		{
			int num = NavGrid.Potentials[i];
			if (Grid.IsValidCell(num) && is_valid_cell(num))
			{
				if (is_target_cell(num))
				{
					break;
				}
				int num2 = linkTable[num];
				if (num2 != PathFinder.InvalidHandle)
				{
					for (int num3 = links[num2].link; num3 != PathFinder.InvalidHandle; num3 = links[num2].link)
					{
						if (!NavGrid.Potentials.Contains(num3))
						{
							NavGrid.Potentials.Add(num3);
						}
						num2++;
					}
				}
			}
		}
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
				NavGrid.DebugDrawPath(path.nodes[i].cell, path.nodes[i + 1].cell);
			}
		}
	}

	private void DebugDrawValidCells()
	{
		int cellCount = Grid.CellCount;
		for (int i = 0; i < cellCount; i++)
		{
			for (int j = 0; j < 7; j++)
			{
				NavType navType = (NavType)j;
				if (this.NavTable.IsValid(i, navType))
				{
					DebugExtension.DebugPoint(NavTypeHelper.GetNavPos(i, navType), this.NavTypeColor(navType), 1f, 0f, true);
				}
			}
		}
	}

	private void DebugDrawLinks()
	{
		for (int i = 0; i < this.LinkTable.Length; i++)
		{
			int num = this.LinkTable[i];
			if (num != NavGrid.InvalidCell)
			{
				for (int num2 = this.Links[num].link; num2 != NavGrid.InvalidCell; num2 = this.Links[num].link)
				{
					Vector3 navPos = NavTypeHelper.GetNavPos(i, this.Links[num].startNavType);
					Vector3 navPos2 = NavTypeHelper.GetNavPos(num2, this.Links[num].endNavType);
					num++;
				}
			}
		}
	}

	public void DebugUpdate()
	{
		if (this.DebugViewValidCells)
		{
			this.DebugDrawValidCells();
		}
		if (this.DebugViewLinks)
		{
			this.DebugDrawLinks();
		}
	}

	public void AddDirtyCell(int cell)
	{
		this.DirtyCells.Add(cell);
	}

	public void Clear()
	{
		foreach (NavTableValidator navTableValidator in this.Validators)
		{
			navTableValidator.Clear();
		}
	}

	private Color NavTypeColor(NavType navType)
	{
		if (this.debugColorLookup == null)
		{
			this.debugColorLookup = new Color[7];
			for (int i = 0; i < 7; i++)
			{
				double num = (double)i / 7.0;
				IList<double> list = ColorConverter.HUSLToRGB(new double[]
				{
					num * 360.0,
					100.0,
					50.0
				});
				this.debugColorLookup[i] = new Color((float)list[0], (float)list[1], (float)list[2]);
			}
		}
		return this.debugColorLookup[(int)navType];
	}

	public static int MaxLinksPerCell = 22;

	public bool DebugViewAllPaths;

	public bool DebugViewValidCells;

	public bool DebugViewLinks;

	public static int InvalidHandle = -1;

	public static int InvalidIdx = -1;

	public static int InvalidCell = -1;

	public NavGrid.Link[] Links;

	private HashSet<int> DirtyCells = new HashSet<int>();

	private HashSet<int> ExpandedDirtyCells = new HashSet<int>();

	private NavTableValidator[] Validators = new NavTableValidator[0];

	private Dictionary<NavType, string> idleAnims;

	private CellOffset[] boundingOffsets;

	public string id;

	public NavType[] ValidNavTypes;

	private static List<int> Potentials = new List<int>();

	private Color[] debugColorLookup;

	public struct Link
	{
		public Link(int link, NavType start_nav_type, NavType end_nav_type, int transition_id, int cost)
		{
			this.link = link;
			this.startNavType = start_nav_type;
			this.endNavType = end_nav_type;
			this.transitionId = transition_id;
			this.cost = cost;
		}

		public int link;

		public NavType startNavType;

		public NavType endNavType;

		public int transitionId;

		public int cost;
	}

	public struct Transition
	{
		public Transition(NavType start, NavType end, int x, int y, bool is_looping, bool is_escape, int cost, string anim, CellOffset[] void_offsets, CellOffset[] solid_offsets, NavOffset[] valid_nav_offsets, NavOffset[] invalid_nav_offsets)
		{
			this.id = -1;
			this.start = start;
			this.end = end;
			this.x = x;
			this.y = y;
			this.isLooping = is_looping;
			this.isEscape = is_escape;
			this.anim = anim;
			this.preAnim = string.Empty;
			this.cost = cost;
			if (string.IsNullOrEmpty(this.anim))
			{
				this.anim = string.Concat(new object[]
				{
					start.ToString().ToLower(),
					"_",
					end.ToString().ToLower(),
					"_",
					x,
					"_",
					y
				});
			}
			if (this.isLooping)
			{
				this.preAnim = this.anim + "_pre";
				this.anim += "_loop";
			}
			this.voidOffsets = void_offsets;
			this.solidOffsets = solid_offsets;
			this.validNavOffsets = valid_nav_offsets;
			this.invalidNavOffsets = invalid_nav_offsets;
		}

		public int IsValid(int cell, NavTable nav_table, ushort[] gridBitFields, bool check_if_current_cell_is_valid)
		{
			int num = Grid.OffsetCell(cell, this.x, this.y);
			if (!Grid.IsValidCell(num))
			{
				return Grid.InvalidCell;
			}
			if (!nav_table.IsValid(cell, this.start) && check_if_current_cell_is_valid)
			{
				return Grid.InvalidCell;
			}
			if (!nav_table.IsValid(num, this.end))
			{
				return Grid.InvalidCell;
			}
			int num2 = this.voidOffsets.Length;
			for (int i = 0; i < num2; i++)
			{
				int num3 = Grid.OffsetCell(cell, this.voidOffsets[i].x, this.voidOffsets[i].y);
				if (Grid.IsValidCell(num3) && (gridBitFields[num3] & 32) != 0)
				{
					return Grid.InvalidCell;
				}
			}
			int num4 = this.solidOffsets.Length;
			for (int j = 0; j < num4; j++)
			{
				int num5 = Grid.OffsetCell(cell, this.solidOffsets[j].x, this.solidOffsets[j].y);
				if (Grid.IsValidCell(num5) && (gridBitFields[num5] & 32) == 0)
				{
					return Grid.InvalidCell;
				}
			}
			int num6 = this.validNavOffsets.Length;
			for (int k = 0; k < num6; k++)
			{
				int num7 = Grid.OffsetCell(cell, this.validNavOffsets[k].offset.x, this.validNavOffsets[k].offset.y);
				if (Grid.IsValidCell(num7) && !nav_table.IsValid(num7, this.validNavOffsets[k].navType))
				{
					return Grid.InvalidCell;
				}
			}
			int num8 = this.invalidNavOffsets.Length;
			for (int l = 0; l < num8; l++)
			{
				int num9 = Grid.OffsetCell(cell, this.invalidNavOffsets[l].offset.x, this.invalidNavOffsets[l].offset.y);
				if (Grid.IsValidCell(num9) && nav_table.IsValid(num9, this.invalidNavOffsets[l].navType))
				{
					return Grid.InvalidCell;
				}
			}
			return num;
		}

		public NavType start;

		public NavType end;

		public int x;

		public int y;

		public bool isLooping;

		public bool isEscape;

		public string preAnim;

		public string anim;

		public int id;

		public int cost;

		public CellOffset[] voidOffsets;

		public CellOffset[] solidOffsets;

		public NavOffset[] validNavOffsets;

		public NavOffset[] invalidNavOffsets;
	}
}
