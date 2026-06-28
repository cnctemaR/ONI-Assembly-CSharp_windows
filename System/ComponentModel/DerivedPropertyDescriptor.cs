using System;
using System.Reflection;

namespace System.ComponentModel
{
	internal class DerivedPropertyDescriptor : PropertyDescriptor
	{
		protected DerivedPropertyDescriptor(string name, Attribute[] attrs)
			: base(name, attrs)
		{
		}

		public DerivedPropertyDescriptor(string name, Attribute[] attrs, int dummy)
			: this(name, attrs)
		{
		}

		public void SetReadOnly(bool value)
		{
			this.readOnly = value;
		}

		public void SetComponentType(Type type)
		{
			this.componentType = type;
		}

		public void SetPropertyType(Type type)
		{
			this.propertyType = type;
		}

		public override object GetValue(object component)
		{
			if (this.prop == null)
			{
				this.prop = this.componentType.GetProperty(this.Name);
			}
			return this.prop.GetValue(component, null);
		}

		public override void SetValue(object component, object value)
		{
			if (this.prop == null)
			{
				this.prop = this.componentType.GetProperty(this.Name);
			}
			this.prop.SetValue(component, value, null);
			this.OnValueChanged(component, new PropertyChangedEventArgs(this.Name));
		}

		[global::System.MonoTODO]
		public override void ResetValue(object component)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override bool CanResetValue(object component)
		{
			throw new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override bool ShouldSerializeValue(object component)
		{
			throw new NotImplementedException();
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
				return this.readOnly;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return this.propertyType;
			}
		}

		private bool readOnly;

		private Type componentType;

		private Type propertyType;

		private PropertyInfo prop;
	}
}
