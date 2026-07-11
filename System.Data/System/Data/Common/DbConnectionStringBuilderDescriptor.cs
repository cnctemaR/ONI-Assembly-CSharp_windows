using System;
using System.ComponentModel;

namespace System.Data.Common
{
	internal class DbConnectionStringBuilderDescriptor : PropertyDescriptor
	{
		internal DbConnectionStringBuilderDescriptor(string propertyName, Type componentType, Type propertyType, bool isReadOnly, Attribute[] attributes)
			: base(propertyName, attributes)
		{
			this.ComponentType = componentType;
			this.PropertyType = propertyType;
			this.IsReadOnly = isReadOnly;
		}

		internal bool RefreshOnChange { get; set; }

		public override Type ComponentType { get; }

		public override bool IsReadOnly { get; }

		public override Type PropertyType { get; }

		public override bool CanResetValue(object component)
		{
			DbConnectionStringBuilder dbConnectionStringBuilder = component as DbConnectionStringBuilder;
			return dbConnectionStringBuilder != null && dbConnectionStringBuilder.ShouldSerialize(this.DisplayName);
		}

		public override object GetValue(object component)
		{
			DbConnectionStringBuilder dbConnectionStringBuilder = component as DbConnectionStringBuilder;
			object obj;
			if (dbConnectionStringBuilder != null && dbConnectionStringBuilder.TryGetValue(this.DisplayName, out obj))
			{
				return obj;
			}
			return null;
		}

		public override void ResetValue(object component)
		{
			DbConnectionStringBuilder dbConnectionStringBuilder = component as DbConnectionStringBuilder;
			if (dbConnectionStringBuilder != null)
			{
				dbConnectionStringBuilder.Remove(this.DisplayName);
				if (this.RefreshOnChange)
				{
					dbConnectionStringBuilder.ClearPropertyDescriptors();
				}
			}
		}

		public override void SetValue(object component, object value)
		{
			DbConnectionStringBuilder dbConnectionStringBuilder = component as DbConnectionStringBuilder;
			if (dbConnectionStringBuilder != null)
			{
				if (typeof(string) == this.PropertyType && string.Empty.Equals(value))
				{
					value = null;
				}
				dbConnectionStringBuilder[this.DisplayName] = value;
				if (this.RefreshOnChange)
				{
					dbConnectionStringBuilder.ClearPropertyDescriptors();
				}
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			DbConnectionStringBuilder dbConnectionStringBuilder = component as DbConnectionStringBuilder;
			return dbConnectionStringBuilder != null && dbConnectionStringBuilder.ShouldSerialize(this.DisplayName);
		}
	}
}
