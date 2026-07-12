using System;
using System.Xml;

namespace System.Runtime.Serialization
{
	internal static class DictionaryGlobals
	{
		static DictionaryGlobals()
		{
			XmlDictionary xmlDictionary = new XmlDictionary(61);
			try
			{
				DictionaryGlobals.SchemaInstanceNamespace = xmlDictionary.Add("http://www.w3.org/2001/XMLSchema-instance");
				DictionaryGlobals.SerializationNamespace = xmlDictionary.Add("http://schemas.microsoft.com/2003/10/Serialization/");
				DictionaryGlobals.SchemaNamespace = xmlDictionary.Add("http://www.w3.org/2001/XMLSchema");
				DictionaryGlobals.XsiTypeLocalName = xmlDictionary.Add("type");
				DictionaryGlobals.XsiNilLocalName = xmlDictionary.Add("nil");
				DictionaryGlobals.IdLocalName = xmlDictionary.Add("Id");
				DictionaryGlobals.RefLocalName = xmlDictionary.Add("Ref");
				DictionaryGlobals.ArraySizeLocalName = xmlDictionary.Add("Size");
				DictionaryGlobals.EmptyString = xmlDictionary.Add(string.Empty);
				DictionaryGlobals.ISerializableFactoryTypeLocalName = xmlDictionary.Add("FactoryType");
				DictionaryGlobals.XmlnsNamespace = xmlDictionary.Add("http://www.w3.org/2000/xmlns/");
				DictionaryGlobals.CharLocalName = xmlDictionary.Add("char");
				DictionaryGlobals.BooleanLocalName = xmlDictionary.Add("boolean");
				DictionaryGlobals.SignedByteLocalName = xmlDictionary.Add("byte");
				DictionaryGlobals.UnsignedByteLocalName = xmlDictionary.Add("unsignedByte");
				DictionaryGlobals.ShortLocalName = xmlDictionary.Add("short");
				DictionaryGlobals.UnsignedShortLocalName = xmlDictionary.Add("unsignedShort");
				DictionaryGlobals.IntLocalName = xmlDictionary.Add("int");
				DictionaryGlobals.UnsignedIntLocalName = xmlDictionary.Add("unsignedInt");
				DictionaryGlobals.LongLocalName = xmlDictionary.Add("long");
				DictionaryGlobals.UnsignedLongLocalName = xmlDictionary.Add("unsignedLong");
				DictionaryGlobals.FloatLocalName = xmlDictionary.Add("float");
				DictionaryGlobals.DoubleLocalName = xmlDictionary.Add("double");
				DictionaryGlobals.DecimalLocalName = xmlDictionary.Add("decimal");
				DictionaryGlobals.DateTimeLocalName = xmlDictionary.Add("dateTime");
				DictionaryGlobals.StringLocalName = xmlDictionary.Add("string");
				DictionaryGlobals.ByteArrayLocalName = xmlDictionary.Add("base64Binary");
				DictionaryGlobals.ObjectLocalName = xmlDictionary.Add("anyType");
				DictionaryGlobals.TimeSpanLocalName = xmlDictionary.Add("duration");
				DictionaryGlobals.GuidLocalName = xmlDictionary.Add("guid");
				DictionaryGlobals.UriLocalName = xmlDictionary.Add("anyURI");
				DictionaryGlobals.QNameLocalName = xmlDictionary.Add("QName");
				DictionaryGlobals.ClrTypeLocalName = xmlDictionary.Add("Type");
				DictionaryGlobals.ClrAssemblyLocalName = xmlDictionary.Add("Assembly");
				DictionaryGlobals.Space = xmlDictionary.Add(" ");
				DictionaryGlobals.timeLocalName = xmlDictionary.Add("time");
				DictionaryGlobals.dateLocalName = xmlDictionary.Add("date");
				DictionaryGlobals.hexBinaryLocalName = xmlDictionary.Add("hexBinary");
				DictionaryGlobals.gYearMonthLocalName = xmlDictionary.Add("gYearMonth");
				DictionaryGlobals.gYearLocalName = xmlDictionary.Add("gYear");
				DictionaryGlobals.gMonthDayLocalName = xmlDictionary.Add("gMonthDay");
				DictionaryGlobals.gDayLocalName = xmlDictionary.Add("gDay");
				DictionaryGlobals.gMonthLocalName = xmlDictionary.Add("gMonth");
				DictionaryGlobals.integerLocalName = xmlDictionary.Add("integer");
				DictionaryGlobals.positiveIntegerLocalName = xmlDictionary.Add("positiveInteger");
				DictionaryGlobals.negativeIntegerLocalName = xmlDictionary.Add("negativeInteger");
				DictionaryGlobals.nonPositiveIntegerLocalName = xmlDictionary.Add("nonPositiveInteger");
				DictionaryGlobals.nonNegativeIntegerLocalName = xmlDictionary.Add("nonNegativeInteger");
				DictionaryGlobals.normalizedStringLocalName = xmlDictionary.Add("normalizedString");
				DictionaryGlobals.tokenLocalName = xmlDictionary.Add("token");
				DictionaryGlobals.languageLocalName = xmlDictionary.Add("language");
				DictionaryGlobals.NameLocalName = xmlDictionary.Add("Name");
				DictionaryGlobals.NCNameLocalName = xmlDictionary.Add("NCName");
				DictionaryGlobals.XSDIDLocalName = xmlDictionary.Add("ID");
				DictionaryGlobals.IDREFLocalName = xmlDictionary.Add("IDREF");
				DictionaryGlobals.IDREFSLocalName = xmlDictionary.Add("IDREFS");
				DictionaryGlobals.ENTITYLocalName = xmlDictionary.Add("ENTITY");
				DictionaryGlobals.ENTITIESLocalName = xmlDictionary.Add("ENTITIES");
				DictionaryGlobals.NMTOKENLocalName = xmlDictionary.Add("NMTOKEN");
				DictionaryGlobals.NMTOKENSLocalName = xmlDictionary.Add("NMTOKENS");
				DictionaryGlobals.AsmxTypesNamespace = xmlDictionary.Add("http://microsoft.com/wsdl/types/");
			}
			catch (Exception ex)
			{
				if (Fx.IsFatal(ex))
				{
					throw;
				}
				throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(ex.Message, ex);
			}
		}

