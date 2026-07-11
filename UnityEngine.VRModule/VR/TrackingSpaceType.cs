using System;
using System.ComponentModel;

namespace UnityEngine.VR
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	[Obsolete("TrackingSpaceType has been moved.  Use UnityEngine.XR.TrackingSpaceType instead (UnityUpgradable) -> UnityEngine.XR.TrackingSpaceType", true)]
	public enum TrackingSpaceType
	{
		Stationary,
		RoomScale
	}
}
