using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Cooking options that are available with MeshCollider.</para>
	/// </summary>
	[Flags]
	public enum MeshColliderCookingOptions
	{
		/// <summary>
		///   <para>No optional cooking steps will be run.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>Allow the physics engine to increase the volume of the input mesh in attempt to generate a valid convex mesh.</para>
		/// </summary>
		InflateConvexMesh = 1,
		/// <summary>
		///   <para>Toggle between cooking for faster simulation or faster cooking time.</para>
		/// </summary>
		CookForFasterSimulation = 2,
		/// <summary>
		///   <para>Toggle cleaning of the mesh.</para>
		/// </summary>
		EnableMeshCleaning = 4,
		/// <summary>
		///   <para>Toggle the removal of equal vertices.</para>
		/// </summary>
		WeldColocatedVertices = 8
	}
}
