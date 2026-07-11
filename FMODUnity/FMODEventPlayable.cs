using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class FMODEventPlayable : PlayableAsset, ITimelineClipAsset
{
	public GameObject TrackTargetObject { get; set; }

	public override double duration
	{
		get
		{
			if (this.eventName == null)
			{
				return base.duration;
			}
			return (double)this.eventLength;
		}
	}

	public ClipCaps clipCaps
	{
		get
		{
			return ClipCaps.None;
		}
	}

	public TimelineClip OwningClip { get; set; }

	public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
	{
		if (!this.cachedParameters && !string.IsNullOrEmpty(this.eventName))
		{
			EventDescription eventDescription;
			RuntimeManager.StudioSystem.getEvent(this.eventName, out eventDescription);
			for (int i = 0; i < this.parameters.Length; i++)
			{
				PARAMETER_DESCRIPTION parameter_DESCRIPTION;
				eventDescription.getParameterDescriptionByName(this.parameters[i].Name, out parameter_DESCRIPTION);
				this.parameters[i].ID = parameter_DESCRIPTION.id;
			}
			this.cachedParameters = true;
		}
		ScriptPlayable<FMODEventPlayableBehavior> scriptPlayable = ScriptPlayable<FMODEventPlayableBehavior>.Create(graph, this.template, 0);
		this.behavior = scriptPlayable.GetBehaviour();
		this.behavior.TrackTargetObject = this.TrackTargetObject;
		this.behavior.eventName = this.eventName;
		this.behavior.stopType = this.stopType;
		this.behavior.parameters = this.parameters;
		this.behavior.OwningClip = this.OwningClip;
		return scriptPlayable;
	}

	public FMODEventPlayableBehavior template = new FMODEventPlayableBehavior();

	public float eventLength;

	private FMODEventPlayableBehavior behavior;

	[EventRef]
	[SerializeField]
	public string eventName;

	[SerializeField]
	public global::STOP_MODE stopType;

	[SerializeField]
	public ParamRef[] parameters = new ParamRef[0];

	[NonSerialized]
	public bool cachedParameters;
}
