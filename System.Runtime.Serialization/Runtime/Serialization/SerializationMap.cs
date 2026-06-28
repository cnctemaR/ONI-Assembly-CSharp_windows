using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	internal abstract class SerializationMap
	{
		protected SerializationMap(Type type, XmlQualifiedName qname, KnownTypeCollection knownTypes)
		{
			this.KnownTypes = knownTypes;
			this.RuntimeType = type;
			if (qname.Namespace == null)
			{
				qname = new XmlQualifiedName(qname.Name, "http://schemas.datacontract.org/2004/07/" + type.Namespace);
			}
			this.XmlName = qname;
			this.Members = new List<DataMemberInfo>();
		}

		public virtual bool OutputXsiType
		{
			get
			{
				return true;
			}
		}

		public XmlQualifiedName XmlName { get; set; }

		public CollectionDataContractAttribute GetCollectionDataContractAttribute(Type type)
		{
			object[] customAttributes = type.GetCustomAttributes(typeof(CollectionDataContractAttribute), false);
			return (customAttributes.Length != 0) ? ((CollectionDataContractAttribute)customAttributes[0]) : null;
		}

		public DataMemberAttribute GetDataMemberAttribute(MemberInfo mi)
		{
			object[] customAttributes = mi.GetCustomAttributes(typeof(DataMemberAttribute), false);
			if (customAttributes.Length == 0)
			{
				return null;
			}
			return (DataMemberAttribute)customAttributes[0];
		}

		private bool IsPrimitive(Type type)
		{
			return Type.GetTypeCode(type) != TypeCode.Object || type == typeof(object);
		}

		public virtual XmlSchemaType GetSchemaType(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
		{
			if (this.IsPrimitive(this.RuntimeType))
			{
				return null;
			}
			if (generated_schema_types.ContainsKey(this.XmlName))
			{
				return generated_schema_types[this.XmlName];
			}
			XmlSchemaComplexType xmlSchemaComplexType = new XmlSchemaComplexType();
			xmlSchemaComplexType.Name = this.XmlName.Name;
			generated_schema_types[this.XmlName] = xmlSchemaComplexType;
			if (this.RuntimeType.BaseType == typeof(object))
			{
				xmlSchemaComplexType.Particle = this.GetSequence(schemas, generated_schema_types);
			}
			else
			{
				XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = new XmlSchemaComplexContentExtension();
				XmlSchemaComplexContent xmlSchemaComplexContent = new XmlSchemaComplexContent();
				xmlSchemaComplexType.ContentModel = xmlSchemaComplexContent;
				xmlSchemaComplexContent.Content = xmlSchemaComplexContentExtension;
				this.KnownTypes.Add(this.RuntimeType.BaseType);
				SerializationMap serializationMap = this.KnownTypes.FindUserMap(this.RuntimeType.BaseType);
				serializationMap.GetSchemaType(schemas, generated_schema_types);
				xmlSchemaComplexContentExtension.Particle = this.GetSequence(schemas, generated_schema_types);
				xmlSchemaComplexContentExtension.BaseTypeName = this.GetQualifiedName(this.RuntimeType.BaseType);
			}
			XmlSchemaElement schemaElement = this.GetSchemaElement(this.XmlName, xmlSchemaComplexType);
			XmlSchema schema = this.GetSchema(schemas, this.XmlName.Namespace);
			schema.Items.Add(xmlSchemaComplexType);
			schema.Items.Add(schemaElement);
			schemas.Reprocess(schema);
			return xmlSchemaComplexType;
		}

		private XmlSchemaSequence GetSequence(XmlSchemaSet schemas, Dictionary<XmlQualifiedName, XmlSchemaType> generated_schema_types)
		{
			List<DataMemberInfo> members = this.GetMembers();
			XmlSchema schema = this.GetSchema(schemas, this.XmlName.Namespace);
			XmlSchemaSequence xmlSchemaSequence = new XmlSchemaSequence();
			foreach (DataMemberInfo dataMemberInfo in members)
			{
				if (dataMemberInfo.MemberType.IsAbstract || !typeof(Delegate).IsAssignableFrom(dataMemberInfo.MemberType))
				{
					XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
					xmlSchemaElement.Name = dataMemberInfo.XmlName;
					this.KnownTypes.Add(dataMemberInfo.MemberType);
					SerializationMap serializationMap = this.KnownTypes.FindUserMap(dataMemberInfo.MemberType);
					if (serializationMap != null)
					{
						XmlSchemaType schemaType = serializationMap.GetSchemaType(schemas, generated_schema_types);
						if (schemaType is XmlSchemaComplexType)
						{
							xmlSchemaElement.IsNillable = true;
						}
					}
					else if (dataMemberInfo.MemberType == typeof(string))
					{
						xmlSchemaElement.IsNillable = true;
					}
					xmlSchemaElement.MinOccurs = 0m;
					xmlSchemaElement.SchemaTypeName = this.GetQualifiedName(dataMemberInfo.MemberType);
					this.AddImport(schema, xmlSchemaElement.SchemaTypeName.Namespace);
					xmlSchemaSequence.Items.Add(xmlSchemaElement);
				}
			}
			schemas.Reprocess(schema);
			return xmlSchemaSequence;
		}

		private void AddImport(XmlSchema schema, string ns)
		{
			if (ns == "http://www.w3.org/2001/XMLSchema" || schema.TargetNamespace == ns)
			{
				return;
			}
			foreach (XmlSchemaObject xmlSchemaObject in schema.Includes)
			{
				XmlSchemaImport xmlSchemaImport = xmlSchemaObject as XmlSchemaImport;
				if (xmlSchemaImport != null)
				{
					if (xmlSchemaImport.Namespace == ns)
					{
						return;
					}
				}
			}
			XmlSchemaImport xmlSchemaImport2 = new XmlSchemaImport();
			xmlSchemaImport2.Namespace = ns;
			schema.Includes.Add(xmlSchemaImport2);
		}

		public virtual List<DataMemberInfo> GetMembers()
		{
			throw new NotImplementedException(string.Format("Implement me for {0}", this));
		}

		protected XmlSchemaElement GetSchemaElement(XmlQualifiedName qname, XmlSchemaType schemaType)
		{
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = qname.Name;
			xmlSchemaElement.SchemaTypeName = qname;
			if (schemaType is XmlSchemaComplexType)
			{
				xmlSchemaElement.IsNillable = true;
			}
			return xmlSchemaElement;
		}

		protected XmlSchema GetSchema(XmlSchemaSet schemas, string ns)
		{
			ICollection collection = schemas.Schemas(ns);
			if (collection.Count > 0)
			{
				if (collection.Count > 1)
				{
					throw new Exception(string.Format("More than 1 schema for namespace '{0}' found.", ns));
				}
				using (IEnumerator enumerator = collection.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						return obj as XmlSchema;
					}
				}
			}
			XmlSchema xmlSchema = new XmlSchema();
			xmlSchema.TargetNamespace = ns;
			xmlSchema.ElementFormDefault = XmlSchemaForm.Qualified;
			schemas.Add(xmlSchema);
			return xmlSchema;
		}

		protected XmlQualifiedName GetQualifiedName(Type type)
		{
			if (this.qname_table.ContainsKey(type))
			{
				return this.qname_table[type];
			}
			XmlQualifiedName xmlQualifiedName = this.KnownTypes.GetQName(type);
			if (xmlQualifiedName.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/")
			{
				xmlQualifiedName = new XmlQualifiedName(xmlQualifiedName.Name, "http://www.w3.org/2001/XMLSchema");
			}
			this.qname_table[type] = xmlQualifiedName;
			return xmlQualifiedName;
		}

		public virtual void Serialize(object graph, XmlFormatterSerializer serializer)
		{
			string text = null;
			if (this.IsReference)
			{
				text = (string)serializer.References[graph];
				if (text != null)
				{
					serializer.Writer.WriteAttributeString("z", "Ref", "http://schemas.microsoft.com/2003/10/Serialization/", text);
					return;
				}
				text = "i" + (serializer.References.Count + 1);
				serializer.References.Add(graph, text);
			}
			else if (serializer.SerializingObjects.Contains(graph))
			{
				throw new SerializationException(string.Format("Circular reference of an object in the object graph was found: '{0}' of type {1}", graph, graph.GetType()));
			}
			serializer.SerializingObjects.Add(graph);
			if (text != null)
			{
				serializer.Writer.WriteAttributeString("z", "Id", "http://schemas.microsoft.com/2003/10/Serialization/", text);
			}
			this.SerializeNonReference(graph, serializer);
			serializer.SerializingObjects.Remove(graph);
		}

		public virtual void SerializeNonReference(object graph, XmlFormatterSerializer serializer)
		{
			foreach (DataMemberInfo dataMemberInfo in this.Members)
			{
				FieldInfo fieldInfo = dataMemberInfo.Member as FieldInfo;
				PropertyInfo propertyInfo = ((fieldInfo != null) ? null : ((PropertyInfo)dataMemberInfo.Member));
				Type type = ((fieldInfo == null) ? propertyInfo.PropertyType : fieldInfo.FieldType);
				object obj = ((fieldInfo == null) ? propertyInfo.GetValue(graph, null) : fieldInfo.GetValue(graph));
				serializer.WriteStartElement(dataMemberInfo.XmlName, dataMemberInfo.XmlRootNamespace, dataMemberInfo.XmlNamespace);
				serializer.Serialize(type, obj);
				serializer.WriteEndElement();
			}
		}

		public virtual object DeserializeObject(XmlReader reader, XmlFormatterDeserializer deserializer)
		{
			bool isEmptyElement = reader.IsEmptyElement;
			reader.ReadStartElement();
			reader.MoveToContent();
			object obj;
			if (isEmptyElement)
			{
				obj = this.DeserializeEmptyContent(reader, deserializer);
			}
			else
			{
				obj = this.DeserializeContent(reader, deserializer);
			}
			reader.MoveToContent();
			if (!isEmptyElement && reader.NodeType == XmlNodeType.EndElement)
			{
				reader.ReadEndElement();
			}
			else if (!isEmptyElement && reader.NodeType != XmlNodeType.None)
			{
				IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
				throw new SerializationException(string.Format("Deserializing type '{3}'. Expecting state 'EndElement'. Encountered state '{0}' with name '{1}' with namespace '{2}'.{4}", new object[]
				{
					reader.NodeType,
					reader.Name,
					reader.NamespaceURI,
					this.RuntimeType.FullName,
					(xmlLineInfo == null || !xmlLineInfo.HasLineInfo()) ? string.Empty : string.Format(" {0}({1},{2})", reader.BaseURI, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition)
				}));
			}
			return obj;
		}

		public virtual object DeserializeEmptyContent(XmlReader reader, XmlFormatterDeserializer deserializer)
		{
			return this.DeserializeContent(reader, deserializer, true);
		}

		public virtual object DeserializeContent(XmlReader reader, XmlFormatterDeserializer deserializer)
		{
			return this.DeserializeContent(reader, deserializer, false);
		}

		private object DeserializeContent(XmlReader reader, XmlFormatterDeserializer deserializer, bool empty)
		{
			object uninitializedObject = FormatterServices.GetUninitializedObject(this.RuntimeType);
			int num = ((reader.NodeType != XmlNodeType.None) ? (reader.Depth - 1) : reader.Depth);
			bool[] array = new bool[this.Members.Count];
			int num2 = -1;
			int num3 = -1;
			while (!empty && reader.NodeType == XmlNodeType.Element && reader.Depth > num)
			{
				DataMemberInfo dataMemberInfo = null;
				int i;
				for (i = 0; i < this.Members.Count; i++)
				{
					if (this.Members[i].Order >= 0)
					{
						break;
					}
					if (reader.LocalName == this.Members[i].XmlName && reader.NamespaceURI == this.Members[i].XmlRootNamespace)
					{
						num2 = i;
						dataMemberInfo = this.Members[i];
						break;
					}
				}
				for (i = Math.Max(i, num3); i < this.Members.Count; i++)
				{
					if (dataMemberInfo != null)
					{
						break;
					}
					if (reader.LocalName == this.Members[i].XmlName && reader.NamespaceURI == this.Members[i].XmlRootNamespace)
					{
						num2 = i;
						num3 = i;
						dataMemberInfo = this.Members[i];
						break;
					}
				}
				if (dataMemberInfo == null)
				{
					reader.Skip();
				}
				else
				{
					this.SetValue(dataMemberInfo, uninitializedObject, deserializer.Deserialize(dataMemberInfo.MemberType, reader));
					array[num2] = true;
					reader.MoveToContent();
				}
			}
			for (int j = 0; j < this.Members.Count; j++)
			{
				if (!array[j] && this.Members[j].IsRequired)
				{
					throw this.MissingRequiredMember(this.Members[j], reader);
				}
			}
			return uninitializedObject;
		}

		protected Exception MissingRequiredMember(DataMemberInfo dmi, XmlReader reader)
		{
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			return new ArgumentException(string.Format("Data contract member {0} for the type {1} is required, but missing in the input XML.{2}", new XmlQualifiedName(dmi.XmlName, dmi.XmlNamespace), this.RuntimeType, (xmlLineInfo == null || !xmlLineInfo.HasLineInfo()) ? null : string.Format(" {0}({1},{2})", reader.BaseURI, xmlLineInfo.LineNumber, xmlLineInfo.LinePosition)));
		}

		protected void SetValue(DataMemberInfo dmi, object obj, object value)
		{
			try
			{
				if (dmi.Member is PropertyInfo)
				{
					((PropertyInfo)dmi.Member).SetValue(obj, value, null);
				}
				else
				{
					((FieldInfo)dmi.Member).SetValue(obj, value);
				}
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to set value of type {0} for property {1}", (value == null) ? null : value.GetType(), dmi.Member), ex);
			}
		}

		protected DataMemberInfo CreateDataMemberInfo(DataMemberAttribute dma, MemberInfo mi, Type type)
		{
			this.KnownTypes.Add(type);
			XmlQualifiedName qname = this.KnownTypes.GetQName(type);
			string @namespace = this.KnownTypes.GetQName(mi.DeclaringType).Namespace;
			if (KnownTypeCollection.GetPrimitiveTypeFromName(qname.Name) != null)
			{
				return new DataMemberInfo(mi, dma, @namespace, null);
			}
			return new DataMemberInfo(mi, dma, @namespace, qname.Namespace);
		}

		public const BindingFlags AllInstanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		public readonly KnownTypeCollection KnownTypes;

		public readonly Type RuntimeType;

		public bool IsReference;

		public List<DataMemberInfo> Members;

		private XmlSchemaSet schema_set;

		private Dictionary<Type, XmlQualifiedName> qname_table = new Dictionary<Type, XmlQualifiedName>();
	}
}
