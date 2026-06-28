using System;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Parameter Trigger")]
	public class StudioParameterTrigger : MonoBehaviour
	{
		private void Start()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectStart);
		}

		private void OnDestroy()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectDestroy);
		}

		private void OnEnable()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectEnable);
		}

		private void OnDisable()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectDisable);
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

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerEnter2D);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerExit2D);
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

		private void OnCollisionEnter2D()
		{
			this.HandleGameEvent(EmitterGameEvent.CollisionEnter2D);
		}

		private void OnCollisionExit2D()
		{
			this.HandleGameEvent(EmitterGameEvent.CollisionExit2D);
		}

		private void HandleGameEvent(EmitterGameEvent gameEvent)
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
	}
}
