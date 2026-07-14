using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Database;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KleiInventoryScreen : KModalScreen
{
	private PermitResource SelectedPermit { get; set; }

	private string SelectedCategoryId { get; set; }

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
			targetGridLayouts = new List<GridLayoutGroup>()
		};
		this.galleryGridLayouter.overrideParentForSizeReference = this.galleryGridContent;
		InventoryOrganization.Initialize();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (KPrivacyPrefs.instance.disableDataCollection)
		{
			this.barterOfflineLabel.GetComponent<ToolTip>().SetSimpleTooltip(UI.LOCKER_MENU.OFFLINE_ICON_TOOLTIP_DATA_COLLECTIONS);
		}
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
			this.InitConfig();
			this.ToggleDoublesOnly(0);
			this.ClearSearch();
			return;
		}
		this.dlcFilter.ResetToDefault();
		this.dlcFilter.HideDropdown();
	}

	private void ToggleDoublesOnly(int newState)
	{
		this.showFilterState = newState;
		this.doublesOnlyToggle.ChangeState(this.showFilterState);
		this.doublesOnlyToggle.GetComponentInChildren<LocText>().text = this.showFilterState.ToString() + "+";
		string text = "";
		switch (this.showFilterState)
		{
		case 0:
			text = UI.KLEI_INVENTORY_SCREEN.TOOLTIP_VIEW_ALL_ITEMS;
			break;
		case 1:
			text = UI.KLEI_INVENTORY_SCREEN.TOOLTIP_VIEW_OWNED_ONLY;
			break;
		case 2:
			text = UI.KLEI_INVENTORY_SCREEN.TOOLTIP_VIEW_DOUBLES_ONLY;
			break;
		}
		ToolTip component = this.doublesOnlyToggle.GetComponent<ToolTip>();
		component.SetSimpleTooltip(text);
		component.refreshWhileHovering = true;
		component.forceRefresh = true;
		this.RefreshGallery();
	}

	private void InitConfig()
	{
		if (this.initConfigComplete)
		{
			return;
		}
		this.initConfigComplete = true;
		this.galleryGridLayouter.RequestGridResize();
		this.categoryListContent.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
		this.dlcFilter.ConfigButtons();
		this.dlcFilter.onDLCFilterChanged = new global::System.Action(this.RefreshGallery);
		this.PopulateCategories();
		this.PopulateGallery();
		this.SelectCategory("BUILDINGS");
		this.searchField.onValueChanged.RemoveAllListeners();
		this.searchField.onValueChanged.AddListener(delegate(string value)
		{
			this.RefreshGallery();
		});
		this.clearSearchButton.ClearOnClick();
		this.clearSearchButton.onClick += this.ClearSearch;
		MultiToggle multiToggle = this.doublesOnlyToggle;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			int num = (this.showFilterState + 1) % 3;
			this.ToggleDoublesOnly(num);
		}));
	}

	private void RegisterPreventScreenPop()
	{
		this.UnregisterPreventScreenPop();
		this.preventScreenPopFn = delegate
		{
			if (this.dlcFilter.IsDropdownVisible())
			{
				this.RegisterPreventScreenPop();
				this.dlcFilter.ResetToDefault();
				this.dlcFilter.HideDropdown();
				return true;
			}
			return false;
		};
		LockerNavigator.Instance.preventScreenPop.Add(this.preventScreenPopFn);
	}

	private void UnregisterPreventScreenPop()
	{
		if (this.preventScreenPopFn != null)
		{
			LockerNavigator.Instance.preventScreenPop.Remove(this.preventScreenPopFn);
			this.preventScreenPopFn = null;
		}
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		this.dlcFilter.ResetToDefault();
		this.ToggleDoublesOnly(0);
		this.ClearSearch();
		if (!this.initConfigComplete)
		{
			this.InitConfig();
		}
		this.RefreshUI();
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.RefreshUI();
		});
		this.RegisterPreventScreenPop();
	}

	private void ClearSearch()
	{
		this.searchField.text = "";
		this.searchField.placeholder.GetComponent<TextMeshProUGUI>().text = UI.KLEI_INVENTORY_SCREEN.SEARCH_PLACEHOLDER;
		this.RefreshGallery();
	}

	private void Update()
	{
		this.galleryGridLayouter.CheckIfShouldResizeGrid();
	}

	private void RefreshUI()
	{
		this.IS_ONLINE = ThreadedHttps<KleiAccount>.Instance.HasValidTicket();
		this.RefreshCategories();
		this.RefreshGallery();
		if (this.SelectedCategoryId.IsNullOrWhiteSpace())
		{
			this.SelectCategory("BUILDINGS");
		}
		this.RefreshDetails();
		this.RefreshBarterPanel();
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
		foreach (KeyValuePair<string, MultiToggle> keyValuePair in this.categoryToggles)
		{
			global::UnityEngine.Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.categoryToggles.Clear();
		foreach (KeyValuePair<string, List<string>> keyValuePair2 in InventoryOrganization.categoryIdToSubcategoryIdsMap)
		{
			string text;
			List<string> list;
			keyValuePair2.Deconstruct(out text, out list);
			string categoryId = text;
			GameObject gameObject = Util.KInstantiateUI(this.categoryRowPrefab, this.categoryListContent.gameObject, true);
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("Label").SetText(InventoryOrganization.GetCategoryName(categoryId));
			component.GetReference<Image>("Icon").sprite = InventoryOrganization.categoryIdToIconMap[categoryId];
			MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
			MultiToggle multiToggle = component2;
			multiToggle.onEnter = (global::System.Action)Delegate.Combine(multiToggle.onEnter, new global::System.Action(this.OnMouseOverToggle));
			component2.onClick = delegate
			{
				this.SelectCategory(categoryId);
			};
			this.categoryToggles.Add(categoryId, component2);
			this.SetCatogoryClickUISound(categoryId, component2);
		}
	}

	public void PopulateGallery()
	{
		foreach (KeyValuePair<PermitResource, MultiToggle> keyValuePair in this.galleryGridButtons)
		{
			this.RecycleGalleryGridButton(keyValuePair.Value.gameObject);
		}
		this.galleryGridButtons.Clear();
		this.galleryGridLayouter.ImmediateSizeGridToScreenResolution();
		HashSet<string> hashSet = new HashSet<string>();
		foreach (KeyValuePair<string, List<string>> keyValuePair2 in InventoryOrganization.subcategoryIdToPermitIdsMap)
		{
			foreach (string text in keyValuePair2.Value)
			{
				PermitResource permitResource = Db.Get().Permits.TryGet(text);
				if (permitResource != null)
				{
					this.AddItemToGallery(permitResource);
					hashSet.Add(text);
				}
			}
		}
		this.subcategories.Sort((KleiInventoryUISubcategory a, KleiInventoryUISubcategory b) => InventoryOrganization.subcategoryIdToPresentationDataMap[a.subcategoryID].sortKey.CompareTo(InventoryOrganization.subcategoryIdToPresentationDataMap[b.subcategoryID].sortKey));
		foreach (KleiInventoryUISubcategory kleiInventoryUISubcategory in this.subcategories)
		{
			kleiInventoryUISubcategory.gameObject.transform.SetAsLastSibling();
		}
		this.CollectSubcategoryGridLayouts();
		this.CloseSubcategory("UNCATEGORIZED");
	}

	private void CloseSubcategory(string subcategoryID)
	{
		KleiInventoryUISubcategory kleiInventoryUISubcategory = this.subcategories.Find((KleiInventoryUISubcategory match) => match.subcategoryID == subcategoryID);
		if (kleiInventoryUISubcategory != null)
		{
			kleiInventoryUISubcategory.ToggleOpen(false);
		}
	}

	private void AddItemToSubcategoryUIContainer(GameObject itemButton, string subcategoryId)
	{
		KleiInventoryUISubcategory kleiInventoryUISubcategory = this.subcategories.Find((KleiInventoryUISubcategory match) => match.subcategoryID == subcategoryId);
		if (kleiInventoryUISubcategory == null)
		{
			kleiInventoryUISubcategory = Util.KInstantiateUI(this.subcategoryPrefab, this.galleryGridContent.gameObject, true).GetComponent<KleiInventoryUISubcategory>();
			kleiInventoryUISubcategory.subcategoryID = subcategoryId;
			this.subcategories.Add(kleiInventoryUISubcategory);
			kleiInventoryUISubcategory.SetIdentity(InventoryOrganization.GetSubcategoryName(subcategoryId), InventoryOrganization.subcategoryIdToPresentationDataMap[subcategoryId].icon);
		}
		itemButton.transform.SetParent(kleiInventoryUISubcategory.gridLayout.transform);
	}

	private void CollectSubcategoryGridLayouts()
	{
		this.galleryGridLayouter.OnSizeGridComplete = null;
		foreach (KleiInventoryUISubcategory kleiInventoryUISubcategory in this.subcategories)
		{
			this.galleryGridLayouter.targetGridLayouts.Add(kleiInventoryUISubcategory.gridLayout);
			GridLayouter gridLayouter = this.galleryGridLayouter;
			gridLayouter.OnSizeGridComplete = (global::System.Action)Delegate.Combine(gridLayouter.OnSizeGridComplete, new global::System.Action(kleiInventoryUISubcategory.RefreshDisplay));
		}
		this.galleryGridLayouter.RequestGridResize();
	}

	private void AddItemToGallery(PermitResource permit)
	{
		if (this.galleryGridButtons.ContainsKey(permit))
		{
			return;
		}
		PermitPresentationInfo permitPresentationInfo = permit.GetPermitPresentationInfo();
		GameObject availableGridButton = this.GetAvailableGridButton();
		this.AddItemToSubcategoryUIContainer(availableGridButton, InventoryOrganization.GetPermitSubcategory(permit));
		HierarchyReferences component = availableGridButton.GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("Icon");
		LocText reference2 = component.GetReference<LocText>("OwnedCountLabel");
		Image reference3 = component.GetReference<Image>("IsUnownedOverlay");
		Image reference4 = component.GetReference<Image>("DlcBanner");
		MultiToggle component2 = availableGridButton.GetComponent<MultiToggle>();
		reference.sprite = permitPresentationInfo.sprite;
		if (permit.IsOwnableOnServer())
		{
			int ownedCount = PermitItems.GetOwnedCount(permit);
			reference2.text = UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", ownedCount.ToString());
			reference2.gameObject.SetActive(ownedCount > 0);
			reference3.gameObject.SetActive(ownedCount <= 0);
		}
		else
		{
			reference2.gameObject.SetActive(false);
			reference3.gameObject.SetActive(false);
		}
		string dlcIdFrom = permit.GetDlcIdFrom();
		if (DlcManager.IsDlcId(dlcIdFrom))
		{
			reference4.gameObject.SetActive(true);
			reference4.sprite = Assets.GetSprite(DlcManager.GetDlcBannerSprite(dlcIdFrom));
			reference4.color = DlcManager.GetDlcBannerColor(dlcIdFrom);
		}
		else
		{
			reference4.gameObject.SetActive(false);
		}
		MultiToggle multiToggle = component2;
		multiToggle.onEnter = (global::System.Action)Delegate.Combine(multiToggle.onEnter, new global::System.Action(this.OnMouseOverToggle));
		component2.onClick = delegate
		{
			this.SelectItem(permit);
		};
		this.galleryGridButtons.Add(permit, component2);
		this.SetItemClickUISound(permit, component2);
		KleiItemsUI.ConfigureTooltipOn(availableGridButton, KleiItemsUI.GetTooltipStringFor(permit));
	}

	public void SelectCategory(string categoryId)
	{
		if (InventoryOrganization.categoryIdToIsEmptyMap[categoryId])
		{
			return;
		}
		this.SelectedCategoryId = categoryId;
		this.galleryHeaderLabel.SetText(InventoryOrganization.GetCategoryName(categoryId));
		this.RefreshCategories();
		this.SelectDefaultCategoryItem();
	}

	private void SelectDefaultCategoryItem()
	{
		foreach (KeyValuePair<PermitResource, MultiToggle> keyValuePair in this.galleryGridButtons)
		{
			if (InventoryOrganization.categoryIdToSubcategoryIdsMap[this.SelectedCategoryId].Contains(InventoryOrganization.GetPermitSubcategory(keyValuePair.Key)))
			{
				this.SelectItem(keyValuePair.Key);
				return;
			}
		}
		this.SelectItem(null);
	}

	public void SelectItem(PermitResource permit)
	{
		this.SelectedPermit = permit;
		this.RefreshGallery();
		this.RefreshDetails();
		this.RefreshBarterPanel();
	}

	private void RefreshGallery()
	{
		string text = this.searchField.text.ToUpper();
		foreach (KeyValuePair<PermitResource, MultiToggle> keyValuePair in this.galleryGridButtons)
		{
			PermitResource permitResource;
			MultiToggle multiToggle;
			keyValuePair.Deconstruct(out permitResource, out multiToggle);
			PermitResource permitResource2 = permitResource;
			MultiToggle multiToggle2 = multiToggle;
			string permitSubcategory = InventoryOrganization.GetPermitSubcategory(permitResource2);
			bool flag = permitSubcategory == "UNCATEGORIZED" || InventoryOrganization.categoryIdToSubcategoryIdsMap[this.SelectedCategoryId].Contains(permitSubcategory);
			flag = flag && (permitResource2.Name.ToUpper().Contains(text) || permitResource2.Id.ToUpper().Contains(text) || permitResource2.Description.ToUpper().Contains(text));
			multiToggle2.ChangeState((permitResource2 == this.SelectedPermit) ? 1 : 0);
			HierarchyReferences component = multiToggle2.gameObject.GetComponent<HierarchyReferences>();
			LocText reference = component.GetReference<LocText>("OwnedCountLabel");
			Image reference2 = component.GetReference<Image>("IsUnownedOverlay");
			if (permitResource2.IsOwnableOnServer())
			{
				int ownedCount = PermitItems.GetOwnedCount(permitResource2);
				reference.text = UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", ownedCount.ToString());
				reference.gameObject.SetActive(ownedCount > 0);
				reference2.gameObject.SetActive(ownedCount <= 0);
				if (this.showFilterState == 2 && ownedCount < 2)
				{
					flag = false;
				}
				else if (this.showFilterState == 1 && ownedCount == 0)
				{
					flag = false;
				}
			}
			else if (!permitResource2.IsUnlocked())
			{
				reference.gameObject.SetActive(false);
				reference2.gameObject.SetActive(true);
				if (this.showFilterState != 0)
				{
					flag = false;
				}
			}
			else
			{
				reference.gameObject.SetActive(false);
				reference2.gameObject.SetActive(false);
				if (this.showFilterState == 2)
				{
					flag = false;
				}
			}
			if (this.dlcFilter.SelectedDLCID != null && permitResource2.GetDlcIdFrom() != this.dlcFilter.SelectedDLCID)
			{
				flag = false;
			}
			if (multiToggle2.gameObject.activeSelf != flag)
			{
				multiToggle2.gameObject.SetActive(flag);
			}
		}
		foreach (KleiInventoryUISubcategory kleiInventoryUISubcategory in this.subcategories)
		{
			kleiInventoryUISubcategory.RefreshDisplay();
		}
	}

	private void RefreshCategories()
	{
		foreach (KeyValuePair<string, MultiToggle> keyValuePair in this.categoryToggles)
		{
			keyValuePair.Value.ChangeState((keyValuePair.Key == this.SelectedCategoryId) ? 1 : 0);
			if (InventoryOrganization.categoryIdToIsEmptyMap[keyValuePair.Key])
			{
				keyValuePair.Value.ChangeState(2);
			}
			else
			{
				keyValuePair.Value.ChangeState((keyValuePair.Key == this.SelectedCategoryId) ? 1 : 0);
			}
		}
	}

	private void RefreshDetails()
	{
		PermitResource selectedPermit = this.SelectedPermit;
		PermitPresentationInfo permitPresentationInfo = selectedPermit.GetPermitPresentationInfo();
		this.permitVis.ConfigureWith(selectedPermit);
		this.selectionDetailsScrollRect.rectTransform().anchorMin = new Vector2(0f, 0f);
		this.selectionDetailsScrollRect.rectTransform().anchorMax = new Vector2(1f, 1f);
		this.selectionDetailsScrollRect.rectTransform().sizeDelta = new Vector2(-24f, 0f);
		this.selectionDetailsScrollRect.rectTransform().anchoredPosition = Vector2.zero;
		this.selectionDetailsScrollRect.content.rectTransform().sizeDelta = new Vector2(0f, this.selectionDetailsScrollRect.content.rectTransform().sizeDelta.y);
		this.selectionDetailsScrollRectScrollBarContainer.anchorMin = new Vector2(1f, 0f);
		this.selectionDetailsScrollRectScrollBarContainer.anchorMax = new Vector2(1f, 1f);
		this.selectionDetailsScrollRectScrollBarContainer.sizeDelta = new Vector2(24f, 0f);
		this.selectionDetailsScrollRectScrollBarContainer.anchoredPosition = Vector2.zero;
		this.selectionHeaderLabel.SetText(selectedPermit.Name);
		this.selectionNameLabel.SetText(selectedPermit.Name);
		this.selectionDescriptionLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(selectedPermit.Description));
		this.selectionDescriptionLabel.SetText(selectedPermit.Description);
		this.selectionFacadeForLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(permitPresentationInfo.facadeFor));
		this.selectionFacadeForLabel.SetText(permitPresentationInfo.facadeFor);
		string dlcIdFrom = selectedPermit.GetDlcIdFrom();
		if (DlcManager.IsDlcId(dlcIdFrom))
		{
			this.selectionRarityDetailsLabel.gameObject.SetActive(false);
			this.selectionOwnedCount.gameObject.SetActive(false);
			this.selectionCollectionLabel.gameObject.SetActive(true);
			if (selectedPermit.Rarity == PermitRarity.UniversalLocked)
			{
				DlcManager.DlcInfo dlcInfo;
				if (DlcManager.DLC_PACKS.TryGetValue(dlcIdFrom, out dlcInfo) && dlcInfo.isCosmetic)
				{
					this.selectionCollectionLabel.SetText(UI.KLEI_INVENTORY_SCREEN.COLLECTION_COMING_SOON_THE.Replace("{Collection}", DlcManager.GetDlcTitle(dlcIdFrom)));
					return;
				}
				this.selectionCollectionLabel.SetText(UI.KLEI_INVENTORY_SCREEN.COLLECTION_COMING_SOON.Replace("{Collection}", DlcManager.GetDlcTitle(dlcIdFrom)));
				return;
			}
			else
			{
				DlcManager.DlcInfo dlcInfo2;
				if (DlcManager.DLC_PACKS.TryGetValue(dlcIdFrom, out dlcInfo2) && dlcInfo2.isCosmetic)
				{
					this.selectionCollectionLabel.SetText(UI.KLEI_INVENTORY_SCREEN.COLLECTION_THE.Replace("{Collection}", DlcManager.GetDlcTitle(dlcIdFrom)));
					return;
				}
				this.selectionCollectionLabel.SetText(UI.KLEI_INVENTORY_SCREEN.COLLECTION.Replace("{Collection}", DlcManager.GetDlcTitle(dlcIdFrom)));
				return;
			}
		}
		else
		{
			this.selectionCollectionLabel.gameObject.SetActive(false);
			string text = UI.KLEI_INVENTORY_SCREEN.ITEM_RARITY_DETAILS.Replace("{RarityName}", selectedPermit.Rarity.GetLocStringName());
			this.selectionRarityDetailsLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(text));
			this.selectionRarityDetailsLabel.SetText(text);
			this.selectionOwnedCount.gameObject.SetActive(true);
			if (!selectedPermit.IsOwnableOnServer())
			{
				this.selectionOwnedCount.SetText(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_UNLOCKED_BUT_UNOWNABLE);
				return;
			}
			int ownedCount = PermitItems.GetOwnedCount(selectedPermit);
			if (ownedCount > 0)
			{
				this.selectionOwnedCount.SetText(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT.Replace("{OwnedCount}", ownedCount.ToString()));
				return;
			}
			this.selectionOwnedCount.SetText(KleiItemsUI.WrapWithColor(UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWN_NONE, KleiItemsUI.TEXT_COLOR__PERMIT_NOT_OWNED));
			return;
		}
	}

	private KleiInventoryScreen.PermitPrintabilityState GetPermitPrintabilityState(PermitResource permit)
	{
		if (!this.IS_ONLINE)
		{
			return KleiInventoryScreen.PermitPrintabilityState.UserOffline;
		}
		ulong num;
		ulong num2;
		PermitItems.TryGetBarterPrice(this.SelectedPermit.Id, out num, out num2);
		if (num == 0UL)
		{
			if (permit.Rarity == PermitRarity.Universal || permit.Rarity == PermitRarity.UniversalLocked || permit.Rarity == PermitRarity.Loyalty || permit.Rarity == PermitRarity.Unknown)
			{
				return KleiInventoryScreen.PermitPrintabilityState.NotForSale;
			}
			return KleiInventoryScreen.PermitPrintabilityState.NotForSaleYet;
		}
		else
		{
			if (PermitItems.GetOwnedCount(permit) > 0)
			{
				return KleiInventoryScreen.PermitPrintabilityState.AlreadyOwned;
			}
			if (KleiItems.GetFilamentAmount() < num)
			{
				return KleiInventoryScreen.PermitPrintabilityState.TooExpensive;
			}
			return KleiInventoryScreen.PermitPrintabilityState.Printable;
		}
	}

	private void RefreshBarterPanel()
	{
		this.barterBuyButton.ClearOnClick();
		this.barterSellButton.ClearOnClick();
		this.barterBuyButton.isInteractable = this.IS_ONLINE;
		this.barterSellButton.isInteractable = this.IS_ONLINE;
		HierarchyReferences component = this.barterBuyButton.GetComponent<HierarchyReferences>();
		HierarchyReferences component2 = this.barterSellButton.GetComponent<HierarchyReferences>();
		new Color(1f, 0.69411767f, 0.69411767f);
		Color color = new Color(0.6f, 0.9529412f, 0.5019608f);
		LocText reference = component.GetReference<LocText>("CostLabel");
		LocText reference2 = component2.GetReference<LocText>("CostLabel");
		this.barterPanelBG.color = (this.IS_ONLINE ? Util.ColorFromHex("575D6F") : Util.ColorFromHex("6F6F6F"));
		this.filamentWalletSection.gameObject.SetActive(this.IS_ONLINE);
		this.barterOfflineLabel.gameObject.SetActive(!this.IS_ONLINE);
		ulong filamentAmount = KleiItems.GetFilamentAmount();
		this.filamentWalletSection.GetComponent<ToolTip>().SetSimpleTooltip((filamentAmount > 1UL) ? string.Format(UI.KLEI_INVENTORY_SCREEN.BARTERING.WALLET_PLURAL_TOOLTIP, filamentAmount) : string.Format(UI.KLEI_INVENTORY_SCREEN.BARTERING.WALLET_TOOLTIP, filamentAmount));
		KleiInventoryScreen.PermitPrintabilityState permitPrintabilityState = this.GetPermitPrintabilityState(this.SelectedPermit);
		if (!this.IS_ONLINE)
		{
			component.GetReference<LocText>("CostLabel").SetText("");
			reference2.SetText("");
			reference2.color = Color.white;
			this.barterBuyButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_ACTION_INVALID_OFFLINE);
			this.barterSellButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_ACTION_INVALID_OFFLINE);
			return;
		}
		ulong num;
		ulong num2;
		PermitItems.TryGetBarterPrice(this.SelectedPermit.Id, out num, out num2);
		this.filamentWalletSection.GetComponentInChildren<LocText>().SetText(KleiItems.GetFilamentAmount().ToString());
		switch (permitPrintabilityState)
		{
		case KleiInventoryScreen.PermitPrintabilityState.Printable:
			this.barterBuyButton.isInteractable = true;
			this.barterBuyButton.GetComponent<ToolTip>().SetSimpleTooltip(string.Format(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_BUY_ACTIVE, num.ToString()));
			reference.SetText("-" + num.ToString());
			this.barterBuyButton.onClick += delegate
			{
				GameObject gameObject = Util.KInstantiateUI(this.barterConfirmationScreenPrefab, LockerNavigator.Instance.gameObject, false);
				gameObject.rectTransform().sizeDelta = Vector2.zero;
				gameObject.GetComponent<BarterConfirmationScreen>().Present(this.SelectedPermit, true);
			};
			break;
		case KleiInventoryScreen.PermitPrintabilityState.AlreadyOwned:
			this.barterBuyButton.isInteractable = false;
			this.barterBuyButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_UNBUYABLE_ALREADY_OWNED);
			reference.SetText("-" + num.ToString());
			break;
		case KleiInventoryScreen.PermitPrintabilityState.TooExpensive:
			this.barterBuyButton.isInteractable = false;
			this.barterBuyButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_BUY_CANT_AFFORD.text);
			reference.SetText("-" + num.ToString());
			break;
		case KleiInventoryScreen.PermitPrintabilityState.NotForSale:
			this.barterBuyButton.isInteractable = false;
			this.barterBuyButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_UNBUYABLE);
			reference.SetText("");
			break;
		case KleiInventoryScreen.PermitPrintabilityState.NotForSaleYet:
			this.barterBuyButton.isInteractable = false;
			this.barterBuyButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_UNBUYABLE_BETA);
			reference.SetText("");
			break;
		}
		if (num2 == 0UL)
		{
			this.barterSellButton.isInteractable = false;
			this.barterSellButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_UNSELLABLE);
			reference2.SetText("");
			reference2.color = Color.white;
			return;
		}
		bool flag = PermitItems.GetOwnedCount(this.SelectedPermit) > 0;
		this.barterSellButton.isInteractable = flag;
		this.barterSellButton.GetComponent<ToolTip>().SetSimpleTooltip(flag ? string.Format(UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_SELL_ACTIVE, num2.ToString()) : UI.KLEI_INVENTORY_SCREEN.BARTERING.TOOLTIP_NONE_TO_SELL.text);
		if (flag)
		{
			reference2.color = color;
			reference2.SetText("+" + num2.ToString());
		}
		else
		{
			reference2.color = Color.white;
			reference2.SetText("+" + num2.ToString());
		}
		this.barterSellButton.onClick += delegate
		{
			GameObject gameObject2 = Util.KInstantiateUI(this.barterConfirmationScreenPrefab, LockerNavigator.Instance.gameObject, false);
			gameObject2.rectTransform().sizeDelta = Vector2.zero;
			gameObject2.GetComponent<BarterConfirmationScreen>().Present(this.SelectedPermit, false);
		};
	}

	private void SetCatogoryClickUISound(string categoryID, MultiToggle toggle)
	{
		if (!this.categoryToggles.ContainsKey(categoryID))
		{
			toggle.states[1].on_click_override_sound_path = "";
			toggle.states[0].on_click_override_sound_path = "";
			return;
		}
		toggle.states[1].on_click_override_sound_path = "General_Category_Click";
		toggle.states[0].on_click_override_sound_path = "General_Category_Click";
	}

	private void SetItemClickUISound(PermitResource permit, MultiToggle toggle)
	{
		string facadeItemSoundName = KleiInventoryScreen.GetFacadeItemSoundName(permit);
		toggle.states[1].on_click_override_sound_path = facadeItemSoundName + "_Click";
		toggle.states[1].sound_parameter_name = "Unlocked";
		toggle.states[1].sound_parameter_value = (permit.IsUnlocked() ? 1f : 0f);
		toggle.states[1].has_sound_parameter = true;
		toggle.states[0].on_click_override_sound_path = facadeItemSoundName + "_Click";
		toggle.states[0].sound_parameter_name = "Unlocked";
		toggle.states[0].sound_parameter_value = (permit.IsUnlocked() ? 1f : 0f);
		toggle.states[0].has_sound_parameter = true;
	}

	public static string GetFacadeItemSoundName(PermitResource permit)
	{
		if (permit == null)
		{
			return "HUD";
		}
		switch (permit.Category)
		{
		case PermitCategory.DupeTops:
			return "tops";
		case PermitCategory.DupeBottoms:
			return "bottoms";
		case PermitCategory.DupeGloves:
			return "gloves";
		case PermitCategory.DupeShoes:
			return "shoes";
		case PermitCategory.DupeHats:
			return "hats";
		case PermitCategory.AtmoSuitHelmet:
			return "atmosuit_helmet";
		case PermitCategory.AtmoSuitBody:
			return "tops";
		case PermitCategory.AtmoSuitGloves:
			return "gloves";
		case PermitCategory.AtmoSuitBelt:
			return "belt";
		case PermitCategory.AtmoSuitShoes:
			return "shoes";
		}
		if (permit.Category == PermitCategory.Building)
		{
			BuildingDef buildingDef = KleiPermitVisUtil.GetBuildingDef(permit);
			if (buildingDef == null)
			{
				return "HUD";
			}
			string text = buildingDef.PrefabID;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 1943253450U)
			{
				if (num <= 1036100273U)
				{
					if (num <= 296872528U)
					{
						if (num <= 112031228U)
						{
							if (num != 6945394U)
							{
								if (num != 38823703U)
								{
									if (num != 112031228U)
									{
										goto IL_0912;
									}
									if (!(text == "LogicGateDemultiplexer"))
									{
										goto IL_0912;
									}
									goto IL_08D6;
								}
								else
								{
									if (!(text == "LogicGateXOR"))
									{
										goto IL_0912;
									}
									goto IL_08D6;
								}
							}
							else
							{
								if (!(text == "GlassCeilingLight"))
								{
									goto IL_0912;
								}
								goto IL_0882;
							}
						}
						else if (num != 228062815U)
						{
							if (num != 228549509U)
							{
								if (num != 296872528U)
								{
									goto IL_0912;
								}
								if (!(text == "ItemPedestal"))
								{
									goto IL_0912;
								}
							}
							else
							{
								if (!(text == "WashSink"))
								{
									goto IL_0912;
								}
								return "sink";
							}
						}
						else
						{
							if (!(text == "LuxuryBed"))
							{
								goto IL_0912;
							}
							string id = permit.Id;
							if (id == "LuxuryBed_boat")
							{
								return "elegantbed_boat";
							}
							if (!(id == "LuxuryBed_bouncy"))
							{
								return "elegantbed";
							}
							return "elegantbed_bouncy";
						}
					}
					else if (num <= 585850236U)
					{
						if (num != 297556592U)
						{
							if (num != 301047391U)
							{
								if (num != 585850236U)
								{
									goto IL_0912;
								}
								if (!(text == "GravitasPedestal"))
								{
									goto IL_0912;
								}
							}
							else
							{
								if (!(text == "WireRefined"))
								{
									goto IL_0912;
								}
								goto IL_08BE;
							}
						}
						else
						{
							if (!(text == "LogicRibbonBridge"))
							{
								goto IL_0912;
							}
							goto IL_08D6;
						}
					}
					else if (num <= 674245745U)
					{
						if (num != 595816591U)
						{
							if (num != 674245745U)
							{
								goto IL_0912;
							}
							if (!(text == "CraftingTable"))
							{
								goto IL_0912;
							}
							return "craftingstation";
						}
						else
						{
							if (!(text == "FlowerVase"))
							{
								goto IL_0912;
							}
							goto IL_083E;
						}
					}
					else if (num != 781890915U)
					{
						if (num != 1036100273U)
						{
							goto IL_0912;
						}
						if (!(text == "WireRefinedBridgeHighWattage"))
						{
							goto IL_0912;
						}
						goto IL_08BE;
					}
					else
					{
						if (!(text == "LogicGateNOT"))
						{
							goto IL_0912;
						}
						goto IL_08D6;
					}
					return "sculpture";
				}
				if (num <= 1526604543U)
				{
					if (num <= 1232204109U)
					{
						if (num != 1038415088U)
						{
							if (num != 1089791339U)
							{
								if (num != 1232204109U)
								{
									goto IL_0912;
								}
								if (!(text == "WireBridge"))
								{
									goto IL_0912;
								}
								goto IL_08BE;
							}
							else
							{
								if (!(text == "Refrigerator"))
								{
									goto IL_0912;
								}
								return "refrigerator";
							}
						}
						else
						{
							if (!(text == "LogicGateFILTER"))
							{
								goto IL_0912;
							}
							goto IL_08D6;
						}
					}
					else if (num != 1269853127U)
					{
						if (num != 1398532937U)
						{
							if (num != 1526604543U)
							{
								goto IL_0912;
							}
							if (!(text == "StorageLockerSmart"))
							{
								goto IL_0912;
							}
							return "storagelockersmart";
						}
						else
						{
							if (!(text == "LogicGateMultiplexer"))
							{
								goto IL_0912;
							}
							goto IL_08D6;
						}
					}
					else
					{
						if (!(text == "AdvancedResearchCenter"))
						{
							goto IL_0912;
						}
						return "advancedresearchcenter";
					}
				}
				else if (num <= 1734850496U)
				{
					if (num != 1607642960U)
					{
						if (num != 1633134164U)
						{
							if (num != 1734850496U)
							{
								goto IL_0912;
							}
							if (!(text == "RockCrusher"))
							{
								goto IL_0912;
							}
							return "rockrefinery";
						}
						else
						{
							if (!(text == "CeilingLight"))
							{
								goto IL_0912;
							}
							goto IL_0882;
						}
					}
					else
					{
						if (!(text == "FlushToilet"))
						{
							goto IL_0912;
						}
						return "flushtoilate";
					}
				}
				else if (num <= 1908704479U)
				{
					if (num != 1815117387U)
					{
						if (num != 1908704479U)
						{
							goto IL_0912;
						}
						if (!(text == "LogicGateAND"))
						{
							goto IL_0912;
						}
						goto IL_08D6;
					}
					else
					{
						if (!(text == "LogicGateOR"))
						{
							goto IL_0912;
						}
						goto IL_08D6;
					}
				}
				else if (num != 1938276536U)
				{
					if (num != 1943253450U)
					{
						goto IL_0912;
					}
					if (!(text == "WaterCooler"))
					{
						goto IL_0912;
					}
					return "watercooler";
				}
				else
				{
					if (!(text == "Wire"))
					{
						goto IL_0912;
					}
					goto IL_08BE;
				}
			}
			else if (num <= 3132083755U)
			{
				if (num <= 2691468069U)
				{
					if (num <= 2076384603U)
					{
						if (num != 2028863301U)
						{
							if (num != 2041738741U)
							{
								if (num != 2076384603U)
								{
									goto IL_0912;
								}
								if (!(text == "GasReservoir"))
								{
									goto IL_0912;
								}
								return "gasstorage";
							}
							else
							{
								if (!(text == "CookingStation"))
								{
									goto IL_0912;
								}
								return "grill";
							}
						}
						else if (!(text == "FlowerVaseHanging"))
						{
							goto IL_0912;
						}
					}
					else if (num != 2402859370U)
					{
						if (num != 2406622476U)
						{
							if (num != 2691468069U)
							{
								goto IL_0912;
							}
							if (!(text == "ResearchCenter"))
							{
								goto IL_0912;
							}
							return "researchcenter";
						}
						else
						{
							if (!(text == "WireBridgeHighWattage"))
							{
								goto IL_0912;
							}
							goto IL_08BE;
						}
					}
					else
					{
						if (!(text == "StorageLocker"))
						{
							goto IL_0912;
						}
						return "storagelocker";
					}
				}
				else if (num <= 2818521706U)
				{
					if (num != 2701698824U)
					{
						if (num != 2722382738U)
						{
							if (num != 2818521706U)
							{
								goto IL_0912;
							}
							if (!(text == "GourmetCookingStation"))
							{
								goto IL_0912;
							}
							return "gasrange";
						}
						else
						{
							if (!(text == "PlanterBox"))
							{
								goto IL_0912;
							}
							return "planterbox";
						}
					}
					else
					{
						if (!(text == "ManualGenerator"))
						{
							goto IL_0912;
						}
						return "manualgenerator";
					}
				}
				else if (num <= 3048425356U)
				{
					if (num != 2899744071U)
					{
						if (num != 3048425356U)
						{
							goto IL_0912;
						}
						if (!(text == "Bed"))
						{
							goto IL_0912;
						}
						return "bed";
					}
					else
					{
						if (!(text == "ExteriorWall"))
						{
							goto IL_0912;
						}
						return "wall";
					}
				}
				else if (num != 3080524513U)
				{
					if (num != 3132083755U)
					{
						goto IL_0912;
					}
					if (!(text == "FlowerVaseWall"))
					{
						goto IL_0912;
					}
				}
				else
				{
					if (!(text == "MilkPress"))
					{
						goto IL_0912;
					}
					return "pulverizer";
				}
			}
			else if (num <= 3562718686U)
			{
				if (num <= 3371266309U)
				{
					if (num != 3228988836U)
					{
						if (num != 3347778080U)
						{
							if (num != 3371266309U)
							{
								goto IL_0912;
							}
							if (!(text == "LogicRibbon"))
							{
								goto IL_0912;
							}
							goto IL_08D6;
						}
						else
						{
							if (!(text == "LogicGateBUFFER"))
							{
								goto IL_0912;
							}
							goto IL_08D6;
						}
					}
					else
					{
						if (!(text == "LogicWire"))
						{
							goto IL_0912;
						}
						goto IL_08D6;
					}
				}
				else if (num != 3422134480U)
				{
					if (num != 3534553076U)
					{
						if (num != 3562718686U)
						{
							goto IL_0912;
						}
						if (!(text == "Headquarters"))
						{
							goto IL_0912;
						}
						return "headquarters";
					}
					else
					{
						if (!(text == "MassageTable"))
						{
							goto IL_0912;
						}
						return "massagetable";
					}
				}
				else
				{
					if (!(text == "MicrobeMusher"))
					{
						goto IL_0912;
					}
					return "microbemusher";
				}
			}
			else if (num <= 3873680366U)
			{
				if (num != 3681463987U)
				{
					if (num != 3716494409U)
					{
						if (num != 3873680366U)
						{
							goto IL_0912;
						}
						if (!(text == "WireRefinedBridge"))
						{
							goto IL_0912;
						}
						goto IL_08BE;
					}
					else
					{
						if (!(text == "HighWattageWire"))
						{
							goto IL_0912;
						}
						goto IL_08BE;
					}
				}
				else
				{
					if (!(text == "FloorLamp"))
					{
						goto IL_0912;
					}
					goto IL_0882;
				}
			}
			else if (num <= 3958671086U)
			{
				if (num != 3903452895U)
				{
					if (num != 3958671086U)
					{
						goto IL_0912;
					}
					if (!(text == "FlowerVaseHangingFancy"))
					{
						goto IL_0912;
					}
				}
				else
				{
					if (!(text == "EggCracker"))
					{
						goto IL_0912;
					}
					return "eggcracker";
				}
			}
			else if (num != 4217645425U)
			{
				if (num != 4243975822U)
				{
					goto IL_0912;
				}
				if (!(text == "WireRefinedHighWattage"))
				{
					goto IL_0912;
				}
				goto IL_08BE;
			}
			else
			{
				if (!(text == "LogicWireBridge"))
				{
					goto IL_0912;
				}
				goto IL_08D6;
			}
			IL_083E:
			return "flowervase";
			IL_0882:
			return "ceilingLight";
			IL_08BE:
			return "wire";
			IL_08D6:
			return "logicwire";
		}
		IL_0912:
		if (permit.Category == PermitCategory.Artwork)
		{
			BuildingDef buildingDef2 = KleiPermitVisUtil.GetBuildingDef(permit);
			if (buildingDef2 == null)
			{
				return "HUD";
			}
			if (KleiInventoryScreen.<GetFacadeItemSoundName>g__Has|81_0<Sculpture>(buildingDef2))
			{
				string text = buildingDef2.PrefabID;
				if (text == "IceSculpture")
				{
					return "icesculpture";
				}
				if (!(text == "WoodSculpture"))
				{
					return "sculpture";
				}
				return "woodsculpture";
			}
			else
			{
				if (KleiInventoryScreen.<GetFacadeItemSoundName>g__Has|81_0<Painting>(buildingDef2))
				{
					return "painting";
				}
				if (KleiInventoryScreen.<GetFacadeItemSoundName>g__Has|81_0<MonumentPart>(buildingDef2))
				{
					return "monument";
				}
			}
		}
		if (permit.Category == PermitCategory.JoyResponse && permit is BalloonArtistFacadeResource)
		{
			return "balloon";
		}
		return "HUD";
	}

	private void OnMouseOverToggle()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover", false));
	}

	[CompilerGenerated]
	internal static bool <GetFacadeItemSoundName>g__Has|81_0<T>(BuildingDef buildingDef) where T : Component
	{
		return !buildingDef.BuildingComplete.GetComponent<T>().IsNullOrDestroyed();
	}

	[Header("Header")]
	[SerializeField]
	private KButton closeButton;

	[Header("CategoryColumn")]
	[SerializeField]
	private RectTransform categoryListContent;

	[SerializeField]
	private GameObject categoryRowPrefab;

	private Dictionary<string, MultiToggle> categoryToggles = new Dictionary<string, MultiToggle>();

	[Header("ItemGalleryColumn")]
	[SerializeField]
	private LocText galleryHeaderLabel;

	[SerializeField]
	private RectTransform galleryGridContent;

	[SerializeField]
	private GameObject gridItemPrefab;

	[SerializeField]
	private GameObject subcategoryPrefab;

	[SerializeField]
	private GameObject itemDummyPrefab;

	[Header("GalleryFilters")]
	[SerializeField]
	private KInputTextField searchField;

	[SerializeField]
	private KButton clearSearchButton;

	[SerializeField]
	private MultiToggle doublesOnlyToggle;

	[SerializeField]
	private KleiInventoryDLCFilter dlcFilter;

	public const int FILTER_SHOW_ALL = 0;

	public const int FILTER_SHOW_OWNED_ONLY = 1;

	public const int FILTER_SHOW_DOUBLES_ONLY = 2;

	private int showFilterState;

	private Func<bool> preventScreenPopFn;

	[Header("BarterSection")]
	[SerializeField]
	private Image barterPanelBG;

	[SerializeField]
	private KButton barterBuyButton;

	[SerializeField]
	private KButton barterSellButton;

	[SerializeField]
	private GameObject barterConfirmationScreenPrefab;

	[SerializeField]
	private GameObject filamentWalletSection;

	[SerializeField]
	private GameObject barterOfflineLabel;

	private Dictionary<PermitResource, MultiToggle> galleryGridButtons = new Dictionary<PermitResource, MultiToggle>();

	private List<KleiInventoryUISubcategory> subcategories = new List<KleiInventoryUISubcategory>();

	private List<GameObject> recycledGalleryGridButtons = new List<GameObject>();

	private GridLayouter galleryGridLayouter;

	[Header("SelectionDetailsColumn")]
	[SerializeField]
	private LocText selectionHeaderLabel;

	[SerializeField]
	private KleiPermitDioramaVis permitVis;

	[SerializeField]
	private KScrollRect selectionDetailsScrollRect;

	[SerializeField]
	private RectTransform selectionDetailsScrollRectScrollBarContainer;

	[SerializeField]
	private LocText selectionNameLabel;

	[SerializeField]
	private LocText selectionDescriptionLabel;

	[SerializeField]
	private LocText selectionFacadeForLabel;

	[SerializeField]
	private LocText selectionCollectionLabel;

	[SerializeField]
	private LocText selectionRarityDetailsLabel;

	[SerializeField]
	private LocText selectionOwnedCount;

	private bool IS_ONLINE;

	private bool initConfigComplete;

	private enum PermitPrintabilityState
	{
		Printable,
		AlreadyOwned,
		TooExpensive,
		NotForSale,
		NotForSaleYet,
		UserOffline
	}

	private enum MultiToggleState
	{
		Default,
		Selected,
		NonInteractable
	}
}
