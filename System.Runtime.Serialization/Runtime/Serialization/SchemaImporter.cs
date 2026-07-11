using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Diagnostics;
using System.Runtime.Serialization.Diagnostics;
using System.Security;
using System.Xml;
using System.Xml.Schema;

namespace System.Runtime.Serialization
{
	internal class SchemaImporter
	{
		internal SchemaImporter(XmlSchemaSet schemas, ICollection<XmlQualifiedName> typeNames, ICollection<XmlSchemaElement> elements, XmlQualifiedName[] elementTypeNames, DataContractSet dataContractSet, bool importXmlDataType)
		{
			this.dataContractSet = dataContractSet;
			this.schemaSet = schemas;
			this.typeNames = typeNames;
			this.elements = elements;
			this.elementTypeNames = elementTypeNames;
			this.importXmlDataType = importXmlDataType;
		}

		internal void Import()
		{
			if (!this.schemaSet.Contains("http://schemas.microsoft.com/2003/10/Serialization/"))
			{
				XmlSchema xmlSchema = XmlSchema.Read(new XmlTextReader(new StringReader("<?xml version='1.0' encoding='utf-8'?>\n<xs:schema elementFormDefault='qualified' attributeFormDefault='qualified' xmlns:tns='http://schemas.microsoft.com/2003/10/Serialization/' targetNamespace='http://schemas.microsoft.com/2003/10/Serialization/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>\n  <xs:element name='anyType' nillable='true' type='xs:anyType' />\n  <xs:element name='anyURI' nillable='true' type='xs:anyURI' />\n  <xs:element name='base64Binary' nillable='true' type='xs:base64Binary' />\n  <xs:element name='boolean' nillable='true' type='xs:boolean' />\n  <xs:element name='byte' nillable='true' type='xs:byte' />\n  <xs:element name='dateTime' nillable='true' type='xs:dateTime' />\n  <xs:element name='decimal' nillable='true' type='xs:decimal' />\n  <xs:element name='double' nillable='true' type='xs:double' />\n  <xs:element name='float' nillable='true' type='xs:float' />\n  <xs:element name='int' nillable='true' type='xs:int' />\n  <xs:element name='long' nillable='true' type='xs:long' />\n  <xs:element name='QName' nillable='true' type='xs:QName' />\n  <xs:element name='short' nillable='true' type='xs:short' />\n  <xs:element name='string' nillable='true' type='xs:string' />\n  <xs:element name='unsignedByte' nillable='true' type='xs:unsignedByte' />\n  <xs:element name='unsignedInt' nillable='true' type='xs:unsignedInt' />\n  <xs:element name='unsignedLong' nillable='true' type='xs:unsignedLong' />\n  <xs:element name='unsignedShort' nillable='true' type='xs:unsignedShort' />\n  <xs:element name='char' nillable='true' type='tns:char' />\n  <xs:simpleType name='char'>\n    <xs:restriction base='xs:int'/>\n  </xs:simpleType>  \n  <xs:element name='duration' nillable='true' type='tns:duration' />\n  <xs:simpleType name='duration'>\n    <xs:restriction base='xs:duration'>\n      <xs:pattern value='\\-?P(\\d*D)?(T(\\d*H)?(\\d*M)?(\\d*(\\.\\d*)?S)?)?' />\n      <xs:minInclusive value='-P10675199DT2H48M5.4775808S' />\n      <xs:maxInclusive value='P10675199DT2H48M5.4775807S' />\n    </xs:restriction>\n  </xs:simpleType>\n  <xs:element name='guid' nillable='true' type='tns:guid' />\n  <xs:simpleType name='guid'>\n    <xs:restriction base='xs:string'>\n      <xs:pattern value='[\\da-fA-F]{8}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{12}' />\n    </xs:restriction>\n  </xs:simpleType>\n  <xs:attribute name='FactoryType' type='xs:QName' />\n  <xs:attribute name='Id' type='xs:ID' />\n  <xs:attribute name='Ref' type='xs:IDREF' />\n</xs:schema>\n"))
				{
					DtdProcessing = DtdProcessing.Prohibit
				}, null);
				if (xmlSchema == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(global::System.Runtime.Serialization.SR.GetString("Could not read serialization schema for '{0}' namespace.", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/" })));
				}
				this.schemaSet.Add(xmlSchema);
			}
			try
			{
				SchemaImporter.CompileSchemaSet(this.schemaSet);
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Cannot import invalid schemas."), ex));
			}
			if (this.typeNames == null)
			{
				using (IEnumerator enumerator = this.schemaSet.Schemas().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						if (obj == null)
						{
							throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Cannot import from schema list that contains null.")));
						}
						XmlSchema xmlSchema2 = (XmlSchema)obj;
						if (xmlSchema2.TargetNamespace != "http://schemas.microsoft.com/2003/10/Serialization/" && xmlSchema2.TargetNamespace != "http://www.w3.org/2001/XMLSchema")
						{
							foreach (object obj2 in xmlSchema2.SchemaTypes.Values)
							{
								XmlSchemaObject xmlSchemaObject = (XmlSchemaObject)obj2;
								this.ImportType((XmlSchemaType)xmlSchemaObject);
							}
							foreach (object obj3 in xmlSchema2.Elements.Values)
							{
								XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)obj3;
								if (xmlSchemaElement.SchemaType != null)
								{
									this.ImportAnonymousGlobalElement(xmlSchemaElement, xmlSchemaElement.QualifiedName, xmlSchema2.TargetNamespace);
								}
							}
						}
					}
					goto IL_030E;
				}
			}
			foreach (XmlQualifiedName xmlQualifiedName in this.typeNames)
			{
				if (xmlQualifiedName == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(global::System.Runtime.Serialization.SR.GetString("Cannot import data contract with null name.")));
				}
				this.ImportType(xmlQualifiedName);
			}
			if (this.elements != null)
			{
				int num = 0;
				foreach (XmlSchemaElement xmlSchemaElement2 in this.elements)
				{
					XmlQualifiedName schemaTypeName = xmlSchemaElement2.SchemaTypeName;
					if (schemaTypeName != null && schemaTypeName.Name.Length > 0)
					{
						this.elementTypeNames[num++] = this.ImportType(schemaTypeName).StableName;
					}
					else
					{
						XmlSchema schemaWithGlobalElementDeclaration = SchemaHelper.GetSchemaWithGlobalElementDeclaration(xmlSchemaElement2, this.schemaSet);
						if (schemaWithGlobalElementDeclaration == null)
						{
							this.elementTypeNames[num++] = this.ImportAnonymousElement(xmlSchemaElement2, xmlSchemaElement2.QualifiedName).StableName;
						}
						else
						{
							this.elementTypeNames[num++] = this.ImportAnonymousGlobalElement(xmlSchemaElement2, xmlSchemaElement2.QualifiedName, schemaWithGlobalElementDeclaration.TargetNamespace).StableName;
						}
					}
				}
			}
			IL_030E:
			this.ImportKnownTypesForObject();
		}

		internal static void CompileSchemaSet(XmlSchemaSet schemaSet)
		{
			if (schemaSet.Contains("http://www.w3.org/2001/XMLSchema"))
			{
				schemaSet.Compile();
				return;
			}
			XmlSchema xmlSchema = new XmlSchema();
			xmlSchema.TargetNamespace = "http://www.w3.org/2001/XMLSchema";
			XmlSchemaElement xmlSchemaElement = new XmlSchemaElement();
			xmlSchemaElement.Name = "schema";
			xmlSchemaElement.SchemaType = new XmlSchemaComplexType();
			xmlSchema.Items.Add(xmlSchemaElement);
			schemaSet.Add(xmlSchema);
			schemaSet.Compile();
		}

		private Dictionary<XmlQualifiedName, SchemaObjectInfo> SchemaObjects
		{
			get
			{
				if (this.schemaObjects == null)
				{
					this.schemaObjects = this.CreateSchemaObjects();
				}
				return this.schemaObjects;
			}
		}

		private List<XmlSchemaRedefine> RedefineList
		{
			get
			{
				if (this.redefineList == null)
				{
					this.redefineList = this.CreateRedefineList();
				}
				return this.redefineList;
			}
		}

		private void ImportKnownTypes(XmlQualifiedName typeName)
		{
			SchemaObjectInfo schemaObjectInfo;
			if (this.SchemaObjects.TryGetValue(typeName, out schemaObjectInfo))
			{
				List<XmlSchemaType> knownTypes = schemaObjectInfo.knownTypes;
				if (knownTypes != null)
				{
					foreach (XmlSchemaType xmlSchemaType in knownTypes)
					{
						this.ImportType(xmlSchemaType);
					}
				}
			}
		}

		internal static bool IsObjectContract(DataContract dataContract)
		{
			Dictionary<Type, object> dictionary = new Dictionary<Type, object>();
			while (dataContract is CollectionDataContract)
			{
				if (dataContract.OriginalUnderlyingType == null)
				{
					dataContract = ((CollectionDataContract)dataContract).ItemContract;
				}
				else
				{
					if (dictionary.ContainsKey(dataContract.OriginalUnderlyingType))
					{
						break;
					}
					dictionary.Add(dataContract.OriginalUnderlyingType, dataContract.OriginalUnderlyingType);
					dataContract = ((CollectionDataContract)dataContract).ItemContract;
				}
			}
			return dataContract is PrimitiveDataContract && ((PrimitiveDataContract)dataContract).UnderlyingType == Globals.TypeOfObject;
		}

