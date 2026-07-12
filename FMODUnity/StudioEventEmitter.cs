using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

namespace FMODUnity
{
	[AddComponentMenu("FMOD Studio/FMOD Studio Event Emitter")]
	public class StudioEventEmitter : EventHandler
	{
		public EventDescription EventDescription
		{
			get
			{
				return this.eventDescription;
			}
		}

		public EventInstance EventInstance
		{
			get
			{
				return this.instance;
			}
		}

		public bool IsActive { get; private set; }

		private float MaxDistance
		{
			get
			{
				if (this.OverrideAttenuation)
				{
					return this.OverrideMaxDistance;
				}
				if (!this.eventDescription.isValid())
				{
					this.Lookup();
				}
				float num;
				float num2;
				this.eventDescription.getMinMaxDistance(out num, out num2);
				return num2;
			}
		}

		public static void UpdateActiveEmitters()
		{
			foreach (StudioEventEmitter studioEventEmitter in StudioEventEmitter.activeEmitters)
			{
				studioEventEmitter.UpdatePlayingStatus(false);
			}
		}

		private static void RegisterActiveEmitter(StudioEventEmitter emitter)
		{
			if (!StudioEventEmitter.activeEmitters.Contains(emitter))
			{
				StudioEventEmitter.activeEmitters.Add(emitter);
			}
		}

		private static void DeregisterActiveEmitter(StudioEventEmitter emitter)
		{
			StudioEventEmitter.activeEmitters.Remove(emitter);
		}

		private void UpdatePlayingStatus(bool force = false)
		{
			bool flag = StudioListener.DistanceToNearestListener(base.transform.position) <= this.MaxDistance;
			if (force || flag != this.IsPlaying())
			{
				if (flag)
				{
					this.PlayInstance();
					return;
				}
				this.StopInstance();
			}
		}

		protected override void Start()
		{
			RuntimeUtils.EnforceLibraryOrder();
			if (this.Preload)
			{
				this.Lookup();
				this.eventDescription.loadSampleData();
			}
			this.HandleGameEvent(EmitterGameEvent.ObjectStart);
		}

		private void OnApplicationQuit()
		{
			this.isQuitting = true;
		}

		protected override void OnDestroy()
		{
			if (!this.isQuitting)
			{
				this.HandleGameEvent(EmitterGameEvent.ObjectDestroy);
				if (this.instance.isValid())
				{
					RuntimeManager.DetachInstanceFromGameObject(this.instance);
					if (this.eventDescription.isValid() && this.isOneshot)
					{
						this.instance.release();
						this.instance.clearHandle();
					}
				}
				StudioEventEmitter.DeregisterActiveEmitter(this);
				if (this.Preload)
				{
					this.eventDescription.unloadSampleData();
				}
			}
		}

		protected override void HandleGameEvent(EmitterGameEvent gameEvent)
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
			this.eventDescription = RuntimeManager.GetEventDescription(this.EventReference);
			if (this.eventDescription.isValid())
			{
				for (int i = 0; i < this.Params.Length; i++)
				{
					PARAMETER_DESCRIPTION parameter_DESCRIPTION;
					this.eventDescription.getParameterDescriptionByName(this.Params[i].Name, out parameter_DESCRIPTION);
					this.Params[i].ID = parameter_DESCRIPTION.id;
				}
			}
		}

		public void Play()
		{
			if (this.TriggerOnce && this.hasTriggered)
			{
				return;
			}
			if (this.EventReference.IsNull)
			{
				return;
			}
			this.cachedParams.Clear();
			if (!this.eventDescription.isValid())
			{
				this.Lookup();
			}
			bool flag;
			this.eventDescription.isSnapshot(out flag);
			if (!flag)
			{
				this.eventDescription.isOneshot(out this.isOneshot);
			}
			bool flag2;
			this.eventDescription.is3D(out flag2);
			this.IsActive = true;
			if (flag2 && !this.isOneshot && Settings.Instance.StopEventsOutsideMaxDistance)
			{
				StudioEventEmitter.RegisterActiveEmitter(this);
				this.UpdatePlayingStatus(true);
				return;
			}
			this.PlayInstance();
		}

