using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Unity.Collections.LowLevel.Unsafe
{
	[GenerateTestsForBurstCompatibility]
	internal struct UnsafeStreamBlock
	{
		internal unsafe UnsafeStreamBlock* Next;

		[FixedBuffer(typeof(byte), 1)]
		internal UnsafeStreamBlock.<Data>e__FixedBuffer Data;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct <Data>e__FixedBuffer
		{
			public byte FixedElementField;
		}
	}
}
