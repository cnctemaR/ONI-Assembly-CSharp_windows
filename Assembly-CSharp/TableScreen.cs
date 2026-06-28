using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class TableScreen : KScreen
{
	protected override void OnActivate()
	{
		base.OnActivate();
		this.title_bar.text = this.title;
		this.ConsumeMouseScroll = true;
		this.CloseButton.onClick += delegate
		{
			ManagementMenu.Instance.CloseAll();
		};
		this.incubating = true;
		base.transform.rectTransform().localScale = Vector3.zero;
		Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
		liveMinionIdentities.OnAdd = (Action<MinionIdentity>)Delegate.Combine(liveMinionIdentities.OnAdd, new Action<MinionIdentity>(delegate(MinionIdentity param)
		{
			this.MarkRowsDirty();
		}));
		Components.Cmps<MinionIdentity> liveMinionIdentities2 = Components.LiveMinionIdentities;
		liveMinionIdentities2.OnRemove = (Action<MinionIdentity>)Delegate.Combine(liveMinionIdentities2.OnRemove, new Action<MinionIdentity>(delegate(MinionIdentity param)
		{
			this.MarkRowsDirty();
		}));
	}

	protected override void OnShow(bool show)
	{
		if (!show)
		{
			this.active_cascade_coroutine_count = 0;
			base.StopAllCoroutines();
			this.StopLoopingCascadeSound();
		}
		this.ZeroScrollers();
		base.OnShow(show);
	}

	private void ZeroScrollers()
	{
		if (this.rows.Count > 0)
		{
			foreach (string text in this.column_scrollers)
			{
				foreach (TableRow tableRow in this.rows)
				{
					tableRow.GetScroller(text).transform.parent.GetComponent<KScrollRect>().horizontalNormalizedPosition = 0f;
				}
			}
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (this.incubating)
		{
			this.ZeroScrollers();
			base.transform.rectTransform().localScale = Vector3.one;
			this.incubating = false;
		}
		if (this.rows_dirty)
		{
			this.RefreshRows();
		}
		foreach (TableRow tableRow in this.rows)
		{
			tableRow.RefreshScrollers();
		}
		foreach (TableColumn tableColumn in this.columns.Values)
		{
			if (tableColumn.isDirty)
			{
				foreach (KeyValuePair<TableRow, GameObject> keyValuePair in tableColumn.widgets_by_row)
				{
					tableColumn.on_load_action(keyValuePair.Key.GetMinionIdentity(), keyValuePair.Value);
					tableColumn.MarkClean();
				}
			}
		}
	}

	protected void MarkRowsDirty()
	{
		this.rows_dirty = true;
	}

	protected virtual void RefreshRows()
	{
		this.ClearRows();
		this.AddRow(null);
		if (this.has_default_duplicant_row)
		{
			this.AddDefaultRow();
		}
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			this.AddRow(Components.LiveMinionIdentities[i]);
		}
		this.SortRows();
		this.rows_dirty = false;
	}

	public virtual void SetSortComparison(Comparison<MinionIdentity> comparison, TableColumn sort_column)
	{
		if (comparison == null)
		{
			return;
		}
		if (this.active_sort_column == sort_column)
		{
			if (this.sort_is_reversed)
			{
				this.sort_is_reversed = false;
				this.active_sort_method = null;
				this.active_sort_column = null;
			}
			else
			{
				this.sort_is_reversed = true;
			}
		}
		else
		{
			this.active_sort_column = sort_column;
			this.active_sort_method = comparison;
			this.sort_is_reversed = false;
		}
	}

	public void SortRows()
	{
		foreach (TableColumn tableColumn in this.columns.Values)
		{
			if (!(tableColumn.column_sort_toggle == null))
			{
				if (tableColumn == this.active_sort_column)
				{
					if (this.sort_is_reversed)
					{
						tableColumn.column_sort_toggle.ChangeState(2);
					}
					else
					{
						tableColumn.column_sort_toggle.ChangeState(1);
					}
				}
				else
				{
					tableColumn.column_sort_toggle.ChangeState(0);
				}
			}
		}
		if (this.active_sort_method == null)
		{
			return;
		}
		Dictionary<MinionIdentity, TableRow> dictionary = new Dictionary<MinionIdentity, TableRow>();
		foreach (TableRow tableRow in this.sortable_rows)
		{
			dictionary.Add(tableRow.GetMinionIdentity(), tableRow);
		}
		List<MinionIdentity> list = new List<MinionIdentity>();
		foreach (KeyValuePair<MinionIdentity, TableRow> keyValuePair in dictionary)
		{
			list.Add(keyValuePair.Key);
		}
		list.Sort(this.active_sort_method);
		if (this.sort_is_reversed)
		{
			list.Reverse();
		}
		this.sortable_rows.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			this.sortable_rows.Add(dictionary[list[i]]);
		}
		for (int j = 0; j < this.sortable_rows.Count; j++)
		{
			this.sortable_rows[j].gameObject.transform.SetSiblingIndex(j);
		}
		if (this.has_default_duplicant_row)
		{
			this.default_row.transform.SetAsFirstSibling();
		}
	}

	protected int compare_rows_alphabetical(MinionIdentity a, MinionIdentity b)
	{
		if (a == null && b == null)
		{
			return 0;
		}
		if (a == null)
		{
			return -1;
		}
		if (b == null)
		{
			return 1;
		}
		return a.GetProperName().CompareTo(b.GetProperName());
	}

	protected int default_sort(TableRow a, TableRow b)
	{
		return 0;
	}

	protected void ClearRows()
	{
		for (int i = this.rows.Count - 1; i >= 0; i--)
		{
			this.rows[i].Clear();
		}
		this.rows.Clear();
		this.sortable_rows.Clear();
	}

	protected void AddRow(MinionIdentity minion)
	{
		bool flag = minion == null;
		GameObject gameObject = global::Util.KInstantiateUI((!flag) ? this.prefab_row_empty : this.prefab_row_header, (!(minion == null)) ? this.scroll_content_transform.gameObject : this.header_content_transform.gameObject, true);
		TableRow component = gameObject.GetComponent<TableRow>();
		if (flag)
		{
			component.rowType = TableRow.RowType.Header;
		}
		else
		{
			component.rowType = TableRow.RowType.Minion;
		}
		this.rows.Add(component);
		component.ConfigureContent(minion, this.columns);
		if (!flag)
		{
			this.sortable_rows.Add(component);
		}
		else
		{
			this.header_row = gameObject;
		}
	}

	protected void AddDefaultRow()
	{
		GameObject gameObject = global::Util.KInstantiateUI(this.prefab_row_empty, this.scroll_content_transform.gameObject, true);
		this.default_row = gameObject;
		TableRow component = gameObject.GetComponent<TableRow>();
		component.rowType = TableRow.RowType.Default;
		component.isDefault = true;
		this.rows.Add(component);
		component.ConfigureContent(null, this.columns);
	}

	protected TableRow GetWidgetRow(GameObject widget_go)
	{
		if (widget_go == null)
		{
			global::Debug.LogWarning("Widget is null", null);
			return null;
		}
		if (this.known_widget_rows.ContainsKey(widget_go))
		{
			return this.known_widget_rows[widget_go];
		}
		foreach (TableRow tableRow in this.rows)
		{
			if (tableRow.ContainsWidget(widget_go))
			{
				this.known_widget_rows.Add(widget_go, tableRow);
				return tableRow;
			}
		}
		global::Debug.LogWarning("Row is null for widget: " + widget_go.name + " parent is " + widget_go.transform.parent.name, null);
		return null;
	}

	protected void StartScrollableContent(string scrollablePanelID)
	{
		if (!this.column_scrollers.Contains(scrollablePanelID))
		{
			DividerColumn dividerColumn = new DividerColumn(() => true, string.Empty);
			this.RegisterColumn("scroller_spacer_" + scrollablePanelID, dividerColumn);
			this.column_scrollers.Add(scrollablePanelID);
		}
	}

	protected PortraitTableColumn AddPortraitColumn(string id, Action<MinionIdentity, GameObject> on_load_action, Comparison<MinionIdentity> sort_comparison, bool double_click_to_target = true)
	{
		PortraitTableColumn portraitTableColumn = new PortraitTableColumn(on_load_action, sort_comparison, double_click_to_target);
		if (this.RegisterColumn(id, portraitTableColumn))
		{
			return portraitTableColumn;
		}
		return null;
	}

	protected ButtonLabelColumn AddButtonLabelColumn(string id, Action<MinionIdentity, GameObject> on_load_action, Func<MinionIdentity, GameObject, string> get_value_action, Action<GameObject> on_click_action, Action<GameObject> on_double_click_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip, bool whiteText = false)
	{
		ButtonLabelColumn buttonLabelColumn = new ButtonLabelColumn(on_load_action, get_value_action, on_click_action, on_double_click_action, sort_comparison, on_tooltip, on_sort_tooltip, whiteText);
		if (this.RegisterColumn(id, buttonLabelColumn))
		{
			return buttonLabelColumn;
		}
		return null;
	}

	protected LabelTableColumn AddLabelColumn(string id, Action<MinionIdentity, GameObject> on_load_action, Func<MinionIdentity, GameObject, string> get_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip, int widget_width = 128, bool should_refresh_columns = false)
	{
		LabelTableColumn labelTableColumn = new LabelTableColumn(on_load_action, get_value_action, sort_comparison, on_tooltip, on_sort_tooltip, widget_width, should_refresh_columns);
		if (this.RegisterColumn(id, labelTableColumn))
		{
			return labelTableColumn;
		}
		return null;
	}

	protected CheckboxTableColumn AddCheckboxColumn(string id, Action<MinionIdentity, GameObject> on_load_action, Func<MinionIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, TableScreen.ResultValues> set_value_function, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip, Action<MinionIdentity, GameObject, ToolTip> on_sort_tooltip)
	{
		CheckboxTableColumn checkboxTableColumn = new CheckboxTableColumn(on_load_action, get_value_action, on_press_action, set_value_function, sort_comparison, on_tooltip, on_sort_tooltip, null);
		if (this.RegisterColumn(id, checkboxTableColumn))
		{
			return checkboxTableColumn;
		}
		return null;
	}

	protected SuperCheckboxTableColumn AddSuperCheckboxColumn(string id, CheckboxTableColumn[] columns_affected, Action<MinionIdentity, GameObject> on_load_action, Func<MinionIdentity, GameObject, TableScreen.ResultValues> get_value_action, Action<GameObject> on_press_action, Action<GameObject, TableScreen.ResultValues> set_value_action, Comparison<MinionIdentity> sort_comparison, Action<MinionIdentity, GameObject, ToolTip> on_tooltip)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = new SuperCheckboxTableColumn(columns_affected, on_load_action, get_value_action, on_press_action, set_value_action, sort_comparison, on_tooltip);
		if (this.RegisterColumn(id, superCheckboxTableColumn))
		{
			foreach (CheckboxTableColumn checkboxTableColumn in columns_affected)
			{
				CheckboxTableColumn checkboxTableColumn2 = checkboxTableColumn;
				checkboxTableColumn2.on_set_action = (Action<GameObject, TableScreen.ResultValues>)Delegate.Combine(checkboxTableColumn2.on_set_action, new Action<GameObject, TableScreen.ResultValues>(superCheckboxTableColumn.MarkDirty));
			}
			superCheckboxTableColumn.MarkDirty(null, TableScreen.ResultValues.False);
			return superCheckboxTableColumn;
		}
		global::Debug.LogWarning("SuperCheckbox column registration failed", null);
		return null;
	}

	protected bool RegisterColumn(string id, TableColumn new_column)
	{
		if (this.columns.ContainsKey(id))
		{
			global::Debug.LogWarning(string.Format("Column with id {0} already in dictionary", id), null);
			return false;
		}
		new_column.screen = this;
		this.columns.Add(id, new_column);
		this.MarkRowsDirty();
		return true;
	}

	protected TableColumn GetWidgetColumn(GameObject widget_go)
	{
		if (this.known_widget_columns.ContainsKey(widget_go))
		{
			return this.known_widget_columns[widget_go];
		}
		foreach (KeyValuePair<string, TableColumn> keyValuePair in this.columns)
		{
			if (keyValuePair.Value.ContainsWidget(widget_go))
			{
				this.known_widget_columns.Add(widget_go, keyValuePair.Value);
				return keyValuePair.Value;
			}
		}
		global::Debug.LogWarning("No column found for widget gameobject " + widget_go.name, null);
		return null;
	}

	protected void on_load_portrait(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = this.GetWidgetRow(widget_go);
		CrewPortrait component = widget_go.GetComponent<CrewPortrait>();
		if (minion != null)
		{
			component.SetIdentityObject(minion, false);
		}
		else if (widgetRow.rowType == TableRow.RowType.Default)
		{
			component.targetImage.enabled = true;
		}
		else
		{
			component.targetImage.enabled = false;
		}
	}

	protected void on_load_name_label(MinionIdentity minion, GameObject widget_go)
	{
		TableRow widgetRow = this.GetWidgetRow(widget_go);
		LocText locText = null;
		HierarchyReferences component = widget_go.GetComponent<HierarchyReferences>();
		LocText locText2 = component.GetReference("Label") as LocText;
		if (component.HasReference("SubLabel"))
		{
			locText = component.GetReference("SubLabel") as LocText;
		}
		if (minion != null)
		{
			locText2.text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
			if (locText != null)
			{
				locText.text = minion.gameObject.GetComponent<MinionResume>().GetCurrentRoleString();
			}
		}
		else
		{
			if (widgetRow.isDefault)
			{
				locText2.text = UI.JOBSCREEN_DEFAULT;
				if (locText != null)
				{
					locText.gameObject.SetActive(false);
				}
			}
			else
			{
				locText2.text = UI.JOBSCREEN_EVERYONE;
			}
			if (locText != null)
			{
				locText.text = string.Empty;
			}
		}
	}

	protected string get_value_name_label(MinionIdentity minion, GameObject widget_go)
	{
		return minion.GetProperName();
	}

	protected void on_load_value_checkbox_column_super(MinionIdentity minion, GameObject widget_go)
	{
		MultiToggle component = widget_go.GetComponent<MultiToggle>();
		TableRow widgetRow = this.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType == TableRow.RowType.Header || rowType == TableRow.RowType.Default || rowType == TableRow.RowType.Minion)
		{
			component.ChangeState((int)this.get_value_checkbox_column_super(minion, widget_go));
		}
	}

	public virtual TableScreen.ResultValues get_value_checkbox_column_super(MinionIdentity minion, GameObject widget_go)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = this.GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = this.GetWidgetRow(widget_go);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		foreach (CheckboxTableColumn checkboxTableColumn in superCheckboxTableColumn.columns_affected)
		{
			if (checkboxTableColumn.isRevealed)
			{
				switch (checkboxTableColumn.get_value_action(widgetRow.GetMinionIdentity(), widgetRow.GetWidget(checkboxTableColumn)))
				{
				case TableScreen.ResultValues.False:
					flag2 = false;
					if (!flag)
					{
						flag5 = true;
					}
					break;
				case TableScreen.ResultValues.Partial:
					flag4 = true;
					flag5 = true;
					break;
				case TableScreen.ResultValues.True:
					flag4 = true;
					flag = false;
					if (!flag2)
					{
						flag5 = true;
					}
					break;
				case TableScreen.ResultValues.ConditionalGroup:
					flag3 = true;
					flag2 = false;
					flag = false;
					break;
				}
				if (flag5)
				{
				}
			}
		}
		TableScreen.ResultValues resultValues = TableScreen.ResultValues.Partial;
		if (flag3 && !flag4 && !flag2 && !flag)
		{
			resultValues = TableScreen.ResultValues.ConditionalGroup;
		}
		else if (flag2)
		{
			resultValues = TableScreen.ResultValues.True;
		}
		else if (flag)
		{
			resultValues = TableScreen.ResultValues.False;
		}
		else if (flag4)
		{
			resultValues = TableScreen.ResultValues.Partial;
		}
		return resultValues;
	}

	protected void set_value_checkbox_column_super(GameObject widget_go, TableScreen.ResultValues new_value)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = this.GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = this.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Header)
		{
			if (rowType != TableRow.RowType.Default)
			{
				if (rowType == TableRow.RowType.Minion)
				{
					base.StartCoroutine(this.CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, widgetRow, new_value, widget_go));
				}
			}
			else
			{
				base.StartCoroutine(this.CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, widgetRow, new_value, widget_go));
			}
		}
		else
		{
			base.StartCoroutine(this.CascadeSetRowCheckBoxes(superCheckboxTableColumn.columns_affected, this.default_row.GetComponent<TableRow>(), new_value, widget_go));
			base.StartCoroutine(this.CascadeSetColumnCheckBoxes(this.sortable_rows, superCheckboxTableColumn, new_value, widget_go));
		}
	}

	protected IEnumerator CascadeSetRowCheckBoxes(CheckboxTableColumn[] checkBoxToggleColumns, TableRow row, TableScreen.ResultValues state, GameObject ignore_widget = null)
	{
		if (this.active_cascade_coroutine_count == 0)
		{
			this.current_looping_sound = LoopingSoundManager.StartSound(this.cascade_sound_path, Vector3.zero, false);
		}
		this.active_cascade_coroutine_count++;
		for (int i = 0; i < checkBoxToggleColumns.Length; i++)
		{
			if (checkBoxToggleColumns[i].widgets_by_row.ContainsKey(row))
			{
				GameObject widget = checkBoxToggleColumns[i].widgets_by_row[row];
				if (!(widget == ignore_widget))
				{
					if (checkBoxToggleColumns[i].isRevealed)
					{
						bool needsSetting = false;
						switch ((this.GetWidgetColumn(widget) as CheckboxTableColumn).get_value_action(row.GetMinionIdentity(), widget))
						{
						case TableScreen.ResultValues.False:
							needsSetting = state != TableScreen.ResultValues.False;
							break;
						case TableScreen.ResultValues.Partial:
						case TableScreen.ResultValues.ConditionalGroup:
							needsSetting = true;
							break;
						case TableScreen.ResultValues.True:
							needsSetting = state != TableScreen.ResultValues.True;
							break;
						}
						if (needsSetting)
						{
							(this.GetWidgetColumn(widget) as CheckboxTableColumn).on_set_action(widget, state);
							yield return null;
						}
					}
				}
			}
		}
		this.active_cascade_coroutine_count--;
		if (this.active_cascade_coroutine_count <= 0)
		{
			this.StopLoopingCascadeSound();
		}
		yield break;
	}

	protected IEnumerator CascadeSetColumnCheckBoxes(List<TableRow> rows, CheckboxTableColumn checkBoxToggleColumn, TableScreen.ResultValues state, GameObject header_widget_go = null)
	{
		if (this.active_cascade_coroutine_count == 0)
		{
			this.current_looping_sound = LoopingSoundManager.StartSound(this.cascade_sound_path, Vector3.zero, false);
		}
		this.active_cascade_coroutine_count++;
		for (int i = 0; i < rows.Count; i++)
		{
			GameObject widget = rows[i].GetWidget(checkBoxToggleColumn);
			if (!(widget == header_widget_go))
			{
				bool needsSetting = false;
				switch ((this.GetWidgetColumn(widget) as CheckboxTableColumn).get_value_action(rows[i].GetMinionIdentity(), widget))
				{
				case TableScreen.ResultValues.False:
					needsSetting = state != TableScreen.ResultValues.False;
					break;
				case TableScreen.ResultValues.Partial:
				case TableScreen.ResultValues.ConditionalGroup:
					needsSetting = true;
					break;
				case TableScreen.ResultValues.True:
					needsSetting = state != TableScreen.ResultValues.True;
					break;
				}
				if (needsSetting)
				{
					(this.GetWidgetColumn(widget) as CheckboxTableColumn).on_set_action(widget, state);
					yield return null;
				}
			}
		}
		if (header_widget_go != null)
		{
			(this.GetWidgetColumn(header_widget_go) as CheckboxTableColumn).on_load_action(null, header_widget_go);
		}
		this.active_cascade_coroutine_count--;
		if (this.active_cascade_coroutine_count <= 0)
		{
			this.StopLoopingCascadeSound();
		}
		yield break;
	}

	private void StopLoopingCascadeSound()
	{
		if (this.current_looping_sound != null)
		{
			LoopingSoundManager.StopSound(this.cascade_sound_path, this.current_looping_sound);
			this.current_looping_sound = null;
		}
	}

	protected void on_press_checkbox_column_super(GameObject widget_go)
	{
		SuperCheckboxTableColumn superCheckboxTableColumn = this.GetWidgetColumn(widget_go) as SuperCheckboxTableColumn;
		TableRow widgetRow = this.GetWidgetRow(widget_go);
		switch (this.get_value_checkbox_column_super(widgetRow.GetMinionIdentity(), widget_go))
		{
		case TableScreen.ResultValues.False:
			superCheckboxTableColumn.on_set_action(widget_go, TableScreen.ResultValues.True);
			break;
		case TableScreen.ResultValues.Partial:
		case TableScreen.ResultValues.ConditionalGroup:
			superCheckboxTableColumn.on_set_action(widget_go, TableScreen.ResultValues.True);
			break;
		case TableScreen.ResultValues.True:
			superCheckboxTableColumn.on_set_action(widget_go, TableScreen.ResultValues.False);
			break;
		}
		superCheckboxTableColumn.on_load_action(widgetRow.GetMinionIdentity(), widget_go);
	}

	protected void on_tooltip_sort_alphabetically(MinionIdentity minion, GameObject widget_go, ToolTip tooltip)
	{
		tooltip.ClearMultiStringTooltip();
		TableRow widgetRow = this.GetWidgetRow(widget_go);
		TableRow.RowType rowType = widgetRow.rowType;
		if (rowType != TableRow.RowType.Default)
		{
			if (rowType != TableRow.RowType.Header)
			{
				if (rowType != TableRow.RowType.Minion)
				{
				}
			}
			else
			{
				tooltip.AddMultiStringTooltip(UI.TABLESCREENS.COLUMN_SORT_BY_NAME, null);
			}
		}
	}

	protected string title;

	protected bool has_default_duplicant_row = true;

	private bool rows_dirty;

	protected Comparison<MinionIdentity> active_sort_method;

	protected TableColumn active_sort_column;

	protected bool sort_is_reversed;

	private int active_cascade_coroutine_count;

	private EventInstance current_looping_sound;

	private bool incubating;

	protected Dictionary<string, TableColumn> columns = new Dictionary<string, TableColumn>();

	public List<TableRow> rows = new List<TableRow>();

	public List<TableRow> sortable_rows = new List<TableRow>();

	public List<string> column_scrollers = new List<string>();

	private Dictionary<GameObject, TableRow> known_widget_rows = new Dictionary<GameObject, TableRow>();

	private Dictionary<GameObject, TableColumn> known_widget_columns = new Dictionary<GameObject, TableColumn>();

	public GameObject prefab_row_empty;

	public GameObject prefab_row_header;

	public GameObject prefab_scroller_border;

	private string cascade_sound_path = GlobalAssets.GetSound("Placers_Unfurl_LP", false);

	public KButton CloseButton;

	[MyCmpGet]
	private VerticalLayoutGroup VLG;

	protected GameObject header_row;

	protected GameObject default_row;

	public LocText title_bar;

	public Transform header_content_transform;

	public Transform scroll_content_transform;

	public Transform scroller_borders_transform;

	public enum ResultValues
	{
		False,
		Partial,
		True,
		ConditionalGroup
	}
}
