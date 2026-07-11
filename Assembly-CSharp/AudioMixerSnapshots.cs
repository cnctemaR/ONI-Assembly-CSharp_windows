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
	public string TechFilterLogicOn;

	[EventRef]
	public string NightStartedMigrated;

	[EventRef]
	public string MenuOpenMigrated;

	[EventRef]
	public string MenuOpenHalfEffect;

	[EventRef]
	public string SpeedPausedMigrated;

	[EventRef]
	public string DuplicantCountAttenuatorMigrated;

	[EventRef]
	public string NewBaseSetupSnapshot;

	[EventRef]
	public string FrontEndSnapshot;

	[EventRef]
	public string FrontEndWelcomeScreenSnapshot;

	[EventRef]
	public string FrontEndWorldGenerationSnapshot;

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

	[EventRef]
	public string PortalLPDimmedSnapshot;

	[EventRef]
	public string DynamicMusicPlayingSnapshot;

	[EventRef]
	public string FabricatorSideScreenOpenSnapshot;

	[EventRef]
	public string SpaceVisibleSnapshot;

	[EventRef]
	public string MENUStarmapSnapshot;

	[EventRef]
	public string GameNotFocusedSnapshot;

	[EventRef]
	public string FacilityVisibleSnapshot;

	[SerializeField]
	[EventRef]
	private string[] snapshots;

	[NonSerialized]
	public List<string> snapshotMap = new List<string>();

	public static AudioMixerSnapshots instance;
}
