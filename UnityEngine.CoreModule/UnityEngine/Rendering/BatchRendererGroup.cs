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
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
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

		public int AddBatch(Mesh mesh, int subMeshIndex, Material material, int layer, ShadowCastingMode castShadows, bool receiveShadows, bool invertCulling, Bounds bounds, int instanceCount, MaterialPropertyBlock customProps, GameObject associatedSceneObject)
		{
			return this.AddBatch(mesh, subMeshIndex, material, layer, castShadows, receiveShadows, invertCulling, bounds, instanceCount, customProps, associatedSceneObject, 9223372036854775808UL, uint.MaxValue);
		}

		public int AddBatch(Mesh mesh, int subMeshIndex, Material material, int layer, ShadowCastingMode castShadows, bool receiveShadows, bool invertCulling, Bounds bounds, int instanceCount, MaterialPropertyBlock customProps, GameObject associatedSceneObject, ulong sceneCullingMask)
		{
			return this.AddBatch(mesh, subMeshIndex, material, layer, castShadows, receiveShadows, invertCulling, bounds, instanceCount, customProps, associatedSceneObject, sceneCullingMask, uint.MaxValue);
		}

		public int AddBatch(Mesh mesh, int subMeshIndex, Material material, int layer, ShadowCastingMode castShadows, bool receiveShadows, bool invertCulling, Bounds bounds, int instanceCount, MaterialPropertyBlock customProps, GameObject associatedSceneObject, ulong sceneCullingMask, uint renderingLayerMask)
		{
			return this.AddBatch_Injected(mesh, subMeshIndex, material, layer, castShadows, receiveShadows, invertCulling, ref bounds, instanceCount, customProps, associatedSceneObject, sceneCullingMask, renderingLayerMask);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetBatchFlags(int batchIndex, ulong flags);

		public void SetBatchPropertyMetadata(int batchIndex, NativeArray<int> cbufferLengths, NativeArray<int> cbufferMetadata)
		{
			this.InternalSetBatchPropertyMetadata(batchIndex, (IntPtr)cbufferLengths.GetUnsafeReadOnlyPtr<int>(), cbufferLengths.Length, (IntPtr)cbufferMetadata.GetUnsafeReadOnlyPtr<int>(), cbufferMetadata.Length);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetBatchPropertyMetadata(int batchIndex, IntPtr cbufferLengths, int cbufferLengthsCount, IntPtr cbufferMetadata, int cbufferMetadataCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetInstancingData(int batchIndex, int instanceCount, MaterialPropertyBlock customProps);

		public unsafe NativeArray<Matrix4x4> GetBatchMatrices(int batchIndex)
		{
			int num = 0;
			void* batchMatrices = this.GetBatchMatrices(batchIndex, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(batchMatrices, num, Allocator.Invalid);
		}

		public unsafe NativeArray<int> GetBatchScalarArrayInt(int batchIndex, string propertyName)
		{
			int num = 0;
			void* batchScalarArray = this.GetBatchScalarArray(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(batchScalarArray, num, Allocator.Invalid);
		}

		public unsafe NativeArray<float> GetBatchScalarArray(int batchIndex, string propertyName)
		{
			int num = 0;
			void* batchScalarArray = this.GetBatchScalarArray(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(batchScalarArray, num, Allocator.Invalid);
		}

		public unsafe NativeArray<int> GetBatchVectorArrayInt(int batchIndex, string propertyName)
		{
			int num = 0;
			void* batchVectorArray = this.GetBatchVectorArray(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(batchVectorArray, num, Allocator.Invalid);
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

		public unsafe NativeArray<int> GetBatchScalarArrayInt(int batchIndex, int propertyName)
		{
			int num = 0;
			void* batchScalarArray_Internal = this.GetBatchScalarArray_Internal(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(batchScalarArray_Internal, num, Allocator.Invalid);
		}

		public unsafe NativeArray<float> GetBatchScalarArray(int batchIndex, int propertyName)
		{
			int num = 0;
			void* batchScalarArray_Internal = this.GetBatchScalarArray_Internal(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(batchScalarArray_Internal, num, Allocator.Invalid);
		}

		public unsafe NativeArray<int> GetBatchVectorArrayInt(int batchIndex, int propertyName)
		{
			int num = 0;
			void* batchVectorArray_Internal = this.GetBatchVectorArray_Internal(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>(batchVectorArray_Internal, num, Allocator.Invalid);
		}

		public unsafe NativeArray<Vector4> GetBatchVectorArray(int batchIndex, int propertyName)
		{
			int num = 0;
			void* batchVectorArray_Internal = this.GetBatchVectorArray_Internal(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector4>(batchVectorArray_Internal, num, Allocator.Invalid);
		}

		public unsafe NativeArray<Matrix4x4> GetBatchMatrixArray(int batchIndex, int propertyName)
		{
			int num = 0;
			void* batchMatrixArray_Internal = this.GetBatchMatrixArray_Internal(batchIndex, propertyName, out num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Matrix4x4>(batchMatrixArray_Internal, num, Allocator.Invalid);
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
		private unsafe extern void* GetBatchScalarArray_Internal(int batchIndex, int propertyName, out int elementCount);

		[NativeName("GetBatchVectorArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchVectorArray_Internal(int batchIndex, int propertyName, out int elementCount);

		[NativeName("GetBatchMatrixArray")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* GetBatchMatrixArray_Internal(int batchIndex, int propertyName, out int elementCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EnableVisibleIndicesYArray(bool enabled);

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
			NativeArray<int> nativeArray4 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<int>((void*)context.visibleIndicesY, context.visibleIndicesCount, Allocator.Invalid);
			try
			{
				context.cullingJobsFence = group.m_PerformCulling(group, new BatchCullingContext(nativeArray, nativeArray2, nativeArray3, nativeArray4, lodParameters, context.cullingMatrix, context.nearPlane));
			}
			finally
			{
				JobHandle.ScheduleBatchedJobs();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddBatch_Injected(Mesh mesh, int subMeshIndex, Material material, int layer, ShadowCastingMode castShadows, bool receiveShadows, bool invertCulling, ref Bounds bounds, int instanceCount, MaterialPropertyBlock customProps, GameObject associatedSceneObject, ulong sceneCullingMask, uint renderingLayerMask);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetBatchBounds_Injected(int batchIndex, ref Bounds bounds);

		private IntPtr m_GroupHandle = IntPtr.Zero;

		private BatchRendererGroup.OnPerformCulling m_PerformCulling;

		public delegate JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext);
	}
}
