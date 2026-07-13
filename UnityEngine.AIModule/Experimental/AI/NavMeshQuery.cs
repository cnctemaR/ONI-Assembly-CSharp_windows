using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.AI;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.AI
{
	[NativeHeader("Runtime/Math/Matrix4x4.h")]
	[NativeHeader("Modules/AI/Public/NavMeshBindingTypes.h")]
	[NativeHeader("Modules/AI/NavMeshExperimental.bindings.h")]
	[StaticAccessor("NavMeshQueryBindings", StaticAccessorType.DoubleColon)]
	[NativeContainer]
	[Obsolete("The experimental NavMeshQuery struct has been deprecated without replacement.")]
	public struct NavMeshQuery : IDisposable
	{
		public NavMeshQuery(NavMeshWorld world, Allocator allocator, int pathNodePoolSize = 0)
		{
			this.m_NavMeshQuery = NavMeshQuery.Create(world, pathNodePoolSize);
			UnsafeUtility.LeakRecord(this.m_NavMeshQuery, LeakCategory.NavMeshQuery, 0);
		}

		public void Dispose()
		{
			UnsafeUtility.LeakErase(this.m_NavMeshQuery, LeakCategory.NavMeshQuery);
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
			void* ptr = ((costs.Length > 0) ? costs.GetUnsafePtr<float>() : null);
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

		public bool IsValid(PolygonId polygon)
		{
			return polygon.polyRef != 0UL && NavMeshQuery.IsValidPolygon(this.m_NavMeshQuery, polygon);
		}

		public bool IsValid(NavMeshLocation location)
		{
			return this.IsValid(location.polygon);
		}

		[ThreadSafe]
		private static int GetAgentTypeIdForPolygon(IntPtr navMeshQuery, PolygonId polygon)
		{
			return NavMeshQuery.GetAgentTypeIdForPolygon_Injected(navMeshQuery, ref polygon);
		}

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

		public NavMeshLocation CreateLocation(Vector3 position, PolygonId polygon)
		{
			Vector3 vector;
			PathQueryStatus closestPointOnPoly = NavMeshQuery.GetClosestPointOnPoly(this.m_NavMeshQuery, polygon, position, out vector);
			return ((closestPointOnPoly & PathQueryStatus.Success) != (PathQueryStatus)0) ? new NavMeshLocation(vector, polygon) : default(NavMeshLocation);
		}

		[ThreadSafe]
		private static NavMeshLocation MapLocation(IntPtr navMeshQuery, Vector3 position, Vector3 extents, int agentTypeID, int areaMask = -1)
		{
			NavMeshLocation navMeshLocation;
			NavMeshQuery.MapLocation_Injected(navMeshQuery, ref position, ref extents, agentTypeID, areaMask, out navMeshLocation);
			return navMeshLocation;
		}

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

		public Matrix4x4 PolygonWorldToLocalMatrix(PolygonId polygon)
		{
			return NavMeshQuery.PolygonWorldToLocalMatrix(this.m_NavMeshQuery, polygon);
		}

		[ThreadSafe]
		private static NavMeshPolyTypes GetPolygonType(IntPtr navMeshQuery, PolygonId polygon)
		{
			return NavMeshQuery.GetPolygonType_Injected(navMeshQuery, ref polygon);
		}

		public NavMeshPolyTypes GetPolygonType(PolygonId polygon)
		{
			return NavMeshQuery.GetPolygonType(this.m_NavMeshQuery, polygon);
		}

		[ThreadSafe]
		private unsafe static PathQueryStatus Raycast(IntPtr navMeshQuery, NavMeshLocation start, Vector3 targetPosition, int areaMask, void* costs, out NavMeshHit hit, void* path, out int pathCount, int maxPath)
		{
			return NavMeshQuery.Raycast_Injected(navMeshQuery, ref start, ref targetPosition, areaMask, costs, out hit, path, out pathCount, maxPath);
		}

		public unsafe PathQueryStatus Raycast(out NavMeshHit hit, NavMeshLocation start, Vector3 targetPosition, int areaMask = -1, NativeArray<float> costs = default(NativeArray<float>))
		{
			void* ptr = ((costs.Length == 32) ? costs.GetUnsafePtr<float>() : null);
			int num;
			PathQueryStatus pathQueryStatus = NavMeshQuery.Raycast(this.m_NavMeshQuery, start, targetPosition, areaMask, ptr, out hit, null, out num, 0);
			return pathQueryStatus & ~PathQueryStatus.BufferTooSmall;
		}

		public unsafe PathQueryStatus Raycast(out NavMeshHit hit, NativeSlice<PolygonId> path, out int pathCount, NavMeshLocation start, Vector3 targetPosition, int areaMask = -1, NativeArray<float> costs = default(NativeArray<float>))
		{
			void* ptr = ((costs.Length == 32) ? costs.GetUnsafePtr<float>() : null);
			void* ptr2 = ((path.Length > 0) ? path.GetUnsafePtr<PolygonId>() : null);
			int num = ((ptr2 != null) ? path.Length : 0);
			return NavMeshQuery.Raycast(this.m_NavMeshQuery, start, targetPosition, areaMask, ptr, out hit, ptr2, out pathCount, num);
		}

		[ThreadSafe]
		private unsafe static PathQueryStatus GetEdgesAndNeighbors(IntPtr navMeshQuery, PolygonId node, int maxVerts, int maxNei, void* verts, void* neighbors, void* edgeIndices, out int vertCount, out int neighborsCount)
		{
			return NavMeshQuery.GetEdgesAndNeighbors_Injected(navMeshQuery, ref node, maxVerts, maxNei, verts, neighbors, edgeIndices, out vertCount, out neighborsCount);
		}

		public unsafe PathQueryStatus GetEdgesAndNeighbors(PolygonId node, NativeSlice<Vector3> edgeVertices, NativeSlice<PolygonId> neighbors, NativeSlice<byte> edgeIndices, out int verticesCount, out int neighborsCount)
		{
			void* ptr = ((edgeVertices.Length > 0) ? edgeVertices.GetUnsafePtr<Vector3>() : null);
			void* ptr2 = ((neighbors.Length > 0) ? neighbors.GetUnsafePtr<PolygonId>() : null);
			void* ptr3 = ((edgeIndices.Length > 0) ? edgeIndices.GetUnsafePtr<byte>() : null);
			int length = edgeVertices.Length;
			int num = ((neighbors.Length > 0) ? neighbors.Length : edgeIndices.Length);
			return NavMeshQuery.GetEdgesAndNeighbors(this.m_NavMeshQuery, node, length, num, ptr, ptr2, ptr3, out verticesCount, out neighborsCount);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create_Injected([In] ref NavMeshWorld world, int nodePoolSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern PathQueryStatus BeginFindPath_Injected(IntPtr navMeshQuery, [In] ref NavMeshLocation start, [In] ref NavMeshLocation end, int areaMask, void* costs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidPolygon_Injected(IntPtr navMeshQuery, [In] ref PolygonId polygon);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAgentTypeIdForPolygon_Injected(IntPtr navMeshQuery, [In] ref PolygonId polygon);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsPositionInPolygon_Injected(IntPtr navMeshQuery, [In] ref Vector3 position, [In] ref PolygonId polygon);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern PathQueryStatus GetClosestPointOnPoly_Injected(IntPtr navMeshQuery, [In] ref PolygonId polygon, [In] ref Vector3 position, out Vector3 nearest);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MapLocation_Injected(IntPtr navMeshQuery, [In] ref Vector3 position, [In] ref Vector3 extents, int agentTypeID, int areaMask, out NavMeshLocation ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MoveLocation_Injected(IntPtr navMeshQuery, [In] ref NavMeshLocation location, [In] ref Vector3 target, int areaMask, out NavMeshLocation ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool GetPortalPoints_Injected(IntPtr navMeshQuery, [In] ref PolygonId polygon, [In] ref PolygonId neighbourPolygon, out Vector3 left, out Vector3 right);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PolygonLocalToWorldMatrix_Injected(IntPtr navMeshQuery, [In] ref PolygonId polygon, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PolygonWorldToLocalMatrix_Injected(IntPtr navMeshQuery, [In] ref PolygonId polygon, out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern NavMeshPolyTypes GetPolygonType_Injected(IntPtr navMeshQuery, [In] ref PolygonId polygon);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern PathQueryStatus Raycast_Injected(IntPtr navMeshQuery, [In] ref NavMeshLocation start, [In] ref Vector3 targetPosition, int areaMask, void* costs, out NavMeshHit hit, void* path, out int pathCount, int maxPath);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern PathQueryStatus GetEdgesAndNeighbors_Injected(IntPtr navMeshQuery, [In] ref PolygonId node, int maxVerts, int maxNei, void* verts, void* neighbors, void* edgeIndices, out int vertCount, out int neighborsCount);

		[NativeDisableUnsafePtrRestriction]
		internal IntPtr m_NavMeshQuery;
	}
}
