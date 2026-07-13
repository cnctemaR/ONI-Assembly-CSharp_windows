using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Graphics/GraphicsBuffer.bindings.h")]
	[UsedByNativeCode]
	[NativeHeader("Runtime/Shaders/GraphicsBuffer.h")]
	public sealed class GraphicsBuffer : IDisposable
	{
		~GraphicsBuffer()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				GraphicsBuffer.DestroyBuffer(this);
			}
			else
			{
				bool flag = this.m_Ptr != IntPtr.Zero;
				if (flag)
				{
					Debug.LogWarning("GarbageCollector disposing of GraphicsBuffer. Please use GraphicsBuffer.Release() or .Dispose() to manually release the buffer.");
				}
			}
			this.m_Ptr = IntPtr.Zero;
		}

		private static bool RequiresCompute(GraphicsBuffer.Target target)
		{
			GraphicsBuffer.Target target2 = GraphicsBuffer.Target.Structured | GraphicsBuffer.Target.Raw | GraphicsBuffer.Target.Append | GraphicsBuffer.Target.Counter | GraphicsBuffer.Target.IndirectArguments;
			return (target & target2) > (GraphicsBuffer.Target)0;
		}

		private static bool IsVertexIndexOrCopyOnly(GraphicsBuffer.Target target)
		{
			GraphicsBuffer.Target target2 = GraphicsBuffer.Target.Vertex | GraphicsBuffer.Target.Index | GraphicsBuffer.Target.CopySource | GraphicsBuffer.Target.CopyDestination;
			return (target & target2) == target;
		}

		[FreeFunction("GraphicsBuffer_Bindings::InitBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InitBuffer(GraphicsBuffer.Target target, GraphicsBuffer.UsageFlags usageFlags, int count, int stride);

		[FreeFunction("GraphicsBuffer_Bindings::DestroyBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyBuffer(GraphicsBuffer buf);

		public GraphicsBuffer(GraphicsBuffer.Target target, int count, int stride)
		{
			GraphicsBuffer.UsageFlags usageFlags = (((target & (GraphicsBuffer.Target.Vertex | GraphicsBuffer.Target.Index)) == target) ? GraphicsBuffer.UsageFlags.LockBufferForWrite : GraphicsBuffer.UsageFlags.None);
			this.InternalInitialization(target, usageFlags, count, stride);
		}

		public GraphicsBuffer(GraphicsBuffer.Target target, GraphicsBuffer.UsageFlags usageFlags, int count, int stride)
		{
			this.InternalInitialization(target, usageFlags, count, stride);
		}

		private void InternalInitialization(GraphicsBuffer.Target target, GraphicsBuffer.UsageFlags usageFlags, int count, int stride)
		{
			bool flag = GraphicsBuffer.RequiresCompute(target) && !SystemInfo.supportsComputeShaders;
			if (flag)
			{
				throw new ArgumentException("Attempting to create a graphics buffer that requires compute shader support, but compute shaders are not supported on this platform. Target: " + target.ToString());
			}
			bool flag2 = count <= 0;
			if (flag2)
			{
				throw new ArgumentException("Attempting to create a zero length graphics buffer", "count");
			}
			bool flag3 = stride <= 0;
			if (flag3)
			{
				throw new ArgumentException("Attempting to create a graphics buffer with a negative or null stride", "stride");
			}
			bool flag4 = (target & GraphicsBuffer.Target.Index) != (GraphicsBuffer.Target)0 && stride != 2 && stride != 4;
			if (flag4)
			{
				throw new ArgumentException("Attempting to create an index buffer with an invalid stride: " + stride.ToString(), "stride");
			}
			bool flag5 = !GraphicsBuffer.IsVertexIndexOrCopyOnly(target) && stride % 4 != 0;
			if (flag5)
			{
				throw new ArgumentException("Stride must be a multiple of 4 unless the buffer is only used as a vertex buffer and/or index buffer ", "stride");
			}
			long num = (long)count * (long)stride;
			long maxGraphicsBufferSize = SystemInfo.maxGraphicsBufferSize;
			bool flag6 = num > maxGraphicsBufferSize;
			if (flag6)
			{
				throw new ArgumentException(string.Format("The total size of the graphics buffer ({0} bytes) exceeds the maximum buffer size. Maximum supported buffer size: {1} bytes.", num, maxGraphicsBufferSize));
			}
			bool flag7 = (usageFlags & GraphicsBuffer.UsageFlags.LockBufferForWrite) != GraphicsBuffer.UsageFlags.None && (target & GraphicsBuffer.Target.CopyDestination) > (GraphicsBuffer.Target)0;
			if (flag7)
			{
				throw new ArgumentException("Attempting to create a LockBufferForWrite capable buffer that can be copied into. LockBufferForWrite buffers are read-only on the GPU.");
			}
			this.m_Ptr = GraphicsBuffer.InitBuffer(target, usageFlags, count, stride);
		}

		public void Release()
		{
			this.Dispose();
		}

		[FreeFunction("GraphicsBuffer_Bindings::IsValidBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidBuffer(GraphicsBuffer buf);

		public bool IsValid()
		{
			return this.m_Ptr != IntPtr.Zero && GraphicsBuffer.IsValidBuffer(this);
		}

		public extern int count
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int stride
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern GraphicsBuffer.Target target
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::GetUsageFlags", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern GraphicsBuffer.UsageFlags GetUsageFlags();

		public GraphicsBuffer.UsageFlags usageFlags
		{
			get
			{
				return this.GetUsageFlags();
			}
		}

		public GraphicsBufferHandle bufferHandle
		{
			get
			{
				GraphicsBufferHandle graphicsBufferHandle;
				this.get_bufferHandle_Injected(out graphicsBufferHandle);
				return graphicsBufferHandle;
			}
		}

		[SecuritySafeCritical]
		public void SetData(Array data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to GraphicsBuffer.SetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			this.InternalSetData(data, 0, 0, data.Length, UnsafeUtility.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetData<T>(List<T> data) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag2)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to GraphicsBuffer.SetData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), 0, 0, NoAllocHelpers.SafeLength<T>(data), Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetData<T>(NativeArray<T> data) where T : struct
		{
			this.InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
		}

		[SecuritySafeCritical]
		public void SetData(Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to GraphicsBuffer.SetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			bool flag3 = managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", managedBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetData(data, managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetData<T>(List<T> data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag2)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to GraphicsBuffer.SetData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			bool flag3 = managedBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", managedBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetData<T>(NativeArray<T> data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count) where T : struct
		{
			bool flag = nativeBufferStartIndex < 0 || graphicsBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (nativeBufferStartIndex:{0} graphicsBufferStartIndex:{1} count:{2})", nativeBufferStartIndex, graphicsBufferStartIndex, count));
			}
			this.InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), nativeBufferStartIndex, graphicsBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetNativeData", HasExplicitThis = true, ThrowsException = true)]
		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

		[SecurityCritical]
		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetData(Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

		[SecurityCritical]
		public void GetData(Array data)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to GraphicsBuffer.GetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			this.InternalGetData(data, 0, 0, data.Length, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecurityCritical]
		public void GetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to GraphicsBuffer.GetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			bool flag3 = managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count argument (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalGetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecurityCritical]
		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetNativeBufferPtr", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeBufferPtr();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void* BeginBufferWrite(int offset = 0, int size = 0);

		public unsafe NativeArray<T> LockBufferForWrite<T>(int bufferStartIndex, int count) where T : struct
		{
			bool flag = !this.IsValid();
			if (flag)
			{
				throw new InvalidOperationException("LockBufferForWrite requires a valid GraphicsBuffer");
			}
			bool flag2 = (this.usageFlags & GraphicsBuffer.UsageFlags.LockBufferForWrite) == GraphicsBuffer.UsageFlags.None;
			if (flag2)
			{
				throw new InvalidOperationException("GraphicsBuffer must be created with usage mode UsageFlage.LockBufferForWrite to use LockBufferForWrite");
			}
			int num = UnsafeUtility.SizeOf<T>();
			bool flag3 = bufferStartIndex < 0 || count < 0 || (bufferStartIndex + count) * num > this.count * this.stride;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (bufferStartIndex:{0} count:{1} elementSize:{2}, this.count:{3}, this.stride{4})", new object[] { bufferStartIndex, count, num, this.count, this.stride }));
			}
			void* ptr = this.BeginBufferWrite(bufferStartIndex * num, count * num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, count, Allocator.Invalid);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void EndBufferWrite(int bytesWritten = 0);

		public void UnlockBufferAfterWrite<T>(int countWritten) where T : struct
		{
			bool flag = countWritten < 0;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (countWritten:{0})", countWritten));
			}
			int num = UnsafeUtility.SizeOf<T>();
			this.EndBufferWrite(countWritten * num);
		}

		public string name
		{
			set
			{
				this.SetName(value);
			}
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::SetName", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetName(string name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCounterValue(uint counterValue);

		[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCountCC(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes);

		[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCountGC(GraphicsBuffer src, ComputeBuffer dst, int dstOffsetBytes);

		[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCountCG(ComputeBuffer src, GraphicsBuffer dst, int dstOffsetBytes);

		[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCountGG(GraphicsBuffer src, GraphicsBuffer dst, int dstOffsetBytes);

		public static void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes)
		{
			GraphicsBuffer.CopyCountCC(src, dst, dstOffsetBytes);
		}

		public static void CopyCount(GraphicsBuffer src, ComputeBuffer dst, int dstOffsetBytes)
		{
			GraphicsBuffer.CopyCountGC(src, dst, dstOffsetBytes);
		}

		public static void CopyCount(ComputeBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
		{
			GraphicsBuffer.CopyCountCG(src, dst, dstOffsetBytes);
		}

		public static void CopyCount(GraphicsBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
		{
			GraphicsBuffer.CopyCountGG(src, dst, dstOffsetBytes);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_bufferHandle_Injected(out GraphicsBufferHandle ret);

		internal IntPtr m_Ptr;

		[Flags]
		public enum Target
		{
			Vertex = 1,
			Index = 2,
			CopySource = 4,
			CopyDestination = 8,
			Structured = 16,
			Raw = 32,
			Append = 64,
			Counter = 128,
			IndirectArguments = 256,
			Constant = 512
		}

		[Flags]
		public enum UsageFlags
		{
			None = 0,
			LockBufferForWrite = 1
		}

		public struct IndirectDrawArgs
		{
			public uint vertexCountPerInstance { readonly get; set; }

			public uint instanceCount { readonly get; set; }

			public uint startVertex { readonly get; set; }

			public uint startInstance { readonly get; set; }

			public const int size = 16;
		}

		public struct IndirectDrawIndexedArgs
		{
			public uint indexCountPerInstance { readonly get; set; }

			public uint instanceCount { readonly get; set; }

			public uint startIndex { readonly get; set; }

			public uint baseVertexIndex { readonly get; set; }

			public uint startInstance { readonly get; set; }

			public const int size = 20;
		}
	}
}
