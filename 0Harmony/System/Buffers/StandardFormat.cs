using System;
using System.Runtime.CompilerServices;

namespace System.Buffers
{
	internal readonly struct StandardFormat : IEquatable<StandardFormat>
	{
		public char Symbol
		{
			get
			{
				return (char)this._format;
			}
		}

		public byte Precision
		{
			get
			{
				return this._precision;
			}
		}

		public bool HasPrecision
		{
			get
			{
				return this._precision != byte.MaxValue;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this._format == 0 && this._precision == 0;
			}
		}

		public StandardFormat(char symbol, byte precision = 255)
		{
			if (precision != 255 && precision > 99)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_PrecisionTooLarge();
			}
			if (symbol != (char)((byte)symbol))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_SymbolDoesNotFit();
			}
			this._format = (byte)symbol;
			this._precision = precision;
		}

		public static implicit operator StandardFormat(char symbol)
		{
			return new StandardFormat(symbol, byte.MaxValue);
		}

		public unsafe static StandardFormat Parse(global::System.ReadOnlySpan<char> format)
		{
			if (format.Length == 0)
			{
				return default(StandardFormat);
			}
			char c = (char)(*format[0]);
			byte b;
			if (format.Length == 1)
			{
				b = byte.MaxValue;
			}
			else
			{
				uint num = 0U;
				for (int i = 1; i < format.Length; i++)
				{
					uint num2 = (uint)(*format[i] - 48);
					if (num2 > 9U)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Cannot parse precision (max is ");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(99);
						defaultInterpolatedStringHandler.AppendLiteral(")");
						throw new FormatException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					num = num * 10U + num2;
					if (num > 99U)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Precision is larger than the maximum ");
						defaultInterpolatedStringHandler.AppendFormatted<byte>(99);
						throw new FormatException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				b = (byte)num;
			}
			return new StandardFormat(c, b);
		}

		[NullableContext(2)]
		public static StandardFormat Parse(string format)
		{
			if (format != null)
			{
				return StandardFormat.Parse(format.AsSpan());
			}
			return default(StandardFormat);
		}

		[NullableContext(2)]
		public override bool Equals(object obj)
		{
			if (obj is StandardFormat)
			{
				StandardFormat standardFormat = (StandardFormat)obj;
				return this.Equals(standardFormat);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this._format.GetHashCode() ^ this._precision.GetHashCode();
		}

		public bool Equals(StandardFormat other)
		{
			return this._format == other._format && this._precision == other._precision;
		}

		[NullableContext(1)]
		public unsafe override string ToString()
		{
			char* ptr = stackalloc char[(UIntPtr)8];
			int num = 0;
			char symbol = this.Symbol;
			if (symbol != '\0')
			{
				ptr[(IntPtr)(num++) * 2] = symbol;
				byte b = this.Precision;
				if (b != 255)
				{
					if (b >= 100)
					{
						ptr[(IntPtr)(num++) * 2] = (char)(48 + b / 100 % 10);
						b %= 100;
					}
					if (b >= 10)
					{
						ptr[(IntPtr)(num++) * 2] = (char)(48 + b / 10 % 10);
						b %= 10;
					}
					ptr[(IntPtr)(num++) * 2] = (char)(48 + b);
				}
			}
			return new string(ptr, 0, num);
		}

		public static bool operator ==(StandardFormat left, StandardFormat right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(StandardFormat left, StandardFormat right)
		{
			return !left.Equals(right);
		}

		public const byte NoPrecision = 255;

		public const byte MaxPrecision = 99;

		private readonly byte _format;

		private readonly byte _precision;
	}
}
