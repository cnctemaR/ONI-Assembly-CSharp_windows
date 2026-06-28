using System;
using System.Collections;
using System.ComponentModel.Design;

namespace System.ComponentModel
{
	internal abstract class Info
	{
		public Info(Type infoType)
		{
			this._infoType = infoType;
		}

		public abstract AttributeCollection GetAttributes();

		public abstract EventDescriptorCollection GetEvents();

		public abstract PropertyDescriptorCollection GetProperties();

		public Type InfoType
		{
			get
			{
				return this._infoType;
			}
		}

		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			EventDescriptorCollection events = this.GetEvents();
			if (attributes == null)
			{
				return events;
			}
			return events.Filter(attributes);
		}

		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection properties = this.GetProperties();
			if (attributes == null)
			{
				return properties;
			}
			return properties.Filter(attributes);
		}

		public EventDescriptor GetDefaultEvent()
		{
			if (this._gotDefaultEvent)
			{
				return this._defaultEvent;
			}
			DefaultEventAttribute defaultEventAttribute = (DefaultEventAttribute)this.GetAttributes()[typeof(DefaultEventAttribute)];
			if (defaultEventAttribute == null || defaultEventAttribute.Name == null)
			{
				this._defaultEvent = null;
			}
			else
			{
				EventDescriptorCollection events = this.GetEvents();
				this._defaultEvent = events[defaultEventAttribute.Name];
			}
			this._gotDefaultEvent = true;
			return this._defaultEvent;
		}

		public PropertyDescriptor GetDefaultProperty()
		{
			if (this._gotDefaultProperty)
			{
				return this._defaultProperty;
			}
			DefaultPropertyAttribute defaultPropertyAttribute = (DefaultPropertyAttribute)this.GetAttributes()[typeof(DefaultPropertyAttribute)];
			if (defaultPropertyAttribute == null || defaultPropertyAttribute.Name == null)
			{
				this._defaultProperty = null;
			}
			else
			{
				PropertyDescriptorCollection properties = this.GetProperties();
				this._defaultProperty = properties[defaultPropertyAttribute.Name];
			}
			this._gotDefaultProperty = true;
			return this._defaultProperty;
		}

		protected AttributeCollection GetAttributes(IComponent comp)
		{
			if (this._attributes != null)
			{
				return this._attributes;
			}
			bool flag = true;
			ArrayList arrayList = new ArrayList();
			foreach (Attribute attribute in this._infoType.GetCustomAttributes(false))
			{
				arrayList.Add(attribute);
			}
			Type type = this._infoType.BaseType;
			while (type != null && type != typeof(object))
			{
				foreach (Attribute attribute2 in type.GetCustomAttributes(false))
				{
					arrayList.Add(attribute2);
				}
				type = type.BaseType;
			}
			foreach (Type type2 in this._infoType.GetInterfaces())
			{
				foreach (object obj in TypeDescriptor.GetAttributes(type2))
				{
					Attribute attribute3 = (Attribute)obj;
					arrayList.Add(attribute3);
				}
			}
			Hashtable hashtable = new Hashtable();
			for (int l = arrayList.Count - 1; l >= 0; l--)
			{
				Attribute attribute4 = (Attribute)arrayList[l];
				hashtable[attribute4.TypeId] = attribute4;
			}
			if (comp != null && comp.Site != null)
			{
				global::System.ComponentModel.Design.ITypeDescriptorFilterService typeDescriptorFilterService = (global::System.ComponentModel.Design.ITypeDescriptorFilterService)comp.Site.GetService(typeof(global::System.ComponentModel.Design.ITypeDescriptorFilterService));
				if (typeDescriptorFilterService != null)
				{
					flag = typeDescriptorFilterService.FilterAttributes(comp, hashtable);
				}
			}
			Attribute[] array = new Attribute[hashtable.Values.Count];
			hashtable.Values.CopyTo(array, 0);
			AttributeCollection attributeCollection = new AttributeCollection(array);
			if (flag)
			{
				this._attributes = attributeCollection;
			}
			return attributeCollection;
		}

		private Type _infoType;

		private EventDescriptor _defaultEvent;

		private bool _gotDefaultEvent;

		private PropertyDescriptor _defaultProperty;

		private bool _gotDefaultProperty;

		private AttributeCollection _attributes;
	}
}
