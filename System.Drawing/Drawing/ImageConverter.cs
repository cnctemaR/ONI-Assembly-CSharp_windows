using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
	public class ImageConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(byte[]);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(byte[]) || destinationType == typeof(string);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			byte[] array = value as byte[];
			if (array == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			MemoryStream memoryStream = new MemoryStream(array);
			return Image.FromStream(memoryStream);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value == null)
			{
				return "(none)";
			}
			if (value is Image)
			{
				if (destinationType == typeof(string))
				{
					return value.ToString();
				}
				if (this.CanConvertTo(null, destinationType))
				{
					MemoryStream memoryStream = new MemoryStream();
					((Image)value).Save(memoryStream, ((Image)value).RawFormat);
					return memoryStream.GetBuffer();
				}
			}
			string text = Locale.GetText("ImageConverter can not convert from type '{0}'.", new object[] { value.GetType() });
			throw new NotSupportedException(text);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return TypeDescriptor.GetProperties(typeof(Image), attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
