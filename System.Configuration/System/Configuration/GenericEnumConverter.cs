using System;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	public sealed class GenericEnumConverter : ConfigurationConverterBase
	{
		public GenericEnumConverter(Type typeEnum)
		{
			if (typeEnum == null)
			{
				throw new ArgumentNullException("typeEnum");
			}
			this.typeEnum = typeEnum;
		}

		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			if (data == null)
			{
				throw new ArgumentException();
			}
			return Enum.Parse(this.typeEnum, (string)data);
		}

		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			return value.ToString();
		}

		private Type typeEnum;
	}
}
