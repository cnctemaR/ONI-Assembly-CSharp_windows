using System;
using UnityEngine;

public class DeconstructTool : FilteredDragTool
{
	public static void DestroyInstance()
	{
		DeconstructTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		DeconstructTool.Instance = this;
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override string GetConfirmSound()
	{
		return "Tile_Confirm_NegativeTool";
	}

	protected override string GetDragSound()
	{
		return "Tile_Drag_NegativeTool";
	}

	protected override void OnDragTool(int cell, int distFromOrigin)
	{
		this.DeconstructCell(cell);
	}

	public void DeconstructCell(int cell)
	{
		for (int i = 0; i < 45; i++)
		{
			GameObject gameObject = Grid.Objects[cell, i];
			if (gameObject != null)
			{
				string filterLayerFromGameObject = this.GetFilterLayerFromGameObject(gameObject);
				if (base.IsActiveLayer(filterLayerFromGameObject))
				{
					gameObject.Trigger(-790448070, null);
					Prioritizable component = gameObject.GetComponent<Prioritizable>();
					if (component != null)
					{
						component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
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

	public static DeconstructTool Instance;
}
