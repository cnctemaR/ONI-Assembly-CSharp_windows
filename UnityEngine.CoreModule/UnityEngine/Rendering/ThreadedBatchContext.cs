using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Camera/BatchRendererGroup.h")]
	public struct ThreadedBatchContext
	{
		[FreeFunction("BatchRendererGroup::AddDrawCommandBatch_Threaded", IsThreadSafe = true)]
		private static BatchID AddDrawCommandBatch(IntPtr brg, IntPtr values, int count, GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize)
		{
			BatchID batchID;
			ThreadedBatchContext.AddDrawCommandBatch_Injected(brg, values, count, ref buffer, bufferOffset, windowSize, out batchID);
			return batchID;
		}

		[FreeFunction("BatchRendererGroup::SetDrawCommandBatchBuffer_Threaded", IsThreadSafe = true)]
		private static void SetDrawCommandBatchBuffer(IntPtr brg, BatchID batchID, GraphicsBufferHandle buffer)
		{
			ThreadedBatchContext.SetDrawCommandBatchBuffer_Injected(brg, ref batchID, ref buffer);
		}

		[FreeFunction("BatchRendererGroup::RemoveDrawCommandBatch_Threaded", IsThreadSafe = true)]
		private static void RemoveDrawCommandBatch(IntPtr brg, BatchID batchID)
		{
			ThreadedBatchContext.RemoveDrawCommandBatch_Injected(brg, ref batchID);
		}

		public BatchID AddBatch(NativeArray<MetadataValue> batchMetadata, GraphicsBufferHandle buffer)
		{
			return ThreadedBatchContext.AddDrawCommandBatch(this.batchRendererGroup, (IntPtr)batchMetadata.GetUnsafeReadOnlyPtr<MetadataValue>(), batchMetadata.Length, buffer, 0U, 0U);
		}

		public BatchID AddBatch(NativeArray<MetadataValue> batchMetadata, GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize)
		{
			return ThreadedBatchContext.AddDrawCommandBatch(this.batchRendererGroup, (IntPtr)batchMetadata.GetUnsafeReadOnlyPtr<MetadataValue>(), batchMetadata.Length, buffer, bufferOffset, windowSize);
		}

		public void SetBatchBuffer(BatchID batchID, GraphicsBufferHandle buffer)
		{
			ThreadedBatchContext.SetDrawCommandBatchBuffer(this.batchRendererGroup, batchID, buffer);
		}

		public void RemoveBatch(BatchID batchID)
		{
			ThreadedBatchContext.RemoveDrawCommandBatch(this.batchRendererGroup, batchID);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddDrawCommandBatch_Injected(IntPtr brg, IntPtr values, int count, ref GraphicsBufferHandle buffer, uint bufferOffset, uint windowSize, out BatchID ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetDrawCommandBatchBuffer_Injected(IntPtr brg, ref BatchID batchID, ref GraphicsBufferHandle buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveDrawCommandBatch_Injected(IntPtr brg, ref BatchID batchID);

		public IntPtr batchRendererGroup;
	}
}
