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
		Vector3 position = behaviour.GetComponent<Transform>().position;
		Workable workable = behaviour.GetComponent<Worker>().workable;
		if (workable != null)
		{
			Building component = workable.GetComponent<Building>();
			if (component != null)
			{
				BuildingDef def = component.Def;
				string text = base.name + "_" + def.AudioCategory;
				string text2 = GlobalAssets.GetSound(text, false);
				if (text2 == null)
				{
					text = "Building_Dmg_Metal";
					text2 = GlobalAssets.GetSound(text, false);
				}
				if (text2 != null)
				{
					SoundEvent.PlayOneShot(text2, position);
				}
			}
		}
	}
}
