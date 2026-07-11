using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0.066f, 0.134f, 0.244f)]
[TrackClipType(typeof(FMODEventPlayable))]
[TrackBindingType(typeof(GameObject))]
public class FMODEventTrack : TrackAsset
{
	public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
	{
		GameObject gameObject = go.GetComponent<PlayableDirector>().GetGenericBinding(this) as GameObject;
		foreach (TimelineClip timelineClip in base.GetClips())
		{
			FMODEventPlayable fmodeventPlayable = timelineClip.asset as FMODEventPlayable;
			if (fmodeventPlayable)
			{
				fmodeventPlayable.TrackTargetObject = gameObject;
				fmodeventPlayable.OwningClip = timelineClip;
			}
		}
		return ScriptPlayable<FMODEventMixerBehaviour>.Create(graph, inputCount);
	}
}
