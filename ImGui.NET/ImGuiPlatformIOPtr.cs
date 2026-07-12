using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImGuiPlatformIOPtr
	{
		public unsafe readonly ImGuiPlatformIO* NativePtr { get; }

		public unsafe ImGuiPlatformIOPtr(ImGuiPlatformIO* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiPlatformIOPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiPlatformIO*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiPlatformIOPtr(ImGuiPlatformIO* nativePtr)
		{
			return new ImGuiPlatformIOPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiPlatformIO*(ImGuiPlatformIOPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiPlatformIOPtr(IntPtr nativePtr)
		{
			return new ImGuiPlatformIOPtr(nativePtr);
		}

		public unsafe ref IntPtr Platform_CreateWindow
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_CreateWindow));
			}
		}

		public unsafe ref IntPtr Platform_DestroyWindow
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_DestroyWindow));
			}
		}

		public unsafe ref IntPtr Platform_ShowWindow
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_ShowWindow));
			}
		}

		public unsafe ref IntPtr Platform_SetWindowPos
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_SetWindowPos));
			}
		}

		public unsafe ref IntPtr Platform_GetWindowPos
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_GetWindowPos));
			}
		}

		public unsafe ref IntPtr Platform_SetWindowSize
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_SetWindowSize));
			}
		}

		public unsafe ref IntPtr Platform_GetWindowSize
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_GetWindowSize));
			}
		}

		public unsafe ref IntPtr Platform_SetWindowFocus
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_SetWindowFocus));
			}
		}

		public unsafe ref IntPtr Platform_GetWindowFocus
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_GetWindowFocus));
			}
		}

		public unsafe ref IntPtr Platform_GetWindowMinimized
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_GetWindowMinimized));
			}
		}

		public unsafe ref IntPtr Platform_SetWindowTitle
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_SetWindowTitle));
			}
		}

		public unsafe ref IntPtr Platform_SetWindowAlpha
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_SetWindowAlpha));
			}
		}

		public unsafe ref IntPtr Platform_UpdateWindow
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_UpdateWindow));
			}
		}

		public unsafe ref IntPtr Platform_RenderWindow
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_RenderWindow));
			}
		}

		public unsafe ref IntPtr Platform_SwapBuffers
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_SwapBuffers));
			}
		}

		public unsafe ref IntPtr Platform_GetWindowDpiScale
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_GetWindowDpiScale));
			}
		}

		public unsafe ref IntPtr Platform_OnChangedViewport
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_OnChangedViewport));
			}
		}

		public unsafe ref IntPtr Platform_SetImeInputPos
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_SetImeInputPos));
			}
		}

		public unsafe ref IntPtr Platform_CreateVkSurface
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Platform_CreateVkSurface));
			}
		}

		public unsafe ref IntPtr Renderer_CreateWindow
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Renderer_CreateWindow));
			}
		}

		public unsafe ref IntPtr Renderer_DestroyWindow
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Renderer_DestroyWindow));
			}
		}

		public unsafe ref IntPtr Renderer_SetWindowSize
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Renderer_SetWindowSize));
			}
		}

		public unsafe ref IntPtr Renderer_RenderWindow
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Renderer_RenderWindow));
			}
		}

		public unsafe ref IntPtr Renderer_SwapBuffers
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->Renderer_SwapBuffers));
			}
		}

		public unsafe ImPtrVector<ImGuiPlatformMonitorPtr> Monitors
		{
			get
			{
				return new ImPtrVector<ImGuiPlatformMonitorPtr>(this.NativePtr->Monitors, Unsafe.SizeOf<ImGuiPlatformMonitor>());
			}
		}

		public unsafe ImVector<ImGuiViewportPtr> Viewports
		{
			get
			{
				return new ImVector<ImGuiViewportPtr>(this.NativePtr->Viewports);
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiPlatformIO_destroy(this.NativePtr);
		}
	}
}
