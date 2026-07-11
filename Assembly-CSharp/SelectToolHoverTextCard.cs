using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class SelectToolHoverTextCard : HoverTextConfiguration
{
	public SelectToolHoverTextCard()
	{
		Dictionary<HashedString, Func<KSelectable, bool>> dictionary = new Dictionary<HashedString, Func<KSelectable, bool>>();
		dictionary.Add(OverlayModes.Oxygen.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowOxygenOverlay));
		dictionary.Add(OverlayModes.Light.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowLightOverlay));
		dictionary.Add(OverlayModes.GasConduits.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowGasConduitOverlay));
		dictionary.Add(OverlayModes.LiquidConduits.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowLiquidConduitOverlay));
		dictionary.Add(OverlayModes.SolidConveyor.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowSolidConveyorOverlay));
		dictionary.Add(OverlayModes.Power.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowPowerOverlay));
		dictionary.Add(OverlayModes.Logic.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowLogicOverlay));
		dictionary.Add(OverlayModes.TileMode.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowTileOverlay));
		dictionary.Add(OverlayModes.Disease.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShowOverlayIfHasComponent<PrimaryElement>));
		dictionary.Add(OverlayModes.Decor.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShowOverlayIfHasComponent<DecorProvider>));
		dictionary.Add(OverlayModes.Crop.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowCropOverlay));
		dictionary.Add(OverlayModes.Temperature.ID, new Func<KSelectable, bool>(SelectToolHoverTextCard.ShouldShowTemperatureOverlay));
		this.modeFilters = dictionary;
		base..ctor();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.overlayFilterMap.Add(OverlayModes.Oxygen.ID, delegate
		{
			int num = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num].IsGas;
		});
		this.overlayFilterMap.Add(OverlayModes.GasConduits.ID, delegate
		{
			int num2 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num2].IsGas;
		});
		this.overlayFilterMap.Add(OverlayModes.LiquidConduits.ID, delegate
		{
			int num3 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			return Grid.Element[num3].IsLiquid;
		});
		this.overlayFilterMap.Add(OverlayModes.Decor.ID, () => false);
		this.overlayFilterMap.Add(OverlayModes.Rooms.ID, () => false);
		this.overlayFilterMap.Add(OverlayModes.Logic.ID, () => false);
		this.overlayFilterMap.Add(OverlayModes.TileMode.ID, delegate
		{
			int num4 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(KInputManager.GetMousePos()));
			Element element = Grid.Element[num4];
			foreach (Tag tag in Game.Instance.tileOverlayFilters)
			{
				if (element.HasTag(tag))
				{
					return true;
				}
			}
			return false;
		});
	}

	public override void ConfigureHoverScreen()
	{
		base.ConfigureHoverScreen();
		HoverTextScreen instance = HoverTextScreen.Instance;
		this.iconWarning = instance.GetSprite("iconWarning");
		this.iconDash = instance.GetSprite("dash");
		this.iconHighlighted = instance.GetSprite("dash_arrow");
		this.maskOverlay = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
	}

	private bool IsStatusItemWarning(StatusItemGroup.Entry item)
	{
		return item.item.notificationType == NotificationType.Bad || item.item.notificationType == NotificationType.BadMinor || item.item.notificationType == NotificationType.DuplicantThreatening;
	}

	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
	{
		if (this.iconWarning == null)
		{
			this.ConfigureHoverScreen();
		}
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()));
		if (OverlayScreen.Instance == null || !Grid.IsValidCell(num))
		{
			return;
		}
		HoverTextScreen instance = HoverTextScreen.Instance;
		HoverTextDrawer hoverTextDrawer = instance.BeginDrawing();
		this.overlayValidHoverObjects.Clear();
		foreach (KSelectable kselectable in hoverObjects)
		{
			if (this.ShouldShowSelectableInCurrentOverlay(kselectable))
			{
				this.overlayValidHoverObjects.Add(kselectable);
			}
		}
		this.currentSelectedSelectableIndex = -1;
		if (SelectToolHoverTextCard.highlightedObjects.Count > 0)
		{
			SelectToolHoverTextCard.highlightedObjects.Clear();
		}
		HashedString mode = SimDebugView.Instance.GetMode();
		bool flag = mode == OverlayModes.Disease.ID;
		bool flag2 = true;
		if (Grid.DupePassable[num])
		{
			flag2 = false;
		}
		bool flag3 = Grid.IsVisible(num);
		if (!flag3)
		{
			flag2 = false;
		}
		foreach (KeyValuePair<HashedString, Func<bool>> keyValuePair in this.overlayFilterMap)
		{
			if (OverlayScreen.Instance.GetMode() == keyValuePair.Key)
			{
				if (!keyValuePair.Value())
				{
					flag2 = false;
				}
				break;
			}
		}
		string text = string.Empty;
		string text2 = string.Empty;
		if (mode == OverlayModes.Temperature.ID && Game.Instance.temperatureOverlayMode == Game.TemperatureOverlayModes.HeatFlow)
		{
			if (!Grid.Solid[num] && flag3)
			{
				float thermalComfort = GameUtil.GetThermalComfort(num, 0f);
				float thermalComfort2 = GameUtil.GetThermalComfort(num, -0.083680004f);
				float num2 = 0f;
				if (thermalComfort2 * 0.001f > -0.27893335f - num2 && thermalComfort2 * 0.001f < 0.27893335f + num2)
				{
					text = UI.OVERLAYS.HEATFLOW.NEUTRAL;
				}
				else if (thermalComfort2 <= ExternalTemperatureMonitor.GetExternalColdThreshold(null))
				{
					text = UI.OVERLAYS.HEATFLOW.COOLING;
				}
				else if (thermalComfort2 >= ExternalTemperatureMonitor.GetExternalWarmThreshold(null))
				{
					text = UI.OVERLAYS.HEATFLOW.HEATING;
				}
				float num3 = 1f * thermalComfort;
				text = text + " (" + GameUtil.GetFormattedHeatEnergyRate(num3, GameUtil.HeatEnergyFormatterUnit.Automatic) + ")";
				hoverTextDrawer.BeginShadowBar(false);
				hoverTextDrawer.DrawText(UI.OVERLAYS.HEATFLOW.HOVERTITLE, this.Styles_Title.Standard);
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawText(text, this.Styles_BodyText.Standard);
				hoverTextDrawer.EndShadowBar();
			}
		}
		else if (mode == OverlayModes.Decor.ID)
		{
			List<DecorProvider> list = new List<DecorProvider>();
			GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.decorProviderLayer, list);
			float decorAtCell = GameUtil.GetDecorAtCell(num);
			hoverTextDrawer.BeginShadowBar(false);
			hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HOVERTITLE, this.Styles_Title.Standard);
			hoverTextDrawer.NewLine(26);
			hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.TOTAL + GameUtil.GetFormattedDecor(decorAtCell, true), this.Styles_BodyText.Standard);
			if (!Grid.Solid[num] && flag3)
			{
				List<EffectorEntry> list2 = new List<EffectorEntry>();
				List<EffectorEntry> list3 = new List<EffectorEntry>();
				foreach (DecorProvider decorProvider in list)
				{
					float decorForCell = decorProvider.GetDecorForCell(num);
					if (decorForCell != 0f)
					{
						string name = decorProvider.GetName();
						KMonoBehaviour component = decorProvider.GetComponent<KMonoBehaviour>();
						if (component != null && component.gameObject != null)
						{
							SelectToolHoverTextCard.highlightedObjects.Add(component.gameObject);
						}
						bool flag4 = false;
						if (decorForCell > 0f)
						{
							for (int i = 0; i < list2.Count; i++)
							{
								if (list2[i].name == name)
								{
									EffectorEntry effectorEntry = list2[i];
									effectorEntry.count++;
									effectorEntry.value += decorForCell;
									list2[i] = effectorEntry;
									flag4 = true;
									break;
								}
							}
							if (!flag4)
							{
								list2.Add(new EffectorEntry(name, decorForCell));
							}
						}
						else
						{
							for (int j = 0; j < list3.Count; j++)
							{
								if (list3[j].name == name)
								{
									EffectorEntry effectorEntry2 = list3[j];
									effectorEntry2.count++;
									effectorEntry2.value += decorForCell;
									list3[j] = effectorEntry2;
									flag4 = true;
									break;
								}
							}
							if (!flag4)
							{
								list3.Add(new EffectorEntry(name, decorForCell));
							}
						}
					}
				}
				int lightDecorBonus = DecorProvider.GetLightDecorBonus(num);
				if (lightDecorBonus > 0)
				{
					list2.Add(new EffectorEntry(UI.OVERLAYS.DECOR.LIGHTING, (float)lightDecorBonus));
				}
				list2.Sort((EffectorEntry x, EffectorEntry y) => y.value.CompareTo(x.value));
				if (list2.Count > 0)
				{
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HEADER_POSITIVE, this.Styles_BodyText.Standard);
				}
				foreach (EffectorEntry effectorEntry3 in list2)
				{
					hoverTextDrawer.NewLine(18);
					hoverTextDrawer.DrawIcon(this.iconDash, 18);
					hoverTextDrawer.DrawText(effectorEntry3.ToString(), this.Styles_BodyText.Standard);
				}
				list3.Sort((EffectorEntry x, EffectorEntry y) => Mathf.Abs(y.value).CompareTo(Mathf.Abs(x.value)));
				if (list3.Count > 0)
				{
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HEADER_NEGATIVE, this.Styles_BodyText.Standard);
				}
				foreach (EffectorEntry effectorEntry4 in list3)
				{
					hoverTextDrawer.NewLine(18);
					hoverTextDrawer.DrawIcon(this.iconDash, 18);
					hoverTextDrawer.DrawText(effectorEntry4.ToString(), this.Styles_BodyText.Standard);
				}
			}
			hoverTextDrawer.EndShadowBar();
		}
		else if (mode == OverlayModes.Rooms.ID)
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell != null)
			{
				Room room = cavityForCell.room;
				RoomType roomType = null;
				if (room != null)
				{
					roomType = room.roomType;
					text2 = roomType.Name;
				}
				else
				{
					text2 = UI.OVERLAYS.ROOMS.NOROOM.HEADER;
				}
				hoverTextDrawer.BeginShadowBar(false);
				hoverTextDrawer.DrawText(text2, this.Styles_Title.Standard);
				text = string.Empty;
				if (room != null)
				{
					string text3 = string.Empty;
					text3 = RoomDetails.EFFECT.resolve_string_function(room);
					string text4 = string.Empty;
					text4 = RoomDetails.ASSIGNED_TO.resolve_string_function(room);
					string text5 = string.Empty;
					text5 = RoomConstraints.RoomCriteriaString(room);
					string text6 = string.Empty;
					text6 = RoomDetails.EFFECTS.resolve_string_function(room);
					if (text3 != string.Empty)
					{
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawText(text3, this.Styles_BodyText.Standard);
					}
					if (text4 != string.Empty && roomType != Db.Get().RoomTypes.Neutral)
					{
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawText(text4, this.Styles_BodyText.Standard);
					}
					hoverTextDrawer.NewLine(22);
					hoverTextDrawer.DrawText(RoomDetails.RoomDetailString(room), this.Styles_BodyText.Standard);
					if (text5 != string.Empty)
					{
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawText(text5, this.Styles_BodyText.Standard);
					}
					if (text6 != string.Empty)
					{
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawText(text6, this.Styles_BodyText.Standard);
					}
				}
				else
				{
					string text7 = UI.OVERLAYS.ROOMS.NOROOM.DESC;
					int maxRoomSize = TuningData<RoomProber.Tuning>.Get().maxRoomSize;
					if (cavityForCell.numCells > maxRoomSize)
					{
						text7 = text7 + "\n" + string.Format(UI.OVERLAYS.ROOMS.NOROOM.TOO_BIG, cavityForCell.numCells, maxRoomSize);
					}
					hoverTextDrawer.NewLine(26);
					hoverTextDrawer.DrawText(text7, this.Styles_BodyText.Standard);
				}
				hoverTextDrawer.EndShadowBar();
			}
		}
		else if (mode == OverlayModes.Light.ID)
		{
			if (flag3)
			{
				string text8 = text;
				text = string.Concat(new string[]
				{
					text8,
					string.Format(UI.OVERLAYS.LIGHTING.DESC, Grid.LightIntensity[num]),
					" (",
					GameUtil.GetLightDescription(Grid.LightIntensity[num]),
					")"
				});
				hoverTextDrawer.BeginShadowBar(false);
				hoverTextDrawer.DrawText(UI.OVERLAYS.LIGHTING.HOVERTITLE, this.Styles_Title.Standard);
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawText(text, this.Styles_BodyText.Standard);
				hoverTextDrawer.EndShadowBar();
			}
		}
		else if (mode == OverlayModes.Logic.ID)
		{
			foreach (KSelectable kselectable2 in hoverObjects)
			{
				LogicPorts component2 = kselectable2.GetComponent<LogicPorts>();
				LogicPorts.Port port;
				bool flag5;
				if (component2 != null && component2.TryGetPortAtCell(num, out port, out flag5))
				{
					bool flag6 = component2.IsPortConnected(port.id);
					hoverTextDrawer.BeginShadowBar(false);
					int num4;
					if (flag5)
					{
						num4 = component2.GetInputValue(port.id);
						hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_INPUT_HOVER_FMT.Replace("{Port}", port.description).Replace("{Name}", kselectable2.GetProperName().ToUpper()), this.Styles_Title.Standard);
					}
					else
					{
						num4 = component2.GetOutputValue(port.id);
						hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_OUTPUT_HOVER_FMT.Replace("{Port}", port.description).Replace("{Name}", kselectable2.GetProperName().ToUpper()), this.Styles_Title.Standard);
					}
					hoverTextDrawer.NewLine(26);
					TextStyleSetting textStyleSetting = ((num4 != 1 || !flag6) ? this.Styles_LogicActive.Standard : this.Styles_LogicActive.Selected);
					hoverTextDrawer.DrawIcon((num4 != 1 || !flag6) ? this.iconDash : this.iconHighlighted, textStyleSetting.textColor, 18, 2);
					hoverTextDrawer.DrawText(port.activeDescription, textStyleSetting);
					hoverTextDrawer.NewLine(26);
					TextStyleSetting textStyleSetting2 = ((num4 != 0 || !flag6) ? this.Styles_LogicStandby.Standard : this.Styles_LogicStandby.Selected);
					hoverTextDrawer.DrawIcon((num4 != 0 || !flag6) ? this.iconDash : this.iconHighlighted, textStyleSetting2.textColor, 18, 2);
					hoverTextDrawer.DrawText(port.inactiveDescription, textStyleSetting2);
					hoverTextDrawer.EndShadowBar();
				}
				LogicGate component3 = kselectable2.GetComponent<LogicGate>();
				LogicGateBase.PortId portId;
				if (component3 != null && component3.TryGetPortAtCell(num, out portId))
				{
					int portValue = component3.GetPortValue(portId);
					bool portConnected = component3.GetPortConnected(portId);
					LogicGate.LogicGateDescriptions.Description portDescription = component3.GetPortDescription(portId);
					hoverTextDrawer.BeginShadowBar(false);
					if (portId == LogicGateBase.PortId.Output)
					{
						hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_OUTPUT_HOVER_FMT.Replace("{Port}", portDescription.name).Replace("{Name}", kselectable2.GetProperName().ToUpper()), this.Styles_Title.Standard);
					}
					else
					{
						hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.LOGIC_INPUT_HOVER_FMT.Replace("{Port}", portDescription.name).Replace("{Name}", kselectable2.GetProperName().ToUpper()), this.Styles_Title.Standard);
					}
					hoverTextDrawer.NewLine(26);
					TextStyleSetting textStyleSetting3 = ((portValue != 1 || !portConnected) ? this.Styles_LogicActive.Standard : this.Styles_LogicActive.Selected);
					hoverTextDrawer.DrawIcon((portValue != 1 || !portConnected) ? this.iconDash : this.iconHighlighted, textStyleSetting3.textColor, 18, 2);
					hoverTextDrawer.DrawText(portDescription.active, textStyleSetting3);
					hoverTextDrawer.NewLine(26);
					TextStyleSetting textStyleSetting4 = ((portValue != 0 || !portConnected) ? this.Styles_LogicStandby.Standard : this.Styles_LogicStandby.Selected);
					hoverTextDrawer.DrawIcon((portValue != 0 || !portConnected) ? this.iconDash : this.iconHighlighted, textStyleSetting4.textColor, 18, 2);
					hoverTextDrawer.DrawText(portDescription.inactive, textStyleSetting4);
					hoverTextDrawer.EndShadowBar();
				}
			}
		}
		int num5 = 0;
		ChoreConsumer choreConsumer = null;
		if (SelectTool.Instance.selected != null)
		{
			choreConsumer = SelectTool.Instance.selected.GetComponent<ChoreConsumer>();
		}
		for (int k = 0; k < this.overlayValidHoverObjects.Count; k++)
		{
			if (this.overlayValidHoverObjects[k] != null && this.overlayValidHoverObjects[k].GetComponent<CellSelectionObject>() == null)
			{
				KSelectable kselectable3 = this.overlayValidHoverObjects[k];
				if (!(OverlayScreen.Instance != null) || !(OverlayScreen.Instance.mode != OverlayModes.None.ID) || (kselectable3.gameObject.layer & this.maskOverlay) == 0)
				{
					if (flag3)
					{
						PrimaryElement component4 = kselectable3.GetComponent<PrimaryElement>();
						bool flag7 = SelectTool.Instance.selected == this.overlayValidHoverObjects[k];
						if (flag7)
						{
							this.currentSelectedSelectableIndex = k;
						}
						num5++;
						hoverTextDrawer.BeginShadowBar(flag7);
						string text9 = GameUtil.GetUnitFormattedName(this.overlayValidHoverObjects[k].gameObject, true);
						if (component4 != null && kselectable3.GetComponent<Building>() != null)
						{
							text9 = StringFormatter.Replace(StringFormatter.Replace(UI.TOOLS.GENERIC.BUILDING_HOVER_NAME_FMT, "{Name}", text9), "{Element}", component4.Element.nameUpperCase);
						}
						hoverTextDrawer.DrawText(text9, this.Styles_Title.Standard);
						bool flag8 = false;
						string text10 = UI.OVERLAYS.DISEASE.NO_DISEASE;
						if (flag)
						{
							if (component4 != null && component4.DiseaseIdx != 255)
							{
								text10 = GameUtil.GetFormattedDisease(component4.DiseaseIdx, component4.DiseaseCount, true);
							}
							flag8 = true;
							Storage component5 = kselectable3.GetComponent<Storage>();
							if (component5 != null && component5.showInUI)
							{
								List<GameObject> items = component5.items;
								for (int l = 0; l < items.Count; l++)
								{
									GameObject gameObject = items[l];
									if (gameObject != null)
									{
										PrimaryElement component6 = gameObject.GetComponent<PrimaryElement>();
										if (component6.DiseaseIdx != 255)
										{
											text10 += string.Format(UI.OVERLAYS.DISEASE.CONTAINER_FORMAT, gameObject.GetComponent<KSelectable>().GetProperName(), GameUtil.GetFormattedDisease(component6.DiseaseIdx, component6.DiseaseCount, true));
										}
									}
								}
							}
						}
						if (flag8)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawIcon(this.iconDash, 18);
							hoverTextDrawer.DrawText(text10, this.Styles_Values.Property.Standard);
						}
						int num6 = 0;
						foreach (StatusItemGroup.Entry entry in this.overlayValidHoverObjects[k].GetStatusItemGroup())
						{
							if (this.ShowStatusItemInCurrentOverlay(entry.item))
							{
								if (num6 >= SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
								{
									break;
								}
								if (entry.category != null && entry.category.Id == "Main" && num6 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
								{
									TextStyleSetting textStyleSetting5 = ((!this.IsStatusItemWarning(entry)) ? this.Styles_BodyText.Standard : this.HoverTextStyleSettings[1]);
									Sprite sprite = ((entry.item.sprite == null) ? this.iconWarning : entry.item.sprite.sprite);
									Color color = ((!this.IsStatusItemWarning(entry)) ? this.Styles_BodyText.Standard.textColor : this.HoverTextStyleSettings[1].textColor);
									hoverTextDrawer.NewLine(26);
									hoverTextDrawer.DrawIcon(sprite, color, 18, 2);
									hoverTextDrawer.DrawText(entry.GetName(), textStyleSetting5);
									num6++;
								}
							}
						}
						foreach (StatusItemGroup.Entry entry2 in this.overlayValidHoverObjects[k].GetStatusItemGroup())
						{
							if (this.ShowStatusItemInCurrentOverlay(entry2.item))
							{
								if (num6 >= SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
								{
									break;
								}
								if ((entry2.category == null || entry2.category.Id != "Main") && num6 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
								{
									TextStyleSetting textStyleSetting6 = ((!this.IsStatusItemWarning(entry2)) ? this.Styles_BodyText.Standard : this.HoverTextStyleSettings[1]);
									Sprite sprite2 = ((entry2.item.sprite == null) ? this.iconWarning : entry2.item.sprite.sprite);
									Color color2 = ((!this.IsStatusItemWarning(entry2)) ? this.Styles_BodyText.Standard.textColor : this.HoverTextStyleSettings[1].textColor);
									hoverTextDrawer.NewLine(26);
									hoverTextDrawer.DrawIcon(sprite2, color2, 18, 2);
									hoverTextDrawer.DrawText(entry2.GetName(), textStyleSetting6);
									num6++;
								}
							}
						}
						float num7 = 0f;
						bool flag9 = true;
						bool flag10 = OverlayModes.Temperature.ID == SimDebugView.Instance.GetMode() && Game.Instance.temperatureOverlayMode != Game.TemperatureOverlayModes.HeatFlow;
						if (kselectable3.GetComponent<Constructable>())
						{
							flag9 = false;
						}
						else if (flag10 && component4)
						{
							num7 = component4.Temperature;
						}
						else if (kselectable3.GetComponent<Building>() && component4)
						{
							num7 = component4.Temperature;
						}
						else if (kselectable3.GetComponent<CellSelectionObject>() != null)
						{
							num7 = kselectable3.GetComponent<CellSelectionObject>().temperature;
						}
						else
						{
							flag9 = false;
						}
						if (mode != OverlayModes.None.ID && mode != OverlayModes.Temperature.ID)
						{
							flag9 = false;
						}
						if (flag9)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawIcon(this.iconDash, 18);
							hoverTextDrawer.DrawText(GameUtil.GetFormattedTemperature(num7, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), this.Styles_BodyText.Standard);
						}
						BuildingComplete component7 = kselectable3.GetComponent<BuildingComplete>();
						if (component7 != null && component7.Def.IsFoundation)
						{
							flag2 = false;
						}
						if (mode == OverlayModes.Light.ID && choreConsumer != null)
						{
							bool flag11 = false;
							foreach (Type type in SelectToolHoverTextCard.hiddenChoreConsumerTypes)
							{
								if (choreConsumer.gameObject.GetComponent(type) != null)
								{
									flag11 = true;
									break;
								}
							}
							if (!flag11)
							{
								choreConsumer.ShowHoverTextOnHoveredItem(kselectable3, hoverTextDrawer, this);
							}
						}
						hoverTextDrawer.EndShadowBar();
					}
				}
			}
		}
		if (flag2)
		{
			CellSelectionObject cellSelectionObject = null;
			if (SelectTool.Instance.selected != null)
			{
				cellSelectionObject = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
			}
			bool flag12 = cellSelectionObject != null && cellSelectionObject.mouseCell == cellSelectionObject.alternateSelectionObject.mouseCell;
			if (flag12)
			{
				this.currentSelectedSelectableIndex = this.recentNumberOfDisplayedSelectables - 1;
			}
			Element element = Grid.Element[num];
			hoverTextDrawer.BeginShadowBar(flag12);
			hoverTextDrawer.DrawText(element.nameUpperCase, this.Styles_Title.Standard);
			if (Grid.DiseaseCount[num] > 0 || flag)
			{
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(this.iconDash, 18);
				hoverTextDrawer.DrawText(GameUtil.GetFormattedDisease(Grid.DiseaseIdx[num], Grid.DiseaseCount[num], true), this.Styles_Values.Property.Standard);
			}
			if (!element.IsVacuum)
			{
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(this.iconDash, 18);
				hoverTextDrawer.DrawText(ElementLoader.elements[(int)Grid.ElementIdx[num]].GetMaterialCategoryTag().ProperName(), this.Styles_BodyText.Standard);
			}
			string[] array = WorldInspector.MassStringsReadOnly(num);
			hoverTextDrawer.NewLine(26);
			hoverTextDrawer.DrawIcon(this.iconDash, 18);
			for (int m = 0; m < array.Length; m++)
			{
				if (m >= 3 || !element.IsVacuum)
				{
					hoverTextDrawer.DrawText(array[m], this.Styles_BodyText.Standard);
				}
			}
			if (!element.IsVacuum)
			{
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(this.iconDash, 18);
				Element element2 = Grid.Element[num];
				string formattedTemperature = this.cachedTemperatureString;
				float num8 = Grid.Temperature[num];
				if (num8 != this.cachedTemperature)
				{
					this.cachedTemperature = num8;
					formattedTemperature = GameUtil.GetFormattedTemperature(Grid.Temperature[num], GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false);
					this.cachedTemperatureString = formattedTemperature;
				}
				string text11 = ((element2.specificHeatCapacity != 0f) ? formattedTemperature : "N/A");
				hoverTextDrawer.DrawText(text11, this.Styles_BodyText.Standard);
			}
			if (CellSelectionObject.IsExposedToSpace(num))
			{
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(this.iconDash, 18);
				hoverTextDrawer.DrawText(MISC.STATUSITEMS.SPACE.NAME, this.Styles_BodyText.Standard);
			}
			if (Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(num))
			{
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(this.iconDash, 18);
				hoverTextDrawer.DrawText(MISC.STATUSITEMS.BURIEDITEM.NAME, this.Styles_BodyText.Standard);
			}
			if (element.id == SimHashes.OxyRock)
			{
				float num9 = Grid.AccumulatedFlow[num] / 3f;
				string text12 = BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.NAME;
				text12 = text12.Replace("{FlowRate}", GameUtil.GetFormattedMass(num9, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(this.iconDash, 18);
				hoverTextDrawer.DrawText(text12, this.Styles_BodyText.Standard);
				if (num9 <= 0f)
				{
					bool flag13;
					bool flag14;
					GameUtil.IsEmissionBlocked(num, out flag13, out flag14);
					string text13 = null;
					if (flag13)
					{
						text13 = MISC.STATUSITEMS.OXYROCK.NEIGHBORSBLOCKED.NAME;
					}
					else if (flag14)
					{
						text13 = MISC.STATUSITEMS.OXYROCK.OVERPRESSURE.NAME;
					}
					if (text13 != null)
					{
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawIcon(this.iconDash, 18);
						hoverTextDrawer.DrawText(text13, this.Styles_BodyText.Standard);
					}
				}
			}
			hoverTextDrawer.EndShadowBar();
		}
		else if (!flag3)
		{
			hoverTextDrawer.BeginShadowBar(false);
			hoverTextDrawer.DrawIcon(this.iconWarning, 18);
			hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.UNKNOWN, this.Styles_BodyText.Standard);
			hoverTextDrawer.EndShadowBar();
		}
		this.recentNumberOfDisplayedSelectables = num5 + 1;
		hoverTextDrawer.EndDrawing();
	}

	private bool ShowStatusItemInCurrentOverlay(StatusItem status)
	{
		return !(OverlayScreen.Instance == null) && (status.status_overlays & (int)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode())) == (int)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode());
	}

	private bool ShouldShowSelectableInCurrentOverlay(KSelectable selectable)
	{
		bool flag = true;
		if (OverlayScreen.Instance == null)
		{
			return flag;
		}
		if (selectable == null)
		{
			return false;
		}
		if (selectable.GetComponent<KPrefabID>() == null)
		{
			return flag;
		}
		HashedString mode = OverlayScreen.Instance.GetMode();
		Func<KSelectable, bool> func;
		if (this.modeFilters.TryGetValue(mode, out func))
		{
			flag = func(selectable);
		}
		return flag;
	}

	private static bool ShouldShowOxygenOverlay(KSelectable selectable)
	{
		return selectable.GetComponent<AlgaeHabitat>() != null || selectable.GetComponent<Electrolyzer>() != null || selectable.GetComponent<AirFilter>() != null;
	}

	private static bool ShouldShowLightOverlay(KSelectable selectable)
	{
		return !(selectable.GetComponent<Light2D>() == null);
	}

	private static bool ShouldShowGasConduitOverlay(KSelectable selectable)
	{
		return (selectable.GetComponent<Conduit>() != null && selectable.GetComponent<Conduit>().type == ConduitType.Gas) || (selectable.GetComponent<Filterable>() != null && selectable.GetComponent<Filterable>().filterElementState == Filterable.ElementState.Gas) || (selectable.GetComponent<Vent>() != null && selectable.GetComponent<Vent>().conduitType == ConduitType.Gas) || (selectable.GetComponent<Pump>() != null && selectable.GetComponent<Pump>().conduitType == ConduitType.Gas) || (selectable.GetComponent<ValveBase>() != null && selectable.GetComponent<ValveBase>().conduitType == ConduitType.Gas);
	}

	private static bool ShouldShowLiquidConduitOverlay(KSelectable selectable)
	{
		return (selectable.GetComponent<Conduit>() != null && selectable.GetComponent<Conduit>().type == ConduitType.Liquid) || (selectable.GetComponent<Filterable>() != null && selectable.GetComponent<Filterable>().filterElementState == Filterable.ElementState.Liquid) || (selectable.GetComponent<Vent>() != null && selectable.GetComponent<Vent>().conduitType == ConduitType.Liquid) || (selectable.GetComponent<Pump>() != null && selectable.GetComponent<Pump>().conduitType == ConduitType.Liquid) || (selectable.GetComponent<ValveBase>() != null && selectable.GetComponent<ValveBase>().conduitType == ConduitType.Liquid);
	}

	private static bool ShouldShowPowerOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayScreen.WireIDs.Contains(prefabTag) || selectable.GetComponent<Battery>() != null || selectable.GetComponent<PowerTransformer>() != null || selectable.GetComponent<EnergyConsumer>() != null || selectable.GetComponent<EnergyGenerator>() != null;
	}

	private static bool ShouldShowTileOverlay(KSelectable selectable)
	{
		bool flag = false;
		PrimaryElement component = selectable.GetComponent<PrimaryElement>();
		if (component != null)
		{
			Element element = component.Element;
			foreach (Tag tag in Game.Instance.tileOverlayFilters)
			{
				if (element.HasTag(tag))
				{
					flag = true;
					break;
				}
			}
		}
		return flag;
	}

	private static bool ShouldShowTemperatureOverlay(KSelectable selectable)
	{
		return selectable.GetComponent<PrimaryElement>() != null;
	}

	private static bool ShouldShowLogicOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayModes.Logic.HighlightItemIDs.Contains(prefabTag) || selectable.GetComponent<LogicPorts>() != null;
	}

	private static bool ShouldShowSolidConveyorOverlay(KSelectable selectable)
	{
		Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
		return OverlayScreen.SolidConveyorIDs.Contains(prefabTag);
	}

	private static bool HideInOverlay(KSelectable selectable)
	{
		return false;
	}

	private static bool ShowOverlayIfHasComponent<T>(KSelectable selectable)
	{
		return selectable.GetComponent<T>() != null;
	}

	private static bool ShouldShowCropOverlay(KSelectable selectable)
	{
		return selectable.GetComponent<Uprootable>() != null || selectable.GetComponent<PlanterBox>() != null;
	}

	public static int maxNumberOfDisplayedSelectableWarnings = 10;

	private Dictionary<HashedString, Func<bool>> overlayFilterMap = new Dictionary<HashedString, Func<bool>>();

	public int recentNumberOfDisplayedSelectables;

	public int currentSelectedSelectableIndex = -1;

	private Sprite iconWarning;

	private Sprite iconDash;

	private Sprite iconHighlighted;

	public HoverTextConfiguration.TextStylePair Styles_LogicActive;

	public HoverTextConfiguration.TextStylePair Styles_LogicStandby;

	public static List<GameObject> highlightedObjects = new List<GameObject>();

	private static readonly List<Type> hiddenChoreConsumerTypes = new List<Type> { typeof(KSelectableHealthBar) };

	private int maskOverlay;

	private string cachedTemperatureString;

	private float cachedTemperature = float.MinValue;

	private List<KSelectable> overlayValidHoverObjects = new List<KSelectable>();

	private Dictionary<HashedString, Func<KSelectable, bool>> modeFilters;
}
