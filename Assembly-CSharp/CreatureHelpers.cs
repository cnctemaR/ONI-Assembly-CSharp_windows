using System;
using UnityEngine;

public static class CreatureHelpers
{
	public static bool isClear(int cell)
	{
		return Grid.IsValidCell(cell) && !Grid.Solid[cell] && !Grid.IsSubstantialLiquid(cell, 0.9f) && (!Grid.IsValidCell(Grid.CellBelow(cell)) || !Grid.IsLiquid(cell) || !Grid.IsLiquid(Grid.CellBelow(cell)));
	}

	public static int FindNearbyBreathableCell(int currentLocation, SimHashes breathableElement)
	{
		return currentLocation;
	}

	public static bool cellsAreClear(int[] cells)
	{
		for (int i = 0; i < cells.Length; i++)
		{
			if (!Grid.IsValidCell(cells[i]))
			{
				return false;
			}
			if (!CreatureHelpers.isClear(cells[i]))
			{
				return false;
			}
		}
		return true;
	}

	public static Vector3 PositionOfCurrentCell(Vector3 transformPosition)
	{
		return Grid.CellToPos(Grid.PosToCell(transformPosition));
	}

	public static Vector3 CenterPositionOfCell(int cell)
	{
		return Grid.CellToPos(cell) + new Vector3(0.5f, 0.5f, -2f);
	}

	public static void DeselectCreature(GameObject creature)
	{
		KSelectable component = creature.GetComponent<KSelectable>();
		if (component != null && SelectTool.Instance.selected == component)
		{
			SelectTool.Instance.Select(null, false);
		}
	}

	public static bool isSwimmable(int cell)
	{
		return Grid.IsValidCell(cell) && !Grid.Solid[cell] && Grid.IsSubstantialLiquid(cell, 0.35f);
	}

	public static bool isSolidGround(int cell)
	{
		return Grid.IsValidCell(cell) && Grid.Solid[cell];
	}

	public static void FlipAnim(KAnimControllerBase anim, Vector3 heading)
	{
		if (heading.x < 0f)
		{
			anim.FlipX = true;
			return;
		}
		if (heading.x > 0f)
		{
			anim.FlipX = false;
		}
	}

	public static void FlipAnim(KBatchedAnimController anim, Vector3 heading)
	{
		if (heading.x < 0f)
		{
			anim.FlipX = true;
			return;
		}
		if (heading.x > 0f)
		{
			anim.FlipX = false;
		}
	}

