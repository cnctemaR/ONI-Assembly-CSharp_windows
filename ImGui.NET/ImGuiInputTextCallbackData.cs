using System;

namespace ImGuiNET
{
	public struct ImGuiInputTextCallbackData
	{
		public ImGuiInputTextFlags EventFlag;

		public ImGuiInputTextFlags Flags;

		public unsafe void* UserData;

		public ushort EventChar;

		public ImGuiKey EventKey;

		public unsafe byte* Buf;

		public int BufTextLen;

		public int BufSize;

		public byte BufDirty;

		public int CursorPos;

		public int SelectionStart;

		public int SelectionEnd;
	}
}
