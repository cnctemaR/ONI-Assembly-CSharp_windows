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

	public SoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, sound_name, frame)
	{
		if (do_load)
		{
			this.sound = GlobalAssets.GetSound(sound_name, false);
			this.soundHash = new HashedString(this.sound);
			if (this.sound == null || this.sound == string.Empty)
			{
			}
		}
		this.minInterval = min_interval;
		this.looping = is_looping;
		this.isDynamic = is_dynamic;
		this.noiseValues = SoundEventVolumeCache.instance.GetVolume(file_name, sound_name);
	}

	public string sound { get; private set; }

	public HashedString soundHash { get; private set; }

	public bool looping { get; private set; }

	public bool ignorePause { get; set; }

	public float minInterval { get; private set; }

	public EffectorValues noiseValues { get; set; }

	public static bool ShouldPlaySound(KBatchedAnimController controller, string sound, bool is_looping, bool is_dynamic)
	{
		CameraController instance = CameraController.Instance;
		if (instance == null)
		{
			return true;
		}
		SpeedControlScreen instance2 = SpeedControlScreen.Instance;
		if (is_dynamic)
		{
			return (!(instance2 != null) || !instance2.IsPaused) && instance.IsAudibleSound(controller.transform.GetPosition());
		}
		if (sound == null || SoundEvent.IsLowPrioritySound(sound))
		{
			return false;
		}
		if (!instance.IsAudibleSound(controller.transform.GetPosition(), sound))
		{
			if (!is_looping && !GlobalAssets.IsHighPriority(sound))
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
		if (SoundEvent.ShouldPlaySound(behaviour.controller, this.sound, this.looping, this.isDynamic))
		{
			this.PlaySound(behaviour);
		}
	}

	protected void PlaySound(AnimEventManager.EventPlayerData behaviour, string sound)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		AudioDebug audioDebug = AudioDebug.Get();
		if (audioDebug != null && audioDebug.debugSoundEvents)
		{
			global::Debug.Log(string.Concat(new object[] { behaviour.name, ", ", sound, ", ", base.frame, ", ", position }), null);
		}
		try
		{
			if (this.looping)
			{
				LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
				if (component == null)
				{
					global::Debug.Log(behaviour.name + " is missing LoopingSounds component. ", null);
				}
				else if (!component.StartSound(sound, behaviour, this.noiseValues, this.ignorePause))
				{
					Output.LogWarning(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", sound, behaviour.name) });
				}
			}
			else if (!SoundEvent.PlayOneShot(sound, behaviour, this.noiseValues))
			{
				Output.LogWarning(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", sound, behaviour.name) });
			}
		}
		catch (Exception ex)
		{
			string text = string.Format(("Error trying to trigger sound [{0}] in behaviour [{1}] [{2}]\n{3}" + sound == null) ? "null" : sound.ToString(), behaviour.GetType().ToString(), ex.Message, ex.StackTrace);
			global::Debug.LogError(text, null);
			throw new ArgumentException(text, ex);
		}
	}

	public virtual void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		this.PlaySound(behaviour, this.sound);
	}

	public static Vector3 GetCameraScaledPosition(Vector3 pos)
	{
		Vector3 vector = Vector3.zero;
		if (CameraController.Instance != null)
		{
			vector = CameraController.Instance.GetVerticallyScaledPosition(pos);
		}
		return vector;
	}

	public static FMOD.Studio.EventInstance BeginOneShot(string ev, Vector3 pos)
	{
		return KFMOD.BeginOneShot(ev, SoundEvent.GetCameraScaledPosition(pos));
	}

	public static bool EndOneShot(FMOD.Studio.EventInstance instance)
	{
		return KFMOD.EndOneShot(instance);
	}

	public static bool PlayOneShot(string sound, Vector3 pos)
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(sound))
		{
			FMOD.Studio.EventInstance eventInstance = SoundEvent.BeginOneShot(sound, pos);
			if (eventInstance.isValid())
			{
				flag = SoundEvent.EndOneShot(eventInstance);
			}
		}
		return flag;
	}

	public static bool PlayOneShot(string sound, AnimEventManager.EventPlayerData behaviour, EffectorValues noiseValues)
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(sound))
		{
			Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
			FMOD.Studio.EventInstance eventInstance = SoundEvent.BeginOneShot(sound, position);
			if (eventInstance.isValid())
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

	protected static bool IsLowPrioritySound(string sound)
	{
		return sound != null && Camera.main.orthographicSize > AudioMixer.LOW_PRIORITY_CUTOFF_DISTANCE && !AudioMixer.instance.activeNIS && GlobalAssets.IsLowPriority(sound);
	}

	protected void PrintSoundDebug(string anim_name, string sound, string sound_name, Vector3 sound_pos)
	{
		if (sound != null)
		{
			global::Debug.Log(string.Concat(new object[] { anim_name, ", ", sound_name, ", ", base.frame, ", ", sound_pos }), null);
		}
		else
		{
			global::Debug.Log("Missing sound: " + anim_name + ", " + sound_name, null);
		}
	}

	public static int IGNORE_INTERVAL = -1;

	protected bool isDynamic;
}
