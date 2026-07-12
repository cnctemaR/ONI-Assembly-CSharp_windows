using System;
using System.Collections;
using UnityEngine;

public static class StoryCompleteSequence
{
	public static void Start(StoryInstance story, MonoBehaviour coroutineRunner)
	{
		StoryCompleteSequence.cameraTargetCell = Grid.PosToCell(coroutineRunner.transform.position);
		if (!Grid.IsValidCell(StoryCompleteSequence.cameraTargetCell))
		{
			DebugUtil.DevLogError("Can not run complete sequence for Story " + story.storyId + "! Camera target has an invalid position!");
			return;
		}
		StoryCompleteSequence.story = story;
		coroutineRunner.StartCoroutine(StoryCompleteSequence.RunSequence());
	}

	public static IEnumerator RunSequence()
	{
		SaveGame.Instance.GetComponent<UserNavigation>();
		bool wasPaused = SpeedControlScreen.Instance.IsPaused;
		if (!wasPaused)
		{
			SpeedControlScreen.Instance.Pause(false, false);
		}
		PlayerController.Instance.CancelDragging();
		CameraController.Instance.SetWorldInteractive(false);
		CameraController.Instance.FadeOut(1f, 1f, null);
		yield return CameraController.Instance.activeFadeRoutine;
		ClusterManager.Instance.SetActiveWorld(StoryCompleteSequence.story.worldId);
		ManagementMenu.Instance.CloseAll();
		StoryCompleteData eventData = StoryCompleteSequence.story.completionData;
		Vector3 vector = Grid.CellToPosCCC(Grid.OffsetCell(StoryCompleteSequence.cameraTargetCell, eventData.CameraTargetOffset), Grid.SceneLayer.Ore);
		CameraController.Instance.SnapTo(vector, 6f);
		EventInfoScreen.ShowPopup(StoryCompleteSequence.story.eventInfo);
		CameraController.Instance.FadeIn(0f, 2f, null);
		yield return CameraController.Instance.activeFadeRoutine;
		Vector3 vector2 = Grid.CellToPosCCC(Grid.OffsetCell(StoryCompleteSequence.cameraTargetCell, eventData.KeepSakeSpawnOffset), Grid.SceneLayer.Ore);
		StoryManager.Instance.CompleteStoryEvent(StoryCompleteSequence.story.GetStory(), vector2);
		CameraController.Instance.SetWorldInteractive(true);
		if (SpeedControlScreen.Instance.IsPaused && !wasPaused)
		{
			SpeedControlScreen.Instance.Unpause(false);
		}
		if (StoryCompleteSequence.story.sequenceCompleteCallback != null)
		{
			StoryCompleteSequence.story.sequenceCompleteCallback(StoryCompleteSequence.story);
		}
		yield break;
	}

	private static int cameraTargetCell = -1;

	private static StoryInstance story = null;
}
