using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OverlayScreen : KIconButtonMenu
{
	private void GetOverlayButtonTooltip(ToolTip mTooltip, KIconButtonMenu.ButtonInfo button_info, OverlayScreen.OverlayButtonInfo button)
	{
		mTooltip.OnToolTip = delegate
		{
			mTooltip.ClearMultiStringTooltip();
			mTooltip.AddMultiStringTooltip(button_info.GetTooltipText(), this.TooltipHeader);
			mTooltip.AddMultiStringTooltip(button.description, this.TooltipDescription);
			return string.Empty;
		};
	}

	protected override void OnPrefabInit()
	{
		OverlayScreen.overlayButtons = new OverlayScreen.OverlayButtonInfo[]
		{
			new OverlayScreen.OverlayButtonInfo("overlay_oxygen", UI.OVERLAYS.OXYGEN.BUTTON, SimViewMode.OxygenMap, "Oxygen", UI.TOOLTIPS.OXYGENOVERLAYSTRING),
			new OverlayScreen.OverlayButtonInfo("overlay_power", UI.OVERLAYS.ELECTRICAL.BUTTON, SimViewMode.PowerMap, "Power", UI.TOOLTIPS.POWEROVERLAYSTRING),
			new OverlayScreen.OverlayButtonInfo("overlay_temperature", UI.OVERLAYS.TEMPERATURE.BUTTON, SimViewMode.TemperatureMap, "Temperature", UI.TOOLTIPS.TEMPERATUREOVERLAYSTRING),
			new OverlayScreen.OverlayButtonInfo("overlay_lights", UI.OVERLAYS.LIGHTING.BUTTON, SimViewMode.Light, "Lights", UI.TOOLTIPS.LIGHTSOVERLAYSTRING),
			new OverlayScreen.OverlayButtonInfo("overlay_liquidvent", UI.OVERLAYS.LIQUIDPLUMBING.BUTTON, SimViewMode.LiquidVentMap, "LiquidVent", UI.TOOLTIPS.LIQUIDVENTOVERLAYSTRING),
			new OverlayScreen.OverlayButtonInfo("overlay_gasvent", UI.OVERLAYS.GASPLUMBING.BUTTON, SimViewMode.GasVentMap, "GasVent", UI.TOOLTIPS.GASVENTOVERLAYSTRING),
			new OverlayScreen.OverlayButtonInfo("overlay_decor", UI.OVERLAYS.DECOR.BUTTON, SimViewMode.Decor, "Decor", UI.TOOLTIPS.DECOROVERLAYSTRING),
			new OverlayScreen.OverlayButtonInfo("overlay_priority", UI.OVERLAYS.PRIORITIES.BUTTON, SimViewMode.Priorities, "Priorities", UI.TOOLTIPS.DECOROVERLAYSTRING)
		};
		OverlayScreen.Instance = this;
		this.powerLabelParent = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
		List<Tag> list = new List<Tag>(OverlayScreen.WireIDs);
		List<Tag> list2 = new List<Tag>();
		List<Tag> list3 = new List<Tag>(OverlayScreen.OxygenBreatherIDs);
		List<Tag> list4 = new List<Tag>(OverlayScreen.LiquidVentIDs);
		List<Tag> list5 = new List<Tag>(OverlayScreen.GasVentIDs);
		List<Tag> list6 = new List<Tag>();
		this.itemOverlays = new OverlayScreen.LayerInfo[]
		{
			new OverlayScreen.LayerInfo(SimViewMode.OxygenMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list3.ToArray()),
			new OverlayScreen.LayerInfo(SimViewMode.PowerMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list.ToArray()),
			new OverlayScreen.LayerInfo(SimViewMode.Rooms, new string[] { "Regions" }, OverlayScreen.RoomBuildingsIDs),
			new OverlayScreen.LayerInfo(SimViewMode.Light, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list2.ToArray()),
			new OverlayScreen.LayerInfo(SimViewMode.Regions, new string[] { "Regions" }, list6.ToArray()),
			new OverlayScreen.LayerInfo(SimViewMode.LiquidVentMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list4.ToArray()),
			new OverlayScreen.LayerInfo(SimViewMode.GasVentMap, new string[] { "MaskedOverlay", "MaskedOverlayBG" }, list5.ToArray())
		};
		this.keepMenuOpen = true;
		this.automaticNavigation = false;
		this.buttons = new KIconButtonMenu.ButtonInfo[OverlayScreen.overlayButtons.Length];
		for (int i = 0; i < OverlayScreen.overlayButtons.Length; i++)
		{
			OverlayScreen.OverlayButtonInfo info = OverlayScreen.overlayButtons[i];
			int idx = i;
			KIconButtonMenu.ButtonInfo buttonInfo = new KIconButtonMenu.ButtonInfo(info.icon, info.text, delegate
			{
				this.OnSelect(idx, info.viewMode);
			}, global::Action.Overlay1 + i, null, null, null, null, string.Empty);
			this.buttons[i] = buttonInfo;
		}
	}

	public override void RefreshButtons()
	{
		base.RefreshButtons();
		for (int i = 0; i < this.buttonObjects.Length; i++)
		{
			GameObject gameObject = this.buttonObjects[i];
			if (!(gameObject == null))
			{
				gameObject.GetComponent<ImageToggleState>().SetInactive();
				KIconButtonMenu.ButtonInfo buttonInfo = this.buttons[i];
				KToggle component = gameObject.GetComponent<KToggle>();
				if (!(component == null))
				{
					ToolTip component2 = component.GetComponent<ToolTip>();
					if (component2)
					{
						this.GetOverlayButtonTooltip(component2, this.buttons[i], OverlayScreen.overlayButtons[i]);
					}
					Image fgImage = component.fgImage;
					if (fgImage != null)
					{
						fgImage.gameObject.SetActive(false);
						foreach (Sprite sprite in this.icons)
						{
							if (sprite.name == buttonInfo.iconName)
							{
								fgImage.sprite = sprite;
								fgImage.gameObject.SetActive(true);
								break;
							}
						}
					}
				}
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		base.OnKeyDown(e);
		if (e.Consumed)
		{
			return;
		}
		if (this.currentMode != SimViewMode.None && (e.TryConsume(global::Action.MouseRight) || e.TryConsume(global::Action.Escape)))
		{
			foreach (KIconButtonMenu.ButtonInfo buttonInfo in this.buttons)
			{
				if (buttonInfo.buttonGo == this.currentlySelectedToggle.gameObject)
				{
					buttonInfo.onClick();
					break;
				}
			}
		}
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

	protected override void OnActivate()
	{
		base.OnActivate();
		base.Subscribe(Game.Instance.gameObject, 1248612973, new global::EventSystem.EventHandler(this.OnEnableOverlay));
		base.Subscribe(Game.Instance.gameObject, 2015652040, new global::EventSystem.EventHandler(this.OnDisableOverlay));
	}

	protected override void OnDeactivate()
	{
		if (Game.Instance != null)
		{
			base.Unsubscribe(Game.Instance.gameObject, 1248612973, new global::EventSystem.EventHandler(this.OnEnableOverlay));
			base.Unsubscribe(Game.Instance.gameObject, 2015652040, new global::EventSystem.EventHandler(this.OnDisableOverlay));
		}
		base.OnDeactivate();
	}

	private void OnSelect(int idx, SimViewMode mode)
	{
		if (global::UnityEngine.EventSystems.EventSystem.current == null || !global::UnityEngine.EventSystems.EventSystem.current.enabled)
		{
			return;
		}
		this.ToggleOverlay(idx, mode);
	}

	private void ToggleOverlay(int idx, SimViewMode mode)
	{
		string text = ((mode == this.currentMode || mode == SimViewMode.None) ? "Off" : OverlayScreen.overlayButtons[idx].sound);
		string sound = GlobalAssets.GetSound(text, false);
		KMonoBehaviour.PlaySound(sound);
		if (this.GetCurrentLayerInfo().IsValid())
		{
			this.ToggleOverlayView();
		}
		SimViewMode simViewMode = this.currentMode;
		if (simViewMode != SimViewMode.TemperatureMap)
		{
			if (simViewMode == SimViewMode.PowerMap)
			{
				this.DisablePowerLabels();
				this.DisableBatteryUIs();
			}
		}
		else
		{
			Infrared.Instance.Toggle(false);
			CameraController.Instance.ToggleTemperatureView(false);
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
			KToggle component = this.buttonObjects[idx].gameObject.GetComponent<KToggle>();
			if (component != null)
			{
				component.Select();
				component.isOn = true;
			}
			else
			{
				KBasicToggle component2 = this.buttonObjects[idx].gameObject.GetComponent<KBasicToggle>();
				if (component2 != null)
				{
					component2.isOn = true;
				}
			}
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
				targets = new HashSet<GameObject>()
			};
			if (currentLayerInfo.IsValid())
			{
				Camera.main.cullingMask |= currentLayerInfo.mask;
				SelectTool.Instance.SetLayerMask(currentLayerInfo.mask);
				DragTool.SetLayerMask(currentLayerInfo.mask);
			}
		}
		else
		{
			foreach (GameObject gameObject in this.targetViewData.targets)
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
			Dictionary<Tag, List<SaveLoadRoot>> lists = saveManager.GetLists();
			if (this.currentMode == SimViewMode.PowerMap)
			{
				this.DisablePowerLabels();
				this.DisableBatteryUIs();
			}
			this.nonVisibleTargets.Clear();
			foreach (GameObject gameObject in this.targetViewData.targets)
			{
				GameObject gameObject2 = gameObject.gameObject;
				if (!(gameObject2 == null))
				{
					KBatchedAnimController component = gameObject2.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						Vector2I vector2I3 = Grid.PosToXY(gameObject2.transform.position);
						if (vector2I <= vector2I3 && vector2I3 <= vector2I2)
						{
							component.TintColour = Color.white;
							component.SetLayer(gameObject2.GetComponent<KPrefabID>().defaultLayer);
							this.nonVisibleTargets.Add(gameObject2);
						}
					}
				}
			}
			foreach (GameObject gameObject3 in this.nonVisibleTargets)
			{
				this.targetViewData.targets.Remove(gameObject3);
			}
			this.nonVisibleTargets.Clear();
			if (layer_info.itemIDs != null)
			{
				SimViewMode simViewMode = this.currentMode;
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
		}
	}

	private bool SetUtilityColours(Color32 base_colour, GameObject root_obj, KAnimGraphTileVisualizer item, int cell)
	{
		if (item == null)
		{
			return false;
		}
		if (root_obj.GetComponent<BuildingComplete>() == null)
		{
			return false;
		}
		base_colour.a = 0;
		KBatchedAnimController component = root_obj.GetComponent<KBatchedAnimController>();
		component.TintColour = base_colour;
		return true;
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

	private int GetNetworkIDInDirection(Orientation direction, KAnimGraphTileVisualizer item)
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

	private void OnEnableOverlay(object data)
	{
		SimViewMode simViewMode = (SimViewMode)((int)data);
		if (this.currentMode != simViewMode)
		{
			int viewModeIdx = this.GetViewModeIdx(simViewMode);
			KToggle ktoggle = ((simViewMode != SimViewMode.None) ? this.buttonObjects[viewModeIdx].GetComponent<KToggle>() : null);
			base.SelectToggle(ktoggle);
			this.OnSelect(viewModeIdx, simViewMode);
		}
	}

	private void OnDisableOverlay(object data)
	{
		SimViewMode simViewMode = (SimViewMode)((int)data);
		if (this.currentMode == simViewMode)
		{
			int viewModeIdx = this.GetViewModeIdx(simViewMode);
			base.SelectToggle(this.buttonObjects[viewModeIdx].GetComponent<KToggle>());
			this.OnSelect(viewModeIdx, simViewMode);
		}
	}

	private int GetViewModeIdx(SimViewMode mode)
	{
		int num = 0;
		for (int i = 0; i < OverlayScreen.overlayButtons.Length; i++)
		{
			if (mode == OverlayScreen.overlayButtons[i].viewMode)
			{
				num = i;
				break;
			}
		}
		return num;
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
		if (this.freePowerLabelIdx < this.batteryUIList.Count)
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

	private void DisablePowerLabels()
	{
		this.freePowerLabelIdx = 0;
		foreach (LocText locText in this.powerLabels)
		{
			locText.gameObject.SetActive(false);
		}
	}

	private void DisableBatteryUIs()
	{
		this.freeBatUIIdx = 0;
		foreach (BatteryUI batteryUI in this.batteryUIList)
		{
			batteryUI.gameObject.SetActive(false);
		}
	}

	private void AddPowerLabels(KBatchedAnimController controller, KMonoBehaviour item)
	{
		IEnergyConsumer componentInChildren = item.gameObject.GetComponentInChildren<IEnergyConsumer>();
		Generator componentInChildren2 = item.gameObject.GetComponentInChildren<Generator>();
		if (componentInChildren != null || componentInChildren2 != null)
		{
			float num = -10f;
			if (componentInChildren2 != null && componentInChildren == null)
			{
				LocText freePowerLabel = this.GetFreePowerLabel();
				freePowerLabel.gameObject.SetActive(true);
				freePowerLabel.gameObject.name = item.gameObject.name + "power label";
				LocText component = freePowerLabel.transform.GetChild(0).GetComponent<LocText>();
				component.gameObject.SetActive(true);
				freePowerLabel.enabled = true;
				component.enabled = true;
				ManualGenerator component2 = componentInChildren2.GetComponent<ManualGenerator>();
				int num2;
				if (component2 == null)
				{
					componentInChildren2.GetComponent<Operational>();
					num2 = Mathf.Max(0, Mathf.RoundToInt(componentInChildren2.WattageRating));
				}
				else
				{
					num2 = Mathf.Max(0, Mathf.RoundToInt(componentInChildren2.WattageRating));
				}
				freePowerLabel.text = ((num2 == 0) ? num2.ToString() : ("+" + num2.ToString()));
				Color color = this.generatorColour;
				BuildingEnabledButton component3 = item.GetComponent<BuildingEnabledButton>();
				if ((component3 != null && !component3.IsEnabled) || componentInChildren2.CircuitID == 65535)
				{
					color = this.buildingDisabledColour;
				}
				Vector3 vector = Grid.CellToPos(componentInChildren2.PowerCell, 0.5f, 0f, 0f);
				freePowerLabel.rectTransform.position = vector + this.powerLabelOffset + Vector3.up * (num * 0.02f);
				freePowerLabel.color = color;
				component.color = color;
				Image outputIcon = componentInChildren2.GetComponent<BuildingCellVisualizer>().GetOutputIcon();
				if (outputIcon != null)
				{
					outputIcon.color = color;
				}
				num -= 15f;
				this.SetToolTip(freePowerLabel, "Watts Generated");
			}
			if (componentInChildren != null)
			{
				LocText freePowerLabel2 = this.GetFreePowerLabel();
				LocText component4 = freePowerLabel2.transform.GetChild(0).GetComponent<LocText>();
				freePowerLabel2.gameObject.SetActive(true);
				component4.gameObject.SetActive(true);
				freePowerLabel2.gameObject.name = item.gameObject.name + "power label";
				freePowerLabel2.enabled = true;
				component4.enabled = true;
				Color color2 = this.consumerColour;
				BuildingEnabledButton component5 = item.GetComponent<BuildingEnabledButton>();
				if ((component5 != null && !component5.IsEnabled) || Game.Instance.circuitManager.GetCircuitID(componentInChildren.PowerCell) == 65535)
				{
					color2 = this.buildingDisabledColour;
				}
				int num3 = Mathf.Max(0, Mathf.RoundToInt(componentInChildren.WattsNeededWhenActive));
				string text = num3.ToString();
				freePowerLabel2.text = ((num3 == 0) ? text : ("-" + text));
				freePowerLabel2.color = color2;
				component4.color = color2;
				Vector3 vector2 = Grid.CellToPos(componentInChildren.PowerCell, 0.5f, 0f, 0f);
				freePowerLabel2.rectTransform.position = vector2 + this.powerLabelOffset + Vector3.up * (num * 0.02f);
				Image inputIcon = item.GetComponentInChildren<BuildingCellVisualizer>().GetInputIcon();
				if (inputIcon != null)
				{
					inputIcon.color = color2;
				}
				this.SetToolTip(freePowerLabel2, "Watts Consumed");
			}
		}
	}

	private void AddBatteryUI(KMonoBehaviour item)
	{
		Battery component = item.GetComponent<Battery>();
		if (component == null)
		{
			return;
		}
		BatteryUI freeBatteryUI = this.GetFreeBatteryUI();
		freeBatteryUI.SetContent(component);
		Vector3 vector = Grid.CellToPos(component.PowerCell, 0.5f, 0f, 0f);
		freeBatteryUI.GetComponent<RectTransform>().position = vector + this.batteryUIOffset + Vector3.up;
	}

	private void SetToolTip(LocText label, string text)
	{
		ToolTip component = label.GetComponent<ToolTip>();
		if (component != null)
		{
			component.toolTip = text;
		}
	}

	public void DisableCurrentOverlay()
	{
		if (this.currentMode == SimViewMode.None)
		{
			return;
		}
		this.ToggleOverlay(this.GetViewModeIdx(this.currentMode), this.currentMode);
		base.ClearSelection();
	}

	private void UpdatePowerOverlayView(OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings)
	{
		Vector2I vector2I;
		Vector2I vector2I2;
		Grid.GetVisibleExtents(out vector2I, out vector2I2);
		int num = LayerMask.NameToLayer("MaskedOverlayBG");
		foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in registered_buildings)
		{
			List<SaveLoadRoot> value = keyValuePair.Value;
			bool flag = Array.IndexOf<Tag>(layer_info.itemIDs, keyValuePair.Key) != -1;
			int num2 = ((!(keyValuePair.Key == OverlayScreen.WireTag)) ? num : layer_info.layer);
			foreach (SaveLoadRoot saveLoadRoot in value)
			{
				GameObject gameObject = saveLoadRoot.gameObject;
				KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
				if (component != null)
				{
					Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
					if (vector2I <= vector2I3 && vector2I3 <= vector2I2)
					{
						if (flag && !this.targetViewData.targets.Contains(gameObject))
						{
							component.SetLayer(num2);
							this.targetViewData.targets.Add(gameObject);
						}
						this.AddPowerLabels(component, saveLoadRoot);
						this.AddBatteryUI(saveLoadRoot);
					}
				}
			}
		}
		CircuitManager circuitManager = Game.Instance.circuitManager;
		foreach (GameObject gameObject2 in this.targetViewData.targets)
		{
			if (!(gameObject2 == null))
			{
				KAnimGraphTileVisualizer component2 = gameObject2.GetComponent<KAnimGraphTileVisualizer>();
				int num3 = Grid.PosToCell(gameObject2.transform.position);
				ushort circuitID = circuitManager.GetCircuitID(num3);
				Color color = this.circuitBalancedColor;
				float wattsNeededWhenActive = circuitManager.GetWattsNeededWhenActive(circuitID);
				float wattsGeneratedByCircuit = circuitManager.GetWattsGeneratedByCircuit(circuitID);
				if (!circuitManager.HasPowerSource(circuitID))
				{
					color = this.circuitInsufficientColor;
				}
				else if (wattsNeededWhenActive <= wattsGeneratedByCircuit)
				{
					color = this.circuitExceedingColor;
				}
				else if (wattsNeededWhenActive > wattsGeneratedByCircuit)
				{
					color = this.circuitBalancedColor;
				}
				if (!this.SetUtilityColours(color, gameObject2, component2, num3))
				{
					this.SetBuildingColor(this.buildingOverlayColor, gameObject2);
				}
			}
		}
	}

	private void UpdateConduitOverlayView(OverlayScreen.LayerInfo layer_info, Dictionary<Tag, List<SaveLoadRoot>> registered_buildings)
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
					KBatchedAnimController component = gameObject.GetComponent<KBatchedAnimController>();
					if (component != null)
					{
						Vector2I vector2I3 = Grid.PosToXY(gameObject.transform.position);
						if (vector2I <= vector2I3 && vector2I3 <= vector2I2 && !this.targetViewData.targets.Contains(gameObject))
						{
							Vent component2 = gameObject.GetComponent<Vent>();
							if (component2 != null && component2.endpointType == Vent.Endpoint.Conduit)
							{
								component.SetLayer(layer_info.layer);
							}
							else
							{
								component.SetLayer(num);
							}
							this.targetViewData.targets.Add(gameObject);
						}
					}
				}
			}
		}
		foreach (GameObject gameObject2 in this.targetViewData.targets)
		{
			if (!(gameObject2 == null))
			{
				Color32 color = Color.white;
				Vent component3 = gameObject2.gameObject.GetComponent<Vent>();
				KAnimGraphTileVisualizer component4 = gameObject2.GetComponent<KAnimGraphTileVisualizer>();
				if (component3 != null)
				{
					switch (component3.endpointType)
					{
					case Vent.Endpoint.Conduit:
						color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
						break;
					case Vent.Endpoint.Source:
						color = this.utilitySourceColour;
						break;
					case Vent.Endpoint.Sink:
						color = this.utilitySinkColour;
						break;
					}
				}
				else
				{
					color = this.GetColourForID(-1);
				}
				int num2 = Grid.PosToCell(gameObject2);
				if (!this.SetUtilityColours(color, gameObject2, component4, num2))
				{
					this.SetBuildingColor(this.buildingOverlayColor, gameObject2);
				}
			}
		}
	}

	private const float lum = 0.25f;

	private const float GlowStrength = 0.25f;

	private static readonly Tag WireTag = TagManager.Create("Wire", null);

	private static readonly Tag[] WireIDs = new Tag[]
	{
		TagManager.Create("Wire", null),
		TagManager.Create("WireUnderConstruction", null),
		TagManager.Create("InsulatedWire", null),
		TagManager.Create("InsulatedWireUnderConstruction", null)
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

	private static readonly Tag[] RoomBuildingsIDs = new Tag[] { TagManager.Create("Bed", null) };

	[SerializeField]
	[EventRef]
	private string techViewSoundPath;

	private EventInstance techViewSound;

	private bool techViewSoundPlaying;

	private OverlayScreen.LayerInfo[] itemOverlays;

	public static OverlayScreen Instance;

	[SerializeField]
	private OverlayScreen.ConduitFlowVisInfo[] conduitFlowVisInfo;

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
	private Color consumerColour;

	[SerializeField]
	private Color generatorColour;

	[SerializeField]
	private Color joulesAvailableColour;

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
	private Color32 utilitySourceColour = Color.green;

	[SerializeField]
	private Color32 utilitySinkColour = Color.blue;

	[SerializeField]
	private Color circuitBalancedColor = Color.yellow;

	[SerializeField]
	private Color circuitInsufficientColor = Color.red;

	[SerializeField]
	private Color circuitExceedingColor = Color.green;

	[SerializeField]
	private Color buildingOverlayColor = Color.gray;

	public static Action<SimViewMode> OnOverlayChanged;

	private static OverlayScreen.OverlayButtonInfo[] overlayButtons;

	private SimViewMode currentMode;

	private OverlayScreen.TargetViewData targetViewData;

	private List<GameObject> nonVisibleTargets = new List<GameObject>();

	private struct LayerInfo
	{
		public LayerInfo(SimViewMode viewMode, string[] layerNames, Tag[] itemIDs)
		{
			this.viewMode = viewMode;
			this.mask = LayerMask.GetMask(layerNames);
			this.layer = LayerMask.NameToLayer(layerNames[0]);
			this.itemIDs = itemIDs;
		}

		public bool IsValid()
		{
			return this.mask != 0;
		}

		public SimViewMode viewMode;

		public int mask;

		public int layer;

		public Tag[] itemIDs;
	}

	[Serializable]
	private struct ConduitFlowVisInfo
	{
		[HashedEnum]
		public SimViewMode viewMode;

		public GameObject visualizerPrefab;

		public Vent.Transfer type;

		public LayerMask layerMask;

		public float previousLerpPercent;

		[NonSerialized]
		public ConduitFlowVisualizer visualizer;

		[NonSerialized]
		public ConduitFlow flowManager;

		[NonSerialized]
		public int renderLayer;
	}

	private struct OverlayButtonInfo
	{
		public OverlayButtonInfo(string icon, string text, SimViewMode viewMode, string sound, string description)
		{
			this.icon = icon;
			this.text = text;
			this.viewMode = viewMode;
			this.sound = sound;
			this.description = description;
		}

		public string icon;

		public string text;

		public SimViewMode viewMode;

		public string sound;

		public string description;
	}

	private class TargetViewData
	{
		public HashSet<GameObject> targets;
	}
}
