using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public static class LargeImpactorLandingSequence
{
	public static Coroutine Start(KMonoBehaviour controller, LargeComet comet, LargeImpactorCrashStamp stamp, int worldID)
	{
		return controller.StartCoroutine(LargeImpactorLandingSequence.Sequence(controller, comet, stamp, worldID));
	}

	private static IEnumerator Sequence(KMonoBehaviour controller, LargeComet comet, LargeImpactorCrashStamp stamp, int worldID)
	{
		yield return null;
		LargeImpactorVisualizer component = controller.GetComponent<LargeImpactorVisualizer>();
		Vector3 templatePosition = Grid.CellToPos(Grid.XYToCell(stamp.stampLocation.x, stamp.stampLocation.y));
		bool cometImpacted = false;
		LargeComet comet2 = comet;
		comet2.OnImpact = (global::System.Action)Delegate.Combine(comet2.OnImpact, new global::System.Action(delegate
		{
			cometImpacted = true;
		}));
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(false);
		}
		SpeedControlScreen.Instance.SetSpeed(0);
		RootMenu.Instance.canTogglePauseScreen = false;
		CameraController.Instance.DisableUserCameraControl = true;
		CameraController.Instance.SetWorldInteractive(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot, STOP_MODE.ALLOWFADEOUT);
		ManagementMenu.Instance.CloseAll();
		StoryMessageScreen.HideInterface(true);
		OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID, false);
		CameraController.Instance.SetOverrideZoomSpeed(0.6f);
		float templateWidth = (float)(component.RangeMax.x - component.RangeMin.x);
		float initialOrthogonalSize = templateWidth * 0.72f;
		float finalOrthogonalSize = templateWidth * 0.62f;
		yield return null;
		AudioMixer.instance.Start(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot);
		MusicManager.instance.PlaySong("Stinger_Demolior_Falling", false);
		EventInstance incomingSFXInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Asteroid_incoming_LP", false), Vector3.zero, 1f);
		incomingSFXInstance.start();
		CameraController.Instance.SetMaxOrthographicSize(finalOrthogonalSize);
		CameraController.Instance.SetTargetPos(comet.transform.position, initialOrthogonalSize, false);
		yield return new WaitUntil(delegate
		{
			float num4 = ((comet == null) ? 1f : comet.LandingProgress);
			Vector3 vector = ((comet == null) ? templatePosition : (Grid.IsValidCellInWorld(Grid.PosToCell(comet.VisualPosition), worldID) ? comet.VisualPosition : comet.transform.position));
			float num5 = Mathf.Lerp(initialOrthogonalSize, finalOrthogonalSize, (num4 <= 0f) ? 0f : Mathf.Pow(num4, 2f));
			CameraController.Instance.SetTargetPos(vector, num5, false);
			return cometImpacted;
		});
		incomingSFXInstance.stop(STOP_MODE.IMMEDIATE);
		incomingSFXInstance.release();
		KFMOD.PlayUISound(GlobalAssets.GetSound("Asteroid_explode", false));
		CameraController.Instance.FadeOutColor(Color.white, 1f, 1f, null);
		bool templateSpawned = false;
		TemplateLoader.Stamp(stamp.asteroidTemplate, stamp.stampLocation, delegate
		{
			templateSpawned = true;
		});
		List<WorldGenSpawner.Spawnable> unspawnedGeysers = new List<WorldGenSpawner.Spawnable>();
		foreach (WorldGenSpawner.Spawnable spawnable in SaveGame.Instance.worldGenSpawner.GeInfoOfUnspawnedWithType<Geyser>(worldID))
		{
			unspawnedGeysers.Add(spawnable);
		}
		yield return null;
		foreach (WorldGenSpawner.Spawnable spawnable2 in SaveGame.Instance.worldGenSpawner.GetSpawnablesWithTag("GeyserGeneric", worldID, false))
		{
			unspawnedGeysers.Add(spawnable2);
		}
		yield return null;
		yield return SequenceUtil.WaitForSecondsRealtime(1.8f);
		yield return new WaitUntil(() => templateSpawned);
		float num = templateWidth * 0.3f;
		CameraController.Instance.SetPosition(templatePosition);
		CameraController.Instance.OrthographicSize = num;
		float num2 = templateWidth * 0.68f;
		CameraController.Instance.SetOverrideZoomSpeed(0.1f);
		CameraController.Instance.SetTargetPos(templatePosition, num2, false);
		bool fadeOutCompleted = false;
		CameraController.Instance.FadeInColor(Color.white, 0f, 1f, delegate
		{
			fadeOutCompleted = true;
		});
		yield return new WaitUntil(() => fadeOutCompleted);
		yield return SequenceUtil.WaitForSecondsRealtime(8f);
		MusicManager.instance.StopSong("Stinger_Demolior_Falling", true, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Stop(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot, STOP_MODE.ALLOWFADEOUT);
		StoryMessageScreen.HideInterface(false);
		foreach (WorldGenSpawner.Spawnable spawnable3 in unspawnedGeysers)
		{
			Vector2I vector2I = Grid.CellToXY(Grid.OffsetCell(spawnable3.cell, 0, 2));
			GridVisibility.Reveal(vector2I.x, vector2I.y, 6, 1f);
			SimMessages.Dig(spawnable3.cell, -1, false, false);
		}
		yield return null;
		List<Geyser> geysers = Components.Geysers.GetItems(worldID);
		geysers.Sort(delegate(Geyser a, Geyser b)
		{
			float magnitude = (a.transform.position - templatePosition).magnitude;
			float magnitude2 = (b.transform.position - templatePosition).magnitude;
			return magnitude.CompareTo(magnitude2);
		});
		float geyserRevealTimer = 0f;
		int geyserCount = geysers.Count;
		Action<int, int> MakeGeyserRangeErupt = delegate(int from_notInclusive, int to)
		{
			for (int i = 0; i < geyserCount; i++)
			{
				if (i > from_notInclusive && i <= to)
				{
					Geyser geyser = geysers[i];
					LargeImpactorLandingSequence.UnentombGeyser(geyser);
					geyser.ShiftTimeTo(Geyser.TimeShiftStep.ActiveState, true);
					Game.Instance.SpawnFX(SpawnFXHashes.MeteorImpactMetal, new Vector3(geyser.transform.position.x, geyser.transform.position.y + 2f, geyser.transform.position.z - 0.1f), 0f);
					LargeImpactorLandingSequence.CreateGeyserEruptionNotification(geyser);
				}
			}
		};
		int lastGeyserIndexRevealed = -1;
		int num3;
		while (geyserRevealTimer < 8f)
		{
			num3 = Mathf.FloorToInt(Mathf.Pow(geyserRevealTimer / 8f, 4f) * (float)geyserCount);
			MakeGeyserRangeErupt(lastGeyserIndexRevealed, num3);
			lastGeyserIndexRevealed = num3;
			geyserRevealTimer += Time.deltaTime;
			yield return null;
		}
		num3 = geyserCount;
		if (lastGeyserIndexRevealed != num3)
		{
			MakeGeyserRangeErupt(lastGeyserIndexRevealed, num3);
		}
		yield return null;
		RootMenu.Instance.canTogglePauseScreen = true;
		CameraController.Instance.DisableUserCameraControl = false;
		CameraController.Instance.SetOverrideZoomSpeed(1f);
		CameraController.Instance.SetMaxOrthographicSize(20f);
		CameraController.Instance.SetWorldInteractive(true);
		HoverTextScreen.Instance.Show(true);
		RootMenu.Instance.canTogglePauseScreen = true;
		CameraController.Instance.SetTargetPos(templatePosition, 20f, true);
		controller.Trigger(-467702038, null);
		yield break;
	}

	private static void CreateGeyserEruptionNotification(Geyser geyser)
	{
		Vector3 pos = geyser.transform.GetPosition();
		Notifier notifier = geyser.gameObject.AddOrGet<Notifier>();
		Notification notification = new Notification(MISC.NOTIFICATIONS.LARGE_IMPACTOR_GEYSER_ERUPTION.NAME, NotificationType.Bad, (List<Notification> notificationList, object data) => MISC.NOTIFICATIONS.LARGE_IMPACTOR_GEYSER_ERUPTION.TOOLTIP + notificationList.ReduceMessages(false), "/t• " + notifier.GetProperName(), false, 0f, delegate(object o)
		{
			GameUtil.FocusCamera(pos, 2f, true, true);
		}, null, null, true, true, false);
		notifier.Add(notification, "");
	}

	private static void UnentombGeyser(Geyser geyser)
	{
		geyser.Unentomb();
		int num = Grid.PosToCell(geyser);
		Vector3 vector = Grid.CellToPos(num);
		int globalWorldSeed = SaveLoader.Instance.clusterDetailSave.globalWorldSeed;
		for (int i = -6; i < 6; i++)
		{
			for (int j = 0; j < 6; j++)
			{
				int num2 = Grid.OffsetCell(num, i, j);
				float magnitude = (Grid.CellToPos(num2) - vector).magnitude;
				float num3 = (float)new KRandom(globalWorldSeed + num2).Next() / 2.1474836E+09f;
				float num4 = Mathf.Clamp01(1f - (magnitude - 4f) / 2f);
				if ((magnitude < 4f || num3 <= 1f * num4) && Grid.IsSolidCell(num2) && !Grid.Foundation[num2] && Grid.Element[num2].id != SimHashes.Unobtanium)
				{
					SimMessages.Dig(num2, -1, false, false);
				}
			}
		}
	}

	private const string SongName = "Stinger_Demolior_Falling";

	private const string IncomingSFX = "Asteroid_incoming_LP";

	private const string ImpactSFX = "Asteroid_explode";
}
