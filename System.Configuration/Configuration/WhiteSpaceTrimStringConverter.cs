using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	public sealed class WhiteSpaceTrimStringConverter : ConfigurationConverterBase
	{
		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			return ((string)data).Trim();
		}

		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			if (value == null)
			{
				return string.Empty;
			}
			if (!(value is string))
			{
				throw new ArgumentException("value");
			}
			return ((string)value).Trim();
		}
	}
}
