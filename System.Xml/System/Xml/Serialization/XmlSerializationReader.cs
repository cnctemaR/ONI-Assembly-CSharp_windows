using System;
using System.Collections;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Xml.Serialization.Configuration;

namespace System.Xml.Serialization
{
	public abstract class XmlSerializationReader : XmlSerializationGeneratedCode
	{
		static XmlSerializationReader()
		{
			XmlSerializerSection xmlSerializerSection = ConfigurationManager.GetSection(ConfigurationStrings.XmlSerializerSectionPath) as XmlSerializerSection;
			XmlSerializationReader.checkDeserializeAdvances = xmlSerializerSection != null && xmlSerializerSection.CheckDeserializeAdvances;
		}

		protected abstract void InitIDs();

		internal void Init(XmlReader r, XmlDeserializationEvents events, string encodingStyle, TempAssembly tempAssembly)
		{
			this.events = events;
			if (XmlSerializationReader.checkDeserializeAdvances)
			{
				this.countingReader = new XmlCountingReader(r);
				this.r = this.countingReader;
			}
			else
			{
				this.r = r;
			}
			this.d = null;
			this.soap12 = encodingStyle == "http://www.w3.org/2003/05/soap-encoding";
			base.Init(tempAssembly);
			this.schemaNsID = r.NameTable.Add("http://www.w3.org/2001/XMLSchema");
			this.schemaNs2000ID = r.NameTable.Add("http://www.w3.org/2000/10/XMLSchema");
			this.schemaNs1999ID = r.NameTable.Add("http://www.w3.org/1999/XMLSchema");
			this.schemaNonXsdTypesNsID = r.NameTable.Add("http://microsoft.com/wsdl/types/");
			this.instanceNsID = r.NameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
			this.instanceNs2000ID = r.NameTable.Add("http://www.w3.org/2000/10/XMLSchema-instance");
			this.instanceNs1999ID = r.NameTable.Add("http://www.w3.org/1999/XMLSchema-instance");
			this.soapNsID = r.NameTable.Add("http://schemas.xmlsoap.org/soap/encoding/");
			this.soap12NsID = r.NameTable.Add("http://www.w3.org/2003/05/soap-encoding");
			this.schemaID = r.NameTable.Add("schema");
			this.wsdlNsID = r.NameTable.Add("http://schemas.xmlsoap.org/wsdl/");
			this.wsdlArrayTypeID = r.NameTable.Add("arrayType");
			this.nullID = r.NameTable.Add("null");
			this.nilID = r.NameTable.Add("nil");
			this.typeID = r.NameTable.Add("type");
			this.arrayTypeID = r.NameTable.Add("arrayType");
			this.itemTypeID = r.NameTable.Add("itemType");
			this.arraySizeID = r.NameTable.Add("arraySize");
			this.arrayID = r.NameTable.Add("Array");
			this.urTypeID = r.NameTable.Add("anyType");
			this.InitIDs();
		}

		protected bool DecodeName
		{
			get
			{
				return this.decodeName;
			}
			set
			{
				this.decodeName = value;
			}
		}

		protected XmlReader Reader
		{
			get
			{
				return this.r;
			}
		}

		protected int ReaderCount
		{
			get
			{
				if (!XmlSerializationReader.checkDeserializeAdvances)
				{
					return 0;
				}
				return this.countingReader.AdvanceCount;
			}
		}

		protected XmlDocument Document
		{
			get
			{
				if (this.d == null)
				{
					this.d = new XmlDocument(this.r.NameTable);
					this.d.SetBaseURI(this.r.BaseURI);
				}
				return this.d;
			}
		}

		protected static Assembly ResolveDynamicAssembly(string assemblyFullName)
		{
			return DynamicAssemblies.Get(assemblyFullName);
		}

		private void InitPrimitiveIDs()
		{
			if (this.tokenID != null)
			{
				return;
			}
			this.r.NameTable.Add("http://www.w3.org/2001/XMLSchema");
			this.r.NameTable.Add("http://microsoft.com/wsdl/types/");
			this.stringID = this.r.NameTable.Add("string");
			this.intID = this.r.NameTable.Add("int");
			this.booleanID = this.r.NameTable.Add("boolean");
			this.shortID = this.r.NameTable.Add("short");
			this.longID = this.r.NameTable.Add("long");
			this.floatID = this.r.NameTable.Add("float");
			this.doubleID = this.r.NameTable.Add("double");
			this.decimalID = this.r.NameTable.Add("decimal");
			this.dateTimeID = this.r.NameTable.Add("dateTime");
			this.qnameID = this.r.NameTable.Add("QName");
			this.dateID = this.r.NameTable.Add("date");
			this.timeID = this.r.NameTable.Add("time");
			this.hexBinaryID = this.r.NameTable.Add("hexBinary");
			this.base64BinaryID = this.r.NameTable.Add("base64Binary");
			this.unsignedByteID = this.r.NameTable.Add("unsignedByte");
			this.byteID = this.r.NameTable.Add("byte");
			this.unsignedShortID = this.r.NameTable.Add("unsignedShort");
			this.unsignedIntID = this.r.NameTable.Add("unsignedInt");
			this.unsignedLongID = this.r.NameTable.Add("unsignedLong");
			this.oldDecimalID = this.r.NameTable.Add("decimal");
			this.oldTimeInstantID = this.r.NameTable.Add("timeInstant");
			this.charID = this.r.NameTable.Add("char");
			this.guidID = this.r.NameTable.Add("guid");
			if (LocalAppContextSwitches.EnableTimeSpanSerialization)
			{
				this.timeSpanID = this.r.NameTable.Add("TimeSpan");
			}
			this.base64ID = this.r.NameTable.Add("base64");
			this.anyURIID = this.r.NameTable.Add("anyURI");
			this.durationID = this.r.NameTable.Add("duration");
			this.ENTITYID = this.r.NameTable.Add("ENTITY");
			this.ENTITIESID = this.r.NameTable.Add("ENTITIES");
			this.gDayID = this.r.NameTable.Add("gDay");
			this.gMonthID = this.r.NameTable.Add("gMonth");
			this.gMonthDayID = this.r.NameTable.Add("gMonthDay");
			this.gYearID = this.r.NameTable.Add("gYear");
			this.gYearMonthID = this.r.NameTable.Add("gYearMonth");
			this.IDID = this.r.NameTable.Add("ID");
			this.IDREFID = this.r.NameTable.Add("IDREF");
			this.IDREFSID = this.r.NameTable.Add("IDREFS");
			this.integerID = this.r.NameTable.Add("integer");
			this.languageID = this.r.NameTable.Add("language");
			this.NameID = this.r.NameTable.Add("Name");
			this.NCNameID = this.r.NameTable.Add("NCName");
			this.NMTOKENID = this.r.NameTable.Add("NMTOKEN");
			this.NMTOKENSID = this.r.NameTable.Add("NMTOKENS");
			this.negativeIntegerID = this.r.NameTable.Add("negativeInteger");
			this.nonNegativeIntegerID = this.r.NameTable.Add("nonNegativeInteger");
			this.nonPositiveIntegerID = this.r.NameTable.Add("nonPositiveInteger");
			this.normalizedStringID = this.r.NameTable.Add("normalizedString");
			this.NOTATIONID = this.r.NameTable.Add("NOTATION");
			this.positiveIntegerID = this.r.NameTable.Add("positiveInteger");
			this.tokenID = this.r.NameTable.Add("token");
		}

		protected XmlQualifiedName GetXsiType()
		{
			string text = this.r.GetAttribute(this.typeID, this.instanceNsID);
			if (text == null)
			{
				text = this.r.GetAttribute(this.typeID, this.instanceNs2000ID);
				if (text == null)
				{
					text = this.r.GetAttribute(this.typeID, this.instanceNs1999ID);
					if (text == null)
					{
						return null;
					}
				}
			}
			return this.ToXmlQualifiedName(text, false);
		}

