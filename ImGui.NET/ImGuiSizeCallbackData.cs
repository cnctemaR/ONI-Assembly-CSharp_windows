using System;
using System.Numerics;

namespace ImGuiNET
{
	public struct ImGuiSizeCallbackData
	{
		public unsafe void* UserData;

		public Vector2 Pos;

		public Vector2 CurrentSize;

		public Vector2 DesiredSize;
	}
}
