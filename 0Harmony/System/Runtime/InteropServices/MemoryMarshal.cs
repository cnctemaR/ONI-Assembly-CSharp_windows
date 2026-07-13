using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Runtime.InteropServices
{
	internal static class MemoryMarshal
	{
		[NullableContext(2)]
		public static bool TryGetArray<T>([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlyMemory<T> memory, [Nullable(new byte[] { 0, 1 })] out ArraySegment<T> segment)
		{
			int num;
			int num2;
			object objectStartLength = memory.GetObjectStartLength(out num, out num2);
			if (num < 0)
			{
				ArraySegment<T> arraySegment;
				if (((global::System.Buffers.MemoryManager<T>)objectStartLength).TryGetArray(out arraySegment))
				{
					segment = new ArraySegment<T>(arraySegment.Array, arraySegment.Offset + (num & int.MaxValue), num2);
					return true;
				}
			}
			else
			{
				T[] array = objectStartLength as T[];
				if (array != null)
				{
					segment = new ArraySegment<T>(array, num, num2 & int.MaxValue);
					return true;
				}
			}
			if ((num2 & 2147483647) == 0)
			{
				segment = new ArraySegment<T>(SpanHelpers.PerTypeValues<T>.EmptyArray);
				return true;
			}
			segment = default(ArraySegment<T>);
			return false;
		}

		[NullableContext(2)]
		public static bool TryGetMemoryManager<T, [Nullable(0)] TManager>([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlyMemory<T> memory, out TManager manager) where TManager : global::System.Buffers.MemoryManager<T>
		{
			int num;
			int num2;
			return (manager = memory.GetObjectStartLength(out num, out num2) as TManager) != null;
		}

		[NullableContext(2)]
		public static bool TryGetMemoryManager<T, [Nullable(0)] TManager>([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlyMemory<T> memory, out TManager manager, out int start, out int length) where TManager : global::System.Buffers.MemoryManager<T>
		{
			TManager tmanager = (manager = memory.GetObjectStartLength(out start, out length) as TManager);
			start &= int.MaxValue;
			if (tmanager == null)
			{
				start = 0;
				length = 0;
				return false;
			}
			return true;
		}

		[NullableContext(1)]
		public unsafe static IEnumerable<T> ToEnumerable<[Nullable(2)] T>([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlyMemory<T> memory)
		{
			int num;
			for (int i = 0; i < memory.Length; i = num + 1)
			{
				yield return *memory.Span[i];
				num = i;
			}
			yield break;
		}

		public static bool TryGetString(global::System.ReadOnlyMemory<char> memory, [Nullable(2)] out string text, out int start, out int length)
		{
			int num;
			int num2;
			string text2 = memory.GetObjectStartLength(out num, out num2) as string;
			if (text2 != null)
			{
				text = text2;
				start = num;
				length = num2;
				return true;
			}
			text = null;
			start = 0;
			length = 0;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T Read<T>(global::System.ReadOnlySpan<byte> source) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if (Unsafe.SizeOf<T>() > source.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
			}
			return Unsafe.ReadUnaligned<T>(MemoryMarshal.GetReference<byte>(source));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryRead<T>(global::System.ReadOnlySpan<byte> source, out T value) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if ((long)Unsafe.SizeOf<T>() > (long)((ulong)source.Length))
			{
				value = default(T);
				return false;
			}
			value = Unsafe.ReadUnaligned<T>(MemoryMarshal.GetReference<byte>(source));
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Write<T>(global::System.Span<byte> destination, ref T value) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if (Unsafe.SizeOf<T>() > destination.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
			}
			Unsafe.WriteUnaligned<T>(MemoryMarshal.GetReference<byte>(destination), value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryWrite<T>(global::System.Span<byte> destination, ref T value) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			if ((long)Unsafe.SizeOf<T>() > (long)((ulong)destination.Length))
			{
				return false;
			}
			Unsafe.WriteUnaligned<T>(MemoryMarshal.GetReference<byte>(destination), value);
			return true;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: Nullable(new byte[] { 0, 1 })]
		public static global::System.Memory<T> CreateFromPinnedArray<[Nullable(2)] T>(T[] array, int start, int length)
		{
			if (array == null)
			{
				if (start != 0 || length != 0)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException();
				}
				return default(global::System.Memory<T>);
			}
			if (default(T) == null && array.GetType() != typeof(T[]))
			{
				ThrowHelper.ThrowArrayTypeMismatchException();
			}
			if (start > array.Length || length > array.Length - start)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException();
			}
			return new global::System.Memory<T>(array, start, length | int.MinValue);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static global::System.Span<byte> AsBytes<T>(global::System.Span<T> span) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			int num = checked(span.Length * Unsafe.SizeOf<T>());
			return new global::System.Span<byte>(span.Pinnable, span.ByteOffset, num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static global::System.ReadOnlySpan<byte> AsBytes<T>(global::System.ReadOnlySpan<T> span) where T : struct
		{
			if (SpanHelpers.IsReferenceOrContainsReferences<T>())
			{
				ThrowHelper.ThrowArgumentException_InvalidTypeWithPointersNotSupported(typeof(T));
			}
			int num = checked(span.Length * Unsafe.SizeOf<T>());
			return new global::System.ReadOnlySpan<byte>(span.Pinnable, span.ByteOffset, num);
		}

		[NullableContext(2)]
		[return: Nullable(new byte[] { 0, 1 })]
		public unsafe static global::System.Memory<T> AsMemory<T>([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlyMemory<T> memory)
		{
			return *Unsafe.As<global::System.ReadOnlyMemory<T>, global::System.Memory<T>>(ref memory);
		}

		[NullableContext(1)]
		public static ref T GetReference<[Nullable(2)] T>([Nullable(new byte[] { 0, 1 })] global::System.Span<T> span)
		{
			return span.DangerousGetPinnableReference();
		}

		[NullableContext(1)]
		public static ref T GetReference<[Nullable(2)] T>([Nullable(new byte[] { 0, 1 })] global::System.ReadOnlySpan<T> span)
		{
			return Unsafe.AsRef<T>(span.GetPinnableReference());
		}

		public static global::System.Span<TTo> Cast<TFrom, TTo>(global::System.Span<TFrom> span) where TFrom : struct where TTo : struct
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
				int num = (int)(unchecked((long)span.Length) * unchecked((long)Unsafe.SizeOf<TFrom>()) / unchecked((long)Unsafe.SizeOf<TTo>()));
				return new global::System.Span<TTo>(span.Pinnable, span.ByteOffset, num);
			}
		}

		public static global::System.ReadOnlySpan<TTo> Cast<TFrom, TTo>(global::System.ReadOnlySpan<TFrom> span) where TFrom : struct where TTo : struct
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
				int num = (int)(unchecked((long)span.Length) * unchecked((long)Unsafe.SizeOf<TFrom>()) / unchecked((long)Unsafe.SizeOf<TTo>()));
				return new global::System.ReadOnlySpan<TTo>(span.Pinnable, span.ByteOffset, num);
			}
		}
	}
}
