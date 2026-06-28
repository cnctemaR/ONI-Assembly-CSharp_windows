using System;
using System.Globalization;

namespace System.ComponentModel
{
	public class Int32Converter : BaseNumberConverter
	{
		public Int32Converter()
		{
			this.InnerType = typeof(int);
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
			return ((int)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return int.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToInt32(value, fromBase);
		}
	}
}
