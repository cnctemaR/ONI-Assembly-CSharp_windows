using System;
using UnityEngine;

public class NavPathDrawer : KMonoBehaviour
{
	public static NavPathDrawer Instance { get; private set; }

	public static void DestroyInstance()
	{
		NavPathDrawer.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		Shader shader = Shader.Find("Lines/Colored Blended");
		this.material = new Material(shader);
		NavPathDrawer.Instance = this;
	}

	protected override void OnCleanUp()
	{
		NavPathDrawer.Instance = null;
	}

	public void DrawPath(Vector3 navigator_pos, PathFinder.Path path)
	{
		this.navigatorPos = navigator_pos;
		this.navigatorPos.y = this.navigatorPos.y + 0.5f;
		this.path = path;
	}

	public Navigator GetNavigator()
	{
		return this.navigator;
	}

	public void SetNavigator(Navigator navigator)
	{
		this.navigator = navigator;
	}

	public void ClearNavigator()
	{
		this.navigator = null;
	}

	private void DrawPath(PathFinder.Path path, Vector3 navigator_pos, Color color)
	{
		if (path.nodes != null && path.nodes.Count > 1)
		{
			GL.PushMatrix();
			this.material.SetPass(0);
			GL.Begin(1);
			GL.Color(color);
			GL.Vertex(navigator_pos);
			GL.Vertex(NavTypeHelper.GetNavPos(path.nodes[1].cell, path.nodes[1].navType));
			for (int i = 1; i < path.nodes.Count - 1; i++)
			{
				Vector3 navPos = NavTypeHelper.GetNavPos(path.nodes[i].cell, path.nodes[i].navType);
				Vector3 navPos2 = NavTypeHelper.GetNavPos(path.nodes[i + 1].cell, path.nodes[i + 1].navType);
				GL.Vertex(navPos);
				GL.Vertex(navPos2);
			}
			GL.End();
			GL.PopMatrix();
		}
	}

	private void OnPostRender()
	{
		this.DrawPath(this.path, this.navigatorPos, Color.white);
		this.path = default(PathFinder.Path);
		this.DebugDrawSelectedNavigator();
		if (this.navigator != null)
		{
			GL.PushMatrix();
			this.material.SetPass(0);
			GL.Begin(1);
			PathFinderQuery pathFinderQuery = PathFinderQueries.drawNavGridQuery.Reset(null);
			this.navigator.RunQuery(pathFinderQuery);
			GL.End();
			GL.PopMatrix();
		}
	}

	private void DebugDrawSelectedNavigator()
	{
		if (!DebugHandler.DebugPathFinding)
		{
			return;
		}
		if (SelectTool.Instance == null)
		{
			return;
		}
		if (SelectTool.Instance.selected == null)
		{
			return;
		}
		Navigator component = SelectTool.Instance.selected.GetComponent<Navigator>();
		if (component == null)
		{
			return;
		}
		int mouseCell = DebugHandler.GetMouseCell();
		if (Grid.IsValidCell(mouseCell))
		{
			PathFinder.PotentialPath potentialPath = new PathFinder.PotentialPath(Grid.PosToCell(component), component.CurrentNavType, component.flags);
			PathFinder.Path path = default(PathFinder.Path);
			PathFinder.UpdatePath(component.NavGrid, component.GetCurrentAbilities(), potentialPath, PathFinderQueries.cellQuery.Reset(mouseCell), ref path);
			string text = string.Empty;
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Source: ",
				Grid.PosToCell(component),
				"\n"
			});
			text2 = text;
			text = string.Concat(new object[] { text2, "Dest: ", mouseCell, "\n" });
			text = text + "Cost: " + path.cost;
			this.DrawPath(path, component.GetComponent<KAnimControllerBase>().GetPivotSymbolPosition(), Color.green);
			DebugText.Instance.Draw(text, Grid.CellToPosCCC(mouseCell, Grid.SceneLayer.Move), Color.white);
		}
	}

	private PathFinder.Path path;

	public Material material;

	private Vector3 navigatorPos;

	private Navigator navigator;
}
