using System;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImFontGlyphPtr
	{
		public unsafe readonly ImFontGlyph* NativePtr { get; }

		public unsafe ImFontGlyphPtr(ImFontGlyph* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImFontGlyphPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImFontGlyph*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImFontGlyphPtr(ImFontGlyph* nativePtr)
		{
			return new ImFontGlyphPtr(nativePtr);
		}

		public unsafe static implicit operator ImFontGlyph*(ImFontGlyphPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImFontGlyphPtr(IntPtr nativePtr)
		{
			return new ImFontGlyphPtr(nativePtr);
		}

		public unsafe ref uint Colored
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->Colored));
			}
		}

		public unsafe ref uint Visible
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->Visible));
			}
		}

		public unsafe ref uint Codepoint
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->Codepoint));
			}
		}

		public unsafe ref float AdvanceX
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->AdvanceX));
			}
		}

		public unsafe ref float X0
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->X0));
			}
		}

		public unsafe ref float Y0
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->Y0));
			}
		}

		public unsafe ref float X1
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->X1));
			}
		}

		public unsafe ref float Y1
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->Y1));
			}
		}

		public unsafe ref float U0
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->U0));
			}
		}

		public unsafe ref float V0
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->V0));
			}
		}

		public unsafe ref float U1
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->U1));
			}
		}

		public unsafe ref float V1
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->V1));
			}
		}
	}
}
