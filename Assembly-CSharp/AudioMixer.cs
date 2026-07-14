using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioMixer
{
	public static AudioMixer instance
	{
		get
		{
			return AudioMixer._instance;
		}
	}

	public static AudioMixer Create()
	{
		AudioMixer._instance = new AudioMixer();
		AudioMixerSnapshots audioMixerSnapshots = AudioMixerSnapshots.Get();
		if (audioMixerSnapshots != null)
		{
			audioMixerSnapshots.ReloadSnapshots();
		}
		return AudioMixer._instance;
	}

	public static void Destroy()
	{
		AudioMixer._instance.StopAll(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
		AudioMixer._instance = null;
	}

	public EventInstance Start(EventReference event_ref)
	{
		RuntimeManager.GetEventDescription(event_ref.Guid);
		EventInstance eventInstance;
		if (!this.activeSnapshots.TryGetValue(event_ref.Guid, out eventInstance))
		{
			if (RuntimeManager.IsInitialized)
			{
				eventInstance = KFMOD.CreateInstance(event_ref);
				this.activeSnapshots[event_ref.Guid] = eventInstance;
				eventInstance.start();
				eventInstance.setParameterByName("snapshotActive", 1f, false);
			}
			else
			{
				eventInstance = default(EventInstance);
			}
		}
		return eventInstance;
	}

	public bool Stop(EventReference event_ref, global::FMOD.Studio.STOP_MODE stop_mode = global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
	{
		return this.Stop(event_ref.Guid, stop_mode);
	}

	public bool Stop(GUID event_guid, global::FMOD.Studio.STOP_MODE stop_mode = global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
	{
		bool flag = false;
		EventInstance eventInstance;
		if (this.activeSnapshots.TryGetValue(event_guid, out eventInstance))
		{
			eventInstance.setParameterByName("snapshotActive", 0f, false);
			eventInstance.stop(stop_mode);
			eventInstance.release();
			this.activeSnapshots.Remove(event_guid);
			flag = true;
		}
		return flag;
	}

	public void Reset()
	{
		this.StopAll(global::FMOD.Studio.STOP_MODE.IMMEDIATE);
	}

	public void StopAll(global::FMOD.Studio.STOP_MODE stop_mode = global::FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		List<GUID> list = new List<GUID>();
		GUID guid = AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot.Guid;
		foreach (KeyValuePair<GUID, EventInstance> keyValuePair in this.activeSnapshots)
		{
			if (keyValuePair.Key != guid)
			{
				list.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.Stop(list[i], stop_mode);
		}
	}

	public bool SnapshotIsActive(EventReference event_ref)
	{
		return this.SnapshotIsActive(event_ref.Guid);
	}

	public bool SnapshotIsActive(GUID guid)
	{
		return this.activeSnapshots.ContainsKey(guid);
	}

	public void SetSnapshotParameter(EventReference event_ref, string parameter_name, float parameter_value, bool shouldLog = true)
	{
		shouldLog = false;
		if (shouldLog)
		{
			this.Log(string.Format("Set Param {0}: {1}, {2}", this.GetSnapshotName(event_ref), parameter_name, parameter_value));
		}
		if (!this.SetSnapshotParameter(event_ref.Guid, parameter_name, parameter_value) && shouldLog)
		{
			this.Log(string.Concat(new string[]
			{
				"Tried to set [",
				parameter_name,
				"] to [",
				parameter_value.ToString(),
				"] but [",
				this.GetSnapshotName(event_ref),
				"] is not active."
			}));
		}
	}

	private bool SetSnapshotParameter(GUID guid, string parameter_name, float parameter_value)
	{
		EventInstance eventInstance;
		if (this.activeSnapshots.TryGetValue(guid, out eventInstance))
		{
			eventInstance.setParameterByName(parameter_name, parameter_value, false);
			return true;
		}
		return false;
	}

	public void StartPersistentSnapshots()
	{
		this.persistentSnapshotsActive = true;
		this.Start(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated);
		this.Start(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot);
		this.Start(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot);
		this.spaceVisibleInst = this.Start(AudioMixerSnapshots.Get().SpaceVisibleSnapshot);
		this.facilityVisibleInst = this.Start(AudioMixerSnapshots.Get().FacilityVisibleSnapshot);
		this.Start(AudioMixerSnapshots.Get().PulseSnapshot);
	}

	public void StopPersistentSnapshots()
	{
		this.persistentSnapshotsActive = false;
		this.Stop(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated, global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this.Stop(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot, global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this.Stop(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot, global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this.Stop(AudioMixerSnapshots.Get().SpaceVisibleSnapshot, global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this.Stop(AudioMixerSnapshots.Get().FacilityVisibleSnapshot, global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		this.Stop(AudioMixerSnapshots.Get().PulseSnapshot, global::FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	private string GetSnapshotName(EventReference event_ref)
	{
		string text;
		RuntimeManager.GetEventDescription(event_ref.Guid).getPath(out text);
		return text;
	}

	public void UpdatePersistentSnapshotParameters()
	{
		this.SetVisibleDuplicants();
		GUID guid = AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot.Guid;
		if (this.activeSnapshots.TryGetValue(guid, out this.duplicantCountMovingInst))
		{
			this.duplicantCountMovingInst.setParameterByName("duplicantCount", (float)Mathf.Max(0, this.visibleDupes["moving"] - AudioMixer.VISIBLE_DUPLICANTS_BEFORE_ATTENUATION), false);
		}
		GUID guid2 = AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot.Guid;
		if (this.activeSnapshots.TryGetValue(guid2, out this.duplicantCountSleepingInst))
		{
			this.duplicantCountSleepingInst.setParameterByName("duplicantCount", (float)Mathf.Max(0, this.visibleDupes["sleeping"] - AudioMixer.VISIBLE_DUPLICANTS_BEFORE_ATTENUATION), false);
		}
		GUID guid3 = AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated.Guid;
		if (this.activeSnapshots.TryGetValue(guid3, out this.duplicantCountInst))
		{
			this.duplicantCountInst.setParameterByName("duplicantCount", (float)Mathf.Max(0, this.visibleDupes["visible"] - AudioMixer.VISIBLE_DUPLICANTS_BEFORE_ATTENUATION), false);
		}
		GUID guid4 = AudioMixerSnapshots.Get().PulseSnapshot.Guid;
		if (this.activeSnapshots.TryGetValue(guid4, out this.pulseInst))
		{
			float num = AudioMixer.PULSE_SNAPSHOT_BPM / 60f;
			int speed = SpeedControlScreen.Instance.GetSpeed();
			if (speed == 1)
			{
				num /= 2f;
			}
			else if (speed == 2)
			{
				num /= 3f;
			}
			float num2 = Mathf.Abs(Mathf.Sin(Time.time * 3.1415927f * num));
			this.pulseInst.setParameterByName("Pulse", num2, false);
		}
	}

	public void UpdateSpaceVisibleSnapshot(float percent)
	{
		this.spaceVisibleInst.setParameterByName("spaceVisible", percent, false);
	}

	public void PauseSpaceVisibleSnapshot(bool pause)
	{
		this.spaceVisibleInst.setParameterByName("spaceVisible", 0f, true);
		this.spaceVisibleInst.setPaused(pause);
	}

	public void UpdateFacilityVisibleSnapshot(float percent)
	{
		this.facilityVisibleInst.setParameterByName("facilityVisible", percent, false);
	}

	private void SetVisibleDuplicants()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			Vector3 position = Components.LiveMinionIdentities[i].transform.GetPosition();
			if (CameraController.Instance.IsVisiblePos(position))
			{
				num++;
				Navigator component = Components.LiveMinionIdentities[i].GetComponent<Navigator>();
				if (component != null && component.IsMoving())
				{
					num2++;
				}
				else
				{
					StaminaMonitor.Instance smi = Components.LiveMinionIdentities[i].GetComponent<WorkerBase>().GetSMI<StaminaMonitor.Instance>();
					if (smi != null && smi.IsSleeping())
					{
						num3++;
					}
				}
			}
		}
		this.visibleDupes["visible"] = num;
		this.visibleDupes["moving"] = num2;
		this.visibleDupes["sleeping"] = num3;
	}

	public void StartUserVolumesSnapshot()
	{
		this.Start(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot);
		GUID guid = AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot.Guid;
		EventInstance eventInstance;
		if (this.activeSnapshots.TryGetValue(guid, out eventInstance))
		{
			EventDescription eventDescription;
			eventInstance.getDescription(out eventDescription);
			USER_PROPERTY user_PROPERTY;
			eventDescription.getUserProperty("buses", out user_PROPERTY);
			string text = user_PROPERTY.stringValue();
			char c = '-';
			string[] array = text.Split(c, StringSplitOptions.None);
			for (int i = 0; i < array.Length; i++)
			{
				float num = 1f;
				string text2 = "Volume_" + array[i];
				if (KPlayerPrefs.HasKey(text2))
				{
					num = KPlayerPrefs.GetFloat(text2);
				}
				AudioMixer.UserVolumeBus userVolumeBus = new AudioMixer.UserVolumeBus();
				userVolumeBus.busLevel = num;
				userVolumeBus.labelString = Strings.Get("STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.AUDIO_BUS_" + array[i].ToUpper());
				this.userVolumeSettings.Add(array[i], userVolumeBus);
				this.SetUserVolume(array[i], userVolumeBus.busLevel);
			}
		}
	}

	public void SetUserVolume(string bus, float value)
	{
		if (!this.userVolumeSettings.ContainsKey(bus))
		{
			global::Debug.LogError("The provided bus doesn't exist. Check yo'self fool!");
			return;
		}
		if (value > 1f)
		{
			value = 1f;
		}
		else if (value < 0f)
		{
			value = 0f;
		}
		this.userVolumeSettings[bus].busLevel = value;
		KPlayerPrefs.SetFloat("Volume_" + bus, value);
		GUID guid = AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot.Guid;
		EventInstance eventInstance;
		if (this.activeSnapshots.TryGetValue(guid, out eventInstance))
		{
			eventInstance.setParameterByName("userVolume_" + bus, this.userVolumeSettings[bus].busLevel, false);
		}
		if (bus == "Music")
		{
			this.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "userVolume_Music", value, true);
		}
	}

	private void Log(string s)
	{
	}

	private static AudioMixer _instance = null;

	private const string DUPLICANT_COUNT_ID = "duplicantCount";

	private const string PULSE_ID = "Pulse";

	private const string SNAPSHOT_ACTIVE_ID = "snapshotActive";

	private const string SPACE_VISIBLE_ID = "spaceVisible";

	private const string FACILITY_VISIBLE_ID = "facilityVisible";

	private const string FOCUS_BUS_PATH = "bus:/SFX/Focus";

	public Dictionary<GUID, EventInstance> activeSnapshots = new Dictionary<GUID, EventInstance>();

	public List<HashedString> SnapshotDebugLog = new List<HashedString>();

	public bool activeNIS;

	public static float LOW_PRIORITY_CUTOFF_DISTANCE = 10f;

	public static float PULSE_SNAPSHOT_BPM = 120f;

	public static int VISIBLE_DUPLICANTS_BEFORE_ATTENUATION = 2;

	private EventInstance duplicantCountInst;

	private EventInstance pulseInst;

	private EventInstance duplicantCountMovingInst;

	private EventInstance duplicantCountSleepingInst;

	private EventInstance spaceVisibleInst;

	private EventInstance facilityVisibleInst;

	private static readonly HashedString UserVolumeSettingsHash = new HashedString("event:/Snapshots/Mixing/Snapshot_UserVolumeSettings");

	public bool persistentSnapshotsActive;

	private Dictionary<string, int> visibleDupes = new Dictionary<string, int>();

	public Dictionary<string, AudioMixer.UserVolumeBus> userVolumeSettings = new Dictionary<string, AudioMixer.UserVolumeBus>();

	public class UserVolumeBus
	{
		public string labelString;

		public float busLevel;
	}
}
