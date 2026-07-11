using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/Subsystems/Raycast/XRRaycastSubsystem.h")]
	[UsedByNativeCode]
	public struct XRRaycastHit
	{
		public TrackableId TrackableId { get; set; }

		public Pose Pose { get; set; }

		public float Distance { get; set; }

		public TrackableType HitType { get; set; }
	}
}
