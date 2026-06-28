using System;
using UnityEngine;

public class LadderSoundEvent : SoundEvent
{
	public LadderSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, false, false, (float)SoundEvent.IGNORE_INTERVAL, true)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		int num = Grid.PosToCell(position);
		BuildingDef buildingDef = null;
		if (Grid.IsValidCell(num))
		{
			GameObject gameObject = Grid.Objects[num, 1];
			if (gameObject != null && gameObject.GetComponent<Ladder>() != null)
			{
				Building component = gameObject.GetComponent<BuildingComplete>();
				if (component != null)
				{
					buildingDef = component.Def;
				}
			}
		}
		if (buildingDef != null)
		{
			string text = ((!(buildingDef.PrefabID == "LadderFast")) ? base.name : (base.name + "_Plastic"));
			string sound = GlobalAssets.GetSound(text, false);
			if (sound != null)
			{
				SoundEvent.PlayOneShot(sound, position);
			}
		}
	}
}
