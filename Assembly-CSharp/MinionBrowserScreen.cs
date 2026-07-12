using System;
using System.Runtime.CompilerServices;
using Database;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class MinionBrowserScreen : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.gridLayouter = new GridLayouter
		{
			minCellSize = 112f,
			maxCellSize = 144f,
			targetGridLayout = this.galleryGridContent.GetComponent<GridLayoutGroup>()
		};
		this.galleryGridItemPool = new UIPrefabLocalPool(this.gridItemPrefab, this.galleryGridContent.gameObject);
	}

	protected override void OnCmpEnable()
	{
		if (this.isFirstDisplay)
		{
			this.isFirstDisplay = false;
			this.PopulateGallery();
			this.RefreshPreview();
			this.cycler.Initialize(this.CreateCycleOptions());
			this.editButton.onClick += delegate
			{
				if (this.OnEditClickedFn != null)
				{
					this.OnEditClickedFn();
				}
			};
			this.changeOutfitButton.onClick += this.OnClickChangeOutfit;
		}
		else
		{
			this.RefreshGallery();
			this.RefreshPreview();
		}
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.RefreshGallery();
			this.RefreshPreview();
		});
	}

	private void Update()
	{
		this.gridLayouter.CheckIfShouldResizeGrid();
	}

	protected override void OnSpawn()
	{
		this.postponeConfiguration = false;
		if (this.Config.isValid)
		{
			this.Configure(this.Config);
			return;
		}
		this.Configure(MinionBrowserScreenConfig.Personalities(default(Option<Personality>)));
	}

	public MinionBrowserScreenConfig Config { get; private set; }

	public void Configure(MinionBrowserScreenConfig config)
	{
		this.Config = config;
		if (this.postponeConfiguration)
		{
			return;
		}
		this.PopulateGallery();
		this.RefreshPreview();
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
		foreach (MinionBrowserScreen.GridItem gridItem in this.Config.items)
		{
			this.<PopulateGallery>g__AddGridIcon|32_0(gridItem);
		}
		this.RefreshGallery();
		this.SelectMinion(this.Config.defaultSelectedItem.Unwrap());
	}

	private void SelectMinion(MinionBrowserScreen.GridItem item)
	{
		this.selectedGridItem = item;
		this.RefreshGallery();
		this.RefreshPreview();
		this.UIMinion.GetMinionVoice().PlaySoundUI("voice_land");
	}

	public void RefreshPreview()
	{
		this.UIMinion.SetMinion(this.selectedGridItem.GetPersonality());
		this.UIMinion.ReactToPersonalityChange();
		this.detailsHeaderText.SetText(this.selectedGridItem.GetName());
		this.detailHeaderIcon.sprite = this.selectedGridItem.GetIcon();
		this.RefreshOutfitDescription();
		this.RefreshPreviewButtonsInteractable();
		this.SetDioramaBG();
	}

	private void RefreshOutfitDescription()
	{
		if (this.RefreshOutfitDescriptionFn != null)
		{
			this.RefreshOutfitDescriptionFn();
		}
	}

	private void OnClickChangeOutfit()
	{
		if (this.selectedOutfitType.IsNone())
		{
			return;
		}
		OutfitBrowserScreenConfig.Minion(this.selectedOutfitType.Unwrap(), this.selectedGridItem).WithOutfit(this.selectedOutfit).ApplyAndOpenScreen();
	}

	private void RefreshPreviewButtonsInteractable()
	{
		this.editButton.isInteractable = true;
		if (this.currentOutfitType == ClothingOutfitUtility.OutfitType.JoyResponse)
		{
			Option<string> joyResponseEditError = this.GetJoyResponseEditError();
			if (joyResponseEditError.IsSome())
			{
				this.editButton.isInteractable = false;
				this.editButton.gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(joyResponseEditError.Unwrap());
				return;
			}
			this.editButton.isInteractable = true;
			this.editButton.gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
		}
	}

	private void SetDioramaBG()
	{
		this.dioramaBGImage.sprite = KleiPermitDioramaVis.GetDioramaBackground(this.currentOutfitType);
	}

	private Option<string> GetJoyResponseEditError()
	{
		string joyTrait = this.selectedGridItem.GetPersonality().joyTrait;
		if (!(joyTrait == "BalloonArtist"))
		{
			return Option.Some<string>(UI.JOY_RESPONSE_DESIGNER_SCREEN.TOOLTIP_NO_FACADES_FOR_JOY_TRAIT.Replace("{JoyResponseType}", Db.Get().traits.Get(joyTrait).Name));
		}
		return Option.None;
	}

	public void SetEditingOutfitType(ClothingOutfitUtility.OutfitType outfitType)
	{
		this.currentOutfitType = outfitType;
		switch (outfitType)
		{
		case ClothingOutfitUtility.OutfitType.Clothing:
			this.editButtonText.text = UI.MINION_BROWSER_SCREEN.BUTTON_EDIT_OUTFIT_ITEMS;
			this.changeOutfitButton.gameObject.SetActive(true);
			break;
		case ClothingOutfitUtility.OutfitType.JoyResponse:
			this.editButtonText.text = UI.MINION_BROWSER_SCREEN.BUTTON_EDIT_JOY_RESPONSE;
			this.changeOutfitButton.gameObject.SetActive(false);
			break;
		case ClothingOutfitUtility.OutfitType.AtmoSuit:
			this.editButtonText.text = UI.MINION_BROWSER_SCREEN.BUTTON_EDIT_ATMO_SUIT_OUTFIT_ITEMS;
			this.changeOutfitButton.gameObject.SetActive(true);
			break;
		default:
			throw new NotImplementedException();
		}
		this.RefreshPreviewButtonsInteractable();
		this.OnEditClickedFn = delegate
		{
			switch (outfitType)
			{
			case ClothingOutfitUtility.OutfitType.Clothing:
			case ClothingOutfitUtility.OutfitType.AtmoSuit:
				OutfitDesignerScreenConfig.Minion(this.selectedOutfit.IsSome() ? this.selectedOutfit.Unwrap() : ClothingOutfitTarget.ForNewTemplateOutfit(outfitType), this.selectedGridItem).ApplyAndOpenScreen();
				return;
			case ClothingOutfitUtility.OutfitType.JoyResponse:
			{
				JoyResponseScreenConfig joyResponseScreenConfig = JoyResponseScreenConfig.From(this.selectedGridItem);
				joyResponseScreenConfig = joyResponseScreenConfig.WithInitialSelection(this.selectedGridItem.GetJoyResponseOutfitTarget().ReadFacadeId().AndThen<BalloonArtistFacadeResource>((string id) => Db.Get().Permits.BalloonArtistFacades.Get(id)));
				joyResponseScreenConfig.ApplyAndOpenScreen();
				return;
			}
			default:
				throw new NotImplementedException();
			}
		};
		this.RefreshOutfitDescriptionFn = delegate
		{
			switch (outfitType)
			{
			case ClothingOutfitUtility.OutfitType.Clothing:
			case ClothingOutfitUtility.OutfitType.AtmoSuit:
				this.selectedOutfit = this.selectedGridItem.GetClothingOutfitTarget(outfitType);
				this.UIMinion.SetOutfit(outfitType, this.selectedOutfit);
				this.outfitDescriptionPanel.Refresh(this.selectedOutfit, outfitType);
				return;
			case ClothingOutfitUtility.OutfitType.JoyResponse:
			{
				this.selectedOutfit = this.selectedGridItem.GetClothingOutfitTarget(ClothingOutfitUtility.OutfitType.Clothing);
				this.UIMinion.SetOutfit(ClothingOutfitUtility.OutfitType.Clothing, this.selectedOutfit);
				string text = this.selectedGridItem.GetJoyResponseOutfitTarget().ReadFacadeId().UnwrapOr(null, null);
				this.outfitDescriptionPanel.Refresh((text != null) ? Db.Get().Permits.Get(text) : null, outfitType);
				return;
			}
			default:
				throw new NotImplementedException();
			}
		};
		this.RefreshOutfitDescription();
	}

	private MinionBrowserScreen.CyclerUI.OnSelectedFn[] CreateCycleOptions()
	{
		MinionBrowserScreen.CyclerUI.OnSelectedFn[] array = new MinionBrowserScreen.CyclerUI.OnSelectedFn[3];
		for (int i = 0; i < 3; i++)
		{
			ClothingOutfitUtility.OutfitType outfitType = (ClothingOutfitUtility.OutfitType)i;
			array[i] = delegate
			{
				this.selectedOutfitType = Option.Some<ClothingOutfitUtility.OutfitType>(outfitType);
				this.cycler.SetLabel(outfitType.GetName());
				this.SetEditingOutfitType(outfitType);
				this.RefreshPreview();
			};
		}
		return array;
	}

	private void OnMouseOverToggle()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover", false));
	}

	[CompilerGenerated]
	private void <PopulateGallery>g__AddGridIcon|32_0(MinionBrowserScreen.GridItem item)
	{
		GameObject gameObject = this.galleryGridItemPool.Borrow();
		gameObject.GetComponent<HierarchyReferences>().GetReference<Image>("Icon").sprite = item.GetIcon();
		gameObject.GetComponent<HierarchyReferences>().GetReference<LocText>("Label").SetText(item.GetName());
		MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
		MultiToggle toggle3 = toggle;
		toggle3.onEnter = (global::System.Action)Delegate.Combine(toggle3.onEnter, new global::System.Action(this.OnMouseOverToggle));
		MultiToggle toggle2 = toggle;
		toggle2.onClick = (global::System.Action)Delegate.Combine(toggle2.onClick, new global::System.Action(delegate
		{
			this.SelectMinion(item);
		}));
		this.RefreshGalleryFn = (global::System.Action)Delegate.Combine(this.RefreshGalleryFn, new global::System.Action(delegate
		{
			toggle.ChangeState((item == this.selectedGridItem) ? 1 : 0);
		}));
	}

	[Header("ItemGalleryColumn")]
	[SerializeField]
	private RectTransform galleryGridContent;

	[SerializeField]
	private GameObject gridItemPrefab;

	private GridLayouter gridLayouter;

	[Header("SelectionDetailsColumn")]
	[SerializeField]
	private KleiPermitDioramaVis permitVis;

	[SerializeField]
	private UIMinion UIMinion;

	[SerializeField]
	private LocText detailsHeaderText;

	[SerializeField]
	private Image detailHeaderIcon;

	[SerializeField]
	private OutfitDescriptionPanel outfitDescriptionPanel;

	[SerializeField]
	private MinionBrowserScreen.CyclerUI cycler;

	[SerializeField]
	private KButton editButton;

	[SerializeField]
	private LocText editButtonText;

	[SerializeField]
	private KButton changeOutfitButton;

	private Option<ClothingOutfitUtility.OutfitType> selectedOutfitType;

	private Option<ClothingOutfitTarget> selectedOutfit;

	[Header("Diorama Backgrounds")]
	[SerializeField]
	private Image dioramaBGImage;

	private MinionBrowserScreen.GridItem selectedGridItem;

	private global::System.Action OnEditClickedFn;

	private bool isFirstDisplay = true;

	private bool postponeConfiguration = true;

	private UIPrefabLocalPool galleryGridItemPool;

	private global::System.Action RefreshGalleryFn;

	private global::System.Action RefreshOutfitDescriptionFn;

	private ClothingOutfitUtility.OutfitType currentOutfitType;

	private enum MultiToggleState
	{
		Default,
		Selected,
		NonInteractable
	}

	[Serializable]
	public class CyclerUI
	{
		public void Initialize(MinionBrowserScreen.CyclerUI.OnSelectedFn[] cycleOptions)
		{
			this.cyclePrevButton.onClick += this.CyclePrev;
			this.cycleNextButton.onClick += this.CycleNext;
			this.SetCycleOptions(cycleOptions);
		}

		public void SetCycleOptions(MinionBrowserScreen.CyclerUI.OnSelectedFn[] cycleOptions)
		{
			DebugUtil.Assert(cycleOptions != null);
			DebugUtil.Assert(cycleOptions.Length != 0);
			this.cycleOptions = cycleOptions;
			this.GoTo(0);
		}

		public void GoTo(int wrappingIndex)
		{
			if (this.cycleOptions == null || this.cycleOptions.Length == 0)
			{
				return;
			}
			while (wrappingIndex < 0)
			{
				wrappingIndex += this.cycleOptions.Length;
			}
			while (wrappingIndex >= this.cycleOptions.Length)
			{
				wrappingIndex -= this.cycleOptions.Length;
			}
			this.selectedIndex = wrappingIndex;
			this.cycleOptions[this.selectedIndex]();
		}

		public void CyclePrev()
		{
			this.GoTo(this.selectedIndex - 1);
		}

		public void CycleNext()
		{
			this.GoTo(this.selectedIndex + 1);
		}

		public void SetLabel(string text)
		{
			this.currentLabel.text = text;
		}

		[SerializeField]
		public KButton cyclePrevButton;

		[SerializeField]
		public KButton cycleNextButton;

		[SerializeField]
		public LocText currentLabel;

		[NonSerialized]
		private int selectedIndex = -1;

		[NonSerialized]
		private MinionBrowserScreen.CyclerUI.OnSelectedFn[] cycleOptions;

		public delegate void OnSelectedFn();
	}

	public abstract class GridItem : IEquatable<MinionBrowserScreen.GridItem>
	{
		public abstract string GetName();

		public abstract Sprite GetIcon();

		public abstract string GetUniqueId();

		public abstract Personality GetPersonality();

		public abstract Option<ClothingOutfitTarget> GetClothingOutfitTarget(ClothingOutfitUtility.OutfitType outfitType);

		public abstract JoyResponseOutfitTarget GetJoyResponseOutfitTarget();

		public override bool Equals(object obj)
		{
			MinionBrowserScreen.GridItem gridItem = obj as MinionBrowserScreen.GridItem;
			return gridItem != null && this.Equals(gridItem);
		}

		public bool Equals(MinionBrowserScreen.GridItem other)
		{
			return this.GetHashCode() == other.GetHashCode();
		}

		public override int GetHashCode()
		{
			return Hash.SDBMLower(this.GetUniqueId());
		}

		public override string ToString()
		{
			return this.GetUniqueId();
		}

		public static MinionBrowserScreen.GridItem.MinionInstanceTarget Of(GameObject minionInstance)
		{
			MinionIdentity component = minionInstance.GetComponent<MinionIdentity>();
			return new MinionBrowserScreen.GridItem.MinionInstanceTarget
			{
				minionInstance = minionInstance,
				minionIdentity = component,
				personality = Db.Get().Personalities.Get(component.personalityResourceId)
			};
		}

		public static MinionBrowserScreen.GridItem.PersonalityTarget Of(Personality personality)
		{
			return new MinionBrowserScreen.GridItem.PersonalityTarget
			{
				personality = personality
			};
		}

		public class MinionInstanceTarget : MinionBrowserScreen.GridItem
		{
			public override Sprite GetIcon()
			{
				return this.personality.GetMiniIcon();
			}

			public override string GetName()
			{
				return this.minionIdentity.GetProperName();
			}

			public override string GetUniqueId()
			{
				return "minion_instance_id::" + this.minionInstance.GetInstanceID().ToString();
			}

			public override Personality GetPersonality()
			{
				return this.personality;
			}

			public override Option<ClothingOutfitTarget> GetClothingOutfitTarget(ClothingOutfitUtility.OutfitType outfitType)
			{
				return ClothingOutfitTarget.FromMinion(outfitType, this.minionInstance);
			}

			public override JoyResponseOutfitTarget GetJoyResponseOutfitTarget()
			{
				return JoyResponseOutfitTarget.FromMinion(this.minionInstance);
			}

			public GameObject minionInstance;

			public MinionIdentity minionIdentity;

			public Personality personality;
		}

		public class PersonalityTarget : MinionBrowserScreen.GridItem
		{
			public override Sprite GetIcon()
			{
				return this.personality.GetMiniIcon();
			}

			public override string GetName()
			{
				return this.personality.Name;
			}

			public override string GetUniqueId()
			{
				return "personality::" + this.personality.nameStringKey;
			}

			public override Personality GetPersonality()
			{
				return this.personality;
			}

			public override Option<ClothingOutfitTarget> GetClothingOutfitTarget(ClothingOutfitUtility.OutfitType outfitType)
			{
				return ClothingOutfitTarget.TryFromTemplateId(this.personality.GetSelectedTemplateOutfitId(outfitType));
			}

			public override JoyResponseOutfitTarget GetJoyResponseOutfitTarget()
			{
				return JoyResponseOutfitTarget.FromPersonality(this.personality);
			}

			public Personality personality;
		}
	}
}
