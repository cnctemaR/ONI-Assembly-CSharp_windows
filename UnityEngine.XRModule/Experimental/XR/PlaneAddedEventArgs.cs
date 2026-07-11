using System;

namespace UnityEngine.Experimental.XR
{
	public struct PlaneAddedEventArgs
	{
		public XRPlaneSubsystem PlaneSubsystem { get; internal set; }

		public BoundedPlane Plane { get; internal set; }
	}
}
