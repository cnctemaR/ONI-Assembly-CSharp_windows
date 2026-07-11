using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public abstract class PropertyDescriptor : MemberDescriptor
	{
		protected PropertyDescriptor(string name, Attribute[] attrs)
			: base(name, attrs)
		{
		}

		protected PropertyDescriptor(MemberDescriptor descr)
			: base(descr)
		{
		}

		protected PropertyDescriptor(MemberDescriptor descr, Attribute[] attrs)
			: base(descr, attrs)
		{
		}

		public abstract Type ComponentType { get; }

		public virtual TypeConverter Converter
		{
			get
			{
				AttributeCollection attributes = this.Attributes;
				if (this.converter == null)
				{
					TypeConverterAttribute typeConverterAttribute = (TypeConverterAttribute)attributes[typeof(TypeConverterAttribute)];
					if (typeConverterAttribute.ConverterTypeName != null && typeConverterAttribute.ConverterTypeName.Length > 0)
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
				return LocalizableAttribute.Yes.Equals(this.Attributes[typeof(LocalizableAttribute)]);
			}
		}

		public abstract bool IsReadOnly { get; }

		public DesignerSerializationVisibility SerializationVisibility
		{
			get
			{
				return ((DesignerSerializationVisibilityAttribute)this.Attributes[typeof(DesignerSerializationVisibilityAttribute)]).Visibility;
			}
		}

		public abstract Type PropertyType { get; }

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
			if (this.valueChangedHandlers == null)
			{
				this.valueChangedHandlers = new Hashtable();
			}
			EventHandler eventHandler = (EventHandler)this.valueChangedHandlers[component];
			this.valueChangedHandlers[component] = Delegate.Combine(eventHandler, handler);
		}

		public abstract bool CanResetValue(object component);

		public override bool Equals(object obj)
		{
			try
			{
				if (obj == this)
				{
					return true;
				}
				if (obj == null)
				{
					return false;
				}
				PropertyDescriptor propertyDescriptor = obj as PropertyDescriptor;
				if (propertyDescriptor != null && propertyDescriptor.NameHashCode == this.NameHashCode && propertyDescriptor.PropertyType == this.PropertyType && propertyDescriptor.Name.Equals(this.Name))
				{
					return true;
				}
			}
			catch
			{
			}
			return false;
		}

		protected object CreateInstance(Type type)
		{
			Type[] array = new Type[] { typeof(Type) };
			if (type.GetConstructor(array) != null)
			{
				return TypeDescriptor.CreateInstance(null, type, array, new object[] { this.PropertyType });
			}
			return TypeDescriptor.CreateInstance(null, type, null, null);
		}

		protected override void FillAttributes(IList attributeList)
		{
			this.converter = null;
			this.editors = null;
			this.editorTypes = null;
			this.editorCount = 0;
			base.FillAttributes(attributeList);
		}

		public PropertyDescriptorCollection GetChildProperties()
		{
			return this.GetChildProperties(null, null);
		}

		public PropertyDescriptorCollection GetChildProperties(Attribute[] filter)
		{
			return this.GetChildProperties(null, filter);
		}

		public PropertyDescriptorCollection GetChildProperties(object instance)
		{
			return this.GetChildProperties(instance, null);
		}

		public virtual PropertyDescriptorCollection GetChildProperties(object instance, Attribute[] filter)
		{
			if (instance == null)
			{
				return TypeDescriptor.GetProperties(this.PropertyType, filter);
			}
			return TypeDescriptor.GetProperties(instance, filter);
		}

		public virtual object GetEditor(Type editorBaseType)
		{
			object obj = null;
			AttributeCollection attributes = this.Attributes;
			if (this.editorTypes != null)
			{
				for (int i = 0; i < this.editorCount; i++)
				{
					if (this.editorTypes[i] == editorBaseType)
					{
						return this.editors[i];
					}
				}
			}
			if (obj == null)
			{
				for (int j = 0; j < attributes.Count; j++)
				{
					EditorAttribute editorAttribute = attributes[j] as EditorAttribute;
					if (editorAttribute != null)
					{
						Type typeFromName = this.GetTypeFromName(editorAttribute.EditorBaseTypeName);
						if (editorBaseType == typeFromName)
						{
							Type typeFromName2 = this.GetTypeFromName(editorAttribute.EditorTypeName);
							if (typeFromName2 != null)
							{
								obj = this.CreateInstance(typeFromName2);
								break;
							}
						}
					}
				}
				if (obj == null)
				{
					obj = TypeDescriptor.GetEditor(this.PropertyType, editorBaseType);
				}
				if (this.editorTypes == null)
				{
					this.editorTypes = new Type[5];
					this.editors = new object[5];
				}
				if (this.editorCount >= this.editorTypes.Length)
				{
					Type[] array = new Type[this.editorTypes.Length * 2];
					object[] array2 = new object[this.editors.Length * 2];
					Array.Copy(this.editorTypes, array, this.editorTypes.Length);
					Array.Copy(this.editors, array2, this.editors.Length);
					this.editorTypes = array;
					this.editors = array2;
				}
				this.editorTypes[this.editorCount] = editorBaseType;
				object[] array3 = this.editors;
				int num = this.editorCount;
				this.editorCount = num + 1;
				array3[num] = obj;
			}
			return obj;
		}

		public override int GetHashCode()
		{
			return this.NameHashCode ^ this.PropertyType.GetHashCode();
		}

		protected override object GetInvocationTarget(Type type, object instance)
		{
			object obj = base.GetInvocationTarget(type, instance);
			ICustomTypeDescriptor customTypeDescriptor = obj as ICustomTypeDescriptor;
			if (customTypeDescriptor != null)
			{
				obj = customTypeDescriptor.GetPropertyOwner(this);
			}
			return obj;
		}

		protected Type GetTypeFromName(string typeName)
		{
			if (typeName == null || typeName.Length == 0)
			{
				return null;
			}
			Type type = Type.GetType(typeName);
			Type type2 = null;
			if (this.ComponentType != null && (type == null || this.ComponentType.Assembly.FullName.Equals(type.Assembly.FullName)))
			{
				int num = typeName.IndexOf(',');
				if (num != -1)
				{
					typeName = typeName.Substring(0, num);
				}
				type2 = this.ComponentType.Assembly.GetType(typeName);
			}
			return type2 ?? type;
		}

		public abstract object GetValue(object component);

		protected virtual void OnValueChanged(object component, EventArgs e)
		{
			if (component != null && this.valueChangedHandlers != null)
			{
				EventHandler eventHandler = (EventHandler)this.valueChangedHandlers[component];
				if (eventHandler != null)
				{
					eventHandler(component, e);
				}
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
			if (this.valueChangedHandlers != null)
			{
				EventHandler eventHandler = (EventHandler)this.valueChangedHandlers[component];
				eventHandler = (EventHandler)Delegate.Remove(eventHandler, handler);
				if (eventHandler != null)
				{
					this.valueChangedHandlers[component] = eventHandler;
					return;
				}
				this.valueChangedHandlers.Remove(component);
			}
		}

		protected internal EventHandler GetValueChangedHandler(object component)
		{
			if (component != null && this.valueChangedHandlers != null)
			{
				return (EventHandler)this.valueChangedHandlers[component];
			}
			return null;
		}

		public abstract void ResetValue(object component);

		public abstract void SetValue(object component, object value);

		public abstract bool ShouldSerializeValue(object component);

		public virtual bool SupportsChangeEvents
		{
			get
			{
				return false;
			}
		}

		private TypeConverter converter;

		private Hashtable valueChangedHandlers;

		private object[] editors;

		private Type[] editorTypes;

		private int editorCount;
	}
}
