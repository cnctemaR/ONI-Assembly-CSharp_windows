using System;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImFontAtlasPtr
	{
		public unsafe readonly ImFontAtlas* NativePtr { get; }

		public unsafe ImFontAtlasPtr(ImFontAtlas* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImFontAtlasPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImFontAtlas*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImFontAtlasPtr(ImFontAtlas* nativePtr)
		{
			return new ImFontAtlasPtr(nativePtr);
		}

		public unsafe static implicit operator ImFontAtlas*(ImFontAtlasPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImFontAtlasPtr(IntPtr nativePtr)
		{
			return new ImFontAtlasPtr(nativePtr);
		}

		public unsafe ref ImFontAtlasFlags Flags
		{
			get
			{
				return Unsafe.AsRef<ImFontAtlasFlags>((void*)(&this.NativePtr->Flags));
			}
		}

		public unsafe ref IntPtr TexID
		{
			get
			{
				return Unsafe.AsRef<IntPtr>((void*)(&this.NativePtr->TexID));
			}
		}

		public unsafe ref int TexDesiredWidth
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->TexDesiredWidth));
			}
		}

		public unsafe ref int TexGlyphPadding
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->TexGlyphPadding));
			}
		}

		public unsafe ref bool Locked
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->Locked));
			}
		}

		public unsafe ref bool TexPixelsUseColors
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->TexPixelsUseColors));
			}
		}

		public unsafe IntPtr TexPixelsAlpha8
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->TexPixelsAlpha8);
			}
			set
			{
				this.NativePtr->TexPixelsAlpha8 = (byte*)(void*)value;
			}
		}

		public unsafe IntPtr TexPixelsRGBA32
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->TexPixelsRGBA32);
			}
			set
			{
				this.NativePtr->TexPixelsRGBA32 = (uint*)(void*)value;
			}
		}

		public unsafe ref int TexWidth
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->TexWidth));
			}
		}

		public unsafe ref int TexHeight
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->TexHeight));
			}
		}

		public unsafe ref Vector2 TexUvScale
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->TexUvScale));
			}
		}

		public unsafe ref Vector2 TexUvWhitePixel
		{
			get
			{
				return Unsafe.AsRef<Vector2>((void*)(&this.NativePtr->TexUvWhitePixel));
			}
		}

		public unsafe ImVector<ImFontPtr> Fonts
		{
			get
			{
				return new ImVector<ImFontPtr>(this.NativePtr->Fonts);
			}
		}

		public unsafe ImPtrVector<ImFontAtlasCustomRectPtr> CustomRects
		{
			get
			{
				return new ImPtrVector<ImFontAtlasCustomRectPtr>(this.NativePtr->CustomRects, Unsafe.SizeOf<ImFontAtlasCustomRect>());
			}
		}

		public unsafe ImPtrVector<ImFontConfigPtr> ConfigData
		{
			get
			{
				return new ImPtrVector<ImFontConfigPtr>(this.NativePtr->ConfigData, Unsafe.SizeOf<ImFontConfig>());
			}
		}

		public unsafe RangeAccessor<Vector4> TexUvLines
		{
			get
			{
				return new RangeAccessor<Vector4>((void*)(&this.NativePtr->TexUvLines_0), 64);
			}
		}

		public unsafe IntPtr FontBuilderIO
		{
			get
			{
				return (IntPtr)((void*)this.NativePtr->FontBuilderIO);
			}
			set
			{
				this.NativePtr->FontBuilderIO = (IntPtr*)(void*)value;
			}
		}

		public unsafe ref uint FontBuilderFlags
		{
			get
			{
				return Unsafe.AsRef<uint>((void*)(&this.NativePtr->FontBuilderFlags));
			}
		}

		public unsafe ref int PackIdMouseCursors
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->PackIdMouseCursors));
			}
		}

		public unsafe ref int PackIdLines
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->PackIdLines));
			}
		}

		public unsafe int AddCustomRectFontGlyph(ImFontPtr font, ushort id, int width, int height, float advance_x)
		{
			ImFont* nativePtr = font.NativePtr;
			Vector2 vector = default(Vector2);
			return ImGuiNative.ImFontAtlas_AddCustomRectFontGlyph(this.NativePtr, nativePtr, id, width, height, advance_x, vector);
		}

		public unsafe int AddCustomRectFontGlyph(ImFontPtr font, ushort id, int width, int height, float advance_x, Vector2 offset)
		{
			ImFont* nativePtr = font.NativePtr;
			return ImGuiNative.ImFontAtlas_AddCustomRectFontGlyph(this.NativePtr, nativePtr, id, width, height, advance_x, offset);
		}

		public int AddCustomRectRegular(int width, int height)
		{
			return ImGuiNative.ImFontAtlas_AddCustomRectRegular(this.NativePtr, width, height);
		}

		public unsafe ImFontPtr AddFont(ImFontConfigPtr font_cfg)
		{
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFont(this.NativePtr, nativePtr));
		}

		public unsafe ImFontPtr AddFontDefault()
		{
			ImFontConfig* ptr = null;
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFontDefault(this.NativePtr, ptr));
		}

		public unsafe ImFontPtr AddFontDefault(ImFontConfigPtr font_cfg)
		{
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFontDefault(this.NativePtr, nativePtr));
		}

		public unsafe ImFontPtr AddFontFromFileTTF(string filename, float size_pixels)
		{
			int num = 0;
			byte* ptr;
			if (filename != null)
			{
				num = Encoding.UTF8.GetByteCount(filename);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(filename, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImFontConfig* ptr2 = null;
			ushort* ptr3 = null;
			ImFont* ptr4 = ImGuiNative.ImFontAtlas_AddFontFromFileTTF(this.NativePtr, ptr, size_pixels, ptr2, ptr3);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return new ImFontPtr(ptr4);
		}

		public unsafe ImFontPtr AddFontFromFileTTF(string filename, float size_pixels, ImFontConfigPtr font_cfg)
		{
			int num = 0;
			byte* ptr;
			if (filename != null)
			{
				num = Encoding.UTF8.GetByteCount(filename);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(filename, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			ushort* ptr2 = null;
			ImFont* ptr3 = ImGuiNative.ImFontAtlas_AddFontFromFileTTF(this.NativePtr, ptr, size_pixels, nativePtr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return new ImFontPtr(ptr3);
		}

		public unsafe ImFontPtr AddFontFromFileTTF(string filename, float size_pixels, ImFontConfigPtr font_cfg, IntPtr glyph_ranges)
		{
			int num = 0;
			byte* ptr;
			if (filename != null)
			{
				num = Encoding.UTF8.GetByteCount(filename);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(filename, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			ushort* ptr2 = (ushort*)glyph_ranges.ToPointer();
			ImFont* ptr3 = ImGuiNative.ImFontAtlas_AddFontFromFileTTF(this.NativePtr, ptr, size_pixels, nativePtr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return new ImFontPtr(ptr3);
		}

		public unsafe ImFontPtr AddFontFromMemoryCompressedBase85TTF(string compressed_font_data_base85, float size_pixels)
		{
			int num = 0;
			byte* ptr;
			if (compressed_font_data_base85 != null)
			{
				num = Encoding.UTF8.GetByteCount(compressed_font_data_base85);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(compressed_font_data_base85, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImFontConfig* ptr2 = null;
			ushort* ptr3 = null;
			ImFont* ptr4 = ImGuiNative.ImFontAtlas_AddFontFromMemoryCompressedBase85TTF(this.NativePtr, ptr, size_pixels, ptr2, ptr3);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return new ImFontPtr(ptr4);
		}

		public unsafe ImFontPtr AddFontFromMemoryCompressedBase85TTF(string compressed_font_data_base85, float size_pixels, ImFontConfigPtr font_cfg)
		{
			int num = 0;
			byte* ptr;
			if (compressed_font_data_base85 != null)
			{
				num = Encoding.UTF8.GetByteCount(compressed_font_data_base85);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(compressed_font_data_base85, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			ushort* ptr2 = null;
			ImFont* ptr3 = ImGuiNative.ImFontAtlas_AddFontFromMemoryCompressedBase85TTF(this.NativePtr, ptr, size_pixels, nativePtr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return new ImFontPtr(ptr3);
		}

		public unsafe ImFontPtr AddFontFromMemoryCompressedBase85TTF(string compressed_font_data_base85, float size_pixels, ImFontConfigPtr font_cfg, IntPtr glyph_ranges)
		{
			int num = 0;
			byte* ptr;
			if (compressed_font_data_base85 != null)
			{
				num = Encoding.UTF8.GetByteCount(compressed_font_data_base85);
				if (num > 2048)
				{
					ptr = Util.Allocate(num + 1);
				}
				else
				{
					ptr = stackalloc byte[(UIntPtr)(num + 1)];
				}
				int utf = Util.GetUtf8(compressed_font_data_base85, ptr, num);
				ptr[utf] = 0;
			}
			else
			{
				ptr = null;
			}
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			ushort* ptr2 = (ushort*)glyph_ranges.ToPointer();
			ImFont* ptr3 = ImGuiNative.ImFontAtlas_AddFontFromMemoryCompressedBase85TTF(this.NativePtr, ptr, size_pixels, nativePtr, ptr2);
			if (num > 2048)
			{
				Util.Free(ptr);
			}
			return new ImFontPtr(ptr3);
		}

		public unsafe ImFontPtr AddFontFromMemoryCompressedTTF(IntPtr compressed_font_data, int compressed_font_size, float size_pixels)
		{
			void* ptr = compressed_font_data.ToPointer();
			ImFontConfig* ptr2 = null;
			ushort* ptr3 = null;
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFontFromMemoryCompressedTTF(this.NativePtr, ptr, compressed_font_size, size_pixels, ptr2, ptr3));
		}

		public unsafe ImFontPtr AddFontFromMemoryCompressedTTF(IntPtr compressed_font_data, int compressed_font_size, float size_pixels, ImFontConfigPtr font_cfg)
		{
			void* ptr = compressed_font_data.ToPointer();
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			ushort* ptr2 = null;
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFontFromMemoryCompressedTTF(this.NativePtr, ptr, compressed_font_size, size_pixels, nativePtr, ptr2));
		}

		public unsafe ImFontPtr AddFontFromMemoryCompressedTTF(IntPtr compressed_font_data, int compressed_font_size, float size_pixels, ImFontConfigPtr font_cfg, IntPtr glyph_ranges)
		{
			void* ptr = compressed_font_data.ToPointer();
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			ushort* ptr2 = (ushort*)glyph_ranges.ToPointer();
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFontFromMemoryCompressedTTF(this.NativePtr, ptr, compressed_font_size, size_pixels, nativePtr, ptr2));
		}

		public unsafe ImFontPtr AddFontFromMemoryTTF(IntPtr font_data, int font_size, float size_pixels)
		{
			void* ptr = font_data.ToPointer();
			ImFontConfig* ptr2 = null;
			ushort* ptr3 = null;
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(this.NativePtr, ptr, font_size, size_pixels, ptr2, ptr3));
		}

		public unsafe ImFontPtr AddFontFromMemoryTTF(IntPtr font_data, int font_size, float size_pixels, ImFontConfigPtr font_cfg)
		{
			void* ptr = font_data.ToPointer();
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			ushort* ptr2 = null;
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(this.NativePtr, ptr, font_size, size_pixels, nativePtr, ptr2));
		}

		public unsafe ImFontPtr AddFontFromMemoryTTF(IntPtr font_data, int font_size, float size_pixels, ImFontConfigPtr font_cfg, IntPtr glyph_ranges)
		{
			void* ptr = font_data.ToPointer();
			ImFontConfig* nativePtr = font_cfg.NativePtr;
			ushort* ptr2 = (ushort*)glyph_ranges.ToPointer();
			return new ImFontPtr(ImGuiNative.ImFontAtlas_AddFontFromMemoryTTF(this.NativePtr, ptr, font_size, size_pixels, nativePtr, ptr2));
		}

		public bool Build()
		{
			return ImGuiNative.ImFontAtlas_Build(this.NativePtr) > 0;
		}

		public unsafe void CalcCustomRectUV(ImFontAtlasCustomRectPtr rect, out Vector2 out_uv_min, out Vector2 out_uv_max)
		{
			ImFontAtlasCustomRect* nativePtr = rect.NativePtr;
			fixed (Vector2* ptr = &out_uv_min)
			{
				Vector2* ptr2 = ptr;
				fixed (Vector2* ptr3 = &out_uv_max)
				{
					Vector2* ptr4 = ptr3;
					ImGuiNative.ImFontAtlas_CalcCustomRectUV(this.NativePtr, nativePtr, ptr2, ptr4);
				}
			}
		}

		public void Clear()
		{
			ImGuiNative.ImFontAtlas_Clear(this.NativePtr);
		}

		public void ClearFonts()
		{
			ImGuiNative.ImFontAtlas_ClearFonts(this.NativePtr);
		}

		public void ClearInputData()
		{
			ImGuiNative.ImFontAtlas_ClearInputData(this.NativePtr);
		}

		public void ClearTexData()
		{
			ImGuiNative.ImFontAtlas_ClearTexData(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImFontAtlas_destroy(this.NativePtr);
		}

		public ImFontAtlasCustomRectPtr GetCustomRectByIndex(int index)
		{
			return new ImFontAtlasCustomRectPtr(ImGuiNative.ImFontAtlas_GetCustomRectByIndex(this.NativePtr, index));
		}

		public unsafe IntPtr GetGlyphRangesChineseFull()
		{
			return (IntPtr)((void*)ImGuiNative.ImFontAtlas_GetGlyphRangesChineseFull(this.NativePtr));
		}

		public unsafe IntPtr GetGlyphRangesChineseSimplifiedCommon()
		{
			return (IntPtr)((void*)ImGuiNative.ImFontAtlas_GetGlyphRangesChineseSimplifiedCommon(this.NativePtr));
		}

		public unsafe IntPtr GetGlyphRangesCyrillic()
		{
			return (IntPtr)((void*)ImGuiNative.ImFontAtlas_GetGlyphRangesCyrillic(this.NativePtr));
		}

		public unsafe IntPtr GetGlyphRangesDefault()
		{
			return (IntPtr)((void*)ImGuiNative.ImFontAtlas_GetGlyphRangesDefault(this.NativePtr));
		}

		public unsafe IntPtr GetGlyphRangesJapanese()
		{
			return (IntPtr)((void*)ImGuiNative.ImFontAtlas_GetGlyphRangesJapanese(this.NativePtr));
		}

		public unsafe IntPtr GetGlyphRangesKorean()
		{
			return (IntPtr)((void*)ImGuiNative.ImFontAtlas_GetGlyphRangesKorean(this.NativePtr));
		}

		public unsafe IntPtr GetGlyphRangesThai()
		{
			return (IntPtr)((void*)ImGuiNative.ImFontAtlas_GetGlyphRangesThai(this.NativePtr));
		}

		public unsafe IntPtr GetGlyphRangesVietnamese()
		{
			return (IntPtr)((void*)ImGuiNative.ImFontAtlas_GetGlyphRangesVietnamese(this.NativePtr));
		}

		public unsafe bool GetMouseCursorTexData(ImGuiMouseCursor cursor, out Vector2 out_offset, out Vector2 out_size, out Vector2 out_uv_border, out Vector2 out_uv_fill)
		{
			fixed (Vector2* ptr = &out_offset)
			{
				Vector2* ptr2 = ptr;
				fixed (Vector2* ptr3 = &out_size)
				{
					Vector2* ptr4 = ptr3;
					fixed (Vector2* ptr5 = &out_uv_border)
					{
						Vector2* ptr6 = ptr5;
						fixed (Vector2* ptr7 = &out_uv_fill)
						{
							Vector2* ptr8 = ptr7;
							return ImGuiNative.ImFontAtlas_GetMouseCursorTexData(this.NativePtr, cursor, ptr2, ptr4, ptr6, ptr8) > 0;
						}
					}
				}
			}
		}

		public unsafe void GetTexDataAsAlpha8(out byte* out_pixels, out int out_width, out int out_height)
		{
			int* ptr = null;
			fixed (byte** ptr2 = &out_pixels)
			{
				byte** ptr3 = ptr2;
				fixed (int* ptr4 = &out_width)
				{
					int* ptr5 = ptr4;
					fixed (int* ptr6 = &out_height)
					{
						int* ptr7 = ptr6;
						ImGuiNative.ImFontAtlas_GetTexDataAsAlpha8(this.NativePtr, ptr3, ptr5, ptr7, ptr);
					}
				}
			}
		}

		public unsafe void GetTexDataAsAlpha8(out byte* out_pixels, out int out_width, out int out_height, out int out_bytes_per_pixel)
		{
			fixed (byte** ptr = &out_pixels)
			{
				byte** ptr2 = ptr;
				fixed (int* ptr3 = &out_width)
				{
					int* ptr4 = ptr3;
					fixed (int* ptr5 = &out_height)
					{
						int* ptr6 = ptr5;
						fixed (int* ptr7 = &out_bytes_per_pixel)
						{
							int* ptr8 = ptr7;
							ImGuiNative.ImFontAtlas_GetTexDataAsAlpha8(this.NativePtr, ptr2, ptr4, ptr6, ptr8);
						}
					}
				}
			}
		}

		public unsafe void GetTexDataAsAlpha8(out IntPtr out_pixels, out int out_width, out int out_height)
		{
			int* ptr = null;
			fixed (IntPtr* ptr2 = &out_pixels)
			{
				IntPtr* ptr3 = ptr2;
				fixed (int* ptr4 = &out_width)
				{
					int* ptr5 = ptr4;
					fixed (int* ptr6 = &out_height)
					{
						int* ptr7 = ptr6;
						ImGuiNative.ImFontAtlas_GetTexDataAsAlpha8(this.NativePtr, ptr3, ptr5, ptr7, ptr);
					}
				}
			}
		}

		public unsafe void GetTexDataAsAlpha8(out IntPtr out_pixels, out int out_width, out int out_height, out int out_bytes_per_pixel)
		{
			fixed (IntPtr* ptr = &out_pixels)
			{
				IntPtr* ptr2 = ptr;
				fixed (int* ptr3 = &out_width)
				{
					int* ptr4 = ptr3;
					fixed (int* ptr5 = &out_height)
					{
						int* ptr6 = ptr5;
						fixed (int* ptr7 = &out_bytes_per_pixel)
						{
							int* ptr8 = ptr7;
							ImGuiNative.ImFontAtlas_GetTexDataAsAlpha8(this.NativePtr, ptr2, ptr4, ptr6, ptr8);
						}
					}
				}
			}
		}

		public unsafe void GetTexDataAsRGBA32(out byte* out_pixels, out int out_width, out int out_height)
		{
			int* ptr = null;
			fixed (byte** ptr2 = &out_pixels)
			{
				byte** ptr3 = ptr2;
				fixed (int* ptr4 = &out_width)
				{
					int* ptr5 = ptr4;
					fixed (int* ptr6 = &out_height)
					{
						int* ptr7 = ptr6;
						ImGuiNative.ImFontAtlas_GetTexDataAsRGBA32(this.NativePtr, ptr3, ptr5, ptr7, ptr);
					}
				}
			}
		}

		public unsafe void GetTexDataAsRGBA32(out byte* out_pixels, out int out_width, out int out_height, out int out_bytes_per_pixel)
		{
			fixed (byte** ptr = &out_pixels)
			{
				byte** ptr2 = ptr;
				fixed (int* ptr3 = &out_width)
				{
					int* ptr4 = ptr3;
					fixed (int* ptr5 = &out_height)
					{
						int* ptr6 = ptr5;
						fixed (int* ptr7 = &out_bytes_per_pixel)
						{
							int* ptr8 = ptr7;
							ImGuiNative.ImFontAtlas_GetTexDataAsRGBA32(this.NativePtr, ptr2, ptr4, ptr6, ptr8);
						}
					}
				}
			}
		}

		public unsafe void GetTexDataAsRGBA32(out IntPtr out_pixels, out int out_width, out int out_height)
		{
			int* ptr = null;
			fixed (IntPtr* ptr2 = &out_pixels)
			{
				IntPtr* ptr3 = ptr2;
				fixed (int* ptr4 = &out_width)
				{
					int* ptr5 = ptr4;
					fixed (int* ptr6 = &out_height)
					{
						int* ptr7 = ptr6;
						ImGuiNative.ImFontAtlas_GetTexDataAsRGBA32(this.NativePtr, ptr3, ptr5, ptr7, ptr);
					}
				}
			}
		}

		public unsafe void GetTexDataAsRGBA32(out IntPtr out_pixels, out int out_width, out int out_height, out int out_bytes_per_pixel)
		{
			fixed (IntPtr* ptr = &out_pixels)
			{
				IntPtr* ptr2 = ptr;
				fixed (int* ptr3 = &out_width)
				{
					int* ptr4 = ptr3;
					fixed (int* ptr5 = &out_height)
					{
						int* ptr6 = ptr5;
						fixed (int* ptr7 = &out_bytes_per_pixel)
						{
							int* ptr8 = ptr7;
							ImGuiNative.ImFontAtlas_GetTexDataAsRGBA32(this.NativePtr, ptr2, ptr4, ptr6, ptr8);
						}
					}
				}
			}
		}

		public bool IsBuilt()
		{
			return ImGuiNative.ImFontAtlas_IsBuilt(this.NativePtr) > 0;
		}

		public void SetTexID(IntPtr id)
		{
			ImGuiNative.ImFontAtlas_SetTexID(this.NativePtr, id);
		}
	}
}
