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

	public override void OnPlay(AnimEventManager.EventPlayerData behaviour)
	{
		if (this.ShouldPlaySound(behaviour))
		{
			this.Play(behaviour.GetComponent<KMonoBehaviour>());
		}
	}

	public void Play(Component cmp)
	{
		MinionIdentity component = cmp.GetComponent<MinionIdentity>();
		if (this.Name.Contains("state") && Time.time - component.timeLastSpoke < this.intervalBetweenSpeaking)
		{
			return;
		}
		if (this.Name.Contains(":"))
		{
			string[] array = this.Name.Split(new char[] { ':' });
			float num = float.Parse(array[1]);
			float num2 = (float)global::UnityEngine.Random.Range(0, 100);
			if (num2 > num)
			{
				return;
			}
		}
		Worker component2 = cmp.GetComponent<Worker>();
		string assetName = this.GetAssetName(cmp);
		StaminaMonitor.Instance smi = component2.GetSMI<StaminaMonitor.Instance>();
		if (!this.Name.Contains("sleep_") && smi != null && smi.IsSleeping())
		{
			return;
		}
		Vector3 position = cmp.transform.position;
		string sound = GlobalAssets.GetSound(assetName, true);
		if (sound != null)
		{
			if (this.looping)
			{
				LoopingSounds component3 = cmp.GetComponent<LoopingSounds>();
				if (component3 == null)
				{
					global::Debug.Log(cmp.name + " is missing LoopingSounds component. ", null);
				}
				else if (!component3.StartSound(sound, position))
				{
					Output.LogWarning(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", sound, cmp.name) });
				}
			}
			else
			{
				EventInstance eventInstance = SoundEvent.BeginOneShot(sound, position);
				if (sound.Contains("sleep_"))
				{
					Traits component4 = cmp.GetComponent<Traits>();
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
		MinionIdentity component = cmp.GetComponent<MinionIdentity>();
		if (component != null)
		{
			text = component.GetVoiceId();
		}
		string text2 = this.Name;
		if (this.Name.Contains(":"))
		{
			string[] array = this.Name.Split(new char[] { ':' });
			text2 = array[0];
		}
		return "DupVoc_" + text + "_" + text2;
	}

	public override void Stop(AnimEventManager.EventPlayerData behaviour)
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

	public float timeLastSpoke;

	public float intervalBetweenSpeaking = 10f;
}
