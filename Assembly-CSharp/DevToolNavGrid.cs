using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ImGuiNET;
using UnityEngine;

public class DevToolNavGrid : DevTool
{
	public DevToolNavGrid()
	{
		DevToolNavGrid.Instance = this;
		this.drawLinkTypes = new Dictionary<NavType, bool>(11);
		foreach (object obj in Enum.GetValues(typeof(NavType)))
		{
			NavType navType = (NavType)obj;
			this.drawLinkTypes.Add(navType, true);
		}
	}

	private bool Init()
	{
		if (Pathfinding.Instance == null)
		{
			return false;
		}
		if (this.navGridNames != null)
		{
			return true;
		}
		this.navGridNames = (from x in Pathfinding.Instance.GetNavGrids()
			select x.id).ToArray<string>();
		return true;
	}

	protected override void RenderTo(DevPanel panel)
	{
		if (this.Init())
		{
			this.Contents();
			return;
		}
		ImGui.Text("Game not initialized");
	}

	public void SetCell(int cell)
	{
		this.selectedCell = cell;
	}

	private void Contents()
	{
		ImGui.Combo("Nav Grid ID", ref this.selectedNavGrid, this.navGridNames, this.navGridNames.Length);
		NavGrid navGrid = Pathfinding.Instance.GetNavGrid(this.navGridNames[this.selectedNavGrid]);
		ImGui.Text("Max Links per cell: " + navGrid.maxLinksPerCell.ToString());
		ImGui.Spacing();
		int num = Marshal.SizeOf<NavGrid.Link>();
		long num2 = (long)navGrid.maxLinksPerCell * (long)Grid.CellCount * (long)num;
		long num3 = (long)Grid.CellCount * 2L;
		long num4 = (long)((Grid.CellCount + 7) / 8);
		long num5 = num2 + num3 + num4;
		ImGui.Text(string.Format("Memory usage: {0} MB (Links: {1} MB [{2}B per link], NavTable: {3} KB)", new object[]
		{
			num5 / 1048576L,
			num2 / 1048576L,
			num,
			num3 / 1024L
		}));
		if (ImGui.Button("Calculate Stats"))
		{
			this.linkStats = new int[navGrid.maxLinksPerCell];
			this.highestLinkCell = 0;
			this.highestLinkCount = 0;
			for (int i = 0; i < Grid.CellCount; i++)
			{
				int num6 = 0;
				for (int j = 0; j < navGrid.maxLinksPerCell; j++)
				{
					int num7 = i * navGrid.maxLinksPerCell + j;
					if (navGrid.Links[num7].link == Grid.InvalidCell)
					{
						break;
					}
					num6++;
				}
				if (num6 > this.highestLinkCount)
				{
					this.highestLinkCell = i;
					this.highestLinkCount = num6;
				}
				this.linkStats[num6]++;
			}
		}
		ImGui.SameLine();
		if (ImGui.Button("Clear"))
		{
			this.linkStats = null;
		}
		ImGui.SameLine();
		if (ImGui.Button("Rescan"))
		{
			navGrid.InitializeGraph();
		}
		ImGui.SameLine();
		if (ImGui.Button("Dirty") && this.selectedCell != Grid.InvalidCell)
		{
			Pathfinding.Instance.AddDirtyNavGridCell(this.selectedCell);
		}
		if (this.linkStats != null)
		{
			ImGui.Text("Highest link count: " + this.highestLinkCount.ToString());
			ImGui.Text(string.Format("Utilized percentage: {0} %", (float)this.highestLinkCount / (float)navGrid.maxLinksPerCell * 100f));
			ImGui.SameLine();
			if (ImGui.Button(string.Format("Select {0}", this.highestLinkCell)))
			{
				this.selectedCell = this.highestLinkCell;
			}
			for (int k = 0; k < this.linkStats.Length; k++)
			{
				if (this.linkStats[k] > 0)
				{
					ImGui.Text(string.Format("\t{0}: {1}", k, this.linkStats[k]));
				}
			}
		}
		if (Camera.main != null && SelectTool.Instance != null)
		{
			GameObject gameObject = null;
			ImGui.Checkbox("Lock", ref this.follow);
			ImGui.Checkbox("DrawDebugPath", ref DebugHandler.DebugPathFinding);
			if (this.follow)
			{
				if (this.lockObject == null && SelectTool.Instance.selected != null)
				{
					this.lockObject = SelectTool.Instance.selected.gameObject;
				}
				gameObject = this.lockObject;
			}
			else if (SelectTool.Instance.selected != null)
			{
				gameObject = SelectTool.Instance.selected.gameObject;
				this.lockObject = null;
			}
			if (gameObject != null)
			{
				Navigator component = gameObject.GetComponent<Navigator>();
				if (component != null)
				{
					Vector2 positionFor = DevToolEntity.GetPositionFor(component.gameObject);
					ImGui.GetBackgroundDrawList().AddCircleFilled(positionFor, 10f, ImGui.GetColorU32(Color.green));
					Vector2 screenPosition = DevToolEntity.GetScreenPosition(component.GetComponent<KBatchedAnimController>().GetPivotSymbolPosition());
					ImGui.GetBackgroundDrawList().AddCircleFilled(screenPosition, 10f, ImGui.GetColorU32(Color.blue));
					TransitionDriver transitionDriver = component.transitionDriver;
					if (transitionDriver.GetTransition != null)
					{
						Vector3 position = component.transform.GetPosition();
						Vector2 vector = gameObject.GetComponent<KBoxCollider2D>().size / 2f;
						if (transitionDriver.GetTransition.x > 0)
						{
							position.x += vector.x;
						}
						else if (transitionDriver.GetTransition.x < 0)
						{
							position.x -= vector.x;
						}
						Vector2 screenPosition2 = DevToolEntity.GetScreenPosition(position);
						ImGui.GetBackgroundDrawList().AddCircleFilled(screenPosition2, 10f, ImGui.GetColorU32(Color.magenta));
					}
					if (DebugHandler.DebugPathFinding)
					{
						int mouseCell = DebugHandler.GetMouseCell();
						if (Grid.IsValidCell(mouseCell))
						{
							PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(Grid.PosToCell(component), component.CurrentNavType, component.flags);
							PathFinder.Path path = default(PathFinder.Path);
							if (!component.PathGrid.BuildPath(component.cachedCell, mouseCell, component.CurrentNavType, ref path))
							{
								PathFinder.UpdatePath(component.NavGrid, component.GetCurrentAbilities(), potentialPath, PathFinderQueries.cellQuery.Reset(mouseCell), ref path);
							}
							if (path.nodes != null)
							{
								for (int l = 0; l < path.nodes.Count - 1; l++)
								{
									if ((int)Grid.WorldIdx[path.nodes[l].cell] == ClusterManager.Instance.activeWorldId)
									{
										NavGrid.Transition transition = navGrid.transitions[(int)path.nodes[l].transitionId];
										ImGui.Text(string.Format("   {0} -> {1} x:{2} y:{3} anim:{4} cost:{5}", new object[] { transition.start, transition.end, transition.x, transition.y, transition.anim, transition.cost }));
									}
								}
							}
							else
							{
								ImGui.Text("No valid path");
							}
						}
					}
				}
			}
		}
		ImGui.Spacing();
		ImGui.Checkbox("Draw Links", ref this.drawLinks);
		if (this.drawLinks)
		{
			ImGui.Indent();
			foreach (NavType navType in this.drawLinkTypes.Keys.ToList<NavType>())
			{
				bool flag = this.drawLinkTypes[navType];
				if (navType != NavType.NumNavTypes)
				{
					ImGui.PushID(navType.ToString());
					if (ImGui.Checkbox(navType.ToString(), ref flag))
					{
						this.drawLinkTypes[navType] = flag;
					}
					ImGui.PopID();
				}
			}
			if (ImGui.Button("Deselect All"))
			{
				foreach (object obj in Enum.GetValues(typeof(NavType)))
				{
					NavType navType2 = (NavType)obj;
					this.drawLinkTypes[navType2] = false;
				}
			}
			ImGui.SameLine();
			if (ImGui.Button("Select All"))
			{
				foreach (object obj2 in Enum.GetValues(typeof(NavType)))
				{
					NavType navType3 = (NavType)obj2;
					this.drawLinkTypes[navType3] = true;
				}
			}
			ImGui.Unindent();
			this.DebugDrawLinks(navGrid);
		}
		ImGui.Spacing();
		int num8;
		int num9;
		Grid.CellToXY(this.selectedCell, out num8, out num9);
		ImGui.Text(string.Format("Selected Cell: {0} ({1},{2})", this.selectedCell, num8, num9));
		if (Grid.IsValidCell(this.selectedCell))
		{
			this.DrawNavTypes(this.selectedCell, navGrid);
			if (navGrid.Links != null && navGrid.Links.Length > navGrid.maxLinksPerCell * this.selectedCell)
			{
				for (int m = 0; m < navGrid.maxLinksPerCell; m++)
				{
					int num10 = this.selectedCell * navGrid.maxLinksPerCell + m;
					NavGrid.Link link = navGrid.Links[num10];
					if (link.link == Grid.InvalidCell)
					{
						break;
					}
					this.DrawLink(m, link, navGrid);
				}
			}
		}
	}

