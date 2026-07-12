using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using STRINGS;
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
		string welcomeMessage = CustomGameSettings.Instance.GetCurrentClusterLayout().welcomeMessage;
		if (welcomeMessage != null)
		{
			StringEntry stringEntry;
			this.message.SetText(Strings.TryGet(welcomeMessage, out stringEntry) ? stringEntry.String : welcomeMessage);
			return;
		}
		if (DlcManager.IsExpansion1Active())
		{
			this.message.SetText(UI.WELCOMEMESSAGEBODY_SPACEDOUT);
			return;
		}
		this.message.SetText(UI.WELCOMEMESSAGEBODY);
	}

	private IEnumerator ExpandPanel()
	{
		this.button.isInteractable = false;
		if (CustomGameSettings.Instance.GetSettingsCoordinate().StartsWith("KF23"))
		{
			this.dialog.rectTransform().rotation = Quaternion.Euler(0f, 0f, -90f);
		}
		yield return SequenceUtil.WaitForSecondsRealtime(0.2f);
		float height = 0f;
		while (height < 299f)
		{
			height = Mathf.Lerp(this.dialog.rectTransform().sizeDelta.y, 300f, Time.unscaledDeltaTime * 15f);
			this.dialog.rectTransform().sizeDelta = new Vector2(this.dialog.rectTransform().sizeDelta.x, height);
			yield return 0;
		}
		if (CustomGameSettings.Instance.GetSettingsCoordinate().StartsWith("KF23"))
		{
			Quaternion initialOrientation = Quaternion.Euler(0f, 0f, -90f);
			yield return SequenceUtil.WaitForSecondsRealtime(1f);
			float t = 0f;
			float duration = 0.5f;
			while (t < duration)
			{
				t += Time.unscaledDeltaTime;
				this.dialog.rectTransform().rotation = Quaternion.Slerp(initialOrientation, Quaternion.identity, t / duration);
				yield return 0;
			}
			initialOrientation = default(Quaternion);
		}
		this.button.isInteractable = true;
		yield return null;
		yield break;
	}

	private IEnumerator CollapsePanel()
	{
		float height = 300f;
		while (height > 1f)
		{
			height = Mathf.Lerp(this.dialog.rectTransform().sizeDelta.y, 0f, Time.unscaledDeltaTime * 15f);
			this.dialog.rectTransform().sizeDelta = new Vector2(this.dialog.rectTransform().sizeDelta.x, height);
			yield return 0;
		}
		this.Deactivate();
		yield return null;
		yield break;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.hideScreensWhileActive.Add(NotificationScreen.Instance);
		this.hideScreensWhileActive.Add(OverlayMenu.Instance);
		if (PlanScreen.Instance != null)
		{
			this.hideScreensWhileActive.Add(PlanScreen.Instance);
		}
		if (BuildMenu.Instance != null)
		{
			this.hideScreensWhileActive.Add(BuildMenu.Instance);
		}
		this.hideScreensWhileActive.Add(ManagementMenu.Instance);
		this.hideScreensWhileActive.Add(ToolMenu.Instance);
		this.hideScreensWhileActive.Add(ToolMenu.Instance.PriorityScreen);
		this.hideScreensWhileActive.Add(PinnedResourcesPanel.Instance);
		this.hideScreensWhileActive.Add(TopLeftControlScreen.Instance);
		this.hideScreensWhileActive.Add(global::DateTime.Instance);
		this.hideScreensWhileActive.Add(BuildWatermark.Instance);
		this.hideScreensWhileActive.Add(BuildWatermark.Instance);
		this.hideScreensWhileActive.Add(ColonyDiagnosticScreen.Instance);
		if (WorldSelector.Instance != null)
		{
			this.hideScreensWhileActive.Add(WorldSelector.Instance);
		}
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
		global::Debug.Log("WattsonMessage OnActivate");
		base.OnActivate();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NewBaseSetupSnapshot, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().IntroNIS);
		AudioMixer.instance.activeNIS = true;
		this.button.onClick += delegate
		{
			base.StartCoroutine(this.CollapsePanel());
		};
		this.dialog.GetComponent<KScreen>().Show(false);
		this.startFade = false;
		GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
		if (telepad != null)
		{
			KAnimControllerBase kac = telepad.GetComponent<KAnimControllerBase>();
			kac.Play(WattsonMessage.WorkLoopAnims, KAnim.PlayMode.Loop);
			NameDisplayScreen.Instance.gameObject.SetActive(false);
			for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
			{
				int idx = i + 1;
				MinionIdentity minionIdentity = Components.LiveMinionIdentities[i];
				minionIdentity.gameObject.transform.SetPosition(new Vector3(telepad.transform.GetPosition().x + (float)idx - 1.5f, telepad.transform.GetPosition().y, minionIdentity.gameObject.transform.GetPosition().z));
				GameObject gameObject = minionIdentity.gameObject;
				ChoreProvider chore_provider = gameObject.GetComponent<ChoreProvider>();
				EmoteChore chorePre = new EmoteChore(chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new HashedString[] { "portalbirth_pre_" + idx.ToString() }, KAnim.PlayMode.Loop, false);
				UIScheduler.Instance.Schedule("DupeBirth", (float)idx * 0.5f, delegate(object data)
				{
					chorePre.Cancel("Done looping");
					EmoteChore emoteChore = new EmoteChore(chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, "anim_interacts_portal_kanim", new HashedString[] { "portalbirth_" + idx.ToString() }, null);
					emoteChore.onComplete = (Action<Chore>)Delegate.Combine(emoteChore.onComplete, new Action<Chore>(delegate(Chore param)
					{
						this.birthsComplete++;
						if (this.birthsComplete == Components.LiveMinionIdentities.Count - 1 && base.IsActive())
						{
							NameDisplayScreen.Instance.gameObject.SetActive(true);
							this.PauseAndShowMessage();
						}
					}));
				}, null, null);
			}
			UIScheduler.Instance.Schedule("Welcome", 6.6f, delegate(object data)
			{
				kac.Play(new HashedString[] { "working_pst", "idle" }, KAnim.PlayMode.Once);
			}, null, null);
			CameraController.Instance.DisableUserCameraControl = true;
		}
		else
		{
			global::Debug.LogWarning("Failed to spawn telepad - does the starting base template lack a 'Headquarters' ?");
			this.PauseAndShowMessage();
		}
		this.scheduleHandles.Add(UIScheduler.Instance.Schedule("GoHome", 0.1f, delegate(object data)
		{
			CameraController.Instance.OrthographicSize = TuningData<WattsonMessage.Tuning>.Get().initialOrthographicSize;
			CameraController.Instance.CameraGoHome(0.5f, false);
			this.startFade = true;
			MusicManager.instance.PlaySong(this.WelcomeMusic, false);
		}, null, null));
	}

	private string WelcomeMusic
	{
		get
		{
			string musicWelcome = CustomGameSettings.Instance.GetCurrentClusterLayout().clusterAudio.musicWelcome;
			if (!musicWelcome.IsNullOrWhiteSpace())
			{
				return musicWelcome;
			}
			return "Music_WattsonMessage";
		}
	}

	protected void PauseAndShowMessage()
	{
		SpeedControlScreen.Instance.Pause(false, false);
		base.StartCoroutine(this.ExpandPanel());
		KFMOD.PlayUISound(this.dialogSound);
		this.dialog.GetComponent<KScreen>().Activate();
		this.dialog.GetComponent<KScreen>().SetShouldFadeIn(true);
		this.dialog.GetComponent<KScreen>().Show(true);
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().IntroNIS, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.StartPersistentSnapshots();
		MusicManager.instance.StopSong(this.WelcomeMusic, true, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		MusicManager.instance.WattsonStartDynamicMusic();
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
			foreach (KScreen kscreen in this.hideScreensWhileActive)
			{
				if (!(kscreen == null))
				{
					kscreen.SetShouldFadeIn(true);
					kscreen.Show(true);
				}
			}
			CameraController.Instance.SetMaxOrthographicSize(20f);
			Game.Instance.StartDelayedInitialSave();
			UIScheduler.Instance.Schedule("InitialScreenshot", 1f, delegate(object data)
			{
				Game.Instance.timelapser.InitialScreenshot();
			}, null, null);
			GameScheduler.Instance.Schedule("BasicTutorial", 1.5f, delegate(object data)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Basics, true);
			}, null, null);
			GameScheduler.Instance.Schedule("WelcomeTutorial", 2f, delegate(object data)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Welcome, true);
			}, null, null);
			GameScheduler.Instance.Schedule("DiggingTutorial", 420f, delegate(object data)
			{
				Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Digging, true);
			}, null, null);
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
			CameraController.Instance.CameraGoHome(2f, false);
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

	private const float ENDTIME = 6.6f;

	private const float ALPHA_SPEED = 0.01f;

	private const float expandedHeight = 300f;

	[SerializeField]
	private GameObject dialog;

	[SerializeField]
	private RectTransform content;

	[SerializeField]
	private LocText message;

	[SerializeField]
	private Image bg;

	[SerializeField]
	private KButton button;

	[SerializeField]
	private EventReference dialogSound;

	private List<KScreen> hideScreensWhileActive = new List<KScreen>();

	private bool startFade;

	private List<SchedulerHandle> scheduleHandles = new List<SchedulerHandle>();

	private static readonly HashedString[] WorkLoopAnims = new HashedString[] { "working_pre", "working_loop" };

	private int birthsComplete;

	public class Tuning : TuningData<WattsonMessage.Tuning>
	{
		public float initialOrthographicSize;
	}
}
