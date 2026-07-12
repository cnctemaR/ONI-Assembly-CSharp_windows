using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace FMODUnity
{
	[Serializable]
	public class FMODEventPlayableBehavior : PlayableBehaviour
	{
		public FMODEventPlayableBehavior()
		{
			this.CurrentVolume = 1f;
		}

		public static event EventHandler<FMODEventPlayableBehavior.EventArgs> Enter;

		public static event EventHandler<FMODEventPlayableBehavior.EventArgs> Exit;

		public static event EventHandler<FMODEventPlayableBehavior.EventArgs> GraphStop;

		public float ClipStartTime { get; private set; }

		public float CurrentVolume { get; private set; }

		protected void PlayEvent()
		{
			if (!this.EventReference.IsNull)
			{
				this.eventInstance = RuntimeManager.CreateInstance(this.EventReference);
				if (Application.isPlaying && this.TrackTargetObject)
				{
					if (this.TrackTargetObject.GetComponent<Rigidbody>())
					{
						RuntimeManager.AttachInstanceToGameObject(this.eventInstance, this.TrackTargetObject.transform, this.TrackTargetObject.GetComponent<Rigidbody>());
					}
					else if (this.TrackTargetObject.GetComponent<Rigidbody2D>())
					{
						RuntimeManager.AttachInstanceToGameObject(this.eventInstance, this.TrackTargetObject.transform, this.TrackTargetObject.GetComponent<Rigidbody2D>());
					}
					else
					{
						RuntimeManager.AttachInstanceToGameObject(this.eventInstance, this.TrackTargetObject.transform, false);
					}
				}
				else
				{
					this.eventInstance.set3DAttributes(Vector3.zero.To3DAttributes());
				}
				foreach (ParamRef paramRef in this.Parameters)
				{
					this.eventInstance.setParameterByID(paramRef.ID, paramRef.Value, false);
				}
				this.eventInstance.setVolume(this.CurrentVolume);
				this.eventInstance.setTimelinePosition((int)(this.ClipStartTime * 1000f));
				this.eventInstance.start();
			}
		}

		protected virtual void OnEnter()
		{
			if (!this.isPlayheadInside)
			{
				this.isPlayheadInside = true;
				if (Application.isPlaying)
				{
					this.PlayEvent();
					return;
				}
				FMODEventPlayableBehavior.EventArgs e = new FMODEventPlayableBehavior.EventArgs();
				FMODEventPlayableBehavior.Enter(this, e);
				this.eventInstance = e.eventInstance;
			}
		}

		protected virtual void OnExit()
		{
			if (this.isPlayheadInside)
			{
				this.isPlayheadInside = false;
				if (Application.isPlaying)
				{
					if (this.eventInstance.isValid())
					{
						if (this.StopType != STOP_MODE.None)
						{
							this.eventInstance.stop((this.StopType == STOP_MODE.Immediate) ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
						}
						this.eventInstance.release();
						this.eventInstance.clearHandle();
						return;
					}
				}
				else
				{
					FMODEventPlayableBehavior.EventArgs e = new FMODEventPlayableBehavior.EventArgs();
					e.eventInstance = this.eventInstance;
					FMODEventPlayableBehavior.Exit(this, e);
				}
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (this.eventInstance.isValid())
			{
				foreach (ParameterAutomationLink parameterAutomationLink in this.ParameterLinks)
				{
					float value = this.ParameterAutomation.GetValue(parameterAutomationLink.Slot);
					this.eventInstance.setParameterByID(parameterAutomationLink.ID, value, false);
				}
			}
		}

		public void UpdateBehavior(float time, float volume)
		{
			if (volume != this.CurrentVolume)
			{
				this.CurrentVolume = volume;
				if (this.eventInstance.isValid())
				{
					this.eventInstance.setVolume(volume);
				}
			}
			if ((double)time >= this.OwningClip.start && (double)time < this.OwningClip.end)
			{
				this.ClipStartTime = time - (float)this.OwningClip.start;
				this.OnEnter();
				return;
			}
			this.OnExit();
		}

		public override void OnGraphStop(Playable playable)
		{
			this.isPlayheadInside = false;
			if (Application.isPlaying)
			{
				if (this.eventInstance.isValid())
				{
					this.eventInstance.stop(STOP_MODE.IMMEDIATE);
					this.eventInstance.release();
					RuntimeManager.StudioSystem.update();
					return;
				}
			}
			else
			{
				FMODEventPlayableBehavior.EventArgs e = new FMODEventPlayableBehavior.EventArgs();
				e.eventInstance = this.eventInstance;
				FMODEventPlayableBehavior.GraphStop(this, e);
			}
		}

		[FormerlySerializedAs("eventReference")]
		public EventReference EventReference;

		[FormerlySerializedAs("stopType")]
		public STOP_MODE StopType;

		[FormerlySerializedAs("parameters")]
		[NotKeyable]
		public ParamRef[] Parameters = new ParamRef[0];

		[FormerlySerializedAs("parameterLinks")]
		public List<ParameterAutomationLink> ParameterLinks = new List<ParameterAutomationLink>();

		[NonSerialized]
		public GameObject TrackTargetObject;

		[NonSerialized]
		public TimelineClip OwningClip;

		[FormerlySerializedAs("parameterAutomation")]
		public AutomatableSlots ParameterAutomation;

		private bool isPlayheadInside;

		private EventInstance eventInstance;

		public class EventArgs : global::System.EventArgs
		{
			public EventInstance eventInstance { get; set; }
		}
	}
}
