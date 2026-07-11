using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class DataObject
	{
		public DataObject()
		{
			this._cachedXml = null;
			this._elData = new CanonicalXmlNodeList();
		}

		public DataObject(string id, string mimeType, string encoding, XmlElement data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			this._id = id;
			this._mimeType = mimeType;
			this._encoding = encoding;
			this._elData = new CanonicalXmlNodeList();
			this._elData.Add(data);
			this._cachedXml = null;
		}

		public string Id
		{
			get
			{
				return this._id;
			}
			set
			{
				this._id = value;
				this._cachedXml = null;
			}
		}

		public string MimeType
		{
			get
			{
				return this._mimeType;
			}
			set
			{
				this._mimeType = value;
				this._cachedXml = null;
			}
		}

		public string Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
				this._encoding = value;
				this._cachedXml = null;
			}
		}

		public XmlNodeList Data
		{
			get
			{
				return this._elData;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this._elData = new CanonicalXmlNodeList();
				foreach (object obj in value)
				{
					XmlNode xmlNode = (XmlNode)obj;
					this._elData.Add(xmlNode);
				}
				this._cachedXml = null;
			}
		}

		private bool CacheValid
		{
			get
			{
				return this._cachedXml != null;
			}
		}

		public XmlElement GetXml()
		{
			if (this.CacheValid)
			{
				return this._cachedXml;
			}
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument document)
		{
			XmlElement xmlElement = document.CreateElement("Object", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this._id))
			{
				xmlElement.SetAttribute("Id", this._id);
			}
			if (!string.IsNullOrEmpty(this._mimeType))
			{
				xmlElement.SetAttribute("MimeType", this._mimeType);
			}
			if (!string.IsNullOrEmpty(this._encoding))
			{
				xmlElement.SetAttribute("Encoding", this._encoding);
			}
			if (this._elData != null)
			{
				foreach (object obj in this._elData)
				{
					XmlNode xmlNode = (XmlNode)obj;
					xmlElement.AppendChild(document.ImportNode(xmlNode, true));
				}
			}
			return xmlElement;
		}

		public void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this._id = Utils.GetAttribute(value, "Id", "http://www.w3.org/2000/09/xmldsig#");
			this._mimeType = Utils.GetAttribute(value, "MimeType", "http://www.w3.org/2000/09/xmldsig#");
			this._encoding = Utils.GetAttribute(value, "Encoding", "http://www.w3.org/2000/09/xmldsig#");
			foreach (object obj in value.ChildNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				this._elData.Add(xmlNode);
			}
			this._cachedXml = value;
		}

		private string _id;

		private string _mimeType;

		private string _encoding;

		private CanonicalXmlNodeList _elData;

		private XmlElement _cachedXml;
	}
}
