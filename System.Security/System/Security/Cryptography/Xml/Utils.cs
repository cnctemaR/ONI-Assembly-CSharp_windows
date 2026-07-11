using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class Utils
	{
		private Utils()
		{
		}

		private static bool HasNamespace(XmlElement element, string prefix, string value)
		{
			return Utils.IsCommittedNamespace(element, prefix, value) || (element.Prefix == prefix && element.NamespaceURI == value);
		}

		internal static bool IsCommittedNamespace(XmlElement element, string prefix, string value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			string text = ((prefix.Length > 0) ? ("xmlns:" + prefix) : "xmlns");
			return element.HasAttribute(text) && element.GetAttribute(text) == value;
		}

		internal static bool IsRedundantNamespace(XmlElement element, string prefix, string value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}
			for (XmlNode xmlNode = element.ParentNode; xmlNode != null; xmlNode = xmlNode.ParentNode)
			{
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null && Utils.HasNamespace(xmlElement, prefix, value))
				{
					return true;
				}
			}
			return false;
		}

		internal static string GetAttribute(XmlElement element, string localName, string namespaceURI)
		{
			string text = (element.HasAttribute(localName) ? element.GetAttribute(localName) : null);
			if (text == null && element.HasAttribute(localName, namespaceURI))
			{
				text = element.GetAttribute(localName, namespaceURI);
			}
			return text;
		}

		internal static bool HasAttribute(XmlElement element, string localName, string namespaceURI)
		{
			return element.HasAttribute(localName) || element.HasAttribute(localName, namespaceURI);
		}

		internal static bool IsNamespaceNode(XmlNode n)
		{
			return n.NodeType == XmlNodeType.Attribute && (n.Prefix.Equals("xmlns") || (n.Prefix.Length == 0 && n.LocalName.Equals("xmlns")));
		}

		internal static bool IsXmlNamespaceNode(XmlNode n)
		{
			return n.NodeType == XmlNodeType.Attribute && n.Prefix.Equals("xml");
		}

		internal static bool IsDefaultNamespaceNode(XmlNode n)
		{
			bool flag = n.NodeType == XmlNodeType.Attribute && n.Prefix.Length == 0 && n.LocalName.Equals("xmlns");
			bool flag2 = Utils.IsXmlNamespaceNode(n);
			return flag || flag2;
		}

		internal static bool IsEmptyDefaultNamespaceNode(XmlNode n)
		{
			return Utils.IsDefaultNamespaceNode(n) && n.Value.Length == 0;
		}

		internal static string GetNamespacePrefix(XmlAttribute a)
		{
			if (a.Prefix.Length != 0)
			{
				return a.LocalName;
			}
			return string.Empty;
		}

		internal static bool HasNamespacePrefix(XmlAttribute a, string nsPrefix)
		{
			return Utils.GetNamespacePrefix(a).Equals(nsPrefix);
		}

		internal static bool IsNonRedundantNamespaceDecl(XmlAttribute a, XmlAttribute nearestAncestorWithSamePrefix)
		{
			if (nearestAncestorWithSamePrefix == null)
			{
				return !Utils.IsEmptyDefaultNamespaceNode(a);
			}
			return !nearestAncestorWithSamePrefix.Value.Equals(a.Value);
		}

		internal static bool IsXmlPrefixDefinitionNode(XmlAttribute a)
		{
			return false;
		}

		internal static string DiscardWhiteSpaces(string inputBuffer)
		{
			return Utils.DiscardWhiteSpaces(inputBuffer, 0, inputBuffer.Length);
		}

		internal static string DiscardWhiteSpaces(string inputBuffer, int inputOffset, int inputCount)
		{
			int num = 0;
			for (int i = 0; i < inputCount; i++)
			{
				if (char.IsWhiteSpace(inputBuffer[inputOffset + i]))
				{
					num++;
				}
			}
			char[] array = new char[inputCount - num];
			num = 0;
			for (int i = 0; i < inputCount; i++)
			{
				if (!char.IsWhiteSpace(inputBuffer[inputOffset + i]))
				{
					array[num++] = inputBuffer[inputOffset + i];
				}
			}
			return new string(array);
		}

		internal static void SBReplaceCharWithString(StringBuilder sb, char oldChar, string newString)
		{
			int i = 0;
			int length = newString.Length;
			while (i < sb.Length)
			{
				if (sb[i] == oldChar)
				{
					sb.Remove(i, 1);
					sb.Insert(i, newString);
					i += length;
				}
				else
				{
					i++;
				}
			}
		}

		internal static XmlReader PreProcessStreamInput(Stream inputStream, XmlResolver xmlResolver, string baseUri)
		{
			XmlReaderSettings secureXmlReaderSettings = Utils.GetSecureXmlReaderSettings(xmlResolver);
			return XmlReader.Create(inputStream, secureXmlReaderSettings, baseUri);
		}

		internal static XmlReaderSettings GetSecureXmlReaderSettings(XmlResolver xmlResolver)
		{
			return new XmlReaderSettings
			{
				XmlResolver = xmlResolver,
				DtdProcessing = DtdProcessing.Parse,
				MaxCharactersFromEntities = 10000000L,
				MaxCharactersInDocument = 0L
			};
		}

		internal static XmlDocument PreProcessDocumentInput(XmlDocument document, XmlResolver xmlResolver, string baseUri)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			MyXmlDocument myXmlDocument = new MyXmlDocument();
			myXmlDocument.PreserveWhitespace = document.PreserveWhitespace;
			using (TextReader textReader = new StringReader(document.OuterXml))
			{
				XmlReader xmlReader = XmlReader.Create(textReader, new XmlReaderSettings
				{
					XmlResolver = xmlResolver,
					DtdProcessing = DtdProcessing.Parse,
					MaxCharactersFromEntities = 10000000L,
					MaxCharactersInDocument = 0L
				}, baseUri);
				myXmlDocument.Load(xmlReader);
			}
			return myXmlDocument;
		}

		internal static XmlDocument PreProcessElementInput(XmlElement elem, XmlResolver xmlResolver, string baseUri)
		{
			if (elem == null)
			{
				throw new ArgumentNullException("elem");
			}
			MyXmlDocument myXmlDocument = new MyXmlDocument();
			myXmlDocument.PreserveWhitespace = true;
			using (TextReader textReader = new StringReader(elem.OuterXml))
			{
				XmlReader xmlReader = XmlReader.Create(textReader, new XmlReaderSettings
				{
					XmlResolver = xmlResolver,
					DtdProcessing = DtdProcessing.Parse,
					MaxCharactersFromEntities = 10000000L,
					MaxCharactersInDocument = 0L
				}, baseUri);
				myXmlDocument.Load(xmlReader);
			}
			return myXmlDocument;
		}

		internal static XmlDocument DiscardComments(XmlDocument document)
		{
			XmlNodeList xmlNodeList = document.SelectNodes("//comment()");
			if (xmlNodeList != null)
			{
				foreach (object obj in xmlNodeList)
				{
					XmlNode xmlNode = (XmlNode)obj;
					xmlNode.ParentNode.RemoveChild(xmlNode);
				}
			}
			return document;
		}

		internal static XmlNodeList AllDescendantNodes(XmlNode node, bool includeComments)
		{
			CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
			CanonicalXmlNodeList canonicalXmlNodeList2 = new CanonicalXmlNodeList();
			CanonicalXmlNodeList canonicalXmlNodeList3 = new CanonicalXmlNodeList();
			CanonicalXmlNodeList canonicalXmlNodeList4 = new CanonicalXmlNodeList();
			int num = 0;
			canonicalXmlNodeList2.Add(node);
			do
			{
				XmlNode xmlNode = canonicalXmlNodeList2[num];
				XmlNodeList childNodes = xmlNode.ChildNodes;
				if (childNodes != null)
				{
					foreach (object obj in childNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj;
						if (includeComments || !(xmlNode2 is XmlComment))
						{
							canonicalXmlNodeList2.Add(xmlNode2);
						}
					}
				}
				if (xmlNode.Attributes != null)
				{
					foreach (object obj2 in xmlNode.Attributes)
					{
						XmlNode xmlNode3 = (XmlNode)obj2;
						if (xmlNode3.LocalName == "xmlns" || xmlNode3.Prefix == "xmlns")
						{
							canonicalXmlNodeList4.Add(xmlNode3);
						}
						else
						{
							canonicalXmlNodeList3.Add(xmlNode3);
						}
					}
				}
				num++;
			}
			while (num < canonicalXmlNodeList2.Count);
			foreach (object obj3 in canonicalXmlNodeList2)
			{
				XmlNode xmlNode4 = (XmlNode)obj3;
				canonicalXmlNodeList.Add(xmlNode4);
			}
			foreach (object obj4 in canonicalXmlNodeList3)
			{
				XmlNode xmlNode5 = (XmlNode)obj4;
				canonicalXmlNodeList.Add(xmlNode5);
			}
			foreach (object obj5 in canonicalXmlNodeList4)
			{
				XmlNode xmlNode6 = (XmlNode)obj5;
				canonicalXmlNodeList.Add(xmlNode6);
			}
			return canonicalXmlNodeList;
		}

		internal static bool NodeInList(XmlNode node, XmlNodeList nodeList)
		{
			using (IEnumerator enumerator = nodeList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if ((XmlNode)enumerator.Current == node)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static string GetIdFromLocalUri(string uri, out bool discardComments)
		{
			string text = uri.Substring(1);
			discardComments = true;
			if (text.StartsWith("xpointer(id(", StringComparison.Ordinal))
			{
				int num = text.IndexOf("id(", StringComparison.Ordinal);
				int num2 = text.IndexOf(")", StringComparison.Ordinal);
				if (num2 < 0 || num2 < num + 3)
				{
					throw new CryptographicException("Malformed reference element.");
				}
				text = text.Substring(num + 3, num2 - num - 3);
				text = text.Replace("'", "");
				text = text.Replace("\"", "");
				discardComments = false;
			}
			return text;
		}

		internal static string ExtractIdFromLocalUri(string uri)
		{
			string text = uri.Substring(1);
			if (text.StartsWith("xpointer(id(", StringComparison.Ordinal))
			{
				int num = text.IndexOf("id(", StringComparison.Ordinal);
				int num2 = text.IndexOf(")", StringComparison.Ordinal);
				if (num2 < 0 || num2 < num + 3)
				{
					throw new CryptographicException("Malformed reference element.");
				}
				text = text.Substring(num + 3, num2 - num - 3);
				text = text.Replace("'", "");
				text = text.Replace("\"", "");
			}
			return text;
		}

		internal static void RemoveAllChildren(XmlElement inputElement)
		{
			XmlNode nextSibling;
			for (XmlNode xmlNode = inputElement.FirstChild; xmlNode != null; xmlNode = nextSibling)
			{
				nextSibling = xmlNode.NextSibling;
				inputElement.RemoveChild(xmlNode);
			}
		}

		internal static long Pump(Stream input, Stream output)
		{
			MemoryStream memoryStream = input as MemoryStream;
			if (memoryStream != null && memoryStream.Position == 0L)
			{
				memoryStream.WriteTo(output);
				return memoryStream.Length;
			}
			byte[] array = new byte[4096];
			long num = 0L;
			int num2;
			while ((num2 = input.Read(array, 0, 4096)) > 0)
			{
				output.Write(array, 0, num2);
				num += (long)num2;
			}
			return num;
		}

		internal static Hashtable TokenizePrefixListString(string s)
		{
			Hashtable hashtable = new Hashtable();
			if (s != null)
			{
				foreach (string text in s.Split(null))
				{
					if (text.Equals("#default"))
					{
						hashtable.Add(string.Empty, true);
					}
					else if (text.Length > 0)
					{
						hashtable.Add(text, true);
					}
				}
			}
			return hashtable;
		}

		internal static string EscapeWhitespaceData(string data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(data);
			Utils.SBReplaceCharWithString(stringBuilder, '\r', "&#xD;");
			return stringBuilder.ToString();
		}

		internal static string EscapeTextData(string data)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(data);
			stringBuilder.Replace("&", "&amp;");
			stringBuilder.Replace("<", "&lt;");
			stringBuilder.Replace(">", "&gt;");
			Utils.SBReplaceCharWithString(stringBuilder, '\r', "&#xD;");
			return stringBuilder.ToString();
		}

		internal static string EscapeCData(string data)
		{
			return Utils.EscapeTextData(data);
		}

		internal static string EscapeAttributeValue(string value)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(value);
			stringBuilder.Replace("&", "&amp;");
			stringBuilder.Replace("<", "&lt;");
			stringBuilder.Replace("\"", "&quot;");
			Utils.SBReplaceCharWithString(stringBuilder, '\t', "&#x9;");
			Utils.SBReplaceCharWithString(stringBuilder, '\n', "&#xA;");
			Utils.SBReplaceCharWithString(stringBuilder, '\r', "&#xD;");
			return stringBuilder.ToString();
		}

		internal static XmlDocument GetOwnerDocument(XmlNodeList nodeList)
		{
			foreach (object obj in nodeList)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.OwnerDocument != null)
				{
					return xmlNode.OwnerDocument;
				}
			}
			return null;
		}

		internal static void AddNamespaces(XmlElement elem, CanonicalXmlNodeList namespaces)
		{
			if (namespaces != null)
			{
				foreach (object obj in namespaces)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string text = ((xmlNode.Prefix.Length > 0) ? (xmlNode.Prefix + ":" + xmlNode.LocalName) : xmlNode.LocalName);
					if (!elem.HasAttribute(text) && (!text.Equals("xmlns") || elem.Prefix.Length != 0))
					{
						XmlAttribute xmlAttribute = elem.OwnerDocument.CreateAttribute(text);
						xmlAttribute.Value = xmlNode.Value;
						elem.SetAttributeNode(xmlAttribute);
					}
				}
			}
		}

		internal static void AddNamespaces(XmlElement elem, Hashtable namespaces)
		{
			if (namespaces != null)
			{
				foreach (object obj in namespaces.Keys)
				{
					string text = (string)obj;
					if (!elem.HasAttribute(text))
					{
						XmlAttribute xmlAttribute = elem.OwnerDocument.CreateAttribute(text);
						xmlAttribute.Value = namespaces[text] as string;
						elem.SetAttributeNode(xmlAttribute);
					}
				}
			}
		}

		internal static CanonicalXmlNodeList GetPropagatedAttributes(XmlElement elem)
		{
			if (elem == null)
			{
				return null;
			}
			CanonicalXmlNodeList canonicalXmlNodeList = new CanonicalXmlNodeList();
			XmlNode xmlNode = elem;
			if (xmlNode == null)
			{
				return null;
			}
			bool flag = true;
			while (xmlNode != null)
			{
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement == null)
				{
					xmlNode = xmlNode.ParentNode;
				}
				else
				{
					if (!Utils.IsCommittedNamespace(xmlElement, xmlElement.Prefix, xmlElement.NamespaceURI) && !Utils.IsRedundantNamespace(xmlElement, xmlElement.Prefix, xmlElement.NamespaceURI))
					{
						string text = ((xmlElement.Prefix.Length > 0) ? ("xmlns:" + xmlElement.Prefix) : "xmlns");
						XmlAttribute xmlAttribute = elem.OwnerDocument.CreateAttribute(text);
						xmlAttribute.Value = xmlElement.NamespaceURI;
						canonicalXmlNodeList.Add(xmlAttribute);
					}
					if (xmlElement.HasAttributes)
					{
						foreach (object obj in xmlElement.Attributes)
						{
							XmlAttribute xmlAttribute2 = (XmlAttribute)obj;
							if (flag && xmlAttribute2.LocalName == "xmlns")
							{
								XmlAttribute xmlAttribute3 = elem.OwnerDocument.CreateAttribute("xmlns");
								xmlAttribute3.Value = xmlAttribute2.Value;
								canonicalXmlNodeList.Add(xmlAttribute3);
								flag = false;
							}
							else if (xmlAttribute2.Prefix == "xmlns" || xmlAttribute2.Prefix == "xml")
							{
								canonicalXmlNodeList.Add(xmlAttribute2);
							}
							else if (xmlAttribute2.NamespaceURI.Length > 0 && !Utils.IsCommittedNamespace(xmlElement, xmlAttribute2.Prefix, xmlAttribute2.NamespaceURI) && !Utils.IsRedundantNamespace(xmlElement, xmlAttribute2.Prefix, xmlAttribute2.NamespaceURI))
							{
								string text2 = ((xmlAttribute2.Prefix.Length > 0) ? ("xmlns:" + xmlAttribute2.Prefix) : "xmlns");
								XmlAttribute xmlAttribute4 = elem.OwnerDocument.CreateAttribute(text2);
								xmlAttribute4.Value = xmlAttribute2.NamespaceURI;
								canonicalXmlNodeList.Add(xmlAttribute4);
							}
						}
					}
					xmlNode = xmlNode.ParentNode;
				}
			}
			return canonicalXmlNodeList;
		}

		internal static byte[] ConvertIntToByteArray(int dwInput)
		{
			byte[] array = new byte[8];
			int num = 0;
			if (dwInput == 0)
			{
				return new byte[1];
			}
			int i = dwInput;
			while (i > 0)
			{
				int num2 = i % 256;
				array[num] = (byte)num2;
				i = (i - num2) / 256;
				num++;
			}
			byte[] array2 = new byte[num];
			for (int j = 0; j < num; j++)
			{
				array2[j] = array[num - j - 1];
			}
			return array2;
		}

		internal static int ConvertByteArrayToInt(byte[] input)
		{
			int num = 0;
			for (int i = 0; i < input.Length; i++)
			{
				num *= 256;
				num += (int)input[i];
			}
			return num;
		}

		internal static int GetHexArraySize(byte[] hex)
		{
			int num = hex.Length;
			while (num-- > 0 && hex[num] == 0)
			{
			}
			return num + 1;
		}

		internal static X509Certificate2Collection BuildBagOfCerts(KeyInfoX509Data keyInfoX509Data, CertUsageType certUsageType)
		{
			X509Certificate2Collection x509Certificate2Collection = new X509Certificate2Collection();
			ArrayList arrayList = ((certUsageType == CertUsageType.Decryption) ? new ArrayList() : null);
			if (keyInfoX509Data.Certificates != null)
			{
				foreach (object obj in keyInfoX509Data.Certificates)
				{
					X509Certificate2 x509Certificate = (X509Certificate2)obj;
					if (certUsageType != CertUsageType.Verification)
					{
						if (certUsageType == CertUsageType.Decryption)
						{
							arrayList.Add(new X509IssuerSerial(x509Certificate.IssuerName.Name, x509Certificate.SerialNumber));
						}
					}
					else
					{
						x509Certificate2Collection.Add(x509Certificate);
					}
				}
			}
			if (keyInfoX509Data.SubjectNames == null && keyInfoX509Data.IssuerSerials == null && keyInfoX509Data.SubjectKeyIds == null && arrayList == null)
			{
				return x509Certificate2Collection;
			}
			X509Store[] array = new X509Store[2];
			string text = ((certUsageType == CertUsageType.Verification) ? "AddressBook" : "My");
			array[0] = new X509Store(text, StoreLocation.CurrentUser);
			array[1] = new X509Store(text, StoreLocation.LocalMachine);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null)
				{
					X509Certificate2Collection x509Certificate2Collection2 = null;
					try
					{
						array[i].Open(OpenFlags.OpenExistingOnly);
						x509Certificate2Collection2 = array[i].Certificates;
						array[i].Close();
						if (keyInfoX509Data.SubjectNames != null)
						{
							foreach (object obj2 in keyInfoX509Data.SubjectNames)
							{
								string text2 = (string)obj2;
								x509Certificate2Collection2 = x509Certificate2Collection2.Find(X509FindType.FindBySubjectDistinguishedName, text2, false);
							}
						}
						if (keyInfoX509Data.IssuerSerials != null)
						{
							foreach (object obj3 in keyInfoX509Data.IssuerSerials)
							{
								X509IssuerSerial x509IssuerSerial = (X509IssuerSerial)obj3;
								x509Certificate2Collection2 = x509Certificate2Collection2.Find(X509FindType.FindByIssuerDistinguishedName, x509IssuerSerial.IssuerName, false);
								x509Certificate2Collection2 = x509Certificate2Collection2.Find(X509FindType.FindBySerialNumber, x509IssuerSerial.SerialNumber, false);
							}
						}
						if (keyInfoX509Data.SubjectKeyIds != null)
						{
							foreach (object obj4 in keyInfoX509Data.SubjectKeyIds)
							{
								string text3 = Utils.EncodeHexString((byte[])obj4);
								x509Certificate2Collection2 = x509Certificate2Collection2.Find(X509FindType.FindBySubjectKeyIdentifier, text3, false);
							}
						}
						if (arrayList != null)
						{
							foreach (object obj5 in arrayList)
							{
								X509IssuerSerial x509IssuerSerial2 = (X509IssuerSerial)obj5;
								x509Certificate2Collection2 = x509Certificate2Collection2.Find(X509FindType.FindByIssuerDistinguishedName, x509IssuerSerial2.IssuerName, false);
								x509Certificate2Collection2 = x509Certificate2Collection2.Find(X509FindType.FindBySerialNumber, x509IssuerSerial2.SerialNumber, false);
							}
						}
					}
					catch (CryptographicException)
					{
					}
					catch (PlatformNotSupportedException)
					{
					}
					if (x509Certificate2Collection2 != null)
					{
						x509Certificate2Collection.AddRange(x509Certificate2Collection2);
					}
				}
			}
			return x509Certificate2Collection;
		}

		internal static string EncodeHexString(byte[] sArray)
		{
			return Utils.EncodeHexString(sArray, 0U, (uint)sArray.Length);
		}

		internal static string EncodeHexString(byte[] sArray, uint start, uint end)
		{
			string text = null;
			if (sArray != null)
			{
				char[] array = new char[(end - start) * 2U];
				uint num = start;
				uint num2 = 0U;
				while (num < end)
				{
					uint num3 = (uint)((sArray[(int)num] & 240) >> 4);
					array[(int)num2++] = Utils.s_hexValues[(int)num3];
					num3 = (uint)(sArray[(int)num] & 15);
					array[(int)num2++] = Utils.s_hexValues[(int)num3];
					num += 1U;
				}
				text = new string(array);
			}
			return text;
		}

		internal static byte[] DecodeHexString(string s)
		{
			string text = Utils.DiscardWhiteSpaces(s);
			uint num = (uint)(text.Length / 2);
			byte[] array = new byte[num];
			int num2 = 0;
			int num3 = 0;
			while ((long)num3 < (long)((ulong)num))
			{
				array[num3] = (byte)(((int)Utils.HexToByte(text[num2]) << 4) | (int)Utils.HexToByte(text[num2 + 1]));
				num2 += 2;
				num3++;
			}
			return array;
		}

		internal static byte HexToByte(char val)
		{
			if (val <= '9' && val >= '0')
			{
				return (byte)(val - '0');
			}
			if (val >= 'a' && val <= 'f')
			{
				return (byte)(val - 'a' + '\n');
			}
			if (val >= 'A' && val <= 'F')
			{
				return (byte)(val - 'A' + '\n');
			}
			return byte.MaxValue;
		}

		internal static bool IsSelfSigned(X509Chain chain)
		{
			X509ChainElementCollection chainElements = chain.ChainElements;
			if (chainElements.Count != 1)
			{
				return false;
			}
			X509Certificate2 certificate = chainElements[0].Certificate;
			return string.Compare(certificate.SubjectName.Name, certificate.IssuerName.Name, StringComparison.OrdinalIgnoreCase) == 0;
		}

		internal static AsymmetricAlgorithm GetAnyPublicKey(X509Certificate2 certificate)
		{
			return certificate.GetRSAPublicKey();
		}

		internal const int MaxCharactersInDocument = 0;

		internal const long MaxCharactersFromEntities = 10000000L;

		internal const int XmlDsigSearchDepth = 20;

		private static readonly char[] s_hexValues = new char[]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F'
		};
	}
}
