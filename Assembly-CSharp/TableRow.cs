using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableRow : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.selectMinionButton != null)
		{
			this.selectMinionButton.onClick += this.SelectMinion;
			this.selectMinionButton.onDoubleClick += this.SelectAndFocusMinion;
		}
	}

	public void SelectMinion()
	{
		if (!(this.minion == null))
		{
			SelectTool.Instance.Select(this.minion.GetComponent<KSelectable>(), false);
		}
	}

	public void SelectAndFocusMinion()
	{
		if (!(this.minion == null))
		{
			SelectTool.Instance.SelectAndFocus(this.minion.transform.position, this.minion.GetComponent<KSelectable>(), new Vector3(5f, 0f, 0f));
		}
	}

	public void ConfigureContent(MinionIdentity minion, Dictionary<string, TableColumn> columns)
	{
		this.minion = minion;
		KImage componentInChildren = base.GetComponentInChildren<KImage>(true);
		componentInChildren.colorStyleSetting = ((!(minion == null)) ? this.style_setting_minion : this.style_setting_default);
		componentInChildren.ColorState = KImage.ColorSelector.Inactive;
		foreach (KeyValuePair<string, TableColumn> keyValuePair in columns)
		{
			GameObject gameObject;
			if (minion == null)
			{
				if (this.isDefault)
				{
					gameObject = keyValuePair.Value.GetDefaultWidget(base.gameObject);
				}
				else
				{
					gameObject = keyValuePair.Value.GetHeaderWidget(base.gameObject);
				}
			}
			else
			{
				gameObject = keyValuePair.Value.GetMinionWidget(base.gameObject);
			}
			this.widgets.Add(keyValuePair.Value, gameObject);
			keyValuePair.Value.widgets_by_row.Add(this, gameObject);
		}
		foreach (KeyValuePair<string, TableColumn> keyValuePair2 in columns)
		{
			if (keyValuePair2.Value.on_load_action != null)
			{
				keyValuePair2.Value.on_load_action(minion, keyValuePair2.Value.widgets_by_row[this]);
			}
		}
		if (minion != null)
		{
			base.gameObject.name = minion.GetProperName();
		}
		else if (this.isDefault)
		{
			base.gameObject.name = "defaultRow";
		}
		if (this.selectMinionButton)
		{
			this.selectMinionButton.transform.SetAsLastSibling();
		}
	}

	public GameObject GetWidget(TableColumn column)
	{
		GameObject gameObject;
		if (this.widgets.ContainsKey(column) && this.widgets[column] != null)
		{
			gameObject = this.widgets[column];
		}
		else
		{
			global::Debug.LogWarning("Widget is null or row does not contain widget for column " + column, null);
			gameObject = null;
		}
		return gameObject;
	}

	public MinionIdentity GetMinionIdentity()
	{
		return this.minion;
	}

	public bool ContainsWidget(GameObject widget)
	{
		return this.widgets.ContainsValue(widget);
	}

	public void Clear()
	{
		foreach (KeyValuePair<TableColumn, GameObject> keyValuePair in this.widgets)
		{
			keyValuePair.Key.widgets_by_row.Remove(this);
		}
		global::UnityEngine.Object.Destroy(base.gameObject);
	}

	public TableRow.RowType rowType;

	private MinionIdentity minion;

	private Dictionary<TableColumn, GameObject> widgets = new Dictionary<TableColumn, GameObject>();

	public bool isDefault = false;

	[MyCmpGet]
	private HorizontalLayoutGroup HLG;

	public KButton selectMinionButton;

	[SerializeField]
	private ColorStyleSetting style_setting_default;

	[SerializeField]
	private ColorStyleSetting style_setting_minion;

	public enum RowType
	{
		Header,
		Default,
		Minion
	}
}
