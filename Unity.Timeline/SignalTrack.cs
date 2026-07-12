using System;

namespace UnityEngine.Timeline
{
	[TrackBindingType(typeof(SignalReceiver))]
	[TrackColor(0.25f, 0.25f, 0.25f)]
	[ExcludeFromPreset]
	[Serializable]
	public class SignalTrack : MarkerTrack
	{
	}
}
