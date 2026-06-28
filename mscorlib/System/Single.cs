using System;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public struct Single : IFormattable, IConvertible, IComparable, IComparable<float>, IEquatable<float>
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
			return Convert.ToSByte(this);
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

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (!(value is float))
			{
				throw new ArgumentException(Locale.GetText("Value is not a System.Single."));
			}
			float num = (float)value;
			if (float.IsPositiveInfinity(this) && float.IsPositiveInfinity(num))
			{
				return 0;
			}
			if (float.IsNegativeInfinity(this) && float.IsNegativeInfinity(num))
			{
				return 0;
			}
			if (float.IsNaN(num))
			{
				if (float.IsNaN(this))
				{
					return 0;
				}
				return 1;
			}
			else if (float.IsNaN(this))
			{
				if (float.IsNaN(num))
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (this == num)
				{
					return 0;
				}
				if (this > num)
				{
					return 1;
				}
				return -1;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is float))
			{
				return false;
			}
			float num = (float)obj;
			if (float.IsNaN(num))
			{
				return float.IsNaN(this);
			}
			return num == this;
		}

		public int CompareTo(float value)
		{
			if (float.IsPositiveInfinity(this) && float.IsPositiveInfinity(value))
			{
				return 0;
			}
			if (float.IsNegativeInfinity(this) && float.IsNegativeInfinity(value))
			{
				return 0;
			}
			if (float.IsNaN(value))
			{
				if (float.IsNaN(this))
				{
					return 0;
				}
				return 1;
			}
			else if (float.IsNaN(this))
			{
				if (float.IsNaN(value))
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (this == value)
				{
					return 0;
				}
				if (this > value)
				{
					return 1;
				}
				return -1;
			}
		}

		public bool Equals(float obj)
		{
			if (float.IsNaN(obj))
			{
				return float.IsNaN(this);
			}
			return obj == this;
		}

		public override int GetHashCode()
		{
			return (int)this;
		}

		public static bool IsInfinity(float f)
		{
			return f == float.PositiveInfinity || f == float.NegativeInfinity;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static bool IsNaN(float f)
		{
			return f != f;
		}

		public static bool IsNegativeInfinity(float f)
		{
			return f < 0f && (f == float.NegativeInfinity || f == float.PositiveInfinity);
		}

		public static bool IsPositiveInfinity(float f)
		{
			return f > 0f && (f == float.NegativeInfinity || f == float.PositiveInfinity);
		}

		public static float Parse(string s)
		{
			double num = double.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, null);
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}

		public static float Parse(string s, IFormatProvider provider)
		{
			double num = double.Parse(s, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent, provider);
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}

		public static float Parse(string s, NumberStyles style)
		{
			double num = double.Parse(s, style, null);
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}

		public static float Parse(string s, NumberStyles style, IFormatProvider provider)
		{
			double num = double.Parse(s, style, provider);
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				throw new OverflowException();
			}
			return (float)num;
		}

		public static bool TryParse(string s, NumberStyles style, IFormatProvider provider, out float result)
		{
			double num;
			Exception ex;
			if (!double.Parse(s, style, provider, true, out num, out ex))
			{
				result = 0f;
				return false;
			}
			if (num - 3.4028234663852886E+38 > 3.6147112457961776E+29 && !double.IsPositiveInfinity(num))
			{
				result = 0f;
				return false;
			}
			result = (float)num;
			return true;
		}

		public static bool TryParse(string s, out float result)
		{
			return float.TryParse(s, NumberStyles.Any, null, out result);
		}

		public override string ToString()
		{
			return NumberFormatter.NumberToString(this, null);
		}

		public string ToString(IFormatProvider provider)
		{
			return NumberFormatter.NumberToString(this, provider);
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
			return TypeCode.Single;
		}

		public const float Epsilon = 1E-45f;

		public const float MaxValue = 3.4028235E+38f;

		public const float MinValue = -3.4028235E+38f;

		public const float NaN = float.NaN;

		public const float PositiveInfinity = float.PositiveInfinity;

		public const float NegativeInfinity = float.NegativeInfinity;

		private const double MaxValueEpsilon = 3.6147112457961776E+29;

		internal float m_value;
	}
}
