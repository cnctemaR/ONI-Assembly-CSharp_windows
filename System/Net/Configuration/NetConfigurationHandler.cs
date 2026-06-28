using System;
using System.Configuration;
using System.Xml;

namespace System.Net.Configuration
{
	internal class NetConfigurationHandler : global::System.Configuration.IConfigurationSectionHandler
	{
		public virtual object Create(object parent, object configContext, XmlNode section)
		{
			NetConfig netConfig = new NetConfig();
			if (section.Attributes != null && section.Attributes.Count != 0)
			{
				HandlersUtil.ThrowException("Unrecognized attribute", section);
			}
			XmlNodeList childNodes = section.ChildNodes;
			foreach (object obj in childNodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				XmlNodeType nodeType = xmlNode.NodeType;
				if (nodeType != XmlNodeType.Whitespace && nodeType != XmlNodeType.Comment)
				{
					if (nodeType != XmlNodeType.Element)
					{
						HandlersUtil.ThrowException("Only elements allowed", xmlNode);
					}
					string name = xmlNode.Name;
					if (name == "ipv6")
					{
						string text = HandlersUtil.ExtractAttributeValue("enabled", xmlNode, false);
						if (xmlNode.Attributes != null && xmlNode.Attributes.Count != 0)
						{
							HandlersUtil.ThrowException("Unrecognized attribute", xmlNode);
						}
						if (text == "true")
						{
							netConfig.ipv6Enabled = true;
						}
						else if (text != "false")
						{
							HandlersUtil.ThrowException("Invalid boolean value", xmlNode);
						}
					}
					else if (name == "httpWebRequest")
					{
						string text2 = HandlersUtil.ExtractAttributeValue("maximumResponseHeadersLength", xmlNode, true);
						HandlersUtil.ExtractAttributeValue("useUnsafeHeaderParsing", xmlNode, true);
						if (xmlNode.Attributes != null && xmlNode.Attributes.Count != 0)
						{
							HandlersUtil.ThrowException("Unrecognized attribute", xmlNode);
						}
						try
						{
							if (text2 != null)
							{
								int num = int.Parse(text2.Trim());
								if (num < -1)
								{
									HandlersUtil.ThrowException("Must be -1 or >= 0", xmlNode);
								}
								netConfig.MaxResponseHeadersLength = num;
							}
						}
						catch
						{
							HandlersUtil.ThrowException("Invalid int value", xmlNode);
						}
					}
					else
					{
						HandlersUtil.ThrowException("Unexpected element", xmlNode);
					}
				}
			}
			return netConfig;
		}
	}
}
