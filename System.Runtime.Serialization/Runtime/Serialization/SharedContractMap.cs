using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class SharedContractMap : SerializationMap
	{
		public SharedContractMap(Type type, XmlQualifiedName qname, KnownTypeCollection knownTypes)
			: base(type, qname, knownTypes)
		{
		}

		internal void Initialize()
		{
			Type type = this.RuntimeType;
			List<DataMemberInfo> list = new List<DataMemberInfo>();
			object[] customAttributes = type.GetCustomAttributes(typeof(DataContractAttribute), false);
			this.IsReference = customAttributes.Length > 0 && ((DataContractAttribute)customAttributes[0]).IsReference;
			while (type != null)
			{
				XmlQualifiedName qname = this.KnownTypes.GetQName(type);
				list = this.GetMembers(type, qname, true);
				list.Sort(DataMemberInfo.DataMemberInfoComparer.Instance);
				this.Members.InsertRange(0, list);
				list.Clear();
				type = type.BaseType;
			}
		}

		private List<DataMemberInfo> GetMembers(Type type, XmlQualifiedName qname, bool declared_only)
		{
			List<DataMemberInfo> list = new List<DataMemberInfo>();
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			if (declared_only)
			{
				bindingFlags |= BindingFlags.DeclaredOnly;
			}
			foreach (PropertyInfo propertyInfo in type.GetProperties(bindingFlags))
			{
				DataMemberAttribute dataMemberAttribute = base.GetDataMemberAttribute(propertyInfo);
				if (dataMemberAttribute != null)
				{
					this.KnownTypes.TryRegister(propertyInfo.PropertyType);
					SerializationMap serializationMap = this.KnownTypes.FindUserMap(propertyInfo.PropertyType);
					if (!propertyInfo.CanRead || (!propertyInfo.CanWrite && !(serializationMap is ICollectionTypeMap)))
					{
						throw new InvalidDataContractException(string.Format("DataMember property '{0}' on type '{1}' must have both getter and setter.", propertyInfo, propertyInfo.DeclaringType));
					}
					list.Add(base.CreateDataMemberInfo(dataMemberAttribute, propertyInfo, propertyInfo.PropertyType));
				}
			}
			foreach (FieldInfo fieldInfo in type.GetFields(bindingFlags))
			{
				DataMemberAttribute dataMemberAttribute2 = base.GetDataMemberAttribute(fieldInfo);
				if (dataMemberAttribute2 != null)
				{
					list.Add(base.CreateDataMemberInfo(dataMemberAttribute2, fieldInfo, fieldInfo.FieldType));
				}
			}
			return list;
		}

		public override List<DataMemberInfo> GetMembers()
		{
			return this.Members;
		}
	}
}
