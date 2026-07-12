using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	internal readonly struct PropertyMember : IMemberInfo
	{
		public string Name { get; }

		public bool IsReadOnly
		{
			get
			{
				return !this.m_PropertyInfo.CanWrite;
			}
		}

		public Type ValueType
		{
			get
			{
				return this.m_PropertyInfo.PropertyType;
			}
		}

		public PropertyMember(PropertyInfo propertyInfo)
		{
			this.m_PropertyInfo = propertyInfo;
			this.Name = ReflectionUtilities.SanitizeMemberName(this.m_PropertyInfo);
		}

		public object GetValue(object obj)
		{
			return this.m_PropertyInfo.GetValue(obj);
		}

		public void SetValue(object obj, object value)
		{
			this.m_PropertyInfo.SetValue(obj, value);
		}

		public IEnumerable<Attribute> GetCustomAttributes()
		{
			return this.m_PropertyInfo.GetCustomAttributes();
		}

		internal readonly PropertyInfo m_PropertyInfo;
	}
}
