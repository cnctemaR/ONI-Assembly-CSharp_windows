using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	public class TimeSpanSecondsConverter : ConfigurationConverterBase
	{
		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			if (!(data is string))
			{
				throw new ArgumentException("data");
			}
			long num;
			if (!long.TryParse((string)data, out num))
			{
				throw new ArgumentException("data");
			}
			return TimeSpan.FromSeconds((double)num);
		}

		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			if (value.GetType() != typeof(TimeSpan))
			{
				throw new ArgumentException();
			}
			return ((long)((TimeSpan)value).TotalSeconds).ToString();
		}
	}
}
