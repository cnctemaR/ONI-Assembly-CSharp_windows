using System;

namespace UnityEngine.SpatialTracking
{
	[Flags]
	public enum PoseDataFlags
	{
		NoData = 0,
		Position = 1,
		Rotation = 2
	}
}
