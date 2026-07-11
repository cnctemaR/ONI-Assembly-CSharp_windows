using System;
using System.Globalization;

namespace System.ComponentModel
{
	public class UInt16Converter : BaseNumberConverter
	{
		public UInt16Converter()
		{
			this.InnerType = typeof(ushort);
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
			return ((ushort)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return ushort.Parse(value, NumberStyles.Integer, format);
		}

		internal override object ConvertFromString(string value, int fromBase)
		{
			return Convert.ToUInt16(value, fromBase);
		}
	}
}
