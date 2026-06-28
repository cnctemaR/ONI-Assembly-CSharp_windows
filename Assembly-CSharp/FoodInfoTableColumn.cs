using System;
using UnityEngine;

public class FoodInfoTableColumn : CheckboxTableColumn
{
	public FoodInfoTableColumn(EdiblesManager.FoodInfo food_info, Action<MinionIdentity, GameObject> load_value_action, Func<MinionIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, bool> set_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip, Func<GameObject, string> get_header_label)
		: base(load_value_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip)
	{
		this.food_info = food_info;
		this.get_header_label = get_header_label;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		GameObject headerWidget = base.GetHeaderWidget(parent);
		LocText componentInChildren = headerWidget.GetComponentInChildren<LocText>();
		if (componentInChildren != null)
		{
			headerWidget.GetComponentInChildren<LocText>().text = this.get_header_label(headerWidget);
		}
		headerWidget.GetComponentInChildren<MultiToggle>().gameObject.SetActive(false);
		return headerWidget;
	}

	public EdiblesManager.FoodInfo food_info;

	public Func<GameObject, string> get_header_label;
}
