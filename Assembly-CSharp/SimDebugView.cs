using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using TUNING;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

[AddComponentMenu("KMonoBehaviour/scripts/SimDebugView")]
public class SimDebugView : KMonoBehaviour
{
	public static void DestroyInstance()
	{
		SimDebugView.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		SimDebugView.Instance = this;
		this.material = global::UnityEngine.Object.Instantiate<Material>(this.material);
		this.diseaseMaterial = global::UnityEngine.Object.Instantiate<Material>(this.diseaseMaterial);
	}

	protected override void OnSpawn()
	{
		SimDebugViewCompositor.Instance.material.SetColor("_Color0", GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[0].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color1", GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[1].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color2", GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[2].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color3", GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[3].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color4", GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[4].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color5", GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[5].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color6", GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[6].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color7", GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[7].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color0", GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[0].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color1", GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[1].colorName));
		SimDebugViewCompositor.Instance.material.SetColor("_Color2", GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[2].colorName));
		this.SetMode(global::OverlayModes.None.ID);
	}

	public void OnReset()
	{
		this.plane = SimDebugView.CreatePlane("SimDebugView", base.transform);
		this.tex = SimDebugView.CreateTexture(out this.texBytes, Grid.WidthInCells, Grid.HeightInCells);
		this.plane.GetComponent<Renderer>().sharedMaterial = this.material;
		this.plane.GetComponent<Renderer>().sharedMaterial.mainTexture = this.tex;
		this.plane.transform.SetLocalPosition(new Vector3(0f, 0f, -6f));
		this.SetMode(global::OverlayModes.None.ID);
	}

