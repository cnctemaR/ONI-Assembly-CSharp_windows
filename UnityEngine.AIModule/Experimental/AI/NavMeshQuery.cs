using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.AI
{
	/// <summary>
	///   <para>Object used for doing navigation operations in a NavMeshWorld.</para>
	/// </summary>
	[NativeContainer]
	[NativeHeader("Modules/AI/NavMeshExperimental.bindings.h")]
	[NativeHeader("Runtime/Math/Matrix4x4.h")]
	[StaticAccessor("NavMeshQueryBindings", StaticAccessorType.DoubleColon)]
	public struct NavMeshQuery : IDisposable
	{
		/// <summary>
		///   <para>Creates the NavMeshQuery object and allocates memory to store NavMesh node information, if required.</para>
		/// </summary>
		/// <param name="world">NavMeshWorld object used as an entry point to the collection of NavMesh objects. This object that can be used by query operations.</param>
		/// <param name="allocator">Label indicating the desired life time of the object. (Known issue: Currently allocator has no effect).</param>
		/// <param name="pathNodePoolSize">The number of nodes that can be temporarily stored in the query during search operations. This value defaults to 0 if no other value is specified.</param>
		public NavMeshQuery(NavMeshWorld world, Allocator allocator, int pathNodePoolSize = 0)
		{
			this.m_Allocator = allocator;
			this.m_NavMeshQuery = NavMeshQuery.Create(world, pathNodePoolSize);
		}

		/// <summary>
		///   <para>Destroys the NavMeshQuery and deallocates all memory used by it.</para>
		/// </summary>
		public void Dispose()
		{
			NavMeshQuery.Destroy(this.m_NavMeshQuery);
			this.m_NavMeshQuery = IntPtr.Zero;
		}

		private static IntPtr Create(NavMeshWorld world, int nodePoolSize)
		{
			return NavMeshQuery.Create_Injected(ref world, nodePoolSize);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr navMeshQuery);

		public unsafe PathQueryStatus BeginFindPath(NavMeshLocation start, NavMeshLocation end, int areaMask = -1, NativeArray<float> costs = default(NativeArray<float>))
		{
			void* ptr = ((costs.Length <= 0) ? null : costs.GetUnsafePtr<float>());
			return NavMeshQuery.BeginFindPath(this.m_NavMeshQuery, start, end, areaMask, ptr);
		}

		public PathQueryStatus UpdateFindPath(int iterations, out int iterationsPerformed)
		{
			return NavMeshQuery.UpdateFindPath(this.m_NavMeshQuery, iterations, out iterationsPerformed);
		}

		public PathQueryStatus EndFindPath(out int pathSize)
		{
			return NavMeshQuery.EndFindPath(this.m_NavMeshQuery, out pathSize);
		}

		public int GetPathResult(NativeSlice<PolygonId> path)
		{
			return NavMeshQuery.GetPathResult(this.m_NavMeshQuery, path.GetUnsafePtr<PolygonId>(), path.Length);
		}

		[ThreadSafe]
		private unsafe static PathQueryStatus BeginFindPath(IntPtr navMeshQuery, NavMeshLocation start, NavMeshLocation end, int areaMask, void* costs)
		{
			return NavMeshQuery.BeginFindPath_Injected(navMeshQuery, ref start, ref end, areaMask, costs);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PathQueryStatus UpdateFindPath(IntPtr navMeshQuery, int iterations, out int iterationsPerformed);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PathQueryStatus EndFindPath(IntPtr navMeshQuery, out int pathSize);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int GetPathResult(IntPtr navMeshQuery, void* path, int maxPath);

		[ThreadSafe]
		private static bool IsValidPolygon(IntPtr navMeshQuery, PolygonId polygon)
		{
			return NavMeshQuery.IsValidPolygon_Injected(navMeshQuery, ref polygon);
		}

		/// <summary>
		///   <para>Returns true if the node referenced by the specified PolygonId is active in the NavMesh.</para>
		/// </summary>
		/// <param name="polygon">Identifier of the NavMesh node to be checked.</param>
		public bool IsValid(PolygonId polygon)
		{
			return polygon.polyRef != 0UL && NavMeshQuery.IsValidPolygon(this.m_NavMeshQuery, polygon);
		}

		/// <summary>
		///   <para>Returns true if the node referenced by the PolygonId contained in the NavMeshLocation is active in the NavMesh.</para>
		/// </summary>
		/// <param name="location">Location on the NavMesh to be checked. Same as checking location.polygon directly.</param>
		public bool IsValid(NavMeshLocation location)
		{
			return this.IsValid(location.polygon);
		}

		[ThreadSafe]
		private static int GetAgentTypeIdForPolygon(IntPtr navMeshQuery, PolygonId polygon)
		{
			return NavMeshQuery.GetAgentTypeIdForPolygon_Injected(navMeshQuery, ref polygon);
		}

		/// <summary>
		///   <para>Returns the identifier of the agent type the NavMesh was baked for or for which the link has been configured.</para>
		/// </summary>
		/// <param name="polygon">Identifier of a node from a NavMesh surface or link.</param>
		/// <returns>
		///   <para>Agent type identifier.</para>
		/// </returns>
		public int GetAgentTypeIdForPolygon(PolygonId polygon)
		{
			return NavMeshQuery.GetAgentTypeIdForPolygon(this.m_NavMeshQuery, polygon);
		}

		[ThreadSafe]
		private static bool IsPositionInPolygon(IntPtr navMeshQuery, Vector3 position, PolygonId polygon)
		{
			return NavMeshQuery.IsPositionInPolygon_Injected(navMeshQuery, ref position, ref polygon);
		}

		[ThreadSafe]
		private static PathQueryStatus GetClosestPointOnPoly(IntPtr navMeshQuery, PolygonId polygon, Vector3 position, out Vector3 nearest)
		{
			return NavMeshQuery.GetClosestPointOnPoly_Injected(navMeshQuery, ref polygon, ref position, out nearest);
		}

		/// <summary>
		///   <para>Returns a valid NavMeshLocation for a position and a polygon provided by the user.</para>
		/// </summary>
		/// <param name="position">World position of the NavMeshLocation to be created.</param>
		/// <param name="polygon">Valid identifier for the NavMesh node.</param>
		/// <returns>
		///   <para>Object containing the desired position and NavMesh node.</para>
		/// </returns>
		public NavMeshLocation CreateLocation(Vector3 position, PolygonId polygon)
		{
			Vector3 vector;
			PathQueryStatus closestPointOnPoly = NavMeshQuery.GetClosestPointOnPoly(this.m_NavMeshQuery, polygon, position, out vector);
			return ((closestPointOnPoly & PathQueryStatus.Success) == (PathQueryStatus)0) ? default(NavMeshLocation) : new NavMeshLocation(vector, polygon);
		}

		[ThreadSafe]
		private static NavMeshLocation MapLocation(IntPtr navMeshQuery, Vector3 position, Vector3 extents, int agentTypeID, int areaMask = -1)
		{
			NavMeshLocation navMeshLocation;
			NavMeshQuery.MapLocation_Injected(navMeshQuery, ref position, ref extents, agentTypeID, areaMask, out navMeshLocation);
			return navMeshLocation;
		}

		/// <summary>
		///   <para>Finds the closest point and PolygonId on the NavMesh for a given world position.</para>
		/// </summary>
		/// <param name="position">World position for which the closest point on the NavMesh needs to be found.</param>
		/// <param name="extents">Maximum distance, from the specified position, expanding along all three axes, within which NavMesh surfaces are searched.</param>
		/// <param name="agentTypeID">Identifier for the agent type whose NavMesh surfaces should be selected for this operation. The Humanoid agent type exists for all NavMeshes and has an ID of 0. Other agent types can be defined manually through the Editor. A separate NavMesh surface needs to be baked for each agent type.</param>
		/// <param name="areaMask">Bitmask used to represent areas of the NavMesh that should (value of 1) or shouldn't (values of 0) be sampled. This parameter is optional and defaults to NavMesh.AllAreas if unspecified. See Also:.</param>
		/// <returns>
		///   <para>An object with position and valid PolygonId  - when a point on the NavMesh has been found.
		///
		/// An invalid object - when no NavMesh surface with the desired features has been found within the search area. See Also: NavMeshQuery.IsValid.</para>
		/// </returns>
		public NavMeshLocation MapLocation(Vector3 position, Vector3 extents, int agentTypeID, int areaMask = -1)
		{
			return NavMeshQuery.MapLocation(this.m_NavMeshQuery, position, extents, agentTypeID, areaMask);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void MoveLocations(IntPtr navMeshQuery, void* locations, void* targets, void* areaMasks, int count);

		public void MoveLocations(NativeSlice<NavMeshLocation> locations, NativeSlice<Vector3> targets, NativeSlice<int> areaMasks)
		{
			NavMeshQuery.MoveLocations(this.m_NavMeshQuery, locations.GetUnsafePtr<NavMeshLocation>(), targets.GetUnsafeReadOnlyPtr<Vector3>(), areaMasks.GetUnsafeReadOnlyPtr<int>(), locations.Length);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void MoveLocationsInSameAreas(IntPtr navMeshQuery, void* locations, void* targets, int count, int areaMask);

		public void MoveLocationsInSameAreas(NativeSlice<NavMeshLocation> locations, NativeSlice<Vector3> targets, int areaMask = -1)
		{
			NavMeshQuery.MoveLocationsInSameAreas(this.m_NavMeshQuery, locations.GetUnsafePtr<NavMeshLocation>(), targets.GetUnsafeReadOnlyPtr<Vector3>(), locations.Length, areaMask);
		}

		[ThreadSafe]
		private static NavMeshLocation MoveLocation(IntPtr navMeshQuery, NavMeshLocation location, Vector3 target, int areaMask)
		{
			NavMeshLocation navMeshLocation;
			NavMeshQuery.MoveLocation_Injected(navMeshQuery, ref location, ref target, areaMask, out navMeshLocation);
			return navMeshLocation;
		}

		/// <summary>
		///   <para>Translates a NavMesh location to another position without losing contact with the surface.</para>
		/// </summary>
		/// <param name="location">Position to be moved across the NavMesh surface.</param>
		/// <param name="target">World position you require the agent to move to.</param>
		/// <param name="areaMask">Bitmask with values of 1 set at the indices corresponding to areas that can be traversed, and with values of 0 for areas that should not be traversed. This parameter can be omitted, in which case it defaults to NavMesh.AllAreas. See Also:.</param>
		/// <returns>
		///   <para>A new location on the NavMesh placed as closely as possible to the specified target position.
		///
		/// The start location is returned when that start is inside an area which is not allowed by the areaMask.</para>
		/// </returns>
		public NavMeshLocation MoveLocation(NavMeshLocation location, Vector3 target, int areaMask = -1)
		{
			return NavMeshQuery.MoveLocation(this.m_NavMeshQuery, location, target, areaMask);
		}

		[ThreadSafe]
		private static bool GetPortalPoints(IntPtr navMeshQuery, PolygonId polygon, PolygonId neighbourPolygon, out Vector3 left, out Vector3 right)
		{
			return NavMeshQuery.GetPortalPoints_Injected(navMeshQuery, ref polygon, ref neighbourPolygon, out left, out right);
		}

		public bool GetPortalPoints(PolygonId polygon, PolygonId neighbourPolygon, out Vector3 left, out Vector3 right)
		{
			return NavMeshQuery.GetPortalPoints(this.m_NavMeshQuery, polygon, neighbourPolygon, out left, out right);
		}

		[ThreadSafe]
		private static Matrix4x4 PolygonLocalToWorldMatrix(IntPtr navMeshQuery, PolygonId polygon)
		{
			Matrix4x4 matrix4x;
			NavMeshQuery.PolygonLocalToWorldMatrix_Injected(navMeshQuery, ref polygon, out matrix4x);
			return matrix4x;
		}

		/// <summary>
		///   <para>Returns the transformation matrix of the NavMesh surface that contains the specified NavMesh node (Read Only).</para>
		/// </summary>
		/// <param name="polygon">NavMesh node for which its owner's transform must be determined.</param>
		/// <returns>
		///   <para>Transformation matrix for the surface owning the specified polygon.
		///
		/// Matrix4x4.identity when the NavMesh node is a.</para>
		/// </returns>
		public Matrix4x4 PolygonLocalToWorldMatrix(PolygonId polygon)
		{
			return NavMeshQuery.PolygonLocalToWorldMatrix(this.m_NavMeshQuery, polygon);
		}

		[ThreadSafe]
		private static Matrix4x4 PolygonWorldToLocalMatrix(IntPtr navMeshQuery, PolygonId polygon)
		{
			Matrix4x4 matrix4x;
			NavMeshQuery.PolygonWorldToLocalMatrix_Injected(navMeshQuery, ref polygon, out matrix4x);
			return matrix4x;
		}

		/// <summary>
		///   <para>Returns the inverse transformation matrix of the NavMesh surface that contains the specified NavMesh node (Read Only).</para>
		/// </summary>
		/// <param name="polygon">NavMesh node for which its owner's inverse transform must be determined.</param>
		/// <returns>
		///   <para>Inverse transformation matrix of the surface owning the specified polygon.
		///
		/// Matrix4x4.identity when the NavMesh node is a.</para>
		/// </returns>
		public Matrix4x4 PolygonWorldToLocalMatrix(PolygonId polygon)
		{
			return NavMeshQuery.PolygonWorldToLocalMatrix(this.m_NavMeshQuery, polygon);
		}

		[ThreadSafe]
		private static NavMeshPolyTypes GetPolygonType(IntPtr navMeshQuery, PolygonId polygon)
		{
			return NavMeshQuery.GetPolygonType_Injected(navMeshQuery, ref polygon);
		}

		/// <summary>
		///   <para>Returns whether the NavMesh node is a polygon or a link.</para>
		/// </summary>
		/// <param name="polygon">Identifier of a node from a NavMesh surface or link.</param>
		/// <returns>
		///   <para>Ground when the node is a polygon on a NavMesh surface.
		///
		/// OffMeshConnection when the node is a.</para>
		/// </returns>
		public NavMeshPolyTypes GetPolygonType(PolygonId polygon)
		{
			return NavMeshQuery.GetPolygonType(this.m_NavMeshQuery, polygon);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected(ref NavMeshWorld world, int nodePoolSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern PathQueryStatus BeginFindPath_Injected(IntPtr navMeshQuery, ref NavMeshLocation start, ref NavMeshLocation end, int areaMask, void* costs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidPolygon_Injected(IntPtr navMeshQuery, ref PolygonId polygon);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAgentTypeIdForPolygon_Injected(IntPtr navMeshQuery, ref PolygonId polygon);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsPositionInPolygon_Injected(IntPtr navMeshQuery, ref Vector3 position, ref PolygonId polygon);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PathQueryStatus GetClosestPointOnPoly_Injected(IntPtr navMeshQuery, ref PolygonId polygon, ref Vector3 position, out Vector3 nearest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MapLocation_Injected(IntPtr navMeshQuery, ref Vector3 position, ref Vector3 extents, int agentTypeID, int areaMask = -1, out NavMeshLocation ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveLocation_Injected(IntPtr navMeshQuery, ref NavMeshLocation location, ref Vector3 target, int areaMask, out NavMeshLocation ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetPortalPoints_Injected(IntPtr navMeshQuery, ref PolygonId polygon, ref PolygonId neighbourPolygon, out Vector3 left, out Vector3 right);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PolygonLocalToWorldMatrix_Injected(IntPtr navMeshQuery, ref PolygonId polygon, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PolygonWorldToLocalMatrix_Injected(IntPtr navMeshQuery, ref PolygonId polygon, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NavMeshPolyTypes GetPolygonType_Injected(IntPtr navMeshQuery, ref PolygonId polygon);

		[NativeDisableUnsafePtrRestriction]
		internal IntPtr m_NavMeshQuery;

		private Allocator m_Allocator;

		private const string k_NoBufferAllocatedErrorMessage = "This query has no buffer allocated for pathfinding operations. Create a different NavMeshQuery with an explicit node pool size.";
	}
}
