using System;
using FMOD.Studio;
using UnityEngine;

public class HatchDrillSoundEvent : SoundEvent
{
	public HatchDrillSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, min_interval, true)
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
		Vector3 position = behaviour.GetComponent<Transform>().position;
		int num = Grid.PosToCell(position);
		int num2 = Grid.CellBelow(num);
		float num3 = (float)HatchDrillSoundEvent.GetAudioCategory(num2);
		EventInstance eventInstance = SoundEvent.BeginOneShot(base.sound, position);
		eventInstance.setParameterValue("material_ID", num3);
		SoundEvent.EndOneShot(eventInstance);
	}

	private static int GetAudioCategory(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return 7;
		}
		Element element = Grid.Element[cell];
		if (element.id == SimHashes.Dirt)
		{
			return 0;
		}
		if (element.HasTag(GameTags.IceOre))
		{
			return 1;
		}
		if (element.id == SimHashes.CrushedIce)
		{
			return 12;
		}
		if (element.id == SimHashes.DirtyIce)
		{
			return 13;
		}
		if (Grid.Foundation[cell])
		{
			return 2;
		}
		if (element.id == SimHashes.OxyRock)
		{
			return 3;
		}
		if (element.id == SimHashes.PhosphateNodules || element.id == SimHashes.Phosphorus || element.id == SimHashes.Phosphorite)
		{
			return 4;
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
		if (element.id == SimHashes.Clay)
		{
			return 9;
		}
		if (element.id == SimHashes.Algae)
		{
			return 10;
		}
		if (element.id == SimHashes.SlimeMold)
		{
			return 11;
		}
		return 7;
	}
}
