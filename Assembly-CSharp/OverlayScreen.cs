using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class OverlayScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		OverlayScreen.Instance = this;
		this.powerLabelParent = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
		List<Tag> list = new List<Tag>(OverlayScreen.WireIDs);
		List<Tag> list2 = new List<Tag>();
		List<Tag> list3 = new List<Tag>(OverlayScreen.OxygenBreatherIDs);
		List<Tag> list4 = new List<Tag>(OverlayScreen.LiquidVentIDs);
		List<Tag> list5 = new List<Tag>(OverlayScreen.GasVentIDs);
		List<Tag> list6 = new List<Tag>(OverlayScreen.HarvestableIDs);
		List<Tag> list7 = new List<Tag>();
		this.itemOverlays = new OverlayScreen.LayerInfo[]
		{
			new OverlayScreen.LayerInfo(SimViewMode.OxygenMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list3.ToArray(), null, null),
			new OverlayScreen.LayerInfo(SimViewMode.PowerMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list.ToArray(), null, null),
			new OverlayScreen.LayerInfo(SimViewMode.Rooms, new string[] { "Regions" }, OverlayScreen.RoomBuildingsIDs, null, null),
			new OverlayScreen.LayerInfo(SimViewMode.Light, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list2.ToArray(), null, null),
			new OverlayScreen.LayerInfo(SimViewMode.Regions, new string[] { "Regions" }, list7.ToArray(), null, null),
			new OverlayScreen.LayerInfo(SimViewMode.LiquidVentMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list4.ToArray(), null, null),
			new OverlayScreen.LayerInfo(SimViewMode.GasVentMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list5.ToArray(), null, null),
			new OverlayScreen.LayerInfo(SimViewMode.HarvestWhenReady, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list6.ToArray(), null, null)
		};
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.techViewSound = KFMOD.CreateInstance(this.techViewSoundPath);
		this.techViewSoundPlaying = false;
		Shader.SetGlobalVector("_OverlayParams", Vector4.zero);
	}

	public SimViewMode mode
	{
		get
		{
			return this.currentMode;
		}
	}

	public void ToggleOverlay(SimViewMode mode)
	{
		string text = string.Empty;
		SimViewMode simViewMode;
		if (mode != this.currentMode && mode != SimViewMode.None)
		{
			simViewMode = mode;
			if (simViewMode != SimViewMode.HeatFlow && simViewMode != SimViewMode.ThermalConductivity)
			{
				if (simViewMode != SimViewMode.TemperatureMap)
				{
					if (simViewMode != SimViewMode.Light)
					{
						if (simViewMode != SimViewMode.Decor)
						{
							if (simViewMode != SimViewMode.OxygenMap)
							{
								if (simViewMode != SimViewMode.HarvestWhenReady)
								{
									if (simViewMode != SimViewMode.LiquidVentMap)
									{
										if (simViewMode != SimViewMode.PowerMap)
										{
											if (simViewMode != SimViewMode.Priorities)
											{
												if (simViewMode == SimViewMode.GasVentMap)
												{
													text = "GasVent";
												}
											}
											else
											{
												text = "Priorities";
											}
										}
										else
										{
											text = "Power";
										}
									}
									else
									{
										text = "LiquidVent";
									}
								}
								else
								{
									text = "Harvest";
								}
							}
							else
							{
								text = "Oxygen";
							}
						}
						else
						{
							text = "Decor";
						}
					}
					else
					{
						text = "Lights";
					}
				}
				else
				{
					text = "Temperature";
				}
			}
			else
			{
				text = "HeatFlow";
			}
			text = GlobalAssets.GetSound(text, false);
		}
		else
		{
			text = GlobalAssets.GetSound("Off", false);
		}
		KMonoBehaviour.PlaySound(text);
		OverlayScreen.LayerInfo currentLayerInfo = this.GetCurrentLayerInfo();
		simViewMode = this.currentMode;
		if (simViewMode != SimViewMode.TemperatureMap)
		{
			if (simViewMode != SimViewMode.HarvestWhenReady)
			{
				if (simViewMode == SimViewMode.PowerMap)
				{
					this.DisablePowerLabels();
					this.DisableBatteryUIs();
				}
			}
			else
			{
				this.DisableHarvestWhenReady(this.GetCurrentLayerInfo(), SaveLoader.Instance.saveManager.GetLists());
			}
		}
		else
		{
			Infrared.Instance.Toggle(false);
			CameraController.Instance.ToggleTemperatureView(false);
		}
		if (currentLayerInfo.IsValid())
		{
			this.ToggleOverlayView();
		}
		if (mode == this.currentMode || mode == SimViewMode.None)
		{
			mode = SimViewMode.None;
			ResourceCategoryScreen.Instance.Show(true);
		}
		else
		{
			ManagementMenu.Instance.CloseAll();
			ResourceCategoryScreen.Instance.Show(false);
		}
		SimDebugView.Instance.SetMode(mode);
		this.currentMode = mode;
		simViewMode = this.currentMode;
		if (simViewMode == SimViewMode.TemperatureMap)
		{
			Infrared.Instance.Toggle(true);
			CameraController.Instance.ToggleTemperatureView(true);
		}
		GridCompositor.Instance.ToggleMinor(mode == SimViewMode.PowerMap || mode == SimViewMode.GasVentMap || mode == SimViewMode.LiquidVentMap);
		if (this.GetCurrentLayerInfo().IsValid())
		{
			this.ToggleOverlayView();
		}
		if (this.currentMode == SimViewMode.None)
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterOnMigrated, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.SetDynamicMusicOverlayInactive();
			this.techViewSound.stop(STOP_MODE.ALLOWFADEOUT);
			this.techViewSoundPlaying = false;
		}
		else if (!this.techViewSoundPlaying)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterOnMigrated);
			MusicManager.instance.SetDynamicMusicOverlayActive();
			this.techViewSound.start();
			this.techViewSound.setParameterValue("View", (float)this.currentMode);
			this.techViewSoundPlaying = true;
		}
		if (OverlayScreen.OnOverlayChanged != null)
		{
			OverlayScreen.OnOverlayChanged(this.currentMode);
		}
		this.ActivateLegend();
	}

	private void ActivateLegend()
	{
		if (OverlayLegend.Instance == null)
		{
			return;
		}
		OverlayLegend.Instance.SetLegend(this.currentMode, false);
	}

	public void Refresh()
	{
		this.Update();
	}

	private void Update()
	{
		OverlayScreen.LayerInfo currentLayerInfo = this.GetCurrentLayerInfo();
		if (currentLayerInfo.IsValid())
		{
			this.UpdateOverlayView(currentLayerInfo);
		}
	}

	private OverlayScreen.LayerInfo GetCurrentLayerInfo()
	{
		OverlayScreen.LayerInfo layerInfo = new OverlayScreen.LayerInfo
		{
			mask = 0
		};
		for (int i = 0; i < this.itemOverlays.Length; i++)
		{
			if (this.itemOverlays[i].viewMode == this.currentMode)
			{
				layerInfo = this.itemOverlays[i];
				break;
			}
		}
		return layerInfo;
	}

	private void ToggleOverlayView()
	{
		OverlayScreen.LayerInfo currentLayerInfo = this.GetCurrentLayerInfo();
		if (this.targetViewData == null)
		{
			this.targetViewData = new OverlayScreen.TargetViewData
			{
				layerTargets = new HashSet<GameObject>(),
				privateTargets = new HashSet<GameObject>()
			};
			if (currentLayerInfo.IsValid())
			{
				Camera.main.cullingMask |= currentLayerInfo.mask;
				SelectTool.Instance.SetLayerMask(currentLayerInfo.mask);
				DragTool.SetLayerMask(currentLayerInfo.mask);
				if (currentLayerInfo.onEnable != null)
				{
					currentLayerInfo.onEnable();
				}
			}
		}
		else
		{
			foreach (GameObject gameObject in this.targetViewData.layerTargets)
			{
				if (!(gameObject == null))
				{
					KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						component.SetLayer(gameObject.GetComponent<KPrefabID>().defaultLayer);
						component.TintColour = Color.white;
					}
				}
			}
			this.targetViewData = null;
			if (currentLayerInfo.IsValid())
			{
				if (currentLayerInfo.onDisable != null)
				{
					currentLayerInfo.onDisable();
				}
				if (Camera.main != null)
				{
					Camera.main.cullingMask &= ~currentLayerInfo.mask;
				}
				SelectTool.Instance.ClearLayerMask();
				DragTool.ClearLayerMask();
			}
		}
	}

	private void UpdateOverlayView(OverlayScreen.LayerInfo layer_info)
	{
		if (this.targetViewData != null)
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			SaveManager saveManager = SaveLoader.Instance.saveManager;
			this.RemoveOffscreenTargets(this.targetViewData.layerTargets, vector2I, vector2I2);
			this.RemoveOffscreenTargets(this.targetViewData.privateTargets, vector2I, vector2I2);
			if (layer_info.itemIDs != null)
			{
				Dictionary<Tag, List<SaveLoadRoot>> lists = saveManager.GetLists();
				SimViewMode simViewMode = this.currentMode;
				if (simViewMode != SimViewMode.HarvestWhenReady)
				{
					if (simViewMode != SimViewMode.LiquidVentMap)
					{
						if (simViewMode == SimViewMode.PowerMap)
						{
							this.UpdatePowerOverlayView(layer_info, lists);
							return;
						}
						if (simViewMode != SimViewMode.GasVentMap)
						{
							return;
						}
					}
					this.UpdateConduitOverlayView(layer_info, lists);
				}
				else
				{
					this.UpdateHarvestWhenReadyOverlayView(layer_info, lists);
				}
			}
		}
	}

	private void RemoveOffscreenTargets(HashSet<GameObject> targets, Vector2I min, Vector2I max)
	{
		this.nonVisibleTargets.Clear();
		foreach (GameObject gameObject in targets)
		{
			if (!(gameObject == null))
			{
				Vector2I vector2I = Grid.PosToXY(gameObject.transform.position);
				if (vector2I < min || max < vector2I)
				{
					KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						component.TintColour = Color.white;
						component.SetLayer(gameObject.GetComponent<KPrefabID>().defaultLayer);
						this.nonVisibleTargets.Add(gameObject);
					}
				}
			}
		}
		foreach (GameObject gameObject2 in this.nonVisibleTargets)
		{
			targets.Remove(gameObject2);
		}
		this.nonVisibleTargets.Clear();
	}

	private void SetBuildingColor(Color32 base_colour, GameObject root_obj)
	{
		if (root_obj == null || root_obj.GetComponent<BuildingComplete>() == null)
		{
			return;
		}
		base_colour.a = 0;
		KBatchedAnimController[] componentsInChildren = root_obj.GetComponentsInChildren<KBatchedAnimController>();
		foreach (KBatchedAnimController kbatchedAnimController in componentsInChildren)
		{
			kbatchedAnimController.TintColour = base_colour;
		}
	}

	private int GetNetworkIDInDirection(Direction direction, KAnimGraphTileVisualizer item)
	{
		int num = 0;
		while (item != null)
		{
			item = item.GetNeighbour(direction);
			if (item != null && item.connectionManager != null)
			{
				num = item.GetNetworkID();
				break;
			}
		}
		return num;
	}

	private Color32 GetColourForID(int id)
	{
		if (id >= 65535 || id < 0)
		{
			return Color.white;
		}
		return SimDebugView.Instance.GetColourForID(id);
	}

	public SimViewMode GetMode()
	{
		return this.currentMode;
	}

	private LocText GetFreePowerLabel()
	{
		LocText locText;
		if (this.freePowerLabelIdx < this.powerLabels.Count)
		{
			locText = this.powerLabels[this.freePowerLabelIdx];
			this.freePowerLabelIdx++;
		}
		else
		{
			locText = global::Util.KInstantiateUI<LocText>(this.powerLabelPrefab.gameObject, this.powerLabelParent.transform.gameObject, false);
			this.powerLabels.Add(locText);
			this.freePowerLabelIdx++;
		}
		return locText;
	}

	private BatteryUI GetFreeBatteryUI()
	{
		BatteryUI batteryUI;
		if (this.freeBatUIIdx < this.batteryUIList.Count)
		{
			batteryUI = this.batteryUIList[this.freeBatUIIdx];
			batteryUI.gameObject.SetActive(true);
			this.freeBatUIIdx++;
		}
		else
		{
			batteryUI = global::Util.KInstantiateUI<BatteryUI>(this.batUIPrefab.gameObject, this.powerLabelParent.transform.gameObject, false);
			this.batteryUIList.Add(batteryUI);
			this.freeBatUIIdx++;
		}
		return batteryUI;
	}

	private void DisableHarvestWhenReady(OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings)
	{
		Vector2I vector2I;
		Vector2I vector2I2;
		Grid.GetVisibleExtents(out vector2I, out vector2I2);
		int num = LayerMask.NameToLayer("MaskedOverlayBG");
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in registered_buildings)
		{
			bool flag = Array.IndexOf<Tag>(layer_info.itemIDs, keyValuePair.Key) != -1;
			if (flag)
			{
				List<SaveLoadRoot> value = keyValuePair.Value;
				foreach (SaveLoadRoot saveLoadRoot in value)
				{
					GameObject gameObject = saveLoadRoot.gameObject;
					Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
					if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.targetViewData.layerTargets.Contains(gameObject))
					{
						KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
						if (component != null)
						{
							Harvestable component2 = gameObject.GetComponent<Harvestable>();
							if (component2 != null)
							{
								component.SetLayer(layer_info.layer);
							}
							else
							{
								component.SetLayer(num);
							}
							this.targetViewData.layerTargets.Add(gameObject);
						}
					}
				}
			}
		}
		Color32 color = Color.clear;
		foreach (GameObject gameObject2 in this.targetViewData.layerTargets)
		{
			if (!(gameObject2 == null))
			{
				KBatchedAnimController component3 = gameObject2.GetComponent<KBatchedAnimController>();
				if (component3 != null)
				{
					component3.HighlightColour = color;
				}
			}
		}
	}

	private void DisablePowerLabels()
	{
		this.freePowerLabelIdx = 0;
		foreach (LocText locText in this.powerLabels)
		{
			locText.gameObject.SetActive(false);
		}
		this.updatePowerInfo.Clear();
	}

	private void DisableBatteryUIs()
	{
		this.freeBatUIIdx = 0;
		foreach (BatteryUI batteryUI in this.batteryUIList)
		{
			batteryUI.gameObject.SetActive(false);
		}
		this.updateBatteryInfo.Clear();
	}

	private void UpdatePowerLabels()
	{
		foreach (OverlayScreen.UpdatePowerInfo updatePowerInfo in this.updatePowerInfo)
		{
			KMonoBehaviour item = updatePowerInfo.item;
			LocText powerLabel = updatePowerInfo.powerLabel;
			LocText unitLabel = updatePowerInfo.unitLabel;
			Generator generator = updatePowerInfo.generator;
			IEnergyConsumer consumer = updatePowerInfo.consumer;
			if (updatePowerInfo.item == null)
			{
				powerLabel.gameObject.SetActive(false);
			}
			else
			{
				if (generator != null && consumer == null)
				{
					ManualGenerator component = generator.GetComponent<ManualGenerator>();
					int num;
					if (component == null)
					{
						generator.GetComponent<Operational>();
						num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
					}
					else
					{
						num = Mathf.Max(0, Mathf.RoundToInt(generator.WattageRating));
					}
					powerLabel.text = ((num == 0) ? num.ToString() : ("+" + num.ToString()));
					BuildingEnabledButton component2 = item.GetComponent<BuildingEnabledButton>();
					Color color = ((!(component2 != null) || component2.IsEnabled) ? this.generatorColour : this.buildingDisabledColour);
					powerLabel.color = color;
					unitLabel.color = color;
					Image outputIcon = generator.GetComponent<BuildingCellVisualizer>().GetOutputIcon();
					if (outputIcon != null)
					{
						outputIcon.color = color;
					}
				}
				if (consumer != null)
				{
					BuildingEnabledButton component3 = item.GetComponent<BuildingEnabledButton>();
					Color color2 = ((!(component3 != null) || component3.IsEnabled) ? this.consumerColour : this.buildingDisabledColour);
					int num2 = Mathf.Max(0, Mathf.RoundToInt(consumer.WattsNeededWhenActive));
					string text = num2.ToString();
					powerLabel.text = ((num2 == 0) ? text : ("-" + text));
					powerLabel.color = color2;
					unitLabel.color = color2;
					Image inputIcon = item.GetComponentInChildren<BuildingCellVisualizer>().GetInputIcon();
					if (inputIcon != null)
					{
						inputIcon.color = color2;
					}
				}
			}
		}
		foreach (OverlayScreen.UpdateBatteryInfo updateBatteryInfo in this.updateBatteryInfo)
		{
			updateBatteryInfo.ui.SetContent(updateBatteryInfo.battery);
		}
	}

	private void AddPowerLabels(KMonoBehaviour item)
	{
		IEnergyConsumer componentInChildren = item.gameObject.GetComponentInChildren<IEnergyConsumer>();
		Generator componentInChildren2 = item.gameObject.GetComponentInChildren<Generator>();
		if (componentInChildren != null || componentInChildren2 != null)
		{
			float num = -10f;
			if (componentInChildren2 != null)
			{
				LocText freePowerLabel = this.GetFreePowerLabel();
				freePowerLabel.gameObject.SetActive(true);
				freePowerLabel.gameObject.name = item.gameObject.name + "power label";
				LocText component = freePowerLabel.transform.GetChild(0).GetComponent<LocText>();
				component.gameObject.SetActive(true);
				freePowerLabel.enabled = true;
				component.enabled = true;
				Vector3 vector = Grid.CellToPos(componentInChildren2.PowerCell, 0.5f, 0f, 0f);
				freePowerLabel.rectTransform.position = vector + this.powerLabelOffset + Vector3.up * (num * 0.02f);
				if (componentInChildren != null && componentInChildren.PowerCell == componentInChildren2.PowerCell)
				{
					num -= 15f;
				}
				this.SetToolTip(freePowerLabel, UI.OVERLAYS.POWER.WATTS_GENERATED);
				this.updatePowerInfo.Add(new OverlayScreen.UpdatePowerInfo(item, freePowerLabel, component, componentInChildren2, null));
			}
			if (componentInChildren != null && componentInChildren.GetType() != typeof(Battery))
			{
				LocText freePowerLabel2 = this.GetFreePowerLabel();
				LocText component2 = freePowerLabel2.transform.GetChild(0).GetComponent<LocText>();
				freePowerLabel2.gameObject.SetActive(true);
				component2.gameObject.SetActive(true);
				freePowerLabel2.gameObject.name = item.gameObject.name + "power label";
				freePowerLabel2.enabled = true;
				component2.enabled = true;
				Vector3 vector2 = Grid.CellToPos(componentInChildren.PowerCell, 0.5f, 0f, 0f);
				freePowerLabel2.rectTransform.position = vector2 + this.powerLabelOffset + Vector3.up * (num * 0.02f);
				this.SetToolTip(freePowerLabel2, UI.OVERLAYS.POWER.WATTS_CONSUMED);
				this.updatePowerInfo.Add(new OverlayScreen.UpdatePowerInfo(item, freePowerLabel2, component2, null, componentInChildren));
			}
		}
	}

	private void AddBatteryUI(Battery bat)
	{
		BatteryUI freeBatteryUI = this.GetFreeBatteryUI();
		freeBatteryUI.SetContent(bat);
		Vector3 vector = Grid.CellToPos(bat.PowerCell, 0.5f, 0f, 0f);
		bool flag = bat.GetComponent<PowerTransformer>() != null;
		freeBatteryUI.GetComponent<RectTransform>().position = Vector3.up + vector + ((!flag) ? this.batteryUIOffset : this.batteryUITransformerOffset);
		this.updateBatteryInfo.Add(new OverlayScreen.UpdateBatteryInfo(bat, freeBatteryUI));
	}

	private void SetToolTip(LocText label, string text)
	{
		ToolTip component = label.GetComponent<ToolTip>();
		if (component != null)
		{
			component.toolTip = text;
		}
	}

	private void UpdatePowerOverlayView(OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings)
	{
		using (new KProfiler.Region("UpdatePowerOverlay", null))
		{
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			using (new KProfiler.Region("Wires", null))
			{
				foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in registered_buildings)
				{
					bool flag = Array.IndexOf<Tag>(layer_info.itemIDs, keyValuePair.Key) != -1;
					if (flag)
					{
						List<SaveLoadRoot> value = keyValuePair.Value;
						foreach (SaveLoadRoot saveLoadRoot in value)
						{
							GameObject gameObject = saveLoadRoot.gameObject;
							Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
							if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.targetViewData.layerTargets.Contains(gameObject))
							{
								KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
								component.SetLayer(layer_info.layer);
								this.targetViewData.layerTargets.Add(gameObject);
							}
						}
					}
				}
				CircuitManager circuitManager = Game.Instance.circuitManager;
				foreach (GameObject gameObject2 in this.targetViewData.layerTargets)
				{
					if (!(gameObject2 == null))
					{
						Wire component2 = gameObject2.GetComponent<Wire>();
						if (component2 != null)
						{
							KBatchedAnimController component3 = component2.GetComponent<KBatchedAnimController>();
							ushort networkID = component2.NetworkID;
							bool flag2 = circuitManager.HasGenerators(networkID) || circuitManager.HasBatteries(networkID);
							Color32 color;
							if (flag2)
							{
								float potentialWattsGeneratedByCircuit = circuitManager.GetPotentialWattsGeneratedByCircuit(networkID);
								float wattsUsedByCircuit = circuitManager.GetWattsUsedByCircuit(networkID);
								float num = wattsUsedByCircuit / potentialWattsGeneratedByCircuit;
								color = ((num >= 0.85f) ? this.circuitStrainingColour : this.circuitSafeColour);
							}
							else
							{
								color = this.circuitUnpoweredColour;
							}
							component3.TintColour = color;
						}
					}
				}
			}
			this.queuedAdds.Clear();
			using (new KProfiler.Region("BatteryUI", null))
			{
				foreach (Battery battery in Components.Batteries)
				{
					GameObject gameObject3 = battery.gameObject;
					Vector2I vector2I4 = Grid.PosToXY(gameObject3.transform.position);
					if (vector2I <= vector2I4 && vector2I4 <= vector2I2 && !this.targetViewData.privateTargets.Contains(gameObject3))
					{
						this.AddBatteryUI(battery);
						this.queuedAdds.Add(gameObject3);
					}
				}
				foreach (Generator generator in Components.Generators)
				{
					GameObject gameObject4 = generator.gameObject;
					Vector2I vector2I5 = Grid.PosToXY(gameObject4.transform.position);
					if (vector2I <= vector2I5 && vector2I5 <= vector2I2 && !this.targetViewData.privateTargets.Contains(gameObject4))
					{
						this.targetViewData.privateTargets.Add(gameObject4);
						if (gameObject4.GetComponent<PowerTransformer>() == null)
						{
							this.AddPowerLabels(generator);
						}
					}
				}
				foreach (EnergyConsumer energyConsumer in Components.EnergyConsumers)
				{
					GameObject gameObject5 = energyConsumer.gameObject;
					Vector2I vector2I6 = Grid.PosToXY(gameObject5.transform.position);
					if (vector2I <= vector2I6 && vector2I6 <= vector2I2 && !this.targetViewData.privateTargets.Contains(gameObject5))
					{
						this.targetViewData.privateTargets.Add(gameObject5);
						this.AddPowerLabels(energyConsumer);
					}
				}
			}
			foreach (GameObject gameObject6 in this.queuedAdds)
			{
				this.targetViewData.privateTargets.Add(gameObject6);
			}
			this.queuedAdds.Clear();
			this.UpdatePowerLabels();
		}
	}

	private void UpdateConduitOverlayView(OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings)
	{
		Vector2I vector2I;
		Vector2I vector2I2;
		Grid.GetVisibleExtents(out vector2I, out vector2I2);
		Game.ConduitVisInfo conduitVisInfo = ((layer_info.viewMode != SimViewMode.LiquidVentMap) ? Game.Instance.gasConduitVisInfo : Game.Instance.liquidConduitVisInfo);
		int num = LayerMask.NameToLayer("MaskedOverlayBG");
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in registered_buildings)
		{
			bool flag = Array.IndexOf<Tag>(layer_info.itemIDs, keyValuePair.Key) != -1;
			if (flag)
			{
				List<SaveLoadRoot> value = keyValuePair.Value;
				foreach (SaveLoadRoot saveLoadRoot in value)
				{
					GameObject gameObject = saveLoadRoot.gameObject;
					Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
					if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.targetViewData.layerTargets.Contains(gameObject))
					{
						KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
						if (component != null)
						{
							Conduit component2 = gameObject.GetComponent<Conduit>();
							if (component2 != null)
							{
								component.SetLayer(layer_info.layer);
							}
							else
							{
								component.SetLayer(num);
							}
							this.targetViewData.layerTargets.Add(gameObject);
						}
					}
				}
			}
		}
		foreach (GameObject gameObject2 in this.targetViewData.layerTargets)
		{
			if (!(gameObject2 == null))
			{
				BuildingDef def = gameObject2.GetComponent<Building>().Def;
				Color32 color = ((!def.IsInsulated) ? conduitVisInfo.overlayTint : conduitVisInfo.overlayInsulatedTint);
				KBatchedAnimController component3 = gameObject2.GetComponent<KBatchedAnimController>();
				component3.TintColour = color;
			}
		}
	}

	private void UpdateHarvestWhenReadyOverlayView(OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings)
	{
		Vector2I vector2I;
		Vector2I vector2I2;
		Grid.GetVisibleExtents(out vector2I, out vector2I2);
		int num = LayerMask.NameToLayer("MaskedOverlayBG");
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in registered_buildings)
		{
			bool flag = Array.IndexOf<Tag>(layer_info.itemIDs, keyValuePair.Key) != -1;
			if (flag)
			{
				List<SaveLoadRoot> value = keyValuePair.Value;
				foreach (SaveLoadRoot saveLoadRoot in value)
				{
					if (Grid.Visible[Grid.PosToCell(saveLoadRoot.gameObject)] > 0 || DebugHandler.FreeCameraMode)
					{
						GameObject gameObject = saveLoadRoot.gameObject;
						Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
						if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.targetViewData.layerTargets.Contains(gameObject))
						{
							KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
							if (component != null)
							{
								Harvestable component2 = gameObject.GetComponent<Harvestable>();
								if (component2 != null)
								{
									component.SetLayer(layer_info.layer);
								}
								else
								{
									component.SetLayer(num);
								}
								this.targetViewData.layerTargets.Add(gameObject);
							}
						}
					}
				}
			}
		}
		Color32 color = new Color32(128, 128, 128, 64);
		foreach (GameObject gameObject2 in this.targetViewData.layerTargets)
		{
			if (!(gameObject2 == null))
			{
				KBatchedAnimController component3 = gameObject2.GetComponent<KBatchedAnimController>();
				if (component3 != null)
				{
					component3.HighlightColour = color;
				}
			}
		}
	}

	private static readonly Tag[] WireIDs = new Tag[]
	{
		TagManager.Create("Wire", null),
		TagManager.Create("WireUnderConstruction", null),
		TagManager.Create("HighWattageWire", null),
		TagManager.Create("HighWattageWireUnderConstruction", null)
	};

	private static readonly Tag[] GasVentIDs = new Tag[]
	{
		TagManager.Create("GasConduit", null),
		TagManager.Create("InsulatedGasConduit", null),
		TagManager.Create("GasConduitUnderConstruction", null),
		TagManager.Create("InsulatedGasConduitUnderConstruction", null)
	};

	private static readonly Tag[] LiquidVentIDs = new Tag[]
	{
		TagManager.Create("LiquidConduit", null),
		TagManager.Create("InsulatedLiquidConduit", null),
		TagManager.Create("LiquidConduitUnderConstruction", null),
		TagManager.Create("InsulatedLiquidConduitUnderConstruction", null)
	};

	private static readonly Tag[] OxygenBreatherIDs = new Tag[]
	{
		TagManager.Create("Minion", null),
		TagManager.Create("OxyRock", null)
	};

	private static readonly Tag[] HarvestableIDs = new Tag[]
	{
		TagManager.Create("BasicSingleHarvestPlant", null),
		TagManager.Create("BasicFabricPlant", null),
		TagManager.Create("PrickleFlower", null),
		TagManager.Create("BasicForagePlantPlanted", null),
		TagManager.Create("ColdWheat", null),
		TagManager.Create("SpiceVine", null)
	};

	private static readonly Tag[] RoomBuildingsIDs = new Tag[] { TagManager.Create("Bed", null) };

	[SerializeField]
	[EventRef]
	private string techViewSoundPath;

	private EventInstance techViewSound;

	private bool techViewSoundPlaying;

	private OverlayScreen.LayerInfo[] itemOverlays;

	public static OverlayScreen Instance;

	[SerializeField]
	private Canvas powerLabelParent;

	[SerializeField]
	private LocText powerLabelPrefab;

	[SerializeField]
	private BatteryUI batUIPrefab;

	[SerializeField]
	private Vector3 powerLabelOffset;

	[SerializeField]
	private Vector3 batteryUIOffset;

	[SerializeField]
	private Vector3 batteryUITransformerOffset;

	[SerializeField]
	private Color consumerColour;

	[SerializeField]
	private Color generatorColour;

	[SerializeField]
	private Color buildingDisabledColour = Color.gray;

	private int freePowerLabelIdx;

	private List<LocText> powerLabels = new List<LocText>();

	private int freeBatUIIdx;

	private List<BatteryUI> batteryUIList = new List<BatteryUI>();

	[SerializeField]
	private TextStyleSetting TooltipHeader;

	[SerializeField]
	private TextStyleSetting TooltipDescription;

	[SerializeField]
	private Color32 circuitUnpoweredColour;

	[SerializeField]
	private Color32 circuitSafeColour;

	[SerializeField]
	private Color32 circuitStrainingColour;

	public static Action<SimViewMode> OnOverlayChanged;

	private SimViewMode currentMode;

	private OverlayScreen.TargetViewData targetViewData;

	private List<GameObject> nonVisibleTargets = new List<GameObject>();

	private List<OverlayScreen.UpdatePowerInfo> updatePowerInfo = new List<OverlayScreen.UpdatePowerInfo>();

	private List<OverlayScreen.UpdateBatteryInfo> updateBatteryInfo = new List<OverlayScreen.UpdateBatteryInfo>();

	private List<GameObject> queuedAdds = new List<GameObject>();

	private struct LayerInfo
	{
		public LayerInfo(SimViewMode viewMode, string[] layerNames, Tag[] itemIDs, global::System.Action onEnable = null, global::System.Action onDisable = null)
		{
			this.viewMode = viewMode;
			this.mask = LayerMask.GetMask(layerNames);
			this.layer = LayerMask.NameToLayer(layerNames[0]);
			this.itemIDs = itemIDs;
			this.onEnable = onEnable;
			this.onDisable = onDisable;
		}

		public bool IsValid()
		{
			return this.mask != 0;
		}

		public SimViewMode viewMode;

		public int mask;

		public int layer;

		public Tag[] itemIDs;

		public global::System.Action onEnable;

		public global::System.Action onDisable;
	}

	private class TargetViewData
	{
		public HashSet<GameObject> layerTargets;

		public HashSet<GameObject> privateTargets;
	}

	private struct UpdatePowerInfo
	{
		public UpdatePowerInfo(KMonoBehaviour item, LocText power_label, LocText unit_label, Generator g, IEnergyConsumer c)
		{
			this.item = item;
			this.powerLabel = power_label;
			this.unitLabel = unit_label;
			this.generator = g;
			this.consumer = c;
		}

		public KMonoBehaviour item;

		public LocText powerLabel;

		public LocText unitLabel;

		public Generator generator;

		public IEnergyConsumer consumer;
	}

	private struct UpdateBatteryInfo
	{
		public UpdateBatteryInfo(Battery battery, BatteryUI ui)
		{
			this.battery = battery;
			this.ui = ui;
		}

		public Battery battery;

		public BatteryUI ui;
	}
}
