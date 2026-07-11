using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.UIR
{
	[NativeHeader("Modules/UIElements/UIRendererUtility.h")]
	internal class Utility
	{
		public static void DrawRanges<I, T>(Utility.GPUBuffer<I> ib, Utility.GPUBuffer<T> vb, NativeSlice<DrawBufferRange> ranges) where I : struct where T : struct
		{
			Utility.DrawRanges(ib.BufferPointer, vb.BufferPointer, vb.ElementStride, new IntPtr(ranges.GetUnsafePtr<DrawBufferRange>()), ranges.Length);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr AllocateBuffer(int elementCount, int elementStride, bool vertexBuffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeBuffer(IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateBufferRanges(IntPtr buffer, IntPtr ranges, int rangeCount, int writeRangeStart, int writeRangeEnd);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawRanges(IntPtr ib, IntPtr vb, int vbElemStride, IntPtr ranges, int rangeCount);

		public static void SetScissorRect(RectInt scissorRect)
		{
			Utility.SetScissorRect_Injected(ref scissorRect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableScissor();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint InsertCPUFence();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CPUFencePassed(uint fence);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SyncRenderThread();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProfileDrawChainBegin();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProfileDrawChainEnd();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScissorRect_Injected(ref RectInt scissorRect);

		internal enum GPUBufferType
		{
			Vertex,
			Index
		}

		public class GPUBuffer<T> : IDisposable where T : struct
		{
			public GPUBuffer(int elementCount, Utility.GPUBufferType type)
			{
				this.elemCount = elementCount;
				this.elemStride = UnsafeUtility.SizeOf<T>();
				this.buffer = Utility.AllocateBuffer(elementCount, this.elemStride, type == Utility.GPUBufferType.Vertex);
			}

			public void Dispose()
			{
				Utility.FreeBuffer(this.buffer);
			}

			public void UpdateRanges(NativeSlice<GfxUpdateBufferRange> ranges, int rangesMin, int rangesMax)
			{
				Utility.UpdateBufferRanges(this.buffer, new IntPtr(ranges.GetUnsafePtr<GfxUpdateBufferRange>()), ranges.Length, rangesMin, rangesMax);
			}

			public int ElementStride
			{
				get
				{
					return this.elemStride;
				}
			}

			public int Count
			{
				get
				{
					return this.elemCount;
				}
			}

			internal IntPtr BufferPointer
			{
				get
				{
					return this.buffer;
				}
			}

			private IntPtr buffer;

			private int elemCount;

			private int elemStride;
		}
	}
}
