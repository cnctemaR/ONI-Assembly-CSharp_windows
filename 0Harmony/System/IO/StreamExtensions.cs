using System;
using System.Runtime.CompilerServices;

namespace System.IO
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class StreamExtensions
	{
		public static void CopyTo(this Stream src, Stream destination)
		{
			ThrowHelper.ThrowIfArgumentNull(src, "src", null);
			src.CopyTo(destination);
		}

		public static void CopyTo(this Stream src, Stream destination, int bufferSize)
		{
			ThrowHelper.ThrowIfArgumentNull(src, "src", null);
			src.CopyTo(destination, bufferSize);
		}
	}
}
