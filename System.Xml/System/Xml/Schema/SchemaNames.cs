using System;

namespace System.Xml.Schema
{
	internal sealed class SchemaNames
	{
		public XmlNameTable NameTable
		{
			get
			{
				return this.nameTable;
			}
		}

		public SchemaNames(XmlNameTable nameTable)
		{
			this.nameTable = nameTable;
			this.NsDataType = nameTable.Add("urn:schemas-microsoft-com:datatypes");
			this.NsDataTypeAlias = nameTable.Add("uuid:C2F41010-65B3-11D1-A29F-00AA00C14882");
			this.NsDataTypeOld = nameTable.Add("urn:uuid:C2F41010-65B3-11D1-A29F-00AA00C14882/");
			this.NsXml = nameTable.Add("http://www.w3.org/XML/1998/namespace");
			this.NsXmlNs = nameTable.Add("http://www.w3.org/2000/xmlns/");
			this.NsXdr = nameTable.Add("urn:schemas-microsoft-com:xml-data");
			this.NsXdrAlias = nameTable.Add("uuid:BDC6E3F0-6DA3-11D1-A2A3-00AA00C14882");
			this.NsXs = nameTable.Add("http://www.w3.org/2001/XMLSchema");
			this.NsXsi = nameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
			this.XsiType = nameTable.Add("type");
			this.XsiNil = nameTable.Add("nil");
			this.XsiSchemaLocation = nameTable.Add("schemaLocation");
			this.XsiNoNamespaceSchemaLocation = nameTable.Add("noNamespaceSchemaLocation");
			this.XsdSchema = nameTable.Add("schema");
			this.XdrSchema = nameTable.Add("Schema");
			this.QnPCData = new XmlQualifiedName(nameTable.Add("#PCDATA"));
			this.QnXml = new XmlQualifiedName(nameTable.Add("xml"));
			this.QnXmlNs = new XmlQualifiedName(nameTable.Add("xmlns"), this.NsXmlNs);
			this.QnDtDt = new XmlQualifiedName(nameTable.Add("dt"), this.NsDataType);
			this.QnXmlLang = new XmlQualifiedName(nameTable.Add("lang"), this.NsXml);
			this.QnName = new XmlQualifiedName(nameTable.Add("name"));
			this.QnType = new XmlQualifiedName(nameTable.Add("type"));
			this.QnMaxOccurs = new XmlQualifiedName(nameTable.Add("maxOccurs"));
			this.QnMinOccurs = new XmlQualifiedName(nameTable.Add("minOccurs"));
			this.QnInfinite = new XmlQualifiedName(nameTable.Add("*"));
			this.QnModel = new XmlQualifiedName(nameTable.Add("model"));
			this.QnOpen = new XmlQualifiedName(nameTable.Add("open"));
			this.QnClosed = new XmlQualifiedName(nameTable.Add("closed"));
			this.QnContent = new XmlQualifiedName(nameTable.Add("content"));
			this.QnMixed = new XmlQualifiedName(nameTable.Add("mixed"));
			this.QnEmpty = new XmlQualifiedName(nameTable.Add("empty"));
			this.QnEltOnly = new XmlQualifiedName(nameTable.Add("eltOnly"));
			this.QnTextOnly = new XmlQualifiedName(nameTable.Add("textOnly"));
			this.QnOrder = new XmlQualifiedName(nameTable.Add("order"));
			this.QnSeq = new XmlQualifiedName(nameTable.Add("seq"));
			this.QnOne = new XmlQualifiedName(nameTable.Add("one"));
			this.QnMany = new XmlQualifiedName(nameTable.Add("many"));
			this.QnRequired = new XmlQualifiedName(nameTable.Add("required"));
			this.QnYes = new XmlQualifiedName(nameTable.Add("yes"));
			this.QnNo = new XmlQualifiedName(nameTable.Add("no"));
			this.QnString = new XmlQualifiedName(nameTable.Add("string"));
			this.QnID = new XmlQualifiedName(nameTable.Add("id"));
			this.QnIDRef = new XmlQualifiedName(nameTable.Add("idref"));
			this.QnIDRefs = new XmlQualifiedName(nameTable.Add("idrefs"));
			this.QnEntity = new XmlQualifiedName(nameTable.Add("entity"));
			this.QnEntities = new XmlQualifiedName(nameTable.Add("entities"));
			this.QnNmToken = new XmlQualifiedName(nameTable.Add("nmtoken"));
			this.QnNmTokens = new XmlQualifiedName(nameTable.Add("nmtokens"));
			this.QnEnumeration = new XmlQualifiedName(nameTable.Add("enumeration"));
			this.QnDefault = new XmlQualifiedName(nameTable.Add("default"));
			this.QnTargetNamespace = new XmlQualifiedName(nameTable.Add("targetNamespace"));
			this.QnVersion = new XmlQualifiedName(nameTable.Add("version"));
			this.QnFinalDefault = new XmlQualifiedName(nameTable.Add("finalDefault"));
			this.QnBlockDefault = new XmlQualifiedName(nameTable.Add("blockDefault"));
			this.QnFixed = new XmlQualifiedName(nameTable.Add("fixed"));
			this.QnAbstract = new XmlQualifiedName(nameTable.Add("abstract"));
			this.QnBlock = new XmlQualifiedName(nameTable.Add("block"));
			this.QnSubstitutionGroup = new XmlQualifiedName(nameTable.Add("substitutionGroup"));
			this.QnFinal = new XmlQualifiedName(nameTable.Add("final"));
			this.QnNillable = new XmlQualifiedName(nameTable.Add("nillable"));
			this.QnRef = new XmlQualifiedName(nameTable.Add("ref"));
			this.QnBase = new XmlQualifiedName(nameTable.Add("base"));
			this.QnDerivedBy = new XmlQualifiedName(nameTable.Add("derivedBy"));
			this.QnNamespace = new XmlQualifiedName(nameTable.Add("namespace"));
			this.QnProcessContents = new XmlQualifiedName(nameTable.Add("processContents"));
			this.QnRefer = new XmlQualifiedName(nameTable.Add("refer"));
			this.QnPublic = new XmlQualifiedName(nameTable.Add("public"));
			this.QnSystem = new XmlQualifiedName(nameTable.Add("system"));
			this.QnSchemaLocation = new XmlQualifiedName(nameTable.Add("schemaLocation"));
			this.QnValue = new XmlQualifiedName(nameTable.Add("value"));
			this.QnUse = new XmlQualifiedName(nameTable.Add("use"));
			this.QnForm = new XmlQualifiedName(nameTable.Add("form"));
			this.QnAttributeFormDefault = new XmlQualifiedName(nameTable.Add("attributeFormDefault"));
			this.QnElementFormDefault = new XmlQualifiedName(nameTable.Add("elementFormDefault"));
			this.QnSource = new XmlQualifiedName(nameTable.Add("source"));
			this.QnMemberTypes = new XmlQualifiedName(nameTable.Add("memberTypes"));
			this.QnItemType = new XmlQualifiedName(nameTable.Add("itemType"));
			this.QnXPath = new XmlQualifiedName(nameTable.Add("xpath"));
			this.QnXdrSchema = new XmlQualifiedName(this.XdrSchema, this.NsXdr);
			this.QnXdrElementType = new XmlQualifiedName(nameTable.Add("ElementType"), this.NsXdr);
			this.QnXdrElement = new XmlQualifiedName(nameTable.Add("element"), this.NsXdr);
			this.QnXdrGroup = new XmlQualifiedName(nameTable.Add("group"), this.NsXdr);
			this.QnXdrAttributeType = new XmlQualifiedName(nameTable.Add("AttributeType"), this.NsXdr);
			this.QnXdrAttribute = new XmlQualifiedName(nameTable.Add("attribute"), this.NsXdr);
			this.QnXdrDataType = new XmlQualifiedName(nameTable.Add("datatype"), this.NsXdr);
			this.QnXdrDescription = new XmlQualifiedName(nameTable.Add("description"), this.NsXdr);
			this.QnXdrExtends = new XmlQualifiedName(nameTable.Add("extends"), this.NsXdr);
			this.QnXdrAliasSchema = new XmlQualifiedName(nameTable.Add("Schema"), this.NsDataTypeAlias);
			this.QnDtType = new XmlQualifiedName(nameTable.Add("type"), this.NsDataType);
			this.QnDtValues = new XmlQualifiedName(nameTable.Add("values"), this.NsDataType);
			this.QnDtMaxLength = new XmlQualifiedName(nameTable.Add("maxLength"), this.NsDataType);
			this.QnDtMinLength = new XmlQualifiedName(nameTable.Add("minLength"), this.NsDataType);
			this.QnDtMax = new XmlQualifiedName(nameTable.Add("max"), this.NsDataType);
			this.QnDtMin = new XmlQualifiedName(nameTable.Add("min"), this.NsDataType);
			this.QnDtMinExclusive = new XmlQualifiedName(nameTable.Add("minExclusive"), this.NsDataType);
			this.QnDtMaxExclusive = new XmlQualifiedName(nameTable.Add("maxExclusive"), this.NsDataType);
			this.QnXsdSchema = new XmlQualifiedName(this.XsdSchema, this.NsXs);
			this.QnXsdAnnotation = new XmlQualifiedName(nameTable.Add("annotation"), this.NsXs);
			this.QnXsdInclude = new XmlQualifiedName(nameTable.Add("include"), this.NsXs);
			this.QnXsdImport = new XmlQualifiedName(nameTable.Add("import"), this.NsXs);
			this.QnXsdElement = new XmlQualifiedName(nameTable.Add("element"), this.NsXs);
			this.QnXsdAttribute = new XmlQualifiedName(nameTable.Add("attribute"), this.NsXs);
			this.QnXsdAttributeGroup = new XmlQualifiedName(nameTable.Add("attributeGroup"), this.NsXs);
			this.QnXsdAnyAttribute = new XmlQualifiedName(nameTable.Add("anyAttribute"), this.NsXs);
			this.QnXsdGroup = new XmlQualifiedName(nameTable.Add("group"), this.NsXs);
			this.QnXsdAll = new XmlQualifiedName(nameTable.Add("all"), this.NsXs);
			this.QnXsdChoice = new XmlQualifiedName(nameTable.Add("choice"), this.NsXs);
			this.QnXsdSequence = new XmlQualifiedName(nameTable.Add("sequence"), this.NsXs);
			this.QnXsdAny = new XmlQualifiedName(nameTable.Add("any"), this.NsXs);
			this.QnXsdNotation = new XmlQualifiedName(nameTable.Add("notation"), this.NsXs);
			this.QnXsdSimpleType = new XmlQualifiedName(nameTable.Add("simpleType"), this.NsXs);
			this.QnXsdComplexType = new XmlQualifiedName(nameTable.Add("complexType"), this.NsXs);
			this.QnXsdUnique = new XmlQualifiedName(nameTable.Add("unique"), this.NsXs);
			this.QnXsdKey = new XmlQualifiedName(nameTable.Add("key"), this.NsXs);
			this.QnXsdKeyRef = new XmlQualifiedName(nameTable.Add("keyref"), this.NsXs);
			this.QnXsdSelector = new XmlQualifiedName(nameTable.Add("selector"), this.NsXs);
			this.QnXsdField = new XmlQualifiedName(nameTable.Add("field"), this.NsXs);
			this.QnXsdMinExclusive = new XmlQualifiedName(nameTable.Add("minExclusive"), this.NsXs);
			this.QnXsdMinInclusive = new XmlQualifiedName(nameTable.Add("minInclusive"), this.NsXs);
			this.QnXsdMaxInclusive = new XmlQualifiedName(nameTable.Add("maxInclusive"), this.NsXs);
			this.QnXsdMaxExclusive = new XmlQualifiedName(nameTable.Add("maxExclusive"), this.NsXs);
			this.QnXsdTotalDigits = new XmlQualifiedName(nameTable.Add("totalDigits"), this.NsXs);
			this.QnXsdFractionDigits = new XmlQualifiedName(nameTable.Add("fractionDigits"), this.NsXs);
			this.QnXsdLength = new XmlQualifiedName(nameTable.Add("length"), this.NsXs);
			this.QnXsdMinLength = new XmlQualifiedName(nameTable.Add("minLength"), this.NsXs);
			this.QnXsdMaxLength = new XmlQualifiedName(nameTable.Add("maxLength"), this.NsXs);
			this.QnXsdEnumeration = new XmlQualifiedName(nameTable.Add("enumeration"), this.NsXs);
			this.QnXsdPattern = new XmlQualifiedName(nameTable.Add("pattern"), this.NsXs);
			this.QnXsdDocumentation = new XmlQualifiedName(nameTable.Add("documentation"), this.NsXs);
			this.QnXsdAppinfo = new XmlQualifiedName(nameTable.Add("appinfo"), this.NsXs);
			this.QnXsdComplexContent = new XmlQualifiedName(nameTable.Add("complexContent"), this.NsXs);
			this.QnXsdSimpleContent = new XmlQualifiedName(nameTable.Add("simpleContent"), this.NsXs);
			this.QnXsdRestriction = new XmlQualifiedName(nameTable.Add("restriction"), this.NsXs);
			this.QnXsdExtension = new XmlQualifiedName(nameTable.Add("extension"), this.NsXs);
			this.QnXsdUnion = new XmlQualifiedName(nameTable.Add("union"), this.NsXs);
			this.QnXsdList = new XmlQualifiedName(nameTable.Add("list"), this.NsXs);
			this.QnXsdWhiteSpace = new XmlQualifiedName(nameTable.Add("whiteSpace"), this.NsXs);
			this.QnXsdRedefine = new XmlQualifiedName(nameTable.Add("redefine"), this.NsXs);
			this.QnXsdAnyType = new XmlQualifiedName(nameTable.Add("anyType"), this.NsXs);
			this.CreateTokenToQNameTable();
		}

