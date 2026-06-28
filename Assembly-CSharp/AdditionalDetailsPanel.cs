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
		this.RefreshDetails();
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
		float num4;
		Element element;
		byte b;
		int num5;
		if (component != null)
		{
			num2 = component.Mass;
			num3 = component.Units;
			num4 = component.Temperature;
			element = component.Element;
			b = component.DiseaseIdx;
			num5 = component.DiseaseCount;
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
			num3 = 1f;
			num4 = component2.temperature;
			element = component2.element;
			b = component2.diseaseIdx;
			num5 = component2.diseaseCount;
		}
		bool flag2 = element.id == SimHashes.Vacuum || element.id == SimHashes.Void;
		float specificHeatCapacity = element.specificHeatCapacity;
		float thermalConductivity = element.thermalConductivity;
		float highTemp = element.highTemp;
		float lowTemp = element.lowTemp;
		GameObject gameObject;
		if (component != null && component.CountableUnits)
		{
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "Units");
			gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.UNITS.NAME, num3.ToString("F0"));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.UNITS.TOOLTIP, num3.ToString("F0"), this.selectedTarget.GetProperName());
		}
		else
		{
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "Units");
			gameObject.SetActive(false);
		}
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "Element");
		gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.PRIMARYELEMENT.NAME, element.name);
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.PRIMARYELEMENT.TOOLTIP, element.name);
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "Mass");
		gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.MASS.NAME, GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.MASS.TOOLTIP, GameUtil.GetFormattedMass(num2, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "Temperature");
		gameObject.SetActive(!flag2);
		gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.TEMPERATURE.TOOLTIP, GameUtil.GetFormattedTemperature(num4, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "Disease");
		gameObject.SetActive(!flag2);
		gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.DISEASE.NAME, GameUtil.GetFormattedDisease(b, num5, false));
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.DISEASE.TOOLTIP, GameUtil.GetFormattedDisease(b, num5, true));
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "SHC");
		gameObject.SetActive(!flag2);
		gameObject.GetComponent<LocText>().text = string.Concat(new string[]
		{
			string.Format(UI.ELEMENTAL.SHC.NAME, specificHeatCapacity),
			" (",
			UI.UNITSUFFIXES.ELECTRICAL.JOULE,
			"/",
			UI.UNITSUFFIXES.MASS.GRAM,
			")/",
			UI.UNITSUFFIXES.TEMPERATURE.KELVIN
		});
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.SHC.TOOLTIP, specificHeatCapacity);
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "THERMALCONDUCTIVITY");
		gameObject.SetActive(!flag2);
		gameObject.GetComponent<LocText>().text = string.Concat(new string[]
		{
			string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.NAME, thermalConductivity),
			" (",
			UI.UNITSUFFIXES.ELECTRICAL.WATT,
			"/",
			UI.UNITSUFFIXES.DISTANCE.METER,
			")/",
			UI.UNITSUFFIXES.TEMPERATURE.KELVIN
		});
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.TOOLTIP, thermalConductivity);
		string.Format(UI.ELEMENTAL.THERMALCONDUCTIVITY.TOOLTIP, thermalConductivity);
		gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "CONDUCTIVITYBARRIER");
		Func<string> func = delegate
		{
			AttributeInstance attributeInstance3 = this.selectedTarget.GetAttributes().Get("ThermalConductivityBarrier");
			string text = string.Format(UI.ELEMENTAL.CONDUCTIVITYBARRIER.NAME, attributeInstance3.GetFormattedValue());
			text += UI.HORIZONTAL_BR_RULE;
			foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in attributeInstance3.Modifiers)
			{
				text += string.Format(DUPLICANTS.ATTRIBUTES.MODIFIER_ENTRY, attributeModifierEntry.Modifier.Description, attributeModifierEntry.Modifier.GetFormattedString(attributeInstance3.gameObject));
			}
			return text;
		};
		if (flag)
		{
			gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.CONDUCTIVITYBARRIER.NAME, GameUtil.GetFormattedDistance(num));
			gameObject.GetComponent<ToolTip>().toolTip = func();
		}
		else
		{
			gameObject.SetActive(false);
		}
		if (element.IsSolid)
		{
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "MELTINGPOINT");
			gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.MELTINGPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.MELTINGPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "OVERHEATPOINT");
			ElementChunk component3 = this.selectedTarget.GetComponent<ElementChunk>();
			if (component3 != null)
			{
				AttributeModifier attributeModifier = component.Element.attributeModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().BuildingAttributes.OverheatTemperature.Id);
				if (attributeModifier != null)
				{
					gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.OVERHEATPOINT.NAME, attributeModifier.GetFormattedString(this.selectedTarget.gameObject));
					gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.OVERHEATPOINT.TOOLTIP, attributeModifier.GetFormattedString(this.selectedTarget.gameObject));
					gameObject.SetActive(true);
				}
				else
				{
					gameObject.SetActive(false);
				}
			}
			else
			{
				gameObject.SetActive(false);
			}
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "FREEZEPOINT");
			gameObject.SetActive(false);
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "VAPOURIZATIONPOINT");
			gameObject.SetActive(false);
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "DEWPOINT");
			gameObject.SetActive(false);
		}
		else if (element.IsLiquid)
		{
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "MELTINGPOINT");
			gameObject.SetActive(false);
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "FREEZEPOINT");
			gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.FREEZEPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.FREEZEPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "VAPOURIZATIONPOINT");
			gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "DEWPOINT");
			gameObject.SetActive(false);
		}
		else if (!flag2)
		{
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "MELTINGPOINT");
			gameObject.SetActive(false);
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "VAPOURIZATIONPOINT");
			gameObject.SetActive(false);
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "FREEZEPOINT");
			gameObject.SetActive(false);
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "DEWPOINT");
			gameObject.GetComponent<LocText>().text = string.Format(UI.ELEMENTAL.DEWPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.ELEMENTAL.DEWPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
		}
		Attributes attributes2 = this.selectedTarget.GetAttributes();
		if (attributes2 != null)
		{
			for (int i = 0; i < attributes2.Count; i++)
			{
				AttributeInstance attributeInstance2 = attributes2.AttributeTable[i];
				if (attributeInstance2.Attribute.ShowInUI == Klei.AI.Attribute.Display.Details)
				{
					gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, attributeInstance2.Id);
					gameObject.GetComponent<LocText>().text = attributeInstance2.modifier.Name + ": " + attributeInstance2.GetFormattedValue();
					gameObject.GetComponent<ToolTip>().toolTip = attributeInstance2.GetAttributeValueTooltip();
					gameObject.SetActive(true);
				}
			}
		}
		List<Descriptor> detailDescriptors = GameUtil.GetDetailDescriptors(GameUtil.GetAllDescriptors(this.selectedTarget, false));
		for (int j = 0; j < detailDescriptors.Count; j++)
		{
			Descriptor descriptor = detailDescriptors[j];
			gameObject = this.AddOrGetLabel(this.detailLabels, this.detailsPanel, "Descriptor" + j.ToString());
			gameObject.GetComponent<LocText>().text = descriptor.text;
			gameObject.GetComponent<ToolTip>().toolTip = descriptor.tooltipText;
			gameObject.SetActive(true);
		}
	}

	public GameObject attributesLabelTemplate;

	private GameObject detailsPanel;

	private Dictionary<string, GameObject> detailLabels = new Dictionary<string, GameObject>();
}
