using System;
using System.Runtime.InteropServices;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfoRetrievalMethod : KeyInfoClause
	{
		public KeyInfoRetrievalMethod()
		{
		}

		public KeyInfoRetrievalMethod(string strUri)
		{
			this.URI = strUri;
		}

		public KeyInfoRetrievalMethod(string strUri, string strType)
			: this(strUri)
		{
			this.Type = strType;
		}

		[ComVisible(false)]
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
				return this.URI;
			}
			set
			{
				this.element = null;
				this.URI = value;
			}
		}

		public override XmlElement GetXml()
		{
			if (this.element != null)
			{
				return this.element;
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("RetrievalMethod", "http://www.w3.org/2000/09/xmldsig#");
			if (this.URI != null && this.URI.Length > 0)
			{
				xmlElement.SetAttribute("URI", this.URI);
			}
			if (this.Type != null)
			{
				xmlElement.SetAttribute("Type", this.Type);
			}
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (value.LocalName != "RetrievalMethod" || value.NamespaceURI != "http://www.w3.org/2000/09/xmldsig#")
			{
				this.URI = string.Empty;
			}
			else
			{
				this.URI = value.Attributes["URI"].Value;
				if (value.HasAttribute("Type"))
				{
					this.Type = value.Attributes["Type"].Value;
				}
				this.element = value;
			}
		}

		private string URI;

		private XmlElement element;

		private string type;
	}
}
