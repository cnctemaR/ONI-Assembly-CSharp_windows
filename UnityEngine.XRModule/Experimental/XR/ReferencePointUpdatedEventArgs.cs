using System;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
	public struct ReferencePointUpdatedEventArgs
	{
		public ReferencePoint ReferencePoint { get; internal set; }

		public TrackingState PreviousTrackingState { get; internal set; }

		public Pose PreviousPose { get; internal set; }
	}
}
