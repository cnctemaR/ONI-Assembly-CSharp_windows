using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NumericDropDownTableColumn : TableColumn
{
	public NumericDropDownTableColumn(object user_data, List<TMP_Dropdown.OptionData> options, Action<IAssignableIdentity, GameObject> on_load_action, Action<GameObject, int> set_value_action, Comparison<IAssignableIdentity> sort_comparer, NumericDropDownTableColumn.ToolTipCallbacks callbacks, Func<bool> revealed = null)
		: base(on_load_action, sort_comparer, callbacks.headerTooltip, callbacks.headerSortTooltip, revealed, false, "")
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
		NumericDropDownTableColumn.<>c__DisplayClass9_0 CS$<>8__locals1 = new NumericDropDownTableColumn.<>c__DisplayClass9_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.DropDownHeader, parent, true);
		HierarchyReferences component = CS$<>8__locals1.widget_go.GetComponent<HierarchyReferences>();
		Component reference = component.GetReference("Label");
		MultiToggle componentInChildren = reference.GetComponentInChildren<MultiToggle>(true);
		this.column_sort_toggle = componentInChildren;
		MultiToggle multiToggle = componentInChildren;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			CS$<>8__locals1.<>4__this.screen.SetSortComparison(CS$<>8__locals1.<>4__this.sort_comparer, CS$<>8__locals1.<>4__this);
			CS$<>8__locals1.<>4__this.screen.SortRows();
		}));
		ToolTip tt2 = reference.GetComponent<ToolTip>();
		tt2.enabled = true;
		tt2.OnToolTip = delegate
		{
			CS$<>8__locals1.<>4__this.callbacks.headerTooltip(null, CS$<>8__locals1.widget_go, tt2);
			return "";
		};
		ToolTip tt3 = componentInChildren.transform.GetComponent<ToolTip>();
		tt3.OnToolTip = delegate
		{
			CS$<>8__locals1.<>4__this.callbacks.headerSortTooltip(null, CS$<>8__locals1.widget_go, tt3);
			return "";
		};
		Component reference2 = component.GetReference("DropDown");
		TMP_Dropdown componentInChildren2 = reference2.GetComponentInChildren<TMP_Dropdown>();
		componentInChildren2.options = this.options;
		componentInChildren2.onValueChanged.AddListener(delegate(int new_value)
		{
			CS$<>8__locals1.<>4__this.set_value_action(CS$<>8__locals1.widget_go, new_value);
		});
		ToolTip tt = reference2.GetComponent<ToolTip>();
		tt.OnToolTip = delegate
		{
			CS$<>8__locals1.<>4__this.callbacks.headerDropdownTooltip(null, CS$<>8__locals1.widget_go, tt);
			return "";
		};
		LayoutElement component2 = CS$<>8__locals1.widget_go.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		component2.preferredWidth = (component2.minWidth = 83f);
		return CS$<>8__locals1.widget_go;
	}

	public object userData;

	private NumericDropDownTableColumn.ToolTipCallbacks callbacks;

	private Action<GameObject, int> set_value_action;

	private List<TMP_Dropdown.OptionData> options;

	public class ToolTipCallbacks
	{
		public Action<IAssignableIdentity, GameObject, ToolTip> headerTooltip;

		public Action<IAssignableIdentity, GameObject, ToolTip> headerSortTooltip;

		public Action<IAssignableIdentity, GameObject, ToolTip> headerDropdownTooltip;
	}
}
