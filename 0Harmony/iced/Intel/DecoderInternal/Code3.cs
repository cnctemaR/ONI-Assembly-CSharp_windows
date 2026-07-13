using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Iced.Intel.DecoderInternal
{
	internal struct Code3
	{
		public unsafe Code3(Code code16, Code code32, Code code64)
		{
			this.codes.FixedElementField = (ushort)code16;
			*((ref this.codes.FixedElementField) + 2) = (ushort)code32;
			*((ref this.codes.FixedElementField) + (IntPtr)2 * 2) = (ushort)code64;
		}

		[FixedBuffer(typeof(ushort), 3)]
		public Code3.<codes>e__FixedBuffer codes;

		[CompilerGenerated]
		[UnsafeValueType]
		[StructLayout(LayoutKind.Sequential, Size = 6)]
		public struct <codes>e__FixedBuffer
		{
			public ushort FixedElementField;
		}
	}
}
