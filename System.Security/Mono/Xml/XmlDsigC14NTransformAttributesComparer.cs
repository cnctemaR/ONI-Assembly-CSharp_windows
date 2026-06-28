using System;
using System.Collections;
using System.Xml;

namespace Mono.Xml
{
	internal class XmlDsigC14NTransformAttributesComparer : IComparer
	{
		public int Compare(object x, object y)
		{
			XmlNode xmlNode = x as XmlNode;
			XmlNode xmlNode2 = y as XmlNode;
			if (xmlNode == xmlNode2)
			{
				return 0;
			}
			if (xmlNode == null)
			{
				return -1;
			}
			if (xmlNode2 == null)
			{
				return 1;
			}
			if (xmlNode.Prefix == xmlNode2.Prefix)
			{
				return string.Compare(xmlNode.LocalName, xmlNode2.LocalName);
			}
			if (xmlNode.Prefix == string.Empty)
			{
				return -1;
			}
			if (xmlNode2.Prefix == string.Empty)
			{
				return 1;
			}
			int num = string.Compare(xmlNode.NamespaceURI, xmlNode2.NamespaceURI);
			if (num == 0)
			{
				num = string.Compare(xmlNode.LocalName, xmlNode2.LocalName);
			}
			return num;
		}
	}
}
