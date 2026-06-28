using System;
using UnityEngine;

public class CancelTool : FilteredDragTool
{
	protected override void OnPrefabInit()
	{
		this.regionLayerMask = LayerMask.GetMask(new string[] { "Regions" });
		base.OnPrefabInit();
		CancelTool.Instance = this;
	}

	public override void ResetFilter()
	{
		base.ResetFilter();
		this.filterTargets.Add(FilteredDragTool.FILTERLAYERS.CLEANANDCLEAR, false);
		this.filterTargets.Add(FilteredDragTool.FILTERLAYERS.DIGPLACER, false);
	}

	protected override string GetConfirmSound()
	{
		return "Tile_Confirm_NegativeTool";
	}

	protected override string GetDragSound()
	{
		return "Tile_Drag_NegativeTool";
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (DragTool.layerMask != this.regionLayerMask)
		{
			for (int i = 0; i < 25; i++)
			{
				GameObject gameObject = Grid.Objects[cell, i];
				if (gameObject != null)
				{
					string filterLayerFromGameObject = base.GetFilterLayerFromGameObject(gameObject);
					if (this.filterTargets[FilteredDragTool.FILTERLAYERS.ALL] || (this.filterTargets.ContainsKey(filterLayerFromGameObject.ToUpper()) && this.filterTargets[filterLayerFromGameObject.ToUpper()]))
					{
						gameObject.Trigger(2127324410, null);
					}
				}
			}
		}
		else
		{
			Region intersectionRegion = Game.Instance.RegionManager.GetIntersectionRegion(cell);
			if (intersectionRegion != null && intersectionRegion.RemoveCell(cell, false))
			{
				Game.Instance.RegionManager.RemoveRegion(intersectionRegion, true);
			}
		}
	}

	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = base.GetRegularizedPos(Vector2.Min(downPos, upPos), true);
		Vector2 regularizedPos2 = base.GetRegularizedPos(Vector2.Max(downPos, upPos), false);
		foreach (FactionAlignment factionAlignment in Components.FactionAlignments)
		{
			Vector2 vector = Grid.PosToXY(factionAlignment.transform.position);
			if (vector.x >= regularizedPos.x && vector.x < regularizedPos2.x && vector.y >= regularizedPos.y && vector.y < regularizedPos2.y)
			{
				factionAlignment.gameObject.Trigger(2127324410, null);
			}
		}
	}

	public static CancelTool Instance;

	private int regionLayerMask;
}
