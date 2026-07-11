using System;
using System.Collections.Generic;
using HUSL;
using UnityEngine;

public class NavGrid
{
	public NavGrid(string id, NavGrid.Transition[] transitions, NavGrid.NavTypeData[] nav_type_data, CellOffset[] bounding_offsets, NavTableValidator[] validators, int update_range_x, int update_range_y, int max_links_per_cell)
	{
		this.id = id;
		this.Validators = validators;
		this.navTypeData = nav_type_data;
		this.transitions = transitions;
		this.boundingOffsets = bounding_offsets;
		List<NavType> list = new List<NavType>();
		this.updateRangeX = update_range_x;
		this.updateRangeY = update_range_y;
		this.maxLinksPerCell = max_links_per_cell + 1;
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
		foreach (NavType navType in this.ValidNavTypes)
		{
			this.GetNavTypeData(navType);
		}
		this.Links = new NavGrid.Link[this.maxLinksPerCell * Grid.CellCount];
		this.NavTable = new NavTable(Grid.CellCount);
		this.transitions = transitions;
		foreach (NavTableValidator navTableValidator in validators)
		{
			NavTableValidator navTableValidator2 = navTableValidator;
			navTableValidator2.onDirty = (Action<int>)Delegate.Combine(navTableValidator2.onDirty, new Action<int>(this.AddDirtyCell));
		}
		this.InitializeGraph();
		this.NavGraph = new NavGraph(Grid.CellCount, this);
	}

	public NavTable NavTable { get; private set; }

	public NavGraph NavGraph { get; private set; }

	public NavGrid.Transition[] transitions { get; set; }

	public int updateRangeX { get; private set; }

	public int updateRangeY { get; private set; }

	public int maxLinksPerCell { get; private set; }

	public static NavType MirrorNavType(NavType nav_type)
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

	public NavGrid.NavTypeData GetNavTypeData(NavType nav_type)
	{
		foreach (NavGrid.NavTypeData navTypeData in this.navTypeData)
		{
			if (navTypeData.navType == nav_type)
			{
				return navTypeData;
			}
		}
		throw new Exception("Missing nav type data for nav type:" + nav_type.ToString());
	}

	public bool HasNavTypeData(NavType nav_type)
	{
		foreach (NavGrid.NavTypeData navTypeData in this.navTypeData)
		{
			if (navTypeData.navType == nav_type)
			{
				return true;
			}
		}
		return false;
	}

	public HashedString GetIdleAnim(NavType nav_type)
	{
		return this.GetNavTypeData(nav_type).idleAnim;
	}

	public void InitializeGraph()
	{
		NavGridUpdater.InitializeNavGrid(this.NavTable, this.ValidNavTypes, this.Validators, this.boundingOffsets, this.maxLinksPerCell, this.Links, this.transitions, Grid.BitFields);
	}

