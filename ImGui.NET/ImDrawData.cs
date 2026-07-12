using System;
using UnityEngine;

namespace ImGuiNET
{
	public struct ImDrawData
	{
		public byte Valid;

		public int CmdListsCount;

		public int TotalIdxCount;

		public int TotalVtxCount;

		public unsafe ImDrawList** CmdLists;

		public Vector2 DisplayPos;

		public Vector2 DisplaySize;

		public Vector2 FramebufferScale;

		public unsafe ImGuiViewport* OwnerViewport;
	}
}
