using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class Manifest
	{
		public Manifest()
		{
			this.references = new ArrayList();
		}

		public Manifest(XmlElement xel)
			: this()
		{
			this.LoadXml(xel);
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

		public ArrayList References
		{
			get
			{
				return this.references;
			}
		}

		public void AddReference(Reference reference)
		{
			this.references.Add(reference);
		}

		public XmlElement GetXml()
		{
			if (this.element != null)
			{
				return this.element;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("SignedInfo", "http://www.w3.org/2000/09/xmldsig#");
			if (this.id != null)
			{
				xmlElement.SetAttribute("Id", this.id);
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
			if (value.LocalName != "Manifest" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
			{
				throw new CryptographicException();
			}
			this.id = this.GetAttribute(value, "Id");
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

		private string id;

		private XmlElement element;
	}
}
