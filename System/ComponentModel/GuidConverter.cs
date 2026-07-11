using System;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class GuidConverter : TypeConverter
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
					return new Guid(text);
				}
				catch
				{
					throw new FormatException(text + "is not a valid GUID.");
				}
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is Guid)
			{
				Guid guid = (Guid)value;
				if (destinationType == typeof(string) && value != null)
				{
					return guid.ToString("D");
				}
				if (destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(Guid).GetConstructor(new Type[] { typeof(string) });
					return new global::System.ComponentModel.Design.Serialization.InstanceDescriptor(constructor, new object[] { guid.ToString("D") });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
