using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Bindings;

namespace UnityEngine.LowLevelPhysics2D
{
	[StaticAccessor("PhysicsComposer2D", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Physics2D/LowLevel/PhysicsComposer2D.h")]
	internal static class PhysicsComposerScripting2D
	{
		[NativeMethod(Name = "Create", IsThreadSafe = true)]
		internal static PhysicsComposer PhysicsComposer_Create(Allocator allocator)
		{
			PhysicsComposer physicsComposer;
			PhysicsComposerScripting2D.PhysicsComposer_Create_Injected(allocator, out physicsComposer);
			return physicsComposer;
		}

		[NativeMethod(Name = "Destroy", IsThreadSafe = true)]
		internal static bool PhysicsComposer_Destroy(PhysicsComposer composer)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_Destroy_Injected(ref composer);
		}

		[NativeMethod(Name = "IsValid", IsThreadSafe = true)]
		internal static bool Composer_IsValid(PhysicsComposer composer)
		{
			return PhysicsComposerScripting2D.Composer_IsValid_Injected(ref composer);
		}

		[NativeMethod(Name = "AddLayer", IsThreadSafe = true)]
		internal static PhysicsComposer.LayerHandle PhysicsComposer_AddLayer(PhysicsComposer composer, PhysicsComposer.Layer layer)
		{
			PhysicsComposer.LayerHandle layerHandle;
			PhysicsComposerScripting2D.PhysicsComposer_AddLayer_Injected(ref composer, ref layer, out layerHandle);
			return layerHandle;
		}

		[NativeMethod(Name = "RemoveLayer", IsThreadSafe = true)]
		internal static void PhysicsComposer_RemoveLayer(PhysicsComposer composer, PhysicsComposer.LayerHandle layerHandle)
		{
			PhysicsComposerScripting2D.PhysicsComposer_RemoveLayer_Injected(ref composer, ref layerHandle);
		}

		[NativeMethod(Name = "GetLayerCount", IsThreadSafe = true)]
		internal static int PhysicsComposer_GetLayerCount(PhysicsComposer composer)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_GetLayerCount_Injected(ref composer);
		}

		[NativeMethod(Name = "GetRejectedGeometryCount", IsThreadSafe = true)]
		internal static int PhysicsComposer_GetRejectedGeometryCount(PhysicsComposer composer)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_GetRejectedGeometryCount_Injected(ref composer);
		}

		[NativeMethod(Name = "GetLayerHandles", IsThreadSafe = true)]
		internal static PhysicsLowLevelScripting2D.PhysicsBuffer PhysicsComposer_GetLayerHandles(PhysicsComposer composer)
		{
			PhysicsLowLevelScripting2D.PhysicsBuffer physicsBuffer;
			PhysicsComposerScripting2D.PhysicsComposer_GetLayerHandles_Injected(ref composer, out physicsBuffer);
			return physicsBuffer;
		}

		[NativeMethod(Name = "SetDelaunay", IsThreadSafe = true)]
		internal static void PhysicsComposer_SetDelaunay(PhysicsComposer composer, bool flag)
		{
			PhysicsComposerScripting2D.PhysicsComposer_SetDelaunay_Injected(ref composer, flag);
		}

