using System;
using System.Collections;
using System.Reflection;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal class XmlFormatterDeserializer
	{
		private XmlFormatterDeserializer(KnownTypeCollection knownTypes, IDataContractSurrogate surrogate)
		{
			this.types = knownTypes;
			this.surrogate = surrogate;
		}

		public static object Deserialize(XmlReader reader, Type type, KnownTypeCollection knownTypes, IDataContractSurrogate surrogate, string name, string ns, bool verifyObjectName)
		{
			reader.MoveToContent();
			if (verifyObjectName && (reader.NodeType != XmlNodeType.Element || reader.LocalName != name || reader.NamespaceURI != ns))
			{
				throw new SerializationException(string.Format("Expected element '{0}' in namespace '{1}', but found {2} node '{3}' in namespace '{4}'", new object[] { name, ns, reader.NodeType, reader.LocalName, reader.NamespaceURI }));
			}
			return new XmlFormatterDeserializer(knownTypes, surrogate).Deserialize(type, reader);
		}

		private static void Verify(KnownTypeCollection knownTypes, Type type, string name, string Namespace, XmlReader reader)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(reader.Name, reader.NamespaceURI);
			if (xmlQualifiedName.Name == name && xmlQualifiedName.Namespace == Namespace)
			{
				return;
			}
			for (Type type2 = type; type2 != null; type2 = type2.BaseType)
			{
				if (knownTypes.GetQName(type2) == xmlQualifiedName)
				{
					return;
				}
			}
			XmlQualifiedName qname = knownTypes.GetQName(type);
			throw new SerializationException(string.Format("Expecting element '{0}' from namespace '{1}'. Encountered 'Element' with name '{2}', namespace '{3}'", new object[] { qname.Name, qname.Namespace, xmlQualifiedName.Name, xmlQualifiedName.Namespace }));
		}

		public Hashtable References
		{
			get
			{
				return this.references;
			}
		}

		public object Deserialize(Type type, XmlReader reader)
		{
			string attribute = reader.GetAttribute("Id", "http://schemas.microsoft.com/2003/10/Serialization/");
			object obj = this.DeserializeCore(type, reader);
			if (attribute != null)
			{
				this.references.Add(attribute, obj);
			}
			return obj;
		}

		public object DeserializeCore(Type type, XmlReader reader)
		{
			XmlQualifiedName xmlQualifiedName = this.types.GetQName(type);
			string attribute = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
			if (attribute != null)
			{
				string[] array = attribute.Split(new char[] { ':' });
				if (array.Length > 1)
				{
					xmlQualifiedName = new XmlQualifiedName(array[1], reader.LookupNamespace(reader.NameTable.Get(array[0])));
				}
				else
				{
					xmlQualifiedName = new XmlQualifiedName(attribute, reader.NamespaceURI);
				}
			}
			string attribute2 = reader.GetAttribute("Ref", "http://schemas.microsoft.com/2003/10/Serialization/");
			if (attribute2 != null)
			{
				object obj = this.references[attribute2];
				if (obj == null)
				{
					throw new SerializationException(string.Format("Deserialized object with reference Id '{0}' was not found", attribute2));
				}
				reader.Skip();
				return obj;
			}
			else
			{
				bool flag = reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true";
				if (flag)
				{
					reader.Skip();
					if (!type.IsValueType)
					{
						return null;
					}
					if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
					{
						return null;
					}
					throw new SerializationException(string.Format("Value type {0} cannot be null.", type));
				}
				else
				{
					if (KnownTypeCollection.GetPrimitiveTypeFromName(xmlQualifiedName.Name) != null)
					{
						string text;
						if (reader.IsEmptyElement)
						{
							reader.Read();
							if (type.IsValueType)
							{
								return Activator.CreateInstance(type);
							}
							text = string.Empty;
						}
						else
						{
							text = reader.ReadElementContentAsString();
						}
						return KnownTypeCollection.PredefinedTypeStringToObject(text, xmlQualifiedName.Name, reader);
					}
					return this.DeserializeByMap(xmlQualifiedName, type, reader);
				}
			}
		}

		private object DeserializeByMap(XmlQualifiedName name, Type type, XmlReader reader)
		{
			SerializationMap serializationMap = this.types.FindUserMap(name);
			if (serializationMap == null && (name.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/Arrays" || name.Namespace.StartsWith("http://schemas.datacontract.org/2004/07/", StringComparison.Ordinal)))
			{
				Type typeFromNamePair = this.GetTypeFromNamePair(name.Name, name.Namespace);
				this.types.TryRegister(typeFromNamePair);
				serializationMap = this.types.FindUserMap(name);
			}
			if (serializationMap == null)
			{
				throw new SerializationException(string.Format("Unknown type {0} is used for DataContract with reference of name {1}. Any derived types of a data contract or a data member should be added to KnownTypes.", type, name));
			}
			return serializationMap.DeserializeObject(reader, this);
		}

		private Type GetTypeFromNamePair(string name, string ns)
		{
			Type primitiveTypeFromName = KnownTypeCollection.GetPrimitiveTypeFromName(name);
			if (primitiveTypeFromName != null)
			{
				return primitiveTypeFromName;
			}
			if (name.StartsWith("ArrayOf", StringComparison.Ordinal) && ns == "http://schemas.microsoft.com/2003/10/Serialization/Arrays")
			{
				return this.GetTypeFromNamePair(name.Substring(7), string.Empty).MakeArrayType();
			}
			int length = "http://schemas.datacontract.org/2004/07/".Length;
			string text = ((ns.Length <= length) ? null : ns.Substring(length));
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					DataContractAttribute customAttribute = type.GetCustomAttribute<DataContractAttribute>(true);
					if (customAttribute != null && customAttribute.Name == name && customAttribute.Namespace == ns)
					{
						return type;
					}
					if (text != null && type.Name == name && type.Namespace == text)
					{
						return type;
					}
				}
			}
			throw new XmlException(string.Format("Type not found; name: {0}, namespace: {1}", name, ns));
		}

		private KnownTypeCollection types;

		private IDataContractSurrogate surrogate;

		private Hashtable references = new Hashtable();
	}
}
