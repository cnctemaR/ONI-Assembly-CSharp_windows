using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using UnityEngine;

public class AdditionalDetailsPanel : DetailScreenTab
{
	public override bool IsValidForTarget(GameObject target)
	{
		return true;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.detailsPanel = base.CreateCollapsableSection(UI.DETAILTABS.DETAILS.GROUPNAME_DETAILS);
		this.drawer = new DetailsPanelDrawer(this.attributesLabelTemplate, this.detailsPanel.GetComponent<CollapsibleDetailContentPanel>().Content.gameObject);
		this.immuneSystemPanel = base.CreateCollapsableSection(UI.DETAILTABS.DISEASE.CONTRACTION_RATES);
		this.diseaseSourcePanel = base.CreateCollapsableSection(UI.DETAILTABS.DISEASE.DISEASE_SOURCE);
		this.currentGermsPanel = base.CreateCollapsableSection(UI.DETAILTABS.DISEASE.CURRENT_GERMS);
		this.overviewPanel = base.CreateCollapsableSection(UI.DETAILTABS.ENERGYGENERATOR.CIRCUITOVERVIEW);
		this.generatorsPanel = base.CreateCollapsableSection(UI.DETAILTABS.ENERGYGENERATOR.GENERATORS);
		this.consumersPanel = base.CreateCollapsableSection(UI.DETAILTABS.ENERGYGENERATOR.CONSUMERS);
		this.batteriesPanel = base.CreateCollapsableSection(UI.DETAILTABS.ENERGYGENERATOR.BATTERIES);
		base.Subscribe<AdditionalDetailsPanel>(-1514841199, AdditionalDetailsPanel.OnRefreshDataDelegate);
	}

	private void OnRefreshData(object obj)
	{
		this.Refresh();
	}

	private void Update()
	{
		this.Refresh();
	}

