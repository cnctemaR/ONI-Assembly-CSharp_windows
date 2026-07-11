using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PhonoboxSoundEvent : SoundEvent
{
	public PhonoboxSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, true, min_interval, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		position.z = 0f;
		AudioDebug audioDebug = AudioDebug.Get();
		if (audioDebug != null && audioDebug.debugSoundEvents)
		{
			global::Debug.Log(string.Concat(new object[] { behaviour.name, ", ", base.sound, ", ", base.frame, ", ", position }));
		}
		try
		{
			LoopingSounds component = behaviour.GetComponent<LoopingSounds>();
			if (component == null)
			{
				global::Debug.Log(behaviour.name + " is missing LoopingSounds component. ");
			}
			else if (!component.IsSoundPlaying(base.sound))
			{
				if (component.StartSound(base.sound, behaviour, base.noiseValues, base.ignorePause, true))
				{
					EventDescription eventDescription = RuntimeManager.GetEventDescription(base.sound);
					PARAMETER_DESCRIPTION parameter_DESCRIPTION;
					eventDescription.getParameter("jukeboxSong", out parameter_DESCRIPTION);
					int num = (int)parameter_DESCRIPTION.maximum;
					PARAMETER_DESCRIPTION parameter_DESCRIPTION2;
					eventDescription.getParameter("jukeboxPitch", out parameter_DESCRIPTION2);
					int num2 = (int)parameter_DESCRIPTION2.maximum;
					this.song = global::UnityEngine.Random.Range(0, num + 1);
					this.pitch = global::UnityEngine.Random.Range(0, num2 + 1);
					component.UpdateFirstParameter(base.sound, "jukeboxSong", (float)this.song);
					component.UpdateSecondParameter(base.sound, "jukeboxPitch", (float)this.pitch);
				}
				else
				{
					DebugUtil.LogWarningArgs(new object[] { string.Format("SoundEvent has invalid sound [{0}] on behaviour [{1}]", base.sound, behaviour.name) });
				}
			}
		}
		catch (Exception ex)
		{
			string text = string.Format(("Error trying to trigger sound [{0}] in behaviour [{1}] [{2}]\n{3}" + base.sound != null) ? base.sound.ToString() : "null", behaviour.GetType().ToString(), ex.Message, ex.StackTrace);
			global::Debug.LogError(text);
			throw new ArgumentException(text, ex);
		}
	}

	private const string SOUND_PARAM_SONG = "jukeboxSong";

	private const string SOUND_PARAM_PITCH = "jukeboxPitch";

	private int song;

	private int pitch;
}
