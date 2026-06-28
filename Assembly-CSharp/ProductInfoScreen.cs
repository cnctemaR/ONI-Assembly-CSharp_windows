using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProductInfoScreen : KScreen
{
	public void ForceClose()
	{
		this.ClearProduct(true);
		PlanScreen.Instance.CloseRecipe();
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
			this.ProductRequirementsPane.SetActive(this.expandedInfo);
		}
		if (this.ProductEffectsPane != null)
		{
			this.ProductEffectsPane.SetActive(this.expandedInfo);
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
			if (this.materialSelectionPanel.CurrentSelectedElement == null)
			{
				KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Deselect", false));
			}
			this.ForceClose();
			return;
		}
		if (!DebugHandler.InstantBuildMode && this.currentDef != null && this.materialSelectionPanel.CurrentSelectedElement != null && this.currentDef.Mass[0] > WorldInventory.Instance.GetAmount(this.materialSelectionPanel.CurrentSelectedElement.tag))
		{
			this.materialSelectionPanel.AutoSelectAvailableMaterial();
		}
		if (this.textLayoutDirty)
		{
			if (this.productFlavourText != null)
			{
				LayoutElement component = this.productFlavourText.GetComponent<LayoutElement>();
				LayoutElement layoutElement = component;
				float num = this.productFlavourText.preferredHeight;
				component.preferredHeight = num;
				layoutElement.minHeight = num;
			}
			if (this.productDescriptionText != null)
			{
				LayoutElement component2 = this.productDescriptionText.GetComponent<LayoutElement>();
				LayoutElement layoutElement2 = component2;
				float num = this.productDescriptionText.preferredHeight;
				component2.preferredHeight = num;
				layoutElement2.minHeight = num;
			}
			this.textLayoutDirty = false;
		}
	}

	private void SetTitle(BuildingDef def)
	{
		this.titleBar.SetTitle(def.Name);
		bool flag = PlanScreen.Instance.BuildableState(this.currentDef) == PlanScreen.RequirementsState.Complete;
		this.titleBar.GetComponentInChildren<Image>().color = ((!flag) ? this.titleBarColors.inactiveColor : this.titleBarColors.activeColor);
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
			this.textLayoutDirty = true;
		}
	}

	private void SetEffects(BuildingDef def)
	{
		if (this.productDescriptionText.text != null)
		{
			this.productDescriptionText.text = string.Format("{0}", def.Effect);
		}
		List<Descriptor> buildingRequirementDescriptors = GameUtil.GetBuildingRequirementDescriptors(def);
		List<Descriptor> buildingEffectsDescriptors = GameUtil.GetBuildingEffectsDescriptors(def);
		foreach (Descriptor descriptor in buildingRequirementDescriptors)
		{
			this.AddOrGetLabel(this.descLabels, descriptor, this.ProductRequirementsPane, descriptor.text);
		}
		foreach (Descriptor descriptor2 in buildingEffectsDescriptors)
		{
			this.AddOrGetLabel(this.descLabels, descriptor2, this.ProductEffectsPane, descriptor2.text);
		}
		this.textLayoutDirty = true;
	}

	private GameObject AddOrGetLabel(Dictionary<string, GameObject> labels, Descriptor descriptor, GameObject panel, string id)
	{
		GameObject gameObject;
		if (labels.ContainsKey(id))
		{
			gameObject = labels[id];
		}
		else
		{
			gameObject = Util.KInstantiate(ScreenPrefabs.Instance.DescriptionLabel, panel, null);
			gameObject.GetComponent<LocText>().text = descriptor.text;
			gameObject.GetComponent<ToolTip>().toolTip = descriptor.tooltipText;
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			labels[id] = gameObject;
		}
		gameObject.SetActive(true);
		return gameObject;
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
			Debug.LogError("Trying to verify the materials on a null recipe!");
			return false;
		}
		if (recipe.Ingredients == null || recipe.Ingredients.Count == 0)
		{
			Debug.LogError("Trying to verify the materials on a recipe with no MaterialCategoryTags!");
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

	public GameObject ProductRequirementsPane;

	public GameObject ProductEffectsPane;

	public GameObject ProductFlavourPane;

	public LocText productFlavourText;

	public RectTransform BGPanel;

	public MaterialSelectionPanel materialSelectionPanelPrefab;

	private Dictionary<string, GameObject> descLabels = new Dictionary<string, GameObject>();

	public ColorStyleSetting titleBarColors;

	[NonSerialized]
	public MaterialSelectionPanel materialSelectionPanel;

	[NonSerialized]
	public BuildingDef currentDef;

	public global::System.Action onElementsFullySelected;

	private bool expandedInfo = true;

	private bool textLayoutDirty;
}