		public static readonly XmlDictionaryString EmptyString;

		public static readonly XmlDictionaryString SchemaInstanceNamespace;

		public static readonly XmlDictionaryString SchemaNamespace;

		public static readonly XmlDictionaryString SerializationNamespace;

		public static readonly XmlDictionaryString XmlnsNamespace;

		public static readonly XmlDictionaryString XsiTypeLocalName;

		public static readonly XmlDictionaryString XsiNilLocalName;

		public static readonly XmlDictionaryString ClrTypeLocalName;

		public static readonly XmlDictionaryString ClrAssemblyLocalName;

		public static readonly XmlDictionaryString ArraySizeLocalName;

		public static readonly XmlDictionaryString IdLocalName;

		public static readonly XmlDictionaryString RefLocalName;

		public static readonly XmlDictionaryString ISerializableFactoryTypeLocalName;

		public static readonly XmlDictionaryString CharLocalName;

		public static readonly XmlDictionaryString BooleanLocalName;

		public static readonly XmlDictionaryString SignedByteLocalName;

		public static readonly XmlDictionaryString UnsignedByteLocalName;

		public static readonly XmlDictionaryString ShortLocalName;

		public static readonly XmlDictionaryString UnsignedShortLocalName;

		public static readonly XmlDictionaryString IntLocalName;

		public static readonly XmlDictionaryString UnsignedIntLocalName;

		public static readonly XmlDictionaryString LongLocalName;

		public static readonly XmlDictionaryString UnsignedLongLocalName;

		public static readonly XmlDictionaryString FloatLocalName;

		public static readonly XmlDictionaryString DoubleLocalName;

		public static readonly XmlDictionaryString DecimalLocalName;

		public static readonly XmlDictionaryString DateTimeLocalName;

		public static readonly XmlDictionaryString StringLocalName;

		public static readonly XmlDictionaryString ByteArrayLocalName;

		public static readonly XmlDictionaryString ObjectLocalName;

		public static readonly XmlDictionaryString TimeSpanLocalName;

		public static readonly XmlDictionaryString GuidLocalName;

		public static readonly XmlDictionaryString UriLocalName;

		public static readonly XmlDictionaryString QNameLocalName;

		public static readonly XmlDictionaryString Space;

		public static readonly XmlDictionaryString timeLocalName;

		public static readonly XmlDictionaryString dateLocalName;

		public static readonly XmlDictionaryString hexBinaryLocalName;

		public static readonly XmlDictionaryString gYearMonthLocalName;

		public static readonly XmlDictionaryString gYearLocalName;

		public static readonly XmlDictionaryString gMonthDayLocalName;

		public static readonly XmlDictionaryString gDayLocalName;

		public static readonly XmlDictionaryString gMonthLocalName;

		public static readonly XmlDictionaryString integerLocalName;

		public static readonly XmlDictionaryString positiveIntegerLocalName;

		public static readonly XmlDictionaryString negativeIntegerLocalName;

		public static readonly XmlDictionaryString nonPositiveIntegerLocalName;

		public static readonly XmlDictionaryString nonNegativeIntegerLocalName;

		public static readonly XmlDictionaryString normalizedStringLocalName;

		public static readonly XmlDictionaryString tokenLocalName;

		public static readonly XmlDictionaryString languageLocalName;

		public static readonly XmlDictionaryString NameLocalName;

		public static readonly XmlDictionaryString NCNameLocalName;

		public static readonly XmlDictionaryString XSDIDLocalName;

		public static readonly XmlDictionaryString IDREFLocalName;

		public static readonly XmlDictionaryString IDREFSLocalName;

		public static readonly XmlDictionaryString ENTITYLocalName;

		public static readonly XmlDictionaryString ENTITIESLocalName;

		public static readonly XmlDictionaryString NMTOKENLocalName;

		public static readonly XmlDictionaryString NMTOKENSLocalName;

		public static readonly XmlDictionaryString AsmxTypesNamespace;
	}
}
