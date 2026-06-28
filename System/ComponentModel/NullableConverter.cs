using System;
using System.Collections;
using System.Globalization;

namespace System.ComponentModel
{
	public class NullableConverter : TypeConverter
	{
		public NullableConverter(Type nullableType)
		{
			if (nullableType == null)
			{
				throw new ArgumentNullException("nullableType");
			}
			this.nullableType = nullableType;
			this.underlyingType = Nullable.GetUnderlyingType(nullableType);
			this.underlyingTypeConverter = TypeDescriptor.GetConverter(this.underlyingType);
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == this.underlyingType)
			{
				return true;
			}
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.CanConvertFrom(context, sourceType);
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == this.underlyingType)
			{
				return true;
			}
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.CanConvertTo(context, destinationType);
			}
			return base.CanConvertFrom(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null || value.GetType() == this.underlyingType)
			{
				return value;
			}
			if (value is string && string.IsNullOrEmpty((string)value))
			{
				return null;
			}
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.ConvertFrom(context, culture, value);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == this.underlyingType && value.GetType() == this.nullableType)
			{
				return value;
			}
			if (this.underlyingTypeConverter != null && value != null)
			{
				return this.underlyingTypeConverter.ConvertTo(context, culture, value, destinationType);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.CreateInstance(context, propertyValues);
			}
			return base.CreateInstance(context, propertyValues);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.GetCreateInstanceSupported(context);
			}
			return base.GetCreateInstanceSupported(context);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.GetProperties(context, value, attributes);
			}
			return base.GetProperties(context, value, attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.GetCreateInstanceSupported(context);
			}
			return base.GetCreateInstanceSupported(context);
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this.underlyingTypeConverter != null && this.underlyingTypeConverter.GetStandardValuesSupported(context))
			{
				TypeConverter.StandardValuesCollection standardValues = this.underlyingTypeConverter.GetStandardValues(context);
				if (standardValues != null)
				{
					return new TypeConverter.StandardValuesCollection(new ArrayList(standardValues) { null });
				}
			}
			return base.GetStandardValues(context);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.GetStandardValuesExclusive(context);
			}
			return base.GetStandardValuesExclusive(context);
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.GetStandardValuesSupported(context);
			}
			return base.GetStandardValuesSupported(context);
		}

		public override bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (this.underlyingTypeConverter != null)
			{
				return this.underlyingTypeConverter.IsValid(context, value);
			}
			return base.IsValid(context, value);
		}

		public Type NullableType
		{
			get
			{
				return this.nullableType;
			}
		}

		public Type UnderlyingType
		{
			get
			{
				return this.underlyingType;
			}
		}

		public TypeConverter UnderlyingTypeConverter
		{
			get
			{
				return this.underlyingTypeConverter;
			}
		}

		private Type nullableType;

		private Type underlyingType;

		private TypeConverter underlyingTypeConverter;
	}
}
