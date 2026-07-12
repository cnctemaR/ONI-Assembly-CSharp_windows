using System;
using Database;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

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
		LockerMenuScreen.<>c__DisplayClass13_0 CS$<>8__locals1 = new LockerMenuScreen.<>c__DisplayClass13_0();
		CS$<>8__locals1.<>4__this = this;
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
		MultiToggle multiToggle4 = this.buttonClaimItems;
		multiToggle4.onClick = (global::System.Action)Delegate.Combine(multiToggle4.onClick, new global::System.Action(delegate
		{
			KleiItemsStatusRefresher.Active = true;
			string text = "https://accounts.klei.com/account/rewards?game=ONI";
			if (KleiAccount.KleiUserID != null)
			{
				text = text + "&expectedKU=" + KleiAccount.KleiUserID;
			}
			Application.OpenURL(text);
			LockerMenuScreen.shouldPreventDisplayingDropsAvailableNotification = true;
			CS$<>8__locals1.<>4__this.dropsAvailableNotification.SetActive(false);
		}));
		this.closeButton.onClick += delegate
		{
			CS$<>8__locals1.<>4__this.Show(false);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSupplyClosetSnapshot, STOP_MODE.ALLOWFADEOUT);
			MusicManager.instance.OnSupplyClosetMenu(false, 1f);
			if (MusicManager.instance.SongIsPlaying("Music_SupplyCloset"))
			{
				MusicManager.instance.StopSong("Music_SupplyCloset", true, STOP_MODE.ALLOWFADEOUT);
			}
		};
		CS$<>8__locals1.defaultColor = new Color(0.30980393f, 0.34117648f, 0.38431373f, 1f);
		CS$<>8__locals1.hoverColor = new Color(0.7019608f, 0.3647059f, 0.53333336f, 1f);
		CS$<>8__locals1.<OnPrefabInit>g__ConfigureHoverFor|1(this.buttonInventory, UI.LOCKER_MENU.BUTTON_INVENTORY_DESCRIPTION);
		CS$<>8__locals1.<OnPrefabInit>g__ConfigureHoverFor|1(this.buttonDuplicants, UI.LOCKER_MENU.BUTTON_DUPLICANTS_DESCRIPTION);
		CS$<>8__locals1.<OnPrefabInit>g__ConfigureHoverFor|1(this.buttonOutfitBroswer, UI.LOCKER_MENU.BUTTON_OUTFITS_DESCRIPTION);
		CS$<>8__locals1.<OnPrefabInit>g__ConfigureHoverFor|1(this.buttonClaimItems, UI.LOCKER_MENU.BUTTON_CLAIM_DESCRIPTION);
		this.descriptionArea.text = UI.LOCKER_MENU.DEFAULT_DESCRIPTION;
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
		if (LockerMenuScreen.shouldPreventDisplayingDropsAvailableNotification)
		{
			this.dropsAvailableNotification.SetActive(false);
			return;
		}
		this.dropsAvailableNotification.SetActive(this.AreAllOwnablePermitsLocked());
	}

	private bool AreAllOwnablePermitsLocked()
	{
		foreach (PermitResource permitResource in Db.Get().Permits.resources)
		{
			if (permitResource.IsOwnable() && permitResource.IsUnlocked())
			{
				return false;
			}
		}
		return true;
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

	private const string REDEEM_MYSTERY_BOX_URL = "https://accounts.klei.com/account/rewards?game=ONI";

	private const string LOCKER_MENU_MUSIC = "Music_SupplyCloset";

	private const string MUSIC_PARAMETER = "SupplyClosetView";

	private static bool shouldPreventDisplayingDropsAvailableNotification;
}
