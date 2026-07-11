using System;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Used with NavMeshBuildSource to define the shape for building NavMesh.</para>
	/// </summary>
	public enum NavMeshBuildSourceShape
	{
		/// <summary>
		///   <para>Describes a Mesh source for use with NavMeshBuildSource.</para>
		/// </summary>
		Mesh,
		/// <summary>
		///   <para>Describes a TerrainData source for use with NavMeshBuildSource.</para>
		/// </summary>
		Terrain,
		/// <summary>
		///   <para>Describes a box primitive for use with NavMeshBuildSource.</para>
		/// </summary>
		Box,
		/// <summary>
		///   <para>Describes a sphere primitive for use with NavMeshBuildSource.</para>
		/// </summary>
		Sphere,
		/// <summary>
		///   <para>Describes a capsule primitive for use with NavMeshBuildSource.</para>
		/// </summary>
		Capsule,
		/// <summary>
		///   <para>Describes a ModifierBox source for use with NavMeshBuildSource.</para>
		/// </summary>
		ModifierBox
	}
}
