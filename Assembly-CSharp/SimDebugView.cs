using System;
using Klei;
using Klei.AI;
using UnityEngine;
using UnityEngine.Rendering;

public class SimDebugView : KMonoBehaviour
{
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
		this.SetMode(SimViewMode.None);
	}

	public void OnReset()
	{
		this.plane = SimDebugView.CreatePlane("SimDebugView", base.transform);
		this.tex = SimDebugView.CreateTexture(out this.texBytes, Grid.WidthInCells, Grid.HeightInCells);
		this.plane.GetComponent<Renderer>().sharedMaterial = this.material;
		this.plane.GetComponent<Renderer>().sharedMaterial.mainTexture = this.tex;
		this.plane.transform.localPosition = new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, -6f);
		this.SetMode(SimViewMode.None);
	}

	public static Texture2D CreateTexture(out byte[] textureBytes, int width, int height)
	{
		textureBytes = new byte[width * height * 4];
		return new Texture2D(width, height, TextureFormat.RGBA32, false)
		{
			name = "SimDebugView",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Point
		};
	}

	public static GameObject CreatePlane(string layer, Transform parent)
	{
		GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
		gameObject.name = "OverlayPlane";
		gameObject.SetLayerRecursively(LayerMask.NameToLayer(layer));
		global::UnityEngine.Object.Destroy(gameObject.GetComponent<Collider>());
		if (parent != null)
		{
			gameObject.transform.SetParent(parent);
		}
		gameObject.transform.SetPosition(Vector3.zero);
		gameObject.transform.localScale = new Vector3(Grid.WidthInMeters / -10f, 1f, Grid.HeightInMeters / -10f);
		gameObject.transform.eulerAngles = new Vector3(270f, 0f, 0f);
		gameObject.GetComponent<MeshRenderer>().reflectionProbeUsage = ReflectionProbeUsage.Off;
		return gameObject;
	}

	private void Update()
	{
		if (!(this.plane == null))
		{
			bool flag = this.mode != SimViewMode.None;
			this.plane.SetActive(flag);
			SimDebugViewCompositor.Instance.Toggle(this.mode != SimViewMode.None);
			SimDebugViewCompositor.Instance.material.SetVector("_Thresholds0", new Vector4(0.1f, 0.2f, 0.3f, 0.4f));
			SimDebugViewCompositor.Instance.material.SetVector("_Thresholds1", new Vector4(0.5f, 0.6f, 0.7f, 0.8f));
			float num = 0f;
			if (this.mode == SimViewMode.ThermalConductivity || this.mode == SimViewMode.TemperatureMap)
			{
				num = 1f;
			}
			SimDebugViewCompositor.Instance.material.SetVector("_ThresholdParameters", new Vector4(num, this.thresholdRange, this.thresholdOpacity, 0f));
			if (flag)
			{
				this.UpdateData(this.tex, this.texBytes, this.mode, 192);
			}
		}
	}

	public void UpdateData(Texture2D texture, byte[] textureBytes, SimViewMode viewMode, byte alpha)
	{
		if (viewMode != SimViewMode.HeatFlow && viewMode != SimViewMode.TemperatureMap)
		{
			if (viewMode == SimViewMode.Disease)
			{
				this.plane.GetComponent<Renderer>().sharedMaterial = this.diseaseMaterial;
				this.plane.GetComponent<Renderer>().sharedMaterial.mainTexture = this.tex;
				texture.filterMode = FilterMode.Bilinear;
				goto IL_00F4;
			}
			if (viewMode != SimViewMode.Decor && viewMode != SimViewMode.OxygenMap)
			{
				this.plane.GetComponent<Renderer>().sharedMaterial = this.material;
				this.plane.GetComponent<Renderer>().sharedMaterial.mainTexture = this.tex;
				texture.filterMode = FilterMode.Point;
				goto IL_00F4;
			}
		}
		this.plane.GetComponent<Renderer>().sharedMaterial = this.material;
		this.plane.GetComponent<Renderer>().sharedMaterial.mainTexture = this.tex;
		texture.filterMode = FilterMode.Bilinear;
		IL_00F4:
		int num;
		int num2;
		int num3;
		int num4;
		Grid.GetVisibleExtents(out num, out num2, out num3, out num4);
		for (int i = num2; i <= num4; i++)
		{
			for (int j = num; j <= num3; j++)
			{
				Color color = Color.black;
				int num5 = Grid.XYToCell(j, i);
				if (Grid.IsValidCell(num5))
				{
					color = this.GetColor(num5, viewMode, this.gameGridMode);
					int num6 = num5 * 4;
					textureBytes[num6] = (byte)(Mathf.Min(color.r, 1f) * 255f);
					textureBytes[num6 + 1] = (byte)(Mathf.Min(color.g, 1f) * 255f);
					textureBytes[num6 + 2] = (byte)(Mathf.Min(color.b, 1f) * 255f);
					textureBytes[num6 + 3] = (byte)(Mathf.Min(color.a, 1f) * 255f);
				}
			}
		}
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

	public void SetMode(SimViewMode mode)
	{
		this.mode = mode;
		EventSystem.Trigger(Game.Instance.gameObject, 1798162660, mode);
	}

	public SimViewMode GetMode()
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

	public Color GetColor(int cell, SimViewMode viewMode, SimDebugView.GameGridMode ggMode)
	{
		Color color = Color.black;
		bool flag = (byte)(Grid.Element[cell].state & Element.State.TemperatureInsulated) != 0;
		if (viewMode != SimViewMode.GameGrid)
		{
			if (viewMode != SimViewMode.Decor)
			{
				if (viewMode != SimViewMode.Joules)
				{
					if (viewMode != SimViewMode.OxygenMap)
					{
						if (viewMode != SimViewMode.MinionGroupProber)
						{
							if (viewMode != SimViewMode.SimCheckErrorMap)
							{
								if (viewMode != SimViewMode.TileType)
								{
									if (viewMode != SimViewMode.Crop)
									{
										if (viewMode == SimViewMode.MassMap)
										{
											if (!flag)
											{
												float mass = Grid.Cell[cell].mass;
												if (mass > 0f)
												{
													float num = (mass - SimDebugView.Instance.minMassExpected) / (SimDebugView.Instance.maxMassExpected - SimDebugView.Instance.minMassExpected);
													color = Color.HSVToRGB(1f - num, 1f, 1f);
												}
											}
											return color;
										}
										if (viewMode != SimViewMode.HarvestWhenReady)
										{
											if (viewMode == SimViewMode.Priorities)
											{
												return Color.black;
											}
											if (viewMode == SimViewMode.TemperatureMapOld)
											{
												if (!flag)
												{
													color = SimDebugView.TemperatureToColor(Grid.Temperature[cell], this.minTempExpected, this.maxTempExpected);
												}
												return color;
											}
											if (viewMode == SimViewMode.NoisePollution)
											{
												return this.GetNoisePollutionColour(cell);
											}
											if (viewMode == SimViewMode.PressureMap)
											{
												return this.GetPressureMapColour(cell);
											}
											if (viewMode == SimViewMode.Rooms)
											{
												CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(cell);
												if (cavityForCell != null && cavityForCell.room != null)
												{
													Room room = cavityForCell.room;
													color = RoomTypes.GetRoomType(room).category.color;
													color.a = 0.45f;
													int num2 = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
													if (Grid.IsValidCell(num2))
													{
														CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(num2);
														if (cavityForCell2 == cavityForCell)
														{
															color.a += 0.3f;
														}
													}
												}
												else
												{
													color = Color.black;
												}
												return color;
											}
											if (viewMode == SimViewMode.MinionOccupied)
											{
												if (Grid.Objects[cell, 0] != null)
												{
													color = Color.white;
												}
												return color;
											}
											if (viewMode == SimViewMode.HeatFlow)
											{
												return this.NormalizedHeatFlow(cell);
											}
											if (viewMode == SimViewMode.SuitRequiredMap)
											{
												return Color.black;
											}
											if (viewMode == SimViewMode.Forcefield)
											{
												if (Grid.ForceField[cell])
												{
													color = Color.white;
												}
												return color;
											}
											if (viewMode == SimViewMode.Reachability)
											{
												return Color.black;
											}
											if (viewMode == SimViewMode.Regions)
											{
												Region regionByID = Game.Instance.RegionManager.GetRegionByID(Game.Instance.RegionManager.GetIntersectionRegionID(cell));
												if (regionByID != null)
												{
													if (regionByID.IsCellBlocked(cell))
													{
														color = Color.clear;
													}
													else
													{
														color = regionByID.OverlayColor;
													}
												}
												else
												{
													color = Color.clear;
												}
												return color;
											}
											if (viewMode == SimViewMode.Flow)
											{
												return color;
											}
											if (viewMode == SimViewMode.PathProber)
											{
												KSelectable selected = SelectTool.Instance.selected;
												if (selected != null)
												{
													PathProber component = selected.GetComponent<PathProber>();
													if (component != null)
													{
														int cost = component.GetCost(cell);
														if (cost != PathProber.InvalidCost)
														{
															color = Color.white;
														}
														else
														{
															color = Color.black;
														}
													}
												}
												return color;
											}
											if (viewMode == SimViewMode.ThermalConductivity)
											{
												return this.GetThermalConductivityColour(flag, cell);
											}
											if (viewMode == SimViewMode.TemperatureMap)
											{
												return this.NormalizedTemperature(Grid.Temperature[cell]);
											}
											if (viewMode == SimViewMode.Disease)
											{
												Sim.DiseaseCell diseaseCell = Grid.Disease[cell];
												if (diseaseCell.diseaseIdx != 255)
												{
													Disease disease = Db.Get().Diseases[(int)diseaseCell.diseaseIdx];
													color = disease.overlayColour;
													color.a = SimUtil.DiseaseCountToAlpha(diseaseCell.elementCount);
												}
												else
												{
													color.a = 0f;
												}
												return color;
											}
											if (viewMode == SimViewMode.Reserved)
											{
												if (Grid.Reserved[cell])
												{
													color = Color.white;
												}
												else
												{
													color = Color.black;
												}
												return color;
											}
											if (viewMode == SimViewMode.StateChange)
											{
												return this.GetStateChangeColour(cell);
											}
											if (viewMode == SimViewMode.SolidLiquidMap)
											{
												return this.GetSolidLiquidMapColour(cell);
											}
											if (viewMode == SimViewMode.DangerMap)
											{
												return this.GetDangerMap(cell);
											}
											if (viewMode == SimViewMode.Light)
											{
												return (Grid.LightCount[cell] <= 0 && !LightGridManager.previewLightCells.Contains(cell)) ? new Color32(0, 0, 0, byte.MaxValue) : Lighting.Instance.Settings.LightColour;
											}
											if (viewMode != SimViewMode.StateMap)
											{
												return color;
											}
											return this.GetStateMapColour(cell);
										}
									}
									color = Color.black;
								}
								else
								{
									color = this.GetTileTypeColour(cell);
								}
							}
							else
							{
								color = this.GetSimCheckErrorMapColour(cell);
							}
						}
						else
						{
							int cost2 = MinionGroupProber.Get().GetPathProber().GetCost(cell);
							if (cost2 != PathProber.InvalidCost)
							{
								color = Color.white;
							}
							else
							{
								color = Color.black;
							}
						}
					}
					else
					{
						color = this.GetOxygenMapColour(cell);
					}
				}
				else
				{
					float num3 = Grid.Element[cell].specificHeatCapacity * Grid.Temperature[cell] * (Grid.Cell[cell].mass * 1000f);
					float num4 = 0.5f * num3 / (ElementLoader.FindElementByHash(SimHashes.SandStone).specificHeatCapacity * 294f * 1000000f);
					color = Color.Lerp(Color.black, Color.red, num4);
				}
			}
			else
			{
				color = this.GetDecorColour(cell);
			}
		}
		else
		{
			color = this.GetGameGridColour(cell, ggMode);
		}
		return color;
	}

	private Color GetGameGridColour(int cell, SimDebugView.GameGridMode mode)
	{
		Color color = new Color32(0, 0, 0, byte.MaxValue);
		switch (mode)
		{
		case SimDebugView.GameGridMode.GameSolidMap:
			color = ((!Grid.Solid[cell]) ? Color.black : Color.white);
			break;
		case SimDebugView.GameGridMode.Lighting:
			color = ((Grid.LightCount[cell] <= 0 && !LightGridManager.previewLightCells.Contains(cell)) ? Color.black : Color.white);
			break;
		case SimDebugView.GameGridMode.DigAmount:
			if (Grid.Element[cell].IsSolid)
			{
				float num = Grid.Damage[cell] / 255f;
				color = Color.HSVToRGB(1f - num, 1f, 1f);
			}
			break;
		case SimDebugView.GameGridMode.ForceField:
			if (Grid.ForceField[cell])
			{
				color = Color.white;
			}
			else
			{
				color = Color.black;
			}
			break;
		}
		return color;
	}

	public Color32 GetColourForID(int id)
	{
		return this.networkColours[id % this.networkColours.Length];
	}

	private Color GetThermalConductivityColour(bool insulated, int cell)
	{
		Color black = Color.black;
		float num = this.maxThermalConductivity - this.minThermalConductivity;
		if (!insulated && num != 0f)
		{
			float num2 = (Grid.Element[cell].thermalConductivity - this.minThermalConductivity) / num;
			num2 = Mathf.Max(num2, 0f);
			num2 = Mathf.Min(num2, 1f);
			black = new Color(num2, num2, num2);
		}
		return black;
	}

	private Color GetPressureMapColour(int cell)
	{
		Color black = Color.black;
		if (Grid.Pressure[cell] > 0f)
		{
			float num = (Grid.Pressure[cell] - this.minPressureExpected) / (this.maxPressureExpected - this.minPressureExpected);
			float num2 = Mathf.Clamp(num, 0f, 1f);
			float num3 = num2 * 0.9f;
			black = new Color(num3, num3, num3, 1f);
		}
		return black;
	}

	private Color GetOxygenMapColour(int cell)
	{
		Color color = Color.black;
		if (!Grid.IsLiquid(cell) && !Grid.Solid[cell])
		{
			if (Grid.Cell[cell].mass > SimDebugView.minimumBreathable && (Grid.Element[cell].id == SimHashes.Oxygen || Grid.Element[cell].id == SimHashes.ContaminatedOxygen))
			{
				float num = Mathf.Clamp((Grid.Cell[cell].mass - SimDebugView.minimumBreathable) / SimDebugView.optimallyBreathable, 0f, 1f);
				color = this.breathableGradient.Evaluate(num);
			}
			else
			{
				color = this.unbreathableColour;
			}
		}
		return color;
	}

	private Color GetTileTypeColour(int cell)
	{
		Element element = Grid.Element[cell];
		return element.substance.debugColour;
	}

	private Color GetStateMapColour(int cell)
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

	private Color GetSolidLiquidMapColour(int cell)
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

	private Color GetStateChangeColour(int cell)
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

	private Color GetNoisePollutionColour(int cell)
	{
		Color color = Color.black;
		if (NoisePolluter.IsNoiseableCell(cell))
		{
			float num = Grid.Loudness[cell];
			if (num == 0f)
			{
				color = this.dbColours[0];
			}
			else
			{
				float num2 = AudioEventManager.LoudnessToDB(num);
				if (num2 <= 36f)
				{
					color = Color.Lerp(this.dbColours[0], this.dbColours[1], Mathf.Abs(num2 / 36f));
				}
				else if (num2 >= 36f && num2 < 45f)
				{
					color = Color.Lerp(this.dbColours[2], this.dbColours[3], Mathf.Abs(num2 / 45f));
				}
				else if (num2 >= 45f && num2 < 60f)
				{
					color = Color.Lerp(this.dbColours[4], this.dbColours[5], Mathf.Abs((num2 - 45f) / 15f));
				}
				else if (num2 >= 60f && num2 < 80f)
				{
					color = Color.Lerp(this.dbColours[6], this.dbColours[7], Mathf.Abs((num2 - 60f) / 20f));
				}
				else if (num2 >= 80f && num2 < 106f)
				{
					color = Color.Lerp(this.dbColours[8], this.dbColours[9], Mathf.Abs((num2 - 80f) / 26f));
				}
				else
				{
					color = Color.Lerp(this.dbColours[10], this.dbColours[11], Mathf.Abs((num2 - 106f) / 19f));
				}
			}
		}
		return color;
	}

	public Color GetNoisePollutionCategoryColourFromDecibels(float db)
	{
		Color color = Color.white;
		if (db < 36f)
		{
			color = Color.Lerp(this.dbColours[0], this.dbColours[1], 0.8f);
		}
		else if (db >= 36f && db < 45f)
		{
			color = Color.Lerp(this.dbColours[2], this.dbColours[3], 0.5f);
		}
		else if (db >= 45f && db < 60f)
		{
			color = Color.Lerp(this.dbColours[4], this.dbColours[5], 0.5f);
		}
		else if (db >= 60f && db < 80f)
		{
			color = Color.Lerp(this.dbColours[6], this.dbColours[7], 0.5f);
		}
		else if (db >= 80f && db < 106f)
		{
			color = Color.Lerp(this.dbColours[8], this.dbColours[9], 0.5f);
		}
		else
		{
			color = Color.Lerp(this.dbColours[10], this.dbColours[11], 0.5f);
		}
		color.a = 1f;
		return color;
	}

	public Color GetNoisePollutionHoverColourFromDecibels(float db)
	{
		Color color = Color.white;
		if (db <= 36f)
		{
			color = Color.Lerp(this.dbColours[0], this.dbColours[1], 0.3f);
		}
		else if (db >= 36f && db < 45f)
		{
			color = Color.Lerp(this.dbColours[2], this.dbColours[3], 0.5f);
		}
		else if (db >= 45f && db < 60f)
		{
			color = Color.Lerp(this.dbColours[4], this.dbColours[5], 0.5f);
		}
		else if (db >= 60f && db < 80f)
		{
			color = Color.Lerp(this.dbColours[6], this.dbColours[7], 0.5f);
		}
		else if (db >= 80f && db < 106f)
		{
			color = Color.Lerp(this.dbColours[8], this.dbColours[9], 0.5f);
		}
		else
		{
			color = Color.Lerp(this.dbColours[10], this.dbColours[11], 0.5f);
		}
		color.a = 1f;
		return color;
	}

	private Color GetDecorColour(int cell)
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

	private Color GetDangerMap(int cell)
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

	private Color GetSimCheckErrorMapColour(int cell)
	{
		Color color = Color.black;
		Element element = Grid.Element[cell];
		float mass = Grid.Cell[cell].mass;
		float num = Grid.Temperature[cell];
		Color color2;
		if (float.IsNaN(mass) || float.IsNaN(num) || mass > 10000f || num > 10000f)
		{
			color2 = Color.red;
		}
		else
		{
			if (element.IsVacuum)
			{
				if (num != 0f)
				{
					color = Color.yellow;
				}
				else if (mass != 0f)
				{
					color = Color.blue;
				}
				else
				{
					color = Color.gray;
				}
			}
			else if (num < 10f)
			{
				color = Color.red;
			}
			else if (Grid.Cell[cell].mass < 1f && Grid.Pressure[cell] < 1f)
			{
				color = Color.green;
			}
			else if (num > element.highTemp + 3f && element.highTempTransition != null)
			{
				color = Color.magenta;
			}
			else if (num < element.lowTemp + 3f && element.lowTempTransition != null)
			{
				color = Color.cyan;
			}
			color2 = color;
		}
		return color2;
	}

	public Material material;

	public Material diseaseMaterial;

	public bool hideFOW = false;

	public const int colourSize = 4;

	private byte[] texBytes;

	private Texture2D tex;

	private GameObject plane;

	private SimViewMode mode = SimViewMode.PowerMap;

	private SimDebugView.GameGridMode gameGridMode = SimDebugView.GameGridMode.DigAmount;

	public float minTempExpected = 173.15f;

	public float maxTempExpected = 423.15f;

	public float minMassExpected = 1.0001f;

	public float maxMassExpected = 10000f;

	public float minPressureExpected = 1.300003f;

	public float maxPressureExpected = 201.3f;

	public float minThermalConductivity = 0f;

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

	private const float lum = 1f;

	private static float minMinionTemperature = 260f;

	private static float maxMinionTemperature = 310f;

	private static float minMinionPressure = 80f;

	public Color[] dbColours = new Color[]
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
