using System;
using System.Globalization;

namespace System.ComponentModel
{
	public class SByteConverter : BaseNumberConverter
	{
		public SByteConverter()
		{
			this.InnerType = typeof(sbyte);
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
			return ((sbyte)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return sbyte.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToSByte(value, fromBase);
		}
	}
}
