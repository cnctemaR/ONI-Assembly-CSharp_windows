using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering
{
	[UsedByNativeCode]
	[NativeHeader("Runtime/Export/Graphics/GraphicsTexture.bindings.h")]
	[NativeType("Runtime/Graphics/Texture/GraphicsTexture.h")]
	public class GraphicsTexture : IDisposable
	{
		private GraphicsTexture(IntPtr ptr)
		{
			this.m_Ptr = ptr;
		}

		~GraphicsTexture()
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
				this.ReleaseBuffer();
			}
			else
			{
				this.ReleaseBufferOnMain();
			}
			this.m_Ptr = IntPtr.Zero;
		}

		public GraphicsTexture(GraphicsTextureDescriptor desc)
		{
			this.m_Ptr = GraphicsTexture.InitBuffer(desc);
		}

		internal void UploadData(IntPtr data, int size)
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (flag)
			{
				throw new ObjectDisposedException("GraphicsTexture");
			}
			bool flag2 = data == IntPtr.Zero || size == 0;
			if (flag2)
			{
				Debug.LogError("No texture data provided to GraphicsTexture.UploadData");
			}
			else
			{
				this.UploadBuffer(data, (ulong)((long)size));
			}
		}

		internal void UploadData(byte[] data)
		{
			bool flag = this.m_Ptr == IntPtr.Zero;
			if (flag)
			{
				throw new ObjectDisposedException("GraphicsTexture");
			}
			bool flag2 = data == null || data.Length == 0;
			if (flag2)
			{
				Debug.LogError("No texture data provided to GraphicsTexture.UploadData");
			}
			else
			{
				this.UploadBuffer_Array(data);
			}
		}

		public GraphicsTextureDescriptor descriptor
		{
			[FreeFunction("GraphicsTexture_Bindings::GetDescriptor", HasExplicitThis = true, IsThreadSafe = true, ThrowsException = true)]
			get
			{
				IntPtr intPtr = GraphicsTexture.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GraphicsTextureDescriptor graphicsTextureDescriptor;
				GraphicsTexture.get_descriptor_Injected(intPtr, out graphicsTextureDescriptor);
				return graphicsTextureDescriptor;
			}
		}

		public GraphicsTextureState state
		{
			[FreeFunction("GraphicsTexture_Bindings::GetState", HasExplicitThis = true, IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = GraphicsTexture.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GraphicsTexture.get_state_Injected(intPtr);
			}
		}

		[FreeFunction("GraphicsTexture_Bindings::GetActive")]
		private static GraphicsTexture GetActive()
		{
			IntPtr active_Injected = GraphicsTexture.GetActive_Injected();
			return (active_Injected == 0) ? null : GraphicsTexture.BindingsMarshaller.ConvertToManaged(active_Injected);
		}

		[FreeFunction("RenderTextureScripting::SetActive")]
		private static void SetActive(GraphicsTexture target)
		{
			GraphicsTexture.SetActive_Injected((target == null) ? ((IntPtr)0) : GraphicsTexture.BindingsMarshaller.ConvertToNative(target));
		}

		public static GraphicsTexture active
		{
			get
			{
				return GraphicsTexture.GetActive();
			}
			set
			{
				GraphicsTexture.SetActive(value);
			}
		}

		[FreeFunction("GraphicsTexture_Bindings::InitBuffer", ThrowsException = true)]
		private static IntPtr InitBuffer(GraphicsTextureDescriptor desc)
		{
			return GraphicsTexture.InitBuffer_Injected(ref desc);
		}

		[FreeFunction("GraphicsTexture_Bindings::ReleaseBuffer", HasExplicitThis = true, IsThreadSafe = true)]
		private void ReleaseBuffer()
		{
			IntPtr intPtr = GraphicsTexture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsTexture.ReleaseBuffer_Injected(intPtr);
		}

		[FreeFunction("GraphicsTexture_Bindings::ReleaseBufferOnMain", HasExplicitThis = true, IsThreadSafe = true)]
		private void ReleaseBufferOnMain()
		{
			IntPtr intPtr = GraphicsTexture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GraphicsTexture.ReleaseBufferOnMain_Injected(intPtr);
		}

		[FreeFunction("GraphicsTexture_Bindings::UploadBuffer", HasExplicitThis = true, ThrowsException = true)]
		private bool UploadBuffer(IntPtr data, ulong size)
		{
			IntPtr intPtr = GraphicsTexture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GraphicsTexture.UploadBuffer_Injected(intPtr, data, size);
		}

		[FreeFunction("GraphicsTexture_Bindings::UploadBuffer", HasExplicitThis = true, ThrowsException = true)]
		private unsafe bool UploadBuffer_Array(byte[] data)
		{
			IntPtr intPtr = GraphicsTexture.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<byte> span = new Span<byte>(data);
			bool flag;
			fixed (byte* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				flag = GraphicsTexture.UploadBuffer_Array_Injected(intPtr, ref managedSpanWrapper);
			}
			return flag;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_descriptor_Injected(IntPtr _unity_self, out GraphicsTextureDescriptor ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsTextureState get_state_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetActive_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetActive_Injected(IntPtr target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InitBuffer_Injected([In] ref GraphicsTextureDescriptor desc);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseBuffer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseBufferOnMain_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UploadBuffer_Injected(IntPtr _unity_self, IntPtr data, ulong size);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool UploadBuffer_Array_Injected(IntPtr _unity_self, ref ManagedSpanWrapper data);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(GraphicsTexture graphicsTexture)
			{
				return graphicsTexture.m_Ptr;
			}

			public static GraphicsTexture ConvertToManaged(IntPtr ptr)
			{
				return new GraphicsTexture(ptr);
			}
		}
	}
}
