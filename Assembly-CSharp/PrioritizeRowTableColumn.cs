using System;
using UnityEngine;

public class PrioritizeRowTableColumn : TableColumn
{
	public PrioritizeRowTableColumn(object user_data, Action<object, int> on_change_priority, Func<object, int, string> on_hover_widget)
		: base(null, null, null, null, null, false, "")
	{
		this.userData = user_data;
		this.onChangePriority = on_change_priority;
		this.onHoverWidget = on_hover_widget;
	}

	public override GameObject GetMinionWidget(GameObject parent)
	{
		return this.GetWidget(parent);
	}

	public override GameObject GetDefaultWidget(GameObject parent)
	{
		return this.GetWidget(parent);
	}

	public override GameObject GetHeaderWidget(GameObject parent)
	{
		return Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PrioritizeRowHeaderWidget, parent, true);
	}

	private GameObject GetWidget(GameObject parent)
	{
		GameObject gameObject = Util.KInstantiateUI(Assets.UIPrefabs.TableScreenWidgets.PrioritizeRowWidget, parent, true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		this.ConfigureButton(component, "UpButton", 1, gameObject);
		this.ConfigureButton(component, "DownButton", -1, gameObject);
		return gameObject;
	}

	private void ConfigureButton(HierarchyReferences refs, string ref_id, int delta, GameObject widget_go)
	{
		KButton kbutton = refs.GetReference(ref_id) as KButton;
		kbutton.onClick += delegate
		{
			this.onChangePriority(widget_go, delta);
		};
		ToolTip component = kbutton.GetComponent<ToolTip>();
		if (component != null)
		{
			component.OnToolTip = () => this.onHoverWidget(widget_go, delta);
		}
	}

	public object userData;

	private Action<object, int> onChangePriority;

	private Func<object, int, string> onHoverWidget;
}
