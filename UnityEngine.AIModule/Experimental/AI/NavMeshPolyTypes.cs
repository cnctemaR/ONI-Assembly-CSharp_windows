using System;

namespace UnityEngine.Experimental.AI
{
	/// <summary>
	///   <para>The types of nodes in the navigation data.</para>
	/// </summary>
	public enum NavMeshPolyTypes
	{
		/// <summary>
		///   <para>Type of node in the NavMesh representing one surface polygon.</para>
		/// </summary>
		Ground,
		/// <summary>
		///   <para>Type of node in the NavMesh representing a point-to-point connection between two positions on the NavMesh surface.</para>
		/// </summary>
		OffMeshConnection
	}
}
