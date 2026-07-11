using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 12)]
	public struct EpollEvent
	{
		[FieldOffset(0)]
		public EpollEvents events;

		[FieldOffset(4)]
		public int fd;

		[FieldOffset(4)]
		public IntPtr ptr;

		[FieldOffset(4)]
		public uint u32;

		[FieldOffset(4)]
		public ulong u64;
	}
}
