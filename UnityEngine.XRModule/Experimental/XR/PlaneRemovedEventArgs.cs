using System;

namespace UnityEngine.Experimental.XR
{
	public struct PlaneRemovedEventArgs
	{
		public XRPlaneSubsystem PlaneSubsystem { get; internal set; }

		public BoundedPlane Plane { get; internal set; }
	}
}
