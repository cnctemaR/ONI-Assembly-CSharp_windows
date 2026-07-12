using System;
using System.Collections.Generic;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using UnityEngine;

public class DiseaseInfoScreen : TargetScreen
{
	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<CellSelectionObject>() != null || target.GetComponent<PrimaryElement>() != null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.diseaseSourcePanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		this.diseaseSourcePanel.SetTitle(UI.DETAILTABS.DISEASE.DISEASE_SOURCE);
		this.immuneSystemPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		this.immuneSystemPanel.SetTitle(UI.DETAILTABS.DISEASE.IMMUNE_SYSTEM);
		this.currentGermsPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		this.currentGermsPanel.SetTitle(UI.DETAILTABS.DISEASE.CURRENT_GERMS);
		this.infoPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		this.infoPanel.SetTitle(UI.DETAILTABS.DISEASE.GERMS_INFO);
		this.infectionPanel = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, base.gameObject, false).GetComponent<CollapsibleDetailContentPanel>();
		this.infectionPanel.SetTitle(UI.DETAILTABS.DISEASE.INFECTION_INFO);
		base.Subscribe<DiseaseInfoScreen>(-1514841199, DiseaseInfoScreen.OnRefreshDataDelegate);
	}

	private void LateUpdate()
	{
		this.Refresh();
	}

	private void OnRefreshData(object obj)
	{
		this.Refresh();
	}

	private void Refresh()
	{
		if (this.selectedTarget == null)
		{
			return;
		}
		List<Descriptor> list = GameUtil.GetAllDescriptors(this.selectedTarget, true);
		Sicknesses sicknesses = this.selectedTarget.GetSicknesses();
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
				this.diseaseSourcePanel.SetLabel("source_" + j.ToString(), list[j].text, list[j].tooltipText);
			}
		}
		this.CreateImmuneInfo();
		if (!this.CreateDiseaseInfo())
		{
			this.currentGermsPanel.SetTitle(UI.DETAILTABS.DISEASE.NO_CURRENT_GERMS);
			this.currentGermsPanel.SetLabel("nodisease", UI.DETAILTABS.DISEASE.DETAILS.NODISEASE, UI.DETAILTABS.DISEASE.DETAILS.NODISEASE_TOOLTIP);
		}
		this.diseaseSourcePanel.Commit();
		this.immuneSystemPanel.Commit();
		this.currentGermsPanel.Commit();
		this.infoPanel.Commit();
		this.infectionPanel.Commit();
	}

	private bool CreateImmuneInfo()
	{
		GermExposureMonitor.Instance smi = this.selectedTarget.GetSMI<GermExposureMonitor.Instance>();
		if (smi != null)
		{
			this.immuneSystemPanel.SetTitle(UI.DETAILTABS.DISEASE.CONTRACTION_RATES);
			this.immuneSystemPanel.SetLabel("germ_resistance", Db.Get().Attributes.GermResistance.Name + ": " + smi.GetGermResistance().ToString(), DUPLICANTS.ATTRIBUTES.GERMRESISTANCE.DESC);
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
							if (!this.selectedTarget.GetComponent<Traits>().HasTrait(exposureTypeForDisease.required_traits[j]))
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
							if (this.selectedTarget.GetComponent<Effects>().HasEffect(exposureTypeForDisease.excluded_effects[k]))
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
							if (this.selectedTarget.GetComponent<Traits>().HasTrait(exposureTypeForDisease.excluded_traits[l]))
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
					string text5 = ((text != "") ? text : string.Format(DUPLICANTS.DISEASES.CONTRACTION_PROBABILITY, GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None), this.selectedTarget.GetProperName(), sicknessForDisease.Name));
					this.immuneSystemPanel.SetLabel("disease_" + disease.Id, "    • " + disease.Name + ": " + GameUtil.GetFormattedPercent(num * 100f, GameUtil.TimeSlice.None), string.Format(DUPLICANTS.DISEASES.RESISTANCES_PANEL_TOOLTIP, text5, sicknessForDisease.Name));
				}
			}
			return true;
		}
		return false;
	}

	private bool CreateDiseaseInfo()
	{
		if (this.selectedTarget.GetComponent<PrimaryElement>() != null)
		{
			return this.CreateDiseaseInfo_PrimaryElement();
		}
		CellSelectionObject component = this.selectedTarget.GetComponent<CellSelectionObject>();
		return component != null && this.CreateDiseaseInfo_CellSelectionObject(component);
	}

	private string GetFormattedHalfLife(float hl)
	{
		return this.GetFormattedGrowthRate(Disease.HalfLifeToGrowthRate(hl, 600f));
	}

	private string GetFormattedGrowthRate(float rate)
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

	private string GetFormattedGrowthEntry(string name, float halfLife, string dyingFormat, string growingFormat, string neutralFormat)
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
		return string.Format(text, name, this.GetFormattedHalfLife(halfLife));
	}

	private void BuildFactorsStrings(int diseaseCount, int elementIdx, int environmentCell, float environmentMass, float temperature, HashSet<Tag> tags, Disease disease)
	{
		this.currentGermsPanel.SetTitle(string.Format(UI.DETAILTABS.DISEASE.CURRENT_GERMS, disease.Name.ToUpper()));
		this.currentGermsPanel.SetLabel("currentgerms", string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT, disease.Name, GameUtil.GetFormattedDiseaseAmount(diseaseCount, GameUtil.TimeSlice.None)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT_TOOLTIP, GameUtil.GetFormattedDiseaseAmount(diseaseCount, GameUtil.TimeSlice.None)));
		Element element = ElementLoader.elements[elementIdx];
		CompositeGrowthRule growthRuleForElement = disease.GetGrowthRuleForElement(element);
		float num = 1f;
		if (tags != null && tags.Count > 0)
		{
			num = disease.GetGrowthRateForTags(tags, (float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass);
		}
		float num2 = DiseaseContainers.CalculateDelta(diseaseCount, elementIdx, environmentMass, environmentCell, temperature, num, disease, 1f, Sim.IsRadiationEnabled());
		this.currentGermsPanel.SetLabel("finaldelta", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE, GameUtil.GetFormattedSimple(num2, GameUtil.TimeSlice.PerSecond, "F0")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE_TOOLTIP, GameUtil.GetFormattedSimple(num2, GameUtil.TimeSlice.PerSecond, "F0")));
		float num3 = Disease.GrowthRateToHalfLife(1f - num2 / (float)diseaseCount);
		if (num3 > 0f)
		{
			this.currentGermsPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG, GameUtil.GetFormattedCycles(num3, "F1", false)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG_TOOLTIP, GameUtil.GetFormattedCycles(num3, "F1", false)));
		}
		else if (num3 < 0f)
		{
			this.currentGermsPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS, GameUtil.GetFormattedCycles(-num3, "F1", false)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS_TOOLTIP, GameUtil.GetFormattedCycles(num3, "F1", false)));
		}
		else
		{
			this.currentGermsPanel.SetLabel("finalhalflife", UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL_TOOLTIP);
		}
		this.currentGermsPanel.SetLabel("factors", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TITLE, Array.Empty<object>()), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TOOLTIP);
		bool flag = false;
		if ((float)diseaseCount < growthRuleForElement.minCountPerKG * environmentMass)
		{
			this.currentGermsPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TITLE, this.GetFormattedGrowthRate(-growthRuleForElement.underPopulationDeathRate)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.minCountPerKG * environmentMass), GameUtil.TimeSlice.None), GameUtil.GetFormattedMass(environmentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), growthRuleForElement.minCountPerKG));
			flag = true;
		}
		else if ((float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass)
		{
			this.currentGermsPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TITLE, this.GetFormattedHalfLife(growthRuleForElement.overPopulationHalfLife)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.maxCountPerKG * environmentMass), GameUtil.TimeSlice.None), GameUtil.GetFormattedMass(environmentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), growthRuleForElement.maxCountPerKG));
			flag = true;
		}
		if (!flag)
		{
			this.currentGermsPanel.SetLabel("substrate", this.GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL), this.GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
		}
		int num4 = 0;
		if (tags != null)
		{
			foreach (Tag tag in tags)
			{
				TagGrowthRule growthRuleForTag = disease.GetGrowthRuleForTag(tag);
				if (growthRuleForTag != null)
				{
					this.currentGermsPanel.SetLabel("tag_" + num4.ToString(), this.GetFormattedGrowthEntry(growthRuleForTag.Name(), growthRuleForTag.populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL), this.GetFormattedGrowthEntry(growthRuleForTag.Name(), growthRuleForTag.populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
				}
				num4++;
			}
		}
		if (Grid.IsValidCell(environmentCell))
		{
			CompositeExposureRule exposureRuleForElement = disease.GetExposureRuleForElement(Grid.Element[environmentCell]);
			if (exposureRuleForElement != null && exposureRuleForElement.populationHalfLife != float.PositiveInfinity)
			{
				if (exposureRuleForElement.GetHalfLifeForCount(diseaseCount) > 0f)
				{
					this.currentGermsPanel.SetLabel("environment", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), this.GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount))), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.DIE_TOOLTIP);
				}
				else
				{
					this.currentGermsPanel.SetLabel("environment", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), this.GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount))), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.GROW_TOOLTIP);
				}
			}
		}
		float num5 = disease.CalculateTemperatureHalfLife(temperature);
		if (num5 != float.PositiveInfinity)
		{
			if (num5 > 0f)
			{
				this.currentGermsPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), this.GetFormattedHalfLife(num5)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.DIE_TOOLTIP);
				return;
			}
			this.currentGermsPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false), this.GetFormattedHalfLife(num5)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.GROW_TOOLTIP);
		}
	}

	private bool CreateDiseaseInfo_PrimaryElement()
	{
		if (this.selectedTarget == null)
		{
			return false;
		}
		PrimaryElement component = this.selectedTarget.GetComponent<PrimaryElement>();
		if (component == null)
		{
			return false;
		}
		if (component.DiseaseIdx != 255 && component.DiseaseCount > 0)
		{
			Disease disease = Db.Get().Diseases[(int)component.DiseaseIdx];
			int num = Grid.PosToCell(component.transform.GetPosition());
			KPrefabID component2 = component.GetComponent<KPrefabID>();
			this.BuildFactorsStrings(component.DiseaseCount, (int)component.Element.idx, num, component.Mass, component.Temperature, component2.Tags, disease);
			return true;
		}
		return false;
	}

	private bool CreateDiseaseInfo_CellSelectionObject(CellSelectionObject cso)
	{
		if (cso.diseaseIdx != 255 && cso.diseaseCount > 0)
		{
			Disease disease = Db.Get().Diseases[(int)cso.diseaseIdx];
			int idx = (int)cso.element.idx;
			this.BuildFactorsStrings(cso.diseaseCount, idx, -1, cso.Mass, cso.temperature, null, disease);
			return true;
		}
		return false;
	}

	private CollapsibleDetailContentPanel infectionPanel;

	private CollapsibleDetailContentPanel immuneSystemPanel;

	private CollapsibleDetailContentPanel diseaseSourcePanel;

	private CollapsibleDetailContentPanel currentGermsPanel;

	private CollapsibleDetailContentPanel infoPanel;

	private static readonly EventSystem.IntraObjectHandler<DiseaseInfoScreen> OnRefreshDataDelegate = new EventSystem.IntraObjectHandler<DiseaseInfoScreen>(delegate(DiseaseInfoScreen component, object data)
	{
		component.OnRefreshData(data);
	});
}
