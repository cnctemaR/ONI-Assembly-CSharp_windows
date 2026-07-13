using System;

namespace UnityEngine.Experimental.AI
{
	[Obsolete("The experimental NavMeshLocation struct has been deprecated without replacement.")]
	public struct NavMeshLocation
	{
		public readonly PolygonId polygon { get; }

		public readonly Vector3 position { get; }

		internal NavMeshLocation(Vector3 position, PolygonId polygon)
		{
			this.position = position;
			this.polygon = polygon;
		}
	}
}
