using System;
using System.Globalization;

namespace System.ComponentModel
{
	public class SingleConverter : BaseNumberConverter
	{
		public SingleConverter()
		{
			this.InnerType = typeof(float);
		}

		internal override bool SupportHex
		{
			get
			{
				return false;
			}
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((float)value).ToString("R", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return float.Parse(value, NumberStyles.Float, format);
		}
	}
}
