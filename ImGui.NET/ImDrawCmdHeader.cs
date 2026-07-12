using System;
using System.Numerics;

namespace ImGuiNET
{
	public struct ImDrawCmdHeader
	{
		public Vector4 ClipRect;

		public IntPtr TextureId;

		public uint VtxOffset;
	}
}
