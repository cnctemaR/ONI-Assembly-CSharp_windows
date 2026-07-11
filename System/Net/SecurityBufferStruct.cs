using System;

namespace System.Net
{
	internal struct SecurityBufferStruct
	{
		public int count;

		public BufferType type;

		public IntPtr token;

		public static readonly int Size = sizeof(SecurityBufferStruct);
	}
}
