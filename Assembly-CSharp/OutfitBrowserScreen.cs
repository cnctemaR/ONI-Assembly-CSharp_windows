using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Database;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutfitBrowserScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.galleryGridItemPool = new UIPrefabLocalPool(this.gridItemPrefab, this.galleryGridContent.gameObject);
		this.gridLayouter = new GridLayouter
		{
			minCellSize = 112f,
			maxCellSize = 144f,
			targetGridLayout = this.galleryGridContent.GetComponent<GridLayoutGroup>()
		};
		this.pickOutfitButton.onClick += this.OnClickPickOutfit;
		this.editOutfitButton.onClick += delegate
		{
			new OutfitDesignerScreenConfig(this.selectedOutfit, this.Config.minionPersonality, this.Config.targetMinionInstance, new Action<ClothingOutfitTarget>(this.OnOutfitDesignerWritesToOutfitTarget)).ApplyAndOpenScreen();
		};
		this.renameOutfitButton.onClick += delegate
		{
			OutfitBrowserScreen.MakeRenamePopup(this.inputFieldPrefab, this.selectedOutfit.Value, () => this.selectedOutfit.Value.ReadName(), delegate(string new_name)
			{
				this.selectedOutfit.Value.WriteName(new_name);
				OutfitBrowserScreenConfig outfitBrowserScreenConfig = this.Config;
				if (!outfitBrowserScreenConfig.minionPersonality.HasValue)
				{
					this.lastMannequinSelectedOutfit = this.selectedOutfit;
				}
				outfitBrowserScreenConfig = this.Config;
				this.Configure(outfitBrowserScreenConfig.WithOutfit(this.selectedOutfit));
			});
		};
		this.deleteOutfitButton.onClick += delegate
		{
			OutfitBrowserScreen.MakeDeletePopup(this.selectedOutfit.Value, delegate
			{
				this.selectedOutfit.Value.Delete();
				this.Configure(this.Config.WithOutfit(Option.None));
			});
		};
	}

	public OutfitBrowserScreenConfig Config { get; private set; }

	protected override void OnCmpEnable()
	{
		if (this.isFirstDisplay)
		{
			this.isFirstDisplay = false;
			this.dioramaMinionOrMannequin.TrySpawn();
			this.postponeConfiguration = false;
			this.Configure(this.Config);
		}
		this.PopulateGallery();
		this.SelectOutfit(this.selectedOutfit, true);
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.RefreshGallery();
			this.outfitDescriptionPanel.Refresh(this.selectedOutfit, ClothingOutfitUtility.OutfitType.Clothing);
		});
		KleiItemsStatusRefresher.RequestRefreshFromServer();
	}

	public void Configure(OutfitBrowserScreenConfig config)
	{
		this.Config = config;
		if (this.postponeConfiguration)
		{
			return;
		}
		this.dioramaMinionOrMannequin.SetFrom(config.minionPersonality);
		if (config.targetMinionInstance.HasValue)
		{
			this.galleryHeaderLabel.text = UI.OUTFIT_BROWSER_SCREEN.COLUMN_HEADERS.MINION_GALLERY_HEADER.Replace("{MinionName}", config.targetMinionInstance.Value.GetProperName());
		}
		else if (config.minionPersonality.HasValue)
		{
			this.galleryHeaderLabel.text = UI.OUTFIT_BROWSER_SCREEN.COLUMN_HEADERS.MINION_GALLERY_HEADER.Replace("{MinionName}", config.minionPersonality.Value.Name);
		}
		else
		{
			this.galleryHeaderLabel.text = UI.OUTFIT_BROWSER_SCREEN.COLUMN_HEADERS.GALLERY_HEADER;
		}
		Option<ClothingOutfitTarget> option;
		if (config.minionPersonality.HasValue || config.selectedTarget.HasValue)
		{
			option = config.selectedTarget;
		}
		else if (this.lastMannequinSelectedOutfit.HasValue)
		{
			option = this.lastMannequinSelectedOutfit;
		}
		else
		{
			option = ClothingOutfitTarget.GetRandom();
		}
		if (option.HasValue && option.Value.DoesExist())
		{
			this.SelectOutfit(option, true);
		}
		else
		{
			this.SelectOutfit(Option.None, true);
		}
		this.pickOutfitButton.gameObject.SetActive(config.isPickingOutfitForDupe);
		this.renameOutfitButton.gameObject.SetActive(false);
		this.deleteOutfitButton.gameObject.SetActive(false);
		if (base.gameObject.activeInHierarchy)
		{
			base.gameObject.SetActive(false);
			base.gameObject.SetActive(true);
		}
	}

	private void RefreshGallery()
	{
		if (this.RefreshGalleryFn != null)
		{
			this.RefreshGalleryFn();
		}
	}

	private void PopulateGallery()
	{
		this.outfits.Clear();
		this.galleryGridItemPool.ReturnAll();
		this.RefreshGalleryFn = null;
		if (this.Config.isPickingOutfitForDupe)
		{
			this.<PopulateGallery>g__AddGridIconForTarget|29_0(Option.None);
		}
		OutfitBrowserScreenConfig outfitBrowserScreenConfig = this.Config;
		if (outfitBrowserScreenConfig.targetMinionInstance.HasValue)
		{
			outfitBrowserScreenConfig = this.Config;
			this.<PopulateGallery>g__AddGridIconForTarget|29_0(ClothingOutfitTarget.FromMinion(outfitBrowserScreenConfig.targetMinionInstance.Value));
		}
		foreach (ClothingOutfitTarget clothingOutfitTarget in ClothingOutfitTarget.GetAll())
		{
			this.<PopulateGallery>g__AddGridIconForTarget|29_0(clothingOutfitTarget);
		}
		this.addButtonGridItem.transform.SetAsLastSibling();
		this.addButtonGridItem.SetActive(true);
		this.addButtonGridItem.GetComponent<MultiToggle>().onClick = delegate
		{
			new OutfitDesignerScreenConfig(ClothingOutfitTarget.ForNewOutfit(), this.Config.minionPersonality, this.Config.targetMinionInstance, new Action<ClothingOutfitTarget>(this.OnOutfitDesignerWritesToOutfitTarget)).ApplyAndOpenScreen();
		};
		this.RefreshGallery();
	}

	private void OnOutfitDesignerWritesToOutfitTarget(ClothingOutfitTarget outfit)
	{
		this.Configure(this.Config.WithOutfit(outfit));
	}

	private void Update()
	{
		this.gridLayouter.CheckIfShouldResizeGrid();
	}

	private void SelectOutfit(string id, bool isFirstOpen = false)
	{
		this.SelectOutfit(ClothingOutfitTarget.FromId(id), isFirstOpen);
	}

	private void SelectOutfit(Option<ClothingOutfitTarget> outfit, bool isFirstOpen = false)
	{
		this.selectionHeaderLabel.text = outfit.ReadName();
		this.selectedOutfit = outfit;
		this.dioramaMinionOrMannequin.current.SetOutfit(outfit);
		this.dioramaMinionOrMannequin.current.ReactToFullOutfitChange();
		this.outfitDescriptionPanel.Refresh(outfit, ClothingOutfitUtility.OutfitType.Clothing);
		OutfitBrowserScreenConfig outfitBrowserScreenConfig = this.Config;
		if (!outfitBrowserScreenConfig.minionPersonality.HasValue)
		{
			this.lastMannequinSelectedOutfit = outfit;
		}
		outfitBrowserScreenConfig = this.Config;
		if (outfitBrowserScreenConfig.minionPersonality.HasValue)
		{
			this.pickOutfitButton.isInteractable = !outfit.HasValue || !outfit.Value.DoesContainNonOwnedItems();
			GameObject gameObject = this.pickOutfitButton.gameObject;
			Option<string> option;
			if (!this.pickOutfitButton.isInteractable)
			{
				LocString tooltip_PICK_OUTFIT_ERROR_LOCKED = UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_PICK_OUTFIT_ERROR_LOCKED;
				string text = "{MinionName}";
				outfitBrowserScreenConfig = this.Config;
				option = Option.Some<string>(tooltip_PICK_OUTFIT_ERROR_LOCKED.Replace(text, outfitBrowserScreenConfig.GetMinionName()));
			}
			else
			{
				option = Option.None;
			}
			KleiItemsUI.ConfigureTooltipOn(gameObject, option);
		}
		this.editOutfitButton.isInteractable = outfit.HasValue;
		this.renameOutfitButton.gameObject.SetActive(true);
		this.renameOutfitButton.isInteractable = outfit.HasValue && outfit.Value.CanWriteName;
		KleiItemsUI.ConfigureTooltipOn(this.renameOutfitButton.gameObject, this.renameOutfitButton.isInteractable ? UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_RENAME_OUTFIT : UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_RENAME_OUTFIT_ERROR_READONLY);
		this.deleteOutfitButton.gameObject.SetActive(true);
		this.deleteOutfitButton.isInteractable = outfit.HasValue && outfit.Value.CanDelete;
		KleiItemsUI.ConfigureTooltipOn(this.deleteOutfitButton.gameObject, this.deleteOutfitButton.isInteractable ? UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_DELETE_OUTFIT : UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_DELETE_OUTFIT_ERROR_READONLY);
		this.RefreshGallery();
	}

	private void OnClickPickOutfit()
	{
		OutfitBrowserScreenConfig outfitBrowserScreenConfig = this.Config;
		if (outfitBrowserScreenConfig.targetMinionInstance.HasValue)
		{
			outfitBrowserScreenConfig = this.Config;
			outfitBrowserScreenConfig.targetMinionInstance.Value.GetComponent<WearableAccessorizer>().ApplyClothingItems(this.selectedOutfit.ReadItemValues());
		}
		else
		{
			outfitBrowserScreenConfig = this.Config;
			if (outfitBrowserScreenConfig.minionPersonality.HasValue)
			{
				ClothingOutfits clothingOutfits = Db.Get().Permits.ClothingOutfits;
				outfitBrowserScreenConfig = this.Config;
				clothingOutfits.SetDuplicantPersonalityOutfit(outfitBrowserScreenConfig.minionPersonality.Value.Id, this.selectedOutfit.GetId(), ClothingOutfitUtility.OutfitType.Clothing);
				LockerNavigator.Instance.duplicantCatalogueScreen.GetComponent<MinionBrowserScreen>().RefreshPreview();
			}
		}
		LockerNavigator.Instance.PopScreen();
	}

	public static void MakeDeletePopup(ClothingOutfitTarget sourceTarget, global::System.Action deleteFn)
	{
		Action<InfoDialogScreen> <>9__1;
		LockerNavigator.Instance.ShowDialogPopup(delegate(InfoDialogScreen dialog)
		{
			InfoDialogScreen infoDialogScreen = dialog.SetHeader(UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.HEADER.Replace("{OutfitName}", sourceTarget.ReadName())).AddPlainText(UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.BODY.Replace("{OutfitName}", sourceTarget.ReadName()));
			string text = UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.BUTTON_YES_DELETE;
			Action<InfoDialogScreen> action;
			if ((action = <>9__1) == null)
			{
				action = (<>9__1 = delegate(InfoDialogScreen d)
				{
					deleteFn();
					d.Deactivate();
				});
			}
			infoDialogScreen.AddOption(text, action, true).AddOption(UI.OUTFIT_BROWSER_SCREEN.DELETE_WARNING_POPUP.BUTTON_DONT_DELETE, delegate(InfoDialogScreen d)
			{
				d.Deactivate();
			}, false);
		});
	}

	public static void MakeRenamePopup(KInputTextField inputFieldPrefab, ClothingOutfitTarget sourceTarget, Func<string> readName, Action<string> writeName)
	{
		KInputTextField inputField;
		InfoScreenPlainText errorText;
		KButton okButton;
		LocText okButtonText;
		LockerNavigator.Instance.ShowDialogPopup(delegate(InfoDialogScreen dialog)
		{
			dialog.SetHeader(UI.OUTFIT_BROWSER_SCREEN.RENAME_POPUP.HEADER).AddUI<KInputTextField>(inputFieldPrefab, out inputField).AddSpacer(8f)
				.AddUI<InfoScreenPlainText>(dialog.GetPlainTextPrefab(), out errorText)
				.AddOption(true, out okButton, out okButtonText)
				.AddOption(UI.CONFIRMDIALOG.CANCEL, delegate(InfoDialogScreen d)
				{
					d.Deactivate();
				}, false);
			inputField.onValueChanged.AddListener(new UnityAction<string>(base.<MakeRenamePopup>g__Refresh|1));
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
				writeName(inputField.text);
				dialog.Deactivate();
			};
			base.<MakeRenamePopup>g__Refresh|1(readName());
		});
	}

	private void SetButtonClickUISound(Option<ClothingOutfitTarget> target, MultiToggle toggle)
	{
		if (!target.HasValue)
		{
			toggle.states[1].on_click_override_sound_path = "HUD_Click";
			toggle.states[0].on_click_override_sound_path = "HUD_Click";
			return;
		}
		bool flag = !target.Value.DoesContainNonOwnedItems();
		toggle.states[1].on_click_override_sound_path = "ClothingItem_Click";
		toggle.states[1].sound_parameter_name = "Unlocked";
		toggle.states[1].sound_parameter_value = (flag ? 1f : 0f);
		toggle.states[1].has_sound_parameter = true;
		toggle.states[0].on_click_override_sound_path = "ClothingItem_Click";
		toggle.states[0].sound_parameter_name = "Unlocked";
		toggle.states[0].sound_parameter_value = (flag ? 1f : 0f);
		toggle.states[0].has_sound_parameter = true;
	}

	private void OnMouseOverToggle()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover", false));
	}

	[CompilerGenerated]
	private void <PopulateGallery>g__AddGridIconForTarget|29_0(Option<ClothingOutfitTarget> target)
	{
		GameObject spawn = this.galleryGridItemPool.Borrow();
		GameObject gameObject = spawn.transform.GetChild(1).gameObject;
		GameObject gameObject2 = spawn.transform.GetChild(2).gameObject;
		GameObject isUnownedOverlayGO = spawn.transform.GetChild(3).gameObject;
		gameObject.SetActive(true);
		gameObject2.SetActive(false);
		gameObject.GetComponentInChildren<UIMannequin>().SetOutfit(target);
		if (!target.HasValue)
		{
			gameObject2.SetActive(true);
			gameObject2.GetComponent<Image>().sprite = KleiItemsUI.GetNoneOutfitIcon();
		}
		MultiToggle button = spawn.GetComponent<MultiToggle>();
		MultiToggle button2 = button;
		button2.onEnter = (global::System.Action)Delegate.Combine(button2.onEnter, new global::System.Action(this.OnMouseOverToggle));
		button.onClick = delegate
		{
			this.SelectOutfit(target, false);
		};
		this.RefreshGalleryFn = (global::System.Action)Delegate.Combine(this.RefreshGalleryFn, new global::System.Action(delegate
		{
			button.ChangeState((target == this.selectedOutfit) ? 1 : 0);
			if (!target.HasValue)
			{
				KleiItemsUI.ConfigureTooltipOn(spawn, KleiItemsUI.WrapAsToolTipTitle(UI.OUTFIT_NAME.NONE));
				isUnownedOverlayGO.SetActive(false);
				return;
			}
			KleiItemsUI.ConfigureTooltipOn(spawn, KleiItemsUI.WrapAsToolTipTitle(target.Value.ReadName()));
			isUnownedOverlayGO.SetActive(target.Value.DoesContainNonOwnedItems());
		}));
		this.SetButtonClickUISound(target, button);
	}

	[Header("ItemGalleryColumn")]
	[SerializeField]
	private LocText galleryHeaderLabel;

	[SerializeField]
	private RectTransform galleryGridContent;

	[SerializeField]
	private GameObject gridItemPrefab;

	[SerializeField]
	private GameObject addButtonGridItem;

	private UIPrefabLocalPool galleryGridItemPool;

	private GridLayouter gridLayouter;

	[Header("SelectionDetailsColumn")]
	[SerializeField]
	private LocText selectionHeaderLabel;

	[SerializeField]
	private UIMinionOrMannequin dioramaMinionOrMannequin;

	[SerializeField]
	private OutfitDescriptionPanel outfitDescriptionPanel;

	[SerializeField]
	private KButton pickOutfitButton;

	[SerializeField]
	private KButton editOutfitButton;

	[SerializeField]
	private KButton renameOutfitButton;

	[SerializeField]
	private KButton deleteOutfitButton;

	[SerializeField]
	private KInputTextField inputFieldPrefab;

	private Option<ClothingOutfitTarget> lastMannequinSelectedOutfit;

	private Option<ClothingOutfitTarget> selectedOutfit;

	private Dictionary<string, MultiToggle> outfits = new Dictionary<string, MultiToggle>();

	private bool postponeConfiguration = true;

	private bool isFirstDisplay = true;

	private global::System.Action RefreshGalleryFn;

	private enum MultiToggleState
	{
		Default,
		Selected,
		NonInteractable
	}
}
