using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/Subsystems/ReferencePoints/XRManagedReferencePoint.h")]
	[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
	[UsedByNativeCode]
	public struct ReferencePoint
	{
		public TrackableId Id { get; internal set; }

		public TrackingState TrackingState { get; internal set; }

		public Pose Pose { get; internal set; }
	}
}
