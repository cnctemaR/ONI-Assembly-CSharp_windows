using System;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class ImmigrantScreen : CharacterSelectionController
{
	public static void DestroyInstance()
	{
		ImmigrantScreen.instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		this.activateOnSpawn = false;
		base.OnSpawn();
		base.IsStarterMinion = false;
		this.rejectButton.onClick += this.OnRejectAll;
		this.confirmRejectionBtn.onClick += this.OnRejectionConfirmed;
		this.cancelRejectionBtn.onClick += this.OnRejectionCancelled;
		ImmigrantScreen.instance = this;
		this.title.text = UI.IMMIGRANTSCREEN.IMMIGRANTSCREENTITLE;
		this.proceedButton.GetComponentInChildren<LocText>().text = UI.IMMIGRANTSCREEN.PROCEEDBUTTON;
		this.closeButton.onClick += delegate
		{
			base.Show(false);
		};
		base.Show(false);
	}

	protected override void OnShow(bool show)
	{
		if (show)
		{
			KFMOD.PlayOneShot(GlobalAssets.GetSound("Dialog_Popup", false));
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
			MusicManager.instance.PlaySong("Music_SelectDuplicant", false);
			this.hasShown = true;
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot, STOP_MODE.ALLOWFADEOUT);
			if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
			{
				MusicManager.instance.StopSong("Music_SelectDuplicant", true, STOP_MODE.ALLOWFADEOUT);
			}
			if (Immigration.Instance.ImmigrantsAvailable && this.hasShown)
			{
				AudioMixer.instance.Start(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
			}
		}
		base.OnShow(show);
	}

	public override void OnPressBack()
	{
		if (this.rejectConfirmationScreen.activeSelf)
		{
			this.OnRejectionCancelled();
		}
		else
		{
			base.OnPressBack();
		}
	}

	public override void Deactivate()
	{
		base.Show(false);
	}

	public static void InitializeImmigrantScreen(Telepad telepad)
	{
		ImmigrantScreen.instance.Initialize(telepad);
		ImmigrantScreen.instance.Show(true);
	}

	private void Initialize(Telepad telepad)
	{
		this.InitializeContainers();
		this.containers.ForEach(delegate(CharacterContainer c)
		{
			c.SetReshufflingState(false);
		});
		this.telepad = telepad;
	}

	protected override void OnProceed()
	{
		this.telepad.OnClickImmigrant(this.startingStats[0]);
		base.Show(false);
		this.containers.ForEach(delegate(CharacterContainer cc)
		{
			global::UnityEngine.Object.Destroy(cc.gameObject);
		});
		this.containers.Clear();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.StopSong("Music_SelectDuplicant", true, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.PlaySong("Stinger_NewDuplicant", false);
	}

	private void OnRejectAll()
	{
		this.rejectConfirmationScreen.transform.SetAsLastSibling();
		this.rejectConfirmationScreen.SetActive(true);
	}

	private void OnRejectionCancelled()
	{
		this.rejectConfirmationScreen.SetActive(false);
	}

	private void OnRejectionConfirmed()
	{
		this.telepad.RejectAll();
		this.containers.ForEach(delegate(CharacterContainer cc)
		{
			global::UnityEngine.Object.Destroy(cc.gameObject);
		});
		this.containers.Clear();
		this.rejectConfirmationScreen.SetActive(false);
		base.Show(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.StopSong("Music_SelectDuplicant", true, STOP_MODE.ALLOWFADEOUT);
	}

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton rejectButton;

	[SerializeField]
	private LocText title;

	[SerializeField]
	private GameObject rejectConfirmationScreen;

	[SerializeField]
	private KButton confirmRejectionBtn;

	[SerializeField]
	private KButton cancelRejectionBtn;

	private static ImmigrantScreen instance;

	private Telepad telepad;

	private bool hasShown;
}
