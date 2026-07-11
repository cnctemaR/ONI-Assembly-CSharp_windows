using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[Serializable]
	public struct SByte : IFormattable, IConvertible, IComparable, IComparable<sbyte>, IEquatable<sbyte>
	{
		bool IConvertible.ToBoolean(IFormatProvider provider)
		{
			return Convert.ToBoolean(this);
		}

		byte IConvertible.ToByte(IFormatProvider provider)
		{
			return Convert.ToByte(this);
		}

		char IConvertible.ToChar(IFormatProvider provider)
		{
			return Convert.ToChar(this);
		}

		DateTime IConvertible.ToDateTime(IFormatProvider provider)
		{
			return Convert.ToDateTime(this);
		}

		decimal IConvertible.ToDecimal(IFormatProvider provider)
		{
			return Convert.ToDecimal(this);
		}

		double IConvertible.ToDouble(IFormatProvider provider)
		{
			return Convert.ToDouble(this);
		}

		short IConvertible.ToInt16(IFormatProvider provider)
		{
			return Convert.ToInt16(this);
		}

		int IConvertible.ToInt32(IFormatProvider provider)
		{
			return Convert.ToInt32(this);
		}

		long IConvertible.ToInt64(IFormatProvider provider)
		{
			return Convert.ToInt64(this);
		}

		sbyte IConvertible.ToSByte(IFormatProvider provider)
		{
			return this;
		}

		float IConvertible.ToSingle(IFormatProvider provider)
		{
			return Convert.ToSingle(this);
		}

		object IConvertible.ToType(Type targetType, IFormatProvider provider)
		{
			if (targetType == null)
			{
				throw new ArgumentNullException("targetType");
			}
			return Convert.ToType(this, targetType, provider, false);
		}

		ushort IConvertible.ToUInt16(IFormatProvider provider)
		{
			return Convert.ToUInt16(this);
		}

		uint IConvertible.ToUInt32(IFormatProvider provider)
		{
			return Convert.ToUInt32(this);
		}

		ulong IConvertible.ToUInt64(IFormatProvider provider)
		{
			return Convert.ToUInt64(this);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is sbyte))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.SByte."));
			}
			sbyte b = (sbyte)obj;
			if ((int)this == (int)b)
			{
				return 0;
			}
			if ((int)this > (int)b)
			{
				return 1;
			}
			return -1;
		}

		public override bool Equals(object obj)
		{
			return obj is sbyte && (int)((sbyte)obj) == (int)this;
		}

		public override int GetHashCode()
		{
			return (int)this;
		}

		public int CompareTo(sbyte value)
		{
			if ((int)this == (int)value)
			{
				return 0;
			}
			if ((int)this > (int)value)
			{
				return 1;
			}
			return -1;
		}

		public bool Equals(sbyte obj)
		{
			return (int)obj == (int)this;
		}

		internal static bool Parse(string s, bool tryParse, out sbyte result, out Exception exc)
		{
			int num = 0;
			bool flag = false;
			bool flag2 = false;
			result = 0;
			exc = null;
			if (s == null)
			{
				if (!tryParse)
				{
					exc = new ArgumentNullException("s");
				}
				return false;
			}
			int length = s.Length;
			int i;
			char c;
			for (i = 0; i < length; i++)
			{
				c = s[i];
				if (!char.IsWhiteSpace(c))
				{
					break;
				}
			}
			if (i == length)
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			c = s[i];
			if (c == '+')
			{
				i++;
			}
			else if (c == '-')
			{
				flag = true;
				i++;
			}
			while (i < length)
			{
				c = s[i];
				if (c >= '0' && c <= '9')
				{
					if (tryParse)
					{
						int num2 = num * 10 - (int)(c - '0');
						if (num2 < -128)
						{
							return false;
						}
						num = (int)((sbyte)num2);
					}
					else
					{
						num = checked(num * 10 - (int)(c - '0'));
					}
					flag2 = true;
					i++;
				}
				else
				{
					if (char.IsWhiteSpace(c))
					{
						for (i++; i < length; i++)
						{
							if (!char.IsWhiteSpace(s[i]))
							{
								if (!tryParse)
								{
									exc = int.GetFormatException();
								}
								return false;
							}
						}
						break;
					}
					if (!tryParse)
					{
						exc = int.GetFormatException();
					}
					return false;
				}
			}
			if (!flag2)
			{
				if (!tryParse)
				{
					exc = int.GetFormatException();
				}
				return false;
			}
			num = ((!flag) ? (-num) : num);
			if (num < -128 || num > 127)
			{
				if (!tryParse)
				{
					exc = new OverflowException();
				}
				return false;
			}
			result = (sbyte)num;
			return true;
		}

		[CLSCompliant(false)]
		public static sbyte Parse(string s, IFormatProvider provider)
		{
			return sbyte.Parse(s, NumberStyles.Integer, provider);
		}

		[CLSCompliant(false)]
		public static sbyte Parse(string s, NumberStyles style)
		{
			return sbyte.Parse(s, style, null);
		}

		[CLSCompliant(false)]
		public static sbyte Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			int num = int.Parse(s, style, provider);
			if (num > 127 || num < -128)
			{
				throw new OverflowException(Locale.GetText("Value too large or too small."));
			}
			return (sbyte)num;
		}

		[CLSCompliant(false)]
		public static sbyte Parse(string s)
		{
			sbyte b;
			Exception ex;
			if (!sbyte.Parse(s, false, out b, out ex))
			{
				throw ex;
			}
			return b;
		}

		[CLSCompliant(false)]
		public static bool TryParse(string s, out sbyte result)
		{
			Exception ex;
			if (!sbyte.Parse(s, true, out result, out ex))
			{
				result = 0;
				return false;
			}
			return true;
		}

		[CLSCompliant(false)]
		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out sbyte result)
		{
			result = 0;
			int num;
			if (!int.TryParse(s, style, provider, out num))
			{
				return false;
			}
			if (num > 127 || num < -128)
			{
				return false;
			}
			result = (sbyte)num;
			return true;
		}

		public override string ToString()
		{
			return NumberFormatter.NumberToString((int)this, null);
		}

		public string ToString(IFormatProvider provider)
		{
			return NumberFormatter.NumberToString((int)this, provider);
		}

		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		public string ToString(string format, IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(format, this, provider);
		}

		public TypeCode GetTypeCode()
		{
			return TypeCode.SByte;
		}

		public const sbyte MinValue = -128;

		public const sbyte MaxValue = 127;

		internal sbyte m_value;
	}
}
