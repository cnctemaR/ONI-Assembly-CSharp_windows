using System;
using System.Runtime.CompilerServices;

namespace System
{
	public readonly struct Index : IEquatable<Index>
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Index(int value, bool fromEnd = false)
		{
			if (value < 0)
			{
				ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
			}
			if (fromEnd)
			{
				this._value = ~value;
				return;
			}
			this._value = value;
		}

		private Index(int value)
		{
			this._value = value;
		}

		public static Index Start
		{
			get
			{
				return new Index(0);
			}
		}

		public static Index End
		{
			get
			{
				return new Index(-1);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Index FromStart(int value)
		{
			if (value < 0)
			{
				ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
			}
			return new Index(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Index FromEnd(int value)
		{
			if (value < 0)
			{
				ThrowHelper.ThrowValueArgumentOutOfRange_NeedNonNegNumException();
			}
			return new Index(~value);
		}

		public int Value
		{
			get
			{
				if (this._value < 0)
				{
					return ~this._value;
				}
				return this._value;
			}
		}

		public bool IsFromEnd
		{
			get
			{
				return this._value < 0;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetOffset(int length)
		{
			int num;
			if (this.IsFromEnd)
			{
				num = length - ~this._value;
			}
			else
			{
				num = this._value;
			}
			return num;
		}

		public override bool Equals(object value)
		{
			return value is Index && this._value == ((Index)value)._value;
		}

		public bool Equals(Index other)
		{
			return this._value == other._value;
		}

		public override int GetHashCode()
		{
			return this._value;
		}

		public static implicit operator Index(int value)
		{
			return Index.FromStart(value);
		}

		public override string ToString()
		{
			if (this.IsFromEnd)
			{
				return this.ToStringFromEnd();
			}
			return ((uint)this.Value).ToString();
		}

		private unsafe string ToStringFromEnd()
		{
			Span<char> span = new Span<char>(stackalloc byte[(UIntPtr)22], 11);
			int num;
			((uint)this.Value).TryFormat(span.Slice(1), out num, default(ReadOnlySpan<char>), null);
			*span[0] = '^';
			return new string(span.Slice(0, num + 1));
		}

		private readonly int _value;
	}
}
