using System;
using System.Collections.Generic;
using UnityEngine;

public class FloodTool : InterfaceTool
{
	public HashSet<int> Flood(int startCell)
	{
		HashSet<int> hashSet = new HashSet<int>();
		HashSet<int> hashSet2 = new HashSet<int>();
		GameUtil.FloodFillConditional(startCell, this.floodCriteria, hashSet, hashSet2);
		return hashSet2;
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

	public Func<int, bool> floodCriteria;

	public Action<HashSet<int>> paintArea;

	protected Color32 areaColour = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	protected int mouseCell = -1;
}
