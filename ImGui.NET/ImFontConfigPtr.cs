using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace ImGuiNET
{
	public struct ImFontConfigPtr
	{
		public unsafe readonly ImFontConfig* NativePtr { get; }

		public unsafe ImFontConfigPtr(ImFontConfig* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImFontConfigPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImFontConfig*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImFontConfigPtr(ImFontConfig* nativePtr)
		{
			return new ImFontConfigPtr(nativePtr);
		}

		public unsafe static implicit operator ImFontConfig*(ImFontConfigPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImFontConfigPtr(IntPtr nativePtr)
		{
			return new ImFontConfigPtr(nativePtr);
		}

		public unsafe IntPtr FontData
		{
			get
			{
				return (IntPtr)this.NativePtr->FontData;
			}
			set
			{
				this.NativePtr->FontData = (void*)value;
			}
		}

		public unsafe ref int FontDataSize
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->FontDataSize));
			}
		}

		public unsafe ref bool FontDataOwnedByAtlas
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->FontDataOwnedByAtlas));
			}
		}

		public unsafe ref int FontNo
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->FontNo));
			}
		}

		public unsafe ref float SizePixels
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->SizePixels));
			}
		}

		public unsafe ref int OversampleH
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->OversampleH));
			}
		}

		public unsafe ref int OversampleV
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->OversampleV));
			}
		}

		public unsafe ref bool PixelSnapH
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->PixelSnapH));
			}
		}

		public unsafe ref Vector2 GlyphExtraSpacing
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->GlyphExtraSpacing));
			}
		}

		public unsafe ref Vector2 GlyphOffset
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->GlyphOffset));
			}
		}

		public unsafe IntPtr GlyphRanges
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->GlyphRanges);
			}
			set
			{
				this.NativePtr->GlyphRanges = (ushort*)(void*)value;
			}
		}

		public unsafe ref float GlyphMinAdvanceX
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->GlyphMinAdvanceX));
			}
		}

		public unsafe ref float GlyphMaxAdvanceX
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->GlyphMaxAdvanceX));
			}
		}

		public unsafe ref bool MergeMode
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->MergeMode));
			}
		}

		public unsafe ref uint FontBuilderFlags
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->FontBuilderFlags));
			}
		}

		public unsafe ref float RasterizerMultiply
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->RasterizerMultiply));
			}
		}

		public unsafe ref ushort EllipsisChar
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->EllipsisChar));
			}
		}

		public unsafe RangeAccessor<byte> Name
		{
			get
			{
				return new RangeAccessor<byte>((void*)(&this.NativePtr->Name.FixedElementField), 40);
			}
		}

		public unsafe ImFontPtr DstFont
		{
			get
			{
				return new ImFontPtr(this.NativePtr->DstFont);
			}
		}

		public void Destroy()
		{
			ImGuiNative.ImFontConfig_destroy(this.NativePtr);
		}
	}
}
