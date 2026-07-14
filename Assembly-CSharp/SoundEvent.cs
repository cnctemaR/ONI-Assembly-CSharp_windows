using System;
using System.Diagnostics;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

[DebuggerDisplay("{name}")]
public class SoundEvent : AnimEvent
{
	public string sound { get; private set; }

	public HashedString soundHash { get; private set; }

	public bool looping { get; private set; }

	public bool ignorePause { get; set; }

	public bool shouldCameraScalePosition { get; set; }

	public float minInterval { get; private set; }

	public bool objectIsSelectedAndVisible { get; set; }

	public EffectorValues noiseValues { get; set; }

	public SoundEvent()
	{
	}

	public SoundEvent(string file_name, string sound_name, int frame, bool do_load, bool is_looping, float min_interval, bool is_dynamic)
		: base(file_name, sound_name, frame)
	{
		this.shouldCameraScalePosition = true;
		if (do_load)
		{
			this.sound = GlobalAssets.GetSound(sound_name, false);
			this.soundHash = new HashedString(this.sound);
			string.IsNullOrEmpty(this.sound);
		}
		this.minInterval = min_interval;
		this.looping = is_looping;
		this.isDynamic = is_dynamic;
		this.noiseValues = SoundEventVolumeCache.instance.GetVolume(file_name, sound_name);
	}

	public static bool ObjectIsSelectedAndVisible(GameObject go)
	{
		return false;
	}

	public static Vector3 AudioHighlightListenerPosition(Vector3 sound_pos)
	{
		Vector3 position = SoundListenerController.Instance.transform.position;
		float num = 1f * sound_pos.x + 0f * position.x;
		float num2 = 1f * sound_pos.y + 0f * position.y;
		float num3 = 0f * position.z;
		return new Vector3(num, num2, num3);
	}

	public static float GetVolume(bool objectIsSelectedAndVisible)
	{
		float num = 1f;
		if (objectIsSelectedAndVisible)
		{
			num = 1f;
		}
		return num;
	}

	public static bool ShouldPlaySound(KBatchedAnimController controller, string sound, bool is_looping, bool is_dynamic)
	{
		return SoundEvent.ShouldPlaySound(controller, sound, sound, is_looping, is_dynamic);
	}

	public static bool ShouldPlaySound(KBatchedAnimController controller, string sound, HashedString soundHash, bool is_looping, bool is_dynamic)
	{
		CameraController instance = CameraController.Instance;
		if (instance == null)
		{
			return true;
		}
		Vector3 position = controller.transform.GetPosition();
		Vector3 offset = controller.Offset;
		position.x += offset.x;
		position.y += offset.y;
		if (!SoundCuller.IsAudibleWorld(position))
		{
			return false;
		}
		SpeedControlScreen instance2 = SpeedControlScreen.Instance;
		if (is_dynamic)
		{
			return (!(instance2 != null) || !instance2.IsPaused) && instance.IsAudibleSound(position);
		}
		if (sound == null || SoundEvent.IsLowPrioritySound(sound))
		{
			return false;
		}
		if (!instance.IsAudibleSound(position, soundHash))
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
		GameObject gameObject = behaviour.controller.gameObject;
		this.objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(gameObject);
		if (this.objectIsSelectedAndVisible || SoundEvent.ShouldPlaySound(behaviour.controller, this.sound, this.soundHash, this.looping, this.isDynamic))
		{
			this.PlaySound(behaviour);
		}
	}

