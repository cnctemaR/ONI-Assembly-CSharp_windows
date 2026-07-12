using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class FilterSideScreen : SingleItemSelectionSideScreenBase
{
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

	public override void ItemRowClicked(SingleItemSelectionRow rowClicked)
	{
		this.SetFilterTag(rowClicked.tag);
		base.ItemRowClicked(rowClicked);
	}

	private void Configure(Filterable filterable)
	{
		Dictionary<Tag, HashSet<Tag>> tagOptions = filterable.GetTagOptions();
		Tag tag = GameTags.Void;
		foreach (Tag tag2 in tagOptions.Keys)
		{
			using (HashSet<Tag>.Enumerator enumerator2 = tagOptions[tag2].GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					if (enumerator2.Current == filterable.SelectedTag)
					{
						tag = tag2;
						break;
					}
				}
			}
		}
		this.SetData(tagOptions);
		SingleItemSelectionSideScreenBase.Category category = null;
		if (this.categories.TryGetValue(GameTags.Void, out category))
		{
			category.SetProihibedState(true);
		}
		if (tag != GameTags.Void)
		{
			this.categories[tag].SetUnfoldedState(SingleItemSelectionSideScreenBase.Category.UnfoldedStates.Unfolded);
		}
		if (this.voidRow == null)
		{
			this.voidRow = this.GetOrCreateItemRow(GameTags.Void);
		}
		this.voidRow.transform.SetAsFirstSibling();
		if (filterable.SelectedTag != GameTags.Void)
		{
			this.SetSelectedItem(filterable.SelectedTag);
		}
		else
		{
			this.SetSelectedItem(this.voidRow);
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
		if (base.CurrentSelectedItem == null || base.CurrentSelectedItem.tag != this.targetFilterable.SelectedTag)
		{
			this.SetSelectedItem(this.targetFilterable.SelectedTag);
		}
		if (this.targetFilterable.SelectedTag != GameTags.Void)
		{
			this.currentSelectionLabel.text = string.Format(locString, this.targetFilterable.SelectedTag.ProperName());
			return;
		}
		this.currentSelectionLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION;
	}

	public HierarchyReferences categoryFoldoutPrefab;

	public RectTransform elementEntryContainer;

	public Image outputIcon;

	public Image everythingElseIcon;

	public LocText outputElementHeaderLabel;

	public LocText everythingElseHeaderLabel;

	public LocText selectElementHeaderLabel;

	public LocText currentSelectionLabel;

	private SingleItemSelectionRow voidRow;

	public bool isLogicFilter;

	private Filterable targetFilterable;
}
