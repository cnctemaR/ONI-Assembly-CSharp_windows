using System;
using UnityEngine;
using UnityEngine.UI;

public class PrioritizationGroupTableColumn : TableColumn
{
	public PrioritizationGroupTableColumn(object user_data, Action<MinionIdentity, GameObject> on_load_action, Action<object, int> on_change_priority, Func<object, string> on_hover_widget, Action<object, int> on_change_header_priority, Func<object, string> on_hover_header_option_selector, Action<object> on_sort_clicked, Func<object, string> on_sort_hovered)
		: base(on_load_action, null, null, null, null, false, string.Empty)
	{
		this.userData = user_data;
		this.onChangePriority = on_change_priority;
		this.onHoverWidget = on_hover_widget;
		this.onHoverHeaderOptionSelector = on_hover_header_option_selector;
		this.onSortClicked = on_sort_clicked;
		this.onSortHovered = on_sort_hovered;
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
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PriorityGroupSelector, parent, true);
		OptionSelector component = widget_go.GetComponent<OptionSelector>();
		component.Initialize(widget_go);
		component.OnChangePriority = delegate(object widget, int delta)
		{
			this.onChangePriority(widget, delta);
		};
		ToolTip[] componentsInChildren = widget_go.transform.GetComponentsInChildren<ToolTip>();
		if (componentsInChildren != null)
		{
			foreach (ToolTip toolTip in componentsInChildren)
			{
				toolTip.OnToolTip = () => this.onHoverWidget(widget_go);
			}
		}
		return widget_go;
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		GameObject widget_go = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PriorityGroupSelectorHeader, parent, true);
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		LayoutElement component2 = widget_go.GetComponentInChildren<LocText>().GetComponent<LayoutElement>();
		LayoutElement layoutElement = component2;
		float num = 63f;
		component2.minWidth = num;
		layoutElement.preferredWidth = num;
		MonoBehaviour reference = component.GetReference("Label");
		LocText component3 = reference.GetComponent<LocText>();
		component3.raycastTarget = true;
		ToolTip component4 = reference.GetComponent<ToolTip>();
		if (component4 != null)
		{
			component4.OnToolTip = () => this.onHoverWidget(widget_go);
		}
		MultiToggle componentInChildren = widget_go.GetComponentInChildren<MultiToggle>(true);
		this.column_sort_toggle = componentInChildren;
		MultiToggle multiToggle = componentInChildren;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.onSortClicked(widget_go);
		}));
		ToolTip component5 = componentInChildren.GetComponent<ToolTip>();
		if (component5 != null)
		{
			component5.OnToolTip = () => this.onSortHovered(widget_go);
		}
		KButton kbutton = component.GetReference("PrioritizeButton") as KButton;
		ToolTip component6 = kbutton.GetComponent<ToolTip>();
		if (component6 != null)
		{
			component6.OnToolTip = () => this.onHoverHeaderOptionSelector(widget_go);
		}
		return widget_go;
	}

	public object userData;

	private Action<object, int> onChangePriority;

	private Func<object, string> onHoverWidget;

	private Func<object, string> onHoverHeaderOptionSelector;

	private Action<object> onSortClicked;

	private Func<object, string> onSortHovered;
}
