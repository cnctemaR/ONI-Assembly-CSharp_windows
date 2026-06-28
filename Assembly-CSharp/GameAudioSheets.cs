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
		AnimEvent animEvent;
		if (type == "FloorSoundEvent")
		{
			animEvent = new FloorSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "SoundEvent" || type == "LoopingSoundEvent")
		{
			bool flag = type == "LoopingSoundEvent";
			animEvent = new SoundEvent(file_name, sound_name, frame, true, flag, min_interval, false);
		}
		else if (type == "LadderSoundEvent")
		{
			animEvent = new LadderSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "LaserSoundEvent")
		{
			animEvent = new LaserSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "HatchDrillSoundEvent")
		{
			animEvent = new HatchDrillSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "HatchChewSoundEvent")
		{
			animEvent = new HatchChewSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "BuildingDamageSoundEvent")
		{
			animEvent = new BuildingDamageSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "WallDamageSoundEvent")
		{
			animEvent = new WallDamageSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "RemoteSoundEvent")
		{
			animEvent = new RemoteSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "VoiceSoundEvent" || type == "LoopingVoiceSoundEvent")
		{
			animEvent = new VoiceSoundEvent(file_name, sound_name, frame, type == "LoopingVoiceSoundEvent");
		}
		else if (type == "MainMenuSoundEvent")
		{
			animEvent = new MainMenuSoundEvent(file_name, sound_name, frame);
		}
		else
		{
			animEvent = null;
		}
		return animEvent;
	}

	private static GameAudioSheets _Instance;
}
