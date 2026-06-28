using System;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Parameter Trigger")]
	public class StudioParameterTrigger : MonoBehaviour
	{
		private void OnEnable()
		{
		}

		private void OnDestroy()
		{
			this.HandleGameEvent(EmitterGameEvent.LevelEnd);
		}

		private void OnTriggerEnter(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerEnter);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerExit);
			}
		}

		private void OnCollisionEnter()
		{
			this.HandleGameEvent(EmitterGameEvent.CollisionEnter);
		}

		private void OnCollisionExit()
		{
			this.HandleGameEvent(EmitterGameEvent.CollisionExit);
		}

		private void HandleGameEvent(EmitterGameEvent gameEvent)
		{
			if (this.TriggerEvent == gameEvent)
			{
				this.TriggerParameters();
			}
		}

		private void Update()
		{
			if (this.firstUpdate)
			{
				this.HandleGameEvent(EmitterGameEvent.LevelStart);
				this.firstUpdate = false;
				base.enabled = false;
			}
		}

		public void TriggerParameters()
		{
			for (int i = 0; i < this.Emitters.Length; i++)
			{
				EmitterRef emitterRef = this.Emitters[i];
				if (emitterRef.Target != null)
				{
					for (int j = 0; j < this.Emitters[i].Params.Length; j++)
					{
						emitterRef.Target.SetParameter(this.Emitters[i].Params[j].Name, this.Emitters[i].Params[j].Value);
					}
				}
			}
		}

		public EmitterRef[] Emitters;

		public EmitterGameEvent TriggerEvent;

		public string CollisionTag;

		private bool firstUpdate = true;
	}
}
