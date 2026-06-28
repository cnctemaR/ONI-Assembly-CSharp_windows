using System;
using UnityEngine;

public static class NavTypeHelper
{
	public static Vector3 GetNavPos(int cell, NavType nav_type)
	{
		Vector3 vector = Vector3.zero;
		switch (nav_type)
		{
		case NavType.Floor:
			vector = Grid.CellToPosCBC(cell, Grid.SceneLayer.Move);
			break;
		case NavType.LeftWall:
			vector = Grid.CellToPosLCC(cell, Grid.SceneLayer.Move);
			break;
		case NavType.RightWall:
			vector = Grid.CellToPosRCC(cell, Grid.SceneLayer.Move);
			break;
		case NavType.Ceiling:
			vector = Grid.CellToPosCTC(cell, Grid.SceneLayer.Move);
			break;
		case NavType.Ladder:
			vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.Move);
			break;
		default:
			vector = Grid.CellToPosCCC(cell, Grid.SceneLayer.Move);
			break;
		}
		return vector;
	}

	public static int GetAnchorCell(NavType nav_type, int cell)
	{
		int num = Grid.InvalidCell;
		if (Grid.IsValidCell(cell))
		{
			switch (nav_type)
			{
			case NavType.Floor:
				num = Grid.CellBelow(cell);
				break;
			case NavType.LeftWall:
				num = Grid.CellLeft(cell);
				break;
			case NavType.RightWall:
				num = Grid.CellRight(cell);
				break;
			case NavType.Ceiling:
				num = Grid.CellAbove(cell);
				break;
			}
		}
		return num;
	}
}
