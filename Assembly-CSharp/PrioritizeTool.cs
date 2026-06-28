using System;
using UnityEngine;

public class PrioritizeTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		PrioritizeTool.Instance = this;
		this.visualizer = Util.KInstantiate(this.visualizer, SceneOrganizer.Instance.GetFolder(Folder.Placers), null);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		int screenPriority = ToolMenuPriorityScreen.Instance.GetScreenPriority();
		int num = 0;
		for (int i = 0; i < 23; i++)
		{
			GameObject gameObject = Grid.Objects[cell, i];
			if (gameObject != null)
			{
				Prioritizable component = gameObject.GetComponent<Prioritizable>();
				if (component != null && component.showIcon)
				{
					component.SetMasterPriority(screenPriority);
					num++;
				}
			}
		}
		if (num > 0)
		{
			ToolMenuPriorityScreen.Instance.PlayPriorityConfirmSound(screenPriority);
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		ToolMenuPriorityScreen.Instance.Show(true);
		this.viewMode = SimViewMode.Priorities;
		SimDebugView.Instance.SetMode(SimViewMode.Priorities);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenuPriorityScreen.Instance.Show(false);
		if (SimDebugView.Instance.GetMode() == SimViewMode.Priorities)
		{
			SimDebugView.Instance.SetMode(SimViewMode.None);
		}
	}

	public GameObject Placer;

	public static PrioritizeTool Instance;
}
