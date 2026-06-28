using System;
using System.Configuration;
using System.Xml;

namespace System.Net.Configuration
{
	internal class HandlersUtil
	{
		private HandlersUtil()
		{
		}

		internal static string ExtractAttributeValue(string attKey, XmlNode node)
		{
			return HandlersUtil.ExtractAttributeValue(attKey, node, false);
		}

		internal static string ExtractAttributeValue(string attKey, XmlNode node, bool optional)
		{
			if (node.Attributes == null)
			{
				if (optional)
				{
					return null;
				}
				HandlersUtil.ThrowException("Required attribute not found: " + attKey, node);
			}
			XmlNode xmlNode = node.Attributes.RemoveNamedItem(attKey);
			if (xmlNode == null)
			{
				if (optional)
				{
					return null;
				}
				HandlersUtil.ThrowException("Required attribute not found: " + attKey, node);
			}
			string value = xmlNode.Value;
			if (value == string.Empty)
			{
				string text = ((!optional) ? "Required" : "Optional");
				HandlersUtil.ThrowException(text + " attribute is empty: " + attKey, node);
			}
			return value;
		}

		internal static void ThrowException(string msg, XmlNode node)
		{
			if (node != null && node.Name != string.Empty)
			{
				msg = msg + " (node name: " + node.Name + ") ";
			}
			throw new global::System.Configuration.ConfigurationException(msg, node);
		}
	}
}
