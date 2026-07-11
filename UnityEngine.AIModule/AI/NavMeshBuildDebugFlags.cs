using System;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Bitmask used for operating with debug data from the NavMesh build process.</para>
	/// </summary>
	[Flags]
	public enum NavMeshBuildDebugFlags
	{
		/// <summary>
		///   <para>No debug data from the NavMesh build process is taken into consideration.</para>
		/// </summary>
		None = 0,
		/// <summary>
		///   <para>The triangles of all the geometry that is used as a base for computing the new NavMesh.</para>
		/// </summary>
		InputGeometry = 1,
		/// <summary>
		///   <para>The voxels produced by rasterizing the source geometry into walkable and unwalkable areas.</para>
		/// </summary>
		Voxels = 2,
		/// <summary>
		///   <para>The segmentation of the traversable surfaces into smaller areas necessary for producing simple polygons.</para>
		/// </summary>
		Regions = 4,
		/// <summary>
		///   <para>The contours that follow precisely the edges of each surface region.</para>
		/// </summary>
		RawContours = 8,
		/// <summary>
		///   <para>Contours bounding each of the surface regions, described through fewer vertices and straighter edges compared to RawContours.</para>
		/// </summary>
		SimplifiedContours = 16,
		/// <summary>
		///   <para>Meshes of convex polygons constructed within the unified contours of adjacent regions.</para>
		/// </summary>
		PolygonMeshes = 32,
		/// <summary>
		///   <para>The triangulated meshes with height details that better approximate the source geometry.</para>
		/// </summary>
		PolygonMeshesDetail = 64,
		/// <summary>
		///   <para>All debug data from the NavMesh build process is taken into consideration.</para>
		/// </summary>
		All = 127
	}
}
