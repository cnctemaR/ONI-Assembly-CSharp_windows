using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace FMODUnity
{
	public abstract class EventHandler : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
	{
		protected virtual void Start()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectStart);
		}

		protected virtual void OnDestroy()
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
			this.HandleGameEvent(EmitterGameEvent.ObjectMouseEnter);
		}

		private void OnMouseExit()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectMouseExit);
		}

		private void OnMouseDown()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectMouseDown);
		}

		private void OnMouseUp()
		{
			this.HandleGameEvent(EmitterGameEvent.ObjectMouseUp);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			this.HandleGameEvent(EmitterGameEvent.UIMouseEnter);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			this.HandleGameEvent(EmitterGameEvent.UIMouseExit);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			this.HandleGameEvent(EmitterGameEvent.UIMouseDown);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			this.HandleGameEvent(EmitterGameEvent.UIMouseUp);
		}

		protected abstract void HandleGameEvent(EmitterGameEvent gameEvent);

		public string CollisionTag = "";
	}
}
