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
		Vector3 vector = behaviour.position;
		KBatchedAnimController controller = behaviour.controller;
		if (controller != null)
		{
			vector = controller.GetPivotSymbolPosition();
		}
		int num = Grid.PosToCell(vector);
		int num2 = Grid.CellBelow(num);
		if (!Grid.IsValidCell(num2))
		{
			return;
		}
		string text = GlobalAssets.GetSound(StringFormatter.Combine(FloorSoundEvent.GetAudioCategory(num2), "_", base.name), true);
		if (text == null)
		{
			text = GlobalAssets.GetSound(StringFormatter.Combine("Rock_", base.name), true);
			if (text == null)
			{
				text = GlobalAssets.GetSound(base.name, true);
			}
		}
		GameObject gameObject = behaviour.controller.gameObject;
		MinionIdentity component = gameObject.GetComponent<MinionIdentity>();
		base.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (SoundEvent.IsLowPrioritySound(text) && !base.objectIsSelectedAndVisible)
		{
			return;
		}
		vector = SoundEvent.GetCameraScaledPosition(vector, false);
		vector.z = 0f;
		if (base.objectIsSelectedAndVisible)
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
		}
		if (Grid.Element == null)
		{
			return;
		}
		bool isLiquid = Grid.Element[num].IsLiquid;
		float num3 = 0f;
		bool flag = Grid.IsSubstantialLiquid(Grid.CellAbove(num), 0.35f);
		if (isLiquid)
		{
			num3 = SoundUtil.GetLiquidDepth(num);
			string sound = GlobalAssets.GetSound(flag ? "uw_footstep" : "Liquid_footstep", false);
			if (sound != null && (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound, base.looping, this.isDynamic)))
			{
				FMOD.Studio.EventInstance eventInstance = SoundEvent.BeginOneShot(sound, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible), false);
				if (num3 > 0f && !flag)
				{
					eventInstance.setParameterByName("liquidDepth", num3, false);
				}
				SoundEvent.EndOneShot(eventInstance);
			}
		}
		if (component != null && component.model == BionicMinionConfig.MODEL)
		{
			string sound2 = GlobalAssets.GetSound("Bionic_move", true);
			if (sound2 != null && (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, sound2, base.looping, this.isDynamic)))
			{
				SoundEvent.EndOneShot(SoundEvent.BeginOneShot(sound2, vector, SoundEvent.GetVolume(base.objectIsSelectedAndVisible), false));
			}
		}
		if (text != null && (base.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, text, base.looping, this.isDynamic)))
		{
			FMOD.Studio.EventInstance eventInstance2 = SoundEvent.BeginOneShot(text, vector, 1f, false);
			if (eventInstance2.isValid())
			{
				if (num3 > 0f)
				{
					eventInstance2.setParameterByName("liquidDepth", num3, false);
				}
				if (behaviour.controller.HasAnimationFile("anim_loco_walk_kanim"))
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
			string text = "";
			if (buildingDef != null)
			{
				string prefabID = buildingDef.PrefabID;
				if (prefabID == "PlasticTile")
				{
					text = "TilePlastic";
				}
				else if (prefabID == "GlassTile")
				{
					text = "TileGlass";
				}
				else if (prefabID == "BunkerTile")
				{
					text = "TileBunker";
				}
				else if (prefabID == "MetalTile")
				{
					text = "TileMetal";
				}
				else if (prefabID == "CarpetTile")
				{
					text = "Carpet";
				}
				else if (prefabID == "SnowTile")
				{
					text = "TileSnow";
				}
				else if (prefabID == "WoodTile")
				{
					text = "TileWood";
				}
				else if (prefabID == "RubberTile")
				{
					text = "TileRubber";
				}
				else
				{
					text = "Tile";
				}
			}
			return text;
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
