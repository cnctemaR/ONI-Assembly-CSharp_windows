using System;
using FMOD.Studio;
using UnityEngine;

[Serializable]
public class RemoteSoundEvent : SoundEvent
{
	public RemoteSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, true, false, min_interval, false)
	{
	}

	public override void PlaySound(AnimEventManager.EventPlayerData behaviour)
	{
		Vector3 position = behaviour.GetComponent<Transform>().GetPosition();
		Workable workable = behaviour.GetComponent<Worker>().workable;
		if (workable != null)
		{
			Toggleable component = workable.GetComponent<Toggleable>();
			if (component != null)
			{
				IToggleHandler toggleHandlerForWorker = component.GetToggleHandlerForWorker(behaviour.GetComponent<Worker>());
				float num = 1f;
				if (toggleHandlerForWorker != null && toggleHandlerForWorker.IsHandlerOn())
				{
					num = 0f;
				}
				EventInstance eventInstance = SoundEvent.BeginOneShot(base.sound, position, 1f);
				eventInstance.setParameterValue("State", num);
				SoundEvent.EndOneShot(eventInstance);
			}
		}
	}

	private const string STATE_PARAMETER = "State";
}
