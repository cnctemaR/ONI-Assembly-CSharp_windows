using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/TimeOfDay")]
public class TimeOfDay : KMonoBehaviour, ISaveLoadable
{
	public static bool IsMilestoneApproaching
	{
		get
		{
			if (TimeOfDay.Instance != null && GameClock.Instance != null)
			{
				int currentTimeRegion = (int)TimeOfDay.Instance.GetCurrentTimeRegion();
				int cycle = GameClock.Instance.GetCycle();
				return currentTimeRegion == 2 && TimeOfDay.MILESTONE_CYCLES != null && TimeOfDay.MILESTONE_CYCLES.Contains(cycle + 1);
			}
			return false;
		}
	}

	public static bool IsMilestoneDay
	{
		get
		{
			if (TimeOfDay.Instance != null && GameClock.Instance != null)
			{
				int currentTimeRegion = (int)TimeOfDay.Instance.GetCurrentTimeRegion();
				int cycle = GameClock.Instance.GetCycle();
				return currentTimeRegion == 1 && TimeOfDay.MILESTONE_CYCLES != null && TimeOfDay.MILESTONE_CYCLES.Contains(cycle);
			}
			return false;
		}
	}

	public TimeOfDay.TimeRegion timeRegion { get; private set; }

	public static void DestroyInstance()
	{
		TimeOfDay.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		TimeOfDay.Instance = this;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		TimeOfDay.Instance = null;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.timeRegion = this.GetCurrentTimeRegion();
		this.UpdateSunlightIntensity();
	}

	[OnDeserialized]
	private void OnDeserialized()
	{
		this.UpdateVisuals();
	}

	public TimeOfDay.TimeRegion GetCurrentTimeRegion()
	{
		if (GameClock.Instance.IsNighttime())
		{
			return TimeOfDay.TimeRegion.Night;
		}
		return TimeOfDay.TimeRegion.Day;
	}

	private void Update()
	{
		this.UpdateVisuals();
		TimeOfDay.TimeRegion currentTimeRegion = this.GetCurrentTimeRegion();
		int cycle = GameClock.Instance.GetCycle();
		if (currentTimeRegion != this.timeRegion)
		{
			if (TimeOfDay.IsMilestoneApproaching)
			{
				Game.Instance.Trigger(-720092972, cycle);
			}
			if (TimeOfDay.IsMilestoneDay)
			{
				Game.Instance.Trigger(2070437606, cycle);
			}
			this.TriggerSoundChange(currentTimeRegion, TimeOfDay.IsMilestoneDay);
			this.timeRegion = currentTimeRegion;
			base.Trigger(1791086652, null);
		}
	}

	private void UpdateVisuals()
	{
		float num = 0.875f;
		float num2 = 0.2f;
		float num3 = 1f;
		float num4 = 0f;
		if (GameClock.Instance.GetCurrentCycleAsPercentage() >= num)
		{
			num4 = num3;
		}
		this.scale = Mathf.Lerp(this.scale, num4, Time.deltaTime * num2);
		float num5 = this.UpdateSunlightIntensity();
		Shader.SetGlobalVector("_TimeOfDay", new Vector4(this.scale, num5, 0f, 0f));
	}

	public void Sim4000ms(float dt)
	{
		this.UpdateSunlightIntensity();
	}

	public void SetEclipse(bool eclipse)
	{
		this.isEclipse = eclipse;
	}

	private float UpdateSunlightIntensity()
	{
		float daytimeDurationInPercentage = GameClock.Instance.GetDaytimeDurationInPercentage();
		float num = GameClock.Instance.GetCurrentCycleAsPercentage() / daytimeDurationInPercentage;
		if (num >= 1f || this.isEclipse)
		{
			num = 0f;
		}
		float num2 = Mathf.Sin(num * 3.1415927f);
		Game.Instance.currentFallbackSunlightIntensity = num2 * 80000f;
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			worldContainer.currentSunlightIntensity = num2 * (float)worldContainer.sunlight;
			worldContainer.currentCosmicIntensity = (float)worldContainer.cosmicRadiation;
		}
		return num2;
	}

	private void TriggerSoundChange(TimeOfDay.TimeRegion new_region, bool milestoneReached)
	{
		if (new_region == TimeOfDay.TimeRegion.Day)
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NightStartedMigrated, STOP_MODE.ALLOWFADEOUT);
			if (MusicManager.instance.SongIsPlaying("Stinger_Loop_Night"))
			{
				MusicManager.instance.StopSong("Stinger_Loop_Night", true, STOP_MODE.ALLOWFADEOUT);
			}
			if (milestoneReached)
			{
				MusicManager.instance.PlaySong("Stinger_Day_Celebrate", false);
			}
			else
			{
				MusicManager.instance.PlaySong("Stinger_Day", false);
			}
			MusicManager.instance.PlayDynamicMusic();
			return;
		}
		if (new_region != TimeOfDay.TimeRegion.Night)
		{
			return;
		}
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().NightStartedMigrated);
		MusicManager.instance.PlaySong("Stinger_Loop_Night", false);
	}

	public void SetScale(float new_scale)
	{
		this.scale = new_scale;
	}

	private const string MILESTONE_CYCLE_REACHED_AUDIO_NAME = "Stinger_Day_Celebrate";

	public static List<int> MILESTONE_CYCLES = new List<int>(2) { 99, 999 };

	[Serialize]
	private float scale;

	private EventInstance nightLPEvent;

	public static TimeOfDay Instance;

	private bool isEclipse;

	public enum TimeRegion
	{
		Invalid,
		Day,
		Night
	}
}
