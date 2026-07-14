using System;
using System.Collections;
using FMOD.Studio;
using UnityEngine;

public static class LargeImpactorDestroyedSequence
{
	public static Coroutine Start()
	{
		GameplayEventInstance gameplayEventInstance = GameplayEventManager.Instance.GetGameplayEventInstance(Db.Get().GameplayEvents.LargeImpactor.Id, -1);
		if (gameplayEventInstance != null)
		{
			LargeImpactorEvent.StatesInstance statesInstance = (LargeImpactorEvent.StatesInstance)gameplayEventInstance.smi;
			if (statesInstance != null && statesInstance.impactorInstance != null)
			{
				LargeImpactorCrashStamp component = statesInstance.impactorInstance.GetComponent<LargeImpactorCrashStamp>();
				return component.StartCoroutine(LargeImpactorDestroyedSequence.Sequence(component, statesInstance.eventInstance.worldId));
			}
		}
		return null;
	}

	private static IEnumerator Sequence(KMonoBehaviour controller, int worldID)
	{
		yield return null;
		WorldContainer world = ClusterManager.Instance.GetWorld(worldID);
		ParallaxBackgroundObject parallaxBackgroundObj = controller.GetComponent<ParallaxBackgroundObject>();
		GameObject telepad = GameUtil.GetTelepad(worldID);
		int centredCell = 0;
		if (telepad != null)
		{
			centredCell = Grid.PosToCell(telepad);
		}
		else
		{
			Vector2 vector = world.WorldOffset * Grid.CellSizeInMeters;
			vector.x += (float)world.Width * Grid.CellSizeInMeters * 0.5f;
			vector.y += (float)world.Height * Grid.CellSizeInMeters * 0.5f;
			centredCell = Grid.PosToCell(vector);
		}
		int num = Grid.XYToCell(Grid.CellToXY(centredCell).x, world.WorldOffset.y + world.Height);
		int num2 = centredCell;
		int midSkyCell = Grid.InvalidCell;
		int num3 = Grid.InvalidCell;
		while (num3 == Grid.InvalidCell && Grid.CellToXY(num2).y < world.WorldOffset.y + world.Height)
		{
			if (Grid.IsCellBiomeSpaceBiome(num2))
			{
				num3 = num2;
				break;
			}
			num2 = Grid.CellAbove(num2);
		}
		midSkyCell = Grid.XYToCell(Grid.CellToXY(centredCell).x, (int)((float)(Grid.CellToXY(num).y + Grid.CellToXY(num3).y) * 0.5f));
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false, false);
		}
		RootMenu.Instance.canTogglePauseScreen = false;
		CameraController.Instance.DisableUserCameraControl = true;
		CameraController.Instance.SetWorldInteractive(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot, STOP_MODE.ALLOWFADEOUT);
		ManagementMenu.Instance.CloseAll();
		StoryMessageScreen.HideInterface(true);
		OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID, false);
		CameraController.Instance.SetOverrideZoomSpeed(0.6f);
		yield return null;
		CameraController.Instance.FadeIn(0f, 1f, null);
		AudioMixer.instance.Start(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot);
		MusicManager.instance.PlaySong("Music_Victory_02_NIS", false);
		KFMOD.PlayUISound(GlobalAssets.GetSound("Asteroid_destroyed_start", false));
		CameraController.Instance.SetTargetPos(Grid.CellToPos(midSkyCell), 20f, false);
		yield return SequenceUtil.WaitForSecondsRealtime(4f);
		parallaxBackgroundObj.PlayExplosion();
		yield return SequenceUtil.WaitForSecondsRealtime(2.2f);
		TerrainBG.preventLargeImpactorFragmentsFromProgressing = false;
		bool fadeOutCompleted = false;
		CameraController.Instance.FadeOutColor(Color.white, 0f, 1f, 1f, delegate
		{
			fadeOutCompleted = true;
		});
		yield return new WaitUntil(() => fadeOutCompleted);
		MissileLauncher.Instance instance = null;
		float num4 = float.MaxValue;
		Vector3 position = CameraController.Instance.overlayCamera.transform.position;
		position.z = 0f;
		foreach (object obj in Components.MissileLaunchers)
		{
			MissileLauncher.Instance instance2 = (MissileLauncher.Instance)obj;
			if (instance2 != null && instance2.GetMyWorldId() == worldID)
			{
				Vector3 position2 = instance2.transform.position;
				position2.z = 0f;
				float magnitude = (position - position2).magnitude;
				if (magnitude < num4)
				{
					num4 = magnitude;
					instance = instance2;
				}
			}
		}
		int num5 = Grid.InvalidCell;
		int num6 = Grid.InvalidCell;
		bool flag = instance != null;
		if (flag)
		{
			num6 = Grid.PosToCell(instance.gameObject);
		}
		else
		{
			num5 = Grid.XYToCell(Grid.CellToXY(centredCell).x, world.WorldOffset.y + world.Height);
			num6 = num5;
		}
		if (flag)
		{
			int num7 = num6;
			int y = CameraController.Instance.VisibleArea.CurrentArea.Max.Y;
			while (Grid.CellToXY(num7).y < y)
			{
				int num8 = Grid.CellAbove(num7);
				if (!Grid.IsValidCellInWorld(num8, worldID) || Grid.Solid[num8])
				{
					break;
				}
				num7 = num8;
			}
			num5 = num7;
		}
		LargeImpactorDestroyedSequence.SpawnKeepsake(Grid.CellToPos(num5));
		yield return SequenceUtil.WaitForSecondsRealtime(2f);
		MusicManager.instance.StopSong("Music_Victory_02_NIS", true, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Stop(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot, STOP_MODE.ALLOWFADEOUT);
		yield return null;
		bool videoCompleted = false;
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
		VideoScreen screen = null;
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false, false);
		}
		screen = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.VideoScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay).GetComponent<VideoScreen>();
		screen.PlayShortWithVictoryLoop(Db.Get().ColonyAchievements.AsteroidDestroyed.shortVideoName, Db.Get().ColonyAchievements.AsteroidDestroyed.messageBody, Db.Get().ColonyAchievements.AsteroidDestroyed.Id, Db.Get().ColonyAchievements.AsteroidDestroyed.loopVideoName, true, AudioMixerSnapshots.Get().VictoryNISGenericSnapshot);
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
		Game.Instance.Subscribe(-821118536, new Action<object>(LargeImpactorDestroyedSequence.OnScreenClosed));
		controller.Trigger(-467702038, null);
		yield break;
	}

	private static void OnScreenClosed(object screenData)
	{
		if (screenData != null && screenData is RetiredColonyInfoScreen)
		{
			LargeImpactorDestroyedSequence.OnAchievementScreenClosed();
		}
	}

	private static void OnAchievementScreenClosed()
	{
		if (SpeedControlScreen.Instance != null && SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(false);
			SpeedControlScreen.Instance.SetSpeed(0);
		}
		Game.Instance.Unsubscribe(-821118536, new Action<object>(LargeImpactorDestroyedSequence.OnScreenClosed));
	}

	private static void SpawnKeepsake(Vector3 position)
	{
		GameObject prefab = Assets.GetPrefab("keepsake_largeimpactor");
		if (prefab != null)
		{
			position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			GameObject gameObject = global::Util.KInstantiate(prefab, position);
			gameObject.SetActive(true);
			new UpgradeFX.Instance(gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, -0.5f, -0.1f)).StartSM();
		}
	}

	private const string SongName = "Music_Victory_02_NIS";

	private const string Sound_Destroyed_Victory_Start_Sequence = "Asteroid_destroyed_start";
}
