using System;
using System.Collections;
using FMOD.Studio;
using UnityEngine;

public static class LargeImpactorRevealSequence
{
	public static Coroutine Start(KMonoBehaviour controller, LargeImpactorSequenceUIReticle reticle, WorldContainer world)
	{
		return controller.StartCoroutine(LargeImpactorRevealSequence.Sequence(controller, reticle, world));
	}

	private static IEnumerator Sequence(KMonoBehaviour controller, LargeImpactorSequenceUIReticle reticle, WorldContainer world)
	{
		LargeImpactorCrashStamp component = controller.GetComponent<LargeImpactorCrashStamp>();
		LargeImpactorVisualizer visualizer = controller.GetComponent<LargeImpactorVisualizer>();
		ParallaxBackgroundObject parallaxBackgroundObject = controller.GetComponent<ParallaxBackgroundObject>();
		float scaleMin = parallaxBackgroundObject.scaleMin;
		parallaxBackgroundObject.scaleMin = 0f;
		int num = Grid.XYToCell(component.stampLocation.x, component.stampLocation.y);
		int num2 = Grid.FindMidSkyCellAlignedWithCellInWorld(num, world.id);
		Vector3 templatePosition = Grid.CellToPos(num);
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false, false);
		}
		CameraController.Instance.SetWorldInteractive(false);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot, STOP_MODE.ALLOWFADEOUT);
		ManagementMenu.Instance.CloseAll();
		StoryMessageScreen.HideInterface(true);
		OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID, false);
		CameraController.Instance.SetOverrideZoomSpeed(0.6f);
		CameraController.Instance.SetTargetPos(Grid.CellToPos(num2), 20f, false);
		yield return null;
		RootMenu.Instance.canTogglePauseScreen = false;
		CameraController.Instance.DisableUserCameraControl = true;
		MusicManager.instance.PlaySong("Stinger_LargeImpactor_Reveal", false);
		do
		{
			yield return 0;
			parallaxBackgroundObject.scaleMin += Time.unscaledDeltaTime * 0.04f;
		}
		while (parallaxBackgroundObject.scaleMin < 0.25f);
		bool reticleSequenceCompleted = false;
		bool reticlePhase1SequenceCompleted = false;
		reticle.Run(delegate
		{
			reticlePhase1SequenceCompleted = true;
		}, delegate
		{
			reticleSequenceCompleted = true;
			reticle.Hide();
		});
		yield return new WaitUntil(() => reticlePhase1SequenceCompleted);
		yield return SequenceUtil.WaitForSecondsRealtime(1f);
		float num3 = (float)(visualizer.RangeMax.x - visualizer.RangeMin.x) * 0.6f;
		CameraController.Instance.SetOverrideZoomSpeed(0.4f);
		CameraController.Instance.SetMaxOrthographicSize(num3);
		CameraController.Instance.SetTargetPos(templatePosition, num3, false);
		yield return SequenceUtil.WaitForSecondsRealtime(1f);
		visualizer.Active = true;
		visualizer.SetFoldedState(false);
		visualizer.BeginEntryEffect(3.5f);
		KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Imperative_calculating", false));
		yield return new WaitUntil(() => reticleSequenceCompleted);
		MusicManager.instance.StopSong("Stinger_LargeImpactor_Reveal", true, STOP_MODE.ALLOWFADEOUT);
		StoryMessageScreen.HideInterface(false);
		RootMenu.Instance.canTogglePauseScreen = true;
		CameraController.Instance.DisableUserCameraControl = false;
		CameraController.Instance.SetOverrideZoomSpeed(1f);
		CameraController.Instance.SetMaxOrthographicSize(20f);
		CameraController.Instance.SetWorldInteractive(true);
		HoverTextScreen.Instance.Show(true);
		RootMenu.Instance.canTogglePauseScreen = true;
		CameraController.Instance.SetTargetPos(templatePosition, 20f, true);
		controller.Trigger(-467702038, null);
		if (SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Unpause(false);
			SpeedControlScreen.Instance.SetSpeed(0);
		}
		parallaxBackgroundObject.scaleMin = scaleMin;
		yield break;
	}

	private const string SongName = "Stinger_LargeImpactor_Reveal";
}
