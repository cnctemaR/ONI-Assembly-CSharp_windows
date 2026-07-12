using System;
using System.Collections.Generic;
using System.Linq;
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
			targetGridLayouts = this.galleryGridContent.GetComponents<GridLayoutGroup>().ToList<GridLayoutGroup>()
		};
		this.categoriesAndSearchBar.InitializeWith(this);
		this.pickOutfitButton.onClick += this.OnClickPickOutfit;
		this.editOutfitButton.onClick += delegate
		{
			if (this.state.SelectedOutfitOpt.IsNone())
			{
				return;
			}
			new OutfitDesignerScreenConfig(this.state.SelectedOutfitOpt.Unwrap(), this.Config.minionPersonality, this.Config.targetMinionInstance, new Action<ClothingOutfitTarget>(this.OnOutfitDesignerWritesToOutfitTarget)).ApplyAndOpenScreen();
		};
		this.renameOutfitButton.onClick += delegate
		{
			ClothingOutfitTarget selectedOutfit = this.state.SelectedOutfitOpt.Unwrap();
			OutfitBrowserScreen.MakeRenamePopup(this.inputFieldPrefab, selectedOutfit, () => selectedOutfit.ReadName(), delegate(string new_name)
			{
				selectedOutfit.WriteName(new_name);
				this.Configure(this.Config.WithOutfit(selectedOutfit));
			});
		};
		this.deleteOutfitButton.onClick += delegate
		{
			ClothingOutfitTarget selectedOutfit = this.state.SelectedOutfitOpt.Unwrap();
			OutfitBrowserScreen.MakeDeletePopup(selectedOutfit, delegate
			{
				selectedOutfit.Delete();
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
			this.FirstTimeSetup();
			this.postponeConfiguration = false;
			this.Configure(this.Config);
		}
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.RefreshGallery();
			this.outfitDescriptionPanel.Refresh(this.state.SelectedOutfitOpt, ClothingOutfitUtility.OutfitType.Clothing, this.Config.minionPersonality);
		});
	}

	private void FirstTimeSetup()
	{
		this.state.OnCurrentOutfitTypeChanged += delegate
		{
			this.PopulateGallery();
			OutfitBrowserScreenConfig outfitBrowserScreenConfig = this.Config;
			Option<ClothingOutfitTarget> option;
			if (!outfitBrowserScreenConfig.minionPersonality.HasValue)
			{
				outfitBrowserScreenConfig = this.Config;
				if (!outfitBrowserScreenConfig.selectedTarget.HasValue)
				{
					option = ClothingOutfitTarget.GetRandom(this.state.CurrentOutfitType);
					goto IL_004F;
				}
			}
			option = this.Config.selectedTarget;
			IL_004F:
			if (option.IsSome() && option.Unwrap().DoesExist())
			{
				this.state.SelectedOutfitOpt = option;
				return;
			}
			this.state.SelectedOutfitOpt = Option.None;
		};
		this.state.OnSelectedOutfitOptChanged += delegate
		{
			if (this.state.SelectedOutfitOpt.IsSome())
			{
				this.selectionHeaderLabel.text = this.state.SelectedOutfitOpt.Unwrap().ReadName();
			}
			else
			{
				this.selectionHeaderLabel.text = UI.OUTFIT_NAME.NONE;
			}
			this.dioramaMinionOrMannequin.current.SetOutfit(this.state.CurrentOutfitType, this.state.SelectedOutfitOpt);
			this.dioramaMinionOrMannequin.current.ReactToFullOutfitChange();
			this.outfitDescriptionPanel.Refresh(this.state.SelectedOutfitOpt, this.state.CurrentOutfitType, this.Config.minionPersonality);
			this.dioramaBG.sprite = KleiPermitDioramaVis.GetDioramaBackground(this.state.CurrentOutfitType);
			this.pickOutfitButton.gameObject.SetActive(this.Config.isPickingOutfitForDupe);
			OutfitBrowserScreenConfig outfitBrowserScreenConfig2 = this.Config;
			if (outfitBrowserScreenConfig2.minionPersonality.IsSome())
			{
				this.pickOutfitButton.isInteractable = !this.state.SelectedOutfitOpt.IsSome() || !this.state.SelectedOutfitOpt.Unwrap().DoesContainNonOwnedItems();
				GameObject gameObject = this.pickOutfitButton.gameObject;
				Option<string> option2;
				if (!this.pickOutfitButton.isInteractable)
				{
					LocString tooltip_PICK_OUTFIT_ERROR_LOCKED = UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_PICK_OUTFIT_ERROR_LOCKED;
					string text = "{MinionName}";
					outfitBrowserScreenConfig2 = this.Config;
					option2 = Option.Some<string>(tooltip_PICK_OUTFIT_ERROR_LOCKED.Replace(text, outfitBrowserScreenConfig2.GetMinionName()));
				}
				else
				{
					option2 = Option.None;
				}
				KleiItemsUI.ConfigureTooltipOn(gameObject, option2);
			}
			this.editOutfitButton.isInteractable = this.state.SelectedOutfitOpt.IsSome();
			this.renameOutfitButton.isInteractable = this.state.SelectedOutfitOpt.IsSome() && this.state.SelectedOutfitOpt.Unwrap().CanWriteName;
			KleiItemsUI.ConfigureTooltipOn(this.renameOutfitButton.gameObject, this.renameOutfitButton.isInteractable ? UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_RENAME_OUTFIT : UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_RENAME_OUTFIT_ERROR_READONLY);
			this.deleteOutfitButton.isInteractable = this.state.SelectedOutfitOpt.IsSome() && this.state.SelectedOutfitOpt.Unwrap().CanDelete;
			KleiItemsUI.ConfigureTooltipOn(this.deleteOutfitButton.gameObject, this.deleteOutfitButton.isInteractable ? UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_DELETE_OUTFIT : UI.OUTFIT_BROWSER_SCREEN.TOOLTIP_DELETE_OUTFIT_ERROR_READONLY);
			this.state.OnSelectedOutfitOptChanged += this.RefreshGallery;
			this.state.OnFilterChanged += this.RefreshGallery;
			this.state.OnCurrentOutfitTypeChanged += this.RefreshGallery;
			this.RefreshGallery();
		};
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
		this.state.CurrentOutfitType = config.onlyShowOutfitType.UnwrapOr(this.lastShownOutfitType.UnwrapOr(ClothingOutfitUtility.OutfitType.Clothing, null), null);
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
			this.<PopulateGallery>g__AddGridIconForTarget|35_0(Option.None);
		}
		OutfitBrowserScreenConfig outfitBrowserScreenConfig = this.Config;
		if (outfitBrowserScreenConfig.targetMinionInstance.HasValue)
		{
			ClothingOutfitUtility.OutfitType currentOutfitType = this.state.CurrentOutfitType;
			outfitBrowserScreenConfig = this.Config;
			this.<PopulateGallery>g__AddGridIconForTarget|35_0(ClothingOutfitTarget.FromMinion(currentOutfitType, outfitBrowserScreenConfig.targetMinionInstance.Value));
		}
		foreach (ClothingOutfitTarget clothingOutfitTarget in from outfit in ClothingOutfitTarget.GetAllTemplates()
			where outfit.OutfitType == this.state.CurrentOutfitType
			select outfit)
		{
			this.<PopulateGallery>g__AddGridIconForTarget|35_0(clothingOutfitTarget);
		}
		this.addButtonGridItem.transform.SetAsLastSibling();
		this.addButtonGridItem.SetActive(true);
		this.addButtonGridItem.GetComponent<MultiToggle>().onClick = delegate
		{
			new OutfitDesignerScreenConfig(ClothingOutfitTarget.ForNewTemplateOutfit(this.state.CurrentOutfitType), this.Config.minionPersonality, this.Config.targetMinionInstance, new Action<ClothingOutfitTarget>(this.OnOutfitDesignerWritesToOutfitTarget)).ApplyAndOpenScreen();
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

	private void OnClickPickOutfit()
	{
		OutfitBrowserScreenConfig outfitBrowserScreenConfig = this.Config;
		if (outfitBrowserScreenConfig.targetMinionInstance.IsSome())
		{
			outfitBrowserScreenConfig = this.Config;
			WearableAccessorizer component = outfitBrowserScreenConfig.targetMinionInstance.Unwrap().GetComponent<WearableAccessorizer>();
			ClothingOutfitUtility.OutfitType currentOutfitType = this.state.CurrentOutfitType;
			Option<ClothingOutfitTarget> option = this.state.SelectedOutfitOpt;
			component.AddCustomClothingOutfit(currentOutfitType, option.AndThen<IEnumerable<ClothingItemResource>>((ClothingOutfitTarget outfit) => outfit.ReadItemValues()).UnwrapOr(ClothingOutfitTarget.NO_ITEM_VALUES, null));
		}
		else
		{
			outfitBrowserScreenConfig = this.Config;
			if (outfitBrowserScreenConfig.minionPersonality.IsSome())
			{
				ClothingOutfits clothingOutfits = Db.Get().Permits.ClothingOutfits;
				outfitBrowserScreenConfig = this.Config;
				string id = outfitBrowserScreenConfig.minionPersonality.Value.Id;
				Option<ClothingOutfitTarget> option = this.state.SelectedOutfitOpt;
				clothingOutfits.SetDuplicantPersonalityOutfit(id, option.AndThen<string>((ClothingOutfitTarget o) => o.OutfitId), this.state.CurrentOutfitType);
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
	private void <PopulateGallery>g__AddGridIconForTarget|35_0(Option<ClothingOutfitTarget> target)
	{
		GameObject spawn = this.galleryGridItemPool.Borrow();
		GameObject gameObject = spawn.transform.GetChild(1).gameObject;
		GameObject isUnownedOverlayGO = spawn.transform.GetChild(2).gameObject;
		gameObject.SetActive(true);
		bool flag = target.IsNone() || this.state.CurrentOutfitType == ClothingOutfitUtility.OutfitType.AtmoSuit;
		UIMannequin componentInChildren = gameObject.GetComponentInChildren<UIMannequin>();
		this.dioramaMinionOrMannequin.mannequin.shouldShowOutfitWithDefaultItems = flag;
		componentInChildren.shouldShowOutfitWithDefaultItems = flag;
		componentInChildren.personalityToUseForDefaultClothing = this.Config.minionPersonality;
		componentInChildren.SetOutfit(this.state.CurrentOutfitType, target);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		float num;
		float num2;
		float num3;
		float num4;
		switch (this.state.CurrentOutfitType)
		{
		case ClothingOutfitUtility.OutfitType.Clothing:
			num = 8f;
			num2 = 8f;
			num3 = 8f;
			num4 = 8f;
			break;
		case ClothingOutfitUtility.OutfitType.JoyResponse:
			throw new NotSupportedException();
		case ClothingOutfitUtility.OutfitType.AtmoSuit:
			num = 24f;
			num2 = 16f;
			num3 = 32f;
			num4 = 8f;
			break;
		default:
			throw new NotImplementedException();
		}
		component.offsetMin = new Vector2(num, num4);
		component.offsetMax = new Vector2(-num2, -num3);
		MultiToggle button = spawn.GetComponent<MultiToggle>();
		MultiToggle button2 = button;
		button2.onEnter = (global::System.Action)Delegate.Combine(button2.onEnter, new global::System.Action(this.OnMouseOverToggle));
		button.onClick = delegate
		{
			this.state.SelectedOutfitOpt = target;
		};
		this.RefreshGalleryFn = (global::System.Action)Delegate.Combine(this.RefreshGalleryFn, new global::System.Action(delegate
		{
			button.ChangeState((target == this.state.SelectedOutfitOpt) ? 1 : 0);
			if (string.IsNullOrWhiteSpace(this.state.Filter) || target.IsNone())
			{
				spawn.SetActive(true);
			}
			else
			{
				spawn.SetActive(target.Unwrap().ReadName().ToLower()
					.Contains(this.state.Filter.ToLower()));
			}
			if (!target.HasValue)
			{
				KleiItemsUI.ConfigureTooltipOn(spawn, KleiItemsUI.WrapAsToolTipTitle(KleiItemsUI.GetNoneOutfitName(this.state.CurrentOutfitType)));
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
	private OutfitBrowserScreen_CategoriesAndSearchBar categoriesAndSearchBar;

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
	private Image dioramaBG;

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

	[Header("Misc")]
	[SerializeField]
	private KInputTextField inputFieldPrefab;

	[SerializeField]
	public ColorStyleSetting selectedCategoryStyle;

	[SerializeField]
	public ColorStyleSetting notSelectedCategoryStyle;

	public OutfitBrowserScreen.State state = new OutfitBrowserScreen.State();

	public Option<ClothingOutfitUtility.OutfitType> lastShownOutfitType = Option.None;

	private Dictionary<string, MultiToggle> outfits = new Dictionary<string, MultiToggle>();

	private bool postponeConfiguration = true;

	private bool isFirstDisplay = true;

	private global::System.Action RefreshGalleryFn;

	public class State
	{
		public event global::System.Action OnSelectedOutfitOptChanged;

		public Option<ClothingOutfitTarget> SelectedOutfitOpt
		{
			get
			{
				return this.m_selectedOutfitOpt;
			}
			set
			{
				this.m_selectedOutfitOpt = value;
				if (this.OnSelectedOutfitOptChanged != null)
				{
					this.OnSelectedOutfitOptChanged();
				}
			}
		}

		public event global::System.Action OnCurrentOutfitTypeChanged;

		public ClothingOutfitUtility.OutfitType CurrentOutfitType
		{
			get
			{
				return this.m_currentOutfitType;
			}
			set
			{
				this.m_currentOutfitType = value;
				if (this.OnCurrentOutfitTypeChanged != null)
				{
					this.OnCurrentOutfitTypeChanged();
				}
			}
		}

		public event global::System.Action OnFilterChanged;

		public string Filter
		{
			get
			{
				return this.m_filter;
			}
			set
			{
				this.m_filter = value;
				if (this.OnFilterChanged != null)
				{
					this.OnFilterChanged();
				}
			}
		}

		private Option<ClothingOutfitTarget> m_selectedOutfitOpt;

		private ClothingOutfitUtility.OutfitType m_currentOutfitType;

		private string m_filter;
	}

	private enum MultiToggleState
	{
		Default,
		Selected,
		NonInteractable
	}
}
