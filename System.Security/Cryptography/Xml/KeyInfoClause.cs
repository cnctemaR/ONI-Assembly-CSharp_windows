using System;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	public abstract class KeyInfoClause
	{
		public abstract XmlElement GetXml();

		internal virtual XmlElement GetXml(XmlDocument xmlDocument)
		{
			XmlElement xml = this.GetXml();
			return (XmlElement)xmlDocument.ImportNode(xml, true);
		}

		public abstract void LoadXml(XmlElement element);
	}
}
