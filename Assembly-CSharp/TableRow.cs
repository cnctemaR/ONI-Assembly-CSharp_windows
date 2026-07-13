using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/TableRow")]
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
		MinionIdentity minionIdentity = this.minion as MinionIdentity;
		if (minionIdentity == null)
		{
			return;
		}
		SelectTool.Instance.Select(minionIdentity.GetComponent<KSelectable>(), false);
	}

	public void SelectAndFocusMinion()
	{
		MinionIdentity minionIdentity = this.minion as MinionIdentity;
		if (minionIdentity == null)
		{
			return;
		}
		SelectTool.Instance.SelectAndFocus(minionIdentity.transform.GetPosition(), minionIdentity.GetComponent<KSelectable>(), new Vector3(8f, 0f, 0f));
	}

	public void ConfigureAsWorldDivider(Dictionary<string, TableColumn> columns, TableScreen screen)
	{
		HierarchyReferences component = base.GetComponent<HierarchyReferences>();
		this.scroll_rect = component.GetReference<ScrollRect>("ScrollerScrollRect");
		this.rowType = TableRow.RowType.WorldDivider;
		foreach (KeyValuePair<string, TableColumn> keyValuePair in columns)
		{
			if (keyValuePair.Value.scrollerID != "")
			{
				TableColumn value = keyValuePair.Value;
				break;
			}
		}
		this.scroll_rect.onValueChanged.AddListener(delegate
		{
			if (screen.CheckScrollersDirty())
			{
				return;
			}
			screen.SetScrollersDirty(this.scroll_rect.horizontalNormalizedPosition);
		});
	}

	public void ConfigureContent(IAssignableIdentity minion, Dictionary<string, TableColumn> columns, TableScreen screen)
	{
		this.minion = minion;
		KImage componentInChildren = base.GetComponentInChildren<KImage>(true);
		componentInChildren.colorStyleSetting = ((minion == null) ? this.style_setting_default : this.style_setting_minion);
		componentInChildren.ColorState = KImage.ColorSelector.Inactive;
		CanvasGroup component = base.GetComponent<CanvasGroup>();
		if (component != null && minion as StoredMinionIdentity != null)
		{
			component.alpha = 0.6f;
		}
		UnityAction<Vector2> <>9__0;
		foreach (KeyValuePair<string, TableColumn> keyValuePair in columns)
		{
			GameObject gameObject = base.gameObject;
			if (keyValuePair.Value.scrollerID != "")
			{
				foreach (string text in keyValuePair.Value.screen.column_scrollers)
				{
					if (!(text != keyValuePair.Value.scrollerID))
					{
						if (!this.scrollers.ContainsKey(text))
						{
							GameObject gameObject2 = Util.KInstantiateUI(this.scrollerPrefab, base.gameObject, true);
							this.scroll_rect = gameObject2.GetComponent<ScrollRect>();
							this.scrollbar = gameObject2.GetComponentInChildren<Scrollbar>();
							this.scroll_rect.horizontalScrollbar = this.scrollbar;
							this.scroll_rect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
							UnityEvent<Vector2> onValueChanged = this.scroll_rect.onValueChanged;
							UnityAction<Vector2> unityAction;
							if ((unityAction = <>9__0) == null)
							{
								unityAction = (<>9__0 = delegate
								{
									if (screen.CheckScrollersDirty())
									{
										return;
									}
									screen.SetScrollersDirty(this.scroll_rect.horizontalNormalizedPosition);
								});
							}
							onValueChanged.AddListener(unityAction);
							this.scrollers.Add(text, this.scroll_rect.content.gameObject);
							if (this.scroll_rect.content.transform.parent.Find("Border") != null)
							{
								this.scrollerBorders.Add(text, this.scroll_rect.content.transform.parent.Find("Border").gameObject);
							}
						}
						gameObject = this.scrollers[text];
					}
				}
			}
			GameObject gameObject3;
			if (minion == null)
			{
				if (this.isDefault)
				{
					gameObject3 = keyValuePair.Value.GetDefaultWidget(gameObject);
				}
				else
				{
					gameObject3 = keyValuePair.Value.GetHeaderWidget(gameObject);
				}
			}
			else
			{
				gameObject3 = keyValuePair.Value.GetMinionWidget(gameObject);
			}
			this.widgets.Add(keyValuePair.Value, gameObject3);
			keyValuePair.Value.widgets_by_row.Add(this, gameObject3);
		}
		this.RefreshColumns(columns);
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

	public void PositionScrollerBorders()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.scrollerBorders)
		{
			RectTransform rectTransform = keyValuePair.Value.rectTransform();
			float width = rectTransform.rect.width;
			keyValuePair.Value.transform.SetParent(base.gameObject.transform);
			rectTransform.anchorMin = (rectTransform.anchorMax = new Vector2(0f, 1f));
			rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
			RectTransform rectTransform2 = this.scrollers[keyValuePair.Key].transform.parent.rectTransform();
			Vector3 vector = this.scrollers[keyValuePair.Key].transform.parent.rectTransform().GetLocalPosition() - new Vector3(rectTransform2.sizeDelta.x / 2f, -1f * (rectTransform2.sizeDelta.y / 2f), 0f);
			vector.y = 0f;
			rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 374f);
			rectTransform.SetLocalPosition(vector + Vector3.up * rectTransform.GetLocalPosition().y + Vector3.up * -rectTransform.anchoredPosition.y);
		}
	}

	public void RefreshColumns(Dictionary<string, TableColumn> columns)
	{
		foreach (KeyValuePair<string, TableColumn> keyValuePair in columns)
		{
			if (keyValuePair.Value.on_load_action != null)
			{
				keyValuePair.Value.on_load_action(this.minion, keyValuePair.Value.widgets_by_row[this]);
			}
		}
	}

	public void RefreshScrollers()
	{
		foreach (KeyValuePair<string, GameObject> keyValuePair in this.scrollers)
		{
			ScrollRect component = keyValuePair.Value.transform.parent.GetComponent<ScrollRect>();
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
		global::Debug.LogWarning("Widget is null or row does not contain widget for column " + ((column != null) ? column.ToString() : null));
		return null;
	}

	public IAssignableIdentity GetIdentity()
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

	private IAssignableIdentity minion;

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
	private Scrollbar scrollbar;

	public ScrollRect scroll_rect;

	public enum RowType
	{
		Header,
		Default,
		Minion,
		StoredMinon,
		WorldDivider
	}
}