	protected void PlaySound(AnimEventManager.EventPlayerData behaviour, string sound)
	{
		Vector3 vector = behaviour.controller.transform.GetPosition();
		vector.z = 0f;
		if (SoundEvent.ObjectIsSelectedAndVisible(behaviour.controller.gameObject))
		{
			vector = SoundEvent.AudioHighlightListenerPosition(vector);
		}
		KBatchedAnimController controller = behaviour.controller;
		if (controller != null)
		{
			Vector3 offset = controller.Offset;
			vector.x += offset.x;
			vector.y += offset.y;
		}
		AudioDebug audioDebug = AudioDebug.Get();
		if (audioDebug != null && audioDebug.debugSoundEvents)
		{
			global::Debug.Log(string.Concat(new string[]
			{
				behaviour.name,
				", ",
				sound,
				", ",
				base.frame.ToString(),
				", ",
				vector.ToString()
			}));
		}
		try
		{
			if (this.looping)
			{
				LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
				if (component == null)
				{
					global::Debug.Log(behaviour.name + " is missing LoopingSounds component. ");
				}
				else if (!component.StartSound(sound, behaviour, this.noiseValues, this.ignorePause, this.shouldCameraScalePosition))
				{
					DebugUtil.LogWarningArgs(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", sound, behaviour.name) });
				}
			}
			else if (!SoundEvent.PlayOneShot(sound, behaviour, this.noiseValues, SoundEvent.GetVolume(this.objectIsSelectedAndVisible), this.objectIsSelectedAndVisible))
			{
				DebugUtil.LogWarningArgs(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", sound, behaviour.name) });
			}
		}
		catch (Exception ex)
		{
			string text = string.Format(("Error trying to trigger sound [{0}] in behaviour [{1}] [{2}]\n{3}" + sound != null) ? sound.ToString() : "null", behaviour.GetType().ToString(), ex.Message, ex.StackTrace);
			global::Debug.LogError(text);
			throw new ArgumentException(text, ex);
		}
	}

	public virtual void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		this.PlaySound(behaviour, this.sound);
	}

	public static Vector3 GetCameraScaledPosition(Vector3 pos, bool objectIsSelectedAndVisible = false)
	{
		Vector3 vector = Vector3.zero;
		if (CameraController.Instance != null)
		{
			vector = CameraController.Instance.GetVerticallyScaledPosition(pos, objectIsSelectedAndVisible);
		}
		return vector;
	}

	public static FMOD.Studio.EventInstance BeginOneShot(EventReference event_ref, Vector3 pos, float volume = 1f, bool objectIsSelectedAndVisible = false)
	{
		return KFMOD.BeginOneShot(event_ref, SoundEvent.GetCameraScaledPosition(pos, objectIsSelectedAndVisible), volume);
	}

	public static FMOD.Studio.EventInstance BeginOneShot(string ev, Vector3 pos, float volume = 1f, bool objectIsSelectedAndVisible = false)
	{
		return SoundEvent.BeginOneShot(RuntimeManager.PathToEventReference(ev), pos, volume, false);
	}

	public static bool EndOneShot(FMOD.Studio.EventInstance instance)
	{
		return KFMOD.EndOneShot(instance);
	}

	public static bool PlayOneShot(EventReference event_ref, Vector3 sound_pos, float volume = 1f)
	{
		bool flag = false;
		if (!event_ref.IsNull)
		{
			FMOD.Studio.EventInstance eventInstance = SoundEvent.BeginOneShot(event_ref, sound_pos, volume, false);
			if (eventInstance.isValid())
			{
				flag = SoundEvent.EndOneShot(eventInstance);
			}
		}
		return flag;
	}

	public static bool PlayOneShot(string sound, Vector3 sound_pos, float volume = 1f)
	{
		return SoundEvent.PlayOneShot(RuntimeManager.PathToEventReference(sound), sound_pos, volume);
	}

	public static bool PlayOneShot(string sound, AnimEventManager.EventPlayerData behaviour, EffectorValues noiseValues, float volume = 1f, bool objectIsSelectedAndVisible = false)
	{
		bool flag = false;
		if (!string.IsNullOrEmpty(sound))
		{
			Vector3 vector = behaviour.controller.transform.GetPosition();
			vector.z = 0f;
			if (objectIsSelectedAndVisible)
			{
				vector = SoundEvent.AudioHighlightListenerPosition(vector);
			}
			FMOD.Studio.EventInstance eventInstance = SoundEvent.BeginOneShot(sound, vector, volume, false);
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
		return sound != null && Game.Instance != null && Game.MainCamera.orthographicSize > AudioMixer.LOW_PRIORITY_CUTOFF_DISTANCE && !AudioMixer.instance.activeNIS && GlobalAssets.IsLowPriority(sound);
	}

	protected void PrintSoundDebug(string anim_name, string sound, string sound_name, Vector3 sound_pos)
	{
		if (sound != null)
		{
			global::Debug.Log(string.Concat(new string[]
			{
				anim_name,
				", ",
				sound_name,
				", ",
				base.frame.ToString(),
				", ",
				sound_pos.ToString()
			}));
			return;
		}
		global::Debug.Log("Missing sound: " + anim_name + ", " + sound_name);
	}

	public static int IGNORE_INTERVAL = -1;

	protected bool isDynamic;
}
