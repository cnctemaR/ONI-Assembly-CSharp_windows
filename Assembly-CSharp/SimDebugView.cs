using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SimDebugView : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		SimDebugView.Instance = this;
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
		this.plane = SimDebugView.CreatePlane("SimDebugView", this.transform);
		this.tex = SimDebugView.CreateTexture(out this.texBytes, Grid.WidthInCells, Grid.HeightInCells);
		this.plane.GetComponent<Renderer>().material = this.material;
		this.plane.GetComponent<Renderer>().material.mainTexture = this.tex;
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
		if (this.plane == null)
		{
			return;
		}
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

	public void UpdateData(Texture2D texture, byte[] textureBytes, SimViewMode viewMode, byte alpha)
	{
		if (viewMode != SimViewMode.HeatFlow && viewMode != SimViewMode.TemperatureMap && viewMode != SimViewMode.Decor && viewMode != SimViewMode.OxygenMap)
		{
			texture.filterMode = FilterMode.Point;
		}
		else
		{
			texture.filterMode = FilterMode.Bilinear;
		}
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
				if (Grid.IsValidCell(num5) && (float)Grid.Visible[num5] + PropertyTextures.FogOfWarScale != 0f)
				{
					color = this.GetColor(num5, viewMode);
					int num6 = num5 * 4;
					textureBytes[num6] = (byte)(Mathf.Min(color.r, 1f) * 255f);
					textureBytes[num6 + 1] = (byte)(Mathf.Min(color.g, 1f) * 255f);
					textureBytes[num6 + 2] = (byte)(Mathf.Min(color.b, 1f) * 255f);
					textureBytes[num6 + 3] = alpha;
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
		ColourHSV colourHSV = new ColourHSV(10f + (1f - num2) * 171f, 1f, 1f);
		return colourHSV.ToColor();
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
		float thermalComfort = GameUtil.GetThermalComfort(cell, -0.08368001f);
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

	public Color GetColor(int cell, SimViewMode viewMode)
	{
		Color color = Color.black;
		bool flag = (byte)(Grid.Element[cell].state & Element.State.TemperatureInsulated) != 0;
		if (viewMode != SimViewMode.TemperatureMapOld)
		{
			if (viewMode != SimViewMode.PressureMap)
			{
				if (viewMode != SimViewMode.MinionOccupied)
				{
					if (viewMode != SimViewMode.HeatFlow)
					{
						if (viewMode != SimViewMode.SuitRequiredMap)
						{
							if (viewMode != SimViewMode.Reachability)
							{
								if (viewMode != SimViewMode.Regions)
								{
									if (viewMode != SimViewMode.Flow)
									{
										if (viewMode != SimViewMode.PathProber)
										{
											if (viewMode != SimViewMode.ThermalConductivity)
											{
												if (viewMode != SimViewMode.TemperatureMap)
												{
													if (viewMode != SimViewMode.LiquidSource)
													{
														if (viewMode != SimViewMode.Reserved)
														{
															if (viewMode != SimViewMode.StateChange)
															{
																if (viewMode != SimViewMode.SolidLiquidMap)
																{
																	if (viewMode != SimViewMode.DangerMap)
																	{
																		if (viewMode != SimViewMode.Light)
																		{
																			if (viewMode != SimViewMode.GameGrid)
																			{
																				if (viewMode != SimViewMode.InsideBase)
																				{
																					if (viewMode != SimViewMode.StateMap)
																					{
																						if (viewMode != SimViewMode.Decor)
																						{
																							if (viewMode != SimViewMode.OxygenMap)
																							{
																								if (viewMode != SimViewMode.MinionGroupProber)
																								{
																									if (viewMode != SimViewMode.SimCheckErrorMap)
																									{
																										if (viewMode != SimViewMode.TileType)
																										{
																											if (viewMode != SimViewMode.MassMap)
																											{
																												if (viewMode != SimViewMode.HarvestWhenReady)
																												{
																													if (viewMode == SimViewMode.Priorities)
																													{
																														color = Color.black;
																													}
																												}
																												else
																												{
																													color = Color.black;
																												}
																											}
																											else if (!flag)
																											{
																												float mass = Grid.Cell[cell].mass;
																												if (mass > 0f)
																												{
																													float num = (mass - SimDebugView.Instance.minMassExpected) / (SimDebugView.Instance.maxMassExpected - SimDebugView.Instance.minMassExpected);
																													ColourHSV colourHSV = new ColourHSV((1f - num) * 100f, 1f, 1f);
																													color = colourHSV.ToColor();
																												}
																											}
																										}
																										else
																										{
																											Element element = Grid.Element[cell];
																											color = element.substance.debugColour;
																										}
																									}
																									else
																									{
																										Element element2 = Grid.Element[cell];
																										float mass2 = Grid.Cell[cell].mass;
																										float num2 = Grid.Temperature[cell];
																										if (float.IsNaN(mass2) || float.IsNaN(num2) || mass2 > 10000f || num2 > 10000f)
																										{
																											color = Color.red;
																										}
																										else if (element2.IsVacuum)
																										{
																											if (num2 != 0f)
																											{
																												color = Color.yellow;
																											}
																											else if (mass2 != 0f)
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
																										else if (Grid.Cell[cell].mass < 1f && Grid.Pressure[cell] < 1f)
																										{
																											color = Color.green;
																										}
																										else if (num2 > element2.highTemp + 3f && element2.highTempTransition != null)
																										{
																											color = Color.magenta;
																										}
																										else if (num2 < element2.lowTemp + 3f && element2.lowTempTransition != null)
																										{
																											color = Color.cyan;
																										}
																									}
																								}
																								else
																								{
																									int cost = MinionGroupProber.Get().GetPathProber().GetCost(cell);
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
																							else if (!Grid.IsLiquid(cell) && !Grid.Solid[cell])
																							{
																								if (Grid.Cell[cell].mass > SimDebugView.minimumBreathable && (Grid.Element[cell].id == SimHashes.Oxygen || Grid.Element[cell].id == SimHashes.ContaminatedOxygen))
																								{
																									float num3 = Mathf.Clamp((Grid.Cell[cell].mass - SimDebugView.minimumBreathable) / SimDebugView.optimallyBreathable, 0f, 1f);
																									color = this.breathableGradient.Evaluate(num3);
																								}
																								else
																								{
																									color = this.unbreathableColour;
																								}
																							}
																						}
																						else
																						{
																							color = Color.black;
																							if (!Grid.Solid[cell])
																							{
																								float num4 = (float)GameUtil.GetDecorAtCell(cell);
																								float num5 = num4 / 100f;
																								if (num5 > 0f)
																								{
																									color = Color.Lerp(new Color(0.15f, 0f, 0f), new Color(0f, 1f, 0f), Mathf.Abs(num5));
																								}
																								else
																								{
																									color = Color.Lerp(new Color(0.15f, 0f, 0f), new Color(1f, 0f, 0f), Mathf.Abs(num5));
																								}
																							}
																						}
																					}
																					else
																					{
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
																					}
																				}
																				else if (BaseArea.Instance.IsInsideBase(cell))
																				{
																					color = Color.white;
																				}
																				else
																				{
																					color = Color.black;
																				}
																			}
																			else
																			{
																				color = this.GetGameGridColour(cell);
																			}
																		}
																		else
																		{
																			color = LightGridManager.GetColorForCell(cell);
																		}
																	}
																	else
																	{
																		SimDebugView.DangerAmount dangerAmount = SimDebugView.DangerAmount.None;
																		if (!Grid.Element[cell].IsSolid)
																		{
																			float num6 = 0f;
																			if (Grid.Temperature[cell] < SimDebugView.minMinionTemperature)
																			{
																				num6 = Mathf.Abs(Grid.Temperature[cell] - SimDebugView.minMinionTemperature);
																			}
																			if (Grid.Temperature[cell] > SimDebugView.maxMinionTemperature)
																			{
																				num6 = Mathf.Abs(Grid.Temperature[cell] - SimDebugView.maxMinionTemperature);
																			}
																			if (num6 > 0f)
																			{
																				if (num6 < 10f)
																				{
																					dangerAmount = SimDebugView.DangerAmount.VeryLow;
																				}
																				else if (num6 < 30f)
																				{
																					dangerAmount = SimDebugView.DangerAmount.Low;
																				}
																				else if (num6 < 100f)
																				{
																					dangerAmount = SimDebugView.DangerAmount.Moderate;
																				}
																				else if (num6 < 200f)
																				{
																					dangerAmount = SimDebugView.DangerAmount.High;
																				}
																				else if (num6 < 400f)
																				{
																					dangerAmount = SimDebugView.DangerAmount.VeryHigh;
																				}
																				else if (num6 > 800f)
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
																			float num7 = (float)dangerAmount / 6f;
																			ColourHSV colourHSV2 = new ColourHSV(80f - num7 * 80f, 1f, 1f);
																			color = colourHSV2.ToColor();
																		}
																	}
																}
																else
																{
																	switch ((byte)(Grid.Element[cell].state & Element.State.Solid))
																	{
																	case 2:
																		color = Color.green;
																		break;
																	case 3:
																		color = Color.blue;
																		break;
																	}
																}
															}
															else
															{
																color = Color.black;
																Element element3 = Grid.Element[cell];
																if (!element3.IsVacuum)
																{
																	float num8 = Grid.Temperature[cell];
																	float num9 = element3.lowTemp * 0.05f;
																	float num10 = Mathf.Abs(num8 - element3.lowTemp);
																	float num11 = num10 / num9;
																	float num12 = element3.highTemp * 0.05f;
																	float num13 = Mathf.Abs(num8 - element3.highTemp);
																	float num14 = num13 / num12;
																	float num15 = Mathf.Max(0f, 1f - Mathf.Min(num11, num14));
																	color = Color.Lerp(Color.black, Color.red, num15);
																}
															}
														}
														else if (Grid.Reserved[cell])
														{
															color = Color.white;
														}
														else
														{
															color = Color.black;
														}
													}
													else if (LiquidSourceDetector2.Instance.HasSource(cell))
													{
														color = new Color(1f, 1f, 1f, 1f);
													}
												}
												else
												{
													color = this.NormalizedTemperature(Grid.Temperature[cell]);
												}
											}
											else
											{
												float num16 = this.maxThermalConductivity - this.minThermalConductivity;
												if (!flag && num16 != 0f)
												{
													float num17 = (Grid.Element[cell].thermalConductivity - this.minThermalConductivity) / num16;
													num17 = Mathf.Max(num17, 0f);
													num17 = Mathf.Min(num17, 1f);
													color = new Color(num17, num17, num17);
												}
											}
										}
										else
										{
											KSelectable selected = SelectTool.Instance.selected;
											if (selected != null)
											{
												PathProber component = selected.GetComponent<PathProber>();
												if (component != null)
												{
													int cost2 = component.GetCost(cell);
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
										}
									}
									else
									{
										Output.LogError(new object[] { "Debug view of flow is broken for optimization reasons" });
									}
								}
								else
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
								}
							}
							else
							{
								color = Color.black;
							}
						}
						else
						{
							color = ((!Grid.SuitRequired[cell]) ? Color.black : Color.red);
						}
					}
					else
					{
						color = this.NormalizedHeatFlow(cell);
					}
				}
				else if (Grid.Objects[cell, 0] != null)
				{
					color = Color.white;
				}
			}
			else if (Grid.Pressure[cell] > 0f)
			{
				float num18 = (Grid.Pressure[cell] - this.minPressureExpected) / (this.maxPressureExpected - this.minPressureExpected);
				float num19 = Mathf.Clamp(num18, 0f, 1f);
				float num20 = num19 * 0.9f;
				color = new Color(num20, num20, num20, 1f);
			}
		}
		else if (!flag)
		{
			color = SimDebugView.TemperatureToColor(Grid.Temperature[cell], this.minTempExpected, this.maxTempExpected);
		}
		return color;
	}

	public Color[] GetRoomColours()
	{
		Color[] array = new Color[this.roomColours.Length];
		this.roomColours.CopyTo(array, 0);
		return array;
	}

	private Color GetGameGridColour(int cell)
	{
		Color color = new Color32(0, 0, 0, byte.MaxValue);
		switch (this.gameGridMode)
		{
		case SimDebugView.GameGridMode.GameSolidMap:
			color = ((!Grid.Solid[cell]) ? Color.black : Color.white);
			break;
		case SimDebugView.GameGridMode.Lighting:
			color = ((Grid.LightCount[cell] <= 0) ? Color.black : Color.white);
			break;
		case SimDebugView.GameGridMode.RoomMap:
		{
			ushort num = Grid.Room[cell];
			int num2 = (int)num % this.roomColours.Length;
			color = ((num == ushort.MaxValue) ? Color.black : this.roomColours[num2]);
			break;
		}
		case SimDebugView.GameGridMode.DigAmount:
			if (Grid.Element[cell].IsSolid)
			{
				float num3 = Grid.Damage[cell] / 255f;
				ColourHSV colourHSV = new ColourHSV((1f - num3) * 350f, 1f, 1f);
				color = colourHSV.ToColor();
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

	public const int colourSize = 4;

	private const float lum = 1f;

	public Material material;

	public bool hideFOW;

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

	private static float minMinionTemperature = 260f;

	private static float maxMinionTemperature = 310f;

	private static float minMinionPressure = 80f;

	[SerializeField]
	private Color[] roomColours = new Color[]
	{
		new Color(1f, 0f, 0f),
		new Color(0f, 1f, 0f),
		new Color(0f, 0f, 1f),
		new Color(1f, 1f, 0f),
		new Color(1f, 0f, 1f),
		new Color(0f, 1f, 1f),
		new Color(1f, 1f, 1f)
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
