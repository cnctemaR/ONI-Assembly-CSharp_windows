using System;
using System.Collections.Generic;
using UnityEngine;

public class EmptyPipeTool : FilteredDragTool
{
	public static void DestroyInstance()
	{
		EmptyPipeTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		EmptyPipeTool.Instance = this;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		for (int i = 0; i < 45; i++)
		{
			if (base.IsActiveLayer((ObjectLayer)i))
			{
				GameObject gameObject = Grid.Objects[cell, i];
				if (!(gameObject == null))
				{
					IEmptyConduitWorkable component = gameObject.GetComponent<IEmptyConduitWorkable>();
					if (!component.IsNullOrDestroyed())
					{
						if (DebugHandler.InstantBuildMode)
						{
							component.EmptyContents();
						}
						else
						{
							component.MarkForEmptying();
							Prioritizable component2 = gameObject.GetComponent<Prioritizable>();
							if (component2 != null)
							{
								component2.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
							}
						}
					}
				}
			}
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenu.Instance.PriorityScreen.Show(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenu.Instance.PriorityScreen.Show(false);
	}

	protected override void GetDefaultFilters(Dictionary<string, ToolParameterMenu.ToggleState> filters)
	{
		filters.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
		filters.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.Off);
		filters.Add(ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, ToolParameterMenu.ToggleState.Off);
	}

	public static EmptyPipeTool Instance;
}
