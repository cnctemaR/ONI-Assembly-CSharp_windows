using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace System
{
	[NullableContext(1)]
	[Nullable(0)]
	internal static class ThrowHelper
	{
		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ThrowIfArgumentNull([<1c2fb156-e9ba-45cc-af54-d5335bdb59af>NotNull] object obj, ExceptionArgument argument)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(argument);
			}
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ThrowIfArgumentNull([<1c2fb156-e9ba-45cc-af54-d5335bdb59af>NotNull] object obj, [Nullable(1)] string argument, string message = null)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(argument, message);
			}
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentNullException(ExceptionArgument argument)
		{
			throw ThrowHelper.CreateArgumentNullException(argument);
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentNullException(string argument, [Nullable(2)] string message = null)
		{
			throw ThrowHelper.CreateArgumentNullException(argument, message);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentNullException(ExceptionArgument argument)
		{
			return ThrowHelper.CreateArgumentNullException(argument.ToString(), null);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentNullException(string argument, [Nullable(2)] string message = null)
		{
			return new ArgumentNullException(argument, message);
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArrayTypeMismatchException()
		{
			throw ThrowHelper.CreateArrayTypeMismatchException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArrayTypeMismatchException()
		{
			return new ArrayTypeMismatchException();
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentException_InvalidTypeWithPointersNotSupported(Type type)
		{
			throw ThrowHelper.CreateArgumentException_InvalidTypeWithPointersNotSupported(type);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_InvalidTypeWithPointersNotSupported(Type type)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Type ");
			defaultInterpolatedStringHandler.AppendFormatted<Type>(type);
			defaultInterpolatedStringHandler.AppendLiteral(" with managed pointers cannot be used in a Span");
			return new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentException_DestinationTooShort()
		{
			throw ThrowHelper.CreateArgumentException_DestinationTooShort();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_DestinationTooShort()
		{
			return new ArgumentException("Destination too short");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentException(string message, [Nullable(2)] string argument = null)
		{
			throw ThrowHelper.CreateArgumentException(message, argument);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException(string message, [Nullable(2)] string argument)
		{
			return new ArgumentException(message, argument ?? "");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowIndexOutOfRangeException()
		{
			throw ThrowHelper.CreateIndexOutOfRangeException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateIndexOutOfRangeException()
		{
			return new IndexOutOfRangeException();
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentOutOfRangeException()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException()
		{
			return new ArgumentOutOfRangeException();
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException(argument);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException(ExceptionArgument argument)
		{
			return new ArgumentOutOfRangeException(argument.ToString());
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentOutOfRangeException_PrecisionTooLarge()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_PrecisionTooLarge();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_PrecisionTooLarge()
		{
			string text = "precision";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Precision too large (max: ");
			defaultInterpolatedStringHandler.AppendFormatted<byte>(99);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			return new ArgumentOutOfRangeException(text, defaultInterpolatedStringHandler.ToStringAndClear());
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentOutOfRangeException_SymbolDoesNotFit()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_SymbolDoesNotFit();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_SymbolDoesNotFit()
		{
			return new ArgumentOutOfRangeException("symbol", "Bad format specifier");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowInvalidOperationException()
		{
			throw ThrowHelper.CreateInvalidOperationException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException()
		{
			return new InvalidOperationException();
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowInvalidOperationException_OutstandingReferences()
		{
			throw ThrowHelper.CreateInvalidOperationException_OutstandingReferences();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_OutstandingReferences()
		{
			return new InvalidOperationException("Outstanding references");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowInvalidOperationException_UnexpectedSegmentType()
		{
			throw ThrowHelper.CreateInvalidOperationException_UnexpectedSegmentType();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_UnexpectedSegmentType()
		{
			return new InvalidOperationException("Unexpected segment type");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowInvalidOperationException_EndPositionNotReached()
		{
			throw ThrowHelper.CreateInvalidOperationException_EndPositionNotReached();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateInvalidOperationException_EndPositionNotReached()
		{
			return new InvalidOperationException("End position not reached");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentOutOfRangeException_PositionOutOfRange()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_PositionOutOfRange();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_PositionOutOfRange()
		{
			return new ArgumentOutOfRangeException("position");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentOutOfRangeException_OffsetOutOfRange()
		{
			throw ThrowHelper.CreateArgumentOutOfRangeException_OffsetOutOfRange();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentOutOfRangeException_OffsetOutOfRange()
		{
			return new ArgumentOutOfRangeException("offset");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowObjectDisposedException_ArrayMemoryPoolBuffer()
		{
			throw ThrowHelper.CreateObjectDisposedException_ArrayMemoryPoolBuffer();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateObjectDisposedException_ArrayMemoryPoolBuffer()
		{
			return new ObjectDisposedException("ArrayMemoryPoolBuffer");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowFormatException_BadFormatSpecifier()
		{
			throw ThrowHelper.CreateFormatException_BadFormatSpecifier();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateFormatException_BadFormatSpecifier()
		{
			return new FormatException("Bad format specifier");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentException_OverlapAlignmentMismatch()
		{
			throw ThrowHelper.CreateArgumentException_OverlapAlignmentMismatch();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateArgumentException_OverlapAlignmentMismatch()
		{
			return new ArgumentException("Overlap alignment mismatch");
		}

		[NullableContext(2)]
		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowNotSupportedException(string msg = null)
		{
			throw ThrowHelper.CreateThrowNotSupportedException(msg);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateThrowNotSupportedException([Nullable(2)] string msg)
		{
			return new NotSupportedException();
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowKeyNullException()
		{
			ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowValueNullException()
		{
			throw ThrowHelper.CreateThrowValueNullException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateThrowValueNullException()
		{
			return new ArgumentException("Value is null");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowOutOfMemoryException()
		{
			throw ThrowHelper.CreateOutOfMemoryException();
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static Exception CreateOutOfMemoryException()
		{
			return new OutOfMemoryException();
		}

		public static bool TryFormatThrowFormatException(out int bytesWritten)
		{
			bytesWritten = 0;
			ThrowHelper.ThrowFormatException_BadFormatSpecifier();
			return false;
		}

		public static bool TryParseThrowFormatException<[Nullable(2)] T>(out T value, out int bytesConsumed)
		{
			value = default(T);
			bytesConsumed = 0;
			ThrowHelper.ThrowFormatException_BadFormatSpecifier();
			return false;
		}

		[NullableContext(2)]
		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		public static void ThrowArgumentValidationException<T>([Nullable(new byte[] { 2, 1 })] global::System.Buffers.ReadOnlySequenceSegment<T> startSegment, int startIndex, [Nullable(new byte[] { 2, 1 })] global::System.Buffers.ReadOnlySequenceSegment<T> endSegment)
		{
			throw ThrowHelper.CreateArgumentValidationException<T>(startSegment, startIndex, endSegment);
		}

		private static Exception CreateArgumentValidationException<[Nullable(2)] T>([Nullable(new byte[] { 2, 1 })] global::System.Buffers.ReadOnlySequenceSegment<T> startSegment, int startIndex, [Nullable(new byte[] { 2, 1 })] global::System.Buffers.ReadOnlySequenceSegment<T> endSegment)
		{
			if (startSegment == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.startSegment);
			}
			if (endSegment == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.endSegment);
			}
			if (startSegment != endSegment && startSegment.RunningIndex > endSegment.RunningIndex)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.endSegment);
			}
			if (startSegment.Memory.Length < startIndex)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.startIndex);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.endIndex);
		}

		[NullableContext(2)]
		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		public static void ThrowArgumentValidationException(Array array, int start)
		{
			throw ThrowHelper.CreateArgumentValidationException(array, start);
		}

		private static Exception CreateArgumentValidationException([Nullable(2)] Array array, int start)
		{
			if (array == null)
			{
				return ThrowHelper.CreateArgumentNullException(ExceptionArgument.array);
			}
			if (start > array.Length)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.length);
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		internal static void ThrowArgumentException_TupleIncorrectType(object other)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Value tuple of incorrect type (found ");
			defaultInterpolatedStringHandler.AppendFormatted<Type>(other.GetType());
			defaultInterpolatedStringHandler.AppendLiteral(")");
			throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear(), "other");
		}

		[<1c2fb156-e9ba-45cc-af54-d5335bdb59af>DoesNotReturn]
		public static void ThrowStartOrEndArgumentValidationException(long start)
		{
			throw ThrowHelper.CreateStartOrEndArgumentValidationException(start);
		}

		private static Exception CreateStartOrEndArgumentValidationException(long start)
		{
			if (start < 0L)
			{
				return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.start);
			}
			return ThrowHelper.CreateArgumentOutOfRangeException(ExceptionArgument.length);
		}
	}
}
