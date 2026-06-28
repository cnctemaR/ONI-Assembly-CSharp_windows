using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

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
		HoverTextScreen instance = HoverTextScreen.Instance;
		if (instance.LoadPreConfiguredToolFields(this))
		{
			this.isConfigured = true;
		}
		else
		{
			instance.currentConfiguration = this;
			instance.ToggleIncubating(true);
			instance.ClearLabels();
			instance.StartShadowBar(0f, 0f, false);
			this.hoverScreenElements.UnknownAreaLine = instance.NewLine("UnknownArea_SelectTool", 24);
			instance.AddIcon(instance.GetSprite("iconWarning"), 18f);
			instance.AddIndent(4f, 18f);
			instance.AddText(UI.TOOLS.GENERIC.UNKNOWN, null, true);
			instance.EndShadowBar();
			base.SetLineActive(this.hoverScreenElements.UnknownAreaLine, false);
			instance.StartShadowBar(0f, 0f, false);
			instance.NewLine("OverlayInfoHeader", 24);
			this.hoverScreenElements.OverlayInfoHeader = instance.AddText("", this.Styles_Title.Standard, true);
			instance.NewLine("OverlayInfo", 24);
			this.hoverScreenElements.OverlayInfo = instance.AddText("", this.Styles_BodyText.Standard, true);
			instance.EndShadowBar();
			this.hoverScreenElements.OverlayInfoDivider = instance.NewLine("Divider", this.dividerHeight);
			this.hoverScreenElements.selectableHoverFields = new SelectToolHoverTextCard.SelectableHoverFields[this.maxNumberOfDisplaySelectables];
			for (int i = 0; i < this.maxNumberOfDisplaySelectables; i++)
			{
				if (i != 0)
				{
					this.hoverScreenElements.selectableHoverFields[i].selectableDivider = instance.NewLine("Divider", this.dividerHeight);
				}
				this.hoverScreenElements.selectableHoverFields[i].shadowBar = instance.StartShadowBar(0f, 0f, i == this.currentSelectedSelectableIndex);
				instance.NewLine("SelectableName", 24);
				this.hoverScreenElements.selectableHoverFields[i].selectableName = instance.AddText("", this.Styles_Title.Standard, true);
				instance.NewLine("SelectableImmunity", 24);
				instance.AddIcon(instance.GetSprite("dash"), 18f, this.iconColor_basic);
				instance.AddIndent(4f, 18f);
				this.hoverScreenElements.selectableHoverFields[i].selectableImmunity = instance.AddText("", this.Styles_Values.Property.Standard, true);
				instance.NewLine("SelectableDisease", 24);
				instance.AddIcon(instance.GetSprite("dash"), 18f, this.iconColor_basic);
				instance.AddIndent(4f, 18f);
				this.hoverScreenElements.selectableHoverFields[i].selectableDisease = instance.AddText("", this.Styles_Values.Property.Standard, true);
				this.hoverScreenElements.selectableHoverFields[i].selectableWarnings = new SelectToolHoverTextCard.StatusIconPair[SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings];
				for (int j = 0; j < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings; j++)
				{
					instance.NewLine(string.Concat(new object[] { "StatusMessage_", i, "_", j }), 24);
					this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[j].statusIcon = instance.AddIcon(instance.GetSprite("iconWarning"), 18f, this.iconColor_basic);
					instance.AddIndent(4f, 18f);
					this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[j].statusText = instance.AddText("", this.Styles_BodyText.Standard, false);
				}
				instance.NewLine("SelectableTemperature", 24);
				instance.AddIcon(instance.GetSprite("dash"), 18f, this.iconColor_basic);
				instance.AddIndent(4f, 18f);
				this.hoverScreenElements.selectableHoverFields[i].selectableTemperature = instance.AddText("", this.Styles_Values.Property.Standard, true);
				instance.EndShadowBar();
			}
			this.hoverScreenElements.selectableHoverFields[0].selectableDivider = instance.NewLine("Divider", this.dividerHeight);
			this.hoverScreenElements.cellElement.ElementShadowBar = instance.StartShadowBar(0f, 0f, false);
			instance.NewLine("Line_ElementName", 24);
			this.hoverScreenElements.cellElement.ElementName = instance.AddText("", this.Styles_Title.Standard, true);
			instance.NewLine("Line_ElementDisease", 24);
			instance.AddIcon(instance.GetSprite("dash"), 18f, this.iconColor_basic);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.cellElement.ElementDisease = instance.AddText("", this.Styles_Values.Property.Standard, true);
			instance.NewLine("Line_Category", 24);
			instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.cellElement.ElementCategory = instance.AddText("", this.HoverTextStyleSettings[0], false);
			instance.NewLine("Mass", 24);
			instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.cellElement.ElementMass = new LocText[4];
			this.hoverScreenElements.cellElement.ElementMass[0] = instance.AddText("", this.Styles_Values.Property.Standard, true);
			this.hoverScreenElements.cellElement.ElementMass[1] = instance.AddText("", this.Styles_Values.Property_Decimal.Standard, true);
			this.hoverScreenElements.cellElement.ElementMass[2] = instance.AddText("", this.Styles_Values.Property_Unit.Standard, false);
			this.hoverScreenElements.cellElement.ElementMass[3] = instance.AddText("", this.Styles_Values.Property_Unit.Standard, true);
			this.hoverScreenElements.cellElement.StandAloneBreathableLine = instance.NewLine("Standalone Breathable", 24);
			instance.AddIcon(instance.GetSprite("dash"), 18f);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.cellElement.StandAloneBreathableDescription = instance.AddText("", this.Styles_BodyText.Standard, false);
			instance.NewLine("Temperature", 24);
			instance.AddIcon(instance.GetSprite("dash"), 18f, this.iconColor_basic);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.cellElement.ElementTemperature = instance.AddText("", this.Styles_Values.Property.Standard, true);
			instance.NewLine("Buried Item", 24);
			instance.AddIcon(instance.GetSprite("dash"), 18f);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.cellElement.BuriedItem = instance.AddText("", this.Styles_BodyText.Standard, false);
			instance.NewLine("Average Flow Rate", 24);
			instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.AverageFlowRateLine = instance.AddText("", this.Styles_BodyText.Standard, true);
			instance.NewLine("Flow Rate Status", 24);
			instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.FlowRateStatusLine = instance.AddText("", this.Styles_BodyText.Standard, true);
			instance.EndShadowBar();
			this.isConfigured = true;
		}
	}

	public override void SetNotConfigured()
	{
		base.SetNotConfigured();
	}

	private bool IsStatusItemWarning(StatusItemGroup.Entry item)
	{
		return item.item.notificationType == NotificationType.Bad || item.item.notificationType == NotificationType.BadMinor || item.item.notificationType == NotificationType.DuplicantThreatening;
	}

	public override void UpdateHoverElements(List<KSelectable> hoverObjects)
	{
		if (!this.isConfigured)
		{
			this.ConfigureHoverScreen();
		}
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
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (Grid.IsValidCell(num))
		{
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
			if (!(OverlayScreen.Instance == null))
			{
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
				int num2 = 0;
				int mask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
				int i = 0;
				while (i < this.hoverScreenElements.selectableHoverFields.Length)
				{
					if (this.overlayValidHoverObjects.Count - 1 >= i && this.overlayValidHoverObjects[i] != null && this.overlayValidHoverObjects[i].GetComponent<CellSelectionObject>() == null)
					{
						KSelectable kselectable2 = this.overlayValidHoverObjects[i];
						if (!(OverlayScreen.Instance != null) || OverlayScreen.Instance.mode == SimViewMode.None || (kselectable2.gameObject.layer & mask) == 0)
						{
							if (Grid.Visible[num] != 0 || DebugPaintElementScreen.Instance.gameObject.activeSelf)
							{
								bool flag3 = SelectTool.Instance.selected == this.overlayValidHoverObjects[i];
								if (flag3)
								{
									this.currentSelectedSelectableIndex = i;
								}
								num2++;
								this.hoverScreenElements.selectableHoverFields[i].shadowBar.toggleSelectionBorder(flag3);
								base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableName.transform.parent.gameObject, true);
								base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableDivider, true);
								if (this.hoverScreenElements.selectableHoverFields[i].selectableName != null)
								{
									this.hoverScreenElements.selectableHoverFields[i].selectableName.text = GameUtil.GetUnitFormattedName(this.overlayValidHoverObjects[i].gameObject, true);
									this.hoverScreenElements.selectableHoverFields[i].selectableName.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_Title.Standard);
								}
								int num3 = 0;
								foreach (StatusItemGroup.Entry entry in this.overlayValidHoverObjects[i].GetStatusItemGroup())
								{
									if (!this.ShowStatusItemInCurrentOverlay(entry.item))
									{
										break;
									}
									if (num3 >= SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
									{
										break;
									}
									if (entry.category != null && entry.category.Id == "Main")
									{
										if (num3 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
										{
											this.ConfigureSelectableStatusItem(entry, i, num3, flag3);
										}
										else
										{
											base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[num3].statusText.transform.parent.gameObject, false);
										}
										num3++;
									}
								}
								foreach (StatusItemGroup.Entry entry2 in this.overlayValidHoverObjects[i].GetStatusItemGroup())
								{
									if (!this.ShowStatusItemInCurrentOverlay(entry2.item))
									{
										break;
									}
									if (num3 >= SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
									{
										break;
									}
									if (entry2.category == null || entry2.category.Id != "Main")
									{
										if (num3 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
										{
											this.ConfigureSelectableStatusItem(entry2, i, num3, flag3);
										}
										else
										{
											base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[num3].statusText.transform.parent.gameObject, false);
										}
										num3++;
									}
								}
								for (int j = num3; j < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings; j++)
								{
									base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[j].statusText.transform.parent.gameObject, false);
								}
								float num4 = 0f;
								bool flag4 = true;
								bool flag5 = SimViewMode.TemperatureMap == SimDebugView.Instance.GetMode();
								PrimaryElement component = kselectable2.GetComponent<PrimaryElement>();
								if (kselectable2.GetComponent<Constructable>())
								{
									flag4 = false;
								}
								else if (flag5 && component)
								{
									num4 = component.Temperature;
								}
								else if (kselectable2.GetComponent<Building>() && component)
								{
									num4 = component.Temperature;
								}
								else if (kselectable2.GetComponent<CellSelectionObject>() != null)
								{
									num4 = kselectable2.GetComponent<CellSelectionObject>().temperature;
								}
								else
								{
									flag4 = false;
								}
								if (mode != SimViewMode.None && mode != SimViewMode.TemperatureMap)
								{
									flag4 = false;
								}
								base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableTemperature.transform.parent.gameObject, flag4);
								if (flag4)
								{
									this.hoverScreenElements.selectableHoverFields[i].selectableTemperature.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
									this.hoverScreenElements.selectableHoverFields[i].selectableTemperature.text = GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
								}
								bool flag6 = false;
								bool flag7 = false;
								string text = UI.OVERLAYS.DISEASE.NO_DISEASE;
								if (flag)
								{
									if (component != null && component.DiseaseIdx != 255)
									{
										text = GameUtil.GetFormattedDisease(component.DiseaseIdx, component.DiseaseCount, true);
									}
									flag6 = this.UpdateImmunityDisplay(kselectable2, this.hoverScreenElements.selectableHoverFields[i].selectableImmunity);
									flag7 = true;
									Storage component2 = kselectable2.GetComponent<Storage>();
									if (component2 != null && component2.showInUI)
									{
										List<GameObject> items = component2.items;
										for (int k = 0; k < items.Count; k++)
										{
											GameObject gameObject = items[k];
											if (gameObject != null)
											{
												PrimaryElement component3 = gameObject.GetComponent<PrimaryElement>();
												if (component3.DiseaseIdx != 255)
												{
													text += string.Format(UI.OVERLAYS.DISEASE.CONTAINER_FORMAT, gameObject.GetComponent<KSelectable>().GetProperName(), GameUtil.GetFormattedDisease(component3.DiseaseIdx, component3.DiseaseCount, true));
												}
											}
										}
									}
								}
								this.hoverScreenElements.selectableHoverFields[i].selectableDisease.text = text;
								base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableDisease.transform.parent.gameObject, flag7);
								base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableImmunity.transform.parent.gameObject, flag6);
								BuildingComplete component4 = kselectable2.GetComponent<BuildingComplete>();
								if (component4 != null && component4.Def.IsFoundation)
								{
									flag2 = false;
								}
							}
						}
					}
					else
					{
						base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableName.transform.parent.gameObject, false);
						for (int l = 0; l < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings; l++)
						{
							base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[l].statusText.transform.parent.gameObject, false);
						}
						base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableDivider, false);
						base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableTemperature.transform.parent.gameObject, false);
						base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableDisease.transform.parent.gameObject, false);
						base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableImmunity.transform.parent.gameObject, false);
					}
					IL_0A3F:
					i++;
					continue;
					goto IL_0A3F;
				}
				base.SetLineActive(this.hoverScreenElements.UnknownAreaLine, Grid.Visible[num] == 0 && !flag2 && !DebugPaintElementScreen.Instance.gameObject.activeSelf);
				base.SetLineActive(this.hoverScreenElements.cellElement.ElementMass[0].transform.parent.gameObject, !Grid.Element[num].IsVacuum && flag2);
				base.SetLineActive(this.hoverScreenElements.cellElement.ElementName.transform.parent.gameObject, flag2);
				base.SetLineActive(this.hoverScreenElements.cellElement.ElementDisease.transform.parent.gameObject, (Grid.Disease[num].elementCount > 0 || flag) && flag2);
				base.SetLineActive(this.hoverScreenElements.cellElement.ElementCategory.transform.parent.gameObject, flag2);
				base.SetLineActive(this.hoverScreenElements.cellElement.ElementTemperature.transform.parent.gameObject, !Grid.Element[num].IsVacuum && flag2);
				base.SetLineActive(this.hoverScreenElements.cellElement.BuriedItem.transform.parent.gameObject, flag2);
				base.SetLineActive(this.hoverScreenElements.FlowRateStatusLine.transform.parent.gameObject, false);
				base.SetLineActive(this.hoverScreenElements.AverageFlowRateLine.transform.parent.gameObject, false);
				bool flag8 = flag2 && num2 > 0;
				base.SetLineActive(this.hoverScreenElements.selectableHoverFields[0].selectableDivider, flag8);
				if (flag2)
				{
					CellSelectionObject cellSelectionObject = null;
					if (SelectTool.Instance.selected != null)
					{
						cellSelectionObject = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
					}
					bool flag9 = cellSelectionObject != null && cellSelectionObject.mouseCell == cellSelectionObject.alternateSelectionObject.mouseCell;
					if (flag9)
					{
						this.currentSelectedSelectableIndex = this.recentNumberOfDisplayedSelectables - 1;
					}
					this.hoverScreenElements.cellElement.ElementShadowBar.toggleSelectionBorder(flag9);
					this.hoverScreenElements.cellElement.ElementName.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_Title.Standard);
					this.hoverScreenElements.cellElement.ElementTemperature.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
					for (int m = 0; m < this.hoverScreenElements.cellElement.ElementMass.Length; m++)
					{
						this.hoverScreenElements.cellElement.ElementMass[m].GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
					}
					this.hoverScreenElements.cellElement.ElementCategory.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
					this.hoverScreenElements.cellElement.ElementCategory.text = ElementLoader.elements[(int)Grid.Cell[num].elementIdx].GetMaterialCategoryTag().ProperName();
					base.SetLineActive(this.hoverScreenElements.cellElement.ElementCategory.transform.parent.gameObject, !ElementLoader.elements[(int)Grid.Cell[num].elementIdx].IsVacuum);
					this.hoverScreenElements.cellElement.ElementName.text = ElementLoader.elements[(int)Grid.Cell[num].elementIdx].name.ToUpper();
					this.hoverScreenElements.cellElement.ElementDisease.text = GameUtil.GetFormattedDisease(Grid.Disease[num].diseaseIdx, Grid.Disease[num].elementCount, true);
					string[] array = WorldInspector.MassStrings(num);
					if (this.hoverScreenElements.cellElement.ElementMass[0].text != array[0])
					{
						this.hoverScreenElements.cellElement.ElementMass[0].text = array[0];
					}
					if (this.hoverScreenElements.cellElement.ElementMass[1].text != array[1])
					{
						this.hoverScreenElements.cellElement.ElementMass[1].text = array[1];
					}
					if (this.hoverScreenElements.cellElement.ElementMass[2].text != array[2])
					{
						this.hoverScreenElements.cellElement.ElementMass[2].text = array[2];
					}
					if (this.hoverScreenElements.cellElement.ElementMass[3].text != array[3])
					{
						this.hoverScreenElements.cellElement.ElementMass[3].text = array[3];
					}
					base.SetLineActive(this.hoverScreenElements.cellElement.StandAloneBreathableLine, !this.hoverScreenElements.cellElement.ElementMass[0].transform.parent.gameObject.activeSelf);
					this.hoverScreenElements.cellElement.StandAloneBreathableDescription.text = array[3];
					this.hoverScreenElements.cellElement.StandAloneBreathableDescription.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
					this.hoverScreenElements.cellElement.BuriedItem.text = Strings.Get("STRINGS.MISC.STATUSITEMS.BURIEDITEM.NAME");
					base.SetLineActive(this.hoverScreenElements.cellElement.BuriedItem.transform.parent.gameObject, Game.Instance.GetComponent<EntombedItemVisualizer>().IsEntombedItem(num));
					Element element = Grid.Element[num];
					string text2 = ((element.specificHeatCapacity != 0f) ? GameUtil.GetFormattedTemperature(Grid.Cell[num].temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true) : "N/A");
					this.hoverScreenElements.cellElement.ElementTemperature.text = text2;
					bool flag10 = false;
					bool flag11 = false;
					if (element.id == SimHashes.OxyRock)
					{
						flag11 = true;
						float num5 = Grid.AccumulatedFlow[num] / 3f;
						string text3 = Strings.Get("STRINGS.BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.NAME");
						text3 = text3.Replace("{FlowRate}", GameUtil.GetFormattedMass(num5, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
						this.hoverScreenElements.AverageFlowRateLine.text = text3;
						if (num5 <= 0f)
						{
							bool flag12;
							bool flag13;
							GameUtil.IsEmissionBlocked(num, out flag12, out flag13);
							string text4 = null;
							if (flag12)
							{
								text4 = MISC.STATUSITEMS.OXYROCK.NEIGHBORSBLOCKED.NAME;
							}
							else if (flag13)
							{
								text4 = MISC.STATUSITEMS.OXYROCK.OVERPRESSURE.NAME;
							}
							flag10 = text4 != null;
							this.hoverScreenElements.FlowRateStatusLine.text = text4;
						}
					}
					base.SetLineActive(this.hoverScreenElements.FlowRateStatusLine.transform.parent.gameObject, flag11 && flag10);
					base.SetLineActive(this.hoverScreenElements.AverageFlowRateLine.transform.parent.gameObject, flag11);
				}
				if (!flag2)
				{
					base.SetLineActive(this.hoverScreenElements.cellElement.StandAloneBreathableLine, false);
				}
				string text5 = "";
				string text6 = "";
				string text7 = "";
				SimViewMode mode2 = SimDebugView.Instance.GetMode();
				if (mode2 != SimViewMode.Rooms)
				{
					if (mode2 != SimViewMode.HeatFlow)
					{
						if (mode2 != SimViewMode.Disease)
						{
							if (mode2 == SimViewMode.Decor)
							{
								List<DecorProvider> list = new List<DecorProvider>();
								GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.decorProviderLayer, list);
								float decorAtCell = GameUtil.GetDecorAtCell(num);
								text5 = UI.OVERLAYS.DECOR.TOTAL + GameUtil.GetFormattedDecor(decorAtCell);
								if (!Grid.Solid[num])
								{
									List<EffectorEntry> list2 = new List<EffectorEntry>();
									List<EffectorEntry> list3 = new List<EffectorEntry>();
									foreach (DecorProvider decorProvider in list)
									{
										float decorForCell = decorProvider.GetDecorForCell(num);
										if (decorForCell != 0f)
										{
											string name = decorProvider.GetName();
											KMonoBehaviour component5 = decorProvider.GetComponent<KMonoBehaviour>();
											if (component5 != null && component5.gameObject != null)
											{
												SelectToolHoverTextCard.highlightedObjects.Add(component5.gameObject);
											}
											bool flag14 = false;
											if (decorForCell > 0f)
											{
												for (int n = 0; n < list2.Count; n++)
												{
													if (list2[n].name == name)
													{
														EffectorEntry effectorEntry = list2[n];
														effectorEntry.count++;
														effectorEntry.value += decorForCell;
														list2[n] = effectorEntry;
														flag14 = true;
														break;
													}
												}
												if (!flag14)
												{
													list2.Add(new EffectorEntry(name, decorForCell));
												}
											}
											else
											{
												for (int num6 = 0; num6 < list3.Count; num6++)
												{
													if (list3[num6].name == name)
													{
														EffectorEntry effectorEntry2 = list3[num6];
														effectorEntry2.count++;
														effectorEntry2.value += decorForCell;
														list3[num6] = effectorEntry2;
														flag14 = true;
														break;
													}
												}
												if (!flag14)
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
										text5 = text5 + "\n\n" + UI.OVERLAYS.DECOR.HEADER_POSITIVE;
									}
									foreach (EffectorEntry effectorEntry3 in list2)
									{
										text5 = text5 + "\n• " + effectorEntry3.ToString();
									}
									list3.Sort((EffectorEntry x, EffectorEntry y) => Mathf.Abs(y.value).CompareTo(Mathf.Abs(x.value)));
									if (list3.Count > 0)
									{
										text5 = text5 + "\n\n" + UI.OVERLAYS.DECOR.HEADER_NEGATIVE;
									}
									foreach (EffectorEntry effectorEntry4 in list3)
									{
										text5 = text5 + "\n• " + effectorEntry4.ToString();
									}
								}
								text5 += "\n";
								text6 = UI.OVERLAYS.DECOR.HOVERTITLE;
								text7 = text5;
							}
						}
						else
						{
							text7 = "";
						}
					}
					else if (!Grid.Solid[num])
					{
						float thermalComfort = GameUtil.GetThermalComfort(num, 0f);
						float thermalComfort2 = GameUtil.GetThermalComfort(num, -0.083680004f);
						float num7 = 0f;
						if (thermalComfort2 * 0.001f > -0.27893335f - num7 && thermalComfort2 * 0.001f < 0.27893335f + num7)
						{
							text5 = UI.OVERLAYS.HEATFLOW.NEUTRAL;
						}
						else if (thermalComfort2 <= ExternalTemperatureMonitor.GetExternalColdThreshold(null))
						{
							text5 = UI.OVERLAYS.HEATFLOW.COOLING;
						}
						else if (thermalComfort2 >= ExternalTemperatureMonitor.GetExternalWarmThreshold(null))
						{
							text5 = UI.OVERLAYS.HEATFLOW.HEATING;
						}
						text5 = text5 + " (" + GameUtil.GetFormattedWattage(thermalComfort, GameUtil.WattageFormatterUnit.Automatic) + ")";
						text7 = text5;
						text6 = UI.OVERLAYS.HEATFLOW.HOVERTITLE;
					}
				}
				else
				{
					CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
					if (cavityForCell != null)
					{
						Room room = cavityForCell.room;
						RoomTypes.RoomType roomType = null;
						if (room != null)
						{
							roomType = RoomTypes.GetRoomType(room);
							text6 = roomType.name;
						}
						else
						{
							text6 = UI.OVERLAYS.ROOMS.NOROOM.HEADER;
						}
						text5 = "";
						if (room != null)
						{
							string text8 = RoomDetails.EFFECT.resolve_string_function(room);
							string text9 = RoomDetails.ASSIGNED_TO.resolve_string_function(room);
							string text10 = RoomDetails.RoomDetailString(room);
							string text11 = RoomConstraints.RoomCriteriaString(room);
							if (text8 != "")
							{
								text5 = text5 + text8 + "\n";
							}
							if (text9 != "" && roomType != RoomTypes.neutral_type)
							{
								text5 = text5 + "\n" + text9 + "\n";
							}
							if (text10 != "")
							{
								text5 = text5 + "\n" + text10 + "\n";
							}
							if (text11 != "")
							{
								text5 = text5 + "\n" + text11 + "\n";
							}
						}
						else
						{
							text5 += UI.OVERLAYS.ROOMS.NOROOM.DESC;
						}
						text7 = text5;
					}
				}
				bool flag15 = SimDebugView.Instance.GetMode() != SimViewMode.None && text7 != "" && (Grid.Visible[num] > 0 || DebugPaintElementScreen.Instance.gameObject.activeSelf);
				this.hoverScreenElements.OverlayInfoHeader.text = text6;
				this.hoverScreenElements.OverlayInfo.text = text7;
				base.SetLineActive(this.hoverScreenElements.OverlayInfoHeader.transform.parent.gameObject, flag15);
				base.SetLineActive(this.hoverScreenElements.OverlayInfo.transform.parent.gameObject, flag15);
				this.recentNumberOfDisplayedSelectables = num2 + 1;
			}
		}
	}

	private void ConfigureSelectableStatusItem(StatusItemGroup.Entry item, int hoverSelectableIndex, int statusIndex, bool selected)
	{
		SelectToolHoverTextCard.StatusIconPair statusIconPair = this.hoverScreenElements.selectableHoverFields[hoverSelectableIndex].selectableWarnings[statusIndex];
		TextStyleSetting textStyleSetting = ((!this.IsStatusItemWarning(item)) ? this.Styles_BodyText.Standard : this.HoverTextStyleSettings[1]);
		Sprite sprite = ((item.item.sprite == null) ? HoverTextScreen.Instance.GetSprite("iconWarning") : item.item.sprite.sprite);
		base.SetLineActive(statusIconPair.statusText.transform.parent.gameObject, true);
		statusIconPair.statusText.text = item.GetName();
		statusIconPair.statusText.GetComponent<SetTextStyleSetting>().SetStyle(textStyleSetting);
		statusIconPair.statusIcon.sprite = sprite;
		statusIconPair.statusIcon.color = ((!this.IsStatusItemWarning(item)) ? this.Styles_BodyText.Standard.textColor : this.HoverTextStyleSettings[1].textColor);
	}

	private bool UpdateImmunityDisplay(KSelectable hover_obj, LocText target_field)
	{
		bool flag = false;
		StateMachineController component = hover_obj.GetComponent<StateMachineController>();
		if (component != null)
		{
			ImmuneSystemMonitor.Instance smi = component.GetSMI<ImmuneSystemMonitor.Instance>();
			if (smi != null)
			{
				flag = true;
				AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(hover_obj);
				float value = amountInstance.value;
				bool flag2 = smi.sm.isLosingImmunity.Get(smi);
				Color32 badColorBG = NotificationScreen.Instance.BadColorBG;
				badColorBG.a = byte.MaxValue;
				Color32 color = ((!flag2) ? new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue) : badColorBG);
				string text = string.Format(UI.OVERLAYS.DISEASE.IMMUNITY, GameUtil.GetFormattedPercent(value, GameUtil.TimeSlice.None));
				target_field.text = GameUtil.ColourizeString(color, text);
			}
		}
		return flag;
	}

	private bool ShouldShowSelectableInCurrentOverlay(KSelectable selectable)
	{
		bool flag = true;
		bool flag2;
		if (OverlayScreen.Instance == null)
		{
			flag2 = flag;
		}
		else
		{
			SimViewMode mode = OverlayScreen.Instance.GetMode();
			if (mode != SimViewMode.PipeMap)
			{
				if (mode == SimViewMode.Decor)
				{
					flag = !(selectable.GetComponent<DecorProvider>() == null);
					goto IL_0302;
				}
				if (mode == SimViewMode.OxygenMap)
				{
					flag = !(selectable.GetComponent<AlgaeHabitat>() == null) || !(selectable.GetComponent<Electrolyzer>() == null) || !(selectable.GetComponent<AirFilter>() == null);
					goto IL_0302;
				}
				if (mode == SimViewMode.Crop)
				{
					flag = !(selectable.GetComponent<Uprootable>() == null) || !(selectable.GetComponent<PlanterBox>() == null);
					goto IL_0302;
				}
				if (mode != SimViewMode.LiquidVentMap)
				{
					if (mode == SimViewMode.PowerMap)
					{
						flag = !(selectable.GetComponent<Battery>() == null) || !(selectable.GetComponent<Wire>() == null) || !(selectable.GetComponent<PowerTransformer>() == null) || !(selectable.GetComponent<EnergyConsumer>() == null) || !(selectable.GetComponent<EnergyGenerator>() == null);
						goto IL_0302;
					}
					if (mode != SimViewMode.GasVentMap)
					{
						if (mode == SimViewMode.HeatFlow || mode == SimViewMode.ThermalConductivity)
						{
							flag = false;
							goto IL_0302;
						}
						if (mode == SimViewMode.TemperatureMap)
						{
							goto IL_0302;
						}
						if (mode == SimViewMode.Disease)
						{
							flag = !(selectable.GetComponent<PrimaryElement>() == null);
							goto IL_0302;
						}
						if (mode != SimViewMode.Light)
						{
							goto IL_0302;
						}
						flag = !(selectable.GetComponent<Light2D>() == null);
						goto IL_0302;
					}
				}
			}
			flag = !(selectable.GetComponent<Conduit>() == null) || !(selectable.GetComponent<Vent>() == null) || !(selectable.GetComponent<Pump>() == null) || !(selectable.GetComponent<LiquidFilterable>() == null) || !(selectable.GetComponent<GasFilterable>() == null);
			IL_0302:
			flag2 = flag;
		}
		return flag2;
	}

	private bool ShowStatusItemInCurrentOverlay(StatusItem status)
	{
		return !(OverlayScreen.Instance == null) && (status.status_overlays & (int)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode())) == (int)StatusItem.GetStatusItemOverlayBySimViewMode(OverlayScreen.Instance.GetMode());
	}

	private int maxNumberOfDisplaySelectables = 5;

	public static int maxNumberOfDisplayedSelectableWarnings = 10;

	private int dividerHeight = 20;

	private Dictionary<SimViewMode, Func<bool>> overlayFilterMap = new Dictionary<SimViewMode, Func<bool>>();

	public int recentNumberOfDisplayedSelectables = 0;

	public int currentSelectedSelectableIndex = -1;

	public static List<GameObject> highlightedObjects = new List<GameObject>();

	private SelectToolHoverTextCard.HoverScreenFields hoverScreenElements;

	private List<KSelectable> overlayValidHoverObjects = new List<KSelectable>();

	private List<KSelectable> previousHoverObjects = new List<KSelectable>();

	private struct HoverScreenFields
	{
		public LocText DirectOrderName;

		public GameObject DirectOrderInstructions;

		public GameObject DirectOrderDivider;

		public GameObject UnknownAreaLine;

		public SelectToolHoverTextCard.SelectableHoverFields[] selectableHoverFields;

		public SelectToolHoverTextCard.CellElementSelectableFields cellElement;

		public LocText AverageFlowRateLine;

		public LocText FlowRateStatusLine;

		public GameObject OverlayInfoDivider;

		public LocText OverlayInfoHeader;

		public LocText OverlayInfo;
	}

	public struct SelectableHoverFields
	{
		public LocText selectableName;

		public LocText selectableStatus;

		public SelectToolHoverTextCard.StatusIconPair[] selectableWarnings;

		public LocText selectableTemperature;

		public LocText selectableImmunity;

		public LocText selectableDisease;

		public GameObject selectableDivider;

		public GameObject selectableLineBreak;

		public ShadowBar shadowBar;
	}

	private struct CellElementSelectableFields
	{
		public ShadowBar ElementShadowBar;

		public Image ElementStateIcon;

		public LocText ElementCategory;

		public LocText ElementName;

		public LocText ElementDisease;

		public LocText[] ElementMass;

		public LocText ElementTemperature;

		public GameObject StandAloneBreathableLine;

		public LocText StandAloneBreathableDescription;

		public LocText BuriedItem;
	}

	public struct StatusIconPair
	{
		public LocText statusText;

		public Image statusIcon;
	}
}
