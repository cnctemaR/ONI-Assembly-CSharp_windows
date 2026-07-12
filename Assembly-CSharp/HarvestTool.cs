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
			foreach (HarvestDesignatable harvestDesignatable in Components.HarvestDesignatables.Items)
			{
				OccupyArea area = harvestDesignatable.area;
				if (Grid.PosToCell(harvestDesignatable) == cell || (area != null && area.CheckIsOccupying(cell)))
				{
					if (this.options["HARVEST_WHEN_READY"] == ToolParameterMenu.ToggleState.On)
					{
						harvestDesignatable.SetHarvestWhenReady(true);
					}
					else if (this.options["DO_NOT_HARVEST"] == ToolParameterMenu.ToggleState.On)
					{
						Harvestable component = harvestDesignatable.GetComponent<Harvestable>();
						if (component != null)
						{
							component.Trigger(2127324410, null);
						}
						harvestDesignatable.SetHarvestWhenReady(false);
					}
					Prioritizable component2 = harvestDesignatable.GetComponent<Prioritizable>();
					if (component2 != null)
					{
						component2.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
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
				return;
			}
			if (this.options["DO_NOT_HARVEST"] == ToolParameterMenu.ToggleState.On)
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
