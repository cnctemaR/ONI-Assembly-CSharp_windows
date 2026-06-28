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
		if (sound != null && Camera.main.orthographicSize > AudioMixer.LOW_PRIORITY_CUTOFF_DISTANCE && !AudioMixer.instance.activeNIS)
		{
			string text = sound.ToLower();
			if (text.Contains("lowpriority"))
			{
				KFMODDebugger.instance.Log("Low priority sound culled:" + sound);
				return true;
			}
		}
		return false;
	}

	public override void OnPlay(IAnimBehaviour behaviour)
	{
		if (CameraController.Instance == null || base.IsFilteredOut(behaviour))
		{
			return;
		}
		Vector3 position = behaviour.GetComponent<Transform>().position;
		int num = Grid.PosToCell(position);
		int num2 = Grid.CellBelow(num);
		Navigator component = behaviour.GetComponent<Navigator>();
		bool flag = component != null && component.CurrentNavType == NavType.Ladder;
		string audioCategory = FloorSoundEvent.GetAudioCategory(num2, flag);
		string text = audioCategory + "_" + this.Name;
		string text2 = GlobalAssets.GetSound(text, true);
		if (text2 == null)
		{
			text = "Rock_" + this.Name;
			text2 = GlobalAssets.GetSound(text, true);
			if (text2 == null)
			{
				text = this.Name;
				text2 = GlobalAssets.GetSound(text, true);
			}
		}
		if (!CameraController.Instance.IsAudibleSound(position, text2) || this.IsLowPrioritySound(text2))
		{
			return;
		}
		string text3 = null;
		string text4 = "Liquid_footstep";
		bool isLiquid = Grid.Element[num].IsLiquid;
		float num3 = 0f;
		if (isLiquid && (this.Name.Contains("footstep") || this.Name.Contains("jump") || this.Name.Contains("land") || this.Name.Contains("bodyfall")))
		{
			num3 = SoundUtil.GetLiquidDepth(num);
			text3 = GlobalAssets.GetSound(text4, true);
			if (text3 != null && !SpeedControlScreen.Instance.IsPaused)
			{
				FMOD.Studio.EventInstance eventInstance = SoundEvent.BeginOneShot(text3, position);
				eventInstance.setParameterValue("liquidDepth", num3);
				SoundEvent.EndOneShot(eventInstance);
			}
		}
		if (text2 != null && !SpeedControlScreen.Instance.IsPaused)
		{
			FMOD.Studio.EventInstance eventInstance2 = SoundEvent.BeginOneShot(text2, position);
			if (eventInstance2 != null)
			{
				eventInstance2.setParameterValue("liquidDepth", num3);
				if (behaviour.currentAnimFile.Contains("anim_loco_walk"))
				{
					eventInstance2.setVolume(FloorSoundEvent.IDLE_WALKING_VOLUME_REDUCTION);
				}
				SoundEvent.EndOneShot(eventInstance2);
			}
		}
		if (AudioDebug.Get().debugFloorSounds)
		{
			this.PrintSoundDebug(behaviour.currentAnim, text2, text, position);
			if (isLiquid)
			{
				this.PrintSoundDebug(behaviour.currentAnim, text3, text4, position);
			}
		}
	}

	private void PrintSoundDebug(string anim_name, string sound, string sound_name, Vector3 sound_pos)
	{
		if (sound != null)
		{
			global::UnityEngine.Debug.Log(string.Concat(new object[] { anim_name, ", ", sound_name, ", ", this.Frame, ", ", sound_pos }));
		}
		else
		{
			global::UnityEngine.Debug.Log("Missing sound: " + anim_name + ", " + sound_name);
		}
	}

	private static string GetAudioCategory(int cell, bool is_on_ladder)
	{
		if (is_on_ladder)
		{
			return "Ladder";
		}
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
