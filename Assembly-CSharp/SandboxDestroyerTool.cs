using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxDestroyerTool : BrushTool
{
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

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		this.recentlyAffectedCells.Add(cell);
		Game.CallbackInfo callbackInfo = new Game.CallbackInfo(delegate
		{
			this.recentlyAffectedCells.Remove(cell);
		}, false);
		int index = Game.Instance.callbackManager.Add(callbackInfo).index;
		int cell2 = cell;
		SimHashes simHashes = SimHashes.Vacuum;
		CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
		float num = 0f;
		float num2 = 0f;
		int num3 = index;
		SimMessages.ReplaceElement(cell2, simHashes, sandBoxTool, num, num2, Db.Get().Diseases.GetIndex(this.settings.Disease.IdHash), 0, num3);
		foreach (Pickupable pickupable in Components.Pickupables)
		{
			if (Grid.PosToCell(pickupable) == cell)
			{
				Util.KDestroyGameObject(pickupable.gameObject);
			}
		}
		foreach (BuildingComplete buildingComplete in Components.BuildingCompletes)
		{
			if (Grid.PosToCell(buildingComplete) == cell)
			{
				Util.KDestroyGameObject(buildingComplete.gameObject);
			}
		}
		if (Grid.Objects[cell, 1] != null)
		{
			Util.KDestroyGameObject(Grid.Objects[cell, 1]);
		}
		foreach (Crop crop in Components.Crops)
		{
			if (Grid.PosToCell(crop) == cell)
			{
				Util.KDestroyGameObject(crop.gameObject);
			}
		}
		foreach (Health health in Components.Health)
		{
			if (Grid.PosToCell(health) == cell)
			{
				Util.KDestroyGameObject(health.gameObject);
			}
		}
	}

	public static SandboxDestroyerTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);
}
