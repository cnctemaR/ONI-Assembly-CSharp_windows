using System;
using UnityEngine;

public class MoveToLocationTool : InterfaceTool
{
	public static void DestroyInstance()
	{
		MoveToLocationTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MoveToLocationTool.Instance = this;
		this.visualizer = Util.KInstantiate(this.visualizer, null, null);
	}

	public void Activate(Navigator navigator)
	{
		this.targetNavigator = navigator;
		this.targetMovable = null;
		PlayerController.Instance.ActivateTool(this);
	}

	public void Activate(Movable movable)
	{
		this.targetNavigator = null;
		this.targetMovable = movable;
		PlayerController.Instance.ActivateTool(this);
	}

	public bool CanMoveTo(int target_cell)
	{
		if (this.targetNavigator != null)
		{
			return this.targetNavigator.GetSMI<MoveToLocationMonitor.Instance>() != null && this.targetNavigator.CanReach(target_cell);
		}
		return this.targetMovable != null && this.targetMovable.CanMoveTo(target_cell);
	}

	private void SetMoveToLocation(int target_cell)
	{
		if (this.targetNavigator != null)
		{
			MoveToLocationMonitor.Instance smi = this.targetNavigator.GetSMI<MoveToLocationMonitor.Instance>();
			if (smi != null)
			{
				smi.MoveToLocation(target_cell);
				return;
			}
		}
		else if (this.targetMovable != null)
		{
			this.targetMovable.MoveToLocation(target_cell);
		}
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		this.visualizer.gameObject.SetActive(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		if (this.targetNavigator != null && new_tool == SelectTool.Instance)
		{
			SelectTool.Instance.SelectNextFrame(this.targetNavigator.GetComponent<KSelectable>(), true);
		}
		this.visualizer.gameObject.SetActive(false);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		if (this.targetNavigator != null || this.targetMovable != null)
		{
			int mouseCell = DebugHandler.GetMouseCell();
			if (this.CanMoveTo(mouseCell))
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click", false));
				this.SetMoveToLocation(mouseCell);
				SelectTool.Instance.Activate();
				return;
			}
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
		}
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

	private Movable targetMovable;
}
