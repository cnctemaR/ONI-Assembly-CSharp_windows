using System;
using System.Collections.Generic;
using Klei;
using Klei.AI;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class SimDebugView : KMonoBehaviour
{
	public SimDebugView()
	{
		Dictionary<HashedString, Action<SimDebugView, Texture>> dictionary = new Dictionary<HashedString, Action<SimDebugView, Texture>>();
		dictionary.Add(global::OverlayModes.Temperature.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear));
		dictionary.Add(global::OverlayModes.HeatFlow.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear));
		dictionary.Add(global::OverlayModes.Oxygen.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear));
		dictionary.Add(global::OverlayModes.Decor.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDefaultBilinear));
		dictionary.Add(global::OverlayModes.Disease.ID, new Action<SimDebugView, Texture>(SimDebugView.SetDisease));
		this.dataUpdateFuncs = dictionary;
		Dictionary<HashedString, Func<SimDebugView, int, Color>> dictionary2 = new Dictionary<HashedString, Func<SimDebugView, int, Color>>();
		dictionary2.Add(global::OverlayModes.ThermalConductivity.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetThermalConductivityColour));
		dictionary2.Add(global::OverlayModes.Temperature.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetNormalizedTemperatureColour));
		dictionary2.Add(global::OverlayModes.Disease.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetDiseaseColour));
		dictionary2.Add(global::OverlayModes.HeatFlow.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetHeatFlowColour));
		dictionary2.Add(global::OverlayModes.Decor.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetDecorColour));
		dictionary2.Add(global::OverlayModes.Oxygen.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetOxygenMapColour));
		dictionary2.Add(global::OverlayModes.Light.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetLightColour));
		dictionary2.Add(global::OverlayModes.Rooms.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetRoomsColour));
		dictionary2.Add(global::OverlayModes.Suit.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetBlack));
		dictionary2.Add(global::OverlayModes.Priorities.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetBlack));
		dictionary2.Add(global::OverlayModes.Crop.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetBlack));
		dictionary2.Add(global::OverlayModes.Harvest.ID, new Func<SimDebugView, int, Color>(SimDebugView.GetBlack));
		dictionary2.Add(SimDebugView.OverlayModes.GameGrid, new Func<SimDebugView, int, Color>(SimDebugView.GetGameGridColour));
		dictionary2.Add(SimDebugView.OverlayModes.StateChange, new Func<SimDebugView, int, Color>(SimDebugView.GetStateChangeColour));
		dictionary2.Add(SimDebugView.OverlayModes.SimCheckErrorMap, new Func<SimDebugView, int, Color>(SimDebugView.GetSimCheckErrorMapColour));
		dictionary2.Add(SimDebugView.OverlayModes.ForceField, new Func<SimDebugView, int, Color>(SimDebugView.GetForceFieldColour));
		dictionary2.Add(SimDebugView.OverlayModes.MinionGroupProber, new Func<SimDebugView, int, Color>(SimDebugView.GetMinionGroupProberColour));
		dictionary2.Add(SimDebugView.OverlayModes.PathProber, new Func<SimDebugView, int, Color>(SimDebugView.GetPathProberColour));
		dictionary2.Add(SimDebugView.OverlayModes.Reserved, new Func<SimDebugView, int, Color>(SimDebugView.GetReservedColour));
		dictionary2.Add(SimDebugView.OverlayModes.AllowPathFinding, new Func<SimDebugView, int, Color>(SimDebugView.GetAllowPathFindingColour));
		dictionary2.Add(SimDebugView.OverlayModes.Danger, new Func<SimDebugView, int, Color>(SimDebugView.GetDangerColour));
		dictionary2.Add(SimDebugView.OverlayModes.MinionOccupied, new Func<SimDebugView, int, Color>(SimDebugView.GetMinionOccupiedColour));
		dictionary2.Add(SimDebugView.OverlayModes.Pressure, new Func<SimDebugView, int, Color>(SimDebugView.GetPressureMapColour));
		dictionary2.Add(SimDebugView.OverlayModes.TileType, new Func<SimDebugView, int, Color>(SimDebugView.GetTileTypeColour));
		dictionary2.Add(SimDebugView.OverlayModes.State, new Func<SimDebugView, int, Color>(SimDebugView.GetStateMapColour));
		dictionary2.Add(SimDebugView.OverlayModes.SolidLiquid, new Func<SimDebugView, int, Color>(SimDebugView.GetSolidLiquidMapColour));
		dictionary2.Add(SimDebugView.OverlayModes.Mass, new Func<SimDebugView, int, Color>(SimDebugView.GetMassColour));
		dictionary2.Add(SimDebugView.OverlayModes.Joules, new Func<SimDebugView, int, Color>(SimDebugView.GetJoulesColour));
		this.getColourFuncs = dictionary2;
		base..ctor();
	}

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
		SimDebugViewCompositor.Instance.material.SetColor("_Color0", this.temperatureThresholds[0].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color1", this.temperatureThresholds[1].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color2", this.temperatureThresholds[2].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color3", this.temperatureThresholds[3].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color4", this.temperatureThresholds[4].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color5", this.temperatureThresholds[5].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color6", this.temperatureThresholds[6].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color7", this.temperatureThresholds[7].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color0", this.heatFlowThresholds[0].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color1", this.heatFlowThresholds[1].color);
		SimDebugViewCompositor.Instance.material.SetColor("_Color2", this.heatFlowThresholds[2].color);
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

	public static GameObject CreatePlane(string layer, Transform parent)
	{
		GameObject gameObject = new GameObject();
		gameObject.name = "overlayViewDisplayPlane";
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layer));
		gameObject.transform.SetParent(parent);
		gameObject.transform.SetPosition(Vector3.zero);
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
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
		SimDebugViewCompositor.Instance.Toggle(this.mode != global::OverlayModes.None.ID);
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
		this.selectedPathProber = null;
		KSelectable selected = SelectTool.Instance.selected;
		if (selected != null)
		{
			this.selectedPathProber = selected.GetComponent<PathProber>();
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
		Game.Instance.gameObject.Trigger(1798162660, mode);
	}

	public HashedString GetMode()
	{
		return this.mode;
	}

	public static Color TemperatureToColor(float temperature, float minTempExpected, float maxTempExpected)
	{
		float num = (temperature - minTempExpected) / (maxTempExpected - minTempExpected);
		float num2 = Mathf.Clamp(num, 0f, 1f);
		return Color.HSVToRGB((10f + (1f - num2) * 171f) / 360f, 1f, 1f);
	}

	public Color NormalizedTemperature(float temperature)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < this.temperatureThresholds.Length; i++)
		{
			if (temperature <= this.temperatureThresholds[i].value)
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
			num3 = (temperature - this.temperatureThresholds[num].value) / (this.temperatureThresholds[num2].value - this.temperatureThresholds[num].value);
		}
		num3 = Mathf.Max(num3, 0f);
		num3 = Mathf.Min(num3, 1f);
		return Color.Lerp(this.temperatureThresholds[num].color, this.temperatureThresholds[num2].color, num3);
	}

	public Color NormalizedHeatFlow(int cell)
	{
		int num = 0;
		int num2 = 0;
		float thermalComfort = GameUtil.GetThermalComfort(cell, -0.083680004f);
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
		Color color = Color.Lerp(this.heatFlowThresholds[num].color, this.heatFlowThresholds[num2].color, num3);
		if (Grid.Solid[cell])
		{
			color = Color.black;
		}
		return color;
	}

	private static bool IsInsulated(int cell)
	{
		return (byte)(Grid.Element[cell].state & Element.State.TemperatureInsulated) != 0;
	}

	private static Color GetDiseaseColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		if (Grid.DiseaseIdx[cell] != 255)
		{
			Disease disease = Db.Get().Diseases[(int)Grid.DiseaseIdx[cell]];
			color = disease.overlayColour;
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
		Color color = new Color(0.8f, 0.7f, 0.3f, Mathf.Clamp(Mathf.Sqrt((float)(Grid.LightIntensity[cell] + LightGridManager.previewLux[cell])) / Mathf.Sqrt(80000f), 0f, 1f));
		if (Grid.LightIntensity[cell] > 71999)
		{
			float num = ((float)Grid.LightIntensity[cell] + (float)LightGridManager.previewLux[cell] - 71999f) / 8001f;
			num /= 10f;
			color.r += Mathf.Min(0.1f, PerlinSimplexNoise.noise(Grid.CellToPos2D(cell).x / 8f, Grid.CellToPos2D(cell).y / 8f + (float)instance.currentFrame / 32f) * num);
		}
		return color;
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
				color = room.roomType.category.color;
				color.a = 0.45f;
				CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(instance.selectedCell);
				if (cavityForCell2 == cavityForCell)
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
			color = ((!Grid.Solid[cell]) ? Color.black : Color.white);
			break;
		case SimDebugView.GameGridMode.Lighting:
			color = ((Grid.LightCount[cell] <= 0 && LightGridManager.previewLux[cell] <= 0) ? Color.black : Color.white);
			break;
		case SimDebugView.GameGridMode.DigAmount:
			if (Grid.Element[cell].IsSolid)
			{
				float num = Grid.Damage[cell] / 255f;
				color = Color.HSVToRGB(1f - num, 1f, 1f);
			}
			break;
		case SimDebugView.GameGridMode.ForceField:
			color = ((!Grid.ForceField[cell]) ? Color.black : Color.white);
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
			float num = (Grid.Pressure[cell] - instance.minPressureExpected) / (instance.maxPressureExpected - instance.minPressureExpected);
			float num2 = Mathf.Clamp(num, 0f, 1f);
			float num3 = num2 * 0.9f;
			color = new Color(num3, num3, num3, 1f);
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

	private static Color GetTileTypeColour(SimDebugView instance, int cell)
	{
		Element element = Grid.Element[cell];
		return element.substance.uiColour;
	}

	private static Color GetStateMapColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		switch ((byte)(Grid.Element[cell].state & Element.State.Solid))
		{
		case 1:
			color = Color.yellow;
			break;
		case 2:
			color = Color.green;
			break;
		case 3:
			color = Color.blue;
			break;
		}
		return color;
	}

	private static Color GetSolidLiquidMapColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		Element.State state = Grid.Element[cell].state & Element.State.Solid;
		if (state != Element.State.Vacuum)
		{
			if (state != Element.State.Solid)
			{
				if (state == Element.State.Liquid)
				{
					color = Color.green;
				}
			}
			else
			{
				color = Color.blue;
			}
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
			float num3 = Mathf.Abs(num - element.lowTemp);
			float num4 = num3 / num2;
			float num5 = element.highTemp * 0.05f;
			float num6 = Mathf.Abs(num - element.highTemp);
			float num7 = num6 / num5;
			float num8 = Mathf.Max(0f, 1f - Mathf.Min(num4, num7));
			color = Color.Lerp(Color.black, Color.red, num8);
		}
		return color;
	}

	private static Color GetDecorColour(SimDebugView instance, int cell)
	{
		Color color = Color.black;
		if (!Grid.Solid[cell])
		{
			float decorAtCell = GameUtil.GetDecorAtCell(cell);
			float num = decorAtCell / 100f;
			if (num > 0f)
			{
				color = Color.Lerp(new Color(0.15f, 0f, 0f), new Color(0f, 1f, 0f), Mathf.Abs(num));
			}
			else
			{
				color = Color.Lerp(new Color(0.15f, 0f, 0f), new Color(1f, 0f, 0f), Mathf.Abs(num));
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

	private static Color GetForceFieldColour(SimDebugView instance, int cell)
	{
		return (!Grid.ForceField[cell]) ? Color.black : Color.white;
	}

	private static Color GetMinionOccupiedColour(SimDebugView instance, int cell)
	{
		return (!(Grid.Objects[cell, 0] != null)) ? Color.black : Color.white;
	}

	private static Color GetMinionGroupProberColour(SimDebugView instance, int cell)
	{
		bool flag = MinionGroupProber.Get().IsReachable(cell);
		return (!flag) ? Color.black : Color.white;
	}

	private static Color GetPathProberColour(SimDebugView instance, int cell)
	{
		return (!(instance.selectedPathProber != null) || instance.selectedPathProber.GetCost(cell) == -1) ? Color.black : Color.white;
	}

	private static Color GetReservedColour(SimDebugView instance, int cell)
	{
		return (!Grid.Reserved[cell]) ? Color.black : Color.white;
	}

	private static Color GetAllowPathFindingColour(SimDebugView instance, int cell)
	{
		return (!Grid.AllowPathfinding[cell]) ? Color.black : Color.white;
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

	private PathProber selectedPathProber;

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

	private Dictionary<HashedString, Action<SimDebugView, Texture>> dataUpdateFuncs;

	private Dictionary<HashedString, Func<SimDebugView, int, Color>> getColourFuncs;

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

		public static readonly HashedString ForceField = "ForceField";

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
		ForceField
	}

	[Serializable]
	public struct ColorThreshold
	{
		public Color color;

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

		public void Run(SimDebugView.UpdateSimViewSharedData shared_data)
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
					Color color = func(shared_data.instance, j);
					int num3 = j * 4;
					shared_data.textureBytes[num3] = (byte)(Mathf.Min(color.r, 1f) * 255f);
					shared_data.textureBytes[num3 + 1] = (byte)(Mathf.Min(color.g, 1f) * 255f);
					shared_data.textureBytes[num3 + 2] = (byte)(Mathf.Min(color.b, 1f) * 255f);
					shared_data.textureBytes[num3 + 3] = (byte)(Mathf.Min(color.a, 1f) * 255f);
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
