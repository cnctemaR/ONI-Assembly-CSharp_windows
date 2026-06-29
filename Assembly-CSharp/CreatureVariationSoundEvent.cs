using System;

public class CreatureVariationSoundEvent : SoundEvent
{
	public CreatureVariationSoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, sound_name, frame, do_load, is_looping, min_interval, is_dynamic)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		string text = base.sound;
		CreatureBrain component = behaviour.GetComponent<CreatureBrain>();
		if (component != null && !string.IsNullOrEmpty(component.symbolPrefix))
		{
			string sound = GlobalAssets.GetSound(component.symbolPrefix + base.name, false);
			if (!string.IsNullOrEmpty(sound))
			{
				text = sound;
			}
		}
		base.PlaySound(behaviour, text);
	}
}
