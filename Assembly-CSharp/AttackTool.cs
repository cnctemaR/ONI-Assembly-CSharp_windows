using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		AttackTool.Instance = this;
		this.options.Add("HARVEST_WHEN_READY", ToolParameterMenu.ToggleState.On);
		this.options.Add("DO_NOT_HARVEST", ToolParameterMenu.ToggleState.Off);
		this.viewMode = SimViewMode.HarvestWhenReady;
		this.hoverScreenUpdate.tickInterval = 0.2f;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (Grid.IsValidCell(cell))
		{
			foreach (Harvestable harvestable in Components.Harvestables)
			{
				OccupyArea area = harvestable.area;
				if (Grid.PosToCell(harvestable) == cell || (area != null && area.CheckIsOccupying(cell)))
				{
					if (this.options["HARVEST_WHEN_READY"] == ToolParameterMenu.ToggleState.On)
					{
						harvestable.SetHarvestWhenReady(true);
					}
					else if (this.options["DO_NOT_HARVEST"] == ToolParameterMenu.ToggleState.On)
					{
						harvestable.SetHarvestWhenReady(false);
					}
					Prioritizable component = harvestable.GetComponent<Prioritizable>();
					if (component != null)
					{
						component.SetMasterPriority(ToolMenuPriorityScreen.Instance.GetScreenPriority());
					}
				}
			}
		}
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		base.OnLeftClickUp(cursor_pos);
	}

	protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
	{
		Vector2 regularizedPos = base.GetRegularizedPos(Vector2.Min(downPos, upPos), true);
		Vector2 regularizedPos2 = base.GetRegularizedPos(Vector2.Max(downPos, upPos), false);
		foreach (FactionAlignment factionAlignment in Components.FactionAlignments)
		{
			Vector2 vector = Grid.PosToXY(factionAlignment.transform.position);
			if (vector.x >= regularizedPos.x && vector.x < regularizedPos2.x && vector.y >= regularizedPos.y && vector.y < regularizedPos2.y && FactionManager.Instance.GetDisposition(FactionManager.FactionID.Duplicant, factionAlignment.Alignment) != FactionManager.Disposition.Assist)
			{
				factionAlignment.SetPlayerTargeted(true);
				break;
			}
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenuPriorityScreen.Instance.Show(true);
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.options);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenuPriorityScreen.Instance.Show(false);
		ToolMenu.Instance.toolParameterMenu.ClearMenu();
	}

	public GameObject Placer;

	public static AttackTool Instance;

	private Dictionary<string, ToolParameterMenu.ToggleState> options = new Dictionary<string, ToolParameterMenu.ToggleState>();
}
