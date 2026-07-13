using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
	[NullableContext(2)]
	[Nullable(0)]
	internal static class SequenceMarshal
	{
		public static bool TryGetReadOnlySequenceSegment<T>([Nullable(new byte[] { 0, 1 })] global::System.Buffers.ReadOnlySequence<T> sequence, [Nullable(new byte[] { 2, 1 })] out global::System.Buffers.ReadOnlySequenceSegment<T> startSegment, out int startIndex, [Nullable(new byte[] { 2, 1 })] out global::System.Buffers.ReadOnlySequenceSegment<T> endSegment, out int endIndex)
		{
			return sequence.TryGetReadOnlySequenceSegment(out startSegment, out startIndex, out endSegment, out endIndex);
		}

		public static bool TryGetArray<T>([Nullable(new byte[] { 0, 1 })] global::System.Buffers.ReadOnlySequence<T> sequence, [Nullable(new byte[] { 0, 1 })] out ArraySegment<T> segment)
		{
			return sequence.TryGetArray(out segment);
		}

		public static bool TryGetReadOnlyMemory<T>([Nullable(new byte[] { 0, 1 })] global::System.Buffers.ReadOnlySequence<T> sequence, [Nullable(new byte[] { 0, 1 })] out global::System.ReadOnlyMemory<T> memory)
		{
			if (!sequence.IsSingleSegment)
			{
				memory = default(global::System.ReadOnlyMemory<T>);
				return false;
			}
			memory = sequence.First;
			return true;
		}

		[NullableContext(0)]
		internal static bool TryGetString(global::System.Buffers.ReadOnlySequence<char> sequence, [Nullable(1)] [<1c2fb156-e9ba-45cc-af54-d5335bdb59af>MaybeNullWhen(false)] out string text, out int start, out int length)
		{
			return sequence.TryGetString(out text, out start, out length);
		}
	}
}
