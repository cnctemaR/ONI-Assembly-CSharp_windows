using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class OverlayScreen : KMonoBehaviour
{
	public OverlayScreen()
	{
		OverlayScreen.ColorHighlightCondition[] array = new OverlayScreen.ColorHighlightCondition[3];
		array[0] = new OverlayScreen.ColorHighlightCondition(new Color(0.95686275f, 0.2509804f, 0.2784314f, 0.75f), delegate(GameObject go)
		{
			WiltCondition component = go.GetComponent<WiltCondition>();
			return component != null && component.IsWilting();
		});
		array[1] = new OverlayScreen.ColorHighlightCondition(new Color(0.9843137f, 0.6901961f, 0.23137255f, 0.75f), (GameObject go) => !go.GetComponent<Harvestable>().CanBeHavested);
		array[2] = new OverlayScreen.ColorHighlightCondition(new Color(0.41960785f, 0.827451f, 0.5176471f, 0.75f), (GameObject go) => go.GetComponent<Harvestable>().CanBeHavested);
		this.cropHighlightConditions = array;
		OverlayScreen.ColorHighlightCondition[] array2 = new OverlayScreen.ColorHighlightCondition[1];
		array2[0] = new OverlayScreen.ColorHighlightCondition(new Color(0.65f, 0.65f, 0.65f, 0.65f), (GameObject go) => true);
		this.harvestHighlightConditions = array2;
		OverlayScreen.ColorHighlightCondition[] array3 = new OverlayScreen.ColorHighlightCondition[1];
		array3[0] = new OverlayScreen.ColorHighlightCondition(new Color(0.65f, 0f, 0f, 0.65f), (GameObject go) => go.GetSMI<ImmuneSystemMonitor.Instance>().IsSick());
		this.diseaseHighlightConditions = array3;
		this.buildingDisabledColour = Color.gray;
		this.powerLabels = new List<LocText>();
		this.batteryUIList = new List<BatteryUI>();
		this.harvestableNotificationList = new List<GameObject>();
		this.diseaseUIList = new List<GameObject>();
		this.nonVisibleTargets = new List<GameObject>();
		this.updatePowerInfo = new List<OverlayScreen.UpdatePowerInfo>();
		this.updateBatteryInfo = new List<OverlayScreen.UpdateBatteryInfo>();
		this.updateCropInfo = new List<OverlayScreen.UpdateCropInfo>();
		this.updateDiseaseInfo = new List<OverlayScreen.UpdateDiseaseInfo>();
		this.queuedAdds = new List<GameObject>();
		this.outsideViewObjects = new List<GameObject>();
		base..ctor();
	}

	public SimViewMode mode
	{
		get
		{
			return this.currentMode;
		}
	}

	protected override void OnPrefabInit()
	{
		OverlayScreen.Instance = this;
		this.powerLabelParent = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
		this.harvestableUIParent = this.powerLabelParent;
		this.diseaseUIParent = this.powerLabelParent;
		this.itemOverlays = new OverlayScreen.LayerInfo[]
		{
			new OverlayScreen.LayerInfo(SimViewMode.OxygenMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, OverlayScreen.OxygenBreatherIDs, null, null),
			new OverlayScreen.LayerInfo(SimViewMode.PowerMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, OverlayScreen.WireIDs, null, null),
			new OverlayScreen.LayerInfo(SimViewMode.Light, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, new Tag[0], null, null),
			new OverlayScreen.LayerInfo(SimViewMode.Regions, new string[] { "Regions" }, new Tag[0], null, null),
			new OverlayScreen.LayerInfo(SimViewMode.LiquidVentMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, OverlayScreen.LiquidVentIDs, null, null),
			new OverlayScreen.LayerInfo(SimViewMode.GasVentMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, OverlayScreen.GasVentIDs, null, null),
			new OverlayScreen.LayerInfo(SimViewMode.HarvestWhenReady, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, OverlayScreen.HarvestableIDs, null, null),
			new OverlayScreen.LayerInfo(SimViewMode.Disease, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, -1, OverlayScreen.DiseaseIDs, null, null),
			new OverlayScreen.LayerInfo(SimViewMode.Crop, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, OverlayScreen.HarvestableIDs, null, null)
		};
	}

	protected override void OnLoadLevel()
	{
		this.itemOverlays = null;
		this.harvestableNotificationPrefab = null;
		this.powerLabels = null;
		this.batteryUIList = null;
		this.harvestableNotificationList = null;
		this.targetViewData = null;
		this.nonVisibleTargets = null;
		this.updatePowerInfo = null;
		this.updateBatteryInfo = null;
		this.updateCropInfo = null;
		this.updateDiseaseInfo = null;
		this.queuedAdds = null;
		this.powerLabelParent = null;
		this.harvestableUIParent = null;
		OverlayScreen.Instance = null;
		base.OnLoadLevel();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.techViewSound = KFMOD.CreateInstance(this.techViewSoundPath);
		this.techViewSoundPlaying = false;
		Shader.SetGlobalVector("_OverlayParams", Vector4.zero);
	}

	private void Update()
	{
		OverlayScreen.LayerInfo currentLayerInfo = this.GetCurrentLayerInfo();
		if (currentLayerInfo.IsValid())
		{
			this.UpdateOverlayView(currentLayerInfo);
		}
	}

	private static bool HasHarvestableComponent(GameObject go)
	{
		return go.GetComponent<Harvestable>() != null;
	}

	public void ToggleOverlay(SimViewMode newMode)
	{
		this.UpdateOverlaySounds(newMode);
		if (newMode != SimViewMode.None)
		{
			ManagementMenu.Instance.CloseAll();
		}
		SimViewMode simViewMode = this.currentMode;
		if (simViewMode != SimViewMode.TemperatureMap)
		{
			if (simViewMode != SimViewMode.Disease)
			{
				if (simViewMode != SimViewMode.Crop)
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
						this.DisableHighlightTypeOverlay(new Func<GameObject, bool>(OverlayScreen.HasHarvestableComponent), this.GetCurrentLayerInfo(), SaveLoader.Instance.saveManager.GetLists());
					}
				}
				else
				{
					this.DisableHarvestableUINotifications();
					this.DisableHighlightTypeOverlay(new Func<GameObject, bool>(OverlayScreen.HasHarvestableComponent), this.GetCurrentLayerInfo(), SaveLoader.Instance.saveManager.GetLists());
				}
			}
			else
			{
				this.DisableDiseaseOverlay();
			}
		}
		else
		{
			Infrared.Instance.SetMode(Infrared.Mode.Disabled);
			CameraController.Instance.ToggleColouredOverlayView(false);
		}
		if (this.GetCurrentLayerInfo().IsValid())
		{
			this.ToggleOverlayView();
		}
		if (newMode != this.currentMode && newMode == SimViewMode.None)
		{
			ManagementMenu.Instance.CloseAll();
		}
		ResourceCategoryScreen.Instance.Show(newMode == SimViewMode.None);
		SimDebugView.Instance.SetMode(newMode);
		this.currentMode = newMode;
		simViewMode = this.currentMode;
		if (simViewMode != SimViewMode.TemperatureMap)
		{
			if (simViewMode == SimViewMode.Disease)
			{
				this.EnableDiseaseOverlay();
			}
		}
		else
		{
			Infrared.Instance.SetMode(Infrared.Mode.Infrared);
			CameraController.Instance.ToggleColouredOverlayView(true);
		}
		GridCompositor.Instance.ToggleMinor(newMode == SimViewMode.PowerMap || newMode == SimViewMode.GasVentMap || newMode == SimViewMode.LiquidVentMap);
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
		if (this.OnOverlayChanged != null)
		{
			this.OnOverlayChanged(this.currentMode);
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
				SelectTool.Instance.SetLayerMask(currentLayerInfo.selectionMask);
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
				if (simViewMode != SimViewMode.Disease)
				{
					if (simViewMode != SimViewMode.Crop)
					{
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
							this.UpdateHighlightTypeOverlay(new Func<GameObject, bool>(OverlayScreen.HasHarvestableComponent), layer_info, lists, this.harvestHighlightConditions);
						}
					}
					else
					{
						this.UpdateHighlightTypeOverlay(new Func<GameObject, bool>(OverlayScreen.HasHarvestableComponent), layer_info, lists, this.cropHighlightConditions);
						this.UpdateCropOverlayView(layer_info, lists);
					}
				}
				else
				{
					this.UpdateDiseaseOverlayView(layer_info, lists);
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

	public GameObject GetFreeCropUI()
	{
		GameObject gameObject;
		if (this.freeHarvestableNotificationIdx < this.harvestableNotificationList.Count)
		{
			gameObject = this.harvestableNotificationList[this.freeHarvestableNotificationIdx];
			gameObject.gameObject.SetActive(true);
			this.freeHarvestableNotificationIdx++;
		}
		else
		{
			gameObject = global::Util.KInstantiateUI(this.harvestableNotificationPrefab.gameObject, this.harvestableUIParent.transform.gameObject, false);
			this.harvestableNotificationList.Add(gameObject);
			this.freeHarvestableNotificationIdx++;
		}
		return gameObject;
	}

	public GameObject GetFreeDiseaseUI()
	{
		GameObject gameObject;
		if (this.freeDiseaseUI < this.diseaseUIList.Count)
		{
			gameObject = this.diseaseUIList[this.freeDiseaseUI];
			gameObject.gameObject.SetActive(true);
			this.freeDiseaseUI++;
		}
		else
		{
			gameObject = global::Util.KInstantiateUI(this.diseaseOverlayPrefab, this.diseaseUIParent.transform.gameObject, false);
			this.diseaseUIList.Add(gameObject);
			this.freeDiseaseUI++;
		}
		return gameObject;
	}

	private void DisableHighlightTypeOverlay(Func<GameObject, bool> set_layer_fn, OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_objects)
	{
		Vector2I vector2I;
		Vector2I vector2I2;
		Grid.GetVisibleExtents(out vector2I, out vector2I2);
		int num = LayerMask.NameToLayer("MaskedOverlayBG");
		foreach (Tag tag in layer_info.itemIDs)
		{
			List<SaveLoadRoot> list;
			if (registered_objects.TryGetValue(tag, out list))
			{
				foreach (SaveLoadRoot saveLoadRoot in list)
				{
					GameObject gameObject = saveLoadRoot.gameObject;
					Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
					if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.targetViewData.layerTargets.Contains(gameObject))
					{
						KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
						if (component != null)
						{
							int num2 = ((!set_layer_fn(gameObject)) ? num : layer_info.layer);
							component.SetLayer(num2);
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
				KBatchedAnimController component2 = gameObject2.GetComponent<KBatchedAnimController>();
				if (component2 != null)
				{
					component2.HighlightColour = color;
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

	private void DisableHarvestableUINotifications()
	{
		this.freeHarvestableNotificationIdx = 0;
		foreach (GameObject gameObject in this.harvestableNotificationList)
		{
			gameObject.SetActive(false);
		}
		this.updateCropInfo.Clear();
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

	private void AddCropUI(Harvestable harvestable)
	{
		GameObject freeCropUI = this.GetFreeCropUI();
		OverlayScreen.UpdateCropInfo updateCropInfo = new OverlayScreen.UpdateCropInfo(harvestable, freeCropUI);
		Vector3 vector = Grid.CellToPos(Grid.PosToCell(harvestable), 0.5f, -1.25f, 0f);
		freeCropUI.GetComponent<RectTransform>().position = Vector3.up + vector;
		this.updateCropInfo.Add(updateCropInfo);
	}

	private void AddDiseaseUI(GameObject target)
	{
		GameObject gameObject = this.GetFreeDiseaseUI();
		DiseaseOverlayWidget component = gameObject.GetComponent<DiseaseOverlayWidget>();
		AmountInstance amountInstance = target.GetComponent<Modifiers>().amounts.Get(Db.Get().Amounts.ImmuneLevel);
		OverlayScreen.UpdateDiseaseInfo updateDiseaseInfo = new OverlayScreen.UpdateDiseaseInfo(amountInstance, component);
		Vector3 vector = new Vector3(0f, -1f, 0f);
		gameObject.GetComponent<RectTransform>().position = target.transform.position + vector;
		this.updateDiseaseInfo.Add(updateDiseaseInfo);
	}

	private void SetToolTip(LocText label, string text)
	{
		ToolTip component = label.GetComponent<ToolTip>();
		if (component != null)
		{
			component.toolTip = text;
		}
	}

	private void UpdateCropOverlayView(OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings)
	{
		using (new KProfiler.Region("UpdateCropOverlay", null))
		{
			this.queuedAdds.Clear();
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			using (new KProfiler.Region("CropUI", null))
			{
				foreach (Harvestable harvestable in Components.Harvestables)
				{
					GameObject gameObject = harvestable.gameObject;
					Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
					if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.targetViewData.privateTargets.Contains(gameObject))
					{
						this.AddCropUI(harvestable);
						this.queuedAdds.Add(gameObject);
					}
				}
				foreach (GameObject gameObject2 in this.queuedAdds)
				{
					this.targetViewData.privateTargets.Add(gameObject2);
				}
				this.queuedAdds.Clear();
			}
		}
		foreach (OverlayScreen.UpdateCropInfo updateCropInfo in this.updateCropInfo)
		{
			updateCropInfo.harvestableUI.GetComponent<HarvestableOverlayWidget>().Refresh(updateCropInfo.harvestable);
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
					bool flag = layer_info.itemIDs.Contains(keyValuePair.Key);
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
			bool flag = layer_info.itemIDs.Contains(keyValuePair.Key);
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

	private void UpdateHighlightTypeOverlay(Func<GameObject, bool> should_highlight, OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings, OverlayScreen.ColorHighlightCondition[] highlights)
	{
		Vector2I vector2I;
		Vector2I vector2I2;
		Grid.GetVisibleExtents(out vector2I, out vector2I2);
		this.outsideViewObjects.Clear();
		foreach (GameObject gameObject in this.targetViewData.layerTargets)
		{
			if (!(gameObject == null))
			{
				Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
				if (!(vector2I <= vector2I3) || !(vector2I3 <= vector2I2))
				{
					this.outsideViewObjects.Add(gameObject);
				}
			}
		}
		foreach (GameObject gameObject2 in this.outsideViewObjects)
		{
			if (!(gameObject2 == null))
			{
				KBatchedAnimController component = gameObject2.GetComponent<KBatchedAnimController>();
				if (!(component == null))
				{
					component.HighlightColour = Color.clear;
					this.targetViewData.layerTargets.Remove(gameObject2);
				}
			}
		}
		this.outsideViewObjects.Clear();
		foreach (Tag tag in layer_info.itemIDs)
		{
			List<SaveLoadRoot> list;
			if (registered_buildings.TryGetValue(tag, out list))
			{
				foreach (SaveLoadRoot saveLoadRoot in list)
				{
					if (!(saveLoadRoot == null))
					{
						if (Grid.Visible[Grid.PosToCell(saveLoadRoot.gameObject)] > 0 || DebugHandler.FreeCameraMode)
						{
							GameObject gameObject3 = saveLoadRoot.gameObject;
							KBatchedAnimController component2 = gameObject3.GetComponent<KBatchedAnimController>();
							if (!(component2 == null))
							{
								if (!this.targetViewData.layerTargets.Contains(gameObject3))
								{
									this.targetViewData.layerTargets.Add(gameObject3);
								}
								component2.SetLayer(layer_info.layer);
								Color32 color = Color.clear;
								if (should_highlight(gameObject3) && highlights != null)
								{
									foreach (OverlayScreen.ColorHighlightCondition colorHighlightCondition in highlights)
									{
										if (colorHighlightCondition.highlight_condition(gameObject3))
										{
											color = colorHighlightCondition.highlight_color;
										}
									}
								}
								component2.HighlightColour = color;
							}
						}
					}
				}
			}
		}
	}

	private void EnableDiseaseOverlay()
	{
		Infrared.Instance.SetMode(Infrared.Mode.Disease);
		CameraController.Instance.ToggleColouredOverlayView(true);
	}

	private void DisableDiseaseOverlay()
	{
		CameraController.Instance.ToggleColouredOverlayView(false);
		Infrared.Instance.SetMode(Infrared.Mode.Disabled);
		OverlayLegend.Instance.DisableDiseaseOverlay();
		Game.Instance.showGasConduitDisease = false;
		Game.Instance.showLiquidConduitDisease = false;
		this.freeDiseaseUI = 0;
		foreach (OverlayScreen.UpdateDiseaseInfo updateDiseaseInfo in this.updateDiseaseInfo)
		{
			updateDiseaseInfo.ui.gameObject.SetActive(false);
		}
		this.updateDiseaseInfo.Clear();
	}

	private void UpdateOverlaySounds(SimViewMode mode)
	{
		string text = string.Empty;
		if (mode != this.currentMode && mode != SimViewMode.None)
		{
			if (mode != SimViewMode.HeatFlow && mode != SimViewMode.ThermalConductivity)
			{
				if (mode != SimViewMode.TemperatureMap)
				{
					if (mode != SimViewMode.Disease)
					{
						if (mode != SimViewMode.Light)
						{
							if (mode != SimViewMode.Decor)
							{
								if (mode != SimViewMode.OxygenMap)
								{
									if (mode != SimViewMode.Crop && mode != SimViewMode.HarvestWhenReady)
									{
										if (mode != SimViewMode.LiquidVentMap)
										{
											if (mode != SimViewMode.PowerMap)
											{
												if (mode != SimViewMode.Priorities)
												{
													if (mode == SimViewMode.GasVentMap)
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
						text = "Disease";
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
		}
		else if (this.currentMode != SimViewMode.None)
		{
			text = "Off";
		}
		if (text != string.Empty)
		{
			text = GlobalAssets.GetSound(text, false);
			KMonoBehaviour.PlaySound(text);
		}
	}

	private void UpdateDiseaseOverlayView(OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings)
	{
		using (new KProfiler.Region("UpdateDiseaseCarriers", null))
		{
			this.queuedAdds.Clear();
			Vector2I vector2I;
			Vector2I vector2I2;
			Grid.GetVisibleExtents(out vector2I, out vector2I2);
			foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities)
			{
				GameObject gameObject = minionIdentity.gameObject;
				Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
				if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.targetViewData.privateTargets.Contains(gameObject))
				{
					this.AddDiseaseUI(gameObject);
					this.queuedAdds.Add(gameObject);
				}
			}
			foreach (GameObject gameObject2 in this.queuedAdds)
			{
				this.targetViewData.privateTargets.Add(gameObject2);
			}
			this.queuedAdds.Clear();
		}
		foreach (OverlayScreen.UpdateDiseaseInfo updateDiseaseInfo in this.updateDiseaseInfo)
		{
			updateDiseaseInfo.ui.Refresh(updateDiseaseInfo.valueSrc);
		}
	}

	public static HashSet<Tag> WireIDs = new HashSet<Tag>();

	public static HashSet<Tag> GasVentIDs = new HashSet<Tag>();

	public static HashSet<Tag> LiquidVentIDs = new HashSet<Tag>();

	public static HashSet<Tag> HarvestableIDs = new HashSet<Tag>();

	private static readonly Tag[] DiseaseIDs = new Tag[] { GameTags.Minion };

	private static readonly Tag[] OxygenBreatherIDs = new Tag[]
	{
		GameTags.Minion,
		GameTags.OxyRock
	};

	private OverlayScreen.ColorHighlightCondition[] cropHighlightConditions;

	private OverlayScreen.ColorHighlightCondition[] harvestHighlightConditions;

	private OverlayScreen.ColorHighlightCondition[] diseaseHighlightConditions;

	[EventRef]
	[SerializeField]
	private string techViewSoundPath;

	private EventInstance techViewSound;

	private bool techViewSoundPlaying;

	private OverlayScreen.LayerInfo[] itemOverlays;

	public static OverlayScreen Instance;

	[Header("Power")]
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
	private Color buildingDisabledColour;

	private int freePowerLabelIdx;

	private List<LocText> powerLabels;

	private int freeBatUIIdx;

	private List<BatteryUI> batteryUIList;

	[Header("Circuits")]
	[SerializeField]
	private Color32 circuitUnpoweredColour;

	[SerializeField]
	private Color32 circuitSafeColour;

	[SerializeField]
	private Color32 circuitStrainingColour;

	[SerializeField]
	[Header("Crops")]
	private Canvas harvestableUIParent;

	[SerializeField]
	private GameObject harvestableNotificationPrefab;

	private int freeHarvestableNotificationIdx;

	private List<GameObject> harvestableNotificationList;

	[SerializeField]
	[Header("Disease")]
	private Canvas diseaseUIParent;

	[SerializeField]
	private GameObject diseaseOverlayPrefab;

	private int freeDiseaseUI;

	private List<GameObject> diseaseUIList;

	[SerializeField]
	[Header("ToolTip")]
	private TextStyleSetting TooltipHeader;

	[SerializeField]
	private TextStyleSetting TooltipDescription;

	public Action<SimViewMode> OnOverlayChanged;

	private SimViewMode currentMode;

	private OverlayScreen.TargetViewData targetViewData;

	private List<GameObject> nonVisibleTargets;

	private List<OverlayScreen.UpdatePowerInfo> updatePowerInfo;

	private List<OverlayScreen.UpdateBatteryInfo> updateBatteryInfo;

	private List<OverlayScreen.UpdateCropInfo> updateCropInfo;

	private List<OverlayScreen.UpdateDiseaseInfo> updateDiseaseInfo;

	private List<GameObject> queuedAdds;

	private List<GameObject> outsideViewObjects;

	private struct LayerInfo
	{
		public LayerInfo(SimViewMode viewMode, string[] layerNames, ICollection<Tag> itemIDs, global::System.Action onEnable = null, global::System.Action onDisable = null)
		{
			this.viewMode = viewMode;
			this.mask = LayerMask.GetMask(layerNames);
			this.selectionMask = this.mask;
			this.layer = LayerMask.NameToLayer(layerNames[0]);
			this.itemIDs = itemIDs;
			this.onEnable = onEnable;
			this.onDisable = onDisable;
		}

		public LayerInfo(SimViewMode viewMode, string[] layerNames, int selectionMask, ICollection<Tag> itemIDs, global::System.Action onEnable = null, global::System.Action onDisable = null)
		{
			this.viewMode = viewMode;
			this.mask = LayerMask.GetMask(layerNames);
			this.selectionMask = selectionMask;
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

		public int selectionMask;

		public ICollection<Tag> itemIDs;

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

	private struct UpdateCropInfo
	{
		public UpdateCropInfo(Harvestable harvestable, GameObject harvestableUI)
		{
			this.harvestable = harvestable;
			this.harvestableUI = harvestableUI;
		}

		public Harvestable harvestable;

		public GameObject harvestableUI;
	}

	private struct UpdateDiseaseInfo
	{
		public UpdateDiseaseInfo(AmountInstance amount_inst, DiseaseOverlayWidget ui)
		{
			this.ui = ui;
			this.valueSrc = amount_inst;
		}

		public DiseaseOverlayWidget ui;

		public AmountInstance valueSrc;
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

	private struct ColorHighlightCondition
	{
		public ColorHighlightCondition(Color highlight_color, Func<GameObject, bool> highlight_condition)
		{
			this.highlight_color = highlight_color;
			this.highlight_condition = highlight_condition;
		}

		public Color32 highlight_color;

		public Func<GameObject, bool> highlight_condition;
	}
}
