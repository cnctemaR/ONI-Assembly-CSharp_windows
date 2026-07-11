using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class SignedInfo : ICollection, IEnumerable
	{
		public SignedInfo()
		{
			this.references = new ArrayList();
			this.c14nMethod = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315";
		}

		public string CanonicalizationMethod
		{
			get
			{
				return this.c14nMethod;
			}
			set
			{
				this.c14nMethod = value;
				this.element = null;
			}
		}

		[ComVisible(false)]
		[MonoTODO]
		public Transform CanonicalizationMethodObject
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Count
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.element = null;
				this.id = value;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public bool IsSynchronized
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public ArrayList References
		{
			get
			{
				return this.references;
			}
		}

		public string SignatureLength
		{
			get
			{
				return this.signatureLength;
			}
			set
			{
				this.element = null;
				this.signatureLength = value;
			}
		}

		public string SignatureMethod
		{
			get
			{
				return this.signatureMethod;
			}
			set
			{
				this.element = null;
				this.signatureMethod = value;
			}
		}

		public object SyncRoot
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public void AddReference(Reference reference)
		{
			this.references.Add(reference);
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public IEnumerator GetEnumerator()
		{
			return this.references.GetEnumerator();
		}

		public XmlElement GetXml()
		{
			if (this.element != null)
			{
				return this.element;
			}
			if (this.signatureMethod == null)
			{
				throw new CryptographicException("SignatureMethod");
			}
			if (this.references.Count == 0)
			{
				throw new CryptographicException("References empty");
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("SignedInfo", "http://www.w3.org/2000/09/xmldsig#");
			if (this.id != null)
			{
				xmlElement.SetAttribute("Id", this.id);
			}
			if (this.c14nMethod != null)
			{
				XmlElement xmlElement2 = xmlDocument.CreateElement("CanonicalizationMethod", "http://www.w3.org/2000/09/xmldsig#");
				xmlElement2.SetAttribute("Algorithm", this.c14nMethod);
				xmlElement.AppendChild(xmlElement2);
			}
			if (this.signatureMethod != null)
			{
				XmlElement xmlElement3 = xmlDocument.CreateElement("SignatureMethod", "http://www.w3.org/2000/09/xmldsig#");
				xmlElement3.SetAttribute("Algorithm", this.signatureMethod);
				if (this.signatureLength != null)
				{
					XmlElement xmlElement4 = xmlDocument.CreateElement("HMACOutputLength", "http://www.w3.org/2000/09/xmldsig#");
					xmlElement4.InnerText = this.signatureLength;
					xmlElement3.AppendChild(xmlElement4);
				}
				xmlElement.AppendChild(xmlElement3);
			}
			if (this.references.Count == 0)
			{
				throw new CryptographicException("At least one Reference element is required in SignedInfo.");
			}
			foreach (object obj in this.references)
			{
				XmlNode xml = ((Reference)obj).GetXml();
				XmlNode xmlNode = xmlDocument.ImportNode(xml, true);
				xmlElement.AppendChild(xmlNode);
			}
			return xmlElement;
		}

		private string GetAttribute(XmlElement xel, string attribute)
		{
			XmlAttribute xmlAttribute = xel.Attributes[attribute];
			if (xmlAttribute == null)
			{
				return null;
			}
			return xmlAttribute.InnerText;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "SignedInfo" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
			{
				throw new CryptographicException();
			}
			this.id = this.GetAttribute(value, "Id");
			this.c14nMethod = XmlSignature.GetAttributeFromElement(value, "Algorithm", "CanonicalizationMethod");
			XmlElement childElement = XmlSignature.GetChildElement(value, "SignatureMethod", "http://www.w3.org/2000/09/xmldsig#");
			if (childElement != null)
			{
				this.signatureMethod = childElement.GetAttribute("Algorithm");
				XmlElement childElement2 = XmlSignature.GetChildElement(childElement, "HMACOutputLength", "http://www.w3.org/2000/09/xmldsig#");
				if (childElement2 != null)
				{
					this.signatureLength = childElement2.InnerText;
				}
			}
			for (int i = 0; i < value.ChildNodes.Count; i++)
			{
				XmlNode xmlNode = value.ChildNodes[i];
				if (xmlNode.NodeType == XmlNodeType.Element && xmlNode.LocalName == "Reference" && xmlNode.NamespaceURI == "http://www.w3.org/2000/09/xmldsig#")
				{
					Reference reference = new Reference();
					reference.LoadXml((XmlElement)xmlNode);
					this.AddReference(reference);
				}
			}
			this.element = value;
		}

		private ArrayList references;

		private string c14nMethod;

		private string id;

		private string signatureMethod;

		private string signatureLength;

		private XmlElement element;
	}
}
