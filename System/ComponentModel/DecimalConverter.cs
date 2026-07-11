using System;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class DecimalConverter : BaseNumberConverter
	{
		public DecimalConverter()
		{
			this.InnerType = typeof(decimal);
		}

		internal override bool SupportHex
		{
			get
			{
				return false;
			}
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor) || base.CanConvertTo(context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor) && value is decimal)
			{
				decimal num = (decimal)value;
				ConstructorInfo constructor = typeof(decimal).GetConstructor(new Type[] { typeof(int[]) });
				return new global::System.ComponentModel.Design.Serialization.InstanceDescriptor(constructor, new object[] { decimal.GetBits(num) });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		internal override string ConvertToString(object value, NumberFormatInfo format)
		{
			return ((decimal)value).ToString("G", format);
		}

		internal override object ConvertFromString(string value, NumberFormatInfo format)
		{
			return decimal.Parse(value, NumberStyles.Float, format);
		}
	}
}
