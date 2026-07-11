using System;
using System.Collections.Generic;
using System.Text;
using Mono.Xml.Schema;

namespace System.Xml.Schema
{
	public abstract class XmlSchemaDatatype
	{
		internal virtual XsdWhitespaceFacet Whitespace
		{
			get
			{
				return this.WhitespaceValue;
			}
		}

		public virtual XmlTypeCode TypeCode
		{
			get
			{
				return XmlTypeCode.None;
			}
		}

		public virtual XmlSchemaDatatypeVariety Variety
		{
			get
			{
				return XmlSchemaDatatypeVariety.Atomic;
			}
		}

		public abstract XmlTokenizedType TokenizedType { get; }

		public abstract Type ValueType { get; }

		[MonoTODO]
		public virtual object ChangeType(object value, Type targetType)
		{
			return this.ChangeType(value, targetType, null);
		}

		[MonoTODO]
		public virtual object ChangeType(object value, Type targetType, IXmlNamespaceResolver nsResolver)
		{
			throw new NotImplementedException();
		}

		public virtual bool IsDerivedFrom(XmlSchemaDatatype datatype)
		{
			return this == datatype;
		}

		public abstract object ParseValue(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr);

		internal virtual ValueType ParseValueType(string s, XmlNameTable nameTable, IXmlNamespaceResolver nsmgr)
		{
			return null;
		}

		internal string Normalize(string s)
		{
			return this.Normalize(s, this.Whitespace);
		}

		internal string Normalize(string s, XsdWhitespaceFacet whitespaceFacet)
		{
			int num = s.IndexOfAny(XmlSchemaDatatype.wsChars);
			if (num < 0)
			{
				return s;
			}
			string text;
			if (whitespaceFacet == XsdWhitespaceFacet.Replace)
			{
				this.sb.Length = 0;
				this.sb.Append(s);
				for (int i = 0; i < this.sb.Length; i++)
				{
					switch (this.sb[i])
					{
					case '\t':
					case '\n':
					case '\r':
						this.sb[i] = ' ';
						break;
					}
				}
				text = this.sb.ToString();
				this.sb.Length = 0;
				return text;
			}
			if (whitespaceFacet != XsdWhitespaceFacet.Collapse)
			{
				return s;
			}
			foreach (string text2 in s.Trim().Split(XmlSchemaDatatype.wsChars))
			{
				if (text2 != string.Empty)
				{
					this.sb.Append(text2);
					this.sb.Append(" ");
				}
			}
			text = this.sb.ToString();
			this.sb.Length = 0;
			return text.Trim();
		}

		internal static XmlSchemaDatatype FromName(XmlQualifiedName qname)
		{
			return XmlSchemaDatatype.FromName(qname.Name, qname.Namespace);
		}

