using System;

namespace UnityEngine.Experimental.AI
{
	public struct NavMeshLocation
	{
		internal NavMeshLocation(Vector3 position, PolygonId polygon)
		{
			this.position = position;
			this.polygon = polygon;
		}

		public PolygonId polygon { get; }

		public Vector3 position { get; }
	}
}