		private Type GetPrimitiveType(XmlQualifiedName typeName, bool throwOnUnknown)
		{
			this.InitPrimitiveIDs();
			if (typeName.Namespace == this.schemaNsID || typeName.Namespace == this.soapNsID || typeName.Namespace == this.soap12NsID)
			{
				if (typeName.Name == this.stringID || typeName.Name == this.anyURIID || typeName.Name == this.durationID || typeName.Name == this.ENTITYID || typeName.Name == this.ENTITIESID || typeName.Name == this.gDayID || typeName.Name == this.gMonthID || typeName.Name == this.gMonthDayID || typeName.Name == this.gYearID || typeName.Name == this.gYearMonthID || typeName.Name == this.IDID || typeName.Name == this.IDREFID || typeName.Name == this.IDREFSID || typeName.Name == this.integerID || typeName.Name == this.languageID || typeName.Name == this.NameID || typeName.Name == this.NCNameID || typeName.Name == this.NMTOKENID || typeName.Name == this.NMTOKENSID || typeName.Name == this.negativeIntegerID || typeName.Name == this.nonPositiveIntegerID || typeName.Name == this.nonNegativeIntegerID || typeName.Name == this.normalizedStringID || typeName.Name == this.NOTATIONID || typeName.Name == this.positiveIntegerID || typeName.Name == this.tokenID)
				{
					return typeof(string);
				}
				if (typeName.Name == this.intID)
				{
					return typeof(int);
				}
				if (typeName.Name == this.booleanID)
				{
					return typeof(bool);
				}
				if (typeName.Name == this.shortID)
				{
					return typeof(short);
				}
				if (typeName.Name == this.longID)
				{
					return typeof(long);
				}
				if (typeName.Name == this.floatID)
				{
					return typeof(float);
				}
				if (typeName.Name == this.doubleID)
				{
					return typeof(double);
				}
				if (typeName.Name == this.decimalID)
				{
					return typeof(decimal);
				}
				if (typeName.Name == this.dateTimeID)
				{
					return typeof(DateTime);
				}
				if (typeName.Name == this.qnameID)
				{
					return typeof(XmlQualifiedName);
				}
				if (typeName.Name == this.dateID)
				{
					return typeof(DateTime);
				}
				if (typeName.Name == this.timeID)
				{
					return typeof(DateTime);
				}
				if (typeName.Name == this.hexBinaryID)
				{
					return typeof(byte[]);
				}
				if (typeName.Name == this.base64BinaryID)
				{
					return typeof(byte[]);
				}
				if (typeName.Name == this.unsignedByteID)
				{
					return typeof(byte);
				}
				if (typeName.Name == this.byteID)
				{
					return typeof(sbyte);
				}
				if (typeName.Name == this.unsignedShortID)
				{
					return typeof(ushort);
				}
				if (typeName.Name == this.unsignedIntID)
				{
					return typeof(uint);
				}
				if (typeName.Name == this.unsignedLongID)
				{
					return typeof(ulong);
				}
				throw this.CreateUnknownTypeException(typeName);
			}
			else if (typeName.Namespace == this.schemaNs2000ID || typeName.Namespace == this.schemaNs1999ID)
			{
				if (typeName.Name == this.stringID || typeName.Name == this.anyURIID || typeName.Name == this.durationID || typeName.Name == this.ENTITYID || typeName.Name == this.ENTITIESID || typeName.Name == this.gDayID || typeName.Name == this.gMonthID || typeName.Name == this.gMonthDayID || typeName.Name == this.gYearID || typeName.Name == this.gYearMonthID || typeName.Name == this.IDID || typeName.Name == this.IDREFID || typeName.Name == this.IDREFSID || typeName.Name == this.integerID || typeName.Name == this.languageID || typeName.Name == this.NameID || typeName.Name == this.NCNameID || typeName.Name == this.NMTOKENID || typeName.Name == this.NMTOKENSID || typeName.Name == this.negativeIntegerID || typeName.Name == this.nonPositiveIntegerID || typeName.Name == this.nonNegativeIntegerID || typeName.Name == this.normalizedStringID || typeName.Name == this.NOTATIONID || typeName.Name == this.positiveIntegerID || typeName.Name == this.tokenID)
				{
					return typeof(string);
				}
				if (typeName.Name == this.intID)
				{
					return typeof(int);
				}
				if (typeName.Name == this.booleanID)
				{
					return typeof(bool);
				}
				if (typeName.Name == this.shortID)
				{
					return typeof(short);
				}
				if (typeName.Name == this.longID)
				{
					return typeof(long);
				}
				if (typeName.Name == this.floatID)
				{
					return typeof(float);
				}
				if (typeName.Name == this.doubleID)
				{
					return typeof(double);
				}
				if (typeName.Name == this.oldDecimalID)
				{
					return typeof(decimal);
				}
				if (typeName.Name == this.oldTimeInstantID)
				{
					return typeof(DateTime);
				}
				if (typeName.Name == this.qnameID)
				{
					return typeof(XmlQualifiedName);
				}
				if (typeName.Name == this.dateID)
				{
					return typeof(DateTime);
				}
				if (typeName.Name == this.timeID)
				{
					return typeof(DateTime);
				}
				if (typeName.Name == this.hexBinaryID)
				{
					return typeof(byte[]);
				}
				if (typeName.Name == this.byteID)
				{
					return typeof(sbyte);
				}
				if (typeName.Name == this.unsignedShortID)
				{
					return typeof(ushort);
				}
				if (typeName.Name == this.unsignedIntID)
				{
					return typeof(uint);
				}
				if (typeName.Name == this.unsignedLongID)
				{
					return typeof(ulong);
				}
				throw this.CreateUnknownTypeException(typeName);
			}
			else if (typeName.Namespace == this.schemaNonXsdTypesNsID)
			{
				if (typeName.Name == this.charID)
				{
					return typeof(char);
				}
				if (typeName.Name == this.guidID)
				{
					return typeof(Guid);
				}
				throw this.CreateUnknownTypeException(typeName);
			}
			else
			{
				if (throwOnUnknown)
				{
					throw this.CreateUnknownTypeException(typeName);
				}
				return null;
			}
		}

		private bool IsPrimitiveNamespace(string ns)
		{
			return ns == this.schemaNsID || ns == this.schemaNonXsdTypesNsID || ns == this.soapNsID || ns == this.soap12NsID || ns == this.schemaNs2000ID || ns == this.schemaNs1999ID;
		}

		private string ReadStringValue()
		{
			if (this.r.IsEmptyElement)
			{
				this.r.Skip();
				return string.Empty;
			}
			this.r.ReadStartElement();
			string text = this.r.ReadString();
			this.ReadEndElement();
			return text;
		}

		private XmlQualifiedName ReadXmlQualifiedName()
		{
			bool flag = false;
			string text;
			if (this.r.IsEmptyElement)
			{
				text = string.Empty;
				flag = true;
			}
			else
			{
				this.r.ReadStartElement();
				text = this.r.ReadString();
			}
			XmlQualifiedName xmlQualifiedName = this.ToXmlQualifiedName(text);
			if (flag)
			{
				this.r.Skip();
				return xmlQualifiedName;
			}
			this.ReadEndElement();
			return xmlQualifiedName;
		}

		private byte[] ReadByteArray(bool isBase64)
		{
			ArrayList arrayList = new ArrayList();
			int num = 1024;
			int num2 = -1;
			int num3 = 0;
			int num4 = 0;
			byte[] array = new byte[num];
			arrayList.Add(array);
			while (num2 != 0)
			{
				if (num3 == array.Length)
				{
					num = Math.Min(num * 2, 65536);
					array = new byte[num];
					num3 = 0;
					arrayList.Add(array);
				}
				if (isBase64)
				{
					num2 = this.r.ReadElementContentAsBase64(array, num3, array.Length - num3);
				}
				else
				{
					num2 = this.r.ReadElementContentAsBinHex(array, num3, array.Length - num3);
				}
				num3 += num2;
				num4 += num2;
			}
			byte[] array2 = new byte[num4];
			num3 = 0;
			foreach (object obj in arrayList)
			{
				byte[] array3 = (byte[])obj;
				num = Math.Min(array3.Length, num4);
				if (num > 0)
				{
					Buffer.BlockCopy(array3, 0, array2, num3, num);
					num3 += num;
					num4 -= num;
				}
			}
			arrayList.Clear();
			return array2;
		}

		protected object ReadTypedPrimitive(XmlQualifiedName type)
		{
			return this.ReadTypedPrimitive(type, false);
		}

