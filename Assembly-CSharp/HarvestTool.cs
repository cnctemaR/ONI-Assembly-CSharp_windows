using System;
using UnityEngine;

public class HarvestTool : DragTool
{
	public static void DestroyInstance()
	{
		HarvestTool.Instance = null;
	}

	private bool IsOptionOn(string name)
	{
		for (int i = 0; i < this.options.Length; i++)
		{
			if (this.options[i].name == name)
			{
				return this.options[i].IsOn;
			}
		}
		return false;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		HarvestTool.Instance = this;
		this.options = new ToolParameterMenu.ToggleData[]
		{
			new ToolParameterMenu.ToggleData("HARVEST_WHEN_READY", ToolParameterMenu.ToggleState.On, false),
			new ToolParameterMenu.ToggleData("DO_NOT_HARVEST", ToolParameterMenu.ToggleState.Off, false)
		};
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
					if (this.IsOptionOn("HARVEST_WHEN_READY"))
					{
						harvestDesignatable.SetHarvestWhenReady(true);
					}
					else if (this.IsOptionOn("DO_NOT_HARVEST"))
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
			if (this.IsOptionOn("HARVEST_WHEN_READY"))
			{
				componentInChildren.material.mainTexture = this.visualizerTextures[0];
				return;
			}
			if (this.IsOptionOn("DO_NOT_HARVEST"))
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

	private ToolParameterMenu.ToggleData[] options;
}
