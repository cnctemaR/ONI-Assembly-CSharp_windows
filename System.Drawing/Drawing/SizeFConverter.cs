using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Drawing
{
	public class SizeFConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string) || destinationType == typeof(InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			string text = value as string;
			if (text == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			string[] array = text.Split(culture.TextInfo.ListSeparator.ToCharArray());
			SingleConverter singleConverter = new SingleConverter();
			float[] array2 = new float[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = (float)singleConverter.ConvertFromString(context, culture, array[i]);
			}
			if (array.Length != 2)
			{
				throw new ArgumentException("Failed to parse Text(" + text + ") expected text in the format \"Width,Height.\"");
			}
			return new SizeF(array2[0], array2[1]);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is SizeF)
			{
				SizeF sizeF = (SizeF)value;
				if (destinationType == typeof(string))
				{
					return sizeF.Width.ToString(culture) + culture.TextInfo.ListSeparator + " " + sizeF.Height.ToString(culture);
				}
				if (destinationType == typeof(InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(SizeF).GetConstructor(new Type[]
					{
						typeof(float),
						typeof(float)
					});
					return new InstanceDescriptor(constructor, new object[] { sizeF.Width, sizeF.Height });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			float num = (float)propertyValues["Width"];
			float num2 = (float)propertyValues["Height"];
			return new SizeF(num, num2);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (value is SizeF)
			{
				return TypeDescriptor.GetProperties(value, attributes);
			}
			return base.GetProperties(context, value, attributes);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
	}
}
