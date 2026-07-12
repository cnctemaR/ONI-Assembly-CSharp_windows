using System;
using System.Collections;
using System.Globalization;

namespace System.ComponentModel
{
	public class NullableConverter : TypeConverter
	{
		public NullableConverter(Type type)
		{
			this.NullableType = type;
			this.UnderlyingType = Nullable.GetUnderlyingType(type);
			if (this.UnderlyingType == null)
			{
				throw new ArgumentException("The specified type is not a nullable type.", "type");
			}
			this.UnderlyingTypeConverter = TypeDescriptor.GetConverter(this.UnderlyingType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == this.UnderlyingType)
			{
				return true;
			}
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.CanConvertFrom(context, sourceType);
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null || value.GetType() == this.UnderlyingType)
			{
				return value;
			}
			if (value is string && string.IsNullOrEmpty(value as string))
			{
				return null;
			}
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.ConvertFrom(context, culture, value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == this.UnderlyingType)
			{
				return true;
			}
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.CanConvertTo(context, destinationType);
			}
			return base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == this.UnderlyingType && value != null && this.NullableType.IsInstanceOfType(value))
			{
				return value;
			}
			if (value == null)
			{
				if (destinationType == typeof(string))
				{
					return string.Empty;
				}
			}
			else if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.ConvertTo(context, culture, value, destinationType);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.CreateInstance(context, propertyValues);
			}
			return base.CreateInstance(context, propertyValues);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.GetCreateInstanceSupported(context);
			}
			return base.GetCreateInstanceSupported(context);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.GetProperties(context, value, attributes);
			}
			return base.GetProperties(context, value, attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.GetPropertiesSupported(context);
			}
			return base.GetPropertiesSupported(context);
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this.UnderlyingTypeConverter != null)
			{
				TypeConverter.StandardValuesCollection standardValues = this.UnderlyingTypeConverter.GetStandardValues(context);
				if (this.GetStandardValuesSupported(context) && standardValues != null)
				{
					object[] array = new object[standardValues.Count + 1];
					int num = 0;
					array[num++] = null;
					foreach (object obj in standardValues)
					{
						array[num++] = obj;
					}
					return new TypeConverter.StandardValuesCollection(array);
				}
			}
			return base.GetStandardValues(context);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.GetStandardValuesExclusive(context);
			}
			return base.GetStandardValuesExclusive(context);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			if (this.UnderlyingTypeConverter != null)
			{
				return this.UnderlyingTypeConverter.GetStandardValuesSupported(context);
			}
			return base.GetStandardValuesSupported(context);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (this.UnderlyingTypeConverter != null)
			{
				return value == null || this.UnderlyingTypeConverter.IsValid(context, value);
			}
			return base.IsValid(context, value);
		}

		public Type NullableType { get; }

		public Type UnderlyingType { get; }

		public TypeConverter UnderlyingTypeConverter { get; }
	}
}