		public void CreateTokenToQNameTable()
		{
			this.TokenToQName[1] = this.QnName;
			this.TokenToQName[2] = this.QnType;
			this.TokenToQName[3] = this.QnMaxOccurs;
			this.TokenToQName[4] = this.QnMinOccurs;
			this.TokenToQName[5] = this.QnInfinite;
			this.TokenToQName[6] = this.QnModel;
			this.TokenToQName[7] = this.QnOpen;
			this.TokenToQName[8] = this.QnClosed;
			this.TokenToQName[9] = this.QnContent;
			this.TokenToQName[10] = this.QnMixed;
			this.TokenToQName[11] = this.QnEmpty;
			this.TokenToQName[12] = this.QnEltOnly;
			this.TokenToQName[13] = this.QnTextOnly;
			this.TokenToQName[14] = this.QnOrder;
			this.TokenToQName[15] = this.QnSeq;
			this.TokenToQName[16] = this.QnOne;
			this.TokenToQName[17] = this.QnMany;
			this.TokenToQName[18] = this.QnRequired;
			this.TokenToQName[19] = this.QnYes;
			this.TokenToQName[20] = this.QnNo;
			this.TokenToQName[21] = this.QnString;
			this.TokenToQName[22] = this.QnID;
			this.TokenToQName[23] = this.QnIDRef;
			this.TokenToQName[24] = this.QnIDRefs;
			this.TokenToQName[25] = this.QnEntity;
			this.TokenToQName[26] = this.QnEntities;
			this.TokenToQName[27] = this.QnNmToken;
			this.TokenToQName[28] = this.QnNmTokens;
			this.TokenToQName[29] = this.QnEnumeration;
			this.TokenToQName[30] = this.QnDefault;
			this.TokenToQName[31] = this.QnXdrSchema;
			this.TokenToQName[32] = this.QnXdrElementType;
			this.TokenToQName[33] = this.QnXdrElement;
			this.TokenToQName[34] = this.QnXdrGroup;
			this.TokenToQName[35] = this.QnXdrAttributeType;
			this.TokenToQName[36] = this.QnXdrAttribute;
			this.TokenToQName[37] = this.QnXdrDataType;
			this.TokenToQName[38] = this.QnXdrDescription;
			this.TokenToQName[39] = this.QnXdrExtends;
			this.TokenToQName[40] = this.QnXdrAliasSchema;
			this.TokenToQName[41] = this.QnDtType;
			this.TokenToQName[42] = this.QnDtValues;
			this.TokenToQName[43] = this.QnDtMaxLength;
			this.TokenToQName[44] = this.QnDtMinLength;
			this.TokenToQName[45] = this.QnDtMax;
			this.TokenToQName[46] = this.QnDtMin;
			this.TokenToQName[47] = this.QnDtMinExclusive;
			this.TokenToQName[48] = this.QnDtMaxExclusive;
			this.TokenToQName[49] = this.QnTargetNamespace;
			this.TokenToQName[50] = this.QnVersion;
			this.TokenToQName[51] = this.QnFinalDefault;
			this.TokenToQName[52] = this.QnBlockDefault;
			this.TokenToQName[53] = this.QnFixed;
			this.TokenToQName[54] = this.QnAbstract;
			this.TokenToQName[55] = this.QnBlock;
			this.TokenToQName[56] = this.QnSubstitutionGroup;
			this.TokenToQName[57] = this.QnFinal;
			this.TokenToQName[58] = this.QnNillable;
			this.TokenToQName[59] = this.QnRef;
			this.TokenToQName[60] = this.QnBase;
			this.TokenToQName[61] = this.QnDerivedBy;
			this.TokenToQName[62] = this.QnNamespace;
			this.TokenToQName[63] = this.QnProcessContents;
			this.TokenToQName[64] = this.QnRefer;
			this.TokenToQName[65] = this.QnPublic;
			this.TokenToQName[66] = this.QnSystem;
			this.TokenToQName[67] = this.QnSchemaLocation;
			this.TokenToQName[68] = this.QnValue;
			this.TokenToQName[119] = this.QnItemType;
			this.TokenToQName[120] = this.QnMemberTypes;
			this.TokenToQName[121] = this.QnXPath;
			this.TokenToQName[74] = this.QnXsdSchema;
			this.TokenToQName[75] = this.QnXsdAnnotation;
			this.TokenToQName[76] = this.QnXsdInclude;
			this.TokenToQName[77] = this.QnXsdImport;
			this.TokenToQName[78] = this.QnXsdElement;
			this.TokenToQName[79] = this.QnXsdAttribute;
			this.TokenToQName[80] = this.QnXsdAttributeGroup;
			this.TokenToQName[81] = this.QnXsdAnyAttribute;
			this.TokenToQName[82] = this.QnXsdGroup;
			this.TokenToQName[83] = this.QnXsdAll;
			this.TokenToQName[84] = this.QnXsdChoice;
			this.TokenToQName[85] = this.QnXsdSequence;
			this.TokenToQName[86] = this.QnXsdAny;
			this.TokenToQName[87] = this.QnXsdNotation;
			this.TokenToQName[88] = this.QnXsdSimpleType;
			this.TokenToQName[89] = this.QnXsdComplexType;
			this.TokenToQName[90] = this.QnXsdUnique;
			this.TokenToQName[91] = this.QnXsdKey;
			this.TokenToQName[92] = this.QnXsdKeyRef;
			this.TokenToQName[93] = this.QnXsdSelector;
			this.TokenToQName[94] = this.QnXsdField;
			this.TokenToQName[95] = this.QnXsdMinExclusive;
			this.TokenToQName[96] = this.QnXsdMinInclusive;
			this.TokenToQName[97] = this.QnXsdMaxExclusive;
			this.TokenToQName[98] = this.QnXsdMaxInclusive;
			this.TokenToQName[99] = this.QnXsdTotalDigits;
			this.TokenToQName[100] = this.QnXsdFractionDigits;
			this.TokenToQName[101] = this.QnXsdLength;
			this.TokenToQName[102] = this.QnXsdMinLength;
			this.TokenToQName[103] = this.QnXsdMaxLength;
			this.TokenToQName[104] = this.QnXsdEnumeration;
			this.TokenToQName[105] = this.QnXsdPattern;
			this.TokenToQName[117] = this.QnXsdWhiteSpace;
			this.TokenToQName[106] = this.QnXsdDocumentation;
			this.TokenToQName[107] = this.QnXsdAppinfo;
			this.TokenToQName[108] = this.QnXsdComplexContent;
			this.TokenToQName[110] = this.QnXsdRestriction;
			this.TokenToQName[113] = this.QnXsdRestriction;
			this.TokenToQName[115] = this.QnXsdRestriction;
			this.TokenToQName[109] = this.QnXsdExtension;
			this.TokenToQName[112] = this.QnXsdExtension;
			this.TokenToQName[111] = this.QnXsdSimpleContent;
			this.TokenToQName[116] = this.QnXsdUnion;
			this.TokenToQName[114] = this.QnXsdList;
			this.TokenToQName[118] = this.QnXsdRedefine;
			this.TokenToQName[69] = this.QnSource;
			this.TokenToQName[72] = this.QnUse;
			this.TokenToQName[73] = this.QnForm;
			this.TokenToQName[71] = this.QnElementFormDefault;
			this.TokenToQName[70] = this.QnAttributeFormDefault;
			this.TokenToQName[122] = this.QnXmlLang;
			this.TokenToQName[0] = XmlQualifiedName.Empty;
		}

