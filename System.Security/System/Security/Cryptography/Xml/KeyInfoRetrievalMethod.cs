using System;
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
			this._uri = strUri;
		}

		public KeyInfoRetrievalMethod(string strUri, string typeName)
		{
			this._uri = strUri;
			this._type = typeName;
		}

		public string Uri
		{
			get
			{
				return this._uri;
			}
			set
			{
				this._uri = value;
			}
		}

		public string Type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		public override XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal override XmlElement GetXml(XmlDocument xmlDocument)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("RetrievalMethod", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this._uri))
			{
				xmlElement.SetAttribute("URI", this._uri);
			}
			if (!string.IsNullOrEmpty(this._type))
			{
				xmlElement.SetAttribute("Type", this._type);
			}
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this._uri = Utils.GetAttribute(value, "URI", "http://www.w3.org/2000/09/xmldsig#");
			this._type = Utils.GetAttribute(value, "Type", "http://www.w3.org/2000/09/xmldsig#");
		}

		private string _uri;

		private string _type;
	}
}
