using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Database;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JoyResponseDesignerScreen : KMonoBehaviour
{
	public JoyResponseScreenConfig Config { get; private set; }

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		global::Debug.Assert(this.categoryRowPrefab.transform.parent == this.categoryListContent.transform);
		global::Debug.Assert(this.galleryItemPrefab.transform.parent == this.galleryGridContent.transform);
		this.categoryRowPrefab.SetActive(false);
		this.galleryItemPrefab.SetActive(false);
		this.galleryGridLayouter = new GridLayouter
		{
			minCellSize = 64f,
			maxCellSize = 96f,
			targetGridLayout = this.galleryGridContent.GetComponent<GridLayoutGroup>()
		};
		this.categoryRowPool = new UIPrefabLocalPool(this.categoryRowPrefab, this.categoryListContent.gameObject);
		this.galleryGridItemPool = new UIPrefabLocalPool(this.galleryItemPrefab, this.galleryGridContent.gameObject);
		JoyResponseDesignerScreen.JoyResponseCategory[] array = new JoyResponseDesignerScreen.JoyResponseCategory[1];
		int num = 0;
		JoyResponseDesignerScreen.JoyResponseCategory joyResponseCategory = new JoyResponseDesignerScreen.JoyResponseCategory();
		joyResponseCategory.displayName = UI.KLEI_INVENTORY_SCREEN.CATEGORIES.JOY_RESPONSES.BALLOON_ARTIST;
		joyResponseCategory.icon = Assets.GetSprite("icon_inventory_balloonartist");
		JoyResponseDesignerScreen.GalleryItem[] array2 = Db.Get().Permits.BalloonArtistFacades.resources.Select<BalloonArtistFacadeResource, JoyResponseDesignerScreen.GalleryItem.BalloonArtistFacadeTarget>((BalloonArtistFacadeResource r) => JoyResponseDesignerScreen.GalleryItem.Of(r)).Prepend(JoyResponseDesignerScreen.GalleryItem.Of(Option.None)).ToArray<JoyResponseDesignerScreen.GalleryItem.BalloonArtistFacadeTarget>();
		joyResponseCategory.items = array2;
		array[num] = joyResponseCategory;
		this.joyResponseCategories = array;
		this.dioramaVis.ConfigureSetup();
	}

	private void Update()
	{
		this.galleryGridLayouter.CheckIfShouldResizeGrid();
	}

	protected override void OnSpawn()
	{
		this.postponeConfiguration = false;
		if (this.Config.isValid)
		{
			this.Configure(this.Config);
			return;
		}
		throw new InvalidOperationException("Cannot open up JoyResponseDesignerScreen without a target personality or minion instance");
	}

	protected override void OnCmpEnable()
	{
		base.OnCmpEnable();
		KleiItemsStatusRefresher.AddOrGetListener(this).OnRefreshUI(delegate
		{
			this.Configure(this.Config);
		});
	}

	public void Configure(JoyResponseScreenConfig config)
	{
		this.Config = config;
		if (this.postponeConfiguration)
		{
			return;
		}
		this.RegisterPreventScreenPop();
		this.primaryButton.ClearOnClick();
		TMP_Text componentInChildren = this.primaryButton.GetComponentInChildren<LocText>();
		LocString button_APPLY_TO_MINION = UI.JOY_RESPONSE_DESIGNER_SCREEN.BUTTON_APPLY_TO_MINION;
		string text = "{MinionName}";
		JoyResponseScreenConfig joyResponseScreenConfig = this.Config;
		componentInChildren.SetText(button_APPLY_TO_MINION.Replace(text, joyResponseScreenConfig.target.GetMinionName()));
		this.primaryButton.onClick += delegate
		{
			Option<PermitResource> permitResource = this.selectedGalleryItem.GetPermitResource();
			if (permitResource.IsSome())
			{
				string text2 = "Save selected balloon ";
				string name = this.selectedGalleryItem.GetName();
				string text3 = " for ";
				JoyResponseScreenConfig joyResponseScreenConfig2 = this.Config;
				global::Debug.Log(text2 + name + text3 + joyResponseScreenConfig2.target.GetMinionName());
				if (this.CanSaveSelection())
				{
					joyResponseScreenConfig2 = this.Config;
					joyResponseScreenConfig2.target.WriteFacadeId(permitResource.Unwrap().Id);
				}
			}
			else
			{
				string text4 = "Save selected balloon ";
				string name2 = this.selectedGalleryItem.GetName();
				string text5 = " for ";
				JoyResponseScreenConfig joyResponseScreenConfig2 = this.Config;
				global::Debug.Log(text4 + name2 + text5 + joyResponseScreenConfig2.target.GetMinionName());
				joyResponseScreenConfig2 = this.Config;
				joyResponseScreenConfig2.target.WriteFacadeId(Option.None);
			}
			LockerNavigator.Instance.PopScreen();
		};
		this.PopulateCategories();
		this.PopulateGallery();
		this.PopulatePreview();
		joyResponseScreenConfig = this.Config;
		if (joyResponseScreenConfig.initalSelectedItem.IsSome())
		{
			joyResponseScreenConfig = this.Config;
			this.SelectGalleryItem(joyResponseScreenConfig.initalSelectedItem.Unwrap());
		}
	}

	private bool CanSaveSelection()
	{
		return this.GetSaveSelectionError().IsNone();
	}

	private Option<string> GetSaveSelectionError()
	{
		if (!this.selectedGalleryItem.IsUnlocked())
		{
			return Option.Some<string>(UI.JOY_RESPONSE_DESIGNER_SCREEN.TOOLTIP_PICK_JOY_RESPONSE_ERROR_LOCKED.Replace("{MinionName}", this.Config.target.GetMinionName()));
		}
		return Option.None;
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
		JoyResponseDesignerScreen.JoyResponseCategory[] array = this.joyResponseCategories;
		for (int i = 0; i < array.Length; i++)
		{
			JoyResponseDesignerScreen.<>c__DisplayClass28_0 CS$<>8__locals1 = new JoyResponseDesignerScreen.<>c__DisplayClass28_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.category = array[i];
			GameObject gameObject = this.categoryRowPool.Borrow();
			HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
			component.GetReference<LocText>("Label").SetText(CS$<>8__locals1.category.displayName);
			component.GetReference<Image>("Icon").sprite = CS$<>8__locals1.category.icon;
			MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
			MultiToggle toggle2 = toggle;
			toggle2.onEnter = (global::System.Action)Delegate.Combine(toggle2.onEnter, new global::System.Action(this.OnMouseOverToggle));
			toggle.onClick = delegate
			{
				CS$<>8__locals1.<>4__this.SelectCategory(CS$<>8__locals1.category);
			};
			this.RefreshCategoriesFn = (global::System.Action)Delegate.Combine(this.RefreshCategoriesFn, new global::System.Action(delegate
			{
				toggle.ChangeState((CS$<>8__locals1.category == CS$<>8__locals1.<>4__this.selectedCategoryOpt) ? 1 : 0);
			}));
			this.SetCatogoryClickUISound(CS$<>8__locals1.category, toggle);
		}
		this.SelectCategory(this.joyResponseCategories[0]);
	}

	public void SelectCategory(JoyResponseDesignerScreen.JoyResponseCategory category)
	{
		this.selectedCategoryOpt = category;
		this.galleryHeaderLabel.text = category.displayName;
		this.RefreshCategories();
		this.PopulateGallery();
		this.RefreshPreview();
	}

	private void SetCatogoryClickUISound(JoyResponseDesignerScreen.JoyResponseCategory category, MultiToggle toggle)
	{
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
		if (this.selectedCategoryOpt.IsNone())
		{
			return;
		}
		JoyResponseDesignerScreen.JoyResponseCategory joyResponseCategory = this.selectedCategoryOpt.Unwrap();
		foreach (JoyResponseDesignerScreen.GalleryItem galleryItem in joyResponseCategory.items)
		{
			this.<PopulateGallery>g__AddGridIcon|36_0(galleryItem);
		}
		this.SelectGalleryItem(joyResponseCategory.items[0]);
	}

	public void SelectGalleryItem(JoyResponseDesignerScreen.GalleryItem item)
	{
		this.selectedGalleryItem = item;
		this.RefreshGallery();
		this.RefreshPreview();
	}

	private void OnMouseOverToggle()
	{
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Mouseover", false));
	}

	public void RefreshPreview()
	{
		if (this.RefreshPreviewFn != null)
		{
			this.RefreshPreviewFn();
		}
	}

	public void PopulatePreview()
	{
		this.RefreshPreviewFn = (global::System.Action)Delegate.Combine(this.RefreshPreviewFn, new global::System.Action(delegate
		{
			JoyResponseDesignerScreen.GalleryItem.BalloonArtistFacadeTarget balloonArtistFacadeTarget = this.selectedGalleryItem as JoyResponseDesignerScreen.GalleryItem.BalloonArtistFacadeTarget;
			if (balloonArtistFacadeTarget == null)
			{
				throw new NotImplementedException();
			}
			Option<PermitResource> permitResource = balloonArtistFacadeTarget.GetPermitResource();
			this.selectionHeaderLabel.SetText(balloonArtistFacadeTarget.GetName());
			this.dioramaVis.SetMinion(this.Config.target.GetPersonality());
			this.dioramaVis.ConfigureWith(balloonArtistFacadeTarget.permit);
			this.outfitDescriptionPanel.Refresh(permitResource.UnwrapOr(null, null), ClothingOutfitUtility.OutfitType.JoyResponse);
			Option<string> saveSelectionError = this.GetSaveSelectionError();
			if (saveSelectionError.IsSome())
			{
				this.primaryButton.isInteractable = false;
				this.primaryButton.gameObject.AddOrGet<ToolTip>().SetSimpleTooltip(saveSelectionError.Unwrap());
				return;
			}
			this.primaryButton.isInteractable = true;
			this.primaryButton.gameObject.AddOrGet<ToolTip>().ClearMultiStringTooltip();
		}));
		this.RefreshPreview();
	}

	private void RegisterPreventScreenPop()
	{
		this.UnregisterPreventScreenPop();
		this.preventScreenPopFn = delegate
		{
			if (this.Config.target.ReadFacadeId() != this.selectedGalleryItem.GetPermitResource().AndThen<string>((PermitResource r) => r.Id))
			{
				this.RegisterPreventScreenPop();
				JoyResponseDesignerScreen.MakeSaveWarningPopup(this.Config.target, delegate
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

	public static void MakeSaveWarningPopup(JoyResponseOutfitTarget target, global::System.Action discardChangesFn)
	{
		Action<InfoDialogScreen> <>9__1;
		LockerNavigator.Instance.ShowDialogPopup(delegate(InfoDialogScreen dialog)
		{
			InfoDialogScreen infoDialogScreen = dialog.SetHeader(UI.JOY_RESPONSE_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.HEADER.Replace("{MinionName}", target.GetMinionName())).AddPlainText(UI.OUTFIT_DESIGNER_SCREEN.CHANGES_NOT_SAVED_WARNING_POPUP.BODY);
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

	[CompilerGenerated]
	private void <PopulateGallery>g__AddGridIcon|36_0(JoyResponseDesignerScreen.GalleryItem item)
	{
		GameObject gameObject = this.galleryGridItemPool.Borrow();
		HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
		component.GetReference<Image>("Icon").sprite = item.GetIcon();
		component.GetReference<Image>("IsUnownedOverlay").gameObject.SetActive(!item.IsUnlocked());
		Option<PermitResource> permitResource = item.GetPermitResource();
		if (permitResource.IsSome())
		{
			KleiItemsUI.ConfigureTooltipOn(gameObject, KleiItemsUI.GetTooltipStringFor(permitResource.Unwrap()));
		}
		else
		{
			KleiItemsUI.ConfigureTooltipOn(gameObject, KleiItemsUI.GetNoneTooltipStringFor(PermitCategory.JoyResponse));
		}
		MultiToggle toggle = gameObject.GetComponent<MultiToggle>();
		MultiToggle toggle3 = toggle;
		toggle3.onEnter = (global::System.Action)Delegate.Combine(toggle3.onEnter, new global::System.Action(this.OnMouseOverToggle));
		MultiToggle toggle2 = toggle;
		toggle2.onClick = (global::System.Action)Delegate.Combine(toggle2.onClick, new global::System.Action(delegate
		{
			this.SelectGalleryItem(item);
		}));
		this.RefreshGalleryFn = (global::System.Action)Delegate.Combine(this.RefreshGalleryFn, new global::System.Action(delegate
		{
			toggle.ChangeState((item == this.selectedGalleryItem) ? 1 : 0);
		}));
	}

	[Header("CategoryColumn")]
	[SerializeField]
	private RectTransform categoryListContent;

	[SerializeField]
	private GameObject categoryRowPrefab;

	[Header("GalleryColumn")]
	[SerializeField]
	private LocText galleryHeaderLabel;

	[SerializeField]
	private RectTransform galleryGridContent;

	[SerializeField]
	private GameObject galleryItemPrefab;

	[Header("SelectionDetailsColumn")]
	[SerializeField]
	private LocText selectionHeaderLabel;

	[SerializeField]
	private KleiPermitDioramaVis_JoyResponseBalloon dioramaVis;

	[SerializeField]
	private OutfitDescriptionPanel outfitDescriptionPanel;

	[SerializeField]
	private KButton primaryButton;

	public JoyResponseDesignerScreen.JoyResponseCategory[] joyResponseCategories;

	private bool postponeConfiguration = true;

	private Option<JoyResponseDesignerScreen.JoyResponseCategory> selectedCategoryOpt;

	private UIPrefabLocalPool categoryRowPool;

	private global::System.Action RefreshCategoriesFn;

	private JoyResponseDesignerScreen.GalleryItem selectedGalleryItem;

	private UIPrefabLocalPool galleryGridItemPool;

	private GridLayouter galleryGridLayouter;

	private global::System.Action RefreshGalleryFn;

	public global::System.Action RefreshPreviewFn;

	private Func<bool> preventScreenPopFn;

	public class JoyResponseCategory
	{
		public string displayName;

		public Sprite icon;

		public JoyResponseDesignerScreen.GalleryItem[] items;
	}

	private enum MultiToggleState
	{
		Default,
		Selected
	}

	public abstract class GalleryItem : IEquatable<JoyResponseDesignerScreen.GalleryItem>
	{
		public abstract string GetName();

		public abstract Sprite GetIcon();

		public abstract string GetUniqueId();

		public abstract bool IsUnlocked();

		public abstract Option<PermitResource> GetPermitResource();

		public override bool Equals(object obj)
		{
			JoyResponseDesignerScreen.GalleryItem galleryItem = obj as JoyResponseDesignerScreen.GalleryItem;
			return galleryItem != null && this.Equals(galleryItem);
		}

		public bool Equals(JoyResponseDesignerScreen.GalleryItem other)
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

		public static JoyResponseDesignerScreen.GalleryItem.BalloonArtistFacadeTarget Of(Option<BalloonArtistFacadeResource> permit)
		{
			return new JoyResponseDesignerScreen.GalleryItem.BalloonArtistFacadeTarget
			{
				permit = permit
			};
		}

		public class BalloonArtistFacadeTarget : JoyResponseDesignerScreen.GalleryItem
		{
			public override Sprite GetIcon()
			{
				return this.permit.AndThen<Sprite>((BalloonArtistFacadeResource p) => p.GetPermitPresentationInfo().sprite).UnwrapOrElse(() => KleiItemsUI.GetNoneBalloonArtistIcon(), null);
			}

			public override string GetName()
			{
				return this.permit.AndThen<string>((BalloonArtistFacadeResource p) => p.Name).UnwrapOrElse(() => KleiItemsUI.GetNoneClothingItemStrings(PermitCategory.JoyResponse).Item1, null);
			}

			public override string GetUniqueId()
			{
				return "balloon_artist_facade::" + this.permit.AndThen<string>((BalloonArtistFacadeResource p) => p.Id).UnwrapOr("<none>", null);
			}

			public override Option<PermitResource> GetPermitResource()
			{
				return this.permit.AndThen<PermitResource>((BalloonArtistFacadeResource p) => p);
			}

			public override bool IsUnlocked()
			{
				return this.GetPermitResource().AndThen<bool>((PermitResource p) => p.IsUnlocked()).UnwrapOr(true, null);
			}

			public Option<BalloonArtistFacadeResource> permit;
		}
	}
}
