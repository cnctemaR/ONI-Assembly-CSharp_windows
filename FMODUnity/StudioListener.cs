using System;
using System.Collections.Generic;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Listener")]
	public class StudioListener : MonoBehaviour
	{
		public static int ListenerCount
		{
			get
			{
				return StudioListener.listeners.Count;
			}
		}

		public int ListenerNumber
		{
			get
			{
				return StudioListener.listeners.IndexOf(this);
			}
		}

		public static float DistanceToNearestListener(Vector3 position)
		{
			float num = float.MaxValue;
			for (int i = 0; i < StudioListener.listeners.Count; i++)
			{
				num = Mathf.Min(num, Vector3.Distance(position, StudioListener.listeners[i].transform.position));
			}
			return num;
		}

		public static float DistanceSquaredToNearestListener(Vector3 position)
		{
			float num = float.MaxValue;
			for (int i = 0; i < StudioListener.listeners.Count; i++)
			{
				num = Mathf.Min(num, (position - StudioListener.listeners[i].transform.position).sqrMagnitude);
			}
			return num;
		}

		private static void AddListener(StudioListener listener)
		{
			if (StudioListener.listeners.Contains(listener))
			{
				Debug.LogWarning(string.Format("[FMOD] Listener has already been added at index {0}.", listener.ListenerNumber));
				return;
			}
			if (StudioListener.listeners.Count >= 8)
			{
				Debug.LogWarning(string.Format("[FMOD] Max number of listeners reached : {0}.", 8));
			}
			StudioListener.listeners.Add(listener);
			RuntimeManager.StudioSystem.setNumListeners(Mathf.Clamp(StudioListener.listeners.Count, 1, 8));
		}

		private static void RemoveListener(StudioListener listener)
		{
			StudioListener.listeners.Remove(listener);
			RuntimeManager.StudioSystem.setNumListeners(Mathf.Clamp(StudioListener.listeners.Count, 1, 8));
		}

		private void OnEnable()
		{
			RuntimeUtils.EnforceLibraryOrder();
			this.rigidBody = base.gameObject.GetComponent<Rigidbody>();
			this.rigidBody2D = base.gameObject.GetComponent<Rigidbody2D>();
			StudioListener.AddListener(this);
		}

		private void OnDisable()
		{
			StudioListener.RemoveListener(this);
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

		[SerializeField]
		private GameObject attenuationObject;

		private Rigidbody rigidBody;

		private Rigidbody2D rigidBody2D;

		private static List<StudioListener> listeners = new List<StudioListener>();
	}
}
