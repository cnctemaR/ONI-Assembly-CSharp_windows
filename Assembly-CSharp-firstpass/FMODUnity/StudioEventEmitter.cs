using System;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Event Emitter")]
	public class StudioEventEmitter : MonoBehaviour
	{
		private void Start()
		{
			RuntimeUtils.EnforceLibraryOrder();
			this.HandleGameEvent(EmitterGameEvent.LevelStart);
		}

		private void OnApplicationQuit()
		{
			this.isQuitting = true;
		}

		private void OnDestroy()
		{
			if (!this.isQuitting)
			{
				this.HandleGameEvent(EmitterGameEvent.LevelEnd);
				if (this.instance != null && this.instance.isValid())
				{
					RuntimeManager.DetachInstanceFromGameObject(this.instance);
				}
			}
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
			if (this.PlayEvent == gameEvent)
			{
				this.Play();
			}
			if (this.StopEvent == gameEvent)
			{
				this.Stop();
			}
		}

		private void Lookup()
		{
			this.eventDescription = RuntimeManager.GetEventDescription(this.Event);
		}

		public void Play()
		{
			if (this.TriggerOnce && this.hasTriggered)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.Event))
			{
				return;
			}
			if (this.eventDescription == null)
			{
				this.Lookup();
			}
			bool flag = false;
			if (!this.Event.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
			{
				this.eventDescription.isOneshot(out flag);
			}
			bool flag2;
			this.eventDescription.is3D(out flag2);
			if (this.instance != null && !this.instance.isValid())
			{
				this.instance = null;
			}
			if (flag && this.instance != null)
			{
				this.instance.release();
				this.instance = null;
			}
			if (this.instance == null)
			{
				this.eventDescription.createInstance(out this.instance);
				if (flag2)
				{
					Rigidbody component = base.GetComponent<Rigidbody>();
					Transform component2 = base.GetComponent<Transform>();
					this.instance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, component));
					RuntimeManager.AttachInstanceToGameObject(this.instance, component2, component);
				}
			}
			foreach (ParamRef paramRef in this.Params)
			{
				this.instance.setParameterValue(paramRef.Name, paramRef.Value);
			}
			this.instance.start();
			this.hasTriggered = true;
		}

		public void Stop()
		{
			if (this.instance != null)
			{
				this.instance.stop((!this.AllowFadeout) ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
				this.instance.release();
				this.instance = null;
			}
		}

		public void SetParameter(string name, float value)
		{
			if (this.instance != null)
			{
				this.instance.setParameterValue(name, value);
			}
		}

		public bool IsPlaying()
		{
			if (this.instance != null && this.instance.isValid())
			{
				PLAYBACK_STATE playback_STATE;
				this.instance.getPlaybackState(out playback_STATE);
				return playback_STATE != PLAYBACK_STATE.STOPPED;
			}
			return false;
		}

		[EventRef]
		public string Event;

		public EmitterGameEvent PlayEvent;

		public EmitterGameEvent StopEvent;

		public string CollisionTag;

		public bool AllowFadeout = true;

		public bool TriggerOnce;

		public ParamRef[] Params;

		private EventDescription eventDescription;

		private EventInstance instance;

		private bool hasTriggered;

		private bool isQuitting;
	}
}
