using System;
using UnityEngine;

public class CopySettingsTool : DragTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		CopySettingsTool.Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	public void SetSourceObject(GameObject sourceGameObject)
	{
		this.sourceGameObject = sourceGameObject;
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		if (!(this.sourceGameObject == null))
		{
			if (Grid.IsValidCell(cell))
			{
				CopyBuildingSettings.ApplyCopy(cell, this.sourceGameObject);
			}
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		this.sourceGameObject = null;
	}

	public GameObject Placer;

	public static CopySettingsTool Instance;

	private GameObject sourceGameObject;
}
