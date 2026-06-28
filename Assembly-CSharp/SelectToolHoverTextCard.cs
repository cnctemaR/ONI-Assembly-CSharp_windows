using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class SelectToolHoverTextCard : HoverTextConfiguration
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.overlayFilterMap.Add(SimViewMode.OxygenMap, delegate
		{
			int num = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(Input.mousePosition));
			return Grid.Element[num].IsGas;
		});
		this.overlayFilterMap.Add(SimViewMode.GasVentMap, delegate
		{
			int num2 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(Input.mousePosition));
			return Grid.Element[num2].IsGas;
		});
		this.overlayFilterMap.Add(SimViewMode.LiquidVentMap, delegate
		{
			int num3 = Grid.PosToCell(CameraController.Instance.baseCamera.ScreenToWorldPoint(Input.mousePosition));
			return Grid.Element[num3].IsLiquid;
		});
		this.overlayFilterMap.Add(SimViewMode.Decor, () => false);
		this.overlayFilterMap.Add(SimViewMode.Rooms, () => false);
	}

	public override void ConfigureHoverScreen()
	{
		base.ConfigureHoverScreen();
		HoverTextScreen instance = HoverTextScreen.Instance;
		this.iconWarning = instance.GetSprite("iconWarning");
		this.iconDash = instance.GetSprite("dash");
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
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
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
		SimViewMode mode = SimDebugView.Instance.GetMode();
		bool flag = SimViewMode.Disease == mode;
		bool flag2 = true;
		if (Grid.ForceField[num])
		{
			flag2 = false;
		}
		if (Grid.Visible[num] == 0 && !DebugPaintElementScreen.Instance.gameObject.activeSelf)
		{
			flag2 = false;
		}
		foreach (KeyValuePair<SimViewMode, Func<bool>> keyValuePair in this.overlayFilterMap)
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
		SimViewMode mode2 = SimDebugView.Instance.GetMode();
		if (mode2 != SimViewMode.HeatFlow)
		{
			if (mode2 != SimViewMode.Decor)
			{
				if (mode2 == SimViewMode.Rooms)
				{
					CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
					if (cavityForCell != null)
					{
						Room room = cavityForCell.room;
						RoomType roomType = null;
						if (room != null)
						{
							roomType = Db.Get().RoomTypes.GetRoomType(room);
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
						}
						else
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawText(UI.OVERLAYS.ROOMS.NOROOM.DESC, this.Styles_BodyText.Standard);
						}
						hoverTextDrawer.EndShadowBar();
					}
				}
			}
			else
			{
				List<DecorProvider> list = new List<DecorProvider>();
				GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.decorProviderLayer, list);
				float decorAtCell = GameUtil.GetDecorAtCell(num);
				hoverTextDrawer.BeginShadowBar(false);
				hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.HOVERTITLE, this.Styles_Title.Standard);
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawText(UI.OVERLAYS.DECOR.TOTAL + GameUtil.GetFormattedDecor(decorAtCell), this.Styles_BodyText.Standard);
				if (!Grid.Solid[num] && Grid.Visible[num] > 0)
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
							bool flag3 = false;
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
										flag3 = true;
										break;
									}
								}
								if (!flag3)
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
										flag3 = true;
										break;
									}
								}
								if (!flag3)
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
		}
		else if (!Grid.Solid[num] && Grid.Visible[num] > 0)
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
			text = text + " (" + GameUtil.GetFormattedWattage(thermalComfort, GameUtil.WattageFormatterUnit.Automatic) + ")";
			hoverTextDrawer.BeginShadowBar(false);
			hoverTextDrawer.DrawText(UI.OVERLAYS.HEATFLOW.HOVERTITLE, this.Styles_Title.Standard);
			hoverTextDrawer.NewLine(26);
			hoverTextDrawer.DrawText(text, this.Styles_BodyText.Standard);
			hoverTextDrawer.EndShadowBar();
		}
		int num3 = 0;
		ChoreConsumer choreConsumer = null;
		if (SelectTool.Instance.selected != null)
		{
			choreConsumer = SelectTool.Instance.selected.GetComponent<ChoreConsumer>();
		}
		int mask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
		for (int k = 0; k < this.overlayValidHoverObjects.Count; k++)
		{
			if (this.overlayValidHoverObjects[k] != null && this.overlayValidHoverObjects[k].GetComponent<CellSelectionObject>() == null)
			{
				KSelectable kselectable2 = this.overlayValidHoverObjects[k];
				if (!(OverlayScreen.Instance != null) || OverlayScreen.Instance.mode == SimViewMode.None || (kselectable2.gameObject.layer & mask) == 0)
				{
					if (Grid.Visible[num] != 0 || DebugPaintElementScreen.Instance.gameObject.activeSelf)
					{
						bool flag4 = SelectTool.Instance.selected == this.overlayValidHoverObjects[k];
						if (flag4)
						{
							this.currentSelectedSelectableIndex = k;
						}
						num3++;
						hoverTextDrawer.BeginShadowBar(flag4);
						hoverTextDrawer.DrawText(GameUtil.GetUnitFormattedName(this.overlayValidHoverObjects[k].gameObject, true), this.Styles_Title.Standard);
						bool flag5 = false;
						string text6 = UI.OVERLAYS.DISEASE.NO_DISEASE;
						PrimaryElement component2 = kselectable2.GetComponent<PrimaryElement>();
						if (flag)
						{
							if (component2 != null && component2.DiseaseIdx != 255)
							{
								text6 = GameUtil.GetFormattedDisease(component2.DiseaseIdx, component2.DiseaseCount, true);
							}
							flag5 = true;
							Storage component3 = kselectable2.GetComponent<Storage>();
							if (component3 != null && component3.showInUI)
							{
								List<GameObject> items = component3.items;
								for (int l = 0; l < items.Count; l++)
								{
									GameObject gameObject = items[l];
									if (gameObject != null)
									{
										PrimaryElement component4 = gameObject.GetComponent<PrimaryElement>();
										if (component4.DiseaseIdx != 255)
										{
											text6 += string.Format(UI.OVERLAYS.DISEASE.CONTAINER_FORMAT, gameObject.GetComponent<KSelectable>().GetProperName(), GameUtil.GetFormattedDisease(component4.DiseaseIdx, component4.DiseaseCount, true));
										}
									}
								}
							}
						}
						if (flag5)
						{
							StateMachineController component5 = kselectable2.GetComponent<StateMachineController>();
							if (component5 != null)
							{
								ImmuneSystemMonitor.Instance smi = component5.GetSMI<ImmuneSystemMonitor.Instance>();
								if (smi != null)
								{
									AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(kselectable2);
									float value = amountInstance.value;
									bool flag6 = smi.sm.isLosingImmunity.Get(smi);
									Color32 badColorBG = NotificationScreen.Instance.BadColorBG;
									badColorBG.a = byte.MaxValue;
									Color32 color = ((!flag6) ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : badColorBG);
									string text7 = string.Format(UI.OVERLAYS.DISEASE.IMMUNITY, GameUtil.GetFormattedPercent(value, GameUtil.TimeSlice.None));
									hoverTextDrawer.NewLine(26);
									hoverTextDrawer.DrawIcon(this.iconDash, 18);
									hoverTextDrawer.DrawText(GameUtil.ColourizeString(color, text7), this.Styles_Values.Property.Standard);
								}
							}
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawIcon(this.iconDash, 18);
							hoverTextDrawer.DrawText(text6, this.Styles_Values.Property.Standard);
						}
						int num4 = 0;
						foreach (StatusItemGroup.Entry entry in this.overlayValidHoverObjects[k].GetStatusItemGroup())
						{
							if (!this.ShowStatusItemInCurrentOverlay(entry.item))
							{
								break;
							}
							if (num4 >= SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
							{
								break;
							}
							if (entry.category != null && entry.category.Id == "Main" && num4 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
							{
								TextStyleSetting textStyleSetting = ((!this.IsStatusItemWarning(entry)) ? this.Styles_BodyText.Standard : this.HoverTextStyleSettings[1]);
								Sprite sprite = ((entry.item.sprite == null) ? this.iconWarning : entry.item.sprite.sprite);
								Color color2 = ((!this.IsStatusItemWarning(entry)) ? this.Styles_BodyText.Standard.textColor : this.HoverTextStyleSettings[1].textColor);
								hoverTextDrawer.NewLine(26);
								hoverTextDrawer.DrawIcon(sprite, color2, 18, 2);
								hoverTextDrawer.DrawText(entry.GetName(), textStyleSetting);
								num4++;
							}
						}
						foreach (StatusItemGroup.Entry entry2 in this.overlayValidHoverObjects[k].GetStatusItemGroup())
						{
							if (!this.ShowStatusItemInCurrentOverlay(entry2.item))
							{
								break;
							}
							if (num4 >= SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
							{
								break;
							}
							if ((entry2.category == null || entry2.category.Id != "Main") && num4 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
							{
								TextStyleSetting textStyleSetting2 = ((!this.IsStatusItemWarning(entry2)) ? this.Styles_BodyText.Standard : this.HoverTextStyleSettings[1]);
								Sprite sprite2 = ((entry2.item.sprite == null) ? this.iconWarning : entry2.item.sprite.sprite);
								Color color3 = ((!this.IsStatusItemWarning(entry2)) ? this.Styles_BodyText.Standard.textColor : this.HoverTextStyleSettings[1].textColor);
								hoverTextDrawer.NewLine(26);
								hoverTextDrawer.DrawIcon(sprite2, color3, 18, 2);
								hoverTextDrawer.DrawText(entry2.GetName(), textStyleSetting2);
								num4++;
							}
						}
						float num5 = 0f;
						bool flag7 = true;
						bool flag8 = SimViewMode.TemperatureMap == SimDebugView.Instance.GetMode();
						if (kselectable2.GetComponent<Constructable>())
						{
							flag7 = false;
						}
						else if (flag8 && component2)
						{
							num5 = component2.Temperature;
						}
						else if (kselectable2.GetComponent<Building>() && component2)
						{
							num5 = component2.Temperature;
						}
						else if (kselectable2.GetComponent<CellSelectionObject>() != null)
						{
							num5 = kselectable2.GetComponent<CellSelectionObject>().temperature;
						}
						else
						{
							flag7 = false;
						}
						if (mode != SimViewMode.None && mode != SimViewMode.TemperatureMap)
						{
							flag7 = false;
						}
						if (flag7)
						{
							hoverTextDrawer.NewLine(26);
							hoverTextDrawer.DrawIcon(this.iconDash, 18);
							hoverTextDrawer.DrawText(GameUtil.GetFormattedTemperature(num5, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), this.Styles_BodyText.Standard);
						}
						BuildingComplete component6 = kselectable2.GetComponent<BuildingComplete>();
						if (component6 != null && component6.Def.IsFoundation)
						{
							flag2 = false;
						}
						if (choreConsumer != null)
						{
							bool flag9 = false;
							foreach (Type type in SelectToolHoverTextCard.hiddenChoreConsumerTypes)
							{
								if (choreConsumer.gameObject.GetComponent(type) != null)
								{
									flag9 = true;
									break;
								}
							}
							if (!flag9)
							{
								choreConsumer.ShowHoverTextOnHoveredItem(kselectable2, hoverTextDrawer, this);
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
			bool flag10 = cellSelectionObject != null && cellSelectionObject.mouseCell == cellSelectionObject.alternateSelectionObject.mouseCell;
			if (flag10)
			{
				this.currentSelectedSelectableIndex = this.recentNumberOfDisplayedSelectables - 1;
			}
			Element element = Grid.Element[num];
			hoverTextDrawer.BeginShadowBar(flag10);
			hoverTextDrawer.DrawText(element.name.ToUpper(), this.Styles_Title.Standard);
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
			string[] array = WorldInspector.MassStrings(num);
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
				string text8 = ((element2.specificHeatCapacity != 0f) ? GameUtil.GetFormattedTemperature(Grid.Temperature[num], GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true) : "N/A");
				hoverTextDrawer.DrawText(text8, this.Styles_BodyText.Standard);
			}
			if (Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(num))
			{
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(this.iconDash, 18);
				hoverTextDrawer.DrawText(MISC.STATUSITEMS.BURIEDITEM.NAME, this.Styles_BodyText.Standard);
			}
			if (element.id == SimHashes.OxyRock)
			{
				float num6 = Grid.AccumulatedFlow[num] / 3f;
				string text9 = BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.NAME;
				text9 = text9.Replace("{FlowRate}", GameUtil.GetFormattedMass(num6, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
				hoverTextDrawer.NewLine(26);
				hoverTextDrawer.DrawIcon(this.iconDash, 18);
				hoverTextDrawer.DrawText(text9, this.Styles_BodyText.Standard);
				if (num6 <= 0f)
				{
					bool flag11;
					bool flag12;
					GameUtil.IsEmissionBlocked(num, out flag11, out flag12);
					string text10 = null;
					if (flag11)
					{
						text10 = MISC.STATUSITEMS.OXYROCK.NEIGHBORSBLOCKED.NAME;
					}
					else if (flag12)
					{
						text10 = MISC.STATUSITEMS.OXYROCK.OVERPRESSURE.NAME;
					}
					if (text10 != null)
					{
						hoverTextDrawer.NewLine(26);
						hoverTextDrawer.DrawIcon(this.iconDash, 18);
						hoverTextDrawer.DrawText(text10, this.Styles_BodyText.Standard);
					}
				}
			}
			hoverTextDrawer.EndShadowBar();
		}
		else if (Grid.Visible[num] <= 0)
		{
			hoverTextDrawer.BeginShadowBar(false);
			hoverTextDrawer.DrawIcon(this.iconWarning, 18);
			hoverTextDrawer.DrawText(UI.TOOLS.GENERIC.UNKNOWN.ToString().ToUpper(), this.Styles_BodyText.Standard);
			hoverTextDrawer.EndShadowBar();
		}
		this.recentNumberOfDisplayedSelectables = num3 + 1;
		hoverTextDrawer.EndDrawing();
	}

	private bool ShouldShowSelectableInCurrentOverlay(KSelectable selectable)
	{
		bool flag = true;
		if (OverlayScreen.Instance == null)
		{
			return flag;
		}
		SimViewMode mode = OverlayScreen.Instance.GetMode();
		if (mode != SimViewMode.Decor)
		{
			if (mode != SimViewMode.OxygenMap)
			{
				if (mode != SimViewMode.Crop)
				{
					if (mode != SimViewMode.LiquidVentMap)
					{
						if (mode != SimViewMode.PowerMap)
						{
							if (mode != SimViewMode.GasVentMap)
							{
								if (mode != SimViewMode.HeatFlow)
								{
									if (mode == SimViewMode.SolidConveyorMap)
									{
										Tag prefabTag = selectable.GetComponent<KPrefabID>().PrefabTag;
										return OverlayScreen.SolidConveyorIDs.Contains(prefabTag);
									}
									if (mode != SimViewMode.ThermalConductivity)
									{
										if (mode == SimViewMode.TemperatureMap)
										{
											return flag;
										}
										if (mode == SimViewMode.Disease)
										{
											return selectable.GetComponent<PrimaryElement>() != null;
										}
										if (mode != SimViewMode.Light)
										{
											return flag;
										}
										return !(selectable.GetComponent<Light2D>() == null);
									}
								}
								flag = false;
							}
							else
							{
								flag = (selectable.GetComponent<Conduit>() != null && selectable.GetComponent<Conduit>().type == ConduitType.Gas) || selectable.GetComponent<GasFilterable>() != null || (selectable.GetComponent<Vent>() != null && selectable.GetComponent<Vent>().conduitType == ConduitType.Gas) || (selectable.GetComponent<Pump>() != null && selectable.GetComponent<Pump>().conduitType == ConduitType.Gas) || (selectable.GetComponent<ValveBase>() != null && selectable.GetComponent<ValveBase>().conduitType == ConduitType.Gas);
							}
						}
						else
						{
							Tag prefabTag2 = selectable.GetComponent<KPrefabID>().PrefabTag;
							flag = OverlayScreen.WireIDs.Contains(prefabTag2) || selectable.GetComponent<Battery>() != null || selectable.GetComponent<PowerTransformer>() != null || selectable.GetComponent<EnergyConsumer>() != null || selectable.GetComponent<EnergyGenerator>() != null;
						}
					}
					else
					{
						flag = (selectable.GetComponent<Conduit>() != null && selectable.GetComponent<Conduit>().type == ConduitType.Liquid) || selectable.GetComponent<LiquidFilterable>() != null || (selectable.GetComponent<Vent>() != null && selectable.GetComponent<Vent>().conduitType == ConduitType.Liquid) || (selectable.GetComponent<Pump>() != null && selectable.GetComponent<Pump>().conduitType == ConduitType.Liquid) || (selectable.GetComponent<ValveBase>() != null && selectable.GetComponent<ValveBase>().conduitType == ConduitType.Liquid);
					}
				}
				else
				{
					flag = selectable.GetComponent<Uprootable>() != null || selectable.GetComponent<PlanterBox>() != null;
				}
			}
			else
			{
				flag = selectable.GetComponent<AlgaeHabitat>() != null || selectable.GetComponent<Electrolyzer>() != null || selectable.GetComponent<AirFilter>() != null;
			}
		}
		else
		{
			flag = selectable.GetComponent<DecorProvider>() != null;
		}
		return flag;
	}

	private bool ShowStatusItemInCurrentOverlay(StatusItem status)
	{
		return !(OverlayScreen.Instance == null) && (status.status_overlays & (int)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode())) == (int)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode());
	}

	public static int maxNumberOfDisplayedSelectableWarnings = 10;

	private Dictionary<SimViewMode, Func<bool>> overlayFilterMap = new Dictionary<SimViewMode, Func<bool>>();

	public int recentNumberOfDisplayedSelectables;

	public int currentSelectedSelectableIndex = -1;

	private Sprite iconWarning;

	private Sprite iconDash;

	public static List<GameObject> highlightedObjects = new List<GameObject>();

	private static readonly List<Type> hiddenChoreConsumerTypes = new List<Type> { typeof(KSelectableHealthBar) };

	private List<KSelectable> overlayValidHoverObjects = new List<KSelectable>();
}
