using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Properties.Internal;

namespace Unity.Properties
{
	internal readonly struct FieldMember : IMemberInfo
	{
		public FieldMember(FieldInfo fieldInfo)
		{
			this.m_FieldInfo = fieldInfo;
			this.Name = ReflectionUtilities.SanitizeMemberName(this.m_FieldInfo);
		}

		public string Name { get; }

		public bool IsReadOnly
		{
			get
			{
				return this.m_FieldInfo.IsInitOnly;
			}
		}

		public Type ValueType
		{
			get
			{
				return this.m_FieldInfo.FieldType;
			}
		}

		public object GetValue(object obj)
		{
			return this.m_FieldInfo.GetValue(obj);
		}

		public void SetValue(object obj, object value)
		{
			this.m_FieldInfo.SetValue(obj, value);
		}

		public IEnumerable<Attribute> GetCustomAttributes()
		{
			return this.m_FieldInfo.GetCustomAttributes();
		}

		internal readonly FieldInfo m_FieldInfo;
	}
}