		public SchemaType SchemaTypeFromRoot(string localName, string ns)
		{
			if (this.IsXSDRoot(localName, ns))
			{
				return SchemaType.XSD;
			}
			if (this.IsXDRRoot(localName, XmlSchemaDatatype.XdrCanonizeUri(ns, this.nameTable, this)))
			{
				return SchemaType.XDR;
			}
			return SchemaType.None;
		}

		public bool IsXSDRoot(string localName, string ns)
		{
			return localName == this.XsdSchema && ns == this.NsXs;
		}

		public bool IsXDRRoot(string localName, string ns)
		{
			return localName == this.XdrSchema && ns == this.NsXdr;
		}

		public XmlQualifiedName GetName(SchemaNames.Token token)
		{
			return this.TokenToQName[(int)token];
		}

		private XmlNameTable nameTable;

		public string NsDataType;

		public string NsDataTypeAlias;

		public string NsDataTypeOld;

		public string NsXml;

		public string NsXmlNs;

		public string NsXdr;

		public string NsXdrAlias;

		public string NsXs;

		public string NsXsi;

		public string XsiType;

		public string XsiNil;

		public string XsiSchemaLocation;

		public string XsiNoNamespaceSchemaLocation;

		public string XsdSchema;

		public string XdrSchema;

		public XmlQualifiedName QnPCData;

