using System;
using UnityEngine;

[Serializable]
public class BuildingDamageSoundEvent : SoundEvent
{
	public BuildingDamageSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, false, false, (float)SoundEvent.IGNORE_INTERVAL, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
		vector.z = 0f;
		GameObject gameObject = behaviour.controller.gameObject;
		base.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (base.objectIsSelectedAndVisible)
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
		}
		Worker component = behaviour.GetComponent<Worker>();
		if (component == null)
		{
			string sound = GlobalAssets.GetSound("Building_Dmg_Metal", false);
			if (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound, base.looping, this.isDynamic))
			{
				SoundEvent.PlayOneShot(base.sound, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
				return;
			}
		}
		Workable workable = component.workable;
		if (workable != null)
		{
			Building component2 = workable.GetComponent<Building>();
			if (component2 != null)
			{
				BuildingDef def = component2.Def;
				string text = StringFormatter.Combine(base.name, "_", def.AudioCategory);
				string text2 = GlobalAssets.GetSound(text, false);
				if (text2 == null)
				{
					text = "Building_Dmg_Metal";
					text2 = GlobalAssets.GetSound(text, false);
				}
				if (text2 != null && (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, text2, base.looping, this.isDynamic)))
				{
					SoundEvent.PlayOneShot(text2, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible));
				}
			}
		}
	}
}
