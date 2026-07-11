using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public sealed class EncryptionProperty
	{
		public EncryptionProperty()
		{
		}

		public EncryptionProperty(XmlElement elemProp)
		{
			this.LoadXml(elemProp);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public XmlElement PropertyElement
		{
			get
			{
				return this.elemProp;
			}
			set
			{
				this.LoadXml(value);
			}
		}

		public string Target
		{
			get
			{
				return this.target;
			}
		}

		public XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument());
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("EncryptionProperty", "http://www.w3.org/2001/04/xmlenc#");
			if (this.Id != null)
			{
				xmlElement.SetAttribute("Id", this.Id);
			}
			if (this.Target != null)
			{
				xmlElement.SetAttribute("Target", this.Target);
			}
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value.LocalName != "EncryptionProperty" || value.NamespaceURI != "http://www.w3.org/2001/04/xmlenc#")
			{
				throw new CryptographicException("Malformed EncryptionProperty element.");
			}
			if (value.HasAttribute("Id"))
			{
				this.id = value.Attributes["Id"].Value;
			}
			if (value.HasAttribute("Target"))
			{
				this.target = value.Attributes["Target"].Value;
			}
		}

		private XmlElement elemProp;

		private string id;

		private string target;
	}
}
