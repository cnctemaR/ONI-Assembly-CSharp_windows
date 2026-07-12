using System;
using System.Text;

namespace ImGuiNET
{
	public struct ImFontGlyphRangesBuilderPtr
	{
		public unsafe readonly ImFontGlyphRangesBuilder* NativePtr { get; }

		public unsafe ImFontGlyphRangesBuilderPtr(ImFontGlyphRangesBuilder* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImFontGlyphRangesBuilderPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImFontGlyphRangesBuilder*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImFontGlyphRangesBuilderPtr(ImFontGlyphRangesBuilder* nativePtr)
		{
			return new ImFontGlyphRangesBuilderPtr(nativePtr);
		}

		public unsafe static implicit operator ImFontGlyphRangesBuilder*(ImFontGlyphRangesBuilderPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImFontGlyphRangesBuilderPtr(IntPtr nativePtr)
		{
			return new ImFontGlyphRangesBuilderPtr(nativePtr);
		}

		public unsafe ImVector<uint> UsedChars
		{
			get
			{
				return new ImVector<uint>(this.NativePtr->UsedChars);
			}
		}

		public void AddChar(ushort c)
		{
			ImGuiNative.ImFontGlyphRangesBuilder_AddChar(this.NativePtr, c);
		}

		public unsafe void AddRanges(IntPtr ranges)
		{
			ushort* ptr = (ushort*)ranges.ToPointer();
			ImGuiNative.ImFontGlyphRangesBuilder_AddRanges(this.NativePtr, ptr);
		}

		public unsafe void AddText(string text)
		{
			int num = 0;
			byte* ptr;
			if (text != null)
			{
				num = Encoding.UTF8.GetByteCount(text);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(text, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			byte* ptr2 = null;
			ImGuiNative.ImFontGlyphRangesBuilder_AddText(this.NativePtr, ptr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
		}

		public unsafe void BuildRanges(out ImVector out_ranges)
		{
			fixed (ImVector* ptr = &out_ranges)
			{
				ImVector* ptr2 = ptr;
				ImGuiNative.ImFontGlyphRangesBuilder_BuildRanges(this.NativePtr, ptr2);
			}
		}

		public void Clear()
		{
			ImGuiNative.ImFontGlyphRangesBuilder_Clear(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImFontGlyphRangesBuilder_destroy(this.NativePtr);
		}

		public bool GetBit(uint n)
		{
			return ImGuiNative.ImFontGlyphRangesBuilder_GetBit(this.NativePtr, n) > 0;
		}

		public void SetBit(uint n)
		{
			ImGuiNative.ImFontGlyphRangesBuilder_SetBit(this.NativePtr, n);
		}
	}
}
