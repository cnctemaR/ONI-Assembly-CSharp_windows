using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.ConsumeMouseScroll = true;
		this.closeButton.onClick += delegate
		{
			this.Stop();
		};
		this.proceedButton.onClick += delegate
		{
			this.Stop();
		};
		this.videoPlayer.isLooping = false;
		this.videoPlayer.loopPointReached += delegate(VideoPlayer data)
		{
			if (this.victoryLoopQueued)
			{
				base.StartCoroutine(this.SwitchToVictoryLoop());
				return;
			}
			if (!this.videoPlayer.isLooping)
			{
				this.Stop();
			}
		};
		VideoScreen.Instance = this;
		this.Show(false);
	}

	protected override void OnForcedCleanUp()
	{
		VideoScreen.Instance = null;
		base.OnForcedCleanUp();
	}

	protected override void OnShow(bool show)
	{
		base.transform.SetAsLastSibling();
		base.OnShow(show);
		this.screen = this.videoPlayer.gameObject.GetComponent<RawImage>();
	}

	public void DisableAllMedia()
	{
		this.overlayContainer.gameObject.SetActive(false);
		this.videoPlayer.gameObject.SetActive(false);
		this.slideshow.gameObject.SetActive(false);
	}

	public void PlaySlideShow(Sprite[] sprites)
	{
		this.Show(true);
		this.DisableAllMedia();
		this.slideshow.updateType = SlideshowUpdateType.preloadedSprites;
		this.slideshow.gameObject.SetActive(true);
		this.slideshow.SetSprites(sprites);
		this.slideshow.SetPaused(false);
	}

	public void PlaySlideShow(string[] files)
	{
		this.Show(true);
		this.DisableAllMedia();
		this.slideshow.updateType = SlideshowUpdateType.loadOnDemand;
		this.slideshow.gameObject.SetActive(true);
		this.slideshow.SetFiles(files, 0);
		this.slideshow.SetPaused(false);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.IsAction(global::Action.Escape))
		{
			if (this.slideshow.gameObject.activeSelf && e.TryConsume(global::Action.Escape))
			{
				this.Stop();
				return;
			}
			if (e.TryConsume(global::Action.Escape))
			{
				if (this.videoSkippable)
				{
					this.Stop();
				}
				return;
			}
		}
		base.OnKeyDown(e);
	}

	public void PlayVideo(VideoClip clip, bool unskippable = false, EventReference overrideAudioSnapshot = default(EventReference), bool showProceedButton = false, bool syncAudio = true)
	{
		global::Debug.Assert(clip != null);
		for (int i = 0; i < this.overlayContainer.childCount; i++)
		{
			global::UnityEngine.Object.Destroy(this.overlayContainer.GetChild(i).gameObject);
		}
		this.Show(true);
		this.videoPlayer.isLooping = false;
		this.activeAudioSnapshot = (overrideAudioSnapshot.IsNull ? AudioMixerSnapshots.Get().TutorialVideoPlayingSnapshot : overrideAudioSnapshot);
		AudioMixer.instance.Start(this.activeAudioSnapshot);
		this.DisableAllMedia();
		this.videoPlayer.gameObject.SetActive(true);
		this.renderTexture = new RenderTexture(Convert.ToInt32(clip.width), Convert.ToInt32(clip.height), 16);
		this.screen.texture = this.renderTexture;
		this.videoPlayer.targetTexture = this.renderTexture;
		this.videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
		this.videoPlayer.clip = clip;
		this.videoPlayer.timeReference = (syncAudio ? VideoTimeReference.ExternalTime : VideoTimeReference.Freerun);
		this.videoPlayer.Play();
		if (this.audioHandle.isValid())
		{
			KFMOD.EndOneShot(this.audioHandle);
			this.audioHandle.clearHandle();
		}
		this.audioHandle = KFMOD.BeginOneShot(GlobalAssets.GetSound("vid_" + clip.name, false), Vector3.zero, 1f);
		KFMOD.EndOneShot(this.audioHandle);
		this.videoSkippable = !unskippable;
		this.closeButton.gameObject.SetActive(this.videoSkippable);
		this.proceedButton.gameObject.SetActive(showProceedButton && this.videoSkippable);
	}

	public void QueueVictoryVideoLoop(bool queue, string message = "", string victoryAchievement = "", string loopVideo = "", bool showAchievements = true, bool syncAudio = false)
	{
		this.victoryLoopQueued = queue;
		this.victoryLoopMessage = message;
		this.victoryLoopClip = loopVideo;
		this.victoryLoopSyncAudio = syncAudio;
		this.OnStop = (global::System.Action)Delegate.Combine(this.OnStop, new global::System.Action(delegate
		{
			if (showAchievements)
			{
				RetireColonyUtility.SaveColonySummaryData();
				MainMenu.ActivateRetiredColoniesScreenFromData(this.transform.parent.gameObject, RetireColonyUtility.GetCurrentColonyRetiredColonyData());
			}
		}));
	}

	public void SetOverlayText(string overlayTemplate, List<string> strings)
	{
		VideoOverlay videoOverlay = null;
		foreach (VideoOverlay videoOverlay2 in this.overlayPrefabs)
		{
			if (videoOverlay2.name == overlayTemplate)
			{
				videoOverlay = videoOverlay2;
				break;
			}
		}
		DebugUtil.Assert(videoOverlay != null, "Could not find a template named ", overlayTemplate);
		global::Util.KInstantiateUI<VideoOverlay>(videoOverlay.gameObject, this.overlayContainer.gameObject, true).SetText(strings);
		this.overlayContainer.gameObject.SetActive(true);
	}

	private IEnumerator SwitchToVictoryLoop()
	{
		this.victoryLoopQueued = false;
		Color color = this.fadeOverlay.color;
		for (float i = 0f; i < 1f; i += Time.unscaledDeltaTime)
		{
			this.fadeOverlay.color = new Color(color.r, color.g, color.b, i);
			yield return SequenceUtil.WaitForNextFrame;
		}
		this.fadeOverlay.color = new Color(color.r, color.g, color.b, 1f);
		MusicManager.instance.PlaySong("Music_Victory_03_StoryAndSummary", false);
		MusicManager.instance.SetSongParameter("Music_Victory_03_StoryAndSummary", "songSection", 1f, true);
		this.closeButton.gameObject.SetActive(true);
		this.proceedButton.gameObject.SetActive(true);
		this.SetOverlayText("VictoryEnd", new List<string> { this.victoryLoopMessage });
		this.videoPlayer.clip = Assets.GetVideo(this.victoryLoopClip);
		this.videoPlayer.isLooping = true;
		this.videoPlayer.Play();
		this.proceedButton.gameObject.SetActive(true);
		this.videoPlayer.timeReference = (this.victoryLoopSyncAudio ? VideoTimeReference.ExternalTime : VideoTimeReference.Freerun);
		yield return SequenceUtil.WaitForSecondsRealtime(1f);
		for (float i = 1f; i >= 0f; i -= Time.unscaledDeltaTime)
		{
			this.fadeOverlay.color = new Color(color.r, color.g, color.b, i);
			yield return SequenceUtil.WaitForNextFrame;
		}
		this.fadeOverlay.color = new Color(color.r, color.g, color.b, 0f);
		yield break;
	}

	public void Stop()
	{
		this.videoPlayer.Stop();
		this.screen.texture = null;
		this.videoPlayer.targetTexture = null;
		if (!this.activeAudioSnapshot.IsNull)
		{
			AudioMixer.instance.Stop(this.activeAudioSnapshot, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.audioHandle.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
		if (this.OnStop != null)
		{
			this.OnStop();
		}
		this.Show(false);
	}

	public override void ScreenUpdate(bool topLevel)
	{
		base.ScreenUpdate(topLevel);
		if (this.audioHandle.isValid())
		{
			int num;
			this.audioHandle.getTimelinePosition(out num);
			this.videoPlayer.externalReferenceTime = (double)((float)num / 1000f);
		}
	}

	public static VideoScreen Instance;

	[SerializeField]
	private VideoPlayer videoPlayer;

	[SerializeField]
	private Slideshow slideshow;

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton proceedButton;

	[SerializeField]
	private RectTransform overlayContainer;

	[SerializeField]
	private List<VideoOverlay> overlayPrefabs;

	private RawImage screen;

	private RenderTexture renderTexture;

	private EventReference activeAudioSnapshot;

	[SerializeField]
	private Image fadeOverlay;

	private EventInstance audioHandle;

	private bool victoryLoopQueued;

	private string victoryLoopMessage = "";

	private string victoryLoopClip = "";

	private bool victoryLoopSyncAudio;

	private bool videoSkippable = true;

	public global::System.Action OnStop;
}
