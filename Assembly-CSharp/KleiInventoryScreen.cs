using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Database;
using STRINGS;
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
			this.categoryListContent.GetComponent<RectTransform>().offsetMax = new Vector2(0f, 0f);
			this.PopulateCategories();
			this.PopulateGallery();
			this.SelectCategory("BUILDINGS");
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
		foreach (KeyValuePair<string, MultiToggle> keyValuePair in this.categoryToggles)
		{
			global::UnityEngine.Object.Destroy(keyValuePair.Value.gameObject);
		}
		this.categoryToggles.Clear();
		using (Dictionary<string, List<string>>.Enumerator enumerator2 = InventoryOrganization.categoryIdToSubcategoryIdsMap.GetEnumerator())
		{
			while (enumerator2.MoveNext())
			{
				string text;
				List<string> list;
				enumerator2.Current.Deconstruct<string, List<string>>(out text, out list);
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
	}

	public void PopulateGallery()
	{
		foreach (KeyValuePair<PermitResource, MultiToggle> keyValuePair in this.galleryGridButtons)
		{
			this.RecycleGalleryGridButton(keyValuePair.Value.gameObject);
		}
		this.galleryGridButtons.Clear();
		this.galleryGridLayouter.ImmediateSizeGridToScreenResolution();
		foreach (PermitResource permitResource in Db.Get().Permits.resources)
		{
			if (!permitResource.Id.StartsWith("visonly_"))
			{
				this.AddItemToGallery(permitResource);
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
		MultiToggle component2 = availableGridButton.GetComponent<MultiToggle>();
		reference.sprite = permitPresentationInfo.sprite;
		if (permit.IsOwnable())
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
	}

	private void RefreshGallery()
	{
		foreach (KeyValuePair<PermitResource, MultiToggle> keyValuePair in this.galleryGridButtons)
		{
			PermitResource permitResource;
			MultiToggle multiToggle;
			keyValuePair.Deconstruct<PermitResource, MultiToggle>(out permitResource, out multiToggle);
			PermitResource permitResource2 = permitResource;
			MultiToggle multiToggle2 = multiToggle;
			string permitSubcategory = InventoryOrganization.GetPermitSubcategory(permitResource2);
			bool flag = permitSubcategory == "UNCATEGORIZED" || InventoryOrganization.categoryIdToSubcategoryIdsMap[this.SelectedCategoryId].Contains(permitSubcategory);
			if (multiToggle2.gameObject.activeSelf != flag)
			{
				multiToggle2.gameObject.SetActive(flag);
			}
			multiToggle2.ChangeState((permitResource2 == this.SelectedPermit) ? 1 : 0);
			HierarchyReferences component = multiToggle2.gameObject.GetComponent<HierarchyReferences>();
			LocText reference = component.GetReference<LocText>("OwnedCountLabel");
			Image reference2 = component.GetReference<Image>("IsUnownedOverlay");
			if (permitResource2.IsOwnable())
			{
				int ownedCount = PermitItems.GetOwnedCount(permitResource2);
				reference.text = UI.KLEI_INVENTORY_SCREEN.ITEM_PLAYER_OWNED_AMOUNT_ICON.Replace("{OwnedCount}", ownedCount.ToString());
				reference.gameObject.SetActive(ownedCount > 0);
				reference2.gameObject.SetActive(ownedCount <= 0);
			}
			else
			{
				reference.gameObject.SetActive(false);
				reference2.gameObject.SetActive(false);
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
		string text = UI.KLEI_INVENTORY_SCREEN.ITEM_RARITY_DETAILS.Replace("{RarityName}", selectedPermit.Rarity.GetLocStringName());
		this.selectionRarityDetailsLabel.gameObject.SetActive(!string.IsNullOrWhiteSpace(text));
		this.selectionRarityDetailsLabel.SetText(text);
		this.selectionOwnedCount.gameObject.SetActive(true);
		if (!selectedPermit.IsOwnable())
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
			bool flag;
			BuildingDef buildingDef;
			KleiPermitVisUtil.GetBuildingDef(permit).Deconstruct(out flag, out buildingDef);
			bool flag2 = flag;
			BuildingDef buildingDef2 = buildingDef;
			if (!flag2)
			{
				return "HUD";
			}
			string prefabID = buildingDef2.PrefabID;
			if (prefabID != null)
			{
				uint num = <PrivateImplementationDetails>.ComputeStringHash(prefabID);
				if (num <= 2028863301U)
				{
					if (num <= 595816591U)
					{
						if (num != 228062815U)
						{
							if (num != 595816591U)
							{
								goto IL_022B;
							}
							if (!(prefabID == "FlowerVase"))
							{
								goto IL_022B;
							}
						}
						else
						{
							if (!(prefabID == "LuxuryBed"))
							{
								goto IL_022B;
							}
							string id = permit.Id;
							if (id != null)
							{
								if (id == "LuxuryBed_boat")
								{
									return "elegantbed_boat";
								}
								if (id == "LuxuryBed_bouncy")
								{
									return "elegantbed_bouncy";
								}
							}
							return "elegantbed";
						}
					}
					else if (num != 1633134164U)
					{
						if (num != 2028863301U)
						{
							goto IL_022B;
						}
						if (!(prefabID == "FlowerVaseHanging"))
						{
							goto IL_022B;
						}
					}
					else
					{
						if (!(prefabID == "CeilingLight"))
						{
							goto IL_022B;
						}
						return "ceilingLight";
					}
				}
				else if (num <= 3048425356U)
				{
					if (num != 2899744071U)
					{
						if (num != 3048425356U)
						{
							goto IL_022B;
						}
						if (!(prefabID == "Bed"))
						{
							goto IL_022B;
						}
						return "bed";
					}
					else
					{
						if (!(prefabID == "ExteriorWall"))
						{
							goto IL_022B;
						}
						return "wall";
					}
				}
				else if (num != 3132083755U)
				{
					if (num != 3958671086U)
					{
						goto IL_022B;
					}
					if (!(prefabID == "FlowerVaseHangingFancy"))
					{
						goto IL_022B;
					}
				}
				else if (!(prefabID == "FlowerVaseWall"))
				{
					goto IL_022B;
				}
				return "flowervase";
			}
		}
		IL_022B:
		if (permit.Category == PermitCategory.Artwork)
		{
			bool flag;
			BuildingDef buildingDef;
			KleiPermitVisUtil.GetBuildingDef(permit).Deconstruct(out flag, out buildingDef);
			bool flag3 = flag;
			BuildingDef buildingDef3 = buildingDef;
			if (!flag3)
			{
				return "HUD";
			}
			ArtableStage artableStage = (ArtableStage)permit;
			if (KleiInventoryScreen.<GetFacadeItemSoundName>g__Has|53_0<Sculpture>(buildingDef3))
			{
				if (buildingDef3.PrefabID == "IceSculpture")
				{
					return "icesculpture";
				}
				return "sculpture";
			}
			else if (KleiInventoryScreen.<GetFacadeItemSoundName>g__Has|53_0<Painting>(buildingDef3))
			{
				return "painting";
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
	internal static bool <GetFacadeItemSoundName>g__Has|53_0<T>(BuildingDef buildingDef) where T : Component
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
	private LocText selectionRarityDetailsLabel;

	[SerializeField]
	private LocText selectionOwnedCount;

	private enum MultiToggleState
	{
		Default,
		Selected,
		NonInteractable
	}
}
