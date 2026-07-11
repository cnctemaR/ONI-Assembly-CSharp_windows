using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	public class TimeSpanMinutesConverter : ConfigurationConverterBase
	{
		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			return TimeSpan.FromMinutes((double)long.Parse((string)data));
		}

		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			if (value.GetType() != typeof(TimeSpan))
			{
				throw new ArgumentException();
			}
			return ((long)((TimeSpan)value).TotalMinutes).ToString();
		}
	}
}
