using System;
using System.Collections.Generic;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class DataObject
	{
		public DataObject()
		{
			this.Build(null, null, null, null);
		}

		public DataObject(string id, string mimeType, string encoding, XmlElement data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this.Build(id, mimeType, encoding, data);
		}

		private void Build(string id, string mimeType, string encoding, XmlElement data)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("Object", "http://www.w3.org/2000/09/xmldsig#");
			if (id != null)
			{
				xmlElement.SetAttribute("Id", id);
			}
			if (mimeType != null)
			{
				xmlElement.SetAttribute("MimeType", mimeType);
			}
			if (encoding != null)
			{
				xmlElement.SetAttribute("Encoding", encoding);
			}
			if (data != null)
			{
				XmlNode xmlNode = xmlDocument.ImportNode(data, true);
				xmlElement.AppendChild(xmlNode);
			}
			this.element = xmlElement;
		}

		public XmlNodeList Data
		{
			get
			{
				return this.element.ChildNodes;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				XmlDocument xmlDocument = new XmlDocument();
				XmlElement xmlElement = (XmlElement)xmlDocument.ImportNode(this.element, true);
				while (xmlElement.LastChild != null)
				{
					xmlElement.RemoveChild(xmlElement.LastChild);
				}
				foreach (object obj in value)
				{
					XmlNode xmlNode = (XmlNode)obj;
					xmlElement.AppendChild(xmlDocument.ImportNode(xmlNode, true));
				}
				this.element = xmlElement;
				this.propertyModified = true;
			}
		}

		public string Encoding
		{
			get
			{
				return this.GetField("Encoding");
			}
			set
			{
				this.SetField("Encoding", value);
			}
		}

		public string Id
		{
			get
			{
				return this.GetField("Id");
			}
			set
			{
				this.SetField("Id", value);
			}
		}

		public string MimeType
		{
			get
			{
				return this.GetField("MimeType");
			}
			set
			{
				this.SetField("MimeType", value);
			}
		}

		private string GetField(string attribute)
		{
			XmlNode xmlNode = this.element.Attributes[attribute];
			return (xmlNode == null) ? null : xmlNode.Value;
		}

		private void SetField(string attribute, string value)
		{
			if (value == null)
			{
				return;
			}
			if (this.propertyModified)
			{
				this.element.SetAttribute(attribute, value);
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlElement xmlElement = xmlDocument.ImportNode(this.element, true) as XmlElement;
				xmlElement.SetAttribute(attribute, value);
				this.element = xmlElement;
				this.propertyModified = true;
			}
		}

		public XmlElement GetXml()
		{
			if (this.propertyModified)
			{
				XmlElement xmlElement = this.element;
				XmlDocument xmlDocument = new XmlDocument();
				this.element = xmlDocument.CreateElement("Object", "http://www.w3.org/2000/09/xmldsig#");
				foreach (object obj in xmlElement.Attributes)
				{
					XmlAttribute xmlAttribute = (XmlAttribute)obj;
					string name = xmlAttribute.Name;
					if (name != null)
					{
						if (DataObject.<>f__switch$map4 == null)
						{
							DataObject.<>f__switch$map4 = new Dictionary<string, int>(3)
							{
								{ "Id", 0 },
								{ "Encoding", 0 },
								{ "MimeType", 0 }
							};
						}
						int num;
						if (DataObject.<>f__switch$map4.TryGetValue(name, out num))
						{
							if (num == 0)
							{
								this.element.SetAttribute(xmlAttribute.Name, xmlAttribute.Value);
							}
						}
					}
				}
				foreach (object obj2 in xmlElement.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj2;
					this.element.AppendChild(xmlDocument.ImportNode(xmlNode, true));
				}
			}
			return this.element;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this.element = value;
			this.propertyModified = false;
		}

		private XmlElement element;

		private bool propertyModified;
	}
}
