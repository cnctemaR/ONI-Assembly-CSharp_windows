using System;
using System.Runtime.Serialization;
using FMOD.Studio;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class TimeOfDay : KMonoBehaviour, ISaveLoadable
{
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
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		if (currentCycleAsPercentage >= 0.875f)
		{
			return TimeOfDay.TimeRegion.Night;
		}
		return TimeOfDay.TimeRegion.Day;
	}

	private void Update()
	{
		this.UpdateVisuals();
		this.UpdateAudio();
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

	private void UpdateAudio()
	{
		TimeOfDay.TimeRegion currentTimeRegion = this.GetCurrentTimeRegion();
		if (currentTimeRegion != this.timeRegion)
		{
			this.TriggerSoundChange(currentTimeRegion);
			this.timeRegion = currentTimeRegion;
			base.Trigger(1791086652, null);
		}
	}

	public void Sim4000ms(float dt)
	{
		this.UpdateSunlightIntensity();
	}

	private float UpdateSunlightIntensity()
	{
		float num = 0.875f;
		float currentCycleAsPercentage = GameClock.Instance.GetCurrentCycleAsPercentage();
		float num2 = currentCycleAsPercentage / num;
		if (num2 >= 1f)
		{
			num2 = 0f;
		}
		float num3 = Mathf.Sin(num2 * 3.1415927f);
		Game.Instance.currentSunlightIntensity = num3 * 80000f;
		return num3;
	}

	private void TriggerSoundChange(TimeOfDay.TimeRegion new_region)
	{
		if (new_region != TimeOfDay.TimeRegion.Day)
		{
			if (new_region == TimeOfDay.TimeRegion.Night)
			{
				AudioMixer.instance.Start(AudioMixerSnapshots.Get().NightStartedMigrated);
				MusicManager.instance.PlaySong("Stinger_Night", false);
				MusicManager.instance.PlaySong("Underscore_Night_LP", false);
			}
		}
		else
		{
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NightStartedMigrated, STOP_MODE.ALLOWFADEOUT);
			if (MusicManager.instance.SongIsPlaying("Underscore_Night_LP"))
			{
				MusicManager.instance.StopSong("Underscore_Night_LP", true, STOP_MODE.ALLOWFADEOUT);
			}
			MusicManager.instance.PlaySong("Stinger_Day", false);
			MusicManager.instance.daysSinceDynamicMusic++;
			if (MusicManager.instance.ShouldPlayDynamicMusicStartOfDay())
			{
				MusicManager.instance.PlayDynamicMusic();
			}
		}
	}

	public void SetScale(float new_scale)
	{
		this.scale = new_scale;
	}

	[Serialize]
	private float scale;

	private TimeOfDay.TimeRegion timeRegion;

	private EventInstance nightLPEvent;

	public static TimeOfDay Instance;

	public enum TimeRegion
	{
		Invalid,
		Day,
		Night
	}
}