		internal static XmlSchemaDatatype FromName(string localName, string ns)
		{
			if (ns != null)
			{
				if (XmlSchemaDatatype.<>f__switch$map3F == null)
				{
					XmlSchemaDatatype.<>f__switch$map3F = new Dictionary<string, int>(2)
					{
						{ "http://www.w3.org/2001/XMLSchema", 0 },
						{ "http://www.w3.org/2003/11/xpath-datatypes", 1 }
					};
				}
				int num;
				if (XmlSchemaDatatype.<>f__switch$map3F.TryGetValue(ns, out num))
				{
					if (num == 0)
					{
						switch (localName)
						{
						case "anySimpleType":
							return XmlSchemaDatatype.datatypeAnySimpleType;
						case "string":
							return XmlSchemaDatatype.datatypeString;
						case "normalizedString":
							return XmlSchemaDatatype.datatypeNormalizedString;
						case "token":
							return XmlSchemaDatatype.datatypeToken;
						case "language":
							return XmlSchemaDatatype.datatypeLanguage;
						case "NMTOKEN":
							return XmlSchemaDatatype.datatypeNMToken;
						case "NMTOKENS":
							return XmlSchemaDatatype.datatypeNMTokens;
						case "Name":
							return XmlSchemaDatatype.datatypeName;
						case "NCName":
							return XmlSchemaDatatype.datatypeNCName;
						case "ID":
							return XmlSchemaDatatype.datatypeID;
						case "IDREF":
							return XmlSchemaDatatype.datatypeIDRef;
						case "IDREFS":
							return XmlSchemaDatatype.datatypeIDRefs;
						case "ENTITY":
							return XmlSchemaDatatype.datatypeEntity;
						case "ENTITIES":
							return XmlSchemaDatatype.datatypeEntities;
						case "NOTATION":
							return XmlSchemaDatatype.datatypeNotation;
						case "decimal":
							return XmlSchemaDatatype.datatypeDecimal;
						case "integer":
							return XmlSchemaDatatype.datatypeInteger;
						case "long":
							return XmlSchemaDatatype.datatypeLong;
						case "int":
							return XmlSchemaDatatype.datatypeInt;
						case "short":
							return XmlSchemaDatatype.datatypeShort;
						case "byte":
							return XmlSchemaDatatype.datatypeByte;
						case "nonPositiveInteger":
							return XmlSchemaDatatype.datatypeNonPositiveInteger;
						case "negativeInteger":
							return XmlSchemaDatatype.datatypeNegativeInteger;
						case "nonNegativeInteger":
							return XmlSchemaDatatype.datatypeNonNegativeInteger;
						case "unsignedLong":
							return XmlSchemaDatatype.datatypeUnsignedLong;
						case "unsignedInt":
							return XmlSchemaDatatype.datatypeUnsignedInt;
						case "unsignedShort":
							return XmlSchemaDatatype.datatypeUnsignedShort;
						case "unsignedByte":
							return XmlSchemaDatatype.datatypeUnsignedByte;
						case "positiveInteger":
							return XmlSchemaDatatype.datatypePositiveInteger;
						case "float":
							return XmlSchemaDatatype.datatypeFloat;
						case "double":
							return XmlSchemaDatatype.datatypeDouble;
						case "base64Binary":
							return XmlSchemaDatatype.datatypeBase64Binary;
						case "boolean":
							return XmlSchemaDatatype.datatypeBoolean;
						case "anyURI":
							return XmlSchemaDatatype.datatypeAnyURI;
						case "duration":
							return XmlSchemaDatatype.datatypeDuration;
						case "dateTime":
							return XmlSchemaDatatype.datatypeDateTime;
						case "date":
							return XmlSchemaDatatype.datatypeDate;
						case "time":
							return XmlSchemaDatatype.datatypeTime;
						case "hexBinary":
							return XmlSchemaDatatype.datatypeHexBinary;
						case "QName":
							return XmlSchemaDatatype.datatypeQName;
						case "gYearMonth":
							return XmlSchemaDatatype.datatypeGYearMonth;
						case "gMonthDay":
							return XmlSchemaDatatype.datatypeGMonthDay;
						case "gYear":
							return XmlSchemaDatatype.datatypeGYear;
						case "gMonth":
							return XmlSchemaDatatype.datatypeGMonth;
						case "gDay":
							return XmlSchemaDatatype.datatypeGDay;
						}
						return null;
					}
					if (num == 1)
					{
						switch (localName)
						{
						case "anyAtomicType":
							return XmlSchemaDatatype.datatypeAnyAtomicType;
						case "untypedAtomic":
							return XmlSchemaDatatype.datatypeUntypedAtomic;
						case "dayTimeDuration":
							return XmlSchemaDatatype.datatypeDayTimeDuration;
						case "yearMonthDuration":
							return XmlSchemaDatatype.datatypeYearMonthDuration;
						}
						return null;
					}
				}
			}
			return null;
		}

		internal XsdWhitespaceFacet WhitespaceValue;

		private static char[] wsChars = new char[] { ' ', '\t', '\n', '\r' };

		private StringBuilder sb = new StringBuilder();

		private static readonly XsdAnySimpleType datatypeAnySimpleType = XsdAnySimpleType.Instance;

