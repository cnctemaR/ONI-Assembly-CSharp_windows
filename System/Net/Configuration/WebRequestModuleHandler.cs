using System;
using System.Configuration;
using System.Xml;

namespace System.Net.Configuration
{
	internal class WebRequestModuleHandler : global::System.Configuration.IConfigurationSectionHandler
	{
		public virtual object Create(object parent, object configContext, XmlNode section)
		{
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
					if (name == "clear")
					{
						if (xmlNode.Attributes != null && xmlNode.Attributes.Count != 0)
						{
							HandlersUtil.ThrowException("Unrecognized attribute", xmlNode);
						}
						WebRequest.ClearPrefixes();
					}
					else
					{
						string text = HandlersUtil.ExtractAttributeValue("prefix", xmlNode);
						if (name == "add")
						{
							string text2 = HandlersUtil.ExtractAttributeValue("type", xmlNode, false);
							if (xmlNode.Attributes != null && xmlNode.Attributes.Count != 0)
							{
								HandlersUtil.ThrowException("Unrecognized attribute", xmlNode);
							}
							WebRequest.AddPrefix(text, text2);
						}
						else if (name == "remove")
						{
							if (xmlNode.Attributes != null && xmlNode.Attributes.Count != 0)
							{
								HandlersUtil.ThrowException("Unrecognized attribute", xmlNode);
							}
							WebRequest.RemovePrefix(text);
						}
						else
						{
							HandlersUtil.ThrowException("Unexpected element", xmlNode);
						}
					}
				}
			}
			return null;
		}
	}
}
