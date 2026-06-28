using System;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel
{
	public class CultureInfoConverter : TypeConverter
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
			string text = value as string;
			if (text == null)
			{
				return base.ConvertFrom(context, culture, value);
			}
			if (string.Compare(text, "(Default)", false) == 0)
			{
				return CultureInfo.InvariantCulture;
			}
			try
			{
				return new CultureInfo(text);
			}
			catch
			{
				foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(CultureTypes.AllCultures))
				{
					if (string.Compare(cultureInfo.DisplayName, 0, text, 0, text.Length, true) == 0)
					{
						return cultureInfo;
					}
				}
			}
			throw new ArgumentException(string.Format("Culture {0} cannot be converted to a CultureInfo or is not available in this environment.", value));
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				if (value == null || !(value is CultureInfo))
				{
					return "(Default)";
				}
				if (value == CultureInfo.InvariantCulture)
				{
					return "(Default)";
				}
				return ((CultureInfo)value).DisplayName;
			}
			else
			{
				if (destinationType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor) && value is CultureInfo)
				{
					CultureInfo cultureInfo = (CultureInfo)value;
					ConstructorInfo constructor = typeof(CultureInfo).GetConstructor(new Type[] { typeof(int) });
					return new global::System.ComponentModel.Design.Serialization.InstanceDescriptor(constructor, new object[] { cultureInfo.LCID });
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}

		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			if (this._standardValues == null)
			{
				CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
				Array.Sort(cultures, new CultureInfoConverter.CultureInfoComparer());
				CultureInfo[] array = new CultureInfo[cultures.Length + 1];
				array[0] = CultureInfo.InvariantCulture;
				Array.Copy(cultures, 0, array, 1, cultures.Length);
				this._standardValues = new TypeConverter.StandardValuesCollection(array);
			}
			return this._standardValues;
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private TypeConverter.StandardValuesCollection _standardValues;

		private class CultureInfoComparer : IComparer
		{
			public int Compare(object first, object second)
			{
				if (first == null)
				{
					if (second == null)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (second == null)
					{
						return 1;
					}
					return string.Compare(((CultureInfo)first).DisplayName, ((CultureInfo)second).DisplayName, false, CultureInfo.CurrentCulture);
				}
			}
		}
	}
}
