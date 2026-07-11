using System;
using System.Xml;

namespace System.Configuration
{
	internal class ConfigurationXmlDocument : XmlDocument
	{
		public override XmlElement CreateElement(string prefix, string localName, string namespaceURI)
		{
			if (namespaceURI == "http://schemas.microsoft.com/.NetConfiguration/v2.0")
			{
				return base.CreateElement(string.Empty, localName, string.Empty);
			}
			return base.CreateElement(prefix, localName, namespaceURI);
		}
	}
}
