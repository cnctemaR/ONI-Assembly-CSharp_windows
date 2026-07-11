using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements.UIR
{
	[NativeHeader("Modules/UIElements/UIRendererUtility.h")]
	internal class Utility
	{
		public static void DrawRanges<I, T>(Utility.GPUBuffer<I> ib, Utility.GPUBuffer<T> vb, NativeSlice<DrawBufferRange> ranges) where I : struct where T : struct
		{
			Debug.Assert(ib.ElementStride == 2);
			Utility.DrawRanges(ib.BufferPointer, vb.BufferPointer, vb.ElementStride, new IntPtr(ranges.GetUnsafePtr<DrawBufferRange>()), ranges.Length);
		}

		public static void SetVectorArray<T>(Material mat, int name, NativeSlice<T> vector4s) where T : struct
		{
			int num = vector4s.Length * vector4s.Stride / 16;
			Utility.SetVectorArray(mat, name, new IntPtr(vector4s.GetUnsafePtr<T>()), num);
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> GraphicsResourcesRecreate;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action EngineUpdate;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action FlushPendingResources;

		[RequiredByNativeCode]
		internal static void RaiseGraphicsResourcesRecreate(bool recreate)
		{
			Action<bool> graphicsResourcesRecreate = Utility.GraphicsResourcesRecreate;
			if (graphicsResourcesRecreate != null)
			{
				graphicsResourcesRecreate(recreate);
			}
		}

		[RequiredByNativeCode]
		internal static void RaiseEngineUpdate()
		{
			bool flag = Utility.EngineUpdate != null;
			if (flag)
			{
				Utility.EngineUpdate();
			}
		}

		[RequiredByNativeCode]
		internal static void RaiseFlushPendingResources()
		{
			Action flushPendingResources = Utility.FlushPendingResources;
			if (flushPendingResources != null)
			{
				flushPendingResources();
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr AllocateBuffer(int elementCount, int elementStride, bool vertexBuffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeBuffer(IntPtr buffer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateBufferRanges(IntPtr buffer, IntPtr ranges, int rangeCount, int writeRangeStart, int writeRangeEnd);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DrawRanges(IntPtr ib, IntPtr vb, int vbElemStride, IntPtr ranges, int rangeCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVectorArray(Material mat, int name, IntPtr vector4s, int count);

		public static void SetScissorRect(RectInt scissorRect)
		{
			Utility.SetScissorRect_Injected(ref scissorRect);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableScissor();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsScissorEnabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint InsertCPUFence();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CPUFencePassed(uint fence);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WaitForCPUFencePassed(uint fence);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SyncRenderThread();

		public static RectInt GetActiveViewport()
		{
			RectInt rectInt;
			Utility.GetActiveViewport_Injected(out rectInt);
			return rectInt;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProfileDrawChainBegin();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProfileDrawChainEnd();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProfileImmediateRendererBegin();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProfileImmediateRendererEnd();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void NotifyOfUIREvents(bool subscribe);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool GetInvertProjectionMatrix();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScissorRect_Injected(ref RectInt scissorRect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveViewport_Injected(out RectInt ret);

		private static ProfilerMarker s_MarkerRaiseEngineUpdate = new ProfilerMarker("UIR.RaiseEngineUpdate");

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
