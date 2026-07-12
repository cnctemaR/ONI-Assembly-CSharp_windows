using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGuiNET
{
	public struct ImFont
	{
		public ImVector IndexAdvanceX;

		public float FallbackAdvanceX;

		public float FontSize;

		public ImVector IndexLookup;

		public ImVector Glyphs;

		public unsafe ImFontGlyph* FallbackGlyph;

		public unsafe ImFontAtlas* ContainerAtlas;

		public unsafe ImFontConfig* ConfigData;

		public short ConfigDataCount;

		public ushort FallbackChar;

		public ushort EllipsisChar;

		public byte DirtyLookupTables;

		public float Scale;

		public float Ascent;

		public float Descent;

		public int MetricsTotalSurface;

		[FixedBuffer(typeof(byte), 2)]
		public ImFont.<Used4kPagesMap>e__FixedBuffer Used4kPagesMap;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 2)]
		public struct <Used4kPagesMap>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
