using System;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class TimeSpanConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value.GetType() == typeof(string))
			{
				string text = (string)value;
				try
				{
					return TimeSpan.Parse(text);
				}
				catch
				{
					throw new FormatException(text + "is not valid for a TimeSpan.");
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is TimeSpan)
			{
				TimeSpan timeSpan = (TimeSpan)value;
				if (destinationType == typeof(string) && value != null)
				{
					return timeSpan.ToString();
				}
				if (destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(TimeSpan).GetConstructor(new Type[] { typeof(long) });
					return new global::System.ComponentModel.Design.Serialization.InstanceDescriptor(constructor, new object[] { timeSpan.Ticks });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
