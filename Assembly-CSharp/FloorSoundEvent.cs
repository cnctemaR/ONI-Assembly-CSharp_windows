using System;
using System.Diagnostics;
using FMOD.Studio;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class FloorSoundEvent : SoundEvent
{
	public FloorSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame, false, false, (float)SoundEvent.IGNORE_INTERVAL, true)
	{
		base.noiseValues = SoundEventVolumeCache.instance.GetVolume("FloorSoundEvent", sound_name);
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 vector = behaviour.GetComponent<Transform>().GetPosition();
		int num = Grid.PosToCell(vector);
		int num2 = Grid.CellBelow(num);
		string audioCategory = FloorSoundEvent.GetAudioCategory(num2);
		string text = audioCategory + "_" + base.name;
		string text2 = GlobalAssets.GetSound(text, true);
		if (text2 == null)
		{
			text = "Rock_" + base.name;
			text2 = GlobalAssets.GetSound("Rock_" + base.name, true);
			if (text2 == null)
			{
				text = base.name;
				text2 = GlobalAssets.GetSound(text, true);
			}
		}
		if (base.IsLowPrioritySound(text2))
		{
			return;
		}
		vector = SoundEvent.GetCameraScaledPosition(vector);
		if (Grid.Element == null)
		{
			return;
		}
		bool isLiquid = Grid.Element[num].IsLiquid;
		float num3 = 0f;
		if (isLiquid)
		{
			num3 = SoundUtil.GetLiquidDepth(num);
			string sound = GlobalAssets.GetSound("Liquid_footstep", true);
			if (sound != null)
			{
				FMOD.Studio.EventInstance eventInstance = SoundEvent.BeginOneShot(sound, vector);
				if (num3 > 0f)
				{
					eventInstance.setParameterValue("liquidDepth", num3);
				}
				SoundEvent.EndOneShot(eventInstance);
			}
		}
		if (text2 != null)
		{
			FMOD.Studio.EventInstance eventInstance2 = SoundEvent.BeginOneShot(text2, vector);
			if (eventInstance2 != null)
			{
				if (num3 > 0f)
				{
					eventInstance2.setParameterValue("liquidDepth", num3);
				}
				if (behaviour.currentAnimFile != null && behaviour.currentAnimFile.Contains("anim_loco_walk"))
				{
					eventInstance2.setVolume(FloorSoundEvent.IDLE_WALKING_VOLUME_REDUCTION);
				}
				SoundEvent.EndOneShot(eventInstance2);
			}
		}
	}

	private static string GetAudioCategory(int cell)
	{
		if (!Grid.IsValidCell(cell))
		{
			return "Rock";
		}
		Element element = Grid.Element[cell];
		if (Grid.Foundation[cell])
		{
			BuildingDef buildingDef = null;
			GameObject gameObject = Grid.Objects[cell, 1];
			if (gameObject != null)
			{
				Building component = gameObject.GetComponent<BuildingComplete>();
				if (component != null)
				{
					buildingDef = component.Def;
				}
			}
			return (!(buildingDef != null) || !(buildingDef.PrefabID == "PlasticTile")) ? "Tile" : "TilePlastic";
		}
		string floorEventAudioCategory = element.substance.GetFloorEventAudioCategory();
		if (floorEventAudioCategory != null)
		{
			return floorEventAudioCategory;
		}
		if (element.HasTag(GameTags.RefinedMetal))
		{
			return "RefinedMetal";
		}
		if (element.HasTag(GameTags.Metal))
		{
			return "RawMetal";
		}
		return "Rock";
	}

	public static float IDLE_WALKING_VOLUME_REDUCTION = 0.55f;
}
