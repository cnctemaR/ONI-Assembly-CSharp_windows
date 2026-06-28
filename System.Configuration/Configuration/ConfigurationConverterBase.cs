using System;
using System.ComponentModel;

namespace System.Configuration
{
	public abstract class ConfigurationConverterBase : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext ctx, Type type)
		{
			return type == typeof(string) || base.CanConvertFrom(ctx, type);
		}

		public override bool CanConvertTo(ITypeDescriptorContext ctx, Type type)
		{
			return type == typeof(string) || base.CanConvertTo(ctx, type);
		}
	}
}
