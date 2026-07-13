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
	[UsedByNativeCode]
	[NativeHeader("Runtime/Shaders/GraphicsBuffer.h")]
	[NativeHeader("Runtime/Export/Graphics/GraphicsBuffer.bindings.h")]
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
		private static void DestroyBuffer(GraphicsBuffer buf)
		{
			GraphicsBuffer.DestroyBuffer_Injected((buf == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(buf));
		}

		private GraphicsBuffer(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

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
		private static bool IsValidBuffer(GraphicsBuffer buf)
		{
			return GraphicsBuffer.IsValidBuffer_Injected((buf == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(buf));
		}

		public bool IsValid()
		{
			return this.m_Ptr != IntPtr.Zero && GraphicsBuffer.IsValidBuffer(this);
		}

		public int count
		{
			get
			{
				IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsBuffer.get_count_Injected(intPtr);
			}
		}

		public int stride
		{
			get
			{
				IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsBuffer.get_stride_Injected(intPtr);
			}
		}

		public GraphicsBuffer.Target target
		{
			get
			{
				IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsBuffer.get_target_Injected(intPtr);
			}
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::GetUsageFlags", HasExplicitThis = true)]
		private GraphicsBuffer.UsageFlags GetUsageFlags()
		{
			IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsBuffer.GetUsageFlags_Injected(intPtr);
		}

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
				IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GraphicsBufferHandle graphicsBufferHandle;
				GraphicsBuffer.get_bufferHandle_Injected(intPtr, out graphicsBufferHandle);
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
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList<T>(data), 0, 0, NoAllocHelpers.SafeLength<T>(data), Marshal.SizeOf(typeof(T)));
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
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList<T>(data), managedBufferStartIndex, graphicsBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
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

		[SecurityCritical]
		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetNativeData", HasExplicitThis = true, ThrowsException = true)]
		private void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize)
		{
			IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsBuffer.InternalSetNativeData_Injected(intPtr, data, nativeBufferStartIndex, graphicsBufferStartIndex, count, elemSize);
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetData", HasExplicitThis = true, ThrowsException = true)]
		[SecurityCritical]
		private void InternalSetData(Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize)
		{
			IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsBuffer.InternalSetData_Injected(intPtr, data, managedBufferStartIndex, graphicsBufferStartIndex, count, elemSize);
		}

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
		private void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize)
		{
			IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsBuffer.InternalGetData_Injected(intPtr, data, managedBufferStartIndex, computeBufferStartIndex, count, elemSize);
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetNativeBufferPtr", HasExplicitThis = true)]
		public IntPtr GetNativeBufferPtr()
		{
			IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsBuffer.GetNativeBufferPtr_Injected(intPtr);
		}

		private unsafe void* BeginBufferWrite(int offset = 0, int size = 0)
		{
			IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsBuffer.BeginBufferWrite_Injected(intPtr, offset, size);
		}

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

		private void EndBufferWrite(int bytesWritten = 0)
		{
			IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsBuffer.EndBufferWrite_Injected(intPtr, bytesWritten);
		}

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
		private unsafe void SetName(string name)
		{
			try
			{
				IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				GraphicsBuffer.SetName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void SetCounterValue(uint counterValue)
		{
			IntPtr intPtr = GraphicsBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsBuffer.SetCounterValue_Injected(intPtr, counterValue);
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
		private static void CopyCountCC(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes)
		{
			GraphicsBuffer.CopyCountCC_Injected((src == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
		private static void CopyCountGC(GraphicsBuffer src, ComputeBuffer dst, int dstOffsetBytes)
		{
			GraphicsBuffer.CopyCountGC_Injected((src == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
		private static void CopyCountCG(ComputeBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
		{
			GraphicsBuffer.CopyCountCG_Injected((src == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::CopyCount")]
		private static void CopyCountGG(GraphicsBuffer src, GraphicsBuffer dst, int dstOffsetBytes)
		{
			GraphicsBuffer.CopyCountGG_Injected((src == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : GraphicsBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

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
		private static extern void DestroyBuffer_Injected(IntPtr buf);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsValidBuffer_Injected(IntPtr buf);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_count_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_stride_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsBuffer.Target get_target_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsBuffer.UsageFlags GetUsageFlags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bufferHandle_Injected(IntPtr _unity_self, out GraphicsBufferHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetNativeData_Injected(IntPtr _unity_self, IntPtr data, int nativeBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetData_Injected(IntPtr _unity_self, Array data, int managedBufferStartIndex, int graphicsBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetData_Injected(IntPtr _unity_self, Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNativeBufferPtr_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* BeginBufferWrite_Injected(IntPtr _unity_self, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndBufferWrite_Injected(IntPtr _unity_self, int bytesWritten);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCounterValue_Injected(IntPtr _unity_self, uint counterValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCountCC_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCountGC_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCountCG_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCountGG_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

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

		internal static class BindingsMarshaller
		{
			public static GraphicsBuffer ConvertToManaged(IntPtr ptr)
			{
				return new GraphicsBuffer(ptr);
			}

			public static IntPtr ConvertToNative(GraphicsBuffer graphicsBuffer)
			{
				return graphicsBuffer.m_Ptr;
			}
		}
	}
}
