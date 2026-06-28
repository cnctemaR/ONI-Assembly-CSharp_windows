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
		this.AddLine(Db.Get().Amounts.HitPoints, this.icon_hitpoints, null);
		this.AddLine(Db.Get().Amounts.Happiness, this.icon_stress, null);
		this.AddLine(Db.Get().Amounts.Wildness, this.icon_hitpoints, null);
		this.AddLine(Db.Get().Amounts.Incubation, this.icon_hitpoints, null);
		this.AddLine(Db.Get().Amounts.Fertility, this.icon_hitpoints, null);
		this.AddLine(Db.Get().Amounts.Stress, this.icon_stress, null);
		this.AddLine(Db.Get().Amounts.Bladder, this.icon_bladder, null);
		this.AddLine(Db.Get().Amounts.Breath, this.icon_breath, null);
		this.AddLine(Db.Get().Amounts.Stamina, this.icon_stamina, null);
		this.AddLine(Db.Get().Amounts.Calories, this.icon_calories, null);
		this.AddLine(Db.Get().Amounts.ImmuneLevel, this.icon_disease, null);
		this.AddLine(Db.Get().Amounts.Temperature, this.icon_temperature, null);
		this.AddLine(Db.Get().Amounts.Decor, this.icon_decor, (AmountInstance ainstance) => this.GetDecorTooltip(ainstance));
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

	private string GetDecorTooltip(AmountInstance amount_instance)
	{
		string tooltip = amount_instance.amount.GetTooltip(amount_instance);
		AttributeInstance attributeInstance = Db.Get().Attributes.DecorExpectation.Lookup(amount_instance.gameObject);
		string text = tooltip;
		return string.Concat(new object[]
		{
			text,
			"\n\n",
			attributeInstance.Name,
			": ",
			attributeInstance.GetTotalValue()
		});
	}

	private void AddLine(Amount amount, Sprite icon, Func<AmountInstance, string> tooltip_func = null)
	{
		GameObject gameObject = Util.KInstantiateUI(this.LineItemPrefab, base.gameObject, false);
		gameObject.GetComponentInChildren<Image>().sprite = icon;
		gameObject.GetComponent<ToolTip>().refreshWhileHovering = true;
		gameObject.SetActive(true);
		MinionVitalsPanel.VitalLine vitalLine = default(MinionVitalsPanel.VitalLine);
		vitalLine.amount = amount;
		vitalLine.go = gameObject;
		vitalLine.locText = gameObject.GetComponentInChildren<LocText>();
		vitalLine.imageToggle = gameObject.GetComponentInChildren<ValueTrendImageToggle>();
		vitalLine.tooltip = ((tooltip_func == null) ? new Func<AmountInstance, string>(amount.GetTooltip) : tooltip_func);
		this.vitalsLines.Add(vitalLine);
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
		if (amounts == null)
		{
			return;
		}
		WiltCondition component = this.selectedEntity.GetComponent<WiltCondition>();
		if (component == null)
		{
			this.conditionsContainerNormal.gameObject.SetActive(false);
			this.conditionsContainerAdditional.gameObject.SetActive(false);
			for (int i = 0; i < this.vitalsLines.Count; i++)
			{
				MinionVitalsPanel.VitalLine vitalLine = this.vitalsLines[i];
				bool flag = false;
				for (int j = 0; j < amounts.Count; j++)
				{
					AmountInstance amountInstance = amounts[j];
					if (vitalLine.amount == amountInstance.amount && !amountInstance.hide)
					{
						vitalLine.locText.SetText(vitalLine.amount.GetDescription(amountInstance));
						vitalLine.imageToggle.SetValue(amountInstance, vitalLine.tooltip);
						flag = true;
						if (!vitalLine.go.activeSelf)
						{
							vitalLine.go.SetActive(true);
						}
						break;
					}
				}
				if (!flag && vitalLine.go.activeSelf)
				{
					vitalLine.go.SetActive(false);
				}
			}
		}
		bool flag2 = false;
		for (int k = 0; k < this.checkboxLines.Count; k++)
		{
			MinionVitalsPanel.CheckboxLine checkboxLine = this.checkboxLines[k];
			MinionVitalsPanel.CheckboxLineDisplayType checkboxLineDisplayType = MinionVitalsPanel.CheckboxLineDisplayType.Hidden;
			if (this.checkboxLines[k].amount != null)
			{
				for (int l = 0; l < amounts.Count; l++)
				{
					AmountInstance amountInstance2 = amounts[l];
					if (checkboxLine.amount == amountInstance2.amount)
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
					flag2 = true;
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
				locText.text = ((!flag2) ? string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.DOMESTIC.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")) : string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC.BASE, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")));
				locText.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.VITALSSCREEN.CONDITIONS_GROWING.ADDITIONAL_DOMESTIC.TOOLTIP, GameUtil.GetFormattedCycles(component.GetComponent<Growing>().DomesticGrowthTime(), "F1")));
			}
			foreach (MinionVitalsPanel.VitalLine vitalLine2 in this.vitalsLines)
			{
				vitalLine2.go.SetActive(false);
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
		return UI.TOOLTIPS.VITALS_CHECKBOX_TEMPERATURE.text.Replace("{temperature}", GameUtil.GetFormattedTemperature(component.InternalTemperature, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true));
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

	private string GetSubmersionTooltip(GameObject go)
	{
		SubmersionMonitor component = go.GetComponent<SubmersionMonitor>();
		return (!component.IsCellSafe()) ? UI.TOOLTIPS.VITALS_CHECKBOX_SUBMERGED_FALSE : UI.TOOLTIPS.VITALS_CHECKBOX_SUBMERGED_TRUE;
	}

	private string GetDrowningTooltip(GameObject go)
	{
		DrowningMonitor component = go.GetComponent<DrowningMonitor>();
		return (!component.IsCellSafe(Grid.PosToCell(go))) ? UI.TOOLTIPS.VITALS_CHECKBOX_DROWNING_FALSE : UI.TOOLTIPS.VITALS_CHECKBOX_DROWNING_TRUE;
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
			GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_Low, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, false),
			" - ",
			GameUtil.GetFormattedTemperature(component.internalTemperatureWarning_High, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Absolute, true)
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

	private string GetSubmersionLabel(GameObject go)
	{
		return UI.VITALSSCREEN.SUBMERSION;
	}

	private string GetDrowningLabel(GameObject go)
	{
		return UI.VITALSSCREEN.NOT_DROWNING;
	}

	private bool check_pressure(GameObject go)
	{
		PressureVulnerable component = go.GetComponent<PressureVulnerable>();
		return !(component != null) || component.GetExternalPressureState == PressureVulnerable.PressureState.Normal || component.GetExternalPressureState == PressureVulnerable.PressureState.Perfect;
	}

	private bool check_temperature(GameObject go)
	{
		TemperatureVulnerable component = go.GetComponent<TemperatureVulnerable>();
		return !(component != null) || component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Normal || component.GetInternalTemperatureState == TemperatureVulnerable.TemperatureState.Perfect;
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

	private bool check_submersion(GameObject go)
	{
		SubmersionMonitor component = go.GetComponent<SubmersionMonitor>();
		return !(component != null) || component.IsCellSafe();
	}

	private bool check_drowning(GameObject go)
	{
		DrowningMonitor component = go.GetComponent<DrowningMonitor>();
		return !(component != null) || component.IsCellSafe(Grid.PosToCell(go));
	}

	public Sprite icon_stress;

	public Sprite icon_breath;

	public Sprite icon_stamina;

	public Sprite icon_calories;

	public Sprite icon_temperature;

	public Sprite icon_bladder;

	public Sprite icon_decor;

	public Sprite icon_hitpoints;

	public Sprite icon_maturity;

	public Sprite icon_disease;

	public GameObject LineItemPrefab;

	public GameObject CheckboxLinePrefab;

	public GameObject selectedEntity;

	public List<MinionVitalsPanel.VitalLine> vitalsLines = new List<MinionVitalsPanel.VitalLine>();

	public List<MinionVitalsPanel.CheckboxLine> checkboxLines = new List<MinionVitalsPanel.CheckboxLine>();

	public Transform conditionsContainerNormal;

	public Transform conditionsContainerAdditional;

	[DebuggerDisplay("{amount.Name}")]
	public struct VitalLine
	{
		public Amount amount;

		public GameObject go;

		public ValueTrendImageToggle imageToggle;

		public LocText locText;

		public Func<AmountInstance, string> tooltip;
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
