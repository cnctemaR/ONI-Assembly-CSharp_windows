using System;
using System.Globalization;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class ArrayConverter : CollectionConverter
	{
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("destinationType");
			}
			if (destinationType == typeof(string) && value is Array)
			{
				return global::SR.GetString("{0} Array", new object[] { value.GetType().Name });
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptor[] array = null;
			if (value.GetType().IsArray)
			{
				int length = ((Array)value).GetLength(0);
				array = new PropertyDescriptor[length];
				Type type = value.GetType();
				Type elementType = type.GetElementType();
				for (int i = 0; i < length; i++)
				{
					array[i] = new ArrayConverter.ArrayPropertyDescriptor(type, elementType, i);
				}
			}
			return new PropertyDescriptorCollection(array);
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}

		private class ArrayPropertyDescriptor : TypeConverter.SimplePropertyDescriptor
		{
			public ArrayPropertyDescriptor(Type arrayType, Type elementType, int index)
				: base(arrayType, "[" + index + "]", elementType, null)
			{
				this.index = index;
			}

			public override object GetValue(object instance)
			{
				if (instance is Array)
				{
					Array array = (Array)instance;
					if (array.GetLength(0) > this.index)
					{
						return array.GetValue(this.index);
					}
				}
				return null;
			}

			public override void SetValue(object instance, object value)
			{
				if (instance is Array)
				{
					Array array = (Array)instance;
					if (array.GetLength(0) > this.index)
					{
						array.SetValue(value, this.index);
					}
					this.OnValueChanged(instance, EventArgs.Empty);
				}
			}

			private int index;
		}
	}
}
