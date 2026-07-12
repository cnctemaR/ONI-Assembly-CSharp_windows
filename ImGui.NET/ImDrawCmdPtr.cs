using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImDrawCmdPtr
	{
		public unsafe readonly ImDrawCmd* NativePtr { get; }

		public unsafe ImDrawCmdPtr(ImDrawCmd* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImDrawCmdPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImDrawCmd*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImDrawCmdPtr(ImDrawCmd* nativePtr)
		{
			return new ImDrawCmdPtr(nativePtr);
		}

		public unsafe static implicit operator ImDrawCmd*(ImDrawCmdPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImDrawCmdPtr(IntPtr nativePtr)
		{
			return new ImDrawCmdPtr(nativePtr);
		}

		public unsafe ref Vector4 ClipRect
		{
			get
			{
				return Unsafe.AsRef<Vector4>((void*)(&this.NativePtr->ClipRect));
			}
		}

		public unsafe ref IntPtr TextureId
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->TextureId));
			}
		}

		public unsafe ref uint VtxOffset
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->VtxOffset));
			}
		}

		public unsafe ref uint IdxOffset
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->IdxOffset));
			}
		}

		public unsafe ref uint ElemCount
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->ElemCount));
			}
		}

		public unsafe ref IntPtr UserCallback
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->UserCallback));
			}
		}

		public unsafe IntPtr UserCallbackData
		{
			get
			{
				return (IntPtr)this.NativePtr->UserCallbackData;
			}
			set
			{
				this.NativePtr->UserCallbackData = (void*)value;
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImDrawCmd_destroy(this.NativePtr);
		}
	}
}