	public static Texture2D CreateTexture(int width, int height)
	{
		return new Texture2D(width, height)
		{
			name = "SimDebugView",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
	}

	public static Texture2D CreateTexture(out byte[] textureBytes, int width, int height)
	{
		textureBytes = new byte[width * height * 4];
		return new Texture2D(width, height, TextureUtil.TextureFormatToGraphicsFormat(TextureFormat.RGBA32), TextureCreationFlags.None)
		{
			name = "SimDebugView",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
	}

	public static Texture2D CreateTexture(int width, int height, Color col)
	{
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = col;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	public static GameObject CreatePlane(string layer, Transform parent)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "overlayViewDisplayPlane";
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layer));
		gameObject.transform.SetParent(parent);
		gameObject.transform.SetPosition(Vector3.zero);
		gameObject.AddComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		meshFilter.mesh = mesh;
		int num = 4;
		Vector3[] array = new Vector3[num];
		Vector2[] array2 = new Vector2[num];
		int[] array3 = new int[6];
		float num2 = 2f * (float)Grid.HeightInCells;
		array = new Vector3[]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3((float)Grid.WidthInCells, 0f, 0f),
			new Vector3(0f, num2, 0f),
			new Vector3(Grid.WidthInMeters, num2, 0f)
		};
		array2 = new Vector2[]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(0f, 2f),
			new Vector2(1f, 2f)
		};
		array3 = new int[] { 0, 2, 1, 1, 2, 3 };
		mesh.vertices = array;
		mesh.uv = array2;
		mesh.triangles = array3;
		Vector2 vector = new Vector2((float)Grid.WidthInCells, num2);
		mesh.bounds = new Bounds(new Vector3(0.5f * vector.x, 0.5f * vector.y, 0f), new Vector3(vector.x, vector.y, 0f));
		return gameObject;
	}

	private void Update()
	{
		if (this.plane == null)
		{
			return;
		}
		bool flag = this.mode != global::OverlayModes.None.ID;
		this.plane.SetActive(flag);
		SimDebugViewCompositor.Instance.Toggle(flag && !GameUtil.IsCapturingTimeLapse());
		SimDebugViewCompositor.Instance.material.SetVector("_Thresholds0", new Vector4(0.1f, 0.2f, 0.3f, 0.4f));
		SimDebugViewCompositor.Instance.material.SetVector("_Thresholds1", new Vector4(0.5f, 0.6f, 0.7f, 0.8f));
		float num = 0f;
		if (this.mode == global::OverlayModes.ThermalConductivity.ID || this.mode == global::OverlayModes.Temperature.ID)
		{
			num = 1f;
		}
		SimDebugViewCompositor.Instance.material.SetVector("_ThresholdParameters", new Vector4(num, this.thresholdRange, this.thresholdOpacity, 0f));
		if (flag)
		{
			this.UpdateData(this.tex, this.texBytes, this.mode, 192);
		}
	}

	private static void SetDefaultBilinear(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.material;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Bilinear;
	}

	private static void SetDefaultPoint(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.material;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Point;
	}

	private static void SetDisease(SimDebugView instance, Texture texture)
	{
		Renderer component = instance.plane.GetComponent<Renderer>();
		component.sharedMaterial = instance.diseaseMaterial;
		component.sharedMaterial.mainTexture = instance.tex;
		texture.filterMode = FilterMode.Bilinear;
	}

	public void UpdateData(Texture2D texture, byte[] textureBytes, HashedString viewMode, byte alpha)
	{
		Action<SimDebugView, Texture> action;
		if (!this.dataUpdateFuncs.TryGetValue(viewMode, out action))
		{
			action = new Action<SimDebugView, Texture>(SimDebugView.SetDefaultPoint);
		}
		action(this, texture);
		int num;
		int num2;
		int num3;
		int num4;
		Grid.GetVisibleExtents(out num, out num2, out num3, out num4);
		this.selectedNavigator = null;
		KSelectable selected = SelectTool.Instance.selected;
		if (selected != null)
		{
			this.selectedNavigator = selected.GetComponent<Navigator>();
		}
		this.updateSimViewWorkItems.Reset(new SimDebugView.UpdateSimViewSharedData(this, this.texBytes, viewMode, this));
		int num5 = 16;
		for (int i = num2; i <= num4; i += num5)
		{
			int num6 = Math.Min(i + num5 - 1, num4);
			this.updateSimViewWorkItems.Add(new SimDebugView.UpdateSimViewWorkItem(num, i, num3, num6));
		}
		this.currentFrame = Time.frameCount;
		this.selectedCell = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		GlobalJobManager.Run(this.updateSimViewWorkItems);
		texture.LoadRawTextureData(textureBytes);
		texture.Apply();
	}

	public void SetGameGridMode(SimDebugView.GameGridMode mode)
	{
		this.gameGridMode = mode;
	}

	public SimDebugView.GameGridMode GetGameGridMode()
	{
		return this.gameGridMode;
	}

	public void SetMode(HashedString mode)
	{
		this.mode = mode;
		Game.Instance.gameObject.BoxingTrigger(1798162660, mode);
	}

	public HashedString GetMode()
	{
		return this.mode;
	}

	public static Color TemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float num = Mathf.Clamp((temperature - minTempExpected) / (maxTempExpected - minTempExpected), 0f, 1f);
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, 1f, 1f);
	}

	public static Color LiquidTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float num = (temperature - minTempExpected) / (maxTempExpected - minTempExpected);
		float num2 = Mathf.Clamp(num, 0.5f, 1f);
		float num3 = Mathf.Clamp(num, 0f, 1f);
		return Color.HSVToRGB((10f + (1f - num2) * 171f) / 360f, num3, 1f);
	}

	public static Color SolidTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float num = Mathf.Clamp((temperature - minTempExpected) / (maxTempExpected - minTempExpected), 0.5f, 1f);
		float num2 = 1f;
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, num2, 1f);
	}

	public static Color GasTemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float num = Mathf.Clamp((temperature - minTempExpected) / (maxTempExpected - minTempExpected), 0f, 0.5f);
		float num2 = 1f;
		return Color.HSVToRGB((10f + (1f - num) * 171f) / 360f, num2, 1f);
	}

	public Color NormalizedTemperature(float actualTemperature)
	{
		float num = this.user_temperatureThresholds[0];
		float num2 = this.user_temperatureThresholds[1];
		float num3 = num2 - num;
		if (actualTemperature < num)
		{
			return GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[0].colorName);
		}
		if (actualTemperature > num2)
		{
			return GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[this.temperatureThresholds.Length - 1].colorName);
		}
		int num4 = 0;
		float num5 = 0f;
		Game.TemperatureOverlayModes temperatureOverlayMode = Game.Instance.temperatureOverlayMode;
		if (temperatureOverlayMode != Game.TemperatureOverlayModes.AbsoluteTemperature)
		{
			if (temperatureOverlayMode == Game.TemperatureOverlayModes.RelativeTemperature)
			{
				float num6 = num;
				for (int i = 0; i < SimDebugView.relativeTemperatureColorIntervals.Length; i++)
				{
					if (actualTemperature < num6 + SimDebugView.relativeTemperatureColorIntervals[i] * num3)
					{
						num4 = i;
						break;
					}
					num6 += SimDebugView.relativeTemperatureColorIntervals[i] * num3;
				}
				num5 = (actualTemperature - num6) / (SimDebugView.relativeTemperatureColorIntervals[num4] * num3);
			}
		}
		else
		{
			float num7 = num;
			for (int j = 0; j < SimDebugView.absoluteTemperatureColorIntervals.Length; j++)
			{
				if (actualTemperature < num7 + SimDebugView.absoluteTemperatureColorIntervals[j])
				{
					num4 = j;
					break;
				}
				num7 += SimDebugView.absoluteTemperatureColorIntervals[j];
			}
			num5 = (actualTemperature - num7) / SimDebugView.absoluteTemperatureColorIntervals[num4];
		}
		return Color.Lerp(GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[num4].colorName), GlobalAssets.Instance.colorSet.GetColorByName(this.temperatureThresholds[num4 + 1].colorName), num5);
	}

	public Color NormalizedHeatFlow(int cell)
	{
		int num = 0;
		int num2 = 0;
		float thermalComfort = GameUtil.GetThermalComfort(GameTags.Minions.Models.Standard, cell, -DUPLICANTSTATS.STANDARD.BaseStats.DUPLICANT_BASE_GENERATION_KILOWATTS);
		for (int i = 0; i < this.heatFlowThresholds.Length; i++)
		{
			if (thermalComfort <= this.heatFlowThresholds[i].value)
			{
				num2 = i;
				break;
			}
			num = i;
			num2 = i;
		}
		float num3 = 0f;
		if (num != num2)
		{
			num3 = (thermalComfort - this.heatFlowThresholds[num].value) / (this.heatFlowThresholds[num2].value - this.heatFlowThresholds[num].value);
		}
		num3 = Mathf.Max(num3, 0f);
		num3 = Mathf.Min(num3, 1f);
		Color color = Color.Lerp(GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[num].colorName), GlobalAssets.Instance.colorSet.GetColorByName(this.heatFlowThresholds[num2].colorName), num3);
		if (Grid.Solid[cell])
		{
			color = Color.black;
		}
		return color;
	}

	private static bool IsInsulated(int cell)
	{
		return (Grid.Element[cell].state & Element.State.TemperatureInsulated) > Element.State.Vacuum;
	}

	private static Color GetDiseaseColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		if (Grid.DiseaseIdx[cell] != 255)
		{
			Disease disease = Db.Get().Diseases[(int)Grid.DiseaseIdx[cell]];
			color = GlobalAssets.Instance.colorSet.GetColorByName(disease.overlayColourName);
			color.a = SimUtil.DiseaseCountToAlpha(Grid.DiseaseCount[cell]);
		}
		else
		{
			color.a = 0f;
		}
		return color;
	}

	private static Color GetHeatFlowColour(SimDebugView instance, int cell)
	{
		return instance.NormalizedHeatFlow(cell);
	}

	private static Color GetBlack(SimDebugView instance, int cell)
	{
		return Color.black;
	}

	public static Color GetLightColour(SimDebugView instance, int cell)
	{
		Color color = GlobalAssets.Instance.colorSet.lightOverlay;
		color.a = Mathf.Clamp(Mathf.Sqrt((float)(Grid.LightIntensity[cell] + LightGridManager.previewLux[cell])) / Mathf.Sqrt(80000f), 0f, 1f);
		if (Grid.LightIntensity[cell] > DUPLICANTSTATS.STANDARD.Light.LUX_SUNBURN)
		{
			float num = ((float)Grid.LightIntensity[cell] + (float)LightGridManager.previewLux[cell] - (float)DUPLICANTSTATS.STANDARD.Light.LUX_SUNBURN) / (float)(80000 - DUPLICANTSTATS.STANDARD.Light.LUX_SUNBURN);
			num /= 10f;
			color.r += Mathf.Min(0.1f, PerlinSimplexNoise.noise(Grid.CellToPos2D(cell).x / 8f, Grid.CellToPos2D(cell).y / 8f + (float)instance.currentFrame / 32f) * num);
		}
		return color;
	}

	public static Color GetRadiationColour(SimDebugView instance, int cell)
	{
		float num = Mathf.Clamp(Mathf.Sqrt(Grid.Radiation[cell]) / 30f, 0f, 1f);
		return new Color(0.2f, 0.9f, 0.3f, num);
	}

	public static Color GetRoomsColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		if (Grid.IsValidCell(instance.selectedCell))
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
			if (cavityForCell != null && cavityForCell.room != null)
			{
				Room room = cavityForCell.room;
				color = GlobalAssets.Instance.colorSet.GetColorByName(room.roomType.category.colorName);
				color.a = 0.45f;
				if (Game.Instance.roomProber.GetCavityForCell(instance.selectedCell) == cavityForCell)
				{
					color.a += 0.3f;
				}
			}
		}
		return color;
	}

	public static Color GetJoulesColour(SimDebugView instance, int cell)
	{
		float num = Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Mass[cell] * 1000f);
		float num2 = 0.5f * num / (ElementLoader.FindElementByHash(SimHashes.SandStone).specificHeatCapacity * 294f * 1000000f);
		return Color.Lerp(Color.black, Color.red, num2);
	}

	public static Color GetNormalizedTemperatureColourMode(SimDebugView instance, int cell)
	{
		switch (Game.Instance.temperatureOverlayMode)
		{
		case Game.TemperatureOverlayModes.AbsoluteTemperature:
			return SimDebugView.GetNormalizedTemperatureColour(instance, cell);
		case Game.TemperatureOverlayModes.AdaptiveTemperature:
			return SimDebugView.GetNormalizedTemperatureColour(instance, cell);
		case Game.TemperatureOverlayModes.HeatFlow:
			return SimDebugView.GetHeatFlowColour(instance, cell);
		case Game.TemperatureOverlayModes.StateChange:
			return SimDebugView.GetStateChangeProximityColour(instance, cell);
		default:
			return SimDebugView.GetNormalizedTemperatureColour(instance, cell);
		}
	}

	public static Color GetStateChangeProximityColour(SimDebugView instance, int cell)
	{
		float num = Grid.Temperature[cell];
		Element element = Grid.Element[cell];
		float num2 = element.lowTemp;
		float num3 = element.highTemp;
		if (element.IsGas)
		{
			num3 = Mathf.Min(num2 + 150f, num3);
			return SimDebugView.GasTemperatureToColor(num, num2, num3);
		}
		if (element.IsSolid)
		{
			num2 = Mathf.Max(num3 - 150f, num2);
			return SimDebugView.SolidTemperatureToColor(num, num2, num3);
		}
		return SimDebugView.TemperatureToColor(num, num2, num3);
	}

	public static Color GetNormalizedTemperatureColour(SimDebugView instance, int cell)
	{
		float num = Grid.Temperature[cell];
		return instance.NormalizedTemperature(num);
	}

	private static Color GetGameGridColour(SimDebugView instance, int cell)
	{
		Color color = new Color32(0, 0, 0, byte.MaxValue);
		switch (instance.gameGridMode)
		{
		case SimDebugView.GameGridMode.GameSolidMap:
			color = (Grid.Solid[cell] ? Color.white : Color.black);
			break;
		case SimDebugView.GameGridMode.Lighting:
			color = ((Grid.LightCount[cell] > 0 || LightGridManager.previewLux[cell] > 0) ? Color.white : Color.black);
			break;
		case SimDebugView.GameGridMode.DigAmount:
			if (Grid.Element[cell].IsSolid)
			{
				float num = Grid.Damage[cell] / 255f;
				color = Color.HSVToRGB(1f - num, 1f, 1f);
			}
			break;
		case SimDebugView.GameGridMode.DupePassable:
			color = (Grid.DupePassable[cell] ? Color.white : Color.black);
			break;
		}
		return color;
	}

	public Color32 GetColourForID(int id)
	{
		return this.networkColours[id % this.networkColours.Length];
	}

	private static Color GetThermalConductivityColour(SimDebugView instance, int cell)
	{
		bool flag = SimDebugView.IsInsulated(cell);
		Color black = Color.black;
		float num = instance.maxThermalConductivity - instance.minThermalConductivity;
		if (!flag && num != 0f)
		{
			float num2 = (Grid.Element[cell].thermalConductivity - instance.minThermalConductivity) / num;
			num2 = Mathf.Max(num2, 0f);
			num2 = Mathf.Min(num2, 1f);
			black = new Color(num2, num2, num2);
		}
		return black;
	}

	private static Color GetPressureMapColour(SimDebugView instance, int cell)
	{
		Color32 color = Color.black;
		if (Grid.Pressure[cell] > 0f)
		{
			float num = Mathf.Clamp((Grid.Pressure[cell] - instance.minPressureExpected) / (instance.maxPressureExpected - instance.minPressureExpected), 0f, 1f) * 0.9f;
			color = new Color(num, num, num, 1f);
		}
		return color;
	}

	private static Color GetOxygenMapColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		if (!Grid.IsLiquid(cell) && !Grid.Solid[cell])
		{
			if (Grid.Mass[cell] > SimDebugView.minimumBreathable && (Grid.Element[cell].id == SimHashes.Oxygen || Grid.Element[cell].id == SimHashes.ContaminatedOxygen))
			{
				float num = Mathf.Clamp((Grid.Mass[cell] - SimDebugView.minimumBreathable) / SimDebugView.optimallyBreathable, 0f, 1f);
				color = instance.breathableGradient.Evaluate(num);
			}
			else
			{
				color = instance.unbreathableColour;
			}
		}
		return color;
	}

	private static Color GetTileColour(SimDebugView instance, int cell)
	{
		float num = 0.33f;
		Color color = new Color(num, num, num);
		Element element = Grid.Element[cell];
		bool flag = false;
		foreach (Tag tag in Game.Instance.tileOverlayFilters)
		{
			if (element.HasTag(tag))
			{
				flag = true;
			}
		}
		if (flag)
		{
			color = element.substance.uiColour;
		}
		return color;
	}

	private static Color GetTileTypeColour(SimDebugView instance, int cell)
	{
		return Grid.Element[cell].substance.uiColour;
	}

	private static Color GetStateMapColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		switch (Grid.Element[cell].state & Element.State.Solid)
		{
		case Element.State.Gas:
			color = Color.yellow;
			break;
		case Element.State.Liquid:
			color = Color.green;
			break;
		case Element.State.Solid:
			color = Color.blue;
			break;
		}
		return color;
	}

	private static Color GetSolidLiquidMapColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		switch (Grid.Element[cell].state & Element.State.Solid)
		{
		case Element.State.Liquid:
			color = Color.green;
			break;
		case Element.State.Solid:
			color = Color.blue;
			break;
		}
		return color;
	}

	private static Color GetStateChangeColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		Element element = Grid.Element[cell];
		if (!element.IsVacuum)
		{
			float num = Grid.Temperature[cell];
			float num2 = element.lowTemp * 0.05f;
			float num3 = Mathf.Abs(num - element.lowTemp) / num2;
			float num4 = element.highTemp * 0.05f;
			float num5 = Mathf.Abs(num - element.highTemp) / num4;
			float num6 = Mathf.Max(0f, 1f - Mathf.Min(num3, num5));
			color = Color.Lerp(Color.black, Color.red, num6);
		}
		return color;
	}

	private static Color GetDecorColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		if (!Grid.Solid[cell])
		{
			float num = GameUtil.GetDecorAtCell(cell) / 100f;
			if (num > 0f)
			{
				color = Color.Lerp(GlobalAssets.Instance.colorSet.decorBaseline, GlobalAssets.Instance.colorSet.decorPositive, Mathf.Abs(num));
			}
			else
			{
				color = Color.Lerp(GlobalAssets.Instance.colorSet.decorBaseline, GlobalAssets.Instance.colorSet.decorNegative, Mathf.Abs(num));
			}
		}
		return color;
	}

	private static Color GetDangerColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		SimDebugView.DangerAmount dangerAmount = SimDebugView.DangerAmount.None;
		if (!Grid.Element[cell].IsSolid)
		{
			float num = 0f;
			if (Grid.Temperature[cell] < SimDebugView.minMinionTemperature)
			{
				num = Mathf.Abs(Grid.Temperature[cell] - SimDebugView.minMinionTemperature);
			}
			if (Grid.Temperature[cell] > SimDebugView.maxMinionTemperature)
			{
				num = Mathf.Abs(Grid.Temperature[cell] - SimDebugView.maxMinionTemperature);
			}
			if (num > 0f)
			{
				if (num < 10f)
				{
					dangerAmount = SimDebugView.DangerAmount.VeryLow;
				}
				else if (num < 30f)
				{
					dangerAmount = SimDebugView.DangerAmount.Low;
				}
				else if (num < 100f)
				{
					dangerAmount = SimDebugView.DangerAmount.Moderate;
				}
				else if (num < 200f)
				{
					dangerAmount = SimDebugView.DangerAmount.High;
				}
				else if (num < 400f)
				{
					dangerAmount = SimDebugView.DangerAmount.VeryHigh;
				}
				else if (num > 800f)
				{
					dangerAmount = SimDebugView.DangerAmount.Extreme;
				}
			}
		}
		if (dangerAmount < SimDebugView.DangerAmount.VeryHigh && (Grid.Element[cell].IsVacuum || (Grid.Element[cell].IsGas && (Grid.Element[cell].id != SimHashes.Oxygen || Grid.Pressure[cell] < SimDebugView.minMinionPressure))))
		{
			dangerAmount++;
		}
		if (dangerAmount != SimDebugView.DangerAmount.None)
		{
			float num2 = (float)dangerAmount / 6f;
			color = Color.HSVToRGB((80f - num2 * 80f) / 360f, 1f, 1f);
		}
		return color;
	}

	private static Color GetSimCheckErrorMapColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		Element element = Grid.Element[cell];
		float num = Grid.Mass[cell];
		float num2 = Grid.Temperature[cell];
		if (float.IsNaN(num) || float.IsNaN(num2) || num > 10000f || num2 > 10000f)
		{
			return Color.red;
		}
		if (element.IsVacuum)
		{
			if (num2 != 0f)
			{
				color = Color.yellow;
			}
			else if (num != 0f)
			{
				color = Color.blue;
			}
			else
			{
				color = Color.gray;
			}
		}
		else if (num2 < 10f)
		{
			color = Color.red;
		}
		else if (Grid.Mass[cell] < 1f && Grid.Pressure[cell] < 1f)
		{
			color = Color.green;
		}
		else if (num2 > element.highTemp + 3f && element.highTempTransition != null)
		{
			color = Color.magenta;
		}
		else if (num2 < element.lowTemp + 3f && element.lowTempTransition != null)
		{
			color = Color.cyan;
		}
		return color;
	}

	private static Color GetFakeFloorColour(SimDebugView instance, int cell)
	{
		if (!Grid.FakeFloor[cell])
		{
			return Color.black;
		}
		return Color.cyan;
	}

	private static Color GetFoundationColour(SimDebugView instance, int cell)
	{
		if (!Grid.Foundation[cell])
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetDupePassableColour(SimDebugView instance, int cell)
	{
		if (!Grid.DupePassable[cell])
		{
			return Color.black;
		}
		return Color.green;
	}

	private static Color GetCritterImpassableColour(SimDebugView instance, int cell)
	{
		if (!Grid.CritterImpassable[cell])
		{
			return Color.black;
		}
		return Color.yellow;
	}

	private static Color GetDupeImpassableColour(SimDebugView instance, int cell)
	{
		if (!Grid.DupeImpassable[cell])
		{
			return Color.black;
		}
		return Color.red;
	}

	private static Color GetMinionOccupiedColour(SimDebugView instance, int cell)
	{
		if (!(Grid.Objects[cell, 0] != null))
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetMinionGroupProberColour(SimDebugView instance, int cell)
	{
		if (!MinionGroupProber.Get().IsReachable(cell))
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetPathProberColour(SimDebugView instance, int cell)
	{
		if (!(instance.selectedNavigator != null) || instance.selectedNavigator.PathGrid.GetCost(cell) == -1)
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetReservedColour(SimDebugView instance, int cell)
	{
		if (!Grid.Reserved[cell])
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetAllowPathFindingColour(SimDebugView instance, int cell)
	{
		if (!Grid.AllowPathfinding[cell])
		{
			return Color.black;
		}
		return Color.white;
	}

	private static Color GetMassColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		if (!SimDebugView.IsInsulated(cell))
		{
			float num = Grid.Mass[cell];
			if (num > 0f)
			{
				float num2 = (num - SimDebugView.Instance.minMassExpected) / (SimDebugView.Instance.maxMassExpected - SimDebugView.Instance.minMassExpected);
				color = Color.HSVToRGB(1f - num2, 1f, 1f);
			}
		}
		return color;
	}

	public static Color GetScenePartitionerColour(SimDebugView instance, int cell)
	{
		if (!GameScenePartitioner.Instance.DoDebugLayersContainItemsOnCell(cell))
		{
			return Color.black;
		}
		return Color.white;
	}

	[SerializeField]
	public Material material;

	public Material diseaseMaterial;

	public bool hideFOW;

	public const int colourSize = 4;

	private byte[] texBytes;

	private int currentFrame;

	[SerializeField]
	private Texture2D tex;

	[SerializeField]
	private GameObject plane;

	private HashedString mode = global::OverlayModes.Power.ID;

	private SimDebugView.GameGridMode gameGridMode = SimDebugView.GameGridMode.DigAmount;

	private Navigator selectedNavigator;

	public float minTempExpected = 173.15f;

	public float maxTempExpected = 423.15f;

	public float minMassExpected = 1.0001f;

	public float maxMassExpected = 10000f;

	public float minPressureExpected = 1.300003f;

	public float maxPressureExpected = 201.3f;

	public float minThermalConductivity;

	public float maxThermalConductivity = 30f;

	public float thresholdRange = 0.001f;

	public float thresholdOpacity = 0.8f;

	public static float minimumBreathable = 0.05f;

	public static float optimallyBreathable = 1f;

	public SimDebugView.ColorThreshold[] temperatureThresholds;

	public Vector2 user_temperatureThresholds = Vector2.zero;

	public SimDebugView.ColorThreshold[] heatFlowThresholds;

	public Color32[] networkColours;

	public Gradient breathableGradient = new Gradient();

	public Color32 unbreathableColour = new Color(0.5f, 0f, 0f);

	public Color32[] toxicColour = new Color32[]
	{
		new Color(0.5f, 0f, 0.5f),
		new Color(1f, 0f, 1f)
	};

	public static SimDebugView Instance;

	private WorkItemCollection<SimDebugView.UpdateSimViewWorkItem, SimDebugView.UpdateSimViewSharedData> updateSimViewWorkItems = new WorkItemCollection<SimDebugView.UpdateSimViewWorkItem, SimDebugView.UpdateSimViewSharedData>();

	private int selectedCell;

	private Dictionary<HashedString, Action<SimDebugView, Texture>> dataUpdateFuncs = new Dictionary<HashedString, Action<SimDebugView, Texture>>
	{
		{
			global::OverlayModes.Temperature.ID,
			new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear)
		},
		{
			global::OverlayModes.Oxygen.ID,
			new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear)
		},
		{
			global::OverlayModes.Decor.ID,
			new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear)
		},
		{
			global::OverlayModes.TileMode.ID,
			new Action<SimDebugView, Texture>(SimDebugView.SetDefaultPoint)
		},
		{
			global::OverlayModes.Disease.ID,
			new Action<SimDebugView, Texture>(SimDebugView.SetDisease)
		}
	};

	private static float[] relativeTemperatureColorIntervals = new float[] { 0.4f, 0.05f, 0.05f, 0.05f, 0.05f, 0.2f, 0.2f };

	private static float[] absoluteTemperatureColorIntervals = new float[] { 273.15f, 10f, 10f, 10f, 7f, 63f, 1700f, 10000f };

	private Dictionary<HashedString, Func<SimDebugView, int, Color>> getColourFuncs = new Dictionary<HashedString, Func<SimDebugView, int, Color>>
	{
		{
			global::OverlayModes.ThermalConductivity.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetThermalConductivityColour)
		},
		{
			global::OverlayModes.Temperature.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetNormalizedTemperatureColourMode)
		},
		{
			global::OverlayModes.Disease.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetDiseaseColour)
		},
		{
			global::OverlayModes.Decor.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetDecorColour)
		},
		{
			global::OverlayModes.Oxygen.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetOxygenMapColour)
		},
		{
			global::OverlayModes.Light.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetLightColour)
		},
		{
			global::OverlayModes.Radiation.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetRadiationColour)
		},
		{
			global::OverlayModes.Rooms.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetRoomsColour)
		},
		{
			global::OverlayModes.TileMode.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetTileColour)
		},
		{
			global::OverlayModes.Suit.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetBlack)
		},
		{
			global::OverlayModes.Priorities.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetBlack)
		},
		{
			global::OverlayModes.Crop.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetBlack)
		},
		{
			global::OverlayModes.Harvest.ID,
			new Func<SimDebugView, int, Color>(SimDebugView.GetBlack)
		},
		{
			SimDebugView.OverlayModes.GameGrid,
			new Func<SimDebugView, int, Color>(SimDebugView.GetGameGridColour)
		},
		{
			SimDebugView.OverlayModes.StateChange,
			new Func<SimDebugView, int, Color>(SimDebugView.GetStateChangeColour)
		},
		{
			SimDebugView.OverlayModes.SimCheckErrorMap,
			new Func<SimDebugView, int, Color>(SimDebugView.GetSimCheckErrorMapColour)
		},
		{
			SimDebugView.OverlayModes.Foundation,
			new Func<SimDebugView, int, Color>(SimDebugView.GetFoundationColour)
		},
		{
			SimDebugView.OverlayModes.FakeFloor,
			new Func<SimDebugView, int, Color>(SimDebugView.GetFakeFloorColour)
		},
		{
			SimDebugView.OverlayModes.DupePassable,
			new Func<SimDebugView, int, Color>(SimDebugView.GetDupePassableColour)
		},
		{
			SimDebugView.OverlayModes.DupeImpassable,
			new Func<SimDebugView, int, Color>(SimDebugView.GetDupeImpassableColour)
		},
		{
			SimDebugView.OverlayModes.CritterImpassable,
			new Func<SimDebugView, int, Color>(SimDebugView.GetCritterImpassableColour)
		},
		{
			SimDebugView.OverlayModes.MinionGroupProber,
			new Func<SimDebugView, int, Color>(SimDebugView.GetMinionGroupProberColour)
		},
		{
			SimDebugView.OverlayModes.PathProber,
			new Func<SimDebugView, int, Color>(SimDebugView.GetPathProberColour)
		},
		{
			SimDebugView.OverlayModes.Reserved,
			new Func<SimDebugView, int, Color>(SimDebugView.GetReservedColour)
		},
		{
			SimDebugView.OverlayModes.AllowPathFinding,
			new Func<SimDebugView, int, Color>(SimDebugView.GetAllowPathFindingColour)
		},
		{
			SimDebugView.OverlayModes.Danger,
			new Func<SimDebugView, int, Color>(SimDebugView.GetDangerColour)
		},
		{
			SimDebugView.OverlayModes.MinionOccupied,
			new Func<SimDebugView, int, Color>(SimDebugView.GetMinionOccupiedColour)
		},
		{
			SimDebugView.OverlayModes.Pressure,
			new Func<SimDebugView, int, Color>(SimDebugView.GetPressureMapColour)
		},
		{
			SimDebugView.OverlayModes.TileType,
			new Func<SimDebugView, int, Color>(SimDebugView.GetTileTypeColour)
		},
		{
			SimDebugView.OverlayModes.State,
			new Func<SimDebugView, int, Color>(SimDebugView.GetStateMapColour)
		},
		{
			SimDebugView.OverlayModes.SolidLiquid,
			new Func<SimDebugView, int, Color>(SimDebugView.GetSolidLiquidMapColour)
		},
		{
			SimDebugView.OverlayModes.Mass,
			new Func<SimDebugView, int, Color>(SimDebugView.GetMassColour)
		},
		{
			SimDebugView.OverlayModes.Joules,
			new Func<SimDebugView, int, Color>(SimDebugView.GetJoulesColour)
		},
		{
			SimDebugView.OverlayModes.ScenePartitioner,
			new Func<SimDebugView, int, Color>(SimDebugView.GetScenePartitionerColour)
		}
	};

	public static readonly Color[] dbColours = new Color[]
	{
		new Color(0f, 0f, 0f, 0f),
		new Color(1f, 1f, 1f, 0.3f),
		new Color(0.7058824f, 0.8235294f, 1f, 0.2f),
		new Color(0f, 0.3137255f, 1f, 0.3f),
		new Color(0.7058824f, 1f, 0.7058824f, 0.5f),
		new Color(0.078431375f, 1f, 0f, 0.7f),
		new Color(1f, 0.9019608f, 0.7058824f, 0.9f),
		new Color(1f, 0.8235294f, 0f, 0.9f),
		new Color(1f, 0.7176471f, 0.3019608f, 0.9f),
		new Color(1f, 0.41568628f, 0f, 0.9f),
		new Color(1f, 0.7058824f, 0.7058824f, 1f),
		new Color(1f, 0f, 0f, 1f),
		new Color(1f, 0f, 0f, 1f)
	};

	private static float minMinionTemperature = 260f;

	private static float maxMinionTemperature = 310f;

	private static float minMinionPressure = 80f;

	public static class OverlayModes
	{
		public static readonly HashedString Mass = "Mass";

		public static readonly HashedString Pressure = "Pressure";

		public static readonly HashedString GameGrid = "GameGrid";

		public static readonly HashedString ScenePartitioner = "ScenePartitioner";

		public static readonly HashedString ConduitUpdates = "ConduitUpdates";

		public static readonly HashedString Flow = "Flow";

		public static readonly HashedString StateChange = "StateChange";

		public static readonly HashedString SimCheckErrorMap = "SimCheckErrorMap";

		public static readonly HashedString DupePassable = "DupePassable";

		public static readonly HashedString Foundation = "Foundation";

		public static readonly HashedString FakeFloor = "FakeFloor";

		public static readonly HashedString CritterImpassable = "CritterImpassable";

		public static readonly HashedString DupeImpassable = "DupeImpassable";

		public static readonly HashedString MinionGroupProber = "MinionGroupProber";

		public static readonly HashedString PathProber = "PathProber";

		public static readonly HashedString Reserved = "Reserved";

		public static readonly HashedString AllowPathFinding = "AllowPathFinding";

		public static readonly HashedString Danger = "Danger";

		public static readonly HashedString MinionOccupied = "MinionOccupied";

		public static readonly HashedString TileType = "TileType";

		public static readonly HashedString State = "State";

		public static readonly HashedString SolidLiquid = "SolidLiquid";

		public static readonly HashedString Joules = "Joules";
	}

	public enum GameGridMode
	{
		GameSolidMap,
		Lighting,
		RoomMap,
		Style,
		PlantDensity,
		DigAmount,
		DupePassable
	}

	[Serializable]
	public struct ColorThreshold
	{
		public string colorName;

		public float value;
	}

	private struct UpdateSimViewSharedData
	{
		public UpdateSimViewSharedData(SimDebugView instance, byte[] texture_bytes, HashedString sim_view_mode, SimDebugView sim_debug_view)
		{
			this.instance = instance;
			this.textureBytes = texture_bytes;
			this.simViewMode = sim_view_mode;
			this.simDebugView = sim_debug_view;
		}

		public SimDebugView instance;

		public HashedString simViewMode;

		public SimDebugView simDebugView;

		public byte[] textureBytes;
	}

	private struct UpdateSimViewWorkItem : IWorkItem<SimDebugView.UpdateSimViewSharedData>
	{
		public UpdateSimViewWorkItem(int x0, int y0, int x1, int y1)
		{
			this.x0 = Mathf.Clamp(x0, 0, Grid.WidthInCells - 1);
			this.x1 = Mathf.Clamp(x1, 0, Grid.WidthInCells - 1);
			this.y0 = Mathf.Clamp(y0, 0, Grid.HeightInCells - 1);
			this.y1 = Mathf.Clamp(y1, 0, Grid.HeightInCells - 1);
		}

		public void Run(SimDebugView.UpdateSimViewSharedData shared_data, int threadIndex)
		{
			Func<SimDebugView, int, Color> func;
			if (!shared_data.instance.getColourFuncs.TryGetValue(shared_data.simViewMode, out func))
			{
				func = new Func<SimDebugView, int, Color>(SimDebugView.GetBlack);
			}
			for (int i = this.y0; i <= this.y1; i++)
			{
				int num = Grid.XYToCell(this.x0, i);
				int num2 = Grid.XYToCell(this.x1, i);
				for (int j = num; j <= num2; j++)
				{
					int num3 = j * 4;
					if (Grid.IsActiveWorld(j))
					{
						Color color = func(shared_data.instance, j);
						shared_data.textureBytes[num3] = (byte)(Mathf.Min(color.r, 1f) * 255f);
						shared_data.textureBytes[num3 + 1] = (byte)(Mathf.Min(color.g, 1f) * 255f);
						shared_data.textureBytes[num3 + 2] = (byte)(Mathf.Min(color.b, 1f) * 255f);
						shared_data.textureBytes[num3 + 3] = (byte)(Mathf.Min(color.a, 1f) * 255f);
					}
					else
					{
						shared_data.textureBytes[num3] = 0;
						shared_data.textureBytes[num3 + 1] = 0;
						shared_data.textureBytes[num3 + 2] = 0;
						shared_data.textureBytes[num3 + 3] = 0;
					}
				}
			}
		}

		private int x0;

		private int y0;

		private int x1;

		private int y1;
	}

	public enum DangerAmount
	{
		None,
		VeryLow,
		Low,
		Moderate,
		High,
		VeryHigh,
		Extreme,
		MAX_DANGERAMOUNT = 6
	}
}