	protected override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
		this.Refresh();
	}

	private void Refresh()
	{
		AdditionalDetailsPanel.RefreshDetailsPanel(this.detailsPanel, this.selectedTarget);
		AdditionalDetailsPanel.RefreshImuneSystemPanel(this.immuneSystemPanel, this.selectedTarget);
		AdditionalDetailsPanel.RefreshCurrentGermsPanel(this.currentGermsPanel, this.selectedTarget);
		AdditionalDetailsPanel.RefreshDiseaseSourcePanel(this.diseaseSourcePanel, this.selectedTarget);
		AdditionalDetailsPanel.RefreshEnergyOverviewPanel(this.overviewPanel, this.selectedTarget);
		AdditionalDetailsPanel.RefreshEnergyGeneratorPanel(this.generatorsPanel, this.selectedTarget);
		AdditionalDetailsPanel.RefreshEnergyConsumerPanel(this.consumersPanel, this.selectedTarget);
		AdditionalDetailsPanel.RefreshEnergyBatteriesPanel(this.batteriesPanel, this.selectedTarget);
	}

	private static void RefreshDetailsPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		PrimaryElement component = targetEntity.GetComponent<PrimaryElement>();
		CellSelectionObject component2 = targetEntity.GetComponent<CellSelectionObject>();
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
		BuildingComplete component3 = targetEntity.GetComponent<BuildingComplete>();
		float num4;
		if (component3 != null)
		{
			num4 = component3.creationTime;
		}
		else
		{
			num4 = -1f;
		}
		LogicPorts component4 = targetEntity.GetComponent<LogicPorts>();
		EnergyConsumer component5 = targetEntity.GetComponent<EnergyConsumer>();
		Operational component6 = targetEntity.GetComponent<Operational>();
		Battery component7 = targetEntity.GetComponent<Battery>();
		targetPanel.SetLabel("element_name", string.Format(UI.ELEMENTAL.PRIMARYELEMENT.NAME, element.name), string.Format(UI.ELEMENTAL.PRIMARYELEMENT.TOOLTIP, element.name));
		targetPanel.SetLabel("element_mass", string.Format(UI.ELEMENTAL.MASS.NAME, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")), string.Format(UI.ELEMENTAL.MASS.TOOLTIP, GameUtil.GetFormattedMass(num, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")));
		if (num4 > 0f)
		{
			targetPanel.SetLabel("element_age", string.Format(UI.ELEMENTAL.AGE.NAME, Util.FormatTwoDecimalPlace((GameClock.Instance.GetTime() - num4) / 600f)), string.Format(UI.ELEMENTAL.AGE.TOOLTIP, Util.FormatTwoDecimalPlace((GameClock.Instance.GetTime() - num4) / 600f)));
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
			targetPanel.SetLabel("uptime_name", text, "");
		}
		if (!flag)
		{
			bool flag2 = false;
			float num9 = element.thermalConductivity;
			Building component8 = targetEntity.GetComponent<Building>();
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
			targetPanel.SetLabel("temperature", string.Format(UI.ELEMENTAL.TEMPERATURE.NAME, GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.ELEMENTAL.TEMPERATURE.TOOLTIP, GameUtil.GetFormattedTemperature(num2, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
			targetPanel.SetLabel("disease", string.Format(UI.ELEMENTAL.DISEASE.NAME, GameUtil.GetFormattedDisease(b, num3, false)), string.Format(UI.ELEMENTAL.DISEASE.TOOLTIP, GameUtil.GetFormattedDisease(b, num3, true)));
			targetPanel.SetLabel("shc", text2, text3);
			targetPanel.SetLabel("tc", text4, text5);
			if (flag2)
			{
				targetPanel.SetLabel("insulated", UI.GAMEOBJECTEFFECTS.INSULATED.NAME, UI.GAMEOBJECTEFFECTS.INSULATED.TOOLTIP);
			}
		}
		if (element.IsSolid)
		{
			targetPanel.SetLabel("melting_point", string.Format(UI.ELEMENTAL.MELTINGPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.ELEMENTAL.MELTINGPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
			targetPanel.SetLabel("melting_point", string.Format(UI.ELEMENTAL.MELTINGPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.ELEMENTAL.MELTINGPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
			if (targetEntity.GetComponent<ElementChunk>() != null)
			{
				AttributeModifier attributeModifier = component.Element.attributeModifiers.Find((AttributeModifier m) => m.AttributeId == Db.Get().BuildingAttributes.OverheatTemperature.Id);
				if (attributeModifier != null)
				{
					targetPanel.SetLabel("overheat", string.Format(UI.ELEMENTAL.OVERHEATPOINT.NAME, attributeModifier.GetFormattedString()), string.Format(UI.ELEMENTAL.OVERHEATPOINT.TOOLTIP, attributeModifier.GetFormattedString()));
				}
			}
		}
		else if (element.IsLiquid)
		{
			targetPanel.SetLabel("freezepoint", string.Format(UI.ELEMENTAL.FREEZEPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.ELEMENTAL.FREEZEPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
			targetPanel.SetLabel("vapourizationpoint", string.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.NAME, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.ELEMENTAL.VAPOURIZATIONPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(highTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
		}
		else if (!flag)
		{
			targetPanel.SetLabel("dewpoint", string.Format(UI.ELEMENTAL.DEWPOINT.NAME, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)), string.Format(UI.ELEMENTAL.DEWPOINT.TOOLTIP, GameUtil.GetFormattedTemperature(lowTemp, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)));
		}
		if (DlcManager.FeatureRadiationEnabled())
		{
			string formattedPercent = GameUtil.GetFormattedPercent(GameUtil.GetRadiationAbsorptionPercentage(Grid.PosToCell(targetEntity)) * 100f, GameUtil.TimeSlice.None);
			targetPanel.SetLabel("radiationabsorption", string.Format(UI.DETAILTABS.DETAILS.RADIATIONABSORPTIONFACTOR.NAME, formattedPercent), string.Format(UI.DETAILTABS.DETAILS.RADIATIONABSORPTIONFACTOR.TOOLTIP, formattedPercent));
		}
		Attributes attributes = targetEntity.GetAttributes();
		if (attributes != null)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				AttributeInstance attributeInstance = attributes.AttributeTable[i];
				if (DlcManager.IsCorrectDlcSubscribed(attributeInstance.Attribute) && (attributeInstance.Attribute.ShowInUI == Klei.AI.Attribute.Display.Details || attributeInstance.Attribute.ShowInUI == Klei.AI.Attribute.Display.Expectation))
				{
					targetPanel.SetLabel(attributeInstance.modifier.Id, attributeInstance.modifier.Name + ": " + attributeInstance.GetFormattedValue(), attributeInstance.GetAttributeValueTooltip());
				}
			}
		}
		List<Descriptor> detailDescriptors = GameUtil.GetDetailDescriptors(GameUtil.GetAllDescriptors(targetEntity, false));
		for (int j = 0; j < detailDescriptors.Count; j++)
		{
			Descriptor descriptor = detailDescriptors[j];
			targetPanel.SetLabel("descriptor_" + j.ToString(), descriptor.text, descriptor.tooltipText);
		}
		targetPanel.Commit();
	}

	private static void RefreshDiseaseSourcePanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		List<Descriptor> list = GameUtil.GetAllDescriptors(targetEntity, true);
		Sicknesses sicknesses = targetEntity.GetSicknesses();
		if (sicknesses != null)
		{
			for (int i = 0; i < sicknesses.Count; i++)
			{
				list.AddRange(sicknesses[i].GetDescriptors());
			}
		}
		list = list.FindAll((Descriptor e) => e.type == Descriptor.DescriptorType.DiseaseSource);
		if (list.Count > 0)
		{
			for (int j = 0; j < list.Count; j++)
			{
				targetPanel.SetLabel("source_" + j.ToString(), list[j].text, list[j].tooltipText);
			}
		}
		targetPanel.Commit();
	}

	private static void RefreshCurrentGermsPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		if (targetEntity != null)
		{
			CellSelectionObject component = targetEntity.GetComponent<CellSelectionObject>();
			if (component != null)
			{
				if (component.diseaseIdx != 255 && component.diseaseCount > 0)
				{
					Disease disease = Db.Get().Diseases[(int)component.diseaseIdx];
					AdditionalDetailsPanel.BuildFactorsStrings(targetPanel, component.diseaseCount, component.element.idx, component.SelectedCell, component.Mass, component.temperature, null, disease, true);
				}
				else
				{
					targetPanel.SetLabel("currentgerms", UI.DETAILTABS.DISEASE.DETAILS.NODISEASE, UI.DETAILTABS.DISEASE.DETAILS.NODISEASE_TOOLTIP);
				}
			}
			else
			{
				PrimaryElement component2 = targetEntity.GetComponent<PrimaryElement>();
				if (component2 != null)
				{
					if (component2.DiseaseIdx != 255 && component2.DiseaseCount > 0)
					{
						Disease disease2 = Db.Get().Diseases[(int)component2.DiseaseIdx];
						int num = Grid.PosToCell(component2.transform.GetPosition());
						KPrefabID component3 = component2.GetComponent<KPrefabID>();
						AdditionalDetailsPanel.BuildFactorsStrings(targetPanel, component2.DiseaseCount, component2.Element.idx, num, component2.Mass, component2.Temperature, component3.Tags, disease2, false);
					}
					else
					{
						targetPanel.SetLabel("currentgerms", UI.DETAILTABS.DISEASE.DETAILS.NODISEASE, UI.DETAILTABS.DISEASE.DETAILS.NODISEASE_TOOLTIP);
					}
				}
			}
		}
		targetPanel.Commit();
	}

	private static void RefreshImuneSystemPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		GermExposureMonitor.Instance smi = targetEntity.GetSMI<GermExposureMonitor.Instance>();
		if (smi != null)
		{
			targetPanel.SetLabel("germ_resistance", Db.Get().Attributes.GermResistance.Name + ": " + smi.GetGermResistance().ToString(), DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.DESC);
			for (int i = 0; i < Db.Get().Diseases.Count; i++)
			{
				Disease disease = Db.Get().Diseases[i];
				ExposureType exposureTypeForDisease = GameUtil.GetExposureTypeForDisease(disease);
				Sickness sicknessForDisease = GameUtil.GetSicknessForDisease(disease);
				if (sicknessForDisease != null)
				{
					bool flag = true;
					List<string> list = new List<string>();
					if (exposureTypeForDisease.required_traits != null && exposureTypeForDisease.required_traits.Count > 0)
					{
						for (int j = 0; j < exposureTypeForDisease.required_traits.Count; j++)
						{
							if (!targetEntity.GetComponent<Traits>().HasTrait(exposureTypeForDisease.required_traits[j]))
							{
								list.Add(exposureTypeForDisease.required_traits[j]);
							}
						}
						if (list.Count > 0)
						{
							flag = false;
						}
					}
					bool flag2 = false;
					List<string> list2 = new List<string>();
					if (exposureTypeForDisease.excluded_effects != null && exposureTypeForDisease.excluded_effects.Count > 0)
					{
						for (int k = 0; k < exposureTypeForDisease.excluded_effects.Count; k++)
						{
							if (targetEntity.GetComponent<Effects>().HasEffect(exposureTypeForDisease.excluded_effects[k]))
							{
								list2.Add(exposureTypeForDisease.excluded_effects[k]);
							}
						}
						if (list2.Count > 0)
						{
							flag2 = true;
						}
					}
					bool flag3 = false;
					List<string> list3 = new List<string>();
					if (exposureTypeForDisease.excluded_traits != null && exposureTypeForDisease.excluded_traits.Count > 0)
					{
						for (int l = 0; l < exposureTypeForDisease.excluded_traits.Count; l++)
						{
							if (targetEntity.GetComponent<Traits>().HasTrait(exposureTypeForDisease.excluded_traits[l]))
							{
								list3.Add(exposureTypeForDisease.excluded_traits[l]);
							}
						}
						if (list3.Count > 0)
						{
							flag3 = true;
						}
					}
					string text = "";
					float num;
					if (!flag)
					{
						num = 0f;
						string text2 = "";
						for (int m = 0; m < list.Count; m++)
						{
							if (text2 != "")
							{
								text2 += ", ";
							}
							text2 += Db.Get().traits.Get(list[m]).Name;
						}
						text += string.Format(DUPLICANTS.DISEASES.IMMUNE_FROM_MISSING_REQUIRED_TRAIT, text2);
					}
					else if (flag3)
					{
						num = 0f;
						string text3 = "";
						for (int n = 0; n < list3.Count; n++)
						{
							if (text3 != "")
							{
								text3 += ", ";
							}
							text3 += Db.Get().traits.Get(list3[n]).Name;
						}
						if (text != "")
						{
							text += "\n";
						}
						text += string.Format(DUPLICANTS.DISEASES.IMMUNE_FROM_HAVING_EXLCLUDED_TRAIT, text3);
					}
					else if (flag2)
					{
						num = 0f;
						string text4 = "";
						for (int num2 = 0; num2 < list2.Count; num2++)
						{
							if (text4 != "")
							{
								text4 += ", ";
							}
							text4 += Db.Get().effects.Get(list2[num2]).Name;
						}
						if (text != "")
						{
							text += "\n";
						}
						text += string.Format(DUPLICANTS.DISEASES.IMMUNE_FROM_HAVING_EXCLUDED_EFFECT, text4);
					}
					else if (exposureTypeForDisease.infect_immediately)
					{
						num = 1f;
					}
					else
					{
						num = GermExposureMonitor.GetContractionChance(smi.GetResistanceToExposureType(exposureTypeForDisease, 3f));
					}
					string text5 = ((text != "") ? text : string.Format(DUPLICANTS.DISEASES.CONTRACTION_PROBABILITY, GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None), targetEntity.GetProperName(), sicknessForDisease.Name));
					targetPanel.SetLabel("disease_" + disease.Id, "    • " + disease.Name + ": " + GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None), string.Format(DUPLICANTS.DISEASES.RESISTANCES_PANEL_TOOLTIP, text5, sicknessForDisease.Name));
				}
			}
		}
		targetPanel.Commit();
	}

	private static string GetFormattedHalfLife(float hl)
	{
		return AdditionalDetailsPanel.GetFormattedGrowthRate(Disease.HalfLifeToGrowthRate(hl, 600f));
	}

	private static string GetFormattedGrowthRate(float rate)
	{
		if (rate < 1f)
		{
			return string.Format(UI.DETAILTABS.DISEASE.DETAILS.DEATH_FORMAT, GameUtil.GetFormattedPercent(100f * (1f - rate), GameUtil.TimeSlice.None), UI.DETAILTABS.DISEASE.DETAILS.DEATH_FORMAT_TOOLTIP);
		}
		if (rate > 1f)
		{
			return string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FORMAT, GameUtil.GetFormattedPercent(100f * (rate - 1f), GameUtil.TimeSlice.None), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FORMAT_TOOLTIP);
		}
		return string.Format(UI.DETAILTABS.DISEASE.DETAILS.NEUTRAL_FORMAT, UI.DETAILTABS.DISEASE.DETAILS.NEUTRAL_FORMAT_TOOLTIP);
	}

	private static string GetFormattedGrowthEntry(string name, float halfLife, string dyingFormat, string growingFormat, string neutralFormat)
	{
		string text;
		if (halfLife == float.PositiveInfinity)
		{
			text = neutralFormat;
		}
		else if (halfLife > 0f)
		{
			text = dyingFormat;
		}
		else
		{
			text = growingFormat;
		}
		return string.Format(text, name, AdditionalDetailsPanel.GetFormattedHalfLife(halfLife));
	}

	private static void BuildFactorsStrings(CollapsibleDetailContentPanel targetPanel, int diseaseCount, ushort elementIdx, int environmentCell, float environmentMass, float temperature, HashSet<Tag> tags, Disease disease, bool isCell = false)
	{
		targetPanel.SetTitle(string.Format(UI.DETAILTABS.DISEASE.CURRENT_GERMS, disease.Name.ToUpper()));
		targetPanel.SetLabel("currentgerms", string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT, disease.Name, GameUtil.GetFormattedDiseaseAmount(diseaseCount, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT_TOOLTIP, GameUtil.GetFormattedDiseaseAmount(diseaseCount, GameUtil.TimeSlice.None)));
		Element element = ElementLoader.elements[(int)elementIdx];
		CompositeGrowthRule growthRuleForElement = disease.GetGrowthRuleForElement(element);
		float num = 1f;
		if (tags != null && tags.Count > 0)
		{
			num = disease.GetGrowthRateForTags(tags, (float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass);
		}
		float num2 = DiseaseContainers.CalculateDelta(diseaseCount, elementIdx, environmentMass, environmentCell, temperature, num, disease, 1f, Sim.IsRadiationEnabled());
		targetPanel.SetLabel("finaldelta", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE, GameUtil.GetFormattedSimple(num2, GameUtil.TimeSlice.PerSecond, "F0")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE_TOOLTIP, GameUtil.GetFormattedSimple(num2, GameUtil.TimeSlice.PerSecond, "F0")));
		float num3 = Disease.GrowthRateToHalfLife(1f - num2 / (float)diseaseCount);
		if (num3 > 0f)
		{
			targetPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG, GameUtil.GetFormattedCycles(num3, "F1", false)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG_TOOLTIP, GameUtil.GetFormattedCycles(num3, "F1", false)));
		}
		else if (num3 < 0f)
		{
			targetPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS, GameUtil.GetFormattedCycles(-num3, "F1", false)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS_TOOLTIP, GameUtil.GetFormattedCycles(num3, "F1", false)));
		}
		else
		{
			targetPanel.SetLabel("finalhalflife", UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL_TOOLTIP);
		}
		targetPanel.SetLabel("factors", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TITLE, Array.Empty<object>()), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TOOLTIP);
		bool flag = false;
		if ((float)diseaseCount < growthRuleForElement.minCountPerKG * environmentMass)
		{
			targetPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TITLE, AdditionalDetailsPanel.GetFormattedGrowthRate(-growthRuleForElement.underPopulationDeathRate)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.minCountPerKG * environmentMass), GameUtil.TimeSlice.None), GameUtil.GetFormattedMass(environmentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), growthRuleForElement.minCountPerKG));
			flag = true;
		}
		else if ((float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass)
		{
			targetPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TITLE, AdditionalDetailsPanel.GetFormattedHalfLife(growthRuleForElement.overPopulationHalfLife)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.maxCountPerKG * environmentMass), GameUtil.TimeSlice.None), GameUtil.GetFormattedMass(environmentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), growthRuleForElement.maxCountPerKG));
			flag = true;
		}
		if (!flag)
		{
			targetPanel.SetLabel("substrate", AdditionalDetailsPanel.GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL), AdditionalDetailsPanel.GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
		}
		int num4 = 0;
		if (tags != null)
		{
			foreach (Tag tag in tags)
			{
				TagGrowthRule growthRuleForTag = disease.GetGrowthRuleForTag(tag);
				if (growthRuleForTag != null)
				{
					targetPanel.SetLabel("tag_" + num4.ToString(), AdditionalDetailsPanel.GetFormattedGrowthEntry(growthRuleForTag.Name(), growthRuleForTag.populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL), AdditionalDetailsPanel.GetFormattedGrowthEntry(growthRuleForTag.Name(), growthRuleForTag.populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
				}
				num4++;
			}
		}
		if (Grid.IsValidCell(environmentCell))
		{
			if (!isCell)
			{
				CompositeExposureRule exposureRuleForElement = disease.GetExposureRuleForElement(Grid.Element[environmentCell]);
				if (exposureRuleForElement != null && exposureRuleForElement.populationHalfLife != float.PositiveInfinity)
				{
					if (exposureRuleForElement.GetHalfLifeForCount(diseaseCount) > 0f)
					{
						targetPanel.SetLabel("environment", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), AdditionalDetailsPanel.GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount))), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.DIE_TOOLTIP);
					}
					else
					{
						targetPanel.SetLabel("environment", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), AdditionalDetailsPanel.GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount))), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.GROW_TOOLTIP);
					}
				}
			}
			if (Sim.IsRadiationEnabled())
			{
				float num5 = Grid.Radiation[environmentCell];
				if (num5 > 0f)
				{
					float num6 = disease.radiationKillRate * num5;
					float num7 = (float)diseaseCount * 0.5f / num6;
					targetPanel.SetLabel("radiation", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RADIATION.TITLE, Mathf.RoundToInt(num5), AdditionalDetailsPanel.GetFormattedHalfLife(num7)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RADIATION.DIE_TOOLTIP);
				}
			}
		}
		float num8 = disease.CalculateTemperatureHalfLife(temperature);
		if (num8 != float.PositiveInfinity)
		{
			if (num8 > 0f)
			{
				targetPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), AdditionalDetailsPanel.GetFormattedHalfLife(num8)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.DIE_TOOLTIP);
				return;
			}
			targetPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), AdditionalDetailsPanel.GetFormattedHalfLife(num8)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.GROW_TOOLTIP);
		}
	}

	private static void RefreshEnergyOverviewPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		if (targetEntity == null)
		{
			return;
		}
		if (targetEntity.GetComponent<ICircuitConnected>() != null || targetEntity.GetComponent<Wire>() != null)
		{
			ushort selectedTargetCircuitID = AdditionalDetailsPanel.GetSelectedTargetCircuitID(targetEntity);
			if (selectedTargetCircuitID == 65535)
			{
				targetPanel.SetLabel("nocircuit", UI.DETAILTABS.ENERGYGENERATOR.DISCONNECTED, UI.DETAILTABS.ENERGYGENERATOR.DISCONNECTED);
			}
			else
			{
				float joulesAvailableOnCircuit = Game.Instance.circuitManager.GetJoulesAvailableOnCircuit(selectedTargetCircuitID);
				targetPanel.SetLabel("joulesAvailable", string.Format(UI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES, GameUtil.GetFormattedJoules(joulesAvailableOnCircuit, "F1", GameUtil.TimeSlice.None)), UI.DETAILTABS.ENERGYGENERATOR.AVAILABLE_JOULES_TOOLTIP);
				float wattsGeneratedByCircuit = Game.Instance.circuitManager.GetWattsGeneratedByCircuit(selectedTargetCircuitID);
				float potentialWattsGeneratedByCircuit = Game.Instance.circuitManager.GetPotentialWattsGeneratedByCircuit(selectedTargetCircuitID);
				string text;
				if (wattsGeneratedByCircuit == potentialWattsGeneratedByCircuit)
				{
					text = GameUtil.GetFormattedWattage(wattsGeneratedByCircuit, GameUtil.WattageFormatterUnit.Automatic, true);
				}
				else
				{
					text = string.Format("{0} / {1}", GameUtil.GetFormattedWattage(wattsGeneratedByCircuit, GameUtil.WattageFormatterUnit.Automatic, true), GameUtil.GetFormattedWattage(potentialWattsGeneratedByCircuit, GameUtil.WattageFormatterUnit.Automatic, true));
				}
				targetPanel.SetLabel("wattageGenerated", string.Format(UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED, text), UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_GENERATED_TOOLTIP);
				targetPanel.SetLabel("wattageConsumed", string.Format(UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(Game.Instance.circuitManager.GetWattsUsedByCircuit(selectedTargetCircuitID), GameUtil.WattageFormatterUnit.Automatic, true)), UI.DETAILTABS.ENERGYGENERATOR.WATTAGE_CONSUMED_TOOLTIP);
				targetPanel.SetLabel("potentialWattageConsumed", string.Format(UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED, GameUtil.GetFormattedWattage(Game.Instance.circuitManager.GetWattsNeededWhenActive(selectedTargetCircuitID), GameUtil.WattageFormatterUnit.Automatic, true)), UI.DETAILTABS.ENERGYGENERATOR.POTENTIAL_WATTAGE_CONSUMED_TOOLTIP);
				targetPanel.SetLabel("maxSafeWattage", string.Format(UI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE, GameUtil.GetFormattedWattage(Game.Instance.circuitManager.GetMaxSafeWattageForCircuit(selectedTargetCircuitID), GameUtil.WattageFormatterUnit.Automatic, true)), UI.DETAILTABS.ENERGYGENERATOR.MAX_SAFE_WATTAGE_TOOLTIP);
			}
		}
		targetPanel.Commit();
	}

	private static void RefreshEnergyGeneratorPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		if (targetEntity == null)
		{
			return;
		}
		ushort selectedTargetCircuitID = AdditionalDetailsPanel.GetSelectedTargetCircuitID(targetEntity);
		if (selectedTargetCircuitID == 65535)
		{
			targetPanel.SetActive(false);
			return;
		}
		targetPanel.SetActive(true);
		List<Generator> generatorsOnCircuit = Game.Instance.circuitManager.GetGeneratorsOnCircuit(selectedTargetCircuitID);
		if (generatorsOnCircuit.Count > 0)
		{
			using (List<Generator>.Enumerator enumerator = generatorsOnCircuit.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Generator generator = enumerator.Current;
					if (generator != null && generator.GetComponent<Battery>() == null)
					{
						string text;
						if (generator.IsProducingPower())
						{
							text = string.Format("{0}: {1}", generator.GetComponent<KSelectable>().entityName, GameUtil.GetFormattedWattage(generator.WattageRating, GameUtil.WattageFormatterUnit.Automatic, true));
						}
						else
						{
							text = string.Format("{0}: {1} / {2}", generator.GetComponent<KSelectable>().entityName, GameUtil.GetFormattedWattage(0f, GameUtil.WattageFormatterUnit.Automatic, true), GameUtil.GetFormattedWattage(generator.WattageRating, GameUtil.WattageFormatterUnit.Automatic, true));
						}
						text = ((generator.gameObject == targetEntity) ? ("<b>" + text + "</b>") : text);
						targetPanel.SetLabel(generator.gameObject.GetInstanceID().ToString(), text, "");
					}
				}
				goto IL_0157;
			}
		}
		targetPanel.SetLabel("nogenerators", UI.DETAILTABS.ENERGYGENERATOR.NOGENERATORS, "");
		IL_0157:
		targetPanel.Commit();
	}

	private static void RefreshEnergyConsumerPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		AdditionalDetailsPanel.<>c__DisplayClass27_0 CS$<>8__locals1;
		CS$<>8__locals1.targetEntity = targetEntity;
		CS$<>8__locals1.targetPanel = targetPanel;
		if (CS$<>8__locals1.targetEntity == null)
		{
			return;
		}
		ushort selectedTargetCircuitID = AdditionalDetailsPanel.GetSelectedTargetCircuitID(CS$<>8__locals1.targetEntity);
		if (selectedTargetCircuitID == 65535)
		{
			CS$<>8__locals1.targetPanel.SetActive(false);
			return;
		}
		CS$<>8__locals1.targetPanel.SetActive(true);
		List<IEnergyConsumer> consumersOnCircuit = Game.Instance.circuitManager.GetConsumersOnCircuit(selectedTargetCircuitID);
		List<Battery> transformersOnCircuit = Game.Instance.circuitManager.GetTransformersOnCircuit(selectedTargetCircuitID);
		if (consumersOnCircuit.Count > 0 || transformersOnCircuit.Count > 0)
		{
			foreach (IEnergyConsumer energyConsumer in consumersOnCircuit)
			{
				AdditionalDetailsPanel.<RefreshEnergyConsumerPanel>g__AddConsumerInfo|27_0(energyConsumer, ref CS$<>8__locals1);
			}
			using (List<Battery>.Enumerator enumerator2 = transformersOnCircuit.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					Battery battery = enumerator2.Current;
					AdditionalDetailsPanel.<RefreshEnergyConsumerPanel>g__AddConsumerInfo|27_0(battery, ref CS$<>8__locals1);
				}
				goto IL_0101;
			}
		}
		CS$<>8__locals1.targetPanel.SetLabel("noconsumers", UI.DETAILTABS.ENERGYGENERATOR.NOCONSUMERS, "");
		IL_0101:
		CS$<>8__locals1.targetPanel.Commit();
	}

	private static void RefreshEnergyBatteriesPanel(CollapsibleDetailContentPanel targetPanel, GameObject targetEntity)
	{
		if (targetEntity == null)
		{
			return;
		}
		ushort selectedTargetCircuitID = AdditionalDetailsPanel.GetSelectedTargetCircuitID(targetEntity);
		if (selectedTargetCircuitID == 65535)
		{
			targetPanel.SetActive(false);
			return;
		}
		targetPanel.SetActive(true);
		List<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(selectedTargetCircuitID);
		if (batteriesOnCircuit.Count > 0)
		{
			using (List<Battery>.Enumerator enumerator = batteriesOnCircuit.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Battery battery = enumerator.Current;
					if (battery != null)
					{
						string text = string.Format("{0}: {1}", battery.GetComponent<KSelectable>().entityName, GameUtil.GetFormattedJoules(battery.JoulesAvailable, "F1", GameUtil.TimeSlice.None));
						text = ((battery.gameObject == targetEntity) ? ("<b>" + text + "</b>") : text);
						targetPanel.SetLabel(battery.gameObject.GetInstanceID().ToString(), text, "");
					}
				}
				goto IL_0103;
			}
		}
		targetPanel.SetLabel("nobatteries", UI.DETAILTABS.ENERGYGENERATOR.NOBATTERIES, "");
		IL_0103:
		targetPanel.Commit();
	}

	private static ushort GetSelectedTargetCircuitID(GameObject targetEntity)
	{
		CircuitManager circuitManager = Game.Instance.circuitManager;
		ICircuitConnected component = targetEntity.GetComponent<ICircuitConnected>();
		ushort num = ushort.MaxValue;
		if (component != null)
		{
			num = Game.Instance.circuitManager.GetCircuitID(component);
		}
		else if (targetEntity.GetComponent<Wire>() != null)
		{
			int num2 = Grid.PosToCell(targetEntity.transform.GetPosition());
			num = Game.Instance.circuitManager.GetCircuitID(num2);
		}
		return num;
	}

	[CompilerGenerated]
	internal static void <RefreshEnergyConsumerPanel>g__AddConsumerInfo|27_0(IEnergyConsumer consumer, ref AdditionalDetailsPanel.<>c__DisplayClass27_0 A_1)
	{
		KMonoBehaviour kmonoBehaviour = consumer as KMonoBehaviour;
		if (kmonoBehaviour != null)
		{
			float wattsUsed = consumer.WattsUsed;
			float wattsNeededWhenActive = consumer.WattsNeededWhenActive;
			string text;
			if (wattsUsed == wattsNeededWhenActive)
			{
				text = GameUtil.GetFormattedWattage(wattsUsed, GameUtil.WattageFormatterUnit.Automatic, true);
			}
			else
			{
				text = string.Format("{0} / {1}", GameUtil.GetFormattedWattage(wattsUsed, GameUtil.WattageFormatterUnit.Automatic, true), GameUtil.GetFormattedWattage(wattsNeededWhenActive, GameUtil.WattageFormatterUnit.Automatic, true));
			}
			string text2 = string.Format("{0}: {1}", consumer.Name, text);
			text2 = ((kmonoBehaviour.gameObject == A_1.targetEntity) ? ("<b>" + text2 + "</b>") : text2);
			A_1.targetPanel.SetLabel(kmonoBehaviour.gameObject.GetInstanceID().ToString(), text2, "");
		}
	}

	public GameObject attributesLabelTemplate;

	private CollapsibleDetailContentPanel detailsPanel;

	private DetailsPanelDrawer drawer;

	private CollapsibleDetailContentPanel immuneSystemPanel;

	private CollapsibleDetailContentPanel diseaseSourcePanel;

	private CollapsibleDetailContentPanel currentGermsPanel;

	private CollapsibleDetailContentPanel overviewPanel;

	private CollapsibleDetailContentPanel generatorsPanel;

	private CollapsibleDetailContentPanel consumersPanel;

	private CollapsibleDetailContentPanel batteriesPanel;

	private static readonly EventSystem.IntraObjectHandler<AdditionalDetailsPanel> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<AdditionalDetailsPanel>(delegate(AdditionalDetailsPanel component, object data)
	{
		component.OnRefreshData(data);
	});
}
