using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxDestroyerTool : BrushTool
{
	public static void DestroyInstance()
	{
		SandboxDestroyerTool.instance = null;
	}

	private SandboxSettings settings
	{
		get
		{
			return SandboxToolParameterMenu.instance.settings;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		SandboxDestroyerTool.instance = this;
		this.affectFoundation = true;
	}

	protected override string GetDragSound()
	{
		return "SandboxTool_Delete_Add";
	}

	public void Activate()
	{
		PlayerController.Instance.ActivateTool(this);
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(false);
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int num in this.cellsInRadius)
		{
			colors.Add(new ToolMenu.CellColorData(num, this.radiusIndicatorColor));
		}
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		KFMOD.PlayUISound(GlobalAssets.GetSound("SandboxTool_Delete", false));
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		GameUtil.DestroyCell(cell, CellEventLogger.Instance.SandBoxTool, true);
	}

	public static SandboxDestroyerTool instance;
}
