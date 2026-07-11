using System;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioSheets : AudioSheets
{
	public static GameAudioSheets Get()
	{
		if (GameAudioSheets._Instance == null)
		{
			GameAudioSheets._Instance = Resources.Load<GameAudioSheets>("GameAudioSheets");
		}
		return GameAudioSheets._Instance;
	}

	public override void Initialize()
	{
		this.validFileNames.Add("game_triggered");
		foreach (KAnimFile kanimFile in Assets.instance.AnimAssets)
		{
			if (!(kanimFile == null))
			{
				this.validFileNames.Add(kanimFile.name);
			}
		}
		base.Initialize();
	}

	protected override AnimEvent CreateSoundOfType(string type, string file_name, string sound_name, int frame, float min_interval)
	{
		if (type == "FloorSoundEvent")
		{
			return new FloorSoundEvent(file_name, sound_name, frame);
		}
		if (type == "SoundEvent" || type == "LoopingSoundEvent")
		{
			bool flag = type == "LoopingSoundEvent";
			string[] array = sound_name.Split(new char[] { ':' });
			sound_name = array[0];
			string text = sound_name;
			bool flag2 = flag;
			SoundEvent soundEvent = new SoundEvent(file_name, text, frame, true, flag2, min_interval, false);
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] == "IGNORE_PAUSE")
				{
					soundEvent.ignorePause = true;
				}
				else
				{
					global::Debug.LogWarning(sound_name + " has unknown parameter " + array[i], null);
				}
			}
			return soundEvent;
		}
		if (type == "LadderSoundEvent")
		{
			return new LadderSoundEvent(file_name, sound_name, frame);
		}
		if (type == "LaserSoundEvent")
		{
			return new LaserSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "HatchDrillSoundEvent")
		{
			return new HatchDrillSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "CreatureChewSoundEvent")
		{
			return new CreatureChewSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "BuildingDamageSoundEvent")
		{
			return new BuildingDamageSoundEvent(file_name, sound_name, frame);
		}
		if (type == "WallDamageSoundEvent")
		{
			return new WallDamageSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "RemoteSoundEvent")
		{
			return new RemoteSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (type == "VoiceSoundEvent" || type == "LoopingVoiceSoundEvent")
		{
			return new VoiceSoundEvent(file_name, sound_name, frame, type == "LoopingVoiceSoundEvent");
		}
		if (type == "MainMenuSoundEvent")
		{
			return new MainMenuSoundEvent(file_name, sound_name, frame);
		}
		if (type == "CreatureVariationSoundEvent")
		{
			string text2 = sound_name;
			bool flag2 = type == "LoopingSoundEvent";
			return new CreatureVariationSoundEvent(file_name, text2, frame, true, flag2, min_interval, false);
		}
		if (type == "CountedSoundEvent")
		{
			return new CountedSoundEvent(file_name, sound_name, frame, true, false, min_interval, false);
		}
		if (type == "PhonoboxSoundEvent")
		{
			return new PhonoboxSoundEvent(file_name, sound_name, frame, min_interval);
		}
		return null;
	}

	private static GameAudioSheets _Instance;

	private HashSet<HashedString> validFileNames = new HashSet<HashedString>();

	private class SingleAudioSheetLoader : AsyncLoader
	{
		public override void Run()
		{
			this.sheet.soundInfos = new ResourceLoader<AudioSheet.SoundInfo>(this.text, this.name).resources.ToArray();
		}

		public AudioSheet sheet;

		public string text;

		public string name;
	}

	private class GameAudioSheetLoader : GlobalAsyncLoader<GameAudioSheets.GameAudioSheetLoader>
	{
		public override void CollectLoaders(List<AsyncLoader> loaders)
		{
			foreach (AudioSheet audioSheet in GameAudioSheets.Get().sheets)
			{
				loaders.Add(new GameAudioSheets.SingleAudioSheetLoader
				{
					sheet = audioSheet,
					text = audioSheet.asset.text,
					name = audioSheet.asset.name
				});
			}
		}

		public override void Run()
		{
		}
	}
}
