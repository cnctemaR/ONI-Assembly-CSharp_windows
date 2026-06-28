using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class Reference
	{
		public Reference()
		{
			this.chain = new TransformChain();
			this.digestMethod = "http://www.w3.org/2000/09/xmldsig#sha1";
		}

		[MonoTODO("There is no description about how it is used.")]
		public Reference(Stream stream)
			: this()
		{
			this.stream = stream;
		}

		public Reference(string uri)
			: this()
		{
			this.uri = uri;
		}

		public string DigestMethod
		{
			get
			{
				return this.digestMethod;
			}
			set
			{
				this.element = null;
				this.digestMethod = value;
			}
		}

		public byte[] DigestValue
		{
			get
			{
				return this.digestValue;
			}
			set
			{
				this.element = null;
				this.digestValue = value;
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

		public TransformChain TransformChain
		{
			get
			{
				return this.chain;
			}
			[ComVisible(false)]
			set
			{
				this.chain = value;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.element = null;
				this.type = value;
			}
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.element = null;
				this.uri = value;
			}
		}

		public void AddTransform(Transform transform)
		{
			this.chain.Add(transform);
		}

		public XmlElement GetXml()
		{
			if (this.element != null)
			{
				return this.element;
			}
			if (this.digestMethod == null)
			{
				throw new CryptographicException("DigestMethod");
			}
			if (this.digestValue == null)
			{
				throw new NullReferenceException("DigestValue");
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("Reference", "http://www.w3.org/2000/09/xmldsig#");
			if (this.id != null)
			{
				xmlElement.SetAttribute("Id", this.id);
			}
			if (this.uri != null)
			{
				xmlElement.SetAttribute("URI", this.uri);
			}
			if (this.type != null)
			{
				xmlElement.SetAttribute("Type", this.type);
			}
			if (this.chain.Count > 0)
			{
				XmlElement xmlElement2 = xmlDocument.CreateElement("Transforms", "http://www.w3.org/2000/09/xmldsig#");
				foreach (object obj in this.chain)
				{
					Transform transform = (Transform)obj;
					XmlNode xml = transform.GetXml();
					XmlNode xmlNode = xmlDocument.ImportNode(xml, true);
					xmlElement2.AppendChild(xmlNode);
				}
				xmlElement.AppendChild(xmlElement2);
			}
			XmlElement xmlElement3 = xmlDocument.CreateElement("DigestMethod", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement3.SetAttribute("Algorithm", this.digestMethod);
			xmlElement.AppendChild(xmlElement3);
			XmlElement xmlElement4 = xmlDocument.CreateElement("DigestValue", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement4.InnerText = Convert.ToBase64String(this.digestValue);
			xmlElement.AppendChild(xmlElement4);
			return xmlElement;
		}

		private string GetAttribute(XmlElement xel, string attribute)
		{
			XmlAttribute xmlAttribute = xel.Attributes[attribute];
			return (xmlAttribute == null) ? null : xmlAttribute.InnerText;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "Reference" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
			{
				throw new CryptographicException();
			}
			this.id = this.GetAttribute(value, "Id");
			this.uri = this.GetAttribute(value, "URI");
			this.type = this.GetAttribute(value, "Type");
			XmlNodeList elementsByTagName = value.GetElementsByTagName("Transform", "http://www.w3.org/2000/09/xmldsig#");
			if (elementsByTagName != null && elementsByTagName.Count > 0)
			{
				foreach (object obj in elementsByTagName)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string attribute = this.GetAttribute((XmlElement)xmlNode, "Algorithm");
					Transform transform = (Transform)CryptoConfig.CreateFromName(attribute);
					if (transform == null)
					{
						throw new CryptographicException("Unknown transform {0}.", attribute);
					}
					if (xmlNode.ChildNodes.Count > 0)
					{
						transform.LoadInnerXml(xmlNode.ChildNodes);
					}
					this.AddTransform(transform);
				}
			}
			this.DigestMethod = XmlSignature.GetAttributeFromElement(value, "Algorithm", "DigestMethod");
			XmlElement childElement = XmlSignature.GetChildElement(value, "DigestValue", "http://www.w3.org/2000/09/xmldsig#");
			if (childElement != null)
			{
				this.DigestValue = Convert.FromBase64String(childElement.InnerText);
			}
			this.element = value;
		}

		private TransformChain chain;

		private string digestMethod;

		private byte[] digestValue;

		private string id;

		private string uri;

		private string type;

		private Stream stream;

		private XmlElement element;
	}
}
