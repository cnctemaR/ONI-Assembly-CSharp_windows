using System;
using System.Collections.Generic;
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
		}
		else if (heading.x > 0f)
		{
			anim.FlipX = false;
		}
	}

	public static void FlipAnim(KBatchedAnimController anim, Vector3 heading)
	{
		if (heading.x < 0f)
		{
			anim.FlipX = true;
		}
		else if (heading.x > 0f)
		{
			anim.FlipX = false;
		}
	}

	public static Vector3 GetWalkMoveTarget(Transform transform, Vector2 Heading)
	{
		int num = Grid.PosToCell(transform.position);
		if (Heading.x == 1f)
		{
			if (CreatureHelpers.isClear(Grid.CellRight(num)) && CreatureHelpers.isClear(Grid.CellDownRight(num)) && CreatureHelpers.isClear(Grid.CellRight(Grid.CellRight(num))) && !CreatureHelpers.isClear(Grid.PosToCell(transform.position + Vector3.right * 2f + Vector3.down)))
			{
				return transform.position + Vector3.right * 2f;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.CellRight(num),
				Grid.CellDownRight(num)
			}) && !CreatureHelpers.isClear(Grid.CellBelow(Grid.CellDownRight(num))))
			{
				return transform.position + Vector3.right + Vector3.down;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.OffsetCell(num, 1, 0),
				Grid.OffsetCell(num, 1, -1),
				Grid.OffsetCell(num, 1, -2)
			}) && !CreatureHelpers.isClear(Grid.OffsetCell(num, 1, -3)))
			{
				return transform.position + Vector3.right + Vector3.down + Vector3.down;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.OffsetCell(num, 1, 0),
				Grid.OffsetCell(num, 1, -1),
				Grid.OffsetCell(num, 1, -2),
				Grid.OffsetCell(num, 1, -3)
			}))
			{
				return transform.position;
			}
			if (CreatureHelpers.isClear(Grid.CellRight(num)))
			{
				return transform.position + Vector3.right;
			}
			if (CreatureHelpers.isClear(Grid.CellUpRight(num)) && !Grid.Solid[Grid.CellAbove(num)] && Grid.Solid[Grid.CellRight(num)])
			{
				return transform.position + Vector3.up + Vector3.right;
			}
			if (!Grid.Solid[Grid.CellAbove(num)] && !Grid.Solid[Grid.CellAbove(Grid.CellAbove(num))] && Grid.Solid[Grid.CellAbove(Grid.CellRight(num))] && CreatureHelpers.isClear(Grid.CellRight(Grid.CellAbove(Grid.CellAbove(num)))))
			{
				return transform.position + Vector3.up + Vector3.up + Vector3.right;
			}
		}
		if (Heading.x == -1f)
		{
			if (CreatureHelpers.isClear(Grid.CellLeft(num)) && CreatureHelpers.isClear(Grid.CellDownLeft(num)) && CreatureHelpers.isClear(Grid.CellLeft(Grid.CellLeft(num))) && !CreatureHelpers.isClear(Grid.PosToCell(transform.position + Vector3.left * 2f + Vector3.down)))
			{
				return transform.position + Vector3.left * 2f;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.CellLeft(num),
				Grid.CellDownLeft(num)
			}) && !CreatureHelpers.isClear(Grid.CellBelow(Grid.CellDownLeft(num))))
			{
				return transform.position + Vector3.left + Vector3.down;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.OffsetCell(num, -1, 0),
				Grid.OffsetCell(num, -1, -1),
				Grid.OffsetCell(num, -1, -2)
			}) && !CreatureHelpers.isClear(Grid.OffsetCell(num, -1, -3)))
			{
				return transform.position + Vector3.left + Vector3.down + Vector3.down;
			}
			if (CreatureHelpers.cellsAreClear(new int[]
			{
				Grid.OffsetCell(num, -1, 0),
				Grid.OffsetCell(num, -1, -1),
				Grid.OffsetCell(num, -1, -2),
				Grid.OffsetCell(num, -1, -3)
			}))
			{
				return transform.position;
			}
			if (CreatureHelpers.isClear(Grid.CellLeft(Grid.PosToCell(transform.position))))
			{
				return transform.position + Vector3.left;
			}
			if (CreatureHelpers.isClear(Grid.CellUpLeft(num)) && !Grid.Solid[Grid.CellAbove(num)] && Grid.Solid[Grid.CellLeft(num)])
			{
				return transform.position + Vector3.up + Vector3.left;
			}
			if (!Grid.Solid[Grid.CellAbove(num)] && !Grid.Solid[Grid.CellAbove(Grid.CellAbove(num))] && Grid.Solid[Grid.CellAbove(Grid.CellLeft(num))] && CreatureHelpers.isClear(Grid.CellLeft(Grid.CellAbove(Grid.CellAbove(num)))))
			{
				return transform.position + Vector3.up + Vector3.up + Vector3.left;
			}
		}
		return transform.position;
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
			Debug.LogWarning(self.name + " is trying to flee, bus has no threats");
			return null;
		}
		int num = Grid.PosToCell(threat);
		int num2 = Grid.PosToCell(self);
		Navigator nav = self.GetComponent<Navigator>();
		if (nav == null)
		{
			Debug.LogWarning(self.name + " is trying to flee, bus has no navigator component attached.");
			return null;
		}
		HashSet<int> hashSet = GameUtil.FloodCollectCells(Grid.PosToCell(self), (int cell) => CreatureHelpers.CanFleeTo(cell, nav), 300, null);
		int num3 = -1;
		int num4 = -1;
		foreach (int num5 in hashSet)
		{
			if (nav.CanReach(num5))
			{
				if (num5 != num2)
				{
					int num6 = -1;
					num6 += Grid.GetCellDistance(num5, num);
					if (CreatureHelpers.isInFavoredFleeDirection(num5, num, self))
					{
						num6 += 2;
					}
					if (num6 > num4)
					{
						num4 = num6;
						num3 = num5;
					}
				}
			}
		}
		if (num3 != -1)
		{
			return ChoreHelpers.CreateLocator("GoToLocator", Grid.CellToPos(num3));
		}
		return null;
	}

	private static bool isInFavoredFleeDirection(int targetFleeCell, int threatCell, GameObject self)
	{
		bool flag = Grid.CellToPos(threatCell).x < self.transform.position.x;
		bool flag2 = Grid.CellToPos(threatCell).x < Grid.CellToPos(targetFleeCell).x;
		return flag == flag2;
	}

	private static bool CanFleeTo(int cell, Navigator nav)
	{
		return nav.CanReach(cell) || nav.CanReach(Grid.OffsetCell(cell, -1, -1)) || nav.CanReach(Grid.OffsetCell(cell, 1, -1)) || nav.CanReach(Grid.OffsetCell(cell, -1, 1)) || nav.CanReach(Grid.OffsetCell(cell, 1, 1));
	}

	public static bool WillEngageNonEssentialTargets(GameObject self, Health.HealthState fleeThresholdState = Health.HealthState.Injured)
	{
		return self.GetSMI<ThreatMonitor.Instance>().WillEngageNonEssentialTargets();
	}
}
