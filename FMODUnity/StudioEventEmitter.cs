using System;
using System.Collections.Generic;
using System.Threading;
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

		public float MaxDistance
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
				this.eventDescription.getMaximumDistance(out num);
				return num;
			}
		}

		protected override void Start()
		{
			RuntimeUtils.EnforceLibraryOrder();
			if (this.Preload)
			{
				this.Lookup();
				this.eventDescription.loadSampleData();
				RuntimeManager.StudioSystem.update();
				LOADING_STATE loading_STATE;
				this.eventDescription.getSampleLoadingState(out loading_STATE);
				while (loading_STATE == LOADING_STATE.LOADING)
				{
					Thread.Sleep(1);
					this.eventDescription.getSampleLoadingState(out loading_STATE);
				}
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
				RuntimeManager.DeregisterActiveEmitter(this);
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
			this.eventDescription = RuntimeManager.GetEventDescription(this.Event);
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
			if (string.IsNullOrEmpty(this.Event))
			{
				return;
			}
			this.cachedParams.Clear();
			if (!this.eventDescription.isValid())
			{
				this.Lookup();
			}
			if (!this.Event.StartsWith("snapshot", StringComparison.CurrentCultureIgnoreCase))
			{
				this.eventDescription.isOneshot(out this.isOneshot);
			}
			bool flag;
			this.eventDescription.is3D(out flag);
			this.IsActive = true;
			if (flag && !this.isOneshot && Settings.Instance.StopEventsOutsideMaxDistance)
			{
				RuntimeManager.RegisterActiveEmitter(this);
				RuntimeManager.UpdateActiveEmitter(this, true);
				return;
			}
			this.PlayInstance();
		}

		public void PlayInstance()
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
			RuntimeManager.DeregisterActiveEmitter(this);
			this.IsActive = false;
			this.cachedParams.Clear();
			this.StopInstance();
		}

		public void StopInstance()
		{
			if (this.TriggerOnce && this.hasTriggered)
			{
				RuntimeManager.DeregisterActiveEmitter(this);
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

		[EventRef]
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

		private const string SnapshotString = "snapshot";
	}
}
