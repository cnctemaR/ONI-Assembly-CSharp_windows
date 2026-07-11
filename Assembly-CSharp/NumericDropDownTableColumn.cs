using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumericDropDownTableColumn : TableColumn
{
	public NumericDropDownTableColumn(object user_data, List<TMP_Dropdown.OptionData> options, Action<MinionIdentity, GameObject> on_load_action, Action<GameObject, int> set_value_action, Comparison<MinionIdentity> sort_comparer, NumericDropDownTableColumn.ToolTipCallbacks callbacks, Func<bool> revealed = null)
		: base(on_load_action, sort_comparer, callbacks.headerTooltip, callbacks.headerSortTooltip, revealed, false, string.Empty)
	{
		this.userData = user_data;
		this.set_value_action = set_value_action;
		this.options = options;
		this.callbacks = callbacks;
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		return this.GetWidget(parent);
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		return this.GetWidget(parent);
	}

	private GameObject GetWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.NumericDropDown, parent, true);
		TMP_Dropdown componentInChildren = widget_go.transform.GetComponentInChildren<TMP_Dropdown>();
		componentInChildren.options = this.options;
		componentInChildren.onValueChanged.AddListener(delegate(int new_value)
		{
			this.set_value_action(widget_go, new_value);
		});
		ToolTip tt = widget_go.transform.GetComponentInChildren<ToolTip>();
		if (tt != null)
		{
			tt.OnToolTip = () => this.GetTooltip(tt);
		}
		return widget_go;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		NumericDropDownTableColumn.<GetHeaderWidget>c__AnonStorey1 <GetHeaderWidget>c__AnonStorey = new NumericDropDownTableColumn.<GetHeaderWidget>c__AnonStorey1();
		<GetHeaderWidget>c__AnonStorey.$this = this;
		<GetHeaderWidget>c__AnonStorey.widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.DropDownHeader, parent, true);
		HierarchyReferences component = <GetHeaderWidget>c__AnonStorey.widget_go.GetComponent<HierarchyReferences>();
		Component reference = component.GetReference("Label");
		MultiToggle componentInChildren = reference.GetComponentInChildren<MultiToggle>(true);
		this.column_sort_toggle = componentInChildren;
		MultiToggle multiToggle = componentInChildren;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			<GetHeaderWidget>c__AnonStorey.$this.screen.SetSortComparison(<GetHeaderWidget>c__AnonStorey.$this.sort_comparer, <GetHeaderWidget>c__AnonStorey.$this);
			<GetHeaderWidget>c__AnonStorey.$this.screen.SortRows();
		}));
		ToolTip tt2 = reference.GetComponent<ToolTip>();
		tt2.enabled = true;
		tt2.OnToolTip = delegate
		{
			<GetHeaderWidget>c__AnonStorey.$this.callbacks.headerTooltip(null, <GetHeaderWidget>c__AnonStorey.widget_go, tt2);
			return string.Empty;
		};
		ToolTip tt3 = componentInChildren.transform.GetComponent<ToolTip>();
		tt3.OnToolTip = delegate
		{
			<GetHeaderWidget>c__AnonStorey.$this.callbacks.headerSortTooltip(null, <GetHeaderWidget>c__AnonStorey.widget_go, tt3);
			return string.Empty;
		};
		Component reference2 = component.GetReference("DropDown");
		TMP_Dropdown componentInChildren2 = reference2.GetComponentInChildren<TMP_Dropdown>();
		componentInChildren2.options = this.options;
		componentInChildren2.onValueChanged.AddListener(delegate(int new_value)
		{
			<GetHeaderWidget>c__AnonStorey.$this.set_value_action(<GetHeaderWidget>c__AnonStorey.widget_go, new_value);
		});
		ToolTip tt = reference2.GetComponent<ToolTip>();
		tt.OnToolTip = delegate
		{
			<GetHeaderWidget>c__AnonStorey.$this.callbacks.headerDropdownTooltip(null, <GetHeaderWidget>c__AnonStorey.widget_go, tt);
			return string.Empty;
		};
		LayoutElement component2 = <GetHeaderWidget>c__AnonStorey.widget_go.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		LayoutElement layoutElement = component2;
		float num = 83f;
		component2.minWidth = num;
		layoutElement.preferredWidth = num;
		return <GetHeaderWidget>c__AnonStorey.widget_go;
	}

	public object userData;

	private NumericDropDownTableColumn.ToolTipCallbacks callbacks;

	private Action<GameObject, int> set_value_action;

	private List<TMP_Dropdown.OptionData> options;

	public class ToolTipCallbacks
	{
		public Action<MinionIdentity, GameObject, ToolTip> headerTooltip;

		public Action<MinionIdentity, GameObject, ToolTip> headerSortTooltip;

		public Action<MinionIdentity, GameObject, ToolTip> headerDropdownTooltip;
	}
}
