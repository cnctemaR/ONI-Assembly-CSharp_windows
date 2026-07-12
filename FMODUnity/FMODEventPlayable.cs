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
	public class FMODEventPlayable : PlayableAsset, ITimelineClipAsset
	{
		public static event EventHandler<EventArgs> OnCreatePlayable;

		public GameObject TrackTargetObject { get; set; }

		public override double duration
		{
			get
			{
				if (this.EventReference.IsNull)
				{
					return base.duration;
				}
				return (double)this.EventLength;
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

		public void LinkParameters(EventDescription eventDescription)
		{
			if (!this.CachedParameters && !this.EventReference.IsNull)
			{
				for (int i = 0; i < this.Parameters.Length; i++)
				{
					PARAMETER_DESCRIPTION parameter_DESCRIPTION;
					eventDescription.getParameterDescriptionByName(this.Parameters[i].Name, out parameter_DESCRIPTION);
					this.Parameters[i].ID = parameter_DESCRIPTION.id;
				}
				List<ParameterAutomationLink> parameterLinks = this.Template.ParameterLinks;
				for (int j = 0; j < parameterLinks.Count; j++)
				{
					PARAMETER_DESCRIPTION parameter_DESCRIPTION2;
					eventDescription.getParameterDescriptionByName(parameterLinks[j].Name, out parameter_DESCRIPTION2);
					parameterLinks[j].ID = parameter_DESCRIPTION2.id;
				}
				this.CachedParameters = true;
			}
		}

		public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
		{
			if (Application.isPlaying)
			{
				this.LinkParameters(RuntimeManager.GetEventDescription(this.EventReference));
			}
			else
			{
				EventArgs e = new EventArgs();
				FMODEventPlayable.OnCreatePlayable(this, e);
			}
			ScriptPlayable<FMODEventPlayableBehavior> scriptPlayable = ScriptPlayable<FMODEventPlayableBehavior>.Create(graph, this.Template, 0);
			this.behavior = scriptPlayable.GetBehaviour();
			this.behavior.TrackTargetObject = this.TrackTargetObject;
			this.behavior.EventReference = this.EventReference;
			this.behavior.StopType = this.StopType;
			this.behavior.Parameters = this.Parameters;
			this.behavior.OwningClip = this.OwningClip;
			return scriptPlayable;
		}

		[FormerlySerializedAs("template")]
		public FMODEventPlayableBehavior Template = new FMODEventPlayableBehavior();

		[FormerlySerializedAs("eventLength")]
		public float EventLength;

		[Obsolete("Use the eventReference field instead")]
		[SerializeField]
		public string eventName;

		[FormerlySerializedAs("eventReference")]
		[SerializeField]
		public EventReference EventReference;

		[FormerlySerializedAs("stopType")]
		[SerializeField]
		public STOP_MODE StopType;

		[FormerlySerializedAs("parameters")]
		[SerializeField]
		public ParamRef[] Parameters = new ParamRef[0];

		[NonSerialized]
		public bool CachedParameters;

		private FMODEventPlayableBehavior behavior;
	}
}
