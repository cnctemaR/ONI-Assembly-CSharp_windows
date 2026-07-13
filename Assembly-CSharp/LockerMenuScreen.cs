using System;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class LockerMenuScreen : KModalScreen
{
	protected override void OnActivate()
	{
		LockerMenuScreen.Instance = this;
		this.Show(false);
	}

	public override float GetSortKey()
	{
		return 40f;
	}

	public void ShowInventoryScreen()
	{
		if (!base.isActiveAndEnabled)
		{
			this.Show(true);
		}
		LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.kleiInventoryScreen, null);
		MusicManager.instance.SetSongParameter("Music_SupplyCloset", "SupplyClosetView", "inventory", true);
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = this.buttonInventory;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.ShowInventoryScreen();
		}));
		MultiToggle multiToggle2 = this.buttonDuplicants;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			MinionBrowserScreenConfig.Personalities(default(Option<Personality>)).ApplyAndOpenScreen(null, ClothingOutfitUtility.OutfitType.Clothing);
			MusicManager.instance.SetSongParameter("Music_SupplyCloset", "SupplyClosetView", "dupe", true);
		}));
		MultiToggle multiToggle3 = this.buttonOutfitBroswer;
		multiToggle3.onClick = (global::System.Action)Delegate.Combine(multiToggle3.onClick, new global::System.Action(delegate
		{
			OutfitBrowserScreenConfig.Mannequin().ApplyAndOpenScreen();
			MusicManager.instance.SetSongParameter("Music_SupplyCloset", "SupplyClosetView", "wardrobe", true);
		}));
		this.closeButton.onClick += delegate
		{
			this.Show(false);
		};
		this.ConfigureHoverForButton(this.buttonInventory, UI.LOCKER_MENU.BUTTON_INVENTORY_DESCRIPTION, true);
		this.ConfigureHoverForButton(this.buttonDuplicants, UI.LOCKER_MENU.BUTTON_DUPLICANTS_DESCRIPTION, true);
		this.ConfigureHoverForButton(this.buttonOutfitBroswer, UI.LOCKER_MENU.BUTTON_OUTFITS_DESCRIPTION, true);
		this.descriptionArea.text = UI.LOCKER_MENU.DEFAULT_DESCRIPTION;
		this.CreateDLCLogos();
	}

	private void ConfigureHoverForButton(MultiToggle toggle, string desc, bool useHoverColor = true)
	{
		LockerMenuScreen.<>c__DisplayClass19_0 CS$<>8__locals1 = new LockerMenuScreen.<>c__DisplayClass19_0();
		CS$<>8__locals1.useHoverColor = useHoverColor;
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.defaultColor = new Color(0.30980393f, 0.34117648f, 0.38431373f, 1f);
		CS$<>8__locals1.hoverColor = new Color(0.7019608f, 0.3647059f, 0.53333336f, 1f);
		toggle.onEnter = null;
		toggle.onExit = null;
		toggle.onEnter = (global::System.Action)Delegate.Combine(toggle.onEnter, CS$<>8__locals1.<ConfigureHoverForButton>g__OnHoverEnterFn|0(toggle, desc));
		toggle.onExit = (global::System.Action)Delegate.Combine(toggle.onExit, CS$<>8__locals1.<ConfigureHoverForButton>g__OnHoverExitFn|1(toggle));
	}

	public override void Show(bool show = true)
	{
		base.Show(show);
		if (show)
		{
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot);
			MusicManager.instance.OnSupplyClosetMenu(true, 0.5f);
			MusicManager.instance.PlaySong("Music_SupplyCloset", false);
			ThreadedHttps<KleiAccount>.Instance.AuthenticateUser(new KleiAccount.GetUserIDdelegate(this.TriggerShouldRefreshClaimItems), false);
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.OnSupplyClosetMenu(false, 1f);
			if (MusicManager.instance.SongIsPlaying("Music_SupplyCloset"))
			{
				MusicManager.instance.StopSong("Music_SupplyCloset", true, STOP_MODE.ALLOWFADEOUT);
			}
		}
		this.RefreshClaimItemsButton();
	}

	private void TriggerShouldRefreshClaimItems()
	{
		this.refreshRequested = true;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (KPrivacyPrefs.instance.disableDataCollection)
		{
			this.noConnectionIcon.GetComponent<ToolTip>().SetSimpleTooltip(UI.LOCKER_MENU.OFFLINE_ICON_TOOLTIP_DATA_COLLECTIONS);
		}
	}

	protected override void OnForcedCleanUp()
	{
		base.OnForcedCleanUp();
	}

	private void RefreshClaimItemsButton()
	{
		this.noConnectionIcon.SetActive(!ThreadedHttps<KleiAccount>.Instance.HasValidTicket());
		this.refreshRequested = false;
		bool hasClaimable = PermitItems.HasUnopenedItem();
		this.dropsAvailableNotification.SetActive(hasClaimable);
		this.buttonClaimItems.ChangeState(hasClaimable ? 0 : 1);
		this.buttonClaimItems.GetComponent<HierarchyReferences>().GetReference<Image>("FGIcon").material = (hasClaimable ? null : this.desatUIMaterial);
		this.buttonClaimItems.onClick = null;
		MultiToggle multiToggle = this.buttonClaimItems;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			if (!hasClaimable)
			{
				return;
			}
			global::UnityEngine.Object.FindObjectOfType<KleiItemDropScreen>(true).Show(true);
			this.Show(false);
		}));
		this.ConfigureHoverForButton(this.buttonClaimItems, hasClaimable ? UI.LOCKER_MENU.BUTTON_CLAIM_DESCRIPTION : UI.LOCKER_MENU.BUTTON_CLAIM_NONE_DESCRIPTION, hasClaimable);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Show(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.OnSupplyClosetMenu(false, 1f);
			if (MusicManager.instance.SongIsPlaying("Music_SupplyCloset"))
			{
				MusicManager.instance.StopSong("Music_SupplyCloset", true, STOP_MODE.ALLOWFADEOUT);
			}
		}
		base.OnKeyDown(e);
	}

	private void Update()
	{
		if (this.refreshRequested)
		{
			this.RefreshClaimItemsButton();
		}
	}

	private void CreateDLCLogos()
	{
		using (Dictionary<string, DlcManager.DlcInfo>.Enumerator enumerator = DlcManager.DLC_PACKS.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, DlcManager.DlcInfo> dlc = enumerator.Current;
				if (dlc.Value.isCosmetic)
				{
					GameObject gameObject = global::Util.KInstantiateUI(this.DLCLogoPrefab, this.DLCLogoContainer, true);
					Image component = gameObject.GetComponent<Image>();
					component.sprite = Assets.GetSprite(DlcManager.GetDlcLargeLogo(dlc.Key));
					component.material = (DlcManager.IsContentSubscribed(dlc.Key) ? GlobalResources.Instance().AnimUIMaterial : GlobalResources.Instance().AnimMaterialUIDesaturated);
					gameObject.GetComponent<MultiToggle>().states[0].sprite = Assets.GetSprite(DlcManager.GetDlcSmallLogo(dlc.Key));
					string text = DlcManager.GetDlcTitle(dlc.Key);
					if (!DlcManager.IsContentSubscribed(dlc.Key))
					{
						text = string.Concat(new string[]
						{
							text,
							"\n\n",
							UI.FRONTEND.MAINMENU.WISHLIST_AD,
							"\n\n",
							UI.FRONTEND.MAINMENU.WISHLIST_AD_TOOLTIP
						});
					}
					else
					{
						text = string.Concat(new string[]
						{
							text,
							"\n\n",
							UI.FRONTEND.MAINMENU.DLC.CONTENT_INSTALLED_LABEL,
							"\n\n",
							UI.FRONTEND.MAINMENU.DLC.COSMETIC_CONTENT_ACTIVE_TOOLTIP,
							"\n\n",
							UI.FRONTEND.MAINMENU.WISHLIST_AD_TOOLTIP
						});
					}
					gameObject.GetComponent<ToolTip>().SetSimpleTooltip(text);
					MultiToggle component2 = gameObject.GetComponent<MultiToggle>();
					component2.onClick = (global::System.Action)Delegate.Combine(component2.onClick, new global::System.Action(delegate
					{
						App.OpenWebURL(this.GetCosmeticDLCStoreURL(dlc.Key));
					}));
					gameObject.gameObject.SetActive(true);
				}
			}
		}
	}

	private string GetCosmeticDLCStoreURL(string dlcId)
	{
		if (DistributionPlatform.Initialized || Application.isEditor)
		{
			if (DistributionPlatform.Inst.Name == "Steam")
			{
				if (dlcId == "COSMETIC1_ID")
				{
					return "https://store.steampowered.com/app/4157740/Oxygen_Not_Included_Neutronium_Cosmetics_Pack/";
				}
				return "";
			}
			else if (DistributionPlatform.Inst.Name == "Epic")
			{
				if (dlcId == "COSMETIC1_ID")
				{
					return "https://store.epicgames.com/p/oxygen-not-included-oxygen-not-included-neutronium-cosmetics-pack-d9e8af";
				}
				return "";
			}
			else if (DistributionPlatform.Inst.Name == "Rail")
			{
				if (dlcId == "COSMETIC1_ID")
				{
					return "https://www.wegame.com.cn/store/2002628";
				}
				return "";
			}
		}
		return "";
	}

	public static LockerMenuScreen Instance;

	[SerializeField]
	private MultiToggle buttonInventory;

	[SerializeField]
	private MultiToggle buttonDuplicants;

	[SerializeField]
	private MultiToggle buttonOutfitBroswer;

	[SerializeField]
	private MultiToggle buttonClaimItems;

	[SerializeField]
	private LocText descriptionArea;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private GameObject dropsAvailableNotification;

	[SerializeField]
	private GameObject noConnectionIcon;

	private const string LOCKER_MENU_MUSIC = "Music_SupplyCloset";

	private const string MUSIC_PARAMETER = "SupplyClosetView";

	[SerializeField]
	private Material desatUIMaterial;

	private bool refreshRequested;

	[SerializeField]
	private GameObject DLCLogoContainer;

	[SerializeField]
	private GameObject DLCLogoPrefab;
}
