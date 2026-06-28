using System;
using FMOD.Studio;
using Klei.AI;
using UnityEngine;

public class VoiceSoundEvent : AnimEvent
{
	public VoiceSoundEvent(string file_name, string sound_name, int frame, bool is_looping)
		: base(file_name, sound_name, frame)
	{
		this.looping = is_looping;
	}

	public override void OnPlay(IAnimBehaviour behaviour)
	{
		this.Play(behaviour.GetComponent<KMonoBehaviour>());
	}

	public void Play(Component cmp)
	{
		if (this.Name == "voice_jump" || this.Name == "voice_land")
		{
			float num = (float)global::UnityEngine.Random.Range(0, 100);
			if (num > VoiceSoundEvent.locomotionSoundProb)
			{
				return;
			}
		}
		Worker component = cmp.GetComponent<Worker>();
		StaminaMonitor.Instance smi = component.GetSMI<StaminaMonitor.Instance>();
		if (!this.Name.Contains("sleep_") && smi != null && smi.IsSleeping())
		{
			return;
		}
		Vector3 position = cmp.transform.position;
		string assetName = this.GetAssetName(cmp);
		string sound = GlobalAssets.GetSound(assetName, true);
		if (sound != null)
		{
			if (this.looping)
			{
				LoopingSounds component2 = cmp.GetComponent<LoopingSounds>();
				if (component2 == null)
				{
					Debug.Log(cmp.name + " is missing LoopingSounds component. ");
				}
				else if (!component2.StartSound(sound, position))
				{
					Output.LogWarning(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", sound, cmp.name) });
				}
			}
			else
			{
				EventInstance eventInstance = SoundEvent.BeginOneShot(sound, position);
				if (sound.Contains("sleep_"))
				{
					Traits component3 = cmp.GetComponent<Traits>();
					if (component3.HasTrait("Snorer"))
					{
						eventInstance.setParameterValue("snoring", 1f);
					}
				}
				SoundEvent.EndOneShot(eventInstance);
			}
		}
		else if (AudioDebug.Get().debugVoiceSounds)
		{
			Debug.LogWarning("Missing voice sound: " + assetName);
		}
	}

	private string GetAssetName(Component cmp)
	{
		string text = "F01";
		MinionIdentity component = cmp.GetComponent<MinionIdentity>();
		if (component != null)
		{
			text = component.GetVoiceId();
		}
		return "DupVoc_" + text + "_" + this.Name;
	}

	public override void Stop(IAnimBehaviour behaviour)
	{
		if (this.looping)
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component != null)
			{
				string assetName = this.GetAssetName(component);
				string sound = GlobalAssets.GetSound(assetName, true);
				component.StopSound(sound);
			}
		}
	}

	public static float locomotionSoundProb = 50f;

	public bool looping;
}
