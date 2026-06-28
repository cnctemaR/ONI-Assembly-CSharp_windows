using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class VitalsTableScreen : TableScreen
{
	protected override void OnActivate()
	{
		this.has_default_duplicant_row = false;
		this.title = UI.VITALS;
		base.OnActivate();
		base.AddPortraitColumn("Portrait", new Action<MinionIdentity, GameObject>(base.on_load_portrait), null, true);
		base.AddButtonLabelColumn("Names", new Action<MinionIdentity, GameObject>(base.on_load_name_label), new Func<MinionIdentity, GameObject, string>(base.get_value_name_label), delegate(GameObject widget_go)
		{
			base.GetWidgetRow(widget_go).SelectMinion();
		}, delegate(GameObject widget_go)
		{
			base.GetWidgetRow(widget_go).SelectAndFocusMinion();
		}, new Comparison<MinionIdentity>(base.compare_rows_alphabetical), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_name), new Action<MinionIdentity, GameObject, ToolTip>(base.on_tooltip_sort_alphabetically), false);
		base.AddLabelColumn("Stress", new Action<MinionIdentity, GameObject>(this.on_load_stress), new Func<MinionIdentity, GameObject, string>(this.get_value_stress_label), new Comparison<MinionIdentity>(this.compare_rows_stress), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_stress), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_stress), 64, true);
		base.AddLabelColumn("FoodExpectations", new Action<MinionIdentity, GameObject>(this.on_load_food_expectations), new Func<MinionIdentity, GameObject, string>(this.get_value_food_expectations_label), new Comparison<MinionIdentity>(this.compare_rows_food_expectations), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_food_expectations), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_food_expectations), 128, true);
		base.AddLabelColumn("DecorExpectations", new Action<MinionIdentity, GameObject>(this.on_load_decor_expectations), new Func<MinionIdentity, GameObject, string>(this.get_value_decor_expectations_label), new Comparison<MinionIdentity>(this.compare_rows_decor_expectations), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_decor_expectations), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_decor_expectations), 128, true);
		base.AddLabelColumn("Fullness", new Action<MinionIdentity, GameObject>(this.on_load_fullness), new Func<MinionIdentity, GameObject, string>(this.get_value_fullness_label), new Comparison<MinionIdentity>(this.compare_rows_fullness), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_fullness), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_fullness), 96, true);
		base.AddLabelColumn("EatenToday", new Action<MinionIdentity, GameObject>(this.on_load_eaten_today), new Func<MinionIdentity, GameObject, string>(this.get_value_eaten_today_label), new Comparison<MinionIdentity>(this.compare_rows_eaten_today), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_eaten_today), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_eaten_today), 96, true);
		base.AddLabelColumn("Health", new Action<MinionIdentity, GameObject>(this.on_load_health), new Func<MinionIdentity, GameObject, string>(this.get_value_health_label), new Comparison<MinionIdentity>(this.compare_rows_health), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_health), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_health), 64, true);
		base.AddLabelColumn("Immunity", new Action<MinionIdentity, GameObject>(this.on_load_immunity), new Func<MinionIdentity, GameObject, string>(this.get_value_immunity_label), new Comparison<MinionIdentity>(this.compare_rows_immunity), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_immunity), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_immunity), 192, true);
	}

	private void on_load_stress(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if (minion != null)
		{
			componentInChildren.text = (base.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else if (widgetRow.isDefault)
		{
			componentInChildren.text = string.Empty;
		}
		else
		{
			componentInChildren.text = UI.CONSUMABLESSCREEN.STRESS;
		}
	}

	private string get_value_stress_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Amounts.Stress.Lookup(minion).GetValueString();
	}

	private int compare_rows_stress(MinionIdentity a, MinionIdentity b)
	{
		float value = Db.Get().Amounts.Stress.Lookup(a).value;
		float value2 = Db.Get().Amounts.Stress.Lookup(b).value;
		if (value > value2)
		{
			return -1;
		}
		if (value < value2)
		{
			return 1;
		}
		return 0;
	}

	protected void on_tooltip_stress(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
						tooltip.AddMultiStringTooltip(Db.Get().Amounts.Stress.Lookup(minion).GetTooltip(), null);
					}
				}
			}
		}
	}

	protected void on_tooltip_sort_stress(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Minion)
				{
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_STRESS, null);
			}
		}
	}

	private void on_load_food_expectations(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if (minion != null)
		{
			componentInChildren.text = (base.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else if (widgetRow.isDefault)
		{
			componentInChildren.text = string.Empty;
		}
		else
		{
			componentInChildren.text = UI.VITALSSCREEN.FOOD_EXPECTATIONS;
		}
	}

	private string get_value_food_expectations_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Attributes.FoodExpectation.Lookup(minion).GetFormattedValue();
	}

	private int compare_rows_food_expectations(MinionIdentity a, MinionIdentity b)
	{
		float totalValue = Db.Get().Attributes.FoodExpectation.Lookup(a).GetTotalValue();
		float totalValue2 = Db.Get().Attributes.FoodExpectation.Lookup(b).GetTotalValue();
		return totalValue.CompareTo(totalValue2);
	}

	protected void on_tooltip_food_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
						tooltip.AddMultiStringTooltip(string.Format(UI.VITALSSCREEN.FOOD_EXPECTATIONS_TOOLTIP, Db.Get().Attributes.FoodExpectation.Lookup(minion).GetFormattedValue()), null);
					}
				}
			}
		}
	}

	protected void on_tooltip_sort_food_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Minion)
				{
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, null);
			}
		}
	}

	private void on_load_decor_expectations(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if (minion != null)
		{
			componentInChildren.text = (base.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else if (widgetRow.isDefault)
		{
			componentInChildren.text = string.Empty;
		}
		else
		{
			componentInChildren.text = UI.VITALSSCREEN.DECOR_EXPECTATIONS;
		}
	}

	private string get_value_decor_expectations_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Attributes.DecorExpectation.Lookup(minion).GetFormattedValue();
	}

	private int compare_rows_decor_expectations(MinionIdentity a, MinionIdentity b)
	{
		float totalValue = Db.Get().Attributes.DecorExpectation.Lookup(a).GetTotalValue();
		float totalValue2 = Db.Get().Attributes.DecorExpectation.Lookup(b).GetTotalValue();
		return totalValue.CompareTo(totalValue2);
	}

	protected void on_tooltip_decor_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
						tooltip.AddMultiStringTooltip(string.Format(UI.VITALSSCREEN.DECOR_EXPECTATIONS_TOOLTIP, Db.Get().Attributes.DecorExpectation.Lookup(minion).GetFormattedValue()), null);
					}
				}
			}
		}
	}

	protected void on_tooltip_sort_decor_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Minion)
				{
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, null);
			}
		}
	}

	private void on_load_health(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if (minion != null)
		{
			componentInChildren.text = (base.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else if (widgetRow.isDefault)
		{
			componentInChildren.text = string.Empty;
		}
		else
		{
			componentInChildren.text = UI.VITALSSCREEN_HEALTH;
		}
	}

	private string get_value_health_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Amounts.HitPoints.Lookup(minion).GetValueString();
	}

	private int compare_rows_health(MinionIdentity a, MinionIdentity b)
	{
		float value = Db.Get().Amounts.HitPoints.Lookup(a).value;
		float value2 = Db.Get().Amounts.HitPoints.Lookup(b).value;
		if (value > value2)
		{
			return -1;
		}
		if (value < value2)
		{
			return 1;
		}
		return 0;
	}

	protected void on_tooltip_health(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
						tooltip.AddMultiStringTooltip(Db.Get().Amounts.HitPoints.Lookup(minion).GetTooltip(), null);
					}
				}
			}
		}
	}

	protected void on_tooltip_sort_health(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Minion)
				{
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_HITPOINTS, null);
			}
		}
	}

	private void on_load_immunity(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if (minion != null)
		{
			componentInChildren.text = (base.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else if (widgetRow.isDefault)
		{
			componentInChildren.text = string.Empty;
		}
		else
		{
			componentInChildren.text = UI.VITALSSCREEN_IMMUNITY;
		}
	}

	private string get_value_immunity_label(MinionIdentity minion, GameObject widget_go)
	{
		Diseases diseases = minion.GetComponent<MinionModifiers>().diseases;
		if (diseases.IsInfected())
		{
			string text = string.Empty;
			if (diseases.Count > 1)
			{
				float num = 0f;
				foreach (DiseaseInstance diseaseInstance in diseases)
				{
					num = Mathf.Min(new float[] { diseaseInstance.GetInfectedTimeRemaining() });
				}
				text += string.Format(UI.VITALSSCREEN.IMMUNITY_MULTIPLE_DISEASES, GameUtil.GetFormattedCycles(num, "F1"));
			}
			else
			{
				foreach (DiseaseInstance diseaseInstance2 in diseases)
				{
					if (!string.IsNullOrEmpty(text))
					{
						text += "\n";
					}
					text += string.Format(UI.VITALSSCREEN.IMMUNITY_DISEASE, diseaseInstance2.modifier.Name, GameUtil.GetFormattedCycles(diseaseInstance2.GetInfectedTimeRemaining(), "F1"));
				}
			}
			return text;
		}
		return Db.Get().Amounts.ImmuneLevel.Lookup(minion).GetValueString();
	}

	private int compare_rows_immunity(MinionIdentity a, MinionIdentity b)
	{
		float value = Db.Get().Amounts.ImmuneLevel.Lookup(a).value;
		float value2 = Db.Get().Amounts.ImmuneLevel.Lookup(b).value;
		if (value > value2)
		{
			return -1;
		}
		if (value < value2)
		{
			return 1;
		}
		return 0;
	}

	protected void on_tooltip_immunity(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
						Diseases diseases = minion.GetComponent<MinionModifiers>().diseases;
						if (diseases.IsInfected())
						{
							foreach (DiseaseInstance diseaseInstance in diseases)
							{
								tooltip.AddMultiStringTooltip(UI.HORIZONTAL_RULE, null);
								tooltip.AddMultiStringTooltip(diseaseInstance.modifier.Name, null);
								StatusItem statusItem = diseaseInstance.GetStatusItem();
								tooltip.AddMultiStringTooltip(statusItem.GetTooltip(diseaseInstance.ExposureInfo), null);
							}
						}
						else
						{
							tooltip.AddMultiStringTooltip(Db.Get().Amounts.ImmuneLevel.Lookup(minion).GetTooltip(), null);
						}
					}
				}
			}
		}
	}

	protected void on_tooltip_sort_immunity(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Minion)
				{
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_IMMUNEPOINTS, null);
			}
		}
	}

	private void on_load_fullness(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if (minion != null)
		{
			componentInChildren.text = (base.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else if (widgetRow.isDefault)
		{
			componentInChildren.text = string.Empty;
		}
		else
		{
			componentInChildren.text = UI.VITALSSCREEN_CALORIES;
		}
	}

	private string get_value_fullness_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Amounts.Calories.Lookup(minion).GetValueString();
	}

	private int compare_rows_fullness(MinionIdentity a, MinionIdentity b)
	{
		float value = Db.Get().Amounts.Calories.Lookup(a).value;
		float value2 = Db.Get().Amounts.Calories.Lookup(b).value;
		if (value > value2)
		{
			return -1;
		}
		if (value < value2)
		{
			return 1;
		}
		return 0;
	}

	protected void on_tooltip_fullness(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
						tooltip.AddMultiStringTooltip(Db.Get().Amounts.Calories.Lookup(minion).GetTooltip(), null);
					}
				}
			}
		}
	}

	protected void on_tooltip_sort_fullness(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Minion)
				{
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_FULLNESS, null);
			}
		}
	}

	protected void on_tooltip_name(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, minion.GetProperName()), null);
					}
				}
			}
		}
	}

	private void on_load_eaten_today(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
		if (minion != null)
		{
			componentInChildren.text = (base.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
		}
		else if (widgetRow.isDefault)
		{
			componentInChildren.text = string.Empty;
		}
		else
		{
			componentInChildren.text = UI.VITALSSCREEN_EATENTODAY;
		}
	}

	private string get_value_eaten_today_label(MinionIdentity minion, GameObject widget_go)
	{
		return GameUtil.GetFormattedCalories(minion.GetSMI<RationMonitor.Instance>().GetRationsAteToday(), GameUtil.TimeSlice.None, true);
	}

	private int compare_rows_eaten_today(MinionIdentity a, MinionIdentity b)
	{
		float rationsAteToday = a.GetSMI<RationMonitor.Instance>().GetRationsAteToday();
		float rationsAteToday2 = b.GetSMI<RationMonitor.Instance>().GetRationsAteToday();
		if (rationsAteToday > rationsAteToday2)
		{
			return -1;
		}
		if (rationsAteToday < rationsAteToday2)
		{
			return 1;
		}
		return 0;
	}

	protected void on_tooltip_eaten_today(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.DUPLICANT_PROPERNAME, minion.GetProperName()), null);
						tooltip.AddMultiStringTooltip(string.Format(UI.VITALSSCREEN.EATEN_TODAY_TOOLTIP, GameUtil.GetFormattedCalories(minion.GetSMI<RationMonitor.Instance>().GetRationsAteToday(), GameUtil.TimeSlice.None, true)), null);
					}
				}
			}
		}
	}

	protected void on_tooltip_sort_eaten_today(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Minion)
				{
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EATEN_TODAY, null);
			}
		}
	}
}
