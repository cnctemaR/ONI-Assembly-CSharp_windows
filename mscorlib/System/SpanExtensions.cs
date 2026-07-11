using System;
using System.Runtime.CompilerServices;

namespace System
{
	public static class SpanExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<byte> AsBytes<T>(this Span<T> source) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			int num = checked(source.Length * Unsafe.SizeOf<T>());
			return new Span<byte>(Unsafe.As<Pinnable<byte>>(source.Pinnable), source.ByteOffset, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<byte> AsBytes<T>(this ReadOnlySpan<T> source) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			int num = checked(source.Length * Unsafe.SizeOf<T>());
			return new ReadOnlySpan<byte>(Unsafe.As<Pinnable<byte>>(source.Pinnable), source.ByteOffset, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<char> AsReadOnlySpan(this string text)
		{
			if (text == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.text);
			}
			return new ReadOnlySpan<char>(Unsafe.As<Pinnable<char>>(text), SpanExtensions.StringAdjustment, text.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<TTo> NonPortableCast<TFrom, TTo>(this Span<TFrom> source) where TFrom : struct where TTo : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<TFrom>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TFrom));
			}
			if (SpanHelpers.IsReferenceOrContainsReferences<TTo>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TTo));
			}
			checked
			{
				int num = (int)(unchecked((long)source.Length) * unchecked((long)Unsafe.SizeOf<TFrom>()) / unchecked((long)Unsafe.SizeOf<TTo>()));
				return new Span<TTo>(Unsafe.As<Pinnable<TTo>>(source.Pinnable), source.ByteOffset, num);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<TTo> NonPortableCast<TFrom, TTo>(this ReadOnlySpan<TFrom> source) where TFrom : struct where TTo : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<TFrom>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TFrom));
			}
			if (SpanHelpers.IsReferenceOrContainsReferences<TTo>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(TTo));
			}
			checked
			{
				int num = (int)(unchecked((long)source.Length) * unchecked((long)Unsafe.SizeOf<TFrom>()) / unchecked((long)Unsafe.SizeOf<TTo>()));
				return new ReadOnlySpan<TTo>(Unsafe.As<Pinnable<TTo>>(source.Pinnable), source.ByteOffset, num);
			}
		}

		private unsafe static IntPtr MeasureStringAdjustment()
		{
			string text;
			object obj = (text = "a");
			char* ptr = text;
			if (ptr != null)
			{
				ptr += RuntimeHelpers.OffsetToStringData / 2;
			}
			return Unsafe.ByteOffset<char>(ref Unsafe.As<Pinnable<char>>(obj).Data, Unsafe.AsRef<char>((void*)ptr));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this Span<T> span, T value) where T : struct, IEquatable<T>
		{
			return SpanHelpers.IndexOf<T>(span.DangerousGetPinnableReference(), value, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf(this Span<byte> span, byte value)
		{
			return SpanHelpers.IndexOf(span.DangerousGetPinnableReference(), value, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this Span<T> span, ReadOnlySpan<T> value) where T : struct, IEquatable<T>
		{
			return SpanHelpers.IndexOf<T>(span.DangerousGetPinnableReference(), span.Length, value.DangerousGetPinnableReference(), value.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf(this Span<byte> span, ReadOnlySpan<byte> value)
		{
			return SpanHelpers.IndexOf(span.DangerousGetPinnableReference(), span.Length, value.DangerousGetPinnableReference(), value.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SequenceEqual<T>(this Span<T> first, ReadOnlySpan<T> second) where T : struct, IEquatable<T>
		{
			int length = first.Length;
			return length == second.Length && SpanHelpers.SequenceEqual<T>(first.DangerousGetPinnableReference(), second.DangerousGetPinnableReference(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SequenceEqual(this Span<byte> first, ReadOnlySpan<byte> second)
		{
			int length = first.Length;
			return length == second.Length && SpanHelpers.SequenceEqual(first.DangerousGetPinnableReference(), second.DangerousGetPinnableReference(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this ReadOnlySpan<T> span, T value) where T : struct, IEquatable<T>
		{
			return SpanHelpers.IndexOf<T>(span.DangerousGetPinnableReference(), value, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf(this ReadOnlySpan<byte> span, byte value)
		{
			return SpanHelpers.IndexOf(span.DangerousGetPinnableReference(), value, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : struct, IEquatable<T>
		{
			return SpanHelpers.IndexOf<T>(span.DangerousGetPinnableReference(), span.Length, value.DangerousGetPinnableReference(), value.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOf(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> value)
		{
			return SpanHelpers.IndexOf(span.DangerousGetPinnableReference(), span.Length, value.DangerousGetPinnableReference(), value.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOfAny(this Span<byte> span, byte value0, byte value1)
		{
			return SpanHelpers.IndexOfAny(span.DangerousGetPinnableReference(), value0, value1, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOfAny(this Span<byte> span, byte value0, byte value1, byte value2)
		{
			return SpanHelpers.IndexOfAny(span.DangerousGetPinnableReference(), value0, value1, value2, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOfAny(this Span<byte> span, ReadOnlySpan<byte> values)
		{
			return SpanHelpers.IndexOfAny(span.DangerousGetPinnableReference(), span.Length, values.DangerousGetPinnableReference(), values.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOfAny(this ReadOnlySpan<byte> span, byte value0, byte value1)
		{
			return SpanHelpers.IndexOfAny(span.DangerousGetPinnableReference(), value0, value1, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOfAny(this ReadOnlySpan<byte> span, byte value0, byte value1, byte value2)
		{
			return SpanHelpers.IndexOfAny(span.DangerousGetPinnableReference(), value0, value1, value2, span.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int IndexOfAny(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> values)
		{
			return SpanHelpers.IndexOfAny(span.DangerousGetPinnableReference(), span.Length, values.DangerousGetPinnableReference(), values.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SequenceEqual<T>(this ReadOnlySpan<T> first, ReadOnlySpan<T> second) where T : struct, IEquatable<T>
		{
			int length = first.Length;
			return length == second.Length && SpanHelpers.SequenceEqual<T>(first.DangerousGetPinnableReference(), second.DangerousGetPinnableReference(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool SequenceEqual(this ReadOnlySpan<byte> first, ReadOnlySpan<byte> second)
		{
			int length = first.Length;
			return length == second.Length && SpanHelpers.SequenceEqual(first.DangerousGetPinnableReference(), second.DangerousGetPinnableReference(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool StartsWith(this Span<byte> span, ReadOnlySpan<byte> value)
		{
			int length = value.Length;
			return length <= span.Length && SpanHelpers.SequenceEqual(span.DangerousGetPinnableReference(), value.DangerousGetPinnableReference(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool StartsWith<T>(this Span<T> span, ReadOnlySpan<T> value) where T : struct, IEquatable<T>
		{
			int length = value.Length;
			return length <= span.Length && SpanHelpers.SequenceEqual<T>(span.DangerousGetPinnableReference(), value.DangerousGetPinnableReference(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool StartsWith(this ReadOnlySpan<byte> span, ReadOnlySpan<byte> value)
		{
			int length = value.Length;
			return length <= span.Length && SpanHelpers.SequenceEqual(span.DangerousGetPinnableReference(), value.DangerousGetPinnableReference(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool StartsWith<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> value) where T : struct, IEquatable<T>
		{
			int length = value.Length;
			return length <= span.Length && SpanHelpers.SequenceEqual<T>(span.DangerousGetPinnableReference(), value.DangerousGetPinnableReference(), length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this T[] array)
		{
			return new Span<T>(array);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Span<T> AsSpan<T>(this ArraySegment<T> arraySegment)
		{
			return new Span<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> AsReadOnlySpan<T>(this T[] array)
		{
			return new ReadOnlySpan<T>(array);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ReadOnlySpan<T> AsReadOnlySpan<T>(this ArraySegment<T> arraySegment)
		{
			return new ReadOnlySpan<T>(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this T[] array, Span<T> destination)
		{
			new ReadOnlySpan<T>(array).CopyTo(destination);
		}

		private static readonly IntPtr StringAdjustment = SpanExtensions.MeasureStringAdjustment();
	}
}
