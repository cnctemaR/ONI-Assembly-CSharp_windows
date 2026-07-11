using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Collections.Concurrent
{
	[DebuggerDisplay("Head = {Head}, Tail = {Tail}")]
	[StructLayout(LayoutKind.Explicit, Size = 384)]
	internal struct PaddedHeadAndTail
	{
		[FieldOffset(128)]
		public int Head;

		[FieldOffset(256)]
		public int Tail;
	}
}
