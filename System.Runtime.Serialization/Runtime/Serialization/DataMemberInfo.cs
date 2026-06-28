using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace System.Runtime.Serialization
{
	internal class DataMemberInfo
	{
		public DataMemberInfo(MemberInfo member, DataMemberAttribute dma, string rootNamespce, string ns)
		{
			if (dma == null)
			{
				throw new ArgumentNullException("dma");
			}
			this.Order = dma.Order;
			this.Member = member;
			this.IsRequired = dma.IsRequired;
			this.XmlName = ((dma.Name == null) ? member.Name : dma.Name);
			this.XmlNamespace = ns;
			this.XmlRootNamespace = rootNamespce;
			if (this.Member is FieldInfo)
			{
				this.MemberType = ((FieldInfo)this.Member).FieldType;
			}
			else
			{
				this.MemberType = ((PropertyInfo)this.Member).PropertyType;
			}
		}

		public readonly int Order;

		public readonly bool IsRequired;

		public readonly string XmlName;

		public readonly MemberInfo Member;

		public readonly string XmlNamespace;

		public readonly string XmlRootNamespace;

		public readonly Type MemberType;

		public class DataMemberInfoComparer : IComparer<DataMemberInfo>, IComparer
		{
			private DataMemberInfoComparer()
			{
			}

			public int Compare(object o1, object o2)
			{
				return this.Compare((DataMemberInfo)o1, (DataMemberInfo)o2);
			}

			public int Compare(DataMemberInfo d1, DataMemberInfo d2)
			{
				if (d1.Order == d2.Order)
				{
					return string.CompareOrdinal(d1.XmlName, d2.XmlName);
				}
				return d1.Order - d2.Order;
			}

			public static readonly DataMemberInfo.DataMemberInfoComparer Instance = new DataMemberInfo.DataMemberInfoComparer();
		}
	}
}