		public XmlQualifiedName QnXml;

		public XmlQualifiedName QnXmlNs;

		public XmlQualifiedName QnDtDt;

		public XmlQualifiedName QnXmlLang;

		public XmlQualifiedName QnName;

		public XmlQualifiedName QnType;

		public XmlQualifiedName QnMaxOccurs;

		public XmlQualifiedName QnMinOccurs;

		public XmlQualifiedName QnInfinite;

		public XmlQualifiedName QnModel;

		public XmlQualifiedName QnOpen;

		public XmlQualifiedName QnClosed;

		public XmlQualifiedName QnContent;

		public XmlQualifiedName QnMixed;

		public XmlQualifiedName QnEmpty;

		public XmlQualifiedName QnEltOnly;

		public XmlQualifiedName QnTextOnly;

		public XmlQualifiedName QnOrder;

		public XmlQualifiedName QnSeq;

		public XmlQualifiedName QnOne;

		public XmlQualifiedName QnMany;

		public XmlQualifiedName QnRequired;

		public XmlQualifiedName QnYes;

		public XmlQualifiedName QnNo;

		public XmlQualifiedName QnString;

		public XmlQualifiedName QnID;

		public XmlQualifiedName QnIDRef;

		public XmlQualifiedName QnIDRefs;

		public XmlQualifiedName QnEntity;

