using System;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Contains data supplied to a XRPlaneSubsystem.PlaneUpdated event.</para>
	/// </summary>
	public struct PlaneUpdatedEventArgs
	{
		/// <summary>
		///   <para>A reference to the XRPlaneSubsystem object from which the event originated.</para>
		/// </summary>
		public XRPlaneSubsystem PlaneSubsystem { get; internal set; }

		/// <summary>
		///   <para>The BoundedPlane that was updated.</para>
		/// </summary>
		public BoundedPlane Plane { get; internal set; }
	}
}
