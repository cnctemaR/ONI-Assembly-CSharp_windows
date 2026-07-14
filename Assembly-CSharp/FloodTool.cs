using System;
using System.Collections.Generic;
using UnityEngine;

public class FloodTool : InterfaceTool
{
	public List<int> Flood(int startCell)
	{
		List<int> list = new List<int>();
		FloodFill.DepthCollect(startCell, this.floodCriteria, list);
		return list;
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		this.paintArea(this.Flood(Grid.PosToCell(cursor_pos)));
	}

	public override void OnMouseMove(Vector3 cursor_pos)
	{
		base.OnMouseMove(cursor_pos);
		this.mouseCell = Grid.PosToCell(cursor_pos);
	}

	public Func<int, FloodFill.BoundaryCheckResult> floodCriteria;

	public Action<List<int>> paintArea;

	protected Color32 areaColour = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	protected int mouseCell = -1;
}
