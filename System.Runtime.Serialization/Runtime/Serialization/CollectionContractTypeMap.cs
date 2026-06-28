using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class CollectionContractTypeMap : CollectionTypeMap
	{
		public CollectionContractTypeMap(Type type, CollectionDataContractAttribute a, Type elementType, XmlQualifiedName qname, KnownTypeCollection knownTypes)
			: base(type, elementType, qname, knownTypes)
		{
			this.IsReference = a.IsReference;
		}

		internal override string CurrentNamespace
		{
			get
			{
				return base.XmlName.Namespace;
			}
		}
	}
}