		public XmlQualifiedName QnEntities;

		public XmlQualifiedName QnNmToken;

		public XmlQualifiedName QnNmTokens;

		public XmlQualifiedName QnEnumeration;

		public XmlQualifiedName QnDefault;

		public XmlQualifiedName QnXdrSchema;

		public XmlQualifiedName QnXdrElementType;

		public XmlQualifiedName QnXdrElement;

		public XmlQualifiedName QnXdrGroup;

		public XmlQualifiedName QnXdrAttributeType;

		public XmlQualifiedName QnXdrAttribute;

		public XmlQualifiedName QnXdrDataType;

		public XmlQualifiedName QnXdrDescription;

		public XmlQualifiedName QnXdrExtends;

		public XmlQualifiedName QnXdrAliasSchema;

		public XmlQualifiedName QnDtType;

		public XmlQualifiedName QnDtValues;

		public XmlQualifiedName QnDtMaxLength;

		public XmlQualifiedName QnDtMinLength;

		public XmlQualifiedName QnDtMax;

		public XmlQualifiedName QnDtMin;

		public XmlQualifiedName QnDtMinExclusive;

		public XmlQualifiedName QnDtMaxExclusive;

		public XmlQualifiedName QnTargetNamespace;

		public XmlQualifiedName QnVersion;

