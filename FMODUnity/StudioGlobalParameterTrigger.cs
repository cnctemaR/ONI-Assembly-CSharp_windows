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
		public PARAMETER_DESCRIPTION ParameterDesctription
		{
			get
			{
				return this.parameterDescription;
			}
		}

		private RESULT Lookup()
		{
			return RuntimeManager.StudioSystem.getParameterDescriptionByName(this.Parameter, out this.parameterDescription);
		}

		private void Awake()
		{
			if (string.IsNullOrEmpty(this.parameterDescription.name))
			{
				this.Lookup();
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
				RESULT result = RuntimeManager.StudioSystem.setParameterByID(this.parameterDescription.id, this.Value, false);
				if (result != RESULT.OK)
				{
					RuntimeUtils.DebugLogError(string.Format("[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}", this.Parameter, result));
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
