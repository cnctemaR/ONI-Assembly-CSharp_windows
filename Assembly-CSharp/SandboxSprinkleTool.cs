using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxSprinkleTool : BrushTool
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
		SandboxSprinkleTool.instance = this;
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
		SandboxToolParameterMenu.instance.noiseScaleSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.noiseDensitySlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.massSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.temperatureSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.elementSelector.row.SetActive(true);
		SandboxToolParameterMenu.instance.diseaseSelector.row.SetActive(true);
		SandboxToolParameterMenu.instance.diseaseCountSlider.row.SetActive(true);
		SandboxToolParameterMenu.instance.brushRadiusSlider.SetValue(5f);
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
			Color color = new Color(this.recentAffectedCellColor[num].r, this.recentAffectedCellColor[num].g, this.recentAffectedCellColor[num].b, MathUtil.ReRange(Mathf.Sin(Time.realtimeSinceStartup * 5f), -1f, 1f, 0.1f, 0.2f));
			colors.Add(new ToolMenu.CellColorData(num, color));
		}
		foreach (int num2 in this.cellsInRadius)
		{
			if (this.recentlyAffectedCells.Contains(num2))
			{
				Color radiusIndicatorColor = this.radiusIndicatorColor;
				Color color2 = this.recentAffectedCellColor[num2];
				color2.a = 0.2f;
				Color color3 = new Color((radiusIndicatorColor.r + color2.r) / 2f, (radiusIndicatorColor.g + color2.g) / 2f, (radiusIndicatorColor.b + color2.b) / 2f, radiusIndicatorColor.a + (1f - radiusIndicatorColor.a) * color2.a);
				colors.Add(new ToolMenu.CellColorData(num2, color3));
			}
			else
			{
				colors.Add(new ToolMenu.CellColorData(num2, this.radiusIndicatorColor));
			}
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
				float num = Vector2.Distance(new Vector2((float)i, (float)j), new Vector2((float)this.brushRadius, (float)this.brushRadius));
				if (num < (float)this.brushRadius - 0.8f)
				{
					Vector2 vector = Grid.CellToXY(Grid.OffsetCell(this.currentCell, i, j));
					float num2 = PerlinSimplexNoise.noise(vector.x / this.settings.NoiseDensity, vector.y / this.settings.NoiseDensity, Time.realtimeSinceStartup);
					if (this.settings.NoiseScale <= num2)
					{
						this.brushOffsets.Add(new Vector2((float)(i - this.brushRadius), (float)(j - this.brushRadius)));
					}
				}
			}
		}
	}

	private void Update()
	{
		this.OnMouseMove(Grid.CellToPos(this.currentCell));
	}

	public override void OnMouseMove(Vector3 cursorPos)
	{
		base.OnMouseMove(cursorPos);
		this.SetBrushSize(this.settings.BrushSize);
	}

	protected override void OnPaintCell(int cell, int distFromOrigin)
	{
		base.OnPaintCell(cell, distFromOrigin);
		this.recentlyAffectedCells.Add(cell);
		if (!this.recentAffectedCellColor.ContainsKey(cell))
		{
			this.recentAffectedCellColor.Add(cell, this.settings.Element.substance.debugColour);
		}
		else
		{
			this.recentAffectedCellColor[cell] = this.settings.Element.substance.debugColour;
		}
		Game.CallbackInfo callbackInfo = new Game.CallbackInfo(delegate
		{
			this.recentlyAffectedCells.Remove(cell);
			this.recentAffectedCellColor.Remove(cell);
		}, false);
		int index = Game.Instance.callbackManager.Add(callbackInfo).index;
		int cell2 = cell;
		SimHashes id = this.settings.Element.id;
		CellElementEvent sandBoxTool = CellEventLogger.Instance.SandBoxTool;
		float mass = this.settings.Mass;
		float temperature = this.settings.temperature;
		int num = index;
		SimMessages.ReplaceElement(cell2, id, sandBoxTool, mass, temperature, Db.Get().Diseases.GetIndex(this.settings.Disease.IdHash), this.settings.diseaseCount, num);
		this.SetBrushSize(this.brushRadius);
	}

	public static SandboxSprinkleTool instance;

	protected HashSet<int> recentlyAffectedCells = new HashSet<int>();

	private Dictionary<int, Color> recentAffectedCellColor = new Dictionary<int, Color>();
}
