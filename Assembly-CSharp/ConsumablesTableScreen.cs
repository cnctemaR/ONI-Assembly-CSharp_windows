using System;
using System.Collections.Generic;
using Klei.AI;
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
		base.AddLabelColumn("Expectations", new Action<MinionIdentity, GameObject>(this.on_load_expectations), new Func<MinionIdentity, GameObject, string>(this.get_value_expectations_label), new Comparison<MinionIdentity>(this.compare_rows_expectations), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_expectations), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_expectations), 128, 0f);
		base.AddLabelColumn("Stress", new Action<MinionIdentity, GameObject>(this.on_load_stress), new Func<MinionIdentity, GameObject, string>(this.get_value_stress_label), new Comparison<MinionIdentity>(this.compare_rows_stress), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_stress), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_stress), 64, 1f);
		List<IConsumableUIItem> list = new List<IConsumableUIItem>();
		for (int i = 0; i < FOOD.FOOD_TYPES_LIST.Count; i++)
		{
			list.Add(FOOD.FOOD_TYPES_LIST[i]);
		}
		List<GameObject> prefabsWithTag = Assets.GetPrefabsWithTag(GameTags.Medicine);
		for (int j = 0; j < prefabsWithTag.Count; j++)
		{
			list.Add(prefabsWithTag[j].GetComponent<MedicinalPill>());
		}
		list.Sort(delegate(IConsumableUIItem a, IConsumableUIItem b)
		{
			int num2 = a.MajorOrder.CompareTo(b.MajorOrder);
			if (num2 == 0)
			{
				num2 = a.MinorOrder.CompareTo(b.MinorOrder);
			}
			return num2;
		});
		ConsumerManager.instance.OnDiscover += this.OnConsumableDiscovered;
		List<ConsumableInfoTableColumn> list2 = new List<ConsumableInfoTableColumn>();
		List<DividerColumn> list3 = new List<DividerColumn>();
		List<ConsumableInfoTableColumn> list4 = new List<ConsumableInfoTableColumn>();
		int num = 0;
		for (int k = 0; k < list.Count; k++)
		{
			if (list[k].Display)
			{
				if (list[k].MajorOrder != num && k != 0)
				{
					string text = "QualityDivider_" + list[k].MajorOrder;
					ConsumableInfoTableColumn[] quality_group_columns = list4.ToArray();
					DividerColumn dividerColumn = new DividerColumn(delegate
					{
						bool flag;
						if (quality_group_columns == null || quality_group_columns.Length == 0)
						{
							flag = true;
						}
						else
						{
							foreach (ConsumableInfoTableColumn consumableInfoTableColumn2 in quality_group_columns)
							{
								if (consumableInfoTableColumn2.isRevealed)
								{
									return true;
								}
							}
							flag = false;
						}
						return flag;
					});
					list3.Add(dividerColumn);
					base.RegisterColumn(text, dividerColumn);
					list4.Clear();
				}
				ConsumableInfoTableColumn consumableInfoTableColumn = this.AddConsumableInfoColumn(list[k].ConsumableId, list[k], new Action<MinionIdentity, GameObject>(this.on_load_consumable_info), new Func<MinionIdentity, GameObject, TableScreen.ResultValues>(this.get_value_consumable_info), new Action<GameObject>(this.on_click_consumable_info), new Action<GameObject, bool>(this.set_value_consumable_info), new Comparison<MinionIdentity>(this.compare_consumable_info), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_consumable_info), new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_sort_consumable_info));
				list2.Add(consumableInfoTableColumn);
				num = list[k].MajorOrder;
				list4.Add(consumableInfoTableColumn);
			}
		}
		base.AddSuperCheckboxColumn("SuperCheckConsumable", list2.ToArray(), new Action<MinionIdentity, GameObject>(base.on_load_value_checkbox_column_super), new Func<MinionIdentity, GameObject, TableScreen.ResultValues>(this.get_value_checkbox_column_super), new Action<GameObject>(base.on_press_checkbox_column_super), new Action<GameObject, bool>(base.set_value_checkbox_column_super), null, new Action<MinionIdentity, GameObject, ToolTip>(this.on_tooltip_consumable_info_super));
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
			componentInChildren.text = "";
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
		int num;
		if (value > value2)
		{
			num = -1;
		}
		else if (value < value2)
		{
			num = 1;
		}
		else
		{
			num = 0;
		}
		return num;
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
						tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.STRESS_TOOLTIP, Db.Get().Amounts.Stress.Lookup(minion).GetValueString()), null);
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
			componentInChildren.text = "";
		}
		else
		{
			componentInChildren.text = UI.CONSUMABLESSCREEN.FOOD_EXPECTATIONS;
		}
	}

	private string get_value_expectations_label(MinionIdentity minion, GameObject widget_go)
	{
		return Db.Get().Attributes.FoodExpectation.Lookup(minion).GetFormattedValue();
	}

	private int compare_rows_expectations(MinionIdentity a, MinionIdentity b)
	{
		float totalValue = Db.Get().Attributes.FoodExpectation.Lookup(a).GetTotalValue();
		float totalValue2 = Db.Get().Attributes.FoodExpectation.Lookup(b).GetTotalValue();
		return totalValue.CompareTo(totalValue2);
	}

	protected void on_tooltip_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
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
						tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_EXPECTATIONS_TOOLTIP, Db.Get().Attributes.FoodExpectation.Lookup(minion).GetFormattedValue()), null);
					}
				}
			}
		}
	}

	protected void on_tooltip_sort_expectations(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
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
			TableScreen.ResultValues resultValues = checkboxTableColumn.get_value_action(widgetRow.GetMinionIdentity(), widgetRow.GetWidget(checkboxTableColumn));
			if (resultValues != TableScreen.ResultValues.False)
			{
				if (resultValues != TableScreen.ResultValues.Partial)
				{
					if (resultValues == TableScreen.ResultValues.True)
					{
						flag = false;
						if (!flag2)
						{
							flag4 = true;
						}
					}
				}
				else
				{
					flag3 = true;
					flag4 = true;
				}
			}
			else
			{
				flag2 = false;
				if (!flag)
				{
					flag4 = true;
				}
			}
			if (flag4)
			{
				break;
			}
		}
		TableScreen.ResultValues resultValues2;
		if (flag3)
		{
			resultValues2 = TableScreen.ResultValues.Partial;
		}
		else if (flag2)
		{
			resultValues2 = TableScreen.ResultValues.True;
		}
		else if (flag)
		{
			resultValues2 = TableScreen.ResultValues.False;
		}
		else
		{
			resultValues2 = TableScreen.ResultValues.Partial;
		}
		return resultValues2;
	}

	private void set_value_consumable_info(GameObject widget_go, bool new_value)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		if (widgetRow == null)
		{
			global::Debug.LogWarning("Row is null", null);
		}
		else
		{
			ConsumableInfoTableColumn consumableInfoTableColumn = base.GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
			MinionIdentity minionIdentity = widgetRow.GetMinionIdentity();
			IConsumableUIItem consumable_info = consumableInfoTableColumn.consumable_info;
			TableRow.RowType rowType = widgetRow.rowType;
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Default)
				{
					if (rowType == TableRow.RowType.Minion)
					{
						if (minionIdentity != null)
						{
							ConsumableConsumer component = minionIdentity.GetComponent<ConsumableConsumer>();
							if (component == null)
							{
								global::Debug.LogError("Could not find minion identity / row associated with the widget", null);
							}
							else
							{
								component.SetPermitted(consumable_info.ConsumableId, new_value);
								consumableInfoTableColumn.on_load_action(widgetRow.GetMinionIdentity(), widget_go);
								foreach (KeyValuePair<TableRow, GameObject> keyValuePair in consumableInfoTableColumn.widgets_by_row)
								{
									if (keyValuePair.Key.rowType == TableRow.RowType.Header)
									{
										consumableInfoTableColumn.on_load_action(null, keyValuePair.Value);
										break;
									}
								}
							}
						}
					}
				}
				else
				{
					if (new_value)
					{
						ConsumerManager.instance.DefaultForbiddenTagsList.Remove(consumable_info.ConsumableId.ToTag());
					}
					else
					{
						ConsumerManager.instance.DefaultForbiddenTagsList.Add(consumable_info.ConsumableId.ToTag());
					}
					consumableInfoTableColumn.on_load_action(minionIdentity, widget_go);
					foreach (KeyValuePair<TableRow, GameObject> keyValuePair2 in consumableInfoTableColumn.widgets_by_row)
					{
						if (keyValuePair2.Key.rowType == TableRow.RowType.Header)
						{
							consumableInfoTableColumn.on_load_action(null, keyValuePair2.Value);
							break;
						}
					}
				}
			}
			else
			{
				this.set_value_consumable_info(this.default_row.GetComponent<TableRow>().GetWidget(consumableInfoTableColumn), new_value);
				base.StartCoroutine(base.CascadeSetColumnCheckBoxes(this.sortable_rows, consumableInfoTableColumn, new_value, widget_go));
			}
		}
	}

	private void on_click_consumable_info(GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		MinionIdentity minionIdentity = widgetRow.GetMinionIdentity();
		ConsumableInfoTableColumn consumableInfoTableColumn = base.GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			if (rowType != TableRow.RowType.Default)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minionIdentity != null)
					{
						ConsumableConsumer component = minionIdentity.GetComponent<ConsumableConsumer>();
						if (component == null)
						{
							global::Debug.LogError("Could not find minion identity / row associated with the widget", null);
						}
						else
						{
							IConsumableUIItem consumableUIItem = consumableInfoTableColumn.consumable_info;
							consumableInfoTableColumn.on_set_action(widget_go, !component.IsPermitted(consumableUIItem.ConsumableId));
						}
					}
				}
			}
			else
			{
				IConsumableUIItem consumableUIItem = consumableInfoTableColumn.consumable_info;
				bool flag = !ConsumerManager.instance.DefaultForbiddenTagsList.Contains(consumableUIItem.ConsumableId.ToTag());
				consumableInfoTableColumn.on_set_action(widget_go, !flag);
			}
		}
		else
		{
			bool flag2 = this.get_value_consumable_info(null, widget_go) == TableScreen.ResultValues.True;
			consumableInfoTableColumn.on_set_action(widget_go, !flag2);
			consumableInfoTableColumn.on_load_action(null, widget_go);
		}
	}

	private void on_tooltip_consumable_info(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		ConsumableInfoTableColumn consumableInfoTableColumn = base.GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		EdiblesManager.FoodInfo foodInfo = consumableInfoTableColumn.consumable_info as EdiblesManager.FoodInfo;
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			if (rowType != TableRow.RowType.Default)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						if (consumableInfoTableColumn.get_value_action(minion, widget_go) == TableScreen.ResultValues.True)
						{
							tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_PERMISSION_ON, minion.GetProperName(), consumableInfoTableColumn.consumable_info.ConsumableName), null);
						}
						else
						{
							tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_PERMISSION_OFF, minion.GetProperName(), consumableInfoTableColumn.consumable_info.ConsumableName), null);
						}
						if (foodInfo != null)
						{
							if (minion.GetAttributes().Get(Db.Get().Attributes.FoodExpectation).GetTotalValue() > (float)foodInfo.Quality)
							{
								tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_QUALITY_VS_EXPECTATION, new object[]
								{
									UI.CONSUMABLESSCREEN.EXPECTATIONS_BELOW,
									minion.GetProperName(),
									foodInfo.Quality,
									minion.GetAttributes().Get(Db.Get().Attributes.FoodExpectation).GetTotalValue()
								}), null);
							}
							else
							{
								tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_QUALITY_VS_EXPECTATION, new object[]
								{
									UI.CONSUMABLESSCREEN.EXPECTATIONS_ABOVE,
									minion.GetProperName(),
									foodInfo.Quality,
									minion.GetAttributes().Get(Db.Get().Attributes.FoodExpectation).GetTotalValue()
								}), null);
							}
						}
					}
				}
			}
			else if (consumableInfoTableColumn.get_value_action(minion, widget_go) == TableScreen.ResultValues.True)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.NEW_MINIONS_FOOD_PERMISSION_ON, consumableInfoTableColumn.consumable_info.ConsumableName), null);
			}
			else
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.NEW_MINIONS_FOOD_PERMISSION_OFF, consumableInfoTableColumn.consumable_info.ConsumableName), null);
			}
		}
		else
		{
			tooltip.AddMultiStringTooltip(consumableInfoTableColumn.consumable_info.ConsumableName, null);
			if (foodInfo != null)
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_AVAILABLE, GameUtil.GetFormattedCalories(WorldInventory.Instance.GetAmount(consumableInfoTableColumn.consumable_info.ConsumableId.ToTag()) * foodInfo.CaloriesPerUnit, GameUtil.TimeSlice.None, true)), null);
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_QUALITY, foodInfo.Quality), null);
			}
			else
			{
				tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.FOOD_AVAILABLE, GameUtil.GetFormattedUnits(WorldInventory.Instance.GetAmount(consumableInfoTableColumn.consumable_info.ConsumableId.ToTag()), GameUtil.TimeSlice.None, true)), null);
			}
		}
	}

	private void on_tooltip_sort_consumable_info(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
	}

	private void on_tooltip_consumable_info_super(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			if (rowType != TableRow.RowType.Default)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						tooltip.AddMultiStringTooltip(string.Format(UI.CONSUMABLESSCREEN.TOOLTIP_TOGGLE_ROW.text, minion.gameObject.GetProperName()), null);
					}
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.CONSUMABLESSCREEN.NEW_MINIONS_TOOLTIP_TOGGLE_ROW, null);
			}
		}
		else
		{
			tooltip.AddMultiStringTooltip(UI.CONSUMABLESSCREEN.TOOLTIP_TOGGLE_ALL.text, null);
		}
	}

	private void on_load_consumable_info(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableColumn widgetColumn = base.GetWidgetColumn(widget_go);
		IConsumableUIItem consumable_info = (widgetColumn as ConsumableInfoTableColumn).consumable_info;
		EdiblesManager.FoodInfo foodInfo = consumable_info as EdiblesManager.FoodInfo;
		KToggle component = widget_go.GetComponent<KToggle>();
		if (!widgetColumn.isRevealed)
		{
			widget_go.SetActive(false);
		}
		else
		{
			if (!widget_go.activeSelf)
			{
				widget_go.SetActive(true);
			}
			TableRow.RowType rowType = widgetRow.rowType;
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Default)
				{
					if (rowType == TableRow.RowType.Minion)
					{
						component.isOn = this.get_value_consumable_info(minion, widget_go) == TableScreen.ResultValues.True;
						if (foodInfo != null)
						{
							Image image = widget_go.GetComponent<HierarchyReferences>().GetReference("BGImage") as Image;
							Color color = new Color(0.72156864f, 0.44313726f, 0.5803922f, Mathf.Max((float)foodInfo.Quality - Db.Get().Attributes.FoodExpectation.Lookup(minion).GetTotalValue() + 1f, 0f) * 0.25f);
							image.color = color;
						}
					}
				}
				else
				{
					component.isOn = this.get_value_consumable_info(minion, widget_go) == TableScreen.ResultValues.True;
				}
			}
			else
			{
				GameObject prefab = Assets.GetPrefab(consumable_info.ConsumableId.ToTag());
				if (!(prefab == null))
				{
					KBatchedAnimController component2 = prefab.GetComponent<KBatchedAnimController>();
					Sprite uispriteFromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(component2.AnimFiles[0], "ui");
					Image image2 = widget_go.GetComponent<HierarchyReferences>().GetReference("PortraitImage") as Image;
					image2.sprite = uispriteFromMultiObjectAnim;
					image2.color = Color.white;
					image2.material = ((WorldInventory.Instance.GetAmount(consumable_info.ConsumableId.ToTag()) <= 0f) ? Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial);
				}
			}
		}
	}

	private int compare_consumable_info(MinionIdentity a, MinionIdentity b)
	{
		return 0;
	}

	private TableScreen.ResultValues get_value_consumable_info(MinionIdentity minion, GameObject widget_go)
	{
		ConsumableConsumer consumableConsumer = null;
		if (minion != null)
		{
			consumableConsumer = minion.GetComponent<ConsumableConsumer>();
		}
		ConsumableInfoTableColumn consumableInfoTableColumn = base.GetWidgetColumn(widget_go) as ConsumableInfoTableColumn;
		IConsumableUIItem consumable_info = consumableInfoTableColumn.consumable_info;
		TableRow widgetRow = base.GetWidgetRow(widget_go);
		TableScreen.ResultValues resultValues = TableScreen.ResultValues.Partial;
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			if (rowType != TableRow.RowType.Default)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					if (minion != null)
					{
						resultValues = ((!consumableConsumer.IsPermitted(consumable_info.ConsumableId)) ? TableScreen.ResultValues.False : TableScreen.ResultValues.True);
					}
					else
					{
						resultValues = TableScreen.ResultValues.True;
					}
				}
			}
			else
			{
				resultValues = ((!ConsumerManager.instance.DefaultForbiddenTagsList.Contains(consumable_info.ConsumableId.ToTag())) ? TableScreen.ResultValues.True : TableScreen.ResultValues.False);
			}
		}
		else
		{
			bool flag = true;
			bool flag2 = true;
			bool flag3 = false;
			bool flag4 = false;
			foreach (KeyValuePair<TableRow, GameObject> keyValuePair in consumableInfoTableColumn.widgets_by_row)
			{
				GameObject value = keyValuePair.Value;
				if (!(value == widget_go))
				{
					if (!(value == null))
					{
						TableScreen.ResultValues resultValues2 = consumableInfoTableColumn.get_value_action(keyValuePair.Key.GetMinionIdentity(), value);
						if (resultValues2 != TableScreen.ResultValues.False)
						{
							if (resultValues2 != TableScreen.ResultValues.Partial)
							{
								if (resultValues2 == TableScreen.ResultValues.True)
								{
									flag = false;
									if (!flag2)
									{
										flag4 = true;
									}
								}
							}
							else
							{
								flag3 = true;
								flag4 = true;
							}
						}
						else
						{
							flag2 = false;
							if (!flag)
							{
								flag4 = true;
							}
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
		}
		return resultValues;
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

	protected ConsumableInfoTableColumn AddConsumableInfoColumn(string id, IConsumableUIItem consumable_info, Action<MinionIdentity, GameObject> load_value_action, Func<MinionIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, bool> set_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip)
	{
		ConsumableInfoTableColumn consumableInfoTableColumn = new ConsumableInfoTableColumn(consumable_info, load_value_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip, (GameObject widget_go) => "");
		ConsumableInfoTableColumn consumableInfoTableColumn2;
		if (base.RegisterColumn(id, consumableInfoTableColumn))
		{
			consumableInfoTableColumn2 = consumableInfoTableColumn;
		}
		else
		{
			consumableInfoTableColumn2 = null;
		}
		return consumableInfoTableColumn2;
	}

	private void OnConsumableDiscovered(Tag tag)
	{
		base.MarkRowsDirty();
	}
}
