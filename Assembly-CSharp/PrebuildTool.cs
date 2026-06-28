using System;
using UnityEngine;

public class PrebuildTool : InterfaceTool
{
	protected override void OnPrefabInit()
	{
		PrebuildTool.Instance = this;
	}

	protected override void OnActivateTool()
	{
		this.viewMode = this.def.ViewMode;
		base.OnActivateTool();
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
	}

	public void Activate(BuildingDef def, PlanScreen.RequirementsState reqState)
	{
		this.def = def;
		PlayerController.Instance.ActivateTool(this);
		PrebuildToolHoverTextCard component = base.GetComponent<PrebuildToolHoverTextCard>();
		component.currentReqState = reqState;
		component.currentDef = def;
		component.ConfigureHoverScreen();
	}

	public void Deactivate()
	{
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		base.OnKeyDown(e);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		UISounds.PlaySound(UISounds.Sound.Negative);
		base.OnLeftClickDown(cursor_pos);
	}

	[SerializeField]
	private TextStyleSetting tooltipStyle;

	public static PrebuildTool Instance;

	private BuildingDef def;
}
