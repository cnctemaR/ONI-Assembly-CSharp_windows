using System;
using System.Collections.Generic;
using System.Linq;
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
		foreach (KeyValuePair<KToggle, Element> keyValuePair in this.ElementToggles)
		{
			keyValuePair.Key.gameObject.SetActive(false);
			Util.KDestroyGameObject(keyValuePair.Key.gameObject);
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
		List<string> list2 = new List<string>();
		foreach (Element element in ElementLoader.elements)
		{
			if (element.IsSolid)
			{
				if (!list.Contains(element))
				{
					if (!list2.Contains(element.tag.ProperName()))
					{
						if (element.tag != ingredient.tag)
						{
							bool flag = false;
							if (element.HasTag(ingredient.tag))
							{
								flag = true;
							}
							if (!flag)
							{
								continue;
							}
						}
						list.Add(element);
						list2.Add(element.tag.ProperName());
					}
				}
			}
		}
		foreach (Element element2 in list)
		{
			if (!this.ElementToggles.ContainsValue(element2))
			{
				GameObject gameObject = Util.KInstantiate(this.TogglePrefab, this.LayoutContainer, "MaterialSelection_" + element2.name);
				gameObject.transform.localScale = Vector3.one;
				gameObject.SetActive(true);
				KToggle component = gameObject.GetComponent<KToggle>();
				this.ElementToggles.Add(component, element2);
				component.group = this.toggleGroup;
				ToolTip component2 = gameObject.gameObject.GetComponent<ToolTip>();
				component2.toolTip = element2.name;
			}
		}
		this.RefreshToggleContents();
	}

	private void SetToggleBGImage(KToggle toggle)
	{
		if (toggle == this.selectedToggle)
		{
			toggle.GetComponentsInChildren<Image>()[1].material = GlobalResources.Instance().AnimUIMaterial;
			toggle.GetComponent<ImageToggleState>().SetActive();
		}
		else if (WorldInventory.Instance.GetAmount(this.ElementToggles[toggle].tag) >= this.activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
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

	public void OnSelectMaterial(KToggle toggle, Recipe recipe)
	{
		if (toggle != this.selectedToggle)
		{
			this.selectedToggle = toggle;
			Element element = this.ElementToggles[toggle];
			if (recipe != null)
			{
				this.previouslySelectedElements[recipe] = element;
			}
			this.CurrentSelectedElement = this.ElementToggles[toggle];
			if (this.selectMaterialActions != null)
			{
				this.selectMaterialActions();
			}
			this.UpdateHeader();
			this.SetDescription(element);
			this.SetEffects(element);
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
		foreach (KeyValuePair<KToggle, Element> keyValuePair in this.ElementToggles)
		{
			KToggle toggle = keyValuePair.Key;
			GameObject gameObject = toggle.gameObject;
			LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>();
			LocText locText = componentsInChildren[0];
			LocText locText2 = componentsInChildren[1];
			Image image = gameObject.GetComponentsInChildren<Image>()[1];
			locText2.text = Util.FormatWholeNumber(WorldInventory.Instance.GetAmount(keyValuePair.Value.tag));
			locText.text = Util.FormatWholeNumber(this.activeMass);
			image.sprite = Def.GetUISpriteFromMultiObjectAnim(keyValuePair.Value.substance.anim, "ui", false);
			gameObject.SetActive(WorldInventory.Instance.IsDiscovered(keyValuePair.Value.tag) || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive);
			this.SetToggleBGImage(keyValuePair.Key);
			Tag tag = keyValuePair.Value.tag;
			toggle.soundPlayer.AcceptClickCondition = () => this.IsEnoughMass(tag);
			toggle.ClearOnClick();
			if (this.IsEnoughMass(keyValuePair.Value.tag))
			{
				toggle.onClick += delegate
				{
					this.OnSelectMaterial(toggle, this.activeRecipe);
				};
			}
		}
		this.SortElementToggles();
		this.UpdateHeader();
	}

	private bool IsEnoughMass(Tag t)
	{
		return WorldInventory.Instance.GetAmount(t) >= this.activeMass || DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive || MaterialSelector.AllowInsufficientMaterialBuild();
	}

	public bool AutoSelectAvailableMaterial()
	{
		if (this.activeRecipe != null && this.previouslySelectedElements.ContainsKey(this.activeRecipe))
		{
			foreach (KeyValuePair<KToggle, Element> keyValuePair in this.ElementToggles)
			{
				if (keyValuePair.Value == this.previouslySelectedElements[this.activeRecipe] && WorldInventory.Instance.GetAmount(keyValuePair.Value.tag) >= this.activeMass)
				{
					this.OnSelectMaterial(keyValuePair.Key, this.activeRecipe);
					return true;
				}
			}
		}
		float num = -1f;
		KToggle ktoggle = null;
		foreach (KeyValuePair<KToggle, Element> keyValuePair2 in this.ElementToggles)
		{
			float amount = WorldInventory.Instance.GetAmount(keyValuePair2.Value.tag);
			if (DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive)
			{
				this.OnSelectMaterial(keyValuePair2.Key, this.activeRecipe);
				return true;
			}
			if (amount >= this.activeMass && amount > num)
			{
				num = amount;
				ktoggle = keyValuePair2.Key;
			}
		}
		if (ktoggle != null)
		{
			this.OnSelectMaterial(ktoggle, this.activeRecipe);
			return true;
		}
		return false;
	}

	private void SortElementToggles()
	{
		List<KToggle> list = new List<KToggle>();
		List<Element> list2 = new List<Element>();
		foreach (KeyValuePair<KToggle, Element> keyValuePair in this.ElementToggles)
		{
			list.Add(keyValuePair.Key);
			list2.Add(keyValuePair.Value);
		}
		list2 = list2.OrderByDescending<Element, bool>((Element e) => WorldInventory.Instance.IsDiscovered(e.tag)).ToList<Element>();
		foreach (KeyValuePair<KToggle, Element> keyValuePair2 in this.ElementToggles)
		{
			ToolTip component = keyValuePair2.Key.gameObject.GetComponent<ToolTip>();
			component.toolTip = GameUtil.GetMaterialTooltips(keyValuePair2.Value);
		}
		this.UpdateScrollBar();
	}

	private void UpdateScrollBar()
	{
		int num = 0;
		foreach (KeyValuePair<KToggle, Element> keyValuePair in this.ElementToggles)
		{
			if (keyValuePair.Key.gameObject.activeSelf)
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
		foreach (KeyValuePair<KToggle, Element> keyValuePair in this.ElementToggles)
		{
			KToggle key = keyValuePair.Key;
			if (key.gameObject.activeSelf)
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

	public Element CurrentSelectedElement;

	public Dictionary<KToggle, Element> ElementToggles = new Dictionary<KToggle, Element>();

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
