using System;

namespace UnityEngine.Timeline
{
	internal interface IClipInitializer
	{
		void OnCreate(TimelineClip owningClip, TrackAsset owningTrack, IExposedPropertyTable resolver);
	}
}
