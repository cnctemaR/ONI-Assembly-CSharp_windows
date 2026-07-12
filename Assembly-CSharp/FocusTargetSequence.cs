using System;
using System.Collections;
using UnityEngine;

public static class FocusTargetSequence
{
	public static void Start(MonoBehaviour coroutineRunner, FocusTargetSequence.Data sequenceData)
	{
		FocusTargetSequence.sequenceCoroutine = coroutineRunner.StartCoroutine(FocusTargetSequence.RunSequence(sequenceData));
	}

	public static void Cancel(MonoBehaviour coroutineRunner)
	{
		if (FocusTargetSequence.sequenceCoroutine == null)
		{
			return;
		}
		coroutineRunner.StopCoroutine(FocusTargetSequence.sequenceCoroutine);
		FocusTargetSequence.sequenceCoroutine = null;
	}

	public static IEnumerator RunSequence(FocusTargetSequence.Data sequenceData)
	{
		SaveGame.Instance.GetComponent<UserNavigation>();
		CameraController.Instance.FadeOut(1f, 1f, null);
		int speed = SpeedControlScreen.Instance.GetSpeed();
		SpeedControlScreen.Instance.SetSpeed(0);
		bool wasPaused = SpeedControlScreen.Instance.IsPaused;
		if (!wasPaused)
		{
			SpeedControlScreen.Instance.Pause(false, false);
		}
		PlayerController.Instance.CancelDragging();
		CameraController.Instance.SetWorldInteractive(false);
		yield return CameraController.Instance.activeFadeRoutine;
		KSelectable selectedObject = SelectTool.Instance.selected;
		SelectTool.Instance.Select(null, true);
		RootMenu.Instance.Show(false);
		ClusterManager.Instance.SetActiveWorld(sequenceData.WorldId);
		ManagementMenu.Instance.CloseAll();
		CameraController.Instance.SnapTo(sequenceData.Target, sequenceData.OrthographicSize);
		if (sequenceData.PopupData != null)
		{
			EventInfoScreen.ShowPopup(sequenceData.PopupData);
		}
		CameraController.Instance.FadeIn(0f, 2f, null);
		if (sequenceData.TargetSize - sequenceData.OrthographicSize > Mathf.Epsilon)
		{
			CameraController.Instance.StartCoroutine(CameraController.Instance.DoCinematicZoom(sequenceData.TargetSize));
		}
		if (sequenceData.CanCompleteCB != null)
		{
			SpeedControlScreen.Instance.Unpause(false);
			while (!sequenceData.CanCompleteCB())
			{
				yield return SequenceUtil.WaitForNextFrame;
			}
			SpeedControlScreen.Instance.Pause(false, false);
		}
		CameraController.Instance.SetWorldInteractive(true);
		SpeedControlScreen.Instance.SetSpeed(speed);
		if (SpeedControlScreen.Instance.IsPaused && !wasPaused)
		{
			SpeedControlScreen.Instance.Unpause(false);
		}
		if (sequenceData.CompleteCB != null)
		{
			sequenceData.CompleteCB();
		}
		RootMenu.Instance.Show(true);
		SelectTool.Instance.Select(selectedObject, true);
		sequenceData.Clear();
		FocusTargetSequence.sequenceCoroutine = null;
		yield break;
	}

	private static Coroutine sequenceCoroutine;

	public struct Data
	{
		public void Clear()
		{
			this.PopupData = null;
			this.CompleteCB = null;
			this.CanCompleteCB = null;
		}

		public int WorldId;

		public float OrthographicSize;

		public float TargetSize;

		public Vector3 Target;

		public EventInfoData PopupData;

		public global::System.Action CompleteCB;

		public Func<bool> CanCompleteCB;
	}
}