		public XmlQualifiedName QnFinalDefault;

		public XmlQualifiedName QnBlockDefault;

		public XmlQualifiedName QnFixed;

		public XmlQualifiedName QnAbstract;

		public XmlQualifiedName QnBlock;

		public XmlQualifiedName QnSubstitutionGroup;

		public XmlQualifiedName QnFinal;

		public XmlQualifiedName QnNillable;

		public XmlQualifiedName QnRef;

		public XmlQualifiedName QnBase;

		public XmlQualifiedName QnDerivedBy;

		public XmlQualifiedName QnNamespace;

		public XmlQualifiedName QnProcessContents;

		public XmlQualifiedName QnRefer;

		public XmlQualifiedName QnPublic;

		public XmlQualifiedName QnSystem;

		public XmlQualifiedName QnSchemaLocation;

		public XmlQualifiedName QnValue;

		public XmlQualifiedName QnUse;

		public XmlQualifiedName QnForm;

		public XmlQualifiedName QnElementFormDefault;

		public XmlQualifiedName QnAttributeFormDefault;

		public XmlQualifiedName QnItemType;

		public XmlQualifiedName QnMemberTypes;

		public XmlQualifiedName QnXPath;

		public XmlQualifiedName QnXsdSchema;

		public XmlQualifiedName QnXsdAnnotation;

