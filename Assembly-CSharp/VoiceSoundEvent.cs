using System;
using FMOD.Studio;
using Klei.AI;
using UnityEngine;

public class VoiceSoundEvent : SoundEvent
{
	public VoiceSoundEvent(string file_name, string sound_name, int frame, bool is_looping)
		: base(file_name, sound_name, frame, false, is_looping, (float)SoundEvent.IGNORE_INTERVAL, true)
	{
		base.noiseValues = SoundEventVolumeCache.instance.GetVolume("VoiceSoundEvent", sound_name);
	}

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		MinionIdentity component = behaviour.GetComponent<MinionIdentity>();
		if (component == null || (base.name.Contains("state") && Time.time - component.timeLastSpoke < this.intervalBetweenSpeaking))
		{
			return;
		}
		if (base.name.Contains(":"))
		{
			string[] array = base.name.Split(new char[] { ':' });
			float num = float.Parse(array[1]);
			float num2 = (float)global::UnityEngine.Random.Range(0, 100);
			if (num2 > num)
			{
				return;
			}
		}
		Worker component2 = behaviour.GetComponent<Worker>();
		string assetName = this.GetAssetName(component2);
		StaminaMonitor.Instance smi = component2.GetSMI<StaminaMonitor.Instance>();
		if (!base.name.Contains("sleep_") && smi != null && smi.IsSleeping())
		{
			return;
		}
		Vector3 position = component2.transform.position;
		string sound = GlobalAssets.GetSound(assetName, true);
		if (sound != null)
		{
			if (base.looping)
			{
				LoopingSounds component3 = behaviour.GetComponent<LoopingSounds>();
				if (component3 == null)
				{
					global::Debug.Log(behaviour.name + " is missing LoopingSounds component. ", null);
				}
				else if (!component3.StartSound(sound, position))
				{
					Output.LogWarning(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", sound, behaviour.name) });
				}
			}
			else
			{
				EventInstance eventInstance = SoundEvent.BeginOneShot(sound, position);
				if (sound.Contains("sleep_"))
				{
					Traits component4 = behaviour.GetComponent<Traits>();
					if (component4.HasTrait("Snorer"))
					{
						eventInstance.setParameterValue("snoring", 1f);
					}
				}
				SoundEvent.EndOneShot(eventInstance);
				component.timeLastSpoke = Time.time;
			}
		}
		else if (AudioDebug.Get().debugVoiceSounds)
		{
			global::Debug.LogWarning("Missing voice sound: " + assetName, null);
		}
	}

	private string GetAssetName(Component cmp)
	{
		string text = "F01";
		if (cmp != null)
		{
			MinionIdentity component = cmp.GetComponent<MinionIdentity>();
			if (component != null)
			{
				text = component.GetVoiceId();
			}
		}
		string text2 = base.name;
		if (base.name.Contains(":"))
		{
			string[] array = base.name.Split(new char[] { ':' });
			text2 = array[0];
		}
		return "DupVoc_" + text + "_" + text2;
	}

	public override void Stop(AnimEventManager.EventPlayerData behaviour)
	{
		if (base.looping)
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

	public float timeLastSpoke;

	public float intervalBetweenSpeaking = 10f;
}
