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
		this.PopulateElements();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<Filterable>() != null;
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (!show)
		{
			return;
		}
		if (DetailsScreen.Instance.target == null)
		{
			return;
		}
		Filterable component = DetailsScreen.Instance.target.GetComponent<Filterable>();
		if (component == null)
		{
			return;
		}
		this.selectElementHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.SELECTELEMENTHEADER;
		this.everythingElseHeaderLabel.text = ((component.filterElementState != Filterable.ElementState.Gas) ? UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.LIQUID : UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.GAS);
		Element element = ((!component.SelectedTag.IsValid) ? null : ElementLoader.GetElement(component.SelectedTag));
		this.SetFilterElement(element);
		this.Configure(component);
	}

	private void PopulateElements()
	{
		List<Element> list = new List<Element>(ElementLoader.elements);
		list.Sort((Element a, Element b) => a.name.CompareTo(b.name));
		foreach (Element element in list)
		{
			FilterSideScreenRow row = Util.KInstantiateUI(this.elementEntryPrefab, this.elementEntryContainer, false).GetComponent<FilterSideScreenRow>();
			row.SetElement(element);
			row.button.onClick += delegate
			{
				this.SetFilterElement(row.element);
			};
			this.filterRowMap.Add(row.element, row);
		}
	}

	private void Configure(Filterable filterable)
	{
		IList<Tag> tagOptions = filterable.GetTagOptions();
		foreach (KeyValuePair<Element, FilterSideScreenRow> keyValuePair in this.filterRowMap)
		{
			Element key = keyValuePair.Key;
			bool flag = tagOptions.Contains(key.tag);
			keyValuePair.Value.gameObject.SetActive(flag);
		}
	}

	private void SetFilterElement(Element element)
	{
		Filterable component = DetailsScreen.Instance.target.GetComponent<Filterable>();
		if (component == null)
		{
			return;
		}
		LocString locString = ((component.filterElementState != Filterable.ElementState.Gas) ? UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.LIQUID : UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.GAS);
		this.currentSelectionLabel.text = string.Format(locString, UI.UISIDESCREENS.FILTERSIDESCREEN.NOELEMENTSELECTED);
		if (element != null)
		{
			component.SelectedTag = element.tag;
			foreach (KeyValuePair<Element, FilterSideScreenRow> keyValuePair in this.filterRowMap)
			{
				bool flag = keyValuePair.Key == element;
				keyValuePair.Value.SetSelected(flag);
				if (flag && element.id != SimHashes.Void && element.id != SimHashes.Vacuum)
				{
					this.currentSelectionLabel.text = string.Format(locString, element.name);
				}
			}
		}
	}

	public GameObject elementEntryPrefab;

	public GameObject elementEntryContainer;

	public Image outputIcon;

	public Image everythingElseIcon;

	public LocText everythingElseHeaderLabel;

	public LocText selectElementHeaderLabel;

	public LocText currentSelectionLabel;

	public Dictionary<Element, FilterSideScreenRow> filterRowMap = new Dictionary<Element, FilterSideScreenRow>();
}
