using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Database;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutfitDesignerScreen : KMonoBehaviour
{
	public OutfitDesignerScreenConfig Config { get; private set; }

	public Option<string> SelectedPermitID { get; private set; }

	public PermitCategory SelectedCategory { get; private set; }

	public OutfitDesignerScreen_OutfitState outfitState { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		global::Debug.Assert(this.categoryRowPrefab.transform.parent == this.categoryListContent.transform);
		global::Debug.Assert(this.gridItemPrefab.transform.parent == this.galleryGridContent.transform);
		this.categoryRowPrefab.SetActive(false);
		this.gridItemPrefab.SetActive(false);
		this.galleryGridLayouter = new GridLayouter
		{
			minCellSize = 64f,
			maxCellSize = 96f,
			targetGridLayout = this.galleryGridContent.GetComponent<GridLayoutGroup>()
		};
		this.categoryRowPool = new UIPrefabLocalPool(this.categoryRowPrefab, this.categoryListContent.gameObject);
		this.galleryGridItemPool = new UIPrefabLocalPool(this.gridItemPrefab, this.galleryGridContent.gameObject);
		if (OutfitDesignerScreen.outfitTypeToCategoriesDict == null)
		{
			Dictionary<ClothingOutfitUtility.OutfitType, PermitCategory[]> dictionary = new Dictionary<ClothingOutfitUtility.OutfitType, PermitCategory[]>();
			dictionary[ClothingOutfitUtility.OutfitType.Clothing] = new PermitCategory[]
			{
				PermitCategory.DupeTops,
				PermitCategory.DupeGloves,
				PermitCategory.DupeBottoms,
				PermitCategory.DupeShoes
			};
			OutfitDesignerScreen.outfitTypeToCategoriesDict = dictionary;
		}
	}

	private void Update()
	{
		this.galleryGridLayouter.CheckIfShouldResizeGrid();
	}

	protected override void OnSpawn()
	{
		this.postponeConfiguration = false;
		this.minionOrMannequin.TrySpawn();
		if (this.Config.isValid)
		{
			this.Configure(this.Config);
			return;
		}
		this.Configure(OutfitDesignerScreenConfig.Mannequin(ClothingOutfitTarget.ForNewOutfit()));
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.RefreshCategories();
			this.RefreshGallery();
			this.RefreshOutfitState();
		});
		KleiItemsStatusRefresher.RequestRefreshFromServer();
	}

	protected override void OnCmpDisable()
	{
		base.OnCmpDisable();
		this.UnregisterPreventScreenPop();
	}

	private void UpdateSaveButtons()
	{
		if (this.updateSaveButtonsFn != null)
		{
			this.updateSaveButtonsFn();
		}
	}

	public void Configure(OutfitDesignerScreenConfig config)
	{
		this.Config = config;
		if (config.targetMinionInstance.HasValue)
		{
			this.outfitState = OutfitDesignerScreen_OutfitState.ForMinionInstance(this.Config.sourceTarget, config.targetMinionInstance.Value);
		}
		else
		{
			this.outfitState = OutfitDesignerScreen_OutfitState.ForTemplateOutfit(this.Config.sourceTarget);
		}
		if (this.postponeConfiguration)
		{
			return;
		}
		this.RegisterPreventScreenPop();
		Accessorizer component = this.minionOrMannequin.SetFrom(config.minionPersonality).SpawnedAvatar.GetComponent<Accessorizer>();
		using (ListPool<ClothingItemResource, OutfitDesignerScreen>.PooledList pooledList = PoolsFor<OutfitDesignerScreen>.AllocateList<ClothingItemResource>())
		{
			this.outfitState.AddItemValuesTo(pooledList);
			component.ApplyClothingItems(pooledList, true);
		}
		this.PopulateCategories();
		this.SelectCategory(OutfitDesignerScreen.outfitTypeToCategoriesDict[this.outfitState.outfitType][0]);
		this.galleryGridLayouter.RequestGridResize();
		this.RefreshOutfitState();
		OutfitDesignerScreenConfig outfitDesignerScreenConfig = this.Config;
		if (outfitDesignerScreenConfig.targetMinionInstance.HasValue)
		{
			this.updateSaveButtonsFn = null;
			this.primaryButton.ClearOnClick();
			TMP_Text componentInChildren = this.primaryButton.GetComponentInChildren<LocText>();
			LocString button_APPLY_TO_MINION = UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.BUTTON_APPLY_TO_MINION;
			string text = "{MinionName}";
			outfitDesignerScreenConfig = this.Config;
			componentInChildren.SetText(button_APPLY_TO_MINION.Replace(text, outfitDesignerScreenConfig.targetMinionInstance.Value.GetProperName()));
			this.primaryButton.onClick += delegate
			{
				ClothingOutfitTarget clothingOutfitTarget = ClothingOutfitTarget.FromMinion(this.Config.targetMinionInstance.Value);
				clothingOutfitTarget.WriteItems(this.outfitState.GetItems());
				if (this.Config.onWriteToOutfitTargetFn != null)
				{
					this.Config.onWriteToOutfitTargetFn(clothingOutfitTarget);
				}
				LockerNavigator.Instance.PopScreen();
			};
			this.secondaryButton.ClearOnClick();
			this.secondaryButton.GetComponentInChildren<LocText>().SetText(UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.BUTTON_APPLY_TO_TEMPLATE);
			this.secondaryButton.onClick += delegate
			{
				OutfitDesignerScreen.MakeApplyToTemplatePopup(this.inputFieldPrefab, this.outfitState, this.Config.targetMinionInstance.Value, this.Config.outfitTemplate, this.Config.onWriteToOutfitTargetFn);
			};
			this.updateSaveButtonsFn = (global::System.Action)Delegate.Combine(this.updateSaveButtonsFn, new global::System.Action(delegate
			{
				if (this.outfitState.DoesContainNonOwnedItems())
				{
					this.primaryButton.isInteractable = false;
					this.primaryButton.gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_LOCKED);
					this.secondaryButton.isInteractable = false;
					this.secondaryButton.gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_LOCKED);
					return;
				}
				this.primaryButton.isInteractable = true;
				this.primaryButton.gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
				this.secondaryButton.isInteractable = true;
				this.secondaryButton.gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
			}));
		}
		else
		{
			outfitDesignerScreenConfig = this.Config;
			if (!outfitDesignerScreenConfig.outfitTemplate.HasValue)
			{
				throw new NotSupportedException();
			}
			this.updateSaveButtonsFn = null;
			this.primaryButton.ClearOnClick();
			this.primaryButton.GetComponentInChildren<LocText>().SetText(UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.BUTTON_SAVE);
			this.primaryButton.onClick += delegate
			{
				this.outfitState.destinationTarget.WriteName(this.outfitState.name);
				this.outfitState.destinationTarget.WriteItems(this.outfitState.GetItems());
				OutfitDesignerScreenConfig outfitDesignerScreenConfig2 = this.Config;
				if (outfitDesignerScreenConfig2.minionPersonality.HasValue)
				{
					ClothingOutfits clothingOutfits = Db.Get().Permits.ClothingOutfits;
					outfitDesignerScreenConfig2 = this.Config;
					clothingOutfits.SetDuplicantPersonalityOutfit(outfitDesignerScreenConfig2.minionPersonality.Value.Id, this.outfitState.destinationTarget.Id, ClothingOutfitUtility.OutfitType.Clothing);
				}
				if (this.Config.onWriteToOutfitTargetFn != null)
				{
					this.Config.onWriteToOutfitTargetFn(this.outfitState.destinationTarget);
				}
				LockerNavigator.Instance.PopScreen();
			};
			this.secondaryButton.ClearOnClick();
			this.secondaryButton.GetComponentInChildren<LocText>().SetText(UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.BUTTON_COPY);
			this.secondaryButton.onClick += delegate
			{
				OutfitDesignerScreen.MakeCopyPopup(this, this.inputFieldPrefab, this.outfitState, this.Config.outfitTemplate.Value, this.Config.minionPersonality, this.Config.onWriteToOutfitTargetFn);
			};
			this.updateSaveButtonsFn = (global::System.Action)Delegate.Combine(this.updateSaveButtonsFn, new global::System.Action(delegate
			{
				if (!this.outfitState.destinationTarget.CanWriteItems)
				{
					this.primaryButton.isInteractable = false;
					this.primaryButton.gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_READONLY);
					this.secondaryButton.isInteractable = true;
					this.secondaryButton.gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
					return;
				}
				if (this.outfitState.DoesContainNonOwnedItems())
				{
					this.primaryButton.isInteractable = false;
					this.primaryButton.gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_LOCKED);
					this.secondaryButton.isInteractable = false;
					this.secondaryButton.gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(UI.OUTFIT_DESIGNER_SCREEN.OUTFIT_TEMPLATE.TOOLTIP_SAVE_ERROR_LOCKED);
					return;
				}
				this.primaryButton.isInteractable = true;
				this.primaryButton.gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
				this.secondaryButton.isInteractable = true;
				this.secondaryButton.gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
			}));
		}
		this.UpdateSaveButtons();
	}

	private void RefreshOutfitState()
	{
		this.selectionHeaderLabel.text = this.outfitState.name;
		this.outfitDescriptionPanel.Refresh(this.outfitState);
		this.UpdateSaveButtons();
	}

	private void RefreshCategories()
	{
		if (this.RefreshCategoriesFn != null)
		{
			this.RefreshCategoriesFn();
		}
	}

	public void PopulateCategories()
	{
		this.RefreshCategoriesFn = null;
		this.categoryRowPool.ReturnAll();
		PermitCategory[] array = OutfitDesignerScreen.outfitTypeToCategoriesDict[this.outfitState.outfitType];
		for (int i = 0; i < array.Length; i++)
		{
			OutfitDesignerScreen.<>c__DisplayClass44_0 CS$<>8__locals1 = new OutfitDesignerScreen.<>c__DisplayClass44_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.permitCategory = array[i];
			GameObject gameObject = this.categoryRowPool.Borrow();
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("Label").SetText(PermitCategories.GetUppercaseDisplayName(CS$<>8__locals1.permitCategory));
			component.GetReference<Image>("Icon").sprite = Assets.GetSprite(PermitCategories.GetIconName(CS$<>8__locals1.permitCategory));
			MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
			toggle.onClick = delegate
			{
				CS$<>8__locals1.<>4__this.SelectCategory(CS$<>8__locals1.permitCategory);
			};
			this.RefreshCategoriesFn = (global::System.Action)Delegate.Combine(this.RefreshCategoriesFn, new global::System.Action(delegate
			{
				toggle.ChangeState((CS$<>8__locals1.permitCategory == CS$<>8__locals1.<>4__this.SelectedCategory) ? 1 : 0);
			}));
		}
	}

	public void SelectCategory(PermitCategory permitCategory)
	{
		this.SelectedCategory = permitCategory;
		this.galleryHeaderLabel.text = PermitCategories.GetDisplayName(permitCategory);
		this.RefreshCategories();
		this.PopulateGallery();
		ref Option<ClothingItemResource> itemSlotForCategory = ref this.outfitState.GetItemSlotForCategory(permitCategory);
		if (itemSlotForCategory.HasValue)
		{
			this.SelectPermit(itemSlotForCategory.Value.Id);
			return;
		}
		this.SelectPermit(Option.None);
	}

	private void RefreshGallery()
	{
		if (this.RefreshGalleryFn != null)
		{
			this.RefreshGalleryFn();
		}
	}

	public void PopulateGallery()
	{
		this.RefreshGalleryFn = null;
		this.galleryGridItemPool.ReturnAll();
		this.<PopulateGallery>g__AddGridIconForPermitId|48_0(Option.None);
		foreach (PermitResource permitResource in Db.Get().Permits.resources)
		{
			if (PermitResources.ShouldDisplayPermitInSupplyCloset(permitResource.Id) && permitResource.PermitCategory == this.SelectedCategory)
			{
				this.<PopulateGallery>g__AddGridIconForPermitId|48_0(permitResource.Id);
			}
		}
		this.RefreshGallery();
	}

	public void SelectPermit(Option<string> permitId)
	{
		this.SelectedPermitID = permitId;
		this.RefreshGallery();
		this.UpdateSelectedItemDetails();
		this.UpdateSaveButtons();
	}

	public unsafe void UpdateSelectedItemDetails()
	{
		Option<ClothingItemResource> option = Option.None;
		if (this.SelectedPermitID.HasValue)
		{
			PermitResource permitResource = Db.Get().Permits.Get(this.SelectedPermitID);
			permitResource.GetPermitPresentationInfo();
			ClothingItemResource clothingItemResource = permitResource as ClothingItemResource;
			if (clothingItemResource != null)
			{
				option = clothingItemResource;
			}
		}
		*this.outfitState.GetItemSlotForCategory(this.SelectedCategory) = option;
		this.minionOrMannequin.current.SetOutfit(this.outfitState);
		this.minionOrMannequin.current.ReactToClothingItemChange(this.SelectedCategory);
		this.outfitDescriptionPanel.Refresh(this.outfitState);
	}

	private void RegisterPreventScreenPop()
	{
		this.UnregisterPreventScreenPop();
		this.preventScreenPopFn = delegate
		{
			if (this.outfitState.IsDirty())
			{
				this.RegisterPreventScreenPop();
				OutfitDesignerScreen.MakeSaveWarningPopup(this.outfitState, delegate
				{
					this.UnregisterPreventScreenPop();
					LockerNavigator.Instance.PopScreen();
				});
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

	public static void MakeSaveWarningPopup(OutfitDesignerScreen_OutfitState outfitState, global::System.Action discardChangesFn)
	{
		Action<InfoDialogScreen> <>9__1;
		LockerNavigator.Instance.ShowDialogPopup(delegate(InfoDialogScreen dialog)
		{
			InfoDialogScreen infoDialogScreen = dialog.SetHeader(UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.HEADER.Replace("{OutfitName}", outfitState.name)).AddPlainText(UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.BODY);
			string text = UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.BUTTON_DISCARD;
			Action<InfoDialogScreen> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(InfoDialogScreen d)
				{
					d.Deactivate();
					discardChangesFn();
				});
			}
			infoDialogScreen.AddOption(text, action, true).AddOption(UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.BUTTON_RETURN, delegate(InfoDialogScreen d)
			{
				d.Deactivate();
			}, false);
		});
	}

	public static void MakeApplyToTemplatePopup(KInputTextField inputFieldPrefab, OutfitDesignerScreen_OutfitState outfitState, GameObject targetMinionInstance, Option<ClothingOutfitTarget> existingOutfitTemplate, Action<ClothingOutfitTarget> onWriteToOutfitTargetFn)
	{
		ClothingOutfitNameProposal proposal = default(ClothingOutfitNameProposal);
		Color errorTextColor = Util.ColorFromHex("F44A47");
		Color defaultTextColor = Util.ColorFromHex("FFFFFF");
		KInputTextField inputField;
		InfoScreenPlainText descLabel;
		KButton saveButton;
		LocText saveButtonText;
		LocText descLocText;
		LockerNavigator.Instance.ShowDialogPopup(delegate(InfoDialogScreen dialog)
		{
			dialog.SetHeader(UI.OUTFIT_DESIGNER_SCREEN.MINION_INSTANCE.APPLY_TEMPLATE_POPUP.HEADER.Replace("{OutfitName}", outfitState.name)).AddUI<KInputTextField>(inputFieldPrefab, out inputField).AddSpacer(8f)
				.AddUI<InfoScreenPlainText>(dialog.GetPlainTextPrefab(), out descLabel)
				.AddOption(true, out saveButton, out saveButtonText)
				.AddDefaultCancel();
			descLocText = descLabel.gameObject.GetComponent<LocText>();
			descLocText.allowOverride = true;
			descLocText.alignment = TextAlignmentOptions.BottomLeft;
			descLocText.color = errorTextColor;
			descLocText.fontSize = 14f;
			descLabel.SetText("");
			inputField.onValueChanged.AddListener(new UnityAction<string>(base.<MakeApplyToTemplatePopup>g__Refresh|1));
			saveButton.onClick += delegate
			{
				ClothingOutfitNameProposal.Result result = proposal.result;
				ClothingOutfitTarget clothingOutfitTarget;
				if (result != ClothingOutfitNameProposal.Result.NewOutfit)
				{
					if (result != ClothingOutfitNameProposal.Result.SameOutfit)
					{
						throw new NotSupportedException(string.Format("Can't save outfit with name \"{0}\", failed with result: {1}", proposal.candidateName, proposal.result));
					}
					clothingOutfitTarget = existingOutfitTemplate.Value;
				}
				else
				{
					clothingOutfitTarget = ClothingOutfitTarget.ForNewOutfit(proposal.candidateName);
				}
				clothingOutfitTarget.WriteItems(outfitState.GetItems());
				ClothingOutfitTarget.FromMinion(targetMinionInstance).WriteItems(outfitState.GetItems());
				if (onWriteToOutfitTargetFn != null)
				{
					onWriteToOutfitTargetFn(clothingOutfitTarget);
				}
				dialog.Deactivate();
				LockerNavigator.Instance.PopScreen();
			};
			if (!existingOutfitTemplate.HasValue)
			{
				base.<MakeApplyToTemplatePopup>g__Refresh|1(outfitState.name);
				return;
			}
			if (existingOutfitTemplate.Value.CanWriteName && existingOutfitTemplate.Value.CanWriteItems)
			{
				base.<MakeApplyToTemplatePopup>g__Refresh|1(existingOutfitTemplate.Value.Id);
				return;
			}
			base.<MakeApplyToTemplatePopup>g__Refresh|1(ClothingOutfitTarget.ForCopyOf(existingOutfitTemplate.Value).Id);
		});
	}

	public static void MakeCopyPopup(OutfitDesignerScreen screen, KInputTextField inputFieldPrefab, OutfitDesignerScreen_OutfitState outfitState, ClothingOutfitTarget outfitTemplate, Option<Personality> minionPersonality, Action<ClothingOutfitTarget> onWriteToOutfitTargetFn)
	{
		ClothingOutfitNameProposal proposal = default(ClothingOutfitNameProposal);
		KInputTextField inputField;
		InfoScreenPlainText errorText;
		KButton okButton;
		LocText okButtonText;
		LockerNavigator.Instance.ShowDialogPopup(delegate(InfoDialogScreen dialog)
		{
			dialog.SetHeader(UI.OUTFIT_DESIGNER_SCREEN.COPY_POPUP.HEADER).AddUI<KInputTextField>(inputFieldPrefab, out inputField).AddSpacer(8f)
				.AddUI<InfoScreenPlainText>(dialog.GetPlainTextPrefab(), out errorText)
				.AddOption(true, out okButton, out okButtonText)
				.AddOption(UI.CONFIRMDIALOG.CANCEL, delegate(InfoDialogScreen d)
				{
					d.Deactivate();
				}, false);
			inputField.onValueChanged.AddListener(new UnityAction<string>(base.<MakeCopyPopup>g__Refresh|1));
			errorText.gameObject.SetActive(false);
			LocText component = errorText.gameObject.GetComponent<LocText>();
			component.allowOverride = true;
			component.alignment = TextAlignmentOptions.BottomLeft;
			component.color = Util.ColorFromHex("F44A47");
			component.fontSize = 14f;
			errorText.SetText("");
			okButtonText.text = UI.CONFIRMDIALOG.OK;
			okButton.onClick += delegate
			{
				if (proposal.result == ClothingOutfitNameProposal.Result.NewOutfit)
				{
					ClothingOutfitTarget clothingOutfitTarget = ClothingOutfitTarget.ForNewOutfit(proposal.candidateName);
					clothingOutfitTarget.WriteItems(outfitState.GetItems());
					if (minionPersonality.HasValue)
					{
						Db.Get().Permits.ClothingOutfits.SetDuplicantPersonalityOutfit(minionPersonality.Value.Id, clothingOutfitTarget.Id, ClothingOutfitUtility.OutfitType.Clothing);
					}
					if (onWriteToOutfitTargetFn != null)
					{
						onWriteToOutfitTargetFn(clothingOutfitTarget);
					}
					dialog.Deactivate();
					screen.Configure(screen.Config.WithOutfit(clothingOutfitTarget));
					return;
				}
				throw new NotSupportedException(string.Format("Can't save outfit with name \"{0}\", failed with result: {1}", proposal.candidateName, proposal.result));
			};
			base.<MakeCopyPopup>g__Refresh|1(ClothingOutfitTarget.ForCopyOf(outfitTemplate).Id);
		});
	}

	[CompilerGenerated]
	private void <PopulateGallery>g__AddGridIconForPermitId|48_0(Option<string> permitId)
	{
		GameObject gameObject = this.galleryGridItemPool.Borrow();
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		Image reference = component.GetReference<Image>("Icon");
		MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
		Image isUnownedOverlay = component.GetReference<Image>("IsUnownedOverlay");
		if (!permitId.HasValue)
		{
			reference.sprite = KleiItemsUI.GetNoneClothingItemIcon(this.SelectedCategory);
			KleiItemsUI.ConfigureTooltipOn(gameObject, KleiItemsUI.GetNoneClothingItemString(this.SelectedCategory));
			isUnownedOverlay.gameObject.SetActive(false);
		}
		else
		{
			PermitPresentationInfo permitPresInfo = Db.Get().Permits.Get(permitId).GetPermitPresentationInfo();
			reference.sprite = permitPresInfo.sprite;
			KleiItemsUI.ConfigureTooltipOn(gameObject, KleiItemsUI.GetTooltipStringFor(permitPresInfo));
			this.RefreshGalleryFn = (global::System.Action)Delegate.Combine(this.RefreshGalleryFn, new global::System.Action(delegate
			{
				isUnownedOverlay.gameObject.SetActive(!permitPresInfo.IsUnlocked());
			}));
		}
		toggle.onClick = delegate
		{
			this.SelectPermit(permitId);
		};
		this.RefreshGalleryFn = (global::System.Action)Delegate.Combine(this.RefreshGalleryFn, new global::System.Action(delegate
		{
			toggle.ChangeState((permitId == this.SelectedPermitID) ? 1 : 0);
		}));
	}

	[Header("CategoryColumn")]
	[SerializeField]
	private RectTransform categoryListContent;

	[SerializeField]
	private GameObject categoryRowPrefab;

	private UIPrefabLocalPool categoryRowPool;

	[Header("ItemGalleryColumn")]
	[SerializeField]
	private LocText galleryHeaderLabel;

	[SerializeField]
	private RectTransform galleryGridContent;

	[SerializeField]
	private GameObject gridItemPrefab;

	private UIPrefabLocalPool galleryGridItemPool;

	private GridLayouter galleryGridLayouter;

	[Header("SelectionDetailsColumn")]
	[SerializeField]
	private LocText selectionHeaderLabel;

	[SerializeField]
	private UIMinionOrMannequin minionOrMannequin;

	[SerializeField]
	private KButton primaryButton;

	[SerializeField]
	private KButton secondaryButton;

	[SerializeField]
	private OutfitDescriptionPanel outfitDescriptionPanel;

	[SerializeField]
	private KInputTextField inputFieldPrefab;

	public static Dictionary<ClothingOutfitUtility.OutfitType, PermitCategory[]> outfitTypeToCategoriesDict;

	private bool postponeConfiguration = true;

	private global::System.Action updateSaveButtonsFn;

	private global::System.Action RefreshCategoriesFn;

	private global::System.Action RefreshGalleryFn;

	private Func<bool> preventScreenPopFn;

	private enum MultiToggleState
	{
		Default,
		Selected,
		NonInteractable
	}
}
