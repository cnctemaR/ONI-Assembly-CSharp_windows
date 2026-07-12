using System;
using System.Collections;
using FMOD.Studio;
using UnityEngine;

public static class EnterTemporalTearSequence
{
	public static void Start(KMonoBehaviour controller)
	{
		controller.StartCoroutine(EnterTemporalTearSequence.Sequence());
	}

	private static IEnumerator Sequence()
	{
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false, false);
		}
		CameraController.Instance.SetWorldInteractive(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot, STOP_MODE.ALLOWFADEOUT);
		CameraController.Instance.FadeOut(1f, 1f);
		yield return new WaitForSecondsRealtime(3f);
		ManagementMenu.Instance.CloseAll();
		AudioMixer.instance.Start(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot);
		MusicManager.instance.PlaySong("Music_Victory_02_NIS", false);
		Vector3 cameraBiasUp = Vector3.up * 5f;
		GameObject cameraTaget = EnterTemporalTearSequence.tearOpenerGameObject;
		if (cameraTaget != null)
		{
			CameraController.Instance.SetTargetPos(cameraTaget.transform.position + cameraBiasUp, 10f, false);
			CameraController.Instance.SetOverrideZoomSpeed(10f);
			yield return new WaitForSecondsRealtime(0.4f);
			if (SpeedControlScreen.Instance.IsPaused)
			{
				SpeedControlScreen.Instance.Unpause(false);
			}
			SpeedControlScreen.Instance.SetSpeed(1);
			CameraController.Instance.SetOverrideZoomSpeed(0.1f);
			CameraController.Instance.SetTargetPos(cameraTaget.transform.position + cameraBiasUp, 20f, false);
			CameraController.Instance.FadeIn(0f, 2f);
			foreach (object obj in Components.LiveMinionIdentities)
			{
				MinionIdentity minionIdentity = (MinionIdentity)obj;
				if (minionIdentity != null)
				{
					minionIdentity.GetComponent<Facing>().Face(cameraTaget.transform.position.x);
					new EmoteChore(minionIdentity.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[] { "cheer_pre", "cheer_loop", "cheer_pst", "cheer_pre", "cheer_loop", "cheer_pst" }, null);
				}
			}
			yield return new WaitForSecondsRealtime(0.5f);
			yield return new WaitForSecondsRealtime(1.5f);
			CameraController.Instance.FadeOut(1f, 1f);
			yield return new WaitForSecondsRealtime(1.5f);
		}
		cameraTaget = null;
		cameraTaget = null;
		foreach (object obj2 in Components.Telepads)
		{
			Telepad telepad = (Telepad)obj2;
			if (telepad != null)
			{
				cameraTaget = telepad.gameObject;
				CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 10f, false);
				CameraController.Instance.SetOverrideZoomSpeed(10f);
				yield return new WaitForSecondsRealtime(0.4f);
				if (SpeedControlScreen.Instance.IsPaused)
				{
					SpeedControlScreen.Instance.Unpause(false);
				}
				SpeedControlScreen.Instance.SetSpeed(1);
				CameraController.Instance.SetOverrideZoomSpeed(0.05f);
				CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 20f, false);
				CameraController.Instance.FadeIn(0f, 2f);
				foreach (object obj3 in Components.LiveMinionIdentities)
				{
					MinionIdentity minionIdentity2 = (MinionIdentity)obj3;
					if (minionIdentity2 != null)
					{
						minionIdentity2.GetComponent<Facing>().Face(cameraTaget.transform.position.x);
						new EmoteChore(minionIdentity2.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[] { "cheer_pre", "cheer_loop", "cheer_pst", "cheer_pre", "cheer_loop", "cheer_pst" }, null);
					}
				}
				yield return new WaitForSecondsRealtime(0.5f);
				yield return new WaitForSecondsRealtime(1.5f);
				CameraController.Instance.FadeOut(1f, 1f);
				yield return new WaitForSecondsRealtime(1.5f);
			}
		}
		IEnumerator enumerator2 = null;
		cameraTaget = null;
		MusicManager.instance.StopSong("Music_Victory_02_NIS", true, STOP_MODE.ALLOWFADEOUT);
		yield return new WaitForSecondsRealtime(2f);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false, false);
		}
		VideoScreen component = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.VideoScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay).GetComponent<VideoScreen>();
		component.PlayVideo(Assets.GetVideo(Db.Get().ColonyAchievements.ReachedDistantPlanet.shortVideoName), true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot, false);
		component.QueueVictoryVideoLoop(true, Db.Get().ColonyAchievements.ReachedDistantPlanet.messageBody, Db.Get().ColonyAchievements.ReachedDistantPlanet.Id, Db.Get().ColonyAchievements.ReachedDistantPlanet.loopVideoName);
		component.OnStop = (global::System.Action)Delegate.Combine(component.OnStop, new global::System.Action(delegate
		{
			StoryMessageScreen.HideInterface(false);
			CameraController.Instance.FadeIn(0f, 1f);
			CameraController.Instance.SetWorldInteractive(true);
			HoverTextScreen.Instance.Show(true);
			CameraController.Instance.SetOverrideZoomSpeed(1f);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryCinematicSnapshot, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot, STOP_MODE.ALLOWFADEOUT);
			RootMenu.Instance.canTogglePauseScreen = true;
		}));
		yield break;
		yield break;
	}

	public static GameObject tearOpenerGameObject;
}
