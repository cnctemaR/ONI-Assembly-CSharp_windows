using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class FilterSideScreen : SideScreenContent
{
	public FilterSideScreen.elementState FilterElementState { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.SetFilterType();
		this.filterRowMap.Clear();
		this.PopulateElements();
		this.ShowElementsByState();
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			this.SetFilterType();
			if (!(DetailsScreen.Instance.target == null) && !(DetailsScreen.Instance.target.GetComponent<ElementFilter>() == null))
			{
				Element element = ElementLoader.FindElementByHash(SimHashes.Void);
				ElementFilter component = DetailsScreen.Instance.target.GetComponent<ElementFilter>();
				if (component != null)
				{
					element = ElementLoader.FindElementByHash(DetailsScreen.Instance.target.GetComponent<ElementFilter>().FilteredElement);
				}
				this.SetFilterElement(element);
				this.ShowElementsByState();
				this.selectElementHeaderLabel.text = UI.UISIDESCREENS.FILTERSIDESCREEN.SELECTELEMENTHEADER;
				this.everythingElseHeaderLabel.text = ((this.FilterElementState != FilterSideScreen.elementState.gas) ? UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.LIQUID : UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.GAS);
			}
		}
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

	private void SetFilterType()
	{
		GasFilterable component = DetailsScreen.Instance.target.GetComponent<GasFilterable>();
		LiquidFilterable component2 = DetailsScreen.Instance.target.GetComponent<LiquidFilterable>();
		if (component != null)
		{
			this.outputIcon.color = this.outputIconColor;
			this.everythingElseIcon.color = this.colorGas;
			this.FilterElementState = FilterSideScreen.elementState.gas;
		}
		else if (component2 != null)
		{
			this.outputIcon.color = this.outputIconColor;
			this.everythingElseIcon.color = this.colorLiquid;
			this.FilterElementState = FilterSideScreen.elementState.liquid;
		}
	}

	private void ShowElementsByState()
	{
		this.SetFilterType();
		foreach (KeyValuePair<Element, FilterSideScreenRow> keyValuePair in this.filterRowMap)
		{
			bool flag = false;
			if (keyValuePair.Key.IsGas && this.FilterElementState == FilterSideScreen.elementState.gas)
			{
				flag = true;
			}
			else if (keyValuePair.Key.IsLiquid && this.FilterElementState == FilterSideScreen.elementState.liquid)
			{
				flag = true;
			}
			keyValuePair.Value.gameObject.SetActive(flag);
		}
	}

	private void SetFilterElement(Element element)
	{
		this.currentSelectionLabel.text = string.Format((this.FilterElementState != FilterSideScreen.elementState.gas) ? UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.LIQUID : UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.GAS, UI.UISIDESCREENS.FILTERSIDESCREEN.NOELEMENTSELECTED);
		if (DetailsScreen.Instance.target.GetComponent<ElementFilter>().filterable != null)
		{
			DetailsScreen.Instance.target.GetComponent<ElementFilter>().filterable.SelectedTag = element.tag;
		}
		foreach (KeyValuePair<Element, FilterSideScreenRow> keyValuePair in this.filterRowMap)
		{
			if (keyValuePair.Key == element)
			{
				keyValuePair.Value.SetSelected(true);
				if (element.tag != ElementLoader.FindElementByHash(SimHashes.Void).tag && element.tag != ElementLoader.FindElementByHash(SimHashes.Vacuum).tag)
				{
					this.currentSelectionLabel.text = string.Format((this.FilterElementState != FilterSideScreen.elementState.gas) ? UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.LIQUID : UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.GAS, element.name);
				}
			}
			else
			{
				keyValuePair.Value.SetSelected(false);
			}
		}
	}

	public GameObject elementEntryPrefab;

	public GameObject elementEntryContainer;

	public Image outputIcon;

	public Image everythingElseIcon;

	public LocText everythingElseHeaderLabel;

	public LocText selectElementHeaderLabel;

	private Color outputIconColor = BuildingCellVisualizer.secondOutputColour;

	private Color colorLiquid = Color.white;

	private Color colorGas = Color.white;

	public Dictionary<Element, FilterSideScreenRow> filterRowMap = new Dictionary<Element, FilterSideScreenRow>();

	public LocText currentSelectionLabel;

	public enum elementState
	{
		solid,
		liquid,
		gas
	}
}
