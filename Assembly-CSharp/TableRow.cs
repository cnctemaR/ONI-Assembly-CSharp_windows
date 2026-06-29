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

	public GameObject GetScroller(string scrollerID)
	{
		return this.scrollers[scrollerID];
	}

	public GameObject GetScrollerBorder(string scrolledID)
	{
		return this.scrollerBorders[scrolledID];
	}

	public void SelectMinion()
	{
		if (this.minion == null)
		{
			return;
		}
		SelectTool.Instance.Select(this.minion.GetComponent<KSelectable>(), false);
	}

	public void SelectAndFocusMinion()
	{
		if (this.minion == null)
		{
			return;
		}
		SelectTool.Instance.SelectAndFocus(this.minion.transform.GetPosition(), this.minion.GetComponent<KSelectable>(), new Vector3(8f, 0f, 0f));
	}

	public void ConfigureContent(MinionIdentity minion, Dictionary<string, TableColumn> columns)
	{
		this.minion = minion;
		KImage componentInChildren = base.GetComponentInChildren<KImage>(true);
		componentInChildren.colorStyleSetting = ((!(minion == null)) ? this.style_setting_minion : this.style_setting_default);
		componentInChildren.ColorState = KImage.ColorSelector.Inactive;
		using (Dictionary<string, TableColumn>.Enumerator enumerator = columns.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, TableColumn> column = enumerator.Current;
				GameObject gameObject;
				if (minion == null)
				{
					if (this.isDefault)
					{
						gameObject = column.Value.GetDefaultWidget(base.gameObject);
					}
					else
					{
						gameObject = column.Value.GetHeaderWidget(base.gameObject);
					}
				}
				else
				{
					gameObject = column.Value.GetMinionWidget(base.gameObject);
				}
				this.widgets.Add(column.Value, gameObject);
				column.Value.widgets_by_row.Add(this, gameObject);
				if (column.Key.Contains("scroller_spacer_") && (minion != null || this.isDefault))
				{
					gameObject.GetComponentInChildren<LayoutElement>().minWidth += 3f;
				}
				if (column.Value.scrollerID != string.Empty)
				{
					foreach (string text in column.Value.screen.column_scrollers)
					{
						if (text == column.Value.scrollerID)
						{
							if (!this.scrollers.ContainsKey(text))
							{
								GameObject gameObject2 = Util.KInstantiateUI(this.scrollerPrefab, base.gameObject, true);
								KScrollRect scroll_rect = gameObject2.GetComponent<KScrollRect>();
								scroll_rect.onValueChanged.AddListener(delegate
								{
									foreach (TableRow tableRow in column.Value.screen.rows)
									{
										KScrollRect componentInChildren2 = tableRow.GetComponentInChildren<KScrollRect>();
										if (componentInChildren2 != null)
										{
											componentInChildren2.horizontalNormalizedPosition = scroll_rect.horizontalNormalizedPosition;
										}
									}
								});
								this.scrollers.Add(text, scroll_rect.content.gameObject);
								Transform transform = scroll_rect.content.transform.parent.Find("Border");
								if (transform != null)
								{
									this.scrollerBorders.Add(text, scroll_rect.content.transform.parent.Find("Border").gameObject);
								}
							}
							gameObject.transform.SetParent(this.scrollers[text].transform);
							this.scrollers[text].transform.parent.GetComponent<KScrollRect>().horizontalNormalizedPosition = 0f;
						}
					}
				}
			}
		}
		foreach (KeyValuePair<string, TableColumn> keyValuePair in columns)
		{
			if (keyValuePair.Value.on_load_action != null)
			{
				keyValuePair.Value.on_load_action(minion, keyValuePair.Value.widgets_by_row[this]);
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
		foreach (KeyValuePair<string, GameObject> keyValuePair2 in this.scrollerBorders)
		{
			RectTransform rectTransform = keyValuePair2.Value.rectTransform();
			float width = rectTransform.rect.width;
			keyValuePair2.Value.transform.SetParent(base.gameObject.transform);
			RectTransform rectTransform2 = rectTransform;
			Vector2 vector = new Vector2(0f, 1f);
			rectTransform.anchorMax = vector;
			rectTransform2.anchorMin = vector;
			rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
			RectTransform rectTransform3 = this.scrollers[keyValuePair2.Key].transform.parent.rectTransform();
			Vector3 vector2 = this.scrollers[keyValuePair2.Key].transform.parent.rectTransform().GetLocalPosition() - new Vector3(rectTransform3.sizeDelta.x / 2f, -1f * (rectTransform3.sizeDelta.y / 2f), 0f);
			vector2.y = 0f;
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 374f);
			rectTransform.SetLocalPosition(vector2 + Vector3.up * rectTransform.GetLocalPosition().y + Vector3.up * -rectTransform.anchoredPosition.y);
		}
	}

	public void RefreshScrollers()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.scrollers)
		{
			KScrollRect component = keyValuePair.Value.transform.parent.GetComponent<KScrollRect>();
			component.GetComponent<LayoutElement>().minWidth = Mathf.Min(768f, component.content.sizeDelta.x);
		}
		foreach (KeyValuePair<string, GameObject> keyValuePair2 in this.scrollerBorders)
		{
			RectTransform rectTransform = keyValuePair2.Value.rectTransform();
			rectTransform.sizeDelta = new Vector2(this.scrollers[keyValuePair2.Key].transform.parent.GetComponent<LayoutElement>().minWidth, rectTransform.sizeDelta.y);
		}
	}

	public GameObject GetWidget(TableColumn column)
	{
		if (this.widgets.ContainsKey(column) && this.widgets[column] != null)
		{
			return this.widgets[column];
		}
		global::Debug.LogWarning("Widget is null or row does not contain widget for column " + column, null);
		return null;
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

	private Dictionary<string, GameObject> scrollers = new Dictionary<string, GameObject>();

	private Dictionary<string, GameObject> scrollerBorders = new Dictionary<string, GameObject>();

	public bool isDefault;

	public KButton selectMinionButton;

	[SerializeField]
	private ColorStyleSetting style_setting_default;

	[SerializeField]
	private ColorStyleSetting style_setting_minion;

	[SerializeField]
	private GameObject scrollerPrefab;

	[SerializeField]
	private GameObject scrollbarPrefab;

	public enum RowType
	{
		Header,
		Default,
		Minion
	}
}
