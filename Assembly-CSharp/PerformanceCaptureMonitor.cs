using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Klei;
using Unity.Profiling;
using UnityEngine;

public class PerformanceCaptureMonitor
{
	public static void WritePerformanceCaptureData()
	{
		PerformanceCaptureMonitor.Data.SWAverageFrameTimeMs = (float)(PerformanceCaptureMonitor.captureTimer.Elapsed.TotalMilliseconds / (double)GenericGameSettings.instance.scriptedProfile.frameCount);
		PerformanceCaptureMonitor.Data.Revision = 704000U;
		PerformanceCaptureMonitor.Data.Branch = "release";
		PerformanceCaptureMonitor.Data.IsBaseGame = !DlcManager.IsExpansion1Active();
		PerformanceCaptureMonitor.Data.LoadedDlcs = DlcManager.GetActiveDLCIds();
		if (SaveLoader.Instance != null)
		{
			PerformanceCaptureMonitor.Data.ActiveDlcsInSave = SaveLoader.Instance.GameInfo.dlcIds.ToArray();
			PerformanceCaptureMonitor.Data.PerfMonAverageFrameTimeMs = SaveLoader.Instance.GetFrameTime() * 1000f;
		}
		if (Game.Instance != null)
		{
			PerformanceCaptureMonitor.Data.Cycle = GameUtil.GetCurrentCycle();
			PerformanceCaptureMonitor.Data.Brains = new List<PerformanceCaptureMonitor.PerformanceCaptureData.BrainInfo>();
			foreach (BrainScheduler.BrainGroup brainGroup in Game.BrainScheduler.debugGetBrainGroups())
			{
				PerformanceCaptureMonitor.Data.Brains.Add(new PerformanceCaptureMonitor.PerformanceCaptureData.BrainInfo
				{
					name = brainGroup.tag.ToString(),
					count = brainGroup.BrainCount
				});
			}
		}
		PerformanceCaptureMonitor.Data.Patch = "";
		PerformanceCaptureMonitor.Data.BuildTags = new List<string>();
		PerformanceCaptureMonitor.Data.BuildTags.Add("release");
		PerformanceCaptureMonitor.Data.BuildConfig = string.Join("_", PerformanceCaptureMonitor.Data.BuildTags);
		if (!PerformanceCaptureMonitor.Data.Patch.IsNullOrWhiteSpace())
		{
			PerformanceCaptureMonitor.Data.BuildTags.Add("PatchedBuild");
		}
		if (SpeedControlScreen.Instance != null)
		{
			PerformanceCaptureMonitor.Data.GameSpeed = SpeedControlScreen.Instance.GetSpeed() + 1;
		}
		PerformanceCaptureMonitor.Data.EndMemoryMegs = PerformanceCaptureMonitor.GetMemoryUsed();
		string text = JsonUtility.ToJson(PerformanceCaptureMonitor.Data);
		File.WriteAllText("PerformanceCaptureData.json", text);
		DebugUtil.LogArgs(new object[] { "Written PerformanceCaptureData.json" });
	}

	public static bool IsCapturingPerformance()
	{
		return !GenericGameSettings.instance.scriptedProfile.saveGame.IsNullOrWhiteSpace();
	}

	public static void Initialize()
	{
		if (PerformanceCaptureMonitor.IsCapturingPerformance())
		{
			Application.targetFrameRate = -1;
			QualitySettings.vSyncCount = 0;
			KProfilerBegin.OnStopCapture = (global::System.Action)Delegate.Combine(KProfilerBegin.OnStopCapture, new global::System.Action(PerformanceCaptureMonitor.WritePerformanceCaptureData));
			KProfilerBegin.OnStartCapture = (global::System.Action)Delegate.Combine(KProfilerBegin.OnStartCapture, new global::System.Action(PerformanceCaptureMonitor.StartCapture));
			PerformanceCaptureMonitor.systemUsedMemory = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory", 1, ProfilerRecorderOptions.Default);
		}
	}

	public static void StartCapture()
	{
		PerformanceCaptureMonitor.captureTimer.Restart();
	}

	public static void StartLoadingSave()
	{
		PerformanceCaptureMonitor.loadTimer.Restart();
	}

	public static IEnumerator FinishedLoadingSave()
	{
		yield return null;
		PerformanceCaptureMonitor.Data.SaveLoadMemoryMegs = PerformanceCaptureMonitor.GetMemoryUsed();
		PerformanceCaptureMonitor.loadTimer.Stop();
		PerformanceCaptureMonitor.Data.SaveLoadTimeSec = (float)PerformanceCaptureMonitor.loadTimer.Elapsed.TotalSeconds;
		yield break;
	}

	public static void TryRecordMainMenuStats()
	{
		if (PerformanceCaptureMonitor.Data.MainMenuMemoryMegs == 0f)
		{
			PerformanceCaptureMonitor.Data.MainMenuMemoryMegs = PerformanceCaptureMonitor.GetMemoryUsed();
			PerformanceCaptureMonitor.Data.MainMenuLoadTimeSec = Time.realtimeSinceStartup;
		}
	}

	public static float GetMemoryUsed()
	{
		if (!PerformanceCaptureMonitor.systemUsedMemory.Valid)
		{
			return 0f;
		}
		return (float)PerformanceCaptureMonitor.systemUsedMemory.CurrentValue / 1048576f;
	}

	public static PerformanceCaptureMonitor.PerformanceCaptureData Data = new PerformanceCaptureMonitor.PerformanceCaptureData();

	private static ProfilerRecorder systemUsedMemory;

	private static Stopwatch loadTimer = new Stopwatch();

	private static Stopwatch captureTimer = new Stopwatch();

	public class PerformanceCaptureData
	{
		public uint Revision;

		public string Patch;

		public string Branch;

		public bool IsDevelopmentBuild;

		public bool IsBaseGame;

		public string[] ActiveDlcsInSave;

		public List<string> LoadedDlcs;

		public List<string> BuildTags;

		public List<PerformanceCaptureMonitor.PerformanceCaptureData.BrainInfo> Brains;

		public int Cycle;

		public float MainMenuLoadTimeSec;

		public float MainMenuMemoryMegs;

		public float SaveLoadTimeSec;

		public float SaveLoadMemoryMegs;

		public float EndMemoryMegs;

		public float PerfMonAverageFrameTimeMs;

		public float SWAverageFrameTimeMs;

		public string BuildConfig;

		public int GameSpeed;

		[Serializable]
		public class BrainInfo
		{
			public string name;

			public int count;
		}
	}
}