	public static Vector3 GetWalkMoveTarget(Transform transform, Vector2 Heading)
	{
		int num = Grid.PosToCell(transform.GetPosition());
		if (Heading.x == 1f)
		{
			if (CreatureHelpers.isClear(Grid.CellRight(num)) && CreatureHelpers.isClear(Grid.CellDownRight(num)) && CreatureHelpers.isClear(Grid.CellRight(Grid.CellRight(num))) && !CreatureHelpers.isClear(Grid.PosToCell(transform.GetPosition() + Vector3.right * 2f + Vector3.down)))
			{
				return transform.GetPosition() + Vector3.right * 2f;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.CellRight(num),
				Grid.CellDownRight(num)
			}) && !CreatureHelpers.isClear(Grid.CellBelow(Grid.CellDownRight(num))))
			{
				return transform.GetPosition() + Vector3.right + Vector3.down;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.OffsetCell(num, 1, 0),
				Grid.OffsetCell(num, 1, -1),
				Grid.OffsetCell(num, 1, -2)
			}) && !CreatureHelpers.isClear(Grid.OffsetCell(num, 1, -3)))
			{
				return transform.GetPosition() + Vector3.right + Vector3.down + Vector3.down;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.OffsetCell(num, 1, 0),
				Grid.OffsetCell(num, 1, -1),
				Grid.OffsetCell(num, 1, -2),
				Grid.OffsetCell(num, 1, -3)
			}))
			{
				return transform.GetPosition();
			}
			if (CreatureHelpers.isClear(Grid.CellRight(num)))
			{
				return transform.GetPosition() + Vector3.right;
			}
			if (CreatureHelpers.isClear(Grid.CellUpRight(num)) && !Grid.Solid[Grid.CellAbove(num)] && Grid.Solid[Grid.CellRight(num)])
			{
				return transform.GetPosition() + Vector3.up + Vector3.right;
			}
			if (!Grid.Solid[Grid.CellAbove(num)] && !Grid.Solid[Grid.CellAbove(Grid.CellAbove(num))] && Grid.Solid[Grid.CellAbove(Grid.CellRight(num))] && CreatureHelpers.isClear(Grid.CellRight(Grid.CellAbove(Grid.CellAbove(num)))))
			{
				return transform.GetPosition() + Vector3.up + Vector3.up + Vector3.right;
			}
		}
		if (Heading.x == -1f)
		{
			if (CreatureHelpers.isClear(Grid.CellLeft(num)) && CreatureHelpers.isClear(Grid.CellDownLeft(num)) && CreatureHelpers.isClear(Grid.CellLeft(Grid.CellLeft(num))) && !CreatureHelpers.isClear(Grid.PosToCell(transform.GetPosition() + Vector3.left * 2f + Vector3.down)))
			{
				return transform.GetPosition() + Vector3.left * 2f;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.CellLeft(num),
				Grid.CellDownLeft(num)
			}) && !CreatureHelpers.isClear(Grid.CellBelow(Grid.CellDownLeft(num))))
			{
				return transform.GetPosition() + Vector3.left + Vector3.down;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.OffsetCell(num, -1, 0),
				Grid.OffsetCell(num, -1, -1),
				Grid.OffsetCell(num, -1, -2)
			}) && !CreatureHelpers.isClear(Grid.OffsetCell(num, -1, -3)))
			{
				return transform.GetPosition() + Vector3.left + Vector3.down + Vector3.down;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.OffsetCell(num, -1, 0),
				Grid.OffsetCell(num, -1, -1),
				Grid.OffsetCell(num, -1, -2),
				Grid.OffsetCell(num, -1, -3)
			}))
			{
				return transform.GetPosition();
			}
			if (CreatureHelpers.isClear(Grid.CellLeft(Grid.PosToCell(transform.GetPosition()))))
			{
				return transform.GetPosition() + Vector3.left;
			}
			if (CreatureHelpers.isClear(Grid.CellUpLeft(num)) && !Grid.Solid[Grid.CellAbove(num)] && Grid.Solid[Grid.CellLeft(num)])
			{
				return transform.GetPosition() + Vector3.up + Vector3.left;
			}
			if (!Grid.Solid[Grid.CellAbove(num)] && !Grid.Solid[Grid.CellAbove(Grid.CellAbove(num))] && Grid.Solid[Grid.CellAbove(Grid.CellLeft(num))] && CreatureHelpers.isClear(Grid.CellLeft(Grid.CellAbove(Grid.CellAbove(num)))))
			{
				return transform.GetPosition() + Vector3.up + Vector3.up + Vector3.left;
			}
		}
		return transform.GetPosition();
	}

	public static bool CrewNearby(Transform transform, int range = 6)
	{
		int num = Grid.PosToCell(transform.gameObject);
		for (int i = 1; i < range; i++)
		{
			int num2 = Grid.OffsetCell(num, i, 0);
			int num3 = Grid.OffsetCell(num, -i, 0);
			if (Grid.Objects[num2, 0] != null)
			{
				return true;
			}
			if (Grid.Objects[num3, 0] != null)
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckHorizontalClear(Vector3 startPosition, Vector3 endPosition)
	{
		int num = Grid.PosToCell(startPosition);
		int num2 = 1;
		if (endPosition.x < startPosition.x)
		{
			num2 = -1;
		}
		float num3 = Mathf.Abs(endPosition.x - startPosition.x);
		int num4 = 0;
		while ((float)num4 < num3)
		{
			int num5 = Grid.OffsetCell(num, num4 * num2, 0);
			if (Grid.Solid[num5])
			{
				return false;
			}
			num4++;
		}
		return true;
	}

	public static GameObject GetFleeTargetLocatorObject(GameObject self, GameObject threat)
	{
		if (threat == null)
		{
			global::Debug.LogWarning(self.name + " is trying to flee, bus has no threats");
			return null;
		}
		CreatureHelpers.fleeThreatInfo fleeThreatInfo;
		fleeThreatInfo.threatCell = Grid.PosToCell(threat);
		fleeThreatInfo.selfCell = Grid.PosToCell(self);
		fleeThreatInfo.nav = self.GetComponent<Navigator>();
		if (fleeThreatInfo.nav == null)
		{
			global::Debug.LogWarning(self.name + " is trying to flee, bus has no navigator component attached.");
			return null;
		}
		int num = GameUtil.FloodFillFindBest<CreatureHelpers.fleeThreatInfo>(CreatureHelpers.fleeCellRater, fleeThreatInfo, CreatureHelpers.fleeCellVaidator, Grid.PosToCell(self), 300);
		if (num != -1)
		{
			return ChoreHelpers.CreateLocator("GoToLocator", Grid.CellToPos(num));
		}
		return null;
	}

	private static bool isInFavoredFleeDirection(int targetFleeCell, int threatCell, int selfCell)
	{
		bool flag = Grid.CellToPos(threatCell).x < Grid.CellToPos(selfCell).x;
		bool flag2 = Grid.CellToPos(threatCell).x < Grid.CellToPos(targetFleeCell).x;
		return flag == flag2;
	}

	private static bool CanFleeTo(int cell, Navigator nav)
	{
		return nav.GetNavigationCost(cell, OffsetGroups.Use) != -1;
	}

	private static Func<int, CreatureHelpers.fleeThreatInfo, float> fleeCellRater = (int cell, CreatureHelpers.fleeThreatInfo threat) => (float)Grid.GetCellDistance(cell, threat.threatCell) + (CreatureHelpers.isInFavoredFleeDirection(cell, threat.threatCell, threat.selfCell) ? 2f : 0f);

	private static Func<int, CreatureHelpers.fleeThreatInfo, bool> fleeCellVaidator = (int cell, CreatureHelpers.fleeThreatInfo info) => CreatureHelpers.CanFleeTo(cell, info.nav);

	private struct fleeThreatInfo
	{
		public int threatCell;

		public int selfCell;

		public Navigator nav;
	}
}
