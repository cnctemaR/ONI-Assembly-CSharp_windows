using System;
using System.Buffers;
using System.Globalization;

namespace System.Runtime.CompilerServices
{
	[NullableContext(2)]
	[Nullable(0)]
	[InterpolatedStringHandler]
	internal ref struct DefaultInterpolatedStringHandler
	{
		public DefaultInterpolatedStringHandler(int literalLength, int formattedCount)
		{
			this._provider = null;
			this._chars = (this._arrayToReturnToPool = global::System.Buffers.ArrayPool<char>.Shared.Rent(DefaultInterpolatedStringHandler.GetDefaultLength(literalLength, formattedCount)));
			this._pos = 0;
			this._hasCustomFormatter = false;
		}

		public DefaultInterpolatedStringHandler(int literalLength, int formattedCount, IFormatProvider provider)
		{
			this._provider = provider;
			this._chars = (this._arrayToReturnToPool = global::System.Buffers.ArrayPool<char>.Shared.Rent(DefaultInterpolatedStringHandler.GetDefaultLength(literalLength, formattedCount)));
			this._pos = 0;
			this._hasCustomFormatter = provider != null && DefaultInterpolatedStringHandler.HasCustomFormatter(provider);
		}

		[NullableContext(0)]
		public DefaultInterpolatedStringHandler(int literalLength, int formattedCount, [Nullable(2)] IFormatProvider provider, global::System.Span<char> initialBuffer)
		{
			this._provider = provider;
			this._chars = initialBuffer;
			this._arrayToReturnToPool = null;
			this._pos = 0;
			this._hasCustomFormatter = provider != null && DefaultInterpolatedStringHandler.HasCustomFormatter(provider);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetDefaultLength(int literalLength, int formattedCount)
		{
			return Math.Max(256, literalLength + formattedCount * 11);
		}

		[NullableContext(1)]
		public override string ToString()
		{
			return this.Text.ToString();
		}

		[NullableContext(1)]
		public string ToStringAndClear()
		{
			string text = this.Text.ToString();
			this.Clear();
			return text;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Clear()
		{
			char[] arrayToReturnToPool = this._arrayToReturnToPool;
			this = default(DefaultInterpolatedStringHandler);
			if (arrayToReturnToPool != null)
			{
				global::System.Buffers.ArrayPool<char>.Shared.Return(arrayToReturnToPool, false);
			}
		}

		[Nullable(0)]
		internal global::System.ReadOnlySpan<char> Text
		{
			[NullableContext(0)]
			get
			{
				return this._chars.Slice(0, this._pos);
			}
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void AppendLiteral(string value)
		{
			if (value.Length == 1)
			{
				global::System.Span<char> chars = this._chars;
				int pos = this._pos;
				if (pos < chars.Length)
				{
					*chars[pos] = value[0];
					this._pos = pos + 1;
					return;
				}
				this.GrowThenCopyString(value);
				return;
			}
			else
			{
				if (value.Length != 2)
				{
					this.AppendStringDirect(value);
					return;
				}
				global::System.Span<char> chars2 = this._chars;
				int pos2 = this._pos;
				if ((ulong)pos2 < (ulong)((long)(chars2.Length - 1)))
				{
					value.AsSpan().CopyTo(chars2.Slice(pos2));
					this._pos = pos2 + 2;
					return;
				}
				this.GrowThenCopyString(value);
				return;
			}
		}

		[NullableContext(1)]
		private void AppendStringDirect(string value)
		{
			if (value.AsSpan().TryCopyTo(this._chars.Slice(this._pos)))
			{
				this._pos += value.Length;
				return;
			}
			this.GrowThenCopyString(value);
		}

		[NullableContext(1)]
		public unsafe void AppendFormatted<[Nullable(2)] T>(T value)
		{
			if (this._hasCustomFormatter)
			{
				this.AppendCustomFormatter<T>(value, null);
				return;
			}
			if (typeof(T) == typeof(IntPtr))
			{
				this.AppendFormatted(*Unsafe.As<T, IntPtr>(ref value));
				return;
			}
			if (typeof(T) == typeof(UIntPtr))
			{
				this.AppendFormatted(*Unsafe.As<T, UIntPtr>(ref value));
				return;
			}
			string text;
			if (value is IFormattable)
			{
				text = ((IFormattable)((object)value)).ToString(null, this._provider);
			}
			else
			{
				ref T ptr = ref value;
				T t = default(T);
				string text2;
				if (t == null)
				{
					t = value;
					ptr = ref t;
					if (t == null)
					{
						text2 = null;
						goto IL_00BD;
					}
				}
				text2 = ptr.ToString();
				IL_00BD:
				text = text2;
			}
			if (text != null)
			{
				this.AppendStringDirect(text);
			}
		}

		public unsafe void AppendFormatted<T>([Nullable(1)] T value, string format)
		{
			if (this._hasCustomFormatter)
			{
				this.AppendCustomFormatter<T>(value, format);
				return;
			}
			if (typeof(T) == typeof(IntPtr))
			{
				this.AppendFormatted(*Unsafe.As<T, IntPtr>(ref value), format);
				return;
			}
			if (typeof(T) == typeof(UIntPtr))
			{
				this.AppendFormatted(*Unsafe.As<T, UIntPtr>(ref value), format);
				return;
			}
			string text;
			if (value is IFormattable)
			{
				text = ((IFormattable)((object)value)).ToString(format, this._provider);
			}
			else
			{
				ref T ptr = ref value;
				T t = default(T);
				string text2;
				if (t == null)
				{
					t = value;
					ptr = ref t;
					if (t == null)
					{
						text2 = null;
						goto IL_00BF;
					}
				}
				text2 = ptr.ToString();
				IL_00BF:
				text = text2;
			}
			if (text != null)
			{
				this.AppendStringDirect(text);
			}
		}

		[NullableContext(1)]
		public void AppendFormatted<[Nullable(2)] T>(T value, int alignment)
		{
			int pos = this._pos;
			this.AppendFormatted<T>(value);
			if (alignment != 0)
			{
				this.AppendOrInsertAlignmentIfNeeded(pos, alignment);
			}
		}

		public void AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
		{
			int pos = this._pos;
			this.AppendFormatted<T>(value, format);
			if (alignment != 0)
			{
				this.AppendOrInsertAlignmentIfNeeded(pos, alignment);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendFormatted(IntPtr value)
		{
			if (IntPtr.Size == 4)
			{
				this.AppendFormatted<int>((int)value);
				return;
			}
			this.AppendFormatted<long>((long)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendFormatted(IntPtr value, string format)
		{
			if (IntPtr.Size == 4)
			{
				this.AppendFormatted<int>((int)value, format);
				return;
			}
			this.AppendFormatted<long>((long)value, format);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendFormatted(UIntPtr value)
		{
			if (UIntPtr.Size == 4)
			{
				this.AppendFormatted<uint>((uint)value);
				return;
			}
			this.AppendFormatted<ulong>((ulong)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AppendFormatted(UIntPtr value, string format)
		{
			if (UIntPtr.Size == 4)
			{
				this.AppendFormatted<uint>((uint)value, format);
				return;
			}
			this.AppendFormatted<ulong>((ulong)value, format);
		}

		[NullableContext(0)]
		public void AppendFormatted(global::System.ReadOnlySpan<char> value)
		{
			if (value.TryCopyTo(this._chars.Slice(this._pos)))
			{
				this._pos += value.Length;
				return;
			}
			this.GrowThenCopySpan(value);
		}

		[NullableContext(0)]
		public void AppendFormatted(global::System.ReadOnlySpan<char> value, int alignment = 0, [Nullable(2)] string format = null)
		{
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num = alignment - value.Length;
			if (num <= 0)
			{
				this.AppendFormatted(value);
				return;
			}
			this.EnsureCapacityForAdditionalChars(value.Length + num);
			if (flag)
			{
				value.CopyTo(this._chars.Slice(this._pos));
				this._pos += value.Length;
				this._chars.Slice(this._pos, num).Fill(' ');
				this._pos += num;
				return;
			}
			this._chars.Slice(this._pos, num).Fill(' ');
			this._pos += num;
			value.CopyTo(this._chars.Slice(this._pos));
			this._pos += value.Length;
		}

		public void AppendFormatted(string value)
		{
			if (!this._hasCustomFormatter && value != null && value.AsSpan().TryCopyTo(this._chars.Slice(this._pos)))
			{
				this._pos += value.Length;
				return;
			}
			this.AppendFormattedSlow(value);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AppendFormattedSlow(string value)
		{
			if (this._hasCustomFormatter)
			{
				this.AppendCustomFormatter<string>(value, null);
				return;
			}
			if (value != null)
			{
				this.EnsureCapacityForAdditionalChars(value.Length);
				value.AsSpan().CopyTo(this._chars.Slice(this._pos));
				this._pos += value.Length;
			}
		}

		public void AppendFormatted(string value, int alignment = 0, string format = null)
		{
			this.AppendFormatted<string>(value, alignment, format);
		}

		public void AppendFormatted(object value, int alignment = 0, string format = null)
		{
			this.AppendFormatted<object>(value, alignment, format);
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool HasCustomFormatter(IFormatProvider provider)
		{
			return provider.GetType() != typeof(CultureInfo) && provider.GetFormat(typeof(ICustomFormatter)) != null;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AppendCustomFormatter<T>([Nullable(1)] T value, string format)
		{
			ICustomFormatter customFormatter = (ICustomFormatter)this._provider.GetFormat(typeof(ICustomFormatter));
			if (customFormatter != null)
			{
				string text = customFormatter.Format(format, value, this._provider);
				if (text != null)
				{
					this.AppendStringDirect(text);
				}
			}
		}

		private void AppendOrInsertAlignmentIfNeeded(int startingPos, int alignment)
		{
			int num = this._pos - startingPos;
			bool flag = false;
			if (alignment < 0)
			{
				flag = true;
				alignment = -alignment;
			}
			int num2 = alignment - num;
			if (num2 > 0)
			{
				this.EnsureCapacityForAdditionalChars(num2);
				if (flag)
				{
					this._chars.Slice(this._pos, num2).Fill(' ');
				}
				else
				{
					this._chars.Slice(startingPos, num).CopyTo(this._chars.Slice(startingPos + num2));
					this._chars.Slice(startingPos, num2).Fill(' ');
				}
				this._pos += num2;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForAdditionalChars(int additionalChars)
		{
			if (this._chars.Length - this._pos < additionalChars)
			{
				this.Grow(additionalChars);
			}
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void GrowThenCopyString(string value)
		{
			this.Grow(value.Length);
			value.AsSpan().CopyTo(this._chars.Slice(this._pos));
			this._pos += value.Length;
		}

		[NullableContext(0)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void GrowThenCopySpan(global::System.ReadOnlySpan<char> value)
		{
			this.Grow(value.Length);
			value.CopyTo(this._chars.Slice(this._pos));
			this._pos += value.Length;
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Grow(int additionalChars)
		{
			this.GrowCore((uint)(this._pos + additionalChars));
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private void Grow()
		{
			this.GrowCore((uint)(this._chars.Length + 1));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void GrowCore(uint requiredMinCapacity)
		{
			int num = (int)MathEx.Clamp(Math.Max(requiredMinCapacity, Math.Min((uint)(this._chars.Length * 2), uint.MaxValue)), 256U, 2147483647U);
			char[] array = global::System.Buffers.ArrayPool<char>.Shared.Rent(num);
			this._chars.Slice(0, this._pos).CopyTo(array);
			char[] arrayToReturnToPool = this._arrayToReturnToPool;
			this._chars = (this._arrayToReturnToPool = array);
			if (arrayToReturnToPool != null)
			{
				global::System.Buffers.ArrayPool<char>.Shared.Return(arrayToReturnToPool, false);
			}
		}

		private const int GuessedLengthPerHole = 11;

		private const int MinimumArrayPoolLength = 256;

		private readonly IFormatProvider _provider;

		private char[] _arrayToReturnToPool;

		[Nullable(0)]
		private global::System.Span<char> _chars;

		private int _pos;

		private readonly bool _hasCustomFormatter;
	}
}
