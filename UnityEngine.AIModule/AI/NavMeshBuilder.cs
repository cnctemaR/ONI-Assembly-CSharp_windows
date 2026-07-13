using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.AI
{
	[StaticAccessor("NavMeshBuilderBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/AI/Builder/NavMeshBuilder.bindings.h")]
	public static class NavMeshBuilder
	{
		public static void CollectSources(Bounds includedWorldBounds, int includedLayerMask, NavMeshCollectGeometry geometry, int defaultArea, bool generateLinksByDefault, List<NavMeshBuildMarkup> markups, bool includeOnlyMarkedObjects, List<NavMeshBuildSource> results)
		{
			bool flag = markups == null;
			if (flag)
			{
				throw new ArgumentNullException("markups");
			}
			bool flag2 = results == null;
			if (flag2)
			{
				throw new ArgumentNullException("results");
			}
			includedWorldBounds.extents = Vector3.Max(includedWorldBounds.extents, 0.001f * Vector3.one);
			NavMeshBuildSource[] array = NavMeshBuilder.CollectSourcesInternal(includedLayerMask, includedWorldBounds, null, true, geometry, defaultArea, generateLinksByDefault, markups.ToArray(), includeOnlyMarkedObjects);
			results.Clear();
			results.AddRange(array);
		}

		public static void CollectSources(Bounds includedWorldBounds, int includedLayerMask, NavMeshCollectGeometry geometry, int defaultArea, List<NavMeshBuildMarkup> markups, List<NavMeshBuildSource> results)
		{
			NavMeshBuilder.CollectSources(includedWorldBounds, includedLayerMask, geometry, defaultArea, false, markups, false, results);
		}

		public static void CollectSources(Transform root, int includedLayerMask, NavMeshCollectGeometry geometry, int defaultArea, bool generateLinksByDefault, List<NavMeshBuildMarkup> markups, bool includeOnlyMarkedObjects, List<NavMeshBuildSource> results)
		{
			bool flag = markups == null;
			if (flag)
			{
				throw new ArgumentNullException("markups");
			}
			bool flag2 = results == null;
			if (flag2)
			{
				throw new ArgumentNullException("results");
			}
			NavMeshBuildSource[] array = NavMeshBuilder.CollectSourcesInternal(includedLayerMask, default(Bounds), root, false, geometry, defaultArea, generateLinksByDefault, markups.ToArray(), includeOnlyMarkedObjects);
			results.Clear();
			results.AddRange(array);
		}

		public static void CollectSources(Transform root, int includedLayerMask, NavMeshCollectGeometry geometry, int defaultArea, List<NavMeshBuildMarkup> markups, List<NavMeshBuildSource> results)
		{
			NavMeshBuilder.CollectSources(root, includedLayerMask, geometry, defaultArea, false, markups, false, results);
		}

		private unsafe static NavMeshBuildSource[] CollectSourcesInternal(int includedLayerMask, Bounds includedWorldBounds, Transform root, bool useBounds, NavMeshCollectGeometry geometry, int defaultArea, bool generateLinksByDefault, NavMeshBuildMarkup[] markups, bool includeOnlyMarkedObjects)
		{
			NavMeshBuildSource[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.Marshal<Transform>(root);
				Span<NavMeshBuildMarkup> span = new Span<NavMeshBuildMarkup>(markups);
				fixed (NavMeshBuildMarkup* ptr = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
					BlittableArrayWrapper blittableArrayWrapper;
					NavMeshBuilder.CollectSourcesInternal_Injected(includedLayerMask, ref includedWorldBounds, intPtr, useBounds, geometry, defaultArea, generateLinksByDefault, ref managedSpanWrapper, includeOnlyMarkedObjects, out blittableArrayWrapper);
				}
			}
			finally
			{
				NavMeshBuildMarkup* ptr = null;
				BlittableArrayWrapper blittableArrayWrapper;
				NavMeshBuildSource[] array;
				blittableArrayWrapper.Unmarshal<NavMeshBuildSource>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static NavMeshData BuildNavMeshData(NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds, Vector3 position, Quaternion rotation)
		{
			bool flag = sources == null;
			if (flag)
			{
				throw new ArgumentNullException("sources");
			}
			NavMeshData navMeshData = new NavMeshData(buildSettings.agentTypeID)
			{
				position = position,
				rotation = rotation
			};
			NavMeshBuilder.UpdateNavMeshDataListInternal(navMeshData, buildSettings, sources, localBounds);
			return navMeshData;
		}

		public static bool UpdateNavMeshData(NavMeshData data, NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = sources == null;
			if (flag2)
			{
				throw new ArgumentNullException("sources");
			}
			return NavMeshBuilder.UpdateNavMeshDataListInternal(data, buildSettings, sources, localBounds);
		}

		private static bool UpdateNavMeshDataListInternal(NavMeshData data, NavMeshBuildSettings buildSettings, object sources, Bounds localBounds)
		{
			return NavMeshBuilder.UpdateNavMeshDataListInternal_Injected(Object.MarshalledUnityObject.Marshal<NavMeshData>(data), ref buildSettings, sources, ref localBounds);
		}

		public static AsyncOperation UpdateNavMeshDataAsync(NavMeshData data, NavMeshBuildSettings buildSettings, List<NavMeshBuildSource> sources, Bounds localBounds)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = sources == null;
			if (flag2)
			{
				throw new ArgumentNullException("sources");
			}
			return NavMeshBuilder.UpdateNavMeshDataAsyncListInternal(data, buildSettings, sources, localBounds);
		}

		[NativeHeader("Modules/AI/NavMeshManager.h")]
		[NativeMethod("Purge")]
		[StaticAccessor("GetNavMeshManager().GetNavMeshBuildManager()", StaticAccessorType.Arrow)]
		public static void Cancel(NavMeshData data)
		{
			NavMeshBuilder.Cancel_Injected(Object.MarshalledUnityObject.Marshal<NavMeshData>(data));
		}

		private static AsyncOperation UpdateNavMeshDataAsyncListInternal(NavMeshData data, NavMeshBuildSettings buildSettings, object sources, Bounds localBounds)
		{
			IntPtr intPtr = NavMeshBuilder.UpdateNavMeshDataAsyncListInternal_Injected(Object.MarshalledUnityObject.Marshal<NavMeshData>(data), ref buildSettings, sources, ref localBounds);
			return (intPtr == 0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CollectSourcesInternal_Injected(int includedLayerMask, [In] ref Bounds includedWorldBounds, IntPtr root, bool useBounds, NavMeshCollectGeometry geometry, int defaultArea, bool generateLinksByDefault, ref ManagedSpanWrapper markups, bool includeOnlyMarkedObjects, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UpdateNavMeshDataListInternal_Injected(IntPtr data, [In] ref NavMeshBuildSettings buildSettings, object sources, [In] ref Bounds localBounds);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Cancel_Injected(IntPtr data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr UpdateNavMeshDataAsyncListInternal_Injected(IntPtr data, [In] ref NavMeshBuildSettings buildSettings, object sources, [In] ref Bounds localBounds);
	}
}
