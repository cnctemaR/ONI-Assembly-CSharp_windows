using System;
using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public class TypeConverter
	{
		public bool CanConvertFrom(Type sourceType)
		{
			return this.CanConvertFrom(null, sourceType);
		}

		public virtual bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof(global::System.ComponentModel.Design.Serialization.InstanceDescriptor);
		}

		public bool CanConvertTo(Type destinationType)
		{
			return this.CanConvertTo(null, destinationType);
		}

		public virtual bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType == typeof(string);
		}

		public object ConvertFrom(object o)
		{
			return this.ConvertFrom(null, CultureInfo.CurrentCulture, o);
		}

		public virtual object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is global::System.ComponentModel.Design.Serialization.InstanceDescriptor)
			{
				return ((global::System.ComponentModel.Design.Serialization.InstanceDescriptor)value).Invoke();
			}
			return this.GetConvertFromException(value);
		}

		public object ConvertFromInvariantString(string text)
		{
			return this.ConvertFromInvariantString(null, text);
		}

		public object ConvertFromInvariantString(ITypeDescriptorContext context, string text)
		{
			return this.ConvertFromString(context, CultureInfo.InvariantCulture, text);
		}

		public object ConvertFromString(string text)
		{
			return this.ConvertFrom(text);
		}

		public object ConvertFromString(ITypeDescriptorContext context, string text)
		{
			return this.ConvertFromString(context, CultureInfo.CurrentCulture, text);
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
			if (destinationType != typeof(string))
			{
				return this.GetConvertToException(value, destinationType);
			}
			if (value != null)
			{
				return value.ToString();
			}
			return string.Empty;
		}

		public string ConvertToInvariantString(object value)
		{
			return this.ConvertToInvariantString(null, value);
		}

		public string ConvertToInvariantString(ITypeDescriptorContext context, object value)
		{
			return (string)this.ConvertTo(context, CultureInfo.InvariantCulture, value, typeof(string));
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

		protected Exception GetConvertFromException(object value)
		{
			string text;
			if (value == null)
			{
				text = "(null)";
			}
			else
			{
				text = value.GetType().FullName;
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "{0} cannot convert from {1}.", new object[]
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
				text = "(null)";
			}
			else
			{
				text = value.GetType().FullName;
			}
			throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "'{0}' is unable to convert '{1}' to '{2}'.", new object[]
			{
				base.GetType().Name,
				text,
				destinationType.FullName
			}));
		}

		public object CreateInstance(IDictionary propertyValues)
		{
			return this.CreateInstance(null, propertyValues);
		}

		public virtual object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			return null;
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
			return true;
		}

		protected PropertyDescriptorCollection SortProperties(PropertyDescriptorCollection props, string[] names)
		{
			props.Sort(names);
			return props;
		}

		public class StandardValuesCollection : ICollection, IEnumerable
		{
			public StandardValuesCollection(ICollection values)
			{
				this.values = values;
			}

			void ICollection.CopyTo(Array array, int index)
			{
				this.CopyTo(array, index);
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
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

			int ICollection.Count
			{
				get
				{
					return this.Count;
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

			public int Count
			{
				get
				{
					return this.values.Count;
				}
			}

			public object this[int index]
			{
				get
				{
					return ((IList)this.values)[index];
				}
			}

			private ICollection values;
		}

		protected abstract class SimplePropertyDescriptor : PropertyDescriptor
		{
			public SimplePropertyDescriptor(Type componentType, string name, Type propertyType)
				: this(componentType, name, propertyType, null)
			{
			}

			public SimplePropertyDescriptor(Type componentType, string name, Type propertyType, Attribute[] attributes)
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

			public override Type PropertyType
			{
				get
				{
					return this.propertyType;
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this.Attributes.Contains(ReadOnlyAttribute.Yes);
				}
			}

			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}

			public override bool CanResetValue(object component)
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];
				return defaultValueAttribute != null && defaultValueAttribute.Value == this.GetValue(component);
			}

			public override void ResetValue(object component)
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];
				if (defaultValueAttribute != null)
				{
					this.SetValue(component, defaultValueAttribute.Value);
				}
			}

			private Type componentType;

			private Type propertyType;
		}
	}
}
