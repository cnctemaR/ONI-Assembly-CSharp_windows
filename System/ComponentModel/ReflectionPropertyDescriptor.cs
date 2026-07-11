using System;
using System.Collections;
using System.ComponentModel.Design;
using System.Reflection;

namespace System.ComponentModel
{
	internal class ReflectionPropertyDescriptor : PropertyDescriptor
	{
		public ReflectionPropertyDescriptor(Type componentType, PropertyDescriptor oldPropertyDescriptor, Attribute[] attributes)
			: base(oldPropertyDescriptor, attributes)
		{
			this._componentType = componentType;
			this._propertyType = oldPropertyDescriptor.PropertyType;
		}

		public ReflectionPropertyDescriptor(Type componentType, string name, Type type, Attribute[] attributes)
			: base(name, attributes)
		{
			this._componentType = componentType;
			this._propertyType = type;
		}

		public ReflectionPropertyDescriptor(PropertyInfo info)
			: base(info.Name, null)
		{
			this._member = info;
			this._componentType = this._member.DeclaringType;
			this._propertyType = info.PropertyType;
		}

		private PropertyInfo GetPropertyInfo()
		{
			if (this._member == null)
			{
				this._member = this._componentType.GetProperty(this.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, this.PropertyType, new Type[0], new ParameterModifier[0]);
				if (this._member == null)
				{
					throw new ArgumentException("Accessor methods for the " + this.Name + " property are missing");
				}
			}
			return this._member;
		}

		public override Type ComponentType
		{
			get
			{
				return this._componentType;
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				ReadOnlyAttribute readOnlyAttribute = (ReadOnlyAttribute)this.Attributes[typeof(ReadOnlyAttribute)];
				return !this.GetPropertyInfo().CanWrite || readOnlyAttribute.IsReadOnly;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this._propertyType;
			}
		}

		protected override void FillAttributes(IList attributeList)
		{
			base.FillAttributes(attributeList);
			if (!this.GetPropertyInfo().CanWrite)
			{
				attributeList.Add(ReadOnlyAttribute.Yes);
			}
			int num = 0;
			Type type = this.ComponentType;
			while (type != null && type != typeof(object))
			{
				num++;
				type = type.BaseType;
			}
			Attribute[][] array = new Attribute[num][];
			type = this.ComponentType;
			while (type != null && type != typeof(object))
			{
				PropertyInfo property = type.GetProperty(this.Name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, this.PropertyType, new Type[0], new ParameterModifier[0]);
				if (property != null)
				{
					object[] customAttributes = property.GetCustomAttributes(false);
					Attribute[] array2 = new Attribute[customAttributes.Length];
					customAttributes.CopyTo(array2, 0);
					array[--num] = array2;
				}
				type = type.BaseType;
			}
			foreach (Attribute[] array4 in array)
			{
				if (array4 != null)
				{
					foreach (Attribute attribute in array4)
					{
						attributeList.Add(attribute);
					}
				}
			}
			foreach (object obj in TypeDescriptor.GetAttributes(this.PropertyType))
			{
				Attribute attribute2 = (Attribute)obj;
				attributeList.Add(attribute2);
			}
		}

		public override object GetValue(object component)
		{
			component = MemberDescriptor.GetInvokee(this._componentType, component);
			this.InitAccessors();
			return this.getter.GetValue(component, null);
		}

		private global::System.ComponentModel.Design.DesignerTransaction CreateTransaction(object obj, string description)
		{
			IComponent component = obj as IComponent;
			if (component == null || component.Site == null)
			{
				return null;
			}
			global::System.ComponentModel.Design.IDesignerHost designerHost = (global::System.ComponentModel.Design.IDesignerHost)component.Site.GetService(typeof(global::System.ComponentModel.Design.IDesignerHost));
			if (designerHost == null)
			{
				return null;
			}
			global::System.ComponentModel.Design.DesignerTransaction designerTransaction = designerHost.CreateTransaction(description);
			global::System.ComponentModel.Design.IComponentChangeService componentChangeService = (global::System.ComponentModel.Design.IComponentChangeService)component.Site.GetService(typeof(global::System.ComponentModel.Design.IComponentChangeService));
			if (componentChangeService != null)
			{
				componentChangeService.OnComponentChanging(component, this);
			}
			return designerTransaction;
		}

		private void EndTransaction(object obj, global::System.ComponentModel.Design.DesignerTransaction tran, object oldValue, object newValue, bool commit)
		{
			if (tran == null)
			{
				this.OnValueChanged(obj, new PropertyChangedEventArgs(this.Name));
				return;
			}
			if (commit)
			{
				IComponent component = obj as IComponent;
				global::System.ComponentModel.Design.IComponentChangeService componentChangeService = (global::System.ComponentModel.Design.IComponentChangeService)component.Site.GetService(typeof(global::System.ComponentModel.Design.IComponentChangeService));
				if (componentChangeService != null)
				{
					componentChangeService.OnComponentChanged(component, this, oldValue, newValue);
				}
				tran.Commit();
				this.OnValueChanged(obj, new PropertyChangedEventArgs(this.Name));
			}
			else
			{
				tran.Cancel();
			}
		}

