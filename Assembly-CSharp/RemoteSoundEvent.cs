using System;
using FMOD.Studio;
using UnityEngine;

[Serializable]
public class RemoteSoundEvent : SoundEvent
{
	public RemoteSoundEvent(string file_name, string sound_name, int frame, float min_interval)
		: base(file_name, sound_name, frame, min_interval, false)
	{
	}

	public override void OnPlay(IAnimBehaviour behaviour)
	{
		if (base.IsFilteredOut(behaviour))
		{
			return;
		}
		Vector3 position = behaviour.GetComponent<Transform>().position;
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
				EventInstance eventInstance = SoundEvent.BeginOneShot(this.sound, position);
				eventInstance.setParameterValue("State", num);
				SoundEvent.EndOneShot(eventInstance);
			}
		}
	}
}
