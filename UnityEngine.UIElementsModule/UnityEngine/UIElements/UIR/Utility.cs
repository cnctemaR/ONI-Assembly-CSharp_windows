using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Profiling;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements.UIR
{
	[NativeHeader("Modules/UIElements/Core/Native/Renderer/UIRendererUtility.h")]
	[VisibleToOtherModules(new string[] { "Unity.UIElements" })]
	internal class Utility
	{
		public static void SetVectorArray(IntPtr shaderPropertySheet, int nameID, Vector4[] vector4s)
		{
			Utility.SetVectorArray(shaderPropertySheet, nameID, vector4s, vector4s.Length);
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

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr AllocateBuffer(int elementCount, int elementStride, bool vertexBuffer);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeBuffer(IntPtr buffer);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UpdateBufferRanges(IntPtr buffer, IntPtr ranges, int rangeCount, int writeRangeStart, int writeRangeEnd);

		[ThreadSafe]
		private unsafe static void SetVectorArray(IntPtr shaderPropertySheet, int name, Vector4[] values, int count)
		{
			Span<Vector4> span = new Span<Vector4>(values);
			fixed (Vector4* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				Utility.SetVectorArray_Injected(shaderPropertySheet, name, ref managedSpanWrapper, count);
			}
		}

		[ThreadSafe]
		public unsafe static IntPtr GetVertexDeclaration(VertexAttributeDescriptor[] vertexAttributes)
		{
			Span<VertexAttributeDescriptor> span = new Span<VertexAttributeDescriptor>(vertexAttributes);
			IntPtr vertexDeclaration_Injected;
			fixed (VertexAttributeDescriptor* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				vertexDeclaration_Injected = Utility.GetVertexDeclaration_Injected(ref managedSpanWrapper);
			}
			return vertexDeclaration_Injected;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void DrawRanges(IntPtr ib, IntPtr* vertexStreams, int streamCount, IntPtr ranges, int rangeCount, IntPtr vertexDecl);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr AllocateShaderPropertySheet();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetAllTextures(IntPtr shaderPropertySheet, IntPtr textureNames, IntPtr texturePtrs, int count);

		[ThreadSafe]
		public static void SetPropertyBlock(MaterialPropertyBlock props)
		{
			Utility.SetPropertyBlock_Injected((props == null) ? ((IntPtr)0) : MaterialPropertyBlock.BindingsMarshaller.ConvertToNative(props));
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ApplyShaderPropertySheet(IntPtr shaderPropertySheet);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ReleasePropertySheet(IntPtr shaderPropertySheet);

		[ThreadSafe]
		public static void SetScissorRect(RectInt scissorRect)
		{
			Utility.SetScissorRect_Injected(ref scissorRect);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DisableScissor();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsScissorEnabled();

		[ThreadSafe]
		public static IntPtr CreateStencilState(StencilState stencilState)
		{
			return Utility.CreateStencilState_Injected(ref stencilState);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetStencilState(IntPtr stencilState, int stencilRef);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasMappedBufferRange();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint InsertCPUFence();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool CPUFencePassed(uint fence);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void WaitForCPUFencePassed(uint fence);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SyncRenderThread();

		[ThreadSafe]
		public static RectInt GetActiveViewport()
		{
			RectInt rectInt;
			Utility.GetActiveViewport_Injected(out rectInt);
			return rectInt;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProfileDrawChainBegin();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void ProfileDrawChainEnd();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void NotifyOfUIREvents(bool subscribe);

		[ThreadSafe]
		public static Matrix4x4 GetUnityProjectionMatrix()
		{
			Matrix4x4 matrix4x;
			Utility.GetUnityProjectionMatrix_Injected(out matrix4x);
			return matrix4x;
		}

		[ThreadSafe]
		public static Matrix4x4 GetDeviceProjectionMatrix()
		{
			Matrix4x4 matrix4x;
			Utility.GetDeviceProjectionMatrix_Injected(out matrix4x);
			return matrix4x;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool DebugIsMainThread();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetVectorArray_Injected(IntPtr shaderPropertySheet, int name, ref ManagedSpanWrapper values, int count);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetVertexDeclaration_Injected(ref ManagedSpanWrapper vertexAttributes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetPropertyBlock_Injected(IntPtr props);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetScissorRect_Injected([In] ref RectInt scissorRect);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateStencilState_Injected([In] ref StencilState stencilState);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetActiveViewport_Injected(out RectInt ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetUnityProjectionMatrix_Injected(out Matrix4x4 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDeviceProjectionMatrix_Injected(out Matrix4x4 ret);

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
