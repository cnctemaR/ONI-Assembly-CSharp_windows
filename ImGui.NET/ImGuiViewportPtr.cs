using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImGuiViewportPtr
	{
		public unsafe readonly ImGuiViewport* NativePtr { get; }

		public unsafe ImGuiViewportPtr(ImGuiViewport* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImGuiViewportPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImGuiViewport*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImGuiViewportPtr(ImGuiViewport* nativePtr)
		{
			return new ImGuiViewportPtr(nativePtr);
		}

		public unsafe static implicit operator ImGuiViewport*(ImGuiViewportPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImGuiViewportPtr(IntPtr nativePtr)
		{
			return new ImGuiViewportPtr(nativePtr);
		}

		public unsafe ref uint ID
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->ID));
			}
		}

		public unsafe ref ImGuiViewportFlags Flags
		{
			get
			{
				return Unsafe.AsRef<ImGuiViewportFlags>((void*)(&this.NativePtr->Flags));
			}
		}

		public unsafe ref Vector2 Pos
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->Pos));
			}
		}

		public unsafe ref Vector2 Size
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->Size));
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

		public unsafe ref uint ParentViewportId
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->ParentViewportId));
			}
		}

		public unsafe ImDrawDataPtr DrawData
		{
			get
			{
				return new ImDrawDataPtr(this.NativePtr->DrawData);
			}
		}

		public unsafe IntPtr RendererUserData
		{
			get
			{
				return (IntPtr)this.NativePtr->RendererUserData;
			}
			set
			{
				this.NativePtr->RendererUserData = (void*)value;
			}
		}

		public unsafe IntPtr PlatformUserData
		{
			get
			{
				return (IntPtr)this.NativePtr->PlatformUserData;
			}
			set
			{
				this.NativePtr->PlatformUserData = (void*)value;
			}
		}

		public unsafe IntPtr PlatformHandle
		{
			get
			{
				return (IntPtr)this.NativePtr->PlatformHandle;
			}
			set
			{
				this.NativePtr->PlatformHandle = (void*)value;
			}
		}

		public unsafe IntPtr PlatformHandleRaw
		{
			get
			{
				return (IntPtr)this.NativePtr->PlatformHandleRaw;
			}
			set
			{
				this.NativePtr->PlatformHandleRaw = (void*)value;
			}
		}

		public unsafe ref bool PlatformRequestMove
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->PlatformRequestMove));
			}
		}

		public unsafe ref bool PlatformRequestResize
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->PlatformRequestResize));
			}
		}

		public unsafe ref bool PlatformRequestClose
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->PlatformRequestClose));
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImGuiViewport_destroy(this.NativePtr);
		}

		public unsafe Vector2 GetCenter()
		{
			Vector2 vector;
			ImGuiNative.ImGuiViewport_GetCenter(&vector, this.NativePtr);
			return vector;
		}

		public unsafe Vector2 GetWorkCenter()
		{
			Vector2 vector;
			ImGuiNative.ImGuiViewport_GetWorkCenter(&vector, this.NativePtr);
			return vector;
		}
	}
}
