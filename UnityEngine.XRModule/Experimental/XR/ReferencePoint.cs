using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Describes the transform data of a real-world point.</para>
	/// </summary>
	[UsedByNativeCode]
	[NativeHeader("Modules/XR/Subsystems/ReferencePoints/XRManagedReferencePoint.h")]
	[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
	public struct ReferencePoint
	{
		/// <summary>
		///   <para>ID for the reference point that is unique across the session.</para>
		/// </summary>
		public TrackableId Id { get; internal set; }

		/// <summary>
		///   <para>The TrackingState of the reference point.</para>
		/// </summary>
		public TrackingState TrackingState { get; internal set; }

		/// <summary>
		///   <para>The pose (position and rotation) of the reference point. Respond to changes in this pose to correct for changes in the device's understanding of where this point is in the real world.</para>
		/// </summary>
		public Pose Pose { get; internal set; }
	}
}
