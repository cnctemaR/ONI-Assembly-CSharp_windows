using System;
using System.Collections.Generic;
using UnityEngine;

public class CancelTool : FilteredDragTool
{
	protected override void OnPrefabInit()
	{
		this.regionLayerMask = LayerMask.GetMask(new string[] { "Regions" });
		base.OnPrefabInit();
		CancelTool.Instance = this;
	}

	protected override void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		base.GetDefaultFilters(filters);
		filters.Add(ToolParameterMenu.FILTERLAYERS.CLEANANDCLEAR, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.DIGPLACER, ToolParameterMenu.ToggleState.Off);
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
			for (int i = 0; i < 32; i++)
			{
				GameObject gameObject = Grid.Objects[cell, i];
				if (gameObject != null)
				{
					string filterLayerFromGameObject = base.GetFilterLayerFromGameObject(gameObject);
					if (base.IsActiveLayer(filterLayerFromGameObject))
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
