using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfo : IEnumerable
	{
		public KeyInfo()
		{
			this._keyInfoClauses = new ArrayList();
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
			}
		}

		public XmlElement GetXml()
		{
			return this.GetXml(new XmlDocument
			{
				PreserveWhitespace = true
			});
		}

		internal XmlElement GetXml(XmlDocument xmlDocument)
		{
			XmlElement xmlElement = xmlDocument.CreateElement("KeyInfo", "http://www.w3.org/2000/09/xmldsig#");
			if (!string.IsNullOrEmpty(this._id))
			{
				xmlElement.SetAttribute("Id", this._id);
			}
			for (int i = 0; i < this._keyInfoClauses.Count; i++)
			{
				XmlElement xml = ((KeyInfoClause)this._keyInfoClauses[i]).GetXml(xmlDocument);
				if (xml != null)
				{
					xmlElement.AppendChild(xml);
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
			for (XmlNode xmlNode = value.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				XmlElement xmlElement = xmlNode as XmlElement;
				if (xmlElement != null)
				{
					string text = xmlElement.NamespaceURI + " " + xmlElement.LocalName;
					if (text == "http://www.w3.org/2000/09/xmldsig# KeyValue")
					{
						foreach (object obj in xmlElement.ChildNodes)
						{
							XmlElement xmlElement2 = ((XmlNode)obj) as XmlElement;
							if (xmlElement2 != null)
							{
								text = text + "/" + xmlElement2.LocalName;
								break;
							}
						}
					}
					KeyInfoClause keyInfoClause = (KeyInfoClause)CryptoHelpers.CreateFromName(text);
					if (keyInfoClause == null)
					{
						keyInfoClause = new KeyInfoNode();
					}
					keyInfoClause.LoadXml(xmlElement);
					this.AddClause(keyInfoClause);
				}
			}
		}

		public int Count
		{
			get
			{
				return this._keyInfoClauses.Count;
			}
		}

		public void AddClause(KeyInfoClause clause)
		{
			this._keyInfoClauses.Add(clause);
		}

		public IEnumerator GetEnumerator()
		{
			return this._keyInfoClauses.GetEnumerator();
		}

		public IEnumerator GetEnumerator(Type requestedObjectType)
		{
			ArrayList arrayList = new ArrayList();
			foreach (object obj in this._keyInfoClauses)
			{
				if (requestedObjectType.Equals(obj.GetType()))
				{
					arrayList.Add(obj);
				}
			}
			return arrayList.GetEnumerator();
		}

		private string _id;

		private ArrayList _keyInfoClauses;
	}
}
