using System;
using System.Globalization;

namespace System.ComponentModel
{
	public class Int64Converter : BaseNumberConverter
	{
		public Int64Converter()
		{
			this.InnerType = typeof(long);
		}

		internal override bool SupportHex
		{
			get
			{
				return true;
			}
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((long)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return long.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToInt64(value, fromBase);
		}
	}
}
