using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGuiNET
{
	public struct ImGuiTextFilter
	{
		[FixedBuffer(typeof(byte), 256)]
		public ImGuiTextFilter.<InputBuf>e__FixedBuffer InputBuf;

		public ImVector Filters;

		public int CountGrep;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 256)]
		public struct <InputBuf>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
