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
			RuntimeManager.HasListener[this.ListenerNumber] = true;
			this.SetListenerLocation();
		}

		private void OnDisable()
		{
			RuntimeManager.HasListener[this.ListenerNumber] = false;
		}

		private void Update()
		{
			this.SetListenerLocation();
		}

		private void SetListenerLocation()
		{
			if (this.rigidBody)
			{
				RuntimeManager.SetListenerLocation(this.ListenerNumber, base.gameObject, this.rigidBody);
			}
			else
			{
				RuntimeManager.SetListenerLocation(this.ListenerNumber, base.gameObject, this.rigidBody2D);
			}
		}

		private Rigidbody rigidBody;

		private Rigidbody2D rigidBody2D;

		public int ListenerNumber = 0;
	}
}
