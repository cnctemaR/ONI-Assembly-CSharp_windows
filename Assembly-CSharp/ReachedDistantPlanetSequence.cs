using System;
using System.Collections;
using FMOD.Studio;
using UnityEngine;

public static class ReachedDistantPlanetSequence
{
	public static void Start(KMonoBehaviour controller)
	{
		controller.StartCoroutine(ReachedDistantPlanetSequence.Sequence());
	}

	private static IEnumerator Sequence()
	{
		Vector3 cameraTagetMid = Vector3.zero;
		Vector3 cameraTargetTop = Vector3.zero;
		Spacecraft spacecraft = null;
		foreach (Spacecraft spacecraft2 in SpacecraftManager.instance.GetSpacecraft())
		{
			if (spacecraft2.state != Spacecraft.MissionState.Grounded && SpacecraftManager.instance.GetSpacecraftDestination(spacecraft2.id).GetDestinationType().Id == Db.Get().SpaceDestinationTypes.Wormhole.Id)
			{
				spacecraft = spacecraft2;
				foreach (RocketModule rocketModule in spacecraft2.launchConditions.rocketModules)
				{
					if (rocketModule.GetComponent<RocketEngine>() != null)
					{
						cameraTagetMid = rocketModule.gameObject.transform.position + Vector3.up * 7f;
						break;
					}
				}
				cameraTargetTop = cameraTagetMid + Vector3.up * 20f;
			}
		}
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		CameraController.Instance.SetWorldInteractive(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot, STOP_MODE.ALLOWFADEOUT);
		CameraController.Instance.FadeOut(1f, 1f);
		yield return new WaitForSecondsRealtime(3f);
		CameraController.Instance.SetTargetPos(cameraTagetMid, 15f, false);
		CameraController.Instance.SetOverrideZoomSpeed(5f);
		yield return new WaitForSecondsRealtime(1f);
		AudioMixer.instance.Start(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot);
		CameraController.Instance.FadeIn(0f, 1f);
		MusicManager.instance.PlaySong("Music_Victory_02_NIS", false);
		foreach (object obj in Components.LiveMinionIdentities)
		{
			MinionIdentity minionIdentity = (MinionIdentity)obj;
			if (minionIdentity != null)
			{
				minionIdentity.GetComponent<Facing>().Face(cameraTagetMid.x);
				new EmoteChore(minionIdentity.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[] { "cheer_pre", "cheer_loop", "cheer_pst", "cheer_pre", "cheer_loop", "cheer_pst" }, null);
				new EmoteChore(minionIdentity.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[] { "cheer_pre", "cheer_loop", "cheer_pst", "cheer_pre", "cheer_loop", "cheer_pst" }, null);
				new EmoteChore(minionIdentity.GetComponent<ChoreProvider>(), Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim", new HashedString[] { "cheer_pre", "cheer_loop", "cheer_pst", "cheer_pre", "cheer_loop", "cheer_pst" }, null);
			}
		}
		yield return new WaitForSecondsRealtime(0.5f);
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(false);
		}
		SpeedControlScreen.Instance.SetSpeed(1);
		CameraController.Instance.SetOverrideZoomSpeed(0.01f);
		CameraController.Instance.SetTargetPos(cameraTargetTop, 35f, false);
		float baseZoomSpeed = 0.03f;
		int num;
		for (int i = 0; i < 10; i = num + 1)
		{
			yield return new WaitForSecondsRealtime(0.5f);
			CameraController.Instance.SetOverrideZoomSpeed(baseZoomSpeed + (float)i * 0.006f);
			num = i;
		}
		yield return new WaitForSecondsRealtime(6f);
		CameraController.Instance.FadeOut(1f, 1f);
		MusicManager.instance.StopSong("Music_Victory_02_NIS", true, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Stop(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot, STOP_MODE.ALLOWFADEOUT);
		yield return new WaitForSecondsRealtime(2f);
		spacecraft.TemporallyTear();
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false);
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
	}
}
