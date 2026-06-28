using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace System.Drawing
{
	public class RectangleConverter : TypeConverter
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
			Int32Converter int32Converter = new Int32Converter();
			int[] array2 = new int[array.Length];
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i] = (int)int32Converter.ConvertFromString(context, culture, array[i]);
			}
			if (array.Length != 4)
			{
				throw new ArgumentException("Failed to parse Text(" + text + ") expected text in the format \"x,y,Width,Height.\"");
			}
			return new Rectangle(array2[0], array2[1], array2[2], array2[3]);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (value is Rectangle)
			{
				Rectangle rectangle = (Rectangle)value;
				if (destinationType == typeof(string))
				{
					string listSeparator = culture.TextInfo.ListSeparator;
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(rectangle.X.ToString(culture));
					stringBuilder.Append(listSeparator);
					stringBuilder.Append(" ");
					stringBuilder.Append(rectangle.Y.ToString(culture));
					stringBuilder.Append(listSeparator);
					stringBuilder.Append(" ");
					stringBuilder.Append(rectangle.Width.ToString(culture));
					stringBuilder.Append(listSeparator);
					stringBuilder.Append(" ");
					stringBuilder.Append(rectangle.Height.ToString(culture));
					return stringBuilder.ToString();
				}
				if (destinationType == typeof(InstanceDescriptor))
				{
					ConstructorInfo constructor = typeof(Rectangle).GetConstructor(new Type[]
					{
						typeof(int),
						typeof(int),
						typeof(int),
						typeof(int)
					});
					return new InstanceDescriptor(constructor, new object[] { rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height });
				}
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			object obj = propertyValues["X"];
			object obj2 = propertyValues["Y"];
			object obj3 = propertyValues["Width"];
			object obj4 = propertyValues["Height"];
			if (obj == null || obj2 == null || obj3 == null || obj4 == null)
			{
				throw new ArgumentException("propertyValues");
			}
			int num = (int)obj;
			int num2 = (int)obj2;
			int num3 = (int)obj3;
			int num4 = (int)obj4;
			return new Rectangle(num, num2, num3, num4);
		}

		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			if (value is Rectangle)
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
