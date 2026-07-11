using System;
using System.Globalization;

namespace System.ComponentModel
{
	public abstract class TypeListConverter : TypeConverter
	{
		protected TypeListConverter(Type[] types)
		{
			this.types = types;
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string) && value != null && value.GetType() == typeof(Type))
			{
				return ((Type)value).ToString();
			}
			throw new InvalidCastException("Cannot cast to System.Type");
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return new TypeConverter.StandardValuesCollection(this.types);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private Type[] types;
	}
}
