using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FMODEventPlayableBehavior : PlayableBehaviour
{
	protected void PlayEvent()
	{
		if (!string.IsNullOrEmpty(this.eventName))
		{
			this.eventInstance = RuntimeManager.CreateInstance(this.eventName);
			if (Application.isPlaying && this.TrackTargetObject)
			{
				Rigidbody component = this.TrackTargetObject.GetComponent<Rigidbody>();
				if (component)
				{
					RuntimeManager.AttachInstanceToGameObject(this.eventInstance, this.TrackTargetObject.transform, component);
				}
				else
				{
					RuntimeManager.AttachInstanceToGameObject(this.eventInstance, this.TrackTargetObject.transform, this.TrackTargetObject.GetComponent<Rigidbody2D>());
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
				if (this.stopType != global::STOP_MODE.None)
				{
					this.eventInstance.stop((this.stopType == global::STOP_MODE.Immediate) ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				}
				this.eventInstance.release();
			}
			this.isPlayheadInside = false;
		}
	}

	public void UpdateBehaviour(float time)
	{
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
			this.eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
			this.eventInstance.release();
			RuntimeManager.StudioSystem.update();
		}
	}

	public string eventName;

	public global::STOP_MODE stopType;

	public ParamRef[] parameters = new ParamRef[0];

	public GameObject TrackTargetObject;

	public TimelineClip OwningClip;

	private bool isPlayheadInside;

	private EventInstance eventInstance;
}
