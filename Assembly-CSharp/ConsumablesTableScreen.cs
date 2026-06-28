using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;
using UnityEngine;
using UnityEngine.UI;

public class ConsumablesTableScreen : TableScreen
{
	protected override void OnActivate()
	{
		this.title = UI.CONSUMABLESSCREEN.TITLE;
		base.OnActivate();
		base.AddPortraitColumn("Portrait", new Action<MinionIdentity, GameObject>(base.on_load_portrait), null);
		base.AddButtonLabelColumn("Names", new Action<MinionIdentity, GameObject>(base.on_load_name_label), new Func<MinionIdentity, GameObject, string>(base.get_value_name_label), delegate(GameObject widget_go)
		{
			base.GetWidgetRow(widget_go).SelectMinion();
		}, delegate(GameObject widget_go)
		{
			base.GetWidgetRow(widget_go).SelectAndFocusMinion();
		}, new Comparison<MinionIdentity>(base.compare_rows_alphabetical), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_name), new Action<MinionIdentity, GameObject, ToolTip>(base.on_tooltip_sort_alphabetically));
		base.AddLabelColumn("Expectations", new Action<MinionIdentity, GameObject>(this.on_load_expectations), new Func<MinionIdentity, GameObject, string>(this.get_value_expectations_label), new Comparison<MinionIdentity>(this.compare_rows_expectations), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_expectations), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_expectations), 128);
		base.AddLabelColumn("Stress", new Action<MinionIdentity, GameObject>(this.on_load_stress), new Func<MinionIdentity, GameObject, string>(this.get_value_stress_label), new Comparison<MinionIdentity>(this.compare_rows_stress), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_stress), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_stress), 64);
		List<EdiblesManager.FoodInfo> list = new List<EdiblesManager.FoodInfo>();
		for (int i = 0; i < FOOD.FOOD_TYPES_LIST.Count; i++)
		{
			list.Add(FOOD.FOOD_TYPES_LIST[i]);
		}
		list.Sort(delegate(EdiblesManager.FoodInfo a, EdiblesManager.FoodInfo b)
		{
			if (a.Quality > b.Quality)
			{
				return 1;
			}
			if (a.Quality < b.Quality)
			{
				return -1;
			}
			if (a.CaloriesPerUnit > b.CaloriesPerUnit)
			{
				return 1;
			}
			if (a.CaloriesPerUnit < b.CaloriesPerUnit)
			{
				return -1;
			}
			return 0;
		});
		List<FoodInfoTableColumn> list2 = new List<FoodInfoTableColumn>();
		int num = 0;
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j].CaloriesPerUnit != 0f)
			{
				if (list[j].Quality != num && j != 0)
				{
					string text = "QualityDivider_" + list[j].Quality;
					DividerColumn dividerColumn = new DividerColumn();
					base.RegisterColumn(text, dividerColumn);
				}
				list2.Add(this.AddFoodInfoColumn(list[j].Id, list[j], new Action<MinionIdentity, GameObject>(this.on_load_food_info), new Func<MinionIdentity, GameObject, TableScreen.ResultValues>(this.get_value_food_info), new Action<GameObject>(this.on_click_food_info), new Action<GameObject, bool>(this.set_value_food_info), new Comparison<MinionIdentity>(this.compare_food_info), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_food_info), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_food_info)));
				num = list[j].Quality;
			}
		}
		string text2 = "QualityDivider_Final";
		DividerColumn dividerColumn2 = new DividerColumn();
		base.RegisterColumn(text2, dividerColumn2);
		base.AddSuperCheckboxColumn("SuperCheckFood", list2.ToArray(), new Action<MinionIdentity, GameObject>(base.on_load_value_checkbox_column_super), new Func<MinionIdentity, GameObject, TableScreen.ResultValues>(this.get_value_checkbox_column_super), new Action<GameObject>(base.on_press_checkbox_column_super), new Action<GameObject, bool>(base.set_value_checkbox_column_super), null, new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_food_info_super));
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
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				tooltip.AddMultiStringTooltip(Db.Get().Amounts.Stress.Lookup(minion).GetValueString(), null);
			}
			break;
		}
	}

	protected void on_tooltip_sort_stress(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_STRESS, null);
			break;
		}
	}

	private void on_load_expectations(MinionIdentity minion, GameObject widget_go)
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
			componentInChildren.text = UI.CONSUMABLESSCREEN.FOOD_EXPECTATIONS;
		}
	}

	private string get_value_expectations_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Attributes.FoodExpectation.Lookup(minion).GetFormattedValue(false);
	}

	private int compare_rows_expectations(MinionIdentity a, MinionIdentity b)
	{
		float totalValue = Db.Get().Attributes.FoodExpectation.Lookup(a).GetTotalValue();
		float totalValue2 = Db.Get().Attributes.FoodExpectation.Lookup(b).GetTotalValue();
		if (totalValue > totalValue2)
		{
			return 1;
		}
		if (totalValue < totalValue2)
		{
			return -1;
		}
		return 0;
	}

	protected void on_tooltip_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				tooltip.AddMultiStringTooltip(Db.Get().Attributes.FoodExpectation.Lookup(minion).GetFormattedValue(false), null);
			}
			break;
		}
	}

	protected void on_tooltip_sort_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, null);
			break;
		}
	}

	private TableScreen.ResultValues get_value_food_info_super(MinionIdentity minion, GameObject widget_go)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = base.GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		bool flag4 = false;
		foreach (CheckboxTableColumn checkboxTableColumn in superCheckboxTableColumn.columns_affected)
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

	private void set_value_food_info(GameObject widget_go, bool new_value)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		if (widgetRow == null)
		{
			Debug.LogWarning("Row is null");
			return;
		}
		FoodInfoTableColumn foodInfoTableColumn = base.GetWidgetColumn(widget_go) as FoodInfoTableColumn;
		MinionIdentity minionIdentity = widgetRow.GetMinionIdentity();
		EdiblesManager.FoodInfo food_info = foodInfoTableColumn.food_info;
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			this.set_value_food_info(this.default_row.GetComponent<TableRow>().GetWidget(foodInfoTableColumn), new_value);
			base.StartCoroutine(base.CascadeSetColumnCheckBoxes(this.sortable_rows, foodInfoTableColumn, new_value, widget_go));
			break;
		case TableRow.RowType.Default:
			if (new_value)
			{
				ConsumerManager.instance.DefaultForbiddenTagsList.Remove(food_info.Id.ToTag());
			}
			else
			{
				ConsumerManager.instance.DefaultForbiddenTagsList.Add(food_info.Id.ToTag());
			}
			foodInfoTableColumn.on_load_action(minionIdentity, widget_go);
			foreach (KeyValuePair<TableRow, GameObject> keyValuePair in foodInfoTableColumn.widgets_by_row)
			{
				if (keyValuePair.Key.rowType == TableRow.RowType.Header)
				{
					foodInfoTableColumn.on_load_action(null, keyValuePair.Value);
					break;
				}
			}
			break;
		case TableRow.RowType.Minion:
			if (minionIdentity != null)
			{
				ConsumableConsumer component = minionIdentity.GetComponent<ConsumableConsumer>();
				if (component == null)
				{
					Debug.LogError("Could not find minion identity / row associated with the widget");
					return;
				}
				component.SetPermitted(food_info.Id, new_value);
				foodInfoTableColumn.on_load_action(widgetRow.GetMinionIdentity(), widget_go);
				foreach (KeyValuePair<TableRow, GameObject> keyValuePair2 in foodInfoTableColumn.widgets_by_row)
				{
					if (keyValuePair2.Key.rowType == TableRow.RowType.Header)
					{
						foodInfoTableColumn.on_load_action(null, keyValuePair2.Value);
						break;
					}
				}
			}
			break;
		}
	}

	private void on_click_food_info(GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		MinionIdentity minionIdentity = widgetRow.GetMinionIdentity();
		FoodInfoTableColumn foodInfoTableColumn = base.GetWidgetColumn(widget_go) as FoodInfoTableColumn;
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
		{
			bool flag = this.get_value_food_info(null, widget_go) == TableScreen.ResultValues.True;
			foodInfoTableColumn.on_set_action(widget_go, !flag);
			foodInfoTableColumn.on_load_action(null, widget_go);
			break;
		}
		case TableRow.RowType.Default:
		{
			EdiblesManager.FoodInfo foodInfo = foodInfoTableColumn.food_info;
			bool flag2 = !ConsumerManager.instance.DefaultForbiddenTagsList.Contains(foodInfo.Id.ToTag());
			foodInfoTableColumn.on_set_action(widget_go, !flag2);
			break;
		}
		case TableRow.RowType.Minion:
			if (minionIdentity != null)
			{
				ConsumableConsumer component = minionIdentity.GetComponent<ConsumableConsumer>();
				if (component == null)
				{
					Debug.LogError("Could not find minion identity / row associated with the widget");
					return;
				}
				EdiblesManager.FoodInfo foodInfo = foodInfoTableColumn.food_info;
				foodInfoTableColumn.on_set_action(widget_go, !component.IsPermitted(foodInfo.Id));
			}
			break;
		}
	}

	private void on_tooltip_food_info(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		FoodInfoTableColumn foodInfoTableColumn = base.GetWidgetColumn(widget_go) as FoodInfoTableColumn;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(foodInfoTableColumn.food_info.Name, null);
			tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_AVAILABLE, GameUtil.GetFormattedCalories(WorldInventory.Instance.GetAmount(foodInfoTableColumn.food_info.Id.ToTag()) * EdiblesManager.instance.GetFoodInfo(foodInfoTableColumn.food_info.Id).CaloriesPerUnit, GameUtil.TimeSlice.None, true)), null);
			tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_QUALITY, foodInfoTableColumn.food_info.Quality), null);
			break;
		case TableRow.RowType.Default:
			if (foodInfoTableColumn.get_value_action(minion, widget_go) == TableScreen.ResultValues.True)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.NEW_MINIONS_FOOD_PERMISSION_ON, foodInfoTableColumn.food_info.Name), null);
			}
			else
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.NEW_MINIONS_FOOD_PERMISSION_OFF, foodInfoTableColumn.food_info.Name), null);
			}
			break;
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				if (foodInfoTableColumn.get_value_action(minion, widget_go) == TableScreen.ResultValues.True)
				{
					tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_PERMISSION_ON, minion.GetProperName(), foodInfoTableColumn.food_info.Name), null);
				}
				else
				{
					tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_PERMISSION_OFF, minion.GetProperName(), foodInfoTableColumn.food_info.Name), null);
				}
			}
			break;
		}
	}

	private void on_tooltip_sort_food_info(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
	}

	private void on_tooltip_food_info_super(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
			tooltip.AddMultiStringTooltip(UI.CONSUMABLESSCREEN.TOOLTIP_TOGGLE_ALL.text, null);
			break;
		case TableRow.RowType.Default:
			tooltip.AddMultiStringTooltip(UI.CONSUMABLESSCREEN.NEW_MINIONS_TOOLTIP_TOGGLE_ROW, null);
			break;
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.TOOLTIP_TOGGLE_ROW.text, minion.gameObject.GetProperName()), null);
			}
			break;
		}
	}

	private void on_load_food_info(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableColumn widgetColumn = base.GetWidgetColumn(widget_go);
		EdiblesManager.FoodInfo food_info = (widgetColumn as FoodInfoTableColumn).food_info;
		KToggle component = widget_go.GetComponent<KToggle>();
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
		{
			GameObject prefab = Assets.GetPrefab(food_info.Id.ToTag());
			if (prefab == null)
			{
				return;
			}
			KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
			Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component2.AnimFiles[0], "ui");
			Image image = widget_go.GetComponent<HierarchyReferences>().GetReference("PortraitImage") as Image;
			image.sprite = uispriteFromMultiObjectAnim;
			image.color = Color.white;
			image.material = ((WorldInventory.Instance.GetAmount(food_info.Id.ToTag()) <= 0f) ? Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial);
			break;
		}
		case TableRow.RowType.Default:
			component.isOn = this.get_value_food_info(minion, widget_go) == TableScreen.ResultValues.True;
			break;
		case TableRow.RowType.Minion:
		{
			component.isOn = this.get_value_food_info(minion, widget_go) == TableScreen.ResultValues.True;
			Color color = new Color(0.72156864f, 0.44313726f, 0.5803922f, Mathf.Max((float)food_info.Quality - Db.Get().Attributes.FoodExpectation.Lookup(minion).GetTotalValue() + 1f, 0f) * 0.25f);
			Image image2 = widget_go.GetComponent<HierarchyReferences>().GetReference("BGImage") as Image;
			image2.color = color;
			break;
		}
		}
	}

	private int compare_food_info(MinionIdentity a, MinionIdentity b)
	{
		return 0;
	}

	private TableScreen.ResultValues get_value_food_info(MinionIdentity minion, GameObject widget_go)
	{
		ConsumableConsumer consumableConsumer = null;
		if (minion != null)
		{
			consumableConsumer = minion.GetComponent<ConsumableConsumer>();
		}
		FoodInfoTableColumn foodInfoTableColumn = base.GetWidgetColumn(widget_go) as FoodInfoTableColumn;
		EdiblesManager.FoodInfo food_info = foodInfoTableColumn.food_info;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableScreen.ResultValues resultValues = TableScreen.ResultValues.Partial;
		switch (widgetRow.rowType)
		{
		case TableRow.RowType.Header:
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = false;
			foreach (KeyValuePair<TableRow, GameObject> keyValuePair in foodInfoTableColumn.widgets_by_row)
			{
				GameObject value = keyValuePair.Value;
				if (!(value == widget_go))
				{
					if (!(value == null))
					{
						switch (foodInfoTableColumn.get_value_action(keyValuePair.Key.GetMinionIdentity(), value))
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
			resultValues = ((!ConsumerManager.instance.DefaultForbiddenTagsList.Contains(food_info.Id.ToTag())) ? TableScreen.ResultValues.True : TableScreen.ResultValues.False);
			break;
		case TableRow.RowType.Minion:
			if (minion != null)
			{
				resultValues = ((!consumableConsumer.IsPermitted(food_info.Id)) ? TableScreen.ResultValues.False : TableScreen.ResultValues.True);
			}
			else
			{
				resultValues = TableScreen.ResultValues.True;
			}
			break;
		}
		return resultValues;
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
				tooltip.AddMultiStringTooltip(minion.GetProperName(), null);
			}
			break;
		}
	}

	protected FoodInfoTableColumn AddFoodInfoColumn(string id, EdiblesManager.FoodInfo food_info, Action<MinionIdentity, GameObject> load_value_action, Func<MinionIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, bool> set_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip)
	{
		FoodInfoTableColumn foodInfoTableColumn = new FoodInfoTableColumn(food_info, load_value_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip, (GameObject widget_go) => string.Empty);
		if (base.RegisterColumn(id, foodInfoTableColumn))
		{
			return foodInfoTableColumn;
		}
		return null;
	}
}
