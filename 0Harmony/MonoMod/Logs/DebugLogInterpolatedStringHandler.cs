using System;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace MonoMod.Logs
{
	[InterpolatedStringHandler]
	internal ref struct DebugLogInterpolatedStringHandler
	{
		public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, bool enabled, bool recordHoles, out bool isEnabled)
		{
			this._pos = (this.holeBegin = (this.holePos = 0));
			isEnabled = enabled;
			this.enabled = enabled;
			if (!enabled)
			{
				this._chars = (this._arrayToReturnToPool = null);
				this.holes = default(global::System.Memory<MessageHole>);
				return;
			}
			this._chars = (this._arrayToReturnToPool = global::System.Buffers.ArrayPool<char>.Shared.Rent(DebugLogInterpolatedStringHandler.GetDefaultLength(literalLength, formattedCount)));
			if (recordHoles)
			{
				this.holes = new MessageHole[formattedCount];
				return;
			}
			this.holes = default(global::System.Memory<MessageHole>);
		}

		public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, out bool isEnabled)
		{
			DebugLog instance = DebugLog.Instance;
			this._pos = (this.holeBegin = (this.holePos = 0));
			if (!instance.ShouldLog)
			{
				this.enabled = (isEnabled = false);
				this._chars = (this._arrayToReturnToPool = null);
				this.holes = default(global::System.Memory<MessageHole>);
				return;
			}
			this.enabled = (isEnabled = true);
			this._chars = (this._arrayToReturnToPool = global::System.Buffers.ArrayPool<char>.Shared.Rent(DebugLogInterpolatedStringHandler.GetDefaultLength(literalLength, formattedCount)));
			if (instance.RecordHoles)
			{
				this.holes = new MessageHole[formattedCount];
				return;
			}
			this.holes = default(global::System.Memory<MessageHole>);
		}

		public DebugLogInterpolatedStringHandler(int literalLength, int formattedCount, LogLevel level, out bool isEnabled)
		{
			DebugLog instance = DebugLog.Instance;
			this._pos = (this.holeBegin = (this.holePos = 0));
			if (!instance.ShouldLogLevel(level))
			{
				this.enabled = (isEnabled = false);
				this._chars = (this._arrayToReturnToPool = null);
				this.holes = default(global::System.Memory<MessageHole>);
				return;
			}
			this.enabled = (isEnabled = true);
			this._chars = (this._arrayToReturnToPool = global::System.Buffers.ArrayPool<char>.Shared.Rent(DebugLogInterpolatedStringHandler.GetDefaultLength(literalLength, formattedCount)));
			if (instance.ShouldLevelRecordHoles(level))
			{
				this.holes = new MessageHole[formattedCount];
				return;
			}
			this.holes = default(global::System.Memory<MessageHole>);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetDefaultLength(int literalLength, int formattedCount)
		{
			return Math.Max(256, literalLength + formattedCount * 11);
		}

		internal global::System.ReadOnlySpan<char> Text
		{
			get
			{
				return this._chars.Slice(0, this._pos);
			}
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

		[return: Nullable(1)]
		internal string ToStringAndClear(out global::System.ReadOnlyMemory<MessageHole> holes)
		{
			holes = this.holes;
			return this.ToStringAndClear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Clear()
		{
			char[] arrayToReturnToPool = this._arrayToReturnToPool;
			this = default(DebugLogInterpolatedStringHandler);
			if (arrayToReturnToPool != null)
			{
				global::System.Buffers.ArrayPool<char>.Shared.Return(arrayToReturnToPool, false);
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void BeginHole()
		{
			this.holeBegin = this._pos;
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EndHole(object obj, bool reprd)
		{
			this.EndHole<object>(in obj, reprd);
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe void EndHole<[Nullable(2)] T>(in T obj, bool reprd)
		{
			if (!this.holes.IsEmpty)
			{
				global::System.Span<MessageHole> span = this.holes.Span;
				int num = this.holePos;
				this.holePos = num + 1;
				*span[num] = (reprd ? new MessageHole(this.holeBegin, this._pos, obj) : new MessageHole(this.holeBegin, this._pos));
			}
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(string value)
		{
			this.BeginHole();
			if (value != null && value.AsSpan().TryCopyTo(this._chars.Slice(this._pos)))
			{
				this._pos += value.Length;
			}
			else
			{
				this.AppendFormattedSlow(value);
			}
			this.EndHole<string>(in value, true);
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private void AppendFormattedSlow(string value)
		{
			if (value != null)
			{
				this.EnsureCapacityForAdditionalChars(value.Length);
				value.AsSpan().CopyTo(this._chars.Slice(this._pos));
				this._pos += value.Length;
			}
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted(string value, int alignment = 0, string format = null)
		{
			this.AppendFormatted<string>(value, alignment, format);
		}

		public void AppendFormatted(global::System.ReadOnlySpan<char> value)
		{
			this.BeginHole();
			if (value.TryCopyTo(this._chars.Slice(this._pos)))
			{
				this._pos += value.Length;
			}
			else
			{
				this.GrowThenCopySpan(value);
			}
			this.EndHole(null, false);
		}

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
			this.BeginHole();
			this.EnsureCapacityForAdditionalChars(value.Length + num);
			if (flag)
			{
				value.CopyTo(this._chars.Slice(this._pos));
				this._pos += value.Length;
				this._chars.Slice(this._pos, num).Fill(' ');
				this._pos += num;
			}
			else
			{
				this._chars.Slice(this._pos, num).Fill(' ');
				this._pos += num;
				value.CopyTo(this._chars.Slice(this._pos));
				this._pos += value.Length;
			}
			this.EndHole(null, false);
		}

		[NullableContext(1)]
		public unsafe void AppendFormatted<[Nullable(2)] T>(T value)
		{
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
			this.BeginHole();
			object obj;
			if (DebugFormatter.CanDebugFormat<T>(in value, out obj))
			{
				int num;
				while (!DebugFormatter.TryFormatInto<T>(in value, obj, this._chars.Slice(this._pos), out num))
				{
					this.Grow();
				}
				this._pos += num;
				return;
			}
			string text;
			if (value is IFormattable)
			{
				text = ((IFormattable)((object)value)).ToString(null, null);
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
						goto IL_00EC;
					}
				}
				text2 = ptr.ToString();
				IL_00EC:
				text = text2;
			}
			if (text != null)
			{
				this.AppendStringDirect(text);
			}
			this.EndHole<T>(in value, true);
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

		[NullableContext(2)]
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

		[NullableContext(2)]
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

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<[Nullable(2)] T>(T value, int alignment)
		{
			int pos = this._pos;
			this.AppendFormatted<T>(value);
			if (alignment != 0)
			{
				this.AppendOrInsertAlignmentIfNeeded(pos, alignment);
			}
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void AppendFormatted<T>([Nullable(1)] T value, string format)
		{
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
			this.BeginHole();
			object obj;
			if (DebugFormatter.CanDebugFormat<T>(in value, out obj))
			{
				int num;
				while (!DebugFormatter.TryFormatInto<T>(in value, obj, this._chars.Slice(this._pos), out num))
				{
					this.Grow();
				}
				this._pos += num;
				return;
			}
			string text;
			if (value is IFormattable)
			{
				text = ((IFormattable)((object)value)).ToString(format, null);
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
						goto IL_00EE;
					}
				}
				text2 = ptr.ToString();
				IL_00EE:
				text = text2;
			}
			if (text != null)
			{
				this.AppendStringDirect(text);
			}
			this.EndHole<T>(in value, true);
		}

		[NullableContext(2)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void AppendFormatted<T>([Nullable(1)] T value, int alignment, string format)
		{
			int pos = this._pos;
			this.AppendFormatted<T>(value, format);
			if (alignment != 0)
			{
				this.AppendOrInsertAlignmentIfNeeded(pos, alignment);
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

		[Nullable(2)]
		private char[] _arrayToReturnToPool;

		private global::System.Span<char> _chars;

		private int _pos;

		private int holeBegin;

		private int holePos;

		private global::System.Memory<MessageHole> holes;

		internal readonly bool enabled;
	}
}
