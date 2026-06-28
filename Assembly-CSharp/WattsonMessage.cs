using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

public class WattsonMessage : KScreen
{
	public override float GetSortKey()
	{
		return 8f;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		Game.Instance.Subscribe(-122303817, new Action<object>(this.OnNewBaseCreated));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.hideScreensWhileActive.Add(NotificationScreen.Instance);
		this.hideScreensWhileActive.Add(OverlayMenu.Instance);
		this.hideScreensWhileActive.Add(PlanScreen.Instance);
		this.hideScreensWhileActive.Add(ManagementMenu.Instance);
		this.hideScreensWhileActive.Add(ToolMenu.Instance);
		this.hideScreensWhileActive.Add(ToolMenuPriorityScreen.Instance);
		this.hideScreensWhileActive.Add(ResourceCategoryScreen.Instance);
		this.hideScreensWhileActive.Add(TopLeftControlScreen.Instance);
		this.hideScreensWhileActive.Add(global::DateTime.Instance);
		this.hideScreensWhileActive.Add(BuildWatermark.Instance);
		foreach (KScreen kscreen in this.hideScreensWhileActive)
		{
			kscreen.Show(false);
		}
	}

	public void Update()
	{
		if (!this.startFade)
		{
			return;
		}
		Color color = this.bg.color;
		color.a -= 0.01f;
		if (color.a <= 0f)
		{
			color.a = 0f;
		}
		this.bg.color = color;
	}

	protected override void OnActivate()
	{
		global::Debug.Log("WattsonMessage OnActivate", null);
		base.OnActivate();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NewBaseSetupSnapshot, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().IntroNIS);
		AudioMixer.instance.activeNIS = true;
		this.button.onClick += delegate
		{
			this.Deactivate();
		};
		this.dialog.GetComponent<KScreen>().Show(false);
		this.startFade = false;
		GameObject telepad = GameUtil.GetTelepad();
		KAnimControllerBase kac = telepad.GetComponent<KAnimControllerBase>();
		kac.Play(WattsonMessage.WorkLoopAnims, KAnim.PlayMode.Loop);
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			int idx = i + 1;
			MinionIdentity minionIdentity = Components.LiveMinionIdentities[i];
			minionIdentity.gameObject.transform.position = new Vector3(telepad.transform.position.x + (float)idx - 1.5f, telepad.transform.position.y, minionIdentity.gameObject.transform.position.z);
			GameObject gameObject = minionIdentity.gameObject;
			ChoreProvider chore_provider = gameObject.GetComponent<ChoreProvider>();
			EmoteChore chorePre = new EmoteChore(chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new HashedString[] { "portalbirth_pre_" + idx }, KAnim.PlayMode.Loop);
			UIScheduler.Instance.Schedule("DupeBirth", (float)idx * 0.5f, delegate(object data)
			{
				chorePre.Cancel("Done looping");
				new EmoteChore(chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new HashedString[] { "portalbirth_" + idx }, null);
			}, null, null);
		}
		CameraController.Instance.DisableUserCameraControl = true;
		this.scheduleHandles.Add(UIScheduler.Instance.Schedule("GoHome", 0.1f, delegate(object data)
		{
			CameraController.Instance.CameraGoHome(1f);
			this.startFade = true;
			MusicManager.instance.PlaySong("Music_WattsonMessage", false);
		}, null, null));
		UIScheduler.Instance.Schedule("Welcome", 4.6f, delegate(object data)
		{
			kac.Play(new HashedString[] { "working_pst", "idle" }, KAnim.PlayMode.Once);
		}, null, null);
		this.scheduleHandles.Add(UIScheduler.Instance.Schedule("WelcomeDialog", 5.6f, delegate(object d)
		{
			SpeedControlScreen.Instance.Pause(false);
			KFMOD.PlayOneShot(this.dialogSound);
			this.dialog.GetComponent<KScreen>().Activate();
			this.dialog.GetComponent<KScreen>().SetShouldFadeIn(true);
			this.dialog.GetComponent<KScreen>().Show(true);
		}, null, null));
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().IntroNIS, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.StartPersistentSnapshots();
		MusicManager.instance.StopSong("Music_WattsonMessage", true, STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.PlayDynamicMusic();
		AudioMixer.instance.activeNIS = false;
		DemoTimer.Instance.CountdownActive = true;
		SpeedControlScreen.Instance.Unpause(false);
		CameraController.Instance.DisableUserCameraControl = false;
		foreach (SchedulerHandle schedulerHandle in this.scheduleHandles)
		{
			schedulerHandle.ClearScheduler();
		}
		UIScheduler.Instance.Schedule("fadeInUI", 0.5f, delegate(object d)
		{
			GameScheduler.Instance.Schedule("BasicTutorial", 1.5f, delegate(object data)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Basics);
			}, null, null);
			GameScheduler.Instance.Schedule("WelcomeTutorial", 2f, delegate(object data)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Welcome);
			}, null, null);
			foreach (KScreen kscreen in this.hideScreensWhileActive)
			{
				kscreen.SetShouldFadeIn(true);
				kscreen.Show(true);
			}
			CameraController.Instance.SetMaxOrthographicSize(20f);
		}, null, null);
		Game.Instance.SetGameStarted();
		if (TopLeftControlScreen.Instance != null)
		{
			TopLeftControlScreen.Instance.RefreshName();
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape))
		{
			CameraController.Instance.CameraGoHome(2f);
			this.Deactivate();
		}
		e.Consumed = true;
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		e.Consumed = true;
	}

	private void OnNewBaseCreated(object data)
	{
		base.gameObject.SetActive(true);
	}

	private const float STARTTIME = 0.1f;

	private const float ENDTIME = 4.6f;

	private const float ALPHA_SPEED = 0.01f;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private GameObject dialog;

	[SerializeField]
	private KButton button;

	[SerializeField]
	[EventRef]
	private string dialogSound;

	private List<KScreen> hideScreensWhileActive = new List<KScreen>();

	private bool startFade;

	private List<SchedulerHandle> scheduleHandles = new List<SchedulerHandle>();

	private static readonly HashedString[] WorkLoopAnims = new HashedString[] { "working_pre", "working_loop" };
}
