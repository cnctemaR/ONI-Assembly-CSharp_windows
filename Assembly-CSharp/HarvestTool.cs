using System;
using UnityEngine;

public class HarvestTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		HarvestTool.Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (Grid.IsValidCell(cell))
		{
			foreach (Harvestable harvestable in Components.Harvestables)
			{
				if (Grid.PosToCell(harvestable) == cell)
				{
					if (DebugHandler.InstantBuildMode)
					{
						harvestable.Harvest();
					}
					else
					{
						harvestable.MarkForHarvest();
						Prioritizable component = harvestable.GetComponent<Prioritizable>();
						if (component != null)
						{
							component.SetMasterPriority(ToolMenuPriorityScreen.Instance.GetScreenPriority());
						}
					}
				}
			}
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenuPriorityScreen.Instance.Show(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenuPriorityScreen.Instance.Show(false);
	}

	public GameObject Placer;

	public static HarvestTool Instance;
}
