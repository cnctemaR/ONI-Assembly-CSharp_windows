using System;
using System.Runtime.CompilerServices;

namespace System
{
	public readonly struct Range : IEquatable<Range>
	{
		public Index Start { get; }

		public Index End { get; }

		public Range(Index start, Index end)
		{
			this.Start = start;
			this.End = end;
		}

		public override bool Equals(object value)
		{
			if (value is Range)
			{
				Range range = (Range)value;
				return range.Start.Equals(this.Start) && range.End.Equals(this.End);
			}
			return false;
		}

		public bool Equals(Range other)
		{
			return other.Start.Equals(this.Start) && other.End.Equals(this.End);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<int, int>(this.Start.GetHashCode(), this.End.GetHashCode());
		}

		public unsafe override string ToString()
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)48], 24);
			int num = 0;
			if (this.Start.IsFromEnd)
			{
				*span[0] = '^';
				num = 1;
			}
			int num2;
			((uint)this.Start.Value).TryFormat(span.Slice(num), out num2, default(ReadOnlySpan<char>), null);
			num += num2;
			*span[num++] = '.';
			*span[num++] = '.';
			if (this.End.IsFromEnd)
			{
				*span[num++] = '^';
			}
			((uint)this.End.Value).TryFormat(span.Slice(num), out num2, default(ReadOnlySpan<char>), null);
			num += num2;
			return new string(span.Slice(0, num));
		}

		public static Range StartAt(Index start)
		{
			return new Range(start, Index.End);
		}

		public static Range EndAt(Index end)
		{
			return new Range(Index.Start, end);
		}

		public static Range All
		{
			get
			{
				return new Range(Index.Start, Index.End);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[return: TupleElementNames(new string[] { "Offset", "Length" })]
		public ValueTuple<int, int> GetOffsetAndLength(int length)
		{
			Index start = this.Start;
			int num;
			if (start.IsFromEnd)
			{
				num = length - start.Value;
			}
			else
			{
				num = start.Value;
			}
			Index end = this.End;
			int num2;
			if (end.IsFromEnd)
			{
				num2 = length - end.Value;
			}
			else
			{
				num2 = end.Value;
			}
			if (num2 > length || num > num2)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.length);
			}
			return new ValueTuple<int, int>(num, num2 - num);
		}
	}
}
