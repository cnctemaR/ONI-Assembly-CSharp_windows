using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;

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
			return RuntimeManager.StudioSystem.getParameterDescriptionByName(this.parameter, out this.parameterDescription);
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
			if (!string.IsNullOrEmpty(this.parameter))
			{
				RESULT result = RuntimeManager.StudioSystem.setParameterByID(this.parameterDescription.id, this.value, false);
				if (result != RESULT.OK)
				{
					global::UnityEngine.Debug.LogError(string.Format("[FMOD] StudioGlobalParameterTrigger failed to set parameter {0} : result = {1}", this.parameter, result));
				}
			}
		}

		[ParamRef]
		public string parameter;

		public EmitterGameEvent TriggerEvent;

		public float value;

		private PARAMETER_DESCRIPTION parameterDescription;
	}
}
