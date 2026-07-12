using System;

namespace UnityEngine.Experimental.AI
{
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
