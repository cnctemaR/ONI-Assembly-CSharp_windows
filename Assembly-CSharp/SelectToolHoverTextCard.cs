using System;
using System.Collections.Generic;
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
	}

	public override void ConfigureHoverScreen()
	{
		HoverTextScreen instance = HoverTextScreen.Instance;
		if (instance.LoadPreConfiguredToolFields(this))
		{
			this.isConfigured = true;
			return;
		}
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
		this.hoverScreenElements.OverlayInfoHeader = instance.AddText(string.Empty, this.Styles_Title.Standard, true);
		instance.NewLine("OverlayInfo", 24);
		this.hoverScreenElements.OverlayInfo = instance.AddText(string.Empty, this.Styles_BodyText.Standard, true);
		instance.EndShadowBar();
		this.hoverScreenElements.OverlayInfoDivider = instance.NewLine("Divider", this.divederHeight);
		this.hoverScreenElements.selectableHoverFields = new SelectToolHoverTextCard.SelectableHoverFields[this.maxNumberOfDisplaySelectables];
		for (int i = 0; i < this.maxNumberOfDisplaySelectables; i++)
		{
			if (i != 0)
			{
				this.hoverScreenElements.selectableHoverFields[i].selectableDivider = instance.NewLine("Divider", this.divederHeight);
			}
			this.hoverScreenElements.selectableHoverFields[i].shadowBar = instance.StartShadowBar(0f, 0f, i == this.currentSelectedSelectableIndex);
			instance.NewLine("SelectableName", 24);
			this.hoverScreenElements.selectableHoverFields[i].selectableName = instance.AddText(string.Empty, this.Styles_Title.Standard, true);
			this.hoverScreenElements.selectableHoverFields[i].selectableWarnings = new SelectToolHoverTextCard.StatusIconPair[SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings];
			for (int j = 0; j < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings; j++)
			{
				instance.NewLine(string.Concat(new object[] { "StatusMessage_", i, "_", j }), 24);
				this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[j].statusIcon = instance.AddIcon(instance.GetSprite("iconWarning"), 18f, this.iconColor_basic);
				instance.AddIndent(4f, 18f);
				this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[j].statusText = instance.AddText(string.Empty, this.Styles_BodyText.Standard, false);
			}
			instance.NewLine("SelectableTemperature", 24);
			instance.AddIcon(instance.GetSprite("dash"), 18f, this.iconColor_basic);
			instance.AddIndent(4f, 18f);
			this.hoverScreenElements.selectableHoverFields[i].selectableTemperature = instance.AddText(string.Empty, this.Styles_Values.Property.Standard, true);
			instance.EndShadowBar();
		}
		this.hoverScreenElements.selectableHoverFields[0].selectableDivider = instance.NewLine("Divider", this.divederHeight);
		this.hoverScreenElements.cellElement.ElementShadowBar = instance.StartShadowBar(0f, 0f, false);
		instance.NewLine("Line_ElementName", 24);
		this.hoverScreenElements.cellElement.ElementName = instance.AddText(string.Empty, this.Styles_Title.Standard, true);
		instance.NewLine("Line_Category", 24);
		instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
		instance.AddIndent(4f, 18f);
		this.hoverScreenElements.cellElement.ElementCategory = instance.AddText(string.Empty, this.HoverTextStyleSettings[0], false);
		instance.NewLine("Mass", 24);
		instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
		instance.AddIndent(4f, 18f);
		this.hoverScreenElements.cellElement.ElementMass = new LocText[4];
		this.hoverScreenElements.cellElement.ElementMass[0] = instance.AddText(string.Empty, this.Styles_Values.Property.Standard, true);
		this.hoverScreenElements.cellElement.ElementMass[1] = instance.AddText(string.Empty, this.Styles_Values.Property_Decimal.Standard, true);
		this.hoverScreenElements.cellElement.ElementMass[2] = instance.AddText(string.Empty, this.Styles_Values.Property_Unit.Standard, false);
		this.hoverScreenElements.cellElement.ElementMass[3] = instance.AddText(string.Empty, this.Styles_Values.Property_Unit.Standard, true);
		this.hoverScreenElements.cellElement.StandAloneBreathableLine = instance.NewLine("Standalone Breathable", 24);
		instance.AddIcon(instance.GetSprite("iconWarning"), 18f);
		instance.AddIndent(4f, 18f);
		this.hoverScreenElements.cellElement.StandAloneBreathableDescription = instance.AddText(string.Empty, this.Styles_BodyText.Standard, false);
		instance.NewLine("Temperature", 24);
		instance.AddIcon(instance.GetSprite("dash"), 18f, this.iconColor_basic);
		instance.AddIndent(4f, 18f);
		this.hoverScreenElements.cellElement.ElementTemperature = instance.AddText(string.Empty, this.Styles_Values.Property.Standard, true);
		instance.NewLine("Buried Item", 24);
		instance.AddIcon(instance.GetSprite("dash"), 18f);
		instance.AddIndent(4f, 18f);
		this.hoverScreenElements.cellElement.BuriedItem = instance.AddText(string.Empty, this.Styles_BodyText.Standard, false);
		instance.NewLine("Average Flow Rate", 24);
		instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
		instance.AddIndent(4f, 18f);
		this.hoverScreenElements.AverageFlowRateLine = instance.AddText(string.Empty, this.Styles_BodyText.Standard, true);
		instance.NewLine("Flow Rate Status", 24);
		instance.AddIcon(instance.GetSprite("dash"), this.iconColor_basic, 18f);
		instance.AddIndent(4f, 18f);
		this.hoverScreenElements.FlowRateStatusLine = instance.AddText(string.Empty, this.Styles_BodyText.Standard, true);
		instance.EndShadowBar();
		this.isConfigured = true;
	}

	public override void SetNotConfigured()
	{
		base.SetNotConfigured();
	}

	private bool IsStatusItemWarning(StatusItemGroup.Entry item)
	{
		return item.item.notificationType == NotificationType.Bad || item.item.notificationType == NotificationType.BadMinor;
	}

	public override void UpdateHoverElements(KSelectable[] hoverObjects)
	{
		if (!this.isConfigured)
		{
			this.ConfigureHoverScreen();
		}
		this.currentSelectedSelectableIndex = -1;
		int num = Grid.PosToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (!Grid.IsValidCell(num))
		{
			return;
		}
		bool flag = true;
		if (Grid.ForceField[num])
		{
			flag = false;
		}
		if (Grid.Visible[num] == 0 && !DebugPaintElementScreen.Instance.gameObject.activeSelf)
		{
			flag = false;
		}
		foreach (KeyValuePair<SimViewMode, Func<bool>> keyValuePair in this.overlayFilterMap)
		{
			if (OverlayScreen.Instance == null)
			{
				return;
			}
			if (OverlayScreen.Instance.GetMode() == keyValuePair.Key)
			{
				if (!keyValuePair.Value())
				{
					flag = false;
				}
				break;
			}
		}
		int num2 = 0;
		int mask = LayerMask.GetMask(new string[] { "MaskedOverlay", "MaskedOverlayBG" });
		for (int i = 0; i < this.hoverScreenElements.selectableHoverFields.Length; i++)
		{
			if (hoverObjects.Length - 1 >= i && hoverObjects[i] != null && hoverObjects[i].GetComponent<CellSelectionObject>() == null)
			{
				KSelectable kselectable = hoverObjects[i];
				if (OverlayScreen.Instance.mode == SimViewMode.None || (kselectable.gameObject.layer & mask) == 0)
				{
					bool flag2 = SelectTool.Instance.selected == hoverObjects[i];
					if (flag2)
					{
						this.currentSelectedSelectableIndex = i;
					}
					num2++;
					this.hoverScreenElements.selectableHoverFields[i].shadowBar.toggleSelectionBorder(flag2);
					base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableName.transform.parent.gameObject, true);
					base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableDivider, true);
					if (this.hoverScreenElements.selectableHoverFields[i].selectableName != null)
					{
						this.hoverScreenElements.selectableHoverFields[i].selectableName.text = GameUtil.GetUnitFormattedName(hoverObjects[i].gameObject, true);
						this.hoverScreenElements.selectableHoverFields[i].selectableName.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_Title.Standard);
					}
					int num3 = 0;
					foreach (StatusItemGroup.Entry entry in hoverObjects[i].GetStatusItemGroup())
					{
						if (num3 >= SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
						{
							break;
						}
						if (entry.category != null && entry.category.Id == "Main")
						{
							if (num3 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
							{
								this.ConfigureSelectableStatusItem(entry, i, num3, flag2);
							}
							else
							{
								base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[num3].statusText.transform.parent.gameObject, false);
							}
							num3++;
						}
					}
					foreach (StatusItemGroup.Entry entry2 in hoverObjects[i].GetStatusItemGroup())
					{
						if (num3 >= SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
						{
							break;
						}
						if (entry2.category == null || entry2.category.Id != "Main")
						{
							if (num3 < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings)
							{
								this.ConfigureSelectableStatusItem(entry2, i, num3, flag2);
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
					bool flag3 = true;
					bool flag4 = SimViewMode.TemperatureMap == SimDebugView.Instance.GetMode();
					if (kselectable.GetComponent<MinionIdentity>())
					{
						flag3 = false;
					}
					else if (kselectable.GetComponent<Constructable>())
					{
						flag3 = false;
					}
					else if (flag4 && kselectable.GetComponent<PrimaryElement>())
					{
						num4 = kselectable.GetComponent<PrimaryElement>().Temperature;
					}
					else if (kselectable.GetComponent<Building>() && kselectable.GetComponent<PrimaryElement>())
					{
						num4 = kselectable.GetComponent<PrimaryElement>().Temperature;
					}
					else if (kselectable.GetComponent<CellSelectionObject>() != null)
					{
						num4 = kselectable.GetComponent<CellSelectionObject>().temperature;
					}
					else
					{
						flag3 = false;
					}
					base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableTemperature.transform.parent.gameObject, flag3);
					if (flag3)
					{
						this.hoverScreenElements.selectableHoverFields[i].selectableTemperature.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
						this.hoverScreenElements.selectableHoverFields[i].selectableTemperature.text = GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true);
					}
					BuildingComplete component = kselectable.GetComponent<BuildingComplete>();
					if (component != null && component.Def.IsFoundation)
					{
						flag = false;
					}
				}
			}
			else
			{
				base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableName.transform.parent.gameObject, false);
				for (int k = 0; k < SelectToolHoverTextCard.maxNumberOfDisplayedSelectableWarnings; k++)
				{
					base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableWarnings[k].statusText.transform.parent.gameObject, false);
				}
				base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableDivider, false);
				base.SetLineActive(this.hoverScreenElements.selectableHoverFields[i].selectableTemperature.transform.parent.gameObject, false);
			}
		}
		base.SetLineActive(this.hoverScreenElements.UnknownAreaLine, Grid.Visible[num] == 0 && !flag && !DebugPaintElementScreen.Instance.gameObject.activeSelf);
		base.SetLineActive(this.hoverScreenElements.cellElement.ElementMass[0].transform.parent.gameObject, !Grid.Element[num].IsVacuum && flag);
		base.SetLineActive(this.hoverScreenElements.cellElement.ElementName.transform.parent.gameObject, flag);
		base.SetLineActive(this.hoverScreenElements.cellElement.ElementCategory.transform.parent.gameObject, flag);
		base.SetLineActive(this.hoverScreenElements.cellElement.ElementTemperature.transform.parent.gameObject, !Grid.Element[num].IsVacuum && flag);
		bool flag5 = flag && num2 > 0;
		base.SetLineActive(this.hoverScreenElements.selectableHoverFields[0].selectableDivider, flag5);
		if (flag)
		{
			CellSelectionObject cellSelectionObject = null;
			if (SelectTool.Instance.selected != null)
			{
				cellSelectionObject = SelectTool.Instance.selected.GetComponent<CellSelectionObject>();
			}
			bool flag6 = cellSelectionObject != null && cellSelectionObject.mouseCell == cellSelectionObject.alternateSelectionObject.mouseCell;
			if (flag6)
			{
				this.currentSelectedSelectableIndex = this.recentNumberOfDisplayedSelectables - 1;
			}
			this.hoverScreenElements.cellElement.ElementShadowBar.toggleSelectionBorder(flag6);
			this.hoverScreenElements.cellElement.ElementName.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_Title.Standard);
			this.hoverScreenElements.cellElement.ElementTemperature.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
			for (int l = 0; l < this.hoverScreenElements.cellElement.ElementMass.Length; l++)
			{
				this.hoverScreenElements.cellElement.ElementMass[l].GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
			}
			this.hoverScreenElements.cellElement.ElementCategory.GetComponent<SetTextStyleSetting>().SetStyle(this.Styles_BodyText.Standard);
			this.hoverScreenElements.cellElement.ElementCategory.text = ElementLoader.elements[(int)Grid.Cell[num].elementIdx].GetMaterialCategoryTag().ProperName();
			base.SetLineActive(this.hoverScreenElements.cellElement.ElementCategory.transform.parent.gameObject, !ElementLoader.elements[(int)Grid.Cell[num].elementIdx].IsVacuum);
			this.hoverScreenElements.cellElement.ElementName.text = ElementLoader.elements[(int)Grid.Cell[num].elementIdx].name.ToUpper();
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
			string text = ((element.specificHeatCapacity != 0f) ? GameUtil.GetFormattedTemperature(Grid.Cell[num].temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true) : "N/A");
			this.hoverScreenElements.cellElement.ElementTemperature.text = text;
			bool flag7 = false;
			bool flag8 = false;
			if (element.id == SimHashes.OxyRock)
			{
				flag8 = true;
				float num5 = Grid.AccumulatedFlow[num] / 3f;
				string text2 = Strings.Get("STRINGS.BUILDING.STATUSITEMS.EMITTINGOXYGENAVG.NAME");
				text2 = text2.Replace("{FlowRate}", GameUtil.GetFormattedMass(num5, GameUtil.TimeSlice.PerSecond, true, "{0:0.#}"));
				this.hoverScreenElements.AverageFlowRateLine.text = text2;
				if (num5 <= 0f)
				{
					bool flag9;
					bool flag10;
					GameUtil.IsEmissionBlocked(num, out flag9, out flag10);
					string text3 = null;
					if (flag9)
					{
						text3 = MISC.STATUSITEMS.OXYROCK.NEIGHBORSBLOCKED.NAME;
					}
					else if (flag10)
					{
						text3 = MISC.STATUSITEMS.OXYROCK.OVERPRESSURE.NAME;
					}
					flag7 = text3 != null;
					this.hoverScreenElements.FlowRateStatusLine.text = text3;
				}
			}
			base.SetLineActive(this.hoverScreenElements.FlowRateStatusLine.transform.parent.gameObject, flag8 && flag7);
			base.SetLineActive(this.hoverScreenElements.AverageFlowRateLine.transform.parent.gameObject, flag8);
		}
		if (!flag)
		{
			base.SetLineActive(this.hoverScreenElements.cellElement.StandAloneBreathableLine, false);
		}
		string text4 = string.Empty;
		string text5 = string.Empty;
		string text6 = string.Empty;
		SimViewMode mode = SimDebugView.Instance.GetMode();
		if (mode != SimViewMode.HeatFlow)
		{
			if (mode == SimViewMode.Decor)
			{
				List<DecorProvider> list = new List<DecorProvider>();
				GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.decorProviders.mask, list);
				text4 = string.Concat(new object[]
				{
					UI.OVERLAYS.DECOR.TOTAL,
					" ",
					Db.Get().BuildingAttributes.Decor.Name,
					": ",
					GameUtil.GetDecorAtCell(num)
				});
				if (!Grid.Solid[num])
				{
					List<DecorEntry> list2 = new List<DecorEntry>();
					foreach (DecorProvider decorProvider in list)
					{
						int decorForCell = decorProvider.GetDecorForCell(num);
						if (decorForCell != 0)
						{
							string name = decorProvider.GetName();
							bool flag11 = false;
							for (int m = 0; m < list2.Count; m++)
							{
								if (list2[m].name == name)
								{
									DecorEntry decorEntry = list2[m];
									decorEntry.count++;
									decorEntry.decor += decorForCell;
									list2[m] = decorEntry;
									flag11 = true;
									break;
								}
							}
							if (!flag11)
							{
								list2.Add(new DecorEntry(name, decorForCell));
							}
						}
					}
					int lightDecorBonus = DecorProvider.GetLightDecorBonus(num);
					if (lightDecorBonus > 0)
					{
						list2.Add(new DecorEntry(UI.OVERLAYS.DECOR.LIGHTING, lightDecorBonus));
					}
					list2.Sort((DecorEntry x, DecorEntry y) => y.decor.CompareTo(x.decor));
					if (list2.Count > 0)
					{
						text4 += "\n";
					}
					foreach (DecorEntry decorEntry2 in list2)
					{
						text4 = text4 + "\n• " + decorEntry2.ToString();
					}
				}
				text4 += "\n";
				text5 = UI.OVERLAYS.DECOR.HOVERTITLE;
				text6 = text4;
			}
		}
		else if (!Grid.Solid[num])
		{
			float thermalComfort = GameUtil.GetThermalComfort(num, 0f);
			float thermalComfort2 = GameUtil.GetThermalComfort(num, -0.08368001f);
			float num6 = 0f;
			if (thermalComfort2 * 0.001f > -0.13946667f - num6 && thermalComfort2 * 0.001f < 0.13946667f + num6)
			{
				text4 = UI.OVERLAYS.HEATFLOW.NEUTRAL;
			}
			else if (thermalComfort2 <= ExternalTemperatureMonitor.GetExternalColdThreshold(null))
			{
				text4 = UI.OVERLAYS.HEATFLOW.COOLING;
			}
			else if (thermalComfort2 >= ExternalTemperatureMonitor.GetExternalWarmThreshold(null))
			{
				text4 = UI.OVERLAYS.HEATFLOW.HEATING;
			}
			text4 = text4 + " (" + GameUtil.GetFormattedWattage(thermalComfort, "F1") + ")";
			text6 = text4;
			text5 = UI.OVERLAYS.HEATFLOW.HOVERTITLE;
		}
		bool flag12 = SimDebugView.Instance.GetMode() != SimViewMode.None && text6 != string.Empty;
		this.hoverScreenElements.OverlayInfoHeader.text = text5;
		this.hoverScreenElements.OverlayInfo.text = text6;
		base.SetLineActive(this.hoverScreenElements.OverlayInfoHeader.transform.parent.gameObject, flag12);
		base.SetLineActive(this.hoverScreenElements.OverlayInfo.transform.parent.gameObject, flag12);
		this.recentNumberOfDisplayedSelectables = num2 + 1;
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

	private int maxNumberOfDisplaySelectables = 5;

	public static int maxNumberOfDisplayedSelectableWarnings = 10;

	private int divederHeight = 20;

	private Dictionary<SimViewMode, Func<bool>> overlayFilterMap = new Dictionary<SimViewMode, Func<bool>>();

	public int recentNumberOfDisplayedSelectables;

	public int currentSelectedSelectableIndex = -1;

	private SelectToolHoverTextCard.HoverScreenFields hoverScreenElements;

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
