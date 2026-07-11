using System;
using System.Collections.Generic;
using System.Diagnostics;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MinionVitalsPanel : KMonoBehaviour
{
	public void Init()
	{
		this.AddAmountLine(Db.Get().Amounts.HitPoints, null);
		this.AddAttributeLine(Db.Get().CritterAttributes.Happiness, null);
		this.AddAmountLine(Db.Get().Amounts.Wildness, null);
		this.AddAmountLine(Db.Get().Amounts.Incubation, null);
		this.AddAmountLine(Db.Get().Amounts.Viability, null);
		this.AddAmountLine(Db.Get().Amounts.Fertility, null);
		this.AddAmountLine(Db.Get().Amounts.Age, null);
		this.AddAmountLine(Db.Get().Amounts.Stress, null);
		this.AddAttributeLine(Db.Get().Attributes.QualityOfLife, null);
		this.AddAmountLine(Db.Get().Amounts.Bladder, null);
		this.AddAmountLine(Db.Get().Amounts.Breath, null);
		this.AddAmountLine(Db.Get().Amounts.Stamina, null);
		this.AddAmountLine(Db.Get().Amounts.Calories, null);
		this.AddAmountLine(Db.Get().Amounts.ScaleGrowth, null);
		this.AddAmountLine(Db.Get().Amounts.Temperature, null);
		this.AddAmountLine(Db.Get().Amounts.Decor, null);
		this.AddCheckboxLine(Db.Get().Amounts.AirPressure, this.conditionsContainerNormal, (GameObject go) => this.GetAirPressureLabel(go), delegate(GameObject go)
		{
			if (go.GetComponent<PressureVulnerable>() != null && go.GetComponent<PressureVulnerable>().pressure_sensitive)
			{
				return MinionVitalsPanel.CheckboxLineDisplayType.Normal;
			}
			return MinionVitalsPanel.CheckboxLineDisplayType.Hidden;
		}, (GameObject go) => this.check_pressure(go), (GameObject go) => this.GetAirPressureTooltip(go));
		this.AddCheckboxLine(null, this.conditionsContainerNormal, (GameObject go) => this.GetAtmosphereLabel(go), delegate(GameObject go)
		{
			if (go.GetComponent<PressureVulnerable>() != null && go.GetComponent<PressureVulnerable>().safe_atmospheres.Count > 0)
			{
				return MinionVitalsPanel.CheckboxLineDisplayType.Normal;
			}
			return MinionVitalsPanel.CheckboxLineDisplayType.Hidden;
		}, (GameObject go) => this.check_atmosphere(go), (GameObject go) => this.GetAtmosphereTooltip(go));
		this.AddCheckboxLine(Db.Get().Amounts.Temperature, this.conditionsContainerNormal, (GameObject go) => this.GetInternalTemperatureLabel(go), delegate(GameObject go)
		{
			if (go.GetComponent<TemperatureVulnerable>() != null)
			{
				return MinionVitalsPanel.CheckboxLineDisplayType.Normal;
			}
			return MinionVitalsPanel.CheckboxLineDisplayType.Hidden;
		}, (GameObject go) => this.check_temperature(go), (GameObject go) => this.GetInternalTemperatureTooltip(go));
		this.AddCheckboxLine(Db.Get().Amounts.Fertilization, this.conditionsContainerAdditional, (GameObject go) => this.GetFertilizationLabel(go), delegate(GameObject go)
		{
			if (go.GetComponent<Growing>() == null)
			{
				return MinionVitalsPanel.CheckboxLineDisplayType.Hidden;
			}
			if (go.GetComponent<Growing>().Replanted)
			{
				return MinionVitalsPanel.CheckboxLineDisplayType.Normal;
			}
			return MinionVitalsPanel.CheckboxLineDisplayType.Diminished;
		}, (GameObject go) => this.check_fertilizer(go), (GameObject go) => this.GetFertilizationTooltip(go));
		this.AddCheckboxLine(Db.Get().Amounts.Irrigation, this.conditionsContainerAdditional, (GameObject go) => this.GetIrrigationLabel(go), delegate(GameObject go)
		{
			Growing component = go.GetComponent<Growing>();
			return (!(component != null) || !component.Replanted) ? MinionVitalsPanel.CheckboxLineDisplayType.Diminished : MinionVitalsPanel.CheckboxLineDisplayType.Normal;
		}, (GameObject go) => this.check_irrigation(go), (GameObject go) => this.GetIrrigationTooltip(go));
		this.AddCheckboxLine(Db.Get().Amounts.Illumination, this.conditionsContainerNormal, (GameObject go) => this.GetIlluminationLabel(go), (GameObject go) => MinionVitalsPanel.CheckboxLineDisplayType.Normal, (GameObject go) => this.check_illumination(go), (GameObject go) => this.GetIlluminationTooltip(go));
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		SimAndRenderScheduler.instance.Add(this, false);
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		SimAndRenderScheduler.instance.Remove(this);
	}

	private void AddAmountLine(Amount amount, Func<AmountInstance, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(this.LineItemPrefab, base.gameObject, false);
		gameObject.GetComponentInChildren<Image>().sprite = Assets.GetSprite(amount.uiSprite);
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(true);
		MinionVitalsPanel.AmountLine amountLine = default(MinionVitalsPanel.AmountLine);
		amountLine.amount = amount;
		amountLine.go = gameObject;
		amountLine.locText = gameObject.GetComponentInChildren<LocText>();
		amountLine.toolTip = gameObject.GetComponentInChildren<ToolTip>();
		amountLine.imageToggle = gameObject.GetComponentInChildren<ValueTrendImageToggle>();
		amountLine.toolTipFunc = ((tooltip_func == null) ? new Func<AmountInstance, string>(amount.GetTooltip) : tooltip_func);
		this.amountsLines.Add(amountLine);
	}

	private void AddAttributeLine(Klei.AI.Attribute attribute, Func<AttributeInstance, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(this.LineItemPrefab, base.gameObject, false);
		gameObject.GetComponentInChildren<Image>().sprite = Assets.GetSprite(attribute.uiSprite);
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(true);
		MinionVitalsPanel.AttributeLine attributeLine = default(MinionVitalsPanel.AttributeLine);
		attributeLine.attribute = attribute;
		attributeLine.go = gameObject;
		attributeLine.locText = gameObject.GetComponentInChildren<LocText>();
		attributeLine.toolTip = gameObject.GetComponentInChildren<ToolTip>();
		gameObject.GetComponentInChildren<ValueTrendImageToggle>().gameObject.SetActive(false);
		attributeLine.toolTipFunc = ((tooltip_func == null) ? new Func<AttributeInstance, string>(attribute.GetTooltip) : tooltip_func);
		this.attributesLines.Add(attributeLine);
	}

	private void AddCheckboxLine(Amount amount, Transform parentContainer, Func<GameObject, string> label_text_func, Func<GameObject, MinionVitalsPanel.CheckboxLineDisplayType> display_condition, Func<GameObject, bool> checkbox_value_func, Func<GameObject, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(this.CheckboxLinePrefab, base.gameObject, false);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(true);
		MinionVitalsPanel.CheckboxLine checkboxLine = default(MinionVitalsPanel.CheckboxLine);
		checkboxLine.go = gameObject;
		checkboxLine.parentContainer = parentContainer;
		checkboxLine.amount = amount;
		checkboxLine.locText = component.GetReference("Label") as LocText;
		checkboxLine.get_value = checkbox_value_func;
		checkboxLine.display_condition = display_condition;
		checkboxLine.label_text_func = label_text_func;
		checkboxLine.go.name = "Checkbox_";
		if (amount != null)
		{
			GameObject go = checkboxLine.go;
			go.name += amount.Name;
		}
		else
		{
			GameObject go2 = checkboxLine.go;
			go2.name += "Unnamed";
		}
		if (tooltip_func != null)
		{
			checkboxLine.tooltip = tooltip_func;
			ToolTip tt = checkboxLine.go.GetComponent<ToolTip>();
			tt.refreshWhileHovering = true;
			tt.OnToolTip = delegate
			{
				tt.ClearMultiStringTooltip();
				tt.AddMultiStringTooltip(tooltip_func(this.selectedEntity), null);
				return string.Empty;
			};
		}
		this.checkboxLines.Add(checkboxLine);
	}

	public void Refresh()
	{
		if (this.selectedEntity == null)
		{
			return;
		}
		if (this.selectedEntity.gameObject == null)
		{
			return;
		}
		Amounts amounts = this.selectedEntity.GetAmounts();
		Attributes attributes = this.selectedEntity.GetAttributes();
		if (amounts == null || attributes == null)
		{
			return;
		}
		WiltCondition component = this.selectedEntity.GetComponent<WiltCondition>();
		if (component == null)
		{
			this.conditionsContainerNormal.gameObject.SetActive(false);
			this.conditionsContainerAdditional.gameObject.SetActive(false);
			foreach (MinionVitalsPanel.AmountLine amountLine in this.amountsLines)
			{
				bool flag = amountLine.TryUpdate(amounts);
				if (amountLine.go.activeSelf != flag)
				{
					amountLine.go.SetActive(flag);
				}
			}
			foreach (MinionVitalsPanel.AttributeLine attributeLine in this.attributesLines)
			{
				bool flag2 = attributeLine.TryUpdate(attributes);
				if (attributeLine.go.activeSelf != flag2)
				{
					attributeLine.go.SetActive(flag2);
				}
			}
		}
		bool flag3 = false;
		for (int i = 0; i < this.checkboxLines.Count; i++)
		{
			MinionVitalsPanel.CheckboxLine checkboxLine = this.checkboxLines[i];
			MinionVitalsPanel.CheckboxLineDisplayType checkboxLineDisplayType = MinionVitalsPanel.CheckboxLineDisplayType.Hidden;
			if (this.checkboxLines[i].amount != null)
			{
				for (int j = 0; j < amounts.Count; j++)
				{
					AmountInstance amountInstance = amounts[j];
					if (checkboxLine.amount == amountInstance.amount)
					{
						checkboxLineDisplayType = checkboxLine.display_condition(this.selectedEntity.gameObject);
						break;
					}
				}
			}
			else
			{
				checkboxLineDisplayType = checkboxLine.display_condition(this.selectedEntity.gameObject);
			}
			if (checkboxLineDisplayType != MinionVitalsPanel.CheckboxLineDisplayType.Hidden)
			{
				checkboxLine.locText.SetText(checkboxLine.label_text_func(this.selectedEntity.gameObject));
				if (!checkboxLine.go.activeSelf)
				{
					checkboxLine.go.SetActive(true);
				}
				GameObject gameObject = checkboxLine.go.GetComponent<HierarchyReferences>().GetReference("Check").gameObject;
				gameObject.SetActive(checkboxLine.get_value(this.selectedEntity.gameObject));
				if (checkboxLine.go.transform.parent != checkboxLine.parentContainer)
				{
					checkboxLine.go.transform.SetParent(checkboxLine.parentContainer);
					checkboxLine.go.transform.localScale = Vector3.one;
				}
				if (checkboxLine.parentContainer == this.conditionsContainerAdditional)
				{
					flag3 = true;
				}
				if (checkboxLineDisplayType == MinionVitalsPanel.CheckboxLineDisplayType.Normal)
				{
					if (checkboxLine.get_value(this.selectedEntity.gameObject))
					{
						checkboxLine.locText.color = Color.black;
						gameObject.transform.parent.GetComponent<Image>().color = Color.black;
					}
					else
					{
						Color color = new Color(0.99215686f, 0f, 0.101960786f);
						checkboxLine.locText.color = color;
						gameObject.transform.parent.GetComponent<Image>().color = color;
					}
				}
				else
				{
					checkboxLine.locText.color = Color.grey;
					gameObject.transform.parent.GetComponent<Image>().color = Color.grey;
				}
			}
			else if (checkboxLine.go.activeSelf)
			{
				checkboxLine.go.SetActive(false);
			}
		}
		if (component != null)
		{
			Growing component2 = component.GetComponent<Growing>();
			this.conditionsContainerNormal.gameObject.SetActive(true);
			this.conditionsContainerAdditional.gameObject.SetActive(component2 != null);
			if (component2 == null)
			{
				LocText locText = this.conditionsContainerNormal.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
				locText.text = string.Empty;
			}
			else
			{
				LocText locText = this.conditionsContainerNormal.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
				locText.text = string.Empty;
				locText.text = string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().WildGrowthTime(), "F1"));
				locText.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.WILD.TOOLTIP, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().WildGrowthTime(), "F1")));
				locText = this.conditionsContainerAdditional.GetComponent<HierarchyReferences>().GetReference<LocText>("Label");
				locText.color = ((!this.selectedEntity.GetComponent<Growing>().Replanted) ? Color.grey : Color.black);
				locText.text = string.Empty;
				locText.text = ((!flag3) ? string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.DOMESTIC.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")) : string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")));
				locText.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC.TOOLTIP, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")));
			}
			foreach (MinionVitalsPanel.AmountLine amountLine2 in this.amountsLines)
			{
				amountLine2.go.SetActive(false);
			}
			foreach (MinionVitalsPanel.AttributeLine attributeLine2 in this.attributesLines)
			{
				attributeLine2.go.SetActive(false);
			}
		}
	}

	private string GetAirPressureTooltip(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if (component == null)
		{
			return string.Empty;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_PRESSURE.text.Replace("{pressure}", GameUtil.GetFormattedMass(component.GetExternalPressure, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string GetInternalTemperatureTooltip(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		if (component == null)
		{
			return string.Empty;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_TEMPERATURE.text.Replace("{temperature}", GameUtil.GetFormattedTemperature(component.InternalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false));
	}

	private string GetFertilizationTooltip(GameObject go)
	{
		FertilizationMonitor.Instance smi = go.GetSMI<FertilizationMonitor.Instance>();
		if (smi == null)
		{
			return string.Empty;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_FERTILIZER.text.Replace("{mass}", GameUtil.GetFormattedMass(smi.total_fertilizer_available, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string GetIrrigationTooltip(GameObject go)
	{
		IrrigationMonitor.Instance smi = go.GetSMI<IrrigationMonitor.Instance>();
		if (smi == null)
		{
			return string.Empty;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_IRRIGATION.text.Replace("{mass}", GameUtil.GetFormattedMass(smi.total_fertilizer_available, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
	}

	private string GetIlluminationTooltip(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		if (component == null)
		{
			return string.Empty;
		}
		if ((component.prefersDarkness && component.IsComfortable()) || (!component.prefersDarkness && !component.IsComfortable()))
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_ILLUMINATION_DARK;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_ILLUMINATION_LIGHT;
	}

	private string GetReceptacleTooltip(GameObject go)
	{
		ReceptacleMonitor component = go.GetComponent<ReceptacleMonitor>();
		if (component == null)
		{
			return string.Empty;
		}
		if (component.HasOperationalReceptacle())
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_RECEPTACLE_OPERATIONAL;
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_RECEPTACLE_INOPERATIONAL;
	}

	private string GetAtmosphereTooltip(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		if (component != null)
		{
			return UI.TOOLTIPS.VITALS_CHECKBOX_ATMOSPHERE.text.Replace("{element}", component.GetExternalElement.name);
		}
		return UI.TOOLTIPS.VITALS_CHECKBOX_ATMOSPHERE;
	}

	private string GetAirPressureLabel(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		return string.Concat(new string[]
		{
			Db.Get().Amounts.AirPressure.Name,
			"\n    • ",
			GameUtil.GetFormattedMass(component.pressureWarning_Low, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Gram, false, "{0:0.#}"),
			" - ",
			GameUtil.GetFormattedMass(component.pressureWarning_High, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.Gram, true, "{0:0.#}")
		});
	}

	private string GetInternalTemperatureLabel(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		return string.Concat(new string[]
		{
			Db.Get().Amounts.Temperature.Name,
			"\n    • ",
			GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false, false),
			" - ",
			GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true, false)
		});
	}

	private string GetFertilizationLabel(GameObject go)
	{
		FertilizationMonitor.Instance smi = go.GetSMI<FertilizationMonitor.Instance>();
		string text = Db.Get().Amounts.Fertilization.Name;
		foreach (PlantElementAbsorber.ConsumeInfo consumeInfo in smi.def.consumedElements)
		{
			string text2 = text;
			text = string.Concat(new string[]
			{
				text2,
				"\n    • ",
				ElementLoader.GetElement(consumeInfo.tag).name,
				" ",
				GameUtil.GetFormattedMass(consumeInfo.massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")
			});
		}
		return text;
	}

	private string GetIrrigationLabel(GameObject go)
	{
		IrrigationMonitor.Instance smi = go.GetSMI<IrrigationMonitor.Instance>();
		return string.Concat(new string[]
		{
			Db.Get().Amounts.Irrigation.Name,
			"\n    • ",
			ElementLoader.GetElement(smi.def.consumedElements[0].tag).name,
			": ",
			GameUtil.GetFormattedMass(smi.def.consumedElements[0].massConsumptionRate, GameUtil.TimeSlice.PerCycle, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")
		});
	}

	private string GetIlluminationLabel(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		return Db.Get().Amounts.Illumination.Name + "\n    • " + ((!component.prefersDarkness) ? UI.GAMEOBJECTEFFECTS.LIGHT : UI.GAMEOBJECTEFFECTS.DARKNESS);
	}

	private string GetAtmosphereLabel(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		string text = UI.VITALSSCREEN.ATMOSPHERE_CONDITION;
		foreach (Element element in component.safe_atmospheres)
		{
			text = text + "\n    • " + element.name;
		}
		return text;
	}

	private bool check_pressure(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		return !(component != null) || component.GetExternalPressureState == PressureVulnerable.PressureState.Normal;
	}

	private bool check_temperature(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		return !(component != null) || component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Normal;
	}

	private bool check_irrigation(GameObject go)
	{
		IrrigationMonitor.Instance smi = go.GetSMI<IrrigationMonitor.Instance>();
		return smi == null || !smi.IsInsideState(smi.sm.replanted.starved);
	}

	private bool check_illumination(GameObject go)
	{
		IlluminationVulnerable component = go.GetComponent<IlluminationVulnerable>();
		return !(component != null) || component.IsComfortable();
	}

	private bool check_receptacle(GameObject go)
	{
		ReceptacleMonitor component = go.GetComponent<ReceptacleMonitor>();
		return !(component == null) && component.HasOperationalReceptacle();
	}

	private bool check_fertilizer(GameObject go)
	{
		FertilizationMonitor.Instance smi = go.GetSMI<FertilizationMonitor.Instance>();
		return smi == null || smi.sm.hasCorrectFertilizer.Get(smi);
	}

	private bool check_atmosphere(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		return !(component != null) || component.IsSafeElement(Grid.Element[Grid.PosToCell(go)]);
	}

	public GameObject LineItemPrefab;

	public GameObject CheckboxLinePrefab;

	public GameObject selectedEntity;

	public List<MinionVitalsPanel.AmountLine> amountsLines = new List<MinionVitalsPanel.AmountLine>();

	public List<MinionVitalsPanel.AttributeLine> attributesLines = new List<MinionVitalsPanel.AttributeLine>();

	public List<MinionVitalsPanel.CheckboxLine> checkboxLines = new List<MinionVitalsPanel.CheckboxLine>();

	public Transform conditionsContainerNormal;

	public Transform conditionsContainerAdditional;

	[DebuggerDisplay("{amount.Name}")]
	public struct AmountLine
	{
		public bool TryUpdate(Amounts amounts)
		{
			foreach (AmountInstance amountInstance in amounts)
			{
				if (this.amount == amountInstance.amount && !amountInstance.hide)
				{
					this.locText.SetText(this.amount.GetDescription(amountInstance));
					this.toolTip.toolTip = this.toolTipFunc(amountInstance);
					this.imageToggle.SetValue(amountInstance);
					return true;
				}
			}
			return false;
		}

		public Amount amount;

		public GameObject go;

		public ValueTrendImageToggle imageToggle;

		public LocText locText;

		public ToolTip toolTip;

		public Func<AmountInstance, string> toolTipFunc;
	}

	[DebuggerDisplay("{attribute.Name}")]
	public struct AttributeLine
	{
		public bool TryUpdate(Attributes attributes)
		{
			foreach (AttributeInstance attributeInstance in attributes)
			{
				if (this.attribute == attributeInstance.modifier && !attributeInstance.hide)
				{
					this.locText.SetText(this.attribute.GetDescription(attributeInstance));
					this.toolTip.toolTip = this.toolTipFunc(attributeInstance);
					return true;
				}
			}
			return false;
		}

		public Klei.AI.Attribute attribute;

		public GameObject go;

		public LocText locText;

		public ToolTip toolTip;

		public Func<AttributeInstance, string> toolTipFunc;
	}

	public struct CheckboxLine
	{
		public Amount amount;

		public GameObject go;

		public LocText locText;

		public Func<GameObject, string> tooltip;

		public Func<GameObject, bool> get_value;

		public Func<GameObject, MinionVitalsPanel.CheckboxLineDisplayType> display_condition;

		public Func<GameObject, string> label_text_func;

		public Transform parentContainer;
	}

	public enum CheckboxLineDisplayType
	{
		Normal,
		Diminished,
		Hidden
	}
}
