using System;
using UnityEngine;

public class GameAudioSheets : AudioSheets
{
	public static GameAudioSheets Get()
	{
		if (GameAudioSheets._Instance == null)
		{
			GameAudioSheets._Instance = Resources.Load<GameAudioSheets>("GameAudioSheets");
			GameAudioSheets._Instance.Initialize();
		}
		return GameAudioSheets._Instance;
	}

	protected override AnimEvent CreateSoundOfType(string type, string file_name, string sound_name, int frame, float min_interval)
	{
		if (type == "FloorSoundEvent")
		{
			return new FloorSoundEvent(file_name, sound_name, frame);
		}
		if (type == "SoundEvent")
		{
			return new SoundEvent(file_name, sound_name, frame, min_interval, false);
		}
		if (type == "LaserSoundEvent")
		{
			return new LaserSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "HatchDrillSoundEvent")
		{
			return new HatchDrillSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "HatchChewSoundEvent")
		{
			return new HatchChewSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "LoopingSoundEvent")
		{
			return new SoundEvent(file_name, sound_name, frame, min_interval, true);
		}
		if (type == "BuildingDamageSoundEvent")
		{
			return new BuildingDamageSoundEvent(file_name, sound_name, frame);
		}
		if (type == "RemoteSoundEvent")
		{
			return new RemoteSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "VoiceSoundEvent")
		{
			return new VoiceSoundEvent(file_name, sound_name, frame, false);
		}
		if (type == "LoopingVoiceSoundEvent")
		{
			return new VoiceSoundEvent(file_name, sound_name, frame, true);
		}
		if (type == "MainMenuSoundEvent")
		{
			return new MainMenuSoundEvent(file_name, sound_name, frame);
		}
		return null;
	}

	private static GameAudioSheets _Instance;
}
