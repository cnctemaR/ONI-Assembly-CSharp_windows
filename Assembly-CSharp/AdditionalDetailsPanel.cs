using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class AdditionalDetailsPanel : TargetScreen
{
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
		float num = 0f;
		bool flag = false;
		float num2;
		float num3;
		Element element;
		byte b;
		int num4;
		if (component != null)
		{
			num2 = component.Mass;
			num3 = component.Temperature;
			element = component.Element;
			b = component.DiseaseIdx;
			num4 = component.DiseaseCount;
			Attributes attributes = this.selectedTarget.GetAttributes();
			if (attributes != null)
			{
				AttributeInstance attributeInstance = this.selectedTarget.GetAttributes().Get(Db.Get().Attributes.ThermalConductivityBarrier);
				if (attributeInstance != null)
				{
					flag = true;
					num = attributeInstance.GetTotalValue();
				}
			}
		}
		else
		{
			if (!(component2 != null))
			{
				return;
			}
			num2 = component2.Mass;
			num3 = component2.temperature;
			element = component2.element;
			b = component2.diseaseIdx;
			num4 = component2.diseaseCount;
		}
		bool flag2 = element.id == SimHashes.Vacuum || element.id == SimHashes.Void;
		float specificHeatCapacity = element.specificHeatCapacity;
		float num5 = element.thermalConductivity;
		Building component3 = this.selectedTarget.GetComponent<Building>();
		num5 *= ((!(component3 != null)) ? 1f : component3.Def.ThermalConductivity);
		float highTemp = element.highTemp;
		float lowTemp = element.lowTemp;
		this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.PRIMARYELEMENT.NAME, element.name)).Tooltip(this.drawer.Format(UI.ELEMENTAL.PRIMARYELEMENT.TOOLTIP, element.name)).NewLabel(this.drawer.Format(UI.ELEMENTAL.MASS.NAME, GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")))
			.Tooltip(this.drawer.Format(UI.ELEMENTAL.MASS.TOOLTIP, GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")));
		if (!flag2)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true))).Tooltip(this.drawer.Format(UI.ELEMENTAL.TEMPERATURE.TOOLTIP, GameUtil.GetFormattedTemperature(num3, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true))).NewLabel(this.drawer.Format(UI.ELEMENTAL.DISEASE.NAME, GameUtil.GetFormattedDisease(b, num4, false)))
				.Tooltip(this.drawer.Format(UI.ELEMENTAL.DISEASE.TOOLTIP, GameUtil.GetFormattedDisease(b, num4, true)))
				.NewLabel(string.Concat(new string[]
				{
					this.drawer.Format(UI.ELEMENTAL.SHC.NAME, specificHeatCapacity),
					" (",
					UI.UNITSUFFIXES.ELECTRICAL.JOULE,
					"/",
					UI.UNITSUFFIXES.MASS.GRAM,
					")/",
					UI.UNITSUFFIXES.TEMPERATURE.KELVIN
				}))
				.Tooltip(this.drawer.Format(UI.ELEMENTAL.SHC.TOOLTIP, specificHeatCapacity))
				.NewLabel(string.Concat(new string[]
				{
					this.drawer.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.NAME, num5),
					" (",
					UI.UNITSUFFIXES.ELECTRICAL.WATT,
					"/",
					UI.UNITSUFFIXES.DISTANCE.METER,
					")/",
					UI.UNITSUFFIXES.TEMPERATURE.KELVIN
				}))
				.Tooltip(this.drawer.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.TOOLTIP, num5));
		}
		if (flag)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.CONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(num))).Tooltip(delegate
			{
				AttributeInstance attributeInstance3 = this.selectedTarget.GetAttributes().Get("ThermalConductivityBarrier");
				string text = this.drawer.Format(UI.ELEMENTAL.CONDUCTIVITYBARRIER.NAME, attributeInstance3.GetFormattedValue());
				text += UI.HORIZONTAL_BR_RULE;
				foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in attributeInstance3.Modifiers)
				{
					text += this.drawer.Format(DUPLICANTS.ATTRIBUTES.MODIFIER_ENTRY, attributeModifierEntry.Modifier.GetDescription(), attributeModifierEntry.Modifier.GetFormattedString(attributeInstance3.gameObject));
				}
				return text;
			});
		}
		if (element.IsSolid)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.MELTINGPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true))).Tooltip(this.drawer.Format(UI.ELEMENTAL.MELTINGPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)));
			ElementChunk component4 = this.selectedTarget.GetComponent<ElementChunk>();
			if (component4 != null)
			{
				AttributeModifier attributeModifier = component.Element.attributeModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().BuildingAttributes.OverheatTemperature.Id);
				if (attributeModifier != null)
				{
					this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.OVERHEATPOINT.NAME, attributeModifier.GetFormattedString(this.selectedTarget.gameObject))).Tooltip(this.drawer.Format(UI.ELEMENTAL.OVERHEATPOINT.TOOLTIP, attributeModifier.GetFormattedString(this.selectedTarget.gameObject)));
				}
			}
		}
		else if (element.IsLiquid)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.FREEZEPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true))).Tooltip(this.drawer.Format(UI.ELEMENTAL.FREEZEPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true))).NewLabel(this.drawer.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)))
				.Tooltip(this.drawer.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)));
		}
		else if (!flag2)
		{
			this.drawer.NewLabel(this.drawer.Format(UI.ELEMENTAL.DEWPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true))).Tooltip(this.drawer.Format(UI.ELEMENTAL.DEWPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)));
		}
		Attributes attributes2 = this.selectedTarget.GetAttributes();
		if (attributes2 != null)
		{
			for (int i = 0; i < attributes2.Count; i++)
			{
				AttributeInstance attributeInstance2 = attributes2.AttributeTable[i];
				if (attributeInstance2.Attribute.ShowInUI == Klei.AI.Attribute.Display.Details)
				{
					this.drawer.NewLabel(attributeInstance2.modifier.Name + ": " + attributeInstance2.GetFormattedValue()).Tooltip(attributeInstance2.GetAttributeValueTooltip());
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
