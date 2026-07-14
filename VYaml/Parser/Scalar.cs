using System;
using System.Buffers;
using System.Buffers.Text;
using System.Runtime.CompilerServices;
using VYaml.Internal;

namespace VYaml.Parser
{
	internal class Scalar : ITokenContent
	{
		public int Length { get; private set; }

		public TokenType Type { get; set; }

		public Scalar(int capacity)
		{
			this.buffer = new byte[capacity];
		}

		public Scalar(ReadOnlySpan<byte> content)
		{
			this.buffer = new byte[content.Length];
			this.Write(content);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<byte> AsSpan()
		{
			return this.buffer.AsSpan<byte>(0, this.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<byte> AsSpan(int start, int length)
		{
			return this.buffer.AsSpan<byte>(start, length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<byte> AsUtf8()
		{
			return this.buffer.AsSpan<byte>(0, this.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(byte code)
		{
			if (this.Length == this.buffer.Length)
			{
				this.Grow();
			}
			byte[] array = this.buffer;
			int length = this.Length;
			this.Length = length + 1;
			array[length] = code;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(LineBreakState lineBreak)
		{
			switch (lineBreak)
			{
			case LineBreakState.None:
				return;
			case LineBreakState.Lf:
				this.Write(10);
				return;
			case LineBreakState.CrLf:
				this.Write(13);
				this.Write(10);
				return;
			case LineBreakState.Cr:
				this.Write(13);
				return;
			default:
				throw new ArgumentOutOfRangeException("lineBreak", lineBreak, null);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Write(ReadOnlySpan<byte> codes)
		{
			this.Grow(this.Length + codes.Length);
			codes.CopyTo(this.buffer.AsSpan<byte>(this.Length, codes.Length));
			this.Length += codes.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe void WriteUnicodeCodepoint(int codepoint)
		{
			IntPtr intPtr = stackalloc byte[(UIntPtr)2];
			*intPtr = (short)((ushort)codepoint);
			Span<char> span = new Span<char>(intPtr, 1);
			int byteCount = StringEncoding.Utf8.GetByteCount(span);
			Span<byte> span2 = new Span<byte>(stackalloc byte[(UIntPtr)byteCount], byteCount);
			StringEncoding.Utf8.GetBytes(span, span2);
			this.Write(span2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			this.Length = 0;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString()
		{
			return StringEncoding.Utf8.GetString(this.AsSpan());
		}

		public unsafe bool IsNull()
		{
			if (this.IsStringScalar())
			{
				return false;
			}
			Span<byte> span = this.AsSpan();
			switch (span.Length)
			{
			case 0:
				break;
			case 1:
				if (*span[0] != 126)
				{
					return false;
				}
				break;
			case 2:
			case 3:
				return false;
			case 4:
				if (!span.SequenceEqual<byte>(YamlCodes.Null0) && !span.SequenceEqual<byte>(YamlCodes.Null1) && !span.SequenceEqual<byte>(YamlCodes.Null2))
				{
					return false;
				}
				break;
			default:
				return false;
			}
			return true;
		}

		public bool TryGetBool(out bool value)
		{
			if (this.IsStringScalar())
			{
				value = false;
				return false;
			}
			Span<byte> span = this.AsSpan();
			int length = span.Length;
			if (length != 4)
			{
				if (length == 5)
				{
					if (span.SequenceEqual<byte>(YamlCodes.False0) || span.SequenceEqual<byte>(YamlCodes.False1) || span.SequenceEqual<byte>(YamlCodes.False2))
					{
						value = false;
						return true;
					}
				}
			}
			else if (span.SequenceEqual<byte>(YamlCodes.True0) || span.SequenceEqual<byte>(YamlCodes.True1) || span.SequenceEqual<byte>(YamlCodes.True2))
			{
				value = true;
				return true;
			}
			value = false;
			return false;
		}

		public bool TryGetInt32(out int value)
		{
			if (this.IsStringScalar())
			{
				value = 0;
				return false;
			}
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			ReadOnlySpan<byte> readOnlySpan;
			if (Scalar.TryDetectHex(span, out readOnlySpan))
			{
				return Utf8Parser.TryParse(readOnlySpan, out value, out num, 'x') && num == readOnlySpan.Length;
			}
			if (Scalar.TryDetectHexNegative(span, out readOnlySpan) && Utf8Parser.TryParse(readOnlySpan, out value, out num, 'x') && num == readOnlySpan.Length)
			{
				value *= -1;
				return true;
			}
			ulong num2;
			if (Scalar.TryParseOctal(span, out num2) && num2 <= 2147483647UL)
			{
				value = (int)num2;
				return true;
			}
			return false;
		}

		public bool TryGetInt64(out long value)
		{
			if (this.IsStringScalar())
			{
				value = 0L;
				return false;
			}
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			if (span.Length > YamlCodes.HexPrefix.Length && span.StartsWith<byte>(YamlCodes.HexPrefix))
			{
				ref Span<byte> ptr = ref span;
				int num2 = YamlCodes.HexPrefix.Length;
				Span<byte> span2 = ptr.Slice(num2, ptr.Length - num2);
				int num3;
				return Utf8Parser.TryParse(span2, out value, out num3, 'x') && num3 == span2.Length;
			}
			if (span.Length > YamlCodes.HexPrefixNegative.Length && span.StartsWith<byte>(YamlCodes.HexPrefixNegative))
			{
				ref Span<byte> ptr = ref span;
				int num2 = YamlCodes.HexPrefixNegative.Length;
				Span<byte> span3 = ptr.Slice(num2, ptr.Length - num2);
				int num4;
				if (Utf8Parser.TryParse(span3, out value, out num4, 'x') && num4 == span3.Length)
				{
					value = -value;
					return true;
				}
			}
			ulong num5;
			if (Scalar.TryParseOctal(span, out num5) && num5 <= 9223372036854775807UL)
			{
				value = (long)num5;
				return true;
			}
			return false;
		}

		public bool TryGetUInt32(out uint value)
		{
			if (this.IsStringScalar())
			{
				value = 0U;
				return false;
			}
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			ReadOnlySpan<byte> readOnlySpan;
			if (Scalar.TryDetectHex(span, out readOnlySpan))
			{
				return Utf8Parser.TryParse(readOnlySpan, out value, out num, 'x') && num == readOnlySpan.Length;
			}
			ulong num2;
			if (Scalar.TryParseOctal(span, out num2) && num2 <= (ulong)(-1))
			{
				value = (uint)num2;
				return true;
			}
			return false;
		}

		public bool TryGetUInt64(out ulong value)
		{
			if (this.IsStringScalar())
			{
				value = 0UL;
				return false;
			}
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			ReadOnlySpan<byte> readOnlySpan;
			if (Scalar.TryDetectHex(span, out readOnlySpan))
			{
				return Utf8Parser.TryParse(readOnlySpan, out value, out num, 'x') && num == readOnlySpan.Length;
			}
			return Scalar.TryParseOctal(span, out value);
		}

		public bool TryGetFloat(out float value)
		{
			if (this.IsStringScalar())
			{
				value = 0f;
				return false;
			}
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			int length = span.Length;
			if (length != 4)
			{
				if (length == 5)
				{
					if (span.SequenceEqual<byte>(YamlCodes.Inf3) || span.SequenceEqual<byte>(YamlCodes.Inf4) || span.SequenceEqual<byte>(YamlCodes.Inf5))
					{
						value = float.PositiveInfinity;
						return true;
					}
					if (span.SequenceEqual<byte>(YamlCodes.NegInf0) || span.SequenceEqual<byte>(YamlCodes.NegInf1) || span.SequenceEqual<byte>(YamlCodes.NegInf2))
					{
						value = float.NegativeInfinity;
						return true;
					}
				}
			}
			else
			{
				if (span.SequenceEqual<byte>(YamlCodes.Inf0) || span.SequenceEqual<byte>(YamlCodes.Inf1) || span.SequenceEqual<byte>(YamlCodes.Inf2))
				{
					value = float.PositiveInfinity;
					return true;
				}
				if (span.SequenceEqual<byte>(YamlCodes.Nan0) || span.SequenceEqual<byte>(YamlCodes.Nan1) || span.SequenceEqual<byte>(YamlCodes.Nan2))
				{
					value = float.NaN;
					return true;
				}
			}
			return false;
		}

		public bool TryGetDouble(out double value)
		{
			if (this.IsStringScalar())
			{
				value = 0.0;
				return false;
			}
			Span<byte> span = this.AsSpan();
			int num;
			if (Utf8Parser.TryParse(span, out value, out num, '\0') && num == span.Length)
			{
				return true;
			}
			int length = span.Length;
			if (length != 4)
			{
				if (length == 5)
				{
					if (span.SequenceEqual<byte>(YamlCodes.Inf3) || span.SequenceEqual<byte>(YamlCodes.Inf4) || span.SequenceEqual<byte>(YamlCodes.Inf5))
					{
						value = double.PositiveInfinity;
						return true;
					}
					if (span.SequenceEqual<byte>(YamlCodes.NegInf0) || span.SequenceEqual<byte>(YamlCodes.NegInf1) || span.SequenceEqual<byte>(YamlCodes.NegInf2))
					{
						value = double.NegativeInfinity;
						return true;
					}
				}
			}
			else
			{
				if (span.SequenceEqual<byte>(YamlCodes.Inf0) || span.SequenceEqual<byte>(YamlCodes.Inf1) || span.SequenceEqual<byte>(YamlCodes.Inf2))
				{
					value = double.PositiveInfinity;
					return true;
				}
				if (span.SequenceEqual<byte>(YamlCodes.Nan0) || span.SequenceEqual<byte>(YamlCodes.Nan1) || span.SequenceEqual<byte>(YamlCodes.Nan2))
				{
					value = double.NaN;
					return true;
				}
			}
			return false;
		}

		[NullableContext(1)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SequenceEqual(Scalar other)
		{
			return this.AsSpan().SequenceEqual<byte>(other.AsSpan());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool SequenceEqual(ReadOnlySpan<byte> span)
		{
			return this.AsSpan().SequenceEqual<byte>(span);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Grow(int sizeHint)
		{
			if (sizeHint <= this.buffer.Length)
			{
				return;
			}
			int i;
			for (i = this.buffer.Length * 200 / 100; i < sizeHint; i = i * 200 / 100)
			{
			}
			this.SetCapacity(i);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryDetectHex(ReadOnlySpan<byte> span, out ReadOnlySpan<byte> slice)
		{
			if (span.Length > YamlCodes.HexPrefix.Length && span.StartsWith<byte>(YamlCodes.HexPrefix))
			{
				ref ReadOnlySpan<byte> ptr = ref span;
				int num = YamlCodes.HexPrefix.Length;
				slice = ptr.Slice(num, ptr.Length - num);
				return true;
			}
			slice = default(ReadOnlySpan<byte>);
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool TryDetectHexNegative(ReadOnlySpan<byte> span, out ReadOnlySpan<byte> slice)
		{
			if (span.Length > YamlCodes.HexPrefixNegative.Length && span.StartsWith<byte>(YamlCodes.HexPrefixNegative))
			{
				ref ReadOnlySpan<byte> ptr = ref span;
				int num = YamlCodes.HexPrefixNegative.Length;
				slice = ptr.Slice(num, ptr.Length - num);
				return true;
			}
			slice = default(ReadOnlySpan<byte>);
			return false;
		}

		private unsafe static bool TryParseOctal(ReadOnlySpan<byte> span, out ulong value)
		{
			if (span.Length <= YamlCodes.OctalPrefix.Length || !span.StartsWith<byte>(YamlCodes.OctalPrefix))
			{
				value = 0UL;
				return false;
			}
			int num = YamlCodes.OctalPrefix.Length;
			while (num < span.Length && *span[num] == 48)
			{
				num++;
			}
			if (num >= span.Length)
			{
				value = 0UL;
				return num == span.Length;
			}
			ref ReadOnlySpan<byte> ptr = ref span;
			int num2 = num;
			ReadOnlySpan<byte> readOnlySpan = ptr.Slice(num2, ptr.Length - num2);
			int num3 = (int)(*readOnlySpan[0] - 48);
			bool flag = num3 < 0 || num3 > 7;
			if (flag || (num3 > 1 && readOnlySpan.Length == 22) || readOnlySpan.Length > 22)
			{
				value = 0UL;
				return false;
			}
			value = (ulong)((long)num3);
			for (int i = 1; i < readOnlySpan.Length; i++)
			{
				num3 = (int)(*readOnlySpan[i] - 48);
				flag = num3 < 0 || num3 > 7;
				if (flag)
				{
					value = 0UL;
					return false;
				}
				value = (value << 3) + (ulong)num3;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void Grow()
		{
			int num = this.buffer.Length * 200 / 100;
			if (num < this.buffer.Length + 4)
			{
				num = this.buffer.Length + 4;
			}
			this.SetCapacity(num);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetCapacity(int newCapacity)
		{
			if (this.buffer.Length >= newCapacity)
			{
				return;
			}
			byte[] array = ArrayPool<byte>.Shared.Rent(newCapacity);
			Array.Copy(this.buffer, 0, array, 0, this.Length);
			ArrayPool<byte>.Shared.Return(this.buffer, false);
			this.buffer = array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsStringScalar()
		{
			TokenType type = this.Type;
			return type - TokenType.SingleQuotedScaler <= 1;
		}

		private const int MinimumGrow = 4;

		private const int GrowFactor = 200;

		[Nullable(1)]
		public static readonly Scalar Null = new Scalar(0);

		[Nullable(1)]
		private byte[] buffer;
	}
}
