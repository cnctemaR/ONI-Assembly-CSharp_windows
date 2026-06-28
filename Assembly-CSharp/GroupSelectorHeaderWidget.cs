using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupSelectorHeaderWidget : MonoBehaviour
{
	public void Initialize(object widget_id, IList<GroupSelectorWidget.ItemData> options, GroupSelectorHeaderWidget.ItemCallbacks item_callbacks)
	{
		GroupSelectorHeaderWidget.<Initialize>c__AnonStorey0 <Initialize>c__AnonStorey = new GroupSelectorHeaderWidget.<Initialize>c__AnonStorey0();
		<Initialize>c__AnonStorey.widget_id = widget_id;
		<Initialize>c__AnonStorey.$this = this;
		this.widgetID = <Initialize>c__AnonStorey.widget_id;
		this.options = options;
		this.itemCallbacks = item_callbacks;
		if (this.itemCallbacks.getTitleHoverText != null)
		{
			this.label.GetComponent<ToolTip>().OnToolTip = () => <Initialize>c__AnonStorey.$this.itemCallbacks.getTitleHoverText(<Initialize>c__AnonStorey.widget_id);
		}
		bool adding_item2 = true;
		this.addItemButton.onClick += delegate
		{
			<Initialize>c__AnonStorey.$this.RebuildSubPanel(<Initialize>c__AnonStorey.$this.addItemButton.transform.GetPosition(), (object widget_go) => <Initialize>c__AnonStorey.$this.itemCallbacks.getHeaderButtonOptions(widget_go, adding_item2), <Initialize>c__AnonStorey.$this.itemCallbacks.onItemAdded, (object widget_go, object item_data) => <Initialize>c__AnonStorey.$this.itemCallbacks.getItemHoverText(widget_go, adding_item2, item_data));
		};
		bool adding_item = false;
		this.removeItemButton.onClick += delegate
		{
			<Initialize>c__AnonStorey.$this.RebuildSubPanel(<Initialize>c__AnonStorey.$this.removeItemButton.transform.GetPosition(), (object widget_go) => <Initialize>c__AnonStorey.$this.itemCallbacks.getHeaderButtonOptions(widget_go, adding_item), <Initialize>c__AnonStorey.$this.itemCallbacks.onItemRemoved, (object widget_go, object item_data) => <Initialize>c__AnonStorey.$this.itemCallbacks.getItemHoverText(widget_go, adding_item, item_data));
		};
		this.sortButton.onClick += delegate
		{
			<Initialize>c__AnonStorey.$this.RebuildSubPanel(<Initialize>c__AnonStorey.$this.sortButton.transform.GetPosition(), <Initialize>c__AnonStorey.$this.itemCallbacks.getValidSortOptionIndices, delegate(object item_data)
			{
				<Initialize>c__AnonStorey.$this.itemCallbacks.onSort(<Initialize>c__AnonStorey.$this.widgetID, item_data);
			}, (object widget_go, object item_data) => <Initialize>c__AnonStorey.$this.itemCallbacks.getSortHoverText(item_data));
		};
		if (this.itemCallbacks.getTitleButtonHoverText != null)
		{
			this.addItemButton.GetComponent<ToolTip>().OnToolTip = () => <Initialize>c__AnonStorey.$this.itemCallbacks.getTitleButtonHoverText(<Initialize>c__AnonStorey.widget_id, true);
			this.removeItemButton.GetComponent<ToolTip>().OnToolTip = () => <Initialize>c__AnonStorey.$this.itemCallbacks.getTitleButtonHoverText(<Initialize>c__AnonStorey.widget_id, false);
		}
	}

	private void RebuildSubPanel(Vector3 pos, Func<object, IList<int>> display_list_query, Action<object> on_item_selected, Func<object, object, string> get_item_hover_text)
	{
		this.itemsPanel.gameObject.transform.position = pos + new Vector3(2f, 2f, 0f);
		IList<int> list = display_list_query(this.widgetID);
		if (list.Count > 0)
		{
			this.ClearSubPanelOptions();
			foreach (int num in list)
			{
				int idx = num;
				GroupSelectorWidget.ItemData itemData = this.options[idx];
				GameObject gameObject = Util.KInstantiateUI(this.itemTemplate, this.itemsPanel.gameObject, true);
				KButton component = gameObject.GetComponent<KButton>();
				component.fgImage.sprite = this.options[idx].sprite;
				component.onClick += delegate
				{
					on_item_selected(this.options[idx].userData);
					this.RebuildSubPanel(pos, display_list_query, on_item_selected, get_item_hover_text);
				};
				if (get_item_hover_text != null)
				{
					ToolTip component2 = gameObject.GetComponent<ToolTip>();
					component2.OnToolTip = () => get_item_hover_text(this.widgetID, this.options[idx].userData);
				}
			}
			GridLayoutGroup component3 = this.itemsPanel.GetComponent<GridLayoutGroup>();
			component3.constraintCount = Mathf.Min(this.numExpectedPanelColumns, this.itemsPanel.childCount);
			this.itemsPanel.gameObject.SetActive(true);
			this.itemsPanel.GetComponent<Selectable>().Select();
		}
		else
		{
			this.CloseSubPanel();
		}
	}

	public void CloseSubPanel()
	{
		this.ClearSubPanelOptions();
		this.itemsPanel.gameObject.SetActive(false);
	}

	private void ClearSubPanelOptions()
	{
		IEnumerator enumerator = this.itemsPanel.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				Transform transform = (Transform)obj;
				Util.KDestroyGameObject(transform.gameObject);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
		}
	}

	public LocText label;

	[SerializeField]
	private GameObject itemTemplate;

	[SerializeField]
	private RectTransform itemsPanel;

	[SerializeField]
	private KButton addItemButton;

	[SerializeField]
	private KButton removeItemButton;

	[SerializeField]
	private KButton sortButton;

	[SerializeField]
	private int numExpectedPanelColumns = 3;

	private object widgetID;

	private GroupSelectorHeaderWidget.ItemCallbacks itemCallbacks;

	private IList<GroupSelectorWidget.ItemData> options;

	public struct ItemCallbacks
	{
		public Func<object, string> getTitleHoverText;

		public Func<object, bool, string> getTitleButtonHoverText;

		public Func<object, bool, IList<int>> getHeaderButtonOptions;

		public Action<object> onItemAdded;

		public Action<object> onItemRemoved;

		public Func<object, bool, object, string> getItemHoverText;

		public Func<object, IList<int>> getValidSortOptionIndices;

		public Func<object, string> getSortHoverText;

		public Action<object, object> onSort;
	}
}
