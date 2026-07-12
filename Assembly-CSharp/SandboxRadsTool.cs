using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxRadsTool : BrushTool
{
	public static void DestroyInstance()
	{
		SandboxRadsTool.instance = null;
	}

	public override string[] DlcIDs
	{
		get
		{
			return DlcManager.AVAILABLE_EXPANSION1_ONLY;
		}
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
		SandboxRadsTool.instance = this;
		this.viewMode = OverlayModes.Radiation.ID;
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
		SandboxToolParameterMenu.instance.radiationAdditiveSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.radiationAdditiveSlider.SetValue(5f, true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(false);
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int num in this.recentlyAffectedCells)
		{
			colors.Add(new ToolMenu.CellColorData(num, this.recentlyAffectedCellColor));
		}
		foreach (int num2 in this.cellsInRadius)
		{
			colors.Add(new ToolMenu.CellColorData(num2, this.radiusIndicatorColor));
		}
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
	}

	private static void OnSimConsumeRadiationCallback(Sim.ConsumedRadiationCallback radiationCBInfo, object data)
	{
		((SandboxRadsTool)data).recentlyAffectedCells.Remove(radiationCBInfo.gameCell);
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		if (this.recentlyAffectedCells.Contains(cell))
		{
			return;
		}
		this.recentlyAffectedCells.Add(cell);
		float num2;
		float num = Mathf.Clamp((num2 = Grid.Radiation[cell]) + SandboxToolParameterMenu.instance.settings.GetFloatSetting("SandbosTools.RadiationAdditive"), 0f, 8999999f) - num2;
		SimMessages.ModifyRadiationOnCell(cell, num, Game.Instance.radiationConsumedCallbackManager.Add(new Action<Sim.ConsumedRadiationCallback, object>(SandboxRadsTool.OnSimConsumeRadiationCallback), this, "SandboxRadTools").index);
	}

	public static SandboxRadsTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);
}
