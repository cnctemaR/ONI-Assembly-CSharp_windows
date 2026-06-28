using System;
using UnityEngine;

public class ButtonLabelColumn : LabelTableColumn
{
	public ButtonLabelColumn(Action<MinionIdentity, GameObject> on_load_action, Func<MinionIdentity, GameObject, string> get_value_action, Action<GameObject> on_click_action, Action<GameObject> on_double_click_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip)
		: base(on_load_action, get_value_action, sort_comparison, on_tooltip, on_sort_tooltip, 128)
	{
		this.on_click_action = on_click_action;
		this.on_double_click_action = on_double_click_action;
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.ButtonLabel, parent, true);
		widget_go.GetComponent<KButton>().onClick += delegate
		{
			this.on_click_action(widget_go);
		};
		widget_go.GetComponent<KButton>().onDoubleClick += delegate
		{
			this.on_double_click_action(widget_go);
		};
		return widget_go;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		return base.GetHeaderWidget(parent);
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.ButtonLabel, parent, true);
		ToolTip tt = widget_go.GetComponent<ToolTip>();
		tt.OnToolTip = () => this.GetTooltip(tt);
		widget_go.GetComponent<KButton>().onClick += delegate
		{
			this.on_click_action(widget_go);
		};
		widget_go.GetComponent<KButton>().onClick += delegate
		{
			this.on_double_click_action(widget_go);
		};
		return widget_go;
	}

	private Action<GameObject> on_click_action;

	private Action<GameObject> on_double_click_action;
}
