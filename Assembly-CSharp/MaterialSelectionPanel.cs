using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MaterialSelectionPanel : KScreen
{
	public Element CurrentSelectedElement
	{
		get
		{
			return this.MaterialSelectors[0].CurrentSelectedElement;
		}
	}

	public IList<Element> GetSelectedElementAsList
	{
		get
		{
			this.currentSelectedElements.Clear();
			foreach (MaterialSelector materialSelector in this.MaterialSelectors)
			{
				if (materialSelector.gameObject.activeSelf)
				{
					this.currentSelectedElements.Add(materialSelector.CurrentSelectedElement);
				}
			}
			return this.currentSelectedElements;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ConsumeMouseScroll = true;
		for (int i = 0; i < 3; i++)
		{
			MaterialSelector materialSelector = Util.KInstantiateUI<MaterialSelector>(this.MaterialSelectorTemplate, base.gameObject, false);
			this.MaterialSelectors.Add(materialSelector);
		}
		this.MaterialSelectors[0].gameObject.SetActive(true);
		this.MaterialSelectorTemplate.SetActive(false);
		this.ResearchRequired.SetActive(false);
		this.priorityScreen.gameObject.SetActive(false);
		Game.Instance.Subscribe(-107300940, delegate(object d)
		{
			this.RefreshSelectors();
		});
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.activateOnSpawn = true;
	}

	public void AddSelectAction(MaterialSelector.SelectMaterialActions action)
	{
		this.MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.selectMaterialActions = (MaterialSelector.SelectMaterialActions)Delegate.Combine(selector.selectMaterialActions, action);
		});
	}

	public void ClearSelectActions()
	{
		this.MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.selectMaterialActions = null;
		});
	}

	public void ClearMaterialToggles()
	{
		this.MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.ClearMaterialToggles();
		});
	}

	public void ConfigureScreen(Recipe recipe)
	{
		this.activeRecipe = recipe;
		this.RefreshSelectors();
	}

	public bool AllSelectorsSelected()
	{
		foreach (MaterialSelector materialSelector in this.MaterialSelectors)
		{
			if (materialSelector.gameObject.activeInHierarchy && materialSelector.CurrentSelectedElement == null)
			{
				return false;
			}
		}
		return true;
	}

	public void RefreshSelectors()
	{
		if (this.activeRecipe == null)
		{
			return;
		}
		this.MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.gameObject.SetActive(false);
		});
		if (!DebugHandler.InstantBuildMode && this.activeRecipe.GetBuildingDef().RequiredTech != null && !this.activeRecipe.GetBuildingDef().RequiredTech.IsComplete())
		{
			this.ResearchRequired.SetActive(true);
			LocText[] componentsInChildren = this.ResearchRequired.GetComponentsInChildren<LocText>();
			componentsInChildren[0].text = UI.PRODUCTINFO_RESEARCHREQUIRED;
			componentsInChildren[1].text = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, this.activeRecipe.GetBuildingDef().RequiredTech.Name);
			componentsInChildren[1].color = Constants.NEGATIVE_COLOR;
			this.priorityScreen.gameObject.SetActive(false);
		}
		else
		{
			this.ResearchRequired.SetActive(false);
			for (int i = 0; i < this.activeRecipe.Ingredients.Count; i++)
			{
				this.MaterialSelectors[i].gameObject.SetActive(true);
				this.MaterialSelectors[i].ConfigureScreen(this.activeRecipe.Ingredients[i], this.activeRecipe);
			}
			this.priorityScreen.gameObject.SetActive(true);
			this.priorityScreen.gameObject.transform.SetAsLastSibling();
		}
	}

	public void UpdateResourceToggleValues()
	{
		this.MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			if (selector.gameObject.activeSelf)
			{
				selector.RefreshToggleContents();
			}
		});
	}

	public void AutoSelectAvailableMaterial()
	{
		this.MaterialSelectors.ForEach(delegate(MaterialSelector selector)
		{
			selector.AutoSelectAvailableMaterial();
		});
	}

	public bool CanBuild(Recipe recipe)
	{
		foreach (MaterialSelector materialSelector in this.MaterialSelectors)
		{
			if (materialSelector.gameObject.activeSelf && materialSelector.CurrentSelectedElement == null)
			{
				return false;
			}
		}
		return true;
	}

	public static MaterialSelectionPanel.SelectedElemInfo Filter(Tag materialCategoryTag, MaterialSelectionPanel.SelectElement callback)
	{
		MaterialSelectionPanel.SelectedElemInfo selectedElemInfo = default(MaterialSelectionPanel.SelectedElemInfo);
		selectedElemInfo.element = null;
		selectedElemInfo.kgAvailable = 0f;
		foreach (Element element in ElementLoader.elements)
		{
			bool flag = element.tag == materialCategoryTag;
			if (!flag && element.HasTag(materialCategoryTag))
			{
				flag = true;
			}
			if (flag)
			{
				if (!(WorldInventory.Instance == null) && element != null)
				{
					float amount = WorldInventory.Instance.GetAmount(element.tag);
					if (callback != null)
					{
						callback(element, amount);
					}
					if (amount > selectedElemInfo.kgAvailable || element.IsLiquid || selectedElemInfo.element == null)
					{
						selectedElemInfo.kgAvailable = amount;
						selectedElemInfo.element = element;
					}
				}
			}
		}
		return selectedElemInfo;
	}

	public Dictionary<KToggle, Element> ElementToggles = new Dictionary<KToggle, Element>();

	private List<MaterialSelector> MaterialSelectors = new List<MaterialSelector>();

	private List<Element> currentSelectedElements = new List<Element>();

	[SerializeField]
	private BuildMenuPriorityScreen priorityScreen;

	public GameObject MaterialSelectorTemplate;

	public GameObject ResearchRequired;

	private Recipe activeRecipe;

	public struct SelectedElemInfo
	{
		public Element element;

		public float kgAvailable;
	}

	public delegate void SelectElement(Element element, float kgAvailable);
}
