using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Klei.AI;
using UnityEngine;

public class SandboxBrushTool : BrushTool
{
	public static void DestroyInstance()
	{
		SandboxBrushTool.instance = null;
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
		SandboxBrushTool.instance = this;
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
		SandboxToolParameterMenu.instance.massSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.elementSelector.row.SetActive(true);
		SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(true);
		SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(true);
		SandboxToolParameterMenu.SelectorValue elementSelector = SandboxToolParameterMenu.instance.elementSelector;
		elementSelector.onValueChanged = (Action<object>)Delegate.Combine(elementSelector.onValueChanged, new Action<object>(this.OnElementChanged));
	}

	protected override void OnDeactivateTool(InterfaceTool new_tool)
	{
		base.OnDeactivateTool(new_tool);
		SandboxToolParameterMenu.instance.gameObject.SetActive(false);
		this.audioEvent.release();
	}

	public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
	{
		colors = new HashSet<ToolMenu.CellColorData>();
		foreach (int num in this.recentlyAffectedCells)
		{
			Color color = new Color(this.recentAffectedCellColor[num].r, this.recentAffectedCellColor[num].g, this.recentAffectedCellColor[num].b, MathUtil.ReRange(Mathf.Sin(Time.realtimeSinceStartup * 10f), -1f, 1f, 0.1f, 0.2f));
			colors.Add(new ToolMenu.CellColorData(num, color));
		}
		foreach (int num2 in this.cellsInRadius)
		{
			colors.Add(new ToolMenu.CellColorData(num2, this.radiusIndicatorColor));
		}
	}

	public override void SetBrushSize(int radius)
	{
		this.brushRadius = radius;
		this.brushOffsets.Clear();
		for (int i = 0; i < this.brushRadius * 2; i++)
		{
			for (int j = 0; j < this.brushRadius * 2; j++)
			{
				if (Vector2.Distance(new Vector2((float)i, (float)j), new Vector2((float)this.brushRadius, (float)this.brushRadius)) < (float)this.brushRadius - 0.8f)
				{
					this.brushOffsets.Add(new Vector2((float)(i - this.brushRadius), (float)(j - this.brushRadius)));
				}
			}
		}
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		this.recentlyAffectedCells.Add(cell);
		Element element = ElementLoader.elements[this.settings.GetIntSetting("SandboxTools.SelectedElement")];
		if (!this.recentAffectedCellColor.ContainsKey(cell))
		{
			this.recentAffectedCellColor.Add(cell, element.substance.uiColour);
		}
		else
		{
			this.recentAffectedCellColor[cell] = element.substance.uiColour;
		}
		Game.CallbackInfo callbackInfo = new Game.CallbackInfo(delegate
		{
			this.recentlyAffectedCells.Remove(cell);
			this.recentAffectedCellColor.Remove(cell);
		}, false);
		int index = Game.Instance.callbackManager.Add(callbackInfo).index;
		byte b = Db.Get().Diseases.GetIndex(Db.Get().Diseases.Get("FoodPoisoning").id);
		Disease disease = Db.Get().Diseases.TryGet(this.settings.GetStringSetting("SandboxTools.SelectedDisease"));
		if (disease != null)
		{
			b = Db.Get().Diseases.GetIndex(disease.id);
		}
		float num = ((element.id == SimHashes.Vacuum) ? 0f : this.settings.GetFloatSetting("SandboxTools.Mass"));
		int cell2 = cell;
		SimHashes id = element.id;
		CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
		float num2 = num;
		float floatSetting = this.settings.GetFloatSetting("SandbosTools.Temperature");
		int num3 = index;
		SimMessages.ReplaceElement(cell2, id, sandBoxTool, num2, floatSetting, b, this.settings.GetIntSetting("SandboxTools.DiseaseCount"), num3);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.SandboxCopyElement))
		{
			int num = Grid.PosToCell(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
			if (Grid.IsValidCell(num))
			{
				SandboxSampleTool.Sample(num);
			}
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnLeftClickDown(Vector3 cursor_pos)
	{
		base.OnLeftClickDown(cursor_pos);
		KFMOD.PlayUISound(GlobalAssets.GetSound("SandboxTool_Click", false));
	}

	public override void OnLeftClickUp(Vector3 cursor_pos)
	{
		base.OnLeftClickUp(cursor_pos);
		this.StopSound();
	}

	private void OnElementChanged(object new_element)
	{
		this.clearVisitedCells();
	}

	protected override string GetDragSound()
	{
		string text = (ElementLoader.elements[this.settings.GetIntSetting("SandboxTools.SelectedElement")].state & Element.State.Solid).ToString();
		return "SandboxTool_Brush_" + text + "_Add";
	}

	protected override void PlaySound()
	{
		base.PlaySound();
		Element element = ElementLoader.elements[this.settings.GetIntSetting("SandboxTools.SelectedElement")];
		string text;
		switch (element.state & Element.State.Solid)
		{
		case Element.State.Vacuum:
			text = GlobalAssets.GetSound("SandboxTool_Brush_Gas", false);
			break;
		case Element.State.Gas:
			text = GlobalAssets.GetSound("SandboxTool_Brush_Gas", false);
			break;
		case Element.State.Liquid:
			text = GlobalAssets.GetSound("SandboxTool_Brush_Liquid", false);
			break;
		case Element.State.Solid:
			text = GlobalAssets.GetSound("Brush_" + element.substance.GetOreBumpSound(), false);
			if (text == null)
			{
				text = GlobalAssets.GetSound("Brush_Rock", false);
			}
			break;
		default:
			text = GlobalAssets.GetSound("Brush_Rock", false);
			break;
		}
		this.audioEvent = KFMOD.CreateInstance(text);
		ATTRIBUTES_3D attributes_3D = SoundListenerController.Instance.transform.GetPosition().To3DAttributes();
		this.audioEvent.set3DAttributes(attributes_3D);
		this.audioEvent.start();
	}

	private void StopSound()
	{
		this.audioEvent.stop(global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this.audioEvent.release();
	}

	public static SandboxBrushTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	private Dictionary<int, Color> recentAffectedCellColor = new Dictionary<int, Color>();

	private EventInstance audioEvent;
}
