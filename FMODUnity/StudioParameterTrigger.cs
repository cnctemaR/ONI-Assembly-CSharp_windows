using System;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Parameter Trigger")]
	public class StudioParameterTrigger : EventHandler
	{
		private void Awake()
		{
			for (int i = 0; i < this.Emitters.Length; i++)
			{
				EmitterRef emitterRef = this.Emitters[i];
				if (emitterRef.Target != null && !emitterRef.Target.EventReference.IsNull)
				{
					EventDescription eventDescription = RuntimeManager.GetEventDescription(emitterRef.Target.EventReference);
					if (eventDescription.isValid())
					{
						for (int j = 0; j < this.Emitters[i].Params.Length; j++)
						{
							PARAMETER_DESCRIPTION parameter_DESCRIPTION;
							eventDescription.getParameterDescriptionByName(emitterRef.Params[j].Name, out parameter_DESCRIPTION);
							emitterRef.Params[j].ID = parameter_DESCRIPTION.id;
						}
					}
				}
			}
		}

		protected override void HandleGameEvent(EmitterGameEvent gameEvent)
		{
			if (this.TriggerEvent == gameEvent)
			{
				this.TriggerParameters();
			}
		}

		public void TriggerParameters()
		{
			for (int i = 0; i < this.Emitters.Length; i++)
			{
				EmitterRef emitterRef = this.Emitters[i];
				if (emitterRef.Target != null && emitterRef.Target.EventInstance.isValid())
				{
					for (int j = 0; j < this.Emitters[i].Params.Length; j++)
					{
						emitterRef.Target.EventInstance.setParameterByID(this.Emitters[i].Params[j].ID, this.Emitters[i].Params[j].Value, false);
					}
				}
			}
		}

		public EmitterRef[] Emitters;

		public EmitterGameEvent TriggerEvent;
	}
}
