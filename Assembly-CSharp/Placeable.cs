using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/Placeable")]
public class Placeable : KMonoBehaviour
{
	public bool IsValidPlaceLocation(int cell, out string reason)
	{
		if (this.placementRules.Contains(Placeable.PlacementRules.RestrictToWorld) && (int)Grid.WorldIdx[cell] != this.restrictWorldId)
		{
			reason = UI.TOOLS.PLACE.REASONS.RESTRICT_TO_WORLD;
			return false;
		}
		if (!this.occupyArea.CanOccupyArea(cell, this.occupyArea.objectLayers[0]))
		{
			reason = UI.TOOLS.PLACE.REASONS.CAN_OCCUPY_AREA;
			return false;
		}
		if (this.placementRules.Contains(Placeable.PlacementRules.OnFoundation))
		{
			bool flag = this.occupyArea.TestAreaBelow(cell, null, new Func<int, object, bool>(this.FoundationTest));
			if (this.checkRootCellOnly)
			{
				flag = this.FoundationTest(Grid.CellBelow(cell), null);
			}
			if (!flag)
			{
				reason = UI.TOOLS.PLACE.REASONS.ON_FOUNDATION;
				return false;
			}
		}
		if (this.placementRules.Contains(Placeable.PlacementRules.VisibleToSpace))
		{
			bool flag2 = this.occupyArea.TestArea(cell, null, new Func<int, object, bool>(this.SunnySpaceTest));
			if (this.checkRootCellOnly)
			{
				flag2 = this.SunnySpaceTest(cell, null);
			}
			if (!flag2)
			{
				reason = UI.TOOLS.PLACE.REASONS.VISIBLE_TO_SPACE;
				return false;
			}
		}
		reason = "ok!";
		return true;
	}

	private bool SunnySpaceTest(int cell, object data)
	{
		if (!Grid.IsValidCell(cell))
		{
			return false;
		}
		int num;
		int num2;
		Grid.CellToXY(cell, out num, out num2);
		int num3 = (int)Grid.WorldIdx[cell];
		if (num3 == 255)
		{
			return false;
		}
		WorldContainer world = ClusterManager.Instance.GetWorld(num3);
		int num4 = world.WorldOffset.y + world.WorldSize.y;
		return !Grid.Solid[cell] && !Grid.Foundation[cell] && (Grid.ExposedToSunlight[cell] >= 253 || this.ClearPathToSky(num, num2, num4));
	}

	private bool ClearPathToSky(int x, int startY, int top)
	{
		for (int i = startY; i < top; i++)
		{
			int num = Grid.XYToCell(x, i);
			if (Grid.Solid[num] || Grid.Foundation[num])
			{
				return false;
			}
		}
		return true;
	}

	private bool FoundationTest(int cell, object data)
	{
		return Grid.IsValidBuildingCell(cell) && (Grid.Solid[cell] || Grid.Foundation[cell]);
	}

	[MyCmpReq]
	private OccupyArea occupyArea;

	public string kAnimName;

	public string animName;

	public List<Placeable.PlacementRules> placementRules = new List<Placeable.PlacementRules>();

	[NonSerialized]
	public int restrictWorldId;

	public bool checkRootCellOnly;

	public enum PlacementRules
	{
		OnFoundation,
		VisibleToSpace,
		RestrictToWorld
	}
}
