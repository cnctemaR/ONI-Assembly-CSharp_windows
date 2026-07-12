using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGuiNET
{
	public struct ImFontConfig
	{
		public unsafe void* FontData;

		public int FontDataSize;

		public byte FontDataOwnedByAtlas;

		public int FontNo;

		public float SizePixels;

		public int OversampleH;

		public int OversampleV;

		public byte PixelSnapH;

		public Vector2 GlyphExtraSpacing;

		public Vector2 GlyphOffset;

		public unsafe ushort* GlyphRanges;

		public float GlyphMinAdvanceX;

		public float GlyphMaxAdvanceX;

		public byte MergeMode;

		public uint FontBuilderFlags;

		public float RasterizerMultiply;

		public ushort EllipsisChar;

		[FixedBuffer(typeof(byte), 40)]
		public ImFontConfig.<Name>e__FixedBuffer Name;

		public unsafe ImFont* DstFont;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 40)]
		public struct <Name>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
