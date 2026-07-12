using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Serialization;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Global Parameter Trigger")]
	public class StudioGlobalParameterTrigger : EventHandler
	{
		public PARAMETER_DESCRIPTION ParameterDescription
		{
			get
			{
				return this.parameterDescription;
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
			if (!string.IsNullOrEmpty(this.Parameter))
			{
				RESULT result;
				if (string.IsNullOrEmpty(this.parameterDescription.name))
				{
					result = RuntimeManager.StudioSystem.getParameterDescriptionByName(this.Parameter, out this.parameterDescription);
					if (result != RESULT.OK)
					{
						RuntimeUtils.DebugLogError(string.Format("[FMOD] StudioGlobalParameterTrigger failed to lookup parameter {0} : result = {1}", this.Parameter, result));
						return;
					}
				}
				result = RuntimeManager.StudioSystem.setParameterByID(this.parameterDescription.id, this.Value, false);
				if (result != RESULT.OK)
				{
					RuntimeUtils.DebugLogError(string.Format("[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}", this.Parameter, result));
					return;
				}
			}
		}

		[ParamRef]
		[FormerlySerializedAs("parameter")]
		public string Parameter;

		public EmitterGameEvent TriggerEvent;

		[FormerlySerializedAs("value")]
		public float Value;

		private PARAMETER_DESCRIPTION parameterDescription;
	}
}
