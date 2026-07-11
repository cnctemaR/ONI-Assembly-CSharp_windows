using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Math/Matrix4x4.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class BatchRendererGroup : IDisposable
	{
		public BatchRendererGroup(BatchRendererGroup.OnPerformCulling cullingCallback)
		{
			this.m_PerformCulling = cullingCallback;
			this.m_GroupHandle = BatchRendererGroup.Create(this);
		}

		public void Dispose()
		{
			BatchRendererGroup.Destroy(this.m_GroupHandle);
			this.m_GroupHandle = IntPtr.Zero;
		}

		public int AddBatch(Mesh mesh, int subMeshIndex, Material material, int layer, ShadowCastingMode castShadows, bool receiveShadows, bool invertCulling, Bounds bounds, int instanceCount, MaterialPropertyBlock customProps, GameObject associatedSceneObject, ulong sceneCullingMask = 9223372036854775808UL)
		{
			return this.AddBatch_Injected(mesh, subMeshIndex, material, layer, castShadows, receiveShadows, invertCulling, ref bounds, instanceCount, customProps, associatedSceneObject, sceneCullingMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInstancingData(int batchIndex, int instanceCount, MaterialPropertyBlock customProps);

		public unsafe NativeArray<Matrix4x4> GetBatchMatrices(int batchIndex)
		{
			int num = 0;
			void* batchMatrices = this.GetBatchMatrices(batchIndex, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(batchMatrices, num, Allocator.Invalid);
		}

		public unsafe NativeArray<float> GetBatchScalarArray(int batchIndex, string propertyName)
		{
			int num = 0;
			void* batchScalarArray = this.GetBatchScalarArray(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(batchScalarArray, num, Allocator.Invalid);
		}

		public unsafe NativeArray<Vector4> GetBatchVectorArray(int batchIndex, string propertyName)
		{
			int num = 0;
			void* batchVectorArray = this.GetBatchVectorArray(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(batchVectorArray, num, Allocator.Invalid);
		}

		public unsafe NativeArray<Matrix4x4> GetBatchMatrixArray(int batchIndex, string propertyName)
		{
			int num = 0;
			void* batchMatrixArray = this.GetBatchMatrixArray(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(batchMatrixArray, num, Allocator.Invalid);
		}

		public unsafe NativeArray<float> GetBatchScalarArray(int batchIndex, int propertyName)
		{
			int num = 0;
			void* batchScalarArray_Int = this.GetBatchScalarArray_Int(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(batchScalarArray_Int, num, Allocator.Invalid);
		}

		public unsafe NativeArray<Vector4> GetBatchVectorArray(int batchIndex, int propertyName)
		{
			int num = 0;
			void* batchVectorArray_Int = this.GetBatchVectorArray_Int(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(batchVectorArray_Int, num, Allocator.Invalid);
		}

		public unsafe NativeArray<Matrix4x4> GetBatchMatrixArray(int batchIndex, int propertyName)
		{
			int num = 0;
			void* batchMatrixArray_Int = this.GetBatchMatrixArray_Int(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(batchMatrixArray_Int, num, Allocator.Invalid);
		}

		public void SetBatchBounds(int batchIndex, Bounds bounds)
		{
			this.SetBatchBounds_Injected(batchIndex, ref bounds);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int GetNumBatches();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveBatch(int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchMatrices(int batchIndex, out int matrixCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchScalarArray(int batchIndex, string propertyName, out int elementCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchVectorArray(int batchIndex, string propertyName, out int elementCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchMatrixArray(int batchIndex, string propertyName, out int elementCount);

		[NativeName("GetBatchScalarArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchScalarArray_Int(int batchIndex, int propertyName, out int elementCount);

		[NativeName("GetBatchVectorArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchVectorArray_Int(int batchIndex, int propertyName, out int elementCount);

		[NativeName("GetBatchMatrixArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchMatrixArray_Int(int batchIndex, int propertyName, out int elementCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Create(BatchRendererGroup group);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr groupHandle);

		[RequiredByNativeCode]
		private unsafe static void InvokeOnPerformCulling(BatchRendererGroup group, ref BatchRendererCullingOutput context, ref LODParameters lodParameters)
		{
			NativeArray<Plane> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Plane>((void*)context.cullingPlanes, context.cullingPlanesCount, Allocator.Invalid);
			NativeArray<BatchVisibility> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BatchVisibility>((void*)context.batchVisibility, context.batchVisibilityCount, Allocator.Invalid);
			NativeArray<int> nativeArray3 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)context.visibleIndices, context.visibleIndicesCount, Allocator.Invalid);
			try
			{
				context.cullingJobsFence = group.m_PerformCulling(group, new BatchCullingContext(nativeArray, nativeArray2, nativeArray3, lodParameters));
			}
			finally
			{
				JobHandle.ScheduleBatchedJobs();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddBatch_Injected(Mesh mesh, int subMeshIndex, Material material, int layer, ShadowCastingMode castShadows, bool receiveShadows, bool invertCulling, ref Bounds bounds, int instanceCount, MaterialPropertyBlock customProps, GameObject associatedSceneObject, ulong sceneCullingMask = 9223372036854775808UL);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBatchBounds_Injected(int batchIndex, ref Bounds bounds);

		private IntPtr m_GroupHandle = IntPtr.Zero;

		private BatchRendererGroup.OnPerformCulling m_PerformCulling;

		public delegate JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext);
	}
}
