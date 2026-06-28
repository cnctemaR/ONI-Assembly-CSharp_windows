using System;
using System.Diagnostics;
using FMOD.Studio;
using UnityEngine;

[DebuggerDisplay("{Name}")]
public class SoundEvent : AnimEvent
{
	public SoundEvent()
	{
	}

	public SoundEvent(string file_name, string sound_name, int frame, float min_interval, bool is_looping)
		: base(file_name, sound_name, frame)
	{
		this.sound = GlobalAssets.GetSound(sound_name, false);
		this.soundHash = new HashedString(this.sound);
		if (this.sound == null || this.sound == string.Empty)
		{
		}
		this.minInterval = min_interval;
		this.looping = is_looping;
	}

	public string sound { get; private set; }

	public HashedString soundHash { get; private set; }

	public override bool ShouldPlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		if (this.sound == null || this.IsLowPrioritySound(this.sound))
		{
			return false;
		}
		CameraController instance = CameraController.Instance;
		SpeedControlScreen instance2 = SpeedControlScreen.Instance;
		if (instance != null && !instance.IsAudibleSound(behaviour.position, this.sound))
		{
			if (!this.looping && !GlobalAssets.IsHighPriority(this.sound))
			{
				return false;
			}
		}
		else if (instance2 != null && instance2.IsPaused)
		{
			return false;
		}
		return true;
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
		Vector3 position2 = behaviour.position;
		Vector3 vector = ((!this.playAtTarget) ? position : position2);
		if (AudioDebug.Get().debugSoundEvents)
		{
			global::UnityEngine.Debug.Log(string.Concat(new object[] { behaviour.name, ", ", this.sound, ", ", this.Frame, ", ", vector }));
		}
		try
		{
			if (this.looping)
			{
				LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
				if (component == null)
				{
					global::UnityEngine.Debug.Log(behaviour.name + " is missing LoopingSounds component. ");
				}
				else if (!component.StartSound(this.sound, vector))
				{
					Output.LogWarning(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", this.sound, behaviour.name) });
				}
			}
			else if (!SoundEvent.PlayOneShot(this.sound, vector))
			{
				Output.LogWarning(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", this.sound, behaviour.name) });
			}
		}
		catch (Exception ex)
		{
			string text = string.Format(("Error trying to trigger sound [{0}] in behaviour [{1}]" + this.sound == null) ? "null" : this.sound.ToString(), behaviour.GetType().ToString());
			Output.LogError(new object[] { text });
			throw new ArgumentException(text, ex);
		}
	}

	public static FMOD.Studio.EventInstance BeginOneShot(string ev, Vector3 pos)
	{
		FMOD.Studio.EventInstance eventInstance = KFMOD.BeginOneShot(ev, CameraController.Instance.GetVerticallyScaledPosition(pos));
		LoopingSoundManager.UpdateSpeed(eventInstance);
		return eventInstance;
	}

	public static FMOD.Studio.EventInstance BeginOneShot(FMOD.Studio.EventInstance instance, Vector3 pos)
	{
		FMOD.Studio.EventInstance eventInstance = KFMOD.BeginOneShot(instance, CameraController.Instance.GetVerticallyScaledPosition(pos));
		LoopingSoundManager.UpdateSpeed(eventInstance);
		return eventInstance;
	}

	public static bool EndOneShot(FMOD.Studio.EventInstance instance)
	{
		return KFMOD.EndOneShot(instance);
	}

	public static bool PlayOneShot(string sound, Vector3 pos)
	{
		bool flag = false;
		if (sound != null && sound != string.Empty)
		{
			FMOD.Studio.EventInstance eventInstance = SoundEvent.BeginOneShot(sound, pos);
			if (eventInstance != null)
			{
				flag = SoundEvent.EndOneShot(eventInstance);
			}
		}
		return flag;
	}

	public override void Stop(AnimEventManager.EventPlayerData behaviour)
	{
		if (this.looping)
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component != null)
			{
				component.StopSound(this.sound);
			}
		}
	}

	private bool IsLowPrioritySound(string sound)
	{
		return sound != null && Camera.main.orthographicSize > AudioMixer.LOW_PRIORITY_CUTOFF_DISTANCE && !AudioMixer.instance.activeNIS && GlobalAssets.IsLowPriority(sound);
	}

	public bool looping;

	public bool playAtTarget;

	public float minInterval;
}
