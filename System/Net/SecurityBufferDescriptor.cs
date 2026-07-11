using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Net
{
	[StructLayout(LayoutKind.Sequential)]
	internal class SecurityBufferDescriptor
	{
		public SecurityBufferDescriptor(int count)
		{
			this.Version = 0;
			this.Count = count;
			this.UnmanagedPointer = null;
		}

		[Conditional("TRAVE")]
		internal void DebugDump()
		{
		}

		public readonly int Version;

		public readonly int Count;

		public unsafe void* UnmanagedPointer;
	}
}
