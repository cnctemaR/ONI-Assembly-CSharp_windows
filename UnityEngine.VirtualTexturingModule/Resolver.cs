using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.Rendering.VirtualTexturing
{
	[NativeHeader("Modules/VirtualTexturing/Public/VirtualTextureResolver.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class Resolver : IDisposable
	{
		public Resolver()
		{
			bool flag = !UnityEngine.Rendering.VirtualTexturing.System.enabled;
			if (flag)
			{
				throw new InvalidOperationException("Virtual texturing is not enabled in the player settings.");
			}
			this.m_Ptr = Resolver.InitNative();
		}

		~Resolver()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			bool flag = this.m_Ptr != IntPtr.Zero;
			if (flag)
			{
				this.Flush_Internal();
				Resolver.ReleaseNative(this.m_Ptr);
				this.m_Ptr = IntPtr.Zero;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr InitNative();

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReleaseNative(IntPtr ptr);

		private void Flush_Internal()
		{
			IntPtr intPtr = Resolver.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Resolver.Flush_Internal_Injected(intPtr);
		}

		private void Init_Internal(int width, int height)
		{
			IntPtr intPtr = Resolver.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Resolver.Init_Internal_Injected(intPtr, width, height);
		}

		public int CurrentWidth { get; private set; } = 0;

		public int CurrentHeight { get; private set; } = 0;

		public void UpdateSize(int width, int height)
		{
			bool flag = this.CurrentWidth != width || this.CurrentHeight != height;
			if (flag)
			{
				bool flag2 = width <= 0 || height <= 0;
				if (flag2)
				{
					throw new ArgumentException(string.Format("Zero sized dimensions are invalid (width: {0}, height: {1}.", width, height));
				}
				this.CurrentWidth = width;
				this.CurrentHeight = height;
				this.Flush_Internal();
				this.Init_Internal(this.CurrentWidth, this.CurrentHeight);
			}
		}

		public void Process(CommandBuffer cmd, RenderTargetIdentifier rt)
		{
			this.Process(cmd, rt, 0, this.CurrentWidth, 0, this.CurrentHeight, 0, 0);
		}

		public void Process(CommandBuffer cmd, RenderTargetIdentifier rt, int x, int width, int y, int height, int mip, int slice)
		{
			bool flag = cmd == null;
			if (flag)
			{
				throw new ArgumentNullException("cmd");
			}
			cmd.ProcessVTFeedback(rt, this.m_Ptr, slice, x, width, y, height, mip);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Flush_Internal_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Init_Internal_Injected(IntPtr _unity_self, int width, int height);

		internal IntPtr m_Ptr;

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(Resolver resolver)
			{
				return resolver.m_Ptr;
			}
		}
	}
}
