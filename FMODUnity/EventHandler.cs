using System;
using UnityEngine;

namespace FMODUnity
{
	public abstract class EventHandler : MonoBehaviour
	{
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
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag) || (other.attachedRigidbody && other.attachedRigidbody.CompareTag(this.CollisionTag)))
			{
				this.HandleGameEvent(EmitterGameEvent.TriggerEnter);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (string.IsNullOrEmpty(this.CollisionTag) || other.CompareTag(this.CollisionTag) || (other.attachedRigidbody && other.attachedRigidbody.CompareTag(this.CollisionTag)))
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

		private void OnMouseEnter()
		{
			this.HandleGameEvent(EmitterGameEvent.MouseEnter);
		}

		private void OnMouseExit()
		{
			this.HandleGameEvent(EmitterGameEvent.MouseExit);
		}

		private void OnMouseDown()
		{
			this.HandleGameEvent(EmitterGameEvent.MouseDown);
		}

		private void OnMouseUp()
		{
			this.HandleGameEvent(EmitterGameEvent.MouseUp);
		}

		protected abstract void HandleGameEvent(EmitterGameEvent gameEvent);

		public string CollisionTag = "";
	}
}
