using System;
using System.Collections.Generic;
using UnityEngine;

public class HarvestTool : DragTool
{
	public static void DestroyInstance()
	{
		HarvestTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		HarvestTool.Instance = this;
		this.options.Add("HARVEST_WHEN_READY", ToolParameterMenu.ToggleState.On);
		this.options.Add("DO_NOT_HARVEST", ToolParameterMenu.ToggleState.Off);
		this.viewMode = OverlayModes.Harvest.ID;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (Grid.IsValidCell(cell))
		{
			foreach (Harvestable harvestable in Components.Harvestables.Items)
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
						component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
					}
				}
			}
		}
	}

	public void Update()
	{
		MeshRenderer componentInChildren = this.visualizer.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren != null)
		{
			if (this.options["HARVEST_WHEN_READY"] == ToolParameterMenu.ToggleState.On)
			{
				componentInChildren.material.mainTexture = this.visualizerTextures[0];
			}
			else if (this.options["DO_NOT_HARVEST"] == ToolParameterMenu.ToggleState.On)
			{
				componentInChildren.material.mainTexture = this.visualizerTextures[1];
			}
		}
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		base.OnLeftClickUp(cursor_pos);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.Show(true);
		ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.options);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
		ToolMenu.Instance.toolParameterMenu.ClearMenu();
	}

	public GameObject Placer;

	public static HarvestTool Instance;

	public Texture2D[] visualizerTextures;

	private Dictionary<string, ToolParameterMenu.ToggleState> options = new Dictionary<string, ToolParameterMenu.ToggleState>();
}
