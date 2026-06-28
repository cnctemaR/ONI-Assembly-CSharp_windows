using System;
using System.Collections.Generic;
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
		AudioMixer._instance.StopAll(STOP_MODE.IMMEDIATE);
		AudioMixer._instance = null;
	}

	public EventInstance Start(string snapshot)
	{
		EventInstance eventInstance = null;
		if (!this.activeSnapshots.TryGetValue(snapshot, out eventInstance))
		{
			eventInstance = RuntimeManager.CreateInstance(snapshot);
			this.activeSnapshots[snapshot] = eventInstance;
			eventInstance.start();
			eventInstance.setParameterValue("snapshotActive", 1f);
		}
		AudioMixer.instance.Log("Start Snapshot: " + snapshot);
		return eventInstance;
	}

	public bool Stop(string snapshot, STOP_MODE stop_mode = STOP_MODE.ALLOWFADEOUT)
	{
		bool flag = false;
		EventInstance eventInstance = null;
		if (this.activeSnapshots.TryGetValue(snapshot, out eventInstance))
		{
			eventInstance.setParameterValue("snapshotActive", 0f);
			eventInstance.stop(stop_mode);
			this.activeSnapshots.Remove(snapshot);
			flag = true;
		}
		AudioMixer.instance.Log(string.Concat(new object[] { "Stop Snapshot: ", snapshot, " with fadeout mode: ", stop_mode }));
		return flag;
	}

	public void Reset()
	{
		this.StopAll(STOP_MODE.IMMEDIATE);
	}

	public void StopAll(STOP_MODE stop_mode = STOP_MODE.IMMEDIATE)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, EventInstance> keyValuePair in this.activeSnapshots)
		{
			if (!keyValuePair.Key.Contains("UserVolumeSettings"))
			{
				list.Add(keyValuePair.Key);
			}
		}
		for (int i = 0; i < list.Count; i++)
		{
			this.Stop(list[i], stop_mode);
		}
	}

	public bool SnapshotIsActive(string snapshot_name)
	{
		return this.activeSnapshots.ContainsKey(snapshot_name);
	}

	public void StartPersistentSnapshots()
	{
		this.persistentSnapshotsActive = true;
		this.Start(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated);
		this.Start(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot);
		this.Start(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot);
		this.Start(AudioMixerSnapshots.Get().PulseSnapshot);
	}

	public void StopPersistentSnapshots()
	{
		this.persistentSnapshotsActive = false;
		this.Stop(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated, STOP_MODE.ALLOWFADEOUT);
		this.Stop(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot, STOP_MODE.ALLOWFADEOUT);
		this.Stop(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot, STOP_MODE.ALLOWFADEOUT);
		this.Stop(AudioMixerSnapshots.Get().PulseSnapshot, STOP_MODE.ALLOWFADEOUT);
	}

	public void UpdatePersistentSnapshotParameters()
	{
		this.SetVisibleDuplicants();
		if (this.activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot, out this.duplicantCountMovingInst))
		{
			this.duplicantCountMovingInst.setParameterValue("duplicantCount", (float)this.visibleDupes["moving"]);
		}
		if (this.activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot, out this.duplicantCountSleepingInst))
		{
			this.duplicantCountSleepingInst.setParameterValue("duplicantCount", (float)this.visibleDupes["sleeping"]);
		}
		if (this.activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated, out this.duplicantCountInst))
		{
			this.duplicantCountInst.setParameterValue("duplicantCount", (float)this.visibleDupes["visible"]);
		}
		if (this.activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().PulseSnapshot, out this.pulseInst))
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
			this.pulseInst.setParameterValue("Pulse", num2);
		}
	}

	private void SetVisibleDuplicants()
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < Components.LiveMinionIdentities.Count; i++)
		{
			Vector3 position = Components.LiveMinionIdentities[i].transform.position;
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
					Worker component2 = Components.LiveMinionIdentities[i].GetComponent<Worker>();
					StaminaMonitor.Instance smi = component2.GetSMI<StaminaMonitor.Instance>();
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
		EventInstance eventInstance;
		if (this.activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot, out eventInstance))
		{
			EventDescription eventDescription;
			eventInstance.getDescription(out eventDescription);
			USER_PROPERTY user_PROPERTY;
			eventDescription.getUserProperty("buses", out user_PROPERTY);
			string stringValue = user_PROPERTY.stringValue;
			char c = '-';
			string[] array = stringValue.Split(new char[] { c });
			for (int i = 0; i < array.Length; i++)
			{
				float num = 1f;
				string text = "Volume_" + array[i];
				if (PlayerPrefs.HasKey(text))
				{
					num = PlayerPrefs.GetFloat(text);
				}
				this.userVolumeSettings.Add(array[i], num);
				this.SetUserVolume(array[i], num);
			}
		}
	}

	public void SetUserVolume(string bus, float value)
	{
		if (!this.userVolumeSettings.ContainsKey(bus))
		{
			Debug.LogError("The provided bus doesn't exist. Check yo'self fool!");
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
		this.userVolumeSettings[bus] = value;
		PlayerPrefs.SetFloat("Volume_" + bus, value);
		EventInstance eventInstance = null;
		if (this.activeSnapshots.TryGetValue(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot, out eventInstance))
		{
			eventInstance.setParameterValue(bus, this.userVolumeSettings[bus]);
		}
		else
		{
			this.Log(string.Concat(new object[] { "Tried to set [", bus, "] to [", value, "] but UserVolumeSettingsSnapshot is not active." }));
		}
	}

	private void Log(string s)
	{
	}

	private static AudioMixer _instance;

	public Dictionary<string, EventInstance> activeSnapshots = new Dictionary<string, EventInstance>();

	public List<string> SnapshotDebugLog = new List<string>();

	public bool activeNIS;

	public static float LOW_PRIORITY_CUTOFF_DISTANCE = 10f;

	public static float PULSE_SNAPSHOT_BPM = 120f;

	private EventInstance duplicantCountInst;

	private EventInstance pulseInst;

	private EventInstance duplicantCountMovingInst;

	private EventInstance duplicantCountSleepingInst;

	public bool persistentSnapshotsActive;

	private Dictionary<string, int> visibleDupes = new Dictionary<string, int>();

	public Dictionary<string, float> userVolumeSettings = new Dictionary<string, float>();
}
