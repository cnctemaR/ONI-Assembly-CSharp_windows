using System;
using System.Collections.Generic;
using Klei.AI;
using STRINGS;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProductInfoScreen : KScreen
{
	public void ForceClose(bool playSound = true)
	{
		this.ClearProduct(true);
		PlanScreen.Instance.CloseRecipe(playSound);
	}

	private void RefreshScreen()
	{
		if (this.currentDef != null)
		{
			this.SetTitle(this.currentDef);
		}
		else
		{
			this.ClearProduct(true);
		}
	}

	public void ClearProduct(bool deactivateTool = true)
	{
		this.currentDef = null;
		this.materialSelectionPanel.ClearMaterialToggles();
		if (PlayerController.Instance.ActiveTool == BuildTool.Instance && deactivateTool)
		{
			BuildTool.Instance.Deactivate();
		}
		if (PlayerController.Instance.ActiveTool == UtilityBuildTool.Instance || PlayerController.Instance.ActiveTool == WireBuildTool.Instance)
		{
			ToolMenu.Instance.ClearSelection();
		}
		this.ClearLabels();
		base.Show(false);
	}

	public new void Awake()
	{
		base.Awake();
		this.materialSelectionPanel = Util.KInstantiateUI<MaterialSelectionPanel>(this.materialSelectionPanelPrefab.gameObject, base.gameObject, false);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		BuildingGroupScreen instance = BuildingGroupScreen.Instance;
		instance.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Combine(instance.pointerEnterActions, new KScreen.PointerEnterActions(this.CheckMouseOver));
		BuildingGroupScreen instance2 = BuildingGroupScreen.Instance;
		instance2.pointerExitActions = (KScreen.PointerExitActions)Delegate.Combine(instance2.pointerExitActions, new KScreen.PointerExitActions(this.CheckMouseOver));
		PlanScreen instance3 = PlanScreen.Instance;
		instance3.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Combine(instance3.pointerEnterActions, new KScreen.PointerEnterActions(this.CheckMouseOver));
		PlanScreen instance4 = PlanScreen.Instance;
		instance4.pointerExitActions = (KScreen.PointerExitActions)Delegate.Combine(instance4.pointerExitActions, new KScreen.PointerExitActions(this.CheckMouseOver));
		this.pointerEnterActions = (KScreen.PointerEnterActions)Delegate.Combine(this.pointerEnterActions, new KScreen.PointerEnterActions(this.CheckMouseOver));
		this.pointerExitActions = (KScreen.PointerExitActions)Delegate.Combine(this.pointerExitActions, new KScreen.PointerExitActions(this.CheckMouseOver));
	}

	public void ConfigureScreen(BuildingDef def)
	{
		this.currentDef = def;
		this.SetTitle(def);
		this.SetDescription(def);
		this.SetEffects(def);
		this.SetMaterials(def);
	}

	private void ExpandInfo(PointerEventData data)
	{
		this.ToggleExpandedInfo(true);
	}

	private void CollapseInfo(PointerEventData data)
	{
		this.ToggleExpandedInfo(false);
	}

	public void ToggleExpandedInfo(bool state)
	{
		this.expandedInfo = state;
		if (this.ProductDescriptionPane != null)
		{
			this.ProductDescriptionPane.SetActive(this.expandedInfo);
		}
		if (this.ProductRequirementsPane != null)
		{
			this.ProductRequirementsPane.gameObject.SetActive(this.expandedInfo);
		}
		if (this.ProductEffectsPane != null)
		{
			this.ProductEffectsPane.gameObject.SetActive(this.expandedInfo);
		}
		if (this.ProductFlavourPane != null)
		{
			this.ProductFlavourPane.SetActive(this.expandedInfo);
		}
	}

	private void CheckMouseOver(PointerEventData data)
	{
		if (!base.GetMouseOver && !PlanScreen.Instance.GetMouseOver && !BuildingGroupScreen.Instance.GetMouseOver)
		{
			this.ToggleExpandedInfo(false);
		}
		else
		{
			this.ToggleExpandedInfo(true);
		}
	}

	private void Update()
	{
		if (PlayerController.Instance.ActiveTool != PrebuildTool.Instance && PlayerController.Instance.ActiveTool != BuildTool.Instance && PlayerController.Instance.ActiveTool != UtilityBuildTool.Instance && PlayerController.Instance.ActiveTool != WireBuildTool.Instance)
		{
			bool flag = false;
			if (PlayerController.Instance.ActiveTool == SelectTool.Instance)
			{
				flag = true;
			}
			this.ForceClose(flag);
			return;
		}
		if (!DebugHandler.InstantBuildMode && this.currentDef != null && this.materialSelectionPanel.CurrentSelectedElement != null && this.currentDef.Mass[0] > WorldInventory.Instance.GetAmount(this.materialSelectionPanel.CurrentSelectedElement.tag))
		{
			this.materialSelectionPanel.AutoSelectAvailableMaterial();
		}
	}

	private void SetTitle(BuildingDef def)
	{
		this.titleBar.SetTitle(def.Name);
		bool flag = PlanScreen.Instance.BuildableState(this.currentDef) == PlanScreen.RequirementsState.Complete;
		this.titleBar.GetComponentInChildren<KImage>().ColorState = ((!flag) ? KImage.ColorSelector.Disabled : KImage.ColorSelector.Active);
	}

	private void SetDescription(BuildingDef def)
	{
		if (this.productFlavourText != null)
		{
			string text = def.Desc;
			Dictionary<Klei.AI.Attribute, float> dictionary = new Dictionary<Klei.AI.Attribute, float>();
			Dictionary<Klei.AI.Attribute, float> dictionary2 = new Dictionary<Klei.AI.Attribute, float>();
			foreach (Klei.AI.Attribute attribute in def.attributes)
			{
				if (!dictionary.ContainsKey(attribute))
				{
					dictionary[attribute] = 0f;
				}
			}
			foreach (AttributeModifier attributeModifier in def.attributeModifiers)
			{
				float num = 0f;
				Klei.AI.Attribute attribute2 = Db.Get().BuildingAttributes.Get(attributeModifier.AttributeId);
				dictionary.TryGetValue(attribute2, out num);
				num += attributeModifier.Value;
				dictionary[attribute2] = num;
			}
			if (this.materialSelectionPanel.CurrentSelectedElement != null)
			{
				foreach (AttributeModifier attributeModifier2 in this.materialSelectionPanel.CurrentSelectedElement.attributeModifiers)
				{
					float num2 = 0f;
					Klei.AI.Attribute attribute3 = Db.Get().BuildingAttributes.Get(attributeModifier2.AttributeId);
					dictionary2.TryGetValue(attribute3, out num2);
					num2 += attributeModifier2.Value;
					dictionary2[attribute3] = num2;
				}
			}
			if (dictionary.Count > 0)
			{
				text += "\n\n";
				foreach (KeyValuePair<Klei.AI.Attribute, float> keyValuePair in dictionary)
				{
					float num3 = 0f;
					dictionary.TryGetValue(keyValuePair.Key, out num3);
					float num4 = 0f;
					string text2 = string.Empty;
					if (dictionary2.TryGetValue(keyValuePair.Key, out num4))
					{
						num4 = Mathf.Abs(num3 * num4);
						text2 = "(+" + num4 + ")";
					}
					string text3 = text;
					text = string.Concat(new object[]
					{
						text3,
						"\n",
						keyValuePair.Key.Name,
						": ",
						num3 + num4,
						text2
					});
				}
			}
			this.productFlavourText.text = text;
		}
	}

	private void SetEffects(BuildingDef def)
	{
		if (this.productDescriptionText.text != null)
		{
			this.productDescriptionText.text = string.Format("{0}", def.Effect);
		}
		List<Descriptor> allDescriptors = GameUtil.GetAllDescriptors(def);
		List<Descriptor> requirementDescriptors = GameUtil.GetRequirementDescriptors(allDescriptors);
		if (requirementDescriptors.Count > 0)
		{
			Descriptor descriptor = default(Descriptor);
			descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONREQUIREMENTS, UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONREQUIREMENTS, Descriptor.DescriptorType.Effect);
			requirementDescriptors.Insert(0, descriptor);
		}
		this.ProductRequirementsPane.SetDescriptors(requirementDescriptors);
		List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(allDescriptors);
		if (effectDescriptors.Count > 0)
		{
			Descriptor descriptor2 = default(Descriptor);
			descriptor2.SetupDescriptor(UI.BUILDINGEFFECTS.OPERATIONEFFECTS, UI.BUILDINGEFFECTS.TOOLTIPS.OPERATIONEFFECTS, Descriptor.DescriptorType.Effect);
			effectDescriptors.Insert(0, descriptor2);
		}
		this.ProductEffectsPane.SetDescriptors(effectDescriptors);
	}

	public void ClearLabels()
	{
		List<string> list = new List<string>(this.descLabels.Keys);
		if (list.Count > 0)
		{
			foreach (string text in list)
			{
				GameObject gameObject = this.descLabels[text];
				if (gameObject != null)
				{
					global::UnityEngine.Object.Destroy(gameObject);
				}
				this.descLabels.Remove(text);
			}
		}
	}

	public void SetMaterials(BuildingDef def)
	{
		this.materialSelectionPanel.gameObject.SetActive(true);
		Recipe craftRecipe = def.CraftRecipe;
		this.materialSelectionPanel.ClearSelectActions();
		this.materialSelectionPanel.ConfigureScreen(craftRecipe);
		this.materialSelectionPanel.AddSelectAction(new MaterialSelector.SelectMaterialActions(this.RefreshScreen));
		this.materialSelectionPanel.AddSelectAction(new MaterialSelector.SelectMaterialActions(this.onMenuMaterialChanged));
		this.materialSelectionPanel.AutoSelectAvailableMaterial();
	}

	private bool BuildRequirementsMet(BuildingDef def)
	{
		if (DebugHandler.InstantBuildMode)
		{
			return true;
		}
		Recipe craftRecipe = def.CraftRecipe;
		return this.materialSelectionPanel.CanBuild(craftRecipe) && (def.RequiredTech == null || def.RequiredTech.IsComplete());
	}

	private void onMenuMaterialChanged()
	{
		if (this.currentDef == null)
		{
			return;
		}
		if (this.materialSelectionPanel.AllSelectorsSelected() && this.BuildRequirementsMet(this.currentDef))
		{
			this.onElementsFullySelected.Signal();
		}
		else
		{
			BuildTool.Instance.Deactivate();
			PrebuildTool.Instance.Activate(this.currentDef, PlanScreen.Instance.BuildableState(this.currentDef));
		}
		this.SetDescription(this.currentDef);
	}

	public static bool MaterialsMet(Recipe recipe)
	{
		if (recipe == null)
		{
			global::Debug.LogError("Trying to verify the materials on a null recipe!", null);
			return false;
		}
		if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
		{
			global::Debug.LogError("Trying to verify the materials on a recipe with no MaterialCategoryTags!", null);
			return false;
		}
		int i;
		for (i = 0; i < recipe.Ingredients.Count; i++)
		{
			bool available = false;
			MaterialSelectionPanel.Filter(recipe.Ingredients[i].tag, delegate(Element element, float kgAvailable)
			{
				if (kgAvailable >= recipe.Ingredients[i].amount)
				{
					available = true;
				}
			});
			if (!available)
			{
				return false;
			}
		}
		return true;
	}

	public TitleBar titleBar;

	public GameObject ProductDescriptionPane;

	public LocText productDescriptionText;

	public DescriptorPanel ProductRequirementsPane;

	public DescriptorPanel ProductEffectsPane;

	public GameObject ProductFlavourPane;

	public LocText productFlavourText;

	public RectTransform BGPanel;

	public MaterialSelectionPanel materialSelectionPanelPrefab;

	private Dictionary<string, GameObject> descLabels = new Dictionary<string, GameObject>();

	[NonSerialized]
	public MaterialSelectionPanel materialSelectionPanel;

	[NonSerialized]
	public BuildingDef currentDef;

	public global::System.Action onElementsFullySelected;

	private bool expandedInfo = true;
}
