using System;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Listener")]
	public class StudioListener : MonoBehaviour
	{
		private void OnEnable()
		{
			RuntimeUtils.EnforceLibraryOrder();
			this.rigidBody = base.gameObject.GetComponent<Rigidbody>();
			this.rigidBody2D = base.gameObject.GetComponent<Rigidbody2D>();
			this.ListenerNumber = RuntimeManager.AddListener(this);
		}

		private void OnDisable()
		{
			RuntimeManager.RemoveListener(this);
		}

		private void Update()
		{
			if (this.ListenerNumber >= 0 && this.ListenerNumber < 8)
			{
				this.SetListenerLocation();
			}
		}

		private void SetListenerLocation()
		{
			if (this.rigidBody)
			{
				RuntimeManager.SetListenerLocation(this.ListenerNumber, base.gameObject, this.rigidBody, this.attenuationObject);
				return;
			}
			if (this.rigidBody2D)
			{
				RuntimeManager.SetListenerLocation(this.ListenerNumber, base.gameObject, this.rigidBody2D, this.attenuationObject);
				return;
			}
			RuntimeManager.SetListenerLocation(this.ListenerNumber, base.gameObject, this.attenuationObject);
		}

		private Rigidbody rigidBody;

		private Rigidbody2D rigidBody2D;

		public GameObject attenuationObject;

		public int ListenerNumber = -1;
	}
}
