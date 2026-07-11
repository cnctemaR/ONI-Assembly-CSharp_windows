using System;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Data to be passed to the user when the device corrects its understanding of the world enough that the ReferencePoint's position or rotation has changed.</para>
	/// </summary>
	[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
	public struct ReferencePointUpdatedEventArgs
	{
		/// <summary>
		///   <para>The reference point that has the value of its position, rotation, or both changed enough through the device correcting its understanding of where this point should be located in device space.</para>
		/// </summary>
		public ReferencePoint ReferencePoint { get; internal set; }

		/// <summary>
		///   <para>The previous TrackingState of the ReferencePoint, prior to this event.</para>
		/// </summary>
		public TrackingState PreviousTrackingState { get; internal set; }

		/// <summary>
		///   <para>The previous Pose of the ReferencePoint, prior to this event.</para>
		/// </summary>
		public Pose PreviousPose { get; internal set; }
	}
}
