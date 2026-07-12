using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImDrawVertPtr
	{
		public unsafe readonly ImDrawVert* NativePtr { get; }

		public unsafe ImDrawVertPtr(ImDrawVert* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImDrawVertPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImDrawVert*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImDrawVertPtr(ImDrawVert* nativePtr)
		{
			return new ImDrawVertPtr(nativePtr);
		}

		public unsafe static implicit operator ImDrawVert*(ImDrawVertPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImDrawVertPtr(IntPtr nativePtr)
		{
			return new ImDrawVertPtr(nativePtr);
		}

		public unsafe ref Vector2 pos
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->pos));
			}
		}

		public unsafe ref Vector2 uv
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->uv));
			}
		}

		public unsafe ref uint col
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->col));
			}
		}
	}
}
