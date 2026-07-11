using System;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class TypeConverter
	{
		private static bool UseCompatibleTypeConversion
		{
			get
			{
				return TypeConverter.useCompatibleTypeConversion;
			}
		}

		public bool CanConvertFrom(Type sourceType)
		{
			return this.CanConvertFrom(null, sourceType);
		}

		public virtual bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(InstanceDescriptor);
		}

		public bool CanConvertTo(Type destinationType)
		{
			return this.CanConvertTo(null, destinationType);
		}

		public virtual bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public object ConvertFrom(object value)
		{
			return this.ConvertFrom(null, CultureInfo.CurrentCulture, value);
		}

		public virtual object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			InstanceDescriptor instanceDescriptor = value as InstanceDescriptor;
			if (instanceDescriptor != null)
			{
				return instanceDescriptor.Invoke();
			}
			throw this.GetConvertFromException(value);
		}

		public object ConvertFromInvariantString(string text)
		{
			return this.ConvertFromString(null, CultureInfo.InvariantCulture, text);
		}

		public object ConvertFromInvariantString(ITypeDescriptorContext context, string text)
		{
			return this.ConvertFromString(context, CultureInfo.InvariantCulture, text);
		}

		public object ConvertFromString(string text)
		{
			return this.ConvertFrom(null, null, text);
		}

		public object ConvertFromString(ITypeDescriptorContext context, string text)
		{
			return this.ConvertFrom(context, CultureInfo.CurrentCulture, text);
		}

		public object ConvertFromString(ITypeDescriptorContext context, CultureInfo culture, string text)
		{
			return this.ConvertFrom(context, culture, text);
		}

		public object ConvertTo(object value, Type destinationType)
		{
			return this.ConvertTo(null, null, value, destinationType);
		}

		public virtual object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (!(destinationType == typeof(string)))
			{
				throw this.GetConvertToException(value, destinationType);
			}
			if (value == null)
			{
				return string.Empty;
			}
			if (culture != null && culture != CultureInfo.CurrentCulture)
			{
				IFormattable formattable = value as IFormattable;
				if (formattable != null)
				{
					return formattable.ToString(null, culture);
				}
			}
			return value.ToString();
		}

		public string ConvertToInvariantString(object value)
		{
			return this.ConvertToString(null, CultureInfo.InvariantCulture, value);
		}

		public string ConvertToInvariantString(ITypeDescriptorContext context, object value)
		{
			return this.ConvertToString(context, CultureInfo.InvariantCulture, value);
		}

		public string ConvertToString(object value)
		{
			return (string)this.ConvertTo(null, CultureInfo.CurrentCulture, value, typeof(string));
		}

		public string ConvertToString(ITypeDescriptorContext context, object value)
		{
			return (string)this.ConvertTo(context, CultureInfo.CurrentCulture, value, typeof(string));
		}

		public string ConvertToString(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return (string)this.ConvertTo(context, culture, value, typeof(string));
		}

		public object CreateInstance(IDictionary propertyValues)
		{
			return this.CreateInstance(null, propertyValues);
		}

		public virtual object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return null;
		}

		protected Exception GetConvertFromException(object value)
		{
			string text;
			if (value == null)
			{
				text = global::SR.GetString("(null)");
			}
			else
			{
				text = value.GetType().FullName;
			}
			throw new NotSupportedException(global::SR.GetString("{0} cannot convert from {1}.", new object[]
			{
				base.GetType().Name,
				text
			}));
		}

		protected Exception GetConvertToException(object value, Type destinationType)
		{
			string text;
			if (value == null)
			{
				text = global::SR.GetString("(null)");
			}
			else
			{
				text = value.GetType().FullName;
			}
			throw new NotSupportedException(global::SR.GetString("'{0}' is unable to convert '{1}' to '{2}'.", new object[]
			{
				base.GetType().Name,
				text,
				destinationType.FullName
			}));
		}

		public bool GetCreateInstanceSupported()
		{
			return this.GetCreateInstanceSupported(null);
		}

		public virtual bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public PropertyDescriptorCollection GetProperties(object value)
		{
			return this.GetProperties(null, value);
		}

		public PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value)
		{
			return this.GetProperties(context, value, new Attribute[] { BrowsableAttribute.Yes });
		}

		public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			return null;
		}

		public bool GetPropertiesSupported()
		{
			return this.GetPropertiesSupported(null);
		}

		public virtual bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public ICollection GetStandardValues()
		{
			return this.GetStandardValues(null);
		}

		public virtual TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
		{
			return null;
		}

		public bool GetStandardValuesExclusive()
		{
			return this.GetStandardValuesExclusive(null);
		}

		public virtual bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public bool GetStandardValuesSupported()
		{
			return this.GetStandardValuesSupported(null);
		}

		public virtual bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public bool IsValid(object value)
		{
			return this.IsValid(null, value);
		}

		public virtual bool IsValid(ITypeDescriptorContext context, object value)
		{
			if (TypeConverter.UseCompatibleTypeConversion)
			{
				return true;
			}
			bool flag = true;
			try
			{
				if (value == null || this.CanConvertFrom(context, value.GetType()))
				{
					this.ConvertFrom(context, CultureInfo.InvariantCulture, value);
				}
				else
				{
					flag = false;
				}
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		protected PropertyDescriptorCollection SortProperties(PropertyDescriptorCollection props, string[] names)
		{
			props.Sort(names);
			return props;
		}

		private const string s_UseCompatibleTypeConverterBehavior = "UseCompatibleTypeConverterBehavior";

		private static volatile bool useCompatibleTypeConversion;

		protected abstract class SimplePropertyDescriptor : PropertyDescriptor
		{
			protected SimplePropertyDescriptor(Type componentType, string name, Type propertyType)
				: this(componentType, name, propertyType, new Attribute[0])
			{
			}

			protected SimplePropertyDescriptor(Type componentType, string name, Type propertyType, Attribute[] attributes)
				: base(name, attributes)
			{
				this.componentType = componentType;
				this.propertyType = propertyType;
			}

			public override Type ComponentType
			{
				get
				{
					return this.componentType;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this.Attributes.Contains(ReadOnlyAttribute.Yes);
				}
			}

			public override Type PropertyType
			{
				get
				{
					return this.propertyType;
				}
			}

			public override bool CanResetValue(object component)
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];
				return defaultValueAttribute != null && defaultValueAttribute.Value.Equals(this.GetValue(component));
			}

			public override void ResetValue(object component)
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];
				if (defaultValueAttribute != null)
				{
					this.SetValue(component, defaultValueAttribute.Value);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			private Type componentType;

			private Type propertyType;
		}

		public class StandardValuesCollection : ICollection, IEnumerable
		{
			public StandardValuesCollection(ICollection values)
			{
				if (values == null)
				{
					values = new object[0];
				}
				Array array = values as Array;
				if (array != null)
				{
					this.valueArray = array;
				}
				this.values = values;
			}

			public int Count
			{
				get
				{
					if (this.valueArray != null)
					{
						return this.valueArray.Length;
					}
					return this.values.Count;
				}
			}

			public object this[int index]
			{
				get
				{
					if (this.valueArray != null)
					{
						return this.valueArray.GetValue(index);
					}
					IList list = this.values as IList;
					if (list != null)
					{
						return list[index];
					}
					this.valueArray = new object[this.values.Count];
					this.values.CopyTo(this.valueArray, 0);
					return this.valueArray.GetValue(index);
				}
			}

			public void CopyTo(Array array, int index)
			{
				this.values.CopyTo(array, index);
			}

			public IEnumerator GetEnumerator()
			{
				return this.values.GetEnumerator();
			}

			int ICollection.Count
			{
				get
				{
					return this.Count;
				}
			}

			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			object ICollection.SyncRoot
			{
				get
				{
					return null;
				}
			}

			void ICollection.CopyTo(Array array, int index)
			{
				this.CopyTo(array, index);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private ICollection values;

			private Array valueArray;
		}
	}
}
