using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	public sealed class InfiniteIntConverter : ConfigurationConverterBase
	{
		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			if ((string)data == "Infinite")
			{
				return int.MaxValue;
			}
			return Convert.ToInt32((string)data, 10);
		}

		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			if (value.GetType() != typeof(int))
			{
				throw new ArgumentException();
			}
			if ((int)value == 2147483647)
			{
				return "Infinite";
			}
			return value.ToString();
		}
	}
}
