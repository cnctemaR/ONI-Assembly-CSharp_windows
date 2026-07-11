using System;
using System.Globalization;

namespace System.ComponentModel
{
	public class ByteConverter : BaseNumberConverter
	{
		public ByteConverter()
		{
			this.InnerType = typeof(byte);
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
			return ((byte)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return byte.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToByte(value, fromBase);
		}
	}
}
