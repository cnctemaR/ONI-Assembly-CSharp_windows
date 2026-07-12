using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal sealed class KnownTypeDataContractResolver : DataContractResolver
	{
		internal KnownTypeDataContractResolver(XmlObjectSerializerContext context)
		{
			this.context = context;
		}

		public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
		{
			if (type == null)
			{
				typeName = null;
				typeNamespace = null;
				return false;
			}
			if (declaredType != null && declaredType.IsInterface && CollectionDataContract.IsCollectionInterface(declaredType))
			{
				typeName = null;
				typeNamespace = null;
				return true;
			}
			DataContract dataContract = DataContract.GetDataContract(type);
			if (this.context.IsKnownType(dataContract, dataContract.KnownDataContracts, declaredType))
			{
				typeName = dataContract.Name;
				typeNamespace = dataContract.Namespace;
				return true;
			}
			typeName = null;
			typeNamespace = null;
			return false;
		}

		public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
		{
			if (typeName == null || typeNamespace == null)
			{
				return null;
			}
			return this.context.ResolveNameFromKnownTypes(new XmlQualifiedName(typeName, typeNamespace));
		}

		private XmlObjectSerializerContext context;
	}
}
