using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class AdditionalDetailsPanel : TargetScreen
{
	public override bool IsValidForTarget(GameObject target)
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.detailsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false);
		this.drawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.detailsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
	}

	private void Update()
	{
		this.Refresh();
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		this.Refresh();
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnDeselectTarget(target);
	}

	private void Refresh()
	{
		this.drawer.BeginDrawing();
		this.RefreshDetails();
		this.drawer.EndDrawing();
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, GameObject panel, string id)
	{
		GameObject gameObject;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(this.attributesLabelTemplate, panel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject, null);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
	}

	private void RefreshDetails()
	{
		this.detailsPanel.SetActive(true);
		this.detailsPanel.GetComponent<CollapsibleDetailContentPanel>().HeaderLabel.text = UI.DETAILTABS.DETAILS.GROUPNAME_DETAILS;
		PrimaryElement component = this.selectedTarget.GetComponent<PrimaryElement>();
		CellSelectionObject component2 = this.selectedTarget.GetComponent<CellSelectionObject>();
		float num;
		float num2;
		Element element;
		byte b;
		int num3;
		if (component != null)
		{
			num = component.Mass;
			num2 = component.Temperature;
			element = component.Element;
			b = component.DiseaseIdx;
			num3 = component.DiseaseCount;
		}
		else
		{
			if (!(component2 != null))
			{
				return;
			}
			num = component2.Mass;
			num2 = component2.temperature;
			element = component2.element;
			b = component2.diseaseIdx;
			num3 = component2.diseaseCount;
		}
		bool flag = element.id == SimHashes.Vacuum || element.id == SimHashes.Void;
		float specificHeatCapacity = element.specificHeatCapacity;
		float highTemp = element.highTemp;
		float lowTemp = element.lowTemp;
		BuildingComplete component3 = this.selectedTarget.GetComponent<BuildingComplete>();
		float num4;
		if (component3 != null)
		{
			num4 = component3.creationTime;
		}
		else
		{
			num4 = -1f;
		}
		LogicPorts component4 = this.selectedTarget.GetComponent<LogicPorts>();
		EnergyConsumer component5 = this.selectedTarget.GetComponent<EnergyConsumer>();
		Operational component6 = this.selectedTarget.GetComponent<Operational>();
		Battery component7 = this.selectedTarget.GetComponent<Battery>();
		this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.PRIMARYELEMENT.NAME, element.name)).Tooltip(this.drawer.Format(UI.ELEMENTAL.PRIMARYELEMENT.TOOLTIP, element.name)).NewLabel(this.drawer.Format(UI.ELEMENTAL.MASS.NAME, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")))
			.Tooltip(this.drawer.Format(UI.ELEMENTAL.MASS.TOOLTIP, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")));
		if (num4 > 0f)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.AGE.NAME, Util.FormatTwoDecimalPlace((GameClock.Instance.GetTime() - num4) / 600f))).Tooltip(this.drawer.Format(UI.ELEMENTAL.AGE.TOOLTIP, Util.FormatTwoDecimalPlace((GameClock.Instance.GetTime() - num4) / 600f)));
		}
		int num5 = 5;
		float num6;
		float num7;
		float num8;
		if (component6 != null && (component4 != null || component5 != null || component7 != null))
		{
			num6 = component6.GetCurrentCycleUptime();
			num7 = component6.GetLastCycleUptime();
			num8 = component6.GetUptimeOverCycles(num5);
		}
		else
		{
			num6 = -1f;
			num7 = -1f;
			num8 = -1f;
		}
		if (num6 >= 0f)
		{
			string text = UI.ELEMENTAL.UPTIME.NAME;
			text = text.Replace("{0}", "    • ");
			text = text.Replace("{1}", UI.ELEMENTAL.UPTIME.THIS_CYCLE);
			text = text.Replace("{2}", GameUtil.GetFormattedPercent(num6 * 100f, GameUtil.TimeSlice.None));
			text = text.Replace("{3}", UI.ELEMENTAL.UPTIME.LAST_CYCLE);
			text = text.Replace("{4}", GameUtil.GetFormattedPercent(num7 * 100f, GameUtil.TimeSlice.None));
			text = text.Replace("{5}", UI.ELEMENTAL.UPTIME.LAST_X_CYCLES.Replace("{0}", num5.ToString()));
			text = text.Replace("{6}", GameUtil.GetFormattedPercent(num8 * 100f, GameUtil.TimeSlice.None));
			this.drawer.NewLabel(text);
		}
		if (!flag)
		{
			bool flag2 = false;
			float num9 = element.thermalConductivity;
			Building component8 = this.selectedTarget.GetComponent<Building>();
			if (component8 != null)
			{
				num9 *= component8.Def.ThermalConductivity;
				flag2 = component8.Def.ThermalConductivity < 1f;
			}
			string temperatureUnitSuffix = GameUtil.GetTemperatureUnitSuffix();
			float num10 = specificHeatCapacity * 1f;
			string text2 = string.Format(UI.ELEMENTAL.SHC.NAME, GameUtil.GetDisplaySHC(num10).ToString("0.000"));
			string text3 = UI.ELEMENTAL.SHC.TOOLTIP;
			text3 = text3.Replace("{SPECIFIC_HEAT_CAPACITY}", text2 + GameUtil.GetSHCSuffix());
			text3 = text3.Replace("{TEMPERATURE_UNIT}", temperatureUnitSuffix);
			string text4 = string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.NAME, GameUtil.GetDisplayThermalConductivity(num9).ToString("0.000"));
			string text5 = UI.ELEMENTAL.THERMALCONDUCTIVITY.TOOLTIP;
			text5 = text5.Replace("{THERMAL_CONDUCTIVITY}", text4 + GameUtil.GetThermalConductivitySuffix());
			text5 = text5.Replace("{TEMPERATURE_UNIT}", temperatureUnitSuffix);
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).Tooltip(this.drawer.Format(UI.ELEMENTAL.TEMPERATURE.TOOLTIP, GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).NewLabel(this.drawer.Format(UI.ELEMENTAL.DISEASE.NAME, GameUtil.GetFormattedDisease(b, num3, false)))
				.Tooltip(this.drawer.Format(UI.ELEMENTAL.DISEASE.TOOLTIP, GameUtil.GetFormattedDisease(b, num3, true)))
				.NewLabel(text2)
				.Tooltip(text3)
				.NewLabel(text4)
				.Tooltip(text5);
			if (flag2)
			{
				this.drawer.NewLabel(UI.GAMEOBJECTEFFECTS.INSULATED.NAME).Tooltip(UI.GAMEOBJECTEFFECTS.INSULATED.TOOLTIP);
			}
		}
		if (element.IsSolid)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.MELTINGPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).Tooltip(this.drawer.Format(UI.ELEMENTAL.MELTINGPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
			if (this.selectedTarget.GetComponent<ElementChunk>() != null)
			{
				AttributeModifier attributeModifier = component.Element.attributeModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().BuildingAttributes.OverheatTemperature.Id);
				if (attributeModifier != null)
				{
					this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.OVERHEATPOINT.NAME, attributeModifier.GetFormattedString())).Tooltip(this.drawer.Format(UI.ELEMENTAL.OVERHEATPOINT.TOOLTIP, attributeModifier.GetFormattedString()));
				}
			}
		}
		else if (element.IsLiquid)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.FREEZEPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).Tooltip(this.drawer.Format(UI.ELEMENTAL.FREEZEPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).NewLabel(this.drawer.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)))
				.Tooltip(this.drawer.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
		}
		else if (!flag)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.DEWPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false))).Tooltip(this.drawer.Format(UI.ELEMENTAL.DEWPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
		}
		if (DlcManager.FeatureRadiationEnabled())
		{
			string formattedPercent = GameUtil.GetFormattedPercent(GameUtil.GetRadiationAbsorptionPercentage(Grid.PosToCell(this.selectedTarget)) * 100f, GameUtil.TimeSlice.None);
			this.drawer.NewLabel(this.drawer.Format(UI.DETAILTABS.DETAILS.RADIATIONABSORPTIONFACTOR.NAME, formattedPercent)).Tooltip(this.drawer.Format(UI.DETAILTABS.DETAILS.RADIATIONABSORPTIONFACTOR.TOOLTIP, formattedPercent));
		}
		Attributes attributes = this.selectedTarget.GetAttributes();
		if (attributes != null)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				AttributeInstance attributeInstance = attributes.AttributeTable[i];
				if (DlcManager.IsDlcListValidForCurrentContent(attributeInstance.Attribute.DLCIds) && (attributeInstance.Attribute.ShowInUI == Klei.AI.Attribute.Display.Details || attributeInstance.Attribute.ShowInUI == Klei.AI.Attribute.Display.Expectation))
				{
					this.drawer.NewLabel(attributeInstance.modifier.Name + ": " + attributeInstance.GetFormattedValue()).Tooltip(attributeInstance.GetAttributeValueTooltip());
				}
			}
		}
		List<Descriptor> detailDescriptors = GameUtil.GetDetailDescriptors(GameUtil.GetAllDescriptors(this.selectedTarget, false));
		for (int j = 0; j < detailDescriptors.Count; j++)
		{
			Descriptor descriptor = detailDescriptors[j];
			this.drawer.NewLabel(descriptor.text).Tooltip(descriptor.tooltipText);
		}
	}

	public GameObject attributesLabelTemplate;

	private GameObject detailsPanel;

	private DetailsPanelDrawer drawer;
}
