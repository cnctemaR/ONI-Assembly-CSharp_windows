using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ImGuiNET
{
	public struct ImGuiPayload
	{
		public unsafe void* Data;

		public int DataSize;

		public uint SourceId;

		public uint SourceParentId;

		public int DataFrameCount;

		[FixedBuffer(typeof(byte), 33)]
		public ImGuiPayload.<DataType>e__FixedBuffer DataType;

		public byte Preview;

		public byte Delivery;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 33)]
		public struct <DataType>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
