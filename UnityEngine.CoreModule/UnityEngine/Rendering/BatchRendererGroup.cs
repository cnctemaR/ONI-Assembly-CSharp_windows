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
	[NativeHeader("Runtime/Math/Matrix4x4.h")]
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class BatchRendererGroup : IDisposable
	{
		public unsafe BatchRendererGroup(BatchRendererGroup.OnPerformCulling cullingCallback, IntPtr userContext)
		{
			this.m_PerformCulling = cullingCallback;
			this.m_GroupHandle = BatchRendererGroup.Create(this, (void*)userContext);
		}

		public void Dispose()
		{
			BatchRendererGroup.Destroy(this.m_GroupHandle);
			this.m_GroupHandle = IntPtr.Zero;
		}

		public ThreadedBatchContext GetThreadedBatchContext()
		{
			return new ThreadedBatchContext
			{
				batchRendererGroup = this.m_GroupHandle
			};
		}

		private BatchID AddDrawCommandBatch(IntPtr values, int count, GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize)
		{
			BatchID batchID;
			this.AddDrawCommandBatch_Injected(values, count, ref buffer, bufferOffset, windowSize, out batchID);
			return batchID;
		}

		public BatchID AddBatch(NativeArray<MetadataValue> batchMetadata, GraphicsBufferHandle buffer)
		{
			return this.AddDrawCommandBatch((IntPtr)batchMetadata.GetUnsafeReadOnlyPtr<MetadataValue>(), batchMetadata.Length, buffer, 0U, 0U);
		}

		public BatchID AddBatch(NativeArray<MetadataValue> batchMetadata, GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize)
		{
			return this.AddDrawCommandBatch((IntPtr)batchMetadata.GetUnsafeReadOnlyPtr<MetadataValue>(), batchMetadata.Length, buffer, bufferOffset, windowSize);
		}

		private void RemoveDrawCommandBatch(BatchID batchID)
		{
			this.RemoveDrawCommandBatch_Injected(ref batchID);
		}

		public void RemoveBatch(BatchID batchID)
		{
			this.RemoveDrawCommandBatch(batchID);
		}

		private void SetDrawCommandBatchBuffer(BatchID batchID, GraphicsBufferHandle buffer)
		{
			this.SetDrawCommandBatchBuffer_Injected(ref batchID, ref buffer);
		}

		public void SetBatchBuffer(BatchID batchID, GraphicsBufferHandle buffer)
		{
			this.SetDrawCommandBatchBuffer(batchID, buffer);
		}

		public BatchMaterialID RegisterMaterial(Material material)
		{
			BatchMaterialID batchMaterialID;
			this.RegisterMaterial_Injected(material, out batchMaterialID);
			return batchMaterialID;
		}

		public BatchMaterialID RegisterMaterial(int materialInstanceID)
		{
			return this.RegisterMaterial_InstanceID(materialInstanceID);
		}

		private BatchMaterialID RegisterMaterial_InstanceID(int materialInstanceID)
		{
			BatchMaterialID batchMaterialID;
			this.RegisterMaterial_InstanceID_Injected(materialInstanceID, out batchMaterialID);
			return batchMaterialID;
		}

		public void UnregisterMaterial(BatchMaterialID material)
		{
			this.UnregisterMaterial_Injected(ref material);
		}

		public Material GetRegisteredMaterial(BatchMaterialID material)
		{
			return this.GetRegisteredMaterial_Injected(ref material);
		}

		public BatchMeshID RegisterMesh(Mesh mesh)
		{
			BatchMeshID batchMeshID;
			this.RegisterMesh_Injected(mesh, out batchMeshID);
			return batchMeshID;
		}

		public BatchMeshID RegisterMesh(int meshInstanceID)
		{
			return this.RegisterMesh_InstanceID(meshInstanceID);
		}

		private BatchMeshID RegisterMesh_InstanceID(int meshInstanceID)
		{
			BatchMeshID batchMeshID;
			this.RegisterMesh_InstanceID_Injected(meshInstanceID, out batchMeshID);
			return batchMeshID;
		}

		public void UnregisterMesh(BatchMeshID mesh)
		{
			this.UnregisterMesh_Injected(ref mesh);
		}

		public Mesh GetRegisteredMesh(BatchMeshID mesh)
		{
			return this.GetRegisteredMesh_Injected(ref mesh);
		}

		public void SetGlobalBounds(Bounds bounds)
		{
			this.SetGlobalBounds_Injected(ref bounds);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetPickingMaterial(Material material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetErrorMaterial(Material material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLoadingMaterial(Material material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetEnabledViewTypes(BatchCullingViewType[] viewTypes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern BatchBufferTarget GetBufferTarget();

		public static BatchBufferTarget BufferTarget
		{
			get
			{
				return BatchRendererGroup.GetBufferTarget();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetConstantBufferMaxWindowSize();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetConstantBufferOffsetAlignment();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern IntPtr Create(BatchRendererGroup group, void* userContext);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy(IntPtr groupHandle);

		[RequiredByNativeCode]
		private unsafe static void InvokeOnPerformCulling(BatchRendererGroup group, ref BatchRendererCullingOutput context, ref LODParameters lodParameters, IntPtr userContext)
		{
			NativeArray<Plane> nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Plane>((void*)context.cullingPlanes, context.cullingPlaneCount, Allocator.Invalid);
			NativeArray<CullingSplit> nativeArray2 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<CullingSplit>((void*)context.cullingSplits, context.cullingSplitCount, Allocator.Invalid);
			NativeArray<BatchCullingOutputDrawCommands> nativeArray3 = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<BatchCullingOutputDrawCommands>((void*)context.drawCommands, 1, Allocator.Invalid);
			try
			{
				BatchCullingOutput batchCullingOutput = new BatchCullingOutput
				{
					drawCommands = nativeArray3
				};
				context.cullingJobsFence = group.m_PerformCulling(group, new BatchCullingContext(nativeArray, nativeArray2, lodParameters, context.localToWorldMatrix, context.viewType, context.projectionType, context.cullingFlags, context.viewID, context.cullingLayerMask, context.sceneCullingMask, context.receiverPlaneOffset, context.receiverPlaneCount), batchCullingOutput, userContext);
			}
			finally
			{
				JobHandle.ScheduleBatchedJobs();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void AddDrawCommandBatch_Injected(IntPtr values, int count, ref GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize, out BatchID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RemoveDrawCommandBatch_Injected(ref BatchID batchID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetDrawCommandBatchBuffer_Injected(ref BatchID batchID, ref GraphicsBufferHandle buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RegisterMaterial_Injected(Material material, out BatchMaterialID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RegisterMaterial_InstanceID_Injected(int materialInstanceID, out BatchMaterialID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void UnregisterMaterial_Injected(ref BatchMaterialID material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Material GetRegisteredMaterial_Injected(ref BatchMaterialID material);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RegisterMesh_Injected(Mesh mesh, out BatchMeshID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void RegisterMesh_InstanceID_Injected(int meshInstanceID, out BatchMeshID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void UnregisterMesh_Injected(ref BatchMeshID mesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Mesh GetRegisteredMesh_Injected(ref BatchMeshID mesh);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetGlobalBounds_Injected(ref Bounds bounds);

		private IntPtr m_GroupHandle = IntPtr.Zero;

		private BatchRendererGroup.OnPerformCulling m_PerformCulling;

		public delegate JobHandle OnPerformCulling(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext, BatchCullingOutput cullingOutput, IntPtr userContext);
	}
}
