using System;
using System.Numerics;

namespace ImGuiNET
{
	public struct ImFontAtlasCustomRect
	{
		public ushort Width;

		public ushort Height;

		public ushort X;

		public ushort Y;

		public uint GlyphID;

		public float GlyphAdvanceX;

		public Vector2 GlyphOffset;

		public unsafe ImFont* Font;
	}
}
