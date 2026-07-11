using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SandboxSampleTool : InterfaceTool
{
	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		colors.Add(new ToolMenu.CellColorData(this.currentCell, this.radiusIndicatorColor));
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		this.currentCell = Grid.PosToCell(cursorPos);
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		int num = Grid.PosToCell(cursor_pos);
		if (!Grid.IsValidCell(num))
		{
			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, UI.DEBUG_TOOLS.INVALID_LOCATION, null, cursor_pos, 1.5f, false, true);
			return;
		}
		this.Sample(num);
	}

	private void Sample(int cell)
	{
		UISounds.PlaySound(UISounds.Sound.ClickObject);
		SandboxToolParameterMenu.instance.settings.Element = Grid.Element[cell];
		SandboxToolParameterMenu.instance.settings.Mass = Mathf.Round(Grid.Mass[cell] * 100f) / 100f;
		SandboxToolParameterMenu.instance.settings.temperature = Mathf.Round(Grid.Temperature[cell] * 10f) / 10f;
		SandboxToolParameterMenu.instance.settings.diseaseCount = Grid.DiseaseCount[cell];
		SandboxToolParameterMenu.instance.RefreshDisplay();
	}

	protected override void OnActivateTool()
	{
		base.OnActivateTool();
		SandboxToolParameterMenu.instance.gameObject.SetActive(true);
		SandboxToolParameterMenu.instance.DisableParameters();
		SandboxToolParameterMenu.instance.massSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.elementSelector.row.SetActive(true);
		SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(true);
		SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(true);
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(false);
	}

	protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);

	private int currentCell;
}
