using System;
using System.Globalization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Diagnostics.Application;
using System.Text;

namespace System.Xml
{
	internal static class XmlExceptionHelper
	{
		private static void ThrowXmlException(XmlDictionaryReader reader, string res)
		{
			XmlExceptionHelper.ThrowXmlException(reader, res, null);
		}

		private static void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1)
		{
			XmlExceptionHelper.ThrowXmlException(reader, res, arg1, null);
		}

		private static void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1, string arg2)
		{
			XmlExceptionHelper.ThrowXmlException(reader, res, arg1, arg2, null);
		}

		private static void ThrowXmlException(XmlDictionaryReader reader, string res, string arg1, string arg2, string arg3)
		{
			string text = global::System.Runtime.Serialization.SR.GetString(res, new object[] { arg1, arg2, arg3 });
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
			{
				text = text + " " + global::System.Runtime.Serialization.SR.GetString("Line {0}, position {1}.", new object[] { xmlLineInfo.LineNumber, xmlLineInfo.LinePosition });
			}
			if (TD.ReaderQuotaExceededIsEnabled())
			{
				TD.ReaderQuotaExceeded(text);
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(text));
		}

		public static void ThrowXmlException(XmlDictionaryReader reader, XmlException exception)
		{
			string text = exception.Message;
			IXmlLineInfo xmlLineInfo = reader as IXmlLineInfo;
			if (xmlLineInfo != null && xmlLineInfo.HasLineInfo())
			{
				text = text + " " + global::System.Runtime.Serialization.SR.GetString("Line {0}, position {1}.", new object[] { xmlLineInfo.LineNumber, xmlLineInfo.LinePosition });
			}
			throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(text));
		}

		private static string GetName(string prefix, string localName)
		{
			if (prefix.Length == 0)
			{
				return localName;
			}
			return prefix + ":" + localName;
		}

		private static string GetWhatWasFound(XmlDictionaryReader reader)
		{
			if (reader.EOF)
			{
				return global::System.Runtime.Serialization.SR.GetString("end of file");
			}
			XmlNodeType nodeType = reader.NodeType;
			if (nodeType <= XmlNodeType.Comment)
			{
				switch (nodeType)
				{
				case XmlNodeType.Element:
					return global::System.Runtime.Serialization.SR.GetString("element '{0}' from namespace '{1}'", new object[]
					{
						XmlExceptionHelper.GetName(reader.Prefix, reader.LocalName),
						reader.NamespaceURI
					});
				case XmlNodeType.Attribute:
					goto IL_00FD;
				case XmlNodeType.Text:
					break;
				case XmlNodeType.CDATA:
					return global::System.Runtime.Serialization.SR.GetString("cdata '{0}'", new object[] { reader.Value });
				default:
					if (nodeType != XmlNodeType.Comment)
					{
						goto IL_00FD;
					}
					return global::System.Runtime.Serialization.SR.GetString("comment '{0}'", new object[] { reader.Value });
				}
			}
			else if (nodeType - XmlNodeType.Whitespace > 1)
			{
				if (nodeType != XmlNodeType.EndElement)
				{
					goto IL_00FD;
				}
				return global::System.Runtime.Serialization.SR.GetString("end element '{0}' from namespace '{1}'", new object[]
				{
					XmlExceptionHelper.GetName(reader.Prefix, reader.LocalName),
					reader.NamespaceURI
				});
			}
			return global::System.Runtime.Serialization.SR.GetString("text '{0}'", new object[] { reader.Value });
			IL_00FD:
			return global::System.Runtime.Serialization.SR.GetString("node {0}", new object[] { reader.NodeType });
		}

		public static void ThrowStartElementExpected(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Start element expected. Found {0}.", XmlExceptionHelper.GetWhatWasFound(reader));
		}

		public static void ThrowStartElementExpected(XmlDictionaryReader reader, string name)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Start element '{0}' expected. Found {1}.", name, XmlExceptionHelper.GetWhatWasFound(reader));
		}

		public static void ThrowStartElementExpected(XmlDictionaryReader reader, string localName, string ns)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Start element '{0}' from namespace '{1}' expected. Found {2}.", localName, ns, XmlExceptionHelper.GetWhatWasFound(reader));
		}

		public static void ThrowStartElementExpected(XmlDictionaryReader reader, XmlDictionaryString localName, XmlDictionaryString ns)
		{
			XmlExceptionHelper.ThrowStartElementExpected(reader, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(ns));
		}

		public static void ThrowFullStartElementExpected(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Non-empty start element expected. Found {0}.", XmlExceptionHelper.GetWhatWasFound(reader));
		}

		public static void ThrowFullStartElementExpected(XmlDictionaryReader reader, string name)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Non-empty start element '{0}' expected. Found {1}.", name, XmlExceptionHelper.GetWhatWasFound(reader));
		}

		public static void ThrowFullStartElementExpected(XmlDictionaryReader reader, string localName, string ns)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Non-empty start element '{0}' from namespace '{1}' expected. Found {2}.", localName, ns, XmlExceptionHelper.GetWhatWasFound(reader));
		}

		public static void ThrowFullStartElementExpected(XmlDictionaryReader reader, XmlDictionaryString localName, XmlDictionaryString ns)
		{
			XmlExceptionHelper.ThrowFullStartElementExpected(reader, XmlDictionaryString.GetString(localName), XmlDictionaryString.GetString(ns));
		}

		public static void ThrowEndElementExpected(XmlDictionaryReader reader, string localName, string ns)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "End element '{0}' from namespace '{1}' expected. Found {2}.", localName, ns, XmlExceptionHelper.GetWhatWasFound(reader));
		}

		public static void ThrowMaxStringContentLengthExceeded(XmlDictionaryReader reader, int maxStringContentLength)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "XML max string content length exceeded. It must be less than {0}.", maxStringContentLength.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowMaxArrayLengthExceeded(XmlDictionaryReader reader, int maxArrayLength)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The maximum array length quota ({0}) has been exceeded while reading XML data. This quota may be increased by changing the MaxArrayLength property on the XmlDictionaryReaderQuotas object used when creating the XML reader.", maxArrayLength.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowMaxArrayLengthOrMaxItemsQuotaExceeded(XmlDictionaryReader reader, int maxQuota)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "XML max array length or max items quota exceeded. It must be less than {0}.", maxQuota.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowMaxDepthExceeded(XmlDictionaryReader reader, int maxDepth)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "XML max depth exceeded. It must be less than {0}.", maxDepth.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowMaxBytesPerReadExceeded(XmlDictionaryReader reader, int maxBytesPerRead)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "XML max bytes per read exceeded. It must be less than {0}.", maxBytesPerRead.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowMaxNameTableCharCountExceeded(XmlDictionaryReader reader, int maxNameTableCharCount)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The maximum nametable character count quota ({0}) has been exceeded while reading XML data. The nametable is a data structure used to store strings encountered during XML processing - long XML documents with non-repeating element names, attribute names and attribute values may trigger this quota. This quota may be increased by changing the MaxNameTableCharCount property on the XmlDictionaryReaderQuotas object used when creating the XML reader.", maxNameTableCharCount.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowBase64DataExpected(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Base64 encoded data expected. Found {0}.", XmlExceptionHelper.GetWhatWasFound(reader));
		}

		public static void ThrowUndefinedPrefix(XmlDictionaryReader reader, string prefix)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The prefix '{0}' is not defined.", prefix);
		}

		public static void ThrowProcessingInstructionNotSupported(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Processing instructions (other than the XML declaration) and DTDs are not supported.");
		}

		public static void ThrowInvalidXml(XmlDictionaryReader reader, byte b)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The byte 0x{0} is not valid at this location.", b.ToString("X2", CultureInfo.InvariantCulture));
		}

		public static void ThrowUnexpectedEndOfFile(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Unexpected end of file. Following elements are not closed: {0}.", ((XmlBaseReader)reader).GetOpenElements());
		}

		public static void ThrowUnexpectedEndElement(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "No matching start tag for end element.");
		}

		public static void ThrowTokenExpected(XmlDictionaryReader reader, string expected, char found)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The token '{0}' was expected but found '{1}'.", expected, found.ToString());
		}

		public static void ThrowTokenExpected(XmlDictionaryReader reader, string expected, string found)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The token '{0}' was expected but found '{1}'.", expected, found);
		}

		public static void ThrowInvalidCharRef(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Character reference not valid.");
		}

		public static void ThrowTagMismatch(XmlDictionaryReader reader, string expectedPrefix, string expectedLocalName, string foundPrefix, string foundLocalName)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Start element '{0}' does not match end element '{1}'.", XmlExceptionHelper.GetName(expectedPrefix, expectedLocalName), XmlExceptionHelper.GetName(foundPrefix, foundLocalName));
		}

		public static void ThrowDuplicateXmlnsAttribute(XmlDictionaryReader reader, string localName, string ns)
		{
			string text;
			if (localName.Length == 0)
			{
				text = "xmlns";
			}
			else
			{
				text = "xmlns:" + localName;
			}
			XmlExceptionHelper.ThrowXmlException(reader, "Duplicate attribute found. Both '{0}' and '{1}' are from the namespace '{2}'.", text, text, ns);
		}

		public static void ThrowDuplicateAttribute(XmlDictionaryReader reader, string prefix1, string prefix2, string localName, string ns)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "Duplicate attribute found. Both '{0}' and '{1}' are from the namespace '{2}'.", XmlExceptionHelper.GetName(prefix1, localName), XmlExceptionHelper.GetName(prefix2, localName), ns);
		}

		public static void ThrowInvalidBinaryFormat(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The input source is not correctly formatted.");
		}

		public static void ThrowInvalidRootData(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The data at the root level is invalid.");
		}

		public static void ThrowMultipleRootElements(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "There are multiple root elements.");
		}

		public static void ThrowDeclarationNotFirst(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "No characters can appear before the XML declaration.");
		}

		public static void ThrowConversionOverflow(XmlDictionaryReader reader, string value, string type)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The value '{0}' cannot be represented with the type '{1}'.", value, type);
		}

		public static void ThrowXmlDictionaryStringIDOutOfRange(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "XmlDictionaryString IDs must be in the range from {0} to {1}.", 0.ToString(NumberFormatInfo.CurrentInfo), 536870911.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowXmlDictionaryStringIDUndefinedStatic(XmlDictionaryReader reader, int key)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "XmlDictionaryString ID {0} not defined in the static dictionary.", key.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowXmlDictionaryStringIDUndefinedSession(XmlDictionaryReader reader, int key)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "XmlDictionaryString ID {0} not defined in the XmlBinaryReaderSession.", key.ToString(NumberFormatInfo.CurrentInfo));
		}

		public static void ThrowEmptyNamespace(XmlDictionaryReader reader)
		{
			XmlExceptionHelper.ThrowXmlException(reader, "The empty namespace requires a null or empty prefix.");
		}

		public static XmlException CreateConversionException(string value, string type, Exception exception)
		{
			return new XmlException(global::System.Runtime.Serialization.SR.GetString("The value '{0}' cannot be parsed as the type '{1}'.", new object[] { value, type }), exception);
		}

		public static XmlException CreateEncodingException(byte[] buffer, int offset, int count, Exception exception)
		{
			return XmlExceptionHelper.CreateEncodingException(new UTF8Encoding(false, false).GetString(buffer, offset, count), exception);
		}

		public static XmlException CreateEncodingException(string value, Exception exception)
		{
			return new XmlException(global::System.Runtime.Serialization.SR.GetString("'{0}' contains invalid UTF8 bytes.", new object[] { value }), exception);
		}
	}
}
