using System;
using System.Diagnostics;
using FMOD.Studio;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class FloorSoundEvent : AnimEvent
{
	public FloorSoundEvent(string file_name, string sound_name, int frame)
		: base(file_name, sound_name, frame)
	{
	}

	private bool IsLowPrioritySound(string sound)
	{
		using (new KProfiler.Region("IsLowPrioritySound", null))
		{
			if (sound != null && Camera.main.orthographicSize > AudioMixer.LOW_PRIORITY_CUTOFF_DISTANCE && !AudioMixer.instance.activeNIS && GlobalAssets.IsLowPriority(sound))
			{
				return true;
			}
		}
		return false;
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
		Vector3 vector = behaviour.GetComponent<Transform>().position;
		int num = Grid.PosToCell(vector);
		int num2 = Grid.CellBelow(num);
		string audioCategory = FloorSoundEvent.GetAudioCategory(num2);
		string text = audioCategory + "_" + this.Name;
		string text2 = GlobalAssets.GetSound(text, true);
		if (text2 == null)
		{
			text = "Rock_" + this.Name;
			text2 = GlobalAssets.GetSound("Rock_" + this.Name, true);
			if (text2 == null)
			{
				text = this.Name;
				text2 = GlobalAssets.GetSound(text, true);
			}
		}
		if (this.IsLowPrioritySound(text2))
		{
			return;
		}
		vector = CameraController.Instance.GetVerticallyScaledPosition(vector);
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

	private void PrintSoundDebug(string anim_name, string sound, string sound_name, Vector3 sound_pos)
	{
		if (sound != null)
		{
			global::Debug.Log(string.Concat(new object[] { anim_name, ", ", sound_name, ", ", this.Frame, ", ", sound_pos }), null);
		}
		else
		{
			global::Debug.Log("Missing sound: " + anim_name + ", " + sound_name, null);
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
			return "Tile";
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
