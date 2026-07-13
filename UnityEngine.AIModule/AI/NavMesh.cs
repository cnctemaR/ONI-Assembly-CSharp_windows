using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.AI
{
	[MovedFrom("UnityEngine")]
	[NativeHeader("Modules/AI/NavMesh/NavMesh.bindings.h")]
	[NativeHeader("Modules/AI/NavMeshManager.h")]
	[StaticAccessor("NavMeshBindings", StaticAccessorType.DoubleColon)]
	public static class NavMesh
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void ClearPreUpdateListeners()
		{
			NavMesh.onPreUpdate = null;
		}

		[RequiredByNativeCode]
		private static void Internal_CallOnNavMeshPreUpdate()
		{
			bool flag = NavMesh.onPreUpdate != null;
			if (flag)
			{
				NavMesh.onPreUpdate();
			}
		}

		public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int areaMask)
		{
			return NavMesh.Raycast_Injected(ref sourcePosition, ref targetPosition, out hit, areaMask);
		}

		public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
		{
			path.ClearCorners();
			return NavMesh.CalculatePathInternal(sourcePosition, targetPosition, areaMask, path);
		}

		private static bool CalculatePathInternal(Vector3 sourcePosition, Vector3 targetPosition, int areaMask, NavMeshPath path)
		{
			return NavMesh.CalculatePathInternal_Injected(ref sourcePosition, ref targetPosition, areaMask, (path == null) ? ((IntPtr)0) : NavMeshPath.BindingsMarshaller.ConvertToNative(path));
		}

		public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, int areaMask)
		{
			return NavMesh.FindClosestEdge_Injected(ref sourcePosition, out hit, areaMask);
		}

		public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask)
		{
			return NavMesh.SamplePosition_Injected(ref sourcePosition, out hit, maxDistance, areaMask);
		}

		[NativeName("SetAreaCost")]
		[Obsolete("Use SetAreaCost instead.")]
		[StaticAccessor("GetNavMeshProjectSettings()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetLayerCost(int layer, float cost);

		[NativeName("GetAreaCost")]
		[StaticAccessor("GetNavMeshProjectSettings()")]
		[Obsolete("Use GetAreaCost instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetLayerCost(int layer);

		[Obsolete("Use GetAreaFromName instead.")]
		[StaticAccessor("GetNavMeshProjectSettings()")]
		[NativeName("GetAreaFromName")]
		public unsafe static int GetNavMeshLayerFromName(string layerName)
		{
			int navMeshLayerFromName_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(layerName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = layerName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				navMeshLayerFromName_Injected = NavMesh.GetNavMeshLayerFromName_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return navMeshLayerFromName_Injected;
		}

		[NativeName("SetAreaCost")]
		[StaticAccessor("GetNavMeshProjectSettings()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAreaCost(int areaIndex, float cost);

		[StaticAccessor("GetNavMeshProjectSettings()")]
		[NativeName("GetAreaCost")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern float GetAreaCost(int areaIndex);

		[NativeName("GetAreaFromName")]
		[StaticAccessor("GetNavMeshProjectSettings()")]
		public unsafe static int GetAreaFromName(string areaName)
		{
			int areaFromName_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(areaName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = areaName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				areaFromName_Injected = NavMesh.GetAreaFromName_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return areaFromName_Injected;
		}

		[StaticAccessor("GetNavMeshProjectSettings()")]
		[NativeName("GetAreaNames")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string[] GetAreaNames();

		public static NavMeshTriangulation CalculateTriangulation()
		{
			NavMeshTriangulation navMeshTriangulation;
			NavMesh.CalculateTriangulation_Injected(out navMeshTriangulation);
			return navMeshTriangulation;
		}

		[Obsolete("use NavMesh.CalculateTriangulation() instead.")]
		public static void Triangulate(out Vector3[] vertices, out int[] indices)
		{
			NavMeshTriangulation navMeshTriangulation = NavMesh.CalculateTriangulation();
			vertices = navMeshTriangulation.vertices;
			indices = navMeshTriangulation.indices;
		}

		[Obsolete("AddOffMeshLinks has no effect and is deprecated.")]
		public static void AddOffMeshLinks()
		{
		}

		[Obsolete("RestoreNavMesh has no effect and is deprecated.")]
		public static void RestoreNavMesh()
		{
		}

		[StaticAccessor("GetNavMeshManager()")]
		public static extern float avoidancePredictionTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[StaticAccessor("GetNavMeshManager()")]
		public static extern int pathfindingIterationsPerFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public static NavMeshDataInstance AddNavMeshData(NavMeshData navMeshData)
		{
			bool flag = navMeshData == null;
			if (flag)
			{
				throw new ArgumentNullException("navMeshData");
			}
			return new NavMeshDataInstance
			{
				id = NavMesh.AddNavMeshDataInternal(navMeshData)
			};
		}

		public static NavMeshDataInstance AddNavMeshData(NavMeshData navMeshData, Vector3 position, Quaternion rotation)
		{
			bool flag = navMeshData == null;
			if (flag)
			{
				throw new ArgumentNullException("navMeshData");
			}
			return new NavMeshDataInstance
			{
				id = NavMesh.AddNavMeshDataTransformedInternal(navMeshData, position, rotation)
			};
		}

		public static void RemoveNavMeshData(NavMeshDataInstance handle)
		{
			NavMesh.RemoveNavMeshDataInternal(handle.id);
		}

		[NativeName("IsValidSurfaceID")]
		[StaticAccessor("GetNavMeshManager()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsValidNavMeshDataHandle(int handle);

		[StaticAccessor("GetNavMeshManager()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsValidLinkHandle(int handle);

		internal static Object InternalGetOwner(int dataID)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(NavMesh.InternalGetOwner_Injected(dataID));
		}

		[NativeName("SetSurfaceUserID")]
		[StaticAccessor("GetNavMeshManager()")]
		internal static bool InternalSetOwner(int dataID, EntityId ownerID)
		{
			return NavMesh.InternalSetOwner_Injected(dataID, ref ownerID);
		}

		internal static Object InternalGetLinkOwner(int linkID)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(NavMesh.InternalGetLinkOwner_Injected(linkID));
		}

		[StaticAccessor("GetNavMeshManager()")]
		[NativeName("SetLinkUserID")]
		internal static bool InternalSetLinkOwner(int linkID, EntityId ownerID)
		{
			return NavMesh.InternalSetLinkOwner_Injected(linkID, ref ownerID);
		}

		[NativeName("LoadData")]
		[StaticAccessor("GetNavMeshManager()")]
		internal static int AddNavMeshDataInternal(NavMeshData navMeshData)
		{
			return NavMesh.AddNavMeshDataInternal_Injected(Object.MarshalledUnityObject.Marshal<NavMeshData>(navMeshData));
		}

		[NativeName("LoadData")]
		[StaticAccessor("GetNavMeshManager()")]
		internal static int AddNavMeshDataTransformedInternal(NavMeshData navMeshData, Vector3 position, Quaternion rotation)
		{
			return NavMesh.AddNavMeshDataTransformedInternal_Injected(Object.MarshalledUnityObject.Marshal<NavMeshData>(navMeshData), ref position, ref rotation);
		}

		[NativeName("UnloadData")]
		[StaticAccessor("GetNavMeshManager()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RemoveNavMeshDataInternal(int handle);

		public static NavMeshLinkInstance AddLink(NavMeshLinkData link)
		{
			return new NavMeshLinkInstance
			{
				id = NavMesh.AddLinkInternal(link, Vector3.zero, Quaternion.identity)
			};
		}

		public static NavMeshLinkInstance AddLink(NavMeshLinkData link, Vector3 position, Quaternion rotation)
		{
			return new NavMeshLinkInstance
			{
				id = NavMesh.AddLinkInternal(link, position, rotation)
			};
		}

		public static void RemoveLink(NavMeshLinkInstance handle)
		{
			NavMesh.RemoveLinkInternal(handle.id);
		}

		public static bool IsLinkActive(NavMeshLinkInstance handle)
		{
			return NavMesh.IsOffMeshConnectionActive(handle.id);
		}

		public static void SetLinkActive(NavMeshLinkInstance handle, bool value)
		{
			NavMesh.SetOffMeshConnectionActive(handle.id, value);
		}

		public static bool IsLinkOccupied(NavMeshLinkInstance handle)
		{
			return NavMesh.IsOffMeshConnectionOccupied(handle.id);
		}

		public static bool IsLinkValid(NavMeshLinkInstance handle)
		{
			return NavMesh.IsValidLinkHandle(handle.id);
		}

		public static Object GetLinkOwner(NavMeshLinkInstance handle)
		{
			return NavMesh.InternalGetLinkOwner(handle.id);
		}

		public static void SetLinkOwner(NavMeshLinkInstance handle, Object owner)
		{
			int num = ((owner != null) ? owner.GetInstanceID() : 0);
			bool flag = !NavMesh.InternalSetLinkOwner(handle.id, num);
			if (flag)
			{
				Debug.LogError("Cannot set 'owner' on an invalid NavMeshLinkInstance");
			}
		}

		[NativeName("AddLink")]
		[StaticAccessor("GetNavMeshManager()")]
		internal static int AddLinkInternal(NavMeshLinkData link, Vector3 position, Quaternion rotation)
		{
			return NavMesh.AddLinkInternal_Injected(ref link, ref position, ref rotation);
		}

		[StaticAccessor("GetNavMeshManager()")]
		[NativeName("RemoveLink")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void RemoveLinkInternal(int handle);

		[StaticAccessor("GetNavMeshManager()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsOffMeshConnectionOccupied(int handle);

		[StaticAccessor("GetNavMeshManager()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsOffMeshConnectionActive(int linkHandle);

		[StaticAccessor("GetNavMeshManager()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetOffMeshConnectionActive(int linkHandle, bool activated);

		public static bool SamplePosition(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, NavMeshQueryFilter filter)
		{
			return NavMesh.SamplePositionFilter(sourcePosition, out hit, maxDistance, filter.agentTypeID, filter.areaMask);
		}

		private static bool SamplePositionFilter(Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int type, int mask)
		{
			return NavMesh.SamplePositionFilter_Injected(ref sourcePosition, out hit, maxDistance, type, mask);
		}

		public static bool FindClosestEdge(Vector3 sourcePosition, out NavMeshHit hit, NavMeshQueryFilter filter)
		{
			return NavMesh.FindClosestEdgeFilter(sourcePosition, out hit, filter.agentTypeID, filter.areaMask);
		}

		private static bool FindClosestEdgeFilter(Vector3 sourcePosition, out NavMeshHit hit, int type, int mask)
		{
			return NavMesh.FindClosestEdgeFilter_Injected(ref sourcePosition, out hit, type, mask);
		}

		public static bool Raycast(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, NavMeshQueryFilter filter)
		{
			return NavMesh.RaycastFilter(sourcePosition, targetPosition, out hit, filter.agentTypeID, filter.areaMask);
		}

		private static bool RaycastFilter(Vector3 sourcePosition, Vector3 targetPosition, out NavMeshHit hit, int type, int mask)
		{
			return NavMesh.RaycastFilter_Injected(ref sourcePosition, ref targetPosition, out hit, type, mask);
		}

		public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, NavMeshQueryFilter filter, NavMeshPath path)
		{
			path.ClearCorners();
			return NavMesh.CalculatePathFilterInternal(sourcePosition, targetPosition, path, filter.agentTypeID, filter.areaMask, filter.costs);
		}

		private unsafe static bool CalculatePathFilterInternal(Vector3 sourcePosition, Vector3 targetPosition, NavMeshPath path, int type, int mask, float[] costs)
		{
			IntPtr intPtr = ((path == null) ? ((IntPtr)0) : NavMeshPath.BindingsMarshaller.ConvertToNative(path));
			Span<float> span = new Span<float>(costs);
			bool flag;
			fixed (float* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				flag = NavMesh.CalculatePathFilterInternal_Injected(ref sourcePosition, ref targetPosition, intPtr, type, mask, ref managedSpanWrapper);
			}
			return flag;
		}

		[StaticAccessor("GetNavMeshProjectSettings()")]
		public static NavMeshBuildSettings CreateSettings()
		{
			NavMeshBuildSettings navMeshBuildSettings;
			NavMesh.CreateSettings_Injected(out navMeshBuildSettings);
			return navMeshBuildSettings;
		}

		[StaticAccessor("GetNavMeshProjectSettings()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveSettings(int agentTypeID);

		public static NavMeshBuildSettings GetSettingsByID(int agentTypeID)
		{
			NavMeshBuildSettings navMeshBuildSettings;
			NavMesh.GetSettingsByID_Injected(agentTypeID, out navMeshBuildSettings);
			return navMeshBuildSettings;
		}

		[StaticAccessor("GetNavMeshProjectSettings()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetSettingsCount();

		public static NavMeshBuildSettings GetSettingsByIndex(int index)
		{
			NavMeshBuildSettings navMeshBuildSettings;
			NavMesh.GetSettingsByIndex_Injected(index, out navMeshBuildSettings);
			return navMeshBuildSettings;
		}

		public static string GetSettingsNameFromID(int agentTypeID)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				NavMesh.GetSettingsNameFromID_Injected(agentTypeID, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[NativeName("CleanupAfterCarving")]
		[StaticAccessor("GetNavMeshManager()")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void RemoveAllNavMeshData();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Raycast_Injected([In] ref Vector3 sourcePosition, [In] ref Vector3 targetPosition, out NavMeshHit hit, int areaMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CalculatePathInternal_Injected([In] ref Vector3 sourcePosition, [In] ref Vector3 targetPosition, int areaMask, IntPtr path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool FindClosestEdge_Injected([In] ref Vector3 sourcePosition, out NavMeshHit hit, int areaMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SamplePosition_Injected([In] ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int areaMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetNavMeshLayerFromName_Injected(ref ManagedSpanWrapper layerName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetAreaFromName_Injected(ref ManagedSpanWrapper areaName);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateTriangulation_Injected(out NavMeshTriangulation ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetOwner_Injected(int dataID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalSetOwner_Injected(int dataID, [In] ref EntityId ownerID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InternalGetLinkOwner_Injected(int linkID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalSetLinkOwner_Injected(int linkID, [In] ref EntityId ownerID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddNavMeshDataInternal_Injected(IntPtr navMeshData);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddNavMeshDataTransformedInternal_Injected(IntPtr navMeshData, [In] ref Vector3 position, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddLinkInternal_Injected([In] ref NavMeshLinkData link, [In] ref Vector3 position, [In] ref Quaternion rotation);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SamplePositionFilter_Injected([In] ref Vector3 sourcePosition, out NavMeshHit hit, float maxDistance, int type, int mask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool FindClosestEdgeFilter_Injected([In] ref Vector3 sourcePosition, out NavMeshHit hit, int type, int mask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool RaycastFilter_Injected([In] ref Vector3 sourcePosition, [In] ref Vector3 targetPosition, out NavMeshHit hit, int type, int mask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CalculatePathFilterInternal_Injected([In] ref Vector3 sourcePosition, [In] ref Vector3 targetPosition, IntPtr path, int type, int mask, ref ManagedSpanWrapper costs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateSettings_Injected(out NavMeshBuildSettings ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSettingsByID_Injected(int agentTypeID, out NavMeshBuildSettings ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSettingsByIndex_Injected(int index, out NavMeshBuildSettings ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSettingsNameFromID_Injected(int agentTypeID, out ManagedSpanWrapper ret);

		public const int AllAreas = -1;

		public static NavMesh.OnNavMeshPreUpdate onPreUpdate;

		public delegate void OnNavMeshPreUpdate();
	}
}
