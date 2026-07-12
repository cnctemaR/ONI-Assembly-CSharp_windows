using System;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImDrawCmd
	{
		public Vector4 ClipRect;

		public IntPtr TextureId;

		public uint VtxOffset;

		public uint IdxOffset;

		public uint ElemCount;

		public IntPtr UserCallback;

		public unsafe void* UserCallbackData;
	}
}
