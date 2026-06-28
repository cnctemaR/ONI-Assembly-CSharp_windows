using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class JobsTableScreen : TableScreen
{
	public void SetCurrentSortChoreGroup(ChoreGroup chore_group)
	{
		this.current_sort_choregroup = chore_group;
	}

	protected override void OnActivate()
	{
		this.title = UI.JOBSSCREEN.TITLE;
		base.OnActivate();
		base.AddPortraitColumn("Portrait", new Action<MinionIdentity, GameObject>(base.on_load_portrait), null);
		base.AddButtonLabelColumn("Names", new Action<MinionIdentity, GameObject>(base.on_load_name_label), new Func<MinionIdentity, GameObject, string>(base.get_value_name_label), delegate(GameObject widget_go)
		{
			base.GetWidgetRow(widget_go).SelectMinion();
		}, delegate(GameObject widget_go)
		{
			base.GetWidgetRow(widget_go).SelectAndFocusMinion();
		}, new Comparison<MinionIdentity>(base.compare_rows_alphabetical), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_name), new Action<MinionIdentity, GameObject, ToolTip>(base.on_tooltip_sort_alphabetically));
		ChoreGroupTableColumn[] array = new ChoreGroupTableColumn[Db.Get().ChoreGroups.Count];
		for (int i = 0; i < Db.Get().ChoreGroups.Count; i++)
		{
			array[i] = this.AddChoreGroupColumn(Db.Get().ChoreGroups[i].Id, Db.Get().ChoreGroups[i], new Action<MinionIdentity, GameObject>(this.on_load_value_choregroup), new Func<MinionIdentity, GameObject, TableScreen.ResultValues>(this.get_value_choregroup), new Action<GameObject>(this.on_press_choregroup), new Action<GameObject, bool>(this.set_value_choregroup), new Comparison<MinionIdentity>(this.compare_chore_group), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_chore_group), new Action<MinionIdentity, GameObject, ToolTip>(this.on_sort_tooltip_chore_group));
		}
		base.AddSuperCheckboxColumn("SuperCheckJobs", array, new Action<MinionIdentity, GameObject>(base.on_load_value_checkbox_column_super), new Func<MinionIdentity, GameObject, TableScreen.ResultValues>(this.get_value_checkbox_column_super), new Action<GameObject>(base.on_press_checkbox_column_super), new Action<GameObject, bool>(base.set_value_checkbox_column_super), null, new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_chore_group_super));
		this.RefreshEffectListeners();
	}

	protected override void RefreshRows()
	{
		base.RefreshRows();
		this.RefreshEffectListeners();
	}

	private void RefreshEffectListeners()
	{
		for (int i = 0; i < this.EffectListeners.Count; i++)
		{
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.level_up);
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.effect_added);
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.effect_removed);
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.disease_added);
			this.EffectListeners[i].Key.Unsubscribe(this.EffectListeners[i].Value.effect_added);
		}
		this.EffectListeners.Clear();
		for (int j = 0; j < Components.LiveMinionIdentities.Count; j++)
		{
			JobsTableScreen.SkillEventHandlerID skillEventHandlerID = default(JobsTableScreen.SkillEventHandlerID);
			MinionIdentity id = Components.LiveMinionIdentities[j];
			skillEventHandlerID.level_up = Components.LiveMinionIdentities[j].gameObject.Subscribe(-110704193, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id);
			});
			skillEventHandlerID.effect_added = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1901442097, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id);
			});
			skillEventHandlerID.effect_removed = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1157678353, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id);
			});
			skillEventHandlerID.disease_added = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1089020, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id);
			});
			skillEventHandlerID.disease_cured = Components.LiveMinionIdentities[j].gameObject.Subscribe(-1516186173, delegate(object o)
			{
				this.MarkSingleMinionRowDirty(id);
			});
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (this.dirty_single_minion_rows.Count != 0)
		{
			foreach (MinionIdentity minionIdentity in this.dirty_single_minion_rows)
			{
				if (!(minionIdentity == null))
				{
					this.RefreshSingleMinionRow(minionIdentity);
				}
			}
			this.dirty_single_minion_rows.Clear();
		}
	}

	protected void MarkSingleMinionRowDirty(MinionIdentity id)
	{
		this.dirty_single_minion_rows.Add(id);
	}

	private void RefreshSingleMinionRow(MinionIdentity id)
	{
		foreach (KeyValuePair<string, TableColumn> keyValuePair in this.columns)
		{
			foreach (KeyValuePair<TableRow, GameObject> keyValuePair2 in keyValuePair.Value.widgets_by_row)
			{
				if (!(keyValuePair2.Value == null))
				{
					if (!(keyValuePair2.Key.GetMinionIdentity() != id))
					{
						keyValuePair.Value.on_load_action(id, keyValuePair2.Value);
					}
				}
			}
		}
	}

	private void set_value_choregroup(GameObject widget_go, bool new_value)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		if (widgetRow == null)
		{
			global::Debug.LogWarning("Row is null", null);
			return;
		}
		ChoreGroupTableColumn choreGroupTableColumn = base.GetWidgetColumn(widget_go) as ChoreGroupTableColumn;
		MinionIdentity minionIdentity = widgetRow.GetMinionIdentity();
		ChoreGroup chore_group = choreGroupTableColumn.chore_group;
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			this.set_value_choregroup(this.default_row.GetComponent<TableRow>().GetWidget(choreGroupTableColumn), new_value);
			base.StartCoroutine(base.CascadeSetColumnCheckBoxes(this.sortable_rows, choreGroupTableColumn, new_value, widget_go));
			break;
		case TableRow.RowType.Default:
			if (new_value)
			{
				ChoreGroupManager.instance.DefaultForbiddenTagsList.Remove(chore_group.Id.ToTag());
			}
			else
			{
				ChoreGroupManager.instance.DefaultForbiddenTagsList.Add(chore_group.Id.ToTag());
			}
			choreGroupTableColumn.on_load_action(minionIdentity, widget_go);
			foreach (KeyValuePair<TableRow, GameObject> keyValuePair in choreGroupTableColumn.widgets_by_row)
			{
				if (keyValuePair.Key.rowType == TableRow.RowType.Header)
				{
					choreGroupTableColumn.on_load_action(null, keyValuePair.Value);
					break;
				}
			}
			break;
		case TableRow.RowType.Minion:
			if (minionIdentity != null)
			{
				ChoreConsumer component = minionIdentity.GetComponent<ChoreConsumer>();
				if (component == null)
				{
					global::Debug.LogError("Could not find minion identity / row associated with the widget", null);
					return;
				}
				component.SetPermitted(chore_group, new_value);
				choreGroupTableColumn.on_load_action(widgetRow.GetMinionIdentity(), widget_go);
				foreach (KeyValuePair<TableRow, GameObject> keyValuePair2 in choreGroupTableColumn.widgets_by_row)
				{
					if (keyValuePair2.Key.rowType == TableRow.RowType.Header)
					{
						choreGroupTableColumn.on_load_action(null, keyValuePair2.Value);
						break;
					}
				}
			}
			break;
		}
	}

	private void on_press_choregroup(GameObject widget_go)
	{
		ChoreGroupTableColumn choreGroupTableColumn = base.GetWidgetColumn(widget_go) as ChoreGroupTableColumn;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		MinionIdentity minionIdentity = widgetRow.GetMinionIdentity();
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
		{
			int num = (int)this.get_value_choregroup(null, widget_go);
			choreGroupTableColumn.on_set_action(widget_go, num != 2);
			choreGroupTableColumn.on_load_action(null, widget_go);
			break;
		}
		case TableRow.RowType.Default:
		{
			ChoreGroup choreGroup = choreGroupTableColumn.chore_group;
			bool flag = !ChoreGroupManager.instance.DefaultForbiddenTagsList.Contains(choreGroup.Id.ToTag());
			choreGroupTableColumn.on_set_action(widget_go, !flag);
			break;
		}
		case TableRow.RowType.Minion:
			if (minionIdentity != null)
			{
				ChoreConsumer component = minionIdentity.GetComponent<ChoreConsumer>();
				if (component == null)
				{
					global::Debug.LogError("Could not find minion identity / row associated with the widget", null);
					return;
				}
				ChoreGroup choreGroup = choreGroupTableColumn.chore_group;
				choreGroupTableColumn.on_set_action(widget_go, !component.IsPermitted(choreGroup));
			}
			break;
		}
	}

	private void on_load_value_choregroup(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableColumn widgetColumn = base.GetWidgetColumn(widget_go);
		ChoreGroup chore_group = (widgetColumn as ChoreGroupTableColumn).chore_group;
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
		{
			Image image = widget_go.GetComponent<HierarchyReferences>().GetReference("PortraitImage") as Image;
			image.rectTransform.sizeDelta = new Vector2(0f, -14f);
			MultiToggle multiToggle = widget_go.GetComponent<HierarchyReferences>().GetReference("Toggle") as MultiToggle;
			multiToggle.ChangeState((int)this.get_value_choregroup(minion, widget_go));
			break;
		}
		case TableRow.RowType.Default:
		{
			KToggle ktoggle = widget_go.GetComponent<KToggle>();
			ktoggle.isOn = this.get_value_choregroup(minion, widget_go) == TableScreen.ResultValues.True;
			break;
		}
		case TableRow.RowType.Minion:
			if (minion != null && widgetRow.GetMinionIdentity().GetComponent<ChoreConsumer>().IsEnabled(chore_group))
			{
				KToggle ktoggle = widget_go.GetComponent<KToggle>();
				ktoggle.isOn = this.get_value_choregroup(minion, widget_go) == TableScreen.ResultValues.True;
				Image image2 = widget_go.GetComponent<HierarchyReferences>().GetReference("BGImage") as Image;
				AttributeInstance attributeInstance = minion.GetAttributes().Get((base.GetWidgetColumn(widget_go) as ChoreGroupTableColumn).chore_group.attribute);
				float num = Mathf.Min(attributeInstance.GetTotalValue() / 10f, 1f);
				Color color = new Color(0.72156864f, 0.44313726f, 0.5803922f, num);
				image2.color = color;
			}
			break;
		}
	}

	protected void on_tooltip_name(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, minion.GetProperName()), null);
			}
			break;
		}
	}

	private TableScreen.ResultValues get_value_choregroup(MinionIdentity minion, GameObject widget_go)
	{
		ChoreGroupTableColumn choreGroupTableColumn = base.GetWidgetColumn(widget_go) as ChoreGroupTableColumn;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableScreen.ResultValues resultValues = TableScreen.ResultValues.False;
		ChoreGroup chore_group = choreGroupTableColumn.chore_group;
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = false;
			foreach (KeyValuePair<TableRow, GameObject> keyValuePair in choreGroupTableColumn.widgets_by_row)
			{
				GameObject value = keyValuePair.Value;
				if (!(value == widget_go))
				{
					if (!(value == null))
					{
						if (!(keyValuePair.Key.GetMinionIdentity() != null) || keyValuePair.Key.GetMinionIdentity().GetComponent<ChoreConsumer>().IsEnabled(chore_group))
						{
							switch (choreGroupTableColumn.get_value_action(keyValuePair.Key.GetMinionIdentity(), value))
							{
							case TableScreen.ResultValues.False:
								flag2 = false;
								if (!flag)
								{
									flag4 = true;
								}
								break;
							case TableScreen.ResultValues.Partial:
								flag3 = true;
								flag4 = true;
								break;
							case TableScreen.ResultValues.True:
								flag = false;
								if (!flag2)
								{
									flag4 = true;
								}
								break;
							}
							if (flag4)
							{
								break;
							}
						}
					}
				}
			}
			if (flag3)
			{
				resultValues = TableScreen.ResultValues.Partial;
			}
			else if (flag2)
			{
				resultValues = TableScreen.ResultValues.True;
			}
			else if (flag)
			{
				resultValues = TableScreen.ResultValues.False;
			}
			else
			{
				resultValues = TableScreen.ResultValues.Partial;
			}
			break;
		}
		case TableRow.RowType.Default:
			resultValues = (ChoreGroupManager.instance.DefaultForbiddenTagsList.Contains(chore_group.Id.ToTag()) ? TableScreen.ResultValues.False : TableScreen.ResultValues.True);
			break;
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				ChoreConsumer component = minion.GetComponent<ChoreConsumer>();
				resultValues = ((!component.IsPermitted(chore_group)) ? TableScreen.ResultValues.False : TableScreen.ResultValues.True);
			}
			else
			{
				global::Debug.Log("Minion is null :( ", null);
			}
			break;
		}
		return resultValues;
	}

	private int compare_chore_group(MinionIdentity a, MinionIdentity b)
	{
		if (a.gameObject.GetAttributes().GetValue(this.current_sort_choregroup.attribute.Id) > b.gameObject.GetAttributes().GetValue(this.current_sort_choregroup.attribute.Id))
		{
			return -1;
		}
		if (a.gameObject.GetAttributes().GetValue(this.current_sort_choregroup.attribute.Id) < b.gameObject.GetAttributes().GetValue(this.current_sort_choregroup.attribute.Id))
		{
			return 1;
		}
		return 0;
	}

	private void on_tooltip_chore_group(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		ChoreGroupTableColumn choreGroupTableColumn = base.GetWidgetColumn(widget_go) as ChoreGroupTableColumn;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(string.Format(UI.JOBSSCREEN.TOOLTIP_TOGGLE_COLUMN, choreGroupTableColumn.chore_group.Name), null);
			tooltip.AddMultiStringTooltip("\n" + Strings.Get("STRINGS.DUPLICANTS.CHOREGROUPS." + choreGroupTableColumn.chore_group.Id.ToUpper() + ".DESC"), null);
			break;
		case TableRow.RowType.Default:
			if (choreGroupTableColumn.get_value_action(minion, widget_go) == TableScreen.ResultValues.True)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.JOBSSCREEN.NEW_MINIONS_JOB_PERMISSION_ON, choreGroupTableColumn.chore_group.Name), null);
			}
			else
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.JOBSSCREEN.NEW_MINIONS_JOB_PERMISSION_OFF, choreGroupTableColumn.chore_group.Name), null);
			}
			break;
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				if (choreGroupTableColumn.get_value_action(minion, widget_go) == TableScreen.ResultValues.True)
				{
					tooltip.AddMultiStringTooltip(string.Format(UI.JOBSSCREEN.JOB_PERMISSION_ON, minion.GetProperName(), choreGroupTableColumn.chore_group.Name), null);
				}
				else
				{
					tooltip.AddMultiStringTooltip(string.Format(UI.JOBSSCREEN.JOB_PERMISSION_OFF, minion.GetProperName(), choreGroupTableColumn.chore_group.Name), null);
				}
				Klei.AI.Attribute attribute = choreGroupTableColumn.chore_group.attribute;
				AttributeInstance attributeInstance = minion.GetAttributes().Get((base.GetWidgetColumn(widget_go) as ChoreGroupTableColumn).chore_group.attribute);
				float totalValue = attributeInstance.GetTotalValue();
				TextStyleSetting textStyleSetting = this.TooltipTextStyle_Ability;
				if (totalValue > 0f)
				{
					textStyleSetting = this.TooltipTextStyle_AbilityPositiveModifier;
				}
				else if (totalValue < 0f)
				{
					textStyleSetting = this.TooltipTextStyle_AbilityNegativeModifier;
				}
				tooltip.AddMultiStringTooltip(string.Concat(new object[]
				{
					"\n",
					attribute.Name,
					" ",
					attributeInstance.GetTotalValue()
				}), textStyleSetting);
			}
			break;
		}
	}

	private void on_sort_tooltip_chore_group(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		ChoreGroupTableColumn choreGroupTableColumn = base.GetWidgetColumn(widget_go) as ChoreGroupTableColumn;
		tooltip.ClearMultiStringTooltip();
		tooltip.AddMultiStringTooltip(string.Format(UI.JOBSSCREEN.COLUMN_SORT_BY_JOB_SKILL, choreGroupTableColumn.chore_group.Name), null);
	}

	private void on_tooltip_chore_group_super(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.JOBSSCREEN.TOOLTIP_TOGGLE_ALL, null);
			break;
		case TableRow.RowType.Default:
			tooltip.AddMultiStringTooltip(UI.JOBSSCREEN.NEW_MINIONS_TOOLTIP_TOGGLE_ROW, null);
			break;
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.JOBSSCREEN.TOOLTIP_TOGGLE_ROW, minion.gameObject.GetProperName()), null);
			}
			break;
		}
	}

	protected ChoreGroupTableColumn AddChoreGroupColumn(string id, ChoreGroup chore_group, Action<MinionIdentity, GameObject> on_load_value_action, Func<MinionIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, bool> set_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip)
	{
		ChoreGroupTableColumn choreGroupTableColumn = new ChoreGroupTableColumn(chore_group, on_load_value_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip, (GameObject widget_go) => chore_group.Name);
		if (base.RegisterColumn(id, choreGroupTableColumn))
		{
			return choreGroupTableColumn;
		}
		return null;
	}

	public override void SetSortComparison(Comparison<MinionIdentity> comparison, TableColumn sort_column)
	{
		if (comparison == null)
		{
			this.SetCurrentSortChoreGroup(null);
			return;
		}
		if (this.active_sort_column == sort_column && this.sort_is_reversed)
		{
			this.SetCurrentSortChoreGroup(null);
		}
		base.SetSortComparison(comparison, sort_column);
	}

	public override TableScreen.ResultValues get_value_checkbox_column_super(MinionIdentity minion, GameObject widget_go)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = base.GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		bool flag4 = false;
		foreach (CheckboxTableColumn checkboxTableColumn in superCheckboxTableColumn.columns_affected)
		{
			ChoreGroup chore_group = (checkboxTableColumn as ChoreGroupTableColumn).chore_group;
			if (!(widgetRow.GetMinionIdentity() != null) || widgetRow.GetMinionIdentity().GetComponent<ChoreConsumer>().IsEnabled(chore_group))
			{
				switch (checkboxTableColumn.get_value_action(widgetRow.GetMinionIdentity(), widgetRow.GetWidget(checkboxTableColumn)))
				{
				case TableScreen.ResultValues.False:
					flag2 = false;
					if (!flag)
					{
						flag4 = true;
					}
					break;
				case TableScreen.ResultValues.Partial:
					flag3 = true;
					flag4 = true;
					break;
				case TableScreen.ResultValues.True:
					flag = false;
					if (!flag2)
					{
						flag4 = true;
					}
					break;
				}
				if (flag4)
				{
					break;
				}
			}
		}
		if (flag3)
		{
			return TableScreen.ResultValues.Partial;
		}
		if (flag2)
		{
			return TableScreen.ResultValues.True;
		}
		if (flag)
		{
			return TableScreen.ResultValues.False;
		}
		return TableScreen.ResultValues.Partial;
	}

	public TextStyleSetting TooltipTextStyle_Ability;

	public TextStyleSetting TooltipTextStyle_AbilityPositiveModifier;

	public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;

	private List<MinionIdentity> dirty_single_minion_rows = new List<MinionIdentity>();

	private ChoreGroup current_sort_choregroup;

	private List<KeyValuePair<GameObject, JobsTableScreen.SkillEventHandlerID>> EffectListeners = new List<KeyValuePair<GameObject, JobsTableScreen.SkillEventHandlerID>>();

	private struct SkillEventHandlerID
	{
		public int level_up;

		public int effect_added;

		public int effect_removed;

		public int disease_added;

		public int disease_cured;
	}
}
