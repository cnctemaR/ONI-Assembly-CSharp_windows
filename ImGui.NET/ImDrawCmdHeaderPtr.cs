using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImDrawCmdHeaderPtr
	{
		public unsafe readonly ImDrawCmdHeader* NativePtr { get; }

		public unsafe ImDrawCmdHeaderPtr(ImDrawCmdHeader* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImDrawCmdHeaderPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImDrawCmdHeader*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImDrawCmdHeaderPtr(ImDrawCmdHeader* nativePtr)
		{
			return new ImDrawCmdHeaderPtr(nativePtr);
		}

		public unsafe static implicit operator ImDrawCmdHeader*(ImDrawCmdHeaderPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImDrawCmdHeaderPtr(IntPtr nativePtr)
		{
			return new ImDrawCmdHeaderPtr(nativePtr);
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
	}
}
