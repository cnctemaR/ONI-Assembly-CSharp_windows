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
		foreach (AudioSheet audioSheet in this.sheets)
		{
			foreach (AudioSheet.SoundInfo soundInfo in audioSheet.soundInfos)
			{
				if (soundInfo.Type == "MouthFlapSoundEvent" || soundInfo.Type == "VoiceSoundEvent")
				{
					HashSet<HashedString> hashSet = null;
					if (!this.animsNotAllowedToPlaySpeech.TryGetValue(soundInfo.File, out hashSet))
					{
						hashSet = new HashSet<HashedString>();
						this.animsNotAllowedToPlaySpeech[soundInfo.File] = hashSet;
					}
					hashSet.Add(soundInfo.Anim);
				}
			}
		}
	}

	protected override AnimEvent CreateSoundOfType(string type, string file_name, string sound_name, int frame, float min_interval)
	{
		SoundEvent soundEvent = null;
		bool flag = true;
		if (sound_name.Contains(":disable_camera_position_scaling"))
		{
			sound_name = sound_name.Replace(":disable_camera_position_scaling", string.Empty);
			flag = false;
		}
		if (type == "FloorSoundEvent")
		{
			soundEvent = new FloorSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "SoundEvent" || type == "LoopingSoundEvent")
		{
			bool flag2 = type == "LoopingSoundEvent";
			string[] array = sound_name.Split(new char[] { ':' });
			sound_name = array[0];
			string text = sound_name;
			bool flag3 = flag2;
			soundEvent = new SoundEvent(file_name, text, frame, true, flag3, min_interval, false);
			for (int i = 1; i < array.Length; i++)
			{
				if (array[i] == "IGNORE_PAUSE")
				{
					soundEvent.ignorePause = true;
				}
				else
				{
					global::Debug.LogWarning(sound_name + " has unknown parameter " + array[i]);
				}
			}
		}
		else if (type == "LadderSoundEvent")
		{
			soundEvent = new LadderSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "LaserSoundEvent")
		{
			soundEvent = new LaserSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "HatchDrillSoundEvent")
		{
			soundEvent = new HatchDrillSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "CreatureChewSoundEvent")
		{
			soundEvent = new CreatureChewSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "BuildingDamageSoundEvent")
		{
			soundEvent = new BuildingDamageSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "WallDamageSoundEvent")
		{
			soundEvent = new WallDamageSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "RemoteSoundEvent")
		{
			soundEvent = new RemoteSoundEvent(file_name, sound_name, frame, min_interval);
		}
		else if (type == "VoiceSoundEvent" || type == "LoopingVoiceSoundEvent")
		{
			soundEvent = new VoiceSoundEvent(file_name, sound_name, frame, type == "LoopingVoiceSoundEvent");
		}
		else if (type == "MouthFlapSoundEvent")
		{
			soundEvent = new MouthFlapSoundEvent(file_name, sound_name, frame, false);
		}
		else if (type == "MainMenuSoundEvent")
		{
			soundEvent = new MainMenuSoundEvent(file_name, sound_name, frame);
		}
		else if (type == "CreatureVariationSoundEvent")
		{
			string text2 = sound_name;
			bool flag3 = type == "LoopingSoundEvent";
			soundEvent = new CreatureVariationSoundEvent(file_name, text2, frame, true, flag3, min_interval, false);
		}
		else if (type == "CountedSoundEvent")
		{
			soundEvent = new CountedSoundEvent(file_name, sound_name, frame, true, false, min_interval, false);
		}
		else if (type == "SculptingSoundEvent")
		{
			soundEvent = new SculptingSoundEvent(file_name, sound_name, frame, true, false, min_interval, false);
		}
		else if (type == "PhonoboxSoundEvent")
		{
			soundEvent = new PhonoboxSoundEvent(file_name, sound_name, frame, min_interval);
		}
		if (soundEvent != null)
		{
			soundEvent.shouldCameraScalePosition = flag;
		}
		return soundEvent;
	}

	public bool IsAnimAllowedToPlaySpeech(KAnim.Anim anim)
	{
		HashSet<HashedString> hashSet = null;
		return !this.animsNotAllowedToPlaySpeech.TryGetValue(anim.animFile.name, out hashSet) || !hashSet.Contains(anim.hash);
	}

	private static GameAudioSheets _Instance;

	private HashSet<HashedString> validFileNames = new HashSet<HashedString>();

	private Dictionary<HashedString, HashSet<HashedString>> animsNotAllowedToPlaySpeech = new Dictionary<HashedString, HashSet<HashedString>>();

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
