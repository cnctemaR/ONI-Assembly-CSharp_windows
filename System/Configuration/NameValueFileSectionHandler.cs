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
			global::System.Collections.Specialized.NameValueCollection nameValueCollection = ConfigHelper.GetNameValueCollection(parent as global::System.Collections.Specialized.NameValueCollection, section, "key", "value");
			if (xmlNode != null && xmlNode.Value != string.Empty)
			{
				string text = ((IConfigXmlNode)section).Filename;
				text = Path.GetFullPath(text);
				string text2 = Path.Combine(Path.GetDirectoryName(text), xmlNode.Value);
				if (!File.Exists(text2))
				{
					return nameValueCollection;
				}
				ConfigXmlDocument configXmlDocument = new ConfigXmlDocument();
				configXmlDocument.Load(text2);
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
