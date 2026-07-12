using System;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class AudioMixerSnapshots : ScriptableObject
{
	[ContextMenu("Reload")]
	public void ReloadSnapshots()
	{
		this.snapshotMap.Clear();
		EventReference[] array = this.snapshots;
		for (int i = 0; i < array.Length; i++)
		{
			string eventReferencePath = KFMOD.GetEventReferencePath(array[i]);
			if (!eventReferencePath.IsNullOrWhiteSpace())
			{
				this.snapshotMap.Add(eventReferencePath);
			}
		}
	}

	public static AudioMixerSnapshots Get()
	{
		if (AudioMixerSnapshots.instance == null)
		{
			AudioMixerSnapshots.instance = Resources.Load<AudioMixerSnapshots>("AudioMixerSnapshots");
		}
		return AudioMixerSnapshots.instance;
	}

	public EventReference TechFilterOnMigrated;

	public EventReference TechFilterLogicOn;

	public EventReference NightStartedMigrated;

	public EventReference MenuOpenMigrated;

	public EventReference MenuOpenHalfEffect;

	public EventReference SpeedPausedMigrated;

	public EventReference DuplicantCountAttenuatorMigrated;

	public EventReference NewBaseSetupSnapshot;

	public EventReference FrontEndSnapshot;

	public EventReference FrontEndWelcomeScreenSnapshot;

	public EventReference FrontEndWorldGenerationSnapshot;

	public EventReference IntroNIS;

	public EventReference PulseSnapshot;

	public EventReference ESCPauseSnapshot;

	public EventReference MENUNewDuplicantSnapshot;

	public EventReference UserVolumeSettingsSnapshot;

	public EventReference DuplicantCountMovingSnapshot;

	public EventReference DuplicantCountSleepingSnapshot;

	public EventReference PortalLPDimmedSnapshot;

	public EventReference DynamicMusicPlayingSnapshot;

	public EventReference FabricatorSideScreenOpenSnapshot;

	public EventReference SpaceVisibleSnapshot;

	public EventReference MENUStarmapSnapshot;

	public EventReference MENUStarmapNotPausedSnapshot;

	public EventReference GameNotFocusedSnapshot;

	public EventReference FacilityVisibleSnapshot;

	public EventReference TutorialVideoPlayingSnapshot;

	public EventReference VictoryMessageSnapshot;

	public EventReference VictoryNISGenericSnapshot;

	public EventReference VictoryNISRocketSnapshot;

	public EventReference VictoryCinematicSnapshot;

	public EventReference VictoryFadeToBlackSnapshot;

	public EventReference MuteDynamicMusicSnapshot;

	public EventReference ActiveBaseChangeSnapshot;

	public EventReference EventPopupSnapshot;

	public EventReference SmallRocketInteriorReverbSnapshot;

	public EventReference MediumRocketInteriorReverbSnapshot;

	public EventReference MainMenuVideoPlayingSnapshot;

	public EventReference TechFilterRadiationOn;

	public EventReference FrontEndSupplyClosetSnapshot;

	public EventReference FrontEndItemDropScreenSnapshot;

	[SerializeField]
	private EventReference[] snapshots;

	[NonSerialized]
	public List<string> snapshotMap = new List<string>();

	private static AudioMixerSnapshots instance;
}