		private static readonly XsdString datatypeString = new XsdString();

		private static readonly XsdNormalizedString datatypeNormalizedString = new XsdNormalizedString();

		private static readonly XsdToken datatypeToken = new XsdToken();

		private static readonly XsdLanguage datatypeLanguage = new XsdLanguage();

		private static readonly XsdNMToken datatypeNMToken = new XsdNMToken();

		private static readonly XsdNMTokens datatypeNMTokens = new XsdNMTokens();

		private static readonly XsdName datatypeName = new XsdName();

		private static readonly XsdNCName datatypeNCName = new XsdNCName();

		private static readonly XsdID datatypeID = new XsdID();

		private static readonly XsdIDRef datatypeIDRef = new XsdIDRef();

		private static readonly XsdIDRefs datatypeIDRefs = new XsdIDRefs();

		private static readonly XsdEntity datatypeEntity = new XsdEntity();

		private static readonly XsdEntities datatypeEntities = new XsdEntities();

		private static readonly XsdNotation datatypeNotation = new XsdNotation();

		private static readonly XsdDecimal datatypeDecimal = new XsdDecimal();

		private static readonly XsdInteger datatypeInteger = new XsdInteger();

		private static readonly XsdLong datatypeLong = new XsdLong();

		private static readonly XsdInt datatypeInt = new XsdInt();

		private static readonly XsdShort datatypeShort = new XsdShort();

		private static readonly XsdByte datatypeByte = new XsdByte();

		private static readonly XsdNonNegativeInteger datatypeNonNegativeInteger = new XsdNonNegativeInteger();

		private static readonly XsdPositiveInteger datatypePositiveInteger = new XsdPositiveInteger();

		private static readonly XsdUnsignedLong datatypeUnsignedLong = new XsdUnsignedLong();

		private static readonly XsdUnsignedInt datatypeUnsignedInt = new XsdUnsignedInt();

		private static readonly XsdUnsignedShort datatypeUnsignedShort = new XsdUnsignedShort();

		private static readonly XsdUnsignedByte datatypeUnsignedByte = new XsdUnsignedByte();

		private static readonly XsdNonPositiveInteger datatypeNonPositiveInteger = new XsdNonPositiveInteger();

		private static readonly XsdNegativeInteger datatypeNegativeInteger = new XsdNegativeInteger();

		private static readonly XsdFloat datatypeFloat = new XsdFloat();

		private static readonly XsdDouble datatypeDouble = new XsdDouble();

		private static readonly XsdBase64Binary datatypeBase64Binary = new XsdBase64Binary();

		private static readonly XsdBoolean datatypeBoolean = new XsdBoolean();

		private static readonly XsdAnyURI datatypeAnyURI = new XsdAnyURI();

		private static readonly XsdDuration datatypeDuration = new XsdDuration();

		private static readonly XsdDateTime datatypeDateTime = new XsdDateTime();

		private static readonly XsdDate datatypeDate = new XsdDate();

		private static readonly XsdTime datatypeTime = new XsdTime();

		private static readonly XsdHexBinary datatypeHexBinary = new XsdHexBinary();

		private static readonly XsdQName datatypeQName = new XsdQName();

		private static readonly XsdGYearMonth datatypeGYearMonth = new XsdGYearMonth();

		private static readonly XsdGMonthDay datatypeGMonthDay = new XsdGMonthDay();

		private static readonly XsdGYear datatypeGYear = new XsdGYear();

		private static readonly XsdGMonth datatypeGMonth = new XsdGMonth();

		private static readonly XsdGDay datatypeGDay = new XsdGDay();

		private static readonly XdtAnyAtomicType datatypeAnyAtomicType = new XdtAnyAtomicType();

		private static readonly XdtUntypedAtomic datatypeUntypedAtomic = new XdtUntypedAtomic();

		private static readonly XdtDayTimeDuration datatypeDayTimeDuration = new XdtDayTimeDuration();

		private static readonly XdtYearMonthDuration datatypeYearMonthDuration = new XdtYearMonthDuration();
	}
}
