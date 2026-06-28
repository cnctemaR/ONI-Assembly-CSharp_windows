using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;

namespace System.Drawing
{
	public class IconConverter : ExpandableObjectConverter
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
			return new Icon(memoryStream);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is Icon && destinationType == typeof(string))
			{
				return value.ToString();
			}
			if (value == null && destinationType == typeof(string))
			{
				return "(none)";
			}
			if (this.CanConvertTo(null, destinationType))
			{
				MemoryStream memoryStream = new MemoryStream();
				((Icon)value).Save(memoryStream);
				return memoryStream.GetBuffer();
			}
			return new NotSupportedException("IconConverter can not convert from " + value.GetType());
		}
	}
}
