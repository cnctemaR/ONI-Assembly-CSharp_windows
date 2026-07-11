using System;
using System.Collections.Specialized;
using System.IO;
using System.Xml;

namespace System.Configuration
{
	public class NameValueFileSectionHandler : IConfigurationSectionHandler
	{
		public object Create(object parent, object configContext, XmlNode section)
		{
			XmlNode xmlNode = null;
			if (section.Attributes != null)
			{
				xmlNode = section.Attributes.RemoveNamedItem("file");
			}
			NameValueCollection nameValueCollection = ConfigHelper.GetNameValueCollection(parent as NameValueCollection, section, "key", "value");
			if (xmlNode != null && xmlNode.Value != string.Empty)
			{
				string text = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(((IConfigXmlNode)section).Filename)), xmlNode.Value);
				if (!File.Exists(text))
				{
					return nameValueCollection;
				}
				ConfigXmlDocument configXmlDocument = new ConfigXmlDocument();
				configXmlDocument.Load(text);
				if (configXmlDocument.DocumentElement.Name != section.Name)
				{
					throw new ConfigurationException("Invalid root element", configXmlDocument.DocumentElement);
				}
				nameValueCollection = ConfigHelper.GetNameValueCollection(nameValueCollection, configXmlDocument.DocumentElement, "key", "value");
			}
			return nameValueCollection;
		}
	}
}
