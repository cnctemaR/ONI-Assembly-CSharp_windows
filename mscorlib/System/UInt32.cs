using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;

namespace System
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[Serializable]
	public struct UInt32 : IComparable, IFormattable, IConvertible, IComparable<uint>, IEquatable<uint>
	{
		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is uint))
			{
				throw new ArgumentException(Environment.GetResourceString("Object must be of type UInt32."));
			}
			uint num = (uint)value;
			if (this < num)
			{
				return -1;
			}
			if (this > num)
			{
				return 1;
			}
			return 0;
		}

		public int CompareTo(uint value)
		{
			if (this < value)
			{
				return -1;
			}
			if (this > value)
			{
				return 1;
			}
			return 0;
		}

		public override bool Equals(object obj)
		{
			return obj is uint && this == (uint)obj;
		}

		public bool Equals(uint obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return (int)this;
		}

		[SecuritySafeCritical]
		public override string ToString()
		{
			return Number.FormatUInt32(this, null, NumberFormatInfo.CurrentInfo);
		}

		[SecuritySafeCritical]
		public string ToString(IFormatProvider provider)
		{
			return Number.FormatUInt32(this, null, NumberFormatInfo.GetInstance(provider));
		}

		[SecuritySafeCritical]
		public string ToString(string format)
		{
			return Number.FormatUInt32(this, format, NumberFormatInfo.CurrentInfo);
		}

		[SecuritySafeCritical]
		public string ToString(string format, IFormatProvider provider)
		{
			return Number.FormatUInt32(this, format, NumberFormatInfo.GetInstance(provider));
		}

		[CLSCompliant(false)]
		public static uint Parse(string s)
		{
			return Number.ParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
		}

		[CLSCompliant(false)]
		public static uint Parse(string s, NumberStyles style)
		{
			NumberFormatInfo.ValidateParseStyleInteger(style);
			return Number.ParseUInt32(s, style, NumberFormatInfo.CurrentInfo);
		}

		[CLSCompliant(false)]
		public static uint Parse(string s, IFormatProvider provider)
		{
			return Number.ParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
		}

		[CLSCompliant(false)]
		public static uint Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			NumberFormatInfo.ValidateParseStyleInteger(style);
			return Number.ParseUInt32(s, style, NumberFormatInfo.GetInstance(provider));
		}

		[CLSCompliant(false)]
		public static bool TryParse(string s, out uint result)
		{
			return Number.TryParseUInt32(s, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
		}

		[CLSCompliant(false)]
		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out uint result)
		{
			NumberFormatInfo.ValidateParseStyleInteger(style);
			return Number.TryParseUInt32(s, style, NumberFormatInfo.GetInstance(provider), out result);
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.UInt32;
		}

		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this);
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
			return this;
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
			return Convert.ToDecimal(this);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			throw new InvalidCastException(Environment.GetResourceString("Invalid cast from '{0}' to '{1}'.", new object[] { "UInt32", "DateTime" }));
		}

		object IConvertible.ToType(Type type, IFormatProvider provider)
		{
			return Convert.DefaultToType(this, type, provider);
		}

		private uint m_value;

		public const uint MaxValue = 4294967295U;

		public const uint MinValue = 0U;
	}
}
