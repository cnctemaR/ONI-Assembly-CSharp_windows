using System;
using System.Collections.Generic;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using STRINGS;
using UnityEngine;

public class DiseaseInfoScreen : TargetScreen
{
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
		base.Subscribe(-1514841199, new Action<object>(this.OnRefreshData));
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
		Diseases diseases = this.selectedTarget.GetDiseases();
		if (diseases != null)
		{
			for (int i = 0; i < diseases.Count; i++)
			{
				list.AddRange(diseases[i].GetDescriptors());
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
		if (this.CreateImmuneInfo())
		{
		}
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
		ImmuneSystemMonitor.Instance smi = this.selectedTarget.GetSMI<ImmuneSystemMonitor.Instance>();
		if (smi != null)
		{
			for (int i = 0; i < Db.Get().Diseases.Count; i++)
			{
				Disease disease = Db.Get().Diseases[i];
				AmountInstance amountInstance = disease.amount.Lookup(this.selectedTarget);
				if (amountInstance.value > 0f)
				{
					this.immuneSystemPanel.SetLabelWithButton("disease_" + disease.Id, string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.INTERNAL_GERMS, disease.Name, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance.value))), string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.INTERNAL_GERMS_TOOLTIP, disease.Name, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance.value))), UI.DETAILTABS.DISEASE.DISEASE_INFO_POPUP_BUTTON, string.Format(UI.DETAILTABS.DISEASE.DISEASE_INFO_POPUP_TOOLTIP, disease.Name), delegate
					{
						this.ShowDiseaseInfoPopup(disease);
					});
					AttributeModifier currentImmuneModifier = smi.GetCurrentImmuneModifier(disease);
					if (currentImmuneModifier != null)
					{
						this.immuneSystemPanel.SetLabel("disease_rate2_" + disease.Id, string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.IMMUNE_ATTACK_RATE2, currentImmuneModifier.GetFormattedString(this.selectedTarget), GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance.value))), string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.IMMUNE_ATTACK_RATE2_TOOLTIP, currentImmuneModifier.GetFormattedString(this.selectedTarget), GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance.value))));
					}
				}
			}
			return true;
		}
		return false;
	}

	private bool CreateDiseaseInfo()
	{
		PrimaryElement component = this.selectedTarget.GetComponent<PrimaryElement>();
		if (component != null)
		{
			return this.CreateDiseaseInfo_PrimaryElement();
		}
		CellSelectionObject component2 = this.selectedTarget.GetComponent<CellSelectionObject>();
		return component2 != null && this.CreateDiseaseInfo_CellSelectionObject(component2);
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

	private void BuildFactorsStrings(int diseaseCount, int elementIdx, int environmentCell, float environmentMass, float temperature, Tag[] tags, Disease disease)
	{
		this.currentGermsPanel.SetTitle(string.Format(UI.DETAILTABS.DISEASE.CURRENT_GERMS, disease.Name.ToUpper()));
		this.currentGermsPanel.SetLabelWithButton("currentgerms", string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT, disease.Name, GameUtil.GetFormattedDiseaseAmount(diseaseCount)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT_TOOLTIP, GameUtil.GetFormattedDiseaseAmount(diseaseCount)), UI.DETAILTABS.DISEASE.DISEASE_INFO_POPUP_BUTTON, string.Format(UI.DETAILTABS.DISEASE.DISEASE_INFO_POPUP_TOOLTIP, disease.Name), delegate
		{
			this.ShowDiseaseInfoPopup(disease);
		});
		Element element = ElementLoader.elements[elementIdx];
		CompositeGrowthRule growthRuleForElement = disease.GetGrowthRuleForElement(element);
		float num = 1f;
		if (tags != null && tags.Length > 0)
		{
			num = disease.GetGrowthRateForTags(tags, (float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass);
		}
		float num2 = DiseaseContainers.CalculateDelta(diseaseCount, elementIdx, environmentMass, environmentCell, temperature, num, disease, 1f);
		this.currentGermsPanel.SetLabel("finaldelta", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE, GameUtil.GetFormattedSimple(num2, GameUtil.TimeSlice.PerSecond, "F0")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE_TOOLTIP, GameUtil.GetFormattedSimple(num2, GameUtil.TimeSlice.PerSecond, "F0")));
		float num3 = Disease.GrowthRateToHalfLife(1f - num2 / (float)diseaseCount);
		if (num3 > 0f)
		{
			this.currentGermsPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG, GameUtil.GetFormattedCycles(num3, "F1")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG_TOOLTIP, GameUtil.GetFormattedCycles(num3, "F1")));
		}
		else if (num3 < 0f)
		{
			this.currentGermsPanel.SetLabel("finalhalflife", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS, GameUtil.GetFormattedCycles(-num3, "F1")), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS_TOOLTIP, GameUtil.GetFormattedCycles(num3, "F1")));
		}
		else
		{
			this.currentGermsPanel.SetLabel("finalhalflife", UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL_TOOLTIP);
		}
		this.currentGermsPanel.SetLabel("factors", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TITLE, new object[0]), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TOOLTIP);
		bool flag = false;
		if ((float)diseaseCount < growthRuleForElement.minCountPerKG * environmentMass)
		{
			this.currentGermsPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TITLE, this.GetFormattedGrowthRate(growthRuleForElement.underPopulationDeathRate)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.DYING_OFF.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.minCountPerKG * environmentMass)), GameUtil.GetFormattedMass(environmentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), growthRuleForElement.minCountPerKG));
			flag = true;
		}
		else if ((float)diseaseCount > growthRuleForElement.maxCountPerKG * environmentMass)
		{
			this.currentGermsPanel.SetLabel("critical_status", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TITLE, this.GetFormattedHalfLife(growthRuleForElement.overPopulationHalfLife)), string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.OVERPOPULATED.TOOLTIP, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(growthRuleForElement.maxCountPerKG * environmentMass)), GameUtil.GetFormattedMass(environmentMass, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"), growthRuleForElement.maxCountPerKG));
			flag = true;
		}
		if (!flag)
		{
			this.currentGermsPanel.SetLabel("substrate", this.GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL), this.GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
		}
		for (int i = 0; i < tags.Length; i++)
		{
			TagGrowthRule growthRuleForTag = disease.GetGrowthRuleForTag(tags[i]);
			if (growthRuleForTag != null)
			{
				CollapsibleDetailContentPanel collapsibleDetailContentPanel = this.currentGermsPanel;
				string text = "tag_" + i;
				string text2 = growthRuleForTag.Name();
				float? populationHalfLife = growthRuleForTag.populationHalfLife;
				string formattedGrowthEntry = this.GetFormattedGrowthEntry(text2, populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL);
				string text3 = growthRuleForTag.Name();
				float? populationHalfLife2 = growthRuleForTag.populationHalfLife;
				collapsibleDetailContentPanel.SetLabel(text, formattedGrowthEntry, this.GetFormattedGrowthEntry(text3, populationHalfLife2.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP));
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
		float num4 = disease.CalculateTemperatureHalfLife(temperature);
		if (num4 != float.PositiveInfinity)
		{
			if (num4 > 0f)
			{
				this.currentGermsPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), this.GetFormattedHalfLife(num4)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.DIE_TOOLTIP);
			}
			else
			{
				this.currentGermsPanel.SetLabel("temperature", string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), this.GetFormattedHalfLife(num4)), UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.GROW_TOOLTIP);
			}
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
			this.BuildFactorsStrings(component.DiseaseCount, ElementLoader.GetElementIndex(component.Element.id), num, component.Mass, component.Temperature, component2.Tags, disease);
			return true;
		}
		return false;
	}

	private bool CreateDiseaseInfo_CellSelectionObject(CellSelectionObject cso)
	{
		if (cso.diseaseIdx != 255 && cso.diseaseCount > 0)
		{
			Disease disease = Db.Get().Diseases[(int)cso.diseaseIdx];
			int elementIndex = ElementLoader.GetElementIndex(cso.element.id);
			this.BuildFactorsStrings(cso.diseaseCount, elementIndex, -1, cso.Mass, cso.temperature, new Tag[0], disease);
			return true;
		}
		return false;
	}

	private void ShowDiseaseInfoPopup(Disease disease)
	{
		InfoDialogScreen infoDialogScreen = (InfoDialogScreen)GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.InfoDialogScreen.gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay);
		infoDialogScreen.SetHeader(string.Format(UI.DETAILTABS.DISEASE.DISEASE_INFO_POPUP_HEADER, disease.Name.ToUpper()));
		infoDialogScreen.AddSubHeader(UI.DETAILTABS.DISEASE.GERMS_INFO);
		List<Descriptor> quantitativeDescriptors = disease.GetQuantitativeDescriptors();
		for (int i = 0; i < quantitativeDescriptors.Count; i++)
		{
			infoDialogScreen.AddLineItem(quantitativeDescriptors[i].IndentedText(), quantitativeDescriptors[i].tooltipText);
		}
		infoDialogScreen.AddSubHeader(UI.DETAILTABS.DISEASE.INFECTION_INFO);
		infoDialogScreen.AddPlainText(UI.DETAILTABS.DISEASE.INFECTION.DISCLAIMER);
		infoDialogScreen.AddLineItem(UI.DETAILTABS.DISEASE.INFECTION.DURATION, UI.DETAILTABS.DISEASE.INFECTION.DURATION_TOOLTIP);
		if (disease.doctorRequired)
		{
			infoDialogScreen.AddLineItem(string.Format(UI.DETAILTABS.DISEASE.INFECTION.DURATION_AIDREQ, GameUtil.GetFormattedCycles(disease.SicknessDuration, "F1")), string.Format(UI.DETAILTABS.DISEASE.INFECTION.DURATION_AIDREQ_TOOLTIP, GameUtil.GetFormattedCycles(disease.SicknessDuration, "F1")));
		}
		else
		{
			infoDialogScreen.AddLineItem(string.Format(UI.DETAILTABS.DISEASE.INFECTION.DURATION_NORMAL, GameUtil.GetFormattedCycles(disease.SicknessDuration, "F1")), string.Format(UI.DETAILTABS.DISEASE.INFECTION.DURATION_NORMAL_TOOLTIP, GameUtil.GetFormattedCycles(disease.SicknessDuration, "F1")));
		}
		List<Descriptor> symptoms = disease.GetSymptoms();
		GameUtil.IndentListOfDescriptors(symptoms);
		List<Descriptor> list = symptoms.FindAll((Descriptor d) => d.type == Descriptor.DescriptorType.SymptomAidable);
		if (list.Count > 0)
		{
			infoDialogScreen.AddLineItem(UI.DETAILTABS.DISEASE.INFECTION.AID_SYMPTOMS, UI.DETAILTABS.DISEASE.INFECTION.AID_SYMPTOMS_TOOLTIP);
			for (int j = 0; j < list.Count; j++)
			{
				infoDialogScreen.AddLineItem(list[j].IndentedText(), list[j].tooltipText);
			}
		}
		List<Descriptor> list2 = symptoms.FindAll((Descriptor d) => d.type == Descriptor.DescriptorType.Symptom);
		if (list.Count > 0)
		{
			infoDialogScreen.AddLineItem(UI.DETAILTABS.DISEASE.INFECTION.SYMPTOMS, UI.DETAILTABS.DISEASE.INFECTION.SYMPTOMS_TOOLTIP);
			for (int k = 0; k < list2.Count; k++)
			{
				infoDialogScreen.AddLineItem(list2[k].IndentedText(), list2[k].tooltipText);
			}
		}
	}

	private CollapsibleDetailContentPanel infectionPanel;

	private CollapsibleDetailContentPanel immuneSystemPanel;

	private CollapsibleDetailContentPanel diseaseSourcePanel;

	private CollapsibleDetailContentPanel currentGermsPanel;

	private CollapsibleDetailContentPanel infoPanel;
}
