using System;
using System.Collections;
using FMOD.Studio;
using UnityEngine;

public static class FindingMinnowCompleteSequence
{
	public static void Start(KMonoBehaviour controller)
	{
		controller.StartCoroutine(FindingMinnowCompleteSequence.Sequence());
	}

	private static IEnumerator Sequence()
	{
		bool videoCompleted = false;
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		VideoScreen screen = null;
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false, false);
		}
		CameraController.Instance.FadeOut(1f, 1f, null);
		yield return SequenceUtil.WaitForSecondsRealtime(2f);
		screen = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.VideoScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay).GetComponent<VideoScreen>();
		screen.PlayVictoryLoop(Db.Get().ColonyAchievements.MinnowRecruited.messageBody, Db.Get().ColonyAchievements.MinnowRecruited.Id, Db.Get().ColonyAchievements.MinnowRecruited.loopVideoName, true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot, true);
		global::System.Action onVideoCompletedCallback = delegate
		{
			videoCompleted = true;
		};
		VideoScreen videoScreen = screen;
		videoScreen.OnStop = (global::System.Action)Delegate.Combine(videoScreen.OnStop, onVideoCompletedCallback);
		yield return new WaitUntil(() => videoCompleted);
		VideoScreen videoScreen2 = screen;
		videoScreen2.OnStop = (global::System.Action)Delegate.Remove(videoScreen2.OnStop, onVideoCompletedCallback);
		SpeedControlScreen.Instance.SetSpeed(0);
		CameraController.Instance.FadeIn(0f, 1f, null);
		CameraController.Instance.SetOverrideZoomSpeed(1f);
		CameraController.Instance.SetWorldInteractive(true);
		CameraController.Instance.DisableUserCameraControl = false;
		CameraController.Instance.SetMaxOrthographicSize(20f);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryCinematicSnapshot, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot, STOP_MODE.ALLOWFADEOUT);
		RootMenu.Instance.canTogglePauseScreen = true;
		HoverTextScreen.Instance.Show(true);
		StoryMessageScreen.HideInterface(false);
		yield break;
	}
}
