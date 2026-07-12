using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiPlatformMonitorPtr
	{
		public unsafe readonly ImGuiPlatformMonitor* NativePtr { get; }

		public unsafe ImGuiPlatformMonitorPtr(ImGuiPlatformMonitor* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiPlatformMonitorPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiPlatformMonitor*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiPlatformMonitorPtr(ImGuiPlatformMonitor* nativePtr)
		{
			return new ImGuiPlatformMonitorPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiPlatformMonitor*(ImGuiPlatformMonitorPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiPlatformMonitorPtr(IntPtr nativePtr)
		{
			return new ImGuiPlatformMonitorPtr(nativePtr);
		}

		public unsafe ref Vector2 MainPos
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->MainPos));
			}
		}

		public unsafe ref Vector2 MainSize
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->MainSize));
			}
		}

		public unsafe ref Vector2 WorkPos
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->WorkPos));
			}
		}

		public unsafe ref Vector2 WorkSize
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->WorkSize));
			}
		}

		public unsafe ref float DpiScale
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->DpiScale));
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiPlatformMonitor_destroy(this.NativePtr);
		}
	}
}
