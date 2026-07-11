using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>GPU data buffer, mostly for use with compute shaders.</para>
	/// </summary>
	[UsedByNativeCode]
	public sealed class ComputeBuffer : IDisposable
	{
		/// <summary>
		///   <para>Create a Compute Buffer.</para>
		/// </summary>
		/// <param name="count">Number of elements in the buffer.</param>
		/// <param name="stride">Size of one element in the buffer. Has to match size of buffer type in the shader. See for cross-platform compatibility information.</param>
		/// <param name="type">Type of the buffer, default is ComputeBufferType.Default (structured buffer).</param>
		public ComputeBuffer(int count, int stride)
			: this(count, stride, ComputeBufferType.Default, 3)
		{
		}

		/// <summary>
		///   <para>Create a Compute Buffer.</para>
		/// </summary>
		/// <param name="count">Number of elements in the buffer.</param>
		/// <param name="stride">Size of one element in the buffer. Has to match size of buffer type in the shader. See for cross-platform compatibility information.</param>
		/// <param name="type">Type of the buffer, default is ComputeBufferType.Default (structured buffer).</param>
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
			this.m_Ptr = IntPtr.Zero;
			ComputeBuffer.InitBuffer(this, count, stride, type);
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

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitBuffer(ComputeBuffer buf, int count, int stride, ComputeBufferType type);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyBuffer(ComputeBuffer buf);

		/// <summary>
		///   <para>Release a Compute Buffer.</para>
		/// </summary>
		public void Release()
		{
			this.Dispose();
		}

		/// <summary>
		///   <para>Returns true if this compute buffer is valid and false otherwise.</para>
		/// </summary>
		public bool IsValid()
		{
			return this.m_Ptr != IntPtr.Zero;
		}

		/// <summary>
		///   <para>Number of elements in the buffer (Read Only).</para>
		/// </summary>
		public extern int count
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Size of one element in the buffer (Read Only).</para>
		/// </summary>
		public extern int stride
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Set the buffer with values from an array.</para>
		/// </summary>
		/// <param name="data">Array of values to fill the buffer.</param>
		[SecuritySafeCritical]
		public void SetData(Array data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsBlittable(data.GetType().GetElementType()))
			{
				throw new ArgumentException(string.Format("{0} type used in ComputeBuffer.SetData(array) must be blittable", data.GetType().GetElementType()));
			}
			this.InternalSetData(data, 0, 0, data.Length, UnsafeUtility.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetData<T>(List<T> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsBlittable(typeof(T)))
			{
				throw new ArgumentException(string.Format("{0} type used in ComputeBuffer.SetData(List<>)) must be blittable", typeof(T)));
			}
			this.InternalSetData(NoAllocHelpers.ExtractArrayFromList(data), 0, 0, NoAllocHelpers.SafeLength<T>(data), Marshal.SizeOf(typeof(T)));
		}

		[SecuritySafeCritical]
		public void SetData<T>(NativeArray<T> data) where T : struct
		{
			this.InternalSetNativeData((IntPtr)data.GetUnsafeReadOnlyPtr<T>(), 0, 0, data.Length, UnsafeUtility.SizeOf<T>());
		}

		/// <summary>
		///   <para>Partial copy of data values from an array into the buffer.</para>
		/// </summary>
		/// <param name="data">Array of values to fill the buffer.</param>
		/// <param name="managedBufferStartIndex">The first element index in data to copy to the compute buffer.</param>
		/// <param name="computeBufferStartIndex">The first element index in compute buffer to receive the data.</param>
		/// <param name="count">The number of elements to copy.</param>
		[SecuritySafeCritical]
		public void SetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsBlittable(data.GetType().GetElementType()))
			{
				throw new ArgumentException(string.Format("{0} type used in ComputeBuffer.SetData(array) must be blittable", data.GetType().GetElementType()));
			}
			if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count arguments (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalSetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecuritySafeCritical]
		public void SetData<T>(List<T> data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsBlittable(typeof(T)))
			{
				throw new ArgumentException(string.Format("{0} type used in ComputeBuffer.SetData(List<>)) must be blittable", typeof(T)));
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
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetNativeData(IntPtr data, int nativeBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		[SecurityCritical]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalSetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		/// <summary>
		///   <para>Read data values from the buffer into an array. The array can only use &lt;a href="https:docs.microsoft.comen-usdotnetframeworkinteropblittable-and-non-blittable-types"&gt;blittable&lt;a&gt; types.</para>
		/// </summary>
		/// <param name="data">An array to receive the data.</param>
		[SecurityCritical]
		public void GetData(Array data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsBlittable(data.GetType().GetElementType()))
			{
				throw new ArgumentException(string.Format("{0} type used in ComputeBuffer.GetData(array) must be blittable", data.GetType().GetElementType()));
			}
			this.InternalGetData(data, 0, 0, data.Length, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		/// <summary>
		///   <para>Partial read of data values from the buffer into an array.</para>
		/// </summary>
		/// <param name="data">An array to receive the data.</param>
		/// <param name="managedBufferStartIndex">The first element index in data where retrieved elements are copied.</param>
		/// <param name="computeBufferStartIndex">The first element index of the compute buffer from which elements are read.</param>
		/// <param name="count">The number of elements to retrieve.</param>
		[SecurityCritical]
		public void GetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			if (!UnsafeUtility.IsBlittable(data.GetType().GetElementType()))
			{
				throw new ArgumentException(string.Format("{0} type used in ComputeBuffer.GetData(array) must be blittable", data.GetType().GetElementType()));
			}
			if (managedBufferStartIndex < 0 || computeBufferStartIndex < 0 || count < 0 || managedBufferStartIndex + count > data.Length)
			{
				throw new ArgumentOutOfRangeException(string.Format("Bad indices/count argument (managedBufferStartIndex:{0} computeBufferStartIndex:{1} count:{2})", managedBufferStartIndex, computeBufferStartIndex, count));
			}
			this.InternalGetData(data, managedBufferStartIndex, computeBufferStartIndex, count, Marshal.SizeOf(data.GetType().GetElementType()));
		}

		[SecurityCritical]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void InternalGetData(Array data, int managedBufferStartIndex, int computeBufferStartIndex, int count, int elemSize);

		/// <summary>
		///   <para>Sets counter value of append/consume buffer.</para>
		/// </summary>
		/// <param name="counterValue">Value of the append/consume counter.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCounterValue(uint counterValue);

		/// <summary>
		///   <para>Copy counter value of append/consume buffer into another buffer.</para>
		/// </summary>
		/// <param name="src">Append/consume buffer to copy the counter from.</param>
		/// <param name="dst">A buffer to copy the counter to.</param>
		/// <param name="dstOffsetBytes">Target byte offset in dst.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void CopyCount(ComputeBuffer src, ComputeBuffer dst, int dstOffsetBytes);

		/// <summary>
		///   <para>Retrieve a native (underlying graphics API) pointer to the buffer.</para>
		/// </summary>
		/// <returns>
		///   <para>Pointer to the underlying graphics API buffer.</para>
		/// </returns>
		public IntPtr GetNativeBufferPtr()
		{
			IntPtr intPtr;
			ComputeBuffer.INTERNAL_CALL_GetNativeBufferPtr(this, out intPtr);
			return intPtr;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void INTERNAL_CALL_GetNativeBufferPtr(ComputeBuffer self, out IntPtr value);

		internal IntPtr m_Ptr;
	}
}
