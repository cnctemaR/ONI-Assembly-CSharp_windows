using System;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace FMODUnity
{
	[Serializable]
	public class FMODEventPlayableBehavior : PlayableBehaviour
	{
		protected void PlayEvent()
		{
			if (!string.IsNullOrEmpty(this.eventName))
			{
				this.eventInstance = RuntimeManager.CreateInstance(this.eventName);
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
						RuntimeManager.AttachInstanceToGameObject(this.eventInstance, this.TrackTargetObject.transform);
					}
				}
				else
				{
					this.eventInstance.set3DAttributes(Vector3.zero.To3DAttributes());
				}
				foreach (ParamRef paramRef in this.parameters)
				{
					this.eventInstance.setParameterByID(paramRef.ID, paramRef.Value, false);
				}
				this.eventInstance.setVolume(this.currentVolume);
				this.eventInstance.start();
			}
		}

		public void OnEnter()
		{
			if (!this.isPlayheadInside)
			{
				this.PlayEvent();
				this.isPlayheadInside = true;
			}
		}

		public void OnExit()
		{
			if (this.isPlayheadInside)
			{
				if (this.eventInstance.isValid())
				{
					if (this.stopType != STOP_MODE.None)
					{
						this.eventInstance.stop((this.stopType == STOP_MODE.Immediate) ? STOP_MODE.IMMEDIATE : STOP_MODE.ALLOWFADEOUT);
					}
					this.eventInstance.release();
				}
				this.isPlayheadInside = false;
			}
		}

		public override void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
			if (this.eventInstance.isValid())
			{
				foreach (ParameterAutomationLink parameterAutomationLink in this.parameterLinks)
				{
					float value = this.parameterAutomation.GetValue(parameterAutomationLink.Slot);
					this.eventInstance.setParameterByID(parameterAutomationLink.ID, value, false);
				}
			}
		}

		public void UpdateBehavior(float time, float volume)
		{
			if (volume != this.currentVolume)
			{
				this.currentVolume = volume;
				if (this.eventInstance.isValid())
				{
					this.eventInstance.setVolume(volume);
				}
			}
			if ((double)time >= this.OwningClip.start && (double)time < this.OwningClip.end)
			{
				this.OnEnter();
				return;
			}
			this.OnExit();
		}

		public override void OnGraphStop(Playable playable)
		{
			this.isPlayheadInside = false;
			if (this.eventInstance.isValid())
			{
				this.eventInstance.stop(STOP_MODE.IMMEDIATE);
				this.eventInstance.release();
				RuntimeManager.StudioSystem.update();
			}
		}

		public string eventName;

		public STOP_MODE stopType;

		[NotKeyable]
		public ParamRef[] parameters = new ParamRef[0];

		public List<ParameterAutomationLink> parameterLinks = new List<ParameterAutomationLink>();

		[NonSerialized]
		public GameObject TrackTargetObject;

		[NonSerialized]
		public TimelineClip OwningClip;

		public AutomatableSlots parameterAutomation;

		private bool isPlayheadInside;

		private EventInstance eventInstance;

		private float currentVolume = 1f;
	}
}
