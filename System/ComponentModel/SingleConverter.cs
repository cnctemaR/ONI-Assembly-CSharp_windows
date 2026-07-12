using System;
using System.Globalization;

namespace System.ComponentModel
{
	public class SingleConverter : BaseNumberConverter
	{
		internal override bool AllowHex
		{
			get
			{
				return false;
			}
		}

		internal override Type TargetType
		{
			get
			{
				return typeof(float);
			}
		}

		internal override object FromString(string value, int radix)
		{
			return Convert.ToSingle(value, CultureInfo.CurrentCulture);
		}

		internal override object FromString(string value, NumberFormatInfo formatInfo)
		{
			return float.Parse(value, NumberStyles.Float, formatInfo);
		}

		internal override string ToString(object value, NumberFormatInfo formatInfo)
		{
			return ((float)value).ToString("R", formatInfo);
		}
	}
}