		private void ImportKnownTypesForObject()
		{
			if (!this.needToImportKnownTypesForObject)
			{
				return;
			}
			this.needToImportKnownTypesForObject = false;
			SchemaObjectInfo schemaObjectInfo;
			if (this.dataContractSet.KnownTypesForObject == null && this.SchemaObjects.TryGetValue(SchemaExporter.AnytypeQualifiedName, out schemaObjectInfo))
			{
				List<XmlSchemaType> knownTypes = schemaObjectInfo.knownTypes;
				if (knownTypes != null)
				{
					Dictionary<XmlQualifiedName, DataContract> dictionary = new Dictionary<XmlQualifiedName, DataContract>();
					foreach (XmlSchemaType xmlSchemaType in knownTypes)
					{
						DataContract dataContract = this.ImportType(xmlSchemaType);
						DataContract dataContract2;
						if (!dictionary.TryGetValue(dataContract.StableName, out dataContract2))
						{
							dictionary.Add(dataContract.StableName, dataContract);
						}
					}
					this.dataContractSet.KnownTypesForObject = dictionary;
				}
			}
		}

		internal Dictionary<XmlQualifiedName, SchemaObjectInfo> CreateSchemaObjects()
		{
			Dictionary<XmlQualifiedName, SchemaObjectInfo> dictionary = new Dictionary<XmlQualifiedName, SchemaObjectInfo>();
			ICollection collection = this.schemaSet.Schemas();
			List<XmlSchemaType> list = new List<XmlSchemaType>();
			dictionary.Add(SchemaExporter.AnytypeQualifiedName, new SchemaObjectInfo(null, null, null, list));
			foreach (object obj in collection)
			{
				XmlSchema xmlSchema = (XmlSchema)obj;
				if (xmlSchema.TargetNamespace != "http://schemas.microsoft.com/2003/10/Serialization/")
				{
					foreach (object obj2 in xmlSchema.SchemaTypes.Values)
					{
						XmlSchemaType xmlSchemaType = ((XmlSchemaObject)obj2) as XmlSchemaType;
						if (xmlSchemaType != null)
						{
							list.Add(xmlSchemaType);
							XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(xmlSchemaType.Name, xmlSchema.TargetNamespace);
							SchemaObjectInfo schemaObjectInfo;
							if (dictionary.TryGetValue(xmlQualifiedName, out schemaObjectInfo))
							{
								schemaObjectInfo.type = xmlSchemaType;
								schemaObjectInfo.schema = xmlSchema;
							}
							else
							{
								dictionary.Add(xmlQualifiedName, new SchemaObjectInfo(xmlSchemaType, null, xmlSchema, null));
							}
							XmlQualifiedName baseTypeName = this.GetBaseTypeName(xmlSchemaType);
							if (baseTypeName != null)
							{
								SchemaObjectInfo schemaObjectInfo2;
								if (dictionary.TryGetValue(baseTypeName, out schemaObjectInfo2))
								{
									if (schemaObjectInfo2.knownTypes == null)
									{
										schemaObjectInfo2.knownTypes = new List<XmlSchemaType>();
									}
								}
								else
								{
									schemaObjectInfo2 = new SchemaObjectInfo(null, null, null, new List<XmlSchemaType>());
									dictionary.Add(baseTypeName, schemaObjectInfo2);
								}
								schemaObjectInfo2.knownTypes.Add(xmlSchemaType);
							}
						}
					}
					foreach (object obj3 in xmlSchema.Elements.Values)
					{
						XmlSchemaElement xmlSchemaElement = ((XmlSchemaObject)obj3) as XmlSchemaElement;
						if (xmlSchemaElement != null)
						{
							XmlQualifiedName xmlQualifiedName2 = new XmlQualifiedName(xmlSchemaElement.Name, xmlSchema.TargetNamespace);
							SchemaObjectInfo schemaObjectInfo3;
							if (dictionary.TryGetValue(xmlQualifiedName2, out schemaObjectInfo3))
							{
								schemaObjectInfo3.element = xmlSchemaElement;
								schemaObjectInfo3.schema = xmlSchema;
							}
							else
							{
								dictionary.Add(xmlQualifiedName2, new SchemaObjectInfo(null, xmlSchemaElement, xmlSchema, null));
							}
						}
					}
				}
			}
			return dictionary;
		}

		private XmlQualifiedName GetBaseTypeName(XmlSchemaType type)
		{
			XmlQualifiedName xmlQualifiedName = null;
			XmlSchemaComplexType xmlSchemaComplexType = type as XmlSchemaComplexType;
			if (xmlSchemaComplexType != null && xmlSchemaComplexType.ContentModel != null)
			{
				XmlSchemaComplexContent xmlSchemaComplexContent = xmlSchemaComplexType.ContentModel as XmlSchemaComplexContent;
				if (xmlSchemaComplexContent != null)
				{
					XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = xmlSchemaComplexContent.Content as XmlSchemaComplexContentExtension;
					if (xmlSchemaComplexContentExtension != null)
					{
						xmlQualifiedName = xmlSchemaComplexContentExtension.BaseTypeName;
					}
				}
			}
			return xmlQualifiedName;
		}

