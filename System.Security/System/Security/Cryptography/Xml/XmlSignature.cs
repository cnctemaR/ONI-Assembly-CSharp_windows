using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class XmlSignature
	{
		public static XmlElement GetChildElement(XmlElement xel, string element, string ns)
		{
			for (int i = 0; i < xel.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = xel.ChildNodes[i];
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.LocalName == element && xmlNode.NamespaceURI == ns)
				{
					return xmlNode as XmlElement;
				}
			}
			return null;
		}

		public static string GetAttributeFromElement(XmlElement xel, string attribute, string element)
		{
			XmlElement childElement = XmlSignature.GetChildElement(xel, element, "http://www.w3.org/2000/09/xmldsig#");
			return (childElement == null) ? null : childElement.GetAttribute(attribute);
		}

		public static XmlElement[] GetChildElements(XmlElement xel, string element)
		{
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < xel.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = xel.ChildNodes[i];
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.LocalName == element && xmlNode.NamespaceURI == "http://www.w3.org/2000/09/xmldsig#")
				{
					arrayList.Add(xmlNode);
				}
			}
			return arrayList.ToArray(typeof(XmlElement)) as XmlElement[];
		}

		public const string NamespaceURI = "http://www.w3.org/2000/09/xmldsig#";

		public const string Prefix = "ds";

		public class ElementNames
		{
			public const string CanonicalizationMethod = "CanonicalizationMethod";

			public const string DigestMethod = "DigestMethod";

			public const string DigestValue = "DigestValue";

			public const string DSAKeyValue = "DSAKeyValue";

			public const string EncryptedKey = "EncryptedKey";

			public const string HMACOutputLength = "HMACOutputLength";

			public const string KeyInfo = "KeyInfo";

			public const string KeyName = "KeyName";

			public const string KeyValue = "KeyValue";

			public const string Manifest = "Manifest";

			public const string Object = "Object";

			public const string Reference = "Reference";

			public const string RetrievalMethod = "RetrievalMethod";

			public const string RSAKeyValue = "RSAKeyValue";

			public const string Signature = "Signature";

			public const string SignatureMethod = "SignatureMethod";

			public const string SignatureValue = "SignatureValue";

			public const string SignedInfo = "SignedInfo";

			public const string Transform = "Transform";

			public const string Transforms = "Transforms";

			public const string X509Data = "X509Data";

			public const string X509IssuerSerial = "X509IssuerSerial";

			public const string X509IssuerName = "X509IssuerName";

			public const string X509SerialNumber = "X509SerialNumber";

			public const string X509SKI = "X509SKI";

			public const string X509SubjectName = "X509SubjectName";

			public const string X509Certificate = "X509Certificate";

			public const string X509CRL = "X509CRL";
		}

		public class AttributeNames
		{
			public const string Algorithm = "Algorithm";

			public const string Encoding = "Encoding";

			public const string Id = "Id";

			public const string MimeType = "MimeType";

			public const string Type = "Type";

			public const string URI = "URI";
		}

		public class AlgorithmNamespaces
		{
			public const string XmlDsigBase64Transform = "http://www.w3.org/2000/09/xmldsig#base64";

			public const string XmlDsigC14NTransform = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

			public const string XmlDsigC14NWithCommentsTransform = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments";

			public const string XmlDsigEnvelopedSignatureTransform = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";

			public const string XmlDsigXPathTransform = "http://www.w3.org/TR/1999/REC-xpath-19991116";

			public const string XmlDsigXsltTransform = "http://www.w3.org/TR/1999/REC-xslt-19991116";

			public const string XmlDsigExcC14NTransform = "http://www.w3.org/2001/10/xml-exc-c14n#";

			public const string XmlDsigExcC14NWithCommentsTransform = "http://www.w3.org/2001/10/xml-exc-c14n#WithComments";

			public const string XmlDecryptionTransform = "http://www.w3.org/2002/07/decrypt#XML";

			public const string XmlLicenseTransform = "urn:mpeg:mpeg21:2003:01-REL-R-NS:licenseTransform";
		}

		public class Uri
		{
			public const string Manifest = "http://www.w3.org/2000/09/xmldsig#Manifest";
		}
	}
}
