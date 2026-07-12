using System;
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

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		MultiToggle multiToggle = this.buttonInventory;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.kleiInventoryScreen, null);
			MusicManager.instance.SetSongParameter("Music_SupplyCloset", "SupplyClosetView", "inventory", true);
		}));
		MultiToggle multiToggle2 = this.buttonDuplicants;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			MinionBrowserScreenConfig.Personalities(default(Option<Personality>)).ApplyAndOpenScreen(null);
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
	}

	private void ConfigureHoverForButton(MultiToggle toggle, string desc, bool useHoverColor = true)
	{
		LockerMenuScreen.<>c__DisplayClass14_0 CS$<>8__locals1 = new LockerMenuScreen.<>c__DisplayClass14_0();
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
		if (show)
		{
			KleiItemsStatusRefresher.RequestRefreshFromServer();
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		KleiItems.AddUserRewardInfoReceivedCallback(new KleiItems.UserRewardInfoReceivedCallback(this.RefreshClaimItemsButton));
	}

	protected override void OnForcedCleanUp()
	{
		KleiItems.RemoveUserRewardInfoReceivedCallback(new KleiItems.UserRewardInfoReceivedCallback(this.RefreshClaimItemsButton));
		base.OnForcedCleanUp();
	}

	private void RefreshClaimItemsButton()
	{
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

	private const string LOCKER_MENU_MUSIC = "Music_SupplyCloset";

	private const string MUSIC_PARAMETER = "SupplyClosetView";

	[SerializeField]
	private Material desatUIMaterial;
}