	private void DrawLink(int idx, NavGrid.Link l, NavGrid navGrid)
	{
		NavGrid.Transition transition = navGrid.transitions[(int)l.transitionId];
		ImGui.Text(string.Format("   {0} -> {1} x:{2} y:{3} anim:{4} cost:{5}", new object[] { transition.start, transition.end, transition.x, transition.y, transition.anim, transition.cost }));
	}

	private void DrawNavTypes(int cell, NavGrid navGrid)
	{
		for (byte b = 0; b < 11; b += 1)
		{
			NavType navType = (NavType)b;
			if (navGrid.NavTable.IsValid(cell, navType))
			{
				ImGui.Text(string.Format("{0}", navType));
			}
		}
	}

	private void DebugDrawLinks(NavGrid navGrid)
	{
		if (Camera.main == null)
		{
			return;
		}
		Camera main = Camera.main;
		int pixelHeight = main.pixelHeight;
		Color white = Color.white;
		for (int i = 0; i < Grid.CellCount; i++)
		{
			int num = i * navGrid.maxLinksPerCell;
			for (int num2 = navGrid.Links[num].link; num2 != NavGrid.InvalidCell; num2 = navGrid.Links[num].link)
			{
				if (this.DrawNavTypeLink(navGrid, num, ref white))
				{
					Vector3 navPos = NavTypeHelper.GetNavPos(i, navGrid.Links[num].startNavType);
					Vector3 navPos2 = NavTypeHelper.GetNavPos(num2, navGrid.Links[num].endNavType);
					if (this.IsInCameraView(main, navPos) && this.IsInCameraView(main, navPos2))
					{
						Vector2 vector = main.WorldToScreenPoint(navPos);
						Vector2 vector2 = main.WorldToScreenPoint(navPos2);
						vector.y = (float)pixelHeight - vector.y;
						vector2.y = (float)pixelHeight - vector2.y;
						uint colorU = ImGui.GetColorU32(white);
						this.DrawArrowLink(vector, vector2, colorU);
					}
				}
				num++;
			}
		}
	}

