using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	internal class DictionaryTypeMap : SerializationMap, ICollectionTypeMap
	{
		public DictionaryTypeMap(Type type, CollectionDataContractAttribute a, KnownTypeCollection knownTypes)
			: base(type, XmlQualifiedName.Empty, knownTypes)
		{
			this.a = a;
			this.key_type = typeof(object);
			this.value_type = typeof(object);
			Type genericDictionaryInterface = DictionaryTypeMap.GetGenericDictionaryInterface(this.RuntimeType);
			if (genericDictionaryInterface != null)
			{
				InterfaceMapping interfaceMap = this.RuntimeType.GetInterfaceMap(genericDictionaryInterface);
				for (int i = 0; i < interfaceMap.InterfaceMethods.Length; i++)
				{
					if (interfaceMap.InterfaceMethods[i].Name == "Add")
					{
						this.add_method = interfaceMap.TargetMethods[i];
						break;
					}
				}
				Type[] genericArguments = genericDictionaryInterface.GetGenericArguments();
				this.key_type = genericArguments[0];
				this.value_type = genericArguments[1];
				if (this.add_method == null)
				{
					this.add_method = type.GetMethod("Add", genericArguments);
				}
			}
			base.XmlName = this.GetDictionaryQName();
			this.item_qname = this.GetItemQName();
			this.key_qname = this.GetKeyQName();
			this.value_qname = this.GetValueQName();
		}

		private static Type GetGenericDictionaryInterface(Type type)
		{
			foreach (Type type2 in type.GetInterfaces())
			{
				if (type2.IsGenericType && type2.GetGenericTypeDefinition() == typeof(IDictionary<, >))
				{
					return type2;
				}
			}
			return null;
		}

		private string ContractNamespace
		{
			get
			{
				return (this.a == null || string.IsNullOrEmpty(this.a.Namespace)) ? "http://schemas.microsoft.com/2003/10/Serialization/Arrays" : this.a.Namespace;
			}
		}

		public Type KeyType
		{
			get
			{
				return this.key_type;
			}
		}

		public Type ValueType
		{
			get
			{
				return this.value_type;
			}
		}

		internal virtual XmlQualifiedName GetDictionaryQName()
		{
			if (this.a != null && !string.IsNullOrEmpty(this.a.Name))
			{
				return new XmlQualifiedName(this.a.Name, this.ContractNamespace);
			}
			return new XmlQualifiedName("ArrayOf" + this.GetItemQName().Name, "http://schemas.microsoft.com/2003/10/Serialization/Arrays");
		}

		internal virtual XmlQualifiedName GetItemQName()
		{
			if (this.a != null && !string.IsNullOrEmpty(this.a.ItemName))
			{
				return new XmlQualifiedName(this.a.ItemName, this.ContractNamespace);
			}
			return new XmlQualifiedName("KeyValueOf" + this.KnownTypes.GetQName(this.key_type).Name + this.KnownTypes.GetQName(this.value_type).Name, "http://schemas.microsoft.com/2003/10/Serialization/Arrays");
		}

		internal virtual XmlQualifiedName GetKeyQName()
		{
			if (this.a != null && !string.IsNullOrEmpty(this.a.KeyName))
			{
				return new XmlQualifiedName(this.a.KeyName, this.ContractNamespace);
			}
			return DictionaryTypeMap.kvpair_key_qname;
		}

		internal virtual XmlQualifiedName GetValueQName()
		{
			if (this.a != null && !string.IsNullOrEmpty(this.a.ValueName))
			{
				return new XmlQualifiedName(this.a.ValueName, this.ContractNamespace);
			}
			return DictionaryTypeMap.kvpair_value_qname;
		}

		internal virtual string CurrentNamespace
		{
			get
			{
				string text = this.item_qname.Namespace;
				if (text == "http://schemas.microsoft.com/2003/10/Serialization/")
				{
					text = "http://schemas.microsoft.com/2003/10/Serialization/Arrays";
				}
				return text;
			}
		}

		public override void SerializeNonReference(object graph, XmlFormatterSerializer serializer)
		{
			if (this.add_method != null)
			{
				if (this.pair_type == null)
				{
					this.pair_type = typeof(KeyValuePair<, >).MakeGenericType(this.add_method.DeclaringType.GetGenericArguments());
					this.pair_key_property = this.pair_type.GetProperty("Key");
					this.pair_value_property = this.pair_type.GetProperty("Value");
				}
				foreach (object obj in ((IEnumerable)graph))
				{
					serializer.WriteStartElement(this.item_qname.Name, this.item_qname.Namespace, this.CurrentNamespace);
					serializer.WriteStartElement(this.key_qname.Name, this.key_qname.Namespace, this.CurrentNamespace);
					serializer.Serialize(this.pair_key_property.PropertyType, this.pair_key_property.GetValue(obj, null));
					serializer.WriteEndElement();
					serializer.WriteStartElement(this.value_qname.Name, this.value_qname.Namespace, this.CurrentNamespace);
					serializer.Serialize(this.pair_value_property.PropertyType, this.pair_value_property.GetValue(obj, null));
					serializer.WriteEndElement();
					serializer.WriteEndElement();
				}
			}
			else
			{
				foreach (object obj2 in ((IEnumerable)graph))
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj2;
					serializer.WriteStartElement(this.item_qname.Name, this.item_qname.Namespace, this.CurrentNamespace);
					serializer.WriteStartElement(this.key_qname.Name, this.key_qname.Namespace, this.CurrentNamespace);
					serializer.Serialize(this.key_type, dictionaryEntry.Key);
					serializer.WriteEndElement();
					serializer.WriteStartElement(this.value_qname.Name, this.value_qname.Namespace, this.CurrentNamespace);
					serializer.Serialize(this.value_type, dictionaryEntry.Value);
					serializer.WriteEndElement();
					serializer.WriteEndElement();
				}
			}
		}

		private object CreateInstance()
		{
			if (!this.RuntimeType.IsInterface)
			{
				return Activator.CreateInstance(this.RuntimeType, true);
			}
			if (this.RuntimeType.IsGenericType && Array.IndexOf<Type>(this.RuntimeType.GetGenericTypeDefinition().GetInterfaces(), typeof(IDictionary<, >)) >= 0)
			{
				Type[] genericArguments = this.RuntimeType.GetGenericArguments();
				return Activator.CreateInstance(typeof(Dictionary<, >).MakeGenericType(new Type[]
				{
					genericArguments[0],
					genericArguments[1]
				}));
			}
			return new Hashtable();
		}

		public override object DeserializeEmptyContent(XmlReader reader, XmlFormatterDeserializer deserializer)
		{
			return this.DeserializeContent(reader, deserializer);
		}

		public override object DeserializeContent(XmlReader reader, XmlFormatterDeserializer deserializer)
		{
			object obj = this.CreateInstance();
			int num = ((reader.NodeType != XmlNodeType.None) ? (reader.Depth - 1) : reader.Depth);
			while (reader.NodeType == XmlNodeType.Element && reader.Depth > num)
			{
				if (reader.IsEmptyElement)
				{
					throw new XmlException(string.Format("Unexpected empty element for dictionary entry: name {0}", reader.Name));
				}
				reader.ReadStartElement();
				reader.MoveToContent();
				object obj2 = deserializer.Deserialize(this.key_type, reader);
				reader.MoveToContent();
				object obj3 = deserializer.Deserialize(this.value_type, reader);
				reader.ReadEndElement();
				if (obj is IDictionary)
				{
					((IDictionary)obj).Add(obj2, obj3);
				}
				else
				{
					if (this.add_method == null)
					{
						throw new NotImplementedException(string.Format("Type {0} is not supported", this.RuntimeType));
					}
					this.add_method.Invoke(obj, new object[] { obj2, obj3 });
				}
			}
			return obj;
		}

		public override List<DataMemberInfo> GetMembers()
		{
			throw new NotImplementedException();
		}

		public override XmlSchemaType GetSchemaType(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
		{
			throw new NotImplementedException();
		}

		private Type key_type;

		private Type value_type;

		private XmlQualifiedName dict_qname;

		private XmlQualifiedName item_qname;

		private XmlQualifiedName key_qname;

		private XmlQualifiedName value_qname;

		private MethodInfo add_method;

		private CollectionDataContractAttribute a;

		private static readonly XmlQualifiedName kvpair_key_qname = new XmlQualifiedName("Key", "http://schemas.microsoft.com/2003/10/Serialization/Arrays");

		private static readonly XmlQualifiedName kvpair_value_qname = new XmlQualifiedName("Value", "http://schemas.microsoft.com/2003/10/Serialization/Arrays");

		private Type pair_type;

		private PropertyInfo pair_key_property;

		private PropertyInfo pair_value_property;
	}
}
