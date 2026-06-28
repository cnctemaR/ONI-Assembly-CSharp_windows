using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;

namespace FMODUnity
{
	public class OneshotList
	{
		public void Add(EventInstance instance)
		{
			this.instances.Add(instance);
		}

		public void Update(ATTRIBUTES_3D attributes)
		{
			PLAYBACK_STATE state;
			List<EventInstance> list = this.instances.FindAll(delegate(EventInstance x)
			{
				x.getPlaybackState(out state);
				return state == PLAYBACK_STATE.STOPPED;
			});
			foreach (EventInstance eventInstance in list)
			{
				eventInstance.release();
			}
			this.instances.RemoveAll((EventInstance x) => !x.isValid());
			foreach (EventInstance eventInstance2 in this.instances)
			{
				eventInstance2.set3DAttributes(attributes);
			}
		}

		public void SetParameterValue(string name, float value)
		{
			foreach (EventInstance eventInstance in this.instances)
			{
				eventInstance.setParameterValue(name, value);
			}
		}

		public void StopAll(STOP_MODE stopMode)
		{
			foreach (EventInstance eventInstance in this.instances)
			{
				eventInstance.stop(stopMode);
				eventInstance.release();
			}
			this.instances.Clear();
		}

		private List<EventInstance> instances = new List<EventInstance>();
	}
}
