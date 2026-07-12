using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImFontPtr
	{
		public unsafe readonly ImFont* NativePtr { get; }

		public unsafe ImFontPtr(ImFont* nativePtr)
		{
			this.NativePtr = nativePtr;
		}

		public unsafe ImFontPtr(IntPtr nativePtr)
		{
			this.NativePtr = (ImFont*)(void*)nativePtr;
		}

		public unsafe static implicit operator ImFontPtr(ImFont* nativePtr)
		{
			return new ImFontPtr(nativePtr);
		}

		public unsafe static implicit operator ImFont*(ImFontPtr wrappedPtr)
		{
			return wrappedPtr.NativePtr;
		}

		public static implicit operator ImFontPtr(IntPtr nativePtr)
		{
			return new ImFontPtr(nativePtr);
		}

		public unsafe ImVector<float> IndexAdvanceX
		{
			get
			{
				return new ImVector<float>(this.NativePtr->IndexAdvanceX);
			}
		}

		public unsafe ref float FallbackAdvanceX
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->FallbackAdvanceX));
			}
		}

		public unsafe ref float FontSize
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->FontSize));
			}
		}

		public unsafe ImVector<ushort> IndexLookup
		{
			get
			{
				return new ImVector<ushort>(this.NativePtr->IndexLookup);
			}
		}

		public unsafe ImPtrVector<ImFontGlyphPtr> Glyphs
		{
			get
			{
				return new ImPtrVector<ImFontGlyphPtr>(this.NativePtr->Glyphs, Unsafe.SizeOf<ImFontGlyph>());
			}
		}

		public unsafe ImFontGlyphPtr FallbackGlyph
		{
			get
			{
				return new ImFontGlyphPtr(this.NativePtr->FallbackGlyph);
			}
		}

		public unsafe ImFontAtlasPtr ContainerAtlas
		{
			get
			{
				return new ImFontAtlasPtr(this.NativePtr->ContainerAtlas);
			}
		}

		public unsafe ImFontConfigPtr ConfigData
		{
			get
			{
				return new ImFontConfigPtr(this.NativePtr->ConfigData);
			}
		}

		public unsafe ref short ConfigDataCount
		{
			get
			{
				return Unsafe.AsRef<short>((void*)(&this.NativePtr->ConfigDataCount));
			}
		}

		public unsafe ref ushort FallbackChar
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->FallbackChar));
			}
		}

		public unsafe ref ushort EllipsisChar
		{
			get
			{
				return Unsafe.AsRef<ushort>((void*)(&this.NativePtr->EllipsisChar));
			}
		}

		public unsafe ref bool DirtyLookupTables
		{
			get
			{
				return Unsafe.AsRef<bool>((void*)(&this.NativePtr->DirtyLookupTables));
			}
		}

		public unsafe ref float Scale
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->Scale));
			}
		}

		public unsafe ref float Ascent
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->Ascent));
			}
		}

		public unsafe ref float Descent
		{
			get
			{
				return Unsafe.AsRef<float>((void*)(&this.NativePtr->Descent));
			}
		}

		public unsafe ref int MetricsTotalSurface
		{
			get
			{
				return Unsafe.AsRef<int>((void*)(&this.NativePtr->MetricsTotalSurface));
			}
		}

		public unsafe RangeAccessor<byte> Used4kPagesMap
		{
			get
			{
				return new RangeAccessor<byte>((void*)(&this.NativePtr->Used4kPagesMap.FixedElementField), 2);
			}
		}

		public unsafe void AddGlyph(ImFontConfigPtr src_cfg, ushort c, float x0, float y0, float x1, float y1, float u0, float v0, float u1, float v1, float advance_x)
		{
			ImFontConfig* nativePtr = src_cfg.NativePtr;
			ImGuiNative.ImFont_AddGlyph(this.NativePtr, nativePtr, c, x0, y0, x1, y1, u0, v0, u1, v1, advance_x);
		}

		public void AddRemapChar(ushort dst, ushort src)
		{
			byte b = 1;
			ImGuiNative.ImFont_AddRemapChar(this.NativePtr, dst, src, b);
		}

		public void AddRemapChar(ushort dst, ushort src, bool overwrite_dst)
		{
			byte b = (overwrite_dst ? 1 : 0);
			ImGuiNative.ImFont_AddRemapChar(this.NativePtr, dst, src, b);
		}

		public void BuildLookupTable()
		{
			ImGuiNative.ImFont_BuildLookupTable(this.NativePtr);
		}

		public void ClearOutputData()
		{
			ImGuiNative.ImFont_ClearOutputData(this.NativePtr);
		}

		public void Destroy()
		{
			ImGuiNative.ImFont_destroy(this.NativePtr);
		}

		public ImFontGlyphPtr FindGlyph(ushort c)
		{
			return new ImFontGlyphPtr(ImGuiNative.ImFont_FindGlyph(this.NativePtr, c));
		}

		public ImFontGlyphPtr FindGlyphNoFallback(ushort c)
		{
			return new ImFontGlyphPtr(ImGuiNative.ImFont_FindGlyphNoFallback(this.NativePtr, c));
		}

		public float GetCharAdvance(ushort c)
		{
			return ImGuiNative.ImFont_GetCharAdvance(this.NativePtr, c);
		}

		public string GetDebugName()
		{
			return Util.StringFromPtr(ImGuiNative.ImFont_GetDebugName(this.NativePtr));
		}

		public void GrowIndex(int new_size)
		{
			ImGuiNative.ImFont_GrowIndex(this.NativePtr, new_size);
		}

		public bool IsLoaded()
		{
			return ImGuiNative.ImFont_IsLoaded(this.NativePtr) > 0;
		}

		public unsafe void RenderChar(ImDrawListPtr draw_list, float size, Vector2 pos, uint col, ushort c)
		{
			ImDrawList* nativePtr = draw_list.NativePtr;
			ImGuiNative.ImFont_RenderChar(this.NativePtr, nativePtr, size, pos, col, c);
		}

		public void SetFallbackChar(ushort c)
		{
			ImGuiNative.ImFont_SetFallbackChar(this.NativePtr, c);
		}

		public void SetGlyphVisible(ushort c, bool visible)
		{
			byte b = (visible ? 1 : 0);
			ImGuiNative.ImFont_SetGlyphVisible(this.NativePtr, c, b);
		}
	}
}
