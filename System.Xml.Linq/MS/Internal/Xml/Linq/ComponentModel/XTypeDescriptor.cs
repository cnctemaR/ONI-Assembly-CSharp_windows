using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace MS.Internal.Xml.Linq.ComponentModel
{
	internal class XTypeDescriptor<T> : CustomTypeDescriptor
	{
		public XTypeDescriptor(ICustomTypeDescriptor parent)
			: base(parent)
		{
		}

		public override PropertyDescriptorCollection GetProperties()
		{
			return this.GetProperties(null);
		}

		public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			if (attributes == null)
			{
				if (typeof(T) == typeof(XElement))
				{
					propertyDescriptorCollection.Add(new XElementAttributePropertyDescriptor());
					propertyDescriptorCollection.Add(new XElementDescendantsPropertyDescriptor());
					propertyDescriptorCollection.Add(new XElementElementPropertyDescriptor());
					propertyDescriptorCollection.Add(new XElementElementsPropertyDescriptor());
					propertyDescriptorCollection.Add(new XElementValuePropertyDescriptor());
					propertyDescriptorCollection.Add(new XElementXmlPropertyDescriptor());
				}
				else if (typeof(T) == typeof(XAttribute))
				{
					propertyDescriptorCollection.Add(new XAttributeValuePropertyDescriptor());
				}
			}
			foreach (object obj in base.GetProperties(attributes))
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
				propertyDescriptorCollection.Add(propertyDescriptor);
			}
			return propertyDescriptorCollection;
		}
	}
}
