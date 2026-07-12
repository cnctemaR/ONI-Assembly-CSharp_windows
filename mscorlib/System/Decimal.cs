using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit)]
	public readonly struct Decimal : IFormattable, IComparable, IConvertible, IComparable<decimal>, IEquatable<decimal>, IDeserializationCallback, ISpanFormattable
	{
		internal uint High
		{
			get
			{
				return (uint)this.hi;
			}
		}

		internal uint Low
		{
			get
			{
				return (uint)this.lo;
			}
		}

		internal uint Mid
		{
			get
			{
				return (uint)this.mid;
			}
		}

		internal bool IsNegative
		{
			get
			{
				return this.flags < 0;
			}
		}

		internal int Scale
		{
			get
			{
				return (int)((byte)(this.flags >> 16));
			}
		}

		private ulong Low64
		{
			get
			{
				if (!BitConverter.IsLittleEndian)
				{
					return ((ulong)this.Mid << 32) | (ulong)this.Low;
				}
				return this.ulomidLE;
			}
		}

		private static ref decimal.DecCalc AsMutable(ref decimal d)
		{
			return Unsafe.As<decimal, decimal.DecCalc>(ref d);
		}

		internal static uint DecDivMod1E9(ref decimal value)
		{
			return decimal.DecCalc.DecDivMod1E9(decimal.AsMutable(ref value));
		}

		public Decimal(int value)
		{
			if (value >= 0)
			{
				this.flags = 0;
			}
			else
			{
				this.flags = int.MinValue;
				value = -value;
			}
			this.lo = value;
			this.mid = 0;
			this.hi = 0;
		}

		[CLSCompliant(false)]
		public Decimal(uint value)
		{
			this.flags = 0;
			this.lo = (int)value;
			this.mid = 0;
			this.hi = 0;
		}

		public Decimal(long value)
		{
			if (value >= 0L)
			{
				this.flags = 0;
			}
			else
			{
				this.flags = int.MinValue;
				value = -value;
			}
			this.lo = (int)value;
			this.mid = (int)(value >> 32);
			this.hi = 0;
		}

		[CLSCompliant(false)]
		public Decimal(ulong value)
		{
			this.flags = 0;
			this.lo = (int)value;
			this.mid = (int)(value >> 32);
			this.hi = 0;
		}

		public Decimal(float value)
		{
			decimal.DecCalc.VarDecFromR4(value, decimal.AsMutable(ref this));
		}

		public Decimal(double value)
		{
			decimal.DecCalc.VarDecFromR8(value, decimal.AsMutable(ref this));
		}

		public static decimal FromOACurrency(long cy)
		{
			bool flag = false;
			ulong num;
			if (cy < 0L)
			{
				flag = true;
				num = (ulong)(-(ulong)cy);
			}
			else
			{
				num = (ulong)cy;
			}
			int num2 = 4;
			if (num != 0UL)
			{
				while (num2 != 0 && num % 10UL == 0UL)
				{
					num2--;
					num /= 10UL;
				}
			}
			return new decimal((int)num, (int)(num >> 32), 0, flag, (byte)num2);
		}

		public static long ToOACurrency(decimal value)
		{
			return decimal.DecCalc.VarCyFromDec(decimal.AsMutable(ref value));
		}

		private static bool IsValid(int flags)
		{
			return (flags & 2130771967) == 0 && (flags & 16711680) <= 1835008;
		}

		public Decimal(int[] bits)
		{
			if (bits == null)
			{
				throw new ArgumentNullException("bits");
			}
			if (bits.Length == 4)
			{
				int num = bits[3];
				if (decimal.IsValid(num))
				{
					this.lo = bits[0];
					this.mid = bits[1];
					this.hi = bits[2];
					this.flags = num;
					return;
				}
			}
			throw new ArgumentException("Decimal byte array constructor requires an array of length four containing valid decimal bytes.");
		}

		public Decimal(int lo, int mid, int hi, bool isNegative, byte scale)
		{
			if (scale > 28)
			{
				throw new ArgumentOutOfRangeException("scale", "Decimal's scale value must be between 0 and 28, inclusive.");
			}
			this.lo = lo;
			this.mid = mid;
			this.hi = hi;
			this.flags = (int)scale << 16;
			if (isNegative)
			{
				this.flags |= int.MinValue;
			}
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			if (!decimal.IsValid(this.flags))
			{
				throw new SerializationException("Value was either too large or too small for a Decimal.");
			}
		}

		private Decimal(int lo, int mid, int hi, int flags)
		{
			if (decimal.IsValid(flags))
			{
				this.lo = lo;
				this.mid = mid;
				this.hi = hi;
				this.flags = flags;
				return;
			}
			throw new ArgumentException("Decimal byte array constructor requires an array of length four containing valid decimal bytes.");
		}

		private Decimal(in decimal d, int flags)
		{
			this = d;
			this.flags = flags;
		}

		internal static decimal Abs(ref decimal d)
		{
			return new decimal(in d, d.flags & int.MaxValue);
		}

		public static decimal Add(decimal d1, decimal d2)
		{
			decimal.DecCalc.DecAddSub(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2), false);
			return d1;
		}

		public static decimal Ceiling(decimal d)
		{
			int num = d.flags;
			if ((num & 16711680) != 0)
			{
				decimal.DecCalc.InternalRound(decimal.AsMutable(ref d), (uint)((byte)(num >> 16)), decimal.DecCalc.RoundingMode.Ceiling);
			}
			return d;
		}

		public static int Compare(decimal d1, decimal d2)
		{
			return decimal.DecCalc.VarDecCmp(in d1, in d2);
		}

		[SecuritySafeCritical]
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is decimal))
			{
				throw new ArgumentException("Object must be of type Decimal.");
			}
			decimal num = (decimal)value;
			return decimal.DecCalc.VarDecCmp(in this, in num);
		}

		public int CompareTo(decimal value)
		{
			return decimal.DecCalc.VarDecCmp(in this, in value);
		}

		public static decimal Divide(decimal d1, decimal d2)
		{
			decimal.DecCalc.VarDecDiv(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2));
			return d1;
		}

		public override bool Equals(object value)
		{
			if (value is decimal)
			{
				decimal num = (decimal)value;
				return decimal.DecCalc.VarDecCmp(in this, in num) == 0;
			}
			return false;
		}

		public bool Equals(decimal value)
		{
			return decimal.DecCalc.VarDecCmp(in this, in value) == 0;
		}

		public override int GetHashCode()
		{
			return decimal.DecCalc.GetHashCode(in this);
		}

		public static bool Equals(decimal d1, decimal d2)
		{
			return decimal.DecCalc.VarDecCmp(in d1, in d2) == 0;
		}

		public static decimal Floor(decimal d)
		{
			int num = d.flags;
			if ((num & 16711680) != 0)
			{
				decimal.DecCalc.InternalRound(decimal.AsMutable(ref d), (uint)((byte)(num >> 16)), decimal.DecCalc.RoundingMode.Floor);
			}
			return d;
		}

		public override string ToString()
		{
			return Number.FormatDecimal(this, null, NumberFormatInfo.CurrentInfo);
		}

		public string ToString(string format)
		{
			return Number.FormatDecimal(this, format, NumberFormatInfo.CurrentInfo);
		}

		[SecuritySafeCritical]
		public string ToString(IFormatProvider provider)
		{
			return Number.FormatDecimal(this, null, NumberFormatInfo.GetInstance(provider));
		}

		[SecuritySafeCritical]
		public string ToString(string format, IFormatProvider provider)
		{
			return Number.FormatDecimal(this, format, NumberFormatInfo.GetInstance(provider));
		}

		public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format = default(ReadOnlySpan<char>), IFormatProvider provider = null)
		{
			return Number.TryFormatDecimal(this, format, NumberFormatInfo.GetInstance(provider), destination, out charsWritten);
		}

		public static decimal Parse(string s)
		{
			if (s == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
			}
			return Number.ParseDecimal(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo);
		}

		public static decimal Parse(string s, NumberStyles style)
		{
			NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
			if (s == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
			}
			return Number.ParseDecimal(s, style, NumberFormatInfo.CurrentInfo);
		}

		public static decimal Parse(string s, IFormatProvider provider)
		{
			if (s == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
			}
			return Number.ParseDecimal(s, NumberStyles.Number, NumberFormatInfo.GetInstance(provider));
		}

		public static decimal Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
			if (s == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.s);
			}
			return Number.ParseDecimal(s, style, NumberFormatInfo.GetInstance(provider));
		}

		public static decimal Parse(ReadOnlySpan<char> s, NumberStyles style = NumberStyles.Number, IFormatProvider provider = null)
		{
			NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
			return Number.ParseDecimal(s, style, NumberFormatInfo.GetInstance(provider));
		}

		public static bool TryParse(string s, out decimal result)
		{
			if (s == null)
			{
				result = 0m;
				return false;
			}
			return Number.TryParseDecimal(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out result);
		}

		public static bool TryParse(ReadOnlySpan<char> s, out decimal result)
		{
			return Number.TryParseDecimal(s, NumberStyles.Number, NumberFormatInfo.CurrentInfo, out result);
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out decimal result)
		{
			NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
			if (s == null)
			{
				result = 0m;
				return false;
			}
			return Number.TryParseDecimal(s, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider provider, out decimal result)
		{
			NumberFormatInfo.ValidateParseStyleFloatingPoint(style);
			return Number.TryParseDecimal(s, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		public static int[] GetBits(decimal d)
		{
			return new int[] { d.lo, d.mid, d.hi, d.flags };
		}

		internal static void GetBytes(in decimal d, byte[] buffer)
		{
			buffer[0] = (byte)d.lo;
			buffer[1] = (byte)(d.lo >> 8);
			buffer[2] = (byte)(d.lo >> 16);
			buffer[3] = (byte)(d.lo >> 24);
			buffer[4] = (byte)d.mid;
			buffer[5] = (byte)(d.mid >> 8);
			buffer[6] = (byte)(d.mid >> 16);
			buffer[7] = (byte)(d.mid >> 24);
			buffer[8] = (byte)d.hi;
			buffer[9] = (byte)(d.hi >> 8);
			buffer[10] = (byte)(d.hi >> 16);
			buffer[11] = (byte)(d.hi >> 24);
			buffer[12] = (byte)d.flags;
			buffer[13] = (byte)(d.flags >> 8);
			buffer[14] = (byte)(d.flags >> 16);
			buffer[15] = (byte)(d.flags >> 24);
		}

		internal static decimal ToDecimal(byte[] buffer)
		{
			int num = (int)buffer[0] | ((int)buffer[1] << 8) | ((int)buffer[2] << 16) | ((int)buffer[3] << 24);
			int num2 = (int)buffer[4] | ((int)buffer[5] << 8) | ((int)buffer[6] << 16) | ((int)buffer[7] << 24);
			int num3 = (int)buffer[8] | ((int)buffer[9] << 8) | ((int)buffer[10] << 16) | ((int)buffer[11] << 24);
			int num4 = (int)buffer[12] | ((int)buffer[13] << 8) | ((int)buffer[14] << 16) | ((int)buffer[15] << 24);
			return new decimal(num, num2, num3, num4);
		}

		internal static readonly ref decimal Max(ref decimal d1, ref decimal d2)
		{
			if (decimal.DecCalc.VarDecCmp(in d1, in d2) < 0)
			{
				return ref d2;
			}
			return ref d1;
		}

		internal static readonly ref decimal Min(ref decimal d1, ref decimal d2)
		{
			if (decimal.DecCalc.VarDecCmp(in d1, in d2) >= 0)
			{
				return ref d2;
			}
			return ref d1;
		}

		public static decimal Remainder(decimal d1, decimal d2)
		{
			decimal.DecCalc.VarDecMod(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2));
			return d1;
		}

		public static decimal Multiply(decimal d1, decimal d2)
		{
			decimal.DecCalc.VarDecMul(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2));
			return d1;
		}

		public static decimal Negate(decimal d)
		{
			return new decimal(in d, d.flags ^ int.MinValue);
		}

		public static decimal Round(decimal d)
		{
			return decimal.Round(ref d, 0, MidpointRounding.ToEven);
		}

		public static decimal Round(decimal d, int decimals)
		{
			return decimal.Round(ref d, decimals, MidpointRounding.ToEven);
		}

		public static decimal Round(decimal d, MidpointRounding mode)
		{
			return decimal.Round(ref d, 0, mode);
		}

		public static decimal Round(decimal d, int decimals, MidpointRounding mode)
		{
			return decimal.Round(ref d, decimals, mode);
		}

		private static decimal Round(ref decimal d, int decimals, MidpointRounding mode)
		{
			if (decimals > 28)
			{
				throw new ArgumentOutOfRangeException("decimals", "Decimal can only round to between 0 and 28 digits of precision.");
			}
			if (mode > MidpointRounding.AwayFromZero)
			{
				throw new ArgumentException(SR.Format("The value '{0}' is not valid for this usage of the type {1}.", mode, "MidpointRounding"), "mode");
			}
			int num = d.Scale - decimals;
			if (num > 0)
			{
				decimal.DecCalc.InternalRound(decimal.AsMutable(ref d), (uint)num, (decimal.DecCalc.RoundingMode)mode);
			}
			return d;
		}

		internal static int Sign(ref decimal d)
		{
			if ((d.lo | d.mid | d.hi) != 0)
			{
				return (d.flags >> 31) | 1;
			}
			return 0;
		}

		public static decimal Subtract(decimal d1, decimal d2)
		{
			decimal.DecCalc.DecAddSub(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2), true);
			return d1;
		}

		public static byte ToByte(decimal value)
		{
			uint num;
			try
			{
				num = decimal.ToUInt32(value);
			}
			catch (OverflowException ex)
			{
				throw new OverflowException("Value was either too large or too small for an unsigned byte.", ex);
			}
			if (num != (uint)((byte)num))
			{
				throw new OverflowException("Value was either too large or too small for an unsigned byte.");
			}
			return (byte)num;
		}

		[CLSCompliant(false)]
		public static sbyte ToSByte(decimal value)
		{
			int num;
			try
			{
				num = decimal.ToInt32(value);
			}
			catch (OverflowException ex)
			{
				throw new OverflowException("Value was either too large or too small for a signed byte.", ex);
			}
			if (num != (int)((sbyte)num))
			{
				throw new OverflowException("Value was either too large or too small for a signed byte.");
			}
			return (sbyte)num;
		}

		public static short ToInt16(decimal value)
		{
			int num;
			try
			{
				num = decimal.ToInt32(value);
			}
			catch (OverflowException ex)
			{
				throw new OverflowException("Value was either too large or too small for an Int16.", ex);
			}
			if (num != (int)((short)num))
			{
				throw new OverflowException("Value was either too large or too small for an Int16.");
			}
			return (short)num;
		}

		public static double ToDouble(decimal d)
		{
			return decimal.DecCalc.VarR8FromDec(in d);
		}

		public static int ToInt32(decimal d)
		{
			decimal.Truncate(ref d);
			if ((d.hi | d.mid) == 0)
			{
				int num = d.lo;
				if (!d.IsNegative)
				{
					if (num >= 0)
					{
						return num;
					}
				}
				else
				{
					num = -num;
					if (num <= 0)
					{
						return num;
					}
				}
			}
			throw new OverflowException("Value was either too large or too small for an Int32.");
		}

		public static long ToInt64(decimal d)
		{
			decimal.Truncate(ref d);
			if (d.hi == 0)
			{
				long num = (long)d.Low64;
				if (!d.IsNegative)
				{
					if (num >= 0L)
					{
						return num;
					}
				}
				else
				{
					num = -num;
					if (num <= 0L)
					{
						return num;
					}
				}
			}
			throw new OverflowException("Value was either too large or too small for an Int64.");
		}

		[CLSCompliant(false)]
		public static ushort ToUInt16(decimal value)
		{
			uint num;
			try
			{
				num = decimal.ToUInt32(value);
			}
			catch (OverflowException ex)
			{
				throw new OverflowException("Value was either too large or too small for a UInt16.", ex);
			}
			if (num != (uint)((ushort)num))
			{
				throw new OverflowException("Value was either too large or too small for a UInt16.");
			}
			return (ushort)num;
		}

		[CLSCompliant(false)]
		public static uint ToUInt32(decimal d)
		{
			decimal.Truncate(ref d);
			if ((d.hi | d.mid) == 0)
			{
				uint low = d.Low;
				if (!d.IsNegative || low == 0U)
				{
					return low;
				}
			}
			throw new OverflowException("Value was either too large or too small for a UInt32.");
		}

		[CLSCompliant(false)]
		public static ulong ToUInt64(decimal d)
		{
			decimal.Truncate(ref d);
			if (d.hi == 0)
			{
				ulong low = d.Low64;
				if (!d.IsNegative || low == 0UL)
				{
					return low;
				}
			}
			throw new OverflowException("Value was either too large or too small for a UInt64.");
		}

		public static float ToSingle(decimal d)
		{
			return decimal.DecCalc.VarR4FromDec(in d);
		}

		public static decimal Truncate(decimal d)
		{
			decimal.Truncate(ref d);
			return d;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Truncate(ref decimal d)
		{
			int num = d.flags;
			if ((num & 16711680) != 0)
			{
				decimal.DecCalc.InternalRound(decimal.AsMutable(ref d), (uint)((byte)(num >> 16)), decimal.DecCalc.RoundingMode.Truncate);
			}
		}

		public static implicit operator decimal(byte value)
		{
			return new decimal((uint)value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(sbyte value)
		{
			return new decimal((int)value);
		}

		public static implicit operator decimal(short value)
		{
			return new decimal((int)value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(ushort value)
		{
			return new decimal((uint)value);
		}

		public static implicit operator decimal(char value)
		{
			return new decimal((uint)value);
		}

		public static implicit operator decimal(int value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(uint value)
		{
			return new decimal(value);
		}

		public static implicit operator decimal(long value)
		{
			return new decimal(value);
		}

		[CLSCompliant(false)]
		public static implicit operator decimal(ulong value)
		{
			return new decimal(value);
		}

		public static explicit operator decimal(float value)
		{
			return new decimal(value);
		}

		public static explicit operator decimal(double value)
		{
			return new decimal(value);
		}

		public static explicit operator byte(decimal value)
		{
			return decimal.ToByte(value);
		}

		[CLSCompliant(false)]
		public static explicit operator sbyte(decimal value)
		{
			return decimal.ToSByte(value);
		}

		public static explicit operator char(decimal value)
		{
			ushort num;
			try
			{
				num = decimal.ToUInt16(value);
			}
			catch (OverflowException ex)
			{
				throw new OverflowException("Value was either too large or too small for a character.", ex);
			}
			return (char)num;
		}

		public static explicit operator short(decimal value)
		{
			return decimal.ToInt16(value);
		}

		[CLSCompliant(false)]
		public static explicit operator ushort(decimal value)
		{
			return decimal.ToUInt16(value);
		}

		public static explicit operator int(decimal value)
		{
			return decimal.ToInt32(value);
		}

		[CLSCompliant(false)]
		public static explicit operator uint(decimal value)
		{
			return decimal.ToUInt32(value);
		}

		public static explicit operator long(decimal value)
		{
			return decimal.ToInt64(value);
		}

		[CLSCompliant(false)]
		public static explicit operator ulong(decimal value)
		{
			return decimal.ToUInt64(value);
		}

		public static explicit operator float(decimal value)
		{
			return decimal.ToSingle(value);
		}

		public static explicit operator double(decimal value)
		{
			return decimal.ToDouble(value);
		}

		public static decimal operator +(decimal d)
		{
			return d;
		}

		public static decimal operator -(decimal d)
		{
			return new decimal(in d, d.flags ^ int.MinValue);
		}

		public static decimal operator ++(decimal d)
		{
			return decimal.Add(d, 1m);
		}

		public static decimal operator --(decimal d)
		{
			return decimal.Subtract(d, 1m);
		}

		public static decimal operator +(decimal d1, decimal d2)
		{
			decimal.DecCalc.DecAddSub(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2), false);
			return d1;
		}

		public static decimal operator -(decimal d1, decimal d2)
		{
			decimal.DecCalc.DecAddSub(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2), true);
			return d1;
		}

		public static decimal operator *(decimal d1, decimal d2)
		{
			decimal.DecCalc.VarDecMul(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2));
			return d1;
		}

		public static decimal operator /(decimal d1, decimal d2)
		{
			decimal.DecCalc.VarDecDiv(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2));
			return d1;
		}

		public static decimal operator %(decimal d1, decimal d2)
		{
			decimal.DecCalc.VarDecMod(decimal.AsMutable(ref d1), decimal.AsMutable(ref d2));
			return d1;
		}

		public static bool operator ==(decimal d1, decimal d2)
		{
			return decimal.DecCalc.VarDecCmp(in d1, in d2) == 0;
		}

		public static bool operator !=(decimal d1, decimal d2)
		{
			return decimal.DecCalc.VarDecCmp(in d1, in d2) != 0;
		}

		public static bool operator <(decimal d1, decimal d2)
		{
			return decimal.DecCalc.VarDecCmp(in d1, in d2) < 0;
		}

		public static bool operator <=(decimal d1, decimal d2)
		{
			return decimal.DecCalc.VarDecCmp(in d1, in d2) <= 0;
		}

		public static bool operator >(decimal d1, decimal d2)
		{
			return decimal.DecCalc.VarDecCmp(in d1, in d2) > 0;
		}

		public static bool operator >=(decimal d1, decimal d2)
		{
			return decimal.DecCalc.VarDecCmp(in d1, in d2) >= 0;
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.Decimal;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.Format("Invalid cast from '{0}' to '{1}'.", "Decimal", "Char"));
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return Convert.ToSByte(this);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return this;
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException(SR.Format("Invalid cast from '{0}' to '{1}'.", "Decimal", "DateTime"));
		}

		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return Convert.DefaultToType(this, type, provider);
		}

		private const int SignMask = -2147483648;

		private const int ScaleMask = 16711680;

		private const int ScaleShift = 16;

		public const decimal Zero = 0m;

		public const decimal One = 1m;

		public const decimal MinusOne = -1m;

		public const decimal MaxValue = 79228162514264337593543950335m;

		public const decimal MinValue = -79228162514264337593543950335m;

		[FieldOffset(0)]
		private readonly int flags;

		[FieldOffset(4)]
		private readonly int hi;

		[FieldOffset(8)]
		private readonly int lo;

		[FieldOffset(12)]
		private readonly int mid;

		[NonSerialized]
		[FieldOffset(8)]
		private readonly ulong ulomidLE;

		[StructLayout(LayoutKind.Explicit)]
		private struct DecCalc
		{
			private uint High
			{
				get
				{
					return this.uhi;
				}
				set
				{
					this.uhi = value;
				}
			}

			private uint Low
			{
				get
				{
					return this.ulo;
				}
				set
				{
					this.ulo = value;
				}
			}

			private uint Mid
			{
				get
				{
					return this.umid;
				}
				set
				{
					this.umid = value;
				}
			}

			private bool IsNegative
			{
				get
				{
					return this.uflags < 0U;
				}
			}

			private int Scale
			{
				get
				{
					return (int)((byte)(this.uflags >> 16));
				}
			}

			private ulong Low64
			{
				get
				{
					if (!BitConverter.IsLittleEndian)
					{
						return ((ulong)this.umid << 32) | (ulong)this.ulo;
					}
					return this.ulomidLE;
				}
				set
				{
					if (BitConverter.IsLittleEndian)
					{
						this.ulomidLE = value;
						return;
					}
					this.umid = (uint)(value >> 32);
					this.ulo = (uint)value;
				}
			}

			private unsafe static uint GetExponent(float f)
			{
				return (uint)((byte)(*(uint*)(&f) >> 23));
			}

			private unsafe static uint GetExponent(double d)
			{
				return (uint)((ulong)(*(long*)(&d)) >> 52) & 2047U;
			}

			private static ulong UInt32x32To64(uint a, uint b)
			{
				return (ulong)a * (ulong)b;
			}

			private static void UInt64x64To128(ulong a, ulong b, ref decimal.DecCalc result)
			{
				ulong num = decimal.DecCalc.UInt32x32To64((uint)a, (uint)b);
				ulong num2 = decimal.DecCalc.UInt32x32To64((uint)a, (uint)(b >> 32));
				ulong num3 = decimal.DecCalc.UInt32x32To64((uint)(a >> 32), (uint)(b >> 32));
				num3 += num2 >> 32;
				num += (num2 <<= 32);
				if (num < num2)
				{
					num3 += 1UL;
				}
				num2 = decimal.DecCalc.UInt32x32To64((uint)(a >> 32), (uint)b);
				num3 += num2 >> 32;
				num += (num2 <<= 32);
				if (num < num2)
				{
					num3 += 1UL;
				}
				if (num3 > (ulong)(-1))
				{
					throw new OverflowException("Value was either too large or too small for a Decimal.");
				}
				result.Low64 = num;
				result.High = (uint)num3;
			}

			private static uint Div96By32(ref decimal.DecCalc.Buf12 bufNum, uint den)
			{
				if (bufNum.U2 != 0U)
				{
					ulong num = bufNum.High64;
					ulong num2 = num / (ulong)den;
					bufNum.High64 = num2;
					num = (num - (ulong)((uint)num2 * den) << 32) | (ulong)bufNum.U0;
					if (num == 0UL)
					{
						return 0U;
					}
					uint num3 = (uint)(num / (ulong)den);
					bufNum.U0 = num3;
					return (uint)num - num3 * den;
				}
				else
				{
					ulong num = bufNum.Low64;
					if (num == 0UL)
					{
						return 0U;
					}
					ulong num2 = num / (ulong)den;
					bufNum.Low64 = num2;
					return (uint)(num - num2 * (ulong)den);
				}
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static bool Div96ByConst(ref ulong high64, ref uint low, uint pow)
			{
				ulong num = high64 / (ulong)pow;
				uint num2 = (uint)(((high64 - num * (ulong)pow << 32) + (ulong)low) / (ulong)pow);
				if (low == num2 * pow)
				{
					high64 = num;
					low = num2;
					return true;
				}
				return false;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static void Unscale(ref uint low, ref ulong high64, ref int scale)
			{
				while ((byte)low == 0 && scale >= 8 && decimal.DecCalc.Div96ByConst(ref high64, ref low, 100000000U))
				{
					scale -= 8;
				}
				if ((low & 15U) == 0U && scale >= 4 && decimal.DecCalc.Div96ByConst(ref high64, ref low, 10000U))
				{
					scale -= 4;
				}
				if ((low & 3U) == 0U && scale >= 2 && decimal.DecCalc.Div96ByConst(ref high64, ref low, 100U))
				{
					scale -= 2;
				}
				if ((low & 1U) == 0U && scale >= 1 && decimal.DecCalc.Div96ByConst(ref high64, ref low, 10U))
				{
					scale--;
				}
			}

			private static uint Div96By64(ref decimal.DecCalc.Buf12 bufNum, ulong den)
			{
				uint u = bufNum.U2;
				if (u == 0U)
				{
					ulong num = bufNum.Low64;
					if (num < den)
					{
						return 0U;
					}
					uint num2 = (uint)(num / den);
					num -= (ulong)num2 * den;
					bufNum.Low64 = num;
					return num2;
				}
				else
				{
					uint num3 = (uint)(den >> 32);
					ulong num;
					uint num2;
					if (u >= num3)
					{
						num = bufNum.Low64;
						num -= den << 32;
						num2 = 0U;
						do
						{
							num2 -= 1U;
							num += den;
						}
						while (num >= den);
						bufNum.Low64 = num;
						return num2;
					}
					ulong high = bufNum.High64;
					if (high < (ulong)num3)
					{
						return 0U;
					}
					num2 = (uint)(high / (ulong)num3);
					num = (ulong)bufNum.U0 | (high - (ulong)(num2 * num3) << 32);
					ulong num4 = decimal.DecCalc.UInt32x32To64(num2, (uint)den);
					num -= num4;
					if (num > ~num4)
					{
						do
						{
							num2 -= 1U;
							num += den;
						}
						while (num >= den);
					}
					bufNum.Low64 = num;
					return num2;
				}
			}

			private static uint Div128By96(ref decimal.DecCalc.Buf16 bufNum, ref decimal.DecCalc.Buf12 bufDen)
			{
				ulong high = bufNum.High64;
				uint u = bufDen.U2;
				if (high < (ulong)u)
				{
					return 0U;
				}
				uint num = (uint)(high / (ulong)u);
				uint num2 = (uint)high - num * u;
				ulong num3 = decimal.DecCalc.UInt32x32To64(num, bufDen.U0);
				ulong num4 = decimal.DecCalc.UInt32x32To64(num, bufDen.U1);
				num4 += num3 >> 32;
				num3 = (ulong)((uint)num3) | (num4 << 32);
				num4 >>= 32;
				ulong num5 = bufNum.Low64;
				num5 -= num3;
				num2 -= (uint)num4;
				if (num5 > ~num3)
				{
					num2 -= 1U;
					if (num2 < ~(uint)num4)
					{
						goto IL_00B4;
					}
				}
				else if (num2 <= ~(uint)num4)
				{
					goto IL_00B4;
				}
				num3 = bufDen.Low64;
				do
				{
					num -= 1U;
					num5 += num3;
					num2 += u;
				}
				while ((num5 >= num3 || num2++ >= u) && num2 >= u);
				IL_00B4:
				bufNum.Low64 = num5;
				bufNum.U2 = num2;
				return num;
			}

			private static uint IncreaseScale(ref decimal.DecCalc.Buf12 bufNum, uint power)
			{
				ulong num = decimal.DecCalc.UInt32x32To64(bufNum.U0, power);
				bufNum.U0 = (uint)num;
				num >>= 32;
				num += decimal.DecCalc.UInt32x32To64(bufNum.U1, power);
				bufNum.U1 = (uint)num;
				num >>= 32;
				num += decimal.DecCalc.UInt32x32To64(bufNum.U2, power);
				bufNum.U2 = (uint)num;
				return (uint)(num >> 32);
			}

			private static void IncreaseScale64(ref decimal.DecCalc.Buf12 bufNum, uint power)
			{
				ulong num = decimal.DecCalc.UInt32x32To64(bufNum.U0, power);
				bufNum.U0 = (uint)num;
				num >>= 32;
				num += decimal.DecCalc.UInt32x32To64(bufNum.U1, power);
				bufNum.High64 = num;
			}

			private unsafe static int ScaleResult(decimal.DecCalc.Buf24* bufRes, uint hiRes, int scale)
			{
				int num = 0;
				if (hiRes > 2U)
				{
					num = (int)(hiRes * 32U - 64U - 1U);
					num -= decimal.DecCalc.LeadingZeroCount(*(uint*)(bufRes + (ulong)hiRes * 4UL / (ulong)sizeof(decimal.DecCalc.Buf24)));
					num = (num * 77 >> 8) + 1;
					if (num > scale)
					{
						goto IL_01CC;
					}
				}
				if (num < scale - 28)
				{
					num = scale - 28;
				}
				if (num != 0)
				{
					scale -= num;
					uint num2 = 0U;
					uint num3 = 0U;
					for (;;)
					{
						num2 |= num3;
						uint num5;
						uint num4;
						switch (num)
						{
						case 1:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 10U);
							break;
						case 2:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 100U);
							break;
						case 3:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 1000U);
							break;
						case 4:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 10000U);
							break;
						case 5:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 100000U);
							break;
						case 6:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 1000000U);
							break;
						case 7:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 10000000U);
							break;
						case 8:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 100000000U);
							break;
						default:
							num4 = decimal.DecCalc.DivByConst((uint*)bufRes, hiRes, out num5, out num3, 1000000000U);
							break;
						}
						*(int*)(bufRes + (ulong)hiRes * 4UL / (ulong)sizeof(decimal.DecCalc.Buf24)) = (int)num5;
						if (num5 == 0U && hiRes != 0U)
						{
							hiRes -= 1U;
						}
						num -= 9;
						if (num <= 0)
						{
							if (hiRes > 2U)
							{
								if (scale == 0)
								{
									goto IL_01CC;
								}
								num = 1;
								scale--;
							}
							else
							{
								num4 >>= 1;
								if (num4 > num3 || (num4 >= num3 && ((*(uint*)bufRes & 1U) | num2) == 0U))
								{
									break;
								}
								uint num6 = *(uint*)bufRes + 1U;
								*(int*)bufRes = (int)num6;
								if (num6 != 0U)
								{
									break;
								}
								uint num7 = 0U;
								do
								{
									decimal.DecCalc.Buf24* ptr = bufRes + (ulong)(num7 += 1U) * 4UL / (ulong)sizeof(decimal.DecCalc.Buf24);
									num6 = *(uint*)ptr + 1U;
									*(int*)ptr = (int)num6;
								}
								while (num6 == 0U);
								if (num7 <= 2U)
								{
									break;
								}
								if (scale == 0)
								{
									goto IL_01CC;
								}
								hiRes = num7;
								num2 = 0U;
								num3 = 0U;
								num = 1;
								scale--;
							}
						}
					}
				}
				return scale;
				IL_01CC:
				throw new OverflowException("Value was either too large or too small for a Decimal.");
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private unsafe static uint DivByConst(uint* result, uint hiRes, out uint quotient, out uint remainder, uint power)
			{
				uint num = result[(ulong)hiRes * 4UL / 4UL];
				remainder = num - (quotient = num / power) * power;
				for (uint num2 = hiRes - 1U; num2 >= 0U; num2 -= 1U)
				{
					ulong num3 = (ulong)result[(ulong)num2 * 4UL / 4UL] + ((ulong)remainder << 32);
					remainder = (uint)num3 - (result[(ulong)num2 * 4UL / 4UL] = (uint)(num3 / (ulong)power)) * power;
				}
				return power;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static int LeadingZeroCount(uint value)
			{
				int num = 1;
				if ((value & 4294901760U) == 0U)
				{
					value <<= 16;
					num += 16;
				}
				if ((value & 4278190080U) == 0U)
				{
					value <<= 8;
					num += 8;
				}
				if ((value & 4026531840U) == 0U)
				{
					value <<= 4;
					num += 4;
				}
				if ((value & 3221225472U) == 0U)
				{
					value <<= 2;
					num += 2;
				}
				return num + ((int)value >> 31);
			}

			private static int OverflowUnscale(ref decimal.DecCalc.Buf12 bufQuo, int scale, bool sticky)
			{
				if (--scale < 0)
				{
					throw new OverflowException("Value was either too large or too small for a Decimal.");
				}
				bufQuo.U2 = 429496729U;
				ulong num = 25769803776UL + (ulong)bufQuo.U1;
				uint num2 = (uint)(num / 10UL);
				bufQuo.U1 = num2;
				ulong num3 = (num - (ulong)(num2 * 10U) << 32) + (ulong)bufQuo.U0;
				num2 = (uint)(num3 / 10UL);
				bufQuo.U0 = num2;
				uint num4 = (uint)(num3 - (ulong)(num2 * 10U));
				if (num4 > 5U || (num4 == 5U && (sticky || (bufQuo.U0 & 1U) != 0U)))
				{
					decimal.DecCalc.Add32To96(ref bufQuo, 1U);
				}
				return scale;
			}

			private static int SearchScale(ref decimal.DecCalc.Buf12 bufQuo, int scale)
			{
				uint u = bufQuo.U2;
				ulong low = bufQuo.Low64;
				int num = 0;
				if (u <= 429496729U)
				{
					decimal.DecCalc.PowerOvfl[] powerOvflValues = decimal.DecCalc.PowerOvflValues;
					if (scale > 19)
					{
						num = 28 - scale;
						if (u < powerOvflValues[num - 1].Hi)
						{
							goto IL_00D1;
						}
					}
					else if (u < 4U || (u == 4U && low <= 5441186219426131129UL))
					{
						return 9;
					}
					if (u > 42949U)
					{
						if (u > 4294967U)
						{
							num = 2;
							if (u > 42949672U)
							{
								num--;
							}
						}
						else
						{
							num = 4;
							if (u > 429496U)
							{
								num--;
							}
						}
					}
					else if (u > 429U)
					{
						num = 6;
						if (u > 4294U)
						{
							num--;
						}
					}
					else
					{
						num = 8;
						if (u > 42U)
						{
							num--;
						}
					}
					if (u == powerOvflValues[num - 1].Hi && low > powerOvflValues[num - 1].MidLo)
					{
						num--;
					}
				}
				IL_00D1:
				if (num + scale < 0)
				{
					throw new OverflowException("Value was either too large or too small for a Decimal.");
				}
				return num;
			}

			private static bool Add32To96(ref decimal.DecCalc.Buf12 bufNum, uint value)
			{
				if ((bufNum.Low64 += (ulong)value) < (ulong)value)
				{
					uint num = bufNum.U2 + 1U;
					bufNum.U2 = num;
					if (num == 0U)
					{
						return false;
					}
				}
				return true;
			}

			internal unsafe static void DecAddSub(ref decimal.DecCalc d1, ref decimal.DecCalc d2, bool sign)
			{
				ulong num = d1.Low64;
				uint num2 = d1.High;
				uint num3 = d1.uflags;
				uint num4 = d2.uflags;
				uint num5 = num4 ^ num3;
				sign ^= (num5 & 2147483648U) > 0U;
				if ((num5 & 16711680U) != 0U)
				{
					uint num6 = num3;
					num3 = (num4 & 16711680U) | (num3 & 2147483648U);
					int i = (int)(num3 - num6) >> 16;
					if (i < 0)
					{
						i = -i;
						num3 = num6;
						if (sign)
						{
							num3 ^= 2147483648U;
						}
						num = d2.Low64;
						num2 = d2.High;
						d2 = d1;
					}
					ulong num10;
					if (num2 == 0U)
					{
						if (num <= (ulong)(-1))
						{
							if ((uint)num == 0U)
							{
								uint num7 = num3 & 2147483648U;
								if (sign)
								{
									num7 ^= 2147483648U;
								}
								d1 = d2;
								d1.uflags = (d2.uflags & 16711680U) | num7;
								return;
							}
							while (i > 9)
							{
								i -= 9;
								num = decimal.DecCalc.UInt32x32To64((uint)num, 1000000000U);
								if (num > (ulong)(-1))
								{
									goto IL_0106;
								}
							}
							num = decimal.DecCalc.UInt32x32To64((uint)num, decimal.DecCalc.s_powers10[i]);
							goto IL_0441;
						}
						do
						{
							IL_0106:
							uint num8 = 1000000000U;
							if (i < 9)
							{
								num8 = decimal.DecCalc.s_powers10[i];
							}
							ulong num9 = decimal.DecCalc.UInt32x32To64((uint)num, num8);
							num10 = decimal.DecCalc.UInt32x32To64((uint)(num >> 32), num8) + (num9 >> 32);
							num = (ulong)((uint)num9) + (num10 << 32);
							num2 = (uint)(num10 >> 32);
							if ((i -= 9) <= 0)
							{
								goto IL_0441;
							}
						}
						while (num2 == 0U);
					}
					do
					{
						uint num8 = 1000000000U;
						if (i < 9)
						{
							num8 = decimal.DecCalc.s_powers10[i];
						}
						ulong num9 = decimal.DecCalc.UInt32x32To64((uint)num, num8);
						num10 = decimal.DecCalc.UInt32x32To64((uint)(num >> 32), num8) + (num9 >> 32);
						num = (ulong)((uint)num9) + (num10 << 32);
						num10 >>= 32;
						num10 += decimal.DecCalc.UInt32x32To64(num2, num8);
						i -= 9;
						if (num10 > (ulong)(-1))
						{
							goto IL_01CF;
						}
						num2 = (uint)num10;
					}
					while (i > 0);
					goto IL_0441;
					IL_01CF:
					decimal.DecCalc.Buf24 buf;
					buf.Low64 = num;
					buf.Mid64 = num10;
					uint num11 = 3U;
					while (i > 0)
					{
						uint num8 = 1000000000U;
						if (i < 9)
						{
							num8 = decimal.DecCalc.s_powers10[i];
						}
						num10 = 0UL;
						uint* ptr = (uint*)(&buf);
						uint num12 = 0U;
						do
						{
							num10 += decimal.DecCalc.UInt32x32To64(ptr[(ulong)num12 * 4UL / 4UL], num8);
							ptr[(ulong)num12 * 4UL / 4UL] = (uint)num10;
							num12 += 1U;
							num10 >>= 32;
						}
						while (num12 <= num11);
						if ((uint)num10 != 0U)
						{
							ptr[(IntPtr)((ulong)(num11 += 1U) * 4UL)] = (uint)num10;
						}
						i -= 9;
					}
					num10 = buf.Low64;
					num = d2.Low64;
					uint u = buf.U2;
					num2 = d2.High;
					if (sign)
					{
						num = num10 - num;
						num2 = u - num2;
						if (num > num10)
						{
							num2 -= 1U;
							if (num2 < u)
							{
								goto IL_034C;
							}
						}
						else if (num2 <= u)
						{
							goto IL_034C;
						}
						uint* ptr2 = (uint*)(&buf);
						uint num13 = 3U;
						uint num14;
						do
						{
							uint* ptr3 = ptr2 + (IntPtr)((ulong)num13++ * 4UL);
							num14 = *ptr3;
							*ptr3 = num14 - 1U;
						}
						while (num14 == 0U);
						if (ptr2[(ulong)num11 * 4UL / 4UL] == 0U && (num11 -= 1U) <= 2U)
						{
							goto IL_04AA;
						}
					}
					else
					{
						num += num10;
						num2 += u;
						if (num < num10)
						{
							num2 += 1U;
							if (num2 > u)
							{
								goto IL_034C;
							}
						}
						else if (num2 >= u)
						{
							goto IL_034C;
						}
						uint* ptr4 = (uint*)(&buf);
						uint num15 = 3U;
						do
						{
							uint* ptr5 = ptr4 + (IntPtr)((ulong)num15++ * 4UL);
							uint num14 = *ptr5 + 1U;
							*ptr5 = num14;
							if (num14 != 0U)
							{
								goto IL_034C;
							}
						}
						while (num11 >= num15);
						ptr4[(ulong)num15 * 4UL / 4UL] = 1U;
						num11 = num15;
					}
					IL_034C:
					buf.Low64 = num;
					buf.U2 = num2;
					i = decimal.DecCalc.ScaleResult(&buf, num11, (int)((byte)(num3 >> 16)));
					num3 = (num3 & 4278255615U) | (uint)((uint)i << 16);
					num = buf.Low64;
					num2 = buf.U2;
					goto IL_04AA;
				}
				IL_0441:
				ulong num16 = num;
				uint num17 = num2;
				if (sign)
				{
					num = num16 - d2.Low64;
					num2 = num17 - d2.High;
					if (num > num16)
					{
						num2 -= 1U;
						if (num2 < num17)
						{
							goto IL_04AA;
						}
					}
					else if (num2 <= num17)
					{
						goto IL_04AA;
					}
					num3 ^= 2147483648U;
					num2 = ~num2;
					num = -num;
					if (num == 0UL)
					{
						num2 += 1U;
					}
				}
				else
				{
					num = num16 + d2.Low64;
					num2 = num17 + d2.High;
					if (num < num16)
					{
						num2 += 1U;
						if (num2 > num17)
						{
							goto IL_04AA;
						}
					}
					else if (num2 >= num17)
					{
						goto IL_04AA;
					}
					if ((num3 & 16711680U) == 0U)
					{
						throw new OverflowException("Value was either too large or too small for a Decimal.");
					}
					num3 -= 65536U;
					ulong num18 = (ulong)num2 + 4294967296UL;
					num2 = (uint)(num18 / 10UL);
					ulong num19 = (num18 - (ulong)(num2 * 10U) << 32) + (num >> 32);
					uint num20 = (uint)(num19 / 10UL);
					ulong num21 = (num19 - (ulong)(num20 * 10U) << 32) + (ulong)((uint)num);
					num = (ulong)num20;
					num <<= 32;
					num20 = (uint)(num21 / 10UL);
					num += (ulong)num20;
					num20 = (uint)num21 - num20 * 10U;
					if (num20 >= 5U && (num20 > 5U || (num & 1UL) != 0UL) && (num += 1UL) == 0UL)
					{
						num2 += 1U;
					}
				}
				IL_04AA:
				d1.uflags = num3;
				d1.High = num2;
				d1.Low64 = num;
			}

			internal static long VarCyFromDec(ref decimal.DecCalc pdecIn)
			{
				int num = pdecIn.Scale - 4;
				long num5;
				if (num < 0)
				{
					if (pdecIn.High != 0U)
					{
						goto IL_0093;
					}
					uint num2 = decimal.DecCalc.s_powers10[-num];
					ulong num3 = decimal.DecCalc.UInt32x32To64(num2, pdecIn.Mid);
					if (num3 > (ulong)(-1))
					{
						goto IL_0093;
					}
					ulong num4 = decimal.DecCalc.UInt32x32To64(num2, pdecIn.Low);
					num4 += (num3 <<= 32);
					if (num4 < num3)
					{
						goto IL_0093;
					}
					num5 = (long)num4;
				}
				else
				{
					if (num != 0)
					{
						decimal.DecCalc.InternalRound(ref pdecIn, (uint)num, decimal.DecCalc.RoundingMode.ToEven);
					}
					if (pdecIn.High != 0U)
					{
						goto IL_0093;
					}
					num5 = (long)pdecIn.Low64;
				}
				if (num5 >= 0L || (num5 == -9223372036854775808L && pdecIn.IsNegative))
				{
					if (pdecIn.IsNegative)
					{
						num5 = -num5;
					}
					return num5;
				}
				IL_0093:
				throw new OverflowException("Value was either too large or too small for a Currency.");
			}

			internal static int VarDecCmp(in decimal d1, in decimal d2)
			{
				if ((d2.Low | d2.Mid | d2.High) == 0U)
				{
					if ((d1.Low | d1.Mid | d1.High) == 0U)
					{
						return 0;
					}
					return (d1.flags >> 31) | 1;
				}
				else
				{
					if ((d1.Low | d1.Mid | d1.High) == 0U)
					{
						return -((d2.flags >> 31) | 1);
					}
					int num = (d1.flags >> 31) - (d2.flags >> 31);
					if (num != 0)
					{
						return num;
					}
					return decimal.DecCalc.VarDecCmpSub(in d1, in d2);
				}
			}

			private static int VarDecCmpSub(in decimal d1, in decimal d2)
			{
				int flags = d2.flags;
				int num = (flags >> 31) | 1;
				int num2 = flags - d1.flags;
				ulong num3 = d1.Low64;
				uint num4 = d1.High;
				ulong num5 = d2.Low64;
				uint num6 = d2.High;
				if (num2 != 0)
				{
					num2 >>= 16;
					if (num2 < 0)
					{
						num2 = -num2;
						num = -num;
						ulong num7 = num3;
						num3 = num5;
						num5 = num7;
						uint num8 = num4;
						num4 = num6;
						num6 = num8;
					}
					for (;;)
					{
						uint num9 = ((num2 >= 9) ? 1000000000U : decimal.DecCalc.s_powers10[num2]);
						ulong num10 = decimal.DecCalc.UInt32x32To64((uint)num3, num9);
						ulong num11 = decimal.DecCalc.UInt32x32To64((uint)(num3 >> 32), num9) + (num10 >> 32);
						num3 = (ulong)((uint)num10) + (num11 << 32);
						num11 >>= 32;
						num11 += decimal.DecCalc.UInt32x32To64(num4, num9);
						if (num11 > (ulong)(-1))
						{
							break;
						}
						num4 = (uint)num11;
						if ((num2 -= 9) <= 0)
						{
							goto IL_00BC;
						}
					}
					return num;
				}
				IL_00BC:
				uint num12 = num4 - num6;
				if (num12 != 0U)
				{
					if (num12 > num4)
					{
						num = -num;
					}
					return num;
				}
				ulong num13 = num3 - num5;
				if (num13 == 0UL)
				{
					num = 0;
				}
				else if (num13 > num3)
				{
					num = -num;
				}
				return num;
			}

			internal unsafe static void VarDecMul(ref decimal.DecCalc d1, ref decimal.DecCalc d2)
			{
				int num = (int)((byte)(d1.uflags + d2.uflags >> 16));
				decimal.DecCalc.Buf24 buf;
				uint num6;
				if ((d1.High | d1.Mid) == 0U)
				{
					ulong num4;
					if ((d2.High | d2.Mid) == 0U)
					{
						ulong num2 = decimal.DecCalc.UInt32x32To64(d1.Low, d2.Low);
						if (num > 28)
						{
							if (num > 47)
							{
								goto IL_03CD;
							}
							num -= 29;
							ulong num3 = decimal.DecCalc.s_ulongPowers10[num];
							num4 = num2 / num3;
							ulong num5 = num2 - num4 * num3;
							num2 = num4;
							num3 >>= 1;
							if (num5 >= num3 && (num5 > num3 || ((uint)num2 & 1U) > 0U))
							{
								num2 += 1UL;
							}
							num = 28;
						}
						d1.Low64 = num2;
						d1.uflags = ((d2.uflags ^ d1.uflags) & 2147483648U) | (uint)((uint)num << 16);
						return;
					}
					num4 = decimal.DecCalc.UInt32x32To64(d1.Low, d2.Low);
					buf.U0 = (uint)num4;
					num4 = decimal.DecCalc.UInt32x32To64(d1.Low, d2.Mid) + (num4 >> 32);
					buf.U1 = (uint)num4;
					num4 >>= 32;
					if (d2.High != 0U)
					{
						num4 += decimal.DecCalc.UInt32x32To64(d1.Low, d2.High);
						if (num4 > (ulong)(-1))
						{
							buf.Mid64 = num4;
							num6 = 3U;
							goto IL_0381;
						}
					}
					if ((uint)num4 != 0U)
					{
						buf.U2 = (uint)num4;
						num6 = 2U;
						goto IL_0381;
					}
					num6 = 1U;
				}
				else if ((d2.High | d2.Mid) == 0U)
				{
					ulong num4 = decimal.DecCalc.UInt32x32To64(d2.Low, d1.Low);
					buf.U0 = (uint)num4;
					num4 = decimal.DecCalc.UInt32x32To64(d2.Low, d1.Mid) + (num4 >> 32);
					buf.U1 = (uint)num4;
					num4 >>= 32;
					if (d1.High != 0U)
					{
						num4 += decimal.DecCalc.UInt32x32To64(d2.Low, d1.High);
						if (num4 > (ulong)(-1))
						{
							buf.Mid64 = num4;
							num6 = 3U;
							goto IL_0381;
						}
					}
					if ((uint)num4 != 0U)
					{
						buf.U2 = (uint)num4;
						num6 = 2U;
						goto IL_0381;
					}
					num6 = 1U;
				}
				else
				{
					ulong num4 = decimal.DecCalc.UInt32x32To64(d1.Low, d2.Low);
					buf.U0 = (uint)num4;
					ulong num7 = decimal.DecCalc.UInt32x32To64(d1.Low, d2.Mid) + (num4 >> 32);
					num4 = decimal.DecCalc.UInt32x32To64(d1.Mid, d2.Low);
					num4 += num7;
					buf.U1 = (uint)num4;
					if (num4 < num7)
					{
						num7 = (num4 >> 32) | 4294967296UL;
					}
					else
					{
						num7 = num4 >> 32;
					}
					num4 = decimal.DecCalc.UInt32x32To64(d1.Mid, d2.Mid) + num7;
					if ((d1.High | d2.High) > 0U)
					{
						num7 = decimal.DecCalc.UInt32x32To64(d1.Low, d2.High);
						num4 += num7;
						uint num8 = 0U;
						if (num4 < num7)
						{
							num8 = 1U;
						}
						num7 = decimal.DecCalc.UInt32x32To64(d1.High, d2.Low);
						num4 += num7;
						buf.U2 = (uint)num4;
						if (num4 < num7)
						{
							num8 += 1U;
						}
						num7 = ((ulong)num8 << 32) | (num4 >> 32);
						num4 = decimal.DecCalc.UInt32x32To64(d1.Mid, d2.High);
						num4 += num7;
						num8 = 0U;
						if (num4 < num7)
						{
							num8 = 1U;
						}
						num7 = decimal.DecCalc.UInt32x32To64(d1.High, d2.Mid);
						num4 += num7;
						buf.U3 = (uint)num4;
						if (num4 < num7)
						{
							num8 += 1U;
						}
						num4 = ((ulong)num8 << 32) | (num4 >> 32);
						buf.High64 = decimal.DecCalc.UInt32x32To64(d1.High, d2.High) + num4;
						num6 = 5U;
					}
					else if (num4 != 0UL)
					{
						buf.Mid64 = num4;
						num6 = 3U;
					}
					else
					{
						num6 = 1U;
					}
				}
				uint* ptr = (uint*)(&buf);
				while (ptr[num6] == 0U)
				{
					if (num6 == 0U)
					{
						goto IL_03CD;
					}
					num6 -= 1U;
				}
				IL_0381:
				if (num6 > 2U || num > 28)
				{
					num = decimal.DecCalc.ScaleResult(&buf, num6, num);
				}
				d1.Low64 = buf.Low64;
				d1.High = buf.U2;
				d1.uflags = ((d2.uflags ^ d1.uflags) & 2147483648U) | (uint)((uint)num << 16);
				return;
				IL_03CD:
				d1 = default(decimal.DecCalc);
			}

			internal static void VarDecFromR4(float input, out decimal.DecCalc result)
			{
				result = default(decimal.DecCalc);
				int num = (int)(decimal.DecCalc.GetExponent(input) - 126U);
				if (num < -94)
				{
					return;
				}
				if (num > 96)
				{
					throw new OverflowException("Value was either too large or too small for a Decimal.");
				}
				uint num2 = 0U;
				if (input < 0f)
				{
					input = -input;
					num2 = 2147483648U;
				}
				double num3 = (double)input;
				int num4 = 6 - (num * 19728 >> 16);
				if (num4 >= 0)
				{
					if (num4 > 28)
					{
						num4 = 28;
					}
					num3 *= decimal.DecCalc.s_doublePowers10[num4];
				}
				else if (num4 != -1 || num3 >= 10000000.0)
				{
					num3 /= decimal.DecCalc.s_doublePowers10[-num4];
				}
				else
				{
					num4 = 0;
				}
				if (num3 < 1000000.0 && num4 < 28)
				{
					num3 *= 10.0;
					num4++;
				}
				uint num5 = (uint)((int)num3);
				num3 -= (double)num5;
				if (num3 > 0.5 || (num3 == 0.5 && (num5 & 1U) != 0U))
				{
					num5 += 1U;
				}
				if (num5 == 0U)
				{
					return;
				}
				if (num4 < 0)
				{
					num4 = -num4;
					if (num4 < 10)
					{
						result.Low64 = decimal.DecCalc.UInt32x32To64(num5, decimal.DecCalc.s_powers10[num4]);
					}
					else if (num4 > 18)
					{
						decimal.DecCalc.UInt64x64To128(decimal.DecCalc.UInt32x32To64(num5, decimal.DecCalc.s_powers10[num4 - 18]), 1000000000000000000UL, ref result);
					}
					else
					{
						ulong num6 = decimal.DecCalc.UInt32x32To64(num5, decimal.DecCalc.s_powers10[num4 - 9]);
						ulong num7 = decimal.DecCalc.UInt32x32To64(1000000000U, (uint)(num6 >> 32));
						num6 = decimal.DecCalc.UInt32x32To64(1000000000U, (uint)num6);
						result.Low = (uint)num6;
						num7 += num6 >> 32;
						result.Mid = (uint)num7;
						num7 >>= 32;
						result.High = (uint)num7;
					}
				}
				else
				{
					int num8 = num4;
					if (num8 > 6)
					{
						num8 = 6;
					}
					if ((num5 & 15U) == 0U && num8 >= 4)
					{
						uint num9 = num5 / 10000U;
						if (num5 == num9 * 10000U)
						{
							num5 = num9;
							num4 -= 4;
							num8 -= 4;
						}
					}
					if ((num5 & 3U) == 0U && num8 >= 2)
					{
						uint num10 = num5 / 100U;
						if (num5 == num10 * 100U)
						{
							num5 = num10;
							num4 -= 2;
							num8 -= 2;
						}
					}
					if ((num5 & 1U) == 0U && num8 >= 1)
					{
						uint num11 = num5 / 10U;
						if (num5 == num11 * 10U)
						{
							num5 = num11;
							num4--;
						}
					}
					num2 |= (uint)((uint)num4 << 16);
					result.Low = num5;
				}
				result.uflags = num2;
			}

			internal static void VarDecFromR8(double input, out decimal.DecCalc result)
			{
				result = default(decimal.DecCalc);
				int num = (int)(decimal.DecCalc.GetExponent(input) - 1022U);
				if (num < -94)
				{
					return;
				}
				if (num > 96)
				{
					throw new OverflowException("Value was either too large or too small for a Decimal.");
				}
				uint num2 = 0U;
				if (input < 0.0)
				{
					input = -input;
					num2 = 2147483648U;
				}
				double num3 = input;
				int num4 = 14 - (num * 19728 >> 16);
				if (num4 >= 0)
				{
					if (num4 > 28)
					{
						num4 = 28;
					}
					num3 *= decimal.DecCalc.s_doublePowers10[num4];
				}
				else if (num4 != -1 || num3 >= 1000000000000000.0)
				{
					num3 /= decimal.DecCalc.s_doublePowers10[-num4];
				}
				else
				{
					num4 = 0;
				}
				if (num3 < 100000000000000.0 && num4 < 28)
				{
					num3 *= 10.0;
					num4++;
				}
				ulong num5 = (ulong)((long)num3);
				num3 -= (double)num5;
				if (num3 > 0.5 || (num3 == 0.5 && (num5 & 1UL) != 0UL))
				{
					num5 += 1UL;
				}
				if (num5 == 0UL)
				{
					return;
				}
				if (num4 < 0)
				{
					num4 = -num4;
					if (num4 < 10)
					{
						uint num6 = decimal.DecCalc.s_powers10[num4];
						ulong num7 = decimal.DecCalc.UInt32x32To64((uint)num5, num6);
						ulong num8 = decimal.DecCalc.UInt32x32To64((uint)(num5 >> 32), num6);
						result.Low = (uint)num7;
						num8 += num7 >> 32;
						result.Mid = (uint)num8;
						num8 >>= 32;
						result.High = (uint)num8;
					}
					else
					{
						decimal.DecCalc.UInt64x64To128(num5, decimal.DecCalc.s_ulongPowers10[num4 - 1], ref result);
					}
				}
				else
				{
					int num9 = num4;
					if (num9 > 14)
					{
						num9 = 14;
					}
					if ((byte)num5 == 0 && num9 >= 8)
					{
						ulong num10 = num5 / 100000000UL;
						if ((uint)num5 == (uint)(num10 * 100000000UL))
						{
							num5 = num10;
							num4 -= 8;
							num9 -= 8;
						}
					}
					if (((uint)num5 & 15U) == 0U && num9 >= 4)
					{
						ulong num11 = num5 / 10000UL;
						if ((uint)num5 == (uint)(num11 * 10000UL))
						{
							num5 = num11;
							num4 -= 4;
							num9 -= 4;
						}
					}
					if (((uint)num5 & 3U) == 0U && num9 >= 2)
					{
						ulong num12 = num5 / 100UL;
						if ((uint)num5 == (uint)(num12 * 100UL))
						{
							num5 = num12;
							num4 -= 2;
							num9 -= 2;
						}
					}
					if (((uint)num5 & 1U) == 0U && num9 >= 1)
					{
						ulong num13 = num5 / 10UL;
						if ((uint)num5 == (uint)(num13 * 10UL))
						{
							num5 = num13;
							num4--;
						}
					}
					num2 |= (uint)((uint)num4 << 16);
					result.Low64 = num5;
				}
				result.uflags = num2;
			}

			internal static float VarR4FromDec(in decimal value)
			{
				return (float)decimal.DecCalc.VarR8FromDec(in value);
			}

			internal static double VarR8FromDec(in decimal value)
			{
				double num = (value.Low64 + value.High * 1.8446744073709552E+19) / decimal.DecCalc.s_doublePowers10[value.Scale];
				if (value.IsNegative)
				{
					num = -num;
				}
				return num;
			}

			internal static int GetHashCode(in decimal d)
			{
				if ((d.Low | d.Mid | d.High) == 0U)
				{
					return 0;
				}
				uint num = (uint)d.flags;
				if ((num & 16711680U) == 0U || (d.Low & 1U) != 0U)
				{
					return (int)(num ^ d.High ^ d.Mid ^ d.Low);
				}
				int num2 = (int)((byte)(num >> 16));
				uint low = d.Low;
				ulong num3 = ((ulong)d.High << 32) | (ulong)d.Mid;
				decimal.DecCalc.Unscale(ref low, ref num3, ref num2);
				num = (num & 4278255615U) | (uint)((uint)num2 << 16);
				return (int)(num ^ (uint)(num3 >> 32) ^ (uint)num3 ^ low);
			}

			internal unsafe static void VarDecDiv(ref decimal.DecCalc d1, ref decimal.DecCalc d2)
			{
				int num = (int)((sbyte)(d1.uflags - d2.uflags >> 16));
				bool flag = false;
				decimal.DecCalc.Buf12 buf;
				if ((d2.High | d2.Mid) == 0U)
				{
					uint low = d2.Low;
					if (low == 0U)
					{
						throw new DivideByZeroException();
					}
					buf.Low64 = d1.Low64;
					buf.U2 = d1.High;
					uint num2 = decimal.DecCalc.Div96By32(ref buf, low);
					for (;;)
					{
						int num3;
						if (num2 == 0U)
						{
							if (num >= 0)
							{
								goto IL_03D2;
							}
							num3 = Math.Min(9, -num);
						}
						else
						{
							flag = true;
							if (num == 28 || (num3 = decimal.DecCalc.SearchScale(ref buf, num)) == 0)
							{
								break;
							}
						}
						uint num4 = decimal.DecCalc.s_powers10[num3];
						num += num3;
						if (decimal.DecCalc.IncreaseScale(ref buf, num4) != 0U)
						{
							goto IL_048A;
						}
						ulong num5 = decimal.DecCalc.UInt32x32To64(num2, num4);
						uint num6 = (uint)(num5 / (ulong)low);
						num2 = (uint)num5 - num6 * low;
						if (!decimal.DecCalc.Add32To96(ref buf, num6))
						{
							goto Block_11;
						}
					}
					uint num7 = num2 << 1;
					if (num7 < num2)
					{
						goto IL_0449;
					}
					if (num7 < low)
					{
						goto IL_03D2;
					}
					if (num7 > low)
					{
						goto IL_0449;
					}
					if ((buf.U0 & 1U) != 0U)
					{
						goto IL_0449;
					}
					goto IL_03D2;
					Block_11:
					num = decimal.DecCalc.OverflowUnscale(ref buf, num, num2 > 0U);
				}
				else
				{
					uint num7 = d2.High;
					if (num7 == 0U)
					{
						num7 = d2.Mid;
					}
					int num3 = decimal.DecCalc.LeadingZeroCount(num7);
					decimal.DecCalc.Buf16 buf2;
					buf2.Low64 = d1.Low64 << num3;
					buf2.High64 = (ulong)d1.Mid + ((ulong)d1.High << 32) >> 32 - num3;
					ulong num8 = d2.Low64 << num3;
					if (d2.High == 0U)
					{
						buf.U1 = decimal.DecCalc.Div96By64(ref *(decimal.DecCalc.Buf12*)(&buf2.U1), num8);
						buf.U0 = decimal.DecCalc.Div96By64(ref *(decimal.DecCalc.Buf12*)(&buf2), num8);
						for (;;)
						{
							if (buf2.Low64 == 0UL)
							{
								if (num >= 0)
								{
									goto IL_03D2;
								}
								num3 = Math.Min(9, -num);
							}
							else
							{
								flag = true;
								if (num == 28 || (num3 = decimal.DecCalc.SearchScale(ref buf, num)) == 0)
								{
									break;
								}
							}
							uint num4 = decimal.DecCalc.s_powers10[num3];
							num += num3;
							if (decimal.DecCalc.IncreaseScale(ref buf, num4) != 0U)
							{
								goto IL_048A;
							}
							decimal.DecCalc.IncreaseScale64(ref *(decimal.DecCalc.Buf12*)(&buf2), num4);
							num7 = decimal.DecCalc.Div96By64(ref *(decimal.DecCalc.Buf12*)(&buf2), num8);
							if (!decimal.DecCalc.Add32To96(ref buf, num7))
							{
								goto Block_22;
							}
						}
						ulong num9 = buf2.Low64;
						if (num9 < 0UL || (num9 <<= 1) > num8)
						{
							goto IL_0449;
						}
						if (num9 == num8 && (buf.U0 & 1U) != 0U)
						{
							goto IL_0449;
						}
						goto IL_03D2;
						Block_22:
						num = decimal.DecCalc.OverflowUnscale(ref buf, num, buf2.Low64 > 0UL);
					}
					else
					{
						decimal.DecCalc.Buf12 buf3;
						buf3.Low64 = num8;
						buf3.U2 = (uint)((ulong)d2.Mid + ((ulong)d2.High << 32) >> 32 - num3);
						buf.Low64 = (ulong)decimal.DecCalc.Div128By96(ref buf2, ref buf3);
						for (;;)
						{
							if ((buf2.Low64 | (ulong)buf2.U2) == 0UL)
							{
								if (num >= 0)
								{
									goto IL_03D2;
								}
								num3 = Math.Min(9, -num);
							}
							else
							{
								flag = true;
								if (num == 28 || (num3 = decimal.DecCalc.SearchScale(ref buf, num)) == 0)
								{
									break;
								}
							}
							uint num4 = decimal.DecCalc.s_powers10[num3];
							num += num3;
							if (decimal.DecCalc.IncreaseScale(ref buf, num4) != 0U)
							{
								goto IL_048A;
							}
							buf2.U3 = decimal.DecCalc.IncreaseScale(ref *(decimal.DecCalc.Buf12*)(&buf2), num4);
							num7 = decimal.DecCalc.Div128By96(ref buf2, ref buf3);
							if (!decimal.DecCalc.Add32To96(ref buf, num7))
							{
								goto Block_33;
							}
						}
						if (buf2.U2 < 0U)
						{
							goto IL_0449;
						}
						num7 = buf2.U1 >> 31;
						buf2.Low64 <<= 1;
						buf2.U2 = (buf2.U2 << 1) + num7;
						if (buf2.U2 > buf3.U2)
						{
							goto IL_0449;
						}
						if (buf2.U2 != buf3.U2)
						{
							goto IL_03D2;
						}
						if (buf2.Low64 > buf3.Low64)
						{
							goto IL_0449;
						}
						if (buf2.Low64 == buf3.Low64 && (buf.U0 & 1U) != 0U)
						{
							goto IL_0449;
						}
						goto IL_03D2;
						Block_33:
						num = decimal.DecCalc.OverflowUnscale(ref buf, num, (buf2.Low64 | buf2.High64) > 0UL);
					}
				}
				IL_03D2:
				if (flag)
				{
					uint u = buf.U0;
					ulong high = buf.High64;
					decimal.DecCalc.Unscale(ref u, ref high, ref num);
					d1.Low = u;
					d1.Mid = (uint)high;
					d1.High = (uint)(high >> 32);
				}
				else
				{
					d1.Low64 = buf.Low64;
					d1.High = buf.U2;
				}
				d1.uflags = ((d1.uflags ^ d2.uflags) & 2147483648U) | (uint)((uint)num << 16);
				return;
				IL_0449:
				ulong num10 = buf.Low64 + 1UL;
				buf.Low64 = num10;
				if (num10 != 0UL)
				{
					goto IL_03D2;
				}
				uint num11 = buf.U2 + 1U;
				buf.U2 = num11;
				if (num11 == 0U)
				{
					num = decimal.DecCalc.OverflowUnscale(ref buf, num, true);
					goto IL_03D2;
				}
				goto IL_03D2;
				IL_048A:
				throw new OverflowException("Value was either too large or too small for a Decimal.");
			}

			internal static void VarDecMod(ref decimal.DecCalc d1, ref decimal.DecCalc d2)
			{
				if ((d2.ulo | d2.umid | d2.uhi) == 0U)
				{
					throw new DivideByZeroException();
				}
				if ((d1.ulo | d1.umid | d1.uhi) == 0U)
				{
					return;
				}
				d2.uflags = (d2.uflags & 2147483647U) | (d1.uflags & 2147483648U);
				int num = decimal.DecCalc.VarDecCmpSub(Unsafe.As<decimal.DecCalc, decimal>(ref d1), Unsafe.As<decimal.DecCalc, decimal>(ref d2));
				if (num == 0)
				{
					d1.ulo = 0U;
					d1.umid = 0U;
					d1.uhi = 0U;
					if (d2.uflags > d1.uflags)
					{
						d1.uflags = d2.uflags;
					}
					return;
				}
				if ((num ^ (int)(d1.uflags & 2147483648U)) < 0)
				{
					return;
				}
				int num2 = (int)((sbyte)(d1.uflags - d2.uflags >> 16));
				if (num2 > 0)
				{
					do
					{
						uint num3 = ((num2 >= 9) ? 1000000000U : decimal.DecCalc.s_powers10[num2]);
						ulong num4 = decimal.DecCalc.UInt32x32To64(d2.Low, num3);
						d2.Low = (uint)num4;
						num4 >>= 32;
						num4 += ((ulong)d2.Mid + ((ulong)d2.High << 32)) * (ulong)num3;
						d2.Mid = (uint)num4;
						d2.High = (uint)(num4 >> 32);
					}
					while ((num2 -= 9) > 0);
					num2 = 0;
				}
				for (;;)
				{
					if (num2 < 0)
					{
						d1.uflags = d2.uflags;
						decimal.DecCalc.Buf12 buf;
						buf.Low64 = d1.Low64;
						buf.U2 = d1.High;
						uint num6;
						do
						{
							int num5 = decimal.DecCalc.SearchScale(ref buf, 28 + num2);
							if (num5 == 0)
							{
								break;
							}
							num6 = ((num5 >= 9) ? 1000000000U : decimal.DecCalc.s_powers10[num5]);
							num2 += num5;
							ulong num7 = decimal.DecCalc.UInt32x32To64(buf.U0, num6);
							buf.U0 = (uint)num7;
							num7 >>= 32;
							buf.High64 = num7 + buf.High64 * (ulong)num6;
						}
						while (num6 == 1000000000U && num2 < 0);
						d1.Low64 = buf.Low64;
						d1.High = buf.U2;
					}
					if (d1.High == 0U)
					{
						break;
					}
					if ((d2.High | d2.Mid) != 0U)
					{
						goto IL_024C;
					}
					uint low = d2.Low;
					ulong num8 = ((ulong)d1.High << 32) | (ulong)d1.Mid;
					num8 = (num8 % (ulong)low << 32) | (ulong)d1.Low;
					d1.Low64 = num8 % (ulong)low;
					d1.High = 0U;
					if (num2 >= 0)
					{
						return;
					}
				}
				d1.Low64 %= d2.Low64;
				return;
				IL_024C:
				decimal.DecCalc.VarDecModFull(ref d1, ref d2, num2);
			}

			private unsafe static void VarDecModFull(ref decimal.DecCalc d1, ref decimal.DecCalc d2, int scale)
			{
				uint num = d2.High;
				if (num == 0U)
				{
					num = d2.Mid;
				}
				int num2 = decimal.DecCalc.LeadingZeroCount(num);
				decimal.DecCalc.Buf28 buf;
				buf.Buf24.Low64 = d1.Low64 << num2;
				buf.Buf24.Mid64 = (ulong)d1.Mid + ((ulong)d1.High << 32) >> 32 - num2;
				uint num3 = 3U;
				while (scale < 0)
				{
					uint num4 = ((scale <= -9) ? 1000000000U : decimal.DecCalc.s_powers10[-scale]);
					uint* ptr = (uint*)(&buf);
					ulong num5 = decimal.DecCalc.UInt32x32To64(buf.Buf24.U0, num4);
					buf.Buf24.U0 = (uint)num5;
					int num6 = 1;
					while ((long)num6 <= (long)((ulong)num3))
					{
						num5 >>= 32;
						num5 += decimal.DecCalc.UInt32x32To64(ptr[num6], num4);
						ptr[num6] = (uint)num5;
						num6++;
					}
					if (num5 > 2147483647UL)
					{
						ptr[(IntPtr)((ulong)(num3 += 1U) * 4UL)] = (uint)(num5 >> 32);
					}
					scale += 9;
				}
				if (d2.High == 0U)
				{
					ulong num7 = d2.Low64 << num2;
					switch (num3)
					{
					case 4U:
						goto IL_015A;
					case 5U:
						break;
					case 6U:
						decimal.DecCalc.Div96By64(ref *(decimal.DecCalc.Buf12*)(&buf.Buf24.U4), num7);
						break;
					default:
						goto IL_016F;
					}
					decimal.DecCalc.Div96By64(ref *(decimal.DecCalc.Buf12*)(&buf.Buf24.U3), num7);
					IL_015A:
					decimal.DecCalc.Div96By64(ref *(decimal.DecCalc.Buf12*)(&buf.Buf24.U2), num7);
					IL_016F:
					decimal.DecCalc.Div96By64(ref *(decimal.DecCalc.Buf12*)(&buf.Buf24.U1), num7);
					decimal.DecCalc.Div96By64(ref *(decimal.DecCalc.Buf12*)(&buf), num7);
					d1.Low64 = buf.Buf24.Low64 >> num2;
					d1.High = 0U;
					return;
				}
				decimal.DecCalc.Buf12 buf2;
				buf2.Low64 = d2.Low64 << num2;
				buf2.U2 = (uint)((ulong)d2.Mid + ((ulong)d2.High << 32) >> 32 - num2);
				switch (num3)
				{
				case 4U:
					goto IL_0225;
				case 5U:
					break;
				case 6U:
					decimal.DecCalc.Div128By96(ref *(decimal.DecCalc.Buf16*)(&buf.Buf24.U3), ref buf2);
					break;
				default:
					goto IL_023A;
				}
				decimal.DecCalc.Div128By96(ref *(decimal.DecCalc.Buf16*)(&buf.Buf24.U2), ref buf2);
				IL_0225:
				decimal.DecCalc.Div128By96(ref *(decimal.DecCalc.Buf16*)(&buf.Buf24.U1), ref buf2);
				IL_023A:
				decimal.DecCalc.Div128By96(ref *(decimal.DecCalc.Buf16*)(&buf), ref buf2);
				d1.Low64 = (buf.Buf24.Low64 >> num2) + ((ulong)buf.Buf24.U2 << 32 - num2 << 32);
				d1.High = buf.Buf24.U2 >> num2;
			}

			internal static void InternalRound(ref decimal.DecCalc d, uint scale, decimal.DecCalc.RoundingMode mode)
			{
				d.uflags -= scale << 16;
				uint num = 0U;
				uint num5;
				while (scale >= 9U)
				{
					scale -= 9U;
					uint num2 = d.uhi;
					uint num4;
					if (num2 == 0U)
					{
						ulong low = d.Low64;
						ulong num3 = low / 1000000000UL;
						d.Low64 = num3;
						num4 = (uint)(low - num3 * 1000000000UL);
					}
					else
					{
						num4 = num2 - (d.uhi = num2 / 1000000000U) * 1000000000U;
						num2 = d.umid;
						if ((num2 | num4) != 0U)
						{
							num4 = num2 - (d.umid = (uint)((((ulong)num4 << 32) | (ulong)num2) / 1000000000UL)) * 1000000000U;
						}
						num2 = d.ulo;
						if ((num2 | num4) != 0U)
						{
							num4 = num2 - (d.ulo = (uint)((((ulong)num4 << 32) | (ulong)num2) / 1000000000UL)) * 1000000000U;
						}
					}
					num5 = 1000000000U;
					if (scale == 0U)
					{
						IL_0194:
						if (mode != decimal.DecCalc.RoundingMode.Truncate)
						{
							if (mode == decimal.DecCalc.RoundingMode.ToEven)
							{
								num4 <<= 1;
								if ((num | (d.ulo & 1U)) != 0U)
								{
									num4 += 1U;
								}
								if (num5 >= num4)
								{
									return;
								}
							}
							else if (mode == decimal.DecCalc.RoundingMode.AwayFromZero)
							{
								num4 <<= 1;
								if (num5 > num4)
								{
									return;
								}
							}
							else if (mode == decimal.DecCalc.RoundingMode.Floor)
							{
								if ((num4 | num) == 0U)
								{
									return;
								}
								if (!d.IsNegative)
								{
									return;
								}
							}
							else if ((num4 | num) == 0U || d.IsNegative)
							{
								return;
							}
							ulong num6 = d.Low64 + 1UL;
							d.Low64 = num6;
							if (num6 == 0UL)
							{
								d.uhi += 1U;
							}
						}
						return;
					}
					num |= num4;
				}
				num5 = decimal.DecCalc.s_powers10[(int)scale];
				uint num7 = d.uhi;
				if (num7 == 0U)
				{
					ulong low2 = d.Low64;
					if (low2 != 0UL)
					{
						ulong num8 = low2 / (ulong)num5;
						d.Low64 = num8;
						uint num4 = (uint)(low2 - num8 * (ulong)num5);
						goto IL_0194;
					}
					if (mode > decimal.DecCalc.RoundingMode.Truncate)
					{
						uint num4 = 0U;
						goto IL_0194;
					}
					return;
				}
				else
				{
					uint num4 = num7 - (d.uhi = num7 / num5) * num5;
					num7 = d.umid;
					if ((num7 | num4) != 0U)
					{
						num4 = num7 - (d.umid = (uint)((((ulong)num4 << 32) | (ulong)num7) / (ulong)num5)) * num5;
					}
					num7 = d.ulo;
					if ((num7 | num4) != 0U)
					{
						num4 = num7 - (d.ulo = (uint)((((ulong)num4 << 32) | (ulong)num7) / (ulong)num5)) * num5;
						goto IL_0194;
					}
					goto IL_0194;
				}
			}

			internal static uint DecDivMod1E9(ref decimal.DecCalc value)
			{
				ulong num = ((ulong)value.uhi << 32) + (ulong)value.umid;
				ulong num2 = num / 1000000000UL;
				value.uhi = (uint)(num2 >> 32);
				value.umid = (uint)num2;
				ulong num3 = (num - (ulong)((uint)num2 * 1000000000U) << 32) + (ulong)value.ulo;
				uint num4 = (uint)(num3 / 1000000000UL);
				value.ulo = num4;
				return (uint)num3 - num4 * 1000000000U;
			}

			[FieldOffset(0)]
			private uint uflags;

			[FieldOffset(4)]
			private uint uhi;

			[FieldOffset(8)]
			private uint ulo;

			[FieldOffset(12)]
			private uint umid;

			[FieldOffset(8)]
			private ulong ulomidLE;

			private const uint SignMask = 2147483648U;

			private const uint ScaleMask = 16711680U;

			private const int DEC_SCALE_MAX = 28;

			private const uint TenToPowerNine = 1000000000U;

			private const ulong TenToPowerEighteen = 1000000000000000000UL;

			private const int MaxInt32Scale = 9;

			private const int MaxInt64Scale = 19;

			private static readonly uint[] s_powers10 = new uint[] { 1U, 10U, 100U, 1000U, 10000U, 100000U, 1000000U, 10000000U, 100000000U, 1000000000U };

			private static readonly ulong[] s_ulongPowers10 = new ulong[]
			{
				10UL, 100UL, 1000UL, 10000UL, 100000UL, 1000000UL, 10000000UL, 100000000UL, 1000000000UL, 10000000000UL,
				100000000000UL, 1000000000000UL, 10000000000000UL, 100000000000000UL, 1000000000000000UL, 10000000000000000UL, 100000000000000000UL, 1000000000000000000UL, 10000000000000000000UL
			};

			private static readonly double[] s_doublePowers10 = new double[]
			{
				1.0, 10.0, 100.0, 1000.0, 10000.0, 100000.0, 1000000.0, 10000000.0, 100000000.0, 1000000000.0,
				10000000000.0, 100000000000.0, 1000000000000.0, 10000000000000.0, 100000000000000.0, 1000000000000000.0, 10000000000000000.0, 1E+17, 1E+18, 1E+19,
				1E+20, 1E+21, 1E+22, 1E+23, 1E+24, 1E+25, 1E+26, 1E+27, 1E+28, 1E+29,
				1E+30, 1E+31, 1E+32, 1E+33, 1E+34, 1E+35, 1E+36, 1E+37, 1E+38, 1E+39,
				1E+40, 1E+41, 1E+42, 1E+43, 1E+44, 1E+45, 1E+46, 1E+47, 1E+48, 1E+49,
				1E+50, 1E+51, 1E+52, 1E+53, 1E+54, 1E+55, 1E+56, 1E+57, 1E+58, 1E+59,
				1E+60, 1E+61, 1E+62, 1E+63, 1E+64, 1E+65, 1E+66, 1E+67, 1E+68, 1E+69,
				1E+70, 1E+71, 1E+72, 1E+73, 1E+74, 1E+75, 1E+76, 1E+77, 1E+78, 1E+79,
				1E+80
			};

			private static readonly decimal.DecCalc.PowerOvfl[] PowerOvflValues = new decimal.DecCalc.PowerOvfl[]
			{
				new decimal.DecCalc.PowerOvfl(429496729U, 2576980377U, 2576980377U),
				new decimal.DecCalc.PowerOvfl(42949672U, 4123168604U, 687194767U),
				new decimal.DecCalc.PowerOvfl(4294967U, 1271310319U, 2645699854U),
				new decimal.DecCalc.PowerOvfl(429496U, 3133608139U, 694066715U),
				new decimal.DecCalc.PowerOvfl(42949U, 2890341191U, 2216890319U),
				new decimal.DecCalc.PowerOvfl(4294U, 4154504685U, 2369172679U),
				new decimal.DecCalc.PowerOvfl(429U, 2133437386U, 4102387834U),
				new decimal.DecCalc.PowerOvfl(42U, 4078814305U, 410238783U)
			};

			internal enum RoundingMode
			{
				ToEven,
				AwayFromZero,
				Truncate,
				Floor,
				Ceiling
			}

			private struct PowerOvfl
			{
				public PowerOvfl(uint hi, uint mid, uint lo)
				{
					this.Hi = hi;
					this.MidLo = ((ulong)mid << 32) + (ulong)lo;
				}

				public readonly uint Hi;

				public readonly ulong MidLo;
			}

			[StructLayout(LayoutKind.Explicit)]
			private struct Buf12
			{
				public ulong Low64
				{
					get
					{
						if (!BitConverter.IsLittleEndian)
						{
							return ((ulong)this.U1 << 32) | (ulong)this.U0;
						}
						return this.ulo64LE;
					}
					set
					{
						if (BitConverter.IsLittleEndian)
						{
							this.ulo64LE = value;
							return;
						}
						this.U1 = (uint)(value >> 32);
						this.U0 = (uint)value;
					}
				}

				public ulong High64
				{
					get
					{
						if (!BitConverter.IsLittleEndian)
						{
							return ((ulong)this.U2 << 32) | (ulong)this.U1;
						}
						return this.uhigh64LE;
					}
					set
					{
						if (BitConverter.IsLittleEndian)
						{
							this.uhigh64LE = value;
							return;
						}
						this.U2 = (uint)(value >> 32);
						this.U1 = (uint)value;
					}
				}

				[FieldOffset(0)]
				public uint U0;

				[FieldOffset(4)]
				public uint U1;

				[FieldOffset(8)]
				public uint U2;

				[FieldOffset(0)]
				private ulong ulo64LE;

				[FieldOffset(4)]
				private ulong uhigh64LE;
			}

			[StructLayout(LayoutKind.Explicit)]
			private struct Buf16
			{
				public ulong Low64
				{
					get
					{
						if (!BitConverter.IsLittleEndian)
						{
							return ((ulong)this.U1 << 32) | (ulong)this.U0;
						}
						return this.ulo64LE;
					}
					set
					{
						if (BitConverter.IsLittleEndian)
						{
							this.ulo64LE = value;
							return;
						}
						this.U1 = (uint)(value >> 32);
						this.U0 = (uint)value;
					}
				}

				public ulong High64
				{
					get
					{
						if (!BitConverter.IsLittleEndian)
						{
							return ((ulong)this.U3 << 32) | (ulong)this.U2;
						}
						return this.uhigh64LE;
					}
					set
					{
						if (BitConverter.IsLittleEndian)
						{
							this.uhigh64LE = value;
							return;
						}
						this.U3 = (uint)(value >> 32);
						this.U2 = (uint)value;
					}
				}

				[FieldOffset(0)]
				public uint U0;

				[FieldOffset(4)]
				public uint U1;

				[FieldOffset(8)]
				public uint U2;

				[FieldOffset(12)]
				public uint U3;

				[FieldOffset(0)]
				private ulong ulo64LE;

				[FieldOffset(8)]
				private ulong uhigh64LE;
			}

			[StructLayout(LayoutKind.Explicit)]
			private struct Buf24
			{
				public ulong Low64
				{
					get
					{
						if (!BitConverter.IsLittleEndian)
						{
							return ((ulong)this.U1 << 32) | (ulong)this.U0;
						}
						return this.ulo64LE;
					}
					set
					{
						if (BitConverter.IsLittleEndian)
						{
							this.ulo64LE = value;
							return;
						}
						this.U1 = (uint)(value >> 32);
						this.U0 = (uint)value;
					}
				}

				public ulong Mid64
				{
					get
					{
						if (!BitConverter.IsLittleEndian)
						{
							return ((ulong)this.U3 << 32) | (ulong)this.U2;
						}
						return this.umid64LE;
					}
					set
					{
						if (BitConverter.IsLittleEndian)
						{
							this.umid64LE = value;
							return;
						}
						this.U3 = (uint)(value >> 32);
						this.U2 = (uint)value;
					}
				}

				public ulong High64
				{
					get
					{
						if (!BitConverter.IsLittleEndian)
						{
							return ((ulong)this.U5 << 32) | (ulong)this.U4;
						}
						return this.uhigh64LE;
					}
					set
					{
						if (BitConverter.IsLittleEndian)
						{
							this.uhigh64LE = value;
							return;
						}
						this.U5 = (uint)(value >> 32);
						this.U4 = (uint)value;
					}
				}

				public int Length
				{
					get
					{
						return 6;
					}
				}

				[FieldOffset(0)]
				public uint U0;

				[FieldOffset(4)]
				public uint U1;

				[FieldOffset(8)]
				public uint U2;

				[FieldOffset(12)]
				public uint U3;

				[FieldOffset(16)]
				public uint U4;

				[FieldOffset(20)]
				public uint U5;

				[FieldOffset(0)]
				private ulong ulo64LE;

				[FieldOffset(8)]
				private ulong umid64LE;

				[FieldOffset(16)]
				private ulong uhigh64LE;
			}

			private struct Buf28
			{
				public int Length
				{
					get
					{
						return 7;
					}
				}

				public decimal.DecCalc.Buf24 Buf24;

				public uint U6;
			}
		}
	}
}
