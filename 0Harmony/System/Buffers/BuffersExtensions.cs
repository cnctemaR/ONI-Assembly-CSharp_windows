using System;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class BuffersExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static global::System.SequencePosition? PositionOf<[Nullable(0)] T>([Nullable(new byte[] { 0, 1 })] this ReadOnlySequence<T> source, T value) where T : IEquatable<T>
		{
			if (!source.IsSingleSegment)
			{
				return BuffersExtensions.PositionOfMultiSegment<T>(in source, value);
			}
			int num = source.First.Span.IndexOf(value);
			if (num != -1)
			{
				return new global::System.SequencePosition?(source.GetPosition((long)num));
			}
			return null;
		}

		private static global::System.SequencePosition? PositionOfMultiSegment<[Nullable(0)] T>([Nullable(new byte[] { 0, 1 })] in ReadOnlySequence<T> source, T value) where T : IEquatable<T>
		{
			global::System.SequencePosition start = source.Start;
			global::System.SequencePosition sequencePosition = start;
			global::System.ReadOnlyMemory<T> readOnlyMemory;
			while (source.TryGet(ref start, out readOnlyMemory, true))
			{
				int num = readOnlyMemory.Span.IndexOf(value);
				if (num != -1)
				{
					return new global::System.SequencePosition?(source.GetPosition((long)num, sequencePosition));
				}
				if (start.GetObject() == null)
				{
					break;
				}
				sequencePosition = start;
			}
			return null;
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>([Nullable(new byte[] { 0, 1 })] this ReadOnlySequence<T> source, [Nullable(new byte[] { 0, 1 })] global::System.Span<T> destination)
		{
			if (source.Length > (long)destination.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.destination);
			}
			if (source.IsSingleSegment)
			{
				source.First.Span.CopyTo(destination);
				return;
			}
			BuffersExtensions.CopyToMultiSegment<T>(in source, destination);
		}

		[NullableContext(2)]
		private static void CopyToMultiSegment<T>([Nullable(new byte[] { 0, 1 })] in ReadOnlySequence<T> sequence, [Nullable(new byte[] { 0, 1 })] global::System.Span<T> destination)
		{
			global::System.SequencePosition start = sequence.Start;
			global::System.ReadOnlyMemory<T> readOnlyMemory;
			while (sequence.TryGet(ref start, out readOnlyMemory, true))
			{
				global::System.ReadOnlySpan<T> span = readOnlyMemory.Span;
				span.CopyTo(destination);
				if (start.GetObject() == null)
				{
					break;
				}
				destination = destination.Slice(span.Length);
			}
		}

		public static T[] ToArray<[Nullable(2)] T>([Nullable(new byte[] { 0, 1 })] this ReadOnlySequence<T> sequence)
		{
			T[] array = new T[sequence.Length];
			(in sequence).CopyTo<T>(array);
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<[Nullable(2)] T>(this IBufferWriter<T> writer, [Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> value)
		{
			ThrowHelper.ThrowIfArgumentNull(writer, "writer", null);
			global::System.Span<T> span = writer.GetSpan(0);
			if (value.Length <= span.Length)
			{
				value.CopyTo(span);
				writer.Advance(value.Length);
				return;
			}
			BuffersExtensions.WriteMultiSegment<T>(writer, in value, span);
		}

		private static void WriteMultiSegment<[Nullable(2)] T>(IBufferWriter<T> writer, [Nullable(new byte[] { 0, 1 })] in global::System.ReadOnlySpan<T> source, [Nullable(new byte[] { 0, 1 })] global::System.Span<T> destination)
		{
			global::System.ReadOnlySpan<T> readOnlySpan = source;
			for (;;)
			{
				int num = Math.Min(destination.Length, readOnlySpan.Length);
				readOnlySpan.Slice(0, num).CopyTo(destination);
				writer.Advance(num);
				readOnlySpan = readOnlySpan.Slice(num);
				if (readOnlySpan.Length <= 0)
				{
					break;
				}
				destination = writer.GetSpan(readOnlySpan.Length);
			}
		}
	}
}
