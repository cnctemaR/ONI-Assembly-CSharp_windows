using System;

namespace UnityEngine.Experimental.XR
{
	/// <summary>
	///   <para>Contains data supplied to a XRPlaneSubsystem.PlaneAdded event.</para>
	/// </summary>
	public struct PlaneAddedEventArgs
	{
		/// <summary>
		///   <para>A reference to the PlaneSubsystem object from which the event originated.</para>
		/// </summary>
		public XRPlaneSubsystem PlaneSubsystem { get; internal set; }

		/// <summary>
		///   <para>The BoundedPlane that was added.</para>
		/// </summary>
		public BoundedPlane Plane { get; internal set; }
	}
}
