using System;
using UnityEngine;
using UnityEngine.UI;

public class LabelTableColumn : TableColumn
{
	public LabelTableColumn(Action<IAssignableIdentity, GameObject> on_load_action, Func<IAssignableIdentity, GameObject, string> get_value_action, Comparison<IAssignableIdentity> sort_comparison, Action<IAssignableIdentity, GameObject, ToolTip> on_tooltip, Action<IAssignableIdentity, GameObject, ToolTip> on_sort_tooltip, int widget_width = 128, bool should_refresh_columns = false)
		: base(on_load_action, sort_comparison, on_tooltip, on_sort_tooltip, null, should_refresh_columns, "")
	{
		this.get_value_action = get_value_action;
		this.widget_width = widget_width;
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Label, parent, true);
		LayoutElement component = gameObject.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		component.preferredWidth = (component.minWidth = (float)this.widget_width);
		return gameObject;
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.Label, parent, true);
		ToolTip tt = gameObject.GetComponent<ToolTip>();
		tt.OnToolTip = () => this.GetTooltip(tt);
		LayoutElement component = gameObject.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		component.preferredWidth = (component.minWidth = (float)this.widget_width);
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
			return "";
		};
		tt = widget_go.GetComponentInChildren<MultiToggle>().GetComponent<ToolTip>();
		tt.OnToolTip = delegate
		{
			this.on_sort_tooltip(null, widget_go, tt);
			return "";
		};
		LayoutElement component = widget_go.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		component.preferredWidth = (component.minWidth = (float)this.widget_width);
		return widget_go;
	}

	public Func<IAssignableIdentity, GameObject, string> get_value_action;

	private int widget_width = 128;
}
