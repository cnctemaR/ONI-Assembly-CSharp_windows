using System;
using FMOD.Studio;
using UnityEngine;

public class WallDamageSoundEvent : SoundEvent
{
	public WallDamageSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, false, min_interval, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 vector = default(Vector3);
		AggressiveChore.StatesInstance smi = behaviour.controller.gameObject.GetSMI<AggressiveChore.StatesInstance>();
		if (smi != null)
		{
			this.tile = smi.sm.wallCellToBreak;
			int audioCategory = WallDamageSoundEvent.GetAudioCategory(this.tile);
			vector = Grid.CellToPos(this.tile);
			EventInstance eventInstance = SoundEvent.BeginOneShot(base.sound, vector);
			eventInstance.setParameterValue("material_ID", (float)audioCategory);
			SoundEvent.EndOneShot(eventInstance);
		}
	}

	private static int GetAudioCategory(int tile)
	{
		Element element = Grid.Element[tile];
		int num;
		if (Grid.Foundation[tile])
		{
			num = 12;
		}
		else if (element.id == SimHashes.Dirt)
		{
			num = 0;
		}
		else if (element.id == SimHashes.CrushedIce || element.id == SimHashes.Ice || element.id == SimHashes.DirtyIce)
		{
			num = 1;
		}
		else if (element.id == SimHashes.OxyRock)
		{
			num = 3;
		}
		else if (element.HasTag(GameTags.Metal))
		{
			num = 5;
		}
		else if (element.HasTag(GameTags.RefinedMetal))
		{
			num = 6;
		}
		else if (element.id == SimHashes.Sand)
		{
			num = 8;
		}
		else if (element.id == SimHashes.Algae)
		{
			num = 10;
		}
		else
		{
			num = 7;
		}
		return num;
	}

	public int tile;
}