		private void PlayInstance()
		{
			if (!this.instance.isValid())
			{
				this.instance.clearHandle();
			}
			if (this.isOneshot && this.instance.isValid())
			{
				this.instance.release();
				this.instance.clearHandle();
			}
			bool flag;
			this.eventDescription.is3D(out flag);
			if (!this.instance.isValid())
			{
				this.eventDescription.createInstance(out this.instance);
				if (flag)
				{
					Transform component = base.GetComponent<Transform>();
					if (base.GetComponent<Rigidbody>())
					{
						Rigidbody component2 = base.GetComponent<Rigidbody>();
						this.instance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, component2));
						RuntimeManager.AttachInstanceToGameObject(this.instance, component, component2);
					}
					else if (base.GetComponent<Rigidbody2D>())
					{
						Rigidbody2D component3 = base.GetComponent<Rigidbody2D>();
						this.instance.set3DAttributes(RuntimeUtils.To3DAttributes(base.gameObject, component3));
						RuntimeManager.AttachInstanceToGameObject(this.instance, component, component3);
					}
					else
					{
						this.instance.set3DAttributes(base.gameObject.To3DAttributes());
						RuntimeManager.AttachInstanceToGameObject(this.instance, component);
					}
				}
			}
			foreach (ParamRef paramRef in this.Params)
			{
				this.instance.setParameterByID(paramRef.ID, paramRef.Value, false);
			}
			foreach (ParamRef paramRef2 in this.cachedParams)
			{
				this.instance.setParameterByID(paramRef2.ID, paramRef2.Value, false);
			}
			if (flag && this.OverrideAttenuation)
			{
				this.instance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, this.OverrideMinDistance);
				this.instance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, this.OverrideMaxDistance);
			}
			this.instance.start();
			this.hasTriggered = true;
		}

		public void Stop()
		{
			StudioEventEmitter.DeregisterActiveEmitter(this);
			this.IsActive = false;
			this.cachedParams.Clear();
			this.StopInstance();
		}

		private void StopInstance()
		{
			if (this.TriggerOnce && this.hasTriggered)
			{
				StudioEventEmitter.DeregisterActiveEmitter(this);
			}
			if (this.instance.isValid())
			{
				this.instance.stop(this.AllowFadeout ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
				this.instance.release();
				this.instance.clearHandle();
			}
		}

		public void SetParameter(string name, float value, bool ignoreseekspeed = false)
		{
			if (Settings.Instance.StopEventsOutsideMaxDistance && this.IsActive)
			{
				ParamRef paramRef = this.cachedParams.Find((ParamRef x) => x.Name == name);
				if (paramRef == null)
				{
					PARAMETER_DESCRIPTION parameter_DESCRIPTION;
					this.eventDescription.getParameterDescriptionByName(name, out parameter_DESCRIPTION);
					paramRef = new ParamRef();
					paramRef.ID = parameter_DESCRIPTION.id;
					paramRef.Name = parameter_DESCRIPTION.name;
					this.cachedParams.Add(paramRef);
				}
				paramRef.Value = value;
			}
			if (this.instance.isValid())
			{
				this.instance.setParameterByName(name, value, ignoreseekspeed);
			}
		}

		public void SetParameter(PARAMETER_ID id, float value, bool ignoreseekspeed = false)
		{
			if (Settings.Instance.StopEventsOutsideMaxDistance && this.IsActive)
			{
				ParamRef paramRef = this.cachedParams.Find((ParamRef x) => x.ID.Equals(id));
				if (paramRef == null)
				{
					PARAMETER_DESCRIPTION parameter_DESCRIPTION;
					this.eventDescription.getParameterDescriptionByID(id, out parameter_DESCRIPTION);
					paramRef = new ParamRef();
					paramRef.ID = parameter_DESCRIPTION.id;
					paramRef.Name = parameter_DESCRIPTION.name;
					this.cachedParams.Add(paramRef);
				}
				paramRef.Value = value;
			}
			if (this.instance.isValid())
			{
				this.instance.setParameterByID(id, value, ignoreseekspeed);
			}
		}

		public bool IsPlaying()
		{
			if (this.instance.isValid())
			{
				PLAYBACK_STATE playback_STATE;
				this.instance.getPlaybackState(out playback_STATE);
				return playback_STATE != PLAYBACK_STATE.STOPPED;
			}
			return false;
		}

		public EventReference EventReference;

		[Obsolete("Use the EventReference field instead")]
		public string Event = "";

		public EmitterGameEvent PlayEvent;

		public EmitterGameEvent StopEvent;

		public bool AllowFadeout = true;

		public bool TriggerOnce;

		public bool Preload;

		public ParamRef[] Params = new ParamRef[0];

		public bool OverrideAttenuation;

		public float OverrideMinDistance = -1f;

		public float OverrideMaxDistance = -1f;

		protected EventDescription eventDescription;

		protected EventInstance instance;

		private bool hasTriggered;

		private bool isQuitting;

		private bool isOneshot;

		private List<ParamRef> cachedParams = new List<ParamRef>();

		private static List<StudioEventEmitter> activeEmitters = new List<StudioEventEmitter>();

		private const string SnapshotString = "snapshot";
	}
}
