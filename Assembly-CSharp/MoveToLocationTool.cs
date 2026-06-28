using System;
using UnityEngine;

public class MoveToLocationTool : InterfaceTool
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MoveToLocationTool.Instance = this;
		this.visualizer = Util.KInstantiate(this.visualizer, SceneOrganizer.Instance.GetFolder(Folder.Placers), null);
	}

	public void Activate(Navigator navigator)
	{
		this.targetNavigator = navigator;
		PlayerController.Instance.ActivateTool(this);
	}

	private bool CanMoveTo(int target_cell)
	{
		return this.targetNavigator.CanReach(target_cell);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		if (this.targetNavigator != null)
		{
			int mouseCell = DebugHandler.GetMouseCell();
			if (this.CanMoveTo(mouseCell))
			{
				MoveToLocationMonitor.Instance smi = this.targetNavigator.GetSMI<MoveToLocationMonitor.Instance>();
				if (smi != null)
				{
					this.targetNavigator.GetSMI<MoveToLocationMonitor.Instance>().MoveToLocation(mouseCell);
				}
			}
		}
		this.visualizer.gameObject.SetActive(false);
		SelectTool.Instance.Activate();
	}

	public override void OnRightClickUp(Vector3 cursor_pos)
	{
		base.OnRightClickUp(cursor_pos);
		SelectTool.Instance.Activate();
	}

	private void RefreshColor()
	{
		Color white = new Color(0.91f, 0.21f, 0.2f);
		if (this.CanMoveTo(DebugHandler.GetMouseCell()))
		{
			white = Color.white;
		}
		this.SetColor(this.visualizer, white);
	}

	public override void OnMouseMove(Vector3 cursor_pos)
	{
		base.OnMouseMove(cursor_pos);
		this.RefreshColor();
	}

	private void SetColor(GameObject root, Color c)
	{
		root.GetComponentInChildren<MeshRenderer>().material.color = c;
	}

	public static MoveToLocationTool Instance;

	private Navigator targetNavigator;
}
