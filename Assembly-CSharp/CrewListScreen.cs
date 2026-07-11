using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class CrewListScreen<EntryType> : KScreen where EntryType : CrewListEntry
{
	protected override void OnActivate()
	{
		base.OnActivate();
		this.ClearEntries();
		this.SpawnEntries();
		this.PositionColumnTitles();
		if (this.autoColumn)
		{
			this.UpdateColumnTitles();
		}
		base.ConsumeMouseScroll = true;
	}

	protected override void OnCmpEnable()
	{
		if (this.autoColumn)
		{
			this.UpdateColumnTitles();
		}
		this.Reconstruct();
	}

	private void ClearEntries()
	{
		for (int i = this.EntryObjects.Count - 1; i > -1; i--)
		{
			Util.KDestroyGameObject(this.EntryObjects[i]);
		}
		this.EntryObjects.Clear();
	}

	protected void RefreshCrewPortraitContent()
	{
		this.EntryObjects.ForEach(delegate(EntryType eo)
		{
			eo.RefreshCrewPortraitContent();
		});
	}

	protected virtual void SpawnEntries()
	{
		if (this.EntryObjects.Count != 0)
		{
			this.ClearEntries();
		}
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (this.autoColumn)
		{
			this.UpdateColumnTitles();
		}
		bool flag = false;
		List<MinionIdentity> liveIdentities = new List<MinionIdentity>(Components.LiveMinionIdentities.Items);
		if (this.EntryObjects.Count != liveIdentities.Count || this.EntryObjects.FindAll((EntryType o) => liveIdentities.Contains(o.Identity)).Count != this.EntryObjects.Count)
		{
			flag = true;
		}
		if (flag)
		{
			this.Reconstruct();
		}
		this.UpdateScroll();
	}

	public void Reconstruct()
	{
		this.ClearEntries();
		this.SpawnEntries();
	}

	private void UpdateScroll()
	{
		if (this.PanelScrollbar)
		{
			if (this.EntryObjects.Count <= this.maxEntriesBeforeScroll)
			{
				this.PanelScrollbar.value = Mathf.Lerp(this.PanelScrollbar.value, 1f, 10f);
				this.PanelScrollbar.gameObject.SetActive(false);
				return;
			}
			this.PanelScrollbar.gameObject.SetActive(true);
		}
	}

	private void SetHeadersActive(bool state)
	{
		for (int i = 0; i < this.ColumnTitlesContainer.childCount; i++)
		{
			this.ColumnTitlesContainer.GetChild(i).gameObject.SetActive(state);
		}
	}

	protected virtual void PositionColumnTitles()
	{
		if (this.ColumnTitlesContainer == null)
		{
			return;
		}
		if (this.EntryObjects.Count <= 0)
		{
			this.SetHeadersActive(false);
			return;
		}
		this.SetHeadersActive(true);
		int childCount = this.EntryObjects[0].transform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			OverviewColumnIdentity component = this.EntryObjects[0].transform.GetChild(i).GetComponent<OverviewColumnIdentity>();
			if (component != null)
			{
				GameObject gameObject = Util.KInstantiate(this.Prefab_ColumnTitle, null, null);
				gameObject.name = component.Column_DisplayName;
				LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
				gameObject.transform.SetParent(this.ColumnTitlesContainer);
				componentInChildren.text = (component.StringLookup ? Strings.Get(component.Column_DisplayName) : component.Column_DisplayName);
				gameObject.GetComponent<ToolTip>().toolTip = string.Format(UI.TOOLTIPS.SORTCOLUMN, componentInChildren.text);
				gameObject.rectTransform().anchoredPosition = new Vector2(component.rectTransform().anchoredPosition.x, 0f);
				OverviewColumnIdentity overviewColumnIdentity = gameObject.GetComponent<OverviewColumnIdentity>();
				if (overviewColumnIdentity == null)
				{
					overviewColumnIdentity = gameObject.AddComponent<OverviewColumnIdentity>();
				}
				overviewColumnIdentity.Column_DisplayName = component.Column_DisplayName;
				overviewColumnIdentity.columnID = component.columnID;
				overviewColumnIdentity.xPivot = component.xPivot;
				overviewColumnIdentity.Sortable = component.Sortable;
				if (overviewColumnIdentity.Sortable)
				{
					overviewColumnIdentity.GetComponentInChildren<ImageToggleState>(true).gameObject.SetActive(true);
				}
			}
		}
		this.UpdateColumnTitles();
		this.sortToggleGroup = base.gameObject.AddComponent<ToggleGroup>();
		this.sortToggleGroup.allowSwitchOff = true;
	}

	protected void SortByName(bool reverse)
	{
		List<EntryType> list = new List<EntryType>(this.EntryObjects);
		list.Sort(delegate(EntryType a, EntryType b)
		{
			string text = a.Identity.GetProperName() + a.gameObject.GetInstanceID();
			string text2 = b.Identity.GetProperName() + b.gameObject.GetInstanceID();
			return text.CompareTo(text2);
		});
		this.ReorderEntries(list, reverse);
	}

	protected void UpdateColumnTitles()
	{
		if (this.EntryObjects.Count <= 0 || !this.EntryObjects[0].gameObject.activeSelf)
		{
			this.SetHeadersActive(false);
			return;
		}
		this.SetHeadersActive(true);
		for (int i = 0; i < this.ColumnTitlesContainer.childCount; i++)
		{
			RectTransform rectTransform = this.ColumnTitlesContainer.GetChild(i).rectTransform();
			for (int j = 0; j < this.EntryObjects[0].transform.childCount; j++)
			{
				OverviewColumnIdentity component = this.EntryObjects[0].transform.GetChild(j).GetComponent<OverviewColumnIdentity>();
				if (component != null && component.Column_DisplayName == rectTransform.name)
				{
					rectTransform.pivot = new Vector2(component.xPivot, rectTransform.pivot.y);
					rectTransform.anchoredPosition = new Vector2(component.rectTransform().anchoredPosition.x + this.columnTitleHorizontalOffset, 0f);
					rectTransform.sizeDelta = new Vector2(component.rectTransform().sizeDelta.x, rectTransform.sizeDelta.y);
					if (rectTransform.anchoredPosition.x == 0f)
					{
						rectTransform.gameObject.SetActive(false);
					}
					else
					{
						rectTransform.gameObject.SetActive(true);
					}
				}
			}
		}
	}

	protected void ReorderEntries(List<EntryType> sortedEntries, bool reverse)
	{
		for (int i = 0; i < sortedEntries.Count; i++)
		{
			if (reverse)
			{
				sortedEntries[i].transform.SetSiblingIndex(sortedEntries.Count - 1 - i);
			}
			else
			{
				sortedEntries[i].transform.SetSiblingIndex(i);
			}
		}
	}

	public GameObject Prefab_CrewEntry;

	public List<EntryType> EntryObjects = new List<EntryType>();

	public Transform ScrollRectTransform;

	public Transform EntriesPanelTransform;

	protected Vector2 EntryRectSize = new Vector2(750f, 64f);

	public int maxEntriesBeforeScroll = 5;

	public Scrollbar PanelScrollbar;

	protected ToggleGroup sortToggleGroup;

	protected Toggle lastSortToggle;

	protected bool lastSortReversed;

	public GameObject Prefab_ColumnTitle;

	public Transform ColumnTitlesContainer;

	public bool autoColumn;

	public float columnTitleHorizontalOffset;
}