		private object ReadTypedPrimitive(XmlQualifiedName type, bool elementCanBeType)
		{
			this.InitPrimitiveIDs();
			if (!this.IsPrimitiveNamespace(type.Namespace) || type.Name == this.urTypeID)
			{
				return this.ReadXmlNodes(elementCanBeType);
			}
			object obj;
			if (type.Namespace == this.schemaNsID || type.Namespace == this.soapNsID || type.Namespace == this.soap12NsID)
			{
				if (type.Name == this.stringID || type.Name == this.normalizedStringID)
				{
					obj = this.ReadStringValue();
				}
				else if (type.Name == this.anyURIID || type.Name == this.durationID || type.Name == this.ENTITYID || type.Name == this.ENTITIESID || type.Name == this.gDayID || type.Name == this.gMonthID || type.Name == this.gMonthDayID || type.Name == this.gYearID || type.Name == this.gYearMonthID || type.Name == this.IDID || type.Name == this.IDREFID || type.Name == this.IDREFSID || type.Name == this.integerID || type.Name == this.languageID || type.Name == this.NameID || type.Name == this.NCNameID || type.Name == this.NMTOKENID || type.Name == this.NMTOKENSID || type.Name == this.negativeIntegerID || type.Name == this.nonPositiveIntegerID || type.Name == this.nonNegativeIntegerID || type.Name == this.NOTATIONID || type.Name == this.positiveIntegerID || type.Name == this.tokenID)
				{
					obj = this.CollapseWhitespace(this.ReadStringValue());
				}
				else if (type.Name == this.intID)
				{
					obj = XmlConvert.ToInt32(this.ReadStringValue());
				}
				else if (type.Name == this.booleanID)
				{
					obj = XmlConvert.ToBoolean(this.ReadStringValue());
				}
				else if (type.Name == this.shortID)
				{
					obj = XmlConvert.ToInt16(this.ReadStringValue());
				}
				else if (type.Name == this.longID)
				{
					obj = XmlConvert.ToInt64(this.ReadStringValue());
				}
				else if (type.Name == this.floatID)
				{
					obj = XmlConvert.ToSingle(this.ReadStringValue());
				}
				else if (type.Name == this.doubleID)
				{
					obj = XmlConvert.ToDouble(this.ReadStringValue());
				}
				else if (type.Name == this.decimalID)
				{
					obj = XmlConvert.ToDecimal(this.ReadStringValue());
				}
				else if (type.Name == this.dateTimeID)
				{
					obj = XmlSerializationReader.ToDateTime(this.ReadStringValue());
				}
				else if (type.Name == this.qnameID)
				{
					obj = this.ReadXmlQualifiedName();
				}
				else if (type.Name == this.dateID)
				{
					obj = XmlSerializationReader.ToDate(this.ReadStringValue());
				}
				else if (type.Name == this.timeID)
				{
					obj = XmlSerializationReader.ToTime(this.ReadStringValue());
				}
				else if (type.Name == this.unsignedByteID)
				{
					obj = XmlConvert.ToByte(this.ReadStringValue());
				}
				else if (type.Name == this.byteID)
				{
					obj = XmlConvert.ToSByte(this.ReadStringValue());
				}
				else if (type.Name == this.unsignedShortID)
				{
					obj = XmlConvert.ToUInt16(this.ReadStringValue());
				}
				else if (type.Name == this.unsignedIntID)
				{
					obj = XmlConvert.ToUInt32(this.ReadStringValue());
				}
				else if (type.Name == this.unsignedLongID)
				{
					obj = XmlConvert.ToUInt64(this.ReadStringValue());
				}
				else if (type.Name == this.hexBinaryID)
				{
					obj = this.ToByteArrayHex(false);
				}
				else if (type.Name == this.base64BinaryID)
				{
					obj = this.ToByteArrayBase64(false);
				}
				else if (type.Name == this.base64ID && (type.Namespace == this.soapNsID || type.Namespace == this.soap12NsID))
				{
					obj = this.ToByteArrayBase64(false);
				}
				else
				{
					obj = this.ReadXmlNodes(elementCanBeType);
				}
			}
			else if (type.Namespace == this.schemaNs2000ID || type.Namespace == this.schemaNs1999ID)
			{
				if (type.Name == this.stringID || type.Name == this.normalizedStringID)
				{
					obj = this.ReadStringValue();
				}
				else if (type.Name == this.anyURIID || type.Name == this.anyURIID || type.Name == this.durationID || type.Name == this.ENTITYID || type.Name == this.ENTITIESID || type.Name == this.gDayID || type.Name == this.gMonthID || type.Name == this.gMonthDayID || type.Name == this.gYearID || type.Name == this.gYearMonthID || type.Name == this.IDID || type.Name == this.IDREFID || type.Name == this.IDREFSID || type.Name == this.integerID || type.Name == this.languageID || type.Name == this.NameID || type.Name == this.NCNameID || type.Name == this.NMTOKENID || type.Name == this.NMTOKENSID || type.Name == this.negativeIntegerID || type.Name == this.nonPositiveIntegerID || type.Name == this.nonNegativeIntegerID || type.Name == this.NOTATIONID || type.Name == this.positiveIntegerID || type.Name == this.tokenID)
				{
					obj = this.CollapseWhitespace(this.ReadStringValue());
				}
				else if (type.Name == this.intID)
				{
					obj = XmlConvert.ToInt32(this.ReadStringValue());
				}
				else if (type.Name == this.booleanID)
				{
					obj = XmlConvert.ToBoolean(this.ReadStringValue());
				}
				else if (type.Name == this.shortID)
				{
					obj = XmlConvert.ToInt16(this.ReadStringValue());
				}
				else if (type.Name == this.longID)
				{
					obj = XmlConvert.ToInt64(this.ReadStringValue());
				}
				else if (type.Name == this.floatID)
				{
					obj = XmlConvert.ToSingle(this.ReadStringValue());
				}
				else if (type.Name == this.doubleID)
				{
					obj = XmlConvert.ToDouble(this.ReadStringValue());
				}
				else if (type.Name == this.oldDecimalID)
				{
					obj = XmlConvert.ToDecimal(this.ReadStringValue());
				}
				else if (type.Name == this.oldTimeInstantID)
				{
					obj = XmlSerializationReader.ToDateTime(this.ReadStringValue());
				}
				else if (type.Name == this.qnameID)
				{
					obj = this.ReadXmlQualifiedName();
				}
				else if (type.Name == this.dateID)
				{
					obj = XmlSerializationReader.ToDate(this.ReadStringValue());
				}
				else if (type.Name == this.timeID)
				{
					obj = XmlSerializationReader.ToTime(this.ReadStringValue());
				}
				else if (type.Name == this.unsignedByteID)
				{
					obj = XmlConvert.ToByte(this.ReadStringValue());
				}
				else if (type.Name == this.byteID)
				{
					obj = XmlConvert.ToSByte(this.ReadStringValue());
				}
				else if (type.Name == this.unsignedShortID)
				{
					obj = XmlConvert.ToUInt16(this.ReadStringValue());
				}
				else if (type.Name == this.unsignedIntID)
				{
					obj = XmlConvert.ToUInt32(this.ReadStringValue());
				}
				else if (type.Name == this.unsignedLongID)
				{
					obj = XmlConvert.ToUInt64(this.ReadStringValue());
				}
				else
				{
					obj = this.ReadXmlNodes(elementCanBeType);
				}
			}
			else if (type.Namespace == this.schemaNonXsdTypesNsID)
			{
				if (type.Name == this.charID)
				{
					obj = XmlSerializationReader.ToChar(this.ReadStringValue());
				}
				else if (type.Name == this.guidID)
				{
					obj = new Guid(this.CollapseWhitespace(this.ReadStringValue()));
				}
				else if (type.Name == this.timeSpanID && LocalAppContextSwitches.EnableTimeSpanSerialization)
				{
					obj = XmlConvert.ToTimeSpan(this.ReadStringValue());
				}
				else
				{
					obj = this.ReadXmlNodes(elementCanBeType);
				}
			}
			else
			{
				obj = this.ReadXmlNodes(elementCanBeType);
			}
			return obj;
		}

		protected object ReadTypedNull(XmlQualifiedName type)
		{
			this.InitPrimitiveIDs();
			if (!this.IsPrimitiveNamespace(type.Namespace) || type.Name == this.urTypeID)
			{
				return null;
			}
			object obj;
			if (type.Namespace == this.schemaNsID || type.Namespace == this.soapNsID || type.Namespace == this.soap12NsID)
			{
				if (type.Name == this.stringID || type.Name == this.anyURIID || type.Name == this.durationID || type.Name == this.ENTITYID || type.Name == this.ENTITIESID || type.Name == this.gDayID || type.Name == this.gMonthID || type.Name == this.gMonthDayID || type.Name == this.gYearID || type.Name == this.gYearMonthID || type.Name == this.IDID || type.Name == this.IDREFID || type.Name == this.IDREFSID || type.Name == this.integerID || type.Name == this.languageID || type.Name == this.NameID || type.Name == this.NCNameID || type.Name == this.NMTOKENID || type.Name == this.NMTOKENSID || type.Name == this.negativeIntegerID || type.Name == this.nonPositiveIntegerID || type.Name == this.nonNegativeIntegerID || type.Name == this.normalizedStringID || type.Name == this.NOTATIONID || type.Name == this.positiveIntegerID || type.Name == this.tokenID)
				{
					obj = null;
				}
				else if (type.Name == this.intID)
				{
					obj = null;
				}
				else if (type.Name == this.booleanID)
				{
					obj = null;
				}
				else if (type.Name == this.shortID)
				{
					obj = null;
				}
				else if (type.Name == this.longID)
				{
					obj = null;
				}
				else if (type.Name == this.floatID)
				{
					obj = null;
				}
				else if (type.Name == this.doubleID)
				{
					obj = null;
				}
				else if (type.Name == this.decimalID)
				{
					obj = null;
				}
				else if (type.Name == this.dateTimeID)
				{
					obj = null;
				}
				else if (type.Name == this.qnameID)
				{
					obj = null;
				}
				else if (type.Name == this.dateID)
				{
					obj = null;
				}
				else if (type.Name == this.timeID)
				{
					obj = null;
				}
				else if (type.Name == this.unsignedByteID)
				{
					obj = null;
				}
				else if (type.Name == this.byteID)
				{
					obj = null;
				}
				else if (type.Name == this.unsignedShortID)
				{
					obj = null;
				}
				else if (type.Name == this.unsignedIntID)
				{
					obj = null;
				}
				else if (type.Name == this.unsignedLongID)
				{
					obj = null;
				}
				else if (type.Name == this.hexBinaryID)
				{
					obj = null;
				}
				else if (type.Name == this.base64BinaryID)
				{
					obj = null;
				}
				else if (type.Name == this.base64ID && (type.Namespace == this.soapNsID || type.Namespace == this.soap12NsID))
				{
					obj = null;
				}
				else
				{
					obj = null;
				}
			}
			else if (type.Namespace == this.schemaNonXsdTypesNsID)
			{
				if (type.Name == this.charID)
				{
					obj = null;
				}
				else if (type.Name == this.guidID)
				{
					obj = null;
				}
				else if (type.Name == this.timeSpanID && LocalAppContextSwitches.EnableTimeSpanSerialization)
				{
					obj = null;
				}
				else
				{
					obj = null;
				}
			}
			else
			{
				obj = null;
			}
			return obj;
		}

