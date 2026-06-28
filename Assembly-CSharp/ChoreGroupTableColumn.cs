using System;
using UnityEngine;

public class ChoreGroupTableColumn : CheckboxTableColumn
{
	public ChoreGroupTableColumn(ChoreGroup chore_group, Action<MinionIdentity, GameObject> on_load_value, Func<MinionIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, TableScreen.ResultValues> set_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip, Func<GameObject, string> get_header_label)
		: base(on_load_value, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip, on_sort_tooltip, null)
	{
		this.chore_group = chore_group;
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
		headerWidget.GetComponentInChildren<MultiToggle>().onClick = delegate
		{
			((JobsTableScreen)this.screen).SetCurrentSortChoreGroup(this.chore_group);
			this.screen.SetSortComparison(this.sort_comparer, this);
			this.screen.SortRows();
		};
		this.column_sort_toggle = headerWidget.GetComponentInChildren<MultiToggle>();
		return headerWidget;
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject gameObject;
		if (!parent.GetComponent<TableRow>().GetMinionIdentity().GetComponent<ChoreConsumer>()
			.IsEnabled(this.chore_group))
		{
			gameObject = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.BlankCell, parent, true);
		}
		else
		{
			gameObject = base.GetMinionWidget(parent);
		}
		return gameObject;
	}

	public ChoreGroup chore_group;

	public Func<GameObject, string> get_header_label;
}
