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
	[NativeHeader("Runtime/Shaders/GraphicsBuffer.h")]
	[UsedByNativeCode]
	[NativeClass("GraphicsBuffer")]
	[NativeHeader("Runtime/Export/Graphics/GraphicsBuffer.bindings.h")]
	public sealed class ComputeBuffer : IDisposable
	{
		~ComputeBuffer()
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
				ComputeBuffer.DestroyBuffer(this);
			}
			else
			{
				bool flag = this.m_Ptr != IntPtr.Zero;
				if (flag)
				{
					Debug.LogWarning("GarbageCollector disposing of ComputeBuffer. Please use ComputeBuffer.Release() or .Dispose() to manually release the buffer.");
				}
			}
			this.m_Ptr = IntPtr.Zero;
		}

		[FreeFunction("GraphicsBuffer_Bindings::InitComputeBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InitBuffer(int count, int stride, ComputeBufferType type, ComputeBufferMode usage);

		[FreeFunction("GraphicsBuffer_Bindings::DestroyComputeBuffer")]
		private static void DestroyBuffer(ComputeBuffer buf)
		{
			ComputeBuffer.DestroyBuffer_Injected((buf == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(buf));
		}

		public ComputeBuffer(int count, int stride)
			: this(count, stride, ComputeBufferType.Default, ComputeBufferMode.Immutable, 3)
		{
		}

		public ComputeBuffer(int count, int stride, ComputeBufferType type)
			: this(count, stride, type, ComputeBufferMode.Immutable, 3)
		{
		}

		public ComputeBuffer(int count, int stride, ComputeBufferType type, ComputeBufferMode usage)
			: this(count, stride, type, usage, 3)
		{
		}

		private ComputeBuffer(int count, int stride, ComputeBufferType type, ComputeBufferMode usage, int stackDepth)
		{
			bool flag = count <= 0;
			if (flag)
			{
				throw new ArgumentException("Attempting to create a zero length compute buffer", "count");
			}
			bool flag2 = stride <= 0;
			if (flag2)
			{
				throw new ArgumentException("Attempting to create a compute buffer with a negative or null stride", "stride");
			}
			long num = (long)count * (long)stride;
			long maxGraphicsBufferSize = SystemInfo.maxGraphicsBufferSize;
			bool flag3 = num > maxGraphicsBufferSize;
			if (flag3)
			{
				throw new ArgumentException(string.Format("The total size of the compute buffer ({0} bytes) exceeds the maximum buffer size. Maximum supported buffer size: {1} bytes.", num, maxGraphicsBufferSize));
			}
			this.m_Ptr = ComputeBuffer.InitBuffer(count, stride, type, usage);
		}

		public void Release()
		{
			this.Dispose();
		}

		[FreeFunction("GraphicsBuffer_Bindings::IsValidBuffer")]
		private static bool IsValidBuffer(ComputeBuffer buf)
		{
			return ComputeBuffer.IsValidBuffer_Injected((buf == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(buf));
		}

		public bool IsValid()
		{
			return this.m_Ptr != IntPtr.Zero && ComputeBuffer.IsValidBuffer(this);
		}

		public int count
		{
			get
			{
				IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ComputeBuffer.get_count_Injected(intPtr);
			}
		}

		public int stride
		{
			get
			{
				IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ComputeBuffer.get_stride_Injected(intPtr);
			}
		}

		private ComputeBufferMode usage
		{
			get
			{
				IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return ComputeBuffer.get_usage_Injected(intPtr);
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
				throw new ArgumentException(string.Format("Array passed to ComputeBuffer.SetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
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
				throw new ArgumentException(string.Format("List<{0}> passed to ComputeBuffer.SetData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList<T>(data), 0, 0, NoAllocHelpers.SafeLength<T>(data), Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetData<T>(NativeArray<T> data) where T : struct
		{
			this.InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
		}

		[SecuritySafeCritical]
		public void SetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsArrayBlittable(data);
			if (flag2)
			{
				throw new ArgumentException(string.Format("Array passed to ComputeBuffer.SetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			bool flag3 = managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalSetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetData<T>(List<T> data, int managedBufferStartIndex, int computeBufferStartIndex, int count) where T : struct
		{
			bool flag = data == null;
			if (flag)
			{
				throw new ArgumentNullException("data");
			}
			bool flag2 = !UnsafeUtility.IsGenericListBlittable<T>();
			if (flag2)
			{
				throw new ArgumentException(string.Format("List<{0}> passed to ComputeBuffer.SetData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			bool flag3 = managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList<T>(data), managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetData<T>(NativeArray<T> data, int nativeBufferStartIndex, int computeBufferStartIndex, int count) where T : struct
		{
			bool flag = nativeBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length;
			if (flag)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (nativeBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", nativeBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), nativeBufferStartIndex, computeBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetNativeData", HasExplicitThis = true, ThrowsException = true)]
		private void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int computeBufferStartIndex, int count, int elemSize)
		{
			IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeBuffer.InternalSetNativeData_Injected(intPtr, data, nativeBufferStartIndex, computeBufferStartIndex, count, elemSize);
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalSetData", HasExplicitThis = true, ThrowsException = true)]
		private void InternalSetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize)
		{
			IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeBuffer.InternalSetData_Injected(intPtr, data, managedBufferStartIndex, computeBufferStartIndex, count, elemSize);
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
				throw new ArgumentException(string.Format("Array passed to ComputeBuffer.GetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
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
				throw new ArgumentException(string.Format("Array passed to ComputeBuffer.GetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			bool flag3 = managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count argument (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalGetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[FreeFunction(Name = "GraphicsBuffer_Bindings::InternalGetData", HasExplicitThis = true, ThrowsException = true)]
		private void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize)
		{
			IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeBuffer.InternalGetData_Injected(intPtr, data, managedBufferStartIndex, computeBufferStartIndex, count, elemSize);
		}

		private unsafe void* BeginBufferWrite(int offset = 0, int size = 0)
		{
			IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ComputeBuffer.BeginBufferWrite_Injected(intPtr, offset, size);
		}

		public unsafe NativeArray<T> BeginWrite<T>(int computeBufferStartIndex, int count) where T : struct
		{
			bool flag = !this.IsValid();
			if (flag)
			{
				throw new InvalidOperationException("BeginWrite requires a valid ComputeBuffer");
			}
			bool flag2 = this.usage != ComputeBufferMode.SubUpdates;
			if (flag2)
			{
				throw new ArgumentException("ComputeBuffer must be created with usage mode ComputeBufferMode.SubUpdates to be able to be mapped with BeginWrite");
			}
			int num = UnsafeUtility.SizeOf<T>();
			bool flag3 = computeBufferStartIndex < 0 || count < 0 || (computeBufferStartIndex + count) * num > this.count * this.stride;
			if (flag3)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (computeBufferStartIndex:{0} count:{1} elementSize:{2}, this.count:{3}, this.stride{4})", new object[] { computeBufferStartIndex, count, num, this.count, this.stride }));
			}
			void* ptr = this.BeginBufferWrite(computeBufferStartIndex * num, count * num);
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, count, Allocator.Invalid);
		}

		private void EndBufferWrite(int bytesWritten = 0)
		{
			IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeBuffer.EndBufferWrite_Injected(intPtr, bytesWritten);
		}

		public void EndWrite<T>(int countWritten) where T : struct
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
				IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
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
				ComputeBuffer.SetName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void SetCounterValue(uint counterValue)
		{
			IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ComputeBuffer.SetCounterValue_Injected(intPtr, counterValue);
		}

		public static void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes)
		{
			ComputeBuffer.CopyCount_Injected((src == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(src), (dst == null) ? ((IntPtr)0) : ComputeBuffer.BindingsMarshaller.ConvertToNative(dst), dstOffsetBytes);
		}

		public IntPtr GetNativeBufferPtr()
		{
			IntPtr intPtr = ComputeBuffer.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return ComputeBuffer.GetNativeBufferPtr_Injected(intPtr);
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
		private static extern ComputeBufferMode get_usage_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetNativeData_Injected(IntPtr _unity_self, IntPtr data, int nativeBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalSetData_Injected(IntPtr _unity_self, Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalGetData_Injected(IntPtr _unity_self, Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* BeginBufferWrite_Injected(IntPtr _unity_self, int offset, int size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EndBufferWrite_Injected(IntPtr _unity_self, int bytesWritten);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCounterValue_Injected(IntPtr _unity_self, uint counterValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CopyCount_Injected(IntPtr src, IntPtr dst, int dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetNativeBufferPtr_Injected(IntPtr _unity_self);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(ComputeBuffer computeBuffer)
			{
				return computeBuffer.m_Ptr;
			}
		}
	}
}
