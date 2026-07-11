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
	[NativeHeader("Runtime/Shaders/ComputeShader.h")]
	[NativeHeader("Runtime/Export/ComputeShader.bindings.h")]
	[UsedByNativeCode]
	public sealed class ComputeBuffer : IDisposable
	{
		public ComputeBuffer(int count, int stride)
			: this(count, stride, ComputeBufferType.Default, 3)
		{
		}

		public ComputeBuffer(int count, int stride, ComputeBufferType type)
			: this(count, stride, type, 3)
		{
		}

		internal ComputeBuffer(int count, int stride, ComputeBufferType type, int stackDepth)
		{
			if (count <= 0)
			{
				throw new ArgumentException("Attempting to create a zero length compute buffer", "count");
			}
			if (stride <= 0)
			{
				throw new ArgumentException("Attempting to create a compute buffer with a negative or null stride", "stride");
			}
			this.m_Ptr = ComputeBuffer.InitBuffer(count, stride, type);
		}

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
			else if (this.m_Ptr != IntPtr.Zero)
			{
				Debug.LogWarning("GarbageCollector disposing of ComputeBuffer. Please use ComputeBuffer.Release() or .Dispose() to manually release the buffer.");
			}
			this.m_Ptr = IntPtr.Zero;
		}

		[FreeFunction("ComputeShader_Bindings::InitBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InitBuffer(int count, int stride, ComputeBufferType type);

		[FreeFunction("ComputeShader_Bindings::DestroyBuffer")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyBuffer(ComputeBuffer buf);

		public void Release()
		{
			this.Dispose();
		}

		public bool IsValid()
		{
			return this.m_Ptr != IntPtr.Zero;
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

		[SecuritySafeCritical]
		public void SetData(Array data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsArrayBlittable(data))
			{
				throw new ArgumentException(string.Format("Array passed to ComputeBuffer.SetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			this.InternalSetData(data, 0, 0, data.Length, UnsafeUtility.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetData<T>(List<T> data) where T : struct
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsGenericListBlittable<T>())
			{
				throw new ArgumentException(string.Format("List<{0}> passed to ComputeBuffer.SetData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), 0, 0, NoAllocHelpers.SafeLength<T>(data), Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetData<T>(NativeArray<T> data) where T : struct
		{
			this.InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
		}

		[SecuritySafeCritical]
		public void SetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsArrayBlittable(data))
			{
				throw new ArgumentException(string.Format("Array passed to ComputeBuffer.SetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalSetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetData<T>(List<T> data, int managedBufferStartIndex, int computeBufferStartIndex, int count) where T : struct
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsGenericListBlittable<T>())
			{
				throw new ArgumentException(string.Format("List<{0}> passed to ComputeBuffer.SetData(List<>) must be blittable.\n{1}", typeof(T), UnsafeUtility.GetReasonForGenericListNonBlittable<T>()));
			}
			if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Count)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetData<T>(NativeArray<T> data, int nativeBufferStartIndex, int computeBufferStartIndex, int count) where T : struct
		{
			if (nativeBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || nativeBufferStartIndex + count > data.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (nativeBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", nativeBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), nativeBufferStartIndex, computeBufferStartIndex, count, UnsafeUtility.SizeOf<T>());
		}

		[SecurityCritical]
		[FreeFunction(Name = "ComputeShader_Bindings::InternalSetNativeData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[SecurityCritical]
		[FreeFunction(Name = "ComputeShader_Bindings::InternalSetData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[SecurityCritical]
		public void GetData(Array data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsArrayBlittable(data))
			{
				throw new ArgumentException(string.Format("Array passed to ComputeBuffer.GetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			this.InternalGetData(data, 0, 0, data.Length, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecurityCritical]
		public void GetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsArrayBlittable(data))
			{
				throw new ArgumentException(string.Format("Array passed to ComputeBuffer.GetData(array) must be blittable.\n{0}", UnsafeUtility.GetReasonForArrayNonBlittable(data)));
			}
			if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count argument (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalGetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecurityCritical]
		[FreeFunction(Name = "ComputeShader_Bindings::InternalGetData", HasExplicitThis = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCounterValue(uint counterValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern IntPtr GetNativeBufferPtr();

		internal IntPtr m_Ptr;
	}
}
