using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System
{
	public class UriTypeConverter : global::System.ComponentModel.TypeConverter
	{
		private bool CanConvert(Type type)
		{
			return type == typeof(string) || type == typeof(global::System.Uri) || type == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor);
		}

		public override bool CanConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == null)
			{
				throw new ArgumentNullException("sourceType");
			}
			return this.CanConvert(sourceType);
		}

		public override bool CanConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType != null && this.CanConvert(destinationType);
		}

		public override object ConvertFrom(global::System.ComponentModel.ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!this.CanConvertFrom(context, value.GetType()))
			{
				throw new NotSupportedException(global::Locale.GetText("Cannot convert from value."));
			}
			if (value is global::System.Uri)
			{
				return value;
			}
			string text = value as string;
			if (text != null)
			{
				return new global::System.Uri(text, global::System.UriKind.RelativeOrAbsolute);
			}
			global::System.ComponentModel.Design.Serialization.InstanceDescriptor instanceDescriptor = value as global::System.ComponentModel.Design.Serialization.InstanceDescriptor;
			if (instanceDescriptor != null)
			{
				return instanceDescriptor.Invoke();
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(global::System.ComponentModel.ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (!this.CanConvertTo(context, destinationType))
			{
				throw new NotSupportedException(global::Locale.GetText("Cannot convert to destination type."));
			}
			global::System.Uri uri = value as global::System.Uri;
			if (uri != null)
			{
				if (destinationType == typeof(string))
				{
					return uri.ToString();
				}
				if (destinationType == typeof(global::System.Uri))
				{
					return uri;
				}
				if (destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(global::System.Uri).GetConstructor(new Type[]
					{
						typeof(string),
						typeof(global::System.UriKind)
					});
					return new global::System.ComponentModel.Design.Serialization.InstanceDescriptor(constructor, new object[]
					{
						uri.ToString(),
						(!uri.IsAbsoluteUri) ? global::System.UriKind.Relative : global::System.UriKind.Absolute
					});
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool IsValid(global::System.ComponentModel.ITypeDescriptorContext context, object value)
		{
			return value != null && (value is string || value is global::System.Uri);
		}
	}
}
