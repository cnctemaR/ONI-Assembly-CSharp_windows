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
			RuntimeManager.HasListener = true;
			RuntimeManager.SetListenerLocation(base.gameObject, this.rigidBody);
		}

		private void OnDisable()
		{
			RuntimeManager.HasListener = false;
		}

		private void Update()
		{
			RuntimeManager.SetListenerLocation(base.gameObject, this.rigidBody);
		}

		private Rigidbody rigidBody;
	}
}
