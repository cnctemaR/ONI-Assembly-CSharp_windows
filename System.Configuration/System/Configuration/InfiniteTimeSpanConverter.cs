using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	public sealed class InfiniteTimeSpanConverter : ConfigurationConverterBase
	{
		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			if ((string)data == "Infinite")
			{
				return TimeSpan.MaxValue;
			}
			return TimeSpan.Parse((string)data);
		}

		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			if (value.GetType() != typeof(TimeSpan))
			{
				throw new ArgumentException();
			}
			if ((TimeSpan)value == TimeSpan.MaxValue)
			{
				return "Infinite";
			}
			return value.ToString();
		}
	}
}
