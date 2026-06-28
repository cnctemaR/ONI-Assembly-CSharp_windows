using System;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.ComponentModel
{
	[ComVisible(true)]
	public abstract class PropertyDescriptor : MemberDescriptor
	{
		protected PropertyDescriptor(MemberDescriptor reference)
			: base(reference)
		{
		}

		protected PropertyDescriptor(MemberDescriptor reference, Attribute[] attrs)
			: base(reference, attrs)
		{
		}

		protected PropertyDescriptor(string name, Attribute[] attrs)
			: base(name, attrs)
		{
		}

		public abstract Type ComponentType { get; }

		public virtual TypeConverter Converter
		{
			get
			{
				if (this.converter == null && this.PropertyType != null)
				{
					TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)this.Attributes[typeof(TypeConverterAttribute)];
					if (typeConverterAttribute != null && typeConverterAttribute != TypeConverterAttribute.Default)
					{
						Type typeFromName = this.GetTypeFromName(typeConverterAttribute.ConverterTypeName);
						if (typeFromName != null && typeof(TypeConverter).IsAssignableFrom(typeFromName))
						{
							this.converter = (TypeConverter)this.CreateInstance(typeFromName);
						}
					}
					if (this.converter == null)
					{
						this.converter = TypeDescriptor.GetConverter(this.PropertyType);
					}
				}
				return this.converter;
			}
		}

		public virtual bool IsLocalizable
		{
			get
			{
				foreach (Attribute attribute in this.AttributeArray)
				{
					if (attribute is LocalizableAttribute)
					{
						return ((LocalizableAttribute)attribute).IsLocalizable;
					}
				}
				return false;
			}
		}

		public abstract bool IsReadOnly { get; }

		public abstract Type PropertyType { get; }

		public virtual bool SupportsChangeEvents
		{
			get
			{
				return false;
			}
		}

		public DesignerSerializationVisibility SerializationVisibility
		{
			get
			{
				foreach (Attribute attribute in this.AttributeArray)
				{
					if (attribute is DesignerSerializationVisibilityAttribute)
					{
						DesignerSerializationVisibilityAttribute designerSerializationVisibilityAttribute = (DesignerSerializationVisibilityAttribute)attribute;
						return designerSerializationVisibilityAttribute.Visibility;
					}
				}
				return DesignerSerializationVisibility.Visible;
			}
		}

		public virtual void AddValueChanged(object component, EventHandler handler)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (this.notifiers == null)
			{
				this.notifiers = new Hashtable();
			}
			EventHandler eventHandler = (EventHandler)this.notifiers[component];
			if (eventHandler != null)
			{
				eventHandler = (EventHandler)Delegate.Combine(eventHandler, handler);
				this.notifiers[component] = eventHandler;
			}
			else
			{
				this.notifiers[component] = handler;
			}
		}

		public virtual void RemoveValueChanged(object component, EventHandler handler)
		{
			if (component == null)
			{
				throw new ArgumentNullException("component");
			}
			if (handler == null)
			{
				throw new ArgumentNullException("handler");
			}
			if (this.notifiers == null)
			{
				return;
			}
			EventHandler eventHandler = (EventHandler)this.notifiers[component];
			eventHandler = (EventHandler)Delegate.Remove(eventHandler, handler);
			if (eventHandler == null)
			{
				this.notifiers.Remove(component);
			}
			else
			{
				this.notifiers[component] = eventHandler;
			}
		}

		protected override void FillAttributes(IList attributeList)
		{
			base.FillAttributes(attributeList);
		}

		protected override object GetInvocationTarget(Type type, object instance)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (instance is CustomTypeDescriptor)
			{
				CustomTypeDescriptor customTypeDescriptor = (CustomTypeDescriptor)instance;
				return customTypeDescriptor.GetPropertyOwner(this);
			}
			return base.GetInvocationTarget(type, instance);
		}

		protected internal EventHandler GetValueChangedHandler(object component)
		{
			if (component == null || this.notifiers == null)
			{
				return null;
			}
			return (EventHandler)this.notifiers[component];
		}

		protected virtual void OnValueChanged(object component, EventArgs e)
		{
			if (this.notifiers == null)
			{
				return;
			}
			EventHandler eventHandler = (EventHandler)this.notifiers[component];
			if (eventHandler == null)
			{
				return;
			}
			eventHandler(component, e);
		}

		public abstract object GetValue(object component);

		public abstract void SetValue(object component, object value);

		public abstract void ResetValue(object component);

		public abstract bool CanResetValue(object component);

		public abstract bool ShouldSerializeValue(object component);

		protected object CreateInstance(Type type)
		{
			if (type == null || this.PropertyType == null)
			{
				return null;
			}
			Type[] array = new Type[] { typeof(Type) };
			ConstructorInfo constructor = type.GetConstructor(array);
			object obj;
			if (constructor != null)
			{
				object[] array2 = new object[] { this.PropertyType };
				obj = TypeDescriptor.CreateInstance(null, type, array, array2);
			}
			else
			{
				obj = TypeDescriptor.CreateInstance(null, type, null, null);
			}
			return obj;
		}

		public override bool Equals(object obj)
		{
			if (!base.Equals(obj))
			{
				return false;
			}
			PropertyDescriptor propertyDescriptor = obj as PropertyDescriptor;
			return propertyDescriptor != null && propertyDescriptor.PropertyType == this.PropertyType;
		}

		public PropertyDescriptorCollection GetChildProperties()
		{
			return this.GetChildProperties(null, null);
		}

		public PropertyDescriptorCollection GetChildProperties(object instance)
		{
			return this.GetChildProperties(instance, null);
		}

		public PropertyDescriptorCollection GetChildProperties(Attribute[] filter)
		{
			return this.GetChildProperties(null, filter);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public virtual PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
		{
			return TypeDescriptor.GetProperties(instance, filter);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			Type type = null;
			Attribute[] attributeArray = this.AttributeArray;
			if (attributeArray != null && attributeArray.Length != 0)
			{
				foreach (Attribute attribute in attributeArray)
				{
					EditorAttribute editorAttribute = attribute as EditorAttribute;
					if (editorAttribute != null)
					{
						type = this.GetTypeFromName(editorAttribute.EditorTypeName);
						if (type != null && type.IsSubclassOf(editorBaseType))
						{
							break;
						}
					}
				}
			}
			object obj = null;
			if (type != null)
			{
				obj = this.CreateInstance(type);
			}
			if (obj == null)
			{
				obj = TypeDescriptor.GetEditor(this.PropertyType, editorBaseType);
			}
			return obj;
		}

		protected Type GetTypeFromName(string typeName)
		{
			if (typeName == null || this.ComponentType == null || typeName.Trim().Length == 0)
			{
				return null;
			}
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				int num = typeName.IndexOf(",");
				if (num != -1)
				{
					typeName = typeName.Substring(0, num);
				}
				type = this.ComponentType.Assembly.GetType(typeName);
			}
			return type;
		}

		private TypeConverter converter;

		private Hashtable notifiers;
	}
}