		private void InitAccessors()
		{
			if (this.accessors_inited)
			{
				return;
			}
			PropertyInfo propertyInfo = this.GetPropertyInfo();
			MethodInfo methodInfo = propertyInfo.GetSetMethod(true);
			MethodInfo methodInfo2 = propertyInfo.GetGetMethod(true);
			if (methodInfo2 != null)
			{
				this.getter = propertyInfo;
			}
			if (methodInfo != null)
			{
				this.setter = propertyInfo;
			}
			if (methodInfo != null && methodInfo2 != null)
			{
				this.accessors_inited = true;
				return;
			}
			if (methodInfo == null && methodInfo2 == null)
			{
				this.accessors_inited = true;
				return;
			}
			MethodInfo methodInfo3 = ((methodInfo2 == null) ? methodInfo : methodInfo2);
			if (methodInfo3 == null || !methodInfo3.IsVirtual || (methodInfo3.Attributes & MethodAttributes.VtableLayoutMask) == MethodAttributes.VtableLayoutMask)
			{
				this.accessors_inited = true;
				return;
			}
			Type type = this._componentType.BaseType;
			while (type != null && type != typeof(object))
			{
				propertyInfo = type.GetProperty(this.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty, null, this.PropertyType, new Type[0], new ParameterModifier[0]);
				if (propertyInfo == null)
				{
					break;
				}
				if (methodInfo == null)
				{
					methodInfo3 = (methodInfo = propertyInfo.GetSetMethod());
				}
				else
				{
					methodInfo3 = (methodInfo2 = propertyInfo.GetGetMethod());
				}
				if (methodInfo2 != null && this.getter == null)
				{
					this.getter = propertyInfo;
				}
				if (methodInfo != null && this.setter == null)
				{
					this.setter = propertyInfo;
				}
				if (methodInfo3 != null)
				{
					break;
				}
				type = type.BaseType;
			}
			this.accessors_inited = true;
		}

		public override void SetValue(object component, object value)
		{
			global::System.ComponentModel.Design.DesignerTransaction designerTransaction = this.CreateTransaction(component, "Set Property '" + this.Name + "'");
			object invokee = MemberDescriptor.GetInvokee(this._componentType, component);
			object value2 = this.GetValue(invokee);
			try
			{
				this.InitAccessors();
				this.setter.SetValue(invokee, value, null);
				this.EndTransaction(component, designerTransaction, value2, value, true);
			}
			catch
			{
				this.EndTransaction(component, designerTransaction, value2, value, false);
				throw;
			}
		}

		private MethodInfo FindPropertyMethod(object o, string method_name)
		{
			MethodInfo methodInfo = null;
			string text = method_name + this.Name;
			foreach (MethodInfo methodInfo2 in o.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (methodInfo2.Name == text && methodInfo2.GetParameters().Length == 0)
				{
					methodInfo = methodInfo2;
					break;
				}
			}
			return methodInfo;
		}

		public override void ResetValue(object component)
		{
			object invokee = MemberDescriptor.GetInvokee(this._componentType, component);
			DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];
			if (defaultValueAttribute != null)
			{
				this.SetValue(invokee, defaultValueAttribute.Value);
			}
			global::System.ComponentModel.Design.DesignerTransaction designerTransaction = this.CreateTransaction(component, "Reset Property '" + this.Name + "'");
			object value = this.GetValue(invokee);
			try
			{
				MethodInfo methodInfo = this.FindPropertyMethod(invokee, "Reset");
				if (methodInfo != null)
				{
					methodInfo.Invoke(invokee, null);
				}
				this.EndTransaction(component, designerTransaction, value, this.GetValue(invokee), true);
			}
			catch
			{
				this.EndTransaction(component, designerTransaction, value, this.GetValue(invokee), false);
				throw;
			}
		}

		public override bool CanResetValue(object component)
		{
			component = MemberDescriptor.GetInvokee(this._componentType, component);
			DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];
			if (defaultValueAttribute != null)
			{
				object value = this.GetValue(component);
				if (defaultValueAttribute.Value == null || value == null)
				{
					if (defaultValueAttribute.Value != value)
					{
						return true;
					}
					if (defaultValueAttribute.Value == null && value == null)
					{
						return false;
					}
				}
				return !defaultValueAttribute.Value.Equals(value);
			}
			if (!this._member.CanWrite)
			{
				return false;
			}
			MethodInfo methodInfo = this.FindPropertyMethod(component, "ShouldPersist");
			if (methodInfo != null)
			{
				return (bool)methodInfo.Invoke(component, null);
			}
			methodInfo = this.FindPropertyMethod(component, "ShouldSerialize");
			if (methodInfo != null && !(bool)methodInfo.Invoke(component, null))
			{
				return false;
			}
			methodInfo = this.FindPropertyMethod(component, "Reset");
			return methodInfo != null;
		}

		public override bool ShouldSerializeValue(object component)
		{
			component = MemberDescriptor.GetInvokee(this._componentType, component);
			if (this.IsReadOnly)
			{
				MethodInfo methodInfo = this.FindPropertyMethod(component, "ShouldSerialize");
				if (methodInfo != null)
				{
					return (bool)methodInfo.Invoke(component, null);
				}
				return this.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);
			}
			else
			{
				DefaultValueAttribute defaultValueAttribute = (DefaultValueAttribute)this.Attributes[typeof(DefaultValueAttribute)];
				if (defaultValueAttribute == null)
				{
					MethodInfo methodInfo2 = this.FindPropertyMethod(component, "ShouldSerialize");
					return methodInfo2 == null || (bool)methodInfo2.Invoke(component, null);
				}
				object value = this.GetValue(component);
				if (defaultValueAttribute.Value == null || value == null)
				{
					return defaultValueAttribute.Value != value;
				}
				return !defaultValueAttribute.Value.Equals(value);
			}
		}

		private PropertyInfo _member;

		private Type _componentType;

		private Type _propertyType;

		private PropertyInfo getter;

		private PropertyInfo setter;

		private bool accessors_inited;
	}
}
