using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public class KeyInfoName : KeyInfoClause
	{
		public KeyInfoName()
			: this(null)
		{
		}

		public KeyInfoName(string keyName)
		{
			this.Value = keyName;
		}

		public string Value
		{
			get
			{
				return this._keyName;
			}
			set
			{
				this._keyName = value;
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
			XmlElement xmlElement = xmlDocument.CreateElement("KeyName", "http://www.w3.org/2000/09/xmldsig#");
			xmlElement.AppendChild(xmlDocument.CreateTextNode(this._keyName));
			return xmlElement;
		}

		public override void LoadXml(XmlElement value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			this._keyName = value.InnerText.Trim();
		}

		private string _keyName;
	}
}
