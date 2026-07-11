using System;

namespace UnityEngine.Experimental.AI
{
	/// <summary>
	///   <para>A world position that is guaranteed to be on the surface of the NavMesh.</para>
	/// </summary>
	public struct NavMeshLocation
	{
		internal NavMeshLocation(Vector3 position, PolygonId polygon)
		{
			this.position = position;
			this.polygon = polygon;
		}

		/// <summary>
		///   <para>Unique identifier for the node in the NavMesh to which the world position has been mapped.</para>
		/// </summary>
		public PolygonId polygon { get; }

		/// <summary>
		///   <para>A world position that sits precisely on the surface of the NavMesh or along its links.</para>
		/// </summary>
		public Vector3 position { get; }
	}
}
