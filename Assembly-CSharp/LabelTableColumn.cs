using System;
using UnityEngine;
using UnityEngine.UI;

public class LabelTableColumn : TableColumn
{
	public LabelTableColumn(Action<MinionIdentity, GameObject> on_load_action, Func<MinionIdentity, GameObject, string> get_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip, int widget_width = 128)
		: base(on_load_action, sort_comparison, on_tooltip, on_sort_tooltip)
	{
		this.get_value_action = get_value_action;
		this.widget_width = widget_width;
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Label, parent, true);
		LayoutElement component = gameObject.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		LayoutElement layoutElement = component;
		float num = (float)this.widget_width;
		component.minWidth = num;
		layoutElement.preferredWidth = num;
		return gameObject;
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Label, parent, true);
		ToolTip tt = gameObject.GetComponent<ToolTip>();
		tt.OnToolTip = () => this.GetTooltip(tt);
		LayoutElement component = gameObject.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		LayoutElement layoutElement = component;
		float num = (float)this.widget_width;
		component.minWidth = num;
		layoutElement.preferredWidth = num;
		return gameObject;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		GameObject widget_go = null;
		widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.LabelHeader, parent, true);
		MultiToggle componentInChildren = widget_go.GetComponentInChildren<MultiToggle>(true);
		this.column_sort_toggle = componentInChildren;
		MultiToggle multiToggle = componentInChildren;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.screen.SetSortComparison(this.sort_comparer, this);
			this.screen.SortRows();
		}));
		ToolTip tt = widget_go.GetComponent<ToolTip>();
		tt.OnToolTip = delegate
		{
			this.on_tooltip(null, widget_go, tt);
			return string.Empty;
		};
		tt = widget_go.GetComponentInChildren<MultiToggle>().GetComponent<ToolTip>();
		tt.OnToolTip = delegate
		{
			this.on_sort_tooltip(null, widget_go, tt);
			return string.Empty;
		};
		LayoutElement component = widget_go.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		LayoutElement layoutElement = component;
		float num = (float)this.widget_width;
		component.minWidth = num;
		layoutElement.preferredWidth = num;
		return widget_go;
	}

	public Func<MinionIdentity, GameObject, string> get_value_action;

	private int widget_width = 128;
}
