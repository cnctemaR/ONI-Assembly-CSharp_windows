using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MaterialSelectionPanel : KScreen
{
	public static void ClearStatics()
	{
		MaterialSelectionPanel.elementsWithTag.Clear();
	}

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

	public PriorityScreen PriorityScreen
	{
		get
		{
			return this.priorityScreen;
		}
	}

	protected override void OnPrefabInit()
	{
		MaterialSelectionPanel.elementsWithTag.Clear();
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
		this.priorityScreen = Util.KInstantiateUI<PriorityScreen>(this.priorityScreenPrefab.gameObject, this.priorityScreenParent, false);
		this.priorityScreen.InstantiateButtons(new Action<PrioritySetting>(this.OnPriorityClicked), true);
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
		TechItem techItem = Db.Get().TechItems.TryGet(this.activeRecipe.GetBuildingDef().PrefabID);
		bool flag = !DebugHandler.InstantBuildMode && !Game.Instance.SandboxModeActive && techItem != null && !techItem.IsComplete();
		if (flag)
		{
			this.ResearchRequired.SetActive(true);
			LocText[] componentsInChildren = this.ResearchRequired.GetComponentsInChildren<LocText>();
			componentsInChildren[0].text = UI.PRODUCTINFO_RESEARCHREQUIRED;
			componentsInChildren[1].text = string.Format(UI.PRODUCTINFO_REQUIRESRESEARCHDESC, techItem.parentTech.Name);
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

	public bool AutoSelectAvailableMaterial()
	{
		bool flag = true;
		for (int i = 0; i < this.MaterialSelectors.Count; i++)
		{
			MaterialSelector materialSelector = this.MaterialSelectors[i];
			if (!materialSelector.AutoSelectAvailableMaterial())
			{
				flag = false;
			}
		}
		return flag;
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

	public static MaterialSelectionPanel.SelectedElemInfo Filter(Tag materialCategoryTag)
	{
		MaterialSelectionPanel.SelectedElemInfo selectedElemInfo = default(MaterialSelectionPanel.SelectedElemInfo);
		selectedElemInfo.element = null;
		selectedElemInfo.kgAvailable = 0f;
		if (WorldInventory.Instance == null || ElementLoader.elements == null || ElementLoader.elements.Count == 0)
		{
			return selectedElemInfo;
		}
		List<Element> list = null;
		if (!MaterialSelectionPanel.elementsWithTag.TryGetValue(materialCategoryTag, out list))
		{
			list = new List<Element>();
			foreach (Element element in ElementLoader.elements)
			{
				if (element.tag == materialCategoryTag || element.HasTag(materialCategoryTag))
				{
					list.Add(element);
				}
			}
			MaterialSelectionPanel.elementsWithTag[materialCategoryTag] = list;
		}
		foreach (Element element2 in list)
		{
			float amount = WorldInventory.Instance.GetAmount(element2.tag);
			if (amount > selectedElemInfo.kgAvailable)
			{
				selectedElemInfo.kgAvailable = amount;
				selectedElemInfo.element = element2;
			}
		}
		return selectedElemInfo;
	}

	public void ToggleShowDescriptorPanels(bool show)
	{
		for (int i = 0; i < this.MaterialSelectors.Count; i++)
		{
			if (this.MaterialSelectors[i] != null)
			{
				this.MaterialSelectors[i].ToggleShowDescriptorsPanel(show);
			}
		}
	}

	private void OnPriorityClicked(PrioritySetting priority)
	{
		this.priorityScreen.SetScreenPriority(priority, false);
	}

	public Dictionary<KToggle, Element> ElementToggles = new Dictionary<KToggle, Element>();

	private List<MaterialSelector> MaterialSelectors = new List<MaterialSelector>();

	private List<Element> currentSelectedElements = new List<Element>();

	[SerializeField]
	protected PriorityScreen priorityScreenPrefab;

	[SerializeField]
	protected GameObject priorityScreenParent;

	private PriorityScreen priorityScreen;

	public GameObject MaterialSelectorTemplate;

	public GameObject ResearchRequired;

	private Recipe activeRecipe;

	private static Dictionary<Tag, List<Element>> elementsWithTag = new Dictionary<Tag, List<Element>>();

	public delegate void SelectElement(Element element, float kgAvailable, float recipe_amount);

	public struct SelectedElemInfo
	{
		public Element element;

		public float kgAvailable;
	}
}