		protected bool IsXmlnsAttribute(string name)
		{
			return name.StartsWith("xmlns", StringComparison.Ordinal) && (name.Length == 5 || name[5] == ':');
		}

		protected void ParseWsdlArrayType(XmlAttribute attr)
		{
			if (attr.LocalName == this.wsdlArrayTypeID && attr.NamespaceURI == this.wsdlNsID)
			{
				int num = attr.Value.LastIndexOf(':');
				if (num < 0)
				{
					attr.Value = this.r.LookupNamespace("") + ":" + attr.Value;
					return;
				}
				attr.Value = this.r.LookupNamespace(attr.Value.Substring(0, num)) + ":" + attr.Value.Substring(num + 1);
			}
		}

		protected bool IsReturnValue
		{
			get
			{
				return this.isReturnValue && !this.soap12;
			}
			set
			{
				this.isReturnValue = value;
			}
		}

		protected bool ReadNull()
		{
			if (!this.GetNullAttr())
			{
				return false;
			}
			if (this.r.IsEmptyElement)
			{
				this.r.Skip();
				return true;
			}
			this.r.ReadStartElement();
			int num = 0;
			int readerCount = this.ReaderCount;
			while (this.r.NodeType != XmlNodeType.EndElement)
			{
				this.UnknownNode(null);
				this.CheckReaderCount(ref num, ref readerCount);
			}
			this.ReadEndElement();
			return true;
		}

		protected bool GetNullAttr()
		{
			string text = this.r.GetAttribute(this.nilID, this.instanceNsID);
			if (text == null)
			{
				text = this.r.GetAttribute(this.nullID, this.instanceNsID);
			}
			if (text == null)
			{
				text = this.r.GetAttribute(this.nullID, this.instanceNs2000ID);
				if (text == null)
				{
					text = this.r.GetAttribute(this.nullID, this.instanceNs1999ID);
				}
			}
			return text != null && XmlConvert.ToBoolean(text);
		}

		protected string ReadNullableString()
		{
			if (this.ReadNull())
			{
				return null;
			}
			return this.r.ReadElementString();
		}

		protected XmlQualifiedName ReadNullableQualifiedName()
		{
			if (this.ReadNull())
			{
				return null;
			}
			return this.ReadElementQualifiedName();
		}

		protected XmlQualifiedName ReadElementQualifiedName()
		{
			if (this.r.IsEmptyElement)
			{
				XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(string.Empty, this.r.LookupNamespace(""));
				this.r.Skip();
				return xmlQualifiedName;
			}
			XmlQualifiedName xmlQualifiedName2 = this.ToXmlQualifiedName(this.CollapseWhitespace(this.r.ReadString()));
			this.r.ReadEndElement();
			return xmlQualifiedName2;
		}

		protected XmlDocument ReadXmlDocument(bool wrapped)
		{
			XmlNode xmlNode = this.ReadXmlNode(wrapped);
			if (xmlNode == null)
			{
				return null;
			}
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.ImportNode(xmlNode, true));
			return xmlDocument;
		}

		protected string CollapseWhitespace(string value)
		{
			if (value == null)
			{
				return null;
			}
			return value.Trim();
		}

		protected XmlNode ReadXmlNode(bool wrapped)
		{
			XmlNode xmlNode = null;
			if (wrapped)
			{
				if (this.ReadNull())
				{
					return null;
				}
				this.r.ReadStartElement();
				this.r.MoveToContent();
				if (this.r.NodeType != XmlNodeType.EndElement)
				{
					xmlNode = this.Document.ReadNode(this.r);
				}
				int num = 0;
				int readerCount = this.ReaderCount;
				while (this.r.NodeType != XmlNodeType.EndElement)
				{
					this.UnknownNode(null);
					this.CheckReaderCount(ref num, ref readerCount);
				}
				this.r.ReadEndElement();
			}
			else
			{
				xmlNode = this.Document.ReadNode(this.r);
			}
			return xmlNode;
		}

		protected static byte[] ToByteArrayBase64(string value)
		{
			return XmlCustomFormatter.ToByteArrayBase64(value);
		}

		protected byte[] ToByteArrayBase64(bool isNull)
		{
			if (isNull)
			{
				return null;
			}
			return this.ReadByteArray(true);
		}

		protected static byte[] ToByteArrayHex(string value)
		{
			return XmlCustomFormatter.ToByteArrayHex(value);
		}

		protected byte[] ToByteArrayHex(bool isNull)
		{
			if (isNull)
			{
				return null;
			}
			return this.ReadByteArray(false);
		}

		protected int GetArrayLength(string name, string ns)
		{
			if (this.GetNullAttr())
			{
				return 0;
			}
			string attribute = this.r.GetAttribute(this.arrayTypeID, this.soapNsID);
			XmlSerializationReader.SoapArrayInfo soapArrayInfo = this.ParseArrayType(attribute);
			if (soapArrayInfo.dimensions != 1)
			{
				throw new InvalidOperationException(Res.GetString("SOAP-ENC:arrayType with multidimensional array found at {0}. Only single-dimensional arrays are supported. Consider using an array of arrays instead.", new object[] { this.CurrentTag() }));
			}
			XmlQualifiedName xmlQualifiedName = this.ToXmlQualifiedName(soapArrayInfo.qname, false);
			if (xmlQualifiedName.Name != name)
			{
				throw new InvalidOperationException(Res.GetString("The SOAP-ENC:arrayType references type is named '{0}'; a type named '{1}' was expected at {2}.", new object[]
				{
					xmlQualifiedName.Name,
					name,
					this.CurrentTag()
				}));
			}
			if (xmlQualifiedName.Namespace != ns)
			{
				throw new InvalidOperationException(Res.GetString("The SOAP-ENC:arrayType references type is from namespace '{0}'; the namespace '{1}' was expected at {2}.", new object[]
				{
					xmlQualifiedName.Namespace,
					ns,
					this.CurrentTag()
				}));
			}
			return soapArrayInfo.length;
		}

		private XmlSerializationReader.SoapArrayInfo ParseArrayType(string value)
		{
			if (value == null)
			{
				throw new ArgumentNullException(Res.GetString("SOAP-ENC:arrayType was missing at {0}.", new object[] { this.CurrentTag() }));
			}
			if (value.Length == 0)
			{
				throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType was empty at {0}.", new object[] { this.CurrentTag() }), "value");
			}
			char[] array = value.ToCharArray();
			int num = array.Length;
			XmlSerializationReader.SoapArrayInfo soapArrayInfo = default(XmlSerializationReader.SoapArrayInfo);
			int num2 = num - 1;
			if (array[num2] != ']')
			{
				throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType must end with a ']' character."), "value");
			}
			num2--;
			while (num2 != -1 && array[num2] != '[')
			{
				if (array[num2] == ',')
				{
					throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType with multidimensional array found at {0}. Only single-dimensional arrays are supported. Consider using an array of arrays instead.", new object[] { this.CurrentTag() }), "value");
				}
				num2--;
			}
			if (num2 == -1)
			{
				throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType has mismatched brackets."), "value");
			}
			int num3 = num - num2 - 2;
			if (num3 > 0)
			{
				string text = new string(array, num2 + 1, num3);
				try
				{
					soapArrayInfo.length = int.Parse(text, CultureInfo.InvariantCulture);
					goto IL_014F;
				}
				catch (Exception ex)
				{
					if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
					{
						throw;
					}
					throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType could not handle '{1}' as the length of the array.", new object[] { text }), "value");
				}
			}
			soapArrayInfo.length = -1;
			IL_014F:
			num2--;
			soapArrayInfo.jaggedDimensions = 0;
			while (num2 != -1 && array[num2] == ']')
			{
				num2--;
				if (num2 < 0)
				{
					throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType has mismatched brackets."), "value");
				}
				if (array[num2] == ',')
				{
					throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType with multidimensional array found at {0}. Only single-dimensional arrays are supported. Consider using an array of arrays instead.", new object[] { this.CurrentTag() }), "value");
				}
				if (array[num2] != '[')
				{
					throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType must end with a ']' character."), "value");
				}
				num2--;
				soapArrayInfo.jaggedDimensions++;
			}
			soapArrayInfo.dimensions = 1;
			soapArrayInfo.qname = new string(array, 0, num2 + 1);
			return soapArrayInfo;
		}

