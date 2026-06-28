using System;
using FMOD.Studio;
using UnityEngine;

public class HatchDrillSoundEvent : SoundEvent
{
	public HatchDrillSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, true, min_interval, false)
	{
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
		int num;
		if (!Grid.IsValidCell(cell))
		{
			num = 7;
		}
		else
		{
			Element element = Grid.Element[cell];
			if (element.id == SimHashes.Dirt)
			{
				num = 0;
			}
			else if (element.HasTag(GameTags.IceOre))
			{
				num = 1;
			}
			else if (element.id == SimHashes.CrushedIce)
			{
				num = 12;
			}
			else if (element.id == SimHashes.DirtyIce)
			{
				num = 13;
			}
			else if (Grid.Foundation[cell])
			{
				num = 2;
			}
			else if (element.id == SimHashes.OxyRock)
			{
				num = 3;
			}
			else if (element.id == SimHashes.PhosphateNodules || element.id == SimHashes.Phosphorus || element.id == SimHashes.Phosphorite)
			{
				num = 4;
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
			else if (element.id == SimHashes.Clay)
			{
				num = 9;
			}
			else if (element.id == SimHashes.Algae)
			{
				num = 10;
			}
			else if (element.id == SimHashes.SlimeMold)
			{
				num = 11;
			}
			else
			{
				num = 7;
			}
		}
		return num;
	}
}
