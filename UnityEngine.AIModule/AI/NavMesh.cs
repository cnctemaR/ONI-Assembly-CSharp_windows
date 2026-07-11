using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Singleton class to access the baked NavMesh.</para>
	/// </summary>
	[MovedFrom("UnityEngine")]
	[NativeHeader("Modules/AI/NavMeshManager.h")]
	public static class NavMesh
	{
		public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int areaMask)
		{
			return NavMesh.INTERNAL_CALL_Raycast(ref sourcePosition, ref targetPosition, out hit, areaMask);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_Raycast(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int areaMask);

		/// <summary>
		///   <para>Calculate a path between two points and store the resulting path.</para>
		/// </summary>
		/// <param name="sourcePosition">The initial position of the path requested.</param>
		/// <param name="targetPosition">The final position of the path requested.</param>
		/// <param name="areaMask">A bitfield mask specifying which NavMesh areas can be passed when calculating a path.</param>
		/// <param name="path">The resulting path.</param>
		/// <returns>
		///   <para>True if a either a complete or partial path is found and false otherwise.</para>
		/// </returns>
		public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
		{
			path.ClearCorners();
			return NavMesh.CalculatePathInternal(sourcePosition, targetPosition, areaMask, path);
		}

		internal static bool CalculatePathInternal(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
		{
			return NavMesh.INTERNAL_CALL_CalculatePathInternal(ref sourcePosition, ref targetPosition, areaMask, path);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CalculatePathInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, int areaMask, NavMeshPath path);

		public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, int areaMask)
		{
			return NavMesh.INTERNAL_CALL_FindClosestEdge(ref sourcePosition, out hit, areaMask);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_FindClosestEdge(ref Vector3 sourcePosition, out NavMeshHit hit, int areaMask);

		public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask)
		{
			return NavMesh.INTERNAL_CALL_SamplePosition(ref sourcePosition, out hit, maxDistance, areaMask);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SamplePosition(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask);

		/// <summary>
		///   <para>Sets the cost for traversing over geometry of the layer type on all agents.</para>
		/// </summary>
		/// <param name="layer"></param>
		/// <param name="cost"></param>
		[Obsolete("Use SetAreaCost instead.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLayerCost(int layer, float cost);

		/// <summary>
		///   <para>Gets the cost for traversing over geometry of the layer type on all agents.</para>
		/// </summary>
		/// <param name="layer"></param>
		[Obsolete("Use GetAreaCost instead.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetLayerCost(int layer);

		/// <summary>
		///   <para>Returns the layer index for a named layer.</para>
		/// </summary>
		/// <param name="layerName"></param>
		[Obsolete("Use GetAreaFromName instead.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNavMeshLayerFromName(string layerName);

		/// <summary>
		///   <para>Sets the cost for finding path over geometry of the area type on all agents.</para>
		/// </summary>
		/// <param name="areaIndex">Index of the area to set.</param>
		/// <param name="cost">New cost.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAreaCost(int areaIndex, float cost);

		/// <summary>
		///   <para>Gets the cost for path finding over geometry of the area type.</para>
		/// </summary>
		/// <param name="areaIndex">Index of the area to get.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetAreaCost(int areaIndex);

		/// <summary>
		///   <para>Returns the area index for a named NavMesh area type.</para>
		/// </summary>
		/// <param name="areaName">Name of the area to look up.</param>
		/// <returns>
		///   <para>Index if the specified are, or -1 if no area found.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetAreaFromName(string areaName);

		/// <summary>
		///   <para>Calculates triangulation of the current navmesh.</para>
		/// </summary>
		public static NavMeshTriangulation CalculateTriangulation()
		{
			return (NavMeshTriangulation)NavMesh.TriangulateInternal();
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern object TriangulateInternal();

		[Obsolete("use NavMesh.CalculateTriangulation () instead.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Triangulate(out Vector3[] vertices, out int[] indices);

		[Obsolete("AddOffMeshLinks has no effect and is deprecated.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void AddOffMeshLinks();

		[GeneratedByOldBindingsGenerator]
		[Obsolete("RestoreNavMesh has no effect and is deprecated.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RestoreNavMesh();

		/// <summary>
		///   <para>Describes how far in the future the agents predict collisions for avoidance.</para>
		/// </summary>
		public static float avoidancePredictionTime
		{
			get
			{
				return NavMesh.GetAvoidancePredictionTime();
			}
			set
			{
				NavMesh.SetAvoidancePredictionTime(value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetAvoidancePredictionTime(float t);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern float GetAvoidancePredictionTime();

		/// <summary>
		///   <para>The maximum amount of nodes processed each frame in the asynchronous pathfinding process.</para>
		/// </summary>
		public static int pathfindingIterationsPerFrame
		{
			get
			{
				return NavMesh.GetPathfindingIterationsPerFrame();
			}
			set
			{
				NavMesh.SetPathfindingIterationsPerFrame(value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetPathfindingIterationsPerFrame(int iter);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetPathfindingIterationsPerFrame();

		/// <summary>
		///   <para>Adds the specified NavMeshData to the game.</para>
		/// </summary>
		/// <param name="navMeshData">Contains the data for the navmesh.</param>
		/// <returns>
		///   <para>Representing the added navmesh.</para>
		/// </returns>
		public static NavMeshDataInstance AddNavMeshData(NavMeshData navMeshData)
		{
			if (navMeshData == null)
			{
				throw new ArgumentNullException("navMeshData");
			}
			return new NavMeshDataInstance
			{
				id = NavMesh.AddNavMeshDataInternal(navMeshData)
			};
		}

		/// <summary>
		///   <para>Adds the specified NavMeshData to the game.</para>
		/// </summary>
		/// <param name="navMeshData">Contains the data for the navmesh.</param>
		/// <param name="position">Translate the navmesh to this position.</param>
		/// <param name="rotation">Rotate the navmesh to this orientation.</param>
		/// <returns>
		///   <para>Representing the added navmesh.</para>
		/// </returns>
		public static NavMeshDataInstance AddNavMeshData(NavMeshData navMeshData, Vector3 position, Quaternion rotation)
		{
			if (navMeshData == null)
			{
				throw new ArgumentNullException("navMeshData");
			}
			return new NavMeshDataInstance
			{
				id = NavMesh.AddNavMeshDataTransformedInternal(navMeshData, position, rotation)
			};
		}

		/// <summary>
		///   <para>Removes the specified NavMeshDataInstance from the game, making it unavailable for agents and queries.</para>
		/// </summary>
		/// <param name="handle">The instance of a NavMesh to remove.</param>
		public static void RemoveNavMeshData(NavMeshDataInstance handle)
		{
			NavMesh.RemoveNavMeshDataInternal(handle.id);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsValidNavMeshDataHandle(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsValidLinkHandle(int handle);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object InternalGetOwner(int dataID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalSetOwner(int dataID, int ownerID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object InternalGetLinkOwner(int linkID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool InternalSetLinkOwner(int linkID, int ownerID);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int AddNavMeshDataInternal(NavMeshData navMeshData);

		internal static int AddNavMeshDataTransformedInternal(NavMeshData navMeshData, Vector3 position, Quaternion rotation)
		{
			return NavMesh.INTERNAL_CALL_AddNavMeshDataTransformedInternal(navMeshData, ref position, ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_AddNavMeshDataTransformedInternal(NavMeshData navMeshData, ref Vector3 position, ref Quaternion rotation);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RemoveNavMeshDataInternal(int handle);

		/// <summary>
		///   <para>Adds a link to the NavMesh. The link is described by the NavMeshLinkData struct.</para>
		/// </summary>
		/// <param name="link">Describing the properties of the link.</param>
		/// <returns>
		///   <para>Representing the added link.</para>
		/// </returns>
		public static NavMeshLinkInstance AddLink(NavMeshLinkData link)
		{
			return new NavMeshLinkInstance
			{
				id = NavMesh.AddLinkInternal(link, Vector3.zero, Quaternion.identity)
			};
		}

		/// <summary>
		///   <para>Adds a link to the NavMesh. The link is described by the NavMeshLinkData struct.</para>
		/// </summary>
		/// <param name="link">Describing the properties of the link.</param>
		/// <param name="position">Translate the link to this position.</param>
		/// <param name="rotation">Rotate the link to this orientation.</param>
		/// <returns>
		///   <para>Representing the added link.</para>
		/// </returns>
		public static NavMeshLinkInstance AddLink(NavMeshLinkData link, Vector3 position, Quaternion rotation)
		{
			return new NavMeshLinkInstance
			{
				id = NavMesh.AddLinkInternal(link, position, rotation)
			};
		}

		/// <summary>
		///   <para>Removes a link from the NavMesh.</para>
		/// </summary>
		/// <param name="handle">The instance of a link to remove.</param>
		public static void RemoveLink(NavMeshLinkInstance handle)
		{
			NavMesh.RemoveLinkInternal(handle.id);
		}

		internal static int AddLinkInternal(NavMeshLinkData link, Vector3 position, Quaternion rotation)
		{
			return NavMesh.INTERNAL_CALL_AddLinkInternal(ref link, ref position, ref rotation);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int INTERNAL_CALL_AddLinkInternal(ref NavMeshLinkData link, ref Vector3 position, ref Quaternion rotation);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RemoveLinkInternal(int handle);

		public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, NavMeshQueryFilter filter)
		{
			return NavMesh.SamplePositionFilter(sourcePosition, out hit, maxDistance, filter.agentTypeID, filter.areaMask);
		}

		private static bool SamplePositionFilter(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int type, int mask)
		{
			return NavMesh.INTERNAL_CALL_SamplePositionFilter(ref sourcePosition, out hit, maxDistance, type, mask);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_SamplePositionFilter(ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int type, int mask);

		public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, NavMeshQueryFilter filter)
		{
			return NavMesh.FindClosestEdgeFilter(sourcePosition, out hit, filter.agentTypeID, filter.areaMask);
		}

		private static bool FindClosestEdgeFilter(Vector3 sourcePosition, out NavMeshHit hit, int type, int mask)
		{
			return NavMesh.INTERNAL_CALL_FindClosestEdgeFilter(ref sourcePosition, out hit, type, mask);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_FindClosestEdgeFilter(ref Vector3 sourcePosition, out NavMeshHit hit, int type, int mask);

		public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, NavMeshQueryFilter filter)
		{
			return NavMesh.RaycastFilter(sourcePosition, targetPosition, out hit, filter.agentTypeID, filter.areaMask);
		}

		private static bool RaycastFilter(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int type, int mask)
		{
			return NavMesh.INTERNAL_CALL_RaycastFilter(ref sourcePosition, ref targetPosition, out hit, type, mask);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_RaycastFilter(ref Vector3 sourcePosition, ref Vector3 targetPosition, out NavMeshHit hit, int type, int mask);

		/// <summary>
		///   <para>Calculates a path between two positions mapped to the NavMesh, subject to the constraints and costs defined by the filter argument.</para>
		/// </summary>
		/// <param name="sourcePosition">The initial position of the path requested.</param>
		/// <param name="targetPosition">The final position of the path requested.</param>
		/// <param name="filter">A filter specifying the cost of NavMesh areas that can be passed when calculating a path.</param>
		/// <param name="path">The resulting path.</param>
		/// <returns>
		///   <para>True if a either a complete or partial path is found and false otherwise.</para>
		/// </returns>
		public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, NavMeshQueryFilter filter, NavMeshPath path)
		{
			path.ClearCorners();
			return NavMesh.CalculatePathFilterInternal(sourcePosition, targetPosition, path, filter.agentTypeID, filter.areaMask, filter.costs);
		}

		internal static bool CalculatePathFilterInternal(Vector3 sourcePosition, Vector3 targetPosition, NavMeshPath path, int type, int mask, float[] costs)
		{
			return NavMesh.INTERNAL_CALL_CalculatePathFilterInternal(ref sourcePosition, ref targetPosition, path, type, mask, costs);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool INTERNAL_CALL_CalculatePathFilterInternal(ref Vector3 sourcePosition, ref Vector3 targetPosition, NavMeshPath path, int type, int mask, float[] costs);

		/// <summary>
		///   <para>Creates and returns a new entry of NavMesh build settings available for runtime NavMesh building.</para>
		/// </summary>
		/// <returns>
		///   <para>The created settings.</para>
		/// </returns>
		public static NavMeshBuildSettings CreateSettings()
		{
			NavMeshBuildSettings navMeshBuildSettings;
			NavMesh.INTERNAL_CALL_CreateSettings(out navMeshBuildSettings);
			return navMeshBuildSettings;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_CreateSettings(out NavMeshBuildSettings value);

		/// <summary>
		///   <para>Removes the build settings matching the agent type ID.</para>
		/// </summary>
		/// <param name="agentTypeID">The ID of the entry to remove.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveSettings(int agentTypeID);

		/// <summary>
		///   <para>Returns an existing entry of NavMesh build settings.</para>
		/// </summary>
		/// <param name="agentTypeID">The ID to look for.</param>
		/// <returns>
		///   <para>The settings found.</para>
		/// </returns>
		public static NavMeshBuildSettings GetSettingsByID(int agentTypeID)
		{
			NavMeshBuildSettings navMeshBuildSettings;
			NavMesh.INTERNAL_CALL_GetSettingsByID(agentTypeID, out navMeshBuildSettings);
			return navMeshBuildSettings;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSettingsByID(int agentTypeID, out NavMeshBuildSettings value);

		/// <summary>
		///   <para>Returns the number of registered NavMesh build settings.</para>
		/// </summary>
		/// <returns>
		///   <para>The number of registered entries.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetSettingsCount();

		/// <summary>
		///   <para>Returns an existing entry of NavMesh build settings by its ordered index.</para>
		/// </summary>
		/// <param name="index">The index to retrieve from.</param>
		/// <returns>
		///   <para>The found settings.</para>
		/// </returns>
		public static NavMeshBuildSettings GetSettingsByIndex(int index)
		{
			NavMeshBuildSettings navMeshBuildSettings;
			NavMesh.INTERNAL_CALL_GetSettingsByIndex(index, out navMeshBuildSettings);
			return navMeshBuildSettings;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetSettingsByIndex(int index, out NavMeshBuildSettings value);

		/// <summary>
		///   <para>Returns the name associated with the NavMesh build settings matching the provided agent type ID.</para>
		/// </summary>
		/// <param name="agentTypeID">The ID to look for.</param>
		/// <returns>
		///   <para>The name associated with the ID found.</para>
		/// </returns>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetSettingsNameFromID(int agentTypeID);

		/// <summary>
		///   <para>Removes all NavMesh surfaces and links from the game.</para>
		/// </summary>
		[NativeName("CleanupAfterCarving")]
		[StaticAccessor("GetNavMeshManager()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveAllNavMeshData();

		[RequiredByNativeCode]
		private static void Internal_CallOnNavMeshPreUpdate()
		{
			if (NavMesh.onPreUpdate != null)
			{
				NavMesh.onPreUpdate();
			}
		}

		/// <summary>
		///   <para>Area mask constant that includes all NavMesh areas.</para>
		/// </summary>
		public const int AllAreas = -1;

		/// <summary>
		///   <para>Set a function to be called before the NavMesh is updated during the frame update execution.</para>
		/// </summary>
		public static NavMesh.OnNavMeshPreUpdate onPreUpdate;

		/// <summary>
		///   <para>A delegate which can be used to register callback methods to be invoked before the NavMesh system updates.</para>
		/// </summary>
		public delegate void OnNavMeshPreUpdate();
	}
}