	public void UpdateGraph()
	{
		foreach (int num in this.DirtyCells)
		{
			for (int i = -this.updateRangeY; i <= this.updateRangeY; i++)
			{
				for (int j = -this.updateRangeX; j <= this.updateRangeX; j++)
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
		NavGridUpdater.UpdateNavGrid(this.NavTable, this.ValidNavTypes, this.Validators, this.boundingOffsets, this.maxLinksPerCell, this.Links, this.transitions, Grid.BitFields, dirty_nav_cells);
		if (this.OnNavGridUpdateComplete != null)
		{
			this.OnNavGridUpdateComplete(dirty_nav_cells);
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
			for (int j = 0; j < 9; j++)
			{
				NavType navType = (NavType)j;
				if (this.NavTable.IsValid(i, navType))
				{
					DebugExtension.DebugPoint(NavTypeHelper.GetNavPos(i, navType), this.NavTypeColor(navType), 1f, 0f, false);
				}
			}
		}
	}

	private void DebugDrawLinks()
	{
		for (int i = 0; i < Grid.CellCount; i++)
		{
			int num = i * this.maxLinksPerCell;
			for (int num2 = this.Links[num].link; num2 != NavGrid.InvalidCell; num2 = this.Links[num].link)
			{
				Vector3 navPos = NavTypeHelper.GetNavPos(i, this.Links[num].startNavType);
				Vector3 navPos2 = NavTypeHelper.GetNavPos(num2, this.Links[num].endNavType);
				num++;
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
			this.debugColorLookup = new Color[9];
			for (int i = 0; i < 9; i++)
			{
				double num = (double)i / 9.0;
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

	private CellOffset[] boundingOffsets;

	public string id;

	public Action<HashSet<int>> OnNavGridUpdateComplete;

	public NavType[] ValidNavTypes;

	public NavGrid.NavTypeData[] navTypeData;

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

	public struct NavTypeData
	{
		public NavType navType;

		public Vector2 animControllerOffset;

		public bool flipX;

		public bool flipY;

		public float rotation;

		public HashedString idleAnim;
	}

	public struct Transition
	{
		public Transition(NavType start, NavType end, int x, int y, NavAxis start_axis, bool is_looping, bool loop_has_pre, bool is_escape, int cost, string anim, CellOffset[] void_offsets, CellOffset[] solid_offsets, NavOffset[] valid_nav_offsets, NavOffset[] invalid_nav_offsets, bool impassable_not_void = false)
		{
			this.id = -1;
			this.start = start;
			this.end = end;
			this.x = x;
			this.y = y;
			this.startAxis = start_axis;
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
				if (loop_has_pre)
				{
					this.preAnim = this.anim + "_pre";
				}
				this.anim += "_loop";
			}
			if (this.startAxis != NavAxis.NA)
			{
				this.anim += ((this.startAxis != NavAxis.X) ? "_y" : "_x");
			}
			this.voidOffsets = void_offsets;
			this.solidOffsets = solid_offsets;
			this.validNavOffsets = valid_nav_offsets;
			this.invalidNavOffsets = invalid_nav_offsets;
			this.impassableNotVoid = impassable_not_void;
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
			foreach (CellOffset cellOffset in this.voidOffsets)
			{
				int num2 = Grid.OffsetCell(cell, cellOffset.x, cellOffset.y);
				if (Grid.IsValidCell(num2))
				{
					if ((gridBitFields[num2] & 32) != 0)
					{
						return Grid.InvalidCell;
					}
					if (this.impassableNotVoid && (gridBitFields[num2] & 256) != 0)
					{
						return Grid.InvalidCell;
					}
				}
			}
			foreach (CellOffset cellOffset2 in this.solidOffsets)
			{
				int num3 = Grid.OffsetCell(cell, cellOffset2.x, cellOffset2.y);
				if (Grid.IsValidCell(num3) && (gridBitFields[num3] & 32) == 0)
				{
					return Grid.InvalidCell;
				}
			}
			foreach (NavOffset navOffset in this.validNavOffsets)
			{
				int num4 = Grid.OffsetCell(cell, navOffset.offset.x, navOffset.offset.y);
				if (!nav_table.IsValid(num4, navOffset.navType))
				{
					return Grid.InvalidCell;
				}
			}
			foreach (NavOffset navOffset2 in this.invalidNavOffsets)
			{
				int num5 = Grid.OffsetCell(cell, navOffset2.offset.x, navOffset2.offset.y);
				if (nav_table.IsValid(num5, navOffset2.navType))
				{
					return Grid.InvalidCell;
				}
			}
			if (this.start == NavType.Tube)
			{
				if (this.end == NavType.Tube)
				{
					GameObject gameObject = Grid.Objects[cell, 9];
					GameObject gameObject2 = Grid.Objects[num, 9];
					TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink = ((!gameObject) ? null : gameObject.GetComponent<TravelTubeUtilityNetworkLink>());
					TravelTubeUtilityNetworkLink travelTubeUtilityNetworkLink2 = ((!gameObject2) ? null : gameObject2.GetComponent<TravelTubeUtilityNetworkLink>());
					if (travelTubeUtilityNetworkLink)
					{
						int num6;
						int num7;
						travelTubeUtilityNetworkLink.GetCells(out num6, out num7);
						if (num != num6 && num != num7)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections utilityConnections = UtilityConnectionsExtensions.DirectionFromToCell(cell, num);
						if (utilityConnections == (UtilityConnections)0)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections connections = Game.Instance.travelTubeSystem.GetConnections(num, false);
						if (connections != utilityConnections)
						{
							return Grid.InvalidCell;
						}
					}
					else if (travelTubeUtilityNetworkLink2)
					{
						int num8;
						int num9;
						travelTubeUtilityNetworkLink2.GetCells(out num8, out num9);
						if (cell != num8 && cell != num9)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections utilityConnections2 = UtilityConnectionsExtensions.DirectionFromToCell(num, cell);
						if (utilityConnections2 == (UtilityConnections)0)
						{
							return Grid.InvalidCell;
						}
						UtilityConnections connections2 = Game.Instance.travelTubeSystem.GetConnections(cell, false);
						if (connections2 != utilityConnections2)
						{
							return Grid.InvalidCell;
						}
					}
					else
					{
						bool flag = this.startAxis == NavAxis.X;
						int num10 = cell;
						for (int m = 0; m < 2; m++)
						{
							bool flag2 = (flag && m == 0) || (!flag && m == 1);
							if (flag2)
							{
								int num11 = ((this.x <= 0) ? (-1) : 1);
								for (int n = 0; n < Mathf.Abs(this.x); n++)
								{
									UtilityConnections connections3 = Game.Instance.travelTubeSystem.GetConnections(num10, false);
									if (num11 > 0 && (connections3 & UtilityConnections.Right) == (UtilityConnections)0)
									{
										return Grid.InvalidCell;
									}
									if (num11 < 0 && (connections3 & UtilityConnections.Left) == (UtilityConnections)0)
									{
										return Grid.InvalidCell;
									}
									num10 = Grid.OffsetCell(num10, num11, 0);
								}
							}
							else
							{
								int num12 = ((this.y <= 0) ? (-1) : 1);
								for (int num13 = 0; num13 < Mathf.Abs(this.y); num13++)
								{
									UtilityConnections connections4 = Game.Instance.travelTubeSystem.GetConnections(num10, false);
									if (num12 > 0 && (connections4 & UtilityConnections.Up) == (UtilityConnections)0)
									{
										return Grid.InvalidCell;
									}
									if (num12 < 0 && (connections4 & UtilityConnections.Down) == (UtilityConnections)0)
									{
										return Grid.InvalidCell;
									}
									num10 = Grid.OffsetCell(num10, 0, num12);
								}
							}
						}
					}
				}
				else
				{
					UtilityConnections connections5 = Game.Instance.travelTubeSystem.GetConnections(cell, false);
					if (this.y > 0)
					{
						if (connections5 != UtilityConnections.Down)
						{
							return Grid.InvalidCell;
						}
					}
					else if (this.x > 0)
					{
						if (connections5 != UtilityConnections.Left)
						{
							return Grid.InvalidCell;
						}
					}
					else if (this.x < 0)
					{
						if (connections5 != UtilityConnections.Right)
						{
							return Grid.InvalidCell;
						}
					}
					else
					{
						if (this.y >= 0)
						{
							return Grid.InvalidCell;
						}
						if (connections5 != UtilityConnections.Up)
						{
							return Grid.InvalidCell;
						}
					}
				}
			}
			else if (this.start == NavType.Floor && this.end == NavType.Tube)
			{
				int num14 = Grid.OffsetCell(cell, this.x, this.y);
				UtilityConnections connections6 = Game.Instance.travelTubeSystem.GetConnections(num14, false);
				if (connections6 != UtilityConnections.Up)
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

		public NavAxis startAxis;

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

		public bool impassableNotVoid;
	}
}
