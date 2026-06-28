using System;
using FMOD.Studio;
using UnityEngine;

public class WallDamageSoundEvent : SoundEvent
{
	public WallDamageSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, min_interval, false)
	{
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (this.ShouldPlaySound(behaviour))
		{
			this.PlaySound(behaviour);
		}
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 vector = default(Vector3);
		int num = 0;
		AggressiveChore.StatesInstance smi = behaviour.controller.gameObject.GetSMI<AggressiveChore.StatesInstance>();
		if (smi != null)
		{
			this.tile = smi.sm.wallCellToBreak;
			num = WallDamageSoundEvent.GetAudioCategory(this.tile);
			vector = Grid.CellToPos(this.tile);
		}
		EventInstance eventInstance = SoundEvent.BeginOneShot(base.sound, vector);
		eventInstance.setParameterValue("material_ID", (float)num);
		SoundEvent.EndOneShot(eventInstance);
	}

	private static int GetAudioCategory(int tile)
	{
		Element element = Grid.Element[tile];
		if (Grid.Foundation[tile])
		{
			return 12;
		}
		if (element.id == SimHashes.Dirt)
		{
			return 0;
		}
		if (element.id == SimHashes.CrushedIce || element.id == SimHashes.Ice || element.id == SimHashes.DirtyIce)
		{
			return 1;
		}
		if (element.id == SimHashes.OxyRock)
		{
			return 3;
		}
		if (element.HasTag(GameTags.Metal))
		{
			return 5;
		}
		if (element.HasTag(GameTags.RefinedMetal))
		{
			return 6;
		}
		if (element.id == SimHashes.Sand)
		{
			return 8;
		}
		if (element.id == SimHashes.Algae)
		{
			return 10;
		}
		return 7;
	}

	public int tile;
}
