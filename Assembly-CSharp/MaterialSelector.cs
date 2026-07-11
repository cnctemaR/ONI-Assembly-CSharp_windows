using System;
using System.Collections.Generic;
using Klei;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MaterialSelector : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.toggleGroup = base.GetComponent<ToggleGroup>();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		base.OnKeyDown(e);
	}

	public void ClearMaterialToggles()
	{
		this.CurrentSelectedElement = null;
		this.NoMaterialDiscovered.gameObject.SetActive(false);
		foreach (KeyValuePair<Element, KToggle> keyValuePair in this.ElementToggles)
		{
			keyValuePair.Value.gameObject.SetActive(false);
			Util.KDestroyGameObject(keyValuePair.Value.gameObject);
		}
		this.ElementToggles.Clear();
	}

	public void ConfigureScreen(Recipe.Ingredient ingredient, Recipe recipe)
	{
		this.ClearMaterialToggles();
		this.activeIngredient = ingredient;
		this.activeRecipe = recipe;
		this.activeMass = ingredient.amount;
		List<Element> list = new List<Element>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid && (element.tag == ingredient.tag || element.HasTag(ingredient.tag)))
			{
				list.Add(element);
			}
		}
		foreach (Element element2 in list)
		{
			if (!this.ElementToggles.ContainsKey(element2))
			{
				GameObject gameObject = Util.KInstantiate(this.TogglePrefab, this.LayoutContainer, "MaterialSelection_" + element2.name);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(true);
				KToggle component = gameObject.GetComponent<KToggle>();
				this.ElementToggles.Add(element2, component);
				component.group = this.toggleGroup;
				ToolTip component2 = gameObject.gameObject.GetComponent<ToolTip>();
				component2.toolTip = element2.name;
			}
		}
		this.RefreshToggleContents();
	}

	private void SetToggleBGImage(KToggle toggle, Element elem)
	{
		if (toggle == this.selectedToggle)
		{
			toggle.GetComponentsInChildren<Image>()[1].material = GlobalResources.Instance().AnimUIMaterial;
			toggle.GetComponent<ImageToggleState>().SetActive();
		}
		else if (WorldInventory.Instance.GetAmount(elem.tag) >= this.activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
		{
			toggle.GetComponentsInChildren<Image>()[1].material = GlobalResources.Instance().AnimUIMaterial;
			toggle.GetComponentsInChildren<Image>()[1].color = Color.white;
			toggle.GetComponent<ImageToggleState>().SetInactive();
		}
		else
		{
			toggle.GetComponentsInChildren<Image>()[1].material = GlobalResources.Instance().AnimMaterialUIDesaturated;
			toggle.GetComponentsInChildren<Image>()[1].color = new Color(1f, 1f, 1f, 0.6f);
			if (!MaterialSelector.AllowInsufficientMaterialBuild())
			{
				toggle.GetComponent<ImageToggleState>().SetDisabled();
			}
		}
	}

	public void OnSelectMaterial(Element elem, Recipe recipe)
	{
		KToggle ktoggle = this.ElementToggles[elem];
		if (ktoggle != this.selectedToggle)
		{
			this.selectedToggle = ktoggle;
			if (recipe != null)
			{
				this.previouslySelectedElements[recipe] = elem;
			}
			this.CurrentSelectedElement = elem;
			if (this.selectMaterialActions != null)
			{
				this.selectMaterialActions();
			}
			this.UpdateHeader();
			this.SetDescription(elem);
			this.SetEffects(elem);
			if (!this.MaterialDescriptionPane.gameObject.activeSelf && !this.MaterialEffectsPane.gameObject.activeSelf)
			{
				this.DescriptorsPanel.SetActive(false);
			}
			else
			{
				this.DescriptorsPanel.SetActive(true);
			}
		}
		this.RefreshToggleContents();
	}

	public void RefreshToggleContents()
	{
		foreach (KeyValuePair<Element, KToggle> keyValuePair in this.ElementToggles)
		{
			KToggle value = keyValuePair.Value;
			Element elem = keyValuePair.Key;
			GameObject gameObject = value.gameObject;
			LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>();
			LocText locText = componentsInChildren[0];
			LocText locText2 = componentsInChildren[1];
			Image image = gameObject.GetComponentsInChildren<Image>()[1];
			locText2.text = Util.FormatWholeNumber(WorldInventory.Instance.GetAmount(elem.tag));
			locText.text = Util.FormatWholeNumber(this.activeMass);
			image.sprite = Def.GetUISpriteFromMultiObjectAnim(keyValuePair.Key.substance.anim, "ui", false);
			gameObject.SetActive(WorldInventory.Instance.IsDiscovered(elem.tag) || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive);
			this.SetToggleBGImage(keyValuePair.Value, keyValuePair.Key);
			value.soundPlayer.AcceptClickCondition = () => this.IsEnoughMass(elem.tag);
			value.ClearOnClick();
			if (this.IsEnoughMass(elem.tag))
			{
				value.onClick += delegate
				{
					this.OnSelectMaterial(elem, this.activeRecipe);
				};
			}
		}
		this.SortElementToggles();
		this.UpdateMaterialTooltips();
		this.UpdateHeader();
	}

	private bool IsEnoughMass(Tag t)
	{
		return WorldInventory.Instance.GetAmount(t) >= this.activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || MaterialSelector.AllowInsufficientMaterialBuild();
	}

	public bool AutoSelectAvailableMaterial()
	{
		if (this.activeRecipe == null || this.ElementToggles.Count == 0)
		{
			return false;
		}
		Element element;
		this.previouslySelectedElements.TryGetValue(this.activeRecipe, out element);
		if (element != null)
		{
			KToggle ktoggle;
			this.ElementToggles.TryGetValue(element, out ktoggle);
			if (ktoggle != null && (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || WorldInventory.Instance.GetAmount(element.tag) >= this.activeMass))
			{
				this.OnSelectMaterial(element, this.activeRecipe);
				return true;
			}
		}
		float num = -1f;
		List<Element> list = new List<Element>();
		foreach (KeyValuePair<Element, KToggle> keyValuePair in this.ElementToggles)
		{
			list.Add(keyValuePair.Key);
		}
		list.Sort(new Comparison<Element>(this.ElementSorter));
		if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
		{
			this.OnSelectMaterial(list[0], this.activeRecipe);
			return true;
		}
		Element element2 = null;
		foreach (Element element3 in list)
		{
			float amount = WorldInventory.Instance.GetAmount(element3.tag);
			if (amount >= this.activeMass && amount > num)
			{
				num = amount;
				element2 = element3;
			}
		}
		if (element2 != null)
		{
			this.OnSelectMaterial(element2, this.activeRecipe);
			return true;
		}
		return false;
	}

	private void SortElementToggles()
	{
		List<Element> list = new List<Element>();
		foreach (KeyValuePair<Element, KToggle> keyValuePair in this.ElementToggles)
		{
			list.Add(keyValuePair.Key);
		}
		list.Sort(new Comparison<Element>(this.ElementSorter));
		foreach (Element element in list)
		{
			this.ElementToggles[element].transform.SetAsLastSibling();
		}
		this.UpdateScrollBar();
	}

	private void UpdateMaterialTooltips()
	{
		foreach (KeyValuePair<Element, KToggle> keyValuePair in this.ElementToggles)
		{
			ToolTip component = keyValuePair.Value.gameObject.GetComponent<ToolTip>();
			component.toolTip = GameUtil.GetMaterialTooltips(keyValuePair.Key);
		}
	}

	private void UpdateScrollBar()
	{
		int num = 0;
		foreach (KeyValuePair<Element, KToggle> keyValuePair in this.ElementToggles)
		{
			if (keyValuePair.Value.gameObject.activeSelf)
			{
				num++;
			}
		}
		this.Scrollbar.SetActive(num > 5);
	}

	private void UpdateHeader()
	{
		if (this.activeIngredient == null)
		{
			return;
		}
		int num = 0;
		foreach (KeyValuePair<Element, KToggle> keyValuePair in this.ElementToggles)
		{
			KToggle value = keyValuePair.Value;
			if (value.gameObject.activeSelf)
			{
				num++;
			}
		}
		LocText componentInChildren = this.Headerbar.GetComponentInChildren<LocText>();
		if (num == 0)
		{
			componentInChildren.text = string.Format(UI.PRODUCTINFO_MISSINGRESOURCES_TITLE, this.activeIngredient.tag.ProperName(), GameUtil.GetFormattedMass(this.activeIngredient.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}"));
			string text = string.Format(UI.PRODUCTINFO_MISSINGRESOURCES_DESC, this.activeIngredient.tag.ProperName());
			this.NoMaterialDiscovered.text = text;
			this.NoMaterialDiscovered.gameObject.SetActive(true);
			this.NoMaterialDiscovered.color = Constants.NEGATIVE_COLOR;
			this.BadBG.SetActive(true);
			this.Scrollbar.SetActive(false);
			this.LayoutContainer.SetActive(false);
		}
		else
		{
			componentInChildren.text = string.Format(UI.PRODUCTINFO_SELECTMATERIAL, this.activeIngredient.tag.ProperName());
			this.NoMaterialDiscovered.gameObject.SetActive(false);
			this.BadBG.SetActive(false);
			this.LayoutContainer.SetActive(true);
			this.UpdateScrollBar();
		}
	}

	public void ToggleShowDescriptorsPanel(bool show)
	{
		this.DescriptorsPanel.gameObject.SetActive(show);
	}

	private void SetDescription(Element element)
	{
		StringEntry stringEntry = null;
		if (Strings.TryGet(new StringKey("STRINGS.ELEMENTS." + element.tag.ToString().ToUpper() + ".BUILD_DESC"), out stringEntry))
		{
			this.MaterialDescriptionText.text = stringEntry.ToString();
			this.MaterialDescriptionPane.SetActive(true);
		}
		else
		{
			this.MaterialDescriptionPane.SetActive(false);
		}
	}

	private void SetEffects(Element element)
	{
		List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(element);
		if (materialDescriptors.Count > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(ELEMENTS.MATERIAL_MODIFIERS.EFFECTS_HEADER, ELEMENTS.MATERIAL_MODIFIERS.TOOLTIP.EFFECTS_HEADER, Descriptor.DescriptorType.Effect);
			materialDescriptors.Insert(0, descriptor);
			this.MaterialEffectsPane.gameObject.SetActive(true);
			this.MaterialEffectsPane.SetDescriptors(materialDescriptors);
		}
		else
		{
			this.MaterialEffectsPane.gameObject.SetActive(false);
		}
	}

	public static bool AllowInsufficientMaterialBuild()
	{
		return GenericGameSettings.instance.allowInsufficientMaterialBuild;
	}

	private int ElementSorter(Element a, Element b)
	{
		if (a.buildMenuSort != b.buildMenuSort)
		{
			return a.buildMenuSort.CompareTo(b.buildMenuSort);
		}
		return a.idx.CompareTo(b.idx);
	}

	public Element CurrentSelectedElement;

	public Dictionary<Element, KToggle> ElementToggles = new Dictionary<Element, KToggle>();

	public Dictionary<Recipe, Element> previouslySelectedElements = new Dictionary<Recipe, Element>();

	public MaterialSelector.SelectMaterialActions selectMaterialActions;

	public MaterialSelector.SelectMaterialActions deselectMaterialActions;

	private ToggleGroup toggleGroup;

	public GameObject TogglePrefab;

	public GameObject LayoutContainer;

	public GameObject Scrollbar;

	public GameObject Headerbar;

	public GameObject BadBG;

	public LocText NoMaterialDiscovered;

	public GameObject MaterialDescriptionPane;

	public LocText MaterialDescriptionText;

	public DescriptorPanel MaterialEffectsPane;

	public GameObject DescriptorsPanel;

	private KToggle selectedToggle;

	private Recipe.Ingredient activeIngredient;

	private Recipe activeRecipe;

	private float activeMass;

	public delegate void SelectMaterialActions();
}