		public XmlQualifiedName QnXsdInclude;

		public XmlQualifiedName QnXsdImport;

		public XmlQualifiedName QnXsdElement;

		public XmlQualifiedName QnXsdAttribute;

		public XmlQualifiedName QnXsdAttributeGroup;

		public XmlQualifiedName QnXsdAnyAttribute;

		public XmlQualifiedName QnXsdGroup;

		public XmlQualifiedName QnXsdAll;

		public XmlQualifiedName QnXsdChoice;

		public XmlQualifiedName QnXsdSequence;

		public XmlQualifiedName QnXsdAny;

		public XmlQualifiedName QnXsdNotation;

		public XmlQualifiedName QnXsdSimpleType;

		public XmlQualifiedName QnXsdComplexType;

		public XmlQualifiedName QnXsdUnique;

		public XmlQualifiedName QnXsdKey;

		public XmlQualifiedName QnXsdKeyRef;

		public XmlQualifiedName QnXsdSelector;

		public XmlQualifiedName QnXsdField;

		public XmlQualifiedName QnXsdMinExclusive;

		public XmlQualifiedName QnXsdMinInclusive;

		public XmlQualifiedName QnXsdMaxInclusive;

		public XmlQualifiedName QnXsdMaxExclusive;

		public XmlQualifiedName QnXsdTotalDigits;

		public XmlQualifiedName QnXsdFractionDigits;

		public XmlQualifiedName QnXsdLength;

		public XmlQualifiedName QnXsdMinLength;

		public XmlQualifiedName QnXsdMaxLength;

		public XmlQualifiedName QnXsdEnumeration;

		public XmlQualifiedName QnXsdPattern;

		public XmlQualifiedName QnXsdDocumentation;