		private List<XmlSchemaRedefine> CreateRedefineList()
		{
			List<XmlSchemaRedefine> list = new List<XmlSchemaRedefine>();
			foreach (object obj in this.schemaSet.Schemas())
			{
				XmlSchema xmlSchema = obj as XmlSchema;
				if (xmlSchema != null)
				{
					foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Includes)
					{
						XmlSchemaRedefine xmlSchemaRedefine = ((XmlSchemaExternal)xmlSchemaObject) as XmlSchemaRedefine;
						if (xmlSchemaRedefine != null)
						{
							list.Add(xmlSchemaRedefine);
						}
					}
				}
			}
			return list;
		}

		[SecuritySafeCritical]
		private DataContract ImportAnonymousGlobalElement(XmlSchemaElement element, XmlQualifiedName typeQName, string ns)
		{
			DataContract dataContract = this.ImportAnonymousElement(element, typeQName);
			XmlDataContract xmlDataContract = dataContract as XmlDataContract;
			if (xmlDataContract != null)
			{
				xmlDataContract.SetTopLevelElementName(new XmlQualifiedName(element.Name, ns));
				xmlDataContract.IsTopLevelElementNullable = element.IsNillable;
			}
			return dataContract;
		}

		private DataContract ImportAnonymousElement(XmlSchemaElement element, XmlQualifiedName typeQName)
		{
			if (SchemaHelper.GetSchemaType(this.SchemaObjects, typeQName) != null)
			{
				int num = 1;
				for (;;)
				{
					typeQName = new XmlQualifiedName(typeQName.Name + num.ToString(NumberFormatInfo.InvariantInfo), typeQName.Namespace);
					if (SchemaHelper.GetSchemaType(this.SchemaObjects, typeQName) == null)
					{
						goto IL_0074;
					}
					if (num == 2147483647)
					{
						break;
					}
					num++;
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Cannot compute unique name for '{0}'.", new object[] { element.Name })));
			}
			IL_0074:
			if (element.SchemaType == null)
			{
				return this.ImportType(SchemaExporter.AnytypeQualifiedName);
			}
			return this.ImportType(element.SchemaType, typeQName, true);
		}

		private DataContract ImportType(XmlQualifiedName typeName)
		{
			DataContract dataContract = DataContract.GetBuiltInDataContract(typeName.Name, typeName.Namespace);
			if (dataContract == null)
			{
				XmlSchemaType schemaType = SchemaHelper.GetSchemaType(this.SchemaObjects, typeName);
				if (schemaType == null)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Specified type '{0}' in '{1}' namespace is not found in the schemas.", new object[] { typeName.Name, typeName.Namespace })));
				}
				dataContract = this.ImportType(schemaType);
			}
			if (SchemaImporter.IsObjectContract(dataContract))
			{
				this.needToImportKnownTypesForObject = true;
			}
			return dataContract;
		}

		private DataContract ImportType(XmlSchemaType type)
		{
			return this.ImportType(type, type.QualifiedName, false);
		}

		private DataContract ImportType(XmlSchemaType type, XmlQualifiedName typeName, bool isAnonymous)
		{
			DataContract dataContract = this.dataContractSet[typeName];
			if (dataContract != null)
			{
				return dataContract;
			}
			try
			{
				using (List<XmlSchemaRedefine>.Enumerator enumerator = this.RedefineList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.SchemaTypes[typeName] != null)
						{
							SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("XML Schema 'redefine' is not supported."));
						}
					}
				}
				if (type is XmlSchemaSimpleType)
				{
					XmlSchemaSimpleType xmlSchemaSimpleType = (XmlSchemaSimpleType)type;
					XmlSchemaSimpleTypeContent content = xmlSchemaSimpleType.Content;
					if (content is XmlSchemaSimpleTypeUnion)
					{
						SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("simpleType union is not supported."));
					}
					else if (content is XmlSchemaSimpleTypeList)
					{
						dataContract = this.ImportFlagsEnum(typeName, (XmlSchemaSimpleTypeList)content, xmlSchemaSimpleType.Annotation);
					}
					else if (content is XmlSchemaSimpleTypeRestriction)
					{
						XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)content;
						if (this.CheckIfEnum(xmlSchemaSimpleTypeRestriction))
						{
							dataContract = this.ImportEnum(typeName, xmlSchemaSimpleTypeRestriction, false, xmlSchemaSimpleType.Annotation);
						}
						else
						{
							dataContract = this.ImportSimpleTypeRestriction(typeName, xmlSchemaSimpleTypeRestriction);
							if (dataContract.IsBuiltInDataContract && !isAnonymous)
							{
								this.dataContractSet.InternalAdd(typeName, dataContract);
							}
						}
					}
				}
				else if (type is XmlSchemaComplexType)
				{
					XmlSchemaComplexType xmlSchemaComplexType = (XmlSchemaComplexType)type;
					if (xmlSchemaComplexType.ContentModel == null)
					{
						this.CheckComplexType(typeName, xmlSchemaComplexType);
						dataContract = this.ImportType(typeName, xmlSchemaComplexType.Particle, xmlSchemaComplexType.Attributes, xmlSchemaComplexType.AnyAttribute, null, xmlSchemaComplexType.Annotation);
					}
					else
					{
						XmlSchemaContentModel contentModel = xmlSchemaComplexType.ContentModel;
						if (contentModel is XmlSchemaSimpleContent)
						{
							SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Simple content is not supported."));
						}
						else if (contentModel is XmlSchemaComplexContent)
						{
							XmlSchemaComplexContent xmlSchemaComplexContent = (XmlSchemaComplexContent)contentModel;
							if (xmlSchemaComplexContent.IsMixed)
							{
								SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Mixed content is not supported."));
							}
							if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentExtension)
							{
								XmlSchemaComplexContentExtension xmlSchemaComplexContentExtension = (XmlSchemaComplexContentExtension)xmlSchemaComplexContent.Content;
								dataContract = this.ImportType(typeName, xmlSchemaComplexContentExtension.Particle, xmlSchemaComplexContentExtension.Attributes, xmlSchemaComplexContentExtension.AnyAttribute, xmlSchemaComplexContentExtension.BaseTypeName, xmlSchemaComplexType.Annotation);
							}
							else if (xmlSchemaComplexContent.Content is XmlSchemaComplexContentRestriction)
							{
								XmlSchemaComplexContentRestriction xmlSchemaComplexContentRestriction = (XmlSchemaComplexContentRestriction)xmlSchemaComplexContent.Content;
								if (xmlSchemaComplexContentRestriction.BaseTypeName == SchemaExporter.AnytypeQualifiedName)
								{
									dataContract = this.ImportType(typeName, xmlSchemaComplexContentRestriction.Particle, xmlSchemaComplexContentRestriction.Attributes, xmlSchemaComplexContentRestriction.AnyAttribute, null, xmlSchemaComplexType.Annotation);
								}
								else
								{
									SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("XML schema complexType restriction is not supported."));
								}
							}
						}
					}
				}
				if (dataContract == null)
				{
					SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, string.Empty);
				}
				if (type.QualifiedName != XmlQualifiedName.Empty)
				{
					this.ImportTopLevelElement(typeName);
				}
				this.ImportDataContractExtension(type, dataContract);
				this.ImportGenericInfo(type, dataContract);
				this.ImportKnownTypes(typeName);
				return dataContract;
			}
			catch (InvalidDataContractException ex)
			{
			}
			if (this.importXmlDataType)
			{
				this.RemoveFailedContract(typeName);
				return this.ImportXmlDataType(typeName, type, isAnonymous);
			}
			Type type2;
			if ((this.dataContractSet.TryGetReferencedType(typeName, dataContract, out type2) || (string.IsNullOrEmpty(type.Name) && this.dataContractSet.TryGetReferencedType(SchemaImporter.ImportActualType(type.Annotation, typeName, typeName), dataContract, out type2))) && Globals.TypeOfIXmlSerializable.IsAssignableFrom(type2))
			{
				this.RemoveFailedContract(typeName);
				return this.ImportXmlDataType(typeName, type, isAnonymous);
			}
			XmlDataContract xmlDataContract = this.ImportSpecialXmlDataType(type, isAnonymous);
			if (xmlDataContract != null)
			{
				this.dataContractSet.Remove(typeName);
				return xmlDataContract;
			}
			InvalidDataContractException ex;
			throw ex;
		}

		private void RemoveFailedContract(XmlQualifiedName typeName)
		{
			ClassDataContract classDataContract = this.dataContractSet[typeName] as ClassDataContract;
			this.dataContractSet.Remove(typeName);
			if (classDataContract != null)
			{
				for (ClassDataContract classDataContract2 = classDataContract.BaseContract; classDataContract2 != null; classDataContract2 = classDataContract2.BaseContract)
				{
					classDataContract2.KnownDataContracts.Remove(typeName);
				}
				if (this.dataContractSet.KnownTypesForObject != null)
				{
					this.dataContractSet.KnownTypesForObject.Remove(typeName);
				}
			}
		}

		private bool CheckIfEnum(XmlSchemaSimpleTypeRestriction restriction)
		{
			using (XmlSchemaObjectEnumerator enumerator = restriction.Facets.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (!(((XmlSchemaFacet)enumerator.Current) is XmlSchemaEnumerationFacet))
					{
						return false;
					}
				}
			}
			XmlQualifiedName stringQualifiedName = SchemaExporter.StringQualifiedName;
			if (restriction.BaseTypeName != XmlQualifiedName.Empty)
			{
				return (restriction.BaseTypeName == stringQualifiedName && restriction.Facets.Count > 0) || this.ImportType(restriction.BaseTypeName) is EnumDataContract;
			}
			if (restriction.BaseType != null)
			{
				DataContract dataContract = this.ImportType(restriction.BaseType);
				return dataContract.StableName == stringQualifiedName || dataContract is EnumDataContract;
			}
			return false;
		}

		private bool CheckIfCollection(XmlSchemaSequence rootSequence)
		{
			if (rootSequence.Items == null || rootSequence.Items.Count == 0)
			{
				return false;
			}
			this.RemoveOptionalUnknownSerializationElements(rootSequence.Items);
			if (rootSequence.Items.Count != 1)
			{
				return false;
			}
			XmlSchemaObject xmlSchemaObject = rootSequence.Items[0];
			if (!(xmlSchemaObject is XmlSchemaElement))
			{
				return false;
			}
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)xmlSchemaObject;
			return xmlSchemaElement.MaxOccursString == "unbounded" || xmlSchemaElement.MaxOccurs > 1m;
		}

		private bool CheckIfISerializable(XmlSchemaSequence rootSequence, XmlSchemaObjectCollection attributes)
		{
			if (rootSequence.Items == null || rootSequence.Items.Count == 0)
			{
				return false;
			}
			this.RemoveOptionalUnknownSerializationElements(rootSequence.Items);
			return attributes != null && attributes.Count != 0 && rootSequence.Items.Count == 1 && rootSequence.Items[0] is XmlSchemaAny;
		}

		[SecuritySafeCritical]
		private void RemoveOptionalUnknownSerializationElements(XmlSchemaObjectCollection items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				XmlSchemaElement xmlSchemaElement = items[i] as XmlSchemaElement;
				if (xmlSchemaElement != null && xmlSchemaElement.RefName != null && xmlSchemaElement.RefName.Namespace == "http://schemas.microsoft.com/2003/10/Serialization/" && xmlSchemaElement.MinOccurs == 0m)
				{
					if (SchemaImporter.serializationSchemaElements == null)
					{
						XmlSchema xmlSchema = XmlSchema.Read(XmlReader.Create(new StringReader("<?xml version='1.0' encoding='utf-8'?>\n<xs:schema elementFormDefault='qualified' attributeFormDefault='qualified' xmlns:tns='http://schemas.microsoft.com/2003/10/Serialization/' targetNamespace='http://schemas.microsoft.com/2003/10/Serialization/' xmlns:xs='http://www.w3.org/2001/XMLSchema'>\n  <xs:element name='anyType' nillable='true' type='xs:anyType' />\n  <xs:element name='anyURI' nillable='true' type='xs:anyURI' />\n  <xs:element name='base64Binary' nillable='true' type='xs:base64Binary' />\n  <xs:element name='boolean' nillable='true' type='xs:boolean' />\n  <xs:element name='byte' nillable='true' type='xs:byte' />\n  <xs:element name='dateTime' nillable='true' type='xs:dateTime' />\n  <xs:element name='decimal' nillable='true' type='xs:decimal' />\n  <xs:element name='double' nillable='true' type='xs:double' />\n  <xs:element name='float' nillable='true' type='xs:float' />\n  <xs:element name='int' nillable='true' type='xs:int' />\n  <xs:element name='long' nillable='true' type='xs:long' />\n  <xs:element name='QName' nillable='true' type='xs:QName' />\n  <xs:element name='short' nillable='true' type='xs:short' />\n  <xs:element name='string' nillable='true' type='xs:string' />\n  <xs:element name='unsignedByte' nillable='true' type='xs:unsignedByte' />\n  <xs:element name='unsignedInt' nillable='true' type='xs:unsignedInt' />\n  <xs:element name='unsignedLong' nillable='true' type='xs:unsignedLong' />\n  <xs:element name='unsignedShort' nillable='true' type='xs:unsignedShort' />\n  <xs:element name='char' nillable='true' type='tns:char' />\n  <xs:simpleType name='char'>\n    <xs:restriction base='xs:int'/>\n  </xs:simpleType>  \n  <xs:element name='duration' nillable='true' type='tns:duration' />\n  <xs:simpleType name='duration'>\n    <xs:restriction base='xs:duration'>\n      <xs:pattern value='\\-?P(\\d*D)?(T(\\d*H)?(\\d*M)?(\\d*(\\.\\d*)?S)?)?' />\n      <xs:minInclusive value='-P10675199DT2H48M5.4775808S' />\n      <xs:maxInclusive value='P10675199DT2H48M5.4775807S' />\n    </xs:restriction>\n  </xs:simpleType>\n  <xs:element name='guid' nillable='true' type='tns:guid' />\n  <xs:simpleType name='guid'>\n    <xs:restriction base='xs:string'>\n      <xs:pattern value='[\\da-fA-F]{8}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{4}-[\\da-fA-F]{12}' />\n    </xs:restriction>\n  </xs:simpleType>\n  <xs:attribute name='FactoryType' type='xs:QName' />\n  <xs:attribute name='Id' type='xs:ID' />\n  <xs:attribute name='Ref' type='xs:IDREF' />\n</xs:schema>\n")), null);
						SchemaImporter.serializationSchemaElements = new Hashtable();
						foreach (XmlSchemaObject xmlSchemaObject in xmlSchema.Items)
						{
							XmlSchemaElement xmlSchemaElement2 = xmlSchemaObject as XmlSchemaElement;
							if (xmlSchemaElement2 != null)
							{
								SchemaImporter.serializationSchemaElements.Add(xmlSchemaElement2.Name, xmlSchemaElement2);
							}
						}
					}
					if (!SchemaImporter.serializationSchemaElements.ContainsKey(xmlSchemaElement.RefName.Name))
					{
						items.RemoveAt(i);
						i--;
					}
				}
			}
		}

		private DataContract ImportType(XmlQualifiedName typeName, XmlSchemaParticle rootParticle, XmlSchemaObjectCollection attributes, XmlSchemaAnyAttribute anyAttribute, XmlQualifiedName baseTypeName, XmlSchemaAnnotation annotation)
		{
			DataContract dataContract = null;
			bool flag = baseTypeName != null;
			bool flag2;
			this.ImportAttributes(typeName, attributes, anyAttribute, out flag2);
			if (rootParticle == null)
			{
				dataContract = this.ImportClass(typeName, new XmlSchemaSequence(), baseTypeName, annotation, flag2);
			}
			else if (!(rootParticle is XmlSchemaSequence))
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Root particle must be sequence to be imported."));
			}
			else
			{
				XmlSchemaSequence xmlSchemaSequence = (XmlSchemaSequence)rootParticle;
				if (xmlSchemaSequence.MinOccurs != 1m)
				{
					SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Root sequence must have an item and minOccurs must be 1."));
				}
				if (xmlSchemaSequence.MaxOccurs != 1m)
				{
					SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("On root sequence, maxOccurs must be 1."));
				}
				if (!flag && this.CheckIfCollection(xmlSchemaSequence))
				{
					dataContract = this.ImportCollection(typeName, xmlSchemaSequence, attributes, annotation, flag2);
				}
				else if (this.CheckIfISerializable(xmlSchemaSequence, attributes))
				{
					dataContract = this.ImportISerializable(typeName, xmlSchemaSequence, baseTypeName, attributes, annotation);
				}
				else
				{
					dataContract = this.ImportClass(typeName, xmlSchemaSequence, baseTypeName, annotation, flag2);
				}
			}
			return dataContract;
		}

		[SecuritySafeCritical]
		private ClassDataContract ImportClass(XmlQualifiedName typeName, XmlSchemaSequence rootSequence, XmlQualifiedName baseTypeName, XmlSchemaAnnotation annotation, bool isReference)
		{
			ClassDataContract classDataContract = new ClassDataContract();
			classDataContract.StableName = typeName;
			this.AddDataContract(classDataContract);
			classDataContract.IsValueType = this.IsValueType(typeName, annotation);
			classDataContract.IsReference = isReference;
			if (baseTypeName != null)
			{
				this.ImportBaseContract(baseTypeName, classDataContract);
				if (classDataContract.BaseContract.IsISerializable)
				{
					if (this.IsISerializableDerived(typeName, rootSequence))
					{
						classDataContract.IsISerializable = true;
					}
					else
					{
						SchemaImporter.ThrowTypeCannotBeImportedException(classDataContract.StableName.Name, classDataContract.StableName.Namespace, global::System.Runtime.Serialization.SR.GetString("On type '{0}' in '{1}' namespace, derived type is not ISerializable.", new object[] { baseTypeName.Name, baseTypeName.Namespace }));
					}
				}
				if (classDataContract.BaseContract.IsReference)
				{
					classDataContract.IsReference = true;
				}
			}
			if (!classDataContract.IsISerializable)
			{
				classDataContract.Members = new List<DataMember>();
				this.RemoveOptionalUnknownSerializationElements(rootSequence.Items);
				for (int i = 0; i < rootSequence.Items.Count; i++)
				{
					XmlSchemaElement xmlSchemaElement = rootSequence.Items[i] as XmlSchemaElement;
					if (xmlSchemaElement == null)
					{
						SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Only local elements can be imported."));
					}
					this.ImportClassMember(xmlSchemaElement, classDataContract);
				}
			}
			return classDataContract;
		}

		[SecuritySafeCritical]
		private DataContract ImportXmlDataType(XmlQualifiedName typeName, XmlSchemaType xsdType, bool isAnonymous)
		{
			DataContract dataContract = this.dataContractSet[typeName];
			if (dataContract != null)
			{
				return dataContract;
			}
			XmlDataContract xmlDataContract = this.ImportSpecialXmlDataType(xsdType, isAnonymous);
			if (xmlDataContract != null)
			{
				return xmlDataContract;
			}
			xmlDataContract = new XmlDataContract();
			xmlDataContract.StableName = typeName;
			xmlDataContract.IsValueType = false;
			this.AddDataContract(xmlDataContract);
			if (xsdType != null)
			{
				this.ImportDataContractExtension(xsdType, xmlDataContract);
				xmlDataContract.IsValueType = this.IsValueType(typeName, xsdType.Annotation);
				xmlDataContract.IsTypeDefinedOnImport = true;
				xmlDataContract.XsdType = (isAnonymous ? xsdType : null);
				xmlDataContract.HasRoot = !this.IsXmlAnyElementType(xsdType as XmlSchemaComplexType);
			}
			else
			{
				xmlDataContract.IsValueType = true;
				xmlDataContract.IsTypeDefinedOnImport = false;
				xmlDataContract.HasRoot = true;
				if (DiagnosticUtility.ShouldTraceVerbose)
				{
					TraceUtility.Trace(TraceEventType.Verbose, 196623, global::System.Runtime.Serialization.SR.GetString("XSD import annotation failed"), new StringTraceRecord("Type", typeName.Namespace + ":" + typeName.Name));
				}
			}
			if (!isAnonymous)
			{
				bool flag;
				xmlDataContract.SetTopLevelElementName(SchemaHelper.GetGlobalElementDeclaration(this.schemaSet, typeName, out flag));
				xmlDataContract.IsTopLevelElementNullable = flag;
			}
			return xmlDataContract;
		}

		private XmlDataContract ImportSpecialXmlDataType(XmlSchemaType xsdType, bool isAnonymous)
		{
			if (!isAnonymous)
			{
				return null;
			}
			XmlSchemaComplexType xmlSchemaComplexType = xsdType as XmlSchemaComplexType;
			if (xmlSchemaComplexType == null)
			{
				return null;
			}
			if (this.IsXmlAnyElementType(xmlSchemaComplexType))
			{
				XmlQualifiedName xmlQualifiedName = new XmlQualifiedName("XElement", "http://schemas.datacontract.org/2004/07/System.Xml.Linq");
				Type type;
				if (this.dataContractSet.TryGetReferencedType(xmlQualifiedName, null, out type) && Globals.TypeOfIXmlSerializable.IsAssignableFrom(type))
				{
					XmlDataContract xmlDataContract = new XmlDataContract(type);
					this.AddDataContract(xmlDataContract);
					return xmlDataContract;
				}
				return (XmlDataContract)DataContract.GetBuiltInDataContract(Globals.TypeOfXmlElement);
			}
			else
			{
				if (this.IsXmlAnyType(xmlSchemaComplexType))
				{
					return (XmlDataContract)DataContract.GetBuiltInDataContract(Globals.TypeOfXmlNodeArray);
				}
				return null;
			}
		}

		private bool IsXmlAnyElementType(XmlSchemaComplexType xsdType)
		{
			if (xsdType == null)
			{
				return false;
			}
			XmlSchemaSequence xmlSchemaSequence = xsdType.Particle as XmlSchemaSequence;
			if (xmlSchemaSequence == null)
			{
				return false;
			}
			if (xmlSchemaSequence.Items == null || xmlSchemaSequence.Items.Count != 1)
			{
				return false;
			}
			XmlSchemaAny xmlSchemaAny = xmlSchemaSequence.Items[0] as XmlSchemaAny;
			return xmlSchemaAny != null && xmlSchemaAny.Namespace == null && xsdType.AnyAttribute == null && (xsdType.Attributes == null || xsdType.Attributes.Count <= 0);
		}

		private bool IsXmlAnyType(XmlSchemaComplexType xsdType)
		{
			if (xsdType == null)
			{
				return false;
			}
			XmlSchemaSequence xmlSchemaSequence = xsdType.Particle as XmlSchemaSequence;
			if (xmlSchemaSequence == null)
			{
				return false;
			}
			if (xmlSchemaSequence.Items == null || xmlSchemaSequence.Items.Count != 1)
			{
				return false;
			}
			XmlSchemaAny xmlSchemaAny = xmlSchemaSequence.Items[0] as XmlSchemaAny;
			return xmlSchemaAny != null && xmlSchemaAny.Namespace == null && !(xmlSchemaAny.MaxOccurs != decimal.MaxValue) && xsdType.AnyAttribute != null && xsdType.Attributes.Count <= 0;
		}

		private bool IsValueType(XmlQualifiedName typeName, XmlSchemaAnnotation annotation)
		{
			string innerText = this.GetInnerText(typeName, SchemaImporter.ImportAnnotation(annotation, SchemaExporter.IsValueTypeName));
			if (innerText != null)
			{
				try
				{
					return XmlConvert.ToBoolean(innerText);
				}
				catch (FormatException ex)
				{
					SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("IsValueType is formatted incorrectly as '{0}': {1}", new object[] { innerText, ex.Message }));
				}
				return false;
			}
			return false;
		}

		[SecuritySafeCritical]
		private ClassDataContract ImportISerializable(XmlQualifiedName typeName, XmlSchemaSequence rootSequence, XmlQualifiedName baseTypeName, XmlSchemaObjectCollection attributes, XmlSchemaAnnotation annotation)
		{
			ClassDataContract classDataContract = new ClassDataContract();
			classDataContract.StableName = typeName;
			classDataContract.IsISerializable = true;
			this.AddDataContract(classDataContract);
			classDataContract.IsValueType = this.IsValueType(typeName, annotation);
			if (baseTypeName == null)
			{
				this.CheckISerializableBase(typeName, rootSequence, attributes);
			}
			else
			{
				this.ImportBaseContract(baseTypeName, classDataContract);
				if (!classDataContract.BaseContract.IsISerializable)
				{
					SchemaImporter.ThrowISerializableTypeCannotBeImportedException(classDataContract.StableName.Name, classDataContract.StableName.Namespace, global::System.Runtime.Serialization.SR.GetString("Base type '{0}' in '{1}' namespace is not ISerializable.", new object[] { baseTypeName.Name, baseTypeName.Namespace }));
				}
				if (!this.IsISerializableDerived(typeName, rootSequence))
				{
					SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Type derived from ISerializable cannot contain more than one item."));
				}
			}
			return classDataContract;
		}

		private void CheckISerializableBase(XmlQualifiedName typeName, XmlSchemaSequence rootSequence, XmlSchemaObjectCollection attributes)
		{
			if (rootSequence == null)
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable does not contain any element."));
			}
			if (rootSequence.Items == null || rootSequence.Items.Count < 1)
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable does not contain any element."));
			}
			else if (rootSequence.Items.Count > 1)
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable cannot contain more than one item."));
			}
			XmlSchemaObject xmlSchemaObject = rootSequence.Items[0];
			if (!(xmlSchemaObject is XmlSchemaAny))
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable does not contain any element."));
			}
			XmlSchemaAny xmlSchemaAny = (XmlSchemaAny)xmlSchemaObject;
			XmlSchemaAny iserializableWildcardElement = SchemaExporter.ISerializableWildcardElement;
			if (xmlSchemaAny.MinOccurs != iserializableWildcardElement.MinOccurs)
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable wildcard maxOccurs must be '{0}'.", new object[] { iserializableWildcardElement.MinOccurs }));
			}
			if (xmlSchemaAny.MaxOccursString != iserializableWildcardElement.MaxOccursString)
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable wildcard maxOccurs must be '{0}'.", new object[] { iserializableWildcardElement.MaxOccursString }));
			}
			if (xmlSchemaAny.Namespace != iserializableWildcardElement.Namespace)
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable wildcard namespace is invalid: '{0}'.", new object[] { iserializableWildcardElement.Namespace }));
			}
			if (xmlSchemaAny.ProcessContents != iserializableWildcardElement.ProcessContents)
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable wildcard processContents is invalid: '{0}'.", new object[] { iserializableWildcardElement.ProcessContents }));
			}
			XmlQualifiedName refName = SchemaExporter.ISerializableFactoryTypeAttribute.RefName;
			bool flag = false;
			if (attributes != null)
			{
				for (int i = 0; i < attributes.Count; i++)
				{
					xmlSchemaObject = attributes[i];
					if (xmlSchemaObject is XmlSchemaAttribute && ((XmlSchemaAttribute)xmlSchemaObject).RefName == refName)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				SchemaImporter.ThrowISerializableTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("ISerializable must have ref attribute that points to its factory type.", new object[] { refName.Name, refName.Namespace }));
			}
		}

		private bool IsISerializableDerived(XmlQualifiedName typeName, XmlSchemaSequence rootSequence)
		{
			return rootSequence == null || rootSequence.Items == null || rootSequence.Items.Count == 0;
		}

		[SecuritySafeCritical]
		private void ImportBaseContract(XmlQualifiedName baseTypeName, ClassDataContract dataContract)
		{
			ClassDataContract classDataContract = this.ImportType(baseTypeName) as ClassDataContract;
			if (classDataContract == null)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(dataContract.StableName.Name, dataContract.StableName.Namespace, global::System.Runtime.Serialization.SR.GetString(dataContract.IsISerializable ? "Invalid ISerializable derivation from '{0}' in '{1}' namespace." : "Invalid class derivation from '{0}' in '{1}' namespace.", new object[] { baseTypeName.Name, baseTypeName.Namespace }));
			}
			if (classDataContract.IsValueType)
			{
				classDataContract.IsValueType = false;
			}
			for (ClassDataContract classDataContract2 = classDataContract; classDataContract2 != null; classDataContract2 = classDataContract2.BaseContract)
			{
				Dictionary<XmlQualifiedName, DataContract> dictionary = classDataContract2.KnownDataContracts;
				if (dictionary == null)
				{
					dictionary = new Dictionary<XmlQualifiedName, DataContract>();
					classDataContract2.KnownDataContracts = dictionary;
				}
				dictionary.Add(dataContract.StableName, dataContract);
			}
			dataContract.BaseContract = classDataContract;
		}

		private void ImportTopLevelElement(XmlQualifiedName typeName)
		{
			XmlSchemaElement schemaElement = SchemaHelper.GetSchemaElement(this.SchemaObjects, typeName);
			if (schemaElement == null)
			{
				return;
			}
			XmlQualifiedName xmlQualifiedName = schemaElement.SchemaTypeName;
			if (xmlQualifiedName.IsEmpty)
			{
				if (schemaElement.SchemaType != null)
				{
					SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Anonymous type is not supported. Type is '{0}' in '{1}' namespace.", new object[] { typeName.Name, typeName.Namespace }));
				}
				else
				{
					xmlQualifiedName = SchemaExporter.AnytypeQualifiedName;
				}
			}
			if (xmlQualifiedName != typeName)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Top-level element represents a different type. Expected '{0}' type in '{1}' namespace.", new object[]
				{
					schemaElement.SchemaTypeName.Name,
					schemaElement.SchemaTypeName.Namespace
				}));
			}
			this.CheckIfElementUsesUnsupportedConstructs(typeName, schemaElement);
		}

		private void ImportClassMember(XmlSchemaElement element, ClassDataContract dataContract)
		{
			XmlQualifiedName stableName = dataContract.StableName;
			if (element.MinOccurs > 1m)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(stableName.Name, stableName.Namespace, global::System.Runtime.Serialization.SR.GetString("On element '{0}', schema element minOccurs must be less or equal to 1.", new object[] { element.Name }));
			}
			if (element.MaxOccurs != 1m)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(stableName.Name, stableName.Namespace, global::System.Runtime.Serialization.SR.GetString("On element '{0}', schema element maxOccurs must be 1.", new object[] { element.Name }));
			}
			DataContract dataContract2 = null;
			string name = element.Name;
			bool flag = element.MinOccurs > 0m;
			bool isNillable = element.IsNillable;
			int num = 0;
			if (((element.Form == XmlSchemaForm.None) ? SchemaHelper.GetSchemaWithType(this.SchemaObjects, this.schemaSet, stableName).ElementFormDefault : element.Form) != XmlSchemaForm.Qualified)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(stableName.Name, stableName.Namespace, global::System.Runtime.Serialization.SR.GetString("On schema element '{0}', form must be qualified.", new object[] { element.Name }));
			}
			this.CheckIfElementUsesUnsupportedConstructs(stableName, element);
			if (element.SchemaTypeName.IsEmpty)
			{
				if (element.SchemaType != null)
				{
					dataContract2 = this.ImportAnonymousElement(element, new XmlQualifiedName(string.Format(CultureInfo.InvariantCulture, "{0}.{1}Type", stableName.Name, element.Name), stableName.Namespace));
				}
				else if (!element.RefName.IsEmpty)
				{
					SchemaImporter.ThrowTypeCannotBeImportedException(stableName.Name, stableName.Namespace, global::System.Runtime.Serialization.SR.GetString("For local element, ref is not supported. The referenced name is '{0}' in '{1}' namespace.", new object[]
					{
						element.RefName.Name,
						element.RefName.Namespace
					}));
				}
				else
				{
					dataContract2 = this.ImportType(SchemaExporter.AnytypeQualifiedName);
				}
			}
			else
			{
				XmlQualifiedName xmlQualifiedName = SchemaImporter.ImportActualType(element.Annotation, element.SchemaTypeName, stableName);
				dataContract2 = this.ImportType(xmlQualifiedName);
				if (SchemaImporter.IsObjectContract(dataContract2))
				{
					this.needToImportKnownTypesForObject = true;
				}
			}
			bool? flag2 = this.ImportEmitDefaultValue(element.Annotation, stableName);
			bool flag3;
			if (!dataContract2.IsValueType && !isNillable)
			{
				if (flag2 != null && flag2.Value)
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Invalid EmilDefault annotation for '{0}' in type '{1}' in '{2}' namespace.", new object[] { name, stableName.Name, stableName.Namespace })));
				}
				flag3 = false;
			}
			else
			{
				flag3 = flag2 == null || flag2.Value;
			}
			int num2 = dataContract.Members.Count - 1;
			if (num2 >= 0)
			{
				DataMember dataMember = dataContract.Members[num2];
				if (dataMember.Order > 0)
				{
					num = dataContract.Members.Count;
				}
				DataMember dataMember2 = new DataMember(dataContract2, name, isNillable, flag, flag3, num);
				int num3 = ClassDataContract.DataMemberComparer.Singleton.Compare(dataMember, dataMember2);
				if (num3 == 0)
				{
					SchemaImporter.ThrowTypeCannotBeImportedException(stableName.Name, stableName.Namespace, global::System.Runtime.Serialization.SR.GetString("Cannot have duplicate element names '{0}'.", new object[] { name }));
				}
				else if (num3 > 0)
				{
					num = dataContract.Members.Count;
				}
			}
			DataMember dataMember3 = new DataMember(dataContract2, name, isNillable, flag, flag3, num);
			XmlQualifiedName surrogateDataAnnotationName = SchemaExporter.SurrogateDataAnnotationName;
			this.dataContractSet.SetSurrogateData(dataMember3, this.ImportSurrogateData(SchemaImporter.ImportAnnotation(element.Annotation, surrogateDataAnnotationName), surrogateDataAnnotationName.Name, surrogateDataAnnotationName.Namespace));
			dataContract.Members.Add(dataMember3);
		}

		private bool? ImportEmitDefaultValue(XmlSchemaAnnotation annotation, XmlQualifiedName typeName)
		{
			XmlElement xmlElement = SchemaImporter.ImportAnnotation(annotation, SchemaExporter.DefaultValueAnnotation);
			if (xmlElement == null)
			{
				return null;
			}
			XmlNode namedItem = xmlElement.Attributes.GetNamedItem("EmitDefaultValue");
			string text = ((namedItem == null) ? null : namedItem.Value);
			if (text == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Annotation attribute was not found: default value annotation is '{0}', type is '{1}' in '{2}' namespace, emit default value is {3}.", new object[]
				{
					SchemaExporter.DefaultValueAnnotation.Name,
					typeName.Name,
					typeName.Namespace,
					"EmitDefaultValue"
				})));
			}
			return new bool?(XmlConvert.ToBoolean(text));
		}

		internal static XmlQualifiedName ImportActualType(XmlSchemaAnnotation annotation, XmlQualifiedName defaultTypeName, XmlQualifiedName typeName)
		{
			XmlElement xmlElement = SchemaImporter.ImportAnnotation(annotation, SchemaExporter.ActualTypeAnnotationName);
			if (xmlElement == null)
			{
				return defaultTypeName;
			}
			XmlNode namedItem = xmlElement.Attributes.GetNamedItem("Name");
			string text = ((namedItem == null) ? null : namedItem.Value);
			if (text == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Annotation attribute was not found: default value annotation is '{0}', type is '{1}' in '{2}' namespace, emit default value is {3}.", new object[]
				{
					SchemaExporter.ActualTypeAnnotationName.Name,
					typeName.Name,
					typeName.Namespace,
					"Name"
				})));
			}
			XmlNode namedItem2 = xmlElement.Attributes.GetNamedItem("Namespace");
			string text2 = ((namedItem2 == null) ? null : namedItem2.Value);
			if (text2 == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Annotation attribute was not found: default value annotation is '{0}', type is '{1}' in '{2}' namespace, emit default value is {3}.", new object[]
				{
					SchemaExporter.ActualTypeAnnotationName.Name,
					typeName.Name,
					typeName.Namespace,
					"Namespace"
				})));
			}
			return new XmlQualifiedName(text, text2);
		}

		[SecuritySafeCritical]
		private CollectionDataContract ImportCollection(XmlQualifiedName typeName, XmlSchemaSequence rootSequence, XmlSchemaObjectCollection attributes, XmlSchemaAnnotation annotation, bool isReference)
		{
			CollectionDataContract collectionDataContract = new CollectionDataContract(CollectionKind.Array);
			collectionDataContract.StableName = typeName;
			this.AddDataContract(collectionDataContract);
			collectionDataContract.IsReference = isReference;
			XmlSchemaElement xmlSchemaElement = (XmlSchemaElement)rootSequence.Items[0];
			collectionDataContract.IsItemTypeNullable = xmlSchemaElement.IsNillable;
			collectionDataContract.ItemName = xmlSchemaElement.Name;
			if (((xmlSchemaElement.Form == XmlSchemaForm.None) ? SchemaHelper.GetSchemaWithType(this.SchemaObjects, this.schemaSet, typeName).ElementFormDefault : xmlSchemaElement.Form) != XmlSchemaForm.Qualified)
			{
				SchemaImporter.ThrowArrayTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("For array item, element 'form' must be {0}.", new object[] { xmlSchemaElement.Name }));
			}
			this.CheckIfElementUsesUnsupportedConstructs(typeName, xmlSchemaElement);
			if (xmlSchemaElement.SchemaTypeName.IsEmpty)
			{
				if (xmlSchemaElement.SchemaType != null)
				{
					XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(xmlSchemaElement.Name, typeName.Namespace);
					if (this.dataContractSet[xmlQualifiedName] == null)
					{
						collectionDataContract.ItemContract = this.ImportAnonymousElement(xmlSchemaElement, xmlQualifiedName);
					}
					else
					{
						XmlQualifiedName xmlQualifiedName2 = new XmlQualifiedName(string.Format(CultureInfo.InvariantCulture, "{0}.{1}Type", typeName.Name, xmlSchemaElement.Name), typeName.Namespace);
						collectionDataContract.ItemContract = this.ImportAnonymousElement(xmlSchemaElement, xmlQualifiedName2);
					}
				}
				else if (!xmlSchemaElement.RefName.IsEmpty)
				{
					SchemaImporter.ThrowArrayTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("For local element, ref is not supported. The referenced name is '{0}' in '{1}' namespace.", new object[]
					{
						xmlSchemaElement.RefName.Name,
						xmlSchemaElement.RefName.Namespace
					}));
				}
				else
				{
					collectionDataContract.ItemContract = this.ImportType(SchemaExporter.AnytypeQualifiedName);
				}
			}
			else
			{
				collectionDataContract.ItemContract = this.ImportType(xmlSchemaElement.SchemaTypeName);
			}
			if (this.IsDictionary(typeName, annotation))
			{
				ClassDataContract classDataContract = collectionDataContract.ItemContract as ClassDataContract;
				DataMember dataMember = null;
				DataMember dataMember2 = null;
				if (classDataContract == null || classDataContract.Members == null || classDataContract.Members.Count != 2 || !(dataMember = classDataContract.Members[0]).IsRequired || !(dataMember2 = classDataContract.Members[1]).IsRequired)
				{
					SchemaImporter.ThrowArrayTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("'{0}' is an invalid key value type.", new object[] { xmlSchemaElement.Name }));
				}
				if (classDataContract.Namespace != collectionDataContract.Namespace)
				{
					SchemaImporter.ThrowArrayTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("'{0}' in '{1}' namespace is an invalid key value type.", new object[] { xmlSchemaElement.Name, classDataContract.Namespace }));
				}
				classDataContract.IsValueType = true;
				collectionDataContract.KeyName = dataMember.Name;
				collectionDataContract.ValueName = dataMember2.Name;
				if (xmlSchemaElement.SchemaType != null)
				{
					this.dataContractSet.Remove(classDataContract.StableName);
					GenericInfo genericInfo = new GenericInfo(DataContract.GetStableName(Globals.TypeOfKeyValue), Globals.TypeOfKeyValue.FullName);
					genericInfo.Add(this.GetGenericInfoForDataMember(dataMember));
					genericInfo.Add(this.GetGenericInfoForDataMember(dataMember2));
					genericInfo.AddToLevel(0, 2);
					collectionDataContract.ItemContract.StableName = new XmlQualifiedName(genericInfo.GetExpandedStableName().Name, typeName.Namespace);
				}
			}
			return collectionDataContract;
		}

		private GenericInfo GetGenericInfoForDataMember(DataMember dataMember)
		{
			GenericInfo genericInfo;
			if (dataMember.MemberTypeContract.IsValueType && dataMember.IsNullable)
			{
				genericInfo = new GenericInfo(DataContract.GetStableName(Globals.TypeOfNullable), Globals.TypeOfNullable.FullName);
				genericInfo.Add(new GenericInfo(dataMember.MemberTypeContract.StableName, null));
			}
			else
			{
				genericInfo = new GenericInfo(dataMember.MemberTypeContract.StableName, null);
			}
			return genericInfo;
		}

		private bool IsDictionary(XmlQualifiedName typeName, XmlSchemaAnnotation annotation)
		{
			string innerText = this.GetInnerText(typeName, SchemaImporter.ImportAnnotation(annotation, SchemaExporter.IsDictionaryAnnotationName));
			if (innerText != null)
			{
				try
				{
					return XmlConvert.ToBoolean(innerText);
				}
				catch (FormatException ex)
				{
					SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("IsDictionary formatted value '{0}' is incorrect: {1}", new object[] { innerText, ex.Message }));
				}
				return false;
			}
			return false;
		}

		private EnumDataContract ImportFlagsEnum(XmlQualifiedName typeName, XmlSchemaSimpleTypeList list, XmlSchemaAnnotation annotation)
		{
			XmlSchemaSimpleType itemType = list.ItemType;
			if (itemType == null)
			{
				SchemaImporter.ThrowEnumTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Enum list must contain an anonymous type."));
			}
			XmlSchemaSimpleTypeContent content = itemType.Content;
			if (content is XmlSchemaSimpleTypeUnion)
			{
				SchemaImporter.ThrowEnumTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Enum union in anonymous type is not supported."));
			}
			else if (content is XmlSchemaSimpleTypeList)
			{
				SchemaImporter.ThrowEnumTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Enum list in anonymous type is not supported."));
			}
			else if (content is XmlSchemaSimpleTypeRestriction)
			{
				XmlSchemaSimpleTypeRestriction xmlSchemaSimpleTypeRestriction = (XmlSchemaSimpleTypeRestriction)content;
				if (this.CheckIfEnum(xmlSchemaSimpleTypeRestriction))
				{
					return this.ImportEnum(typeName, xmlSchemaSimpleTypeRestriction, true, annotation);
				}
				SchemaImporter.ThrowEnumTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("For simpleType restriction, only enum is supported and this type could not be convert to enum."));
			}
			return null;
		}

		[SecuritySafeCritical]
		private EnumDataContract ImportEnum(XmlQualifiedName typeName, XmlSchemaSimpleTypeRestriction restriction, bool isFlags, XmlSchemaAnnotation annotation)
		{
			EnumDataContract enumDataContract = new EnumDataContract();
			enumDataContract.StableName = typeName;
			enumDataContract.BaseContractName = SchemaImporter.ImportActualType(annotation, SchemaExporter.DefaultEnumBaseTypeName, typeName);
			enumDataContract.IsFlags = isFlags;
			this.AddDataContract(enumDataContract);
			enumDataContract.Values = new List<long>();
			enumDataContract.Members = new List<DataMember>();
			foreach (XmlSchemaObject xmlSchemaObject in restriction.Facets)
			{
				XmlSchemaEnumerationFacet xmlSchemaEnumerationFacet = ((XmlSchemaFacet)xmlSchemaObject) as XmlSchemaEnumerationFacet;
				if (xmlSchemaEnumerationFacet == null)
				{
					SchemaImporter.ThrowEnumTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("For schema facets, only enumeration is supported."));
				}
				if (xmlSchemaEnumerationFacet.Value == null)
				{
					SchemaImporter.ThrowEnumTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Schema enumeration facet must have values."));
				}
				string innerText = this.GetInnerText(typeName, SchemaImporter.ImportAnnotation(xmlSchemaEnumerationFacet.Annotation, SchemaExporter.EnumerationValueAnnotationName));
				if (innerText == null)
				{
					enumDataContract.Values.Add(SchemaExporter.GetDefaultEnumValue(isFlags, enumDataContract.Members.Count));
				}
				else
				{
					enumDataContract.Values.Add(enumDataContract.GetEnumValueFromString(innerText));
				}
				DataMember dataMember = new DataMember(xmlSchemaEnumerationFacet.Value);
				enumDataContract.Members.Add(dataMember);
			}
			return enumDataContract;
		}

		private DataContract ImportSimpleTypeRestriction(XmlQualifiedName typeName, XmlSchemaSimpleTypeRestriction restriction)
		{
			DataContract dataContract = null;
			if (!restriction.BaseTypeName.IsEmpty)
			{
				dataContract = this.ImportType(restriction.BaseTypeName);
			}
			else if (restriction.BaseType != null)
			{
				dataContract = this.ImportType(restriction.BaseType);
			}
			else
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("This simpleType restriction does not specify the base type."));
			}
			return dataContract;
		}

		private void ImportDataContractExtension(XmlSchemaType type, DataContract dataContract)
		{
			if (type.Annotation == null || type.Annotation.Items == null)
			{
				return;
			}
			foreach (XmlSchemaObject xmlSchemaObject in type.Annotation.Items)
			{
				XmlSchemaAppInfo xmlSchemaAppInfo = xmlSchemaObject as XmlSchemaAppInfo;
				if (xmlSchemaAppInfo != null && xmlSchemaAppInfo.Markup != null)
				{
					XmlNode[] markup = xmlSchemaAppInfo.Markup;
					for (int i = 0; i < markup.Length; i++)
					{
						XmlElement xmlElement = markup[i] as XmlElement;
						XmlQualifiedName surrogateDataAnnotationName = SchemaExporter.SurrogateDataAnnotationName;
						if (xmlElement != null && xmlElement.NamespaceURI == surrogateDataAnnotationName.Namespace && xmlElement.LocalName == surrogateDataAnnotationName.Name)
						{
							object obj = this.ImportSurrogateData(xmlElement, surrogateDataAnnotationName.Name, surrogateDataAnnotationName.Namespace);
							this.dataContractSet.SetSurrogateData(dataContract, obj);
						}
					}
				}
			}
		}

		[SecuritySafeCritical]
		private void ImportGenericInfo(XmlSchemaType type, DataContract dataContract)
		{
			if (type.Annotation == null || type.Annotation.Items == null)
			{
				return;
			}
			foreach (XmlSchemaObject xmlSchemaObject in type.Annotation.Items)
			{
				XmlSchemaAppInfo xmlSchemaAppInfo = xmlSchemaObject as XmlSchemaAppInfo;
				if (xmlSchemaAppInfo != null && xmlSchemaAppInfo.Markup != null)
				{
					XmlNode[] markup = xmlSchemaAppInfo.Markup;
					for (int i = 0; i < markup.Length; i++)
					{
						XmlElement xmlElement = markup[i] as XmlElement;
						if (xmlElement != null && xmlElement.NamespaceURI == "http://schemas.microsoft.com/2003/10/Serialization/" && xmlElement.LocalName == "GenericType")
						{
							dataContract.GenericInfo = this.ImportGenericInfo(xmlElement, type);
						}
					}
				}
			}
		}

		private GenericInfo ImportGenericInfo(XmlElement typeElement, XmlSchemaType type)
		{
			XmlNode namedItem = typeElement.Attributes.GetNamedItem("Name");
			string text = ((namedItem == null) ? null : namedItem.Value);
			if (text == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On type '{0}' Generic annotation attribute '{1}' was not found.", new object[] { type.Name, "Name" })));
			}
			XmlNode namedItem2 = typeElement.Attributes.GetNamedItem("Namespace");
			string text2 = ((namedItem2 == null) ? null : namedItem2.Value);
			if (text2 == null)
			{
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On type '{0}' Generic annotation attribute '{1}' was not found.", new object[] { type.Name, "Namespace" })));
			}
			if (typeElement.ChildNodes.Count > 0)
			{
				text = DataContract.EncodeLocalName(text);
			}
			int num = 0;
			GenericInfo genericInfo = new GenericInfo(new XmlQualifiedName(text, text2), type.Name);
			foreach (object obj in typeElement.ChildNodes)
			{
				XmlElement xmlElement = ((XmlNode)obj) as XmlElement;
				if (xmlElement != null)
				{
					if (xmlElement.LocalName != "GenericParameter" || xmlElement.NamespaceURI != "http://schemas.microsoft.com/2003/10/Serialization/")
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On type '{2}', generic annotation has invalid element. Argument element is '{0}' in '{1}' namespace.", new object[] { xmlElement.LocalName, xmlElement.NamespaceURI, type.Name })));
					}
					XmlNode namedItem3 = xmlElement.Attributes.GetNamedItem("NestedLevel");
					int num2 = 0;
					if (namedItem3 != null && !int.TryParse(namedItem3.Value, out num2))
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On type '{2}', generic annotation has invalid attribute value '{3}'. Argument element is '{0}' in '{1}' namespace. Nested level attribute attribute name is '{4}'. Type is '{5}'.", new object[]
						{
							xmlElement.LocalName,
							xmlElement.NamespaceURI,
							type.Name,
							namedItem3.Value,
							namedItem3.LocalName,
							Globals.TypeOfInt.Name
						})));
					}
					if (num2 < num)
					{
						throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On type '{2}', generic annotation for nested level must be increasing. Argument element is '{0}' in '{1}' namespace.", new object[] { xmlElement.LocalName, xmlElement.NamespaceURI, type.Name })));
					}
					genericInfo.Add(this.ImportGenericInfo(xmlElement, type));
					genericInfo.AddToLevel(num2, 1);
					num = num2;
				}
			}
			XmlNode namedItem4 = typeElement.Attributes.GetNamedItem("NestedLevel");
			if (namedItem4 != null)
			{
				int num3 = 0;
				if (!int.TryParse(namedItem4.Value, out num3))
				{
					throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("On type '{2}', generic annotation has invalid attribute value '{3}'. Argument element is '{0}' in '{1}' namespace. Nested level attribute attribute name is '{4}'. Type is '{5}'.", new object[]
					{
						typeElement.LocalName,
						typeElement.NamespaceURI,
						type.Name,
						namedItem4.Value,
						namedItem4.LocalName,
						Globals.TypeOfInt.Name
					})));
				}
				if (num3 - 1 > num)
				{
					genericInfo.AddToLevel(num3 - 1, 0);
				}
			}
			return genericInfo;
		}

		private object ImportSurrogateData(XmlElement typeElement, string name, string ns)
		{
			if (this.dataContractSet.DataContractSurrogate != null && typeElement != null)
			{
				Collection<Type> collection = new Collection<Type>();
				DataContractSurrogateCaller.GetKnownCustomDataTypes(this.dataContractSet.DataContractSurrogate, collection);
				return new DataContractSerializer(Globals.TypeOfObject, name, ns, collection, int.MaxValue, false, true, null).ReadObject(new XmlNodeReader(typeElement));
			}
			return null;
		}

		private void CheckComplexType(XmlQualifiedName typeName, XmlSchemaComplexType type)
		{
			if (type.IsAbstract)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Abstract type is not supported"));
			}
			if (type.IsMixed)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Mixed content is not supported."));
			}
		}

		private void CheckIfElementUsesUnsupportedConstructs(XmlQualifiedName typeName, XmlSchemaElement element)
		{
			if (element.IsAbstract)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Abstract element '{0}' is not supported.", new object[] { element.Name }));
			}
			if (element.DefaultValue != null)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("On element '{0}', default value is not supported.", new object[] { element.Name }));
			}
			if (element.FixedValue != null)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("On schema element '{0}', fixed value is not supported.", new object[] { element.Name }));
			}
			if (!element.SubstitutionGroup.IsEmpty)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("substitutionGroups on elements are not supported.", new object[] { element.Name }));
			}
		}

		private void ImportAttributes(XmlQualifiedName typeName, XmlSchemaObjectCollection attributes, XmlSchemaAnyAttribute anyAttribute, out bool isReference)
		{
			if (anyAttribute != null)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("XML Schema 'any' attribute is not supported"));
			}
			isReference = false;
			if (attributes != null)
			{
				bool flag = false;
				bool flag2 = false;
				for (int i = 0; i < attributes.Count; i++)
				{
					XmlSchemaObject xmlSchemaObject = attributes[i];
					if (xmlSchemaObject is XmlSchemaAttribute)
					{
						XmlSchemaAttribute xmlSchemaAttribute = (XmlSchemaAttribute)xmlSchemaObject;
						if (xmlSchemaAttribute.Use != XmlSchemaUse.Prohibited && !this.TryCheckIfAttribute(typeName, xmlSchemaAttribute, Globals.IdQualifiedName, ref flag) && !this.TryCheckIfAttribute(typeName, xmlSchemaAttribute, Globals.RefQualifiedName, ref flag2) && (xmlSchemaAttribute.RefName.IsEmpty || xmlSchemaAttribute.RefName.Namespace != "http://schemas.microsoft.com/2003/10/Serialization/" || xmlSchemaAttribute.Use == XmlSchemaUse.Required))
						{
							SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Type should not contain attributes. Serialization namespace: '{0}'.", new object[] { "http://schemas.microsoft.com/2003/10/Serialization/" }));
						}
					}
				}
				isReference = flag && flag2;
			}
		}

		private bool TryCheckIfAttribute(XmlQualifiedName typeName, XmlSchemaAttribute attribute, XmlQualifiedName refName, ref bool foundAttribute)
		{
			if (attribute.RefName != refName)
			{
				return false;
			}
			if (foundAttribute)
			{
				SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("Cannot have duplicate attribute names '{0}'.", new object[] { refName.Name }));
			}
			foundAttribute = true;
			return true;
		}

		private void AddDataContract(DataContract dataContract)
		{
			this.dataContractSet.Add(dataContract.StableName, dataContract);
		}

		private string GetInnerText(XmlQualifiedName typeName, XmlElement xmlElement)
		{
			if (xmlElement != null)
			{
				for (XmlNode xmlNode = xmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
				{
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						SchemaImporter.ThrowTypeCannotBeImportedException(typeName.Name, typeName.Namespace, global::System.Runtime.Serialization.SR.GetString("For annotation element '{0}' in namespace '{1}', expected text but got element '{2}' in '{3}' namespace.", new object[] { xmlElement.LocalName, xmlElement.NamespaceURI, xmlNode.LocalName, xmlNode.NamespaceURI }));
					}
				}
				return xmlElement.InnerText;
			}
			return null;
		}

		private static XmlElement ImportAnnotation(XmlSchemaAnnotation annotation, XmlQualifiedName annotationQualifiedName)
		{
			if (annotation != null && annotation.Items != null && annotation.Items.Count > 0 && annotation.Items[0] is XmlSchemaAppInfo)
			{
				XmlNode[] markup = ((XmlSchemaAppInfo)annotation.Items[0]).Markup;
				if (markup != null)
				{
					for (int i = 0; i < markup.Length; i++)
					{
						XmlElement xmlElement = markup[i] as XmlElement;
						if (xmlElement != null && xmlElement.LocalName == annotationQualifiedName.Name && xmlElement.NamespaceURI == annotationQualifiedName.Namespace)
						{
							return xmlElement;
						}
					}
				}
			}
			return null;
		}

		private static void ThrowTypeCannotBeImportedException(string name, string ns, string message)
		{
			SchemaImporter.ThrowTypeCannotBeImportedException(global::System.Runtime.Serialization.SR.GetString("Type '{0}' in '{1}' namespace cannot be imported: {2}", new object[] { name, ns, message }));
		}

		private static void ThrowArrayTypeCannotBeImportedException(string name, string ns, string message)
		{
			SchemaImporter.ThrowTypeCannotBeImportedException(global::System.Runtime.Serialization.SR.GetString("Array type cannot be imported for '{0}' in '{1}' namespace: {2}.", new object[] { name, ns, message }));
		}

		private static void ThrowEnumTypeCannotBeImportedException(string name, string ns, string message)
		{
			SchemaImporter.ThrowTypeCannotBeImportedException(global::System.Runtime.Serialization.SR.GetString("For '{0}' in '{1}' namespace, enum type cannot be imported: {2}", new object[] { name, ns, message }));
		}

		private static void ThrowISerializableTypeCannotBeImportedException(string name, string ns, string message)
		{
			SchemaImporter.ThrowTypeCannotBeImportedException(global::System.Runtime.Serialization.SR.GetString("ISerializable type '{0}' in '{1}' namespace cannot be imported: {2}", new object[] { name, ns, message }));
		}

		private static void ThrowTypeCannotBeImportedException(string message)
		{
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidDataContractException(global::System.Runtime.Serialization.SR.GetString("Type cannot be imported: {0}", new object[] { message })));
		}

		private DataContractSet dataContractSet;

		private XmlSchemaSet schemaSet;

		private ICollection<XmlQualifiedName> typeNames;

		private ICollection<XmlSchemaElement> elements;

		private XmlQualifiedName[] elementTypeNames;

		private bool importXmlDataType;

		private Dictionary<XmlQualifiedName, SchemaObjectInfo> schemaObjects;

		private List<XmlSchemaRedefine> redefineList;

		private bool needToImportKnownTypesForObject;

		[SecurityCritical]
		private static Hashtable serializationSchemaElements;
	}
}
