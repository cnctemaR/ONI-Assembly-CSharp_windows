using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class FilterSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.filterRowMap.Clear();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		bool flag;
		if (this.isLogicFilter)
		{
			flag = target.GetComponent<ConduitElementSensor>() != null || target.GetComponent<LogicElementSensor>() != null;
		}
		else
		{
			flag = target.GetComponent<ElementFilter>() != null || target.GetComponent<RocketConduitStorageAccess>() != null || target.GetComponent<DevPump>() != null;
		}
		return flag && target.GetComponent<Filterable>() != null;
	}

	public override void SetTarget(GameObject target)
	{
		base.SetTarget(target);
		this.targetFilterable = target.GetComponent<Filterable>();
		if (this.targetFilterable == null)
		{
			return;
		}
		switch (this.targetFilterable.filterElementState)
		{
		case Filterable.ElementState.Solid:
			this.everythingElseHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.SOLID;
			goto IL_0087;
		case Filterable.ElementState.Gas:
			this.everythingElseHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.GAS;
			goto IL_0087;
		}
		this.everythingElseHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.LIQUID;
		IL_0087:
		this.Configure(this.targetFilterable);
		this.SetFilterTag(this.targetFilterable.SelectedTag);
	}

	private void ToggleCategory(Tag tag, bool forceOn = false)
	{
		HierarchyReferences hierarchyReferences = this.categoryToggles[tag];
		if (hierarchyReferences != null)
		{
			MultiToggle reference = hierarchyReferences.GetReference<MultiToggle>("Toggle");
			if (!forceOn)
			{
				reference.NextState();
			}
			else
			{
				reference.ChangeState(1);
			}
			hierarchyReferences.GetReference<RectTransform>("Entries").gameObject.SetActive(reference.CurrentState != 0);
		}
	}

	private void Configure(Filterable filterable)
	{
		Dictionary<Tag, HashSet<Tag>> tagOptions = filterable.GetTagOptions();
		using (Dictionary<Tag, HashSet<Tag>>.Enumerator enumerator = tagOptions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<Tag, HashSet<Tag>> category_tags = enumerator.Current;
				if (!this.filterRowMap.ContainsKey(category_tags.Key))
				{
					if (category_tags.Key != GameTags.Void)
					{
						HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.categoryFoldoutPrefab.gameObject, this.elementEntryContainer.gameObject, false);
						hierarchyReferences.GetReference<LocText>("Label").text = category_tags.Key.ProperName();
						hierarchyReferences.GetReference<MultiToggle>("Toggle").onClick = delegate
						{
							this.ToggleCategory(category_tags.Key, false);
						};
						this.categoryToggles.Add(category_tags.Key, hierarchyReferences);
					}
					this.filterRowMap[category_tags.Key] = new SortedDictionary<Tag, FilterSideScreenRow>(FilterSideScreen.comparer);
				}
				else if (category_tags.Key == GameTags.Void && !this.filterRowMap.ContainsKey(category_tags.Key))
				{
					this.filterRowMap[category_tags.Key] = new SortedDictionary<Tag, FilterSideScreenRow>(FilterSideScreen.comparer);
				}
				foreach (Tag tag in category_tags.Value)
				{
					if (!this.filterRowMap[category_tags.Key].ContainsKey(tag))
					{
						RectTransform rectTransform = ((category_tags.Key != GameTags.Void) ? this.categoryToggles[category_tags.Key].GetReference<RectTransform>("Entries") : this.elementEntryContainer);
						FilterSideScreenRow row = Util.KInstantiateUI<FilterSideScreenRow>(this.elementEntryPrefab.gameObject, rectTransform.gameObject, false);
						row.SetTag(tag);
						row.button.onClick += delegate
						{
							this.SetFilterTag(row.tag);
						};
						this.filterRowMap[category_tags.Key].Add(row.tag, row);
					}
				}
			}
		}
		int num = 0;
		this.filterRowMap[GameTags.Void][GameTags.Void].transform.SetSiblingIndex(num++);
		foreach (KeyValuePair<Tag, SortedDictionary<Tag, FilterSideScreenRow>> keyValuePair in this.filterRowMap)
		{
			if (tagOptions.ContainsKey(keyValuePair.Key) && tagOptions[keyValuePair.Key].Count > 0)
			{
				if (keyValuePair.Key != GameTags.Void)
				{
					this.categoryToggles[keyValuePair.Key].name = "CATE " + num.ToString();
					this.categoryToggles[keyValuePair.Key].transform.SetSiblingIndex(num++);
					this.categoryToggles[keyValuePair.Key].gameObject.SetActive(true);
				}
				int num2 = 0;
				using (SortedDictionary<Tag, FilterSideScreenRow>.Enumerator enumerator4 = keyValuePair.Value.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						KeyValuePair<Tag, FilterSideScreenRow> keyValuePair2 = enumerator4.Current;
						keyValuePair2.Value.name = "ELE " + num2.ToString();
						keyValuePair2.Value.transform.SetSiblingIndex(num2++);
						keyValuePair2.Value.gameObject.SetActive(tagOptions[keyValuePair.Key].Contains(keyValuePair2.Value.tag));
						if (keyValuePair2.Key != GameTags.Void && keyValuePair2.Key == this.targetFilterable.SelectedTag)
						{
							this.ToggleCategory(keyValuePair.Key, true);
						}
					}
					continue;
				}
			}
			if (keyValuePair.Key != GameTags.Void)
			{
				this.categoryToggles[keyValuePair.Key].gameObject.SetActive(false);
			}
		}
		this.RefreshUI();
	}

	private void SetFilterTag(Tag tag)
	{
		if (this.targetFilterable == null)
		{
			return;
		}
		if (tag.IsValid)
		{
			this.targetFilterable.SelectedTag = tag;
		}
		this.RefreshUI();
	}

	private void RefreshUI()
	{
		LocString locString;
		switch (this.targetFilterable.filterElementState)
		{
		case Filterable.ElementState.Solid:
			locString = UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.SOLID;
			goto IL_0038;
		case Filterable.ElementState.Gas:
			locString = UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.GAS;
			goto IL_0038;
		}
		locString = UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.LIQUID;
		IL_0038:
		this.currentSelectionLabel.text = string.Format(locString, UI.UISIDESCREENS.FILTERSIDESCREEN.NOELEMENTSELECTED);
		foreach (KeyValuePair<Tag, SortedDictionary<Tag, FilterSideScreenRow>> keyValuePair in this.filterRowMap)
		{
			foreach (KeyValuePair<Tag, FilterSideScreenRow> keyValuePair2 in keyValuePair.Value)
			{
				bool flag = keyValuePair2.Key == this.targetFilterable.SelectedTag;
				keyValuePair2.Value.SetSelected(flag);
				if (flag)
				{
					if (keyValuePair2.Value.tag != GameTags.Void)
					{
						this.currentSelectionLabel.text = string.Format(locString, this.targetFilterable.SelectedTag.ProperName());
					}
					else
					{
						this.currentSelectionLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION;
					}
				}
			}
		}
	}

	public HierarchyReferences categoryFoldoutPrefab;

	public FilterSideScreenRow elementEntryPrefab;

	public RectTransform elementEntryContainer;

	public Image outputIcon;

	public Image everythingElseIcon;

	public LocText outputElementHeaderLabel;

	public LocText everythingElseHeaderLabel;

	public LocText selectElementHeaderLabel;

	public LocText currentSelectionLabel;

	private static TagNameComparer comparer = new TagNameComparer(GameTags.Void);

	public Dictionary<Tag, HierarchyReferences> categoryToggles = new Dictionary<Tag, HierarchyReferences>();

	public SortedDictionary<Tag, SortedDictionary<Tag, FilterSideScreenRow>> filterRowMap = new SortedDictionary<Tag, SortedDictionary<Tag, FilterSideScreenRow>>(FilterSideScreen.comparer);

	public bool isLogicFilter;

	private Filterable targetFilterable;
}