	private bool IsInCameraView(Camera camera, Vector3 pos)
	{
		Vector3 vector = camera.WorldToViewportPoint(pos);
		return vector.x >= 0f && vector.y >= 0f && vector.x <= 1f && vector.y <= 1f;
	}

	private bool DrawNavTypeLink(NavGrid navGrid, int end_cell_idx, ref Color color)
	{
		for (int i = 0; i < navGrid.ValidNavTypes.Length; i++)
		{
			if (navGrid.ValidNavTypes[i] == navGrid.Links[end_cell_idx].startNavType)
			{
				color = NavGrid.NavTypeColor(navGrid.Links[end_cell_idx].startNavType);
				return this.drawLinkTypes[navGrid.Links[end_cell_idx].startNavType] || this.drawLinkTypes[navGrid.Links[end_cell_idx].endNavType];
			}
			if (navGrid.ValidNavTypes[i] == navGrid.Links[end_cell_idx].endNavType)
			{
				color = NavGrid.NavTypeColor(navGrid.Links[end_cell_idx].endNavType);
				return this.drawLinkTypes[navGrid.Links[end_cell_idx].startNavType] || this.drawLinkTypes[navGrid.Links[end_cell_idx].endNavType];
			}
		}
		return false;
	}

	private void DrawArrowLink(Vector2 start, Vector2 end, uint color)
	{
		ImDrawListPtr backgroundDrawList = ImGui.GetBackgroundDrawList();
		Vector2 vector = end - start;
		float magnitude = vector.magnitude;
		if (magnitude > 0f)
		{
			vector *= 1f / Mathf.Sqrt(magnitude);
		}
		Vector2 vector2 = end - vector * 1f + new Vector2(-vector.y, vector.x) * 1f;
		Vector2 vector3 = end - vector * 1f - new Vector2(-vector.y, vector.x) * 1f;
		backgroundDrawList.AddLine(start, end, color);
		backgroundDrawList.AddTriangleFilled(end, vector2, vector3, color);
	}

	private const string INVALID_OVERLAY_MODE_STR = "None";

	private string[] navGridNames;

	private int selectedNavGrid;

	private bool drawLinks;

	private Dictionary<NavType, bool> drawLinkTypes = new Dictionary<NavType, bool>();

	public static DevToolNavGrid Instance;

	private int[] linkStats;

	private int highestLinkCell;

	private int highestLinkCount;

	private int selectedCell = Grid.InvalidCell;

	private bool follow;

	private GameObject lockObject;
}
