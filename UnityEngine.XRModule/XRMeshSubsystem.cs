using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR
{
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshingSubsystem.h")]
	[NativeConditional("ENABLE_XR")]
	[UsedByNativeCode]
	public class XRMeshSubsystem : IntegratedSubsystem<XRMeshSubsystemDescriptor>
	{
		public bool TryGetMeshInfos(List<MeshInfo> meshInfosOut)
		{
			bool flag = meshInfosOut == null;
			if (flag)
			{
				throw new ArgumentNullException("meshInfosOut");
			}
			return this.GetMeshInfosAsList(meshInfosOut);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetMeshInfosAsList(List<MeshInfo> meshInfos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern MeshInfo[] GetMeshInfosAsFixedArray();

		public void GenerateMeshAsync(MeshId meshId, Mesh mesh, MeshCollider meshCollider, MeshVertexAttributes attributes, Action<MeshGenerationResult> onMeshGenerationComplete)
		{
			this.GenerateMeshAsync(meshId, mesh, meshCollider, attributes, onMeshGenerationComplete, MeshGenerationOptions.None);
		}

		public void GenerateMeshAsync(MeshId meshId, Mesh mesh, MeshCollider meshCollider, MeshVertexAttributes attributes, Action<MeshGenerationResult> onMeshGenerationComplete, MeshGenerationOptions options)
		{
			this.GenerateMeshAsync_Injected(ref meshId, mesh, meshCollider, attributes, onMeshGenerationComplete, options);
		}

		[RequiredByNativeCode]
		private void InvokeMeshReadyDelegate(MeshGenerationResult result, Action<MeshGenerationResult> onMeshGenerationComplete)
		{
			bool flag = onMeshGenerationComplete != null;
			if (flag)
			{
				onMeshGenerationComplete(result);
			}
		}

		public extern float meshDensity
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public bool SetBoundingVolume(Vector3 origin, Vector3 extents)
		{
			return this.SetBoundingVolume_Injected(ref origin, ref extents);
		}

		public unsafe NativeArray<MeshTransform> GetUpdatedMeshTransforms(Allocator allocator)
		{
			NativeArray<MeshTransform> nativeArray2;
			using (XRMeshSubsystem.MeshTransformList meshTransformList = new XRMeshSubsystem.MeshTransformList(this.GetUpdatedMeshTransforms()))
			{
				NativeArray<MeshTransform> nativeArray = new NativeArray<MeshTransform>(meshTransformList.Count, allocator, NativeArrayOptions.UninitializedMemory);
				UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr<MeshTransform>(), meshTransformList.Data.ToPointer(), (long)(meshTransformList.Count * sizeof(MeshTransform)));
				nativeArray2 = nativeArray;
			}
			return nativeArray2;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern IntPtr GetUpdatedMeshTransforms();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GenerateMeshAsync_Injected(ref MeshId meshId, Mesh mesh, MeshCollider meshCollider, MeshVertexAttributes attributes, Action<MeshGenerationResult> onMeshGenerationComplete, MeshGenerationOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool SetBoundingVolume_Injected(ref Vector3 origin, ref Vector3 extents);

		[NativeConditional("ENABLE_XR")]
		private readonly struct MeshTransformList : IDisposable
		{
			public MeshTransformList(IntPtr self)
			{
				this.m_Self = self;
			}

			public int Count
			{
				get
				{
					return XRMeshSubsystem.MeshTransformList.GetLength(this.m_Self);
				}
			}

			public IntPtr Data
			{
				get
				{
					return XRMeshSubsystem.MeshTransformList.GetData(this.m_Self);
				}
			}

			public void Dispose()
			{
				XRMeshSubsystem.MeshTransformList.Dispose(this.m_Self);
			}

			[FreeFunction("UnityXRMeshTransformList_get_Length")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetLength(IntPtr self);

			[FreeFunction("UnityXRMeshTransformList_get_Data")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern IntPtr GetData(IntPtr self);

			[FreeFunction("UnityXRMeshTransformList_Dispose")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern void Dispose(IntPtr self);

			private readonly IntPtr m_Self;
		}
	}
}
