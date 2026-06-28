using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioSheets : ScriptableObject
{
	protected virtual void Initialize()
	{
		foreach (AudioSheet audioSheet in this.sheets)
		{
			audioSheet.Load();
			foreach (AudioSheet.SoundInfo soundInfo in audioSheet.soundInfos)
			{
				string text = soundInfo.Type;
				if (text == null || text == "")
				{
					text = audioSheet.defaultType;
				}
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name0, soundInfo.Frame0);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name1, soundInfo.Frame1);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name2, soundInfo.Frame2);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name3, soundInfo.Frame3);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name4, soundInfo.Frame4);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name5, soundInfo.Frame5);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name6, soundInfo.Frame6);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name7, soundInfo.Frame7);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name8, soundInfo.Frame8);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name9, soundInfo.Frame9);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name10, soundInfo.Frame10);
				this.CreateSound(soundInfo.File, soundInfo.Anim, text, soundInfo.MinInterval, soundInfo.Name11, soundInfo.Frame11);
			}
		}
	}

	private void CreateSound(string file_name, string anim_name, string type, float min_interval, string sound_name, int frame)
	{
		string text = file_name + "." + anim_name;
		if (sound_name != null && !(sound_name == ""))
		{
			AnimEvent animEvent = this.CreateSoundOfType(type, file_name, sound_name, frame, min_interval);
			if (animEvent == null)
			{
				global::Debug.LogError("Unknown sound type: " + type, null);
			}
			else
			{
				List<AnimEvent> list = null;
				if (!this.events.TryGetValue(text, out list))
				{
					list = new List<AnimEvent>();
					this.events[text] = list;
				}
				list.Add(animEvent);
			}
		}
	}

	protected virtual AnimEvent CreateSoundOfType(string type, string file_name, string sound_name, int frame, float min_interval)
	{
		return null;
	}

	public List<AnimEvent> GetEvents(HashedString anim_id)
	{
		List<AnimEvent> list = null;
		this.events.TryGetValue(anim_id, out list);
		return list;
	}

	public List<AudioSheet> sheets = new List<AudioSheet>();

	public Dictionary<HashedString, List<AnimEvent>> events = new Dictionary<HashedString, List<AnimEvent>>();
}