		[NativeMethod(Name = "GetDelaunay", IsThreadSafe = true)]
		internal static bool PhysicsComposer_GetDelaunay(PhysicsComposer composer)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_GetDelaunay_Injected(ref composer);
		}

		[NativeMethod(Name = "SetMaxPolygonVertices", IsThreadSafe = true)]
		internal static void PhysicsComposer_SetMaxPolygonVertices(PhysicsComposer composer, int maxPolygonVertices)
		{
			PhysicsComposerScripting2D.PhysicsComposer_SetMaxPolygonVertices_Injected(ref composer, maxPolygonVertices);
		}

		[NativeMethod(Name = "GetMaxPolygonVertices", IsThreadSafe = true)]
		internal static int PhysicsComposer_GetMaxPolygonVertices(PhysicsComposer composer)
		{
			return PhysicsComposerScripting2D.PhysicsComposer_GetMaxPolygonVertices_Injected(ref composer);
		}

		[NativeMethod(Name = "CreatePolygonGeometry", IsThreadSafe = true)]
		internal static PhysicsLowLevelScripting2D.PhysicsBuffer PhysicsComposer_CreatePolygonGeometry(PhysicsComposer composer, Vector2 vertexScale, Allocator allocator)
		{
			PhysicsLowLevelScripting2D.PhysicsBuffer physicsBuffer;
			PhysicsComposerScripting2D.PhysicsComposer_CreatePolygonGeometry_Injected(ref composer, ref vertexScale, allocator, out physicsBuffer);
			return physicsBuffer;
		}

		[NativeMethod(Name = "CreateConvexHulls", IsThreadSafe = true)]
		internal static PhysicsLowLevelScripting2D.PhysicsBuffer PhysicsComposer_CreateConvexHulls(PhysicsComposer composer, Vector2 vertexScale, Allocator allocator)
		{
			PhysicsLowLevelScripting2D.PhysicsBuffer physicsBuffer;
			PhysicsComposerScripting2D.PhysicsComposer_CreateConvexHulls_Injected(ref composer, ref vertexScale, allocator, out physicsBuffer);
			return physicsBuffer;
		}

		[NativeMethod(Name = "CreateChainGeometry", IsThreadSafe = true)]
		internal static PhysicsLowLevelScripting2D.PhysicsBufferPair PhysicsComposer_CreateChainGeometry(PhysicsComposer composer, Vector2 vertexScale, Allocator allocator)
		{
			PhysicsLowLevelScripting2D.PhysicsBufferPair physicsBufferPair;
			PhysicsComposerScripting2D.PhysicsComposer_CreateChainGeometry_Injected(ref composer, ref vertexScale, allocator, out physicsBufferPair);
			return physicsBufferPair;
		}

		[NativeMethod(Name = "GetGeometryIslands", IsThreadSafe = true)]
		internal static PhysicsLowLevelScripting2D.PhysicsBuffer PhysicsComposer_GetGeometryIslands(PhysicsComposer composer, Allocator allocator)
		{
			PhysicsLowLevelScripting2D.PhysicsBuffer physicsBuffer;
			PhysicsComposerScripting2D.PhysicsComposer_GetGeometryIslands_Injected(ref composer, allocator, out physicsBuffer);
			return physicsBuffer;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_Create_Injected(Allocator allocator, out PhysicsComposer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool PhysicsComposer_Destroy_Injected([In] ref PhysicsComposer composer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Composer_IsValid_Injected([In] ref PhysicsComposer composer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_AddLayer_Injected([In] ref PhysicsComposer composer, [In] ref PhysicsComposer.Layer layer, out PhysicsComposer.LayerHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_RemoveLayer_Injected([In] ref PhysicsComposer composer, [In] ref PhysicsComposer.LayerHandle layerHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PhysicsComposer_GetLayerCount_Injected([In] ref PhysicsComposer composer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PhysicsComposer_GetRejectedGeometryCount_Injected([In] ref PhysicsComposer composer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_GetLayerHandles_Injected([In] ref PhysicsComposer composer, out PhysicsLowLevelScripting2D.PhysicsBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_SetDelaunay_Injected([In] ref PhysicsComposer composer, bool flag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool PhysicsComposer_GetDelaunay_Injected([In] ref PhysicsComposer composer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_SetMaxPolygonVertices_Injected([In] ref PhysicsComposer composer, int maxPolygonVertices);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PhysicsComposer_GetMaxPolygonVertices_Injected([In] ref PhysicsComposer composer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_CreatePolygonGeometry_Injected([In] ref PhysicsComposer composer, [In] ref Vector2 vertexScale, Allocator allocator, out PhysicsLowLevelScripting2D.PhysicsBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_CreateConvexHulls_Injected([In] ref PhysicsComposer composer, [In] ref Vector2 vertexScale, Allocator allocator, out PhysicsLowLevelScripting2D.PhysicsBuffer ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_CreateChainGeometry_Injected([In] ref PhysicsComposer composer, [In] ref Vector2 vertexScale, Allocator allocator, out PhysicsLowLevelScripting2D.PhysicsBufferPair ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PhysicsComposer_GetGeometryIslands_Injected([In] ref PhysicsComposer composer, Allocator allocator, out PhysicsLowLevelScripting2D.PhysicsBuffer ret);
	}
}
