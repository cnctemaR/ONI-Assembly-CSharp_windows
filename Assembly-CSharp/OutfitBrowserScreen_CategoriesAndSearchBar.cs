using System;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class OutfitBrowserScreen_CategoriesAndSearchBar
{
	public void InitializeWith(OutfitBrowserScreen outfitBrowserScreen)
	{
		this.outfitBrowserScreen = outfitBrowserScreen;
		this.clothingOutfitTypeButton = new OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButton(outfitBrowserScreen, Util.KInstantiateUI(this.selectOutfitType_Prefab.gameObject, this.categoryRow, true));
		this.clothingOutfitTypeButton.button.onClick += delegate
		{
			this.SetOutfitType(ClothingOutfitUtility.OutfitType.Clothing);
		};
		this.clothingOutfitTypeButton.icon.sprite = Assets.GetSprite("icon_inventory_equipment");
		KleiItemsUI.ConfigureTooltipOn(this.clothingOutfitTypeButton.button.gameObject, UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_FILTER_BY_CLOTHING);
		this.atmosuitOutfitTypeButton = new OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButton(outfitBrowserScreen, Util.KInstantiateUI(this.selectOutfitType_Prefab.gameObject, this.categoryRow, true));
		this.atmosuitOutfitTypeButton.button.onClick += delegate
		{
			this.SetOutfitType(ClothingOutfitUtility.OutfitType.AtmoSuit);
		};
		this.atmosuitOutfitTypeButton.icon.sprite = Assets.GetSprite("icon_inventory_atmosuits");
		KleiItemsUI.ConfigureTooltipOn(this.atmosuitOutfitTypeButton.button.gameObject, UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_FILTER_BY_ATMO_SUITS);
		this.jetsuitOutfitTypeButton = new OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButton(outfitBrowserScreen, Util.KInstantiateUI(this.selectOutfitType_Prefab.gameObject, this.categoryRow, true));
		this.jetsuitOutfitTypeButton.button.onClick += delegate
		{
			this.SetOutfitType(ClothingOutfitUtility.OutfitType.JetSuit);
		};
		this.jetsuitOutfitTypeButton.icon.sprite = Assets.GetSprite("icon_inventory_jetsuits");
		KleiItemsUI.ConfigureTooltipOn(this.jetsuitOutfitTypeButton.button.gameObject, UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_FILTER_BY_JET_SUITS);
		this.searchTextField.onValueChanged.AddListener(delegate(string newFilter)
		{
			outfitBrowserScreen.state.Filter = newFilter;
		});
		this.searchTextField.transform.SetAsLastSibling();
		outfitBrowserScreen.state.OnCurrentOutfitTypeChanged += delegate
		{
			if (outfitBrowserScreen.Config.onlyShowOutfitType.IsSome())
			{
				this.clothingOutfitTypeButton.root.gameObject.SetActive(false);
				this.atmosuitOutfitTypeButton.root.gameObject.SetActive(false);
				this.jetsuitOutfitTypeButton.root.gameObject.SetActive(false);
				return;
			}
			this.clothingOutfitTypeButton.root.gameObject.SetActive(true);
			this.atmosuitOutfitTypeButton.root.gameObject.SetActive(true);
			this.jetsuitOutfitTypeButton.root.gameObject.SetActive(true);
			this.clothingOutfitTypeButton.SetState(OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Unselected);
			this.atmosuitOutfitTypeButton.SetState(OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Unselected);
			this.jetsuitOutfitTypeButton.SetState(OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Unselected);
			switch (outfitBrowserScreen.state.CurrentOutfitType)
			{
			case ClothingOutfitUtility.OutfitType.Clothing:
				this.clothingOutfitTypeButton.SetState(OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Selected);
				return;
			case ClothingOutfitUtility.OutfitType.AtmoSuit:
				this.atmosuitOutfitTypeButton.SetState(OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Selected);
				return;
			case ClothingOutfitUtility.OutfitType.JetSuit:
				this.jetsuitOutfitTypeButton.SetState(OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Selected);
				return;
			}
			throw new NotImplementedException();
		};
	}

	public void SetOutfitType(ClothingOutfitUtility.OutfitType outfitType)
	{
		this.outfitBrowserScreen.state.CurrentOutfitType = outfitType;
	}

	[NonSerialized]
	public OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButton clothingOutfitTypeButton;

	[NonSerialized]
	public OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButton atmosuitOutfitTypeButton;

	[NonSerialized]
	public OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButton jetsuitOutfitTypeButton;

	[NonSerialized]
	public OutfitBrowserScreen outfitBrowserScreen;

	public KButton selectOutfitType_Prefab;

	public KInputTextField searchTextField;

	public GameObject categoryRow;

	public enum SelectOutfitTypeButtonState
	{
		Disabled,
		Unselected,
		Selected
	}

	public readonly struct SelectOutfitTypeButton
	{
		public SelectOutfitTypeButton(OutfitBrowserScreen outfitBrowserScreen, GameObject rootGameObject)
		{
			this.outfitBrowserScreen = outfitBrowserScreen;
			this.root = rootGameObject.GetComponent<RectTransform>();
			this.button = rootGameObject.GetComponent<KButton>();
			this.buttonImage = rootGameObject.GetComponent<KImage>();
			this.icon = this.root.GetChild(0).GetComponent<Image>();
			global::Debug.Assert(this.root != null);
			global::Debug.Assert(this.button != null);
			global::Debug.Assert(this.buttonImage != null);
			global::Debug.Assert(this.icon != null);
		}

		public void SetState(OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState state)
		{
			switch (state)
			{
			case OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Disabled:
				this.button.isInteractable = false;
				this.buttonImage.colorStyleSetting = this.outfitBrowserScreen.notSelectedCategoryStyle;
				this.buttonImage.ApplyColorStyleSetting();
				return;
			case OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Unselected:
				this.button.isInteractable = true;
				this.buttonImage.colorStyleSetting = this.outfitBrowserScreen.notSelectedCategoryStyle;
				this.buttonImage.ApplyColorStyleSetting();
				return;
			case OutfitBrowserScreen_CategoriesAndSearchBar.SelectOutfitTypeButtonState.Selected:
				this.button.isInteractable = true;
				this.buttonImage.colorStyleSetting = this.outfitBrowserScreen.selectedCategoryStyle;
				this.buttonImage.ApplyColorStyleSetting();
				return;
			default:
				throw new NotImplementedException();
			}
		}

		public readonly OutfitBrowserScreen outfitBrowserScreen;

		public readonly RectTransform root;

		public readonly KButton button;

		public readonly KImage buttonImage;

		public readonly Image icon;
	}
}