		public XmlQualifiedName QnXsdAppinfo;

		public XmlQualifiedName QnSource;

		public XmlQualifiedName QnXsdComplexContent;

		public XmlQualifiedName QnXsdSimpleContent;

		public XmlQualifiedName QnXsdRestriction;

		public XmlQualifiedName QnXsdExtension;

		public XmlQualifiedName QnXsdUnion;

		public XmlQualifiedName QnXsdList;

		public XmlQualifiedName QnXsdWhiteSpace;

		public XmlQualifiedName QnXsdRedefine;

		public XmlQualifiedName QnXsdAnyType;

		internal XmlQualifiedName[] TokenToQName = new XmlQualifiedName[123];

		public enum Token
		{
			Empty,
			SchemaName,
			SchemaType,
			SchemaMaxOccurs,
			SchemaMinOccurs,
			SchemaInfinite,
			SchemaModel,
			SchemaOpen,
			SchemaClosed,
			SchemaContent,
			SchemaMixed,
			SchemaEmpty,
			SchemaElementOnly,
			SchemaTextOnly,
			SchemaOrder,
			SchemaSeq,
			SchemaOne,
			SchemaMany,
			SchemaRequired,
			SchemaYes,
			SchemaNo,
			SchemaString,
			SchemaId,
			SchemaIdref,
			SchemaIdrefs,
			SchemaEntity,
			SchemaEntities,
			SchemaNmtoken,
			SchemaNmtokens,
			SchemaEnumeration,
			SchemaDefault,
			XdrRoot,
			XdrElementType,
			XdrElement,
			XdrGroup,
			XdrAttributeType,
			XdrAttribute,
			XdrDatatype,
			XdrDescription,
			XdrExtends,
			SchemaXdrRootAlias,
			SchemaDtType,
			SchemaDtValues,
			SchemaDtMaxLength,
			SchemaDtMinLength,
			SchemaDtMax,
			SchemaDtMin,
			SchemaDtMinExclusive,
			SchemaDtMaxExclusive,
			SchemaTargetNamespace,
			SchemaVersion,
			SchemaFinalDefault,
			SchemaBlockDefault,
			SchemaFixed,
			SchemaAbstract,
			SchemaBlock,
			SchemaSubstitutionGroup,
			SchemaFinal,
			SchemaNillable,
			SchemaRef,
			SchemaBase,
			SchemaDerivedBy,
			SchemaNamespace,
			SchemaProcessContents,
			SchemaRefer,
			SchemaPublic,
			SchemaSystem,
			SchemaSchemaLocation,
			SchemaValue,
			SchemaSource,
			SchemaAttributeFormDefault,
			SchemaElementFormDefault,
			SchemaUse,
			SchemaForm,
			XsdSchema,
			XsdAnnotation,
			XsdInclude,
			XsdImport,
			XsdElement,
			XsdAttribute,
			xsdAttributeGroup,
			XsdAnyAttribute,
			XsdGroup,
			XsdAll,
			XsdChoice,
			XsdSequence,
			XsdAny,
			XsdNotation,
			XsdSimpleType,
			XsdComplexType,
			XsdUnique,
			XsdKey,
			XsdKeyref,
			XsdSelector,
			XsdField,
			XsdMinExclusive,
			XsdMinInclusive,
			XsdMaxExclusive,
			XsdMaxInclusive,
			XsdTotalDigits,
			XsdFractionDigits,
			XsdLength,
			XsdMinLength,
			XsdMaxLength,
			XsdEnumeration,
			XsdPattern,
			XsdDocumentation,
			XsdAppInfo,
			XsdComplexContent,
			XsdComplexContentExtension,
			XsdComplexContentRestriction,
			XsdSimpleContent,
			XsdSimpleContentExtension,
			XsdSimpleContentRestriction,
			XsdSimpleTypeList,
			XsdSimpleTypeRestriction,
			XsdSimpleTypeUnion,
			XsdWhitespace,
			XsdRedefine,
			SchemaItemType,
			SchemaMemberTypes,
			SchemaXPath,
			XmlLang
		}
	}
}
