using System;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class DateTimeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
				string text = (string)value;
				try
				{
					if (text != null && text.Trim().Length == 0)
					{
						return DateTime.MinValue;
					}
					if (culture == null)
					{
						return DateTime.Parse(text);
					}
					DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
					return DateTime.Parse(text, dateTimeFormatInfo);
				}
				catch
				{
					throw new FormatException(text + " is not a valid DateTime value.");
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is DateTime)
			{
				DateTime dateTime = (DateTime)value;
				if (destinationType == typeof(string))
				{
					if (culture == null)
					{
						culture = CultureInfo.CurrentCulture;
					}
					if (dateTime == DateTime.MinValue)
					{
						return string.Empty;
					}
					DateTimeFormatInfo dateTimeFormatInfo = (DateTimeFormatInfo)culture.GetFormat(typeof(DateTimeFormatInfo));
					if (culture == CultureInfo.InvariantCulture)
					{
						if (dateTime.Equals(dateTime.Date))
						{
							return dateTime.ToString("yyyy-MM-dd", culture);
						}
						return dateTime.ToString(culture);
					}
					else
					{
						if (dateTime == dateTime.Date)
						{
							return dateTime.ToString(dateTimeFormatInfo.ShortDatePattern, culture);
						}
						return dateTime.ToString(dateTimeFormatInfo.ShortDatePattern + " " + dateTimeFormatInfo.ShortTimePattern, culture);
					}
				}
				else if (destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(DateTime).GetConstructor(new Type[] { typeof(long) });
					return new global::System.ComponentModel.Design.Serialization.InstanceDescriptor(constructor, new object[] { dateTime.Ticks });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
