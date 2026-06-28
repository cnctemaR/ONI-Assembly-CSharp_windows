using System;
using System.Collections.Generic;
using UnityEngine;

public class TableColumn : IRender1000ms
{
	public TableColumn(Action<MinionIdentity, GameObject> on_load_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip = null, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip = null, Func<bool> revealed = null, bool should_refresh_columns = false, string scrollerID = "")
	{
		this.on_load_action = on_load_action;
		this.sort_comparer = sort_comparison;
		this.on_tooltip = on_tooltip;
		this.on_sort_tooltip = on_sort_tooltip;
		this.revealed = revealed;
		this.scrollerID = scrollerID;
		if (should_refresh_columns)
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}
	}

	public bool isRevealed
	{
		get
		{
			return this.revealed == null || this.revealed();
		}
	}

	protected string GetTooltip(ToolTip tool_tip_instance)
	{
		GameObject gameObject = tool_tip_instance.gameObject;
		HierarchyReferences component = tool_tip_instance.GetComponent<HierarchyReferences>();
		if (component != null && component.HasReference("Widget"))
		{
			gameObject = component.GetReference("Widget").gameObject;
		}
		TableRow tableRow = null;
		foreach (KeyValuePair<TableRow, GameObject> keyValuePair in this.widgets_by_row)
		{
			if (keyValuePair.Value == gameObject)
			{
				tableRow = keyValuePair.Key;
				break;
			}
		}
		if (tableRow != null && this.on_tooltip != null)
		{
			this.on_tooltip(tableRow.GetMinionIdentity(), gameObject, tool_tip_instance);
		}
		return string.Empty;
	}

	protected string GetSortTooltip(ToolTip sort_tooltip_instance)
	{
		GameObject gameObject = sort_tooltip_instance.transform.parent.gameObject;
		TableRow tableRow = null;
		foreach (KeyValuePair<TableRow, GameObject> keyValuePair in this.widgets_by_row)
		{
			if (keyValuePair.Value == gameObject)
			{
				tableRow = keyValuePair.Key;
				break;
			}
		}
		if (tableRow != null && this.on_sort_tooltip != null)
		{
			this.on_sort_tooltip(tableRow.GetMinionIdentity(), gameObject, sort_tooltip_instance);
		}
		return string.Empty;
	}

	public bool isDirty
	{
		get
		{
			return this.dirty;
		}
	}

	public bool ContainsWidget(GameObject widget)
	{
		return this.widgets_by_row.ContainsValue(widget);
	}

	public virtual GameObject GetMinionWidget(GameObject parent)
	{
		global::Debug.LogError("Table Column has no Widget prefab", null);
		return null;
	}

	public virtual GameObject GetHeaderWidget(GameObject parent)
	{
		global::Debug.LogError("Table Column has no Widget prefab", null);
		return null;
	}

	public virtual GameObject GetDefaultWidget(GameObject parent)
	{
		global::Debug.LogError("Table Column has no Widget prefab", null);
		return null;
	}

	public void Render1000ms(float dt)
	{
		this.MarkDirty(null, TableScreen.ResultValues.False);
	}

	public void MarkDirty(GameObject triggering_obj = null, TableScreen.ResultValues triggering_object_state = TableScreen.ResultValues.False)
	{
		this.dirty = true;
	}

	public void MarkClean()
	{
		this.dirty = false;
	}

	public Action<MinionIdentity, GameObject> on_load_action;

	public Action<MinionIdentity, GameObject, ToolTip> on_tooltip;

	public Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip;

	public Comparison<MinionIdentity> sort_comparer;

	public Dictionary<TableRow, GameObject> widgets_by_row = new Dictionary<TableRow, GameObject>();

	public string scrollerID;

	public TableScreen screen;

	public MultiToggle column_sort_toggle;

	private Func<bool> revealed;

	protected bool dirty;
}
