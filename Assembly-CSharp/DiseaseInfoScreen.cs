using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using TMPro;
using UnityEngine;

public class DiseaseInfoScreen : TargetScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.diseaseSourcePanel = new DiseaseInfoScreen.InfoPanel(UI.DETAILTABS.DISEASE.DISEASE_SOURCE, this.labelTemplate, base.gameObject);
		this.immuneSystemPanel = new DiseaseInfoScreen.InfoPanel(UI.DETAILTABS.DISEASE.IMMUNE_SYSTEM, this.labelTemplate, base.gameObject);
		this.currentGermsPanel = new DiseaseInfoScreen.InfoPanel(UI.DETAILTABS.DISEASE.CURRENT_GERMS, this.labelTemplate, base.gameObject);
		this.infoPanel = new DiseaseInfoScreen.InfoPanel(UI.DETAILTABS.DISEASE.GERMS_INFO, this.labelTemplate, base.gameObject);
		this.infectionPanel = new DiseaseInfoScreen.InfoPanel(UI.DETAILTABS.DISEASE.INFECTION_INFO, this.labelTemplate, base.gameObject);
		this.Subscribe(-1514841199, new Action<object>(this.OnRefreshData));
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
		this.diseaseSourcePanel.DeactivateAll();
		this.immuneSystemPanel.DeactivateAll();
		this.currentGermsPanel.DeactivateAll();
		this.infoPanel.DeactivateAll();
		this.infectionPanel.DeactivateAll();
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
				GameObject gameObject = this.diseaseSourcePanel.AddOrGetLabel("source_" + j.ToString());
				gameObject.GetComponent<LocText>().text = list[j].text;
				gameObject.GetComponent<ToolTip>().toolTip = list[j].tooltipText;
				gameObject.SetActive(true);
			}
		}
		else
		{
			this.diseaseSourcePanel.SetActive(false);
		}
		if (!this.CreateImmuneInfo())
		{
			this.immuneSystemPanel.SetActive(false);
		}
		if (!this.CreateDiseaseInfo())
		{
			this.infoPanel.SetActive(false);
			this.infectionPanel.SetActive(false);
			GameObject gameObject = this.currentGermsPanel.AddOrGetLabel("nodisease");
			this.currentGermsPanel.container.HeaderLabel.text = UI.DETAILTABS.DISEASE.NO_CURRENT_GERMS;
			gameObject.GetComponent<LocText>().text = UI.DETAILTABS.DISEASE.DETAILS.NODISEASE;
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.NODISEASE_TOOLTIP;
			gameObject.SetActive(true);
		}
	}

	private bool CreateImmuneInfo()
	{
		ImmuneSystemMonitor.Instance smi = this.selectedTarget.GetSMI<ImmuneSystemMonitor.Instance>();
		if (smi != null)
		{
			AmountInstance amountInstance = Db.Get().Amounts.ImmuneLevel.Lookup(this.selectedTarget);
			for (int i = 0; i < Db.Get().Diseases.Count; i++)
			{
				Disease disease = Db.Get().Diseases[i];
				AmountInstance amountInstance2 = disease.amount.Lookup(this.selectedTarget);
				if (amountInstance2.value > 0f)
				{
					GameObject gameObject = this.immuneSystemPanel.AddOrGetLabel("disease_" + disease.Id);
					gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.INTERNAL_GERMS, disease.Name, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance2.value)));
					gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.INTERNAL_GERMS_TOOLTIP, disease.Name, GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance2.value)));
					gameObject.SetActive(true);
					AttributeModifier currentImmuneModifier = smi.GetCurrentImmuneModifier(disease);
					if (currentImmuneModifier != null)
					{
						gameObject = this.immuneSystemPanel.AddOrGetLabel("disease_rate2_" + disease.Id);
						gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.IMMUNE_ATTACK_RATE2, currentImmuneModifier.GetFormattedString(this.selectedTarget), GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance2.value)));
						gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.DISEASE.IMMUNE_FACTORS.IMMUNE_ATTACK_RATE2_TOOLTIP, currentImmuneModifier.GetFormattedString(this.selectedTarget), GameUtil.GetFormattedDiseaseAmount(Mathf.RoundToInt(amountInstance2.value)));
						gameObject.SetActive(true);
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

	private string GetFormattedHalfLife(int count, Disease.CompositeGrowthRule rule)
	{
		if (count >= rule.minCount)
		{
			return this.GetFormattedHalfLife(rule.GetHalfLifeForCount(count));
		}
		return UI.DETAILTABS.DISEASE.DETAILS.DYING_OFF;
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

	private void BuildFactorsStrings(int diseaseCount, int elementIdx, int environmentCell, float temperature, Tag[] tags, Disease disease)
	{
		this.currentGermsPanel.container.HeaderLabel.text = string.Format(UI.DETAILTABS.DISEASE.CURRENT_GERMS, disease.Name.ToUpper());
		this.infectionPanel.container.HeaderLabel.text = string.Format(UI.DETAILTABS.DISEASE.INFECTION_INFO, disease.Name.ToUpper());
		this.infoPanel.container.HeaderLabel.text = string.Format(UI.DETAILTABS.DISEASE.GERMS_INFO, disease.Name.ToUpper());
		GameObject gameObject = this.currentGermsPanel.AddOrGetLabel("currentgerms");
		gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT, disease.Name, GameUtil.GetFormattedDiseaseAmount(diseaseCount));
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.DISEASE.DETAILS.DISEASE_AMOUNT_TOOLTIP, GameUtil.GetFormattedDiseaseAmount(diseaseCount));
		gameObject.SetActive(true);
		Element element = ElementLoader.elements[elementIdx];
		Disease.CompositeGrowthRule growthRuleForElement = disease.GetGrowthRuleForElement(element);
		if (diseaseCount < growthRuleForElement.minCount)
		{
			gameObject = this.currentGermsPanel.AddOrGetLabel("critical_status");
			gameObject.GetComponent<LocText>().text = UI.DETAILTABS.DISEASE.DETAILS.DYING_OFF;
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.DYING_OFF_TOOLTIP;
			gameObject.SetActive(true);
		}
		else if (diseaseCount > growthRuleForElement.maxCount)
		{
			gameObject = this.currentGermsPanel.AddOrGetLabel("critical_status");
			gameObject.GetComponent<LocText>().text = UI.DETAILTABS.DISEASE.DETAILS.OVERPOPULATED;
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.OVERPOPULATED_TOOLTIP;
			gameObject.SetActive(true);
		}
		float num = 1f;
		if (tags != null && tags.Length > 0)
		{
			num = disease.GetGrowthRateForTags(tags, diseaseCount > growthRuleForElement.maxCount);
		}
		float num2 = DiseaseContainers.CalculateDelta(diseaseCount, elementIdx, environmentCell, temperature, num, disease, 1f);
		gameObject = this.currentGermsPanel.AddOrGetLabel("finaldelta");
		gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE, GameUtil.GetFormattedSimple(num2, GameUtil.TimeSlice.PerSecond, "F0"));
		gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.RATE_OF_CHANGE_TOOLTIP, GameUtil.GetFormattedSimple(num2, GameUtil.TimeSlice.PerSecond, "F0"));
		gameObject.SetActive(true);
		float num3 = Disease.GrowthRateToHalfLife(1f - num2 / (float)diseaseCount);
		gameObject = this.currentGermsPanel.AddOrGetLabel("finalhalflife");
		if (num3 > 0f)
		{
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG, GameUtil.GetFormattedCycles(num3, "F1"));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEG_TOOLTIP, GameUtil.GetFormattedCycles(num3, "F1"));
		}
		else if (num3 < 0f)
		{
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS, GameUtil.GetFormattedCycles(-num3, "F1"));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_POS_TOOLTIP, GameUtil.GetFormattedCycles(num3, "F1"));
		}
		else
		{
			gameObject.GetComponent<LocText>().text = UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL;
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.HALF_LIFE_NEUTRAL_TOOLTIP;
		}
		gameObject.SetActive(true);
		gameObject = this.currentGermsPanel.AddOrGetLabel("factors");
		gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TITLE, new object[0]);
		gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TOOLTIP;
		gameObject.SetActive(true);
		gameObject = this.currentGermsPanel.AddOrGetLabel("substrate");
		gameObject.GetComponent<LocText>().text = this.GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL);
		gameObject.GetComponent<ToolTip>().toolTip = this.GetFormattedGrowthEntry(growthRuleForElement.Name(), growthRuleForElement.populationHalfLife, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP);
		gameObject.SetActive(true);
		for (int i = 0; i < tags.Length; i++)
		{
			Disease.TagGrowthRule growthRuleForTag = disease.GetGrowthRuleForTag(tags[i]);
			if (growthRuleForTag != null)
			{
				gameObject = this.currentGermsPanel.AddOrGetLabel("tag_" + i);
				TMP_Text component = gameObject.GetComponent<LocText>();
				string text = growthRuleForTag.Name();
				float? populationHalfLife = growthRuleForTag.populationHalfLife;
				component.text = this.GetFormattedGrowthEntry(text, populationHalfLife.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL);
				ToolTip component2 = gameObject.GetComponent<ToolTip>();
				string text2 = growthRuleForTag.Name();
				float? populationHalfLife2 = growthRuleForTag.populationHalfLife;
				component2.toolTip = this.GetFormattedGrowthEntry(text2, populationHalfLife2.Value, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.DIE_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.GROW_TOOLTIP, UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.SUBSTRATE.NEUTRAL_TOOLTIP);
				gameObject.SetActive(true);
			}
		}
		if (Grid.IsValidCell(environmentCell))
		{
			Disease.CompositeGrowthRule exposureRuleForElement = disease.GetExposureRuleForElement(Grid.Element[environmentCell]);
			if (exposureRuleForElement != null && exposureRuleForElement.populationHalfLife != float.PositiveInfinity)
			{
				gameObject = this.currentGermsPanel.AddOrGetLabel("environment");
				gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.TITLE, exposureRuleForElement.Name(), this.GetFormattedHalfLife(exposureRuleForElement.GetHalfLifeForCount(diseaseCount)));
				if (exposureRuleForElement.GetHalfLifeForCount(diseaseCount) > 0f)
				{
					gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.DIE_TOOLTIP;
				}
				else
				{
					gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.ENVIRONMENT.GROW_TOOLTIP;
				}
				gameObject.SetActive(true);
			}
		}
		float num4 = disease.CalculateTemperatureHalfLife(temperature);
		if (num4 != float.PositiveInfinity)
		{
			gameObject = this.currentGermsPanel.AddOrGetLabel("temperature");
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.TITLE, GameUtil.GetFormattedTemperature(temperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true), this.GetFormattedHalfLife(num4));
			if (num4 > 0f)
			{
				gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.DIE_TOOLTIP;
			}
			else
			{
				gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.DETAILS.GROWTH_FACTORS.TEMPERATURE.GROW_TOOLTIP;
			}
			gameObject.SetActive(true);
		}
		List<Descriptor> quantitativeDescriptors = disease.GetQuantitativeDescriptors();
		for (int j = 0; j < quantitativeDescriptors.Count; j++)
		{
			gameObject = this.infoPanel.AddOrGetLabel("info_" + j);
			gameObject.GetComponent<LocText>().text = quantitativeDescriptors[j].IndentedText();
			gameObject.GetComponent<ToolTip>().toolTip = quantitativeDescriptors[j].tooltipText;
			gameObject.SetActive(true);
		}
		gameObject = this.infectionPanel.AddOrGetLabel("disclaimer");
		gameObject.GetComponent<LocText>().text = UI.DETAILTABS.DISEASE.INFECTION.DISCLAIMER;
		gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.INFECTION.DISCLAIMER_TOOLTIP;
		gameObject.SetActive(true);
		gameObject = this.infectionPanel.AddOrGetLabel("duration");
		gameObject.GetComponent<LocText>().text = UI.DETAILTABS.DISEASE.INFECTION.DURATION;
		gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.INFECTION.DURATION_TOOLTIP;
		gameObject.SetActive(true);
		gameObject = this.infectionPanel.AddOrGetLabel("duration_amount");
		if (disease.doctorRequired)
		{
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.INFECTION.DURATION_AIDREQ, GameUtil.GetFormattedCycles(disease.SicknessDuration, "F1"));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.DISEASE.INFECTION.DURATION_AIDREQ_TOOLTIP, GameUtil.GetFormattedCycles(disease.SicknessDuration, "F1"));
		}
		else
		{
			gameObject.GetComponent<LocText>().text = string.Format(UI.DETAILTABS.DISEASE.INFECTION.DURATION_NORMAL, GameUtil.GetFormattedCycles(disease.SicknessDuration, "F1"));
			gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.DETAILTABS.DISEASE.INFECTION.DURATION_NORMAL_TOOLTIP, GameUtil.GetFormattedCycles(disease.SicknessDuration, "F1"));
		}
		gameObject.SetActive(true);
		List<Descriptor> symptoms = disease.GetSymptoms();
		GameUtil.IndentListOfDescriptors(symptoms);
		List<Descriptor> list = symptoms.FindAll((Descriptor d) => d.type == Descriptor.DescriptorType.SymptomAidable);
		if (list.Count > 0)
		{
			gameObject = this.infectionPanel.AddOrGetLabel("symptoms_aid");
			gameObject.GetComponent<LocText>().text = UI.DETAILTABS.DISEASE.INFECTION.AID_SYMPTOMS;
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.INFECTION.AID_SYMPTOMS_TOOLTIP;
			gameObject.SetActive(true);
			for (int k = 0; k < list.Count; k++)
			{
				gameObject = this.infectionPanel.AddOrGetLabel("symptoms_aid_" + k);
				gameObject.GetComponent<LocText>().text = list[k].IndentedText();
				gameObject.GetComponent<ToolTip>().toolTip = list[k].tooltipText;
				gameObject.SetActive(true);
			}
		}
		List<Descriptor> list2 = symptoms.FindAll((Descriptor d) => d.type == Descriptor.DescriptorType.Symptom);
		if (list.Count > 0)
		{
			gameObject = this.infectionPanel.AddOrGetLabel("symptoms_noaid");
			gameObject.GetComponent<LocText>().text = UI.DETAILTABS.DISEASE.INFECTION.SYMPTOMS;
			gameObject.GetComponent<ToolTip>().toolTip = UI.DETAILTABS.DISEASE.INFECTION.SYMPTOMS_TOOLTIP;
			gameObject.SetActive(true);
			for (int l = 0; l < list2.Count; l++)
			{
				gameObject = this.infectionPanel.AddOrGetLabel("symptoms_noaid_" + l);
				gameObject.GetComponent<LocText>().text = list2[l].IndentedText();
				gameObject.GetComponent<ToolTip>().toolTip = list2[l].tooltipText;
				gameObject.SetActive(true);
			}
		}
	}

	public override void OnSelectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
	}

	public override void OnDeselectTarget(GameObject target)
	{
		base.OnSelectTarget(target);
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
			int num = Grid.PosToCell(component.transform.position);
			KPrefabID component2 = component.GetComponent<KPrefabID>();
			this.BuildFactorsStrings(component.DiseaseCount, ElementLoader.GetElementIndex(component.Element.id), num, component.Temperature, component2.Tags, disease);
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
			this.BuildFactorsStrings(cso.diseaseCount, elementIndex, -1, cso.temperature, new Tag[0], disease);
			return true;
		}
		return false;
	}

	public GameObject labelTemplate;

	private DiseaseInfoScreen.InfoPanel infectionPanel;

	private DiseaseInfoScreen.InfoPanel immuneSystemPanel;

	private DiseaseInfoScreen.InfoPanel diseaseSourcePanel;

	private DiseaseInfoScreen.InfoPanel currentGermsPanel;

	private DiseaseInfoScreen.InfoPanel infoPanel;

	public class InfoPanel
	{
		public InfoPanel(string label, GameObject labelTemplate, GameObject parent)
		{
			GameObject gameObject = Util.KInstantiateUI(ScreenPrefabs.Instance.CollapsableContentPanel, parent, false);
			this.container = gameObject.GetComponent<CollapsibleDetailContentPanel>();
			this.container.HeaderLabel.text = label;
			this.labelTemplate = labelTemplate;
			this.labels = new Dictionary<string, GameObject>();
		}

		public GameObject AddOrGetLabel(string id)
		{
			if (!this.container.gameObject.activeSelf)
			{
				this.container.gameObject.SetActive(true);
			}
			GameObject gameObject;
			if (this.labels.ContainsKey(id))
			{
				gameObject = this.labels[id];
			}
			else
			{
				gameObject = Util.KInstantiate(this.labelTemplate, this.container.Content.gameObject, null);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				this.labels[id] = gameObject;
			}
			gameObject.transform.SetAsLastSibling();
			return gameObject;
		}

		public void DeactivateAll()
		{
			foreach (KeyValuePair<string, GameObject> keyValuePair in this.labels)
			{
				keyValuePair.Value.SetActive(false);
			}
		}

		public void SetActive(bool active)
		{
			this.container.gameObject.SetActive(active);
		}

		public GameObject labelTemplate;

		public CollapsibleDetailContentPanel container;

		public Dictionary<string, GameObject> labels;
	}
}
