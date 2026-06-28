using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	public sealed class TypeNameConverter : ConfigurationConverterBase
	{
		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			return Type.GetType((string)data);
		}

		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			if (value == null)
			{
				return null;
			}
			if (!(value is Type))
			{
				throw new ArgumentException("value");
			}
			return ((Type)value).AssemblyQualifiedName;
		}
	}
}
