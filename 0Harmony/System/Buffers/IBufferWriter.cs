using System;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	[NullableContext(2)]
	internal interface IBufferWriter<T>
	{
		void Advance(int count);

		[return: Nullable(new byte[] { 0, 1 })]
		global::System.Memory<T> GetMemory(int sizeHint = 0);

		[return: Nullable(new byte[] { 0, 1 })]
		global::System.Span<T> GetSpan(int sizeHint = 0);
	}
}