		private XmlSerializationReader.SoapArrayInfo ParseSoap12ArrayType(string itemType, string arraySize)
		{
			XmlSerializationReader.SoapArrayInfo soapArrayInfo = default(XmlSerializationReader.SoapArrayInfo);
			if (itemType != null && itemType.Length > 0)
			{
				soapArrayInfo.qname = itemType;
			}
			else
			{
				soapArrayInfo.qname = "";
			}
			string[] array;
			if (arraySize != null && arraySize.Length > 0)
			{
				array = arraySize.Split(null);
			}
			else
			{
				array = new string[0];
			}
			soapArrayInfo.dimensions = 0;
			soapArrayInfo.length = -1;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Length > 0)
				{
					if (array[i] == "*")
					{
						soapArrayInfo.dimensions++;
					}
					else
					{
						try
						{
							soapArrayInfo.length = int.Parse(array[i], CultureInfo.InvariantCulture);
							soapArrayInfo.dimensions++;
						}
						catch (Exception ex)
						{
							if (ex is ThreadAbortException || ex is StackOverflowException || ex is OutOfMemoryException)
							{
								throw;
							}
							throw new ArgumentException(Res.GetString("SOAP-ENC:arrayType could not handle '{1}' as the length of the array.", new object[] { array[i] }), "value");
						}
					}
				}
			}
			if (soapArrayInfo.dimensions == 0)
			{
				soapArrayInfo.dimensions = 1;
			}
			return soapArrayInfo;
		}

		protected static DateTime ToDateTime(string value)
		{
			return XmlCustomFormatter.ToDateTime(value);
		}

		protected static DateTime ToDate(string value)
		{
			return XmlCustomFormatter.ToDate(value);
		}

		protected static DateTime ToTime(string value)
		{
			return XmlCustomFormatter.ToTime(value);
		}

		protected static char ToChar(string value)
		{
			return XmlCustomFormatter.ToChar(value);
		}

		protected static long ToEnum(string value, Hashtable h, string typeName)
		{
			return XmlCustomFormatter.ToEnum(value, h, typeName, true);
		}

		protected static string ToXmlName(string value)
		{
			return XmlCustomFormatter.ToXmlName(value);
		}

		protected static string ToXmlNCName(string value)
		{
			return XmlCustomFormatter.ToXmlNCName(value);
		}

		protected static string ToXmlNmToken(string value)
		{
			return XmlCustomFormatter.ToXmlNmToken(value);
		}

		protected static string ToXmlNmTokens(string value)
		{
			return XmlCustomFormatter.ToXmlNmTokens(value);
		}

		protected XmlQualifiedName ToXmlQualifiedName(string value)
		{
			return this.ToXmlQualifiedName(value, this.DecodeName);
		}

		internal XmlQualifiedName ToXmlQualifiedName(string value, bool decodeName)
		{
			int num = ((value == null) ? (-1) : value.LastIndexOf(':'));
			string text = ((num < 0) ? null : value.Substring(0, num));
			string text2 = value.Substring(num + 1);
			if (decodeName)
			{
				text = XmlConvert.DecodeName(text);
				text2 = XmlConvert.DecodeName(text2);
			}
			if (text == null || text.Length == 0)
			{
				return new XmlQualifiedName(this.r.NameTable.Add(value), this.r.LookupNamespace(string.Empty));
			}
			string text3 = this.r.LookupNamespace(text);
			if (text3 == null)
			{
				throw new InvalidOperationException(Res.GetString("Namespace prefix '{0}' is not defined.", new object[] { text }));
			}
			return new XmlQualifiedName(this.r.NameTable.Add(text2), text3);
		}

		protected void UnknownAttribute(object o, XmlAttribute attr)
		{
			this.UnknownAttribute(o, attr, null);
		}

		protected void UnknownAttribute(object o, XmlAttribute attr, string qnames)
		{
			if (this.events.OnUnknownAttribute != null)
			{
				int num;
				int num2;
				this.GetCurrentPosition(out num, out num2);
				XmlAttributeEventArgs e = new XmlAttributeEventArgs(attr, num, num2, o, qnames);
				this.events.OnUnknownAttribute(this.events.sender, e);
			}
		}

		protected void UnknownElement(object o, XmlElement elem)
		{
			this.UnknownElement(o, elem, null);
		}

		protected void UnknownElement(object o, XmlElement elem, string qnames)
		{
			if (this.events.OnUnknownElement != null)
			{
				int num;
				int num2;
				this.GetCurrentPosition(out num, out num2);
				XmlElementEventArgs e = new XmlElementEventArgs(elem, num, num2, o, qnames);
				this.events.OnUnknownElement(this.events.sender, e);
			}
		}

		protected void UnknownNode(object o)
		{
			this.UnknownNode(o, null);
		}

		protected void UnknownNode(object o, string qnames)
		{
			if (this.r.NodeType == XmlNodeType.None || this.r.NodeType == XmlNodeType.Whitespace)
			{
				this.r.Read();
				return;
			}
			if (this.r.NodeType == XmlNodeType.EndElement)
			{
				return;
			}
			if (this.events.OnUnknownNode != null)
			{
				this.UnknownNode(this.Document.ReadNode(this.r), o, qnames);
				return;
			}
			if (this.r.NodeType == XmlNodeType.Attribute && this.events.OnUnknownAttribute == null)
			{
				return;
			}
			if (this.r.NodeType == XmlNodeType.Element && this.events.OnUnknownElement == null)
			{
				this.r.Skip();
				return;
			}
			this.UnknownNode(this.Document.ReadNode(this.r), o, qnames);
		}

		private void UnknownNode(XmlNode unknownNode, object o, string qnames)
		{
			if (unknownNode == null)
			{
				return;
			}
			if (unknownNode.NodeType != XmlNodeType.None && unknownNode.NodeType != XmlNodeType.Whitespace && this.events.OnUnknownNode != null)
			{
				int num;
				int num2;
				this.GetCurrentPosition(out num, out num2);
				XmlNodeEventArgs e = new XmlNodeEventArgs(unknownNode, num, num2, o);
				this.events.OnUnknownNode(this.events.sender, e);
			}
			if (unknownNode.NodeType == XmlNodeType.Attribute)
			{
				this.UnknownAttribute(o, (XmlAttribute)unknownNode, qnames);
				return;
			}
			if (unknownNode.NodeType == XmlNodeType.Element)
			{
				this.UnknownElement(o, (XmlElement)unknownNode, qnames);
			}
		}

		private void GetCurrentPosition(out int lineNumber, out int linePosition)
		{
			if (this.Reader is IXmlLineInfo)
			{
				IXmlLineInfo xmlLineInfo = (IXmlLineInfo)this.Reader;
				lineNumber = xmlLineInfo.LineNumber;
				linePosition = xmlLineInfo.LinePosition;
				return;
			}
			lineNumber = (linePosition = -1);
		}

		protected void UnreferencedObject(string id, object o)
		{
			if (this.events.OnUnreferencedObject != null)
			{
				UnreferencedObjectEventArgs e = new UnreferencedObjectEventArgs(o, id);
				this.events.OnUnreferencedObject(this.events.sender, e);
			}
		}

		private string CurrentTag()
		{
			XmlNodeType nodeType = this.r.NodeType;
			switch (nodeType)
			{
			case XmlNodeType.Element:
				return string.Concat(new string[]
				{
					"<",
					this.r.LocalName,
					" xmlns='",
					this.r.NamespaceURI,
					"'>"
				});
			case XmlNodeType.Attribute:
			case XmlNodeType.EntityReference:
			case XmlNodeType.Entity:
				break;
			case XmlNodeType.Text:
				return this.r.Value;
			case XmlNodeType.CDATA:
				return "CDATA";
			case XmlNodeType.ProcessingInstruction:
				return "<?";
			case XmlNodeType.Comment:
				return "<--";
			default:
				if (nodeType == XmlNodeType.EndElement)
				{
					return ">";
				}
				break;
			}
			return "(unknown)";
		}

		protected Exception CreateUnknownTypeException(XmlQualifiedName type)
		{
			return new InvalidOperationException(Res.GetString("The specified type was not recognized: name='{0}', namespace='{1}', at {2}.", new object[]
			{
				type.Name,
				type.Namespace,
				this.CurrentTag()
			}));
		}

		protected Exception CreateReadOnlyCollectionException(string name)
		{
			return new InvalidOperationException(Res.GetString("Could not deserialize {0}. Parameterless constructor is required for collections and enumerators.", new object[] { name }));
		}

		protected Exception CreateAbstractTypeException(string name, string ns)
		{
			return new InvalidOperationException(Res.GetString("The specified type is abstract: name='{0}', namespace='{1}', at {2}.", new object[]
			{
				name,
				ns,
				this.CurrentTag()
			}));
		}

		protected Exception CreateInaccessibleConstructorException(string typeName)
		{
			return new InvalidOperationException(Res.GetString("{0} cannot be serialized because it does not have a parameterless constructor.", new object[] { typeName }));
		}

		protected Exception CreateCtorHasSecurityException(string typeName)
		{
			return new InvalidOperationException(Res.GetString("The type '{0}' cannot be serialized because its parameterless constructor is decorated with declarative security permission attributes. Consider using imperative asserts or demands in the constructor.", new object[] { typeName }));
		}

		protected Exception CreateUnknownNodeException()
		{
			return new InvalidOperationException(Res.GetString("{0} was not expected.", new object[] { this.CurrentTag() }));
		}

		protected Exception CreateUnknownConstantException(string value, Type enumType)
		{
			return new InvalidOperationException(Res.GetString("Instance validation error: '{0}' is not a valid value for {1}.", new object[] { value, enumType.Name }));
		}

		protected Exception CreateInvalidCastException(Type type, object value)
		{
			return this.CreateInvalidCastException(type, value, null);
		}

		protected Exception CreateInvalidCastException(Type type, object value, string id)
		{
			if (value == null)
			{
				return new InvalidCastException(Res.GetString("Cannot assign null value to an object of type {1}.", new object[] { type.FullName }));
			}
			if (id == null)
			{
				return new InvalidCastException(Res.GetString("Cannot assign object of type {0} to an object of type {1}.", new object[]
				{
					value.GetType().FullName,
					type.FullName
				}));
			}
			return new InvalidCastException(Res.GetString("Cannot assign object of type {0} to an object of type {1}. The error occurred while reading node with id='{2}'.", new object[]
			{
				value.GetType().FullName,
				type.FullName,
				id
			}));
		}

		protected Exception CreateBadDerivationException(string xsdDerived, string nsDerived, string xsdBase, string nsBase, string clrDerived, string clrBase)
		{
			return new InvalidOperationException(Res.GetString("Type '{0}' from namespace '{1}' declared as derivation of type '{2}' from namespace '{3}, but corresponding CLR types are not compatible.  Cannot convert type '{4}' to '{5}'.", new object[] { xsdDerived, nsDerived, xsdBase, nsBase, clrDerived, clrBase }));
		}

		protected Exception CreateMissingIXmlSerializableType(string name, string ns, string clrType)
		{
			return new InvalidOperationException(Res.GetString("Type '{0}' from namespace '{1}' does not have corresponding IXmlSerializable type. Please consider adding {2} to '{3}'.", new object[]
			{
				name,
				ns,
				typeof(XmlIncludeAttribute).Name,
				clrType
			}));
		}

		protected Array EnsureArrayIndex(Array a, int index, Type elementType)
		{
			if (a == null)
			{
				return Array.CreateInstance(elementType, 32);
			}
			if (index < a.Length)
			{
				return a;
			}
			Array array = Array.CreateInstance(elementType, a.Length * 2);
			Array.Copy(a, array, index);
			return array;
		}

		protected Array ShrinkArray(Array a, int length, Type elementType, bool isNullable)
		{
			if (a == null)
			{
				if (isNullable)
				{
					return null;
				}
				return Array.CreateInstance(elementType, 0);
			}
			else
			{
				if (a.Length == length)
				{
					return a;
				}
				Array array = Array.CreateInstance(elementType, length);
				Array.Copy(a, array, length);
				return array;
			}
		}

		protected string ReadString(string value)
		{
			return this.ReadString(value, false);
		}

		protected string ReadString(string value, bool trim)
		{
			string text = this.r.ReadString();
			if (text != null && trim)
			{
				text = text.Trim();
			}
			if (value == null || value.Length == 0)
			{
				return text;
			}
			return value + text;
		}

		protected IXmlSerializable ReadSerializable(IXmlSerializable serializable)
		{
			return this.ReadSerializable(serializable, false);
		}

		protected IXmlSerializable ReadSerializable(IXmlSerializable serializable, bool wrappedAny)
		{
			string text = null;
			string text2 = null;
			if (wrappedAny)
			{
				text = this.r.LocalName;
				text2 = this.r.NamespaceURI;
				this.r.Read();
				this.r.MoveToContent();
			}
			serializable.ReadXml(this.r);
			if (wrappedAny)
			{
				while (this.r.NodeType == XmlNodeType.Whitespace)
				{
					this.r.Skip();
				}
				if (this.r.NodeType == XmlNodeType.None)
				{
					this.r.Skip();
				}
				if (this.r.NodeType == XmlNodeType.EndElement && this.r.LocalName == text && this.r.NamespaceURI == text2)
				{
					this.Reader.Read();
				}
			}
			return serializable;
		}

		protected bool ReadReference(out string fixupReference)
		{
			string text = (this.soap12 ? this.r.GetAttribute("ref", "http://www.w3.org/2003/05/soap-encoding") : this.r.GetAttribute("href"));
			if (text == null)
			{
				fixupReference = null;
				return false;
			}
			if (!this.soap12)
			{
				if (!text.StartsWith("#", StringComparison.Ordinal))
				{
					throw new InvalidOperationException(Res.GetString("The referenced element with ID '{0}' is located outside the current document and cannot be retrieved.", new object[] { text }));
				}
				fixupReference = text.Substring(1);
			}
			else
			{
				fixupReference = text;
			}
			if (this.r.IsEmptyElement)
			{
				this.r.Skip();
			}
			else
			{
				this.r.ReadStartElement();
				this.ReadEndElement();
			}
			return true;
		}

		protected void AddTarget(string id, object o)
		{
			if (id == null)
			{
				if (this.targetsWithoutIds == null)
				{
					this.targetsWithoutIds = new ArrayList();
				}
				if (o != null)
				{
					this.targetsWithoutIds.Add(o);
					return;
				}
			}
			else
			{
				if (this.targets == null)
				{
					this.targets = new Hashtable();
				}
				if (!this.targets.Contains(id))
				{
					this.targets.Add(id, o);
				}
			}
		}

		protected void AddFixup(XmlSerializationReader.Fixup fixup)
		{
			if (this.fixups == null)
			{
				this.fixups = new ArrayList();
			}
			this.fixups.Add(fixup);
		}

		protected void AddFixup(XmlSerializationReader.CollectionFixup fixup)
		{
			if (this.collectionFixups == null)
			{
				this.collectionFixups = new ArrayList();
			}
			this.collectionFixups.Add(fixup);
		}

		protected object GetTarget(string id)
		{
			object obj = ((this.targets != null) ? this.targets[id] : null);
			if (obj == null)
			{
				throw new InvalidOperationException(Res.GetString("The referenced element with ID '{0}' was not found in the document.", new object[] { id }));
			}
			this.Referenced(obj);
			return obj;
		}

		protected void Referenced(object o)
		{
			if (o == null)
			{
				return;
			}
			if (this.referencedTargets == null)
			{
				this.referencedTargets = new Hashtable();
			}
			this.referencedTargets[o] = o;
		}

		private void HandleUnreferencedObjects()
		{
			if (this.targets != null)
			{
				foreach (object obj in this.targets)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					if (this.referencedTargets == null || !this.referencedTargets.Contains(dictionaryEntry.Value))
					{
						this.UnreferencedObject((string)dictionaryEntry.Key, dictionaryEntry.Value);
					}
				}
			}
			if (this.targetsWithoutIds != null)
			{
				foreach (object obj2 in this.targetsWithoutIds)
				{
					if (this.referencedTargets == null || !this.referencedTargets.Contains(obj2))
					{
						this.UnreferencedObject(null, obj2);
					}
				}
			}
		}

		private void DoFixups()
		{
			if (this.fixups == null)
			{
				return;
			}
			for (int i = 0; i < this.fixups.Count; i++)
			{
				XmlSerializationReader.Fixup fixup = (XmlSerializationReader.Fixup)this.fixups[i];
				fixup.Callback(fixup);
			}
			if (this.collectionFixups == null)
			{
				return;
			}
			for (int j = 0; j < this.collectionFixups.Count; j++)
			{
				XmlSerializationReader.CollectionFixup collectionFixup = (XmlSerializationReader.CollectionFixup)this.collectionFixups[j];
				collectionFixup.Callback(collectionFixup.Collection, collectionFixup.CollectionItems);
			}
		}

		protected void FixupArrayRefs(object fixup)
		{
			XmlSerializationReader.Fixup fixup2 = (XmlSerializationReader.Fixup)fixup;
			Array array = (Array)fixup2.Source;
			for (int i = 0; i < array.Length; i++)
			{
				string text = fixup2.Ids[i];
				if (text != null)
				{
					object target = this.GetTarget(text);
					try
					{
						array.SetValue(target, i);
					}
					catch (InvalidCastException)
					{
						throw new InvalidOperationException(Res.GetString("Invalid reference id='{0}'. Object of type {1} cannot be stored in an array of this type. Details: array index={2}.", new object[]
						{
							text,
							target.GetType().FullName,
							i.ToString(CultureInfo.InvariantCulture)
						}));
					}
				}
			}
		}

		private object ReadArray(string typeName, string typeNs)
		{
			Type type = null;
			XmlSerializationReader.SoapArrayInfo soapArrayInfo;
			if (this.soap12)
			{
				string attribute = this.r.GetAttribute(this.itemTypeID, this.soap12NsID);
				string attribute2 = this.r.GetAttribute(this.arraySizeID, this.soap12NsID);
				Type type2 = (Type)this.types[new XmlQualifiedName(typeName, typeNs)];
				if (attribute == null && attribute2 == null && (type2 == null || !type2.IsArray))
				{
					return null;
				}
				soapArrayInfo = this.ParseSoap12ArrayType(attribute, attribute2);
				if (type2 != null)
				{
					type = TypeScope.GetArrayElementType(type2, null);
				}
			}
			else
			{
				string attribute3 = this.r.GetAttribute(this.arrayTypeID, this.soapNsID);
				if (attribute3 == null)
				{
					return null;
				}
				soapArrayInfo = this.ParseArrayType(attribute3);
			}
			if (soapArrayInfo.dimensions != 1)
			{
				throw new InvalidOperationException(Res.GetString("SOAP-ENC:arrayType with multidimensional array found at {0}. Only single-dimensional arrays are supported. Consider using an array of arrays instead.", new object[] { this.CurrentTag() }));
			}
			Type type3 = null;
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(this.urTypeID, this.schemaNsID);
			XmlQualifiedName xmlQualifiedName2;
			if (soapArrayInfo.qname.Length > 0)
			{
				xmlQualifiedName2 = this.ToXmlQualifiedName(soapArrayInfo.qname, false);
				type3 = (Type)this.types[xmlQualifiedName2];
			}
			else
			{
				xmlQualifiedName2 = xmlQualifiedName;
			}
			if (this.soap12 && type3 == typeof(object))
			{
				type3 = null;
			}
			bool flag;
			if (type3 == null)
			{
				if (!this.soap12)
				{
					type3 = this.GetPrimitiveType(xmlQualifiedName2, true);
					flag = true;
				}
				else
				{
					if (xmlQualifiedName2 != xmlQualifiedName)
					{
						type3 = this.GetPrimitiveType(xmlQualifiedName2, false);
					}
					if (type3 != null)
					{
						flag = true;
					}
					else if (type == null)
					{
						type3 = typeof(object);
						flag = false;
					}
					else
					{
						type3 = type;
						XmlQualifiedName xmlQualifiedName3 = (XmlQualifiedName)this.typesReverse[type3];
						if (xmlQualifiedName3 == null)
						{
							xmlQualifiedName3 = XmlSerializationWriter.GetPrimitiveTypeNameInternal(type3);
							flag = true;
						}
						else
						{
							flag = type3.IsPrimitive;
						}
						if (xmlQualifiedName3 != null)
						{
							xmlQualifiedName2 = xmlQualifiedName3;
						}
					}
				}
			}
			else
			{
				flag = type3.IsPrimitive;
			}
			if (!this.soap12 && soapArrayInfo.jaggedDimensions > 0)
			{
				for (int i = 0; i < soapArrayInfo.jaggedDimensions; i++)
				{
					type3 = type3.MakeArrayType();
				}
			}
			if (this.r.IsEmptyElement)
			{
				this.r.Skip();
				return Array.CreateInstance(type3, 0);
			}
			this.r.ReadStartElement();
			this.r.MoveToContent();
			int num = 0;
			Array array = null;
			if (type3.IsValueType)
			{
				if (!flag && !type3.IsEnum)
				{
					throw new NotSupportedException(Res.GetString("Cannot serialize {0}. Arrays of structs are not supported with encoded SOAP.", new object[] { type3.FullName }));
				}
				int num2 = 0;
				int readerCount = this.ReaderCount;
				while (this.r.NodeType != XmlNodeType.EndElement)
				{
					array = this.EnsureArrayIndex(array, num, type3);
					array.SetValue(this.ReadReferencedElement(xmlQualifiedName2.Name, xmlQualifiedName2.Namespace), num);
					num++;
					this.r.MoveToContent();
					this.CheckReaderCount(ref num2, ref readerCount);
				}
				array = this.ShrinkArray(array, num, type3, false);
			}
			else
			{
				string[] array2 = null;
				int num3 = 0;
				int num4 = 0;
				int readerCount2 = this.ReaderCount;
				while (this.r.NodeType != XmlNodeType.EndElement)
				{
					array = this.EnsureArrayIndex(array, num, type3);
					array2 = (string[])this.EnsureArrayIndex(array2, num3, typeof(string));
					string text;
					string text2;
					if (this.r.NamespaceURI.Length != 0)
					{
						text = this.r.LocalName;
						if (this.r.NamespaceURI == this.soapNsID)
						{
							text2 = "http://www.w3.org/2001/XMLSchema";
						}
						else
						{
							text2 = this.r.NamespaceURI;
						}
					}
					else
					{
						text = xmlQualifiedName2.Name;
						text2 = xmlQualifiedName2.Namespace;
					}
					array.SetValue(this.ReadReferencingElement(text, text2, out array2[num3]), num);
					num++;
					num3++;
					this.r.MoveToContent();
					this.CheckReaderCount(ref num4, ref readerCount2);
				}
				if (this.soap12 && type3 == typeof(object))
				{
					Type type4 = null;
					for (int j = 0; j < num; j++)
					{
						object value = array.GetValue(j);
						if (value != null)
						{
							Type type5 = value.GetType();
							if (type5.IsValueType)
							{
								type4 = null;
								break;
							}
							if (type4 == null || type5.IsAssignableFrom(type4))
							{
								type4 = type5;
							}
							else if (!type4.IsAssignableFrom(type5))
							{
								type4 = null;
								break;
							}
						}
					}
					if (type4 != null)
					{
						type3 = type4;
					}
				}
				array2 = (string[])this.ShrinkArray(array2, num3, typeof(string), false);
				array = this.ShrinkArray(array, num, type3, false);
				XmlSerializationReader.Fixup fixup = new XmlSerializationReader.Fixup(array, new XmlSerializationFixupCallback(this.FixupArrayRefs), array2);
				this.AddFixup(fixup);
			}
			this.ReadEndElement();
			return array;
		}

		protected abstract void InitCallbacks();

		protected void ReadReferencedElements()
		{
			this.r.MoveToContent();
			int num = 0;
			int readerCount = this.ReaderCount;
			while (this.r.NodeType != XmlNodeType.EndElement && this.r.NodeType != XmlNodeType.None)
			{
				string text;
				this.ReadReferencingElement(null, null, true, out text);
				this.r.MoveToContent();
				this.CheckReaderCount(ref num, ref readerCount);
			}
			this.DoFixups();
			this.HandleUnreferencedObjects();
		}

		protected object ReadReferencedElement()
		{
			return this.ReadReferencedElement(null, null);
		}

		protected object ReadReferencedElement(string name, string ns)
		{
			string text;
			return this.ReadReferencingElement(name, ns, out text);
		}

		protected object ReadReferencingElement(out string fixupReference)
		{
			return this.ReadReferencingElement(null, null, out fixupReference);
		}

		protected object ReadReferencingElement(string name, string ns, out string fixupReference)
		{
			return this.ReadReferencingElement(name, ns, false, out fixupReference);
		}

		protected object ReadReferencingElement(string name, string ns, bool elementCanBeType, out string fixupReference)
		{
			if (this.callbacks == null)
			{
				this.callbacks = new Hashtable();
				this.types = new Hashtable();
				XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(this.urTypeID, this.r.NameTable.Add("http://www.w3.org/2001/XMLSchema"));
				this.types.Add(xmlQualifiedName, typeof(object));
				this.typesReverse = new Hashtable();
				this.typesReverse.Add(typeof(object), xmlQualifiedName);
				this.InitCallbacks();
			}
			this.r.MoveToContent();
			if (this.ReadReference(out fixupReference))
			{
				return null;
			}
			if (this.ReadNull())
			{
				return null;
			}
			string text = (this.soap12 ? this.r.GetAttribute("id", "http://www.w3.org/2003/05/soap-encoding") : this.r.GetAttribute("id", null));
			object obj;
			if ((obj = this.ReadArray(name, ns)) == null)
			{
				XmlQualifiedName xmlQualifiedName2 = this.GetXsiType();
				if (xmlQualifiedName2 == null)
				{
					if (name == null)
					{
						xmlQualifiedName2 = new XmlQualifiedName(this.r.NameTable.Add(this.r.LocalName), this.r.NameTable.Add(this.r.NamespaceURI));
					}
					else
					{
						xmlQualifiedName2 = new XmlQualifiedName(this.r.NameTable.Add(name), this.r.NameTable.Add(ns));
					}
				}
				XmlSerializationReadCallback xmlSerializationReadCallback = (XmlSerializationReadCallback)this.callbacks[xmlQualifiedName2];
				if (xmlSerializationReadCallback != null)
				{
					obj = xmlSerializationReadCallback();
				}
				else
				{
					obj = this.ReadTypedPrimitive(xmlQualifiedName2, elementCanBeType);
				}
			}
			this.AddTarget(text, obj);
			return obj;
		}

		protected void AddReadCallback(string name, string ns, Type type, XmlSerializationReadCallback read)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(this.r.NameTable.Add(name), this.r.NameTable.Add(ns));
			this.callbacks[xmlQualifiedName] = read;
			this.types[xmlQualifiedName] = type;
			this.typesReverse[type] = xmlQualifiedName;
		}

		protected void ReadEndElement()
		{
			while (this.r.NodeType == XmlNodeType.Whitespace)
			{
				this.r.Skip();
			}
			if (this.r.NodeType == XmlNodeType.None)
			{
				this.r.Skip();
				return;
			}
			this.r.ReadEndElement();
		}

		private object ReadXmlNodes(bool elementCanBeType)
		{
			ArrayList arrayList = new ArrayList();
			string localName = this.Reader.LocalName;
			string namespaceURI = this.Reader.NamespaceURI;
			string name = this.Reader.Name;
			string text = null;
			string text2 = null;
			int num = 0;
			int num2 = -1;
			int num3 = -1;
			XmlNode xmlNode;
			if (this.Reader.NodeType == XmlNodeType.Attribute)
			{
				XmlAttribute xmlAttribute = this.Document.CreateAttribute(name, namespaceURI);
				xmlAttribute.Value = this.Reader.Value;
				xmlNode = xmlAttribute;
			}
			else
			{
				xmlNode = this.Document.CreateElement(name, namespaceURI);
			}
			this.GetCurrentPosition(out num2, out num3);
			XmlElement xmlElement = xmlNode as XmlElement;
			while (this.Reader.MoveToNextAttribute())
			{
				if (this.IsXmlnsAttribute(this.Reader.Name) || (this.Reader.Name == "id" && (!this.soap12 || this.Reader.NamespaceURI == "http://www.w3.org/2003/05/soap-encoding")))
				{
					num++;
				}
				if (this.Reader.LocalName == this.typeID && (this.Reader.NamespaceURI == this.instanceNsID || this.Reader.NamespaceURI == this.instanceNs2000ID || this.Reader.NamespaceURI == this.instanceNs1999ID))
				{
					string value = this.Reader.Value;
					int num4 = value.LastIndexOf(':');
					text = ((num4 >= 0) ? value.Substring(num4 + 1) : value);
					text2 = this.Reader.LookupNamespace((num4 >= 0) ? value.Substring(0, num4) : "");
				}
				XmlAttribute xmlAttribute2 = (XmlAttribute)this.Document.ReadNode(this.r);
				arrayList.Add(xmlAttribute2);
				if (xmlElement != null)
				{
					xmlElement.SetAttributeNode(xmlAttribute2);
				}
			}
			if (elementCanBeType && text == null)
			{
				text = localName;
				text2 = namespaceURI;
				XmlAttribute xmlAttribute3 = this.Document.CreateAttribute(this.typeID, this.instanceNsID);
				xmlAttribute3.Value = name;
				arrayList.Add(xmlAttribute3);
			}
			if (text == "anyType" && (text2 == this.schemaNsID || text2 == this.schemaNs1999ID || text2 == this.schemaNs2000ID))
			{
				num++;
			}
			this.Reader.MoveToElement();
			if (this.Reader.IsEmptyElement)
			{
				this.Reader.Skip();
			}
			else
			{
				this.Reader.ReadStartElement();
				this.Reader.MoveToContent();
				int num5 = 0;
				int readerCount = this.ReaderCount;
				while (this.Reader.NodeType != XmlNodeType.EndElement)
				{
					XmlNode xmlNode2 = this.Document.ReadNode(this.r);
					arrayList.Add(xmlNode2);
					if (xmlElement != null)
					{
						xmlElement.AppendChild(xmlNode2);
					}
					this.Reader.MoveToContent();
					this.CheckReaderCount(ref num5, ref readerCount);
				}
				this.ReadEndElement();
			}
			if (arrayList.Count <= num)
			{
				return new object();
			}
			object obj = (XmlNode[])arrayList.ToArray(typeof(XmlNode));
			this.UnknownNode(xmlNode, null, null);
			return obj;
		}

		protected void CheckReaderCount(ref int whileIterations, ref int readerCount)
		{
			if (XmlSerializationReader.checkDeserializeAdvances)
			{
				whileIterations++;
				if ((whileIterations & 128) == 128)
				{
					if (readerCount == this.ReaderCount)
					{
						throw new InvalidOperationException(Res.GetString("Internal error: deserialization failed to advance over underlying stream."));
					}
					readerCount = this.ReaderCount;
				}
			}
		}

		private XmlReader r;

		private XmlCountingReader countingReader;

		private XmlDocument d;

		private Hashtable callbacks;

		private Hashtable types;

		private Hashtable typesReverse;

		private XmlDeserializationEvents events;

		private Hashtable targets;

		private Hashtable referencedTargets;

		private ArrayList targetsWithoutIds;

		private ArrayList fixups;

		private ArrayList collectionFixups;

		private bool soap12;

		private bool isReturnValue;

		private bool decodeName = true;

		private string schemaNsID;

		private string schemaNs1999ID;

		private string schemaNs2000ID;

		private string schemaNonXsdTypesNsID;

		private string instanceNsID;

		private string instanceNs2000ID;

		private string instanceNs1999ID;

		private string soapNsID;

		private string soap12NsID;

		private string schemaID;

		private string wsdlNsID;

		private string wsdlArrayTypeID;

		private string nullID;

		private string nilID;

		private string typeID;

		private string arrayTypeID;

		private string itemTypeID;

		private string arraySizeID;

		private string arrayID;

		private string urTypeID;

		private string stringID;

		private string intID;

		private string booleanID;

		private string shortID;

		private string longID;

		private string floatID;

		private string doubleID;

		private string decimalID;

		private string dateTimeID;

		private string qnameID;

		private string dateID;

		private string timeID;

		private string hexBinaryID;

		private string base64BinaryID;

		private string base64ID;

		private string unsignedByteID;

		private string byteID;

		private string unsignedShortID;

		private string unsignedIntID;

		private string unsignedLongID;

		private string oldDecimalID;

		private string oldTimeInstantID;

		private string anyURIID;

		private string durationID;

		private string ENTITYID;

		private string ENTITIESID;

		private string gDayID;

		private string gMonthID;

		private string gMonthDayID;

		private string gYearID;

		private string gYearMonthID;

		private string IDID;

		private string IDREFID;

		private string IDREFSID;

		private string integerID;

		private string languageID;

		private string NameID;

		private string NCNameID;

		private string NMTOKENID;

		private string NMTOKENSID;

		private string negativeIntegerID;

		private string nonPositiveIntegerID;

		private string nonNegativeIntegerID;

		private string normalizedStringID;

		private string NOTATIONID;

		private string positiveIntegerID;

		private string tokenID;

		private string charID;

		private string guidID;

		private string timeSpanID;

		private static bool checkDeserializeAdvances;

		private struct SoapArrayInfo
		{
			public string qname;

			public int dimensions;

			public int length;

			public int jaggedDimensions;
		}

		protected class Fixup
		{
			public Fixup(object o, XmlSerializationFixupCallback callback, int count)
				: this(o, callback, new string[count])
			{
			}

			public Fixup(object o, XmlSerializationFixupCallback callback, string[] ids)
			{
				this.callback = callback;
				this.Source = o;
				this.ids = ids;
			}

			public XmlSerializationFixupCallback Callback
			{
				get
				{
					return this.callback;
				}
			}

			public object Source
			{
				get
				{
					return this.source;
				}
				set
				{
					this.source = value;
				}
			}

			public string[] Ids
			{
				get
				{
					return this.ids;
				}
			}

			private XmlSerializationFixupCallback callback;

			private object source;

			private string[] ids;
		}

		protected class CollectionFixup
		{
			public CollectionFixup(object collection, XmlSerializationCollectionFixupCallback callback, object collectionItems)
			{
				this.callback = callback;
				this.collection = collection;
				this.collectionItems = collectionItems;
			}

			public XmlSerializationCollectionFixupCallback Callback
			{
				get
				{
					return this.callback;
				}
			}

			public object Collection
			{
				get
				{
					return this.collection;
				}
			}

			public object CollectionItems
			{
				get
				{
					return this.collectionItems;
				}
			}

			private XmlSerializationCollectionFixupCallback callback;

			private object collection;

			private object collectionItems;
		}
	}
}
