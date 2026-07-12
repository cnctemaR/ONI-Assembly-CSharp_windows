using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImFontAtlasCustomRectPtr
	{
		public unsafe readonly ImFontAtlasCustomRect* NativePtr { get; }

		public unsafe ImFontAtlasCustomRectPtr(ImFontAtlasCustomRect* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImFontAtlasCustomRectPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImFontAtlasCustomRect*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImFontAtlasCustomRectPtr(ImFontAtlasCustomRect* nativePtr)
		{
			return new ImFontAtlasCustomRectPtr(nativePtr);
		}

		public unsafe static implicit operator ImFontAtlasCustomRect*(ImFontAtlasCustomRectPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImFontAtlasCustomRectPtr(IntPtr nativePtr)
		{
			return new ImFontAtlasCustomRectPtr(nativePtr);
		}

		public unsafe ref ushort Width
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->Width));
			}
		}

		public unsafe ref ushort Height
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->Height));
			}
		}

		public unsafe ref ushort X
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->X));
			}
		}

		public unsafe ref ushort Y
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->Y));
			}
		}

		public unsafe ref uint GlyphID
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->GlyphID));
			}
		}

		public unsafe ref float GlyphAdvanceX
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->GlyphAdvanceX));
			}
		}

		public unsafe ref Vector2 GlyphOffset
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->GlyphOffset));
			}
		}

		public unsafe ImFontPtr Font
		{
			get
			{
				return new ImFontPtr(this.NativePtr->Font);
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImFontAtlasCustomRect_destroy(this.NativePtr);
		}

		public bool IsPacked()
		{
			return ImGuiNative.ImFontAtlasCustomRect_IsPacked(this.NativePtr) > 0;
		}
	}
}
