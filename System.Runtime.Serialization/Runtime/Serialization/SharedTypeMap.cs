using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class SharedTypeMap : SerializationMap
	{
		public SharedTypeMap(Type type, XmlQualifiedName qname, KnownTypeCollection knownTypes)
			: base(type, qname, knownTypes)
		{
			this.Members = this.GetMembers(type, base.XmlName, false);
		}

		private List<DataMemberInfo> GetMembers(Type type, XmlQualifiedName qname, bool declared_only)
		{
			List<DataMemberInfo> list = new List<DataMemberInfo>();
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
			if (declared_only)
			{
				bindingFlags |= BindingFlags.DeclaredOnly;
			}
			foreach (FieldInfo fieldInfo in type.GetFields(bindingFlags))
			{
				if (fieldInfo.GetCustomAttributes(typeof(NonSerializedAttribute), false).Length <= 0)
				{
					if (fieldInfo.IsInitOnly)
					{
						throw new InvalidDataContractException(string.Format("DataMember field {0} must not be read-only.", fieldInfo));
					}
					DataMemberAttribute dataMemberAttribute = new DataMemberAttribute();
					list.Add(base.CreateDataMemberInfo(dataMemberAttribute, fieldInfo, fieldInfo.FieldType));
				}
			}
			list.Sort(DataMemberInfo.DataMemberInfoComparer.Instance);
			return list;
		}

		public override List<DataMemberInfo> GetMembers()
		{
			return this.Members;
		}
	}
}
