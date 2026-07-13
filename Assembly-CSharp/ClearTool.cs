using System;
using UnityEngine;

public class ClearTool : DragTool
{
	public static void DestroyInstance()
	{
		ClearTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ClearTool.Instance = this;
		this.interceptNumberKeysForPriority = true;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		GameObject gameObject = Grid.Objects[cell, 3];
		if (gameObject == null)
		{
			return;
		}
		ObjectLayerListItem objectLayerListItem = gameObject.GetComponent<Pickupable>().objectLayerListItem;
		while (objectLayerListItem != null)
		{
			GameObject gameObject2 = objectLayerListItem.gameObject;
			Pickupable pickupable = objectLayerListItem.pickupable;
			objectLayerListItem = objectLayerListItem.nextItem;
			if (!(gameObject2 == null) && !pickupable.KPrefabID.HasTag(GameTags.BaseMinion) && pickupable.Clearable.isClearable)
			{
				pickupable.Clearable.MarkForClear(false, false);
				Prioritizable component = gameObject2.GetComponent<Prioritizable>();
				if (component != null)
				{
					component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
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

	public static ClearTool Instance;
}
