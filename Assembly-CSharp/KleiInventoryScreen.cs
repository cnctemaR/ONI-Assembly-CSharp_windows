using System;
using System.Collections.Generic;
using Database;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class KleiInventoryScreen : KModalScreen
{
	private string SelectedItemFacadeID { get; set; }

	private PermitCategory SelectedCategory { get; set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.closeButton.onClick += delegate
		{
			this.Show(false);
		};
		base.ConsumeMouseScroll = true;
		this.galleryGridLayouter = new GridLayouter
		{
			minCellSize = 64f,
			maxCellSize = 96f,
			targetGridLayout = this.galleryGridContent.GetComponent<GridLayoutGroup>()
		};
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Show(false);
		}
		base.OnKeyDown(e);
	}

	public override float GetSortKey()
	{
		return 20f;
	}

	protected override void OnActivate()
	{
		this.OnShow(true);
	}

	protected override void OnShow(bool show)
	{
		base.OnShow(show);
		if (show)
		{
			this.galleryGridLayouter.RequestGridResize();
			this.PopulateCategories();
			this.PopulateGallery();
			this.SelectCategory(PermitCategory.Building);
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.RefreshCategories();
			this.RefreshGallery();
			this.RefreshDetails();
		});
		KleiItemsStatusRefresher.RequestRefreshFromServer();
	}

	private void Update()
	{
		this.galleryGridLayouter.CheckIfShouldResizeGrid();
	}

	private GameObject GetAvailableGridButton()
	{
		if (this.recycledGalleryGridButtons.Count == 0)
		{
			return Util.KInstantiateUI(this.gridItemPrefab, this.galleryGridContent.gameObject, true);
		}
		GameObject gameObject = this.recycledGalleryGridButtons[0];
		this.recycledGalleryGridButtons.RemoveAt(0);
		return gameObject;
	}

	private void RecycleGalleryGridButton(GameObject button)
	{
		button.GetComponent<MultiToggle>().onClick = null;
		this.recycledGalleryGridButtons.Add(button);
	}

	public void PopulateCategories()
	{
		foreach (KeyValuePair<PermitCategory, MultiToggle> keyValuePair in this.categoryToggles)
		{
			global::UnityEngine.Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.categoryToggles.Clear();
		this.emptyCategories.Clear();
		this.AddPermitCategory(PermitCategory.Building);
		this.AddPermitCategory(PermitCategory.Artwork);
		this.AddPermitCategory(PermitCategory.DupeTops);
		this.AddPermitCategory(PermitCategory.DupeBottoms);
		this.AddPermitCategory(PermitCategory.DupeGloves);
		this.AddPermitCategory(PermitCategory.DupeShoes);
	}

	private void AddPermitCategory(PermitCategory permitCategory)
	{
		GameObject gameObject = Util.KInstantiateUI(this.categoryRowPrefab, this.categoryListContent.gameObject, true);
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<LocText>("Label").SetText(PermitCategories.GetUppercaseDisplayName(permitCategory));
		component.GetReference<Image>("Icon").sprite = Assets.GetSprite(PermitCategories.GetIconName(permitCategory));
		MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
		component2.onClick = delegate
		{
			this.SelectCategory(permitCategory);
		};
		this.categoryToggles.Add(permitCategory, component2);
		this.emptyCategories.Add(permitCategory, true);
	}

	public void PopulateGallery()
	{
		foreach (KeyValuePair<string, MultiToggle> keyValuePair in this.galleryGridButtons)
		{
			this.RecycleGalleryGridButton(keyValuePair.Value.gameObject);
		}
		this.galleryGridButtons.Clear();
		this.galleryGridLayouter.ImmediateSizeGridToScreenResolution();
		foreach (PermitResource permitResource in Db.Get().Permits.resources)
		{
			if (PermitResources.ShouldDisplayPermitInSupplyCloset(permitResource.Id))
			{
				this.AddItemToGallery(permitResource.Id);
			}
		}
	}

	private void AddItemToGallery(string facadeID)
	{
		if (this.galleryGridButtons.ContainsKey(facadeID))
		{
			return;
		}
		PermitPresentationInfo permitPresentationInfo = PermitItems.GetPermitPresentationInfo(facadeID);
		this.emptyCategories[permitPresentationInfo.category] = false;
		GameObject availableGridButton = this.GetAvailableGridButton();
		HierarchyReferences component = availableGridButton.GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("Icon");
		LocText reference2 = component.GetReference<LocText>("OwnedCountLabel");
		Image reference3 = component.GetReference<Image>("IsUnownedOverlay");
		MultiToggle component2 = availableGridButton.GetComponent<MultiToggle>();
		reference.sprite = permitPresentationInfo.sprite;
		if (permitPresentationInfo.ownedCount.HasValue)
		{
			reference2.text = UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", permitPresentationInfo.ownedCount.Value.ToString());
			reference2.gameObject.SetActive(permitPresentationInfo.ownedCount.Value > 0);
			reference3.gameObject.SetActive(permitPresentationInfo.ownedCount.Value <= 0);
		}
		else
		{
			reference2.gameObject.SetActive(false);
			reference3.gameObject.SetActive(false);
		}
		MultiToggle multiToggle = component2;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.SelectItem(facadeID);
		}));
		this.galleryGridButtons.Add(facadeID, component2);
		KleiItemsUI.ConfigureTooltipOn(availableGridButton, KleiItemsUI.GetTooltipStringFor(permitPresentationInfo));
	}

	public void SelectCategory(PermitCategory category)
	{
		if (this.emptyCategories[category])
		{
			return;
		}
		this.SelectedCategory = category;
		this.galleryHeaderLabel.SetText(PermitCategories.GetDisplayName(category));
		this.RefreshCategories();
		this.SelectDefaultCategoryItem();
	}

	private void SelectDefaultCategoryItem()
	{
		foreach (KeyValuePair<string, MultiToggle> keyValuePair in this.galleryGridButtons)
		{
			if (PermitItems.GetPermitPresentationInfo(keyValuePair.Key).category == this.SelectedCategory)
			{
				this.SelectItem(keyValuePair.Key);
				return;
			}
		}
		this.SelectItem(null);
	}

	public void SelectItem(string facadeID)
	{
		this.SelectedItemFacadeID = facadeID;
		this.RefreshGallery();
		this.RefreshDetails();
	}

	private void RefreshGallery()
	{
		foreach (KeyValuePair<string, MultiToggle> keyValuePair in this.galleryGridButtons)
		{
			string text;
			MultiToggle multiToggle;
			keyValuePair.Deconstruct<string, MultiToggle>(out text, out multiToggle);
			string text2 = text;
			MultiToggle multiToggle2 = multiToggle;
			PermitPresentationInfo permitPresentationInfo = PermitItems.GetPermitPresentationInfo(text2);
			multiToggle2.gameObject.SetActive(permitPresentationInfo.category == this.SelectedCategory);
			multiToggle2.ChangeState((text2 == this.SelectedItemFacadeID) ? 1 : 0);
			HierarchyReferences component = multiToggle2.gameObject.GetComponent<HierarchyReferences>();
			LocText reference = component.GetReference<LocText>("OwnedCountLabel");
			Image reference2 = component.GetReference<Image>("IsUnownedOverlay");
			if (permitPresentationInfo.ownedCount.HasValue)
			{
				reference.text = UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", permitPresentationInfo.ownedCount.Value.ToString());
				reference.gameObject.SetActive(permitPresentationInfo.ownedCount.Value > 0);
				reference2.gameObject.SetActive(permitPresentationInfo.ownedCount.Value <= 0);
			}
			else
			{
				reference.gameObject.SetActive(false);
				reference2.gameObject.SetActive(false);
			}
		}
	}

	private void RefreshCategories()
	{
		foreach (KeyValuePair<PermitCategory, MultiToggle> keyValuePair in this.categoryToggles)
		{
			PermitCategory key = keyValuePair.Key;
			if (this.emptyCategories[key])
			{
				keyValuePair.Value.ChangeState(2);
			}
			else
			{
				keyValuePair.Value.ChangeState((key == this.SelectedCategory) ? 1 : 0);
			}
		}
	}

	private void RefreshDetails()
	{
		PermitResource permitResource = Db.Get().Permits.TryGet(this.SelectedItemFacadeID);
		PermitPresentationInfo permitPresentationInfo = PermitItems.GetPermitPresentationInfo(this.SelectedItemFacadeID);
		this.permitVis.ConfigureWith(permitResource, permitPresentationInfo);
		this.selectionHeaderLabel.SetText(permitPresentationInfo.name);
		this.selectionNameLabel.SetText(permitPresentationInfo.name);
		this.selectionDescriptionLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(permitPresentationInfo.description));
		this.selectionDescriptionLabel.SetText(permitPresentationInfo.description);
		this.selectionFacadeForLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(permitPresentationInfo.facadeFor));
		this.selectionFacadeForLabel.SetText(permitPresentationInfo.facadeFor);
		this.selectionRarityDetailsLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(permitPresentationInfo.rarityDetails));
		this.selectionRarityDetailsLabel.SetText(permitPresentationInfo.rarityDetails);
		this.selectionOwnedCount.gameObject.SetActive(!permitPresentationInfo.isNone);
		if (!permitPresentationInfo.ownedCount.HasValue)
		{
			this.selectionOwnedCount.SetText(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_UNLOCKED_BUT_UNOWNABLE);
			return;
		}
		if (permitPresentationInfo.ownedCount.Value > 0)
		{
			this.selectionOwnedCount.SetText(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT.Replace("{OwnedCount}", permitPresentationInfo.ownedCount.Value.ToString()));
			return;
		}
		this.selectionOwnedCount.SetText(KleiItemsUI.WrapWithColor(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED));
	}

	[Header("Header")]
	[SerializeField]
	private KButton closeButton;

	[Header("CategoryColumn")]
	[SerializeField]
	private RectTransform categoryListContent;

	[SerializeField]
	private GameObject categoryRowPrefab;

	private Dictionary<PermitCategory, MultiToggle> categoryToggles = new Dictionary<PermitCategory, MultiToggle>();

	private Dictionary<PermitCategory, bool> emptyCategories = new Dictionary<PermitCategory, bool>();

	[Header("ItemGalleryColumn")]
	[SerializeField]
	private LocText galleryHeaderLabel;

	[SerializeField]
	private RectTransform galleryGridContent;

	[SerializeField]
	private GameObject gridItemPrefab;

	private Dictionary<string, MultiToggle> galleryGridButtons = new Dictionary<string, MultiToggle>();

	private List<GameObject> recycledGalleryGridButtons = new List<GameObject>();

	private GridLayouter galleryGridLayouter;

	[Header("SelectionDetailsColumn")]
	[SerializeField]
	private LocText selectionHeaderLabel;

	[SerializeField]
	private KleiPermitDioramaVis permitVis;

	[SerializeField]
	private LocText selectionNameLabel;

	[SerializeField]
	private LocText selectionDescriptionLabel;

	[SerializeField]
	private LocText selectionFacadeForLabel;

	[SerializeField]
	private LocText selectionRarityDetailsLabel;

	[SerializeField]
	private LocText selectionOwnedCount;
}
