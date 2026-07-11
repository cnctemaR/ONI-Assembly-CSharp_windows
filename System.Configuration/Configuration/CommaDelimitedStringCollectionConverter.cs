using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;

namespace System.Configuration
{
	public sealed class CommaDelimitedStringCollectionConverter : ConfigurationConverterBase
	{
		public override object ConvertFrom(ITypeDescriptorContext ctx, CultureInfo ci, object data)
		{
			CommaDelimitedStringCollection commaDelimitedStringCollection = new CommaDelimitedStringCollection();
			foreach (string text in ((string)data).Split(new char[] { ',' }))
			{
				commaDelimitedStringCollection.Add(text.Trim());
			}
			commaDelimitedStringCollection.UpdateStringHash();
			return commaDelimitedStringCollection;
		}

		public override object ConvertTo(ITypeDescriptorContext ctx, CultureInfo ci, object value, Type type)
		{
			if (value == null)
			{
				return null;
			}
			if (!typeof(StringCollection).IsAssignableFrom(value.GetType()))
			{
				throw new ArgumentException();
			}
			return value.ToString();
		}
	}
}
