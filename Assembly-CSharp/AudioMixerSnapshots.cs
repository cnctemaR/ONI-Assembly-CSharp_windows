using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AudioMixerSnapshots : ScriptableObject
{
	public static AudioMixerSnapshots Get()
	{
		if (AudioMixerSnapshots.instance == null)
		{
			AudioMixerSnapshots.instance = Resources.Load<AudioMixerSnapshots>("AudioMixerSnapshots");
		}
		return AudioMixerSnapshots.instance;
	}

	[ContextMenu("Reload")]
	public void ReloadSnapshots()
	{
		this.snapshotMap.Clear();
		foreach (string text in this.snapshots)
		{
			this.snapshotMap.Add(text);
		}
	}

	[EventRef]
	public string TechFilterOnMigrated;

	[EventRef]
	public string NightStartedMigrated;

	[EventRef]
	public string MenuOpenMigrated;

	[EventRef]
	public string SpeedPausedMigrated;

	[EventRef]
	public string DuplicantCountAttenuatorMigrated;

	[EventRef]
	public string NewBaseSetupSnapshot;

	[EventRef]
	public string FrontEndSnapshot;

	[EventRef]
	public string IntroNIS;

	[EventRef]
	public string PulseSnapshot;

	[EventRef]
	public string ESCPauseSnapshot;

	[EventRef]
	public string MENUNewDuplicantSnapshot;

	[EventRef]
	public string UserVolumeSettingsSnapshot;

	[EventRef]
	public string DuplicantCountMovingSnapshot;

	[EventRef]
	public string DuplicantCountSleepingSnapshot;

	[SerializeField]
	[EventRef]
	private string[] snapshots;

	public List<string> snapshotMap = new List<string>();

	public static AudioMixerSnapshots instance;
}
