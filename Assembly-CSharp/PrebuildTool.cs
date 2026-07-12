using System;
using UnityEngine;

public class PrebuildTool : InterfaceTool
{
	public static void DestroyInstance()
	{
		PrebuildTool.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		PrebuildTool.Instance = this;
	}

	protected override void OnActivateTool()
	{
		this.viewMode = this.def.ViewMode;
		base.OnActivateTool();
	}

	public void Activate(BuildingDef def, string errorMessage)
	{
		this.def = def;
		PlayerController.Instance.ActivateTool(this);
		PrebuildToolHoverTextCard component = base.GetComponent<PrebuildToolHoverTextCard>();
		component.errorMessage = errorMessage;
		component.currentDef = def;
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		UISounds.PlaySound(UISounds.Sound.Negative);
		base.OnLeftClickDown(cursor_pos);
	}

	public static PrebuildTool Instance;

	private BuildingDef def;
}
