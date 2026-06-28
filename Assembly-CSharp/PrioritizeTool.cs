using System;
using UnityEngine;

public class PrioritizeTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.interceptNumberKeysForPriority = true;
		PrioritizeTool.Instance = this;
		this.visualizer = Util.KInstantiate(this.visualizer, SceneOrganizer.Instance.GetFolder(Folder.Placers), null);
		this.viewMode = SimViewMode.Priorities;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		PrioritySetting screenPriority = ToolMenuPriorityScreen.Instance.GetScreenPriority();
		int num = 0;
		for (int i = 0; i < 32; i++)
		{
			GameObject gameObject = Grid.Objects[cell, i];
			if (gameObject != null)
			{
				Prioritizable component = gameObject.GetComponent<Prioritizable>();
				if (component != null && component.showIcon && component.IsPrioritizable())
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
		ToolMenuPriorityScreen.Instance.transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		ToolMenuPriorityScreen.Instance.Show(false);
		ToolMenuPriorityScreen.Instance.transform.localScale = new Vector3(1f, 1f, 1f);
	}

	public override void Update()
	{
		base.Update();
		PrioritySetting screenPriority = ToolMenuPriorityScreen.Instance.GetScreenPriority();
		int num = 0;
		num += screenPriority.priority_value;
		Texture2D texture2D = this.cursors[num - 1];
		MeshRenderer componentInChildren = this.visualizer.GetComponentInChildren<MeshRenderer>();
		if (componentInChildren != null)
		{
			componentInChildren.material.mainTexture = texture2D;
		}
	}

	public GameObject Placer;

	public static PrioritizeTool Instance;

	public Texture2D[] cursors;
}
